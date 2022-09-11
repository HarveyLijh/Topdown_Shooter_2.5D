using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;

namespace RSM
{
    [CustomEditor(typeof(ConditionTrigger))]
    public class ConditionTriggerEditor : Editor
    {
        private ConditionTrigger conditionTrigger;
        private VisualElement root;

        public void OnEnable()
        {
            conditionTrigger = (ConditionTrigger)target;
            root = new VisualElement();
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (conditionTrigger.behaviour == null) conditionTrigger.behaviour = (MonoBehaviour)conditionTrigger.transform.parent.GetComponent<IStateBehaviour>();
            if (conditionTrigger.behaviour == null) conditionTrigger.behaviour = (MonoBehaviour)conditionTrigger.GetComponent<State>().stateMachine.behaviour;
            ConditionSelector();
            return root;
        }

        private PopupField<string> transitionDropdown;

        private void ConditionSelector()
        {
            List<string> conditionNames = new List<string>() { "Never" };

            List<MethodInfo> conditions = conditionTrigger.behaviour.GetType().GetMethods().Where(m => m.ReturnType == typeof(TransitionCondition)).ToList();
            conditions.ForEach(c => conditionNames.Add(c.Name));
            string selectedTransition = conditionNames[0];
            if (!string.IsNullOrEmpty(conditionTrigger.conditionName))
            {
                if (conditionNames.Contains(conditionTrigger.conditionName)) selectedTransition = conditionTrigger.conditionName;
            }
            transitionDropdown = new PopupField<string>(conditionNames, selectedTransition);
            transitionDropdown.style.fontSize = 18;

            transitionDropdown.formatSelectedValueCallback += (string input) =>
            {
                conditionTrigger.conditionName = input;
                return input;
            };
            root.Add(transitionDropdown);
        }

    }
}