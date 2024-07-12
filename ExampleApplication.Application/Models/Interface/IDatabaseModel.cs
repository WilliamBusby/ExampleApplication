using System.Data;

namespace ExampleApplication.Application.Models.Interface
{
    /// <summary>
    /// Database model used for querying.
    /// </summary>
    /// <typeparam name="T">Type of database model.</typeparam>
    public interface IDatabaseModel<T> where T : IDatabaseModel<T>
    {
        /// <summary>
        /// Gets an object of type <typeparamref name="T"/> from an <seealso cref="IDataReader"/> via SQL query.
        /// </summary>
        /// <param name="reader">IDataReader from query.</param>
        /// <returns>Object from query result.</returns>
        public static abstract T GetFromReader(IDataReader reader);
    }
}
