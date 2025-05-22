**GIT Repo Management POC APP for XOPERO Interview.**

Project structure:
- GitIssueManager.API
- GitIssueManager.Core
- GitIssueManager.Core (Some Unit Tests using xUnit for services)

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

**How to use my application?**

Please run API and go to Swagger page: **http://localhost:5254/swagger/index.html** (for example)

![image](https://github.com/user-attachments/assets/1048b5a7-042b-40ae-a0cb-d6db834e5757)


**Select one of 3 end points of our API:**

For GitHUB and GitLab You have to provide owner/group name which is for example "OdynPL"
Repository name like "TestREPO"
Issue/ticket ID which can be found inside #22 number.

![image](https://github.com/user-attachments/assets/0ca1f6c6-4f4f-4e14-8965-0c25efb0f723)



1. Add new issue (POST)
   
![image](https://github.com/user-attachments/assets/33ba9e8c-956b-4283-b66b-721bb856d04d)

3. Update current issue (PUT)
   
![{B61CE6C4-B2D0-4336-929C-3FB6E5BAEA40}](https://github.com/user-attachments/assets/68da3f2e-694f-4c72-b0f6-eaed59eb7517)

5. Close/complete issue (DELETE)

![{1165C804-597B-4314-B131-9E332CE8C60C}](https://github.com/user-attachments/assets/24c8f7c7-f11e-4d3e-acf7-96cc4e38dc4c)





