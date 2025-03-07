using LifeStyleChecker.Models;
using LifeStyleChecker.Utilities;

namespace PatientLifestyleCheckerTest.Models
{
    public class FormPatientModelTests
    {
        [Fact]
        public void TestGetPatientAge()
        {
            // Arrange
            var formPatientModel = new FormPatientModel();
            formPatientModel.Born = new DateOnly(1990, 01, 01);
            var birthDate = new DateTime(1990, 1, 1);
            var expectedAge = DateTime.Now.Year - birthDate.Year;

            // Act
            var actualAge = formPatientModel.GetPatientAge();

            // Assert
            Assert.Equal(expectedAge, actualAge);
        }

       

        [Fact]
        public void TestName()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                FirstName = "John",
                LastName = "Doe"
            };
            var expectedName = "DOE, John";

            // Act
            var actualName = formPatientModel.Name();

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Fact]
        public void TestIsFormPatientValid()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };
            var patientModel = new PatientModel
            {
                NhsNumber = 123456789,
                Name = "DOE, John",
                Born = new DateTime(1990, 01, 01)
            };

            // Act
            var isValid = formPatientModel.IsFormPatientValid(patientModel);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void TestIsFormPatientNHSIdValid()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                NhsNumber = 123456789
            };
            var patientModel = new PatientModel
            {
                NhsNumber = 123456789
            };

            // Act
            var isNHSIdValid = formPatientModel.IsFormPatientNHSIdValid(patientModel);

            // Assert
            Assert.True(isNHSIdValid);
        }

        [Fact]
        public void TestEvaluateSearchResponse_EligiblePatientFound()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };
            var patientModel = new PatientModel
            {
                NhsNumber = 123456789,
                Name = "DOE, John",
                Born = new DateTime(1990, 01, 01)
            };

            // Act
            var outcome = formPatientModel.EvaluateSearchResponse(patientModel);

            // Assert
            Assert.Equal(SearchOutcomes.EligiblePatientFound, outcome);
        }

        [Fact]
        public void TestEvaluateSearchResponse_UnderAge()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(DateTime.Now.Year - 10, 01, 01)
            };
            var patientModel = new PatientModel
            {
                NhsNumber = 123456789,
                Name = "DOE, John",
                Born = new DateTime(DateTime.Now.Year - 10, 01, 01)
            };

            // Act
            var outcome = formPatientModel.EvaluateSearchResponse(patientModel);

            // Assert
            Assert.Equal(SearchOutcomes.UnderAge, outcome);
        }

        [Fact]
        public void TestEvaluateSearchResponse_DetailsNotMatched()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };
            var patientModel = new PatientModel
            {
                NhsNumber = 123456789,
                Name = "DOE, Jane",
                Born = new DateTime(1990, 01, 01)
            };

            // Act
            var outcome = formPatientModel.EvaluateSearchResponse(patientModel);

            // Assert
            Assert.Equal(SearchOutcomes.DetailsNotMatched, outcome);
        }

        [Fact]
        public void TestEvaluateSearchResponse_NotFound()
        {
            // Arrange
            var formPatientModel = new FormPatientModel
            {
                NhsNumber = 123456789,
                FirstName = "John",
                LastName = "Doe",
                Born = new DateOnly(1990, 01, 01)
            };
            var patientModel = new PatientModel
            {
                NhsNumber = 987654321,
                Name = "DOE, Jane",
                Born = new DateTime(1990, 01, 01)
            };

            // Act
            var outcome = formPatientModel.EvaluateSearchResponse(patientModel);

            // Assert
            Assert.Equal(SearchOutcomes.NotFound, outcome);
        }
    }
}