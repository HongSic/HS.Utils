namespace HS.Utils.Web.Http
{
    //https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers
    public static class HttpHeaderKind
    {
        /// <summary>
        /// Initiates a request for cross-origin resource sharing with Origin (below).
        /// <code>Access-Control-Request-Method: GET</code>
        /// </summary>
        public const string AccessControlRequestMethods = "Access-Control-Request-Methods";
        /// <summary>
        /// Initiates a request for cross-origin resource sharing with Origin (below).
        /// <code>Access-Control-Request-Method: GET</code>
        /// </summary>
        public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        /// <summary>
        /// Control options for the current connection and list of hop-by-hop request fields.<br/><br/>
        /// Must not be used with HTTP/2
        /// <code>Connection: keep-alive</code>
        /// <code>Connection: Upgrade</code>
        /// </summary>
        public const string Connection = "Connection";
        /// <summary>
        /// The type of encoding used on the data. See HTTP compression.
        /// <code>Content-Encoding: gzip</code>
        /// </summary>
        public const string ContentEncoding = "Content-Encoding";
        /// <summary>
        /// The length of the request body in octets (8-bit bytes).
        /// <code>Content-Length: 348</code>
        /// </summary>
        public const string ContentLength = "Content-Length";
        /// <summary>
        /// The Media type of the body of the request (used with POST and PUT requests).
        /// <code>Content-Type: application/x-www-form-urlencoded</code>
        /// </summary>
        public const string ContentType = "Content-Type";
        /// <summary>
        /// The date and time at which the message was originated (in "HTTP-date" format as defined by RFC 7231 Date/Time Formats).
        /// <code>Date: Tue, 15 Nov 1994 08:12:31 GMT</code>
        /// </summary>
        public const string Date = "Date";
        /// <summary>
        /// Implementation-specific fields that may have various effects anywhere along the request-response chain.
        /// <code>Pragma: no-cache</code>
        /// </summary>
        public const string Pragma = "Pragma";
        /// <summary>
        /// 
        /// </summary>
        public const string UserAgent = "User-Agent";
        /// <summary>
        /// 
        /// <code></code>
        /// </summary>
        //public const string aaaaa = "aaaaa";

        public static class Request
        {
            /// <summary>
            /// Media type(s) that is/are acceptable for the response. See Content negotiation.
            /// <code>Accept: text/html</code>
            /// </summary>
            public const string Accept = "Accept";
            /// <summary>
            /// Character sets that are acceptable.
            /// <code>Accept-Charset: utf-8</code>
            /// </summary>
            public const string AcceptCharset = "Accept-Charset";
            /// <summary>
            /// List of acceptable encodings. See HTTP compression.
            /// <code>Accept-Encoding: gzip, deflate</code>
            /// </summary>
            public const string AcceptEncoding = "Accept-Encoding";
            /// <summary>
            /// List of acceptable human languages for response. See Content negotiation.
            /// <code>Accept-Language: ko-KR</code>
            /// </summary>
            public const string AcceptLanguage = "Accept-Language";
            /// <summary>
            /// Authentication credentials for HTTP authentication.
            /// <code>Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==</code>
            /// </summary>
            public const string Authorization = "Authorization";
            /// <summary>
            /// Used to specify directives that must be obeyed by all caching mechanisms along the request-response chain.
            /// <code>	Cache-Control: no-cache</code>
            /// </summary>
            public const string CacheControl = "Cache-Control";
            /// <summary>
            /// An HTTP cookie previously sent by the server with <see cref="Response.SetCookie"/> (Set-Cookie).
            /// <code>Cookie: $Version=1; Skin=new;</code>
            /// </summary>
            public const string Cookie = "Cookie";
            /// <summary>
            /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.<br/>
            /// The port number may be omitted if the port is the standard port for the service requested.<br/><br/>
            /// Mandatory since HTTP/1.1 If the request is generated directly in HTTP/2, it should not be used
            /// <code>Host: en.wikipedia.org:8080</code>
            /// <code>Host: en.wikipedia.org</code>
            /// </summary>
            public const string Host = "Host";
            /// <summary>
            /// Limit the number of times the message can be forwarded through proxies or gateways.
            /// <code>	Max-Forwards: 10</code>
            /// </summary>
            public const string MaxForwards = "Max-Forwards";
            /// <summary>
            /// Initiates a request for cross-origin resource sharing (asks server for Access-Control-* response fields).
            /// <code>Origin: http://www.example.com</code>
            /// </summary>
            public const string Origin = "Origin";
            /// <summary>
            /// This is the address of the previous web page from which a link to the currently requested page was followed.<br/>
            /// (The word "referrer" has been misspelled in the RFC as well as in most implementations to the point that it has become standard usage and is considered correct terminology)
            /// <code>Referer: http://en.wikipedia.org/wiki/MainPage</code>
            /// </summary>
            public const string Referer = "Referer";
            /// <summary>
            /// The user agent string of the user agent.
            /// <code>User-Agent: Mozilla/5.0 (X11; Linux x8664; rv:12.0) Gecko/20100101 Firefox/12.0</code>
            /// </summary>
            public const string UserAgent = "User-Agent";
            /// <summary>
            /// 
            /// <code></code>
            /// </summary>
            public const string aaaaa = "aaaaa";
        }
        public static class Response
        {
            public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
            public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
            public const string AccessControlMaxAge = "Access-Control-Max-Age";
            /// <summary>
            /// An alternate location for the returned data
            /// <code>Content-Location: /index.html</code>
            /// </summary>
            public const string ContentLocation = "Content-Location";
            /// <summary>
            /// Used in redirection, or when a new resource has been created.
            /// <code>Location: http://www.w3.org/pub/WWW/People.html</code>
            /// <code>Location: /pub/WWW/People.html</code>
            /// </summary>
            public const string Location = "Location";
            /// <summary>
            /// The last modified date for the requested object (in "HTTP-date" format as defined by RFC 7231)
            /// <code>Last-Modified: Tue, 15 Nov 1994 12:45:26 GMT</code>
            /// </summary>
            public const string LastModified = "Last-Modified";
            /// <summary>
            /// An HTTP cookie
            /// <code>Set-Cookie: UserID=JohnDoe; Max-Age=3600; Version=1</code>
            /// </summary>
            public const string SetCookie = "Set-Cookie";
            /// <summary>
            /// A name for the server
            /// <code>Server: Apache/2.4.1 (Unix)</code>
            /// </summary>
            public const string Server = "Server";
            /// <summary>
            /// 
            /// <code></code>
            /// </summary>
            public const string aaaaa = "aaaaa";
        }
    }
}
