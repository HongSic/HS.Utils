using System;
using System.Runtime.InteropServices;

namespace HS.Utils
{
    public static class SerializerUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToSerializeBytes(this object obj)
        {
            //구조체 사이즈 
            int iSize = Marshal.SizeOf(obj);

            //사이즈 만큼 메모리 할당 받기
            byte[] arr = new byte[iSize];

            IntPtr ptr = Marshal.AllocHGlobal(iSize);
            //구조체 주소값 가져오기
            Marshal.StructureToPtr(obj, ptr, false);
            //메모리 복사 
            Marshal.Copy(ptr, arr, 0, iSize);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="CheckLength"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T DeserializeFromByte<T>(this byte[] buffer, bool CheckLength = true)
        {
            //구조체 사이즈 
            int size = Marshal.SizeOf(typeof(T));

            if (CheckLength && size > buffer.Length)
            {
                throw new Exception(string.Format("Buffer length must be same or bigger than T ({0}) size", typeof(T).Name));
            }

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, ptr, size);
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return obj;
        }
    }
}
