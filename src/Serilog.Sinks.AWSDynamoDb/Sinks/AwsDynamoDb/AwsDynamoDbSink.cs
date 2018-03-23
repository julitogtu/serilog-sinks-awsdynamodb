using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.AwsDynamoDb
{
    /// <summary>
    /// Sink that writes log events to a table in AWS DynamoDB.
    /// </summary>
    public class AwsDynamoDbSink : PeriodicBatchingSink
    {
        /// <summary>
        /// Default time to batch information.
        /// </summary>
        public static readonly TimeSpan DefaultBatchPeriod = TimeSpan.FromSeconds(5);

        private readonly IFormatProvider formatProvider;
        private readonly string tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AwsDynamoDbSink"/> class.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <param name="region">AWS DynamoDB region.</param>
        /// <param name="accessKey">AWS access key.</param>
        /// <param name="secretKey">AWS secret key.</param>
        /// <param name="tableName">Name of the table to store the events.</param>
        /// <param name="batchPeriod">Time to wait between checking for event batches.</param>
        /// <param name="autoCreateTable">Indicates if the table must be created or not.</param>
        public AwsDynamoDbSink(IFormatProvider formatProvider, string region, string accessKey, string secretKey, string tableName, TimeSpan batchPeriod, bool autoCreateTable = false)
            : base(1000, batchPeriod)
        {
            this.formatProvider = formatProvider;
            this.tableName = tableName;

            BasicAwsCredentials = new BasicAWSCredentials(
                accessKey: accessKey,
                secretKey: secretKey);

            AmazonDynamoDbConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(region)
            };

            DynamoDbOperationConfig = new DynamoDBOperationConfig
            {
                OverrideTableName = tableName
            };

            if (autoCreateTable) CreateDynamoDbLogTable();
        }

        private void CreateDynamoDbLogTable()
        {
            var request = new CreateTableRequest(
                tableName: tableName,
                keySchema: new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = KeyType.HASH
                    }
                },
                attributeDefinitions: new List<AttributeDefinition>
                {
                    new AttributeDefinition()
                    {
                        AttributeName = "Id",
                        AttributeType = ScalarAttributeType.S
                    }
                },
                provisionedThroughput: new ProvisionedThroughput
                {
                    ReadCapacityUnits = 1,
                    WriteCapacityUnits = 1
                }
            );

            try
            {
                using (var client = new AmazonDynamoDBClient(BasicAwsCredentials, AmazonDynamoDbConfig))
                {
                    var result = client.CreateTableAsync(request).Result;
                }
            }
            catch (ResourceInUseException)
            {
                SelfLog.WriteLine("Table already exists.");
            }
        }

        private DynamoDBOperationConfig DynamoDbOperationConfig { get; }

        private AmazonDynamoDBConfig AmazonDynamoDbConfig { get; }

        private BasicAWSCredentials BasicAwsCredentials { get;  }

        /// <summary>
        /// Sends logs to AWS DynamoDB.
        /// </summary>
        /// <param name="events">Events</param>
        /// <returns>Task</returns>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var records = events.Select(x => new LogDocument(x, x.RenderMessage(formatProvider)));

            try
            {
                using (var client = new AmazonDynamoDBClient(BasicAwsCredentials, AmazonDynamoDbConfig))
                {
                    using (var context = new DynamoDBContext(client))
                    {
                        var batchWrite = context.CreateBatchWrite<LogDocument>(DynamoDbOperationConfig);
                        batchWrite.AddPutItems(records);
                        await batchWrite.ExecuteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Cannot write events to AWS DynamoDB table {0}. Exception {1}", tableName, ex);
            }
        }
    }
}