using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Interview_Suraj.Domain.Repositories;
using Interview_Suraj.Domain.Responses;
using Interview_Suraj.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview_Suraj.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationController(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        //returns the login url hosted by truelayer that the application redirects to
        [HttpGet("AuthUrl")]
        public ActionResult AuthUrl()
        {
            var response = _authenticationRepository.GetAuthenticationUrl();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return NotFound(new ApiException(response.StatusCode, response.ErrorMessage));
            }

            return Ok(response.ResponseUri.ToString());
        }

        [HttpGet("AccessToken")]
        public ActionResult AccessToken(string code)
        {
            var response = _authenticationRepository.GetAccessToken(code);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return NotFound(new ApiException(response.StatusCode, response.ErrorMessage));
            }
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessToken>(response.Content);

            return Ok(accessTokenResponse);
        }

        [HttpGet("RenewToken")]
        public ActionResult RenewToken(string refreshToken)
        {
            var response = _authenticationRepository.GetRefreshToken(refreshToken);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return NotFound(new ApiException(response.StatusCode, response.ErrorMessage));
            }
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessToken>(response.Content);

            return Ok(accessTokenResponse);
        }
    }
}
