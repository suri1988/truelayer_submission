using System;
using Interview_Suraj.Domain.Responses;
using RestSharp;

namespace Interview_Suraj.Domain.Repositories
{
    public interface IAuthenticationRepository
    {
        IRestResponse GetAuthenticationUrl();
        IRestResponse GetAccessToken(string code);
        IRestResponse GetRefreshToken(string refreshToken);
    }
}
