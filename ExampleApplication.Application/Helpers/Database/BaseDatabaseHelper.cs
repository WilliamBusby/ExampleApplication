using ExampleApplication.Application.Helpers.Extension;
using ExampleApplication.Application.Models.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using SqlException = ExampleApplication.Application.Models.Exceptions.SqlException;

namespace ExampleApplication.Application.Helpers.Database
{
    /// <summary>
    /// Base database helper with querying methods.
    /// </summary>
    public abstract class BaseDatabaseHelper
    {
        /// <summary>
        /// Protected constructor. All methods should be static, this constructor exists for classes inheriting from this class.
        /// </summary>
        protected BaseDatabaseHelper()
        { }

        /// <summary>
        /// Executes a SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected</param>
        /// <exception cref="SqlException">Thrown if the number of rows affected does not match <paramref name="expectedRowChanges"/>.</exception>
        protected static void ValidateNonQuery(string connectionString, string sql, Dictionary<string, object?> parameters, int expectedRowChanges)
        {
            int rowsAffected = NonQuery(connectionString, sql, parameters);

            if (rowsAffected != expectedRowChanges)
            {
                throw new SqlException("Invalid SQL rows.");
            }
        }

        /// <summary>
        /// Executes a parameterless SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected</param>
        protected static void ValidateNonQuery(string connectionString, string sql, int expectedRowChanges)
        {
            ValidateNonQuery(connectionString, sql, new(), expectedRowChanges);
        }

        /// <summary>
        /// Executes a SQL non-query.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Number of rows affected.</returns>
        protected static int NonQuery(string connectionString, string sql, Dictionary<string, object?> parameters)
        {
            using SqlConnection connection = new(connectionString);

            SqlCommand command = new()
            {
                CommandType = System.Data.CommandType.Text,
                CommandText = sql,
                Connection = connection,
            };

            _ = command.AddParameters(parameters);

            connection.Open();

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected;
        }

        /// <summary>
        /// Executes a parameterless SQL non-query.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected static int NonQuery(string connectionString, string sql)
        {
            return NonQuery(connectionString, sql, new());
        }

        /// <summary>
        /// Executes a SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultiple<T>(string connectionString, string sql, Dictionary<string, object?> parameters) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(connectionString);

            SqlCommand command = new()
            {
                CommandType = System.Data.CommandType.Text,
                CommandText = sql,
                Connection = connection,
            };

            _ = command.AddParameters(parameters);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            List<T> output = reader.GetMultiple<T>();

            return output;
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultiple<T>(string connectionString, string sql) where T : IDatabaseModel<T>
        {
            return QueryMultiple<T>(connectionString, sql, new());
        }

        /// <summary>
        /// Executes a SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// <br/>
        /// <typeparamref name="T"/> should be a simple type, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultipleSimple<T>(string connectionString, string sql, Dictionary<string, object?> parameters) where T : IConvertible
        {
            using SqlConnection connection = new(connectionString);

            SqlCommand command = new()
            {
                CommandType = System.Data.CommandType.Text,
                CommandText = sql,
                Connection = connection,
            };

            _ = command.AddParameters(parameters);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            List<T> output = reader.GetMultipleSimple<T>();

            return output;
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// <br/>
        /// <typeparamref name="T"/> should be a simple type, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultipleSimple<T>(string connectionString, string sql) where T : IConvertible
        {
            return QueryMultipleSimple<T>(connectionString, sql, new());
        }

        /// <summary>
        /// Executes a SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized value from SQL query.</returns>
        protected static T QuerySingle<T>(string connectionString, string sql, Dictionary<string, object?> parameters) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(connectionString);

            SqlCommand command = new()
            {
                CommandType = System.Data.CommandType.Text,
                CommandText = sql,
                Connection = connection,
            };

            _ = command.AddParameters(parameters);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            return reader.GetSingle<T>(false);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized value from SQL query.</returns>
        protected static T QuerySingle<T>(string connectionString, string sql) where T : IDatabaseModel<T>
        {
            return QuerySingle<T>(connectionString, sql, new());
        }

        /// <summary>
        /// Executes a SQL query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="SqlException">If there is no data to read.</exception>
        protected static T QueryScalar<T>(string connectionString, string sql, Dictionary<string, object?> parameters) where T : IConvertible
        {
            using SqlConnection connection = new(connectionString);

            SqlCommand command = new()
            {
                CommandType = System.Data.CommandType.Text,
                CommandText = sql,
                Connection = connection,
            };

            _ = command.AddParameters(parameters);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new SqlException("No value found.");
            }

            T output = reader.ReadValue<T>(0);

            return output;
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        protected static T QueryScalar<T>(string connectionString, string sql) where T : IConvertible
        {
            return QueryScalar<T>(connectionString, sql, new());
        }

        /// <summary>
        /// Executes a SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        protected static T? QueryNullableScalar<T>(string connectionString, string sql, Dictionary<string, object?> parameters) where T : IConvertible
        {
            using SqlConnection connection = new(connectionString);

            SqlCommand command = new()
            {
                CommandType = System.Data.CommandType.Text,
                CommandText = sql,
                Connection = connection,
            };

            _ = command.AddParameters(parameters);

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return default;
            }

            T? output = reader.ReadNullableValue<T>(0);

            return output;
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        protected static T? QueryNullableScalar<T>(string connectionString, string sql) where T : IConvertible
        {
            return QueryNullableScalar<T>(connectionString, sql, new());
        }
    }
}
