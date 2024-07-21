using System;

namespace ExampleApplication.Common.Models.Exceptions
{
    /// <summary>
    /// Thrown when a SQL validation check fails.
    /// </summary>
    [Serializable]
    public class SqlValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <seealso cref="SqlValidationException"/> with custom message and inner exception.
        /// </summary>
        public SqlValidationException() : base() { }

        /// <summary>
        /// Initializes a new instance of <seealso cref="SqlValidationException"/> with custom message and inner exception.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public SqlValidationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <seealso cref="SqlValidationException"/> with custom message and inner exception.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SqlValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
