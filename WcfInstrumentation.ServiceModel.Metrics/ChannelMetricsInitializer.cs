using System.Diagnostics.Metrics;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace WcfInstrumentation.ServiceModel.Metrics;

public class ChannelMetricsInitializer : IChannelInitializer
{
    public ChannelMetricsInitializer(ChannelMetrics channelMetrics, string typeName)
    {
        ChannelMetrics = channelMetrics;
        TypeName = typeName;
    }

    public ChannelMetrics ChannelMetrics { get; }
    public string TypeName { get; }

    public void Initialize(IClientChannel channel)
    {
        channel.Opened += (sender, e)  =>
        {
            ChannelMetrics.OpenChannel(TypeName);
        };
        channel.Closed += (sender, e) =>
        {
            ChannelMetrics.CloseChannel(TypeName);
        };
        channel.Faulted += (sender, e) =>
        {
            ChannelMetrics.CloseChannel(TypeName);
        };
    }
}
