using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// Preview Attribute - Displays visual preview of sprites, GameObjects, and other Unity objects
    /// Shows a thumbnail preview with object information in the inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PreviewAttribute : PropertyAttribute
    {
        /// <summary>
        /// Width of the preview area in pixels (default: 120)
        /// </summary>
        public float Width { get; set; } = 120f;

        /// <summary>
        /// Height of the preview area in pixels (default: 120)
        /// </summary>
        public float Height { get; set; } = 120f;

        /// <summary>
        /// Whether to show the object name below the preview (default: true)
        /// </summary>
        public bool ShowName { get; set; } = true;

        /// <summary>
        /// Whether to show object type information (default: true)
        /// </summary>
        public bool ShowType { get; set; } = true;

        /// <summary>
        /// Background color for the preview area (default: "gray")
        /// Supports: "white", "black", "gray", "darkgray", "clear"
        /// </summary>
        public string BackgroundColor { get; set; } = "gray";

        /// <summary>
        /// Whether to show a border around the preview (default: true)
        /// </summary>
        public bool ShowBorder { get; set; } = true;

        /// <summary>
        /// Additional spacing around the preview (default: 5)
        /// </summary>
        public float Padding { get; set; } = 5f;

        public PreviewAttribute()
        {
        }

        public PreviewAttribute(float width, float height)
        {
            Width = Mathf.Max(50f, width);
            Height = Mathf.Max(50f, height);
        }
    }
}