namespace TWILiteEditor.Extensions
{
    using System.Collections.Generic;
    using UnityEditor;

    public static class SerializedProperty_GetChildren
    {
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property, bool enterChildren = false)
        {
            if (property == null || !property.hasVisibleChildren || !property.isExpanded) yield break;
            SerializedProperty copy = property.Copy();
            SerializedProperty next = property.Copy();
            next.NextVisible(false);
            copy.NextVisible(true);
            yield return copy;

            while (copy.NextVisible(enterChildren && copy.isExpanded))
                if (SerializedProperty.EqualContents(copy, next)) break;
                else yield return copy;
        }
    }
}