using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Diagnostics;

namespace RSM
{
    public class VSManager : MonoBehaviour
    {
        static int line;

        public static void Trace()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;
            var stackTrace = new StackTrace(true);
            line = stackTrace.GetFrames()[1].GetFileLineNumber();
#endif
        }

        public static void OpenMethod(UnityEngine.Object component, MethodInfo method)
        {
#if UNITY_EDITOR
            Type type = component.GetType();
            var test = Activator.CreateInstance(type);
            try
            {
                method.Invoke(test, null);
            }
            catch { }

            string path = $"{component.GetType().Name}.cs";
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (assetPath.EndsWith(path))
                {
                    var script = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
                    if (script != null)
                    {
                        AssetDatabase.OpenAsset(script, line);
                        //If your IDE isn't working, try using line 44 instead of line 42.
                        //UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(assetPath, line, 0);
                        break;
                    }
                }
            }
#endif
        }
    }
}