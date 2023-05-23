﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.EwalletMerchant.Models
{
    public class CustomerDetails
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExternalCustomerID { get; set; }
        public string BackofficeID { get; set; }
        public string CompanyID { get; set; }
    }
}
