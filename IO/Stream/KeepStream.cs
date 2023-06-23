using System.IO;

namespace HS.Utils.IO.Stream
{
    public class KeepStream : System.IO.Stream
    {
        public KeepStream(System.IO.Stream BaseStream, bool LeaveOpen = true)
        {
            this.BaseStream = BaseStream;
            this.LeaveOpen = LeaveOpen;
        }
        public System.IO.Stream BaseStream { get; private set; }
        public bool LeaveOpen { get; set; }

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override void Flush() => BaseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        public override void SetLength(long value) => BaseStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);

        public override void Close()
        {
            if (!LeaveOpen) BaseStream.Close();
        }

        public new void Dispose()
        {
            if (!LeaveOpen) BaseStream.Dispose();
        }
        /*
#if NETCORE || NETCOREAPP || NETSTANDARD || NET45
        public override ValueTask DisposeAsync()
        {
            if (!LeaveOpen) return BaseStream.DisposeAsync();
            else return new ValueTask();
        }
#endif
        */
    }
}
