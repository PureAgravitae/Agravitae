using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.EwalletMerchant.Ewallet
{
    public class EwalletMoneyOutMerchant : CommissionMerchant
    {
        private const string account_number_key = "AccountId";
        private readonly IAssociateService _associateservice;
        private readonly IEwalletService _ewalletservice;
        private const string merchant_name = "EwalletMoneyOutMerchant";
        private readonly IMoneyOutService _moneyoutservice;
        private const int merchant_id = 9013;

        public EwalletMoneyOutMerchant(IAssociateService associateservice, IEwalletService ewalletservice, IMoneyOutService moneyoutservice)
        {
            _associateservice = associateservice ?? throw new ArgumentNullException(nameof(associateservice));
            _ewalletservice = ewalletservice ?? throw new ArgumentNullException(nameof(ewalletservice));
            _moneyoutservice = moneyoutservice ?? throw new ArgumentNullException(nameof(moneyoutservice));
        }

        public override async Task<CommissionPaymentResult[]> PayCommissions(int batchId, CommissionPayment[] payments)
        {
            _ewalletservice.SaveErrorLogResponse(batchId, 0, "Call Start for E-Wallet moneyout", "");
            var results = new List<CommissionPaymentResult>();
            var paymentStatus = new List<PaymentProcessStatus> { PaymentProcessStatus.Paid, PaymentProcessStatus.Batched };
            foreach (var payment in payments)
            {
                try
                {
                    if (paymentStatus.Contains(payment.PaymentStatus))
                    {
                        payment.MerchantCustomFields.TryGetValue(account_number_key, out string accountnumber);
                        if (string.IsNullOrEmpty(accountnumber))
                        {
                            // provision associate's ewallet account
                            accountnumber = await ProvisionNewAccount(payment.AssociateId, merchant_id, merchant_name);

                            if (string.IsNullOrEmpty(accountnumber)) throw new Exception($"Failed to provision account for {merchant_name}");
                        }
                        string comment = $"Commissions amount {Convert.ToDecimal(payment.Amount)} transfer";

                        var creditResult = _ewalletservice.CreditPayment(accountnumber, payment.Id, "usd", payment.Amount, payment.Amount, "", payment.PaymentUniqueId, payment.Notes);

                        if (creditResult.Status != PaymentStatus.Accepted)
                        {
                            _ewalletservice.SaveErrorLogResponse(Int32.Parse(accountnumber), Int32.Parse(accountnumber), "", "Error  from Ewallet in Save Commission Result : " + comment);
                            throw new Exception($"Failed to provision account for {merchant_name}");
                        }
                        results.Add(new CommissionPaymentResult()
                        {
                            PaymentUniqueId = payment.PaymentUniqueId,
                            TransactionNumber = payment.PaymentUniqueId,
                            Status = CommissionPaymentStatus.Paid,
                            DatePaid = DateTime.Now,
                            CheckNumber = 100,
                        });
                    }
                    else
                    {
                        _ewalletservice.SaveErrorLogResponse(batchId, payment.AssociateId, "Failed moneyout process account due to paymentStatus", payment.PaymentStatus.ToString());
                        results.Add(new CommissionPaymentResult()
                        {
                            PaymentUniqueId = payment.PaymentUniqueId,
                            Status = CommissionPaymentStatus.Failed,
                            ErrorMessage = $"exception occurred while paying {payment.AssociateId}"
                        });
                    }
                }
                catch (Exception e)
                {
                    _ewalletservice.SaveErrorLogResponse(batchId, payment.AssociateId, "Failed moneyout process account" + payment.PaymentStatus.ToString(), e.ToString());
                    results.Add(new CommissionPaymentResult()
                    {
                        PaymentUniqueId = payment.PaymentUniqueId,
                        Status = CommissionPaymentStatus.Failed,
                        ErrorMessage = $"exception occurred while paying {payment.AssociateId} : {e.Message}"
                    });
                }
            }
            return await Task.FromResult(results.ToArray());
        }

        public async override Task<Dictionary<string, string>> ProvisionAccount(int associateId)
        {
            var accountNumber = await ProvisionNewAccount(associateId, merchant_id, merchant_name);
            return new Dictionary<string, string> { { account_number_key, accountNumber } };
        }

        private async Task<string> ProvisionNewAccount(int associateId, int merchantId, string merchantName)
        {
            var onFileMerchants = await _moneyoutservice.GetOnFileMerchants(associateId);
            bool alreadyProvisioned = onFileMerchants.FirstOrDefault(x => x.MerchantId == merchantId)?.CustomValues?.ContainsKey(account_number_key) ?? false;

            string accountNumber = "";
            if (!alreadyProvisioned)
            {
                accountNumber = await CreateAssociateAccount(associateId);
                var accountInfo = new OnFileMerchant()
                {
                    AssociateId = associateId,
                    CustomValues = new Dictionary<string, string>()
                    {
                        { account_number_key, accountNumber }
                    },
                    MerchantId = merchantId,
                    MerchantName = merchantName
                };

                await _moneyoutservice.SetActiveOnFileMerchant(accountInfo);
            }

            return await Task.FromResult(accountNumber);
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
