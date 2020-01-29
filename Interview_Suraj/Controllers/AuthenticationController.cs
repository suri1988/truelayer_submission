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

        /// <summary>
        /// Returns the true layer sandbox login auth url, which the user has to authorize
        /// </summary>
        /// <returns></returns>
        [HttpGet("AuthUrl")]
        public ActionResult AuthUrl()
        {
            var response = _authenticationRepository.GetAuthenticationUrl();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return NotFound(new ApiException(response.StatusCode, response.ErrorMessage));
            }

            return Redirect(response.ResponseUri.ToString());
        }

        /// <summary>
        /// Returns the access token and refresh token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a refresh token, which will replace the old refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
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
