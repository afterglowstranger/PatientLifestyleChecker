using LifeStyleChecker.Models;
using LifeStyleChecker.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeStyleChecker.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ILifeStyleSurveyService _lifeStyleSurveyService;
        

        public SurveyController(ILifeStyleSurveyService lifeStyleSurveyService)
        {
            _lifeStyleSurveyService = lifeStyleSurveyService;
            
        }

        public IActionResult Index(int patientAge)
        {
            
            var survey = _lifeStyleSurveyService.GetSurvey().Result;
            if (!survey.Questions.Any()) {

                SurveyOutcome outcome = new SurveyOutcome();
                outcome.Outcome = "Error: Unable to load Survey with at least one question, please contact support";
                return View("OutcomePage", outcome);
            }
            survey.PatientAge = patientAge;
            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSurvey( Survey surveyResponse)
        {
            var survey = await _lifeStyleSurveyService.GetSurvey();
            var patientScore = GetPatientScore(survey, surveyResponse).Result;

            var derivedOutcome = GetOutcomeFromScore(survey, patientScore);

            return View("OutcomePage", derivedOutcome);
        }

        public async Task<int> GetPatientScore(Survey survey,Survey surveyResponse)
        {
            int patientScore = 0;

            foreach (var answer in surveyResponse.Questions)
            {
                //We only want to count the score if the answer is affirmative(true) and the ScoreAffirmative is true or if the answer is negative(false) and the ScoreAffirmative is false
                var answerScores = survey.Questions.Where(a => a.Id == answer.Id && answer.AffirmativeResponse == a.ScoreAffirmative).SingleOrDefault();

                if (answerScores != null)
                {
                    var answerScore = answerScores.QuestionScores.Where(a => a.LowerBound <= surveyResponse.PatientAge && surveyResponse.PatientAge <= a.UpperBound).SingleOrDefault()?.Score;
                    //Guards incase the Patients age doesn't have a score band associated with it
                    if (answerScore.HasValue)
                    {
                        patientScore += answerScore.Value;
                    }
                }
            }
 
            return patientScore; ;
        }

        public SurveyOutcome? GetOutcomeFromScore(Survey survey, int PatientScore)
        {
            //Once we have the Patients Score we check for the outcome where
            //the score is between a lower and upper mark or above a lower mark
            //where no upper mark and vice versa

            var derivedOutcome = survey.Outcomes
                .Where(s => (s.LowerBound <= PatientScore && PatientScore <= s.UpperBound)
                            || (s.LowerBound is null && PatientScore <= s.UpperBound)
                            || (s.LowerBound <= PatientScore && s.UpperBound is null))
                .SingleOrDefault();
            return derivedOutcome;
        }

    }
}
