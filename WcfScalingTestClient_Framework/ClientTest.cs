using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfScalingTestContract;

namespace WcfScalingTestClient_Framework
{
    public class ClientTest
    {
        public ClientTest()
        {
        }

        public async Task Test(bool streaming, IContractBehavior[] contractBehaviors, IEndpointBehavior[] endpointBehaviors)
        {
            var netTcpBinding = new NetTcpBinding()
            {
                TransferMode = streaming
                    ? TransferMode.Streamed
                    : TransferMode.Buffered,
                Security = new NetTcpSecurity
                {
                    Transport = new TcpTransportSecurity
                    {
                        ClientCredentialType = TcpClientCredentialType.None
                    },
                    Mode = SecurityMode.Transport
                }
            };
            var endpoint = streaming
                ? "net.tcp://localhost:3001/streaming_service"
                : "net.tcp://localhost:3001/buffered_service";
            var address = new EndpointAddress(endpoint);


            //const string fileNamePrefix = "output";
            //const string fileNameExtension = "txt";
            //var fileName = $"{fileNamePrefix}.{fileNameExtension}";
            //if (File.Exists(fileName))
            //{
            //    var currentDir = Directory.GetCurrentDirectory();
            //    var lastNumber = Directory.GetFiles(currentDir, $"{fileNamePrefix}.*.{fileNameExtension}")
            //        .Select(item => int.Parse(item.Split(".")[^2]))
            //        .OrderByDescending(item => item).FirstOrDefault();
            //    fileName = $"{fileNamePrefix}.{lastNumber + 1}.{fileNameExtension}";
            //}

            const int clientCount = 500;
            var taskList = new Task[clientCount];
            //using var fileStream = new FileStream(fileName, FileMode.Create);
            //var timer = new System.Diagnostics.Stopwatch();
            //timer.Start();

            var channelFactory = new ChannelFactory<IService>(netTcpBinding, address);
            foreach (var behavior in contractBehaviors)
            {
                channelFactory.Endpoint.Contract.ContractBehaviors.Add(behavior);
            }
            foreach (var behavior in endpointBehaviors)
            {
                channelFactory.Endpoint.EndpointBehaviors.Add(behavior);
            }

            for (var i = 0; i < clientCount; i++)
            {
                var channel = channelFactory.CreateChannel();

                int localCopy = i;
                taskList[i] = Task.Run(() =>
                {
                    for (var j = 0; j < 3; j++)
                    {
                        //try
                        //{
                            var result = channel.TestOperation($"test{localCopy}_{j}");
                            //var log = $"{timer.ElapsedMilliseconds}{result}{Environment.NewLine}";
                            //await fileStream.WriteAsync(Encoding.UTF8.GetBytes(log));
                        //}
                        //catch (Exception e)
                        //{
                        //    Logger?.LogError(e, e.Message);
                        //}
                    }
                });
            }
            await Task.WhenAll(taskList);

            // channelFactory.Close();
            //fileStream.Flush();
        }

        public async Task TestAsync(bool streaming, IContractBehavior[] contractBehaviors, IEndpointBehavior[] endpointBehaviors)
        {
            var netTcpBinding = new NetTcpBinding()
            {
                TransferMode = streaming
                    ? TransferMode.Streamed
                    : TransferMode.Buffered,
                Security = new NetTcpSecurity
                {
                    Transport = new TcpTransportSecurity
                    {
                        ClientCredentialType = TcpClientCredentialType.None
                    },
                    Mode = SecurityMode.Transport
                }
            };
            var endpoint = streaming
                ? "net.tcp://localhost:3001/streaming_service"
                : "net.tcp://localhost:3001/buffered_service";
            var address = new EndpointAddress(endpoint);

            const int clientCount = 500;
            var taskList = new Task[clientCount];

            var channelFactory = new ChannelFactory<IServiceAsync>(netTcpBinding, address);
            foreach (var behavior in contractBehaviors)
            {
                channelFactory.Endpoint.Contract.ContractBehaviors.Add(behavior);
            }
            foreach (var behavior in endpointBehaviors)
            {
                channelFactory.Endpoint.EndpointBehaviors.Add(behavior);
            }

            for (var i = 0; i < clientCount; i++)
            {
                var channel = channelFactory.CreateChannel();

                int localCopy = i;
                taskList[i] = Task.Run(() =>
                {
                    for (var j = 0; j < 3; j++)
                    {
                        //try
                        //{
                            var result = channel.TestOperationAsync($"test{localCopy}_{j}");
                            //var log = $"{timer.ElapsedMilliseconds}{result}{Environment.NewLine}";
                            //await fileStream.WriteAsync(Encoding.UTF8.GetBytes(log));
                        //}
                        //catch (Exception e)
                        //{
                        //    Logger?.LogError(e, e.Message);
                        //}
                    }
                });
            }
            await Task.WhenAll(taskList);

            // channelFactory.Close();
            //fileStream.Flush();
        }
    }
}
