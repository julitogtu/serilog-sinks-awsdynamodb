# Serilog.Sinks.AWSDynamoDB

A Serilog sink for .Net Core that writes events to an AWS DynamoDB table. (This is inspired by the original sink [serilog-sinks-dynamodb
](https://github.com/serilog/serilog-sinks-dynamodb))

## Getting started

Install the [Serilog.Sinks.AWSDynamoDB](https://www.nuget.org/packages/Serilog.Sinks.AWSDynamoDb) package from Nuget

```powershell
Install-Package Serilog.Sinks.AWSDynamoDB
```

To configure the sink, add a call to the `AwsDynamoDb` extension method:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.AwsDynamoDb("{AWS-SERVICE-REGION}", "{AWS-ACCESS-KEY}", "{AWS-SECRET KEY}", "{TABLE-NAME}", autoCreateTable: true)
    .CreateLogger();
```

The extension method receives the following parameters:

- AWS-SERVICE-REGION: Region in which is located the DynamoDB
- AWS-ACCESS-KEY: Access key to the AWS services.
- AWS-SECRET KEY: Access secret key to the AWS services.
- TABLE-NAME: Name of the table that will store the logs.
- autoCreateTable (optional): When true, a new table is created.

> Important: I'm working to enhance the sink, if you find some issues or new features that you want to see, please, create a new issue for that or twitt me [@julitogtu](https://twitter.com/julitogtu)

_Copyright &copy; 2018 Julio Avellaneda - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._