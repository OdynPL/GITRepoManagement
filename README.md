**GIT Repo Management POC APP for XOPERO Interview.**

Project structure:
- GitIssueManager.API
- GitIssueManager.Core

Project preparad with Swagger and works currently for GitHUB and GitLab (other may be add also)

**Please configure git tokens inside "appsettings.json" of API**

```
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",

    "GitTokens": {
        "GitHubToken": "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",
        "GitLabToken": "glpat-GYFtmdhPq8SJeQWxonrm"
    }
}
```

Or use mine if needed (for testing)

**Public repositories for testing:**

GitHUB repository: https://github.com/OdynPL/TestREPO/issues
GitLAB repository: https://gitlab.com/odynpl-group/OdynPL
