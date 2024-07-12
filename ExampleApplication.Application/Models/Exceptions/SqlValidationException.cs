using System;

namespace ExampleApplication.Application.Models.Exceptions
{
    [Serializable]
    public class SqlValidationException : Exception
    {
        public SqlValidationException() : base() { }
        public SqlValidationException(string message) : base(message) { }
        public SqlValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
