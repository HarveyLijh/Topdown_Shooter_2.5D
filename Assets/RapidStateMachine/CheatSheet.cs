using UnityEngine;
using RSM;
// These includes are required for StateBehaviours

namespace RSM
{
    public class CheatSheet : MonoBehaviour, IStateBehaviour
    {
        //STATE BEHAVIOUR METHODS
        //Replace "State" in "EnterState" to match the name of the State.
        public void EnterState()
        {
            VSManager.Trace();
            //your code here
        }
        //Replace "State" to match the name of the State.
        public void State()
        {
            VSManager.Trace();
            //your code here
        }
        //Replace "State" in "ExitState" to match the name of the State.
        public void ExitState()
        {
            VSManager.Trace();
            //your code here
        }

        //CONDITION METHODS
        //replace YourCondition with the name of your condition
        //replace yourBool with a bool that determines your condition, e.g. a raycast that checks if the character is grounded
        bool yourBoolHere;
        public TransitionCondition YourCondition()
        {
            VSManager.Trace();
            return new TransitionCondition(yourBoolHere);
        }

        //TRIGGER METHODS
        //replace "condition" in "conditionTrigger" to the name of your trigger. This must be done for all instances of conditionTrigger.
        //replace "Trigger" with the name of your trigger

        private TransitionCondition conditionTrigger;
        public TransitionCondition Trigger()
        {
            VSManager.Trace();
            if (conditionTrigger == null) conditionTrigger = new TransitionCondition(false, true);
            return conditionTrigger;
        }

        public void ManipulatingTriggers() //example method, normally you'd trigger Triggers via Update() or state behaviour methods.
        {
            //activates a trigger
            conditionTrigger.Trigger();

            //deactivates a trigger
            conditionTrigger.CancelTrigger();
        }
    }
}