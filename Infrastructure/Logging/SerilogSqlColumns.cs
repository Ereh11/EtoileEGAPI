using Serilog.Sinks.MSSqlServer;
using System.Data;

namespace Infrastructure.Logging;

public static class SerilogSqlColumns
{
    public static ColumnOptions Create()
    {
        var options = new ColumnOptions
        {
            TimeStamp =
            {
                ConvertToUtc = false
            }
        };

        options.Store.Remove(StandardColumn.Message);
        options.Store.Remove(StandardColumn.MessageTemplate);
        options.Store.Remove(StandardColumn.Level);
        options.Store.Remove(StandardColumn.Properties);

        options.AdditionalColumns =
        [
            CreateColumn("Project", SqlDbType.NVarChar, 200, allowNull: false),
            CreateColumn("Method", SqlDbType.NVarChar, 50),
            CreateColumn("RequestPath", SqlDbType.NVarChar, 255),
            CreateColumn("RequestBody", SqlDbType.NVarChar),
            CreateColumn("ResponseBody", SqlDbType.NVarChar),
            CreateColumn("ResponseStatusCode", SqlDbType.NVarChar, 3),
            CreateColumn("Status", SqlDbType.NVarChar, 50),
            CreateColumn("RequestId", SqlDbType.NVarChar, 100)
        ];

        return options;
    }
    private static SqlColumn CreateColumn(
        string name,
        SqlDbType type,
        int? length = null,
        bool allowNull = true)
    {
        return new SqlColumn
        {
            ColumnName = name,
            DataType = type,
            DataLength = length ?? 10,
            AllowNull = allowNull
        };
    }
}
