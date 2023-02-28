using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HS.Utils
{
    public static class EncryptUtils
    {
        #region Hash
        public static string HashSHA256(this byte[] Data, bool IsUpper = true) { return HashSHA256(Data, 0, Data.Length, IsUpper); }
        public static string HashSHA256(this byte[] Data, int Offset, int Count, bool IsUpper = true) { return DataToString(new SHA256Managed().ComputeHash(Data, Offset, Count), IsUpper); }
        public static string HashSHA256(this System.IO.Stream BaseStream, bool IsUpper = true) { return DataToString(new SHA256Managed().ComputeHash(BaseStream), IsUpper); }
        private static string DataToString(byte[] Data, bool IsUpper)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < Data.Length; i++)
                stringBuilder.AppendFormat(IsUpper ? "{0:X2}" : "{0:x2}", Data[i]);
            return stringBuilder.ToString();
        }
        #endregion

        #region Encrypt / Decrypt AES
        //https://kimkitty.net/archives/1520

        private static byte[] UTF8Array(string Text) { return string.IsNullOrEmpty(Text) ? null : Encoding.UTF8.GetBytes(Text); }

        #region Encrypt / Decrypt AES Stream
        public static CryptoStream GetEncryptStreamAES(this System.IO.Stream BaseStream, string Password) { return _GetEncryptStreamAES(BaseStream, Password, null); }
        public static CryptoStream GetEncryptStreamAES(this System.IO.Stream BaseStream, string Password, string Salt) { return _GetEncryptStreamAES(BaseStream, Password, UTF8Array(Salt)); }
        public static CryptoStream GetEncryptStreamAES(this System.IO.Stream BaseStream, string Password, byte[] Salt) { return _GetEncryptStreamAES(BaseStream, Password, Salt); }
        private static CryptoStream _GetEncryptStreamAES(this System.IO.Stream BaseStream, string Password, byte[] Salt)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            var salt = Salt == null ? Encoding.ASCII.GetBytes(Password.Length.ToString()) : Salt;

            // PasswordDeriveBytes 클래스를 사용해서 SecretKey를 얻는다.
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, salt);
            // Create a encryptor from the existing SecretKey bytes.
            // encryptor 객체를 SecretKey로부터 만든다.
            // Secret Key에는 32바이트
            // (Rijndael의 디폴트인 256bit가 바로 32바이트입니다)를 사용하고, 
            // Initialization Vector로 16바이트
            // (역시 디폴트인 128비트가 바로 16바이트입니다)를 사용한다.
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            // CryptoStream객체를 암호화된 데이터를 쓰기 위한 용도로 선언
            return new CryptoStream(BaseStream, Encryptor, CryptoStreamMode.Write);
        }

        public static CryptoStream GetDecryptStreamAES(this System.IO.Stream BaseStream, string Password) { return _GetDecryptStreamAES(BaseStream, Password, null); }
        public static CryptoStream GetDecryptStreamAES(this System.IO.Stream BaseStream, string Password, byte[] Salt) { return _GetDecryptStreamAES(BaseStream, Password, Salt); }
        public static CryptoStream GetDecryptStreamAES(this System.IO.Stream BaseStream, string Password, string Salt) { return _GetDecryptStreamAES(BaseStream, Password, UTF8Array(Salt)); }
        private static CryptoStream _GetDecryptStreamAES(this System.IO.Stream BaseStream, string Password, byte[] Salt = null)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            // 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서
            // Salt를 사용한다.
            var salt = Salt == null ? Encoding.ASCII.GetBytes(Password.Length.ToString()) : Salt;

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, salt);
            // Decryptor 객체를 만든다.
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            // 데이터 읽기(복호화이므로) 용도로 cryptoStream객체를 선언, 초기화
            return new CryptoStream(BaseStream, Decryptor, CryptoStreamMode.Read);
        }
        #endregion

        #region Encrypt / Decrypt AES Data
        public static byte[] EncryptDataAES(this byte[] Data, string Password) { return _EncryptDataAES(Data, Password, null); }
        public static byte[] EncryptDataAES(this byte[] Data, string Password, byte[] Salt) { return _EncryptDataAES(Data, Password, Salt); }
        public static byte[] EncryptDataAES(this byte[] Data, string Password, string Salt) { return _EncryptDataAES(Data, Password, UTF8Array(Salt)); }
        private static byte[] _EncryptDataAES(this byte[] Data, string Password, byte[] Salt)
        {
            using (var memory = new MemoryStream())
            using (var crypto = GetEncryptStreamAES(memory, Password, Salt))
            {
                // 암호화 프로세스가 진행된다.
                crypto.Write(Data, 0, Data.Length);

                // 암호화 종료
                crypto.FlushFinalBlock();

                return memory.ToArray();
            }
        }

        public static byte[] DecryptDataAES(this byte[] EncryptedData, string Password) { return _DecryptDataAES(EncryptedData, Password, null); }
        public static byte[] DecryptDataAES(this byte[] EncryptedData, string Password, string Salt) { return _DecryptDataAES(EncryptedData, Password, UTF8Array(Salt)); }
        public static byte[] DecryptDataAES(this byte[] EncryptedData, string Password, byte[] Salt) { return _DecryptDataAES(EncryptedData, Password, Salt); }
        private static byte[] _DecryptDataAES(this byte[] EncryptedData, string Password, byte[] Salt)
        {
            using (var outmemory = new MemoryStream())
            using (var inmemory = new MemoryStream(EncryptedData))
            using (var crypto = GetDecryptStreamAES(inmemory, Password, Salt))
            {

                // 복호화된 데이터를 담을 바이트 배열을 선언한다.
                // 길이는 알 수 없지만, 일단 복호화되기 전의 데이터의 길이보다는
                // 길지 않을 것이기 때문에 그 길이로 선언한다.

                /*
                byte[] buffer = new byte[EncryptedData.Length];
                DecryptedCount = crypto.Read(buffer, 0, buffer.Length);
                */

                byte[] buffer = new byte[1024];

                // 복호화 시작
                int DecryptedCount = 0;
                while ((DecryptedCount = crypto.Read(buffer, 0, buffer.Length)) > 0)
                    outmemory.Write(buffer, 0, DecryptedCount);

                outmemory.Position = 0;
                return outmemory.ToArray();
            }
        }
        #endregion

        #region Encrypt / Decrypt AES String
        public static string EncryptToBase64AES(this string Text, string Password) { return Convert.ToBase64String(EncryptStringAES(Text, Password), Base64FormattingOptions.InsertLineBreaks); }
        public static string EncryptBase64AES(this string Text, string Password, string Salt) { return Convert.ToBase64String(EncryptStringAES(Text, Password, Salt), Base64FormattingOptions.InsertLineBreaks); }
        public static string EncryptToBase64AES(this string Text, string Password, byte[] Salt) { return Convert.ToBase64String(EncryptStringAES(Text, Password, Salt), Base64FormattingOptions.InsertLineBreaks); }

        private static byte[] EncryptStringAES(this string Text, string Password) { return _EncryptStringAES(Text, Password, null); }
        private static byte[] EncryptStringAES(this string Text, string Password, string Salt) { return _EncryptStringAES(Text, Password, UTF8Array(Salt)); }
        private static byte[] EncryptStringAES(this string Text, string Password, byte[] Salt) { return _EncryptStringAES(Text, Password, Salt); }
        private static byte[] _EncryptStringAES(this string Text, string Password, byte[] Salt)
        {
            if (Text == null) return null;
            // 입력받은 문자열을 바이트 배열로 변환
            byte[] PlainText = Encoding.UTF8.GetBytes(Text);
            // 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서 
            // Salt를 사용한다.
            return EncryptDataAES(PlainText, Password, Salt);
        }


        public static string DecryptBase64AES(this string EncryptedBase64, string Password) { return DecryptStringAES(Convert.FromBase64String(EncryptedBase64), Password); }
        public static string DecryptBase64AES(this string EncryptedBase64, string Password, string Salt = null) { return DecryptStringAES(Convert.FromBase64String(EncryptedBase64), Password, Salt); }
        public static string DecryptBase64AES(this string EncryptedBase64, string Password, byte[] Salt = null) { return DecryptStringAES(Convert.FromBase64String(EncryptedBase64), Password, Salt); }

        public static string DecryptStringAES(this byte[] EncryptedData, string Password) { return _DecryptStringAES(EncryptedData, Password, null); }
        public static string DecryptStringAES(this byte[] EncryptedData, string Password, string Salt) { return _DecryptStringAES(EncryptedData, Password, UTF8Array(Salt)); }
        public static string DecryptStringAES(this byte[] EncryptedData, string Password, byte[] Salt) { return _DecryptStringAES(EncryptedData, Password, Salt); }
        private static string _DecryptStringAES(this byte[] EncryptedData, string Password, byte[] Salt)
        {
            if(EncryptedData.Length == 0) return null;
            // 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서 
            // Salt를 사용한다.

            // 복호화 시작
            var data = DecryptDataAES(EncryptedData, Password, Salt);
            // 복호화된 데이터를 문자열로 바꾼다.
            return data == null || data.Length == 0 ? null : Encoding.UTF8.GetString(data);
        }
        #endregion
        #endregion
    }
}