namespace Owin.Limits
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin.Infrastructure;
    using Owin.Testing;
    using Xunit;

    public class MaxConcurrentRequestsTests
    {
        [Fact]
        public async Task When_max_concurrent_request_is_1_then_second_request_should_get_service_unavailable()
        {
            OwinTestServer owinTestServer = CreateTestServer(1);
            Task<HttpResponseMessage> request1 = owinTestServer.CreateHttpClient().GetAsync("http://example.com");
            Task<HttpResponseMessage> request2 = owinTestServer.CreateHttpClient().GetAsync("http://example.com");

            await Task.WhenAll(request1, request2);

            request1.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            request2.Result.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task When_max_concurrent_request_is_2_then_second_request_should_get_ok()
        {
            OwinTestServer owinTestServer = CreateTestServer(2);
            Task<HttpResponseMessage> request1 = owinTestServer.CreateHttpClient().GetAsync("http://example.com");
            Task<HttpResponseMessage> request2 = owinTestServer.CreateHttpClient().GetAsync("http://example.com");

            await Task.WhenAll(request1, request2);

            request1.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            request2.Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_max_concurrent_request_is_0_then_second_request_should_get_ok()
        {
            OwinTestServer owinTestServer = CreateTestServer(0);
            Task<HttpResponseMessage> request1 = owinTestServer.CreateHttpClient().GetAsync("http://example.com");
            Task<HttpResponseMessage> request2 = owinTestServer.CreateHttpClient().GetAsync("http://example.com");

            await Task.WhenAll(request1, request2);

            request1.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            request2.Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private static OwinTestServer CreateTestServer(int maxConcurrentRequests)
        {
            return OwinTestServer.Create(builder =>
            {
                SignatureConversions.AddConversions(builder); // supports Microsoft.Owin.OwinMiddleWare
                builder
                    .MaxBandwidth(1)
                    .MaxConcurrentRequests(maxConcurrentRequests)
                    .Use(async context =>
                    {
                        byte[] bytes = Enumerable.Repeat((byte)0x1, 2).ToArray();
                        context.Response.StatusCode = 200;
                        context.Response.ReasonPhrase = "OK";
                        context.Response.ContentLength = bytes.LongLength;
                        context.Response.ContentType = "application/octet-stream";
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
            });
        }
    }
}