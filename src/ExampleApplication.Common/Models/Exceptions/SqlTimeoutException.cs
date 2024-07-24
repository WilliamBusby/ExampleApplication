using System;

namespace ExampleApplication.Common.Models.Exceptions
{
    /// <summary>
    /// Thrown when a SQL timeout occurs.
    /// </summary>
    [Serializable]
    public class SqlTimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <seealso cref="SqlTimeoutException"/> with custom message and inner exception.
        /// </summary>
        public SqlTimeoutException() : base() { }

        /// <summary>
        /// Initializes a new instance of <seealso cref="SqlTimeoutException"/> with custom message and inner exception.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public SqlTimeoutException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <seealso cref="SqlTimeoutException"/> with custom message and inner exception.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SqlTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }
}
