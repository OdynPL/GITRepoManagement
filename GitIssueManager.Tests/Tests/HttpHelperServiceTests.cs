using System.Net;
using Newtonsoft.Json;

namespace GitIssueManager.Tests.Services
{
    public class HttpHelperServiceTests
    {
        private HttpClient CreateHttpClient(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
        {
            var handlerMock = new DelegatingHandlerStub(handlerFunc);
            return new HttpClient(handlerMock);
        }

        [Fact]
        public async Task SendAsync_ReturnsResponseContent_WhenRequestIsSuccessful()
        {
            // Arrange
            var expectedResponse = "Success!";
            var httpClient = CreateHttpClient(_ =>
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

            var service = new HttpHelperService(httpClient);

            // Act
            var result = await service.SendAsync("https://example.com", HttpMethod.Get);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task SendAsync_ThrowsException_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var httpClient = CreateHttpClient(_ =>
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad request")
                });

            var service = new HttpHelperService(httpClient);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.SendAsync("https://example.com", HttpMethod.Get));

            Assert.Contains("HTTP error", ex.Message);
            Assert.Contains("Bad request", ex.Message);
        }

        [Fact]
        public async Task SendAsync_SendsPayloadAsJson_WhenPayloadIsProvided()
        {
            // Arrange
            object capturedPayload = null;

            var httpClient = CreateHttpClient(request =>
            {
                var json = request.Content.ReadAsStringAsync().Result;
                capturedPayload = JsonConvert.DeserializeObject(json);

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("ok")
                };
            });

            var service = new HttpHelperService(httpClient);

            var payload = new { title = "Test", description = "Example" };

            // Act
            var result = await service.SendAsync("https://example.com", HttpMethod.Post, payload);

            // Assert
            Assert.NotNull(capturedPayload);
            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task SendAsync_AddsHeaders_WhenProvided()
        {
            // Arrange
            var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer token" },
            { "Custom-Header", "123" }
        };

            Dictionary<string, string> capturedHeaders = null;

            var httpClient = CreateHttpClient(request =>
            {
                capturedHeaders = new Dictionary<string, string>();
                foreach (var header in request.Headers)
                {
                    capturedHeaders[header.Key] = string.Join(",", header.Value);
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("headers-ok")
                };
            });

            var service = new HttpHelperService(httpClient);

            // Act
            var result = await service.SendAsync("https://example.com", HttpMethod.Get, null, headers);

            // Assert
            Assert.Equal("headers-ok", result);
            Assert.True(capturedHeaders.ContainsKey("Authorization"));
            Assert.Equal("Bearer token", capturedHeaders["Authorization"]);
            Assert.True(capturedHeaders.ContainsKey("Custom-Header"));
        }
    }

    // Stub for HTTPClient testing helper class using HttpMessageHandler class
    // Instead of sending real requests we use faked/stubbed
    public class DelegatingHandlerStub : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

        public DelegatingHandlerStub(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
        {
            _handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handlerFunc(request));
        }
    }
}
