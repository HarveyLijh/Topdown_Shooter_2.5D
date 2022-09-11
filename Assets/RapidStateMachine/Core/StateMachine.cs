using System.Collections.Generic;
using UnityEngine;

namespace RSM
{
    [System.Serializable]
    public class StateMachine : MonoBehaviour
    {
        public IStateBehaviour behaviour;

        public State currentState;
        [SerializeField] public List<State> states;
        public List<StateTransition> anyTransitions;
        [HideInInspector] public List<State> excluding;

        public StateMachineEvents.StateChangeEvent OnStateChange;
        public StateMachineEvents.StateChangeEvent AfterStateChange;

        private void Start()
        {
            behaviour = transform.parent.GetComponent<IStateBehaviour>();
            ImportStates();
        }

        private void Update()
        {
            CheckAnyTransitions();
            CheckCurrentStateTransitions();
            Tick();
        }

        public void CheckAnyTransitions()
        {
            foreach (StateTransition transition in anyTransitions)
            {
                if (transition.to == null) return;
                if (transition.to.excluding.Contains(currentState)) return;
                if (transition.ShouldTransition())
                {
                    MoveToState(transition);
                    return;
                }
            }
        }

        private void CheckCurrentStateTransitions()
        {
            if (currentState == null) return;
            foreach (StateTransition transition in currentState.stateTransitions)
            {
                if (transition.ShouldTransition())
                {
                    MoveToState(transition);
                    return;
                }
            }
        }

        public void Tick()
        {
            if (currentState != null) currentState.Tick();
            else Debug.LogError("state machine missing default state", gameObject);
        }

        public void MoveToState(State newState)
        {
            currentState?.OnExit(newState);
            State previousState = currentState;
            OnStateChange?.Invoke(previousState, newState);
            currentState = newState;
            newState.OnEnter(previousState);
            AfterStateChange?.Invoke(previousState, newState);
        }

        public void MoveToState(StateTransition transition)
        {
            State previousState = currentState;
            OnStateChange?.Invoke(previousState, transition.to);
            currentState.OnExit(transition.to);
            transition.to.OnEnter(previousState);
            currentState = transition.to;
            AfterStateChange?.Invoke(previousState, transition.to);
        }

        [ContextMenu("Import state")]
        public void ImportStates()
        {
            behaviour = transform.parent.GetComponent<IStateBehaviour>();
            states?.Clear();
            foreach (Transform child in transform)
            {
                State state = child.GetComponent<State>();
                if (state != null)
                {
                    if (!states.Contains(state)) states.Add(state);
                }
            }

            anyTransitions = new List<StateTransition>();
            if (excluding == null) excluding = new List<State>();
            if (states == null) states = new List<State>();
            foreach (State state in states)
            {
                state.SetStateMachine(this);
                if (state.transitionFromAny) anyTransitions.Add(state.anyTransition);
            }
            if (currentState == null)
            {
                if (states.Count <= 0) return;
                MoveToState(states[0]);
            }
            if (Application.isPlaying) currentState.OnEnter(null);
        }

        public void RemoveTransitionsTo(State to)
        {
            foreach (State state in states)
            {
                state.RemoveTransitionsTo(to);
            }
        }
    }
}