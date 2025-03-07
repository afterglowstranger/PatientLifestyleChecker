using LifeStyleChecker.Models;

namespace LifeStyleChecker.Services
{
    public interface ILifeStyleSurveyService
    {

        public void AddSurvey();

        public Task<Survey> GetSurvey();
    }
}
