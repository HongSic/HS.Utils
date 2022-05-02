using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HS.Utils
{
    public static class JSONUtils
    {
        public static string EncodeJSON(this string text)
        {
            if (text == null || text.Length == 0) return "";

            int i;
            int len = text.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

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
                            t = "000" + String.Format("X", c);
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
            sb.Append(EncodeJSON(Message)).Append("\",");
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
            else if(Data.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)Data;
                return string.Format("\"{0}\"", dt.ToISO8601(false));
            }
            else if(Data.GetType() == typeof(TimeSpan))
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
                sb.AppendFormat((first ? "\"{0}\":\"{1}\"" : ",\"{0}\":\"{1}\""), key, Dic[key]);
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
                sb.AppendFormat((first ? "\"{0}\":{1}" : ",\"{0}\":{1}"), key, Dic[key].ToStringForJSON());
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
                sb.AppendFormat((first ? "\"{0}\":{1}" : ",\"{0}\":{1}"), key, Dic[key].ToStringForJSON());
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

        public static string ToSerializeJSON_NS(this object Instance, JsonSerializerSettings JSONSetting = null)
        {
            using (var ms = new StreamReaderTemp())
            using (var sr = new StreamReader(ToSerializeJSONStream_NS(Instance, ms, JSONSetting)))
            {
                ms.Position = 0;
                try { return sr.ReadToEnd(); }
                finally { ms.LOCK_DISPOSE = true; ms.Close(); }
            }
        }

        public static string ToSerializeJSON_NS(this IEnumerable<SerializeJSON> Instance, JsonSerializerSettings JSONSetting = null)
        {
            using (var ms = new StreamReaderTemp())
            using (var sr = new StreamReader(ToSerializeJSONStream_NS(Instance, ms, JSONSetting)))
            {
                ms.Position = 0;
                return sr.ReadToEnd();
            }
        }
        public static Stream ToSerializeJSONStream_NS(this object Instance, Stream OutputStream, JsonSerializerSettings JSONSetting = null) { return ToSerializeJSONStream_NS(Instance, OutputStream, Encoding.UTF8, JSONSetting); }
        public static Stream ToSerializeJSONStream_NS(this object Instance, Stream OutputStream, Encoding Encoding, JsonSerializerSettings JSONSetting = null)
        {
            using (StreamWriter sw = new StreamWriter(OutputStream, Encoding))
            {
                var json = JsonSerializer.Create(JSONSetting ?? DefaultJsonSerializerSetting);
                //json.Formatting = Minify ? Formatting.None : Formatting.Indented;
                json.Serialize(sw, Instance);
            }
            return OutputStream;
        }

        public static Stream ToSerializeJSONStream_NS(this IEnumerable<SerializeJSON> Instance, Stream OutputStream, JsonSerializerSettings JSONSetting = null) { return ToSerializeJSONStream_NS(Instance, OutputStream, Encoding.UTF8, JSONSetting); }
        public static Stream ToSerializeJSONStream_NS(this IEnumerable<SerializeJSON> Instance, Stream OutputStream, Encoding Encoding, JsonSerializerSettings JSONSetting = null)
        {
            using (StreamWriter sw = new StreamWriter(OutputStream, Encoding))
            {
                var json = JsonSerializer.Create(JSONSetting ?? DefaultJsonSerializerSetting);
                JObject obj = new JObject();
                foreach(var ins in Instance)
                {
                    if(ins != null)
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
        /// JSON 문자열으로부터 설정 불러오기
        /// </summary>
        /// <param name="JSONString">설정 JSON 문자열</param>
        /// <returns></returns>
        public static T DeserializeJSON<T>(string JSONString, JsonSerializerSettings Settings = null) => JsonConvert.DeserializeObject<T>(JSONString, Settings);
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다)
        /// </summary>
        /// <param name="JSONString">설정 JSON 문자열</param>
        /// <returns></returns>
        public static T DeserializeJSON<T>(this Stream JSONStream)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(JSONStream))
            using (var jsonTextReader = new JsonTextReader(sr))
                return serializer.Deserialize<T>(jsonTextReader);
        }
        /// <summary>
        /// JSON 문자열 스트림으로부터 설정 불러오기 (자동으로 스트림이 닫힙니다)
        /// </summary>
        /// <param name="JSONString">설정 JSON 문자열</param>
        /// <returns></returns>
        public static async Task<T> DeserializeJSONAsync<T>(Stream JSONStream) => await Task.Run(() => DeserializeJSON<T>(JSONStream));

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
