using System;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Serilog.Events;

namespace Serilog.Sinks.AwsDynamoDb
{
    /// <summary>
    /// Represents a log document in DynamoDB.
    /// </summary>
    public class LogDocument
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LogDocument"/> class.
        /// </summary>
        public LogDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogDocument"/> class.
        /// </summary>
        /// <param name="logEvent">Log.</param>
        /// <param name="renderedMessage">Log message.</param>
        public LogDocument(LogEvent logEvent, string renderedMessage)
        {
            Id = Guid.NewGuid();
            Timestamp = logEvent.Timestamp.UtcDateTime;
            MessageTemplate = logEvent.MessageTemplate.Text;
            Level = logEvent.Level.ToString();
            Message = renderedMessage;
            Exception = logEvent.Exception?.ToString() ?? string.Empty;

            if (logEvent.Properties.Any())
            {
                Properties = logEvent.Properties
                    .Select(x => $"{x.Key}:{x.Value}")
                    .Aggregate((working, next) => $"{working} {next}");
            }
        }

        /// <summary>
        /// Gets or sets the DynamoDB key.
        /// </summary>
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        /// <summary>
        /// The time at which the event occurred.
        /// </summary>

        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the message template.
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the log exception.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the message that will be logged.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the log properties.
        /// </summary>
        public string Properties { get; set; }
    }
}