using System;
using System.IO;
using System.Net;
using System.Text;

namespace HS.Utils.Web.Http
{
    public static class HttpRequestBuilder
    {
        private const bool CLOSE = true;
        private static readonly CookieCollection CookieCollectionEmpty = null;

        public static Uri BuildURL(string URL, HttpKeyValue Param = null)
        {
            if (URL == null) return null;

            if (Param != null && Param.Count > 0)
            {
                string param = Param.ToString();

                bool IsFind = false;
                for (int i = 0; i < URL.Length; i++)
                    if (URL[i] == '?') { IsFind = true; break; }

                return new Uri(IsFind && URL[URL.Length - 1] != '&' ?
                    string.Format("{0}&{1}", URL, param) :
                    string.Format("?{0}{1}", URL, param));
            }
            else return new Uri(URL);
        }

        public static void SetParam(this Stream Stream, HttpKeyValue Params, bool Close = CLOSE)
        {
            if (Params != null && Params.Count > 0)
            {
                try
                {
                    var writer = new StreamWriter(Stream);

                    bool First = true;
                    var keys = Params.AllKeys;
                    for (int i = 0; i < Params.Count; i++)
                    {
                        if (First) First = false;
                        else writer.Write('&');

                        writer.Write(keys[i]);
                        writer.Write('=');
                        writer.Write(Uri.EscapeDataString(Params[i]));
                    }
                }
                finally { if (Close) Stream.Close(); }
            }
        }
        public static HttpWebRequest SetParam(this HttpWebRequest Request, HttpKeyValue Params, bool Close = CLOSE)
        {
            if (Params != null && Params.Count > 0)  SetParam(Request.GetRequestStream(), Params, Close);
            return Request;
        }

        #region Build
        public static HttpWebRequest Build(this Uri URL, string Method = HttpMethod.GET) { return Build(URL, Method, null, null, CookieCollectionEmpty); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data) { return Build(URL, Method, Data, null, CookieCollectionEmpty); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header) { return Build(URL, Method, Data, Header, CookieCollectionEmpty); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, HttpKeyValue Cookie) {return Build(URL, Method, Data, Header, (CookieCollection)Cookie); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, CookieCollection Cookie)
        {
            var Request = (HttpWebRequest)WebRequest.Create(URL);
            Request.PreAuthenticate = true;
            Request.Method = Method;
            Request.AllowAutoRedirect = true;
            Request.ServicePoint.Expect100Continue = true;
            Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (Header != null) Header.Apply(Request.Headers);
            if (Cookie != null) Request.CookieContainer.Add(Cookie);
            if (URL.Scheme.ToLower() == "https")
            {
                //Request.Credentials = CredentialCache.DefaultCredentials;
            }

            if(Data != null) Request.SetParam(Data);
            return Request;
        }
        #endregion

        #region SetData
        public static HttpWebRequest SetData(this HttpWebRequest Request, string Data, bool Close = CLOSE) { return SetData(Request, Data, Encoding.UTF8, Close); }
        public static HttpWebRequest SetData(this HttpWebRequest Request, string Data, Encoding Encoding, bool Close = CLOSE) { return SetData(Request, Encoding.GetBytes(Data), Close); }
        public static HttpWebRequest SetData(this HttpWebRequest Request, byte[] Data, bool Close = CLOSE) {  return Data != null && Data.Length > 0 ? SetData(Request, Data, 0, Data.Length, Close) : Request; }
        public static HttpWebRequest SetData(this HttpWebRequest Request, byte[] Data, int Offset, int Count, bool Close = CLOSE)
        {
            if (Data != null && Data.Length > 0)
            {
                var writer = Request.GetRequestStream();
                try { writer.Write(Data, Offset, Count); }
                finally { if (Close) writer.Close(); }
            }
            return Request;
        }

        public static HttpWebRequest SetData(this HttpWebRequest Request, Stream Stream, bool Close = CLOSE)
        {
            if (Stream != null)
            {
                var writer = Request.GetRequestStream();
                try { Stream.CopyStream(writer); }
                finally { if (Close) writer.Close(); }
            }
            return Request;
        }
        #endregion

        public static HttpWebRequest SetDataJSON(this HttpWebRequest Request, object Data, bool Close = CLOSE)
        {
            if (Data != null)
            {
                var writer = Request.GetRequestStream();
                try 
                {
                    Data.ToSerializeJSONStream1(writer);
                    Request.ContentType = "application/json; charset=utf-8";
                }
                finally { if (Close) writer.Close(); }
            }
            return Request;
        }
    }

    public static class HttpMethod
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
        public const string ADD = "ADD";
    }
}
