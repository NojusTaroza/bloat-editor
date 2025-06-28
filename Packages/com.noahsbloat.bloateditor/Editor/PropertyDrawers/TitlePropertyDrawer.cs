using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// Property drawer for Title attribute - enhanced header with styling options
    /// </summary>
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitlePropertyDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            TitleAttribute titleAttr = (TitleAttribute)attribute;

            // Calculate positions
            float currentY = position.y + titleAttr.SpaceAbove;
            float titleHeight = GetTitleHeight(titleAttr);

            Rect titleRect = new Rect(position.x, currentY, position.width, titleHeight);

            // Create title style
            GUIStyle titleStyle = CreateTitleStyle(titleAttr);

            // Draw the title
            GUI.Label(titleRect, titleAttr.Text, titleStyle);

            // Draw separator line if enabled
            if (titleAttr.DrawLine)
            {
                float separatorY = currentY + titleHeight + 2f;
                Rect separatorRect = new Rect(position.x, separatorY, position.width, 1f);

                Color originalColor = GUI.color;
                GUI.color = GetSeparatorColor(titleAttr.Color);
                GUI.DrawTexture(separatorRect, EditorGUIUtility.whiteTexture);
                GUI.color = originalColor;
            }
        }

        public override float GetHeight()
        {
            TitleAttribute titleAttr = (TitleAttribute)attribute;

            float height = titleAttr.SpaceAbove + titleAttr.SpaceBelow;
            height += GetTitleHeight(titleAttr);

            if (titleAttr.DrawLine)
            {
                height += 3f; // Space for separator line
            }

            return height;
        }

        /// <summary>
        /// Creates a GUIStyle for the title based on attribute settings
        /// </summary>
        private GUIStyle CreateTitleStyle(TitleAttribute titleAttr)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = titleAttr.FontSize,
                fontStyle = titleAttr.Bold ? FontStyle.Bold : FontStyle.Normal,
                normal = { textColor = GetTextColor(titleAttr.Color) },
                wordWrap = true
            };

            return style;
        }

        /// <summary>
        /// Gets the height needed for the title text
        /// </summary>
        private float GetTitleHeight(TitleAttribute titleAttr)
        {
            // Simple height calculation without GUI access
            // This provides a reasonable estimate without requiring OnGUI context
            float baseHeight = titleAttr.FontSize + 4f;

            // Add extra height for potential line wrapping
            if (!string.IsNullOrEmpty(titleAttr.Text) && titleAttr.Text.Length > 40)
            {
                baseHeight += titleAttr.FontSize * 0.5f; // Add space for potential wrap
            }

            return baseHeight;
        }

        /// <summary>
        /// Converts color string to Unity Color
        /// </summary>
        private Color GetTextColor(string colorName)
        {
            return colorName.ToLower() switch
            {
                "white" => Color.white,
                "black" => Color.black,
                "gray" or "grey" => Color.gray,
                "red" => Color.red,
                "green" => Color.green,
                "blue" => Color.blue,
                "yellow" => Color.yellow,
                "cyan" => Color.cyan,
                "magenta" => Color.magenta,
                _ => EditorGUIUtility.isProSkin ? Color.white : Color.black
            };
        }

        /// <summary>
        /// Gets a dimmed version of the text color for the separator
        /// </summary>
        private Color GetSeparatorColor(string colorName)
        {
            Color baseColor = GetTextColor(colorName);
            return new Color(baseColor.r, baseColor.g, baseColor.b, 0.3f);
        }
    }
}