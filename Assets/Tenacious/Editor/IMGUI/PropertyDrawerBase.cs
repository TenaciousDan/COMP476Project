using UnityEditor;

using UnityEngine;

using System.Collections.Generic;

namespace TenaciousEditor.IMGUI
{
    public class PropertyDrawerBase : PropertyDrawer
    {
        private Stack<Rect> horizontalStartingPositions = new Stack<Rect>();
        private LayoutMode layoutMode;

        protected enum LayoutMode { VERTICAL, HORIZONTAL }
        protected float tallestHorizontalElementHeight;
        protected float indent;
        protected Color originalColor;
        protected Color originalBackgroundColor;
        protected Color originalContentColor;
        protected float originalLabelWidth;
        protected float height = -1;
        protected Vector2 startPosition;
        protected float horizontalSpacing = 2f;
        protected float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
        protected float indentSpacing = 15f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (height == -1) OnGUI(new Rect(), property, label);

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            layoutMode = LayoutMode.VERTICAL;
            height = 0;
            startPosition = position.position;
            indent = 0;
            horizontalStartingPositions.Clear();
            tallestHorizontalElementHeight = EditorGUIUtility.singleLineHeight;
            originalColor = GUI.color;
            originalBackgroundColor = GUI.backgroundColor;
            originalContentColor = GUI.contentColor;

            originalLabelWidth = EditorGUIUtility.labelWidth;
        }

        protected virtual Rect GetRect(ref Rect position, float drawWidth = -1, float drawHeight = -1)
        {
            if (drawHeight < 0)
                drawHeight = layoutMode == LayoutMode.VERTICAL ? EditorGUIUtility.singleLineHeight : tallestHorizontalElementHeight;

            drawWidth = drawWidth < 0 ? position.width : drawWidth;

            Rect rect = new Rect(position.x, position.y, drawWidth, drawHeight);

            AdjustPosition(ref position, ref rect);

            return rect;
        }

        private void AdjustPosition(ref Rect position, ref Rect rect)
        {
            if (layoutMode == LayoutMode.VERTICAL)
            {
                float verticalSpaceUsed = rect.height + verticalSpacing;
                height += verticalSpaceUsed;
                position.y += verticalSpaceUsed;
            }
            else
            {
                float horizontalSpaceUsed = rect.width + horizontalSpacing;
                position.x += horizontalSpaceUsed;
                position.width -= horizontalSpaceUsed;

                if (tallestHorizontalElementHeight < rect.height)
                    tallestHorizontalElementHeight = rect.height;
            }
        }

        protected virtual void BeginHorizontal(ref Rect position)
        {
            layoutMode = LayoutMode.HORIZONTAL;

            horizontalStartingPositions.Push(position);
        }

        protected virtual void EndHorizontal(ref Rect position)
        {
            layoutMode = LayoutMode.VERTICAL;

            Rect r = horizontalStartingPositions.Pop();
            position.x = r.x;
            position.width = r.width;

            float verticalSpaceUsed = tallestHorizontalElementHeight + verticalSpacing;
            position.y += verticalSpaceUsed;
            height += verticalSpaceUsed;

            tallestHorizontalElementHeight = EditorGUIUtility.singleLineHeight;
        }

        protected virtual void Indent(ref Rect position, float n = 1)
        {
            float space = indentSpacing * n;
            indent += space;
            position.x += space;
            position.width -= space;
        }

        protected virtual void DrawLine(ref Rect position, float drawWidth = -1, float drawHeight = -1)
        {
            DrawLine(ref position, new Color(0, 0, 0, 0.5f), drawWidth, drawHeight);
        }
        protected virtual void DrawLine(ref Rect position, Color color, float drawWidth = -1, float drawHeight = -1)
        {
            if (layoutMode == LayoutMode.VERTICAL)
                drawHeight = drawHeight < 0 ? 1 : drawHeight;
            else
                drawWidth = drawWidth < 0 ? 1 : drawWidth;

            Color previousBGColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(GetRect(ref position, drawWidth, drawHeight), GUIContent.none);
            GUI.backgroundColor = previousBGColor;
        }

        /// <summary>
        /// Begin a padded area. padding is processed as (up, right, down, left).
        /// </summary>
        protected virtual void BeginPadding(ref Rect position, Vector4 padding)
        {
            position.x += padding.w;
            position.width -= (padding.w + padding.y);
            position.y += padding.x;
            height += padding.x;
        }

        /// <summary>
        /// End a padded area. padding is processed as (up, right, down, left).
        /// </summary>
        protected virtual void EndPadding(ref Rect position, Vector4 padding)
        {
            position.x -= padding.w;
            position.width += (padding.w + padding.y);
            position.y += padding.z;
            height += padding.z;
        }
    }
}
