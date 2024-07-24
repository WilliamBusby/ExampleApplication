using System.Collections.Generic;
using ExampleApplication.Common.Helpers;
using ExampleApplication.Common.Models;
using ExampleApplication.Database.Models;

namespace ExampleApplication.Database.Helpers
{
    public class TestDatabaseHelper : BaseDatabaseHelper, ITestDatabaseHelper
    {
        private readonly string _ConnectionString;

        public TestDatabaseHelper(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        /// <inheritdoc/>
        public TestDatabaseModel GetTestQuery()
        {
            string sql = "SELECT 'Test' [Name], 18 [Age]";
            SqlQuery query = new(_ConnectionString, sql);

            return QuerySingle<TestDatabaseModel>(query);
        }

        public string GetTestQuery2()
        {
            string sql = "SELECT 'Test2'";
            return QueryScalar<string>(_ConnectionString, sql);
        }

        public List<TestDatabaseModel> GetTestQuery3()
        {
            string sql = "SELECT 'Test' [Name], 18 [Age] UNION SELECT 'Test2' [Name], 19 [Age]";
            return QueryMultiple<TestDatabaseModel>(_ConnectionString, sql);
        }

        public string? GetTestQuery4()
        {
            string sql = "SELECT NULL";
            return QueryNullableScalar<string>(_ConnectionString, sql);
        }

        public string? GetTestQuery5()
        {
            string sql = "SELECT TOP(1) [Y] FROM (SELECT 1 [Y]) X WHERE [Y] <> 1";
            return QueryNullableScalar<string>(_ConnectionString, sql);
        }
    }

    public interface ITestDatabaseHelper
    {
        /// <summary>
        /// Test Query 1.
        /// </summary>
        /// <returns>Test Model 1.</returns>
        public TestDatabaseModel GetTestQuery();
        public string GetTestQuery2();
        public List<TestDatabaseModel> GetTestQuery3();
        string? GetTestQuery4();
        string? GetTestQuery5();
    }
}
