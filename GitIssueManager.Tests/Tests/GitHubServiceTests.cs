using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;
using Moq;

namespace GitIssueManager.Tests.Services
{
    public class GitHubServiceTests
    {
        private readonly Mock<IHttpHelperService> _httpHelperMock;
        private readonly GitHubService _gitHubService;
        private const string Token = "test-token";

        public GitHubServiceTests()
        {
            _httpHelperMock = new Mock<IHttpHelperService>();
            _gitHubService = new GitHubService(_httpHelperMock.Object, Token);
        }

        [Fact]
        public async Task CreateIssueAsync_ReturnsResponseFromHttpHelper_WhenCalledWithValidParameters()
        {
            // Arrange
            var repo = "owner/repo";
            var issue = new ProjectIssue { Title = "Test", Description = "Desc" };
            _httpHelperMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<HttpMethod>(), It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync("response");

            // Act
            var result = await _gitHubService.CreateIssueAsync(repo, issue);

            // Assert
            Assert.Equal("response", result);
            _httpHelperMock.Verify(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Post, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateIssueAsync_ThrowsArgumentException_WhenRepositoryIsNullOrEmpty(string invalidRepo)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _gitHubService.CreateIssueAsync(invalidRepo, new ProjectIssue()));
        }

        [Fact]
        public async Task CreateIssueAsync_ThrowsArgumentNullException_WhenIssueIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gitHubService.CreateIssueAsync("repo", null));
        }

        [Fact]
        public async Task UpdateIssueAsync_ReturnsTrue_WhenCalledWithValidParameters()
        {
            _httpHelperMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<HttpMethod>(), It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync("response");

            var result = await _gitHubService.UpdateIssueAsync("repo", 1, new ProjectIssue());

            Assert.True(result);
            _httpHelperMock.Verify(x => x.SendAsync(It.IsAny<string>(), It.Is<HttpMethod>(m => m.Method == "PATCH"), It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateIssueAsync_ThrowsArgumentException_WhenRepositoryIsNullOrEmpty(string invalidRepo)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _gitHubService.UpdateIssueAsync(invalidRepo, 1, new ProjectIssue()));
        }

        [Fact]
        public async Task UpdateIssueAsync_ThrowsArgumentNullException_WhenIssueIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gitHubService.UpdateIssueAsync("repo", 1, null));
        }

        [Fact]
        public async Task CloseIssueAsync_ReturnsTrue_WhenCalledWithValidParameters()
        {
            _httpHelperMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<HttpMethod>(), It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync("response");

            var result = await _gitHubService.CloseIssueAsync("repo", 1);

            Assert.True(result);
            _httpHelperMock.Verify(x => x.SendAsync(It.IsAny<string>(), It.Is<HttpMethod>(m => m.Method == "PATCH"), It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CloseIssueAsync_ThrowsArgumentException_WhenRepositoryIsNullOrEmpty(string invalidRepo)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _gitHubService.CloseIssueAsync(invalidRepo, 1));
        }
    }
}
