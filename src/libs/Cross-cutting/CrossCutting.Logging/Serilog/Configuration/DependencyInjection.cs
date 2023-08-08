using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Shared.Settings;
using System.Data;
using System.Text;

namespace CrossCutting.Logging.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddMultiSinkLogging(this IServiceCollection services)
        => services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var loggingSettings = configuration.GetSection(LoggingSettings.SectionName).Get<LoggingSettings>()!;
            if (!loggingSettings.Enabled)
                return;

            var dataSourceSettings = configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!;

            loggerConfiguration.Enrich.FromLogContext();

            if (loggingSettings.ConsoleSink.Enabled)
                loggerConfiguration.WriteTo.Console(ParseLogEvent(loggingSettings.ConsoleSink.LoggingLevel));

            if (loggingSettings.FileSink.Enabled)
                loggerConfiguration.WriteTo.File(
                    loggingSettings.FileSink.LogFilePath,
                    restrictedToMinimumLevel: ParseLogEvent(loggingSettings.FileSink.LoggingLevel),
                    encoding: Encoding.Latin1);

            if (loggingSettings.DatabaseSink.Enabled)
                loggerConfiguration.WriteTo.MSSqlServer(
                    dataSourceSettings.ConnectionString,
                    restrictedToMinimumLevel: ParseLogEvent(loggingSettings.DatabaseSink.LoggingLevel),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        AutoCreateSqlTable = loggingSettings.DatabaseSink.AutoCreateTable,
                        TableName = loggingSettings.DatabaseSink.LogTableName,
                        SchemaName = loggingSettings.DatabaseSink.LogSchemaName
                    },
                    columnOptions: new ColumnOptions
                    {
                        AdditionalColumns = new List<SqlColumn>
                        {
                            new SqlColumn { AllowNull = true, DataType = SqlDbType.UniqueIdentifier, ColumnName = "CorrelationId" },
                            new SqlColumn { AllowNull = true, DataType = SqlDbType.UniqueIdentifier, ColumnName = "UserId" },
                            new SqlColumn { AllowNull = true, DataType = SqlDbType.VarChar, ColumnName = "Payload", DataLength = -1 },
                        }
                    });
        });

    private static LogEventLevel ParseLogEvent(string logLevel)
    {
        if (!Enum.TryParse<LogEventLevel>(logLevel, out var level))
            return LogEventLevel.Warning;

        return level;
    }
}