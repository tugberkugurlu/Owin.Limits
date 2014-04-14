namespace Owin.Limits
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin.Testing;
    using Xunit;

    public class MaxUrlLengthTests
    {
        [Fact]
        public async Task When_max_urlLength_is_20_and_a_url_with_length_18_should_be_served()
        {
            HttpClient client = CreateClient(20);

            HttpResponseMessage response = await client.GetAsync("http://example.com");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_max_urlLength_is_20_and_a_url_with_length_39_is_coming_it_should_be_rejected_with_custom_reasonPhrase()
        {
            HttpClient client = CreateClient(20);

            HttpResponseMessage response = await client.GetAsync("http://example.com/example/example.html");

            response.StatusCode.Should().Be(HttpStatusCode.RequestUriTooLong);
            response.ReasonPhrase.Should().Be("custom phrase");
        }

        [Fact]
        public async Task When_max_urlLength_is_30_and_a_url_with_escaped_length_42_but_unescaped_28_it_should_be_served()
        {
            HttpClient client = CreateClient(30);

            HttpResponseMessage response = await client.GetAsync("http://example.com?q=%48%49%50%51%52%53%54");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        private static HttpClient CreateClient(int length)
        {
            return TestServer.Create(builder => builder
                .MaxUrlLength(new MaxUrlLengthOptions(length)
                {
                    LimitReachedReasonPhrase = code => "custom phrase"
                })
                .Use((context, next) =>
                {
                    context.Response.StatusCode = 200;
                    context.Response.ReasonPhrase = "OK";
                    return Task.FromResult(0);
                })).HttpClient;
        }
    }
}