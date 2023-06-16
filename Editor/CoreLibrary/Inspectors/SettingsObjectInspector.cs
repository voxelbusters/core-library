using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public abstract class SettingsObjectInspector : UnityEditor.Editor
    {
        #region Constants

        private     static  readonly    ButtonInfo[]            s_emptyButtonArray          = new ButtonInfo[0];

        private     static  readonly    PropertyGroupInfo[]     s_emptyPropertyGroupArray   = new PropertyGroupInfo[0];
         
        private     static  readonly    string[]                s_ignoredProperties         = new string[] { "m_Script" };

        #endregion

        #region Fields

        private     string                  m_productName;

        private     string                  m_productVersion;

        private     ButtonInfo[]            m_topBarButtons;

        private     PropertyGroupInfo[]     m_propertyGroups;

        private     int                     m_propertyGroupCount;

        private     InspectorDrawStyle      m_drawStyle;

        // assets
        private     Texture2D               m_logoIcon;

        private     Texture2D               m_toggleOnIcon;

        private     Texture2D               m_toggleOffIcon;

        #endregion

        #region Properties

        protected GUIStyle HeaderButtonStyle { get; private set; }

        protected GUIStyle HeaderFoldoutStyle { get; private set; }

        protected GUIStyle HeaderLabelStyle { get; private set; }

        protected SerializedProperty FocusProperty { get; private set; }

        #endregion

        #region Abstract methods

        protected abstract UnityPackageDefinition GetOwner();

        #endregion

        #region Unity methods

        protected virtual void OnEnable()
        {
            // set properties
            LoadAssets();
        }

        public override void OnInspectorGUI()
        {
            EnsurePropertiesAreSet();
            EnsureStylesAreLoaded();

            EditorGUI.BeginChangeCheck();
            if (CanShowMainMenu())
            {
                DrawMainMenuContent();
            }
            else
            {
                DrawFocusPropertyContent();
            }
            TryApplyModifiedProperties();
        }

        #endregion

        #region Getter methods

        protected virtual InspectorDrawStyle GetDrawStyle() => InspectorDrawStyle.Default;

        protected virtual ButtonInfo[] GetTopBarButtons() => s_emptyButtonArray;

        protected virtual PropertyGroupInfo[] GetPropertyGroups() => s_emptyPropertyGroupArray;

        #endregion

        #region Draw methods

        protected virtual void DrawTopBar()
        {
            GUILayout.BeginHorizontal(CustomEditorStyles.GroupBackground);

            // logo section
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
            GUILayout.Label(m_logoIcon, GUILayout.Height(64f), GUILayout.Width(64f));
            GUILayout.Space(2f);
            GUILayout.EndVertical();

            // product info
            GUILayout.BeginVertical();
            GUILayout.Label(m_productName, CustomEditorStyles.Heading1);
            GUILayout.Label(m_productVersion, CustomEditorStyles.Normal);
            GUILayout.Label("Copyright © 2022 Voxel Busters Interactive LLP.", CustomEditorStyles.Options);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // top bar buttons
            if (m_topBarButtons != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                int     buttonCount = m_topBarButtons.Length;
                for (int iter = 0; iter < buttonCount; iter++)
                {
                    var     current = m_topBarButtons[iter];
                    string  style   = "ButtonMid";
                    if (iter == 0)
                    {
                        style       = "ButtonLeft";
                    }
                    else if (iter == (buttonCount - 1))
                    {
                        style       = "ButtonRight";
                    }
                    if (GUILayout.Button(current.Label, style))
                    {
                        current.OnClick?.Invoke();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        private void DrawMainMenuContent()
        {
            DrawTopBar();
            GUILayout.Space(5f);

            switch (m_drawStyle)
            {
                case InspectorDrawStyle.Default:
                    DrawDefaultStyleInspector();
                    break;

                case InspectorDrawStyle.Group:
                    DrawGroupStyleInspector();
                    break;

                case InspectorDrawStyle.Custom:
                    DrawCustomStyleInspector();
                    break;
            }
            
            GUILayout.Space(5f);
            DrawFooter();
        }

        private bool CanShowMainMenu()
        {
            return !TryGetFocusPropertyGroup(m_propertyGroups, out PropertyGroupInfo focusGroup) ||
                (focusGroup.DrawStyle == PropertyGroupDrawStyle.Expand);
        }

        private void DrawDefaultStyleInspector()
        {
            EditorGUILayout.BeginVertical(CustomEditorStyles.GroupBackground);
            var     property    = serializedObject.GetIterator();
            if (property.NextVisible(enterChildren: true))
            {
                do
                {
                    if (System.Array.Exists(s_ignoredProperties, (item) => string.Equals(item, property.name))) continue;

                    EditorGUILayout.PropertyField(property);
                } while (property.NextVisible(enterChildren: false));
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawGroupStyleInspector()
        {
            for (int iter = 0; iter < m_propertyGroupCount; iter++)
            {
                var     property    = m_propertyGroups[iter];
                DrawPropertyGroup(property);
            }
        }

        protected virtual void DrawCustomStyleInspector()
        { }

        protected virtual void DrawFooter()
        { }

        protected void DrawFocusPropertyContent()
        {
            TryGetFocusPropertyGroup(m_propertyGroups, out PropertyGroupInfo focusGroup);

            char upArrow = '\u2190';
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"{upArrow} Back To Main Menu", CustomEditorStyles.SelectableLabel))
            {
                FocusProperty.isExpanded    = false;
                FocusProperty               = null;
                return;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            FocusProperty.isExpanded    = true;
            DrawPropertyGroup(focusGroup, selectable: false);
        }

        protected void DrawPropertyGroup(PropertyGroupInfo propertyGroup, bool selectable = true)
        {
            var     property        = propertyGroup.Reference;
            EditorGUILayout.BeginVertical(CustomEditorStyles.GroupBackground);
            if (DrawPropertyGroupHeader(property,
                                        propertyGroup.DisplayName,
                                        propertyGroup.Description,
                                        propertyGroup.DrawStyle,
                                        selectable))
            {
                DrawPropertyGroupChildren(propertyGroup);
            }
            EditorGUILayout.EndVertical();
        }

        private bool DrawPropertyGroupHeader(SerializedProperty property, string displayName,
                                             string description, PropertyGroupDrawStyle drawStyle,
                                             bool selectable = true)
        {
            bool    isSelected          = (property == FocusProperty);

            // Draw rect
            var     rect                = EditorGUILayout.GetControlRect(false, 80f);
            //GUI.Box(rect, GUIContent.none, HeaderButtonStyle);

            /*
            // Draw expand control
            if (drawStyle == PropertyGroupDrawStyle.Expand)
            {
                var     foldOutRect         = new Rect(rect.x + 5f, rect.y, 50f, rect.height);
                EditorGUI.LabelField(foldOutRect, isSelected ? "-" : "+", CustomEditorStyles.Heading3);
            }
            */

            // Draw text 
            var     titleRect           = new Rect(rect.x + 5f, rect.y + 10f, rect.width * 0.8f, 22f);
            var     descriptionRect     = new Rect(rect.x + 5f, rect.y + 42f, rect.width * 0.8f, 30f);
            EditorGUI.LabelField(titleRect, displayName, CustomEditorStyles.Normal);
            EditorGUI.LabelField(descriptionRect, description, CustomEditorStyles.Options);

            // Draw selectable rect
            var     selectableRect      = new Rect(rect.x, rect.y, rect.width * 0.8f, rect.height);
            if (selectable && EditorLayoutUtility.TransparentButton(selectableRect))
            {
                isSelected              = OnPropertyGroupSelect(property);
            }

            // Draw toggle button
            var     enabledProperty     = property.FindPropertyRelative("m_isEnabled");
            if (enabledProperty != null)
            {

                Vector2 iconSize = new Vector2(32f, 12f);
                Rect toggleRect = new Rect(rect.xMax - (iconSize.x * 1.2f), rect.y + 42f, iconSize.x, iconSize.y);

                if (GUI.Button(toggleRect, enabledProperty.boolValue ? m_toggleOnIcon : m_toggleOffIcon, CustomEditorStyles.InvisibleButton))
                {
                    enabledProperty.boolValue       = !enabledProperty.boolValue;
#if UNITY_ANDROID
                    //TODO : Fire an event if any feature toggles and listent for adding the dependencies
                    EditorPrefs.SetBool("refresh-feature-dependencies", true);
#endif
                }
            }
            return isSelected;
        }

        private void DrawPropertyGroupChildren(PropertyGroupInfo propertyGroup)
        {
            var     property        = propertyGroup.Reference;
            bool    oldGUIState     = GUI.enabled;
            var     enabledProperty = property.FindPropertyRelative("m_isEnabled");

            GUI.enabled             = (enabledProperty == null) || enabledProperty.boolValue;

            // display child properties
            if (propertyGroup.OnDrawChildProperties != null)
            {
                propertyGroup.OnDrawChildProperties(propertyGroup.Reference);
            }
            else
            {
                DrawChildProperties(property, ignoreProperties: "m_isEnabled");
            }

            // reset gui state
            GUI.enabled     = oldGUIState;
        }

        protected void DrawChildProperties(SerializedProperty property, string prefix = null,
            bool indent = true, params string[] ignoreProperties)
        {
            try
            {
                if (indent)
                {
                    EditorGUI.indentLevel++;
                }

                // move pointer to first element
                var     currentProperty  = property.Copy();
                var     endProperty      = default(SerializedProperty);

                // start iterating through the properties
                bool    firstTime       = true;
                while (currentProperty.NextVisible(enterChildren: firstTime))
                {
                    if (firstTime)
                    {
                        endProperty      = property.GetEndProperty();
                        firstTime        = false;
                    }
                    if (SerializedProperty.EqualContents(currentProperty, endProperty))
                    {
                        break;
                    }

                    // exclude specified properties
                    if ((ignoreProperties != null) && System.Array.Exists(ignoreProperties, (item) => string.Equals(item, currentProperty.name)))
                    {
                        continue;
                    }

                    // display the property
                    if (prefix != null)
                    {
                        EditorGUILayout.PropertyField(currentProperty, new GUIContent($"{prefix} {currentProperty.displayName}", currentProperty.tooltip), true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(currentProperty, true);
                    }
                }
            }
            finally
            {
                if (indent)
                {
                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion

        #region Private methods

        private void EnsurePropertiesAreSet()
        {
            if (m_topBarButtons != null) return;
            
            var     ownerPackage        = GetOwner();
            m_productName               = ownerPackage.DisplayName;
            m_productVersion            = $"v{ownerPackage.Version}";
            m_drawStyle                 = GetDrawStyle();
            m_topBarButtons             = GetTopBarButtons();
            m_propertyGroups            = GetPropertyGroups() ?? new PropertyGroupInfo[0];
            m_propertyGroupCount        = m_propertyGroups.Length;
        }

        private void EnsureStylesAreLoaded()
        {
            // check whether styles are already loaded
            if (null != HeaderButtonStyle) return;

            // set custom style properties
            HeaderButtonStyle   = new GUIStyle("PreButton")
            {
                fixedHeight     = 0,
                fontSize        = 20,
                alignment       = TextAnchor.MiddleLeft,
            };
            HeaderFoldoutStyle  = new GUIStyle("WhiteBoldLabel")
            {
                fontSize        = 20,
                alignment       = TextAnchor.MiddleLeft,
            };
            HeaderLabelStyle    = new GUIStyle("WhiteBoldLabel")
            {
                fontSize        = 20,
                alignment       = TextAnchor.MiddleLeft,
            };
        }

        protected void EnsureChangesAreSerialized()
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        protected void TryApplyModifiedProperties()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        private void LoadAssets()
        {
            // load custom assets
            var     ownerResourcePath   = GetOwner().GetEditorResourcesPath();
            m_logoIcon                  = AssetDatabase.LoadAssetAtPath<Texture2D>(ownerResourcePath + "/Textures/logo.png");

            // load default assets
            var     commonResourcePath  = CoreLibrarySettings.Package.GetEditorResourcesPath();
            m_toggleOnIcon              = AssetDatabase.LoadAssetAtPath<Texture2D>(commonResourcePath + "/Textures/toggle-on.png");
            m_toggleOffIcon             = AssetDatabase.LoadAssetAtPath<Texture2D>(commonResourcePath + "/Textures/toggle-off.png");
        }

        protected bool TryGetFocusPropertyGroup(PropertyGroupInfo[] groups, out PropertyGroupInfo groupInfo)
        {
            groupInfo   = null;
            if (FocusProperty == null) return false;

            groupInfo = System.Array.Find(groups, (item) => (item.Reference == FocusProperty));
            return (groupInfo != null);
        }

        #endregion

        #region Additional methods

        protected void ShowMigrateToUpmOption()
        {
            EditorLayoutUtility.Helpbox(
                title: "UPM Support",
                description: "You can install the package on UPM.",
                actionLabel: "Migrate To UPM",
                onClick: GetOwner().MigrateToUpm,
                style: CustomEditorStyles.GroupBackground);
        }

        [System.Obsolete("Use DrawCustomStyleInspector instead.", false)]
        protected virtual void DrawCustomInspector()
        { }

        #endregion

        #region Callback methods

        protected bool OnPropertyGroupSelect(SerializedProperty property)
        {
            var     lastActiveProperty  = FocusProperty;
            if (FocusProperty == null)
            {
                property.isExpanded     = true;
                FocusProperty           = property;

                return true;
            }
            if (FocusProperty == property)
            {
                property.isExpanded     = false;
                FocusProperty           = null;

                return false;
            }

            // update reference
            lastActiveProperty.isExpanded   = false;
            FocusProperty                   = property;
            FocusProperty.isExpanded        = true;

            return true;
        }

        #endregion

        #region Nested types

        protected enum InspectorDrawStyle
        {
            Default = 1,

            Group,

            Custom,
        }

        protected class ButtonInfo
        {
            #region Properties

            public string Label { get; private set; }

            public System.Action OnClick { get; private set; }

            #endregion

            #region Constructors

            public ButtonInfo(string label, System.Action onClick)
            {
                // set properties
                Label       = label;
                OnClick     = onClick;
            }

            #endregion
        }

        protected class PropertyGroupInfo
        {
            #region Properties

            public SerializedProperty Reference { get; private set; }

            public string DisplayName { get; private set; }

            public string Description { get; private set; }

            public System.Action<SerializedProperty> OnDrawChildProperties { get; private set; }

            public PropertyGroupDrawStyle DrawStyle { get; private set; }

            #endregion

            #region Constructors

            public PropertyGroupInfo(SerializedProperty reference, string displayName,
                                     string description, PropertyGroupDrawStyle drawStyle = PropertyGroupDrawStyle.Expand,
                                     System.Action<SerializedProperty> onDrawChildProperties = null)
            {
                Assert.IsArgNotNull(reference, displayName);

                // set properties
                Reference                   = reference;
                DisplayName                 = displayName;
                Description                 = description;
                DrawStyle                   = drawStyle;
                OnDrawChildProperties       = onDrawChildProperties;
            }

            #endregion
        }

        protected enum PropertyGroupDrawStyle
        {
            Expand,

            Detailed,
        }

        #endregion
    }
}