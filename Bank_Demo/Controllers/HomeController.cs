using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bank_Demo.Models;
using Interview_Suraj.Domain.Repositories;
using RestSharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Interview_Suraj.Domain.Responses;
using Bank_Demo.Models.ViewModels;

namespace Bank_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IAuthenticationRepository authenticationRepository,
            IConfiguration configuration)
        {
            _logger = logger;
            _authenticationRepository = authenticationRepository;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var client = new RestClient("https://localhost:5001/api/Authentication/AuthUrl");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);

            return Redirect(response.ResponseUri.ToString());
        }


        public IActionResult Callback(string code)
        {
            var client = new RestClient("https://localhost:5001/api/Authentication/AccessToken?code=" + code);
            var request = new RestRequest(Method.GET);
            var response = client.Execute<AccessToken>(request);
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessToken>(response.Content);

            //retrieve accounts
            client = new RestClient("https://localhost:5001/api/Transactions?token="+accessTokenResponse.access_token);
            var accountRequest = new RestRequest(Method.GET);
            var accountResponse = client.Execute<List<AccountTransaction>>(accountRequest);

            client = new RestClient("https://localhost:5001/api/Transactions/UserMetrics?token="+accessTokenResponse.access_token);
            var metricRequest = new RestRequest(Method.GET);
            var metricResponse = client.Execute<List<UserTransactionMetrics>>(request);

            var viewModel = new AccountTransactionViewModel();
            viewModel.AccountTransactions = accountResponse.Data;
            viewModel.UserTransactionMetrics = metricResponse.Data;

            return View(viewModel);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
