using System.ServiceModel;
using WcfScalingTestContract;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.Logging;
using WcfApplicationInsights;
using WcfInstrumentation.ServiceModel.Metrics;
using System.Collections.Concurrent;
using OpenTelemetrySdk = OpenTelemetry.Sdk;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry;
using System.Text;

IServiceCollection services = new ServiceCollection();

const string aiConnectionString = "InstrumentationKey=defe10ee-50d7-4c13-8896-9877ffcbf995;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/";
services.AddLogging(loggingBuilder => loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", LogLevel.Information));
services.AddApplicationInsightsTelemetryWorkerService((ApplicationInsightsServiceOptions options) => options.ConnectionString = aiConnectionString);
OpenTelemetrySdk.CreateMeterProviderBuilder()
    .AddMeter(ChannelMetrics.MeterName)
    .AddAzureMonitorMetricExporter(o => o.ConnectionString = aiConnectionString)
    .Build();

services.AddSingleton(new ChannelMetrics());
services.AddSingleton<MetricInstrumentationBehavior>();

IServiceProvider serviceProvider = services.BuildServiceProvider();

// Obtain logger instance from DI.
ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// Obtain TelemetryClient instance from DI, for additional manual tracking or to flush.
var telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();

var netTcpBinding = new NetTcpBinding()
{
    Security = new NetTcpSecurity
    {
        Transport = new TcpTransportSecurity
        {
            ClientCredentialType = TcpClientCredentialType.None
        },
        Mode = SecurityMode.Transport
    }
};
var address = new EndpointAddress("net.tcp://localhost:3001/service");
var channelFactory = new ChannelFactory<IService>(netTcpBinding, address);
channelFactory.Endpoint.Contract.ContractBehaviors.Add(serviceProvider.GetRequiredService<MetricInstrumentationBehavior>());
channelFactory.Endpoint.EndpointBehaviors.Add(new ClientTelemetryEndpointBehavior(telemetryClient));

const string fileNamePrefix = "output";
const string fileNameExtension = "txt";
var fileName = $"{fileNamePrefix}.{fileNameExtension}";
if (File.Exists(fileName))
{
    var currentDir = Directory.GetCurrentDirectory();
    var lastNumber = Directory.GetFiles(currentDir, $"{fileNamePrefix}.*.{fileNameExtension}").Select(item => int.Parse(item.Split(".")[1])).OrderByDescending(item => item).First();
    fileName = $"{fileNamePrefix}.{lastNumber + 1}.{fileNameExtension}";
}
var taskList = new Task[100];
using var fileStream = new FileStream(fileName, FileMode.Create);
var timer = new System.Diagnostics.Stopwatch();
timer.Start();
for (var i = 0; i < 100; i++)
{
    var channel = channelFactory.CreateChannel();
    
    int localCopy = i;
    taskList[i] = Task.Run(async () =>
    {
        for (var j = 0; j < 3; j++)
        {
            var result = channel.TestOperation($"test{localCopy}_{j}");
            var log = $"{timer.ElapsedMilliseconds}{result}{Environment.NewLine}";
            await fileStream.WriteAsync(Encoding.UTF8.GetBytes(log));
        }
    });
}
await Task.WhenAll(taskList);


channelFactory.Close();
fileStream.Flush();
telemetryClient.Flush();
Task.Delay(30000).Wait();