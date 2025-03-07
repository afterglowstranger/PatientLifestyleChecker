namespace LifeStyleChecker.Models
{
    public class LifestyleQuestion
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public bool ScoreAffirmative { get; set; } = true;
        public int QuestionOrder { get; set; }
        public bool? AffirmativeResponse { get; set; }

        public List<QuestionScore> QuestionScores { get; set; }

        public LifestyleQuestion()
        {
            QuestionScores = new List<QuestionScore>();
        }

    }
}
