using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HS.Utils.Web
{
    public static class WebUtils
    {
        public static string GetURLEncode(string URL) { return HttpUtility.UrlEncode(URL); }
        public static string GetRedirectHTML(string URL, int Delay = 0)
        {
            return string.Format("<meta http-equiv='refresh' content='{0};url={1}' />", Delay, URL);
        }
    }
}
