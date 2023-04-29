using HS.Utils.Stream;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HS.Utils.Text;

namespace HS.Utils.Web.Http
{
    public delegate void HttpReponseSuccessHandler(HttpWebResponse Response);
    public delegate void HttpReponseFailHandler(HttpWebResponse Response, Exception Exception);

    public static class HttpReponse
    {
        public static string GetString(this HttpWebResponse Response, bool Close = true) { return Response.GetResponseStream().GetString(Close); }
        public static string GetString(this HttpWebResponse Response, Encoding Encoding, bool Close = true) { return Response.GetResponseStream().GetString(Encoding, Close); }

        public static byte[] GetData(this HttpWebResponse Response, bool Close = true) { return Response.GetResponseStream().GetData(Close); }

#if NETSTANDARD2_0_OR_GREATER && (!NET20 && !NET30 && !NET35 && !NET40) || NETCORE || NETCOREAPP || NETSTANDARD
        public static async Task<string> GetStringAsync(this HttpWebResponse Response, bool Close = true) { return await Response.GetResponseStream().GetStringAsync(Close); }
        public static async Task<string> GetStringAsync(this HttpWebResponse Response, Encoding Encoding, bool Close = true) { return await Response.GetResponseStream().GetStringAsync(Encoding, Close); }

        public static async Task<byte[]> GetDataAsync(this HttpWebResponse Response, bool Close = true) { return await Response.GetResponseStream().GetDataAsync(Close); }
        

        public static T GetInstance<T>(this HttpWebResponse Response)
        {
            using(Response) return JSONUtils.DeserializeJSON_NS<T>(Response.GetResponseStream());
        }
        public static async Task<T> GetInstanceAsync<T>(this HttpWebResponse Response)
        {
            using(Response) return await JSONUtils.DeserializeJSONAsync_NS<T>(Response.GetResponseStream()); 
        }
#endif
    }
}
