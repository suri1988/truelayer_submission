using System;
using System.Collections.Generic;
using Interview_Suraj.Domain.Models;
using RestSharp;

namespace Interview_Suraj.Domain.Repositories
{
    public interface IAccountRepository
    {
        IRestResponse GetAllAccounts(string token);
    }
}
