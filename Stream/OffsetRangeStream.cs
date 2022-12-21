using System;
using System.IO;
using System.Threading.Tasks;

namespace HS.Utils.Stream
{
    /// <summary>
    /// 
    /// </summary>
    public class OffsetRangeStream : System.IO.Stream
    {
        public OffsetRangeStream(System.IO.Stream BaseStream, long Offset, bool LeaveOpen = false)
        {
            this.BaseStream = BaseStream;
            this.Offset = Offset;
            this.LeaveOpen = LeaveOpen;

            BaseStream.Position = Offset;
        }
        public OffsetRangeStream(System.IO.Stream BaseStream, long Offset, long MaxLength, bool LeaveOpen = false): this(BaseStream, Offset, LeaveOpen)
        {
            this.MaxLength = MaxLength;
        }

        public bool LeaveOpen { get; set; } = false;

        public long MaxLength { get; private set; }
        public bool MaxLengthEnable => MaxLength > 0;

        public long Offset { get; private set; }
        public System.IO.Stream BaseStream { get; private set; }

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length - Offset;

        public override long Position { get => BaseStream.Position - Offset; set => BaseStream.Position = value + Offset; }

        public override void Flush() => BaseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            int count_new = count;
            if (MaxLengthEnable && Length + count > MaxLength)
                count_new = (int)(Length + count - MaxLength);

            return BaseStream.Read(buffer, offset, count_new);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long offset_new = offset;
            if (MaxLengthEnable)
            {
                if (origin == SeekOrigin.Current) offset_new = Position + offset;
                else if (origin == SeekOrigin.End) offset_new = Length + offset;

                if (offset_new > MaxLength) throw new Exception(string.Format("Offset ({0}) cannot over than MaxLength ({1})", offset, MaxLength));

                //return BaseStream.Seek(offset_new + Offset, SeekOrigin.Begin);
            }

            return BaseStream.Seek(offset + (origin == SeekOrigin.Begin ? Offset : 0), origin);
        }

        public override void SetLength(long value) => BaseStream.SetLength(value + Offset);

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (MaxLengthEnable && Length + count > MaxLength)
            {
                long count_new = Length + count - MaxLength;
                BaseStream.Write(buffer, offset, (int)count_new);
            }
            else BaseStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            if(!LeaveOpen) BaseStream.Close();
        }

        public new void Dispose()
        {
            if (!LeaveOpen) BaseStream.Dispose();
        }

#if NETCORE || NETCOREAPP || NETSTANDARD || NET45
        public override ValueTask DisposeAsync()
        {
            if (!LeaveOpen) return BaseStream.DisposeAsync();
            else return new ValueTask();
        }
#endif
    }
}
