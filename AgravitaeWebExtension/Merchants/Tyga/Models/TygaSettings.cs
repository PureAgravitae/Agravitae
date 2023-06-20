using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgravitaeExtension.Merchants.Tyga.Models
{

    public class TygaSettings
    {
        public string ApiBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string IntegrationCode { get; set; }
        public bool IsActive { get; set; }
        public double IsLive { get; set; }
        public string NotifyUrl { get; set; }
        public string ReturnUrl { get; set; }
    }

}
