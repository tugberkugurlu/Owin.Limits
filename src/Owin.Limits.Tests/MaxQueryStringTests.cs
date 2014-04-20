namespace Owin.Limits
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin.Testing;
    using Xunit;

    public class MaxQueryStringTests
    {
        [Fact]
        public async Task When_max_queryString_length_is_10_then_a_request_with_9_should_be_served()
        {
            HttpClient client = CreateClient(10);

            HttpResponseMessage response = await client.GetAsync("http://example.com?q=1234567");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_max_queryString_length_is_10_then_a_request_with_11_should_be_rejected()
        {
            HttpClient client = CreateClient(10);

            HttpResponseMessage response = await client.GetAsync("http://example.com?q=123456789");

            response.StatusCode.Should().Be(HttpStatusCode.RequestUriTooLong);
        }

        [Fact]
        public async Task
            When_max_queryString_length_is_10_then_a_request_with_escaped_length_greater_than_10_but_unescaped_lower_or_equal_than_10_should_be_serverd()
        {
            HttpClient client = CreateClient(10);

            HttpResponseMessage response = await client.GetAsync("http://example.com?q=%48%49%50%51%52%53%54");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_queryString_exceeds_max_length_then_request_should_be_rejected_with_custom_reasonPhrase()
        {
            HttpClient client = CreateClient(5, "custom phrase");

            HttpResponseMessage response = await client.GetAsync("http://example.com?q=123456");

            response.StatusCode.Should().Be(HttpStatusCode.RequestUriTooLong);
            response.ReasonPhrase.Should().Be("custom phrase");
        }

        private static HttpClient CreateClient(int length)
        {
            return CreateClient(length, "");
        }

        private static HttpClient CreateClient(int length, string reasonPhrase)
        {
            return TestServer.Create(builder => builder
                .MaxQueryStringLength(new MaxQueryStringLengthOptions(length) {LimitReachedReasonPhrase = code => reasonPhrase})
                .Use((context, next) =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ReasonPhrase = "OK";
                    return Task.FromResult(0);
                })).HttpClient;
        }
    }
}