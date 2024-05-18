using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using Workout_API.Models;

namespace Workout_API_Test_Suite
{
    public class UserControllerTests
    {
        private static readonly string Name = "Test";
        private static readonly string Email = "test@email.com";

        [Fact]
        public async Task CreateUser()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            var response = await AttemptCreateUser(httpClient);
            response.EnsureSuccessStatusCode();

            var clientReponse = await response.Content.ReadFromJsonAsync<User>();
            clientReponse?.Id.Should().BePositive();
            clientReponse?.Name.Should().Be(Name);
            clientReponse?.Email.Should().Be(Email);
        }

        [Fact]
        public async Task CreateInvalidUsersShouldFail()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            User[] invalidUsers = GetInvalidUsers();

            for (int i = 0; i < invalidUsers.Length; i++)
            {
                User user = invalidUsers[i];
                var response = await httpClient.PostAsJsonAsync("/User", user);
                response.StatusCode.ToString().Should().Be("BadRequest");
            }
        }

        [Fact]
        public async Task CreateUserThatAlreadyExistsShouldFail()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            await AttemptCreateUser(httpClient);

            var badCreateResponse = await AttemptCreateUser(httpClient);
            badCreateResponse.StatusCode.ToString().Should().Be("BadRequest");

            string responseBody = await badCreateResponse.Content.ReadAsStringAsync();
            responseBody.Should().Be("A user is already associated with that email");
        }

        private User[] GetInvalidUsers()
        {
            string invalidEmail = "testemai.com";

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

            return invalidUsers;
        }

        [Fact]
        public async Task GetUser()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            await AttemptCreateUser(httpClient);
            var response = await AttemptGetUser(httpClient, Email);

            response.EnsureSuccessStatusCode();
            var clientReponse = await response.Content.ReadFromJsonAsync<User>();
            clientReponse?.Name.Should().Be(Name);
            clientReponse?.Email.Should().Be(Email);
        }

        [Fact]
        public async Task DeleteUser()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            await AttemptCreateUser(httpClient);

            var response = await httpClient.DeleteAsync($"/User?Email={Email}");
            response.EnsureSuccessStatusCode();

            var ensureDeletedResponse = await httpClient.GetAsync($"/User?Email={Email}");
            ensureDeletedResponse.StatusCode.ToString().Should().Be("NotFound");
        }

        [Fact]
        public async Task UpdateUser()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            // create a valid user
            await AttemptCreateUser(httpClient);
            var getResponse = await AttemptGetUser(httpClient, Email);
            var clientReponse = await getResponse.Content.ReadFromJsonAsync<User>();

            if (clientReponse == null)
            {
                throw new Exception("Failed initial user creation");
            }

            string updatedName = "Updated";
            clientReponse.Name = updatedName;
            var updateResponse = await AttemptUserUpdate(httpClient, clientReponse);
            updateResponse.EnsureSuccessStatusCode();

            // validate update succeeded
            getResponse = await AttemptGetUser(httpClient, Email);
            getResponse.EnsureSuccessStatusCode();

            clientReponse = await getResponse.Content.ReadFromJsonAsync<User>();
            clientReponse?.Name.Should().Be(updatedName);
        }

        [Fact]
        public async Task UpdateUserThatDoesNotExistShouldFail()
        {
            var httpClient = Utils.ScaffoldApplicationAndGetClient();

            User user = new User()
            {
                Name = Name,
                Email = Email, 
                Id = 0
                
            };

            var updateResponse = await AttemptUserUpdate(httpClient, user);
            updateResponse.StatusCode.ToString().Should().Be("BadRequest");
        }

        private static async Task<HttpResponseMessage> AttemptUserUpdate(HttpClient httpClient, User user)
        {
            return await httpClient.PutAsJsonAsync("/User", user);
        }

        private static async Task<HttpResponseMessage> AttemptCreateUser(HttpClient httpClient)
        {
            User? user = new User
            {
                Name = Name,
                Email = Email,
                Id = 0
            };

            return await httpClient.PostAsJsonAsync("/User", user);
        }

        private static async Task<HttpResponseMessage> AttemptGetUser(HttpClient httpClient, string email)
        {
            return await httpClient.GetAsync($"/User?Email={email}");
        }
    }
}