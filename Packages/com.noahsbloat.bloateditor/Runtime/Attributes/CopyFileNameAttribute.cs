using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// 📁 CopyFileName Attribute - Enables drag & drop file names onto string fields
    /// Automatically extracts and copies the file name when assets are dropped
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CopyFileNameAttribute : PropertyAttribute
    {
        /// <summary>
        /// Whether to include the file extension (default: false)
        /// </summary>
        public bool IncludeExtension { get; set; } = false;

        /// <summary>
        /// Whether to show a visual drop zone indicator (default: true)
        /// </summary>
        public bool ShowDropZone { get; set; } = true;

        /// <summary>
        /// Color of the drop zone border when hovering (default: "blue")
        /// Supports: "blue", "green", "red", "yellow", "purple", "orange", "cyan", "magenta"
        /// </summary>
        public string DropZoneColor { get; set; } = "blue";

        /// <summary>
        /// Custom suffix to append to the file name (optional)
        /// Example: "Controller" -> "PlayerController" if suffix is "Controller"
        /// </summary>
        public string Suffix { get; set; } = "";

        /// <summary>
        /// Custom prefix to prepend to the file name (optional)
        /// Example: "Player" -> "PlayerScript" if prefix is "Script"
        /// </summary>
        public string Prefix { get; set; } = "";

        /// <summary>
        /// Whether to convert the result to camelCase (default: false)
        /// Example: "my_script" -> "myScript"
        /// </summary>
        public bool ToCamelCase { get; set; } = false;

        /// <summary>
        /// Whether to convert the result to PascalCase (default: false)
        /// Example: "my_script" -> "MyScript"
        /// </summary>
        public bool ToPascalCase { get; set; } = false;

        /// <summary>
        /// List of file extensions to filter (empty = accept all)
        /// Example: new[] { ".cs", ".js", ".txt" }
        /// </summary>
        public string[] AllowedExtensions { get; set; } = new string[0];

        /// <summary>
        /// Help text to show in the drop zone (default: "Drag file here")
        /// </summary>
        public string HelpText { get; set; } = "Drag file here";

        /// <summary>
        /// Height of the drop zone area in pixels (default: 20)
        /// </summary>
        public float DropZoneHeight { get; set; } = 20f;

        public CopyFileNameAttribute()
        {
        }

        public CopyFileNameAttribute(bool includeExtension)
        {
            IncludeExtension = includeExtension;
        }
    }
}