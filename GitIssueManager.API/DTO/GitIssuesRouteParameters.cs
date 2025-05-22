using GitIssueManager.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class GitIssuesRouteParameters
{
    [Required]
    [MinLength(1)]
    [FromRoute(Name = "owner")]
    public required string Owner { get; set; }

    [Required]
    [MinLength(1)]
    [FromRoute(Name = "repo")]
    public required string Repo { get; set; }

    [Required]
    [FromRoute(Name = "service")]
    public GitServiceType ServiceType { get; set; }
}
