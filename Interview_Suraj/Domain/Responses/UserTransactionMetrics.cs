using System;
namespace Interview_Suraj.Domain.Responses
{
    public class UserTransactionMetrics
    {
        public string TransactionCategory { get; set; }
        public double MinimumTransaction { get; set; }
        public double MaximumTransaction { get; set; }
        public double AverageTransaction { get; set; }
        public double Count { get; set; }
    }
}
