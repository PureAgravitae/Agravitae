using DirectScale.Disco.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExtension.Merchants.Models
{
    public class GetCheckInfoResponse
    {
        public GetCheckInfoResponse()
        {
            CheckInfo = new List<GetCheckInfoResponseData>();
            Company = new Company();
        }

        public List<GetCheckInfoResponseData> CheckInfo { get; set; }
        public Company Company { get; set; }
    }

    public class GetCheckInfoResponseData
    {
        public string AssociateName { get; set; }
        public Address AssociateAddress { get; set; }
        public int AssociateId { get; set; }
        string PrimaryPhone { get; set; }
        string SecondaryPhone { get; set; }
        public string EmailAddress { get; set; }
        public DateTime SignupDate { get; set; }
        public int AssociateType { get; set; }
        public string BackOfficeId { get; set; }
        public DateTime BirthDate { get; set; }

    }
}
