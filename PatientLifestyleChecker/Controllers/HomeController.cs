using System.Diagnostics;
using LifeStyleChecker.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using LifeStyleChecker.Utilities;
using Xunit;

namespace LifeStyleChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private string _SubscriptionKey;
        public HttpClient Client { get; set; }

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _SubscriptionKey = _config["Configurations:NhsNumberApiKey"];
            Client = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> PatientSearch(FormPatientModel model) {

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            model.ErrorMessage = string.Empty;

            var patient = await GetPatientFromAPI(model.NhsNumber);

            if (patient.Name == null)
            {
                ModelState.AddModelError("NhsNumber", "Your details could not be found");
                return View("Index", model);
                
            }

            var searchResponse = model.EvaluateSearchResponse(patient);

            return GetViewBasedOnParameters(searchResponse, model);
        }

       
        public async Task<PatientModel> GetPatientFromAPI(int nhsNumber)
        {
            PatientModel patient;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://al-tech-test-apim.azure-api.net/tech-test/t2/patients/{nhsNumber}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _SubscriptionKey);

                //GET Method
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    patient = JsonConvert.DeserializeObject<PatientModel>(json) ?? new PatientModel();
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Patient not found");
                    patient = new PatientModel();
                    ModelState.AddModelError("NhsNumber", "Your details could not be found");
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                    patient = new PatientModel();
                }
            }
            return patient;
        }

        public IActionResult GetViewBasedOnParameters(Utilities.SearchOutcomes searchOutcome, FormPatientModel model)
        {
            switch (searchOutcome)
            {
                case Utilities.SearchOutcomes.EligiblePatientFound:
                    return RedirectToAction("Index", "Survey", new { patientAge = model.GetPatientAge() });
                case Utilities.SearchOutcomes.UnderAge:
                    ModelState.AddModelError("NhsNumber", "You are not eligible for this service, thank you for your interest");
                    return View("Index", model);
                case Utilities.SearchOutcomes.DetailsNotMatched:
                    ModelState.AddModelError("NhsNumber", "Your details could not be found");
                    return View("Index", model);
                case Utilities.SearchOutcomes.NotFound:
                    ModelState.AddModelError("NhsNumber", "Your details could not be found");
                    return View("Index", model);
                default:
                    return View("Index", model);
            }

        }

    }
}
