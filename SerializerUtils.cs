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
        public static string ToSerializeXML<T>(this T obj)
        {
            using (MemoryStream ms = ToSerializeBytesXMLStream(obj))
            using (StreamReader sr = new StreamReader(ms)) return sr.ReadToEnd();
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
        /// <param name="XMLData"></param>
        /// <returns></returns>
        public static T DeserializeFromXML<T>(this string XMLData) { return (T)DeserializeFromXML(XMLData, typeof(T)); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="XMLData"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static object DeserializeFromXML(this string XMLData, Type Type)
        {
            XmlSerializer xs = new XmlSerializer(Type);
            using (TextReader reader = new StringReader(XMLData))
                return xs.Deserialize(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <param name="Type"></param>
        /// <param name="CheckLength"></param>
        /// <returns></returns>
        public static T DeserializeFromByte<T>(this byte[] Data, bool CheckLength = true) { return (T)DeserializeFromByte(Data, typeof(T), CheckLength); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Type"></param>
        /// <param name="CheckLength"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static object DeserializeFromByte(this byte[] Data, Type Type, bool CheckLength = true)
        {
            //구조체 사이즈 
            int size = Marshal.SizeOf(Type);

            if (CheckLength && size > Data.Length)
            {
                throw new Exception(string.Format("Buffer length must be same or bigger than T ({0}) size", Type.Name));
            }

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(Data, 0, ptr, size);
            object obj = Marshal.PtrToStructure(ptr, Type);
            Marshal.FreeHGlobal(ptr);
            return obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static T DeserializeFromByteXML<T>(this byte[] Data) { return (T)DeserializeFromByteXML(Data, typeof(T)); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static object DeserializeFromByteXML(this byte[] Data, Type Type)
        {
            using (var stream = new MemoryStream(Data))
                return DeserializeFromByteXML(stream, Type);
        }
        /// <summary>
        /// /
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static T DeserializeFromByteXML<T>(System.IO.Stream Data) { return (T)DeserializeFromByteXML(Data, typeof(T)); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static object DeserializeFromByteXML(System.IO.Stream Data, Type Type)
        {
            XmlSerializer xs = new XmlSerializer(Type);
            return xs.Deserialize(Data);
        }
        #endregion
    }
}
