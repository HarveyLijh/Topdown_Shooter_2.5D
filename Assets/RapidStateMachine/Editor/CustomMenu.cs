using UnityEngine;
using UnityEditor;

namespace RSM
{
    public class CustomMenu
    {
        [MenuItem("GameObject/State Machine")]
        public static void CreateNewStateMachine()
        {
            GameObject newCharacter = new GameObject("StateBehaviour");
            GameObject stateMachine = new GameObject("StateMachine");
            stateMachine.transform.parent = newCharacter.transform;
            stateMachine.AddComponent<StateMachine>();
            stateMachine.AddComponent<StateMachineDebugger>();
            Selection.activeGameObject = stateMachine;
        }
    }
}