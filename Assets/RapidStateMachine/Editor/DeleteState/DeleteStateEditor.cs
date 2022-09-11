using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace RSM
{
    public class DeleteStateEditor : EditorWindow
    {
        private static DeleteStateEditor current;
        private VisualElement root;
        private VisualTreeAsset tree;
        private SerializedObject so;

        public void OnEnable()
        {
            root = new VisualElement();
            tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RapidStateMachine/Editor/DeleteState/DeleteStateWindow.uxml");
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RapidStateMachine/Editor/StateStyle.uss");

            tree.CloneTree(root);
            root.styleSheets.Add(style);
        }
        public static void Show(State state)
        {
            if (current == null) current = new DeleteStateEditor();
            EditorWindow wnd = GetWindow<DeleteStateEditor>();
            wnd.titleContent = new GUIContent("Delete State?");
            wnd.minSize = new Vector2(350, 200);
            wnd.maxSize = new Vector2(350, 200);

            current.SetHeader(state);
            current.SetDeleteBoth(state);
            current.SetOnlyDeleteState(state);
            current.SetCancel();

            wnd.rootVisualElement.Add(current.root);
            wnd.ShowModal();
        }

        private void SetHeader(State state)
        {
            Label header = root.Q<Label>("Header");
            header.text = $"Delete \"{state.name}\"?";
        }

        private void SetDeleteBoth(State state)
        {
            Button deleteBoth = root.Q<Button>("DeleteBoth");
            so = new SerializedObject(state.stateMachine.gameObject);
            deleteBoth.clicked += () =>
            {
                EditorUtility.SetDirty(state.stateMachine.gameObject);
                StateMachine stateMachine = state.stateMachine;

                stateMachine.RemoveTransitionsTo(state);
                so.Update();
                stateMachine.states.Remove(state);
                Undo.DestroyObjectImmediate(state.gameObject);
                this.Close();
            };
        }

        private void SetOnlyDeleteState(State state)
        {
            Button onlyDeleteState = root.Q<Button>("OnlyDeleteState");
            onlyDeleteState.clicked += () =>
            {
                EditorUtility.SetDirty(state.stateMachine.gameObject);
                StateMachine stateMachine = state.stateMachine;

                stateMachine.states.Remove(state);
                Undo.DestroyObjectImmediate(state.gameObject);
                this.Close();
            };
        }

        private void SetCancel()
        {
            Button cancel = root.Q<Button>("Cancel");
            cancel.clicked += () =>
            {
                this.Close();
            };
        }
    }
}