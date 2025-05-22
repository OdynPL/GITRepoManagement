namespace GitIssueManager.API.DTO
{
    public class GitTokensOptions
    {
        public required string GitHubToken { get; set; }
        public required string GitLabToken { get; set; }
    }

}
