using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workout_API;

namespace Workout_API_Test_Suite
{
    internal class Utils
    {
        private static IConfigurationRoot? configuration;

        public static HttpClient ScaffoldApplicationAndGetClient()
        {
            var application = new WorkoutWebApplicationFactory();
            return application.CreateClient();
        }

        public static IConfigurationRoot GetUserSecretsConfiguration()
        {
            configuration ??= new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            return configuration;
        }

        public static string GetConfigurationItem(string key)
        {
            var config = GetUserSecretsConfiguration();

            string configurationItem = config[key];

            if (configurationItem.IsNullOrEmpty())
                throw new Exception("User secret has not been configured");

            return configurationItem;
        }
    }
}
