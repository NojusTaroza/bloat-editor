using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// Property drawer for Gap attribute - enhanced spacing with optional line
    /// </summary>
    [CustomPropertyDrawer(typeof(GapAttribute))]
    public class GapPropertyDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            GapAttribute gapAttr = (GapAttribute)attribute;

            // Only draw a line if DrawLine is enabled
            if (gapAttr.DrawLine)
            {
                // Calculate line position (centered in the gap)
                float lineY = position.y + (gapAttr.Size * 0.5f) - (gapAttr.LineThickness * 0.5f);
                Rect lineRect = new Rect(position.x + 20f, lineY, position.width - 40f, gapAttr.LineThickness);

                // Draw the line
                Color originalColor = GUI.color;
                GUI.color = GetColor(gapAttr.Color);
                GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture);
                GUI.color = originalColor;
            }
        }

        public override float GetHeight()
        {
            GapAttribute gapAttr = (GapAttribute)attribute;
            return gapAttr.Size;
        }

        /// <summary>
        /// Converts color string to Unity Color for the line
        /// </summary>
        private Color GetColor(string colorName)
        {
            Color baseColor = colorName.ToLower() switch
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
                _ => Color.gray
            };

            // Make the line semi-transparent for a subtle effect
            return new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f);
        }
    }
}