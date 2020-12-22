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

        /// <summary>
        /// 랜덤문자열 반환
        /// </summary>
        /// <param name="Random">랜덤 인스턴스</param>
        /// <param name="Length">길이</param>
        /// <param name="Seed">랜덤 시드값</param>
        /// <returns></returns>
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
        /// <summary>
        /// 현재 실행 파일이 있는 디렉터리 가져오기
        /// </summary>
        /// <returns></returns>
        public static string GetExcutePath()
        {
            //string path = Process.GetCurrentProcess().Modules[1].FileName;
            //string path = Process.GetCurrentProcess().StartInfo.WorkingDirectory;
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return path.Remove(path.LastIndexOf("\\"));
            else return path.Remove(path.LastIndexOf("/"));
        }

        /// <summary>
        /// 올바른 파일이름으로 교정 (예: \ 을 ＼ 으로)
        /// </summary>
        /// <param name="FileName">교정할 파일이름</param>
        /// <param name="UnderBar">대체문자 대신 언더바(_)로 표시 여부</param>
        /// <param name="IgnorePathChar">디렉터리 구분 문자열은 건너뛰기 여부 (\, /)</param>
        /// <returns></returns>
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
        /// <summary>
        /// 두 경로를 합쳐 올바른 경로로 만들어 줍니다
        /// </summary>
        /// <param name="A">경로 1</param>
        /// <param name="B">경로 2</param>
        /// <returns>두 경로가 합쳐진 경로 입니다</returns>
        public static string PathMaker(string A, string B)
        {
            char c = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';
            return PathMaker(A, B, c);
        }
        /// <summary>
        /// 두 경로를 합쳐 올바른 경로로 만들어 줍니다
        /// </summary>
        /// <param name="A">경로 1</param>
        /// <param name="B">경로 2</param>
        /// <param name="PathChar">두 경로 사이에 붙을 경로 문자 입니다</param>
        /// <returns>두 경로가 합쳐진 경로 입니다</returns>
        public static string PathMaker(string A, string B, char PathChar)
        {
            if (IsNullOrWhiteSpace(A) || IsNullOrWhiteSpace(B)) return A + B;
            else if (Same(A[A.Length - 1]) && Same(B[0])) return A + B.Substring(1, B.Length - 1);
            else if (Same(B[0]) || Same(A[A.Length - 1])) return A + B;
            else return A + PathChar + B;
        }
        private static bool Same(char p) { return p == '/' || p == '\\'; }

        /// <summary>
        /// 디렉터리 이름을 가져옵니다
        /// </summary>
        /// <param name="Path">경로</param>
        /// <returns>디렉터리 이름을 반환합니다</returns>
        public static string GetDirectoryName(string Path)
        {
            if (IsNullOrWhiteSpace(Path) || Path == "\\" || Path == "/") return string.Empty;
            //if (string.IsNullOrWhiteSpace(path)) return string.Empty;

#if VS2019 || VS2017
            int index = GetLastIndexOfPath(Path, out string dirchar);
#else
            int index = GetLastIndexOfPath(Path, out dirchar);
#endif

            if (index > -1)
            {
                string result = Path.Remove(index);
                if (result == "") return dirchar;
                else return result;
            }
            return string.Empty;
        }

        /// <summary>
        /// 파일이름을 가져옵니다
        /// </summary>
        /// <param name="Path">파일 경로 입니다</param>
        /// <returns>파일이름 입니다</returns>
        public static string GetFileName(string Path)
        {
            if (IsNullOrWhiteSpace(Path)) return string.Empty;

            int index = GetLastIndexOfPath(Path);
            if (index < 0) return Path;
            else return Path.Substring(index + 1);
        }

        /// <summary>
        /// 현재 이 함수를 호출한 어셈블리의 파일이름을 가져옵니다
        /// </summary>
        /// <returns>이 함수를 호출한 어셈블리의 파일이름 입니다</returns>
        public static string GetCurrentModulePath() { return Assembly.GetCallingAssembly().Location; }

#if VS2019 || VS2017
        private static int GetLastIndexOfPath(string Path) { return GetLastIndexOfPath(Path, out _); }
#else
        string dirchar;
        private static int GetLastIndexOfPath(string Path) { string dirchar; return GetLastIndexOfPath(Path, out dirchar); }
#endif

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
        /// <summary>
        /// DateTime 형식을 ISO8601 포맷에 맞게 반환합니다
        /// </summary>
        /// <param name="date">DateTime 인스턴스 입니다</param>
        /// <param name="TimeZone">타임존 포함 여부 입니다</param>
        /// <returns>yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz 로 이루어진 문자열 입니다.</returns>
        public static string ToISO8601(this DateTime date, bool TimeZone = true)
        {
            if(TimeZone) return date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            else return date.ToString("o", CultureInfo.InvariantCulture);
        }
        #endregion

        #region SubStringOf, RemoveOf
        /// <summary>
        /// 특정 문자의 위치를 중심으로 
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Search">검색할 문자 입니다</param>
        /// <param name="LastIndexOf">True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <returns></returns>
        public static string SubStringOf(this string Text, char Search, bool LastIndexOf)
        {
            if (Text == null) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Substring(index + 1);
        }
        /// <summary>
        /// 특정 문자열의 위치를 중심으로
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Search">검색할 문자열 입니다</param>
        /// <param name="LastIndexOf">True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <returns></returns>
        public static string SubStringOf(this string Text, string Search, bool LastIndexOf)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Substring(index + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Search">검색할 문자 입니다</param>
        /// <param name="LastIndexOf">True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <returns></returns>
        public static string RemoveOf(this string Text, char Search, bool LastIndexOf)
        {
            if (Text == null) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Remove(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Search">검색할 문자열 입니다</param>
        /// <param name="LastIndexOf">True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <returns></returns>
        public static string RemoveOf(this string Text, string Search, bool LastIndexOf)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? null : Text.Remove(index);
        }
#endregion

#region .Net 4.0 Under
        /// <summary>
        /// 문자열이 공백 또는 비었는지의 결과를 판단합니다
        /// </summary>
        /// <param name="Text"></param>
        /// <returns>공백 또는 빈 문자열인지의 여부입니다</returns>
        public static bool IsNullOrWhiteSpace(string Text)
        {
#if NET20 || NET35 || NET40
            return Text == null || Text == "" || Text.Trim() == "";
#else
            return string.IsNullOrWhiteSpace(Text);
#endif
        }
#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
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
