using System.Security.Cryptography.X509Certificates;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WcfScalingTestContract;
using WcfScalingTestServer;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseNetTcp(3001)
    .ConfigureKestrel((context, options) =>
    {
        options.AllowSynchronousIO = true;
    });

builder.Services.AddServiceModelServices().AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

var netTcpBinding = new NetTcpBinding(SecurityMode.Transport);
netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

app.UseServiceModel(builder =>
{
    builder.AddService<Service>(opt =>
    {
        opt.DebugBehavior.IncludeExceptionDetailInFaults = true;
    })
    .AddServiceEndpoint<Service, IService>(netTcpBinding, "net.tcp://0.0.0.0:3001/service", opt =>
    {
        opt.Address = new EndpointAddress("net.tcp://0.0.0.0:3001/service");
    });
    builder.ConfigureServiceHostBase<Service>(host =>
    {
        using var store = new X509Store(StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        var certs = store.Certificates.Find(X509FindType.FindByThumbprint, "fda34f7ce6e5470025a145e4f6bd5bf6a495eaed", validOnly: false);
        host.Credentials.ServiceCertificate.Certificate = certs[0];
    });
});

var metadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
metadataBehavior.HttpsGetEnabled = true;

app.Run();
