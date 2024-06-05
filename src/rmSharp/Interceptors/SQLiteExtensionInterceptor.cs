using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace rmSharp
{
    public class SQLiteExtensionInterceptor : IDbConnectionInterceptor
    {
        public DbConnection ConnectionCreated(ConnectionCreatedEventData eventData, DbConnection result)
        {
            var sqliteConnection = (SqliteConnection)result;
            sqliteConnection.EnableExtensions();
            sqliteConnection.LoadExtension(@"unifuzz64.dll");
            return sqliteConnection;
        }
    }
}

