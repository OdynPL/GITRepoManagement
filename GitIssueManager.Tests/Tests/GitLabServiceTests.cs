using GitIssueManager.Core.Interfaces;
using GitIssueManager.Core.Models;
using Moq;

namespace GitIssueManager.Tests.Services
{
    public class GitLabServiceTests
    {
        private readonly Mock<IHttpHelperService> _httpHelperMock;
        private readonly GitLabService _gitLabService;
        private const string Token = "test-token";

        public GitLabServiceTests()
        {
            _httpHelperMock = new Mock<IHttpHelperService>();
            _gitLabService = new GitLabService(_httpHelperMock.Object, Token);
        }

        [Fact]
        public async Task CreateIssueAsync_ReturnsResponseFromHttpHelper_WhenCalledWithValidParameters()
        {
            // Arrange
            var repo = "group/project";
            var issue = new ProjectIssue { Title = "Test", Description = "Test description" };
            _httpHelperMock.Setup(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Post, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync("created");

            // Act
            var result = await _gitLabService.CreateIssueAsync(repo, issue);

            // Assert
            Assert.Equal("created", result);
            _httpHelperMock.Verify(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Post, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateIssueAsync_ThrowsArgumentException_WhenRepositoryIsInvalid(string invalidRepo)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _gitLabService.CreateIssueAsync(invalidRepo, new ProjectIssue()));
        }

        [Fact]
        public async Task CreateIssueAsync_ThrowsArgumentNullException_WhenIssueIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gitLabService.CreateIssueAsync("repo", null));
        }

        [Fact]
        public async Task UpdateIssueAsync_ReturnsTrue_WhenCalledWithValidParameters()
        {
            // Arrange
            var issue = new ProjectIssue { Title = "Updated", Description = "Updated desc" };
            _httpHelperMock.Setup(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Put, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync("updated");

            // Act
            var result = await _gitLabService.UpdateIssueAsync("repo", 123, issue);

            // Assert
            Assert.True(result);
            _httpHelperMock.Verify(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Put, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateIssueAsync_ThrowsArgumentException_WhenRepositoryIsInvalid(string invalidRepo)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _gitLabService.UpdateIssueAsync(invalidRepo, 1, new ProjectIssue()));
        }

        [Fact]
        public async Task UpdateIssueAsync_ThrowsArgumentNullException_WhenIssueIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gitLabService.UpdateIssueAsync("repo", 1, null));
        }

        [Fact]
        public async Task CloseIssueAsync_ReturnsTrue_WhenCalledWithValidParameters()
        {
            _httpHelperMock.Setup(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Put, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync("closed");

            var result = await _gitLabService.CloseIssueAsync("repo", 123);

            Assert.True(result);
            _httpHelperMock.Verify(x => x.SendAsync(It.IsAny<string>(), HttpMethod.Put, It.IsAny<object>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CloseIssueAsync_ThrowsArgumentException_WhenRepositoryIsInvalid(string invalidRepo)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _gitLabService.CloseIssueAsync(invalidRepo, 123));
        }
    }
}
