using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetoworkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetoworkLib.Tests
{
    using System.Net;
    using System.Net.NetworkInformation;

    [TestClass()]
    public class NetworkUtilsTests
    {
        [TestMethod()]
        public void PingHostTest()
        {
            var ipAddress = IPAddress.Parse("192.168.0.1");
            long roundTripTime;
            var result = NetworkUtils.PingHost(ipAddress.ToString(), out roundTripTime);
            Console.WriteLine($"{ipAddress} : {roundTripTime} : {result}");
        }

        [TestMethod()]
        public void PingHostTest2()
        {
            foreach (var result in new[]
                                  {
                                      "www.albahari.com",
                                      "www.linqpad.net",
                                      "www.oreilly.com",
                                      "www.takeonit.com",
                                      "stackoverflow.com",
                                      "www.rebeccarey.com"
                                  }
                .AsParallel().WithDegreeOfParallelism(6).Select(site => new { site, p = new Ping().Send(site) }).Select(
                    @t => new
                              {
                                  @t.site,
                                  Result = @t.p.Status,
                                  Time = @t.p.RoundtripTime
                              }))
            {

                Console.WriteLine($"{result}");
            }
        }
    }
}