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

            HttpResponseMessage request = await client.GetAsync("http://example.com?q=1234567");

            request.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_max_queryString_length_is_10_then_a_request_with_11_should_be_rejected()
        {
            HttpClient client = CreateClient(10);

            HttpResponseMessage request = await client.GetAsync("http://example.com?q=123456789");

            request.StatusCode.Should().Be(HttpStatusCode.RequestUriTooLong);
            request.ReasonPhrase.Should().Be("The (unescaped) querystring is too long. Only 10 characters are allowed.");
        }

        [Fact]
        public async Task
            When_max_queryString_length_is_10_then_a_request_with_escaped_length_greater_than_10_but_unescaped_lower_or_equal_than_10_should_be_serverd
            ()
        {
            HttpClient client = CreateClient(10);

            HttpResponseMessage request = await client.GetAsync("http://example.com?q=%48%49%50%51%52%53%54");

            request.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        private static HttpClient CreateClient(int length)
        {
            return TestServer.Create(builder => builder
                .MaxQueryStringLength(length)
                .Use((context, next) =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ReasonPhrase = "OK";
                    return Task.FromResult(0);
                })).HttpClient;
        }
    }
}