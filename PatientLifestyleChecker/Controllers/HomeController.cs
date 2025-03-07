using System.Diagnostics;
using LifeStyleChecker.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace LifeStyleChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private string _SubscriptionKey;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _SubscriptionKey = _config["Configurations:NhsNumberApiKey"];
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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://al-tech-test-apim.azure-api.net/tech-test/t2/patients/{model.NhsNumber}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _SubscriptionKey);


                //client.DefaultRequestHeaders.Accept.Add(newMediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    PatientModel patient = JsonConvert.DeserializeObject<PatientModel>(json);

                    var searchResponse = model.EvaluateSearchResponse(patient);
                    switch (searchResponse)
                    {
                        case Utilities.SearchOutcomes.EligiblePatientFound:
                            //return this.RedirectToAction<SurveyController>(m => m.GetPatientAge());
                            //return View("../Survey/Index");
                            return RedirectToAction("Index", "Survey", new { patientAge = model.GetPatientAge() });//
                            
                        case Utilities.SearchOutcomes.UnderAge:
                            
                            ModelState.AddModelError("NhsNumber", "You are not eligible for this service, thank you for your interest");
                            return View("Index",model);
                            
                        case Utilities.SearchOutcomes.DetailsNotMatched:
                            
                            //model.ErrorMessage = "Your details could not be found";
                            ModelState.AddModelError("NhsNumber", "Your details could not be found");
                            return View("Index", model);
                            
                        case Utilities.SearchOutcomes.NotFound:
                            //This branch should never be hit if the api call returns 404 correctly
                            break;
                        default:

                            break;
                    }
                    

                    

                    //{ "nhsNumber":"111222333","name":"DOE, John","born":"14-01-2007"}
                    // Get the URI of the created resource.
                    //UrireturnUrl = response.Headers.Location;
                    //Console.WriteLine(returnUrl);

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Patient not found");
                    ModelState.AddModelError("NhsNumber", "Your details could not be found");
                    return View("Index", model);
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                        return View("Index");
                }
            }

            return View("Index",model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
