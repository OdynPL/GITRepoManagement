using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load Auth tokens from Configuration inside "appsettings.json"
var gitHubToken = builder.Configuration["GitTokens:GitHubToken"];
var gitLabToken = builder.Configuration["GitTokens:GitLabToken"];

// Register HTTP Client instance as singleton
builder.Services.AddSingleton<HttpClient>();

// Register GitHUB service with token using Factory
builder.Services.AddTransient<GitHubService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();

    var client = new HttpClient
    {
        BaseAddress = new Uri("https://api.github.com/")
    };
    client.DefaultRequestHeaders.UserAgent.ParseAdd("GitIssueManagerApp/1.0");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", gitHubToken);

    return new GitHubService(client, gitHubToken);
});

// Register GitHUB service with token using Factory
builder.Services.AddTransient<GitLabService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();

    var client = new HttpClient
    {
        BaseAddress = new Uri("https://gitlab.com/api/v4/")
    };
    client.DefaultRequestHeaders.UserAgent.ParseAdd("GitIssueManagerApp/1.0");
    client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", gitLabToken);

    return new GitLabService(client, gitLabToken);
});

// Register GIT services factory based on enum
builder.Services.AddSingleton<Func<GitServiceType, IGitService>>(sp => key =>
{
    return key switch
    {
        GitServiceType.github => sp.GetRequiredService<GitHubService>(),
        GitServiceType.gitlab => sp.GetRequiredService<GitLabService>(),
        _ => throw new NotSupportedException($"Git service type {key} is not supported.")
    };
});

// Configure controllers and JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add Swagger and Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware for exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
