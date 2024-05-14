using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using Workout_API.Models;

namespace Workout_API_Test_Suite
{
    public class UserControllerTests
    {
        private static string Name = "Test";
        private static string Email = "test@email.com";
        [Fact]
        public async Task CreateUser_successful()
        {
            var application = new WorkoutWebApplicationFactory();
            var httpClient = application.CreateClient();

            var response = await CreateUserHelper(httpClient);
            response.EnsureSuccessStatusCode();

            var clientReponse = await response.Content.ReadFromJsonAsync<User>();

            clientReponse?.Id.Should().BePositive();
            clientReponse?.Name.Should().Be(Name);
            clientReponse?.Email.Should().Be(Email);
        }

        [Fact]
        public async Task CreateUser_shouldFail()
        {
            string invalidEmail = "testemai.com";

            // probably move this out to some form of metadata
            // todo: create parser for plain text metadata
            User[] invalidUsers = new User[]
            {
                new User
                {
                    Name = Name,
                    Email = String.Empty,
                    Id = 0
                },
                new User
                {
                    Name = String.Empty,
                    Email = Email,
                    Id = 0
                },
                new User
                {
                    Name = String.Empty,
                    Email = invalidEmail,
                    Id = 0
                },
                new User
                {
                    Name = String.Empty,
                    Email = String.Empty,
                    Id = 0
                },
            };

            var application = new WorkoutWebApplicationFactory();
            var httpClient = application.CreateClient();

            for (int i = 0; i < invalidUsers.Length; i++)
            {
                User user = invalidUsers[i];

                var response = await httpClient.PostAsJsonAsync("/User", user);
                response.StatusCode.ToString().Should().Be("BadRequest");
            }
        }

        [Fact]
        public async Task GetUser()
        {
            // Arrange
            var application = new WorkoutWebApplicationFactory();
            var httpClient = application.CreateClient();

            // Act
            var response = await CreateAndGetUserHelper(httpClient);

            // Assert
            response.EnsureSuccessStatusCode();

            var clientReponse = await response.Content.ReadFromJsonAsync<User>();

            clientReponse?.Name.Should().Be(Name);
            clientReponse?.Email.Should().Be(Email);
        }

        [Fact]
        public async Task DeleteUser()
        {
            var application = new WorkoutWebApplicationFactory();
            var httpClient = application.CreateClient();

            var getResponse = await CreateAndGetUserHelper(httpClient);
            getResponse.EnsureSuccessStatusCode();

            var response = await httpClient.DeleteAsync($"/User?Email={Email}");

            response.EnsureSuccessStatusCode();

            getResponse = await httpClient.GetAsync($"/User?Email={Email}");
            getResponse.StatusCode.ToString().Should().Be("NotFound");
        }

        [Fact]
        public async Task UpdateUser()
        {
            string updatedName = "Updated";

            var application = new WorkoutWebApplicationFactory();
            var httpClient = application.CreateClient();

            // create a valid user
            var getResponse = await CreateAndGetUserHelper(httpClient);
            getResponse.EnsureSuccessStatusCode();

            var clientReponse = await getResponse.Content.ReadFromJsonAsync<User>();
            clientReponse?.Should().NotBeNull();

            // update the user
            clientReponse.Name = updatedName;
            var response = await httpClient.PutAsJsonAsync("/User", clientReponse);
            response.EnsureSuccessStatusCode();

            getResponse = await httpClient.GetAsync($"/User?Email={Email}");
            getResponse.EnsureSuccessStatusCode();

            clientReponse = await getResponse.Content.ReadFromJsonAsync<User>();

            clientReponse?.Name.Should().Be(updatedName);
        }

        private async Task<HttpResponseMessage> CreateUserHelper(HttpClient httpClient)
        {
            User? user = new User
            {
                Name = Name,
                Email = Email,
                Id = 0
            };

            return await httpClient.PostAsJsonAsync("/User", user);
        }

        private async Task<HttpResponseMessage> CreateAndGetUserHelper(HttpClient httpClient)
        {
            await CreateUserHelper(httpClient);
            return await httpClient.GetAsync($"/User?Email={Email}");
        }
    }
}