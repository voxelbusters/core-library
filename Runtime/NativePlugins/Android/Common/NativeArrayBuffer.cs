#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.NativePlugins.Android
{
    public class NativeArrayBuffer<T> : NativeAndroidJavaObjectWrapper
    {
        public NativeArrayBuffer(AndroidJavaObject androidJavaObject) : base("com.voxelbusters.android.essentialkit.common.ArrayBuffer", androidJavaObject)
        {
        }

        public int Size()
        {
            return m_nativeObject.Call<int>("size");
        }

        public T Get(int index)
        {
            if (m_nativeObject == null)
                return default(T);

            return m_nativeObject.Call<T>("get", index);
        }

        public T[] GetArray()
        {
            if (NativeObject == null)
                return default(T[]);

            List<T> list = new List<T>();
            int size = Size();
            for (int i = 0; i < size; i++)
            {
                T each = Get(i);
                list.Add(each);
            }

            return list.ToArray();
        }
    }
}
#endif