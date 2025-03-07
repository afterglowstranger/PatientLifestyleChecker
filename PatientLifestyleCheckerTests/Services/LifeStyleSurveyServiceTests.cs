using LifeStyleChecker.Models;
using LifeStyleChecker.Services;
using Moq;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PatientLifestyleCheckerTest.Services
{
    public class LifeStyleSurveyServiceTests
    {
        private readonly LifeStyleSurveyService _service;

        public LifeStyleSurveyServiceTests()
        {
            _service = new LifeStyleSurveyService();
        }

        // I manually tested both paths
        // from the get survey method.
        // Forcing the code to return and empty
        // survey and with genuine json file contents

    }
}

