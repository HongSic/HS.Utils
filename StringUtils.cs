using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace HS.Utils
{
    public static class StringUtils
    {
        //public enum SearchDirection { Start, End }

        /*
         * Check Flag: (AFlag & BFlag) == BFlag
         * 
         * Adding Flag1: AFlag |= BFlag
         * Adding Flag2: AFlag = AFlag | BFlag
         * 
         * Remove Flag1: AFlag &= ~BFlag 
         * Remove Flag2: AFlag = AFlag & ~BFlag
         */


        public static string NextString(this Random Random, int Length, string Seed = "ABCDEFGHIGJKLMOPQUSTUVWXYZabcdefghigklmnopqustuvwxyz1234567890")
        {
            var stringChars = new char[Length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = Seed[Random.Next(Seed.Length)];
            }

            return new string(stringChars);
        }

        #region IO String Utils
        public static string GetExcutePath()
        {
            //string path = Process.GetCurrentProcess().Modules[1].FileName;
            //string path = Process.GetCurrentProcess().StartInfo.WorkingDirectory;
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return path.Remove(path.LastIndexOf("\\"));
            else return path.Remove(path.LastIndexOf("/"));
        }

        public static string MakeValidFileName(this string FileName, bool UnderBar = false, bool IgnorePathChar = true)
        {
            char[] orig = {'/', '\\', '"',  ':', '?', '<', '>', '|' };
            char[] repl = { 
                '⁄',  // U+2044 fraction slash 
                '＼', // U+33FC back slash
                '”', // U+201D right double quotation mark
                '：', '？', '＜', '＞', 'ǀ' };

            var array = FileName.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = IgnorePathChar ? 2 : 0; j < orig.Length; j++)
                    if (array[i] == orig[j]) array[i] = UnderBar ? '_' : repl[j];
            }
            return new string(array);
        }

        public static string PathMaker(string A, string B)
        {
            //char p = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';

            if (IsNullOrWhiteSpace(A) || IsNullOrWhiteSpace(B)) return A + B;
            else if (Same(A[A.Length - 1]) && Same(B[0])) return A + B.Substring(1, B.Length - 1);
            else if (Same(B[0]) || Same(A[A.Length - 1])) return A + B;
            else return A + "/" + B;
        }
        private static bool Same(char p) { return p == '/' || p == '\\'; }

        public static string GetDirectoryName(string Path)
        {
            if (IsNullOrWhiteSpace(Path) || Path == "\\" || Path == "/") return string.Empty;
            //if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            string dirchar;
            int index = GetLastIndexOfPath(Path, out dirchar);

            if (index > -1)
            {
                string result = Path.Remove(index);
                if (result == "") return dirchar;
                else return result;
            }
            return string.Empty;
        }

        public static string GetFileName(string Path)
        {
            if (IsNullOrWhiteSpace(Path)) return string.Empty;

            int index = GetLastIndexOfPath(Path);
            if (index < 0) return Path;
            else return Path.Substring(index + 1);
        }

        public static string GetCurrentModulePath() { return Assembly.GetCallingAssembly().Location; }

        private static int GetLastIndexOfPath(string Path) { string dirchar; return GetLastIndexOfPath(Path, out dirchar); }
        private static int GetLastIndexOfPath(string Path, out string dirchar)
        {
            int index1 = Path.LastIndexOf('\\');
            int index2 = Path.LastIndexOf('/');

            int index = Math.Max(index1, index2);
            if (index > -1) dirchar = index == index1 ? "\\" : "/";
            else dirchar = null;

            return index;

        }
        #endregion

        #region DateTime Utils
        public static string ToISO8601(this DateTime date, bool TimeZone = true)
        {
            if(TimeZone) return date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            else return date.ToString("o", CultureInfo.InvariantCulture);
        }
        #endregion

        #region SubStringOf, RemoveOf
        public static string SubStringOf(this string Text, char Search, bool LastIndexOf)
        {
            if (Text == null) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Substring(index + 1);
        }
        public static string SubStringOf(this string Text, string Search, bool LastIndexOf)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Substring(index + 1);
        }
        public static string RemoveOf(this string Text, char Search, bool LastIndexOf)
        {
            if (Text == null) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Remove(index);
        }
        public static string RemoveOf(this string Text, string Search, bool LastIndexOf)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Remove(index);
        }
        #endregion

        #region .Net 4.0 Under
        public static bool IsNullOrWhiteSpace(string Text)
        {
#if NET20 || NET35 || NET40
            return Text == null || Text == "" || Text.Trim() == "";
#else
            return string.IsNullOrWhiteSpace(Text);
#endif
        }
        #endregion

        [Obsolete]
        public static object EnumParse(Type enumType, params string[] values)
        {
            if (values.Length == 1) return Enum.Parse(enumType, values[0]);
            else if (typeof(Enum).Name == enumType.Name)
            {
                return null;
            }
            else return null;
        }
    }
}
