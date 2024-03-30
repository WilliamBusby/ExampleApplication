using System;

namespace ExampleApplication.Application.Models.Exceptions
{
    [Serializable]
    public class SqlException : Exception
    {
        public SqlException() : base() { }
        public SqlException(string message) : base(message) { }
        public SqlException(string message, Exception innerException) : base(message, innerException) { }
    }
}
