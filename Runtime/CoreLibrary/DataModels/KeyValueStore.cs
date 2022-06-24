using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.CoreLibrary
{
    public class KeyValueStore
    {
        #region Fields

        private     Dictionary<string, string>      m_dataCollection;

        private     string                          m_savePath;

        #endregion

        #region Constructors

        public KeyValueStore(string savePath)
        {
            // set properties
            m_dataCollection    = LoadDataFromPath(savePath) ?? new Dictionary<string, string>();
            m_savePath          = savePath;
        }

        #endregion

        #region Get value methods

        public bool GetBool(string key, bool defaultValue = default)
        {
            if (m_dataCollection.TryGetValue(key, out string value))
            {
                return bool.Parse(value);
            }
            return defaultValue;
        }

        public long GetLong(string key, long defaultValue = default)
        {
            if (m_dataCollection.TryGetValue(key, out string value))
            {
                return long.Parse(value);
            }
            return defaultValue;
        }

        public double GetDouble(string key, double defaultValue = default)
        {
            if (m_dataCollection.TryGetValue(key, out string value))
            {
                return double.Parse(value);
            }
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = default)
        {
            if (m_dataCollection.TryGetValue(key, out string value))
            {
                return value;
            }
            return defaultValue;
        }

        public byte[] GetByteArray(string key, byte[] defaultValue = default)
        {
            if (m_dataCollection.TryGetValue(key, out string value))
            {
                return System.Convert.FromBase64String(value);
            }
            return defaultValue;
        }

        #endregion

        #region Set value methods

        public void SetBool(string key, bool value)
        {
            // save value
            m_dataCollection[key]   = value.ToString();
        }

        public void SetLong(string key, long value)
        {
            // save value
            m_dataCollection[key]   = value.ToString();
        }

        public void SetDouble(string key, double value)
        {
            // save value
            m_dataCollection[key]   = value.ToString();
        }

        public void SetString(string key, string value)
        {
            // save value
            m_dataCollection[key]   = value;
        }

        public void SetByteArray(string key, byte[] value)
        {
            // save value
            m_dataCollection[key]   = System.Convert.ToBase64String(value);
        }

        #endregion

        #region Misc methods

        public void Synchronize()
        {
            string  jsonContent = ExternalServiceProvider.JsonServiceProvider.ToJson(m_dataCollection);
            IOServices.CreateFile(m_savePath, jsonContent);
        }

        public void Clear()
        {
            m_dataCollection.Clear();
            Synchronize();
        }

        public bool RemoveKey(string key)
        {
            return m_dataCollection.Remove(key);
        }

        #endregion

        #region Private methods

        private Dictionary<string, string> LoadDataFromPath(string path)
        {
            if (!IOServices.FileExists(path)) return null;

            var     jsonContent = IOServices.ReadFile(path);
            return ExternalServiceProvider.JsonServiceProvider.FromJson(jsonContent) as Dictionary<string, string>;
        }

        #endregion
    }
}