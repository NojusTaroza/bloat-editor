using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// Gap Attribute - Enhanced spacing replacement that works with all field types
    /// Provides precise control over spacing and visual separation
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class GapAttribute : PropertyAttribute
    {
        /// <summary>
        /// The amount of space to add in pixels (default: 20)
        /// </summary>
        public float Size { get; }

        /// <summary>
        /// Whether to draw a subtle line in the gap (default: false)
        /// </summary>
        public bool DrawLine { get; set; } = false;

        /// <summary>
        /// Color of the line if ShowLine is true (default: "gray")
        /// Supports: "white", "black", "gray", "red", "green", "blue", "yellow", "cyan", "magenta"
        /// </summary>
        public string Color { get; set; } = "gray";

        /// <summary>
        /// Thickness of the line in pixels (default: 1)
        /// </summary>
        public float LineThickness { get; set; } = 1f;

        /// <summary>
        /// Creates a gap with the specified size
        /// </summary>
        /// <param name="size">Size of the gap in pixels</param>
        public GapAttribute(float size = 20f)
        {
            Size = Mathf.Max(0f, size);
        }
    }
}