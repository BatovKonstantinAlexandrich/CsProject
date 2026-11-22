using System.Data.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Console_RoyaltyData.RoyaltyData
{
    /// <summary>
    /// Дополняет уже созданный файл SQLite столбцами и таблицами расчёта.
    /// Для SQL Server схему не меняет.
    /// </summary>
    internal sealed class RoyaltySchemaInterceptor : DbConnectionInterceptor
    {
        internal static readonly RoyaltySchemaInterceptor Instance = new();
        private static int _ready;

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            EnsureSqlite(connection);
            base.ConnectionOpened(connection, eventData);
        }

        public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            EnsureSqlite(connection);
            return base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }

        private static void EnsureSqlite(DbConnection connection)
        {
            if (!string.Equals(connection.GetType().Name, "SqliteConnection", StringComparison.Ordinal))
                return;
            if (Volatile.Read(ref _ready) == 1)
                return;

            lock (Instance)
            {
                if (_ready == 1)
                    return;

                if (HasTable(connection, "Departments"))
                {
                    AddColumn(connection, "Departments", "DivisionId", "INTEGER NOT NULL DEFAULT 0");
                    AddColumn(connection, "Departments", "OwnerId", "INTEGER NOT NULL DEFAULT 0");
                    AddColumn(connection, "Departments", "DivisionType", "TEXT NOT NULL DEFAULT ''");
                    AddColumn(connection, "Departments", "IsEa", "INTEGER NOT NULL DEFAULT 0");
                }

                Execute(connection, """
                    CREATE TABLE IF NOT EXISTS "RoyaltyCalculations" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_RoyaltyCalculations" PRIMARY KEY AUTOINCREMENT,
                        "DepartmentId" INTEGER NOT NULL,
                        "ReportYear" INTEGER NOT NULL,
                        "ReportMonth" INTEGER NOT NULL,
                        "KoefRoyalty" TEXT NOT NULL,
                        "KoefCalc" TEXT NOT NULL,
                        "NdsSt" TEXT NOT NULL,
                        "OwnerId" INTEGER NOT NULL,
                        "DiscountForBuy" TEXT NOT NULL,
                        "DiscountLoyalty" TEXT NOT NULL,
                        CONSTRAINT "FK_RoyaltyCalculations_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE CASCADE
                    );
                    """);
                Execute(connection, """CREATE INDEX IF NOT EXISTS "IX_RoyaltyCalculations_DepartmentId" ON "RoyaltyCalculations" ("DepartmentId");""");
                Execute(connection, """
                    CREATE UNIQUE INDEX IF NOT EXISTS "IX_RoyaltyCalculations_DepartmentId_ReportYear_ReportMonth"
                    ON "RoyaltyCalculations" ("DepartmentId", "ReportYear", "ReportMonth");
                    """);
                Execute(connection, """
                    CREATE TABLE IF NOT EXISTS "RoyaltyMonthData" (
                        "Id" INTEGER NOT NULL CONSTRAINT "PK_RoyaltyMonthData" PRIMARY KEY AUTOINCREMENT,
                        "DepartmentId" INTEGER NOT NULL,
                        "ReportYear" INTEGER NOT NULL,
                        "ReportMonth" INTEGER NOT NULL,
                        "OwnerId" INTEGER NOT NULL,
                        "KoefRoyalty" TEXT NOT NULL,
                        "KoefCalc" TEXT NOT NULL,
                        "NdsSt" TEXT NOT NULL,
                        "SumProceeds" TEXT NOT NULL,
                        "SumPurEa" TEXT NOT NULL,
                        "SumPurOther" TEXT NOT NULL,
                        "SumBuyEa" TEXT NOT NULL,
                        "SumBuyOther" TEXT NOT NULL,
                        "RatioPurEa" TEXT NOT NULL,
                        "RatioPretensions" TEXT NOT NULL,
                        "SumRoyalty" TEXT NOT NULL,
                        "SumRoyaltyNds" TEXT NOT NULL,
                        "SumSmb" TEXT NOT NULL,
                        "SumWorkBuh" TEXT NOT NULL,
                        "SumPartBuh" TEXT NOT NULL,
                        "CountRun" INTEGER NOT NULL,
                        "IndexLoyalty" TEXT NOT NULL,
                        "K2" TEXT NOT NULL,
                        "K3" TEXT NOT NULL,
                        "K4" TEXT NOT NULL,
                        "RoyaltyCalc" TEXT NOT NULL,
                        CONSTRAINT "FK_RoyaltyMonthData_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE CASCADE
                    );
                    """);
                Execute(connection, """CREATE INDEX IF NOT EXISTS "IX_RoyaltyMonthData_DepartmentId" ON "RoyaltyMonthData" ("DepartmentId");""");
                Execute(connection, """
                    CREATE UNIQUE INDEX IF NOT EXISTS "IX_RoyaltyMonthData_DepartmentId_ReportYear_ReportMonth"
                    ON "RoyaltyMonthData" ("DepartmentId", "ReportYear", "ReportMonth");
                    """);

                Volatile.Write(ref _ready, 1);
            }
        }

        private static bool HasTable(DbConnection connection, string table)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = $name";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "$name";
            parameter.Value = table;
            command.Parameters.Add(parameter);
            return command.ExecuteScalar() is not null;
        }

        private static bool HasColumn(DbConnection connection, string table, string column)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{table}\")";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), column, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static void AddColumn(DbConnection connection, string table, string column, string definition)
        {
            if (HasColumn(connection, table, column))
                return;
            Execute(connection, $"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {definition}");
        }

        private static void Execute(DbConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}
