using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal sealed class PluginProductSettingsProvider : SettingsProvider
    {
        private const string kProviderRoot = "Project/Voxel Busters";
        private const string kFeatureEnabledProperty = "m_isEnabled";
        private const string kStyleSheetName = PluginProductFrameworkConstants.SettingsProviderUssName;
        private const string kRootUxmlName = PluginProductFrameworkConstants.SettingsProviderUxmlName;
        private const string kHeaderUxmlName = PluginProductFrameworkConstants.SettingsHeaderUxmlName;
        private const string kTabsUxmlName = PluginProductFrameworkConstants.SettingsTabsUxmlName;
        private const string kCardUxmlName = PluginProductFrameworkConstants.SettingsCardUxmlName;
        private const string kFeatureRowUxmlName = PluginProductFrameworkConstants.SettingsFeatureRowUxmlName;
        private const string kStatusUxmlName = PluginProductFrameworkConstants.SettingsStatusUxmlName;
        private const string kResourceRowUxmlName = PluginProductFrameworkConstants.SettingsResourceRowUxmlName;
        private const string kToggleOnTextureName = "toggle-on";
        private const string kToggleOffTextureName = "toggle-off";

        private readonly PluginProductDescriptor m_descriptor;
        private PluginProductSettings m_settings;
        private VisualElement m_statusRoot;
        private Label m_statusLabel;
        private VisualElement m_headerHost;
        private VisualElement m_tabsHost;
        private VisualElement m_generalContainer;
        private VisualElement m_featuresContainer;
        private VisualElement m_resourcesContainer;
        private VisualElement m_statusHost;
        private static Texture2D s_toggleOnTexture;
        private static Texture2D s_toggleOffTexture;

        private PluginProductSettingsProvider(string path, PluginProductDescriptor descriptor)
            : base(path, SettingsScope.Project)
        {
            m_descriptor = descriptor;
            label = descriptor != null && !string.IsNullOrEmpty(descriptor.ProductDisplayName)
                ? descriptor.ProductDisplayName
                : descriptor?.ProductCodeName ?? "Plugin Product";
        }

        [SettingsProviderGroup]
        private static SettingsProvider[] CreateProviders()
        {
            List<PluginProductDescriptor> descriptors = PluginProductDescriptorUtility.FindAllPluginProductDescriptors();
            if (descriptors.Count == 0)
            {
                return null;
            }

            var providers = new List<SettingsProvider>();
            foreach (PluginProductDescriptor descriptor in descriptors)
            {
                if (descriptor == null)
                {
                    continue;
                }

                string displayName = string.IsNullOrEmpty(descriptor.ProductDisplayName)
                    ? descriptor.ProductCodeName
                    : descriptor.ProductDisplayName;
                string path = $"{kProviderRoot}/{displayName}";
                providers.Add(new PluginProductSettingsProvider(path, descriptor));
            }

            return providers.ToArray();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            rootElement.Clear();
            var styleSheet = LoadStyleSheet(kStyleSheetName);
            if (styleSheet != null)
            {
                rootElement.styleSheets.Add(styleSheet);
            }

            var rootUxml = LoadUxml(kRootUxmlName);
            if (rootUxml == null)
            {
                rootElement.Add(new HelpBox("Settings UI template missing.", HelpBoxMessageType.Error));
                return;
            }

            VisualElement root = rootUxml.CloneTree();
            rootElement.Add(root);

            m_headerHost = root.Q<VisualElement>("header-host");
            m_tabsHost = root.Q<VisualElement>("tabs-host");
            m_generalContainer = root.Q<ScrollView>("general-container");
            m_featuresContainer = root.Q<ScrollView>("features-container");
            m_resourcesContainer = root.Q<ScrollView>("resources-container");
            m_statusHost = root.Q<VisualElement>("status-host");

            if (m_generalContainer == null || m_featuresContainer == null || m_resourcesContainer == null)
            {
                rootElement.Add(new HelpBox("Settings UI template is missing containers.", HelpBoxMessageType.Error));
                return;
            }

            BuildHeader(m_headerHost);
            BuildTabs(m_tabsHost);
            BuildFooter(m_statusHost);

            RefreshStatusMessage();
        }

        private void BuildHeader(VisualElement host)
        {
            if (host == null)
            {
                return;
            }

            host.Clear();
            var headerUxml = LoadUxml(kHeaderUxmlName);
            if (headerUxml == null)
            {
                return;
            }

            VisualElement header = headerUxml.CloneTree();
            host.Add(header);

            var icon = header.Q<Image>("product-icon");
            var title = header.Q<Label>("product-title");
            var versionLabel = header.Q<Label>("product-version");
            var meta = header.Q<Label>("product-meta");

            if (title != null)
            {
                title.text = GetProductTitle();
            }

            if (versionLabel != null)
            {
                string version = GetProductVersion();
                if (string.IsNullOrEmpty(version))
                {
                    versionLabel.style.display = DisplayStyle.None;
                }
                else
                {
                    versionLabel.text = $"Version {version}";
                }
            }

            if (icon != null)
            {
                if (m_descriptor != null && m_descriptor.ProductIcon != null)
                {
                    icon.image = m_descriptor.ProductIcon;
                }
                else
                {
                    icon.style.display = DisplayStyle.None;
                }
            }

            if (meta != null)
            {
                if (m_descriptor != null && !string.IsNullOrEmpty(m_descriptor.Copyright))
                {
                    meta.text = m_descriptor.Copyright;
                }
                else
                {
                    meta.style.display = DisplayStyle.None;
                }
            }
        }

        private string GetProductTitle()
        {
            if (m_descriptor == null)
            {
                return "Plugin Product Settings";
            }

            return string.IsNullOrEmpty(m_descriptor.ProductDisplayName)
                ? m_descriptor.ProductCodeName
                : m_descriptor.ProductDisplayName;
        }

        private string GetProductVersion()
        {
            if (m_descriptor == null)
            {
                return string.Empty;
            }

            return m_descriptor.Version;
        }

        private void BuildTabs(VisualElement host)
        {
            if (host == null)
            {
                return;
            }

            host.Clear();
            var tabsUxml = LoadUxml(kTabsUxmlName);
            if (tabsUxml == null)
            {
                return;
            }

            VisualElement toolbar = tabsUxml.CloneTree();
            host.Add(toolbar);

            var generalToggle = toolbar.Q<ToolbarToggle>("tab-general");
            var featuresToggle = toolbar.Q<ToolbarToggle>("tab-features");
            var resourcesToggle = toolbar.Q<ToolbarToggle>("tab-resources");
            if (generalToggle == null || featuresToggle == null || resourcesToggle == null)
            {
                return;
            }

            generalToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    SetActiveTab(SettingsTab.General, generalToggle, featuresToggle, resourcesToggle);
                }
            });
            featuresToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    SetActiveTab(SettingsTab.Features, generalToggle, featuresToggle, resourcesToggle);
                }
            });
            resourcesToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    SetActiveTab(SettingsTab.Resources, generalToggle, featuresToggle, resourcesToggle);
                }
            });

            SetActiveTab(SettingsTab.General, generalToggle, featuresToggle, resourcesToggle);

            BuildGeneralSection();
            BuildFeaturesSection();
            BuildResourcesSection();
        }

        private enum SettingsTab
        {
            General = 0,
            Features = 1,
            Resources = 2
        }

        private void SetActiveTab(SettingsTab tab,
                                  ToolbarToggle generalToggle,
                                  ToolbarToggle featuresToggle,
                                  ToolbarToggle resourcesToggle)
        {
            generalToggle.SetValueWithoutNotify(tab == SettingsTab.General);
            featuresToggle.SetValueWithoutNotify(tab == SettingsTab.Features);
            resourcesToggle.SetValueWithoutNotify(tab == SettingsTab.Resources);
            SelectTab(tab);
        }

        private void SelectTab(SettingsTab tab)
        {
            m_generalContainer.style.display = tab == SettingsTab.General ? DisplayStyle.Flex : DisplayStyle.None;
            m_featuresContainer.style.display = tab == SettingsTab.Features ? DisplayStyle.Flex : DisplayStyle.None;
            m_resourcesContainer.style.display = tab == SettingsTab.Resources ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void BuildGeneralSection()
        {
            m_generalContainer.Clear();
            if (m_descriptor == null)
            {
                m_generalContainer.Add(new HelpBox("Descriptor not found.", HelpBoxMessageType.Error));
                return;
            }

            m_settings = PluginProductSettingsUtility.TryLoadProductSettings(m_descriptor);
            if (m_settings == null)
            {
                m_generalContainer.Add(new HelpBox("Settings asset not found. Use Window → Voxel Busters → Open Settings to create it.", HelpBoxMessageType.Warning));
                return;
            }

            if (m_settings.GeneralSettings == null)
            {
                m_generalContainer.Add(new HelpBox("General settings are missing from the settings asset.", HelpBoxMessageType.Warning));
                return;
            }

            var generalObject = new SerializedObject(m_settings.GeneralSettings);
            var card = CreateCard(out var content);
            BuildInspectorExcludingScript(generalObject, content ?? card);
            m_generalContainer.Add(card);
        }

        private void BuildResourcesSection()
        {
            m_resourcesContainer.Clear();
            if (m_descriptor == null)
            {
                m_resourcesContainer.Add(new HelpBox("Descriptor not found.", HelpBoxMessageType.Error));
                return;
            }

            m_settings = PluginProductSettingsUtility.TryLoadProductSettings(m_descriptor);
            if (m_settings == null)
            {
                m_resourcesContainer.Add(new HelpBox("Settings asset not found. Use Window → Voxel Busters → Open Settings to create it.", HelpBoxMessageType.Warning));
                return;
            }

            if (m_settings.ResourcesSettings == null)
            {
                m_resourcesContainer.Add(new HelpBox("Resources settings are missing from the settings asset.", HelpBoxMessageType.Warning));
                return;
            }

            var resourcesObject = new SerializedObject(m_settings.ResourcesSettings);
            var card = CreateCard(out var content);
            BuildResourcesContent(resourcesObject, content ?? card);
            m_resourcesContainer.Add(card);
        }

        private void BuildFeaturesSection()
        {
            m_featuresContainer.Clear();

            if (m_descriptor == null)
            {
                m_featuresContainer.Add(new HelpBox("Descriptor not found.", HelpBoxMessageType.Error));
                return;
            }

            m_settings = PluginProductSettingsUtility.TryLoadProductSettings(m_descriptor);
            if (m_settings == null)
            {
                m_featuresContainer.Add(new HelpBox("Settings asset not found. Use Window → Voxel Busters → Open Settings to create it.", HelpBoxMessageType.Warning));
                return;
            }

            PluginProductSettingsUtility.EnsureFeatureSettings(m_descriptor, m_settings);

            List<FeatureSettings> features = PluginProductSettingsUtility.GetFeatureSettings(m_settings);
            if (features.Count == 0)
            {
                m_featuresContainer.Add(new HelpBox("No features found.", HelpBoxMessageType.Info));
                return;
            }

            var ordered = features
                .Where(feature => feature != null)
                .OrderBy(feature => feature.FeatureDescriptor != null
                    ? feature.FeatureDescriptor.FeatureDisplayName
                    : feature.name)
                .ToList();

            foreach (FeatureSettings feature in ordered)
            {
                m_featuresContainer.Add(CreateFeatureRow(feature));
            }
        }

        private VisualElement CreateFeatureRow(FeatureSettings feature)
        {
            var rowUxml = LoadUxml(kFeatureRowUxmlName);
            if (rowUxml == null)
            {
                return new VisualElement();
            }

            var container = rowUxml.CloneTree();
            var toggle = container.Q<Toggle>("feature-toggle");
            var nameLabel = container.Q<Label>("feature-name");
            var descriptionLabel = container.Q<Label>("feature-description");
            var details = container.Q<VisualElement>("feature-details");
            var header = container.Q<VisualElement>("feature-header") ?? container;

            string featureName = GetFeatureDisplayName(feature);
            string featureDescription = GetFeatureDescription(feature);
            var serializedObject = new SerializedObject(feature);

            SerializedProperty enabledProperty = serializedObject.FindProperty(kFeatureEnabledProperty);
            if (enabledProperty != null && toggle != null)
            {
                toggle.BindProperty(enabledProperty);
            }

            if (nameLabel != null)
            {
                nameLabel.text = featureName;
            }

            if (descriptionLabel != null)
            {
                if (string.IsNullOrEmpty(featureDescription))
                {
                    descriptionLabel.text = "Add a short description for this feature.";
                    descriptionLabel.AddToClassList("vb-feature-description-placeholder");
                }
                else
                {
                    descriptionLabel.text = featureDescription;
                    descriptionLabel.RemoveFromClassList("vb-feature-description-placeholder");
                }
            }

            if (details != null)
            {
                BuildInspectorExcludingScript(serializedObject, details);
                details.style.display = DisplayStyle.None;
            }

            if (details != null)
            {
                header.tooltip = "Click to configure";
                header.RegisterCallback<MouseUpEvent>(evt =>
                {
                    if (toggle != null && toggle.worldBound.Contains(evt.mousePosition))
                    {
                        return;
                    }

                    bool isExpanded = details.style.display != DisplayStyle.None;
                    details.style.display = isExpanded ? DisplayStyle.None : DisplayStyle.Flex;
                });
            }

            container.TrackSerializedObjectValue(serializedObject, _ => OnFeatureSettingsChanged(feature));
            if (toggle != null)
            {
                InitializeToggleVisuals(toggle);
                toggle.RegisterValueChangedCallback(evt =>
                {
                    UpdateFeatureRowState(container, nameLabel, descriptionLabel, evt.newValue);
                    UpdateToggleVisuals(toggle, evt.newValue);
                    OnFeatureSettingsChanged(feature);
                });
            }

            bool initialEnabled = enabledProperty == null || enabledProperty.boolValue;
            UpdateFeatureRowState(container, nameLabel, descriptionLabel, initialEnabled);
            if (toggle != null)
            {
                UpdateToggleVisuals(toggle, initialEnabled);
            }

            return container;
        }

        private string GetFeatureDisplayName(FeatureSettings feature)
        {
            if (feature == null)
            {
                return "Unknown Feature";
            }

            FeatureDescriptor descriptor = feature.FeatureDescriptor;
            if (descriptor == null || string.IsNullOrEmpty(descriptor.FeatureDisplayName))
            {
                return feature.name;
            }

            return descriptor.FeatureDisplayName;
        }

        private string GetFeatureDescription(FeatureSettings feature)
        {
            if (feature == null)
            {
                return string.Empty;
            }

            FeatureDescriptor descriptor = feature.FeatureDescriptor;
            return descriptor != null ? descriptor.FeatureDescription : string.Empty;
        }

        private void OnFeatureSettingsChanged(FeatureSettings feature)
        {
            if (m_descriptor == null || feature == null)
            {
                return;
            }

            FeatureSettingsObserver.NotifyFeatureChanged(m_descriptor, feature);
            RefreshStatusMessage();
        }

        private void BuildFooter(VisualElement host)
        {
            if (host == null)
            {
                return;
            }

            host.Clear();
            var statusUxml = LoadUxml(kStatusUxmlName);
            if (statusUxml == null)
            {
                return;
            }

            VisualElement statusRoot = statusUxml.CloneTree();
            host.Add(statusRoot);
            m_statusRoot = statusRoot.Q<VisualElement>("status-root");
            m_statusLabel = statusRoot.Q<Label>("status-text");
        }

        private static void BuildResourcesContent(SerializedObject resourcesObject, VisualElement container)
        {
            AddResourceRow(container, "Documentation", resourcesObject.FindProperty("m_documentation")?.stringValue);
            AddResourceRow(container, "Tutorials", resourcesObject.FindProperty("m_tutorials")?.stringValue);
            AddResourceRow(container, "Forum", resourcesObject.FindProperty("m_forum")?.stringValue);
            AddResourceRow(container, "Support", resourcesObject.FindProperty("m_support")?.stringValue);
            AddResourceRow(container, "Write Review", resourcesObject.FindProperty("m_writeReview")?.stringValue);
        }

        private static void AddResourceRow(VisualElement container, string labelText, string url)
        {
            var rowUxml = LoadUxml(kResourceRowUxmlName);
            if (rowUxml == null)
            {
                return;
            }

            var row = rowUxml.CloneTree();
            var label = row.Q<Label>("resource-label");
            var valueHost = row.Q<VisualElement>("resource-value");
            if (label != null)
            {
                label.text = labelText;
            }

            if (valueHost == null)
            {
                container.Add(row);
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                var empty = new Label("-");
                empty.style.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                valueHost.Add(empty);
            }
            else
            {
                var linkButton = new Button(() => Application.OpenURL(url))
                {
                    text = url
                };
                linkButton.AddToClassList("vb-link");
                linkButton.style.unityTextAlign = TextAnchor.MiddleLeft;
                linkButton.style.flexGrow = 1f;
                valueHost.Add(linkButton);
            }

            container.Add(row);
        }

        private void RefreshStatusMessage()
        {
            if (m_statusLabel == null)
            {
                return;
            }

            if (m_settings == null)
            {
                m_statusLabel.text = "Settings asset not loaded.";
                SetStatusClass(FeatureSettingsValidationStatus.Error);
                return;
            }

            FeatureSettingsValidationStatus maxStatus = FeatureSettingsValidationStatus.None;
            string message = "All settings look good.";

            List<FeatureSettings> features = PluginProductSettingsUtility.GetFeatureSettings(m_settings);
            foreach (FeatureSettings feature in features)
            {
                if (feature == null)
                {
                    continue;
                }

                if (feature is IFeatureSettingsValidator validator &&
                    validator.TryGetValidationMessage(out string validationMessage, out FeatureSettingsValidationStatus status) &&
                    status != FeatureSettingsValidationStatus.None &&
                    !string.IsNullOrEmpty(validationMessage))
                {
                    if (status > maxStatus)
                    {
                        maxStatus = status;
                        message = validationMessage;
                    }
                }
            }

            m_statusLabel.text = message;
            SetStatusClass(maxStatus);
        }

        private static void BuildInspectorExcludingScript(SerializedObject serializedObject, VisualElement container)
        {
            if (serializedObject == null || container == null)
            {
                return;
            }

            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath == "m_Script")
                {
                    enterChildren = false;
                    continue;
                }

                SerializedProperty property = iterator.Copy();
                var field = new PropertyField(property);
                field.BindProperty(property);
                container.Add(field);
                enterChildren = false;
            }
        }


        private static void UpdateFeatureRowState(
            VisualElement container,
            Label nameLabel,
            Label descriptionLabel,
            bool isEnabled)
        {
            container.EnableInClassList("vb-feature-disabled", !isEnabled);
            if (nameLabel != null)
            {
                nameLabel.EnableInClassList("vb-feature-name-disabled", !isEnabled);
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.EnableInClassList("vb-feature-description-disabled", !isEnabled);
            }
        }

        private static void InitializeToggleVisuals(Toggle toggle)
        {
            var checkmark = toggle.Q<VisualElement>("unity-checkmark");
            if (checkmark != null)
            {
                checkmark.style.display = DisplayStyle.None;
            }

            var label = toggle.Q<Label>("unity-label");
            if (label != null)
            {
                label.style.display = DisplayStyle.None;
            }
        }

        private static void UpdateToggleVisuals(Toggle toggle, bool isOn)
        {
            EnsureToggleTexturesLoaded();
            toggle.style.backgroundImage = isOn
                ? new StyleBackground(s_toggleOnTexture)
                : new StyleBackground(s_toggleOffTexture);
            toggle.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }

        private static void EnsureToggleTexturesLoaded()
        {
            if (s_toggleOnTexture == null)
            {
                s_toggleOnTexture = LoadTextureByName(kToggleOnTextureName);
            }

            if (s_toggleOffTexture == null)
            {
                s_toggleOffTexture = LoadTextureByName(kToggleOffTextureName);
            }
        }

        private static Texture2D LoadTextureByName(string textureName)
        {
            if (string.IsNullOrEmpty(textureName))
            {
                return null;
            }

            string[] guids = AssetDatabase.FindAssets($"t:Texture2D {textureName}");
            if (guids == null || guids.Length == 0)
            {
                return null;
            }

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.Equals(Path.GetFileNameWithoutExtension(path), textureName, StringComparison.OrdinalIgnoreCase))
                {
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            string fallbackPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(fallbackPath);
        }

        private void SetStatusClass(FeatureSettingsValidationStatus status)
        {
            if (m_statusRoot == null)
            {
                return;
            }

            m_statusRoot.RemoveFromClassList("vb-status-info");
            m_statusRoot.RemoveFromClassList("vb-status-warning");
            m_statusRoot.RemoveFromClassList("vb-status-error");

            switch (status)
            {
                case FeatureSettingsValidationStatus.Warning:
                    m_statusRoot.AddToClassList("vb-status-warning");
                    break;
                case FeatureSettingsValidationStatus.Error:
                    m_statusRoot.AddToClassList("vb-status-error");
                    break;
                default:
                    m_statusRoot.AddToClassList("vb-status-info");
                    break;
            }
        }

        private static StyleSheet LoadStyleSheet(string assetName)
        {
            return LoadEditorResource<StyleSheet>(assetName);
        }

        private static VisualTreeAsset LoadUxml(string assetName)
        {
            return LoadEditorResource<VisualTreeAsset>(assetName);
        }

        private static T LoadEditorResource<T>(string assetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            string filter = $"t:{typeof(T).Name} {assetName}";
            string[] guids = AssetDatabase.FindAssets(filter);
            if (guids == null || guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static VisualElement CreateCard(out VisualElement content)
        {
            content = null;
            var cardUxml = LoadUxml(kCardUxmlName);
            if (cardUxml == null)
            {
                return new VisualElement();
            }

            var card = cardUxml.CloneTree();
            content = card.Q<VisualElement>("card-content");
            return card;
        }
    }
}
