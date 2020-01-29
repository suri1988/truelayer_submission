using System;
namespace Interview_Suraj.Domain.Models
{
    public class AccountNumber
    {
        public string iban { get; set; }
        public string number { get; set; }
        public string sort_code { get; set; }
        public string swift_bic { get; set; }
    }
}
