using System;
namespace Interview_Suraj.Domain.Responses
{
    public class AccountTransaction
    {
       public Guid Id { get; set; }         
       public string AccountId { get; set; }
       public string TransactionId { get; set; }
       public string Category { get; set; }
       public double Amount { get; set; }
    }
}
