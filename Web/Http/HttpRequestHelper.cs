using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HS.Utils.Web.Http
{
    public delegate void OnHttpSuccessHandler(HttpWebRequest Request, HttpWebResponse Response);
    public delegate void OnHttpFailHandler(HttpWebRequest Request, HttpWebResponse Response, Exception Exception);

    public static class HttpRequestHelper
    {

        #region Send
        public static HttpWebResponse Send(this HttpWebRequest Request) { return (HttpWebResponse)Request.GetResponse(); }
        public static void Send(this HttpWebRequest Request, HttpReponseSuccessHandler OnSuccess, HttpReponseFailHandler OnFail)
        {
            try
            {
                var Response = Send(Request);
                if (Response.StatusCode == HttpStatusCode.OK) if (OnSuccess != null) OnSuccess(Response);
                    else if (OnFail != null) OnFail(Response, null);
            }
            catch (Exception ex) { if (OnFail != null) OnFail(null, ex); }
        }

#if NETSTANDARD2_0_OR_GREATER && (!NET20 && !NET30 && !NET35 && !NET40)
        public static async Task<HttpWebResponse> SendAsync(this HttpWebRequest Request)
        {
            try { return (HttpWebResponse)await Request.GetResponseAsync(); }
            catch (WebException ex) { throw (HttpException)ex; }
        }
        public static async Task SendAsync(this HttpWebRequest Request, HttpReponseSuccessHandler OnSuccess, HttpReponseFailHandler OnFail)
        {
            try
            {
                var Response = await SendAsync(Request);
                if (OnSuccess != null) OnSuccess(Response);
            }
            catch (WebException ex) { if (OnFail != null) OnFail(null, (HttpException)ex); }
        }
#endif
        #endregion

        
        #region SendRestful
        public static HttpWebResponse SendRestful(this Uri URL, string Method = HttpMethod.GET, HttpKeyValue Data = null, HttpKeyValue Header = null, CookieCollection Cookie = null) { return SendRestful(HttpRequestBuilder.Build(URL, Method, Data, Header, Cookie)); }
        public static HttpWebResponse SendRestful(this HttpWebRequest Request, Stream Data = null, int Depth = 10)
        {
            Request.AllowAutoRedirect = false;
            try { return (HttpWebResponse)Request.GetResponse(); }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    if (Depth > 0)
                    {
                        Uri url = new Uri(Request.RequestUri, response.Headers["Location"]);
                        var request = Request.Clone(url, Data);
                        response = SendRestful(request, Data, Depth--);
                    }
                }
                else throw new HttpException(ex, response);

                return response;
            }
        }
        

#if NETSTANDARD2_0_OR_GREATER || (!NET20 && !NET30 && !NET35 && !NET40)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Method"></param>
        /// <param name="Data"></param>
        /// <param name="Header"></param>
        /// <param name="Cookie"></param>
        /// <param name="Depth">If server sent redirect, redirect depth</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> SendRestfulAsyncTask(this Uri URL, string Method = HttpMethod.GET, HttpKeyValue Data = null, HttpKeyValue Header = null, CookieCollection Cookie = null, int Depth = 10)
        {
            var request = HttpRequestBuilder.Build(URL, Method, null, Header, Cookie);
            if (Data != null && Data.Count > 0)
            {
                using (var ms = new MemoryStream())
                {
                    ms.SetParam(Data, false);
                    ms.CopyStream(request.GetRequestStream());
                    return await SendRestfulAsyncTask(request, ms, Depth);
                }
            }
            else return await SendRestfulAsyncTask(request, null, Depth);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Depth">If server sent redirect, redirect depth</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> SendRestfulAsyncTask(this HttpWebRequest Request, Stream Data = null, int Depth = 10)
        {
            Request.AllowAutoRedirect = false;
            try { return (HttpWebResponse)Request.GetResponse(); }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                if (response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    if (Depth > 0)
                    {
                        Uri url = new Uri(Request.RequestUri, response.Headers["Location"]);
                        var request = Request.Clone(url, Data);
                        response = await SendRestfulAsyncTask(request, Data, Depth--);
                    }
                }
                else throw new HttpException(ex, response);

                return response;
            }
        }
        public static void SendRestfulAsync(OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, HttpWebRequest Request) { SendRestfulAsync(Request, OnSuccess, OnFail); }
        public static void SendRestfulAsync(OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, Uri URL, string Method = HttpMethod.GET, HttpKeyValue Data = null, HttpKeyValue Header = null, CookieCollection Cookie = null) { SendRestfulAsync(HttpRequestBuilder.Build(URL, Method, Data, Header, Cookie), OnSuccess, OnFail); }
        public static void SendRestfulAsync(this Uri URL, OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, string Method = HttpMethod.GET, HttpKeyValue Data = null, HttpKeyValue Header = null, CookieCollection Cookie = null) { SendRestfulAsync(HttpRequestBuilder.Build(URL, Method, Data, Header, Cookie), OnSuccess, OnFail); }
        public static void SendRestfulAsync(this HttpWebRequest Request, OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail)
        {
            SendRestfulPack pack = new SendRestfulPack(OnSuccess, OnFail, Request);
            new TaskFactory().StartNew(_SendRestfulAsync, pack);
        }
#else
        //Compatable VS2010

        public static void SendRestfulAsync(OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, HttpWebRequest Request) { SendRestfulAsync(Request, OnSuccess, OnFail); }
        public static void SendRestfulAsync(OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, Uri URL, string Method = HttpMethod.GET, HttpKeyValue Data = null, HttpKeyValue Header = null, CookieCollection Cookie = null) { SendRestfulAsync(HttpRequestBuilder.Build(URL, Method, Data, Header, Cookie), OnSuccess, OnFail); }
        public static void SendRestfulAsync(this Uri URL, OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, string Method = HttpMethod.GET, HttpKeyValue Data = null, HttpKeyValue Header = null, CookieCollection Cookie = null) { SendRestfulAsync(HttpRequestBuilder.Build(URL, Method, Data, Header, Cookie), OnSuccess, OnFail); }
        public static void SendRestfulAsync(this HttpWebRequest Request, OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail)
        {
            SendRestfulPack pack = new SendRestfulPack(OnSuccess, OnFail, Request);
            new Thread(new ParameterizedThreadStart(_SendRestfulAsync)).Start();
        }
#endif
        private static void _SendRestfulAsync(object Pack)
        {
            var pack = (SendRestfulPack)Pack;
            HttpWebRequest request = pack.Request;
            try
            {
                var response = Send(request);
                if (pack.OnSuccess != null) pack.OnSuccess(request, response);
            }
            catch (WebException ex) { pack.OnFail(request, (HttpWebResponse)ex.Response, ex); }
            catch (Exception ex) { pack.OnFail(request, null, ex); }
        }

#if NETSTANDARD2_0_OR_GREATER || (!NET20 && !NET30 && !NET35 && !NET40)

        //Compatable VS2010
#endif

        private class SendRestfulPack : IDisposable
        {
            public OnHttpSuccessHandler OnSuccess;
            public OnHttpFailHandler OnFail;
            public HttpWebRequest Request;
            public SendRestfulPack(OnHttpSuccessHandler OnSuccess, OnHttpFailHandler OnFail, HttpWebRequest Request)
            {
                this.OnSuccess = OnSuccess;
                this.OnFail = OnFail;
                this.Request = Request;
            }

            public void Dispose()
            {
                OnSuccess = null;
                OnFail = null;
                Request = null;
            }
        }
        #endregion

        public static HttpWebRequest Clone(this HttpWebRequest Request, Uri URL, Stream Data = null)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);
            request.Accept = Request.Accept;
            request.PreAuthenticate = Request.PreAuthenticate;
            request.UseDefaultCredentials = Request.UseDefaultCredentials;
            request.Credentials = Request.Credentials;
            request.ContentType = Request.ContentType;
            request.ContentLength = Request.ContentLength;
            request.UserAgent = Request.UserAgent;
            request.AutomaticDecompression = Request.AutomaticDecompression;
            request.Date = Request.Date;
            request.Timeout = Request.Timeout;
            request.Proxy = Request.Proxy;
            request.Headers = Request.Headers;
            request.CookieContainer = Request.CookieContainer;
            request.ServicePoint.Expect100Continue = Request.ServicePoint.Expect100Continue;
            request.PreAuthenticate = Request.PreAuthenticate;

            if(Data != null) Data.CopyStream(request.GetRequestStream());

            return request;
        }
    }
}
