using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;

public class GitHubService : IGitService
{
    private readonly IHttpHelperService _httpHelper;
    private readonly string _token;

    public GitHubService(IHttpHelperService httpHelper, string token)
    {
        _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    private IDictionary<string, string> CreateHeaders()
    {
        return new Dictionary<string, string>
        {
            { "User-Agent", "GitIssueManagerApp" },
            { "Authorization", $"token {_token}" }
        };
    }

    public async Task<string> CreateIssueAsync(string repo, ProjectIssue issue)
    {
        if (string.IsNullOrWhiteSpace(repo)) throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));
        if (issue == null) throw new ArgumentNullException(nameof(issue));

        var url = $"https://api.github.com/repos/{Uri.UnescapeDataString(repo)}/issues";

        var payload = new
        {
            title = issue.Title,
            body = issue.Description
        };

        return await _httpHelper.SendAsync(url, HttpMethod.Post, payload, CreateHeaders());
    }

    public async Task<bool> UpdateIssueAsync(string repo, int issueId, ProjectIssue issue)
    {
        if (string.IsNullOrWhiteSpace(repo)) throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));
        if (issue == null) throw new ArgumentNullException(nameof(issue));

        var url = $"https://api.github.com/repos/{Uri.UnescapeDataString(repo)}/issues/{issueId}";

        var payload = new
        {
            title = issue.Title,
            body = issue.Description
        };

        var response = await _httpHelper.SendAsync(url, new HttpMethod("PATCH"), payload, CreateHeaders());

        return true;
    }

    public async Task<bool> CloseIssueAsync(string repo, int issueId)
    {
        if (string.IsNullOrWhiteSpace(repo)) throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));

        var url = $"https://api.github.com/repos/{Uri.UnescapeDataString(repo)}/issues/{issueId}";

        var payload = new
        {
            state = "closed"
        };

        await _httpHelper.SendAsync(url, new HttpMethod("PATCH"), payload, CreateHeaders());

        return true;
    }
}