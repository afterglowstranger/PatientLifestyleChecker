namespace LifeStyleChecker.Models
{
    public class SurveyOutcome
    {
        public Guid Id { get; set; }
        public int? LowerBound { get; set; }
        public int? UpperBound { get; set; }

        public string Outcome { get; set; }

    }
}
