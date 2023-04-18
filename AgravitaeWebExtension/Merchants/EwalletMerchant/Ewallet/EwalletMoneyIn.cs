using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using WebExtension.Merchants.EwalletMerchant.Models;
using DirectScale.Disco.Extension.MoneyIn.Custom.Models;

namespace WebExtension.Merchants.EwalletMerchant.Ewallet
{
    public class EwalletMoneyIn : SinglePaymentMoneyInMerchant
    {
        private readonly IAssociateService _associateService;
        private readonly IEwalletService _ewalletService;
        private readonly IOrderService _orderService;

        public EwalletMoneyIn(IEwalletService ewalletService, IAssociateService associateService, IOrderService orderService)
        {
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _ewalletService = ewalletService ?? throw new ArgumentNullException(nameof(ewalletService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public override Task<PaymentResponse> ChargePayment(string payerId, int orderNumber, double amount, Address billingAddress, string currencyCode)
        {
            try
            {
                _ewalletService.SaveErrorLogResponse(int.Parse(payerId), orderNumber, "Call Start for E-Wallet Charge Payment", "");
            }
            catch (Exception ex)
            {
                _ewalletService.SaveErrorLogResponse(0, 0, "Call Start for E-Wallet Charge Payment", "Exception" + ex.Message);

            }
            if (amount < 1)
            {
                throw new ArgumentException("Amount cannot be less than 1", nameof(amount));
            }

            var associate = _associateService.GetAssociate(int.Parse(payerId));

            if (associate == null)
            {
                throw new ArgumentNullException($"PayorID of {payerId} is invalid.");
            }

            var res = new PaymentResponse();
            res.TransactionNumber = Guid.NewGuid().ToString();
            res.Status = PaymentStatus.Pending;

            try
            {
                GetPointBalanceRequest balReq = new GetPointBalanceRequest { ExternalCustomerId = payerId };
                var balanceAmt = Convert.ToDouble(_ewalletService.GetPointBalance(balReq).Amount);
                var settings = _ewalletService.GetEwalletSettings();
                var order = _orderService.GetOrderByOrderNumber(orderNumber).Result;

                res.Amount = amount;
                res.Currency = currencyCode.ToUpper();

                if (order.OrderType == OrderType.Autoship)
                {
                    //if (Convert.ToDouble(balanceAmt) < paymentMethodInfo.Amount)
                    //{
                    //    if (settings.BackUpMerchantId > 0)
                    //    {
                    //        var backupPaymentMethod = GetBackupMerchantPaymentFromSettings(settings.BackUpMerchantId, Convert.ToInt32(payerId));
                    //        if (settings.SplitPayment && balanceAmt > 1) //split payment between Ewallet and backup if Ewallet has balance
                    //        {
                    //            _logger.LogInformation($"EwalletMerchant.ChargeSinglePayment", "Attempting to split payment");
                    //            var backupMerchantAmount = Math.Round(amount - balanceAmt, 2); //remainder of balance to be charged to backup merchant
                    //            amount = balanceAmt; //update amount to current Ewallet balance 

                    //            //attempt to charge backup first

                    //            var backupMerchantResponse = ChargeBackupPayment(backupPaymentMethod, Convert.ToInt32(payerId), orderNumber, backupMerchantAmount, backupPaymentMethod.CurrencyCode);

                    //            if (backupMerchantResponse.Status != PaymentStatus.Accepted)
                    //            {
                    //                return new PaymentResponse { Response = backupMerchantResponse.Response };
                    //            }

                    //            //commit the payment response to the DB
                    //            _logger.LogInformation($"EwalletMerchant.ChargeSinglePayment", $"Insert backup merchant payment trasaction number {backupMerchantResponse.TransactionNumber}");
                    //            _orderService.InsertOrderPayment(new PaymentResponse { Response = backupMerchantResponse.Response, ResponseId = backupMerchantResponse.ResponseId, Amount = backupMerchantResponse.Amount, AuthorizationCode = backupMerchantResponse.AuthorizationCode, Currency = backupMerchantResponse.Currency, Merchant = backupMerchantResponse.Merchant, OrderNumber = backupMerchantResponse.OrderNumber, PaymentType = backupMerchantResponse.PaymentType, Status = backupMerchantResponse.Status == PaymentStatus.Accepted ? PaymentStatus.Accepted : PaymentStatus.Pending });
                    //        }
                    //        else
                    //        {
                    //            var backUpresponse = ChargeBackupPayment(backupPaymentMethod, Convert.ToInt32(payerId), orderNumber, amount, backupPaymentMethod.CurrencyCode);
                    //            return new PaymentResponse { Response = backUpresponse.Response };
                    //        }
                    //    }
                    //}
                }
                else if (Convert.ToDouble(balanceAmt) < amount)
                {
                    res.Response = "Balance Amount Must be Equal or Higher then Order Amount";
                    res.ResponseId = "2";
                    res.Status = PaymentStatus.Rejected;
                    _ewalletService.SaveErrorLogResponse(Convert.ToInt32(payerId), orderNumber, "Balance Amount Must be Equal or Higher then Order Amount", "GotBalance Amount Must be Equal or Higher then Order Amount " + orderNumber + ". response: " + balanceAmt);
                    return Task.Run(() => { return (res); });
                }

                string comment = $"Amount:{amount} Debited From OrderNumber:{orderNumber}";
                CustomerPointTransactionsRequest data = new CustomerPointTransactionsRequest
                { Amount = (decimal)amount, ExternalCustomerID = payerId, RedeemType = RedeemType.Order, TransactionType = TransactionType.Debit, Comment = comment, ReferenceNo = orderNumber.ToString() };
                CreatePointAccountTransaction response = _ewalletService.CreatePointTransaction(data);
                if (!string.IsNullOrEmpty(response.Status))
                {
                    if (response.Status.Contains("Error") || response.Status.Contains("Failed"))
                    {
                        res.Response = response.Status;
                        res.TransactionNumber = "";
                        res.ResponseId = "2";
                        _ewalletService.SaveErrorLogResponse(Convert.ToInt32(payerId), orderNumber, "CreatePointTransaction Contains error", "Got Error when sending or processing Ewallet payment response for order " + orderNumber + ". response: " + response);
                    }
                    else
                    {
                        res.Status = PaymentStatus.Accepted;
                        res.TransactionNumber = response.TransactionNumber;
                        res.AuthorizationCode = response.TransactionNumber;

                    }
                }
            }
            catch (Exception e)
            {
                _ewalletService.SaveErrorLogResponse(Convert.ToInt32(payerId), orderNumber, "ChargeSavedPayment Exception", "Exception thrown when sending or processing Ewallet payment response for order " + orderNumber + ". Exception: " + e);
            }

            return Task.Run(() => { return (res); });
        }


        public override async Task<ExtendedPaymentResult> RefundPayment(string payerId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string referenceNumber, string transactionNumber, string authorizationCode)
        {
            _ewalletService.SaveErrorLogResponse(Convert.ToInt32(payerId), orderNumber, "Refund Amount", "Refund Order" + orderNumber + ". Refund Amount: " + refundAmount);
            var response = _ewalletService.CreditPayment(payerId, orderNumber, currencyCode, Convert.ToDecimal(paymentAmount), Convert.ToDecimal(refundAmount), "", transactionNumber, "refund");
            ExtendedPaymentResult paymentResult = new ExtendedPaymentResult
            {
                Amount = refundAmount,
                Currency = currencyCode,
                ResponseId = Guid.NewGuid().ToString(),
                Response = Guid.NewGuid().ToString(),
                TransactionNumber = transactionNumber,
                Status = response.Status,
                AuthorizationCode = Convert.ToString(orderNumber) + "" + "Refunded" + payerId
            };
            return await Task.FromResult(paymentResult);
        }
    }
}
