using UnityEngine.UIElements;
using UnityEditor;

namespace RSM
{
    public class TransitionRecordView
    {
        public static implicit operator VisualElement(TransitionRecordView transitionRecord)
        => transitionRecord.root;

        private TransitionRecord transitionRecord;
        private VisualElement root;
        public TransitionRecordView(TransitionRecord transitionRecord)
        {
            root = new VisualElement();
            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RapidStateMachine/Editor/StateMachineDebugger/TransitionRecordInspector.uxml");
            tree.CloneTree(root);
            this.transitionRecord = transitionRecord;

            SetFrom();
            SetTo();
            SetTime();
        }

        private Label from;
        private void SetFrom()
        {
            from = root.Q<Label>("From");
            from.text = transitionRecord.from;
        }

        private Label to;
        private void SetTo()
        {
            to = root.Q<Label>("To");
            to.text = transitionRecord.to;
        }

        private Label time;
        private void SetTime()
        {
            time = root.Q<Label>("Time");
            time.text = transitionRecord.duration.ToString();
        }
    }
}