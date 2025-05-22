using System.Threading.Tasks;
using GitIssueManager.Core.Models;

namespace GitIssueManager.Core.Interfaces
{
    public interface IGitService
    {
        Task<string> CreateIssueAsync(string repo, ProjectIssue issue);
        Task<bool> UpdateIssueAsync(string repo, int issueId, ProjectIssue issue);
        Task<bool> CloseIssueAsync(string repo, int issueId);
    }
}
