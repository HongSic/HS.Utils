using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HS.Utils.Stream
{
    public static class StreamUtils
    {
        private const bool CLOSE = true;

        #region CopyStream
        public static System.IO.Stream CopyStream(this System.IO.Stream Source, System.IO.Stream Destination, int Buffer = 1024, bool ThrowException = true)
        {
            if (Source == null) { if (ThrowException) throw new ArgumentNullException("Source"); else return null; }
            if (Destination == null) { if (ThrowException) throw new ArgumentNullException("Destination"); else return null; }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (Buffer < 128) { if (ThrowException) throw new ArgumentNullException("Buffer is too small", "buffer"); else return null; }

            try
            {
                bool copying = true;

                byte[] buffer = new byte[Buffer];
                while (copying)
                {
                    int bytesRead = Source.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0) Destination.Write(buffer, 0, bytesRead);
                    else
                    {
                        Destination.Flush();
                        copying = false;
                    }
                }
                return Destination;
            }
            catch { if (ThrowException) throw; else return null; }
        }

#if NETCORE || NETCOREAPP || NETSTANDARD || NET45
        public static async Task<System.IO.Stream> CopyStreamAsync(this System.IO.Stream Source, System.IO.Stream Destination, int Buffer = 512, bool ThrowException = true)
        {
            if (Source == null) { if (ThrowException) throw new ArgumentNullException("Source"); else return null; }
            if (Destination == null) { if (ThrowException) throw new ArgumentNullException("Destination"); else return null; }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (Buffer < 128) { if (ThrowException) throw new ArgumentNullException("Buffer is too small", "buffer"); else return null; }

            try
            {
                bool copying = true;

                byte[] buffer = new byte[Buffer];
                while (copying)
                {
                    int bytesRead = await Source.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0) await Destination.WriteAsync(buffer, 0, bytesRead);
                    else
                    {
                        await Destination.FlushAsync();
                        copying = false;
                    }
                }
                return Destination;
            }
            catch { if (ThrowException) throw; else return null; }
        }
#endif

        public static void WriteStreamToFile(string Path, System.IO.Stream Stream, bool IsAppend = false)
        {
            FileStream fs = new FileStream(Path, IsAppend ? FileMode.Append : FileMode.Create);
            Stream.CopyStream(fs)?.Close();
        }

        public static System.IO.Stream GetResourceStream(string Namespace, string Name) { return GetResourceStream(Namespace + "." + Name, Assembly.GetCallingAssembly()); }
        public static System.IO.Stream GetResourceStream(string ResourcePath, Assembly FromAssembly = null)
        {
#if VS2019 || VS2017
            Assembly assembly = FromAssembly ?? Assembly.GetCallingAssembly();
#else
            Assembly assembly = FromAssembly == null ? Assembly.GetCallingAssembly() : FromAssembly;
#endif

            try { return assembly.GetManifestResourceStream(ResourcePath); }
            catch { return null; }
        }
        #endregion

        #region GetString, GetData
        public static string GetString(this System.IO.Stream Stream, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream);
                return reader.ReadToEnd();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }
        public static string GetString(this System.IO.Stream Stream, Encoding Encoding, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream, Encoding);
                return reader.ReadToEnd();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }

        public static byte[] GetData(this System.IO.Stream Stream, bool Close = CLOSE)
        {
            try
            {
                using (var memory = new MemoryStream())
                {
                    Stream.CopyStream(memory);
                    return memory.ToArray();
                }
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }


#if (NETSTANDARD2_0_OR_GREATER && (!NET20 && !NET30 && !NET35 && !NET40)) || NETCOREAPP || NETCORE || NETSTANDARD
        public static async Task<string> GetStringAsync(this System.IO.Stream Stream, Encoding Encoding, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream, Encoding);
                return await reader.ReadToEndAsync();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }
        public static async Task<string> GetStringAsync(this System.IO.Stream Stream, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream);
                return await reader.ReadToEndAsync();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }

        public static async Task<byte[]> GetDataAsync(this System.IO.Stream Stream, bool Close = CLOSE)
        {
            try
            {
                using (var memory = new MemoryStream())
                {
                    await Stream.CopyStreamAsync(memory);
                    return memory.ToArray();
                }
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }
#endif
        #endregion


        public static string HashMD5(this System.IO.Stream Stream, bool UpperCase)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Stream);
                if (Stream.CanSeek) Stream.Position = 0;

                StringBuilder sb = new StringBuilder();
                //Convert the byte array to hexadecimal string
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString(UpperCase ? "X2" : "x2"));
                return sb.ToString();
            }
        }
    }
}
