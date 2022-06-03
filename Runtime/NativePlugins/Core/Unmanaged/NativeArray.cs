using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VoxelBusters.CoreLibrary.NativePlugins
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArray
    {
        #region Properties

        public IntPtr Pointer
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public T[] GetStructArray<T>() where T : struct
        {
            T[] structArray = new T[Length];
            int structSize = Marshal.SizeOf(typeof(T));

            for(int i=0; i<Length; i++)
            {
                structArray[i] = MarshalUtility.PtrToStructure<T>(new IntPtr(Pointer.ToInt64() + (i * structSize)));
            }
            return structArray;
        }

        public string[] GetStringArray()
        {
            //Marshal ptr to array
            return MarshalUtility.CreateStringArray(Pointer, Length);
        }

        #endregion
    }
}