using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// 📁 Enhanced Tab Attribute - Self-contained tabbed organization system
    /// Creates tabs dynamically based on usage, with intelligent color inheritance
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TabAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the tab group this field belongs to
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// The name of the specific tab within the group
        /// </summary>
        public string TabName { get; }

        /// <summary>
        /// Color theme for this tab (default: inherits from previous tab in group)
        /// Supports: "default", "blue", "green", "red", "yellow", "purple", "orange", "cyan", "magenta"
        /// </summary>
        public string Color { get; set; } = null;

        /// <summary>
        /// Display order within the tab group (lower numbers appear first)
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Whether this field should be indented within the tab (default: false)
        /// </summary>
        public bool Indent { get; set; } = false;

        /// <summary>
        /// Height of the tab buttons in pixels (default: 28)
        /// </summary>
        public float TabHeight { get; set; } = 28f;

        /// <summary>
        /// Font size for tab text (default: 10)
        /// </summary>
        public int FontSize { get; set; } = 10;

        /// <summary>
        /// Whether to show a separator line above the tab group (default: true)
        /// </summary>
        public bool ShowSeparator { get; set; } = true;

        /// <summary>
        /// Additional spacing above the tab group (default: 10)
        /// </summary>
        public float SpaceAbove { get; set; } = 10f;

        /// <summary>
        /// Additional spacing below the tab group (default: 5)
        /// </summary>
        public float SpaceBelow { get; set; } = 5f;

        /// <summary>
        /// Whether to make the tab group title bold (default: true)
        /// </summary>
        public bool BoldGroupTitle { get; set; } = true;

        /// <summary>
        /// Creates a Tab attribute with the specified group and tab names
        /// </summary>
        /// <param name="groupName">Name of the tab group</param>
        /// <param name="tabName">Name of the specific tab</param>
        public TabAttribute(string groupName, string tabName)
        {
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
            TabName = tabName ?? throw new ArgumentNullException(nameof(tabName));
        }
    }
}