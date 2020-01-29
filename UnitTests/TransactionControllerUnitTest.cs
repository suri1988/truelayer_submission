using System.Collections.Generic;
using Interview_Suraj.Controllers;
using Interview_Suraj.Domain.Models;
using Interview_Suraj.Domain.Repositories;
using Interview_Suraj.Domain.Responses;
using Interview_Suraj.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Tests
{
    [TestFixture]
    public class TransactionControllerUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetTransactions_Unauthorized_ThrowsException()
        {
            var accountRepo = new Mock<IAccountRepository>();
            var transactionRepo = new Mock<ITransactionRepository>();

            accountRepo.Setup(a => a.GetAllAccounts(string.Empty)).Throws(new ApiException(System.Net.HttpStatusCode.Unauthorized,
                "Unauthorized"));

            var controller = new TransactionController(accountRepo.Object, transactionRepo.Object);

            Assert.Throws<ApiException>(() => controller.Get(string.Empty));
        }

        [Test]
        public void TestGetTransactions_RecordExistsInDb_DoesNotGetAdded()
        {
            var accountRepo = new Mock<IAccountRepository>();
            var transactionRepo = new Mock<ITransactionRepository>();
            var fakeToken = "fake";
            var fakeAccountId = "123";
            var fakeTransactionId = "456";
            var fakeAccount = new Account { account_id = fakeAccountId };
            var fakeTransaction = new Transaction { transaction_id = fakeTransactionId };
            var fakeAccountResponse = new AccountRootObject { results = new List<Account> { fakeAccount } };
            var fakeTransactionResponse = new TransactionRootObject { results = new List<Transaction> { fakeTransaction } };
            var fakeAccountTransaction = new AccountTransaction
            {
                Id = new System.Guid(),
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId
            };

            var mockResponse = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fakeAccountResponse)
            };

            var mockTransactionResponse = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fakeTransactionResponse)
            };

            accountRepo.Setup(a => a.GetAllAccounts(fakeToken)).Returns(mockResponse);
            transactionRepo.Setup(a => a.GetAllTransactions(fakeAccountId, fakeToken)).Returns(mockTransactionResponse);
            transactionRepo.Setup(a => a.GetAccountTransaction(fakeAccountId, fakeTransactionId)).Returns(fakeAccountTransaction);

            var controller = new TransactionController(accountRepo.Object, transactionRepo.Object);
            controller.Get(fakeToken);
            transactionRepo.Verify(a => a.AddAccountTransaction(It.IsAny<AccountTransaction>()), Times.Never);
        }

        [Test]
        public void TestGetUserMetrics_MetricsForSingleTransactionSameAsTransaction()
        {
            var accountRepo = new Mock<IAccountRepository>();
            var transactionRepo = new Mock<ITransactionRepository>();
            var fakeToken = "fake";
            var fakeAccountId = "123";
            var fakeTransactionId = "456";
            var fakeCategory = "cat1";
            var fakeAccount = new Account { account_id = fakeAccountId };
            var fakeAccountResponse = new AccountRootObject { results = new List<Account> { fakeAccount } };
            var accountTransactionObject = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId,
                Amount = 10,
                Category = fakeCategory
            };
            var listOfAccountTransactions = new List<AccountTransaction> { accountTransactionObject };

            var mockResponse = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fakeAccountResponse)
            };

            accountRepo.Setup(a => a.GetAllAccounts(fakeToken)).Returns(mockResponse);
            transactionRepo.Setup(t => t.GetAccountTransactionsByAccounts(new List<string> { fakeAccountId })).Returns(listOfAccountTransactions);

            var controller = new TransactionController(accountRepo.Object, transactionRepo.Object);
            var result = controller.UserMetrics(fakeToken);
            var metricsList = result.Result as OkObjectResult;
            var final = (List<UserTransactionMetrics>)metricsList.Value;

            Assert.AreEqual(1, final.Count);
            Assert.AreEqual(10, final[0].MaximumTransaction);
            Assert.AreEqual(10, final[0].MinimumTransaction);
            Assert.AreEqual(10, final[0].AverageTransaction);

        }

        [Test]
        public void TestGetUserMetrics_CalculatesAverageCorrectlyForSingleCategory()
        {
            var accountRepo = new Mock<IAccountRepository>();
            var transactionRepo = new Mock<ITransactionRepository>();
            var fakeToken = "fake";
            var fakeAccountId = "123";
            var fakeTransactionId = "456";
            var fakeCategory = "cat1";
            var fakeAccount = new Account { account_id = fakeAccountId };
            var fakeAccountResponse = new AccountRootObject { results = new List<Account> { fakeAccount } };
            var accountTransactionObject = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId,
                Amount = 10,
                Category = fakeCategory
            };

            var accountTransactionObject2 = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId,
                Amount = 20,
                Category = fakeCategory
            };

            var listOfAccountTransactions = new List<AccountTransaction>();
            listOfAccountTransactions.Add(accountTransactionObject);
            listOfAccountTransactions.Add(accountTransactionObject2);

            var mockResponse = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fakeAccountResponse)
            };

            accountRepo.Setup(a => a.GetAllAccounts(fakeToken)).Returns(mockResponse);
            transactionRepo.Setup(t => t.GetAccountTransactionsByAccounts(new List<string> { fakeAccountId })).Returns(listOfAccountTransactions);

            var controller = new TransactionController(accountRepo.Object, transactionRepo.Object);
            var result = controller.UserMetrics(fakeToken);
            var metricsList = result.Result as OkObjectResult;
            var final = (List<UserTransactionMetrics>)metricsList.Value;

            Assert.AreEqual(1, final.Count);
            Assert.AreEqual(20, final[0].MaximumTransaction);
            Assert.AreEqual(10, final[0].MinimumTransaction);
            Assert.AreEqual(15, final[0].AverageTransaction);

        }

        [Test]
        public void TestGetUserMetrics_CalculatesAverageCorrectlyForMultipleCategories()
        {
            var accountRepo = new Mock<IAccountRepository>();
            var transactionRepo = new Mock<ITransactionRepository>();
            var fakeToken = "fake";
            var fakeAccountId = "123";
            var fakeTransactionId = "456";
            var fakeCategory = "cat1";
            var fakeCategory2 = "cat2";
            var fakeAccount = new Account { account_id = fakeAccountId };
            var fakeAccountResponse = new AccountRootObject { results = new List<Account> { fakeAccount } };
            var accountTransactionObject = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId,
                Amount = 10,
                Category = fakeCategory
            };

            var accountTransactionObject2 = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId,
                Amount = 20,
                Category = fakeCategory2
            };

            var listOfAccountTransactions = new List<AccountTransaction>();
            listOfAccountTransactions.Add(accountTransactionObject);
            listOfAccountTransactions.Add(accountTransactionObject2);

            var mockResponse = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fakeAccountResponse)
            };

            accountRepo.Setup(a => a.GetAllAccounts(fakeToken)).Returns(mockResponse);
            transactionRepo.Setup(t => t.GetAccountTransactionsByAccounts(new List<string> { fakeAccountId })).Returns(listOfAccountTransactions);

            var controller = new TransactionController(accountRepo.Object, transactionRepo.Object);
            var result = controller.UserMetrics(fakeToken);
            var metricsList = result.Result as OkObjectResult;
            var final = (List<UserTransactionMetrics>)metricsList.Value;

            Assert.AreEqual(2, final.Count);
            Assert.AreEqual(10, final[0].MaximumTransaction);
            Assert.AreEqual(10, final[0].MinimumTransaction);
            Assert.AreEqual(10, final[0].AverageTransaction);

            Assert.AreEqual(20, final[1].MaximumTransaction);
            Assert.AreEqual(20, final[1].MinimumTransaction);
            Assert.AreEqual(20, final[1].AverageTransaction);
        }

        [Test]
        public void TestGetUserMetrics_CalculatesAverageCorrectlyForMultipleCategoriesAndMultipleTransactions()
        {
            var accountRepo = new Mock<IAccountRepository>();
            var transactionRepo = new Mock<ITransactionRepository>();
            var fakeToken = "fake";
            var fakeAccountId = "123";
            var fakeTransactionId = "456";
            var fakeTransactionId2 = "789";
            var fakeTransactionId3 = "111";
            var fakeTransactionId4 = "222";
            var fakeCategory = "cat1";
            var fakeCategory2 = "cat2";
            var fakeAccount = new Account { account_id = fakeAccountId };
            var fakeAccountResponse = new AccountRootObject { results = new List<Account> { fakeAccount } };
            var accountTransactionObject = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId,
                Amount = 10,
                Category = fakeCategory
            };

            var accountTransactionObject2 = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId2,
                Amount = 5,
                Category = fakeCategory
            };

            var accountTransactionObject3 = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId3,
                Amount = 20,
                Category = fakeCategory2
            };

            var accountTransactionObject4 = new AccountTransaction
            {
                AccountId = fakeAccountId,
                TransactionId = fakeTransactionId4,
                Amount = 25,
                Category = fakeCategory2
            };

            var listOfAccountTransactions = new List<AccountTransaction>();
            listOfAccountTransactions.Add(accountTransactionObject);
            listOfAccountTransactions.Add(accountTransactionObject2);
            listOfAccountTransactions.Add(accountTransactionObject3);
            listOfAccountTransactions.Add(accountTransactionObject4);


            var mockResponse = new RestResponse
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fakeAccountResponse)
            };

            accountRepo.Setup(a => a.GetAllAccounts(fakeToken)).Returns(mockResponse);
            transactionRepo.Setup(t => t.GetAccountTransactionsByAccounts(new List<string> { fakeAccountId })).Returns(listOfAccountTransactions);

            var controller = new TransactionController(accountRepo.Object, transactionRepo.Object);
            var result = controller.UserMetrics(fakeToken);
            var metricsList = result.Result as OkObjectResult;
            var final = (List<UserTransactionMetrics>)metricsList.Value;

            Assert.AreEqual(2, final.Count);
            Assert.AreEqual(10, final[0].MaximumTransaction);
            Assert.AreEqual(5, final[0].MinimumTransaction);
            Assert.AreEqual(7.5, final[0].AverageTransaction);

            Assert.AreEqual(25, final[1].MaximumTransaction);
            Assert.AreEqual(20, final[1].MinimumTransaction);
            Assert.AreEqual(22.5, final[1].AverageTransaction);
        }
    }
}