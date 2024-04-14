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
using System.Diagnostics;
using WcfScalingTestClient;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

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

ThreadPool.SetMinThreads(100, 100);

IServiceProvider serviceProvider = services.BuildServiceProvider();

// Obtain logger instance from DI.
ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// Obtain TelemetryClient instance from DI, for additional manual tracking or to flush.
var telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();

var clientTest = new ClientTest(logger);

await clientTest.Test(streaming: true, contractBehaviors: new IContractBehavior[] { serviceProvider.GetRequiredService<MetricInstrumentationBehavior>() }, endpointBehaviors: new IEndpointBehavior[] { new ClientTelemetryEndpointBehavior(telemetryClient) });

telemetryClient.Flush();
//Task.Delay(30000).Wait();