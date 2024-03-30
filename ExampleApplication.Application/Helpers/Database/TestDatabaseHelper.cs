using ExampleApplication.Application.Models.Database;
using System.Collections.Generic;

namespace ExampleApplication.Application.Helpers.Database
{
    public class TestDatabaseHelper : BaseDatabaseHelper
    {
        private static readonly string _ConnectionString = "data source=DESKTOP-FD5NGCT;initial catalog=Finances;trusted_connection=true; TrustServerCertificate=True;";

        public static TestDatabaseModel GetTestQuery()
        {
            string sql = "SELECT 'Test' [Name], 18 [Age]";
            return QuerySingle<TestDatabaseModel>(_ConnectionString, sql);
        }

        public static string GetTestQuery2()
        {
            string sql = "SELECT 'Test2'";
            return QueryScalar<string>(_ConnectionString, sql);
        }

        public static List<TestDatabaseModel> GetTestQuery3()
        {
            string sql = "SELECT 'Test' [Name], 18 [Age] UNION SELECT 'Test2' [Name], 19 [Age]";
            return QueryMultiple<TestDatabaseModel>(_ConnectionString, sql);
        }
    }
}
