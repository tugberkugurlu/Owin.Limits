namespace Owin.Limits
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin.Testing;
    using Xunit;

    public class MaxBandwidthTests
    {
        [Fact]
        public async Task When_bandwidth_is_applied_then_time_to_receive_data_should_be_longer()
        {
            int bandwidth = 0;
            // ReSharper disable once AccessToModifiedClosure - yeah we want to modify it...
            Func<int> getMaxBandwidth = () => bandwidth;
            HttpClient httpClient = CreateHttpClient(getMaxBandwidth);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await httpClient.GetAsync("http://example.com");
            TimeSpan nolimitTimeSpan = stopwatch.Elapsed;

            bandwidth = 1; // ~1bps, should take ~3s
            await httpClient.GetAsync("http://example.com");
            TimeSpan limitedTimeSpan = stopwatch.Elapsed;

            Console.WriteLine(nolimitTimeSpan);
            Console.WriteLine(limitedTimeSpan);

            limitedTimeSpan.Should().BeGreaterThan(nolimitTimeSpan);
        }

        private static HttpClient CreateHttpClient(Func<int> getMaxBytesPerSecond)
        {
            return TestServer.Create(builder => builder
                .MaxBandwidth(getMaxBytesPerSecond)
                .Use(async (context, _) =>
                {
                    byte[] bytes = Enumerable.Repeat((byte) 0x1, 3).ToArray();
                    context.Response.StatusCode = 200;
                    context.Response.ReasonPhrase = "OK";
                    context.Response.ContentLength = bytes.LongLength;
                    context.Response.ContentType = "application/octet-stream";
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                })).HttpClient;
        }
    }
}