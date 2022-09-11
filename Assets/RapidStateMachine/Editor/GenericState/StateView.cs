using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

namespace RSM
{
    public class StateView
    {
        public static implicit operator VisualElement(StateView stateView)
        => stateView.root;

        public PopupField<string> stateDropdown;
        public VisualElement root;
        private StateMachine stateMachine;
        private State currentState;
        public State selectedState;
        private StateMachineEvents.GenericEvent<StateView> remove;

        public StateView(StateMachine stateMachine, State currentState = null, StateMachineEvents.GenericEvent<StateView> remove = null)
        {
            this.stateMachine = stateMachine;
            this.currentState = currentState;
            this.remove = remove;
            root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;

            CreateStateDropdown();
            CreateRemoveButton();

            root.Insert(0, stateDropdown);
        }

        private void CreateRemoveButton()
        {
            if (this.remove == null) return;
            
            Button removeButton = new Button();
            removeButton.tooltip = "Delete condition";

            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/RapidStateMachine/Editor/Icons/DeleteIcon.png");
            removeButton.style.SetIcon(icon, 25, 6);

            removeButton.clicked += () =>
            {
                remove?.Invoke(this);
            };
            root.Add(removeButton);
        }

        private void CreateStateDropdown()
        {
            List<string> stateNames = new List<string>() { "Missing" };
            stateMachine.states.ForEach(s => stateNames.Add(s.gameObject.name));
            string selectedState = stateNames[0];
            if (currentState != null) selectedState = currentState.gameObject.name;
            stateDropdown = new PopupField<string>(stateNames, selectedState);
            stateDropdown.style.fontSize = 18;
            stateDropdown.style.SetBorderRadius(5);
        }
        public StateView(StateMachine stateMachine)
        {
            root = new VisualElement();
            List<string> stateNames = new List<string>() { "Missing" };
            stateMachine.states.ForEach(s => stateNames.Add(s.gameObject.name));
            string selectedState = stateNames[0];
            stateDropdown = new PopupField<string>(stateNames, selectedState);
            stateDropdown.style.fontSize = 18;
            root.Insert(0, stateDropdown);
        }

        public void DisplayIndex(string name)
            => stateDropdown.value = name;
    }
}