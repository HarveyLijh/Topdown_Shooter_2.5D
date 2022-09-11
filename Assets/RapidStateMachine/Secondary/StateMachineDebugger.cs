using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSM
{
    public class StateMachineDebugger : MonoBehaviour
    {
        private StateMachine stateMachine;

        public List<TransitionRecord> transitionRecords = new List<TransitionRecord>();
        public int maxTransitionRecords = 10;
        public bool trackTransitions = false;
        private float lastTransitioned = 0;

        public StateMachineEvents.VoidEvent OnChangedState;

        private void OnEnable()
        {
            stateMachine = GetComponent<StateMachine>();
            stateMachine.OnStateChange += ShowStateChange;
        }

        private void ShowStateChange(State from, State to)
        {
            CreateTransitionRecord(from, to);
            OnChangedState?.Invoke();
        }

        private void CreateTransitionRecord(State from, State to)
        {
            if (!trackTransitions) return;
            if (transitionRecords.Count >= maxTransitionRecords) transitionRecords.RemoveAt(0);
            transitionRecords.Add(new TransitionRecord(from.name, to.name, Time.time - lastTransitioned));
            lastTransitioned = Time.time;
        }

    }

    [System.Serializable]
    public class TransitionRecord
    {
        public string from;
        public string to;
        public float duration;
        public TransitionRecord(string from, string to, float duration)
        {
            this.from = from;
            this.to = to;
            this.duration = duration;
        }
    }
}