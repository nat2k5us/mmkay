using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.SqlClient;

    public static class DBExtensions
    {
        //private static bool CreateDatabase(string databaseName)
        //{
        //    Console.Write("creating database '" + databaseName + "'...");
        //    using (var connection = CreateConnection(CreateConnectionString(databaseName)))
        //    {
        //        using (var context = new DbContext(connection, false))
        //        {
        //            var a = context.As.ToList(); // CompatibleWithModel must not be the first call
        //            var result = context.Database.CompatibleWithModel(false);
        //            Console.WriteLine(result ? "DONE" : "FAIL");
        //            return result;
        //        }
        //    }
        //}


        private static bool DeleteDatabase(string databaseName)
        {
            using (var connection = CreateConnection(CreateConnectionString(databaseName)))
            {
                if (System.Data.Entity.Database.Exists(connection))
                {
                    Console.Write("deleting database '" + databaseName + "'...");
                    var result = System.Data.Entity.Database.Delete(connection);
                    Console.WriteLine(result ? "DONE" : "FAIL");
                    return result;
                }
                return true;
            }
        }

        private static DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        private static string CreateConnectionString(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "server",
                InitialCatalog = databaseName,
                IntegratedSecurity = false,
                MultipleActiveResultSets = false,
                PersistSecurityInfo = true,
                UserID = "username",
                Password = "password"
            };
            return builder.ConnectionString;
        }
    }
}
