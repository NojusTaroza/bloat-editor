using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// 🎨 Property drawer for ShowIf attribute - handles conditional field visibility
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            if (ShouldShowProperty(property, showIf))
            {
                if (showIf.DisablingType == DisablingType.ReadOnly && !EvaluateCondition(property, showIf.Condition))
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label, true);
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            if (ShouldShowProperty(property, showIf))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        /// <summary>
        /// 🎯 Determines if the property should be shown based on the ShowIf condition
        /// </summary>
        private bool ShouldShowProperty(SerializedProperty property, ShowIfAttribute showIf)
        {
            bool conditionResult = EvaluateCondition(property, showIf.Condition);

            if (showIf.Invert)
                conditionResult = !conditionResult;

            return showIf.DisablingType == DisablingType.ReadOnly || conditionResult;
        }

        /// <summary>
        /// 🔍 Evaluates the condition string and returns the result
        /// </summary>
        private bool EvaluateCondition(SerializedProperty property, string condition)
        {
            try
            {
                // 🔹 Handle simple boolean field references (e.g., "boolField")
                if (IsSimpleBooleanReference(condition))
                {
                    return GetBooleanValue(property, condition.Trim());
                }

                // 🔹 Handle boolean comparisons (e.g., "boolField == true")
                if (TryParseBooleanComparison(condition, out string boolField, out bool expectedValue))
                {
                    bool fieldValue = GetBooleanValue(property, boolField);
                    return fieldValue == expectedValue;
                }

                // 🔹 Handle enum comparisons (e.g., "enumField == EnumType.Value")
                if (TryParseEnumComparison(condition, out string enumField, out string enumValue))
                {
                    return CompareEnumValue(property, enumField, enumValue);
                }

                Debug.LogWarning($"🚨 BloatEditor: Unsupported condition format: '{condition}'");
                return true; // Default to showing the field if condition can't be parsed
            }
            catch (Exception ex)
            {
                Debug.LogError($"🚨 BloatEditor: Error evaluating condition '{condition}': {ex.Message}");
                return true; // Default to showing the field on error
            }
        }

        /// <summary>
        /// 🔍 Checks if the condition is a simple boolean field reference
        /// </summary>
        private bool IsSimpleBooleanReference(string condition)
        {
            return !condition.Contains("==") && !condition.Contains("!=") &&
                   !condition.Contains("true") && !condition.Contains("false");
        }

        /// <summary>
        /// 🔍 Parses boolean comparison expressions like "field == true" or "field == false"
        /// </summary>
        private bool TryParseBooleanComparison(string condition, out string fieldName, out bool expectedValue)
        {
            fieldName = null;
            expectedValue = false;

            // Match patterns like "fieldName == true" or "fieldName == false"
            var match = Regex.Match(condition.Trim(), @"^(\w+)\s*==\s*(true|false)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                fieldName = match.Groups[1].Value;
                expectedValue = bool.Parse(match.Groups[2].Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 🔍 Parses enum comparison expressions like "enumField == EnumType.Value"
        /// </summary>
        private bool TryParseEnumComparison(string condition, out string fieldName, out string enumValue)
        {
            fieldName = null;
            enumValue = null;

            // Match patterns like "fieldName == EnumType.Value"
            var match = Regex.Match(condition.Trim(), @"^(\w+)\s*==\s*(\w+\.\w+|\w+)$");
            if (match.Success)
            {
                fieldName = match.Groups[1].Value;
                enumValue = match.Groups[2].Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 📊 Gets the boolean value of a field in the same object
        /// </summary>
        private bool GetBooleanValue(SerializedProperty property, string fieldName)
        {
            SerializedProperty targetProperty = GetSiblingProperty(property, fieldName);
            if (targetProperty != null && targetProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return targetProperty.boolValue;
            }

            Debug.LogWarning($"🚨 BloatEditor: Boolean field '{fieldName}' not found or not a boolean type");
            return false;
        }

        /// <summary>
        /// 🎯 Compares an enum field value with the expected enum value
        /// </summary>
        private bool CompareEnumValue(SerializedProperty property, string fieldName, string enumValue)
        {
            SerializedProperty targetProperty = GetSiblingProperty(property, fieldName);
            if (targetProperty != null && targetProperty.propertyType == SerializedPropertyType.Enum)
            {
                string currentEnumName = targetProperty.enumNames[targetProperty.enumValueIndex];

                // Handle both "EnumType.Value" and "Value" formats
                string expectedEnumName = enumValue.Contains(".") ?
                    enumValue.Substring(enumValue.LastIndexOf('.') + 1) : enumValue;

                return string.Equals(currentEnumName, expectedEnumName, StringComparison.OrdinalIgnoreCase);
            }

            Debug.LogWarning($"🚨 BloatEditor: Enum field '{fieldName}' not found or not an enum type");
            return false;
        }

        /// <summary>
        /// 🔗 Gets a sibling property (field in the same object) by name
        /// </summary>
        private SerializedProperty GetSiblingProperty(SerializedProperty property, string fieldName)
        {
            string propertyPath = property.propertyPath;
            string parentPath = propertyPath.Contains(".") ?
                propertyPath.Substring(0, propertyPath.LastIndexOf('.')) : "";

            string targetPath = string.IsNullOrEmpty(parentPath) ? fieldName : $"{parentPath}.{fieldName}";

            return property.serializedObject.FindProperty(targetPath);
        }
    }
}