using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace LifeStyleChecker.Models
{
    public class PatientModel
    {
        
        [Required]
        [Range(100000000, 999999999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int NhsNumber { get; set; }
       
        public string Name { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Born { get; set; }


       
    }

    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd-MM-yyyy";
        }
    }
}
