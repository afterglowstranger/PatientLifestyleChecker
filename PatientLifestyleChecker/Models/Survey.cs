namespace LifeStyleChecker.Models
{
    public class Survey
    {
        public Guid Id { get; set; }
        
        public List<SurveyOutcome> Outcomes { get; set; }

        public List<LifestyleQuestion> Questions { get; set; }

        public int PatientAge { get; set; }
        public Survey()
        {
            Questions = new List<LifestyleQuestion>();
            Outcomes = new List<SurveyOutcome>();
        }
    }
}
