using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace RSM
{
    public class TransitionView
    {
        public static implicit operator VisualElement(TransitionView transitionView)
            => transitionView.root;

        private StateTransition transition;
        private VisualElement root;
        private StateMachine stateMachine;
        private Action refresh;
        private List<ConditionView> conditionViews;


        private SerializedObject so;
        public TransitionView(StateTransition transition, Action refresh, SerializedObject so)
        {
            root = new VisualElement();
            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RapidStateMachine/Editor/GenericState/TransitionInspector.uxml");
            tree.CloneTree(root);
            this.transition = transition;
            this.refresh = refresh;
            this.stateMachine = transition.stateMachine;

            if (transition.from != null) this.so = new SerializedObject(transition.from);
            else if (transition.to != null) this.so = new SerializedObject(transition.to);

            RightClickMenu();
            CreateBehaviourTracker();
            SetMuteButton();
            SetRemoveButton();

            CreateStateView(transition);

            DrawConditions();
            AddConditionButton();
        }

        private void DrawConditions()
        {
            if (conditionViews == null) conditionViews = new List<ConditionView>();
            VisualElement conditions = root.Q<VisualElement>("Conditions");
            foreach (StateCondition condition in transition.conditions)
            {
                ConditionView newCondition = new ConditionView(condition, refresh, so, RemoveCondition);
                conditionViews.Add(newCondition);
                conditions.Add(newCondition);
            }
            foreach (ConditionView condition in conditionViews)
            {
                condition.transitionDropdown.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Delete Condition", (x) =>
                    {
                        EditorUtility.SetDirty(stateMachine.gameObject);
                        so.GetProperty(GetTransitionPath("conditions")).DeleteArrayElementAtIndex(conditionViews.IndexOf(condition));
                        so.ApplyModifiedProperties();
                        transition.ResetDelays();
                        conditionViews.Remove(condition);

                        so.Update();
                        so.ApplyModifiedProperties();
                        Refresh();
                    });
                    evt.menu.AppendAction("Open Condition", (x) =>
                    {
                        condition.condition.OpenCondition();
                    });
                }));
            }
        }

        private void AddConditionButton()
        {
            VisualElement conditions = root.Q<VisualElement>("Conditions");

            var addConditionButton = new Button();
            addConditionButton.clicked += AddCondition;
            addConditionButton.text = "Add Condition";
            conditions.Add(addConditionButton);
        }

        private void RightClickMenu()
        {
            so.Update();
            root.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Move to top", (x) =>
                {
                    EditorUtility.SetDirty(stateMachine.gameObject);
                    so.FindProperty("stateTransitions").MoveArrayElement(transition.from.stateTransitions.IndexOf(transition), 0);
                    so.ApplyModifiedProperties();
                    Refresh();
                });
                evt.menu.AppendAction("Move up", (x) =>
                {
                    EditorUtility.SetDirty(stateMachine.gameObject);
                    int index = transition.from.stateTransitions.IndexOf(transition);
                    so.FindProperty("stateTransitions").MoveArrayElement(index, index - 1);
                    so.ApplyModifiedProperties();
                    Refresh();
                });
                evt.menu.AppendAction("Move down", (x) =>
                {
                    EditorUtility.SetDirty(stateMachine.gameObject);
                    int index = transition.from.stateTransitions.IndexOf(transition);
                    so.FindProperty("stateTransitions").MoveArrayElement(index, index + 1);
                    so.ApplyModifiedProperties();
                    Refresh();
                });
                evt.menu.AppendAction("Move to bottom", (x) =>
                {
                    EditorUtility.SetDirty(stateMachine.gameObject);
                    so.FindProperty("stateTransitions").MoveArrayElement(transition.from.stateTransitions.IndexOf(transition), transition.from.stateTransitions.Count - 1);
                    so.ApplyModifiedProperties();
                    Refresh();
                });
            }));
        }

        private void RemoveCondition(ConditionView condition)
        {
            EditorUtility.SetDirty(stateMachine.gameObject);
            so.GetProperty(GetTransitionPath("conditions")).DeleteArrayElementAtIndex(conditionViews.IndexOf(condition));
            so.ApplyModifiedProperties();
            transition.ResetDelays();

            conditionViews.Remove(condition);

            so.Update();
            so.ApplyModifiedProperties();
            Refresh();
        }

        private void CreateStateView(StateTransition transition)
        {
            VisualElement header = root.Q<VisualElement>("Header");
            StateView stateview;
            State currentState = (State)so.GetProperty(GetTransitionPath("to")).objectReferenceValue;

            if (transition.to != null) stateview = new StateView(stateMachine, currentState);
            else stateview = new StateView(stateMachine);

            stateview = new StateView(stateMachine);
            stateview.stateDropdown.formatSelectedValueCallback += (string input) =>
            {
                if (stateview.stateDropdown.text == "Missing" && input == "Missing") return input;
                if ((transition.to == null && input != "Missing") || transition.to.name != input)
                {
                    EditorUtility.SetDirty(stateMachine.gameObject);
                    so.Update();
                    so.SetProperty(GetTransitionPath("to"), stateMachine.states.Where(s => s.gameObject.name == input).SingleOrDefault());
                    so.Update();
                    so.ApplyModifiedProperties();
                    UpdateBehaviourTracker((GenericState)so.GetProperty(GetTransitionPath("to")).objectReferenceValue);
                    Refresh();
                }
                stateview.stateDropdown.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Open State", (x) =>
                    {
                        if (currentState == null) return;
                        Selection.activeGameObject = currentState.gameObject;
                    });
                }));
                return input;
            };

            if (currentState == null) stateview.DisplayIndex("Missing");
            else stateview.DisplayIndex(currentState.name);

            header.Insert(0, stateview);
        }

        private void SetMuteButton()
        {
            var muteButton = root.Q<Button>("Mute");

            muteButton.style.unityBackgroundImageTintColor = transition.muted ? Color.red : Color.grey;
            muteButton.clicked += () =>
            {
                so.SetProperty(GetTransitionPath("muted"), !transition.muted);
                Refresh();
            };
        }

        private void SetRemoveButton()
        {
            var removeButton = root.Q<Button>("RemoveTransition");
            removeButton.clicked += () =>
            {
                if (transition == null) return;
                if (transition.from == null) return;
                EditorUtility.SetDirty(stateMachine.gameObject);
                so.GetProperty("stateTransitions").DeleteArrayElementAtIndex(transition.from.stateTransitions.IndexOf(transition));
                so.ApplyModifiedProperties();
                Refresh();
            };
        }

        private void AddCondition()
        {
            Refresh();
            EditorUtility.SetDirty(stateMachine.gameObject);
            so.GetProperty(GetTransitionPath("conditions")).arraySize += 1;
            so.ApplyModifiedProperties();
            int lastIndex = so.GetProperty(GetTransitionPath("conditions")).arraySize - 1;
            so.GetProperty(GetTransitionPath("conditions")).GetArrayElementAtIndex(lastIndex).FindPropertyRelative("conditionName").stringValue = "Never";
            so.GetProperty(GetTransitionPath("conditions")).GetArrayElementAtIndex(lastIndex).FindPropertyRelative("invertCondition").boolValue = false;
            so.ApplyModifiedProperties();
            Refresh();
        }

        BehaviourTrackerView behaviourTracker;
        private void CreateBehaviourTracker()
        {
            VisualElement transitionInspector = root.Q<VisualElement>("TransitionRoot");
            behaviourTracker = new BehaviourTrackerView(null);
            if (transition.to == null) transitionInspector.Insert(0, behaviourTracker);
            else if (transition.to.GetType() == typeof(GenericState)) transitionInspector.Insert(0, new BehaviourTrackerView((GenericState)transition.to));
        }

        private void UpdateBehaviourTracker(GenericState state)
            => behaviourTracker.SetButtons(state);

        public string GetTransitionPath(string propertyName)
        {
            if (transition.from == null && transition.to == null) return "";
            if (transition.from != null && transition.from.stateTransitions.Contains(transition)) return GetToTransitionPath(propertyName);
            else if (transition.to != null) return GetAnyTransitionPath(propertyName);
            return "";
        }
        private string GetToTransitionPath(string propertyName)
        {
            int transitionIndex = transition.from.stateTransitions.IndexOf(transition);
            return $"stateTransitions.Array.data[{transitionIndex}].{propertyName}";
        }
        private string GetAnyTransitionPath(string propertyName)
            => $"anyTransition.{propertyName}";
        public void Refresh()
            => refresh?.Invoke();
    }
}