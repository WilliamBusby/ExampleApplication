using ExampleApplication.Common.Helpers;
using ExampleApplication.Common.Models;
using ExampleApplication.Common.Models.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ExampleApplication.Common.Helpers
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
        /// <param name="commandType">Command type.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected.</param>
        /// <exception cref="SqlValidationException">Thrown if the number of rows affected does not match <paramref name="expectedRowChanges"/>.</exception>
        protected static void ValidateNonQuery(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType, int expectedRowChanges)
        {
            SqlQuery sqlQuery = new SqlQuery(connectionString, sql, parameters, commandType);
            ValidateNonQuery(sqlQuery, expectedRowChanges);
        }

        /// <summary>
        /// Executes a parameterless SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected.</param>
        protected static void ValidateNonQuery(string connectionString, string sql, CommandType commandType, int expectedRowChanges)
        {
            ValidateNonQuery(connectionString, sql, new Dictionary<string, object?>(), commandType, expectedRowChanges);
        }

        /// <summary>
        /// Executes a text SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected.</param>
        protected static void ValidateNonQuery(string connectionString, string sql, IDictionary<string, object?> parameters, int expectedRowChanges)
        {
            ValidateNonQuery(connectionString, sql, parameters, CommandType.Text, expectedRowChanges);
        }

        /// <summary>
        /// Executes a parameterless text SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected.</param>
        protected static void ValidateNonQuery(string connectionString, string sql, int expectedRowChanges)
        {
            ValidateNonQuery(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text, expectedRowChanges);
        }

        /// <summary>
        /// Executes a SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected.</param>
        /// <exception cref="SqlValidationException">Thrown if the number of rows affected does not match <paramref name="expectedRowChanges"/>.</exception>
        protected static void ValidateNonQuery(SqlQuery sqlQuery, int expectedRowChanges)
        {
            int rowsAffected = NonQuery(sqlQuery);

            if (rowsAffected != expectedRowChanges)
            {
                throw new SqlValidationException($"Invalid SQL response, expected {expectedRowChanges} but got {rowsAffected}.");
            }
        }

        /// <summary>
        /// Executes a SQL non-query.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Number of rows affected.</returns>
        protected static int NonQuery(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType)
        {
            SqlQuery query = new(connectionString, sql, parameters, commandType);
            return NonQuery(query);
        }

        /// <summary>
        /// Executes a parameterless SQL non-query.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Number of rows affected.</returns>
        protected static int NonQuery(string connectionString, string sql, CommandType commandType)
        {
            return NonQuery(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }

        /// <summary>
        /// Executes a text SQL non-query.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Number of rows affected.</returns>
        protected static int NonQuery(string connectionString, string sql, IDictionary<string, object?> parameters)
        {
            return NonQuery(connectionString, sql, parameters, CommandType.Text);
        }

        /// <summary>
        /// Executes a parameterless text SQL non-query.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected static int NonQuery(string connectionString, string sql)
        {
            return NonQuery(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL non-query.
        /// </summary>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlTimeoutException">If a <seealso cref="SqlException"/> is thrown and <seealso cref="SqlException.Number"/> is -2.</exception>
        protected static int NonQuery(SqlQuery sqlQuery)
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout from SQL.", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultiple<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IDatabaseModel<T>
        {
            SqlQuery sqlQuery = new(connectionString, sql, parameters, commandType);
            return QueryMultiple<T>(sqlQuery);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultiple<T>(string connectionString, string sql, CommandType commandType) where T : IDatabaseModel<T>
        {
            return QueryMultiple<T>(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }
        
        /// <summary>
        /// Executes a text SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultiple<T>(string connectionString, string sql, IDictionary<string, object?> parameters) where T : IDatabaseModel<T>
        {
            return QueryMultiple<T>(connectionString, sql, parameters, CommandType.Text);
        }

        /// <summary>
        /// Executes a parameterless text SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultiple<T>(string connectionString, string sql) where T : IDatabaseModel<T>
        {
            return QueryMultiple<T>(connectionString, sql, new Dictionary<string, object?>());
        }

        /// <summary>
        /// Executes a SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Serialized values from SQL query.</returns>
        /// <exception cref="SqlTimeoutException">If a <seealso cref="SqlException"/> is thrown and <seealso cref="SqlException.Number"/> is -2.</exception>
        protected static List<T> QueryMultiple<T>(SqlQuery sqlQuery) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                List<T> output = reader.GetMultiple<T>();

                return output;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout detected from SQL.", ex);
            }
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
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultipleSimple<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IConvertible
        {
            SqlQuery sqlQuery = new(connectionString, sql, parameters, commandType);
            return QueryMultipleSimple<T>(sqlQuery);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// <br/>
        /// <typeparamref name="T"/> should be a simple type, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultipleSimple<T>(string connectionString, string sql, CommandType commandType) where T : IConvertible
        {
            return QueryMultipleSimple<T>(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }

        /// <summary>
        /// Executes a text SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// <br/>
        /// <typeparamref name="T"/> should be a simple type, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultipleSimple<T>(string connectionString, string sql, IDictionary<string, object?> parameters) where T : IConvertible
        {
            return QueryMultipleSimple<T>(connectionString, sql, parameters, CommandType.Text);
        }

        /// <summary>
        /// Executes a parameterless text SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// <br/>
        /// <typeparamref name="T"/> should be a simple type, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized values from SQL query.</returns>
        protected static List<T> QueryMultipleSimple<T>(string connectionString, string sql) where T : IConvertible
        {
            return QueryMultipleSimple<T>(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL query and reads multiple <typeparamref name="T"/> from the result.
        /// <br/>
        /// <typeparamref name="T"/> should be a simple type, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Serialized values from SQL query.</returns>
        /// <exception cref="SqlTimeoutException">If a <seealso cref="SqlException"/> is thrown and <seealso cref="SqlException.Number"/> is -2.</exception>
        protected static List<T> QueryMultipleSimple<T>(SqlQuery sqlQuery) where T : IConvertible
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                List<T> output = reader.GetMultipleSimple<T>();

                return output;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout detected from SQL.", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized value from SQL query.</returns>
        protected static T QuerySingle<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IDatabaseModel<T>
        {
            SqlQuery sqlQuery = new(connectionString, sql, parameters, commandType);
            return QuerySingle<T>(sqlQuery);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized value from SQL query.</returns>
        protected static T QuerySingle<T>(string connectionString, string sql, CommandType commandType) where T : IDatabaseModel<T>
        {
            return QuerySingle<T>(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }

        /// <summary>
        /// Executes a text SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized value from SQL query.</returns>
        protected static T QuerySingle<T>(string connectionString, string sql, IDictionary<string, object?> parameters) where T : IDatabaseModel<T>
        {
            return QuerySingle<T>(connectionString, sql, parameters, CommandType.Text);
        }

        /// <summary>
        /// Executes a parameterless text SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized value from SQL query.</returns>
        protected static T QuerySingle<T>(string connectionString, string sql) where T : IDatabaseModel<T>
        {
            return QuerySingle<T>(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Serialized value from SQL query.</returns>
        /// <exception cref="SqlTimeoutException">If a SqlException is thrown when <seealso cref="SqlException.Number"/> is 2.</exception>
        /// <exception cref="MissingSqlResultException">If there is no data to read.</exception>
        protected static T QuerySingle<T>(SqlQuery sqlQuery) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    throw new MissingSqlResultException("No value found.");
                }

                return reader.GetSingle<T>();
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout detected from SQL.", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized value from SQL query or default.</returns>
        protected static T? QueryNullableSingle<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IDatabaseModel<T>
        {
            SqlQuery sqlQuery = new(connectionString, sql, parameters, commandType);
            return QueryNullableSingle<T>(sqlQuery);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized value from SQL query or default.</returns>
        protected static T? QueryNullableSingle<T>(string connectionString, string sql, CommandType commandType) where T : IDatabaseModel<T>
        {
            return QueryNullableSingle<T>(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }

        /// <summary>
        /// Executes a text SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Serialized value from SQL query or default.</returns>
        protected static T? QueryNullableSingle<T>(string connectionString, string sql, IDictionary<string, object?> parameters) where T : IDatabaseModel<T>
        {
            return QueryNullableSingle<T>(connectionString, sql, parameters, CommandType.Text);
        }

        /// <summary>
        /// Executes a parameterless text SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Serialized value from SQL query or default.</returns>
        protected static T? QueryNullableSingle<T>(string connectionString, string sql) where T : IDatabaseModel<T>
        {
            return QueryNullableSingle<T>(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Serialized value from SQL query or default.</returns>
        /// <exception cref="SqlTimeoutException">If a SqlException is thrown when <seealso cref="SqlException.Number"/> is 2.</exception>
        protected static T? QueryNullableSingle<T>(SqlQuery sqlQuery) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    return default;
                }

                return reader.GetSingle<T>();
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout detected from SQL.", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        protected static T QueryScalar<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IConvertible
        {
            SqlQuery sqlQuery = new(connectionString, sql, parameters, commandType);
            return QueryScalar<T>(sqlQuery);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        protected static T QueryScalar<T>(string connectionString, string sql, CommandType commandType) where T : IConvertible
        {
            return QueryScalar<T>(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }

        /// <summary>
        /// Executes a SQL text query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        protected static T QueryScalar<T>(string connectionString, string sql, IDictionary<string, object?> parameters) where T : IConvertible
        {
            return QueryScalar<T>(connectionString, sql, parameters, CommandType.Text);
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
            return QueryScalar<T>(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="SqlTimeoutException">If a SqlException is thrown when <seealso cref="SqlException.Number"/> is 2.</exception>
        /// <exception cref="MissingSqlResultException">If there is no data to read.</exception>
        protected static T QueryScalar<T>(SqlQuery sqlQuery) where T : IConvertible
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    throw new MissingSqlResultException("No value found.");
                }

                T output = reader.ReadValue<T>(0);

                return output;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout detected from SQL.", ex);
            }
        }

        /// <summary>
        /// Executes a SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        protected static T? QueryNullableScalar<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IConvertible
        {
            SqlQuery sqlQuery = new(connectionString, sql, parameters, commandType);
            return QueryNullableScalar<T>(sqlQuery);
        }

        /// <summary>
        /// Executes a parameterless SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        protected static T? QueryNullableScalar<T>(string connectionString, string sql, CommandType commandType) where T : IConvertible
        {
            return QueryNullableScalar<T>(connectionString, sql, new Dictionary<string, object?>(), commandType);
        }

        /// <summary>
        /// Executes a text SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        protected static T? QueryNullableScalar<T>(string connectionString, string sql, IDictionary<string, object?> parameters) where T : IConvertible
        {
            return QueryNullableScalar<T>(connectionString, sql, parameters, CommandType.Text);
        }

        /// <summary>
        /// Executes a parameterless text SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        protected static T? QueryNullableScalar<T>(string connectionString, string sql) where T : IConvertible
        {
            return QueryNullableScalar<T>(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text);
        }
        
        /// <summary>
        /// Executes a SQL query and reads a single value from the first column or default if it does not exist.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="sqlQuery">SQL query information.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/> or default.</returns>
        /// <exception cref="SqlTimeoutException">If a SqlException is thrown when <seealso cref="SqlException.Number"/> is 2.</exception>
        protected static T? QueryNullableScalar<T>(SqlQuery sqlQuery) where T : IConvertible
        {
            using SqlConnection connection = new(sqlQuery.ConnectionString);

            using SqlCommand command = new(sqlQuery.QueryText, connection)
            {
                CommandType = sqlQuery.CommandType,
                CommandTimeout = sqlQuery.QueryTimeout,
            };

            _ = command.AddParameters(sqlQuery.Parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    return default;
                }

                T? output = reader.ReadNullableValue<T?>(0);

                return output;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                throw new SqlTimeoutException("Timeout detected from SQL.", ex);
            }
        }
    }
}
