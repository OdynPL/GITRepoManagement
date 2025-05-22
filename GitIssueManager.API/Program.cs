using GitIssueManager.API.DTO;
using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load tokens
builder.Services.Configure<GitTokensOptions>(builder.Configuration.GetSection("GitTokens"));

// Register HTTP Client
builder.Services.AddHttpClient();

// Register HttpHelperService
builder.Services.AddSingleton<IHttpHelperService, HttpHelperService>();

// Register GitHubService and GitLabService with HttpHelperService + token
builder.Services.AddTransient<GitHubService>(sp =>
{
    var httpHelper = sp.GetRequiredService<IHttpHelperService>();
    var gitTokens = sp.GetRequiredService<IOptions<GitTokensOptions>>().Value;
    return new GitHubService(httpHelper, gitTokens.GitHubToken);
});

builder.Services.AddTransient<GitLabService>(sp =>
{
    var httpHelper = sp.GetRequiredService<IHttpHelperService>();
    var gitTokens = sp.GetRequiredService<IOptions<GitTokensOptions>>().Value;
    return new GitLabService(httpHelper, gitTokens.GitLabToken);
});

// Register factory for IGitService by enum
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseInlineDefinitionsForEnums());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
