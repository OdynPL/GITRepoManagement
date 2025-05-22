using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitIssueManager.Core.Interfaces
{
    public interface IHttpHelperService
    {
        Task<string> SendAsync(string url, HttpMethod method, object payload = null, IDictionary<string, string> headers = null);
    }

}
