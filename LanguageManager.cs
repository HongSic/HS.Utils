using System.Collections.Generic;

namespace HS.Utils
{
    public class LanguageManager
    {
        Dictionary<string, string> lng = new Dictionary<string, string>();
        public LanguageManager() { }
        public LanguageManager(string[] Language)
        {
            for (int i = 0; i < Language.Length; i++)
            {
                if (!string.IsNullOrEmpty(Language[i]) &&
                    (Language[i][0] >= 'A' && Language[i][0] <= 'Z'))
                {
                    int index = Language[i].IndexOf('=');
                    if (index > 0)
                    {
                        string key = Language[i].Remove(index);
                        if (!Exist(key)) lng.Add(key, Language[i].Substring(index + 1).Replace("\\n", "\n"));
                    }
                }
            }
        }


        public string this[string Key] { get { return lng.ContainsKey(Key) ? lng[Key] : Key; } }
        public bool Exist(string Key) { return lng.ContainsKey(Key); }
        public LanguageManager Add(string Key, string Value) { if (!Exist(Key)) lng.Add(Key, Value); return this; }
        public int Count { get { return lng.Count; } }
    }
}
