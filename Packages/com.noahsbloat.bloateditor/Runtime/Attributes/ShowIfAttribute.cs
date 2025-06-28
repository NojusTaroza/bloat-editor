using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// 🎨 ShowIf Attribute - Conditionally shows/hides fields in the inspector
    /// Supports boolean fields and enum comparisons
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// The condition expression to evaluate
        /// Examples:
        /// - "boolFieldName" 
        /// - "boolField == true"
        /// - "boolField == false"
        /// - "enumField == EnumType.Value"
        /// </summary>
        public string Condition { get; }

        /// <summary>
        /// Whether to invert the condition result
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// Action to take when condition is false
        /// </summary>
        public DisablingType DisablingType { get; set; } = DisablingType.DontDraw;

        public ShowIfAttribute(string condition)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }
    }

    /// <summary>
    /// 🎯 Defines how to handle fields when the ShowIf condition is false
    /// </summary>
    public enum DisablingType
    {
        /// <summary>Don't draw the field at all</summary>
        DontDraw,
        /// <summary>Draw the field but make it read-only</summary>
        ReadOnly
    }
}