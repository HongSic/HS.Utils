using System;
using System.Collections;
using System.Net;

namespace HS.Utils.Web.Http
{
    public class HttpException : Exception
    {
        public HttpException(WebException BaseException, HttpWebResponse Response) : base(BaseException.Message, BaseException)
        {
            this.BaseException = BaseException;
            this.Response = Response;
        }

        public WebException BaseException { get; private set; }

        public HttpWebResponse Response { get; private set; }

        public HttpStatusCode Status { get { return Response.StatusCode; } }

        public static implicit operator HttpException(WebException Exception) { return new HttpException(Exception, Exception.Response as HttpWebResponse); }
    }
}
