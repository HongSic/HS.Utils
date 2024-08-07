﻿using System;
using System.IO;
using System.Net;
using System.Text;
using HS.Utils.IO.Stream;
using HS.Utils.Text;

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

        public static void SetParam(this System.IO.Stream Stream, HttpKeyValue Params, bool Close = CLOSE) { SetParam(Stream, Params, Encoding.UTF8, Close); }
        public static void SetParam(this System.IO.Stream Stream, HttpKeyValue Params, Encoding Encoding, bool Close = CLOSE)
        {
            if (Params != null && Params.Count > 0)
            {
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(Stream, Encoding);

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

                    writer.Flush();
                }
                finally { if (Close) writer?.Close(); }
            }
        }
        public static HttpWebRequest SetParam(this HttpWebRequest Request, HttpKeyValue Params) { return SetParam(Request, Params, new UTF8Encoding(false)); }
        public static HttpWebRequest SetParam(this HttpWebRequest Request, HttpKeyValue Params, Encoding Encoding)
        {
            if (Params != null && Params.Count > 0)  SetParam(Request.GetRequestStream(), Params, Encoding, false);
            return Request;
        }

        #region Build
        public static HttpWebRequest Build(this Uri URL, string Method = HttpMethod.GET) { return Build(URL, Method, null, null, CookieCollectionEmpty); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data) { return Build(URL, Method, Data, Encoding.UTF8); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, Encoding Encoding) { return Build(URL, Method, Data, null, CookieCollectionEmpty, Encoding); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header) { return Build(URL, Method, Data, Header, Encoding.UTF8); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, Encoding Encoding) { return Build(URL, Method, Data, Header, CookieCollectionEmpty, Encoding); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, HttpKeyValue Cookie) {return Build(URL, Method, Data, Header, Cookie, Encoding.UTF8); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, HttpKeyValue Cookie, Encoding Encoding) {return Build(URL, Method, Data, Header, (CookieCollection)Cookie, Encoding); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, CookieCollection Cookie) { return Build(URL, Method, Data, Header, Cookie, Encoding.UTF8); }
        public static HttpWebRequest Build(this Uri URL, string Method, HttpKeyValue Data, HttpKeyValue Header, CookieCollection Cookie, Encoding Encoding)
        {
            var Request = (HttpWebRequest)WebRequest.Create(URL);
            Request.PreAuthenticate = true;
            Request.Method = Method;
            Request.AllowAutoRedirect = true;
            Request.ServicePoint.Expect100Continue = true;
            Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (Cookie != null && Cookie.Count > 0) Request.CookieContainer.Add(Cookie);
            if (URL.Scheme.ToLower() == "https")
            {
                //Request.Credentials = CredentialCache.DefaultCredentials;
            }

            if (Data != null)
            {
                Request.SetParam(Data, Encoding);
                if (Header == null) Header = new HttpKeyValue();
                Header.Set("Content-type", "application/x-www-form-urlencoded");
            }

            if (Header != null) Header.Apply(Request.Headers);
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

        public static HttpWebRequest SetData(this HttpWebRequest Request, System.IO.Stream Stream, bool Close = CLOSE)
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
                    Data.ToSerializeJSONStream_NS(writer);
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
        public const string PATCH = "PATCH";
        public const string FETCH = "FETCH";
        public const string DELETE = "DELETE";
        public const string COPY = "COPY";
        public const string HEAD = "HEAD";
        public const string OPTIONS = "OPTIONS";
        public const string LINK = "LINK";
        public const string UNLINK = "UNLINK";
        public const string LOCK = "LOCK";
        public const string UNLOCK = "UNLOCK";
        public const string FIND = "FIND";
        public const string ADD = "ADD";
        public const string LIST = "LIST";
        public const string VIEW = "VIEW";
        public const string COUNT = "COUNT";
    }
}
