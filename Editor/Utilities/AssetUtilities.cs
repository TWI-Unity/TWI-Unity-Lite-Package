namespace TWILiteEditor.Utilities
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using TWILite.Utilities;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEditor.PackageManager.Requests;
    using UnityEngine;

    using CreateOptions = TWILite.Utilities.FileUtilities.CreateOptions;
    using Object = UnityEngine.Object;
    using PackageInfo = UnityEditor.PackageManager.PackageInfo;

    public static class AssetUtilities
    {
        public enum LoadOptions : byte
        {
            None,
            Rename,
            Select,
        }

        public static T[] ChangeExtensions<T>(T[] objs, string ext, LoadOptions loadOptions = default) where T : Object
        {
            for (int i = 0; i < objs.Length; i++) objs[i] = ChangeExtension(objs[i], ext, LoadOptions.None);
            if (loadOptions == LoadOptions.Select) Selection.objects = objs;
            return objs;
        }
        public static T ChangeExtension<T>(T obj, string ext, LoadOptions loadOptions = default) where T : Object
        {
            string source = GetEditorPath(obj);
            if (string.IsNullOrWhiteSpace(source)) return null;
            string target = Path.ChangeExtension(source, ext);

            string sourceMeta = source + ".meta";
            string targetMeta = target + ".meta";
            if (File.Exists(source)) File.Move(source, target);
            if (File.Exists(sourceMeta)) File.Move(sourceMeta, targetMeta);

            AssetDatabase.DeleteAsset(source);
            return Load<T>(target, loadOptions);
        }

        public static Object Create(string path, byte[] data, CreateOptions createOptions, LoadOptions loadOptions) => Create<Object>(path, data, createOptions, loadOptions);
        public static Object Create(string path, string text, CreateOptions createOptions, LoadOptions loadOptions) => Create<Object>(path, text, createOptions, loadOptions);
        public static Object Create(string path, Object obj, CreateOptions createOptions, LoadOptions loadOptions) => Create<Object>(path, obj, createOptions, loadOptions);

        public static T Create<T>(string path, byte[] data, CreateOptions createOptions, LoadOptions loadOptions) where T : Object => Load<T>(FileUtilities.Create(GetEditorPath(path), data, createOptions), loadOptions);
        public static T Create<T>(string path, string text, CreateOptions createOptions, LoadOptions loadOptions) where T : Object => Load<T>(FileUtilities.Create(GetEditorPath(path), text, createOptions), loadOptions);
        public static T Create<T>(string path, CreateOptions createOptions, LoadOptions loadOptions) where T : ScriptableObject => Create(path, ScriptableObject.CreateInstance<T>(), createOptions, loadOptions);
        public static T Create<T>(string path, T obj, CreateOptions createOptions, LoadOptions loadOptions) where T : Object
        {
            path = GetEditorPath(path);
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");
            else if (obj == null) throw new ArgumentNullException("obj");
            switch (createOptions)
            {
                case CreateOptions.UniqueID: path = FileUtilities.GetUniqueFilepathWithID(path); break;
                case CreateOptions.UniqueGUID: path = FileUtilities.GetUniqueFilepathWithGUID(path); break;
                case CreateOptions.Skip:
                    if (File.Exists(path)) return Load<T>(path, loadOptions);
                    break;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (AssetDatabase.LoadMainAssetAtPath(path) is T asset) EditorUtility.CopySerialized(obj, asset);
            else AssetDatabase.CreateAsset(obj, path);
            return Load<T>(path, loadOptions);
        }

        private static PackageCollection GetAllRegisteredPackages()
        {
            ListRequest request = Client.List(true);
            while (request.Status == StatusCode.InProgress) ;
            return request.Result;
        }
        public static string GetCallerFilePath(string ext, [CallerFilePath] string callerFilePath = default) => ext == null ? callerFilePath : Path.ChangeExtension(callerFilePath, ext);
        public static string GetEditorPath(Object obj, bool includePackages = false) => obj == null ? null : GetEditorPath(AssetDatabase.GetAssetPath(obj), includePackages);
        public static string GetEditorPath(string path, bool includePackages = false)
        {
            path = FileUtilities.GetFullPath(path);
            if (string.IsNullOrWhiteSpace(path)) return null;
            else if (path.StartsWith(Application.dataPath)) return "Assets" + path.Substring(Application.dataPath.Length);
            else if (!includePackages) return null;

            PackageCollection packages = GetAllRegisteredPackages();
            if (packages.FirstOrDefault(p => p.source == PackageSource.Local && path.StartsWith(p.resolvedPath.Replace('\\', '/'))) is PackageInfo info) return info.assetPath + path.Substring(info.resolvedPath.Length);
            else return null;
        }
        public static string GetSelectedPath(string name = null)
        {
            string path;
            if (Selection.objects.Length == 0) path = Application.dataPath;
            else path = GetSelectedPath(Selection.objects[0]);

            Object[] objs = Selection.objects;
            for (int i = 1; i < objs.Length; i++)
                if (path != GetSelectedPath(objs[i])) return Application.dataPath;
            return string.IsNullOrWhiteSpace(name) ? path : path + "/" + name;
        }
        public static string GetSelectedPath(Object obj, string name = null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) path = Application.dataPath;
            else if (File.GetAttributes(path) != FileAttributes.Directory) path = Path.GetDirectoryName(path);
            return string.IsNullOrWhiteSpace(name) ? path : path + "/" + name;
        }

        public static T Load<T>(string path, LoadOptions loadOptions = default) where T : Object
        {
            path = GetEditorPath(path);
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("path");
            else AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);

            T obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (loadOptions == LoadOptions.Rename) StartRename(obj);
            else if (loadOptions == LoadOptions.Select) Selection.activeObject = obj;
            return obj;
        }

        public static void Open(Object obj, string appPath = null, params string[] args)
        {
            if (obj == null) throw new ArgumentNullException();
            string path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrWhiteSpace(path)) throw new FileNotFoundException();
            else FileUtilities.Open(AssetDatabase.GetAssetPath(obj), appPath, args);
        }
        public static void Open(Object[] objs, string appPath = null, params string[] args)
        {
            for (int i = 0; i < objs.Length; i++) Open(objs[i], appPath, args);
        }

        public static void StartRename(Object obj) => EditorCoroutineUtility.StartCoroutineOwnerless(StartRename_Coroutine(obj));
        private static IEnumerator StartRename_Coroutine(Object obj)
        {
            Selection.activeObject = obj;
            EditorUtility.FocusProjectWindow();
            yield return null;

            EditorWindow window = EditorUtilities.GetWindow(EditorWindowTypes.Project);
            window.SendEvent(new Event() { keyCode = KeyCode.Escape, type = EventType.KeyDown });
            window.SendEvent(new Event() { keyCode = KeyCode.F2, type = EventType.KeyDown });
        }

        public static bool ValidateExtension(string path, params string[] exts)
        {
            if (string.IsNullOrWhiteSpace(path) || exts == null || exts.Length == 0) return false;
            string ext = Path.GetExtension(path).ToLower().TrimStart('.');
            return exts.Any(e => e.ToLower().TrimStart('.') == ext);
        }
        public static bool ValidateExtension(Object obj, params string[] exts) => obj != null && ValidateExtension(AssetDatabase.GetAssetPath(obj), exts);
        public static bool ValidateExtensions(Object[] objs, params string[] exts)
        {
            for (int i = 0; i < objs.Length; i++)
                if (!ValidateExtension(objs[i], exts)) return false;
            return true;
        }
    }
}