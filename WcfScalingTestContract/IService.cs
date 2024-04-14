using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfScalingTestContract
{
    [ServiceContract(Name = nameof(IService), Namespace = "WcfScalingTestContract")]
    public interface IService
    {
        [OperationContract]
        string TestOperation(string input);
    }

    [ServiceContract(Name = nameof(IService), Namespace = "WcfScalingTestContract")]
    public interface IServiceAsync
    {
        [OperationContract]
        Task<string> TestOperationAsync(string input);
    }
}