using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyValueStore.Core.Exceptions
{
    public class InvalidKeyException : ArgumentException
    {
        public InvalidKeyException()
            : base("The provided key is invalid.") { }

        public InvalidKeyException(string message)
            : base(message) { }

        public InvalidKeyException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidKeyException(string message, string paramName)
            : base(message, paramName) { }
    }
}
