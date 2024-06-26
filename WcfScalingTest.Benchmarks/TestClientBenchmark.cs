﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WcfScalingTestClient;

namespace WcfScalingTest.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, runtimeMoniker: RuntimeMoniker.Net60, iterationCount: 10, id: "MonitoringJob")]
    [SimpleJob(RunStrategy.Monitoring, runtimeMoniker: RuntimeMoniker.Net80, iterationCount: 10, id: "MonitoringJob")]
    [MinColumn, Q1Column, Q3Column, MaxColumn]
    public class TestClientBenchmark
    {
        [Benchmark]
        public async Task TestStreamed()
        {
            var clientTest = new ClientTest(null);
            await clientTest.Test(streaming: true, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }

        [Benchmark]
        public async Task TestBuffered()
        {
            var clientTest = new ClientTest(null);
            await clientTest.Test(streaming: false, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }

        [Benchmark]
        public async Task TestStreamed_500Threads()
        {
            System.Threading.ThreadPool.SetMinThreads(500, 100);
            var clientTest = new ClientTest(null);
            await clientTest.Test(streaming: true, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }

        [Benchmark]
        public async Task TestBuffered_500Threads()
        {
            System.Threading.ThreadPool.SetMinThreads(500, 100);
            var clientTest = new ClientTest(null);
            await clientTest.Test(streaming: false, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }


        [Benchmark]
        public async Task TestAsyncStreamed()
        {
            var clientTest = new ClientTest(null);
            await clientTest.TestAsync(streaming: true, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }

        [Benchmark]
        public async Task TestAsyncBuffered()
        {
            var clientTest = new ClientTest(null);
            await clientTest.TestAsync(streaming: false, Array.Empty<IContractBehavior>(), Array.Empty<IEndpointBehavior>());
        }
    }
}
