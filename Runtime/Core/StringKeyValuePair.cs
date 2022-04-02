using System;

namespace VoxelBusters.CoreLibrary
{
    [Serializable]
    public class StringKeyValuePair : SerializableKeyValuePair<string, string>
    {
        #region Constructors

        public StringKeyValuePair(string key, string value)
        {
            // set properties
            Key     = key;
            Value   = value;
        }

        #endregion
    }
}