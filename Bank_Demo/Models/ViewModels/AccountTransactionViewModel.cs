using System;
using System.Collections.Generic;
using Interview_Suraj.Domain.Responses;

namespace Bank_Demo.Models.ViewModels
{
    public class AccountTransactionViewModel
    {
        public IEnumerable<AccountTransaction> AccountTransactions {get;set;}
        public IEnumerable<UserTransactionMetrics> UserTransactionMetrics { get; set; }
    }
}
