using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace HS.Utils
{
    public static class SerializerUtils
    {
        #region Serializer
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
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToSerializeBytesXML<T>(this T obj)
        {
            using (MemoryStream ms = ToSerializeBytesXMLStream(obj))
                return ms.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MemoryStream ToSerializeBytesXMLStream<T>(this T obj)
        {
            MemoryStream ms = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            xs.Serialize(ms, obj);
            ms.Position = 0;
            return ms;
        }
        #endregion


        #region Deserializer
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="CheckLength"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T DeserializeFromByte<T>(this byte[] data, bool CheckLength = true)
        {
            //구조체 사이즈 
            int size = Marshal.SizeOf(typeof(T));

            if (CheckLength && size > data.Length)
            {
                throw new Exception(string.Format("Buffer length must be same or bigger than T ({0}) size", typeof(T).Name));
            }

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T DeserializeFromByteXML<T>(this byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return DeserializeFromByteXML<T>(stream);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static T DeserializeFromByteXML<T>(System.IO.Stream Data)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(Data);
        }
        #endregion
    }
}
