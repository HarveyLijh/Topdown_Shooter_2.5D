using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSM
{
    public class State : MonoBehaviour
    {
        public StateMachine stateMachine;
        public List<StateTransition> stateTransitions = new List<StateTransition>();
        public bool transitionFromAny;
        public StateTransition anyTransition;
        public List<State> excluding;
        public float inStateFor = 0;

        public UnityEvent enterEvent;
        public UnityEvent exitEvent;

        public virtual void OnEnter(State from)
        {
            enterEvent?.Invoke();
            inStateFor = 0;
        }

        public virtual void OnExit(State to)
        {
            exitEvent?.Invoke();
            inStateFor = 0;
        }

        public virtual void Tick()
            => inStateFor += Time.deltaTime;

        public virtual void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            foreach (StateTransition transition in stateTransitions)
            {
                if (transition != null) transition.SetStateMachine(this.stateMachine);
            }

            if (anyTransition == null) anyTransition = new StateTransition(null, this, null, $"{name} any");
            anyTransition.SetStateMachine(this.stateMachine);
        }

        public List<StateTransition> GetTransitionsTo(State to)
        {
            List<StateTransition> transitionsTo = new List<StateTransition>();
            foreach (StateTransition transition in stateTransitions)
            {
                if (transition.to == to) transitionsTo.Add(transition);
            }
            return transitionsTo;
        }

        public void RemoveTransitionsTo(State to)
        {
            List<StateTransition> toRemove = new List<StateTransition>();
            foreach (StateTransition transition in stateTransitions)
            {
                if (transition.to == to) toRemove.Add(transition);
            }
            foreach (StateTransition transition in toRemove)
            {
                stateTransitions.Remove(transition);
            }
        }
    }
}