﻿using System;
namespace Interview_Suraj.Domain.Responses
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
    }
}
