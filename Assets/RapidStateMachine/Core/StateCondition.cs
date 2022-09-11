using UnityEngine;
using System.Reflection;

namespace RSM
{
    [System.Serializable]
    public class StateCondition
    {
        public StateMachine stateMachine;
        [System.NonSerialized] public StateTransition transition;
        public string conditionName;
        public MethodInfo conditionMethod;
        public bool invertCondition = false;

        private bool isDefaultCondition(string conditionName) => conditionName == "Delay" || conditionName == "Delay Between" || conditionName == "Cooldown" || conditionName == "Cooldown Between";

        public bool ConditionIsTrue()
        {
            if (conditionMethod == null)
            {
                if (isDefaultCondition(conditionName)) return true;
                Debug.LogWarning($"{stateMachine.behaviour.ToString()}, {stateMachine.currentState.name} to {transition.to.name}, {conditionName} condition has no method so returns false", stateMachine.gameObject);
                return false;
            }
            return invertCondition ? !(TransitionCondition)conditionMethod?.Invoke(stateMachine.behaviour, null) : (TransitionCondition)conditionMethod?.Invoke(stateMachine.behaviour, null);
        }

        public void SetStateMachine(StateMachine stateMachine, StateTransition transition)
        {
            this.stateMachine = stateMachine;
            this.transition = transition;
            if (conditionName != null) conditionMethod = stateMachine.behaviour.GetType().GetMethod(conditionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public TransitionCondition GetTransitionCondition()
        {
            if (conditionMethod == null) return null;
            return (TransitionCondition)conditionMethod?.Invoke(stateMachine.behaviour, null);
        }

        public bool ConditionIsTrigger()
        {
            if (GetTransitionCondition() == null) return false;
            return GetTransitionCondition().isTrigger;
        }

        public void OpenCondition()
        {
            MonoBehaviour mono = (MonoBehaviour)stateMachine.behaviour;
            VSManager.OpenMethod(mono, conditionMethod);
        }
    }
}