using LifeStyleChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // add unit test for Name method

    }
}
