namespace TWILiteEditor.Utilities.MenuItemAutogen
{
    using System;
    using System.IO;
    using TWILite.Extensions;
    using TWILite.Utilities.MenuItemAutogen;
    using UnityEditor;
    using UnityEngine;

    using DataSources = TWILite.Utilities.MenuItemAutogen.MenuItemAutogenItem_CreateFile.DataSources;

    [CustomEditor(typeof(MenuItemAutogenCollection_CreateFile))]
    public sealed class MenuItemAutogenEditor_CreateFile : MenuItemAutogenEditor<MenuItemAutogenCollection_CreateFile, MenuItemAutogenItem_CreateFile>
    {
        private Rect[] columns;
        private string buttonText;

        private SerializedProperty dataSource;
        protected override float OnDrawProperty(SerializedProperty element, SerializedProperty property, Rect rect)
        {
            switch (property.name)
            {
                case MenuItemAutogenItem_CreateFile.NAMEOF_DATA:
                    columns = rect.SubdivideColumns(2, float.PositiveInfinity, 64);
                    dataSource = element.FindPropertyRelative(MenuItemAutogenItem_CreateFile.NAMEOF_DATA_SOURCE);
                    base.OnDrawProperty(element, property, columns[0]);
                    
                    switch ((DataSources)dataSource.enumValueIndex)
                    {
                        case DataSources.Base64String: buttonText = "Import"; break;
                        case DataSources.CopyFile: buttonText = "Browse"; break;
                        case DataSources.RawString: buttonText = "Import"; break;
                        default: throw new NotImplementedException();
                    }

                    if (GUI.Button(columns[1], buttonText))
                    {
                        SerializedProperty data = element.FindPropertyRelative(MenuItemAutogenItem_CreateFile.NAMEOF_DATA);
                        string path = EditorUtility.OpenFilePanel("Select file", Application.dataPath, "");
                        DataSources source = (DataSources)dataSource.enumValueIndex;
                        if (string.IsNullOrEmpty(path)) break;

                        switch(source)
                        {
                            case DataSources.CopyFile: data.stringValue = path; break;
                            case DataSources.Base64String:
                            case DataSources.RawString:
                                FileInfo info = new FileInfo(path);
                                string value = null;
                                int selection = 3;

                                if (info.Length > ushort.MaxValue) selection = RequestSourceChange();
                                if (selection > 1)
                                {
                                    if (source == DataSources.RawString) value = File.ReadAllText(path);
                                    else value = Convert.ToBase64String(File.ReadAllBytes(path));
                                }

                                if (selection == 3 && value.Length > ushort.MaxValue) selection = RequestSourceChange();
                                if (selection > 1) data.stringValue = value;
                                else if (selection == 0)
                                {
                                    dataSource.enumValueIndex = (int)DataSources.CopyFile;
                                    data.stringValue = path;
                                }
                                break;
                        }
                    }
                    break;
                case MenuItemAutogenItem_CreateFile.NAMEOF_DATA_SOURCE:
                    EditorGUI.BeginChangeCheck();
                    base.OnDrawProperty(element, property, rect);
                    if (EditorGUI.EndChangeCheck()) element.FindPropertyRelative(MenuItemAutogenItem_CreateFile.NAMEOF_DATA).stringValue = null;
                    break;
                default:
                    base.OnDrawProperty(element, property, rect);
                    break;
            }
            return rect.yMax;
        }

        private int RequestSourceChange() => EditorUtility.DisplayDialogComplex("File Too Large",
                "The selected file is larger than 64KB, which may cause problems in side of the unity editor. Changing the data source to 'Copy From File' will should prevent any problems from occurring.",
                "Change",
                "Cancel",
                "Ignore");
    }
}