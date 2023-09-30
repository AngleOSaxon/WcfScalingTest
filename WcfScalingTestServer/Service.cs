using WcfScalingTestContract;

namespace WcfScalingTestServer;

public class Service : IService
{
    public string TestOperation(string input)
    {
        Thread.Sleep(5000);
        return $"echoed_{input}";
    }
}