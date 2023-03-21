﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExtension.Merchants.Models
{
    public class EwalletSettingsRequest
    {
        public string CompanyId { get; set; }
        public string PointAccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int BackUpMerchantId { get; set; }
        public bool SplitPayment { get; set; }
        public string ApiUrl { get; set; }
    }
}
