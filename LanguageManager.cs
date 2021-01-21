using System;
using System.Collections.Generic;

namespace HS.Utils
{
    public class LanguageManager : IDisposable
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

        public void Dispose()
        {
            lng.Clear();
        }

        /// <summary>
        /// Create new instance and concat
        /// </summary>
        /// <param name="Overwrite">Whether to overwrite when language key exist/param>
        /// <param name="Instances">LanguageManager instances</param>
        /// <returns></returns>
        public static LanguageManager Concat(bool Overwrite, params LanguageManager[] Instances)
        {
            return Merge(Overwrite, new LanguageManager(), Instances);
        }
        /// <summary>
        /// Merge the another LanguageManager instance to Original
        /// </summary>
        /// <param name="Overwrite">Whether to overwrite when language key exist</param>
        /// <param name="Original">Original LanguageManager instance</param>
        /// <param name="Manager">LanguageManager instances</param>
        /// <returns></returns>
        public static LanguageManager Merge(bool Overwrite, LanguageManager Original, params LanguageManager[] Manager)
        {
            if (Manager == null) return null;

            for (int i = 0; i < Manager.Length; i++)
            {
                foreach (var lng in Original.lng)
                {
                    if (!Original.lng.ContainsKey(lng.Key)) Original.lng.Add(lng.Key, lng.Value);
                    else if (Overwrite) Original.lng[lng.Key] = lng.Value;
                }
            }
            return Original;
        }
    }
}
