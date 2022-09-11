using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace RSM
{
    public class BehaviourTrackerView
    {
        public static implicit operator VisualElement(BehaviourTrackerView behaviourTracker)
            => behaviourTracker.root;

        private VisualElement root;
        public BehaviourTrackerView(GenericState state)
        {
            root = new VisualElement();
            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RapidStateMachine/Editor/BehaviourTracker/BehaviourTracker.uxml");
            tree.CloneTree(root);
            SetButtons(state);
        }

        private GenericState displayState = null;
        private Button enter;
        private Button tick;
        private Button exit;
        private Color activeColour = new Color(1, 1, 0.8f, 1);
        private Color inactiveColour = new Color(0.3f, 0.3f, 0.3f, 1);
        public void SetButtons(GenericState state)
        {
            if (displayState != null)
            {
                if (state.HasEnterMethod()) enter.clicked -= displayState.OpenEnter;
                if (state.HasTickMethod()) tick.clicked -= displayState.OpenTick;
                if (state.HasExitMethod()) exit.clicked -= displayState.OpenExit;
                displayState = state;
            }
            enter = root.Q<Button>("Enter");
            tick = root.Q<Button>("Tick");
            exit = root.Q<Button>("Exit");

            if (state == null)
            {
                enter.style.backgroundColor = inactiveColour;
                tick.style.backgroundColor = inactiveColour;
                exit.style.backgroundColor = inactiveColour;
                displayState = null;
                return;
            }

            enter.style.backgroundColor = state.HasEnterMethod() ? activeColour : inactiveColour;
            if (state.HasEnterMethod()) enter.clicked += state.OpenEnter;
            tick.style.backgroundColor = state.HasTickMethod() ? activeColour : inactiveColour;
            if (state.HasTickMethod()) tick.clicked += state.OpenTick;
            exit.style.backgroundColor = state.HasExitMethod() ? activeColour : inactiveColour;
            if (state.HasExitMethod()) exit.clicked += state.OpenExit;
        }
    }
}