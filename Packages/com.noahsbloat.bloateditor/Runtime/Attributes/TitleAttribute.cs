using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// Title Attribute - Enhanced header replacement that works with all field types
    /// Provides better styling and universal compatibility
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TitleAttribute : PropertyAttribute
    {
        /// <summary>
        /// The title text to display
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Font size for the title (default: 14)
        /// </summary>
        public int FontSize { get; set; } = 14;

        /// <summary>
        /// Whether to make the title bold (default: true)
        /// </summary>
        public bool Bold { get; set; } = true;

        /// <summary>
        /// Whether to add a separator line below the title (default: true)
        /// </summary>
        public bool DrawLine { get; set; } = true;

        /// <summary>
        /// Color of the title text (default: "white")
        /// Supports: "white", "black", "gray", "red", "green", "blue", "yellow", "cyan", "magenta"
        /// </summary>
        public string Color { get; set; } = "white";

        /// <summary>
        /// Additional spacing above the title (default: 10)
        /// </summary>
        public float SpaceAbove { get; set; } = 10f;

        /// <summary>
        /// Additional spacing below the title (default: 5)
        /// </summary>
        public float SpaceBelow { get; set; } = 5f;

        public TitleAttribute(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}