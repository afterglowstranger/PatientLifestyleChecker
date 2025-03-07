using System.Diagnostics;
using LifeStyleChecker.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

            var patient = await GetPatientFromAPI(model.NhsNumber);

            if (patient == null)
            {
                ModelState.AddModelError("NhsNumber", "Your details could not be found");
                return View("Index", model);
                
            }

            var searchResponse = model.EvaluateSearchResponse(patient);

            return GetViewBasedOnParameters(searchResponse, model);

            //switch (searchResponse)
            //{
            //    case Utilities.SearchOutcomes.EligiblePatientFound:
            //        return RedirectToAction("Index", "Survey", new { patientAge = model.GetPatientAge() });
            //    case Utilities.SearchOutcomes.UnderAge:
            //        ModelState.AddModelError("NhsNumber", "You are not eligible for this service, thank you for your interest");
            //        return View("Index", model);
            //    case Utilities.SearchOutcomes.DetailsNotMatched:
            //        ModelState.AddModelError("NhsNumber", "Your details could not be found");
            //        return View("Index", model);
            //    case Utilities.SearchOutcomes.NotFound:
            //        break;
            //    default:
            //        break;
            //}

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri($"https://al-tech-test-apim.azure-api.net/tech-test/t2/patients/{model.NhsNumber}");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _SubscriptionKey);


            //    //client.DefaultRequestHeaders.Accept.Add(newMediaTypeWithQualityHeaderValue("application/json"));
            //    //GET Method
            //    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var json = await response.Content.ReadAsStringAsync();

            //        PatientModel patient = JsonConvert.DeserializeObject<PatientModel>(json);

            //        var searchResponse = model.EvaluateSearchResponse(patient);
            //        switch (searchResponse)
            //        {
            //            case Utilities.SearchOutcomes.EligiblePatientFound:
            //                //return this.RedirectToAction<SurveyController>(m => m.GetPatientAge());
            //                //return View("../Survey/Index");
            //                return RedirectToAction("Index", "Survey", new { patientAge = model.GetPatientAge() });//

            //            case Utilities.SearchOutcomes.UnderAge:

            //                ModelState.AddModelError("NhsNumber", "You are not eligible for this service, thank you for your interest");
            //                return View("Index",model);

            //            case Utilities.SearchOutcomes.DetailsNotMatched:

            //                //model.ErrorMessage = "Your details could not be found";
            //                ModelState.AddModelError("NhsNumber", "Your details could not be found");
            //                return View("Index", model);

            //            case Utilities.SearchOutcomes.NotFound:
            //                //This branch should never be hit if the api call returns 404 correctly
            //                break;
            //            default:

            //                break;
            //        }




            //        //{ "nhsNumber":"111222333","name":"DOE, John","born":"14-01-2007"}
            //        // Get the URI of the created resource.
            //        //UrireturnUrl = response.Headers.Location;
            //        //Console.WriteLine(returnUrl);

            //    }
            //    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            //    {
            //        Console.WriteLine("Patient not found");
            //        ModelState.AddModelError("NhsNumber", "Your details could not be found");
            //        return View("Index", model);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Internal server Error");
            //            return View("Index");
            //    }
            //}

            //return View("Index",model);
        }

       
        public async Task<PatientModel> GetPatientFromAPI(int nhsNumber)
        {
            //var client = new HttpClient();
            //client.BaseAddress = new Uri("https://al-tech-test-apim.azure-api.net/tech-test/t2/patients/{nhsNumber}");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _SubscriptionKey);
            //HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            //if (response.IsSuccessStatusCode)
            //{
            //    var json = await response.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<PatientModel>(json);
            //}
            //else
            //{
            //    return null;
            //}
            PatientModel patient;// = new PatientModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://al-tech-test-apim.azure-api.net/tech-test/t2/patients/{nhsNumber}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _SubscriptionKey);


                //client.DefaultRequestHeaders.Accept.Add(newMediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    //patient = JsonConvert.DeserializeObject<PatientModel>(json);
                    patient = JsonConvert.DeserializeObject<PatientModel>(json) ?? new PatientModel();
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Patient not found");
                    patient = new PatientModel();
                    ModelState.AddModelError("NhsNumber", "Your details could not be found");
                    //return View("Index", model);
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                    patient = new PatientModel();
                    //return View("Index");
                }
            }
            return patient;
        }

        public IActionResult GetViewBasedOnParameters(Utilities.SearchOutcomes searchOutcome, FormPatientModel model)//string viewName, object model = null)//, bool isError = false)
        {
            //if (isError)
            //{
            //    ModelState.AddModelError("Error", "An error occurred");
            //    return View("Error", model);
            //}

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
                    return View("Index", model);
                default:
                    return View("Index", model);
            }

            //switch (viewName)
            //{
            //    case "Index":
            //        return View("Index", model);
            //    case "Privacy":
            //        return View("Privacy", model);
            //    case "Details":
            //        return View("Details", model);
            //    default:
            //        return View("NotFound", model);
            //}
        }
    }
}
