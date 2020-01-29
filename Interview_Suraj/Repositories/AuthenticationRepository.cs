using System;
using Interview_Suraj.Domain.Repositories;
using Interview_Suraj.Domain.Responses;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Interview_Suraj.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IConfiguration _configuration;

        public AuthenticationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IRestResponse GetAccessToken(string code)
        {
            var client = new RestClient("https://auth.truelayer-sandbox.com/connect/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", _configuration["clientid"]);
            request.AddParameter("client_secret", _configuration["clientsecret"]);
            request.AddParameter("redirect_uri", "https://localhost:5003/Home/Callback");
            request.AddParameter("code", code);
            var response = client.Execute(request);

            return response;
        }

        public IRestResponse GetAuthenticationUrl()
        {
            var client = new RestClient("https://auth.truelayer-sandbox.com/?response_type=code&client_id="+ _configuration["clientid"] + "&scope=info%20accounts%20balance%20cards%20transactions%20direct_debits%20standing_orders%20offline_access&redirect_uri=https://localhost:5003/Home/Callback&providers=uk-ob-all%20uk-oauth-all%20uk-cs-mock");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute(request);

            return response;
        }

        public IRestResponse GetRefreshToken(string refreshToken)
        {
            var client = new RestClient("https://auth.truelayer-sandbox.com/connect/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", _configuration["clientid"]);
            request.AddParameter("client_secret", _configuration["clientsecret"]);
            request.AddParameter("refresh_token", refreshToken);
            var response = client.Execute(request);

            return response;
        }
    }
}
