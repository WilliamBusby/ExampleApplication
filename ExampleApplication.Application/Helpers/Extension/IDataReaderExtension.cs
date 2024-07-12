using ExampleApplication.Application.Models.Exceptions;
using ExampleApplication.Application.Models.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ExampleApplication.Application.Helpers.Extension
{
    /// <summary>
    /// Extension methods for <seealso cref="IDataReader"/>.
    /// </summary>
    public static class IDataReaderExtension
    {
        /// <summary>
        /// Gets a nullable value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="key">Column name.</param>
        /// <returns>Value from column or default if it is null.</returns>
        public static T? ReadNullableValue<T>(this IDataReader reader, string key)
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
        public static T? ReadNullableValue<T>(this IDataReader reader, int ordinal)
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
        public static T? ReadNullableValue<T>(this IDataReader reader, string key, T? defaultValue)
        {
            int ordinal = reader.GetOrdinal(key);
            return reader.ReadNullableValue(ordinal, defaultValue);
        }

        /// <summary>
        /// Gets a nullable value from the <paramref name="reader"/> by column ordinal.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="ordinal">Column number.</param>
        /// <param name="defaultValue">Default value if value is null.</param>
        /// <returns>Value from column or <paramref name="defaultValue"/> if it is null.</returns>
        public static T? ReadNullableValue<T>(this IDataReader reader, int ordinal, T? defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : (T)reader.GetValue(ordinal);
        }

        /// <summary>
        /// Gets a value from the <paramref name="reader"/> by column name.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="key">Column name.</param>
        /// <returns>Value from column.</returns>
        public static T ReadValue<T>(this IDataReader reader, string key)
        {
            int ordinal = reader.GetOrdinal(key);
            return reader.ReadValue<T>(ordinal);
        }

        /// <summary>
        /// Gets a value from the <paramref name="reader"/> by column ordinal.
        /// </summary>
        /// <typeparam name="T">Column type.</typeparam>
        /// <param name="reader">SqlDataReader from query.</param>
        /// <param name="ordinal">Column number.</param>
        /// <returns>Value from column.</returns>
        public static T ReadValue<T>(this IDataReader reader, int ordinal)
        {
            return (T)reader.GetValue(ordinal);
        }

        /// <summary>
        /// Gets a single value of <typeparamref name="T"/> from <paramref name="reader"/> using <seealso cref="IDatabaseModel{T}.GetFromReader(SqlDataReader)"/>.
        /// </summary>
        /// <typeparam name="T">Type returned from the query.</typeparam>
        /// <param name="reader">SqlDataReader from the query.</param>
        /// <returns>The type returned from the query.</returns>
        public static T GetSingle<T>(this IDataReader reader) where T : IDatabaseModel<T>
        {
            T output = T.GetFromReader(reader);

            return output;
        }

        /// <summary>
        /// Gets multiple values of <typeparamref name="T"/> from <paramref name="reader"/> using <seealso cref="IDatabaseModel{T}.GetFromReader(IDataReader)"/>.
        /// </summary>
        /// <typeparam name="T">Type returned from the query.</typeparam>
        /// <param name="reader">SqlDataReader from the query.</param>
        /// <returns>List of <typeparamref name="T"/> from the query.</returns>
        public static List<T> GetMultiple<T>(this IDataReader reader) where T : IDatabaseModel<T>
        {
            List<T> output = [];

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
        public static List<T> GetMultipleSimple<T>(this IDataReader reader) where T : IConvertible
        {
            List<T> output = [];

            while (reader.Read())
            {
                T value = reader.ReadValue<T>(0);
                output.Add(value);
            }

            return output;
        }
    }
}
