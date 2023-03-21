using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class GetBenefisaryRulesResponse
    {
        public string Id { get; set; }
        public string RegEx { get; set; }
        public string ErrorMessage { get; set; }
        public string IsRequired { get; set; }
        public string validationRules { get; set; }
        public string valueSet { get; set; }
        public string isRequiredInValueSet { get; set; }
        public string defaultValue { get; set; }
        public string error { get; set; }
    }
    public class GetBeneRulesResponse
    {
        public List<GetBenefisaryRulesResponse> Rules { get; set; }
        public List<GetBenefisaryRulesResponse> RegulatoryRules { get; set; }
        public string error { get; set; }
    }
}
