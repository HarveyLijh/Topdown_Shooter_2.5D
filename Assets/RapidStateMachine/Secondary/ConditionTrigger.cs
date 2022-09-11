using UnityEngine;
using System.Reflection;
namespace RSM
{
    [ExecuteInEditMode]
    public class ConditionTrigger : MonoBehaviour
    {
        public string conditionName;
        public MonoBehaviour behaviour;
        public MethodInfo conditionMethod;

        public void Trigger()
        {
            TransitionCondition transitionCondition = (TransitionCondition)conditionMethod?.Invoke(behaviour, null);
            transitionCondition?.Trigger();
        }

        private void OnEnable()
        {
            behaviour = GetStateBehaviour();
            if(behaviour != null && conditionName != null) conditionMethod = behaviour.GetType().
                    GetMethod(conditionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        private MonoBehaviour GetStateBehaviour()
        {
            MonoBehaviour stateBehaviour = (MonoBehaviour)transform.parent.GetComponent<IStateBehaviour>();
            if(stateBehaviour == null)
            {
                if (transform.parent.transform.parent != null) stateBehaviour = (MonoBehaviour)transform.parent.transform.parent.GetComponent<IStateBehaviour>();
            }
            return stateBehaviour;
        }
    }
}