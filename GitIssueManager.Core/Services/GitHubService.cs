using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;
using Newtonsoft.Json;

public class GitHubService : IGitService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public GitHubService(HttpClient httpClient, string token)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    public async Task<string> CreateIssueAsync(string repo, ProjectIssue issue)
    {
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));
        if (issue == null)
            throw new ArgumentNullException(nameof(issue));

        repo = Uri.UnescapeDataString(repo);
        var url = $"https://api.github.com/repos/{repo}/issues";

        var payload = new
        {
            title = issue.Title,
            body = issue.Description
        };

        var request = CreateRequest(HttpMethod.Post, url, payload);
        var response = await _httpClient.SendAsync(request);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"GitHub error ({response.StatusCode}): {content}");

        return content;
    }

    public async Task<bool> UpdateIssueAsync(string repo, int issueId, ProjectIssue issue)
    {
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));
        if (issue == null)
            throw new ArgumentNullException(nameof(issue));

        repo = Uri.UnescapeDataString(repo);
        var url = $"https://api.github.com/repos/{repo}/issues/{issueId}";

        var payload = new
        {
            title = issue.Title,
            body = issue.Description
        };

        var request = CreateRequest(new HttpMethod("PATCH"), url, payload);
        var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CloseIssueAsync(string repo, int issueId)
    {
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));

        repo = Uri.UnescapeDataString(repo);
        var url = $"https://api.github.com/repos/{repo}/issues/{issueId}";

        var payload = new
        {
            state = "closed"
        };

        var request = CreateRequest(new HttpMethod("PATCH"), url, payload);
        var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, object payload)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        };
        AddHeaders(request);
        return request;
    }

    private void AddHeaders(HttpRequestMessage request)
    {
        request.Headers.UserAgent.ParseAdd("GitIssueManagerApp"); // wymagany User-Agent
        request.Headers.Authorization = new AuthenticationHeaderValue("token", _token);
    }
}
