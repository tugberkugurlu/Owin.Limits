namespace Owin.Limits
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin.Testing;
    using Xunit;

    public class MaxConcurrentRequestsTests
    {
        [Fact]
        public async Task When_max_concurrent_request_is_1_then_second_request_should_get_service_unavailable_and_custom_reasonPhrase()
        {
            HttpClient httpClient = CreateHttpClient(1);
            Task<HttpResponseMessage> request1 = httpClient.GetAsync("http://example.com");
            Task<HttpResponseMessage> request2 = httpClient.GetAsync("http://example.com");

            await Task.WhenAll(request1, request2);

            request1.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            request2.Result.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            request2.Result.ReasonPhrase.Should().Be("custom phrase");
        }

        [Fact]
        public async Task When_max_concurrent_request_is_2_then_second_request_should_get_ok()
        {
            HttpClient httpClient = CreateHttpClient(2);
            Task<HttpResponseMessage> request1 = httpClient.GetAsync("http://example.com");
            Task<HttpResponseMessage> request2 = httpClient.GetAsync("http://example.com");

            await Task.WhenAll(request1, request2);

            request1.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            request2.Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_max_concurrent_request_is_0_then_second_request_should_get_ok()
        {
            HttpClient httpClient = CreateHttpClient(0);
            Task<HttpResponseMessage> request1 = httpClient.GetAsync("http://example.com");
            Task<HttpResponseMessage> request2 = httpClient.GetAsync("http://example.com");

            await Task.WhenAll(request1, request2);

            request1.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            request2.Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private static HttpClient CreateHttpClient(int maxConcurrentRequests)
        {
            return TestServer.Create(builder => builder
                .Use().MaxBandwidth(1)
                .MaxConcurrentRequests(new MaxConcurrentRequestOptions(maxConcurrentRequests)
                {
                    LimitReachedReasonPhrase = code => "custom phrase"
                })
                .Use(builder)
                .Use(async (context, _) =>
                {
                    byte[] bytes = Enumerable.Repeat((byte) 0x1, 2).ToArray();
                    context.Response.StatusCode = 200;
                    context.Response.ReasonPhrase = "OK";
                    context.Response.ContentLength = bytes.LongLength;
                    context.Response.ContentType = "application/octet-stream";
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                })).HttpClient;
        }
    }
}