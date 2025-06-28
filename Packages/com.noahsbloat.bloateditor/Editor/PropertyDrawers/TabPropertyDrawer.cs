using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BloatEditor.Editor
{
    /// <summary>
    /// Enhanced Property drawer for Tab attribute - Dynamic tab system with smooth transitions
    /// </summary>
    [CustomPropertyDrawer(typeof(TabAttribute))]
    public class TabPropertyDrawer : PropertyDrawer
    {
        // Static state storage: ObjectID -> GroupName -> ActiveTabIndex
        private static readonly Dictionary<int, Dictionary<string, int>> GroupStates =
            new Dictionary<int, Dictionary<string, int>>();

        // Cache for group information: ObjectID -> GroupName -> GroupInfo
        private static readonly Dictionary<int, Dictionary<string, GroupInfo>> GroupCache =
            new Dictionary<int, Dictionary<string, GroupInfo>>();

        // Height cache for smooth transitions: ObjectID -> GroupName -> TabName -> TotalHeight
        private static readonly Dictionary<int, Dictionary<string, Dictionary<string, float>>> HeightCache =
            new Dictionary<int, Dictionary<string, Dictionary<string, float>>>();

        private class GroupInfo
        {
            public List<TabInfo> Tabs = new List<TabInfo>();
            public TabAttribute FirstTabAttribute; // For styling configuration
            public bool HasBeenDrawn; // Track if group header has been drawn
            public float MaxTabHeight; // Precomputed max height for smooth transitions
        }

        private class TabInfo
        {
            public string Name;
            public string Color;
            public int Order;
            public List<FieldInfo> Fields = new List<FieldInfo>();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TabAttribute tabAttr = (TabAttribute)attribute;
            int objectId = property.serializedObject.targetObject.GetInstanceID();

            // Initialize cache if needed
            RefreshGroupCache(property.serializedObject.targetObject, objectId);

            // Get group info
            if (!GroupCache.ContainsKey(objectId) || !GroupCache[objectId].ContainsKey(tabAttr.GroupName))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            GroupInfo groupInfo = GroupCache[objectId][tabAttr.GroupName];

            // Initialize state if needed
            if (!GroupStates.ContainsKey(objectId))
                GroupStates[objectId] = new Dictionary<string, int>();
            if (!GroupStates[objectId].ContainsKey(tabAttr.GroupName))
                GroupStates[objectId][tabAttr.GroupName] = 0;

            // Check if this is the first field in the group to draw header
            bool shouldDrawHeader = ShouldDrawGroupHeader(property, tabAttr, groupInfo);

            // Get current active tab
            int activeTabIndex = GroupStates[objectId][tabAttr.GroupName];
            string activeTabName = groupInfo.Tabs.Count > activeTabIndex ?
                groupInfo.Tabs[activeTabIndex].Name : "";

            // Only draw content if this field belongs to the active tab
            if (tabAttr.TabName == activeTabName)
            {
                float currentY = position.y;

                // Draw group header if this is the first field
                if (shouldDrawHeader)
                {
                    float headerHeight = GetGroupHeaderHeight(groupInfo.FirstTabAttribute);
                    Rect headerRect = new Rect(position.x, currentY, position.width, headerHeight);
                    DrawGroupHeader(headerRect, tabAttr, groupInfo, objectId);
                    currentY += headerHeight;
                    groupInfo.HasBeenDrawn = true;
                }

                // Draw the field with optional indentation
                Rect fieldRect = new Rect(position.x, currentY, position.width,
                    EditorGUI.GetPropertyHeight(property, label, true));

                if (tabAttr.Indent)
                {
                    fieldRect.x += 15f;
                    fieldRect.width -= 15f;
                }

                EditorGUI.PropertyField(fieldRect, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TabAttribute tabAttr = (TabAttribute)attribute;
            int objectId = property.serializedObject.targetObject.GetInstanceID();

            // Initialize cache if needed
            RefreshGroupCache(property.serializedObject.targetObject, objectId);

            // Get group info
            if (!GroupCache.ContainsKey(objectId) || !GroupCache[objectId].ContainsKey(tabAttr.GroupName))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            GroupInfo groupInfo = GroupCache[objectId][tabAttr.GroupName];

            // Initialize state if needed
            if (!GroupStates.ContainsKey(objectId))
                GroupStates[objectId] = new Dictionary<string, int>();
            if (!GroupStates[objectId].ContainsKey(tabAttr.GroupName))
                GroupStates[objectId][tabAttr.GroupName] = 0;

            // Get current active tab
            int activeTabIndex = GroupStates[objectId][tabAttr.GroupName];
            string activeTabName = groupInfo.Tabs.Count > activeTabIndex ?
                groupInfo.Tabs[activeTabIndex].Name : "";

            // Only return height if this field belongs to the active tab
            if (tabAttr.TabName == activeTabName)
            {
                float height = EditorGUI.GetPropertyHeight(property, label, true);

                // Add header height if this is the first field in the group
                if (ShouldDrawGroupHeader(property, tabAttr, groupInfo))
                {
                    height += GetGroupHeaderHeight(groupInfo.FirstTabAttribute);
                }

                return height;
            }

            // Hide inactive fields
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        /// <summary>
        /// Precomputes all tab heights to prevent layout shifts during tab switching
        /// </summary>
        private void PrecomputeTabHeights(Object targetObject, GroupInfo groupInfo)
        {
            // This helps eliminate the "loading from top to bottom" effect
            // by ensuring Unity knows the space requirements upfront

            float maxHeight = 0f;

            foreach (var tab in groupInfo.Tabs)
            {
                float tabHeight = 0f;

                foreach (var field in tab.Fields)
                {
                    // Estimate field height (this is a simplified calculation)
                    // In practice, Unity will still calculate exact heights during layout
                    tabHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    // Add extra height for complex field types
                    var fieldType = field.FieldType;
                    if (fieldType.IsArray || (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>)))
                    {
                        tabHeight += EditorGUIUtility.singleLineHeight * 2; // Estimate for arrays/lists
                    }
                }

                maxHeight = Mathf.Max(maxHeight, tabHeight);
            }

            groupInfo.MaxTabHeight = maxHeight;
        }

        /// <summary>
        /// Refreshes the group cache by scanning all fields with Tab attributes
        /// </summary>
        private void RefreshGroupCache(Object targetObject, int objectId)
        {
            if (!GroupCache.ContainsKey(objectId))
                GroupCache[objectId] = new Dictionary<string, GroupInfo>();

            var fields = targetObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var processedGroups = new HashSet<string>();

            foreach (var field in fields)
            {
                var tabAttr = field.GetCustomAttribute<TabAttribute>();
                if (tabAttr == null) continue;

                // Initialize group if needed
                if (!GroupCache[objectId].ContainsKey(tabAttr.GroupName))
                {
                    GroupCache[objectId][tabAttr.GroupName] = new GroupInfo();
                }

                var groupInfo = GroupCache[objectId][tabAttr.GroupName];

                // Set first tab attribute for styling
                if (groupInfo.FirstTabAttribute == null)
                    groupInfo.FirstTabAttribute = tabAttr;

                // Process this group only once
                if (!processedGroups.Contains(tabAttr.GroupName))
                {
                    processedGroups.Add(tabAttr.GroupName);
                    BuildGroupTabs(fields, tabAttr.GroupName, groupInfo);
                }
            }
        }

        /// <summary>
        /// Builds the tab structure for a group with intelligent color inheritance
        /// </summary>
        private void BuildGroupTabs(FieldInfo[] fields, string groupName, GroupInfo groupInfo)
        {
            groupInfo.Tabs.Clear();
            var tabDict = new Dictionary<string, TabInfo>();
            string lastColor = "default";

            // First pass: collect all tabs and their colors
            foreach (var field in fields)
            {
                var tabAttr = field.GetCustomAttribute<TabAttribute>();
                if (tabAttr?.GroupName != groupName) continue;

                if (!tabDict.ContainsKey(tabAttr.TabName))
                {
                    tabDict[tabAttr.TabName] = new TabInfo
                    {
                        Name = tabAttr.TabName,
                        Color = tabAttr.Color ?? lastColor,
                        Order = tabAttr.Order
                    };
                }

                // Update color if specified (for color inheritance)
                if (!string.IsNullOrEmpty(tabAttr.Color))
                {
                    tabDict[tabAttr.TabName].Color = tabAttr.Color;
                    lastColor = tabAttr.Color;
                }
                else
                {
                    // Inherit last color if not specified
                    tabDict[tabAttr.TabName].Color = lastColor;
                }

                tabDict[tabAttr.TabName].Fields.Add(field);
            }

            // Sort tabs by order, then by name
            groupInfo.Tabs = tabDict.Values
                .OrderBy(t => t.Order)
                .ThenBy(t => t.Name)
                .ToList();

            // Reset draw state
            groupInfo.HasBeenDrawn = false;
        }

        /// <summary>
        /// Determines if this field should draw the group header
        /// </summary>
        private bool ShouldDrawGroupHeader(SerializedProperty property, TabAttribute tabAttr, GroupInfo groupInfo)
        {
            if (groupInfo.HasBeenDrawn) return false;

            // Check if this is the first field in the currently active tab
            int activeTabIndex = GroupStates[property.serializedObject.targetObject.GetInstanceID()][tabAttr.GroupName];
            if (activeTabIndex >= groupInfo.Tabs.Count) return false;

            string activeTabName = groupInfo.Tabs[activeTabIndex].Name;
            if (tabAttr.TabName != activeTabName) return false;

            // Check if this is the first field in the active tab
            var activeTab = groupInfo.Tabs[activeTabIndex];
            return activeTab.Fields.Count > 0 && activeTab.Fields[0].Name == property.name;
        }

        /// <summary>
        /// Gets the height needed for the group header
        /// </summary>
        private float GetGroupHeaderHeight(TabAttribute tabAttr)
        {
            float height = tabAttr.SpaceAbove;

            if (tabAttr.ShowSeparator)
                height += 8f; // Separator line + spacing

            height += tabAttr.TabHeight; // Tab buttons
            height += tabAttr.SpaceBelow;

            return height;
        }

        /// <summary>
        /// Draws the group header with tabs
        /// </summary>
        private void DrawGroupHeader(Rect rect, TabAttribute tabAttr, GroupInfo groupInfo, int objectId)
        {
            float currentY = rect.y;

            // Draw separator line
            if (tabAttr.ShowSeparator)
            {
                Rect separatorRect = new Rect(rect.x, currentY, rect.width, 1f);
                Color separatorColor = EditorGUIUtility.isProSkin ?
                    new Color(0.3f, 0.3f, 0.3f, 1f) :
                    new Color(0.6f, 0.6f, 0.6f, 1f);
                EditorGUI.DrawRect(separatorRect, separatorColor);
                currentY += 8f;
            }
            else
            {
                currentY += tabAttr.SpaceAbove;
            }

            // Draw tab buttons
            Rect tabAreaRect = new Rect(rect.x, currentY, rect.width, tabAttr.TabHeight);
            DrawTabButtons(tabAreaRect, groupInfo, tabAttr, objectId);
        }

        /// <summary>
        /// Draws the tab buttons and handles interaction with optimized repainting
        /// </summary>
        private void DrawTabButtons(Rect area, GroupInfo groupInfo, TabAttribute tabAttr, int objectId)
        {
            if (groupInfo.Tabs.Count == 0) return;

            int activeTabIndex = GroupStates[objectId][tabAttr.GroupName];
            activeTabIndex = Mathf.Clamp(activeTabIndex, 0, groupInfo.Tabs.Count - 1);

            float tabWidth = area.width / groupInfo.Tabs.Count;

            // Handle mouse clicks with optimized repaint strategy
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                for (int i = 0; i < groupInfo.Tabs.Count; i++)
                {
                    Rect tabRect = new Rect(area.x + i * tabWidth, area.y, tabWidth, area.height);

                    if (tabRect.Contains(Event.current.mousePosition))
                    {
                        if (activeTabIndex != i)
                        {
                            // Update state immediately
                            GroupStates[objectId][tabAttr.GroupName] = i;
                            Event.current.Use();

                            // Optimized repaint strategy to prevent loading effect
                            var target = EditorUtility.InstanceIDToObject(objectId);
                            if (target != null)
                            {
                                // Mark as dirty for immediate update
                                EditorUtility.SetDirty(target);

                                // Force immediate layout recalculation
                                var activeWindow = EditorWindow.focusedWindow;
                                if (activeWindow != null)
                                {
                                    activeWindow.Repaint();
                                }

                                // Additional repaint to ensure smooth transition  
                                EditorApplication.delayCall += () =>
                                {
                                    if (target != null && activeWindow != null)
                                    {
                                        activeWindow.Repaint();
                                    }
                                };
                            }
                        }
                        break;
                    }
                }
            }

            // Draw each tab with enhanced visual feedback
            for (int i = 0; i < groupInfo.Tabs.Count; i++)
            {
                Rect tabRect = new Rect(area.x + i * tabWidth, area.y, tabWidth, area.height);
                bool isActive = i == activeTabIndex;

                DrawSingleTab(tabRect, groupInfo.Tabs[i], isActive, tabAttr);
            }
        }

        /// <summary>
        /// Draws a single tab button with styling
        /// </summary>
        private void DrawSingleTab(Rect rect, TabInfo tabInfo, bool isActive, TabAttribute tabAttr)
        {
            // Get colors based on tab's color theme and state
            Color backgroundColor = GetTabBackgroundColor(tabInfo.Color, isActive);
            Color borderColor = GetTabBorderColor(isActive);
            Color textColor = GetTabTextColor(isActive);

            // Draw background
            EditorGUI.DrawRect(rect, backgroundColor);

            // Draw border
            DrawTabBorder(rect, borderColor);

            // Draw text
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = tabAttr.FontSize,
                fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal,
                normal = { textColor = textColor }
            };

            GUI.Label(rect, tabInfo.Name, textStyle);

            // Draw active indicator with tab's color
            if (isActive)
            {
                Color accentColor = GetThemeColor(tabInfo.Color);
                Rect indicatorRect = new Rect(rect.x + 2, rect.y + rect.height - 3, rect.width - 4, 3);
                EditorGUI.DrawRect(indicatorRect, accentColor);
            }

            // Add hover cursor
            if (rect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            }
        }

        /// <summary>
        /// Draws border lines for a tab
        /// </summary>
        private void DrawTabBorder(Rect rect, Color borderColor)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), borderColor);
            EditorGUI.DrawRect(new Rect(rect.x + rect.width - 1, rect.y, 1, rect.height), borderColor);
        }

        // Color helper methods
        private Color GetTabBackgroundColor(string colorTheme, bool isActive)
        {
            if (isActive)
            {
                Color baseColor = GetThemeColor(colorTheme);
                return EditorGUIUtility.isProSkin ?
                    new Color(baseColor.r * 0.2f, baseColor.g * 0.2f, baseColor.b * 0.2f, 1f) :
                    new Color(baseColor.r * 0.9f, baseColor.g * 0.9f, baseColor.b * 0.9f, 1f);
            }
            else
            {
                return EditorGUIUtility.isProSkin ?
                    new Color(0.25f, 0.25f, 0.25f, 0.7f) :
                    new Color(0.75f, 0.75f, 0.75f, 0.7f);
            }
        }

        private Color GetTabBorderColor(bool isActive)
        {
            return EditorGUIUtility.isProSkin ?
                new Color(0.1f, 0.1f, 0.1f, 1f) :
                new Color(0.6f, 0.6f, 0.6f, 1f);
        }

        private Color GetTabTextColor(bool isActive)
        {
            if (isActive)
            {
                return EditorGUIUtility.isProSkin ? Color.white : Color.black;
            }
            else
            {
                return EditorGUIUtility.isProSkin ?
                    new Color(0.8f, 0.8f, 0.8f, 1f) :
                    new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }

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
                _ => EditorGUIUtility.isProSkin ?
                    new Color(0.3f, 0.6f, 1f, 1f) :
                    new Color(0.2f, 0.4f, 0.8f, 1f)
            };
        }
    }
}