namespace TWILiteEditor.Utilities.MenuItemAutogen
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using TWILite.Extensions;
    using TWILite.Utilities;
    using TWILite.Utilities.CoroutineTasks;
    using TWILite.Utilities.MenuItemAutogen;
    using TWILiteEditor.Extensions;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public abstract class MenuItemAutogenEditor<TC, TI> : Editor where TC : MenuItemAutogenCollection where TI : MenuItemAutogenItem
    {
        protected const float FOLDOUT_PADDING = 2f;

        protected const float REORDERABLE_LIST_ELEMENT_HEIGHT = 21f;
        protected const float REORDERABLE_LIST_HEIGHT_DEFAULT = 40f;
        protected const float REORDERABLE_LIST_HEIGHT_EMPTY = REORDERABLE_LIST_HEIGHT_DEFAULT + REORDERABLE_LIST_ELEMENT_HEIGHT;

        protected const float SINGLE_ELEMENT_HEIGHT = 18f;

        private readonly GUIContent content_Label = new GUIContent();
        private string content_Text = string.Empty;

        private ReorderableList list;
        private readonly Dictionary<string, ReorderableList> lists = new Dictionary<string, ReorderableList>();
        private readonly GUIContent list_Dropdown = new GUIContent("Options");
        private SerializedProperty list_Element;

        private SerializedProperty property_ClassName;
        private SerializedProperty property_Items;
        private SerializedProperty property_Namespace;
        private SerializedProperty property_RootMenuPath;
        private SerializedProperty property_JsonLock;

        private Rect[] rect_Bounds;

        private GUIStyle style_DropdownButton;

        private Vector2 vector2_ScrollPos = Vector2.zero;

        public MenuItemAutogenWindow window;

        private void OnEnable()
        {
            property_ClassName = serializedObject.FindProperty(MenuItemAutogenCollection.NAMEOF_CLASS_NAME);
            property_Items = serializedObject.FindProperty("items");
            property_Namespace = serializedObject.FindProperty(MenuItemAutogenCollection.NAMEOF_NAMESPACE);
            property_RootMenuPath = serializedObject.FindProperty(MenuItemAutogenCollection.NAMEOF_ROOT_MENU_PATH);
            property_JsonLock = serializedObject.FindProperty(MenuItemAutogenCollection.NAMEOF_LOCK_JSON);

            list = new ReorderableList(serializedObject, property_Items)
            {
                drawHeaderCallback = collection_OnDrawHeader,
                drawElementCallback = collection_OnDrawElement,
                elementHeightCallback = collection_OnGetElementHeight,
                onMouseDragCallback = collection_OnMouseDrag,
            };
        }
        public override void OnInspectorGUI()
        {
            if (style_DropdownButton == null) style_DropdownButton = new GUIStyle(EditorStyles.popup) { fixedHeight = 0 };
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(property_Namespace);
            EditorGUILayout.PropertyField(property_ClassName);
            EditorGUILayout.PropertyField(property_RootMenuPath);

            vector2_ScrollPos = EditorGUILayout.BeginScrollView(vector2_ScrollPos);
            list.DoLayoutList();
            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();

            if (serializedObject.ApplyModifiedProperties())
                if (window != null) window.SaveCollection();
        }

        protected string GetDisplayName() => (target as TC).GetDisplayName();
        protected ReorderableList GetPropertyList(SerializedProperty property)
        {
            property = serializedObject.FindProperty(property.propertyPath);
            if (!lists.TryGetValue(property.propertyPath, out ReorderableList list)) lists.Add(property.propertyPath, list = OnCreatePropertyList(property));
            return list;
        }

        protected virtual ReorderableList OnCreatePropertyList(SerializedProperty element) => new ReorderableList(serializedObject, element)
        {
            drawElementCallback = (rect, index, isActive, isFocused) => OnDrawPropertyElement(element, rect, index, isActive, isFocused),
            drawHeaderCallback = rect => OnDrawPropertyHeader(element, rect),
        };

        protected virtual float OnDrawProperty(SerializedProperty element, SerializedProperty property, Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            if (!property.IsArray()) EditorGUI.PropertyField(rect, property);
            else
            {
                if (window != null) EditorGUI.indentLevel--;
                if (property.isExpanded) property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, property.displayName, true);
                else property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, property.displayName + " (" + property.arraySize + ')', true);
                if (window != null) EditorGUI.indentLevel++;

                if (property.isExpanded)
                {
                    rect.y += rect.height + FOLDOUT_PADDING;
                    rect.height = collection_OnGetPropertyHeight(property, 0) - FOLDOUT_PADDING;
                    rect = EditorGUI.IndentedRect(rect);

                    EditorGUI.indentLevel--;
                    GetPropertyList(property).DoList(rect);
                    EditorGUI.indentLevel++;
                }
            }
            return rect.yMax;
        }
        protected virtual void OnDrawPropertyHeader(SerializedProperty property, Rect rect) => EditorGUI.LabelField(rect, property.displayName + " (" + property.arraySize + ")");
        protected virtual void OnDrawPropertyElement(SerializedProperty property, Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            content_Text = OnGetPropertyDisplayName(property) + ' ';
            content_Label.text = content_Text + new string('0', property.arraySize.ToString().Length);
            rect_Bounds = rect.SubdivideColumns(4, EditorStyles.label.CalcSize(content_Label).x, float.PositiveInfinity);
            content_Label.text = content_Text + (index + 1);

            EditorGUI.LabelField(rect_Bounds[0], content_Label);
            property = property.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect_Bounds[1], property, GUIContent.none);
        }

        protected virtual string OnGetPropertyDisplayName(SerializedProperty property) => property.displayName.TrimEnd('s');

        private void collection_OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            list_Element = property_Items.GetArrayElementAtIndex(index);

            rect = new Rect(rect.x, rect.y, rect.width, rect.height - FOLDOUT_PADDING);
            EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox);

            if (window == null) EditorGUI.indentLevel++;
            rect = new Rect(rect.x + FOLDOUT_PADDING, rect.y, rect.width - FOLDOUT_PADDING, EditorGUIUtility.singleLineHeight);
            list_Element.isExpanded = EditorGUI.Foldout(rect, list_Element.isExpanded, (index + 1) + ") " + list_Element.FindPropertyRelative(MenuItemAutogenItem.NAMEOF_MENU_PATH).stringValue, true);
            if (window == null) EditorGUI.indentLevel--;

            EditorGUI.indentLevel++;
            rect = new Rect(rect.x, rect.y + rect.height, rect.width - 4, rect.height);
            foreach (var child in list_Element.GetChildren()) rect.y = OnDrawProperty(list_Element, child, rect) + 2;
            EditorGUI.indentLevel--;
        }
        private void collection_OnDrawHeader(Rect rect)
        {
            rect.width -= 55;
            EditorGUI.LabelField(rect, GetDisplayName());

            rect = new Rect(rect.xMax, rect.y, 60, rect.height);
            if (EditorGUI.DropdownButton(rect, list_Dropdown, FocusType.Passive, style_DropdownButton))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Compile"), false, dropdown_OnCompile);
                menu.AddSeparator(null);
                menu.AddItem(new GUIContent("Copy"), false, dropdown_OnExportToSYSTEM);
                menu.AddItem(new GUIContent("Paste"), false, dropdown_OnImportFromSYSTEM);
                menu.AddSeparator(null);
                menu.AddItem(new GUIContent("Export/MIA Collection"), false, dropdown_OnExportToASSET);
                menu.AddItem(new GUIContent("Export/JSON File"), false, dropdown_OnExportToJSON);
                menu.AddItem(new GUIContent("Import/MIA Collection"), false, dropdown_OnImportFromASSET);
                menu.AddItem(new GUIContent("Import/JSON File"), false, dropdown_OnImportFromJSON);
                menu.AddSeparator(null);
                menu.AddItem(new GUIContent("Reset"), false, dropdown_OnReset);
                menu.AddSeparator(null);
                menu.AddItem(new GUIContent("Clear"), false, dropdown_OnClear);
                menu.ShowAsContext();
            }
        }
        private float collection_OnGetElementHeight(int index)
        {
            float height = SINGLE_ELEMENT_HEIGHT;
            list_Element = property_Items.GetArrayElementAtIndex(index);
            if (list_Element.isExpanded)
                foreach (var child in list_Element.GetChildren())
                    height += collection_OnGetPropertyHeight(child);
            return height + FOLDOUT_PADDING;
        }
        private float collection_OnGetPropertyHeight(SerializedProperty property, float height = SINGLE_ELEMENT_HEIGHT)
        {
            if (property.isExpanded)
            {
                if (property.arraySize == 0) height += REORDERABLE_LIST_HEIGHT_EMPTY;
                else height += property.arraySize * REORDERABLE_LIST_ELEMENT_HEIGHT + REORDERABLE_LIST_HEIGHT_DEFAULT;
            }
            return height;
        }
        private void collection_OnMouseDrag(ReorderableList list)
        {
            for (int i = 0; i < list.serializedProperty.arraySize; i++)
                list.serializedProperty.GetArrayElementAtIndex(i).isExpanded = false;
        }

        private void dropdown_ImportCollection(TC collection)
        {
            SerializedProperty jsonLock = new SerializedObject(collection).FindProperty(MenuItemAutogenCollection.NAMEOF_LOCK_JSON);
            if (jsonLock.stringValue != property_JsonLock.stringValue) EditorUtility.DisplayDialog("Invalid Asset", $"The selected asset is not of type '{typeof(TC).Name}'", "Ok");
            else
            {
                EditorUtility.CopySerialized(collection, target);
                if (window != null) window.SaveCollection();
                DestroyImmediate(collection);
            }
        }
        private void dropdown_OnClear()
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to clear the collection?", "Yes", "No"))
                (target as MenuItemAutogenCollection).Clear();
            if (window != null) window.SaveCollection();
        }
        private void dropdown_OnCompile()
        {
            MenuItemAutogenCollection<TI> collection = target as MenuItemAutogenCollection<TI>;

            string @class = collection.ClassName;
            if (string.IsNullOrWhiteSpace(@class)) @class = collection.GetClassNameFallback();
            else if (!@class.StartsWithLetter('_')) @class = collection.GetClassNameFallback();
            else if (!@class.IsAlphaNumeric('_')) @class = collection.GetClassNameFallback();

            string path = EditorUtility.SaveFilePanel("Save To...", Application.dataPath, @class, "cs");
            if (string.IsNullOrWhiteSpace(path)) return;

            CoroutineTaskInterval coroutine = new CoroutineTaskInterval(EditorCoroutineUtility.StartCoroutineOwnerless);
            coroutine.OnStateChanged += dropdown_OnCompileStateChanged;
            coroutine.Start(dropdown_OnCompile(coroutine, path));
        }
        private IEnumerator dropdown_OnCompile(CoroutineTaskInterval coroutine, string path)
        {
            MenuItemAutogenCollection<TI> collection = target as MenuItemAutogenCollection<TI>;
            MenuItemAutogenCollectionCompiler<TI> compiler = new MenuItemAutogenCollectionCompiler<TI>(collection);
            compiler.OnWarning += dropdown_OnComileWarning;

            string titleFormat = "Compiling scripting... ({0}/{1}) ({2:P0})";
            string msgFormat = "Compiling script, please wait...";

            msgFormat += Environment.NewLine + "Progress: {0}/{1} ({2:P0})";
            string title = string.Format(titleFormat, compiler.Index, compiler.Count, compiler.Progress);
            string msg = string.Format(msgFormat, compiler.Index, compiler.Count, compiler.Progress);
            EditorUtility.DisplayCancelableProgressBar(title, msg, compiler.Progress);

            string temp = "%TEMP%/TW Innovations/Autogen";
            temp = FileUtilities.GetFullPath(temp);
            Directory.CreateDirectory(temp);

            temp = Path.Combine(temp, Path.GetFileName(path));
            using (TextWriter stream = new StreamWriter(temp))
            using (IndentedTextWriter writer = new IndentedTextWriter(stream))
            {
                IEnumerator enumerator = compiler.Start(writer);
                while (enumerator.MoveNext())
                    if (coroutine.Update())
                    {
                        msg = string.Format(msgFormat, compiler.Index, compiler.Count, compiler.Progress);
                        title = string.Format(titleFormat, compiler.Index, compiler.Count, compiler.Progress);
                        if (EditorUtility.DisplayCancelableProgressBar(title, msg, compiler.Progress)) coroutine.Cancel();
                        yield return null;
                    }
            }

            title = string.Format(titleFormat, compiler.Index, compiler.Count, compiler.Progress);
            msg = string.Format(msgFormat, compiler.Index, compiler.Count, compiler.Progress);
            EditorUtility.DisplayCancelableProgressBar(title, msg, compiler.Progress);
            path = AssetUtilities.GetEditorPath(path);
            if (File.Exists(path)) File.Delete(path);
            File.Move(temp, path);

            EditorUtility.ClearProgressBar();
            if (path != null) AssetDatabase.ImportAsset(path);
        }
        private void dropdown_OnCompileStateChanged(CoroutineTaskInterval coroutine)
        {
            EditorUtility.ClearProgressBar();
            switch (coroutine.State)
            {
                case CoroutineTaskStates.Error:
                    Debug.LogError(coroutine.Exception);
                    break;
                case CoroutineTaskStates.Cancelled:
                    Debug.Log("Operation 'Compile' cancelled");
                    break;
            }
        }
        private void dropdown_OnComileWarning(string warning) => Debug.LogWarning(warning);
        private void dropdown_OnExportToASSET()
        {
            string path = EditorUtility.SaveFilePanel("Save as...", Application.dataPath, $"New MIA Collection ({GetDisplayName()})", "asset");

            if (string.IsNullOrWhiteSpace(path)) return;
            else path = AssetUtilities.GetEditorPath(path, false);
            if (string.IsNullOrWhiteSpace(path)) EditorUtility.DisplayDialog("Need to save in the Assets folder", "You need to save the file from inside of the project's assets folder", "Ok");
            else
            {
                TC collection = Instantiate(target as TC);
                AssetDatabase.CreateAsset(collection, path);
                AssetDatabase.ImportAsset(path);
            }
        }
        private void dropdown_OnExportToJSON()
        {
            string path = EditorUtility.SaveFilePanel("Save as...", Application.dataPath, $"New MIA JSON File ({GetDisplayName()})", "json");
            if (!string.IsNullOrWhiteSpace(path)) File.WriteAllText(FileUtilities.GetFullPath(path), JsonUtility.ToJson(target, true));
        }
        private void dropdown_OnExportToSYSTEM() => EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(target, true);
        private void dropdown_OnImportFromASSET()
        {
            string path = EditorUtility.OpenFilePanel("Import...", Application.dataPath, "asset");

            if (string.IsNullOrWhiteSpace(path)) return;
            else path = AssetUtilities.GetEditorPath(path);
            if (string.IsNullOrWhiteSpace(path)) EditorUtility.DisplayDialog("Need to open in the Assets folder", "You need to open the file from inside of the project's assets folder", "Ok");
            else
            {
                MenuItemAutogenCollection collection = AssetDatabase.LoadAssetAtPath<MenuItemAutogenCollection>(path);
                if (collection == null) EditorUtility.DisplayDialog("Invalid Asset", "The selected asset is not of type 'MenuItemAutogenCollection'", "Ok");
                else if (!(collection is TC)) EditorUtility.DisplayDialog("Invalid Asset", $"The selected asset is not of type '{typeof(TC).Name}'", "Ok");
                else dropdown_ImportCollection(collection as TC);
            }
        }
        private void dropdown_OnImportFromJSON()
        {
            string path = EditorUtility.OpenFilePanel("Import...", Application.dataPath, "json");
            if (!string.IsNullOrWhiteSpace(path))
            {
                TC collection = CreateInstance<TC>();
                path = FileUtilities.GetFullPath(path);
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), collection);
                dropdown_ImportCollection(collection);
            }
        }
        private void dropdown_OnImportFromSYSTEM()
        {
            TC collection = CreateInstance<TC>();
            JsonUtility.FromJsonOverwrite(EditorGUIUtility.systemCopyBuffer, collection);
            dropdown_ImportCollection(collection);
        }
        private void dropdown_OnReset()
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to reset the collection to its default values?", "Yes", "No")) (target as TC).Reset();
            if (window != null) window.SaveCollection();
        }
    }
}