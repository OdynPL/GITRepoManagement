using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitIssueManager.Core.Interfaces;
using Newtonsoft.Json;

public class HttpHelperService : IHttpHelperService
{
    private readonly HttpClient _httpClient;

    public HttpHelperService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> SendAsync(string url, HttpMethod method, object payload = null, IDictionary<string, string> headers = null)
    {
        using (var request = new HttpRequestMessage(method, url))
        {
            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP error ({response.StatusCode}): {content}");
            }

            return content;
        }
    }

}
