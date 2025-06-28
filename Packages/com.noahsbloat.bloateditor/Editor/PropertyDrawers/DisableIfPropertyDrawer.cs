using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// 🚫 Property drawer for DisableIf attribute - handles conditional field disabling
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisableIfAttribute disableIf = (DisableIfAttribute)attribute;

            bool shouldDisable = EvaluateCondition(property, disableIf.Condition);

            if (disableIf.Invert)
                shouldDisable = !shouldDisable;

            bool wasEnabled = GUI.enabled;
            GUI.enabled = wasEnabled && !shouldDisable;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <summary>
        /// 🔍 Evaluates the condition string and returns the result
        /// </summary>
        private bool EvaluateCondition(SerializedProperty property, string condition)
        {
            try
            {
                // Handle simple boolean field references (e.g., "boolField")
                if (IsSimpleBooleanReference(condition))
                {
                    return GetBooleanValue(property, condition.Trim());
                }

                // Handle boolean comparisons (e.g., "boolField == true")
                if (TryParseBooleanComparison(condition, out string boolField, out bool expectedValue))
                {
                    bool fieldValue = GetBooleanValue(property, boolField);
                    return fieldValue == expectedValue;
                }

                // Handle enum comparisons (e.g., "enumField == EnumType.Value")
                if (TryParseEnumComparison(condition, out string enumField, out string enumValue))
                {
                    return CompareEnumValue(property, enumField, enumValue);
                }

                // Handle numeric comparisons (e.g., "intField > 5", "floatField <= 10.5")
                if (TryParseNumericComparison(condition, out string numericField, out string op, out float numericValue))
                {
                    return CompareNumericValue(property, numericField, op, numericValue);
                }

                Debug.LogWarning($"🚨 BloatEditor: Unsupported condition format: '{condition}'");
                return false; // Default to enabled if condition can't be parsed
            }
            catch (Exception ex)
            {
                Debug.LogError($"🚨 BloatEditor: Error evaluating condition '{condition}': {ex.Message}");
                return false; // Default to enabled on error
            }
        }

        /// <summary>
        /// 🔍 Checks if the condition is a simple boolean field reference
        /// </summary>
        private bool IsSimpleBooleanReference(string condition)
        {
            return !condition.Contains("==") && !condition.Contains("!=") &&
                   !condition.Contains("true") && !condition.Contains("false") &&
                   !condition.Contains(">") && !condition.Contains("<");
        }

        /// <summary>
        /// 🔍 Parses boolean comparison expressions like "field == true" or "field == false"
        /// </summary>
        private bool TryParseBooleanComparison(string condition, out string fieldName, out bool expectedValue)
        {
            fieldName = null;
            expectedValue = false;

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

            var match = Regex.Match(condition.Trim(), @"^(\w+)\s*==\s*(\w+\.\w+|\w+)$");
            if (match.Success)
            {
                fieldName = match.Groups[1].Value;
                enumValue = match.Groups[2].Value;

                // Make sure it's not a boolean comparison
                if (enumValue.ToLower() != "true" && enumValue.ToLower() != "false")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 🔍 Parses numeric comparison expressions like "intField > 5" or "floatField <= 10.5"
        /// </summary>
        private bool TryParseNumericComparison(string condition, out string fieldName, out string op, out float numericValue)
        {
            fieldName = null;
            op = null;
            numericValue = 0f;

            var match = Regex.Match(condition.Trim(), @"^(\w+)\s*(>=|<=|>|<|==|!=)\s*([+-]?\d*\.?\d+)$");
            if (match.Success)
            {
                fieldName = match.Groups[1].Value;
                op = match.Groups[2].Value;
                if (float.TryParse(match.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue))
                {
                    return true;
                }
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

                string expectedEnumName = enumValue.Contains(".") ?
                    enumValue.Substring(enumValue.LastIndexOf('.') + 1) : enumValue;

                return string.Equals(currentEnumName, expectedEnumName, StringComparison.OrdinalIgnoreCase);
            }

            Debug.LogWarning($"🚨 BloatEditor: Enum field '{fieldName}' not found or not an enum type");
            return false;
        }

        /// <summary>
        /// 🔢 Compares a numeric field value with the expected value using the specified operator
        /// </summary>
        private bool CompareNumericValue(SerializedProperty property, string fieldName, string op, float expectedValue)
        {
            SerializedProperty targetProperty = GetSiblingProperty(property, fieldName);
            if (targetProperty == null)
            {
                Debug.LogWarning($"🚨 BloatEditor: Numeric field '{fieldName}' not found");
                return false;
            }

            float fieldValue = GetNumericPropertyValue(targetProperty);

            return op switch
            {
                ">" => fieldValue > expectedValue,
                "<" => fieldValue < expectedValue,
                ">=" => fieldValue >= expectedValue,
                "<=" => fieldValue <= expectedValue,
                "==" => Mathf.Approximately(fieldValue, expectedValue),
                "!=" => !Mathf.Approximately(fieldValue, expectedValue),
                _ => false
            };
        }

        /// <summary>
        /// 🔢 Gets the numeric value from a SerializedProperty
        /// </summary>
        private float GetNumericPropertyValue(SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.Boolean => property.boolValue ? 1f : 0f,
                _ => 0f
            };
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