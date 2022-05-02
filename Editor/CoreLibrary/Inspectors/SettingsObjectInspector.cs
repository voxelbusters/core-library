using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public abstract class SettingsObjectInspector : UnityEditor.Editor
    {
        #region Fields

        private     string                  m_productName;

        private     string                  m_productVersion;

        private     ButtonInfo[]            m_topBarButtons;

        private     PropertyGroupInfo[]     m_propertyGroups;

        private     int                     m_propertyGroupCount;

        private     SerializedProperty      m_activePropertyGroup;

        private     InspectorDrawStyle      m_drawStyle;

        // assets
        private     Texture2D               m_logoIcon;

        private     Texture2D               m_toggleOnIcon;

        private     Texture2D               m_toggleOffIcon;

        #endregion

        #region Properties

        protected GUIStyle GroupBackgroundStyle { get; private set; }

        protected GUIStyle HeaderStyle { get; private set; }

        protected GUIStyle HeaderFoldoutStyle { get; private set; }

        protected GUIStyle HeaderLabelStyle { get; private set; }

        protected GUIStyle HeaderToggleStyle { get; private set; }

        #endregion

        #region Unity methods

        protected virtual void OnEnable()
        {
            // set properties
            var     ownerPackage        = GetOwner();
            m_productName               = ownerPackage.DisplayName;
            m_productVersion            = $"v{ownerPackage.Version}";
            m_drawStyle                 = GetDrawStyle();
            m_topBarButtons             = GetTopBarButtons();
            if (m_drawStyle == InspectorDrawStyle.Group)
            {
                m_propertyGroups        = GetPropertyGroups();
                m_propertyGroupCount    = m_propertyGroups.Length;
            }
            LoadAssets();
        }

        public override void OnInspectorGUI()
        {
            EnsureStylesAreLoaded();
            DrawTopBar();
            EditorGUI.BeginChangeCheck();
            switch (m_drawStyle)
            {
                case InspectorDrawStyle.Default:
                    DrawDefaultInspector();
                    break;

                case InspectorDrawStyle.Group:
                    DrawGroupStyleInspector();
                    break;

                case InspectorDrawStyle.Custom:
                    DrawCustomInspector();
                    break;
            }
            GUILayout.Space(5f);
            DrawFooter();
            if (EditorGUI.EndChangeCheck())
            {
                // save changes
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        #endregion

        #region Abstract methods

        protected abstract UnityPackageDefinition GetOwner();

        #endregion

        #region Getter methods

        protected virtual InspectorDrawStyle GetDrawStyle() => InspectorDrawStyle.Default;

        protected virtual ButtonInfo[] GetTopBarButtons() => new ButtonInfo[0];

        protected virtual PropertyGroupInfo[] GetPropertyGroups() => new PropertyGroupInfo[0];

        #endregion

        #region Draw methods

        protected virtual void DrawTopBar()
        {
            GUILayout.BeginHorizontal(GroupBackgroundStyle);

            // logo section
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
            GUILayout.Label(m_logoIcon, GUILayout.Height(64f), GUILayout.Width(64f));
            GUILayout.Space(2f);
            GUILayout.EndVertical();

            // product info
            GUILayout.BeginVertical();
            GUILayout.Label(m_productName, "HeaderLabel");
            GUILayout.Label(m_productVersion, "MiniLabel");
            GUILayout.Label("Copyright © 2022 Voxel Busters Interactive LLP.", "MiniLabel");
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

        protected virtual void DrawGroupStyleInspector()
        {
            for (int iter = 0; iter < m_propertyGroupCount; iter++)
            {
                var     property    = m_propertyGroups[iter];
                DrawPropertyGroup(property);
            }
        }

        protected virtual void DrawCustomInspector()
        { }

        protected virtual void DrawFooter()
        { }

        private void DrawPropertyGroup(PropertyGroupInfo propertyMeta)
        {
            var     property        = propertyMeta.Reference;
            EditorGUILayout.BeginVertical(GroupBackgroundStyle);
            if (DrawControlHeader(property, propertyMeta.DisplayName))
            {
                bool    oldGUIState         = GUI.enabled;
                var     enabledProperty     = property.FindPropertyRelative("m_isEnabled");

                // update gui state
                GUI.enabled     = (enabledProperty == null || enabledProperty.boolValue);

                // show internal properties
                EditorGUI.indentLevel++;
                if (enabledProperty != null)
                {
                    DrawSettingsInternalProperties(property);
                }
                else
                {
                    DrawControlInternalProperties(property);
                }
                propertyMeta?.OnAfterPropertyDraw(property);
                EditorGUI.indentLevel--;

                // reset gui state
                GUI.enabled     = oldGUIState;
            }
            EditorGUILayout.EndVertical();
        }

        private bool DrawControlHeader(SerializedProperty property, string displayName)
        {
            // draw rect
            var     rect                = EditorGUILayout.GetControlRect(false, 30f);
            GUI.Box(rect, GUIContent.none, HeaderStyle);

            // draw foldable control
            bool    isSelected          = property == m_activePropertyGroup;
            var     foldOutRect         = new Rect(rect.x, rect.y, 50f, rect.height);
            EditorGUI.LabelField(foldOutRect, isSelected ? "-" : "+", HeaderFoldoutStyle);

            // draw label 
            var     labelRect           = new Rect(rect.x + 25f, rect.y, rect.width - 100f, rect.height);
            EditorGUI.LabelField(labelRect, displayName, HeaderLabelStyle);

            // draw selectable rect
            var     selectableRect      = new Rect(rect.x, rect.y, rect.width - 100f, rect.height);
            if (DrawTransparentButton(selectableRect, string.Empty))
            {
                isSelected              = OnPropertyGroupHeaderSelect(property);
            }

            // draw toggle button
            var     enabledProperty     = property.FindPropertyRelative("m_isEnabled");
            if ((enabledProperty != null) /*&& NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState()*/)
            {
                Rect    toggleRect                  = new Rect(rect.xMax - 64f, rect.y, 64f, 25f);
                if (GUI.Button(toggleRect, enabledProperty.boolValue ? m_toggleOnIcon : m_toggleOffIcon, HeaderToggleStyle))
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

        private void DrawSettingsInternalProperties(SerializedProperty settingsProperty)
        {
            // move pointer to first element
            var     currentProperty  = settingsProperty.Copy();
            currentProperty.NextVisible(enterChildren: true);
            var     endProperty      = settingsProperty.GetEndProperty();

            // start iterating through the properties
            while (currentProperty.NextVisible(enterChildren: false))
            {
                if (SerializedProperty.EqualContents(currentProperty, endProperty))
                {
                    break;
                }
                EditorGUILayout.PropertyField(currentProperty, true);
            }
        }

        private void DrawControlInternalProperties(SerializedProperty property)
        {
            // move pointer to first element
            var     currentProperty  = property.Copy();
            var     endProperty      = default(SerializedProperty);

            // start iterating through the properties
            bool    firstTime   = true;
            while (currentProperty.NextVisible(enterChildren: firstTime))
            {
                if (firstTime)
                {
                    endProperty = property.GetEndProperty();
                    firstTime   = false;
                }
                if (SerializedProperty.EqualContents(currentProperty, endProperty))
                {
                    break;
                }
                EditorGUILayout.PropertyField(currentProperty, true);
            }
        }

        protected bool DrawTransparentButton(Rect rect, string label)
        {
            var     originalColor   = GUI.color;
            try
            {
                GUI.color   = Color.clear;
                return GUI.Button(rect, string.Empty);
            }
            finally
            {
                GUI.color   = originalColor;
            }
        }

        #endregion

        #region Private methods

        private void EnsureStylesAreLoaded()
        {
            // check whether styles are already loaded
            if (null != GroupBackgroundStyle)
            {
                return;
            }

            // bg style
            GroupBackgroundStyle          = new GUIStyle("HelpBox");
            var     bgOffset                = GroupBackgroundStyle.margin;
            bgOffset.bottom                 = 5;
            GroupBackgroundStyle.margin   = bgOffset;

            // header style
            HeaderStyle                   = new GUIStyle("PreButton");
            HeaderStyle.fixedHeight       = 0;

            // foldout style
            HeaderFoldoutStyle            = new GUIStyle("WhiteBoldLabel");
            HeaderFoldoutStyle.fontSize   = 20;
            HeaderFoldoutStyle.alignment  = TextAnchor.MiddleLeft;

            // label style
            HeaderLabelStyle              = new GUIStyle("WhiteBoldLabel");
            HeaderLabelStyle.fontSize     = 12;
            HeaderLabelStyle.alignment    = TextAnchor.MiddleLeft;

            // enabled style
            HeaderToggleStyle             = new GUIStyle("InvisibleButton");
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

        #endregion

        #region Callback methods

        private bool OnPropertyGroupHeaderSelect(SerializedProperty property)
        {
            var     lastActiveProperty  = m_activePropertyGroup;
            if (m_activePropertyGroup == null)
            {
                property.isExpanded     = true;
                m_activePropertyGroup   = property;

                return true;
            }
            if (m_activePropertyGroup == property)
            {
                property.isExpanded     = false;
                m_activePropertyGroup   = null;

                return false;
            }

            // update reference
            m_activePropertyGroup               = property;
            m_activePropertyGroup.isExpanded    = true;
            lastActiveProperty.isExpanded       = false;

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

            public System.Action<SerializedProperty> OnAfterPropertyDraw { get; private set; }

            #endregion

            #region Constructors

            public PropertyGroupInfo(SerializedProperty reference, string displayName, System.Action<SerializedProperty> onAfterPropertyDraw)
            {
                // set properties
                Reference           = reference;
                DisplayName         = displayName;
                OnAfterPropertyDraw = onAfterPropertyDraw;
            }

            #endregion
        }

        #endregion
    }
}