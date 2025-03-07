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
            survey.PatientAge = patientAge;
            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSurvey( Survey surveyResponse)
        {
            var survey = await _lifeStyleSurveyService.GetSurvey();

            int PatientScore = 0;

            foreach(var answer in surveyResponse.Questions)
            {
                var answerScores = survey.Questions.Where(a => a.Id == answer.Id && answer.AffirmativeResponse == a.ScoreAffirmative).SingleOrDefault();

                if (answerScores != null)
                {
                    var answerScore = answerScores.QuestionScores.Where(a => a.LowerBound <= surveyResponse.PatientAge &&  surveyResponse.PatientAge <= a.UpperBound ).SingleOrDefault().Score;
                    PatientScore += answerScore;
                }

            }

            var derivedOutcome = survey.Outcomes.Where(s => (s.LowerBound <= PatientScore && PatientScore <= s.UpperBound) || (s.LowerBound is null && PatientScore <= s.UpperBound) || (s.LowerBound <= PatientScore && s.UpperBound is null)).SingleOrDefault();


            return View("OutcomePage", derivedOutcome);
        }
    }
}
