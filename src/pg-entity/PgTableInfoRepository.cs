using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using TableEntityGenerator.Core.Models;
using TableEntityGenerator.Core.Repositories;

namespace TableEntityGenerator.Npgsql
{
    public class PgTableInfoRepository : ITableInfoRepository
    {
        private readonly string? _schema;
        private readonly string _connectionString;

        public PgTableInfoRepository(string connectionString, string? schema = null)
            => (_connectionString, _schema) = (connectionString, schema);
        public IDictionary<string, string> TypeMapping => throw new System.NotImplementedException();

        public async ValueTask<IEnumerable<TableInfo>> ListAllAsync()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            #region SQL
            const string sql =
            "SELECT\n"
            + "  tbl.table_schema AS schema,\n"
            + "  tbl.table_name AS name,\n"
            + "  pgd.description\n"
            + "FROM\n"
            + "  information_schema.tables tbl\n"
            + "  LEFT JOIN pg_stat_user_tables psut\n"
            + "    ON psut.relname = tbl.table_name\n"
            + "  LEFT JOIN pg_description pgd\n"
            + "    ON pgd.objoid = psut.relid\n"
            + "    AND pgd.objsubid = 0\n"
            + "WHERE\n"
            + "  tbl.table_schema NOT IN ('pg_catalog', 'information_schema')\n"
            + "  AND (tbl.table_schema = :schema OR :schema IS NULL)";
            #endregion

            var tableData = await conn.QueryAsync(sql, new { schema = _schema });
            foreach (var table in tableData)
            {
                yield return new TableInfo(table.name, await ListColumns(table.schema, table.name), table.description);
            }

            async ValueTask<IEnumerable<ColumnInfo>> ListColumns(string schema, string tableName)
            {
                #region SQL
                const string sql =
                "SELECT\n"
                + "  col.column_name AS \"" + nameof(ColumnInfo.Name) + "\",\n"
                + "  col.data_type AS \"" + nameof(ColumnInfo.Type) + "\",\n"
                + "  (pk.column_name IS NOT NULL) AS \"" + nameof(ColumnInfo.IsPrimary) + "\",\n"
                + "  (col.is_nullable = 'NO') AS \"" + nameof(ColumnInfo.NotNull) + "\",\n"
                + "  pgd.description AS \"" + nameof(ColumnInfo.Description) + "\"\n"
                + "FROM\n"
                + "  information_schema.columns col\n"
                + "  LEFT JOIN information_schema.table_constraints tbcon\n"
                + "    ON col.table_catalog = tbcon.table_catalog\n"
                + "    AND col.table_schema = tbcon.table_schema\n"
                + "    AND col.table_name = tbcon.table_name\n"
                + "    AND tbcon.constraint_type = 'PRIMARY KEY'\n"
                + "  LEFT JOIN information_schema.constraint_column_usage pk\n"
                + "    ON tbcon.constraint_catalog = pk.constraint_catalog\n"
                + "    AND tbcon.constraint_schema = pk.constraint_schema\n"
                + "    AND tbcon.constraint_name = pk.constraint_name\n"
                + "    AND col.table_catalog = pk.table_catalog\n"
                + "    AND col.table_schema = pk.table_schema\n"
                + "    AND col.table_name = pk.table_name\n"
                + "    AND col.column_name = pk.column_name\n"
                + "  LEFT JOIN pg_stat_user_tables psut\n"
                + "    ON psut.relname = col.table_name\n"
                + "  LEFT JOIN pg_description pgd\n"
                + "    ON pgd.objoid = psut.relid\n"
                + "    AND pgd.objsubid = col.ordinal_position\n"
                + "WHERE\n"
                + "  col.table_schema = :" + nameof(schema) + "\n"
                + "  AND col.table_name = :" + nameof(tableName) + "\n"
                + "ORDER BY\n"
                + "  col.ordinal_position";
                #endregion
                return await conn.QueryAsync<ColumnInfo>(sql, new { schema, tableName });
            }
        }

    }
}