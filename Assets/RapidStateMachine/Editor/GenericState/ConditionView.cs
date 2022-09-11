using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace RSM
{
    public class ConditionView
    {
        public static implicit operator VisualElement(ConditionView conditionView)
       => conditionView.root;

        public StateCondition condition;
        public VisualElement root;
        public PopupField<string> transitionDropdown;
        public Button removeButton;
        public Action refresh;
        public StateMachineEvents.GenericEvent<ConditionView> remove;

        SerializedObject so;
        public ConditionView(StateCondition condition, Action refresh, SerializedObject so, StateMachineEvents.GenericEvent<ConditionView> remove = null)
        {
            this.root = new VisualElement();
            this.condition = condition;
            this.refresh = refresh;
            this.remove = remove;

            //this.so = so;
            if (condition.transition.from != null) this.so = new SerializedObject(condition.transition.from);
            else if (condition.transition.to != null) this.so = new SerializedObject(condition.transition.to);
            string selectedTransition = CreateConditionDropdown();
            CreateDefaultInputs(selectedTransition);
            CreateInvertButton(condition);
            CreateRemoveButton();

            root.style.flexGrow = 1;
            root.style.paddingRight = 5;
            root.style.flexDirection = FlexDirection.Row;
        }

        private void CreateInvertButton(StateCondition condition)
        {

            Button notLabel = new Button();
            notLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            notLabel.style.fontSize = 16;
            notLabel.style.width = 50;
            notLabel.style.SetBorderColour(Color.clear);
            notLabel.style.color = Color.white;

            notLabel.style.backgroundColor = Color.clear;
            if (condition.invertCondition)
            {
                notLabel.text = "NOT";
                notLabel.tooltip = "click to un-invert condition";
                notLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            }
            else
            {
                notLabel.text = "When";
                notLabel.tooltip = "click to invert condition";
            }

            notLabel.clicked += () =>
            {
                so.Update();
                so.SetProperty(GetConditionPath("invertCondition"), !so.GetProperty(GetConditionPath("invertCondition")).boolValue);
                Refresh();
            };
            root.Insert(0, notLabel);
        }

        private List<string> GetDefaultConditions()
        {
            List<string> defaultConditionNames = new List<string>() { "Never" };

            if (condition.transition != null)
            {
                if ((!condition.transition.HasConditionWithName("Delay") && !condition.transition.HasConditionWithName("Delay Between"))
                    || (condition.conditionName == "Delay" || condition.conditionName == "Delay Between"))
                {
                    defaultConditionNames.Add("Delay");
                    defaultConditionNames.Add("Delay Between");
                }
                if ((!condition.transition.HasConditionWithName("Cooldown") && !condition.transition.HasConditionWithName("Cooldown Between"))
                    || (condition.conditionName == "Cooldown" || condition.conditionName == "Cooldown Between"))
                {
                    defaultConditionNames.Add("Cooldown");
                    defaultConditionNames.Add("Cooldown Between");
                }
            }
            return defaultConditionNames;
        }

        private void CreateRemoveButton()
        {
            removeButton = new Button();
            removeButton.tooltip = "Delete condition";
            removeButton.clicked += () =>
            {
                remove?.Invoke(this);
            };
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/RapidStateMachine/Editor/Icons/DeleteIcon.png");
            removeButton.style.SetIcon(icon, 25, 6);
            removeButton.style.unityBackgroundImageTintColor = Color.grey;

            root.Add(removeButton);
        }

        private string GetTransitionPath(string propertyName)
        {
            if (condition.transition == null) return "";
            if (condition.transition.from == null && condition.transition.to == null) return "";

            if (condition.transition.from != null && condition.transition.from.stateTransitions.Contains(condition.transition)) return GetToTransitionPath(propertyName);
            else if (condition.transition.to != null) return GetAnyTransitionPath(propertyName);
            return "";
        }
        private string GetToTransitionPath(string propertyName)
        {
            int transitionIndex = condition.transition.from.stateTransitions.IndexOf(condition.transition);
            return $"stateTransitions.Array.data[{transitionIndex}].{propertyName}";
        }
        private string GetAnyTransitionPath(string propertyName)
            => $"anyTransition.{propertyName}";

        public string GetConditionPath(string propertyName)
        {
            int conditionIndex = condition.transition.conditions.IndexOf(condition);
            return $"{GetTransitionPath("")}conditions.Array.data[{conditionIndex}].{propertyName}";
        }

        private void CreateDefaultInputs(string selectedTransition)
        {
            FloatField leftField = new FloatField();
            leftField.isDelayed = true;
            FloatField rightField = new FloatField();
            rightField.isDelayed = true;

            leftField.style.minWidth = 35;
            rightField.style.minWidth = 30;
            leftField.style.paddingLeft = 10;
            leftField.style.paddingRight = 5;
            rightField.style.paddingRight = 5;
            rightField.style.paddingLeft = 5;

            switch (selectedTransition)
            {
                case "Delay":

                    leftField.tooltip = "Delay in seconds";
                    leftField.value = so.GetProperty(GetTransitionPath("delay")).floatValue;
                    leftField.RegisterValueChangedCallback<float>(evt =>
                    {
                        so.SetProperty(GetTransitionPath("delay"), evt.newValue);
                        Refresh();
                    }
                    );
                    root.Add(leftField);
                    break;

                case "Delay Between":

                    leftField.tooltip = "Minumum delay in seconds";
                    leftField.value = condition.transition.minDelay;
                    leftField.RegisterValueChangedCallback<float>(evt =>
                    {
                        so.SetProperty(GetTransitionPath("minDelay"), evt.newValue);
                        bool updateMax = evt.newValue > condition.transition.maxDelay;
                        if (updateMax) so.SetProperty(GetTransitionPath("maxDelay"), evt.newValue);
                        Refresh();
                    }
                    );
                    root.Add(leftField);

                    rightField.tooltip = "Maximum delay in seconds";
                    rightField.value = condition.transition.maxDelay;
                    rightField.RegisterValueChangedCallback<float>(evt =>
                    {
                        so.SetProperty(GetTransitionPath("maxDelay"), evt.newValue >= condition.transition.minDelay ? evt.newValue : condition.transition.minDelay);
                        Refresh();
                    }
                    );
                    root.Add(rightField);
                    break;

                case "Cooldown":

                    leftField.tooltip = "Cooldown in seconds";
                    leftField.value = condition.transition.cooldown;
                    leftField.RegisterValueChangedCallback<float>(evt =>
                    {
                        so.SetProperty(GetTransitionPath("cooldown"), evt.newValue);
                        Refresh();
                    }
                    );
                    root.Add(leftField);
                    break;

                case "Cooldown Between":

                    leftField.tooltip = "Minimum cooldown in seconds";
                    leftField.value = condition.transition.minCooldown;
                    leftField.RegisterValueChangedCallback<float>(evt =>
                    {
                        so.SetProperty(GetTransitionPath("minCooldown"), evt.newValue);
                        bool updateMax = evt.newValue > condition.transition.maxCooldown;
                        if (updateMax) so.SetProperty(GetTransitionPath("maxCooldown"), evt.newValue);
                        Refresh();
                    }
                    );
                    root.Add(leftField);

                    rightField.tooltip = "Maximum cooldown in seconds";
                    rightField.value = condition.transition.maxCooldown;
                    rightField.RegisterValueChangedCallback<float>(evt =>
                    {
                        so.SetProperty(GetTransitionPath("maxCooldown"), evt.newValue >= condition.transition.minCooldown ? evt.newValue : condition.transition.minCooldown);
                        Refresh();
                    }
                    );
                    root.Add(rightField);
                    break;
            }
        }

        private string CreateConditionDropdown()
        {
            List<string> conditionNames = GetDefaultConditions();

            GetConditions().ForEach(c => conditionNames.Add(c.Name));
            string selectedTransition = conditionNames[0];
            if (condition.conditionName != null && conditionNames.Contains(condition.conditionName)) selectedTransition = condition.conditionName;

            transitionDropdown = new PopupField<string>(conditionNames, selectedTransition);
            transitionDropdown.style.fontSize = 18;

            transitionDropdown.RegisterValueChangedCallback<string>(evt =>
            {
                so.Update();
                so.SetProperty(GetConditionPath("conditionName"), evt.newValue);
                condition.transition.ResetDelays();
                Refresh();
            });

            transitionDropdown.style.flexGrow = 1;
            root.Add(transitionDropdown);
            return selectedTransition;
        }

        private List<MethodInfo> GetConditions()
        {
            List<MethodInfo> conditions = condition.stateMachine.behaviour.GetType().GetMethods().Where(m => m.ReturnType == typeof(TransitionCondition)).ToList();
            MonoBehaviour behaviourMono = (MonoBehaviour)condition.stateMachine.behaviour;
            return conditions;
        }

        private void Refresh()
            => refresh?.Invoke();
    }
}