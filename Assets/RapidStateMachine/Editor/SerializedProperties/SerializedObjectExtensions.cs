using UnityEditor;

namespace RSM
{
    public static class SerializedObjectExtensions
    {
        public static SerializedProperty GetProperty(this SerializedObject so, string path)
            => so.FindProperty(path);
        public static void SetProperty(this SerializedObject so, string path, float input)
        {
            EditorUtility.SetDirty(so.targetObject);
            SerializedProperty property = so.GetProperty(path);
            property.floatValue = input;
            so.ApplyModifiedProperties();
        }
        public static void SetProperty(this SerializedObject so, string path, string input)
        {
            EditorUtility.SetDirty(so.targetObject);
            SerializedProperty property = so.GetProperty(path);
            property.stringValue = input;
            so.ApplyModifiedProperties();
        }
        public static void SetProperty(this SerializedObject so, string path, bool input)
        {
            EditorUtility.SetDirty(so.targetObject);
            SerializedProperty property = so.GetProperty(path);
            property.boolValue = input;
            so.ApplyModifiedProperties();
        }
        public static void SetProperty(this SerializedObject so, string path, State input)
        {
            EditorUtility.SetDirty(so.targetObject);
            SerializedProperty property = so.GetProperty(path);
            property.objectReferenceValue = input;
            so.ApplyModifiedProperties();
        }
    }
}