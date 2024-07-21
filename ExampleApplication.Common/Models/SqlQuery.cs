using System.Collections.Generic;
using System.Data;

namespace ExampleApplication.Common.Models
{
    /// <summary>
    /// Class used to connect to databases and execute SQL queries.
    /// </summary>
    public class SqlQuery
    {
        /// <summary>
        /// SQL database connection string. Required.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// SQL query text. Required.
        /// </summary>
        public string QueryText { get; set; }

        /// <summary>
        /// SQL parameters. Default of empty dictionary (no parameters).
        /// </summary>
        public IDictionary<string, object?> Parameters { get; }

        /// <summary>
        /// SQL command type. Default of <seealso cref="CommandType.Text"/>.
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// SQL query timeout. Default of 30 seconds.
        /// </summary>
        public int QueryTimeout { get; set; }

        /// <summary>
        /// Initializes a new <seealso cref="SqlQuery"/>.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">Query text.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="queryTimeout">Query timeout.</param>
        public SqlQuery(string connectionString, string queryText, IDictionary<string, object?> parameters, CommandType commandType, int queryTimeout)
        {
            ConnectionString = connectionString;
            QueryText = queryText;
            Parameters = parameters;
            CommandType = commandType;
            QueryTimeout = queryTimeout;
        }

        /// <summary>
        /// Initializes a new <seealso cref="SqlQuery"/> with 30 second query timeout.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">Query text.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        public SqlQuery(string connectionString, string queryText, IDictionary<string, object?> parameters, CommandType commandType)
        {
            ConnectionString = connectionString;
            QueryText = queryText;
            Parameters = parameters;
            CommandType = commandType;
            QueryTimeout = 30;
        }

        /// <summary>
        /// Initializes a new text <seealso cref="SqlQuery"/> with 30 second query timeout.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">Query text.</param>
        /// <param name="parameters">SQL parameters.</param>
        public SqlQuery(string connectionString, string queryText, IDictionary<string, object?> parameters)
        {
            ConnectionString = connectionString;
            QueryText = queryText;
            Parameters = parameters;
            CommandType = CommandType.Text;
            QueryTimeout = 30;
        }

        /// <summary>
        /// Initializes a new <seealso cref="SqlQuery"/> with 30 second query timeout and no parameters.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">Query text.</param>
        /// <param name="commandType">Command type.</param>
        public SqlQuery(string connectionString, string queryText, CommandType commandType)
        {
            ConnectionString = connectionString;
            QueryText = queryText;
            Parameters = new Dictionary<string, object?>();
            CommandType = commandType;
            QueryTimeout = 30;
        }

        /// <summary>
        /// Initializes a new text <seealso cref="SqlQuery"/> with 30 second query timeout and no parameters.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">Query text.</param>
        public SqlQuery(string connectionString, string queryText)
        {
            ConnectionString = connectionString;
            QueryText = queryText;
            Parameters = new Dictionary<string, object?>();
            CommandType = CommandType.Text;
            QueryTimeout = 30;
        }
    }
}
