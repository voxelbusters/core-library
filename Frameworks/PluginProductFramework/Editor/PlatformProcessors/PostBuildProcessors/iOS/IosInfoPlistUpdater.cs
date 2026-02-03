#if UNITY_IOS || UNITY_TVOS
using System.Collections.Generic;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework;

namespace VoxelBusters.CoreLibrary.Frameworks.PluginProductFramework.Editor
{
    internal static class IosInfoPlistUpdater
    {
        internal static void Update(string buildPath, List<IosPlatformConfiguration> configs)
        {
            string plistPath = Path.Combine(buildPath, IosPostBuildConstants.InfoPlistFileName);
            if (!File.Exists(plistPath))
            {
                return;
            }

            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            var root = plist.root;

            foreach (var config in configs)
            {
                var entries = config.InfoPlistEntries;
                if (entries == null)
                {
                    continue;
                }

                foreach (var entry in entries)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.Key))
                    {
                        continue;
                    }

                    SetPlistStringWithWarning(root, entry.Key, entry.Value ?? string.Empty);
                }

                var urlSchemes = config.UrlSchemes;
                if (urlSchemes != null && urlSchemes.Length > 0)
                {
                    PlistElementArray urlTypes = null;
                    if (root.values.TryGetValue(IosPostBuildConstants.PlistUrlTypesKey, out PlistElement element))
                    {
                        urlTypes = element as PlistElementArray;
                    }
                    if (urlTypes == null)
                    {
                        urlTypes = root.CreateArray(IosPostBuildConstants.PlistUrlTypesKey);
                    }

                    var existingSchemes = CollectUrlSchemes(urlTypes);
                    foreach (var scheme in urlSchemes)
                    {
                        if (scheme == null || string.IsNullOrEmpty(scheme.Scheme))
                        {
                            continue;
                        }

                        if (existingSchemes.Contains(scheme.Scheme))
                        {
                            continue;
                        }

                        var dict = urlTypes.AddDict();
                        var schemesArray = dict.CreateArray(IosPostBuildConstants.PlistUrlSchemesKey);
                        schemesArray.AddString(scheme.Scheme);
                        existingSchemes.Add(scheme.Scheme);
                    }
                }
            }

            plist.WriteToFile(plistPath);
        }

        private static void SetPlistStringWithWarning(PlistElementDict root, string key, string value)
        {
            if (root.values.TryGetValue(key, out PlistElement existing))
            {
                string existingValue = existing.AsString();
                if (!string.Equals(existingValue, value))
                {
                    Debug.LogWarning($"[IosPlatformPostBuildProcessor] Info.plist key '{key}' value replaced from '{existingValue}' to '{value}'.");
                }
            }

            root.SetString(key, value);
        }

        private static HashSet<string> CollectUrlSchemes(PlistElementArray urlTypes)
        {
            var schemes = new HashSet<string>(System.StringComparer.Ordinal);
            if (urlTypes == null)
            {
                return schemes;
            }

            var urlTypeEntries = urlTypes.values;
            for (int i = 0; i < urlTypeEntries.Count; i++)
            {
                var dict = urlTypeEntries[i] as PlistElementDict;
                if (dict == null)
                {
                    continue;
                }

                if (!dict.values.TryGetValue(IosPostBuildConstants.PlistUrlSchemesKey, out PlistElement element))
                {
                    continue;
                }

                var schemesArray = element as PlistElementArray;
                if (schemesArray == null)
                {
                    continue;
                }

                var schemeElements = schemesArray.values;
                for (int j = 0; j < schemeElements.Count; j++)
                {
                    string scheme = schemeElements[j].AsString();
                    if (!string.IsNullOrEmpty(scheme))
                    {
                        schemes.Add(scheme);
                    }
                }
            }

            return schemes;
        }
    }
}
#endif