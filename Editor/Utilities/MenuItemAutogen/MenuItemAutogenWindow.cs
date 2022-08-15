namespace TWILiteEditor.Utilities.MenuItemAutogen
{
    using System;
    using System.IO;
    using TWILite.Utilities.MenuItemAutogen;
    using UnityEditor;
    using UnityEngine;

    public sealed class MenuItemAutogenWindow : EditorWindow
    {
        [MenuItem("TWI/Utilities/MIA Editor")]
        private static void ShowWindow() => GetWindow<MenuItemAutogenWindow>();

        private const string EDITOR_PREF_BASE = "TWIEditor/Utilities/MenuItemAutogen/";
        private const string EDITOR_PREF_COLLECTION = "TWIEditor/Utilities/MenuItemAutogen/Collection/";
        private const string EDITOR_PREF_MODE = EDITOR_PREF_BASE + "Mode";

        private Editor editor;
        private MenuItemAutogenWindowModes mode;
        private GUIContent header = new GUIContent();

        private void OnEnable()
        {
            titleContent = new GUIContent("TWI MIA Editor");
            Undo.undoRedoPerformed += Undo_OnUndoRedoPerformed;
            OnModeChanged(null);
        }
        private void OnDisable() => Undo.undoRedoPerformed -= Undo_OnUndoRedoPerformed;
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            mode = (MenuItemAutogenWindowModes)EditorGUILayout.EnumPopup("Mode", mode);
            if (EditorGUI.EndChangeCheck()) OnModeChanged(mode);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            editor.OnInspectorGUI();
        }

        private void Undo_OnUndoRedoPerformed()
        {
            Repaint();
            SaveCollection();
        }

        private Editor CreateEditor<TC, TI>() where TC : MenuItemAutogenCollection where TI : MenuItemAutogenItem
        {
            DestroyImmediate(editor);
            TC collection = CreateInstance<TC>();
            string json = EditorPrefs.GetString(EDITOR_PREF_COLLECTION + collection.GetDisplayName(), null);
            if (!string.IsNullOrWhiteSpace(json)) JsonUtility.FromJsonOverwrite(json, collection);
            header.text = $"MIA Collection ({collection.GetDisplayName()})";

            editor = Editor.CreateEditor(collection);
            (editor as MenuItemAutogenEditor<TC, TI>).window = this;
            return editor;
        }

        private void OnModeChanged(MenuItemAutogenWindowModes? mode)
        {
            if (!mode.HasValue) mode = (MenuItemAutogenWindowModes)EditorPrefs.GetInt(EDITOR_PREF_MODE, 0);
            else EditorPrefs.SetInt(EDITOR_PREF_MODE, (int)this.mode);
            this.mode = mode.Value;

            switch (this.mode)
            {
                case MenuItemAutogenWindowModes.ChangeEXT: CreateEditor<MenuItemAutogenCollection_ChangeEXT, MenuItemAutogenItem_ChangeEXT>(); break;
                case MenuItemAutogenWindowModes.CreateFile: CreateEditor<MenuItemAutogenCollection_CreateFile, MenuItemAutogenItem_CreateFile>(); break;
                case MenuItemAutogenWindowModes.OpenWith: CreateEditor<MenuItemAutogenCollection_OpenWith, MenuItemAutogenItem_OpenWith>(); break;
                default: throw new NotImplementedException();
            }
        }

        public void SaveCollection()
        {
            MenuItemAutogenCollection collection = editor.target as MenuItemAutogenCollection;
            EditorPrefs.SetString(EDITOR_PREF_COLLECTION + collection.GetDisplayName(), JsonUtility.ToJson(collection, true));
            Debug.Log("IN 1 >> " + JsonUtility.ToJson(collection, true));
        }
    }
}