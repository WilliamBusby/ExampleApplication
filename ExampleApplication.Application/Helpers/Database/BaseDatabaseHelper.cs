using ExampleApplication.Application.Helpers.Extension;
using ExampleApplication.Application.Models.Exceptions;
using ExampleApplication.Application.Models.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

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
        /// <param name="commandType">Command type.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected</param>
        /// <exception cref="SqlValidationException">Thrown if the number of rows affected does not match <paramref name="expectedRowChanges"/>.</exception>
        protected static void ValidateNonQuery(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType, int expectedRowChanges)
        {
            int rowsAffected = NonQuery(connectionString, sql, parameters, commandType);

            if (rowsAffected != expectedRowChanges)
            {
                throw new SqlValidationException($"Invalid SQL response, expected {expectedRowChanges} but got {rowsAffected}.");
            }
        }

        /// <summary>
        /// Executes a parameterless SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected</param>
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
        /// <param name="expectedRowChanges">Number of expected rows to be affected</param>
        protected static void ValidateNonQuery(string connectionString, string sql, IDictionary<string, object?> parameters, int expectedRowChanges)
        {
            ValidateNonQuery(connectionString, sql, parameters, CommandType.Text, expectedRowChanges);
        }

        /// <summary>
        /// Executes a parameterless text SQL non-query and validates that the number of rows affected is the same as the number of expected row changes.
        /// </summary>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="expectedRowChanges">Number of expected rows to be affected</param>
        protected static void ValidateNonQuery(string connectionString, string sql, int expectedRowChanges)
        {
            ValidateNonQuery(connectionString, sql, new Dictionary<string, object?>(), CommandType.Text, expectedRowChanges);
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
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            _ = command.AddParameters(parameters);

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
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            _ = command.AddParameters(parameters);

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
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            _ = command.AddParameters(parameters);

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
        /// Executes a SQL query and reads a single <typeparamref name="T"/> from the result.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized value from SQL query.</returns>
        /// <exception cref="SqlTimeoutException">If a SqlException is thrown when <seealso cref="SqlException.Number"/> is 2.</exception>
        protected static T QuerySingle<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            try
            {
                _ = command.AddParameters(parameters);

                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                if(!reader.Read())
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
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Serialized value from SQL query or default.</returns>
        protected static T? QueryNullableSingle<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IDatabaseModel<T>
        {
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            _ = command.AddParameters(parameters);

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
        /// Executes a SQL query and reads a single value from the first column.
        /// </summary>
        /// <typeparam name="T">Type to read from query response.</typeparam>
        /// <param name="connectionString">Connection string to database.</param>
        /// <param name="sql">SQL query to execute.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <param name="commandType">Command type.</param>
        /// <returns>Single value from the first column cast to <typeparamref name="T"/>.</returns>
        /// <exception cref="MissingSqlResultException">If there is no data to read.</exception>
        protected static T QueryScalar<T>(string connectionString, string sql, IDictionary<string, object?> parameters, CommandType commandType) where T : IConvertible
        {
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            _ = command.AddParameters(parameters);

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
            using SqlConnection connection = new(connectionString);

            using SqlCommand command = new(sql, connection)
            {
                CommandType = commandType
            };

            _ = command.AddParameters(parameters);

            try
            {
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();

                if(!reader.Read())
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
    }
}
