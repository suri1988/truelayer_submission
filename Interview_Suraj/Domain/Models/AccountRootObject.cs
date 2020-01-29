using System;
using System.Collections.Generic;

namespace Interview_Suraj.Domain.Models
{
    public class AccountRootObject
    {
        public List<Account> results { get; set; }
        public string status { get; set; }
    }
}
