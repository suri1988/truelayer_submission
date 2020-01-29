using System;
using System.Collections.Generic;

namespace Interview_Suraj.Domain.Models
{
    public class TransactionRootObject
    {
        public List<Transaction> results { get; set; }
        public string status { get; set; }
    }
}
