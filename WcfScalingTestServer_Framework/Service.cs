using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WcfScalingTestContract;

namespace WcfScalingTestServer_Framework
{
    internal class Service : IService
    {
        public string TestOperation(string input)
        {
            Thread.Sleep(50);
            return $"echoed_{input}_framework";
        }
    }
}
