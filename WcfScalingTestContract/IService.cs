using System.ServiceModel;

namespace WcfScalingTestContract;

[ServiceContract(Name = nameof(IService), Namespace = "WcfScalingTestContract")]
public interface IService
{
    [OperationContract]
    string TestOperation(string input);
}