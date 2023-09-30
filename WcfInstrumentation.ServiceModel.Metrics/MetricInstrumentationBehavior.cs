using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WcfInstrumentation.ServiceModel.Metrics;

public class MetricInstrumentationBehavior : IContractBehavior
{
    public MetricInstrumentationBehavior(ChannelMetrics channelMetrics)
    {
        ChannelMetrics = channelMetrics;
    }

    public ChannelMetrics ChannelMetrics { get; }

    public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        var typeName = clientRuntime.ContractClientType.Name;
        clientRuntime.ChannelInitializers.Add(new ChannelMetricsInitializer(channelMetrics: ChannelMetrics, typeName: typeName));
    }

    public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
    {
    }

    public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
    {
    }
}