using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public abstract class SettingsObjectInspector : UnityEditor.Editor
    {
        #region Constants

        private     static  readonly    ButtonMeta[]            s_emptyButtonArray          = new ButtonMeta[0];

        private     static  readonly    string[]                s_ignoredProperties         = new string[] { "m_Script" };

        #endregion

        #region Fields

        private     string                  m_productName;

        private     string                  m_productVersion;

        private     EditorLayoutBuilder     m_layoutBuilder;

        private     GUIStyle                m_customMarginStyle;

        // Assets
        private     Texture2D               m_logoIcon;

        #endregion

        #region Properties

        protected GUIStyle HeaderButtonStyle { get; private set; }

        protected GUIStyle HeaderFoldoutStyle { get; private set; }

        protected GUIStyle HeaderLabelStyle { get; private set; }

        #endregion

        #region Abstract methods

        protected abstract UnityPackageDefinition GetOwner();

        protected abstract string[] GetTabNames();

        protected abstract EditorSectionInfo[] GetSectionsForTab(string tab);

        #endregion

        #region Unity methods

        protected virtual void OnEnable()
        { }

        public override void OnInspectorGUI()
        {
            EnsurePropertiesAreSet();

            EditorGUILayout.BeginVertical(m_customMarginStyle);
            m_layoutBuilder.DoLayout();
            EditorGUILayout.EndVertical();
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        #endregion

        #region Draw methods

        protected virtual void DrawTopBar(string tab)
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
        }

        protected virtual bool DrawTabView(string tab)
        {
            return false;
        }

        protected virtual void DrawFooter(string tab)
        { }

        #endregion

        #region Private methods

        private void EnsurePropertiesAreSet()
        {
            if (m_layoutBuilder != null) return;

            LoadCustomStyles();
            LoadAssets();

            // Set properties
            var     commonResourcePath  = CoreLibrarySettings.Package.GetEditorResourcesPath();
            var     ownerPackage        = GetOwner();
            m_productName               = ownerPackage.DisplayName;
            m_productVersion            = $"v{ownerPackage.Version}";
            m_layoutBuilder             = new EditorLayoutBuilder(serializedObject: serializedObject,
                                                                  tabs: GetTabNames(),
                                                                  getSectionsCallback: GetSectionsForTab,
                                                                  drawTopBarCallback: DrawTopBar,
                                                                  drawTabViewCallback: DrawTabView,
                                                                  drawFooterCallback: DrawFooter,
                                                                  toggleOnIcon: AssetDatabase.LoadAssetAtPath<Texture2D>(commonResourcePath + "/Textures/toggle-on.png"),
                                                                  toggleOffIcon: AssetDatabase.LoadAssetAtPath<Texture2D>(commonResourcePath + "/Textures/toggle-off.png"));
        }

        private void LoadCustomStyles()
        {
            // Set custom style properties
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
            m_customMarginStyle = new GUIStyle(EditorStyles.inspectorFullWidthMargins)
            {
                margin          = new RectOffset(2, 2, 0, 0),
            };
        }

        private void LoadAssets()
        {
            // load custom assets
            var     ownerResourcePath   = GetOwner().GetEditorResourcesPath();
            m_logoIcon                  = AssetDatabase.LoadAssetAtPath<Texture2D>(ownerResourcePath + "/Textures/logo.png");
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

        #endregion

        #region Misc methods

        protected void ShowMigrateToUpmOption()
        {
            EditorLayoutUtility.Helpbox(
                title: "UPM Support",
                description: "You can install the package on UPM.",
                actionLabel: "Migrate To UPM",
                onClick: GetOwner().MigrateToUpm,
                style: CustomEditorStyles.GroupBackground);
        }

        #endregion

        #region Nested types

        protected class ButtonMeta
        {
            #region Properties

            public string Label { get; private set; }

            public System.Action OnClick { get; private set; }

            #endregion

            #region Constructors

            public ButtonMeta(string label, System.Action onClick)
            {
                // set properties
                Label       = label;
                OnClick     = onClick;
            }

            #endregion
        }

        #endregion
    }
}