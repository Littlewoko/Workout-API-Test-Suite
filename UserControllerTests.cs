using FluentAssertions;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using Workout_API.Models;

namespace Workout_API_Test_Suite
{
    public class UserControllerTests
    {
        [Fact]
        public async Task CreateUser_successful()
        {
            string Name = "Test";
            string Email = "test@email.com";
            int Id = 0;

            // Arrange
            var application = new WorkoutWebApplicationFactory();

            User validUser = new User
            {
                Name = Name,
                Email = Email,
                Id = Id
            };

            var httpClient = application.CreateClient();

            // Act
            var response = await httpClient.PostAsJsonAsync("/User", validUser);

            // Assert
            response.EnsureSuccessStatusCode();

            var clientReponse = await response.Content.ReadFromJsonAsync<User>();

            clientReponse?.Id.Should().BePositive();
            clientReponse?.Name.Should().Be(Name);
            clientReponse?.Email.Should().Be(Email);
        }
    }
}