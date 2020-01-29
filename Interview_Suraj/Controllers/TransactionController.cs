using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Interview_Suraj.Database;
using Interview_Suraj.Domain.Models;
using Interview_Suraj.Domain.Repositories;
using Interview_Suraj.Domain.Responses;
using Interview_Suraj.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview_Suraj.Controllers
{
    [Route("api/Transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public ActionResult<List<AccountTransaction>> Get(string token)
        {
            var response = _accountRepository.GetAllAccounts(token);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return NotFound(new ApiException(response.StatusCode, response.ErrorMessage));
            }

            var accounts = JsonConvert.DeserializeObject<AccountRootObject>(response.Content);
            var accountTransactions = new List<AccountTransaction>();
            foreach (var account in accounts.results)
            {
                var transactionResponse = _transactionRepository.GetAllTransactions(account.account_id, token);
                if (transactionResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return NotFound(new ApiException(transactionResponse.StatusCode, transactionResponse.ErrorMessage));
                }

                var transactions = JsonConvert.DeserializeObject<TransactionRootObject>(transactionResponse.Content);
                foreach (var transaction in transactions.results)
                {
                    var accountTransaction = new AccountTransaction
                    {
                        AccountId = account.account_id,
                        Amount = transaction.amount,
                        Category = transaction.transaction_category,
                        TransactionId = transaction.transaction_id
                    };

                    var existingTransaction = _transactionRepository.GetAccountTransaction(account.account_id, transaction.transaction_id);
                    //if transaction does not exist, then add to db
                    if (existingTransaction == null)
                    {
                        existingTransaction = _transactionRepository.AddAccountTransaction(accountTransaction);
                    }

                    accountTransactions.Add(existingTransaction);

                }
            }

            return Ok(accountTransactions);
        }

        [HttpGet("UserMetrics")]
        public ActionResult<List<UserTransactionMetrics>> UserMetrics(string token)
        {
            var response = _accountRepository.GetAllAccounts(token);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return NotFound(new ApiException(response.StatusCode, response.ErrorMessage));
            }

            var accounts = JsonConvert.DeserializeObject<AccountRootObject>(response.Content);
            var accountIds = accounts.results.Select(a => a.account_id).ToList();

            var userMetrics = new List<UserTransactionMetrics>();
            var totalTransactions = _transactionRepository.GetAccountTransactionsByAccounts(accountIds);
            foreach (var transaction in totalTransactions)
            {
                var existingMetric = userMetrics.FirstOrDefault(a => a.TransactionCategory == transaction.Category);
                if ( existingMetric == null)
                {
                    userMetrics.Add(new UserTransactionMetrics
                    {
                        TransactionCategory = transaction.Category,
                        AverageTransaction = transaction.Amount,
                        MaximumTransaction = transaction.Amount,
                        MinimumTransaction = transaction.Amount,
                        Count = 1
                    });
                }
                else
                {
                    var count = existingMetric.Count;
                    var newAvg = ((existingMetric.AverageTransaction * count) + transaction.Amount) / (count + 1);
                    existingMetric.AverageTransaction = newAvg;
                    if (existingMetric.MinimumTransaction > transaction.Amount)
                    {
                        existingMetric.MinimumTransaction = transaction.Amount;
                    }
                    if (existingMetric.MaximumTransaction < transaction.Amount)
                    {
                        existingMetric.MaximumTransaction = transaction.Amount;
                    }
                    existingMetric.Count += 1;
                }
            }

            return Ok(userMetrics);
        }
    }
}
