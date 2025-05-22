
using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/issues")]
public class ProjectIssuesController : ControllerBase
{
    private readonly Func<GitServiceType, IGitService> _gitServiceFactory;

    public ProjectIssuesController(Func<GitServiceType, IGitService> gitServiceFactory)
    {
        _gitServiceFactory = gitServiceFactory;
    }

    // Create new Git issue. Provide owner name and repository name
    [HttpPost("{service}/{owner}/{repo}")]
    public async Task<IActionResult> Create(
        [FromRoute(Name = "service")] GitServiceType serviceType,
        [FromRoute] string owner,
        [FromRoute] string repo,
        [FromBody] ProjectIssue issue)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        IGitService serviceInstance;
        try
        {
            serviceInstance = _gitServiceFactory(serviceType);
        }
        catch (NotSupportedException)
        {
            return BadRequest($"Git service not configured: {serviceType}");
        }

        var fullRepo = $"{owner}/{repo}";

        var result = await serviceInstance.CreateIssueAsync(fullRepo, issue);

        return Ok(result);
    }

    // Update git issue, provide owner name, repository name and issue ID
    [HttpPut("{service}/{owner}/{repo}/{id:int}")]
    public async Task<IActionResult> Update(
        [FromRoute(Name = "service")] GitServiceType serviceType,
        [FromRoute] string owner,
        [FromRoute] string repo,
        [FromRoute] int id,
        [FromBody] ProjectIssue issue)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        IGitService serviceInstance;
        try
        {
            serviceInstance = _gitServiceFactory(serviceType);
        }
        catch (NotSupportedException)
        {
            return BadRequest($"Git service not configured: {serviceType}");
        }

        var fullRepo = $"{owner}/{repo}";

        var success = await serviceInstance.UpdateIssueAsync(fullRepo, id, issue);

        return success ? Ok() : StatusCode(500, "Failed to update issue.");
    }

    // Close current issue endpoint, provide owner ID, repository name and issue ID
    [HttpDelete("{service}/{owner}/{repo}/{id:int}")]
    public async Task<IActionResult> Close(
        [FromRoute(Name = "service")] GitServiceType serviceType,
        [FromRoute] string owner,
        [FromRoute] string repo,
        [FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        IGitService serviceInstance;
        try
        {
            serviceInstance = _gitServiceFactory(serviceType);
        }
        catch (NotSupportedException)
        {
            return BadRequest($"Git service not configured: {serviceType}");
        }

        var fullRepo = $"{owner}/{repo}";

        var success = await serviceInstance.CloseIssueAsync(fullRepo, id);

        return success ? Ok() : StatusCode(500, "Failed to close issue.");
    }
}