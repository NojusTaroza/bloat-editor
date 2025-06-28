using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// Property drawer for Preview attribute - displays visual previews alongside property fields
    /// </summary>
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public class PreviewPropertyDrawer : PropertyDrawer
    {
        private const float LABEL_HEIGHT = 18f;
        private const float SPACING = 4f;
        private const float PREVIEW_SELECTOR_GAP = 8f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PreviewAttribute previewAttr = (PreviewAttribute)attribute;
            Object targetObject = property.objectReferenceValue;

            if (targetObject == null)
            {
                // If no object assigned, just draw the normal property field
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            // Calculate layout dimensions
            float previewWidth = previewAttr.Width + (previewAttr.Padding * 2);
            float previewHeight = previewAttr.Height + (previewAttr.Padding * 2);

            // Right side area for name and selector - ensure it stays in column 2
            float rightAreaX = position.x + previewWidth + PREVIEW_SELECTOR_GAP;
            float rightAreaWidth = position.width - previewWidth - PREVIEW_SELECTOR_GAP;

            // Draw preview background and border (Column 1)
            Rect previewBackgroundRect = new Rect(position.x, position.y, previewWidth, previewHeight);
            DrawPreviewBackground(previewBackgroundRect, previewAttr);

            // Draw the actual preview (Column 1)
            Rect previewRect = new Rect(
                position.x + previewAttr.Padding,
                position.y + previewAttr.Padding,
                previewAttr.Width,
                previewAttr.Height
            );
            DrawObjectPreview(previewRect, targetObject, previewAttr);

            // Column 2: Stack everything vertically in the right area
            float currentRightY = position.y;

            // 1. Variable name at top of column 2
            if (previewAttr.ShowName)
            {
                Rect nameRect = new Rect(rightAreaX, currentRightY, rightAreaWidth, LABEL_HEIGHT);
                DrawVariableName(nameRect, label.text);
                currentRightY += LABEL_HEIGHT + 2f;
            }

            // 2. Object selector in column 2
            Rect selectorRect = new Rect(rightAreaX, currentRightY, rightAreaWidth, EditorGUIUtility.singleLineHeight);

            // Use PropertyField with empty label to prevent column shift
            GUIContent emptyLabel = new GUIContent("");
            EditorGUI.PropertyField(selectorRect, property, emptyLabel, true);

            currentRightY += EditorGUIUtility.singleLineHeight + 2f;

            // 3. Object type at bottom of column 2
            if (previewAttr.ShowType)
            {
                Rect typeRect = new Rect(rightAreaX, currentRightY, rightAreaWidth, LABEL_HEIGHT);
                DrawObjectType(typeRect, targetObject);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            PreviewAttribute previewAttr = (PreviewAttribute)attribute;
            Object targetObject = property.objectReferenceValue;

            if (targetObject == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            // Calculate height needed
            float previewHeight = previewAttr.Height + (previewAttr.Padding * 2);

            // Calculate height needed for right side content (stack vertically)
            float rightSideHeight = 0f;

            if (previewAttr.ShowName)
                rightSideHeight += LABEL_HEIGHT + 2f; // Name at top

            rightSideHeight += EditorGUIUtility.singleLineHeight + 2f; // Selector in middle

            if (previewAttr.ShowType)
                rightSideHeight += LABEL_HEIGHT; // Type at bottom

            // Return the maximum of preview height or right side content height
            return Mathf.Max(previewHeight, rightSideHeight);
        }

        /// <summary>
        /// Draws the background for the preview area
        /// </summary>
        private void DrawPreviewBackground(Rect rect, PreviewAttribute previewAttr)
        {
            Color backgroundColor = GetBackgroundColor(previewAttr.BackgroundColor);
            Color originalColor = GUI.backgroundColor;

            GUI.backgroundColor = backgroundColor;
            GUI.Box(rect, "", GUI.skin.box);

            if (previewAttr.ShowBorder)
            {
                Color borderColor = EditorGUIUtility.isProSkin ?
                    new Color(0.3f, 0.3f, 0.3f, 1f) :
                    new Color(0.6f, 0.6f, 0.6f, 1f);
                DrawBorder(rect, borderColor, 1f);
            }

            GUI.backgroundColor = originalColor;
        }

        /// <summary>
        /// Draws the actual object preview
        /// </summary>
        private void DrawObjectPreview(Rect rect, Object targetObject, PreviewAttribute previewAttr)
        {
            switch (targetObject)
            {
                case Sprite sprite:
                    DrawSpritePreview(rect, sprite);
                    break;
                case Texture2D texture:
                    DrawTexturePreview(rect, texture);
                    break;
                case GameObject gameObject:
                    DrawGameObjectPreview(rect, gameObject);
                    break;
                case Material material:
                    DrawMaterialPreview(rect, material);
                    break;
                default:
                    DrawGenericObjectPreview(rect, targetObject);
                    break;
            }
        }

        /// <summary>
        /// Draws the variable name with styling
        /// </summary>
        private void DrawVariableName(Rect rect, string variableName)
        {
            GUIStyle nameStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            // Convert camelCase to readable format
            string displayName = ObjectNames.NicifyVariableName(variableName);
            GUI.Label(rect, displayName, nameStyle);
        }

        /// <summary>
        /// Draws the object type information
        /// </summary>
        private void DrawObjectType(Rect rect, Object targetObject)
        {
            GUIStyle typeStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 9,
                fontStyle = FontStyle.Italic
            };

            Color originalColor = GUI.color;
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            string typeName = targetObject.GetType().Name;
            GUI.Label(rect, $"Type: {typeName}", typeStyle);

            GUI.color = originalColor;
        }

        /// <summary>
        /// Draws a sprite preview
        /// </summary>
        private void DrawSpritePreview(Rect rect, Sprite sprite)
        {
            if (sprite?.texture != null)
            {
                Rect spriteRect = sprite.rect;
                Rect texCoords = new Rect(
                    spriteRect.x / sprite.texture.width,
                    spriteRect.y / sprite.texture.height,
                    spriteRect.width / sprite.texture.width,
                    spriteRect.height / sprite.texture.height
                );

                GUI.DrawTextureWithTexCoords(rect, sprite.texture, texCoords);
            }
            else
            {
                DrawMissingPreview(rect, "Missing Sprite");
            }
        }

        /// <summary>
        /// Draws a texture preview
        /// </summary>
        private void DrawTexturePreview(Rect rect, Texture2D texture)
        {
            if (texture != null)
            {
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
            }
            else
            {
                DrawMissingPreview(rect, "Missing Texture");
            }
        }

        /// <summary>
        /// Draws a GameObject preview using AssetPreview
        /// </summary>
        private void DrawGameObjectPreview(Rect rect, GameObject gameObject)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(gameObject);
            if (preview != null)
            {
                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }
            else
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(gameObject);
                if (icon != null)
                {
                    Rect iconRect = new Rect(
                        rect.x + rect.width * 0.5f - 16f,
                        rect.y + rect.height * 0.5f - 16f,
                        32f, 32f
                    );
                    GUI.DrawTexture(iconRect, icon);
                }
                else
                {
                    DrawMissingPreview(rect, "GameObject");
                }
            }
        }

        /// <summary>
        /// Draws a material preview
        /// </summary>
        private void DrawMaterialPreview(Rect rect, Material material)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(material);
            if (preview != null)
            {
                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }
            else
            {
                DrawMissingPreview(rect, "Material");
            }
        }

        /// <summary>
        /// Draws a generic object preview using AssetPreview
        /// </summary>
        private void DrawGenericObjectPreview(Rect rect, Object targetObject)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(targetObject);
            if (preview != null)
            {
                GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }
            else
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(targetObject);
                if (icon != null)
                {
                    Rect iconRect = new Rect(
                        rect.x + rect.width * 0.5f - 16f,
                        rect.y + rect.height * 0.5f - 16f,
                        32f, 32f
                    );
                    GUI.DrawTexture(iconRect, icon);
                }
                else
                {
                    DrawMissingPreview(rect, targetObject.GetType().Name);
                }
            }
        }

        /// <summary>
        /// Draws a placeholder when preview is not available
        /// </summary>
        private void DrawMissingPreview(Rect rect, string label)
        {
            Color originalColor = GUI.color;
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                wordWrap = true
            };

            GUI.Label(rect, label, centeredStyle);
            GUI.color = originalColor;
        }

        /// <summary>
        /// Draws a border around the specified rectangle
        /// </summary>
        private void DrawBorder(Rect rect, Color color, float thickness)
        {
            Color originalColor = GUI.color;
            GUI.color = color;

            // Top
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), EditorGUIUtility.whiteTexture);
            // Bottom
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), EditorGUIUtility.whiteTexture);
            // Left
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), EditorGUIUtility.whiteTexture);
            // Right
            GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), EditorGUIUtility.whiteTexture);

            GUI.color = originalColor;
        }

        /// <summary>
        /// Converts color string to Unity Color for the background
        /// </summary>
        private Color GetBackgroundColor(string colorName)
        {
            return colorName.ToLower() switch
            {
                "white" => Color.white,
                "black" => Color.black,
                "gray" => new Color(0.8f, 0.8f, 0.8f, 0.3f),
                "darkgray" => new Color(0.4f, 0.4f, 0.4f, 0.3f),
                "clear" => Color.clear,
                _ => new Color(0.8f, 0.8f, 0.8f, 0.3f)
            };
        }
    }
}