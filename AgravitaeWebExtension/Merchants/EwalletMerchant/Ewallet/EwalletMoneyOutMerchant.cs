using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebExtension.Merchants.EwalletMerchant.Ewallet
{
    public class EwalletMoneyOutMerchant : CommissionMerchant
    {
        private const string account_number_key = "actnum";
        private readonly IAssociateService _associateservice;
        private readonly IEwalletService _ewalletservice;
        private const string merchant_name = "EwalletMoneyOutMerchant";

        public EwalletMoneyOutMerchant(IAssociateService associateservice, IEwalletService ewalletservice)
        {
            _associateservice = associateservice ?? throw new ArgumentNullException(nameof(associateservice));
            _ewalletservice = ewalletservice ?? throw new ArgumentNullException(nameof(ewalletservice));
        }

        public override async Task<CommissionPaymentResult[]> PayCommissions(int batchId, CommissionPayment[] payments)
        {
            var results = new List<CommissionPaymentResult>();

            foreach (var payment in payments)
            {
                try
                {
                    payment.MerchantCustomFields.TryGetValue(account_number_key, out string accountnumber);

                    // Added for successfully disco batch processing status 12-sep-2022 for 3260
                    if(payment.PaymentStatus== PaymentProcessStatus.Paid)  
                    {
                        var creditResult = _ewalletservice.CreditPayment(accountnumber, payment.Id, "usd", payment.Amount, payment.Amount, "", payment.PaymentUniqueId, payment.Notes);

                        if (creditResult.Status != PaymentStatus.Accepted) throw new Exception($"Failed to provision account for {merchant_name}");
                    }

                    results.Add(new CommissionPaymentResult()
                    {
                        PaymentUniqueId = payment.PaymentUniqueId, // this one is important that you manually set this equal to the payment you just processed
                        TransactionNumber = payment.PaymentUniqueId,
                        Status = CommissionPaymentStatus.Paid,
                        DatePaid = DateTime.Now,
                        CheckNumber = 100,
                    });
                }
                catch (Exception e)
                {
                    results.Add(new CommissionPaymentResult()
                    {
                        PaymentUniqueId = payment.PaymentUniqueId, // this one is important that you manually set this equal to the payment you just processed
                        Status = CommissionPaymentStatus.Failed,
                        ErrorMessage = $"exception occurred while paying {payment.AssociateId} : {e.Message}"
                    });
                }
            }

            return await Task.FromResult(results.ToArray());
        }

        public async override Task<Dictionary<string, string>> ProvisionAccount(int associateId)
        {
            var accountNumber = await CreateAssociateAccount(associateId);

            return new Dictionary<string, string> { { account_number_key, accountNumber } };
        }

        private async Task<string> CreateAssociateAccount(int associateId)
        {
            var associateinfo = _associateservice.GetAssociate(associateId).Result;
            var app = new Application
            {
                FirstName = associateinfo.DisplayFirstName,
                LastName = associateinfo.DisplayLastName,
                ExternalId = associateinfo.AssociateId.ToString(),
                BackOfficeId = associateinfo.BackOfficeId
            };
            var id = _ewalletservice.CreateCustomer(app, associateId);
            return await Task.FromResult(id);
        }
    }
}
