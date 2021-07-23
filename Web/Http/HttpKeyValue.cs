using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace HS.Utils.Web.Http
{
    public class HttpKeyValue : NameValueCollection
    {
        NameValueCollection Params;
        public HttpKeyValue() { this.Params = new NameValueCollection(); }
        public HttpKeyValue(NameValueCollection ParamMerge) { this.Params = ParamMerge; }
        public HttpKeyValue(string Key, string Value) { Params = new NameValueCollection(); Add(Key, Value); }
#if NETSTANDARD2_0_OR_GREATER && (!NET20 && !NET30 && !NET35 && !NET40)
        public HttpKeyValue(IReadOnlyDictionary<string, string> Params)
        {
            this.Params = new NameValueCollection(Params.Count);
            foreach (var pair in Params) this.Params.Add(pair.Key, pair.Value);
        }
#endif
        public HttpKeyValue(IDictionary<string, string> Params)
        {
            this.Params = new NameValueCollection(Params.Count);
            foreach (var pair in Params) this.Params.Add(pair.Key, pair.Value);
        }

        public HttpKeyValue Add(KeyValuePair<string, object> Item) { BaseAdd(Item.Key, Item.Value); return this; }
        public HttpKeyValue Add(KeyValuePair<string, string> Item) { BaseAdd(Item.Key, Item.Value); return this; }

        public NameValueCollection Apply(NameValueCollection Collection)
        {
            Collection.Add(this);
            return Collection;
        }

        public override string ToString() { return ToString(false); }
        public string ToString(bool EscapeEncoding)
        {
            var keys = AllKeys;
            var sb = new StringBuilder();
            bool First = true;

            for(int i = 0; i < Count; i++)
            {
                if (First) First = false;
                else sb.Append('&');
                sb.Append(keys[i]).Append('=').Append(EscapeEncoding ? Uri.EscapeDataString(this[i]) : this[i]);
            }
            return sb.ToString();
        }

        public static explicit operator Dictionary<string, string>(HttpKeyValue Param) 
        {
            if (Param == null) return null;
            var dic = new Dictionary<string, string>(Param.Count);
            var keys = Param.AllKeys;
            for(int i = 0; i < keys.Length; i++)
            {
                if (dic.ContainsKey(keys[i])) dic[keys[i]] = Param[i];
                else dic.Add(keys[i], Param[i]);
            }
            return dic;
        }

        public static explicit operator CookieCollection(HttpKeyValue Param)
        {
            if (Param == null) return null;
            var cookie = new CookieCollection();

            var keys = Param.AllKeys;
            for (int i = 0; i < keys.Length; i++) cookie.Add(new Cookie(keys[i], Param[i]));

            return cookie;
        }
    }
}
