using System;
using System.Collections.Generic;
using Interview_Suraj.Domain.Models;
using Interview_Suraj.Domain.Responses;
using RestSharp;

namespace Interview_Suraj.Domain.Repositories
{
    public interface ITransactionRepository
    {
        IRestResponse GetAllTransactions(string accountId, string token);
        AccountTransaction AddAccountTransaction(AccountTransaction transaction);
        AccountTransaction GetAccountTransaction(string accountId, string transactionId);
        List<AccountTransaction> GetAccountTransactionsByAccounts(List<string> accountIds);
    }
}
