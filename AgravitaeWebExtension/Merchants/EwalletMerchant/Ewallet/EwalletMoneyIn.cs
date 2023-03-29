using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Microsoft.Extensions.Logging;
using WebExtension.Merchants.EwalletMerchant.Models;
using System;
using System.Text;
using System.Threading.Tasks;
using DirectScale.Disco.Extension.MoneyIn.Custom.Models;

namespace WebExtension.Merchants.EwalletMerchant.Ewallet
{
    public class EwalletMoneyIn : SavedPaymentMoneyInMerchant
    {
        private readonly IAssociateService _associateService;
        private readonly IClientService _clientService;
        private readonly IEwalletService _ewalletService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;

        public EwalletMoneyIn(IEwalletService ewalletService, IClientService clientService, IAssociateService associateService, ILogger logger, IOrderService orderService)
        {
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _ewalletService = ewalletService ?? throw new ArgumentNullException(nameof(ewalletService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public override Task<ExtendedPaymentResult> ChargePayment(string payerId, Payment payment, double amount, string currencyCode, int orderNumber)
        {
            if (amount < 1)
            {
                throw new ArgumentException("Amount cannot be less than 1", nameof(amount));
            }

            var associate = _associateService.GetAssociate(int.Parse(payerId));

            if (associate == null)
            {
                throw new ArgumentNullException($"PayorID of {payerId} is invalid.");
            }

            var res = new ExtendedPaymentResult();
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
                //res.Merchant = payment.MerchantId;
                //res.Redirect = false;
                //res.OrderNumber = orderNumber;
                //res.PaymentType = "Authorization";


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
                    return Task.Run(() => { return (res); });
                }

                CustomerPointTransactionsRequest data = new CustomerPointTransactionsRequest
                { Amount = (decimal)amount, ExternalCustomerID = payerId, RedeemType = RedeemType.Order, TransactionType = TransactionType.Debit };
                string response = _ewalletService.CreatePointTransaction(data);
                if (!string.IsNullOrEmpty(response))
                {
                    if (response.Contains("Error"))
                    {
                        res.Response = response;
                        res.TransactionNumber = "";
                        res.ResponseId = "2";
                        _logger.LogError("ChargeSavedPayment", "Gor Error when sending or processing Ewallet payment response for order " + orderNumber + ". response: " + response);
                    }
                    else
                    {
                        res.Status = PaymentStatus.Accepted;
                        res.TransactionNumber = response;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ChargeSavedPayment", "Exception thrown when sending or processing Ewallet payment response for order " + orderNumber + ". Exception: " + e);
            }

            return Task.Run(() => { return (res); });
        }

        public async override Task<AddPaymentFrameData> GetSavePaymentFrame(string payorId, int? associateId, string languageCode, string countryCode, Region region)
        {
            var response = new AddPaymentFrameData
            {
                IFrameURL = "https://agravitae.clientextension.directscalestage.com/PaymentMethod",
                IFrameHeight = 500,
                IFrameWidth = 400
            };

            return await Task.FromResult(response);


            //var head = new StringBuilder();

            //var body = new StringBuilder();
            //var apirequest = new GetPointBalanceApiRequest { CustomerId = associateId.ToString() };
            ////head.AppendLine("<link href='~/Styles/iframe.css' rel='stylesheet' />");
            //head.AppendLine("<style >");
            //head.AppendLine("    .msg {");
            //head.AppendLine("        font-style: italic;");
            //head.AppendLine("        font-weight: bold;");
            //head.AppendLine("        color: #999999;");
            //head.AppendLine("    }");
            //head.AppendLine("    .mail {");
            //head.AppendLine("        font-weight: bold;");
            //head.AppendLine("        color: #169BD7;");
            //head.AppendLine("    }");
            //head.AppendLine("</style>");
            ////head.AppendLine("<script src='~/Scripts/addcardframe.js'></script>");
            ////head.AppendLine("<link href='/Styles/bootstrapCS.css' rel='stylesheet' />");
            //head.AppendLine("<script src='https://code.jquery.com/jquery-3.4.1.min.js'></script>");
            //head.AppendLine("<script>");
            //head.AppendLine("    $(document).ready(function () {");
            //head.AppendLine("        var url = window.location.href;");
            //head.AppendLine("        var arr = url.split('/');");
            //head.AppendLine("        var apiurl = arr[0] + '/Command/ClientAPI/Merchants/Ewallet/GetPointBalance';");

            //head.AppendLine("            var jsonObjects =  {");
            //head.AppendLine($"                'CustomerId': '{associateId}'");
            //head.AppendLine("            };");

            //head.AppendLine("        var settings = {");
            //head.AppendLine("            'async': true,");
            //head.AppendLine("            'crossDomain': true,");
            //head.AppendLine("            'url': apiurl,");
            //head.AppendLine("            'method': 'POST',");
            //head.AppendLine("            'headers': {");
            //head.AppendLine("                'content-type': 'application/json; charset=UTF-8',");
            //head.AppendLine("                'Accept': 'application/json',");
            //head.AppendLine("                'dataType': 'json',");
            //head.AppendLine("                'Cache-Control': 'no-cache'");
            //head.AppendLine("            },");
            //head.AppendLine("            'data': JSON.stringify(jsonObjects)");
            //head.AppendLine("        }");
            //head.AppendLine("        $.ajax(settings).done(function (r) {");
            //head.AppendLine("            if (r.Status === 0) {");
            //head.AppendLine("                if (r.Data) {");
            //head.AppendLine("                    if (r.Status === 0) {");
            //head.AppendLine("                        $('#ewalletbal').text(r.Data.Amount);");
            //head.AppendLine("                    }");
            //head.AppendLine("                    else {");
            //head.AppendLine("                        alert(r.Message);");
            //head.AppendLine("                    }");
            //head.AppendLine("                }");
            //head.AppendLine("            }");
            //head.AppendLine("        });");
            //head.AppendLine("    });");
            //head.AppendLine("</script>");
            //head.AppendLine("<script>");
            //head.AppendLine("    function SavePayment() {");
            //head.AppendLine("        var data = {};");
            //head.AppendLine("        data.card_ref = guid();");
            //head.AppendLine("        data.expireMonth = 1;");
            //head.AppendLine("        data.type = 'E-Wallet';");
            //head.AppendLine("        data.token = 'test';");
            //head.AppendLine("        DS_SavePaymentMethod(data);");
            //head.AppendLine("    }");

            //head.AppendLine("function guid() {");
            //head.AppendLine("function s4() {");
            //head.AppendLine("    return Math.floor((1 + Math.random()) * 0x10000)");
            //head.AppendLine("        .toString(16)");
            //head.AppendLine("        .substring(1);");
            //head.AppendLine("}");
            //head.AppendLine("return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();");
            //head.AppendLine("}");

            //head.AppendLine("</script>");
            //body.AppendLine("<p class='text-center msg' id='ewalletmsg'>Your E-Wallet Balance is :</p>");
            //body.AppendLine("<p class='text-center mail' id='ewalletbal'></p>");
            //body.AppendLine("<div class='row' style='margin-top:25px;'>");
            //body.AppendLine("    <div class='col-xs-12 text-center'>");
            //body.AppendLine("        <button type='button' class='btn btn-primary' onclick='SavePayment();'>Save E-Wallet Account</button>");
            //body.AppendLine("    </div>");
            //body.AppendLine("</div>");

            //var res = new AddPaymentFrameData
            //{
            //    Head = head.ToString(),
            //    Body = body.ToString()
            //};

            //return res;
            throw new NotImplementedException();
        }

        public override Task<ExtendedPaymentResult> RefundPayment(string payerId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string referenceNumber, string transactionNumber, string authorizationCode)
        {
            throw new NotImplementedException();
        }
    }

    //public override PaymentResponse ChargePayment(string payerId, NewPayment paymentMethodInfo, int orderNumber)
    //{
    //    if (string.IsNullOrWhiteSpace(paymentMethodInfo.PaymentMethodId))
    //    {
    //        throw new ArgumentNullException(paymentMethodInfo.PaymentMethodId);
    //    }

    //    if (paymentMethodInfo.Amount < 1)
    //    {
    //        throw new ArgumentException("Amount cannot be less than 1", nameof(paymentMethodInfo.Amount));
    //    }

    //    var associate = _associateService.GetAssociate(int.Parse(payerId));

    //    if (associate == null)
    //    {
    //        throw new ArgumentNullException($"PayorID of {payerId} is invalid.");
    //    }

    //    var res = new PaymentResponse();
    //    res.TransactionNumber = Guid.NewGuid().ToString();
    //    res.Status = PaymentStatus.Pending;

    //    try
    //    {
    //        GetPointBalanceRequest balReq = new GetPointBalanceRequest { ExternalCustomerId = payerId };
    //        var balanceAmt = Convert.ToDouble(_ewalletService.GetPointBalance(balReq).Amount);
    //        var settings = _ewalletRepository.GetSettings();
    //        var amount = Convert.ToDecimal(paymentMethodInfo.Amount);
    //        var order = _orderService.GetOrderByOrderNumber(orderNumber);

    //        res.Amount = paymentMethodInfo.Amount;
    //        res.OrderNumber = orderNumber;
    //        res.PaymentType = "Authorization";
    //        res.Currency = paymentMethodInfo.CurrencyCode.ToUpper();
    //        res.Merchant = MerchantInfo.Id;
    //        res.Redirect = false;


    //        if (order.OrderType == OrderType.Autoship)
    //        {
    //            //if (Convert.ToDouble(balanceAmt) < paymentMethodInfo.Amount)
    //            //{
    //            //    if (settings.BackUpMerchantId > 0)
    //            //    {
    //            //        var backupPaymentMethod = GetBackupMerchantPaymentFromSettings(settings.BackUpMerchantId, Convert.ToInt32(payerId));
    //            //        if (settings.SplitPayment && balanceAmt > 1) //split payment between Ewallet and backup if Ewallet has balance
    //            //        {
    //            //            _logger.LogInformation($"EwalletMerchant.ChargeSinglePayment", "Attempting to split payment");
    //            //            var backupMerchantAmount = Math.Round(amount - balanceAmt, 2); //remainder of balance to be charged to backup merchant
    //            //            amount = balanceAmt; //update amount to current Ewallet balance 

    //            //            //attempt to charge backup first

    //            //            var backupMerchantResponse = ChargeBackupPayment(backupPaymentMethod, Convert.ToInt32(payerId), orderNumber, backupMerchantAmount, backupPaymentMethod.CurrencyCode);

    //            //            if (backupMerchantResponse.Status != PaymentStatus.Accepted)
    //            //            {
    //            //                return new PaymentResponse { Response = backupMerchantResponse.Response };
    //            //            }

    //            //            //commit the payment response to the DB
    //            //            _logger.LogInformation($"EwalletMerchant.ChargeSinglePayment", $"Insert backup merchant payment trasaction number {backupMerchantResponse.TransactionNumber}");
    //            //            _orderService.InsertOrderPayment(new PaymentResponse { Response = backupMerchantResponse.Response, ResponseId = backupMerchantResponse.ResponseId, Amount = backupMerchantResponse.Amount, AuthorizationCode = backupMerchantResponse.AuthorizationCode, Currency = backupMerchantResponse.Currency, Merchant = backupMerchantResponse.Merchant, OrderNumber = backupMerchantResponse.OrderNumber, PaymentType = backupMerchantResponse.PaymentType, Status = backupMerchantResponse.Status == PaymentStatus.Accepted ? PaymentStatus.Accepted : PaymentStatus.Pending });
    //            //        }
    //            //        else
    //            //        {
    //            //            var backUpresponse = ChargeBackupPayment(backupPaymentMethod, Convert.ToInt32(payerId), orderNumber, amount, backupPaymentMethod.CurrencyCode);
    //            //            return new PaymentResponse { Response = backUpresponse.Response };
    //            //        }
    //            //    }
    //            //}
    //        }
    //        else if (Convert.ToDouble(balanceAmt) < paymentMethodInfo.Amount)
    //        {
    //            res.Response = "Balance Amount Must be Equal or Higher then Order Amount";
    //            res.ResponseId = "2";
    //            res.Status = PaymentStatus.Rejected;
    //            return res;
    //        }

    //        CustomerPointTransactionsRequest data = new CustomerPointTransactionsRequest
    //        { Amount = amount, ExternalCustomerID = payerId, RedeemType = RedeemType.Order, TransactionType = TransactionType.Debit };
    //        string response = _ewalletService.CreatePointTransaction(data);
    //        if (!string.IsNullOrEmpty(response))
    //        {
    //            if (response.Contains("Error"))
    //            {
    //                res.Response = response;
    //                res.TransactionNumber = "";
    //                res.ResponseId = "2";
    //                _logger.LogError("ChargeSavedPayment", "Gor Error when sending or processing Ewallet payment response for order " + orderNumber + ". response: " + response);
    //            }
    //            else
    //            {
    //                res.Status = PaymentStatus.Accepted;
    //                res.TransactionNumber = response;
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError($"ChargeSavedPayment", "Exception thrown when sending or processing Ewallet payment response for order " + orderNumber + ". Exception: " + e);
    //    }


    //    return res;
    //}

    //public override void DeletePayment(string payerId, string paymentMethodId)
    //{
    //    base.DeletePayment(payerId, paymentMethodId);
    //}

    //public override PaymentResponse RefundPayment(string payerId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string referenceNumber, string transactionNumber, string authorizationCode)
    //{
    //    throw new NotImplementedException();
    //}

    //public override SavePaymentForm GetSavePaymentForm(string payerId, int associateId, string oneTimeAuthToken, string languageCode, string countryCode)
    //{
    //    var head = new StringBuilder();

    //    var body = new StringBuilder();
    //    var apirequest = new GetPointBalanceApiRequest { CustomerId = associateId.ToString() };
    //    //head.AppendLine("<link href='~/Styles/iframe.css' rel='stylesheet' />");
    //    head.AppendLine("<style >");
    //    head.AppendLine("    .msg {");
    //    head.AppendLine("        font-style: italic;");
    //    head.AppendLine("        font-weight: bold;");
    //    head.AppendLine("        color: #999999;");
    //    head.AppendLine("    }");
    //    head.AppendLine("    .mail {");
    //    head.AppendLine("        font-weight: bold;");
    //    head.AppendLine("        color: #169BD7;");
    //    head.AppendLine("    }");
    //    head.AppendLine("</style>");
    //    //head.AppendLine("<script src='~/Scripts/addcardframe.js'></script>");
    //    //head.AppendLine("<link href='/Styles/bootstrapCS.css' rel='stylesheet' />");
    //    head.AppendLine("<script src='https://code.jquery.com/jquery-3.4.1.min.js'></script>");
    //    head.AppendLine("<script>");
    //    head.AppendLine("    $(document).ready(function () {");
    //    head.AppendLine("        var url = window.location.href;");
    //    head.AppendLine("        var arr = url.split('/');");
    //    head.AppendLine("        var apiurl = arr[0] + '/Command/ClientAPI/Merchants/Ewallet/GetPointBalance';");

    //    head.AppendLine("            var jsonObjects =  {");
    //    head.AppendLine($"                'CustomerId': '{associateId}'");
    //    head.AppendLine("            };");

    //    head.AppendLine("        var settings = {");
    //    head.AppendLine("            'async': true,");
    //    head.AppendLine("            'crossDomain': true,");
    //    head.AppendLine("            'url': apiurl,");
    //    head.AppendLine("            'method': 'POST',");
    //    head.AppendLine("            'headers': {");
    //    head.AppendLine("                'content-type': 'application/json; charset=UTF-8',");
    //    head.AppendLine("                'Accept': 'application/json',");
    //    head.AppendLine("                'dataType': 'json',");
    //    head.AppendLine("                'Cache-Control': 'no-cache'");
    //    head.AppendLine("            },");
    //    head.AppendLine("            'data': JSON.stringify(jsonObjects)");
    //    head.AppendLine("        }");
    //    head.AppendLine("        $.ajax(settings).done(function (r) {");
    //    head.AppendLine("            if (r.Status === 0) {");
    //    head.AppendLine("                if (r.Data) {");
    //    head.AppendLine("                    if (r.Status === 0) {");
    //    head.AppendLine("                        $('#ewalletbal').text(r.Data.Amount);");
    //    head.AppendLine("                    }");
    //    head.AppendLine("                    else {");
    //    head.AppendLine("                        alert(r.Message);");
    //    head.AppendLine("                    }");
    //    head.AppendLine("                }");
    //    head.AppendLine("            }");
    //    head.AppendLine("        });");
    //    head.AppendLine("    });");
    //    head.AppendLine("</script>");
    //    head.AppendLine("<script>");
    //    head.AppendLine("    function SavePayment() {");
    //    head.AppendLine("        var data = {};");
    //    head.AppendLine("        data.card_ref = guid();");
    //    head.AppendLine("        data.expireMonth = 1;");
    //    head.AppendLine("        data.type = 'E-Wallet';");
    //    head.AppendLine("        data.token = 'test';");
    //    head.AppendLine("        DS_SavePaymentMethod(data);");
    //    head.AppendLine("    }");

    //    head.AppendLine("function guid() {");
    //    head.AppendLine("function s4() {");
    //    head.AppendLine("    return Math.floor((1 + Math.random()) * 0x10000)");
    //    head.AppendLine("        .toString(16)");
    //    head.AppendLine("        .substring(1);");
    //    head.AppendLine("}");
    //    head.AppendLine("return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();");
    //    head.AppendLine("}");

    //    head.AppendLine("</script>");
    //    body.AppendLine("<p class='text-center msg' id='ewalletmsg'>Your E-Wallet Balance is :</p>");
    //    body.AppendLine("<p class='text-center mail' id='ewalletbal'></p>");
    //    body.AppendLine("<div class='row' style='margin-top:25px;'>");
    //    body.AppendLine("    <div class='col-xs-12 text-center'>");
    //    body.AppendLine("        <button type='button' class='btn btn-primary' onclick='SavePayment();'>Save E-Wallet Account</button>");
    //    body.AppendLine("    </div>");
    //    body.AppendLine("</div>");

    //    var res = new SavePaymentForm
    //    {
    //        Head = head.ToString(),
    //        Body = body.ToString()
    //    };

    //    return res;
    //}

    //public override string GenerateOneTimeAuthToken(string payerId, int associateId, string languageCode, string countryCode)
    //{
    //    try
    //    {
    //        string token = _ewalletService.CreateToken();
    //        return token;
    //    }
    //    catch (Exception e)
    //    { }
    //    return "";
    //}


    //internal PaymentMethod GetBackupMerchantPaymentFromSettings(int backupMerchantId, int payorId)
    //{
    //    _logger.LogInformation($"EwalletMerchant.GetBackupMerchantPaymentFromSettings", $"Inadequate E-Wallet balance to pay for autoship. Failing over to use the backup merchant ({backupMerchantId}).");

    //    //If they have backupMerchantId set in settings attempt to charge a saved payment from that merchant. 
    //    //TODO: Does not account for different currencies this will have to be addressed in the future when time permits.
    //    var fetchedMerchant = _moneyInService.GetMerchant(backupMerchantId);
    //    if (fetchedMerchant != null && fetchedMerchant.CanSavePayments)
    //    {
    //        PaymentMethod payment = _moneyInService.GetPaymentMethods(payorId, 0)?.OrderByDescending(x => x.PaymentId).FirstOrDefault();
    //        //Attempt to grab latest saved payment for specified backup merchant id. 
    //        if (payment == null)
    //        {
    //            throw new Exception("Error, no backup payment method found.");
    //        }

    //        return payment;
    //    }

    //    throw new Exception($"Error, merchant {backupMerchantId} not found or is set to not save payments.");
    //}

    //public PaymentResponse ChargeBackupPayment(PaymentMethod payment, int payorId, int orderNumber, double amount, string currencyCode)
    //{
    //    var fetchedMerchant = _moneyInService.GetMerchant(payment.MerchantId);
    //    var paymentResponse = ChargeSavedPayment(payorId.ToString(),
    //        new NewPaymentInfo
    //        {
    //            PaymentMethodId = payment.PaymentMethodId,
    //            Amount = amount,
    //            CurrencyCode = currencyCode
    //        },
    //        orderNumber);
    //    paymentResponse.ReferenceNumber = payment.Ending;

    //    return paymentResponse;
    //}

}
