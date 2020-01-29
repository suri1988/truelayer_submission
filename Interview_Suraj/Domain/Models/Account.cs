using System;
namespace Interview_Suraj.Domain.Models
{
    public class Account
    {
        public DateTime update_timestamp { get; set; }
        public string account_id { get; set; }
        public string account_type { get; set; }
        public string display_name { get; set; }
        public string currency { get; set; }
        public AccountNumber account_number { get; set; }
        public Provider provider { get; set; }
    }
}
