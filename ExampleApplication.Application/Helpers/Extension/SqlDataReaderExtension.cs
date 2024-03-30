using ExampleApplication.Application.Models.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using SqlException = ExampleApplication.Application.Models.Exceptions.SqlException;

namespace ExampleApplication.Application.Helpers.Extension
{
    /// <summary>
    /// Extension methods for <seealso cref="SqlDataReader"/>.
    /// </summary>
    public static class SqlDataReaderExtension
    {
        /// <summary>
        /// Gets a nullable value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="key">Column name.</param>
        /// <returns>Value from column or default if it is null.</returns>
        public static T? ReadNullableValue<T>(this SqlDataReader reader, string key)
        {
            int ordinal = reader.GetOrdinal(key);
            return reader.ReadNullableValue<T?>(ordinal);
        }

        /// <summary>
        /// Gets a nullable value from the <paramref name="reader"/> by column ordinal.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="ordinal">Column number.</param>
        /// <returns>Value from column or default if it is null.</returns>
        public static T? ReadNullableValue<T>(this SqlDataReader reader, int ordinal)
        {
            return reader.ReadNullableValue<T?>(ordinal, default);
        }

        /// <summary>
        /// Gets a nullable value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="key">Column name.</param>
        /// <param name="defaultValue">Default value if value is null.</param>
        /// <returns>Value from column or <paramref name="defaultValue"/> if it is null.</returns>
        public static T ReadNullableValue<T>(this SqlDataReader reader, string key, T defaultValue)
        {
            int ordinal = reader.GetOrdinal(key);
            return reader.ReadNullableValue<T>(ordinal, defaultValue);
        }

        /// <summary>
        /// Gets a nullable value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="ordinal">Column number.</param>
        /// <param name="defaultValue">Default value if value is null.</param>
        /// <returns>Value from column or <paramref name="defaultValue"/> if it is null.</returns>
        public static T ReadNullableValue<T>(this SqlDataReader reader, int ordinal, T defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetFieldValue<T>(ordinal);
        }

        /// <summary>
        /// Gets a value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="key">Column name.</param>
        /// <returns>Value from column.</returns>
        public static T ReadValue<T>(this SqlDataReader reader, string key)
        {
            int ordinal = reader.GetOrdinal(key);
            return reader.ReadValue<T>(ordinal);
        }

        /// <summary>
        /// Gets a value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="ordinal">Column number.</param>
        /// <returns>Value from column.</returns>
        public static T ReadValue<T>(this SqlDataReader reader, int ordinal)
        {
            return reader.GetFieldValue<T>(ordinal);
        }

        /// <summary>
        /// Gets a single value of <typeparamref name="T"/> from <paramref name="reader"/> using <seealso cref="IDatabaseModel{T}.GetFromReader(SqlDataReader)"/>.
        /// </summary>
        /// <typeparam name="T">Type returned from the query.</typeparam>
        /// <param name="reader">SqlDataReader from the query.</param>
        /// <param name="quietFail">If the value should quietly fail instead of throwing an exception</param>
        /// <returns>The type returned from the query.</returns>
        /// <exception cref="SqlException">If <paramref name="quietFail"/> is false and there is no row to read.</exception>
        public static T GetSingle<T>(this SqlDataReader reader, bool quietFail) where T : IDatabaseModel<T>
        {
            if (!quietFail && !reader.Read())
            {
                throw new SqlException("Failed to read value from SQL.");
            }
            
            T output = T.GetFromReader(reader);

            return output;
        }

        /// <summary>
        /// Gets multiple values of <typeparamref name="T"/> from <paramref name="reader"/> using <seealso cref="IDatabaseModel{T}.GetFromReader(SqlDataReader)"/>.
        /// </summary>
        /// <typeparam name="T">Type returned from the query.</typeparam>
        /// <param name="reader">SqlDataReader from the query.</param>
        /// <returns>List of <typeparamref name="T"/> from the query.</returns>
        public static List<T> GetMultiple<T>(this SqlDataReader reader) where T : IDatabaseModel<T>
        {
            List<T> output = new();

            while (reader.Read())
            {
                T value = T.GetFromReader(reader);
                output.Add(value);
            }

            return output;
        }

        /// <summary>
        /// Gets multiple values of <typeparamref name="T"/> from <paramref name="reader"/> using the first column ordinal.
        /// <br/>
        /// This should be used with simple types, e.g. <seealso cref="string"/>, <seealso cref="int"/>.
        /// </summary>
        /// <typeparam name="T">Type returned from the query.</typeparam>
        /// <param name="reader">SqlDataReader from the query.</param>
        /// <returns>List of <typeparamref name="T"/> from the query.</returns>
        public static List<T> GetMultipleSimple<T>(this SqlDataReader reader) where T : IConvertible
        {
            List<T> output = new();

            while (reader.Read())
            {
                T value = reader.ReadValue<T>(0);
                output.Add(value);
            }

            return output;
        }
    }
}
