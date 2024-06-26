﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HS.Utils.Text
{
    public static class JSONUtils
    {
        public static string EncodeJSON(this string text)
        {
            if (text == null || text.Length == 0) return "";

            int i;
            int len = text.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            string t;

            for (i = 0; i < len; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + string.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string JSONException(this Exception ex)
        {
            StringBuilder sb = new StringBuilder("{\"result\":\"fail\",\"message\":\"내부 오류로 불러오는데 실패하였습니다.\",");
            sb.Append("\"exception\":{\"message\":\"").Append(ex.Message.EncodeJSON());
            sb.Append("\"}}");
            return sb.ToString();
        }
        public static string JSONException(this Exception ex, string Message)
        {
            StringBuilder sb = new StringBuilder("{\"result\":\"fail\",\"message\":\"");
            sb.Append(Message.EncodeJSON()).Append("\",");
            sb.Append("\"exception\":{\"message\":\"").Append(ex.Message.EncodeJSON());
            sb.Append("\"}}");
            return sb.ToString();
        }

        /// <summary>
        /// 객체를 JSON 체계 문자로 변환
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="AutoQuote">true 면 자동쌍따옴표 false 면 항상 쌍따옴표</param>
        /// <returns></returns>
        public static string ToStringForJSON(this object Data, bool AutoQuote = true)
        {
            if (Data == null) return "null";
            else if (!AutoQuote) return string.Format("\"{0}\"", Data.ToString().EncodeJSON());
            else if (Data.GetType() == typeof(byte) |
                    Data.GetType() == typeof(sbyte) |
                    Data.GetType() == typeof(short) |
                    Data.GetType() == typeof(ushort) |
                    Data.GetType() == typeof(int) |
                    Data.GetType() == typeof(uint) |
                    Data.GetType() == typeof(long) |
                    Data.GetType() == typeof(ulong) |
                    Data.GetType() == typeof(float) |
                    Data.GetType() == typeof(double) |
                    Data.GetType() == typeof(decimal) |
                    Data.GetType() == typeof(BigInteger)) return Data.ToString();
            else if (Data.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)Data;
                return string.Format("\"{0}\"", dt.ToISO8601(false));
            }
            else if (Data.GetType() == typeof(TimeSpan))
            {
                TimeSpan ts = (TimeSpan)Data;
                return string.Format("\"{0:00}:{1:00}:{2:00}.{3:000}\"", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            }
            else return string.Format("\"{0}\"", Data.ToString().EncodeJSON());
        }

        #region ToJSON (구현필요)
        public static string ToJSON(this Dictionary<string, string> Dic, bool Bracket = false)
        {
            StringBuilder sb = new StringBuilder(Bracket ? "{" : "");

            bool first = true;
            foreach (var key in Dic.Keys)
            {
                sb.AppendFormat(first ? "\"{0}\":\"{1}\"" : ",\"{0}\":\"{1}\"", key, Dic[key]);
                first = false;
            }
            if (Bracket) sb.Append("}");
            return sb.ToString();
        }
        public static string ToJSON<T>(this Dictionary<string, T> Dic, bool Bracket = false)
        {
            StringBuilder sb = new StringBuilder(Bracket ? "{" : "");

            bool first = true;
            foreach (var key in Dic.Keys)
            {
                sb.AppendFormat(first ? "\"{0}\":{1}" : ",\"{0}\":{1}", key, Dic[key].ToStringForJSON());
                first = false;
            }
            if (Bracket) sb.Append("}");
            return sb.ToString();
        }
        public static string ToJSON(this IDictionary Dic, bool Bracket = false)
        {
            StringBuilder sb = new StringBuilder(Bracket ? "{" : "");

            bool first = true;
            foreach (var key in Dic.Keys)
            {
                sb.AppendFormat(first ? "\"{0}\":{1}" : ",\"{0}\":{1}", key, Dic[key].ToStringForJSON());
                first = false;
            }
            if (Bracket) sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region ToJSONSerialize1 (Newtonsoft.JSON 라이브러리 필요)
        public static JsonSerializerSettings DefaultJsonSerializerSetting = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.Indented,
        };
        public static JsonSerializerOptions DefaultJsonSerializerOption = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        public class SerializeJSON
        {
            public SerializeJSON(object Value, string Name = null) { this.Name = Name; this.Value = Value; }
            public string Name;
            public object Value;
        }
        private class StreamReaderTemp : MemoryStream
        {
            public bool LOCK_DISPOSE = false;
            protected override void Dispose(bool disposing) { if (LOCK_DISPOSE) base.Dispose(disposing); }
            public override void Close() { if (LOCK_DISPOSE) base.Close(); }
        }

        public static string ToSerializeJSON_MS(this object Instance, JsonSerializerOptions JSONOption = null) { return System.Text.Json.JsonSerializer.Serialize(Instance, JSONOption ?? DefaultJsonSerializerOption); }
        public static string ToSerializeJSON_NS(this object Instance, JsonSerializerSettings JSONSetting = null)
        {
            using (var ms = new StreamReaderTemp())
            using (var sr = new StreamReader(Instance.ToSerializeJSONStream_NS(ms, JSONSetting)))
            {
                ms.Position = 0;
                try { return sr.ReadToEnd(); }
                finally { ms.LOCK_DISPOSE = true; ms.Close(); }
            }
        }

        public static string ToSerializeJSON_MS(this IEnumerable<SerializeJSON> Instance, JsonSerializerOptions JSONOption = null) { return System.Text.Json.JsonSerializer.Serialize(Instance, JSONOption ?? DefaultJsonSerializerOption); }
        public static string ToSerializeJSON_NS(this IEnumerable<SerializeJSON> Instance, JsonSerializerSettings JSONSetting = null)
        {
            using (var ms = new StreamReaderTemp())
            using (var sr = new StreamReader(Instance.ToSerializeJSONStream_NS(ms, JSONSetting)))
            {
                ms.Position = 0;
                return sr.ReadToEnd();
            }
        }

        public static System.IO.Stream ToSerializeJSONStream_MS(this object Instance, System.IO.Stream OutputStream, JsonSerializerOptions JSONOption = null)
        {
            using (Utf8JsonWriter sw = new Utf8JsonWriter(OutputStream))
            {
                System.Text.Json.JsonSerializer.Serialize(sw, Instance, JSONOption ?? DefaultJsonSerializerOption);
            }
            return OutputStream;
        }
        public static System.IO.Stream ToSerializeJSONStream_NS(this object Instance, System.IO.Stream OutputStream, JsonSerializerSettings JSONSetting = null) { return Instance.ToSerializeJSONStream_NS(OutputStream, Encoding.UTF8, JSONSetting); }
        public static System.IO.Stream ToSerializeJSONStream_NS(this object Instance, System.IO.Stream OutputStream, Encoding Encoding, JsonSerializerSettings JSONSetting = null)
        {
            using (StreamWriter sw = new StreamWriter(OutputStream, Encoding))
            {
                var json = Newtonsoft.Json.JsonSerializer.Create(JSONSetting ?? DefaultJsonSerializerSetting);
                //json.Formatting = Minify ? Formatting.None : Formatting.Indented;
                json.Serialize(sw, Instance);
            }
            return OutputStream;
        }

        public static System.IO.Stream ToSerializeJSONStream_MS(this IEnumerable<SerializeJSON> Instance, System.IO.Stream OutputStream, JsonSerializerOptions JSONOption = null)
        {
            using (Utf8JsonWriter sw = new Utf8JsonWriter(OutputStream))
            {
                System.Text.Json.JsonSerializer.Serialize(sw, Instance, JSONOption ?? DefaultJsonSerializerOption);
            }
            return OutputStream;
        }
        public static System.IO.Stream ToSerializeJSONStream_NS(this IEnumerable<SerializeJSON> Instance, System.IO.Stream OutputStream, JsonSerializerSettings JSONSetting = null) { return Instance.ToSerializeJSONStream_NS(OutputStream, Encoding.UTF8, JSONSetting); }
        public static System.IO.Stream ToSerializeJSONStream_NS(this IEnumerable<SerializeJSON> Instance, System.IO.Stream OutputStream, Encoding Encoding, JsonSerializerSettings JSONSetting = null)
        {
            using (StreamWriter sw = new StreamWriter(OutputStream, Encoding))
            {
                var json = Newtonsoft.Json.JsonSerializer.Create(JSONSetting ?? DefaultJsonSerializerSetting);
                JObject obj = new JObject();
                foreach (var ins in Instance)
                {
                    if (ins != null)
                    {
                        if (ins.Name == null) obj.Add(JObject.FromObject(ins.Value));
                        else obj.Add(new JProperty(ins.Name, JObject.FromObject(ins.Value)));
                    }
                }
            }
            return OutputStream;
        }
        #endregion

        /// <summary>
        /// JSON 문자열으로부터 불러오기 [Microsoft.Test.Json]
        /// </summary>
        /// <param name="JSONString">JSON 문자열</param>
        /// <returns></returns>
        public static T DeserializeJSON_MS<T>(this string JSONString, JsonSerializerOptions JSONOption = null) { return (T)DeserializeJSON_MS(JSONString, typeof(T), JSONOption);  }
        /// <summary>
        /// JSON 문자열으로부터 불러오기 [Microsoft.Test.Json]
        /// </summary>
        /// <param name="JSONString">JSON 문자열</param>
        /// <param name="Type">변환할 형식</param>
        /// <returns></returns>
        public static object DeserializeJSON_MS(this string JSONString, Type Type, JsonSerializerOptions JSONOption = null)
        {
            return System.Text.Json.JsonSerializer.Deserialize(JSONString, Type, JSONOption ?? DefaultJsonSerializerOption);
        }
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Microsoft.Test.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <returns></returns>
        public static T DeserializeJSON_MS<T>(this System.IO.Stream JSONStream, JsonSerializerOptions JSONOption = null) { return (T)DeserializeJSON_MS(JSONStream, typeof(T), JSONOption); }
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Microsoft.Test.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <param name="Type">변환할 형식</param>
        /// <returns></returns>
        public static object DeserializeJSON_MS(this System.IO.Stream JSONStream, Type Type, JsonSerializerOptions JSONOption = null)
        {
            return System.Text.Json.JsonSerializer.Deserialize(JSONStream, Type, JSONOption ?? DefaultJsonSerializerOption);
        }

        /// <summary>
        /// JSON 문자열으로부터 불러오기 [Newtonsoft.Json]
        /// </summary>
        /// <param name="JSONString">JSON 문자열</param>
        /// <returns></returns>
        public static T DeserializeJSON_NS<T>(string JSONString, JsonSerializerSettings Settings = null) { return (T)DeserializeJSON_NS(JSONString, typeof(T), Settings); }
        /// <summary>
        /// JSON 문자열으로부터 불러오기 [Newtonsoft.Json]
        /// </summary>
        /// <param name="JSONString">JSON 문자열</param>
        /// <param name="Type">변환할 형식</param>
        /// <returns></returns>
        public static object DeserializeJSON_NS(string JSONString, Type Type, JsonSerializerSettings Settings = null) => JsonConvert.DeserializeObject(JSONString, Type, Settings ?? DefaultJsonSerializerSetting);
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Newtonsoft.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <returns></returns>
        public static T DeserializeJSON_NS<T>(this System.IO.Stream JSONStream) { return (T)DeserializeJSON_NS(JSONStream, typeof(T)); }
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Newtonsoft.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <param name="Type">변환할 형식</param>
        /// <returns></returns>
        public static object DeserializeJSON_NS(this System.IO.Stream JSONStream, Type Type)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var sr = new StreamReader(JSONStream))
            using (var jsonTextReader = new JsonTextReader(sr))
                return serializer.Deserialize(jsonTextReader, Type);
        }

#if NETSTANDARD2_0_OR_GREATER || (!NET20 && !NET30 && !NET35 && !NET40) || NETCORE || NETCOREAPP || NETSTANDARD
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Microsoft.Test.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <returns></returns>
        public static ValueTask<T> DeserializeJSONAsync_MS<T>(System.IO.Stream JSONStream, JsonSerializerOptions JSONOption = null) { return System.Text.Json.JsonSerializer.DeserializeAsync<T>(JSONStream, JSONOption ?? DefaultJsonSerializerOption); }
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Microsoft.Test.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <param name="Type">변환할 형식</param>
        /// <returns></returns>
        public static ValueTask<object> DeserializeJSONAsync_MS(System.IO.Stream JSONStream, Type Type, JsonSerializerOptions JSONOption = null) { return System.Text.Json.JsonSerializer.DeserializeAsync(JSONStream, Type, JSONOption ?? DefaultJsonSerializerOption); }
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Newtonsoft.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <returns></returns>
        public static async Task<T> DeserializeJSONAsync_NS<T>(System.IO.Stream JSONStream) => await Task.Run(() => DeserializeJSON_NS<T>(JSONStream));
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다) [Newtonsoft.Json]
        /// </summary>
        /// <param name="JSONStream">JSON 스트림</param>
        /// <param name="Type">변환할 형식</param>
        /// <returns></returns>
        public static async Task<object> DeserializeJSONAsync_NS(System.IO.Stream JSONStream, Type Type) => await Task.Run(() => DeserializeJSON_NS(JSONStream, Type));
#endif

        #region Etc
        public static Dictionary<string, object> ToDictionaryFromProperties<T>(this T Instance) where T : class
        {
            var properties = Instance.GetType().GetProperties();

            var dic = new Dictionary<string, object>(properties.Length);
            for (int i = 0; i < properties.Length; i++)
                dic.Add(properties[i].Name, properties[i].GetValue(Instance));

            return dic;
        }
        #endregion
    }
}
