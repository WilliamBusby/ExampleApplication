using System;

namespace ExampleApplication.Application.Models.Exceptions
{
    [Serializable]
    public class MissingSqlResultException : Exception
    {
        public MissingSqlResultException() : base() { }
        public MissingSqlResultException(string message) : base(message) { }
        public MissingSqlResultException(string message, Exception innerException) : base(message, innerException) { }
    }
}
