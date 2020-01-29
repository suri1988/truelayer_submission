using System;
using System.Collections.Generic;
using System.Linq;
using Interview_Suraj.Database;
using Interview_Suraj.Domain.Models;
using Interview_Suraj.Domain.Repositories;
using Interview_Suraj.Domain.Responses;
using RestSharp;

namespace Interview_Suraj.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly CustomDbContext _context;

        public TransactionRepository(CustomDbContext context)
        {
            _context = context;
        }

        public AccountTransaction AddAccountTransaction(AccountTransaction transaction)
        {
            _context.AccountTransactions.Add(transaction);
            _context.SaveChanges();
            return transaction;
        }

        public AccountTransaction GetAccountTransaction(string accountId, string transactionId)
        {
            return _context.AccountTransactions.ToList().FirstOrDefault(a => a.AccountId == accountId
            && a.TransactionId == transactionId);
        }

        public List<AccountTransaction> GetAccountTransactionsByAccounts(List<string> accountIds)
        {
            return _context.AccountTransactions.ToList().Where(a => accountIds.Contains(a.AccountId)).ToList();
        }

        public IRestResponse GetAllTransactions(string accountId, string token)
        {
            var client = new RestClient("https://api.truelayer-sandbox.com/data/v1/accounts/" + accountId + "/transactions");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);

            return response;
        }
    }
}
