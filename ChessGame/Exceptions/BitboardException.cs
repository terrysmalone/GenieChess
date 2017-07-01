using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Exceptions
{
    [Serializable]
    public class BitboardException : Exception
    {
        public BitboardException()
            : base() { }

        public BitboardException(string message)
            : base(message) { }

        public BitboardException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public BitboardException(string message, Exception innerException)
            : base(message, innerException) { }

        public BitboardException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected BitboardException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
