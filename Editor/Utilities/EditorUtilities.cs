namespace TWILiteEditor.Utilities
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class EditorUtilities
    {
        public static T GetWindow<T>() where T : EditorWindow => GetWindow<T>(typeof(T));
        public static T GetWindow<T>(Type type) where T : EditorWindow => Resources.FindObjectsOfTypeAll<T>().Where(w => w.GetType() == type).FirstOrDefault();
        public static EditorWindow GetWindow(EditorWindowTypes window)
        {
            string windowTypeName = null;
            switch (window)
            {
                case EditorWindowTypes.Console: windowTypeName = "UnityEditor.ConsoleWindow"; break;
                case EditorWindowTypes.Game: windowTypeName = "UnityEditor.GameView"; break;
                case EditorWindowTypes.Hierarchy: windowTypeName = "UnityEditor.SceneHierarchyWindow"; break;
                case EditorWindowTypes.Inspector: windowTypeName = "UnityEditor.InspectorWindow"; break;
                case EditorWindowTypes.Project: windowTypeName = "UnityEditor.ProjectBrowser"; break;
                case EditorWindowTypes.Scene: windowTypeName = "UnityEditor.SceneView"; break;
                default: throw new NotImplementedException($"The '{window}' window is not supported");
            }

            Type windowType = typeof(EditorWindow).Assembly.GetType(windowTypeName);
            return Resources.FindObjectsOfTypeAll<EditorWindow>().Where(w => w.GetType() == windowType).FirstOrDefault();
        }
    }
}