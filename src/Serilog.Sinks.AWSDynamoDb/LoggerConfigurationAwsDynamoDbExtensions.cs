using System;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.AwsDynamoDb;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.AwsDynamoDb() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationAwsDynamoDbExtensions
    {
        /// <summary>
        /// Adds a sink that writes events to a table in AWS DynamoDB
        /// </summary>
        /// <param name="loggerConfiguration">Logger configuration.</param>
        /// <param name="region">AWS DynamoDB region.</param>
        /// <param name="accessKey">AWS access key.</param>
        /// <param name="secretKey">AWS secret key.</param>
        /// <param name="tableName">Name of the table to store the events.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="period">Time to wait between checking for event batches.</param>
        /// <param name="autoCreateTable">Indicates if the table must be created or not.</param>
        /// <returns></returns>
        public static LoggerConfiguration AwsDynamoDb(
            this LoggerSinkConfiguration loggerConfiguration,
            string region,
            string accessKey,
            string secretKey,
            string tableName,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            TimeSpan? period = null,
            bool autoCreateTable = false)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));

            if (string.IsNullOrWhiteSpace(region)) throw new ArgumentNullException(nameof(region));

            if (string.IsNullOrWhiteSpace(accessKey)) throw new ArgumentNullException(nameof(accessKey));

            if (string.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException(nameof(secretKey));

            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            var batchPeriod = period ?? AwsDynamoDbSink.DefaultBatchPeriod;
            var logEventSink = new AwsDynamoDbSink(formatProvider, region, accessKey, secretKey, tableName, batchPeriod, autoCreateTable);

            return loggerConfiguration.Sink(logEventSink, restrictedToMinimumLevel);
        }
    }
}