using LifeStyleChecker.Utilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LifeStyleChecker.Models
{
    public class FormPatientModel //:PatientModel
    {

        [Required]
        [DisplayName("Nhs Number")]
        [Range(100000000, 999999999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int NhsNumber { get; set; }
        
        [Required]
        [DisplayName("first name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [DisplayName("last name")]
        public string LastName { get; set; } = string.Empty;
        //public string Name => LastName.ToUpper() + ", " + CapitalizeFirstLetter(FirstName);
        [Required]
        public DateOnly Born { get; set; }

        public string ErrorMessage { get; set; }= string.Empty;

        internal string Name() => LastName.ToUpper() + ", " + CapitalizeFirstLetter(FirstName);

        public int GetPatientAge()
        {
            DateTime now = DateTime.Now;
            int age = now.Year - Born.Year;
            if (now.Month < Born.Month || (now.Month == Born.Month && now.Day < Born.Day))
                age--;
            return age;
        }   

        public string CapitalizeFirstLetter(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        internal bool IsFormPatientValid(PatientModel model)
        {
            return this.NhsNumber == model.NhsNumber && 
                this.Name() == model.Name && 
                this.Born.Day == model.Born.Day &&
                this.Born.Month == model.Born.Month && 
                this.Born.Year == model.Born.Year;
        }

        internal bool IsFormPatientNHSIdValid(PatientModel model)
        {
            return this.NhsNumber == model.NhsNumber; 
        }



        internal SearchOutcomes EvaluateSearchResponse(PatientModel model)
        {
            if(this.IsFormPatientValid(model))
            {
                return this.GetPatientAge() >= 16 ? SearchOutcomes.EligiblePatientFound : SearchOutcomes.UnderAge;
            }
            else if (this.IsFormPatientNHSIdValid(model))
            {
                return SearchOutcomes.DetailsNotMatched;
            }
            else
            {
                //logger.LogWarning("This should never happen!");

                return SearchOutcomes.NotFound;
            }
        }
    }
}
