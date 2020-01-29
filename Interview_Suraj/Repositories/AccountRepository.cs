using System;
using System.Collections.Generic;
using Interview_Suraj.Domain.Models;
using Interview_Suraj.Domain.Repositories;
using RestSharp;

namespace Interview_Suraj.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public IRestResponse GetAllAccounts(string token)
        {
            var client = new RestClient("https://api.truelayer-sandbox.com/data/v1/accounts");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);

            return response;
        }
    }
}
