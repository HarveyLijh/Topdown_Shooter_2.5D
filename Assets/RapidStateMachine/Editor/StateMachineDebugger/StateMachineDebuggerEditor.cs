using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace RSM
{
    [CustomEditor(typeof(StateMachineDebugger))]
    public class StateMachineDebuggerEditor : Editor
    {
        private StateMachineDebugger debugger;
        private VisualElement root;
        private VisualTreeAsset tree;

        public void OnEnable()
        {
            debugger = (StateMachineDebugger)target;

            root = new VisualElement();
            tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RapidStateMachine/Editor/StateMachineDebugger/StateMachineDebuggerInspector.uxml");
            debugger.OnChangedState += RefreshEditor;
        }

        private void OnDisable()
        {
            debugger.OnChangedState -= RefreshEditor;
        }

        public override VisualElement CreateInspectorGUI()
        {
            root.Clear();
            tree.CloneTree(root);

            SetTrackButton();
            SetClearButton();
            CreateTransitionRecords();

            return root;
        }

        private Button trackButton;
        private void SetTrackButton()
        {
            trackButton = root.Q<Button>("Track");
            trackButton.style.backgroundColor = debugger.trackTransitions ? Color.green : Color.grey;
            trackButton.clicked += () =>
            {
                debugger.trackTransitions = !debugger.trackTransitions;
                trackButton.style.backgroundColor = debugger.trackTransitions ? Color.green : Color.grey;
            };
        }

        private void SetClearButton()
        {
            Button clearButton = root.Q<Button>("Clear");
            clearButton.clicked += () =>
            {
                debugger.transitionRecords.Clear();
                RefreshEditor();
            };
        }

        private void CreateTransitionRecords()
        {
            VisualElement transitionRecordVE = root.Q<VisualElement>("TransitionRecords");
            List<TransitionRecord> reversedList = new List<TransitionRecord>();
            debugger.transitionRecords.ForEach(record => reversedList.Add(record));
            reversedList.Reverse();
            foreach (TransitionRecord transitionRecord in reversedList)
            {
                transitionRecordVE.Add(CreateTransitionRecord(transitionRecord));
            }
        }

        private VisualElement CreateTransitionRecord(TransitionRecord transitionRecord)
        {
            TransitionRecordView transitionRecordView = new TransitionRecordView(transitionRecord);
            return transitionRecordView;
        }

        private void RefreshEditor()
        {
            CreateInspectorGUI();
        }
    }
}