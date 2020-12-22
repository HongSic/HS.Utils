using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace HS.Utils
{
    public static class IOUtils
    {
        public static Stream CopyStream(this Stream Source, Stream Destination, int Buffer = 1024, bool ThrowException = true)
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
            catch (Exception ex) { if (ThrowException) throw ex; else return null; }
        }
        public static async Task<Stream> CopyStreamAsync(this Stream Source, Stream Destination, int Buffer = 512, bool ThrowException = true)
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
            catch (Exception ex) { if (ThrowException) throw ex; else return null; }
        }

        public static void WriteStreamToFile(string Path, Stream Stream, bool IsAppend = false)
        {
            FileStream fs = new FileStream(Path, IsAppend ? FileMode.Append : FileMode.Create);
            CopyStream(Stream, fs)?.Close();
        }

        public static Stream GetResourceStream(string Namespace, string Name) { return GetResourceStream(Namespace + "." + Name, Assembly.GetCallingAssembly()); }
        public static Stream GetResourceStream(string ResourcePath, Assembly FromAssembly = null)
        {
#if VS2019 || VS2017
            Assembly assembly = FromAssembly ?? Assembly.GetCallingAssembly();
#else
            Assembly assembly = FromAssembly == null ? Assembly.GetCallingAssembly() : FromAssembly;
#endif

            try { return assembly.GetManifestResourceStream(ResourcePath); }
            catch { return null; }
        }
    }
}
