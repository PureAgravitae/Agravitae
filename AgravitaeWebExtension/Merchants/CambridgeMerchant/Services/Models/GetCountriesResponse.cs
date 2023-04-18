using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class GetCountriesResponse
    {
        public string countryName { get; set; }
        public string country { get; set; }
        public string defaultCurrency { get; set; }
        public string error { get; set; }
    }
}
