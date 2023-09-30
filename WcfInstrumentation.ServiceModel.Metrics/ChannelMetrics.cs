using System.Diagnostics.Metrics;
using System.Reflection.Metadata;

namespace WcfInstrumentation.ServiceModel.Metrics;

public class ChannelMetrics
{
    public const string MeterName = "wcf_instrumentation.metrics.channel";

    public ChannelMetrics()
    {
        Meter = new Meter(MeterName);
        OpenChannelCount = Meter.CreateUpDownCounter<int>("channel.open_channel_count");
        OpenedConnections = Meter.CreateCounter<int>("channel.opened_channel_count");
        ClosedConnections = Meter.CreateCounter<int>("channel.closed_channel_count");
    }

    private Meter Meter { get; }

    private UpDownCounter<int> OpenChannelCount { get; }

    private Counter<int> OpenedConnections { get; }

    private Counter<int> ClosedConnections { get; }

    public void OpenChannel(string channelName)
    {
        var metadata = new KeyValuePair<string, object?>("channel_type", channelName);
        OpenChannelCount.Add(1, metadata);
        OpenedConnections.Add(1, metadata);
    }

    public void CloseChannel(string channelName)
    {
        var metadata = new KeyValuePair<string, object?>("channel_type", channelName);
        OpenChannelCount.Add(-1, metadata);
        ClosedConnections.Add(1, metadata);
    }
}