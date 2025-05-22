using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;

public class GitLabService : IGitService
{
    private readonly IHttpHelperService _httpHelper;
    private readonly string _token;

    public GitLabService(IHttpHelperService httpHelper, string token)
    {
        _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    public async Task<string> CreateIssueAsync(string repo, ProjectIssue issue)
    {
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));
        if (issue == null)
            throw new ArgumentNullException(nameof(issue));

        var encodedRepo = Uri.EscapeDataString(repo);
        var url = $"https://gitlab.com/api/v4/projects/{encodedRepo}/issues";

        var payload = new
        {
            title = issue.Title,
            description = issue.Description
        };

        var headers = new Dictionary<string, string>
        {
            { "User-Agent", "GitIssueManagerApp" },
            { "PRIVATE-TOKEN", _token }
        };

        return await _httpHelper.SendAsync(url, HttpMethod.Post, payload, headers);
    }

    public async Task<bool> UpdateIssueAsync(string repo, int issueId, ProjectIssue issue)
    {
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));
        if (issue == null)
            throw new ArgumentNullException(nameof(issue));

        var encodedRepo = Uri.EscapeDataString(repo);
        var url = $"https://gitlab.com/api/v4/projects/{encodedRepo}/issues/{issueId}";

        var payload = new
        {
            title = issue.Title,
            description = issue.Description
        };

        var headers = new Dictionary<string, string>
        {
            { "User-Agent", "GitIssueManagerApp" },
            { "PRIVATE-TOKEN", _token }
        };

        var response = await _httpHelper.SendAsync(url, HttpMethod.Put, payload, headers);

        // Jeśli nie ma wyjątku, to znaczy że jest sukces
        return true;
    }

    public async Task<bool> CloseIssueAsync(string repo, int issueId)
    {
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty.", nameof(repo));

        var encodedRepo = Uri.EscapeDataString(repo);
        var url = $"https://gitlab.com/api/v4/projects/{encodedRepo}/issues/{issueId}";

        var payload = new
        {
            state_event = "close"
        };

        var headers = new Dictionary<string, string>
        {
            { "User-Agent", "GitIssueManagerApp" },
            { "PRIVATE-TOKEN", _token }
        };

        await _httpHelper.SendAsync(url, HttpMethod.Put, payload, headers);
        return true;
    }
}
