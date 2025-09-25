using System.Net;
using Moq;
using Moq.Protected;
using speedCode.codeTesting;
using speedCode.dto;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using speedCode.Tests.dto;

namespace speedCode.speedCode.Tests
{
    public class Judge0ClientTests
    {
        [Fact]
        public async Task SubmitCodeAsync_ShouldReturnSubmissionResult()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Judge0ClientTests>()
                .AddEnvironmentVariables()
                .Build();

            string baseUrl = config["Judge0:BaseUrl"] ?? "http://localhost:2358";

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            var client = new Judge0Client(httpClient);

            SubmissionResult result = await client.SubmitCodeAsync("print('Hello World')", 70);
            
            Assert.Equal(Judge0Statuses.Accepted, result.Status.Id);
            Assert.Equal(result.Stdout, "Hello World\n");
        }
    }
}