using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// 📁 Property drawer for CopyFileName attribute - Enables drag & drop file names
    /// </summary>
    [CustomPropertyDrawer(typeof(CopyFileNameAttribute))]
    public class CopyFileNamePropertyDrawer : PropertyDrawer
    {
        private const float SPACING = 2f;
        private bool _isDragHovering = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "CopyFileName can only be used with string fields");
                return;
            }

            CopyFileNameAttribute copyAttr = (CopyFileNameAttribute)attribute;

            // Calculate layout
            float dropZoneHeight = copyAttr.ShowDropZone ? copyAttr.DropZoneHeight : 0f;
            float stringFieldHeight = EditorGUIUtility.singleLineHeight;

            Rect stringFieldRect = new Rect(position.x, position.y, position.width, stringFieldHeight);
            Rect dropZoneRect = new Rect(position.x, position.y + stringFieldHeight + SPACING,
                position.width, dropZoneHeight);

            // Draw the string field
            EditorGUI.PropertyField(stringFieldRect, property, label, true);

            // Draw drop zone if enabled
            if (copyAttr.ShowDropZone)
            {
                DrawDropZone(dropZoneRect, copyAttr);
            }

            // Handle drag and drop for both areas
            Rect totalArea = copyAttr.ShowDropZone ?
                new Rect(position.x, position.y, position.width, stringFieldHeight + SPACING + dropZoneHeight) :
                stringFieldRect;

            HandleDragAndDrop(totalArea, property, copyAttr);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            CopyFileNameAttribute copyAttr = (CopyFileNameAttribute)attribute;

            float height = EditorGUIUtility.singleLineHeight; // String field

            if (copyAttr.ShowDropZone)
            {
                height += SPACING + copyAttr.DropZoneHeight; // Drop zone
            }

            return height;
        }

        /// <summary>
        /// Draws the visual drop zone area
        /// </summary>
        private void DrawDropZone(Rect rect, CopyFileNameAttribute copyAttr)
        {
            // Get colors based on drag state
            Color backgroundColor = GetDropZoneBackgroundColor(copyAttr.DropZoneColor, _isDragHovering);
            Color borderColor = GetDropZoneBorderColor(copyAttr.DropZoneColor, _isDragHovering);
            Color textColor = GetDropZoneTextColor(_isDragHovering);

            // Draw background
            EditorGUI.DrawRect(rect, backgroundColor);

            // Draw dashed border
            DrawDashedBorder(rect, borderColor, _isDragHovering ? 2f : 1f);

            // Draw help text
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                fontStyle = FontStyle.Italic,
                normal = { textColor = textColor }
            };

            string displayText = _isDragHovering ? "Drop to copy filename" : copyAttr.HelpText;
            GUI.Label(rect, displayText, textStyle);
        }

        /// <summary>
        /// Handles the drag and drop functionality
        /// </summary>
        private void HandleDragAndDrop(Rect area, SerializedProperty property, CopyFileNameAttribute copyAttr)
        {
            Event currentEvent = Event.current;
            bool containsMouse = area.Contains(currentEvent.mousePosition);

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                    if (containsMouse)
                    {
                        bool canAccept = CanAcceptDraggedObjects(copyAttr);
                        DragAndDrop.visualMode = canAccept ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

                        if (canAccept && !_isDragHovering)
                        {
                            _isDragHovering = true;
                            EditorWindow.focusedWindow?.Repaint();
                        }
                        else if (!canAccept && _isDragHovering)
                        {
                            _isDragHovering = false;
                            EditorWindow.focusedWindow?.Repaint();
                        }

                        currentEvent.Use();
                    }
                    break;

                case EventType.DragPerform:
                    if (containsMouse && CanAcceptDraggedObjects(copyAttr))
                    {
                        DragAndDrop.AcceptDrag();

                        // Process the first acceptable file
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(draggedObject);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                string fileName = ProcessFileName(assetPath, copyAttr);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    property.stringValue = fileName;
                                    property.serializedObject.ApplyModifiedProperties();
                                    break;
                                }
                            }
                        }

                        _isDragHovering = false;
                        currentEvent.Use();
                        EditorWindow.focusedWindow?.Repaint();
                    }
                    break;

                case EventType.DragExited:
                    if (_isDragHovering)
                    {
                        _isDragHovering = false;
                        EditorWindow.focusedWindow?.Repaint();
                    }
                    break;

                case EventType.MouseMove:
                    if (_isDragHovering && !containsMouse)
                    {
                        _isDragHovering = false;
                        EditorWindow.focusedWindow?.Repaint();
                    }
                    break;
            }
        }

        /// <summary>
        /// Checks if the currently dragged objects can be accepted
        /// </summary>
        private bool CanAcceptDraggedObjects(CopyFileNameAttribute copyAttr)
        {
            if (DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0)
                return false;

            // Check if at least one object is acceptable
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(assetPath)) continue;

                // Check file extension if filter is specified
                if (copyAttr.AllowedExtensions.Length > 0)
                {
                    string extension = Path.GetExtension(assetPath).ToLower();
                    if (!copyAttr.AllowedExtensions.Any(ext => ext.ToLower() == extension))
                        continue;
                }

                return true; // At least one acceptable file found
            }

            return false;
        }

        /// <summary>
        /// Processes the file name according to the attribute settings
        /// </summary>
        private string ProcessFileName(string assetPath, CopyFileNameAttribute copyAttr)
        {
            // Get the file name
            string fileName = copyAttr.IncludeExtension ?
                Path.GetFileName(assetPath) :
                Path.GetFileNameWithoutExtension(assetPath);

            // Apply prefix and suffix
            if (!string.IsNullOrEmpty(copyAttr.Prefix))
                fileName = copyAttr.Prefix + fileName;

            if (!string.IsNullOrEmpty(copyAttr.Suffix))
                fileName = fileName + copyAttr.Suffix;

            // Apply case conversions
            if (copyAttr.ToCamelCase)
                fileName = ToCamelCase(fileName);
            else if (copyAttr.ToPascalCase)
                fileName = ToPascalCase(fileName);

            return fileName;
        }

        /// <summary>
        /// Converts string to camelCase
        /// </summary>
        private string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Handle snake_case and kebab-case
            string result = Regex.Replace(input, @"[_-]+(.)", match => match.Groups[1].Value.ToUpper());

            // Make first character lowercase
            return char.ToLower(result[0]) + result.Substring(1);
        }

        /// <summary>
        /// Converts string to PascalCase
        /// </summary>
        private string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Handle snake_case and kebab-case
            string result = Regex.Replace(input, @"[_-]+(.)", match => match.Groups[1].Value.ToUpper());

            // Make first character uppercase
            return char.ToUpper(result[0]) + result.Substring(1);
        }

        /// <summary>
        /// Draws a dashed border around the rectangle
        /// </summary>
        private void DrawDashedBorder(Rect rect, Color color, float thickness)
        {
            Color originalColor = GUI.color;
            GUI.color = color;

            // Draw dashed lines by drawing small segments
            float dashSize = 4f;
            float gapSize = 3f;
            float segmentSize = dashSize + gapSize;

            // Top border
            for (float x = rect.x; x < rect.x + rect.width; x += segmentSize)
            {
                float width = Mathf.Min(dashSize, rect.x + rect.width - x);
                GUI.DrawTexture(new Rect(x, rect.y, width, thickness), EditorGUIUtility.whiteTexture);
            }

            // Bottom border
            for (float x = rect.x; x < rect.x + rect.width; x += segmentSize)
            {
                float width = Mathf.Min(dashSize, rect.x + rect.width - x);
                GUI.DrawTexture(new Rect(x, rect.y + rect.height - thickness, width, thickness), EditorGUIUtility.whiteTexture);
            }

            // Left border
            for (float y = rect.y; y < rect.y + rect.height; y += segmentSize)
            {
                float height = Mathf.Min(dashSize, rect.y + rect.height - y);
                GUI.DrawTexture(new Rect(rect.x, y, thickness, height), EditorGUIUtility.whiteTexture);
            }

            // Right border
            for (float y = rect.y; y < rect.y + rect.height; y += segmentSize)
            {
                float height = Mathf.Min(dashSize, rect.y + rect.height - y);
                GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, y, thickness, height), EditorGUIUtility.whiteTexture);
            }

            GUI.color = originalColor;
        }

        /// <summary>
        /// Gets the background color for the drop zone
        /// </summary>
        private Color GetDropZoneBackgroundColor(string colorTheme, bool isHovering)
        {
            Color baseColor = GetThemeColor(colorTheme);

            if (isHovering)
            {
                return new Color(baseColor.r, baseColor.g, baseColor.b, 0.15f);
            }
            else
            {
                return new Color(baseColor.r, baseColor.g, baseColor.b, 0.05f);
            }
        }

        /// <summary>
        /// Gets the border color for the drop zone
        /// </summary>
        private Color GetDropZoneBorderColor(string colorTheme, bool isHovering)
        {
            Color baseColor = GetThemeColor(colorTheme);

            if (isHovering)
            {
                return new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
            }
            else
            {
                return new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f);
            }
        }

        /// <summary>
        /// Gets the text color for the drop zone
        /// </summary>
        private Color GetDropZoneTextColor(bool isHovering)
        {
            if (isHovering)
            {
                return EditorGUIUtility.isProSkin ? Color.white : Color.black;
            }
            else
            {
                return EditorGUIUtility.isProSkin ?
                    new Color(0.7f, 0.7f, 0.7f, 1f) :
                    new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }

        /// <summary>
        /// Gets the theme color for the specified color name
        /// </summary>
        private Color GetThemeColor(string colorTheme)
        {
            return colorTheme?.ToLower() switch
            {
                "blue" => new Color(0.2f, 0.5f, 1f, 1f),
                "green" => new Color(0.2f, 0.8f, 0.2f, 1f),
                "red" => new Color(1f, 0.3f, 0.3f, 1f),
                "yellow" => new Color(1f, 0.8f, 0.2f, 1f),
                "purple" => new Color(0.7f, 0.3f, 1f, 1f),
                "orange" => new Color(1f, 0.6f, 0.2f, 1f),
                "cyan" => new Color(0.2f, 0.8f, 1f, 1f),
                "magenta" => new Color(1f, 0.3f, 0.8f, 1f),
                _ => new Color(0.2f, 0.5f, 1f, 1f) // Default blue
            };
        }
    }
}