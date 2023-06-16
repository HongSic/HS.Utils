using System;

namespace HS.Utils.Convert
{
    public class ConvertFailException : Exception
    {
        public ConvertFailException() { }
        public ConvertFailException(string Message) : base(Message) { }
        public ConvertFailException(Exception InnerException) : base(InnerException.Message, InnerException) { }
        public ConvertFailException(string Message, Exception InnerException) : base(Message, InnerException) { }
    }
}
