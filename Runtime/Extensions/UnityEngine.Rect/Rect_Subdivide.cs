namespace TWILite.Extensions
{
    using System.Linq;
    using UnityEngine;

    public static class Rect_Subdivide
    {
        public static Rect[] SubdivideColumns(this Rect rect, float paddingRight = 0, params float[] columnWidths)
        {
            if (columnWidths == null || columnWidths.Length == 0) columnWidths = new float[] { rect.width };
            rect.width -= paddingRight * (columnWidths.Where(w => w != 0).Count() - 1);
            Rect[] columns = new Rect[columnWidths.Length];

            float width;
            float totalWidth = 0;
            int flexibleColumns = 0;
            for (int i = 0; i < columns.Length; i++)
            {
                width = columnWidths[i];
                if (float.IsInfinity(width)) flexibleColumns++;
                else totalWidth += width;
                columns[i] = rect;
            }

            float flexibleWidth = totalWidth < rect.width ? (rect.width - totalWidth) / flexibleColumns : 0;
            for (int i = 0; i < columns.Length; i++) columns[i].width = float.IsInfinity(columnWidths[i]) ? flexibleWidth : columnWidths[i];
            for (int i = 1; i < columns.Length; i++) columns[i].x = columns[i - 1].xMax + paddingRight;
            return columns;
        }
        public static Rect[] SubdivideRows(this Rect rect, float paddingBottom = 0, params float[] rowHeights)
        {
            if (rowHeights == null || rowHeights.Length == 0) rowHeights = new float[] { rect.height };
            rect.height -= paddingBottom * (rowHeights.Where(h => h != 0).Count() - 1);
            Rect[] rows = new Rect[rowHeights.Length];

            float height;
            float totalHeight = 0;
            int flexibleRows = 0;
            for (int i = 0; i < rows.Length; i++)
            {
                height = rowHeights[i];
                if (float.IsInfinity(height)) flexibleRows++;
                else totalHeight += height;
                rows[i] = rect;
            }

            float flexibleHeight = totalHeight < rect.height ? (rect.height - totalHeight) / flexibleRows : 0;
            for (int i = 0; i < rows.Length; i++) rows[i].height = float.IsInfinity(rowHeights[i]) ? flexibleHeight : rowHeights[i];
            for (int i = 1; i < rows.Length; i++) rows[i].y = rows[i - 1].yMax + paddingBottom;
            return rows;
        }
    }
}