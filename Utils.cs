using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workout_API_Test_Suite
{
    internal static class Utils
    {
        public static HttpClient ScaffoldApplicationAndGetClient()
        {
            var application = new WorkoutWebApplicationFactory();
            return application.CreateClient();
        }
    }
}
