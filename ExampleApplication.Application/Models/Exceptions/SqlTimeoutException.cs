using System;

namespace ExampleApplication.Application.Models.Exceptions
{
    [Serializable]
    public class SqlTimeoutException : Exception
    {
        public SqlTimeoutException() : base() { }
        public SqlTimeoutException(string message) : base(message) { }
        public SqlTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }
}
