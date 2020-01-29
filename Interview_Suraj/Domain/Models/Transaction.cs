using System;
using System.Collections.Generic;

namespace Interview_Suraj.Domain.Models
{
    public class Transaction
    {
        public string transaction_id { get; set; }
        public DateTime timestamp { get; set; }
        public string description { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string transaction_type { get; set; }
        public string transaction_category { get; set; }
        public List<string> transaction_classification { get; set; }
        public string merchant_name { get; set; }
        public RunningBalance running_balance { get; set; }
        public Meta meta { get; set; }
    }
}
