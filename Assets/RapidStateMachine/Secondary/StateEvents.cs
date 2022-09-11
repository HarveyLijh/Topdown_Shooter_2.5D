using UnityEngine;
using UnityEngine.Events;

namespace RSM
{
    public class StateEvents : MonoBehaviour
    {
        State state;
        public UnityEvent enterEvent;
        public UnityEvent exitEvent;

        private void Awake()
        {
            if (state == null) state = GetComponent<State>();
        }

        private void OnEnable()
        {
            state.enterEvent.AddListener(EnterEvent);
            state.exitEvent.AddListener(ExitEvent);
        }
        private void OnDisable()
        {
            state.enterEvent.RemoveListener(EnterEvent);
            state.exitEvent.RemoveListener(ExitEvent);
        }

        private void EnterEvent()
            => enterEvent?.Invoke();
        private void ExitEvent()
            => exitEvent?.Invoke();
    }
}