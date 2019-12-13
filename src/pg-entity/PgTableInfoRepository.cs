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

        /// <summary>
        /// PostgreSQL - C# Type Mapping.
        /// <see cref="https://www.npgsql.org/doc/types/basic.html" />
        /// </summary>
        /// <typeparam name="string">PostgreSQL type</typeparam>
        /// <typeparam name="string">C# type</typeparam>
        public IDictionary<string, string> TypeMapping => new Dictionary<string, string>(){
            { "boolean", "bool" },
            { "smallint", "short" },
            { "integer", "int" },
            { "bigint", "long" },
            { "real", "float" },
            { "double precision", "double" },
            { "numeric","decimal" },
            { "money","decimal" },
            { "text","string" },
            { "character varying","string" },
            { "character","string" },
            { "citext","string" },
            { "json","string" },
            { "jsonb","string" },
            { "xml","string" },
            { "point", "NpgsqlPoint" },
            { "lseg", "NpgsqlLSeg" },
            { "path", "NpgsqlPath" },
            { "polygon", "NpgsqlPolygon" },
            { "line", "NpgsqlLine" },
            { "circle", "NpgsqlCircle" },
            { "box", "NpgsqlBox" },
            { "bit", "BitArray" },
            { "bit varying", "BitArray" },
            { "hstore", "Dictionary<string, string>" },
            { "uuid", "Guid" },
            { "cidr", "(IPAddress, int)" },
            { "inet", "IPAddress" },
            { "macaddr", "PhysicalAddress" },
            { "tsquery", "NpgsqlTsQuery" },
            { "tsvector", "NpgsqlTsVector" },
            { "date", "DateTime" },
            { "interval", "TimeSpan" },
            { "timestamp", "DateTime" },
            { "timestamp without time zone", "DateTime" },
            { "timestamp with time zone", "DateTimeOffset" },
            { "time", "TimeSpan" },
            { "time without time zone", "DateTimeOffset" },
            { "time with time zone", "DateTimeOffset" },
            { "bytea", "byte[]" },
            { "oid", "uint" },
            { "xid", "uint" },
            { "cid", "uint" },
            { "oidvector", "uint[]" },
            { "name", "string" },
            { "char", "char" },
            { "geometry", "PostgisGeometry" },
            { "record", "object[]" }
        };

        public async Task<IEnumerable<TableInfo>> ListAllAsync()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            #region SQL
            const string tableSql =
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
            + "  AND tbl.table_schema = COALESCE(:schema, tbl.table_schema)";

            const string columnsSql =
            "SELECT\n"
            + "  col.ordinal_position,\n"
            + "  col.table_name,\n"
            + "  col.column_name,\n"
            + "  col.data_type,\n"
            + "  (pk.column_name IS NOT NULL) AS is_primary,\n"
            + "  (col.is_nullable = 'NO') AS not_null,\n"
            + "  pgd.description\n"
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
            + "  col.table_schema NOT IN ('pg_catalog', 'information_schema')\n"
            + "  AND col.table_schema = COALESCE(:schema, col.table_schema)\n"
            + "ORDER BY\n"
            + "  col.ordinal_position";
            #endregion

            var tableData = await conn.QueryAsync(tableSql, new { schema = _schema });
            var columnsData = await conn.QueryAsync(columnsSql, new { schema = _schema });
            return tableData
                .Select(
                    t => new TableInfo(
                        t.name,
                        columnsData.Where(c => c.table_name == t.name)
                            .OrderBy(c => c.ordinal_position)
                            .Select(c =>
                                new ColumnInfo(
                                    c.column_name,
                                    c.data_type,
                                    c.is_primary,
                                    c.not_null,
                                    c.description
                                )
                            ),
                        t.description
                    )
                );
        }

    }
}