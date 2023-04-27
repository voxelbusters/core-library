using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.NativePlugins
{
    public class NativeFeatureRuntimePackage
    {
        #region Properties

        private RuntimePlatform[] Platforms { get; set; }

        public string Assembly { get; private set; }

        public string Namespace { get; private set; }

        public string NativeInterfaceType { get; private set; }

        public string[] BindingTypes { get; private set; }

        #endregion

        #region Constructors

        private NativeFeatureRuntimePackage(RuntimePlatform[] platforms, string assembly,
            string ns, string nativeInterfaceType, string[] bindingTypes = null)
        {
            // Set properties
            Platforms           = platforms;
            Assembly            = assembly;
            Namespace           = ns;
            NativeInterfaceType = GetTypeFullName(ns, nativeInterfaceType);
            BindingTypes        = (bindingTypes != null)
                ? System.Array.ConvertAll(bindingTypes, (type) => GetTypeFullName(ns, type))
                : new string[0];
        }

        #endregion

        #region Static methods

        public static NativeFeatureRuntimePackage Generic(string assembly, string ns,
            string nativeInterfaceType, string[] bindingTypes = null)
        {
            return new NativeFeatureRuntimePackage(
                platforms: null,
                assembly: assembly,
                ns: ns,
                nativeInterfaceType: nativeInterfaceType,
                bindingTypes: bindingTypes);
        }

        public static NativeFeatureRuntimePackage Android(string assembly, string ns,
            string nativeInterfaceType, string[] bindingTypes = null)
        {
            return new NativeFeatureRuntimePackage(
                platforms: new [] { RuntimePlatform.Android },
                assembly: assembly,
                ns: ns,
                nativeInterfaceType: nativeInterfaceType,
                bindingTypes: bindingTypes);
        }

        public static NativeFeatureRuntimePackage IPhonePlayer(string assembly, string ns,
            string nativeInterfaceType, string[] bindingTypes = null)
        {
            return new NativeFeatureRuntimePackage(
                platforms: new [] { RuntimePlatform.IPhonePlayer },
                assembly: assembly,
                ns: ns,
                nativeInterfaceType: nativeInterfaceType,
                bindingTypes: bindingTypes);
        }

        public static NativeFeatureRuntimePackage iOS(string assembly, string ns,
            string nativeInterfaceType, string[] bindingTypes = null)
        {
            return new NativeFeatureRuntimePackage(
                platforms: new [] { RuntimePlatform.IPhonePlayer, RuntimePlatform.tvOS },
                assembly: assembly,
                ns: ns,
                nativeInterfaceType: nativeInterfaceType,
                bindingTypes: bindingTypes);
        }

        private static string GetTypeFullName(string ns, string type) => $"{ns}.{type}";

        #endregion

        #region Public methods

        public System.Type[] GetBindingTypeReferences()
        {
            var     assembly    = ReflectionUtility.FindAssemblyWithName(Assembly);
            return System.Array.ConvertAll(BindingTypes, (item) => assembly.GetType(item));
        }

        public bool SupportsPlatform(RuntimePlatform platform)
        {
            return (Platforms != null) && System.Array.Exists(Platforms, (value) => (value == platform));
        }

        #endregion
    }
}