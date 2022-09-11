using System.Collections.Generic;
using UnityEngine;

namespace RSM
{
    [System.Serializable]
    public class StateTransition
    {
        public StateMachine stateMachine;
        public State from;
        public State to;
        public List<StateCondition> conditions;
        public bool muted;
        public float timeLastPassed = 0;

        public float delay = 0;
        public float minDelay = 0;
        public float maxDelay = 0;
        public bool invertDelay = false;

        public float cooldown = 0;
        public float minCooldown = 0;
        public float maxCooldown = 0;
        public bool invertCooldown = false;

        public StateTransition(State from, State to, List<StateCondition> conditions = null, string test = "")
        {
            this.from = from;
            this.to = to;
            if (this.conditions == null) this.conditions = new List<StateCondition>();
            if (conditions != null) conditions.ForEach(condition => this.conditions.Add(condition));
        }

        public bool ShouldTransition()
        {
            if (muted) return false;
            if (!hasConditions) return false;
            if (!delayComplete) return false;
            if (!cooldownComplete) return false;

            List<TransitionCondition> triggers = new List<TransitionCondition>();

            foreach (StateCondition con in conditions)
            {
                bool conditionWasTrue = con.ConditionIsTrue();
                if (con.ConditionIsTrigger())
                {
                    if (conditionWasTrue) triggers.Add(con.GetTransitionCondition());
                }
                if (!conditionWasTrue)
                {
                    triggers.ForEach(trigger => trigger.Trigger());
                    return false;
                }
            }
            ResetDelays();
            timeLastPassed = Time.time;
            return true;
        }

        public void ResetDelays()
        {
            if (!HasConditionWithName("Delay")) delay = 0;
            if (HasConditionWithName("Delay Between")) delay = UnityEngine.Random.Range(minDelay, maxDelay);
            else
            {
                minDelay = 0;
                maxDelay = 0;
            }
            if (!HasConditionWithName("Cooldown")) cooldown = 0;
            if (HasConditionWithName("Cooldown Between")) cooldown = UnityEngine.Random.Range(minCooldown, maxCooldown);
            else
            {
                minCooldown = 0;
                maxCooldown = 0;
            }
        }

        private bool hasConditions => conditions != null && conditions.Count > 0;
        private bool delayComplete => !invertDelay == (delay <= 0 || stateMachine.currentState.inStateFor >= delay);
        private bool cooldownComplete => cooldown <= 0 || Time.time - timeLastPassed >= cooldown;

        public bool HasConditionWithName(string name)
        {
            foreach (StateCondition condition in conditions)
            {
                if (condition.conditionName == name) return true;
            }
            return false;
        }
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            if (conditions == null) conditions = new List<StateCondition>();
            conditions.ForEach(con => con.SetStateMachine(stateMachine, this));
        }
    }
}