using System;
using System.IO;
using System.Linq;
using System.Xml;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal sealed class AndroidManifestDocument
    {
        private readonly XmlDocument m_document;
        private readonly string m_path;
        private readonly XmlNamespaceManager m_namespaceManager;

        private const string kAndroidNs = "http://schemas.android.com/apk/res/android";
        private const string kToolsNs = "http://schemas.android.com/tools";

        public AndroidManifestDocument(string path, string packageName)
        {
            m_path = path;
            m_document = new XmlDocument();

            var declaration = m_document.CreateXmlDeclaration("1.0", "UTF-8", null);
            m_document.AppendChild(declaration);
            m_document.AppendChild(m_document.CreateComment("DONT MODIFY HERE. THIS FILE IS AUTO GENERATED."));

            var root = m_document.CreateElement("manifest");
            m_document.AppendChild(root);

            m_namespaceManager = new XmlNamespaceManager(m_document.NameTable);
            m_namespaceManager.AddNamespace("android", kAndroidNs);
            m_namespaceManager.AddNamespace("tools", kToolsNs);

            EnsureNamespaceDeclared("android", kAndroidNs);
            EnsureNamespaceDeclared("tools", kToolsNs);

            if (!string.IsNullOrEmpty(packageName))
            {
                root.SetAttribute("package", packageName);
            }

            var application = m_document.CreateElement("application");
            root.AppendChild(application);
        }

        public void Save()
        {
            string directory = Path.GetDirectoryName(m_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            m_document.Save(m_path);
        }

        public void AddUsesPermission(string name, string maxSdkVersion)
        {
            if (HasNodeWithName("/manifest/uses-permission", name))
            {
                return;
            }

            var element = m_document.CreateElement("uses-permission");
            element.Attributes.Append(CreateAndroidAttribute("name", name));
            if (!string.IsNullOrEmpty(maxSdkVersion))
            {
                element.Attributes.Append(CreateAndroidAttribute("maxSdkVersion", maxSdkVersion));
            }
            m_document.DocumentElement?.AppendChild(element);
        }

        public void AddManifestAttributes(AndroidManifestAttribute[] attributes)
        {
            var root = m_document.DocumentElement;
            if (root == null || attributes == null)
            {
                return;
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute == null || string.IsNullOrEmpty(attribute.Name))
                {
                    continue;
                }

                SetAttribute(root, attribute.Name, attribute.Value);
            }
        }

        public void AddApplicationAttributes(AndroidManifestAttribute[] attributes)
        {
            var application = GetApplicationElement();
            if (application == null || attributes == null)
            {
                return;
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute == null || string.IsNullOrEmpty(attribute.Name))
                {
                    continue;
                }

                SetAttribute(application, attribute.Name, attribute.Value);
            }
        }

        public void AddActivities(AndroidManifestActivity[] activities)
        {
            var application = GetApplicationElement();
            if (application == null || activities == null)
            {
                return;
            }

            for (int i = 0; i < activities.Length; i++)
            {
                var activity = activities[i];
                if (activity == null || string.IsNullOrEmpty(activity.Name))
                {
                    continue;
                }

                if (HasNodeWithName("/manifest/application/activity", activity.Name))
                {
                    continue;
                }

                var element = m_document.CreateElement("activity");
                element.Attributes.Append(CreateAndroidAttribute("name", activity.Name));
                AddAttributes(element, activity.Attributes, "android:name");
                AddIntentFilters(element, activity.IntentFilters);
                application.AppendChild(element);
            }
        }

        public void AddProviders(AndroidManifestProvider[] providers)
        {
            var application = GetApplicationElement();
            if (application == null || providers == null)
            {
                return;
            }

            for (int i = 0; i < providers.Length; i++)
            {
                var provider = providers[i];
                if (provider == null || string.IsNullOrEmpty(provider.Name))
                {
                    continue;
                }

                if (HasNodeWithName("/manifest/application/provider", provider.Name))
                {
                    continue;
                }

                var element = m_document.CreateElement("provider");
                element.Attributes.Append(CreateAndroidAttribute("name", provider.Name));
                AddAttributes(element, provider.Attributes, "android:name");
                application.AppendChild(element);
            }
        }

        public void AddServices(AndroidManifestService[] services)
        {
            var application = GetApplicationElement();
            if (application == null || services == null)
            {
                return;
            }

            for (int i = 0; i < services.Length; i++)
            {
                var service = services[i];
                if (service == null || string.IsNullOrEmpty(service.Name))
                {
                    continue;
                }

                if (HasNodeWithName("/manifest/application/service", service.Name))
                {
                    continue;
                }

                var element = m_document.CreateElement("service");
                element.Attributes.Append(CreateAndroidAttribute("name", service.Name));
                AddAttributes(element, service.Attributes, "android:name");
                AddIntentFilters(element, service.IntentFilters);
                application.AppendChild(element);
            }
        }

        public void AddReceivers(AndroidManifestReceiver[] receivers)
        {
            var application = GetApplicationElement();
            if (application == null || receivers == null)
            {
                return;
            }

            for (int i = 0; i < receivers.Length; i++)
            {
                var receiver = receivers[i];
                if (receiver == null || string.IsNullOrEmpty(receiver.Name))
                {
                    continue;
                }

                if (HasNodeWithName("/manifest/application/receiver", receiver.Name))
                {
                    continue;
                }

                var element = m_document.CreateElement("receiver");
                element.Attributes.Append(CreateAndroidAttribute("name", receiver.Name));
                AddAttributes(element, receiver.Attributes, "android:name");
                AddIntentFilters(element, receiver.IntentFilters);
                application.AppendChild(element);
            }
        }

        public void AddUsesFeature(string name, bool required)
        {
            if (HasNodeWithName("/manifest/uses-feature", name))
            {
                return;
            }

            var element = m_document.CreateElement("uses-feature");
            element.Attributes.Append(CreateAndroidAttribute("name", name));
            element.Attributes.Append(CreateAndroidAttribute("required", required ? "true" : "false"));
            m_document.DocumentElement?.AppendChild(element);
        }

        public void AddMetaData(string name, string value)
        {
            var application = GetApplicationElement();
            if (application == null)
            {
                return;
            }

            if (HasNodeWithName("/manifest/application/meta-data", name))
            {
                return;
            }

            var element = m_document.CreateElement("meta-data");
            element.Attributes.Append(CreateAndroidAttribute("name", name));
            if (!string.IsNullOrEmpty(value))
            {
                element.Attributes.Append(CreateAndroidAttribute("value", value));
            }
            application.AppendChild(element);
        }

        public void AddQuery(string action, string scheme, string host, string path)
        {
            var queries = GetOrCreateQueriesElement();
            if (queries == null)
            {
                return;
            }

            if (HasQuery(action, scheme, host, path))
            {
                return;
            }

            var intent = m_document.CreateElement("intent");
            var actionElement = m_document.CreateElement("action");
            actionElement.Attributes.Append(CreateAndroidAttribute("name", action));
            intent.AppendChild(actionElement);

            if (!string.IsNullOrEmpty(scheme) || !string.IsNullOrEmpty(host) || !string.IsNullOrEmpty(path))
            {
                var dataElement = m_document.CreateElement("data");
                if (!string.IsNullOrEmpty(scheme))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("scheme", scheme));
                }
                if (!string.IsNullOrEmpty(host))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("host", host));
                }
                if (!string.IsNullOrEmpty(path))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("path", path));
                }
                intent.AppendChild(dataElement);
            }

            queries.AppendChild(intent);
        }

        private void AddIntentFilters(XmlElement parent, AndroidManifestIntentFilter[] filters)
        {
            if (filters == null)
            {
                return;
            }

            for (int i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];
                if (filter == null)
                {
                    continue;
                }

                var element = m_document.CreateElement("intent-filter");
                if (!string.IsNullOrEmpty(filter.Label))
                {
                    element.Attributes.Append(CreateAndroidAttribute("label", filter.Label));
                }
                if (filter.AutoVerify)
                {
                    element.Attributes.Append(CreateAndroidAttribute("autoVerify", "true"));
                }

                AddActions(element, filter.Actions);
                AddCategories(element, filter.Categories);
                AddDataElements(element, filter.Data);

                parent.AppendChild(element);
            }
        }

        private void AddActions(XmlElement intentFilter, string[] actions)
        {
            if (actions == null)
            {
                return;
            }

            for (int i = 0; i < actions.Length; i++)
            {
                string action = actions[i];
                if (string.IsNullOrEmpty(action))
                {
                    continue;
                }

                var actionElement = m_document.CreateElement("action");
                actionElement.Attributes.Append(CreateAndroidAttribute("name", action));
                intentFilter.AppendChild(actionElement);
            }
        }

        private void AddCategories(XmlElement intentFilter, string[] categories)
        {
            if (categories == null)
            {
                return;
            }

            for (int i = 0; i < categories.Length; i++)
            {
                string category = categories[i];
                if (string.IsNullOrEmpty(category))
                {
                    continue;
                }

                var categoryElement = m_document.CreateElement("category");
                categoryElement.Attributes.Append(CreateAndroidAttribute("name", category));
                intentFilter.AppendChild(categoryElement);
            }
        }

        private void AddDataElements(XmlElement intentFilter, AndroidManifestData[] data)
        {
            if (data == null)
            {
                return;
            }

            for (int i = 0; i < data.Length; i++)
            {
                var item = data[i];
                if (item == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(item.Scheme) &&
                    string.IsNullOrEmpty(item.Host) &&
                    string.IsNullOrEmpty(item.Path) &&
                    string.IsNullOrEmpty(item.PathPrefix))
                {
                    continue;
                }

                var dataElement = m_document.CreateElement("data");
                if (!string.IsNullOrEmpty(item.Scheme))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("scheme", item.Scheme));
                }
                if (!string.IsNullOrEmpty(item.Host))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("host", item.Host));
                }
                if (!string.IsNullOrEmpty(item.Path))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("path", item.Path));
                }
                if (!string.IsNullOrEmpty(item.PathPrefix))
                {
                    dataElement.Attributes.Append(CreateAndroidAttribute("pathPrefix", item.PathPrefix));
                }
                intentFilter.AppendChild(dataElement);
            }
        }

        private bool HasNodeWithName(string xpath, string name)
        {
            var nodes = m_document.SelectNodes(xpath, m_namespaceManager);
            if (nodes == null)
            {
                return false;
            }

            return nodes.Cast<XmlNode>().Any(node =>
            {
                var attr = node.Attributes?["android:name"];
                return attr != null && attr.Value == name;
            });
        }

        private void AddAttributes(XmlElement element, AndroidManifestAttribute[] attributes, string skipName)
        {
            if (attributes == null)
            {
                return;
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute == null || string.IsNullOrEmpty(attribute.Name))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(skipName) && string.Equals(attribute.Name, skipName))
                {
                    continue;
                }

                SetAttribute(element, attribute.Name, attribute.Value);
            }
        }

        private void SetAttribute(XmlElement element, string name, string value)
        {
            if (element == null || string.IsNullOrEmpty(name))
            {
                return;
            }

            if (name.StartsWith("xmlns:", StringComparison.Ordinal))
            {
                element.SetAttribute(name, value ?? string.Empty);
                return;
            }

            XmlAttribute attribute = CreateAttributeWithNamespace(name, value ?? string.Empty);
            element.Attributes.RemoveNamedItem(attribute.Name, attribute.NamespaceURI);
            element.Attributes.Append(attribute);
        }

        private bool HasQuery(string action, string scheme, string host, string path)
        {
            var intents = m_document.SelectNodes("/manifest/queries/intent", m_namespaceManager);
            if (intents == null)
            {
                return false;
            }

            foreach (XmlNode intent in intents)
            {
                var actionNode = intent.SelectSingleNode("action", m_namespaceManager);
                var actionAttr = actionNode?.Attributes?["android:name"];
                if (actionAttr == null || actionAttr.Value != action)
                {
                    continue;
                }

                var dataNode = intent.SelectSingleNode("data", m_namespaceManager);
                string existingScheme = dataNode?.Attributes?["android:scheme"]?.Value;
                string existingHost = dataNode?.Attributes?["android:host"]?.Value;
                string existingPath = dataNode?.Attributes?["android:path"]?.Value;

                if (string.Equals(existingScheme, scheme) &&
                    string.Equals(existingHost, host) &&
                    string.Equals(existingPath, path))
                {
                    return true;
                }
            }

            return false;
        }

        private XmlElement GetApplicationElement()
        {
            var application = m_document.SelectSingleNode("/manifest/application", m_namespaceManager) as XmlElement;
            if (application != null)
            {
                return application;
            }

            var root = m_document.DocumentElement;
            if (root == null)
            {
                return null;
            }

            application = m_document.CreateElement("application");
            root.AppendChild(application);
            return application;
        }

        private XmlElement GetOrCreateQueriesElement()
        {
            var queries = m_document.SelectSingleNode("/manifest/queries", m_namespaceManager) as XmlElement;
            if (queries != null)
            {
                return queries;
            }

            queries = m_document.CreateElement("queries");
            m_document.DocumentElement?.AppendChild(queries);
            return queries;
        }

        private XmlAttribute CreateAndroidAttribute(string name, string value)
        {
            var attr = m_document.CreateAttribute("android", name, kAndroidNs);
            attr.Value = value;
            return attr;
        }

        private XmlAttribute CreateAttributeWithNamespace(string name, string value)
        {
            string prefix = null;
            string localName = name;
            int index = name.IndexOf(':');
            if (index > 0 && index < name.Length - 1)
            {
                prefix = name.Substring(0, index);
                localName = name.Substring(index + 1);
            }

            XmlAttribute attr;
            if (prefix == "android")
            {
                EnsureNamespaceDeclared("android", kAndroidNs);
                attr = m_document.CreateAttribute("android", localName, kAndroidNs);
            }
            else if (prefix == "tools")
            {
                EnsureNamespaceDeclared("tools", kToolsNs);
                attr = m_document.CreateAttribute("tools", localName, kToolsNs);
            }
            else
            {
                attr = m_document.CreateAttribute(localName);
            }
            attr.Value = value;
            return attr;
        }

        private void EnsureNamespaceDeclared(string prefix, string uri)
        {
            var root = m_document.DocumentElement;
            if (root == null)
            {
                return;
            }

            string attributeName = "xmlns:" + prefix;
            if (!root.HasAttribute(attributeName))
            {
                root.SetAttribute(attributeName, uri);
            }
        }
    }
}
