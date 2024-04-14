using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WcfScalingTestClient_Framework;

namespace WcfScalingTest.Benchmarks_Framework
{
    [SimpleJob(RunStrategy.Monitoring, iterationCount: 10, id: "MonitoringJob")]
    [MinColumn, Q1Column, Q3Column, MaxColumn]
    public class TestClientBenchmark
    {
        [Benchmark]
        public async Task TestStreamed()
        {
            var clientTest = new ClientTest();
            await clientTest.Test(streaming: true, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }

        [Benchmark]
        public async Task TestBuffered()
        {
            var clientTest = new ClientTest();
            await clientTest.Test(streaming: false, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }


        [Benchmark]
        public async Task TestAsyncStreamed()
        {
            var clientTest = new ClientTest();
            await clientTest.TestAsync(streaming: true, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }

        [Benchmark]
        public async Task TestAsyncBuffered()
        {
            var clientTest = new ClientTest();
            await clientTest.TestAsync(streaming: false, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }
    }
}
