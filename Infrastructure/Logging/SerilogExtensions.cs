using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Infrastructure.Logging;

public static class SerilogExtensions
{
    public static IHostBuilder UseSerilogLogging(
        this IHostBuilder host,
        IConfiguration configuration)
    {
        EnsureLogDirectories();

        host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteToMSSqlServer(configuration);
        });

        return host;
    }

    private static void WriteToMSSqlServer(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LogsDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        loggerConfiguration.WriteTo.MSSqlServer(
            connectionString,
            new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = true,
                BatchPostingLimit = 50
            },
            columnOptions: SerilogSqlColumns.Create()
        );
    }

    private static void EnsureLogDirectories()
    {
        Directory.CreateDirectory("Logs");
        Directory.CreateDirectory("Errors");
    }
}
