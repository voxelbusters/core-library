using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    public static class CollectionUtility
    {
        #region Extension methods

        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return (dict == null) || (dict.Count == 0);
        }

        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return (list == null) || (list.Count == 0);
        }

        public static bool AddUnique<T>(this List<T> list, T item)
        {
            if (null == list) return false;

            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        #endregion
    }
}