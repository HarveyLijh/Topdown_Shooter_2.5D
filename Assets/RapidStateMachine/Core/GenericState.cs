using System;
using UnityEngine;
using System.Reflection;

namespace RSM
{
    public class GenericState : State
    {
        private MethodInfo enter;
        private MethodInfo tick;
        private MethodInfo exit;

        public override void OnEnter(State from)
        {
            enter?.Invoke(stateMachine.behaviour, null);
            base.OnEnter(from);
        }

        public override void Tick()
        {
            tick?.Invoke(stateMachine.behaviour, null);
            base.Tick();
        }

        public override void OnExit(State to)
        {
            exit?.Invoke(stateMachine.behaviour, null);
            base.OnExit(to);
        }

        public override void SetStateMachine(StateMachine stateMachine)
        {
            base.SetStateMachine(stateMachine);

            MonoBehaviour mono = (MonoBehaviour)stateMachine.behaviour;

            Type behaviour = mono.GetType();
            enter = behaviour.GetMethod($"Enter{gameObject.name}", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            tick = behaviour.GetMethod(gameObject.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            exit = behaviour.GetMethod($"Exit{gameObject.name}", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public bool HasEnterMethod() => enter != null;
        public bool HasTickMethod() => tick != null;
        public bool HasExitMethod() => exit != null;

        public void OpenEnter()
        {
            MonoBehaviour mono = (MonoBehaviour)stateMachine.behaviour;
            VSManager.OpenMethod(mono, enter);
        }
        public void OpenTick()
        {
            MonoBehaviour mono = (MonoBehaviour)stateMachine.behaviour;
            VSManager.OpenMethod(mono, tick);
        }
        public void OpenExit()
        {
            MonoBehaviour mono = (MonoBehaviour)stateMachine.behaviour;
            VSManager.OpenMethod(mono, exit);
        }
    }
}