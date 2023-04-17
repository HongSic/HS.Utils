using System.Text;
using System.Web;

namespace HS.Utils.Web
{
    public static class WebUtils
    {
        public static string GetURLEncode(this string URL) { return GetURLEncode(URL, Encoding.UTF8); }
        public static string GetURLEncode(this string URL, Encoding Encoding) { return URL == null ? null : HttpUtility.UrlEncode(URL, Encoding); }
        public static string GetURLDecode(string URL) { return GetURLDecode(URL, Encoding.UTF8); }
        public static string GetURLDecode(string URL, Encoding Encoding) { return HttpUtility.UrlDecode(URL, Encoding); }
        public static string GetRedirectHTML(string URL, int Delay = 0)
        {
            return string.Format("<meta http-equiv='refresh' content='{0};url={1}' />", Delay, URL);
        }
    }
}
