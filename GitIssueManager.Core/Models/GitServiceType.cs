using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitIssueManager.Core.Models
{
    // GIT repository enumerable with service type providers
    // Add more if needed here to display on Swagger drop down
    public enum GitServiceType
    {
        github,
        gitlab
    }
}
