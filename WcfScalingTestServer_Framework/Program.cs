using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WcfScalingTestContract;

namespace WcfScalingTestServer_Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var streamingUri = new Uri("net.tcp://0.0.0.0:3001/streaming_service");
            var bufferedUri = new Uri("net.tcp://0.0.0.0:3001/buffered_service");
            using (ServiceHost host = new ServiceHost(typeof(Service), streamingUri))
            using (X509Store store = new X509Store(StoreLocation.CurrentUser))
            {
                var streamingBinding = new NetTcpBinding(SecurityMode.Transport);
                streamingBinding.TransferMode = TransferMode.Streamed;
                streamingBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                host.AddServiceEndpoint(typeof(IService), streamingBinding, streamingUri, streamingUri);


                var bufferedBinding = new NetTcpBinding(SecurityMode.Transport);
                bufferedBinding.TransferMode = TransferMode.Buffered;
                bufferedBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                host.AddServiceEndpoint(typeof(IService), bufferedBinding, bufferedUri, bufferedUri);

                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, "fda34f7ce6e5470025a145e4f6bd5bf6a495eaed", validOnly: false);
                host.Credentials.ServiceCertificate.Certificate = certs[0];
                //ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                //smb.HttpGetEnabled = true;
                //smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                //host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                Console.WriteLine("The streaming service is ready at {0}", streamingUri);
                Console.WriteLine("The buffered service is ready at {0}", bufferedUri);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}
