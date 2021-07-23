using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HS.Utils
{
    public static class StreamUtils
    {
        private const bool CLOSE = true;

        public static string GetString(this Stream Stream, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream);
                return reader.ReadToEnd();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }
        public static string GetString(this Stream Stream, Encoding Encoding, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream, Encoding);
                return reader.ReadToEnd();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }

        public static byte[] GetData(this Stream Stream, bool Close = CLOSE)
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


#if NETSTANDARD2_0_OR_GREATER && (!NET20 && !NET30 && !NET35 && !NET40)
        public static async Task<string> GetStringAsync(this Stream Stream, Encoding Encoding, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream, Encoding);
                return await reader.ReadToEndAsync();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }
        public static async Task<string> GetStringAsync(this Stream Stream, bool Close = CLOSE)
        {
            try
            {
                StreamReader reader = new StreamReader(Stream);
                return await reader.ReadToEndAsync();
            }
            finally { if (Close && Stream != null) Stream.Close(); }
        }

        public static async Task<byte[]> GetDataAsync(this Stream Stream, bool Close = CLOSE)
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
    }
}
