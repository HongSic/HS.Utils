using HS.Utils.Text;
using System;
using System.Collections.Generic;

namespace HS.Utils.Convert
{
    public static class ConvertUtils
    {
#if NETCORE || NETSTANDARD
        public static readonly Type TYPE_NULLABLE = typeof(Nullable);
#endif
        public static readonly Type TYPE_STRING = typeof(string);
        public static readonly Type TYPE_CHAR = typeof(char);
        public static readonly Type TYPE_ENUM = typeof(Enum);
        public static readonly Type TYPE_BOOL = typeof(bool);
        public static readonly Type TYPE_BYTE = typeof(byte);
        public static readonly Type TYPE_SBYTE = typeof(sbyte);
        public static readonly Type TYPE_SHORT = typeof(short);
        public static readonly Type TYPE_USHORT = typeof(ushort);
        public static readonly Type TYPE_INT = typeof(int);
        public static readonly Type TYPE_UINT = typeof(uint);
        public static readonly Type TYPE_LONG = typeof(long);
        public static readonly Type TYPE_ULONG = typeof(ulong);
        public static readonly Type TYPE_DECIMAL = typeof(decimal);
        public static readonly Type TYPE_DOUBLE = typeof(double);
        public static readonly Type TYPE_FLOAT = typeof(float);
        public static readonly Type TYPE_DATETIME = typeof(DateTime);
        public static readonly Type TYPE_DATA = typeof(byte[]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static T ConvertValue<T>(this object Value, ConvertType Type = ConvertType.Auto) { return (T)ConvertValue(Value, typeof(T), Type); }
#if NETSTANDARD || NETCORE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool TryConvertValue<T>(this object Value, out T Converted) => TryConvertValue<T>(Value, ConvertType.Auto, out Converted);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool TryConvertValue<T>(this object Value, ConvertType Type, out T Converted)
        {
            if (Value == null)
            {
                Converted = default;
                return false;
            }

            try { Converted = ConvertValue<T>(Value, Type); return true; }
            catch { Converted = default; return false; }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="DestinationType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool TryConvertValue(this object Value, Type DestinationType, out object Converted) { return TryConvertValue(Value, DestinationType, ConvertType.Auto, out Converted); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OriginalValue"></param>
        /// <param name="DestinationType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool TryConvertValue(this object OriginalValue, Type DestinationType, ConvertType Type, out object Value) 
        {
            Value = null;
            if (Value == null) return false;

            try { Value = ConvertValue(OriginalValue, DestinationType, Type); return true; }
            catch { return false; }
        }
        /// <summary>
        /// Convert to various type. (Not convert when Value is null)
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="DestinationType"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static object ConvertValue(this object Value, Type DestinationType, ConvertType Type = ConvertType.Auto)
        {
            try
            {
                if (Value != null)
                {
                    Type type_col = Value.GetType();

                    if (DestinationType != type_col)
                    {
                        if (type_col == typeof(DBNull)) Value = null;
                        else
                        {

#if NETCORE || NETSTANDARD
                            if (DestinationType.Namespace == TYPE_NULLABLE.Namespace &&
                                DestinationType.Name.StartsWith(TYPE_NULLABLE.Name)) return ConvertValue(Value, Nullable.GetUnderlyingType(DestinationType), Type);
#endif
                            if (Type == ConvertType.XML) Value = Value.ToString().DeserializeFromXML(DestinationType);
                            else if (Type == ConvertType.JSON) Value = JSONUtils.DeserializeJSON_NS(Value.ToString(), DestinationType);
                            else if (DestinationType == TYPE_BOOL) Value = System.Convert.ToBoolean(Value);
                            else if (DestinationType == TYPE_CHAR) Value = System.Convert.ToChar(Value);
                            else if (DestinationType == TYPE_STRING) Value = System.Convert.ToString(Value)?.Trim();
                            else if (DestinationType == TYPE_BYTE) Value = System.Convert.ToByte(Value);
                            else if (DestinationType == TYPE_SBYTE) Value = System.Convert.ToSByte(Value);
                            else if (DestinationType == TYPE_USHORT) Value = System.Convert.ToInt16(Value);
                            else if (DestinationType == TYPE_SHORT) Value = System.Convert.ToUInt16(Value);
                            else if (DestinationType == TYPE_INT) Value = System.Convert.ToInt32(Value);
                            else if (DestinationType == TYPE_UINT) Value = System.Convert.ToUInt32(Value);
                            else if (DestinationType == TYPE_LONG) Value = System.Convert.ToInt64(Value);
                            else if (DestinationType == TYPE_ULONG) Value = System.Convert.ToUInt64(Value);
                            else if (DestinationType == TYPE_FLOAT) Value = System.Convert.ToSingle(Value);
                            else if (DestinationType == TYPE_DOUBLE) Value = System.Convert.ToDouble(Value);
                            else if (DestinationType == TYPE_DATETIME) Value = System.Convert.ToDateTime(Value);
                            else
                            {
                                if (DestinationType.BaseType == TYPE_ENUM)
                                {
                                    if (type_col == TYPE_STRING)
                                    {
                                        string strval = ((string)Value)?.Trim();
                                        //Enum 이름에 숫자를 사용할 수 없으므로 숫자는 실제 값으로 취급
                                        if (int.TryParse(strval, out int Result)) Value = Result;
                                        else Value = Enum.Parse(DestinationType, strval);
                                    }
                                    else Value = Enum.ToObject(DestinationType, Value);
                                }
                            }
                        }
                    }
                    else if (type_col == TYPE_STRING) return ((string)Value)?.Trim();
                }
                return Value;
            }
            catch (Exception ex) { throw new ConvertFailException(ex); }
        }

        #region Clone List
        public static List<T> CloneList<T>(this IList<T> list, bool deepClone = false)
        {
            var _list = new List<T>(list.Count);
            _list.AddRange(list);
            return _list;
        }
        public static List<T> DeepCloneList<T>(this IList<T> list) where T : ICloneable
        {
            var _list = new List<T>(list.Count);
            for (int i = 0; i < _list.Count; i++)
                _list.Add((T)list[i].Clone());
            return _list;
            //return list.Select(item => (T)item.Clone()).ToList();
        }
        #endregion
    }
}
