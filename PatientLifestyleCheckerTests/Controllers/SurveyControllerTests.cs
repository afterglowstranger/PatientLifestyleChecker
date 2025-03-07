using LifeStyleChecker.Controllers;
using LifeStyleChecker.Models;
using LifeStyleChecker.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace PatientLifestyleCheckerTest.Controllers
{
    public class SurveyControllerTests
    {
        
            private readonly Mock<ILifeStyleSurveyService> _mockService;
            private readonly SurveyController _controller;

            public SurveyControllerTests()
            {
                _mockService = new Mock<ILifeStyleSurveyService>();
                _controller = new SurveyController(_mockService.Object);
            }

        [Fact]
        public async Task Index_SurveyWithQuestions_ReturnsSurveyView()
        {
            // Arrange
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        Question = "Do you exercise regularly?",
                        ScoreAffirmative = true,
                        QuestionOrder = 1,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                Outcomes = new List<SurveyOutcome>()
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            var result = _controller.Index(25);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Survey>(viewResult.Model);
            Assert.Equal(25, model.PatientAge);
            Assert.Single(model.Questions);
        }

        [Fact]
        public async Task Index_SurveyWithoutQuestions_ReturnsOutcomePageWithError()
        {
            // Arrange
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>(),
                Outcomes = new List<SurveyOutcome>()
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            var result = _controller.Index(25);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("OutcomePage", viewResult.ViewName);
            var model = Assert.IsType<SurveyOutcome>(viewResult.Model);
            Assert.Equal("Error: Unable to load Survey with at least one question, please contact support", model.Outcome);
        }

        [Fact]
            public async Task SubmitSurvey_ValidSurvey_ReturnsOutcomePage()
            {
                // Arrange
                var survey = new Survey
                {
                    Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                    Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
                };

                var surveyResponse = new Survey
                {
                    PatientAge = 25,
                    Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = true
                    }
                }
                };

                _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

                // Act
                var result = await _controller.SubmitSurvey(surveyResponse);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("OutcomePage", viewResult.ViewName);
                var model = Assert.IsType<SurveyOutcome>(viewResult.Model);
                Assert.Equal("Outcome 1", model.Outcome);
            }

            [Fact]
            public async Task SubmitSurvey_NoMatchingScores_ReturnsOutcomePageWithDefaultOutcome()
            {
                // Arrange
                var survey = new Survey
                {
                    Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                    Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
                };

                var surveyResponse = new Survey
                {
                    PatientAge = 25,
                    Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = false
                    }
                }
                };

                _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

                // Act
                var result = await _controller.SubmitSurvey(surveyResponse);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal("OutcomePage", viewResult.ViewName);
                var model = Assert.IsType<SurveyOutcome>(viewResult.Model);
                Assert.Equal("Outcome 1", model.Outcome);
            }

        [Fact]
        public async Task GetOutcome_ValidSurvey_ReturnsCorrectOutcome()
        {
            // Arrange
            var patientScore = 0;
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 25,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = true
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            patientScore = await _controller.GetPatientScore(survey, surveyResponse);
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 1", outcome.Outcome);
        }

        [Fact]
        public async Task GetOutcome_NoMatchingScores_ReturnsDefaultOutcome()
        {
            // Arrange
            var patientScore = 0;
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 25,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = false
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            patientScore = await _controller.GetPatientScore(survey, surveyResponse);
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 1", outcome.Outcome);
        }

        [Fact]
        public async Task GetOutcome_NoMatchingScores_ReturnsDefaultOutcome2()
        {
            // Arrange
            var patientScore = 0;
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 4 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 41,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = true
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            patientScore = await _controller.GetPatientScore(survey, surveyResponse);
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 2", outcome.Outcome);
        }

        [Fact]
        public async Task GetOutcome_MatchingScoreNegate_ReturnsDefaultOutcome()
        {
            // Arrange
            var patientScore = 0;
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = false,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 4 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                },
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 41,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = false
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            patientScore = await _controller.GetPatientScore(survey, surveyResponse);
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 2", outcome.Outcome);
        }

        [Fact]
        public async Task GetOutcome_NoMatchingScoreNegate_ReturnsDefaultOutcome()
        {
            // Arrange
            var patientScore = 0;
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = false,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 4 }
                        }
                    },
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 10 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 11 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 12 },
                            new QuestionScore { LowerBound = 65, Score = 13 }
                        }
                    }
                },
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 0, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 15, UpperBound = 15, Outcome = "Outcome 2" }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 41,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = false
                    },
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[1].Id,
                        AffirmativeResponse = true
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            patientScore = await _controller.GetPatientScore(survey, surveyResponse);
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 2", outcome.Outcome);
        }

        [Fact]
        public async Task GetPatientScore_ValidSurvey_ReturnsCorrectScore()
        {
            // Arrange
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 25,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = true
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            var patientScore = await _controller.GetPatientScore(survey, surveyResponse);

            // Assert
            Assert.Equal(2, patientScore);
        }

        [Fact]
        public async Task GetPatientScore_NoMatchingScores_ReturnsZero()
        {
            // Arrange
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 25,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = false
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            var patientScore = await _controller.GetPatientScore(survey, surveyResponse);

            // Assert
            Assert.Equal(0, patientScore);
        }

        [Fact]
        public async Task GetPatientScore_MultipleQuestions_ReturnsCorrectScore()
        {
            // Arrange
            var survey = new Survey
            {
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
                            new QuestionScore { LowerBound = 65, Score = 3 }
                        }
                    },
                    new LifestyleQuestion
                    {
                        Id = Guid.NewGuid(),
                        ScoreAffirmative = true,
                        QuestionScores = new List<QuestionScore>
                        {
                            new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 2 },
                            new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 3 },
                            new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 4 },
                            new QuestionScore { LowerBound = 65, Score = 4 }
                        }
                    }
                }
            };

            var surveyResponse = new Survey
            {
                PatientAge = 25,
                Questions = new List<LifestyleQuestion>
                {
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[0].Id,
                        AffirmativeResponse = true
                    },
                    new LifestyleQuestion
                    {
                        Id = survey.Questions[1].Id,
                        AffirmativeResponse = true
                    }
                }
            };

            _mockService.Setup(service => service.GetSurvey()).ReturnsAsync(survey);

            // Act
            var patientScore = await _controller.GetPatientScore(survey, surveyResponse);

            // Assert
            Assert.Equal(5, patientScore);
        }

        [Fact]
        public void GetOutcomeFromScore_ValidScore_ReturnsCorrectOutcome()
        {
            // Arrange
            var survey = new Survey
            {
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };
            int patientScore = 2;

            // Act
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 1", outcome.Outcome);
        }

        [Fact]
        public void GetOutcomeFromScore_ScoreInUpperBound_ReturnsCorrectOutcome()
        {
            // Arrange
            var survey = new Survey
            {
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };
            int patientScore = 5;

            // Act
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 2", outcome.Outcome);
        }

        [Fact]
        public void GetOutcomeFromScore_ScoreInLowerBound_ReturnsCorrectOutcome()
        {
            // Arrange
            var survey = new Survey
            {
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };
            int patientScore = 4;

            // Act
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 2", outcome.Outcome);
        }

        [Fact]
        public void GetOutcomeFromScore_ScoreOutOfBounds_ReturnsNull()
        {
            // Arrange
            var survey = new Survey
            {
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };
            int patientScore = 7;

            // Act
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.Null(outcome);
        }

        [Fact]
        public void GetOutcomeFromScore_ScoreMatchesLowerBoundOnly_ReturnsCorrectOutcome()
        {
            // Arrange
            var survey = new Survey
            {
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = 0, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = null, Outcome = "Outcome 2" }
                }
            };
            int patientScore = 5;

            // Act
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 2", outcome.Outcome);
        }

        [Fact]
        public void GetOutcomeFromScore_ScoreMatchesUpperBoundOnly_ReturnsCorrectOutcome()
        {
            // Arrange
            var survey = new Survey
            {
                Outcomes = new List<SurveyOutcome>
                {
                    new SurveyOutcome { LowerBound = null, UpperBound = 3, Outcome = "Outcome 1" },
                    new SurveyOutcome { LowerBound = 4, UpperBound = 6, Outcome = "Outcome 2" }
                }
            };
            int patientScore = 2;

            // Act
            var outcome = _controller.GetOutcomeFromScore(survey, patientScore);

            // Assert
            Assert.NotNull(outcome);
            Assert.Equal("Outcome 1", outcome.Outcome);
        }
    }
}

