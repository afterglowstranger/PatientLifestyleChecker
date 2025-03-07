using LifeStyleChecker.Models;
using LifeStyleChecker.Utilities;
using Newtonsoft.Json;
using Xunit; // Add this using directive

namespace PatientLifestyleCheckerTest.Models
{
    public class PatientModelTests
    {
        [Fact]
        public void TestNhsNumberValidation()
        {
            // Arrange
            var patientModel = new PatientModel();

            // Act
            patientModel.NhsNumber = 123456789;

            // Assert
            Assert.Equal(123456789, patientModel.NhsNumber);
        }

        [Fact]
        public void TestNameProperty()
        {
            // Arrange
            var patientModel = new PatientModel();

            // Act
            patientModel.Name = "John Doe";

            // Assert
            Assert.Equal("John Doe", patientModel.Name);
        }

        [Fact]
        public void TestBornProperty()
        {
            // Arrange
            var patientModel = new PatientModel();
            var expectedDate = new DateTime(1990, 1, 1);

            // Act
            patientModel.Born = expectedDate;

            // Assert
            Assert.Equal(expectedDate, patientModel.Born);
        }

        [Fact]
        public void TestCustomDateTimeConverter()
        {
            // Arrange
            var date = new DateTime(1990, 1, 1);
            var patientModel = new PatientModel
            {
                Born = date
            };
            var expectedJson = "{\"NhsNumber\":0,\"Name\":null,\"Born\":\"01-01-1990\"}";

            // Act
            var json = JsonConvert.SerializeObject(patientModel);

            // Assert
            Assert.Equal(expectedJson, json);
        }
    }
}
