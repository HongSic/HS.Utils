using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HS.Utils.Text
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Array"></param>
        /// <param name="Separater"></param>
        /// <returns></returns>
        public static string Merge(this string[] Array, char Separater)
        {
            if (Array == null) return null;
            else if (Array.Length == 0) return "";
            else
            {
                bool First = true;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Array.Length; i++)
                {
                    if (First) { First = false; sb.Append(Array[i]); }
                    else { sb.Append(Separater).Append(Array[i]); }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 주어진 단위 만큼 문자열을 자릅니다 
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Distance"></param>
        /// <returns></returns>
        public static string[] Split(this string Text, int Distance) { return Split(Text, Distance, 0); }
        /// <summary>
        /// 주어진 단위 만큼 문자열을 자릅니다 
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Distance">자를 단위 값입니다</param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static string[] Split(this string Text, int Distance, int Count)
        {
            if (Count < 0) throw new ArgumentOutOfRangeException("Count", "Negative number does not allow");

            int Length = Text.Length / Distance;
            if (Length == 0) return new string[1] { Text };

            Length += (Text.Length % Distance == 0 ? 0 : 1);
            if (Count > 0) Length = Math.Min(Length, Count);

            string[] list = new string[Length];
            for (int i = 0, j = 0; i < Text.Length; i += Distance)
            {
                int split = i + Distance > Text.Length ? Text.Length - i : Distance;
                list[j++] = Text.Substring(i, split);
            }

            return list;
        }

        #region URI Utils
        /// <summary>
        /// 부모 주소와 하위 경로를 합쳐 최종 주소를 만들어줍니다
        /// </summary>
        /// <param name="URL">부모 URL 입니다</param>
        /// <param name="RelativeLink">하위 경로 입니다</param>
        /// <returns></returns>
        public static string ToAbsolutePath(this Uri URL, string RelativeLink) { return new Uri(URL, RelativeLink).AbsoluteUri; }
        /// <summary>
        /// 부모 주소와 하위 경로를 합쳐 최종 주소를 만들어줍니다
        /// </summary>
        /// <param name="URL">부모 URL 입니다</param>
        /// <param name="RelativeLink">하위 경로 입니다</param>
        /// <returns></returns>
        public static string ToAbsolutePath(this string URL, string RelativeLink) { return new Uri(URL).ToAbsolutePath(RelativeLink); }

        //https://rextester.com/AKMG13869
        //이 함수 현재 속도 1(4n) 을 1(n) 로 낮출 필요 있음
        /// <summary>
        ///  HTML 중 특수문자 (&amp; &nbsp; 등)을 필터링 합니다
        /// </summary>
        /// <param name="HTML">HTML 문자열 입니다</param>
        /// <returns></returns>
        public static string FilterHTMLTag(this string HTML)
        {
            string[] OldWords = { "&nbsp;", "&amp;", "&quot;", "&lt;", "&gt;" };
            string[] NewWords = { " ", "&", "\"", "<", ">" };
            //string[] OldWords = { "&nbsp;", "&amp;", "&quot;", "&lt;", "&gt;", "&reg;", "&copy;", "&bull;", "&trade;" };
            //string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢" };
            for (int i = 0; i < OldWords.Length; i++)
                HTML = HTML.Replace(OldWords[i], NewWords[i]);
            return HTML;
        }
        #endregion

        #region IO String Utils
        /// <summary>
        /// 현재 실행 파일이 있는 디렉터리 가져오기
        /// </summary>
        /// <returns></returns>
        public static string GetExcuteDirectory()
        {
            //string path = Process.GetCurrentProcess().Modules[1].FileName;
            //string path = Process.GetCurrentProcess().StartInfo.WorkingDirectory;
            string path = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(path);
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
            char[] orig = { '/', '\\', '"', ':', '?', '<', '>', '|' };
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
        /// 두 경로를 합쳐 올바른 경로로 만들어 줍니다 (경로문자는 현재 OS 를 따라갑니다)
        /// </summary>
        /// <param name="A">경로 1</param>
        /// <param name="B">경로 2</param>
        /// <returns>두 경로가 합쳐진 경로 입니다</returns>
        public static string PathMaker(string A, string B)
        {
            char PathChar = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';

            if (IsNullOrWhiteSpace(A) || IsNullOrWhiteSpace(B)) return A + B;
            else if (Same(A[A.Length - 1]) && Same(B[0])) return A + B.Substring(1, B.Length - 1);
            else if (Same(B[0]) || Same(A[A.Length - 1])) return A + B;
            else return A + PathChar + B;
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
            else if (A[A.Length - 1] == PathChar && B[0] == PathChar) return A + B.Substring(1, B.Length - 1);
            else if (B[0] == PathChar || A[A.Length - 1] == PathChar) return A + B;
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

            string dirchar;
            int index = GetLastIndexOfPath(Path, out dirchar);

            if (index > -1)
            {
                string result = Path.Remove(index);
                if (result == "") return dirchar;
                return result;
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
        /// <summary>
        /// DateTime 형식을 ISO8601 포맷에 맞게 반환합니다
        /// </summary>
        /// <param name="date">DateTime 인스턴스 입니다</param>
        /// <param name="TimeZone">타임존 포함 여부 입니다</param>
        /// <returns>yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz 로 이루어진 문자열 입니다.</returns>
        public static string ToISO8601(this DateTime date, bool TimeZone = true)
        {
            if (TimeZone) return date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
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
            return index < 0 ? null : Text.Substring(index + Search.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Search">검색할 문자 입니다</param>
        /// <param name="LastIndexOf">True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <param name="NotFoundNull">문자열을 찾을 수 없을때 True 면 null 을 False 면 원본 문자열을 반환합니다</param>
        /// <returns></returns>
        public static string RemoveOf(this string Text, char Search, bool LastIndexOf, bool NotFoundNull = true)
        {
            if (Text == null) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? NotFoundNull ? null : Text : Text.Remove(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Search">검색할 문자열 입니다</param>
        /// <param name="NotFoundNull">문자를 찾을 수 없을때 True 면 null 을 False 면 원본 문자를 반환합니다</param>
        /// <returns></returns>
        public static string RemoveOf(this string Text, string Search, bool LastIndexOf, bool NotFoundNull = true)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            int index = LastIndexOf ? Text.LastIndexOf(Search) : Text.IndexOf(Search);
            return index < 0 ? NotFoundNull ? null : Text : Text.Remove(index);
        }
        #endregion

        #region Between
        /// <summary>
        /// 시작 문자열와 끝 문자열 사이의 문자열를 가져옵니다 
        /// 만약 StartLastIndexOf 와 EndLastIndexOf 가 모두 True면 
        /// </summary>
        /// <param name="Text">원본 문자열 입니다</param>
        /// <param name="Start">시작 문자열 입니다</param>
        /// <param name="End">끝 문자열 입니다</param>
        /// <param name="EndNotFoundEmpty">True 면 끝 문자를 찾을 수 없을 때 null 반환을, False 면 시작점 부터 문자열 끝까지 가져옵니다</param>
        /// <param name="LastIndexOf">True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <param name="Direction">끝방향의 검색 방향을 지정합니다</param>
        /// <returns>  </returns>
        public static string Between(this string Text, string Start, string End, bool EndNotFoundEmpty, bool LastIndexOf = false, EndDirection Direction = EndDirection.StartIndexOf)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            else
            {
                int index1 = LastIndexOf ? Text.LastIndexOf(Start) : Text.IndexOf(Start);
                if (index1 < 0) return null;
                else index1 += Start.Length;

                int index2;
                if (Direction == EndDirection.StartLastIndexOf) index2 = index1 > 1 ? Text.Remove(index1 - 1).LastIndexOf(End) : -1;
                else if (Direction == EndDirection.TextLastIndexOf) index2 = Text.LastIndexOf(End);
                else if (Direction == EndDirection.TextIndexOf) index2 = Text.IndexOf(End);
                else index2 = Text.IndexOf(End, index1);


                if (index2 < index1)
                {
                    if (Direction == EndDirection.StartLastIndexOf) { index2++; return Text.Substring(index2, index1 - (index2 + 1)); }
                    else index2 = -1;
                }
                if (index2 < 0) return EndNotFoundEmpty ? null : Text.Substring(index1);
                else return Text.Substring(index1, index2 - index1);
            }
        }
        /// <summary>
        /// 시작 문자와 끝 문자 사이의 문자열를 가져옵니다
        /// </summary>
        /// <param name="Text">원본 문자 입니다</param>
        /// <param name="Start">시작 문자 입니다</param>
        /// <param name="End">끝 문자열 입니다</param>
        /// <param name="EndNotFoundEmpty">True 면 끝 문자를 찾을 수 없을 때 null 반환을, False 면 시작점 부터 문자열 끝까지 가져옵니다</param>
        /// <param name="LastIndexOf">시작문자열을 True 면 끝에서부터 False 면 처음부터 검색합니다</param>
        /// <param name="Direction">끝방향의 검색 방향을 지정합니다</param>
        /// <returns></returns>
        public static string Between(this string Text, char Start, char End, bool EndNotFoundEmpty, bool LastIndexOf = false, EndDirection Direction = EndDirection.StartIndexOf)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            else
            {
                int index1 = LastIndexOf ? Text.LastIndexOf(Start) : Text.IndexOf(Start);
                if (index1 < 0) return null;
                else index1++;

                int index2 = -1;
                if (Direction == EndDirection.StartLastIndexOf)
                {
                    if (index1 > 1)
                        for (int i = index1 - 2; i >= 0; i--)
                            if (Text[i] == End) { index2 = i; break; }
                }
                else if (Direction == EndDirection.TextLastIndexOf) index2 = Text.LastIndexOf(End);
                else if (Direction == EndDirection.TextIndexOf) index2 = Text.IndexOf(End);
                else index2 = Text.IndexOf(End);

                if (index2 < index1)
                {
                    if (Direction == EndDirection.StartLastIndexOf) { index2++; return Text.Substring(index2, index1 - (index2 + 1)); }
                    else index2 = -1;
                }
                if (index2 < 0) return EndNotFoundEmpty ? null : Text.Substring(index1);
                else return Text.Substring(index1, index2 - index1);
            }
        }

        /// <summary>
        /// 끝방향의 검색 방향입니다
        /// </summary>
        public enum EndDirection
        {
            /// <summary>
            /// 찾은 지점부터 순차적으로 검색합니다
            /// </summary>
            StartIndexOf,
            /// <summary>
            /// 찾은 지점부터 역방향으로 검색합니다
            /// </summary>
            StartLastIndexOf,
            /// <summary>
            /// 텍스트의 끝에서부터 검색합니다
            /// </summary>
            TextLastIndexOf,
            /// <summary>
            /// 텍스트의 처음부터 검색합니다
            /// </summary>
            TextIndexOf,
        }
        #endregion

        #region Check
        /// <summary>
        /// 이메일 주소 형식 검사
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static bool CheckEmailAddress(this string Email) { return Regex.IsMatch(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"); }
        
        #region CheckPhoneNumber
        /// <summary>
        /// 국제 전화번호 형식(E.164)을 검사합니다. 예: +82 10-1234-5678, +821012345678
        /// </summary>
        /// <param name="Phone">전화번호 문자열</param>
        /// <returns>국제 전화번호 형식이면 true</returns>
        public static bool CheckPhoneNumberGlobalFormat(this string Phone)
        {
            if (string.IsNullOrWhiteSpace(Phone)) return false;

            // 공백 제거
            var normalized = Regex.Replace(Phone, @"[\s\-]", "");

            // E.164 기반: +국가코드 + 숫자 (최소 8자리 ~ 최대 15자리)
            return Regex.IsMatch(normalized, @"^\+[1-9]\d{7,14}$");
        }
        
        /// <summary>
        /// 국내 전화번호(대한민국) 하이픈 포함 형식 검사
        /// - 휴대폰: 010-1234-5678, 011-123-4567 등
        /// - 유선(지역번호): 02-123-4567, 02-1234-5678, 031-123-4567 등
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool CheckPhoneNumber(this string Phone)
        {
            return CheckPhoneNumberKoreaDomesticHyphenFormat(Phone);
        }
        /// <summary>
        /// (호환/레거시) 3-3or4-4 형식만 허용하는 단순 검사
        /// 예: 010-1234-5678, 031-123-4567
        /// 주의: 02-1234-5678 같은 2자리 지역번호는 실패합니다.
        /// </summary>
        public static bool CheckPhoneNumberLegacyHyphen3_3or4_4(this string Phone)
        {
            if (string.IsNullOrWhiteSpace(Phone)) return false;
            return Regex.IsMatch(Phone, @"^\d{3}-\d{3,4}-\d{4}$");
        }

        /// <summary>
        /// 국내 전화번호(대한민국) 하이픈 포함 형식 검사
        /// - 02-xxxx-xxxx / 02-xxx-xxxx
        /// - 0xx-xxx-xxxx / 0xx-xxxx-xxxx
        /// </summary>
        public static bool CheckPhoneNumberKoreaDomesticHyphenFormat(this string Phone)
        {
            if (string.IsNullOrWhiteSpace(Phone)) return false;

            // 02(서울) 또는 0xx(그 외 지역/휴대폰) + 하이픈 + 3~4자리 + 하이픈 + 4자리
            // 예: 02-123-4567, 02-1234-5678, 031-123-4567, 010-1234-5678
            return Regex.IsMatch(Phone, @"^(02|0\d{2})-\d{3,4}-\d{4}$");
        }

        /// <summary>
        /// 휴대폰 번호(대한민국) 하이픈 포함 형식 검사
        /// 예: 010-1234-5678, 011-123-4567
        /// </summary>
        public static bool CheckPhoneNumberKoreaMobileHyphenFormat(this string Phone)
        {
            if (string.IsNullOrWhiteSpace(Phone)) return false;
            return Regex.IsMatch(Phone, @"^01[016789]-\d{3,4}-\d{4}$");
        }
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool CheckValidFileName(this string FileName)
        {
            if (!string.IsNullOrEmpty(FileName)) return false;

            var chars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < FileName.Length; i++)
                for (int j = 0; j < chars.Length; j++)
                    if (FileName[i] == chars[j]) return false;
            return true;
        }

        /// <summary>
        /// Check Alphabet, Number Only
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool CheckAlphabetNumber(this string Text)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                //아이디가 해당영역에 포함되어 있지 않으면...
                if (!(Text[i] >= 0x30 && Text[i] <= 0x39) && //숫자 영역
                    !(Text[i] >= 0x41 && Text[i] <= 0x5A) && //알파벳 대문자
                    !(Text[i] >= 0x61 && Text[i] <= 0x7A))   //알파벳 소문자
                    return false;
            }

            return true;
        }
        #endregion

        #region Nomalizer
        /// <summary>
        /// 국제코드 + 한국번호로 노멀라이징 합니다 (반드시 전화번호 형식 확인 후 작업해주세요)
        /// 예) <br/>
        /// 010-1234-5678 -> +82 10-1234-5678 <br/>
        /// +82 1012345678 -> +82 10-1234-5678 <br/>
        /// +1 1012345678 -> +1 10-1234-5678 <br/>
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static string NormalizeGlobalPhoneNumberKorea(this string Phone)
        {
            if (string.IsNullOrWhiteSpace(Phone)) return null;

            // 공백/하이픈 제거 (표시용 포맷으로 재구성할 것이므로 일단 숫자만 남기기)
            var raw = Regex.Replace(Phone, @"[\s\-]", "").Trim();

            // 국제번호: +<country><digits>
            if (raw.StartsWith("+"))
            {
                // 국가코드(1~3자리) + 나머지 번호(최소 8자리)
                // 예: +821012345678, +11012345678
                var match = Regex.Match(raw, @"^(\+[1-9]\d{0,2})(\d{8,15})$");
                if (!match.Success) return null;

                var country = match.Groups[1].Value; // +82, +1 등
                var number = match.Groups[2].Value;  // 나머지 숫자

                if (number.Length < 8) return null;

                // 뒤 8자리는 xxxx-xxxx 로 고정
                var mid = number.Substring(number.Length - 8, 4);
                var tail = number.Substring(number.Length - 4, 4);
                var head = number.Substring(0, number.Length - 8);

                // head가 너무 짧거나 길어도 일단 표시 포맷으로 반환
                return $"{country} {head}-{mid}-{tail}";
            }

            // 국내번호(대한민국): 0 + (2 또는 2자리 지역/휴대폰 코드) + (7~8자리)
            // 예: 01012345678, 0212345678, 0311234567
            var domestic = Regex.Match(raw, @"^0(2|\d{2})(\d{7,8})$");
            if (!domestic.Success) return null;

            var area = domestic.Groups[1].Value; // 2, 10, 31 등 (앞의 0 제거된 값)
            var rest = domestic.Groups[2].Value; // 7~8자리

            // 가운데 자리: 7자리면 3-4, 8자리면 4-4
            var mid2 = rest.Length == 8 ? rest.Substring(0, 4) : rest.Substring(0, 3);
            var tail2 = rest.Length == 8 ? rest.Substring(4, 4) : rest.Substring(3, 4);

            return $"+82 {area}-{mid2}-{tail2}";
        }
        #endregion

        //(StackOverflow 링크 추가)
        public static ulong ExtractNumber(this string Text)
        {
            ulong val = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                if (c >= '0' && c <= '9')
                {
                    val *= 10;
                    //(ASCII code reference)
                    val += (ulong)(c - 48);
                }
            }
            return val;
        }

        #region Hash
        public static string HashMD5(this string Text, bool UpperCase, Encoding EncodingApply = null)
        {
            return Text == null ? null :
                (EncodingApply == null ? Encoding.Default.GetBytes(Text) : EncodingApply.GetBytes(Text)).HashMD5(UpperCase);
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
