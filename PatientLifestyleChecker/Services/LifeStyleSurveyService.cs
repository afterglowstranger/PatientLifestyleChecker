using LifeStyleChecker.Models;
using System.Text.Json;

namespace LifeStyleChecker.Services
{
    public class LifeStyleSurveyService : ILifeStyleSurveyService
    {
        public LifeStyleSurveyService() { }

        public void AddSurvey()
        {
            throw new NotImplementedException();
        }

        public async Task<Survey> GetSurvey()
        {
         
            Survey survey = new Survey();
                        
            //survey.Id = Guid.NewGuid();

            //survey.Questions.Add(new LifestyleQuestion
            //{
            //    Id = Guid.NewGuid(),
            //    Question = "Q1. Do you drink on more than 2 days a week?",
            //    QuestionOrder = 1,
            //    ScoreAffirmative = true,
            //    QuestionScores = new List<QuestionScore>
            //    {
            //        new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
            //        new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
            //        new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 3 },
            //        new QuestionScore { LowerBound = 65, Score = 3 },
            //    }
            //});

            //survey.Questions.Add(new LifestyleQuestion
            //{
            //    Id = Guid.NewGuid(),
            //    Question = "Q2. Do you smoke?",
            //    QuestionOrder = 2,
            //    ScoreAffirmative = true,
            //    QuestionScores = new List<QuestionScore>
            //    {
            //        new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 2 },
            //        new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 2 },
            //        new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 2 },
            //        new QuestionScore { LowerBound = 65, Score = 3 },
            //    }
            //});

            //survey.Questions.Add(new LifestyleQuestion
            //{
            //    Id = Guid.NewGuid(),
            //    Question = "Q3. Do you exercise more than 1 hour per week?",
            //    QuestionOrder = 3,
            //    ScoreAffirmative = false,
            //    QuestionScores = new List<QuestionScore>
            //    {
            //        new QuestionScore { LowerBound = 16, UpperBound = 21, Score = 1 },
            //        new QuestionScore { LowerBound = 22, UpperBound = 40, Score = 3 },
            //        new QuestionScore { LowerBound = 41, UpperBound = 64, Score = 2 },
            //        new QuestionScore { LowerBound = 65, Score = 1 },
            //    }
            //});

            //survey.Outcomes.Add(new SurveyOutcome
            //{
            //    Id = Guid.NewGuid(),
            //    Outcome = "Thank you for answering our questions, we don't need to see you at this time. Keep up the good work!",
            //    UpperBound = 3
            //});

            //survey.Outcomes.Add(new SurveyOutcome
            //{
            //    Id = Guid.NewGuid(),
            //    Outcome = "We think there are some simple things you could do to improve you quality of life, please phone to book an appointment",
            //    LowerBound = 4
            //});

            //await using FileStream createStream = File.OpenWrite("wwwroot/Data/survey.json");
            //await JsonSerializer.SerializeAsync(createStream, survey);

            string text = File.ReadAllText("wwwroot/Data/survey.json");
            survey = JsonSerializer.Deserialize<Survey>(text) ?? new Survey();
            

            return survey;
        }
    }
}
