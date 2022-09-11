using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace RSM
{
    [CustomEditor(typeof(GenericState))]
    public class GenericStateEditor : Editor
    {
        private GenericState state;
        private VisualElement root;
        private VisualTreeAsset tree;
        private SerializedObject so;

        public void OnEnable()
        {
            state = (GenericState)target;
            if (state != null) state.stateMachine.ImportStates();
            Undo.undoRedoPerformed += Refresh;
            so = new SerializedObject(target);
            so.Update();

            root = new VisualElement();
            tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RapidStateMachine/Editor/GenericState/StateInspector.uxml");
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed -= Refresh;
        }

        public override VisualElement CreateInspectorGUI()
        {
            state.stateMachine.ImportStates();
            if (state == null) return root;

            root.Clear();
            tree.CloneTree(root);

            StateTitle();
            ToTransitions();
            AnyTransitions();
            if (Selection.activeGameObject.GetComponent<StateMachine>() != null) so?.Dispose();
            return root;
        }

        private void StateTitle()
        {
            SetStateName();
            SetReturnButton();
        }
        private void SetStateName()
        {
            var stateName = root.Q<Label>("StateName");
            stateName.text = state.name;
        }
        private void SetReturnButton()
        {
            var returnButton = root.Q<Button>("ReturnButton");
            returnButton.clicked += Return;
        }

        private void ToTransitions()
        {
            SetStateBehaviourIndicators();
            SetAddTransitionButton();
            CreateTransitionViews();
        }

        private List<StateView> stateViews;
        VisualElement anyTransitions;

        private void SetStateBehaviourIndicators()
        {
            var enterButton = root.Q<Button>("Enter");
            enterButton.style.backgroundColor = state.HasEnterMethod() ? Color.green : Color.grey;
            enterButton.clicked += state.OpenEnter;
            var tickButton = root.Q<Button>("Tick");
            tickButton.style.backgroundColor = state.HasTickMethod() ? Color.green : Color.grey;
            tickButton.clicked += state.OpenTick;
            var exitButton = root.Q<Button>("Exit");
            exitButton.style.backgroundColor = state.HasExitMethod() ? Color.green : Color.grey;
            exitButton.clicked += state.OpenExit;
        }


        private void Return()
            => Selection.activeGameObject = state.transform.parent.gameObject;

        private void CreateTransitionViews()
        {
            VisualElement transitions = root.Q<VisualElement>("Transitions");

            foreach (StateTransition transition in state.stateTransitions)
            {
                transitions.Insert(transitions.childCount - 1, new TransitionView(transition, Refresh, so));
            }
        }
        private void SetAddTransitionButton()
        {
            var addTransitionButton = root.Q<Button>("AddTransition");
            addTransitionButton.clicked += () =>
            {
                so.Update();
                EditorUtility.SetDirty(state.stateMachine.gameObject);
                so.GetProperty("stateTransitions").arraySize += 1;
                so.ApplyModifiedProperties();
                state.stateTransitions[so.GetProperty("stateTransitions").arraySize - 1] = new StateTransition(state, null);
                so.Update();
                so.ApplyModifiedProperties();
                Refresh();
            };
        }

        private void AnyTransitions()
        {
            SetAddAnyTransitionButton();
            DrawAnyConditions();
            DrawExcluding();
            SetAddAnyConditionButton();
            SetAddExcludeButton();
        }
        private void SetAddAnyTransitionButton()
        {
            Button addAny = root.Q<Button>("AddAnyTransition");
            if (state.transitionFromAny)
            {
                addAny.parent.Remove(addAny);
                return;
            }
            addAny.clicked += () =>
            {
                EditorUtility.SetDirty(state.stateMachine.gameObject);
                if (state.anyTransition.conditions == null) state.anyTransition.conditions = new List<StateCondition>();
                if (state.anyTransition.to == null) state.anyTransition.to = state;
                if (state.anyTransition.conditions.Count <= 0) state.anyTransition.conditions.Add(new StateCondition());
                so.Update();
                so.SetProperty("transitionFromAny", true);
                so.ApplyModifiedProperties();
                Refresh();
            };
        }

        private void DrawAnyConditions()
        {
            anyTransitions = root.Q<VisualElement>("AnyTransition");
            if (!state.transitionFromAny) return;

            int count = 0;
            foreach (StateCondition anyCondition in state.anyTransition.conditions)
            {
                anyTransitions.Insert(count, new ConditionView(anyCondition, Refresh, so, RemoveAnyCondition));
                count++;
            }
        }

        private void DrawExcluding()
        {
            if (state.excluding == null) return;

            anyTransitions = root.Q<VisualElement>("AnyTransition");

            stateViews = new List<StateView>();
            List<VisualElement> excludingViews = new List<VisualElement>();
            excludingViews.Clear();

            foreach (State state in state.excluding)
            {
                VisualElement excludingElement = new VisualElement();
                excludingElement.style.flexDirection = FlexDirection.Row;
                StateView stateView = new StateView(this.state.stateMachine, state, RemoveExcluding);
                stateViews.Add(stateView);

                stateView.stateDropdown.formatSelectedValueCallback += (string input) =>
                {
                    so.Update();
                    EditorUtility.SetDirty(this.state.stateMachine.gameObject);
                    so.GetProperty("excluding").GetArrayElementAtIndex(stateViews.IndexOf(stateView)).objectReferenceValue
                    = this.state.stateMachine.states.Where(s => s.gameObject.name == input).SingleOrDefault();

                    stateView.selectedState = this.state.stateMachine.states.Where(s => s.gameObject.name == input).SingleOrDefault();
                    so.ApplyModifiedProperties();
                    return input;
                };

                excludingElement.Add(stateView);
                excludingViews.Add(excludingElement);
            }


            foreach (VisualElement excludingElement in excludingViews)
            {
                anyTransitions.Insert(anyTransitions.childCount - 1, excludingElement);
            }
        }

        private void SetAddAnyConditionButton()
        {
            Button addCondition = root.Q<Button>("AddCondition");
            if (!state.transitionFromAny)
            {
                addCondition.parent.Remove(addCondition);
                return;
            }
            addCondition.clicked += () =>
            {
                so.Update();
                EditorUtility.SetDirty(state.stateMachine.gameObject);
                so.FindProperty("anyTransition.conditions").arraySize += 1;
                so.ApplyModifiedProperties();
                int lastIndex = so.FindProperty("anyTransition.conditions").arraySize - 1;
                so.FindProperty("anyTransition.conditions").GetArrayElementAtIndex(lastIndex).FindPropertyRelative("conditionName").stringValue = "Never";
                so.FindProperty("anyTransition.conditions").GetArrayElementAtIndex(lastIndex).FindPropertyRelative("invertCondition").boolValue = false;
                so.ApplyModifiedProperties();
                Refresh();
            };
        }
        private void RemoveAnyCondition(ConditionView conditionView)
        {
            so.Update();
            EditorUtility.SetDirty(state.stateMachine.gameObject);
            so.GetProperty("anyTransition.conditions").DeleteArrayElementAtIndex(state.anyTransition.conditions.IndexOf(conditionView.condition));
            if (so.GetProperty("anyTransition.conditions").arraySize <= 0)
            {
                so.SetProperty("transitionFromAny", false);
                so.GetProperty("excluding").ClearArray();
            }
            so.ApplyModifiedProperties();
            Refresh();
        }
        private void SetAddExcludeButton()
        {
            Button addExclude = root.Q<Button>("AddExclude");
            if (!state.transitionFromAny)
            {
                addExclude.parent.Remove(addExclude);
                return;
            }
            addExclude.clicked += () =>
            {
                so.Update();
                EditorUtility.SetDirty(state.stateMachine.gameObject);
                so.FindProperty("excluding").arraySize += 1;
                so.ApplyModifiedProperties();
                Refresh();
            };
        }

        private void RemoveExcluding(StateView stateView)
        {
            so.Update();
            EditorUtility.SetDirty(this.state.stateMachine.gameObject);
            so.GetProperty("excluding").DeleteArrayElementAtIndex(state.excluding.IndexOf(stateView.selectedState));
            so.ApplyModifiedProperties();
            Refresh();
        }

        public void Refresh()
            => CreateInspectorGUI();
    }
}