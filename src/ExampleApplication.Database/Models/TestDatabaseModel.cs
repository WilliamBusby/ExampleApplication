using ExampleApplication.Common.Helpers;
using ExampleApplication.Common.Models;
using System.Data;

namespace ExampleApplication.Database.Models
{
    /// <summary>
    /// Example model for testing.
    /// </summary>
    public class TestDatabaseModel : IDatabaseModel<TestDatabaseModel>
    {
        /// <summary>
        /// Example nullable string property.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Example non-nullable integer property.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Example constructor for reading from <seealso cref="IDataReader"/>.
        /// </summary>
        /// <param name="reader">Reader with query result.</param>
        public TestDatabaseModel(IDataReader reader)
        {
            Name = reader.ReadNullableValue<string>("Name");
            Age = reader.ReadValue<int>("Age");
        }

        /// <inheritdoc />
        public static TestDatabaseModel GetFromReader(IDataReader reader)
        {
            return new TestDatabaseModel(reader);
        }
    }
}
