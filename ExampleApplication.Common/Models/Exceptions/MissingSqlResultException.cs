using System;

namespace ExampleApplication.Common.Models.Exceptions
{
    /// <summary>
    /// Thrown when a non-nullable SQL result returns null or no result.
    /// </summary>
    [Serializable]
    public class MissingSqlResultException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <seealso cref="MissingSqlResultException"/>.
        /// </summary>
        public MissingSqlResultException() : base() { }

        /// <summary>
        /// Initializes a new instance of <seealso cref="MissingSqlResultException"/> with custom message.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public MissingSqlResultException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <seealso cref="MissingSqlResultException"/> with custom message and inner exception.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public MissingSqlResultException(string message, Exception innerException) : base(message, innerException) { }
    }
}
