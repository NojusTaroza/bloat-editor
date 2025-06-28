using System;
using UnityEngine;

namespace BloatEditor
{
    /// <summary>
    /// 🚫 DisableIf Attribute - Conditionally disables fields in the inspector
    /// Supports boolean, enum, and numeric comparisons
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DisableIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// The condition expression to evaluate
        /// Examples:
        /// - "boolField" (disable when true)
        /// - "boolField == false" (disable when false)
        /// - "enumField == EnumType.Value"
        /// - "intField > 5" (disable when greater than 5)
        /// - "floatField <= 10.5" (disable when less than or equal to 10.5)
        /// </summary>
        public string Condition { get; }

        /// <summary>
        /// Whether to invert the condition result
        /// </summary>
        public bool Invert { get; set; }

        public DisableIfAttribute(string condition)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }
    }
}