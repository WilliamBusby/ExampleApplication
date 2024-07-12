using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace ExampleApplication.Application.Helpers.Extension
{
    /// <summary>
    /// Extension methods for <seealso cref="SqlCommand"/>.
    /// </summary>
    public static class SqlCommandExtension
    {
        /// <summary>
        /// Adds <paramref name="parameters"/> to the <seealso cref="SqlCommand.Parameters"/>.
        /// </summary>
        /// <param name="command">SqlCommand to add parameters to.</param>
        /// <param name="parameters">SQL parameters.</param>
        /// <returns>SqlParameters added to the command.</returns>
        public static List<SqlParameter> AddParameters(this SqlCommand command, IDictionary<string, object?> parameters)
        {
            List<SqlParameter> output = new();

            foreach (KeyValuePair<string, object?> parameter in parameters)
            {
                SqlParameter outputParameter = command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                output.Add(outputParameter);
            }

            return output;
        }
    }
}
