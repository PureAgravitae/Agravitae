using System;
using DirectScale.Disco.Extension.Services;
using WebExtension.Merchants.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DirectScale.Disco.Extension;
using System.Text;
using WebExtension.Helper;

namespace WebExtension.Merchants
{
    public interface IClientService
    {
        int GetAssociateActiveCommissionMerchant(int associateId);
        bool GetAssociateMerchantAccountInfo(GetAssociateMerchantAccountInfoRequest request);
        string GetCommissionMerchantInfo(GetCommissionMerchantInfoRequest request);
        string GetSavePaymentFrameDetails(int associateId);
    }
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAssociateService _associateService;
        //private readonly ILoggingService _loggingService;
        private readonly ICompanyService _companyService;

        public ClientService(IClientRepository clientRepository, IAssociateService associateService, ICompanyService companyService)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            //_loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        }


        public int GetAssociateActiveCommissionMerchant(int associateId)
        {
            return _clientRepository.GetAssociateActiveCommissionMerchant(associateId);
        }

        public bool GetAssociateMerchantAccountInfo(GetAssociateMerchantAccountInfoRequest request)
        {
            var achInfoResponse = new GetAchInfoResponse() { AssociateAchInfos = new List<AchInfo>(), FailedRetrievals = new List<FailedCommissionMerchantInfoRetrieval>() };

            if (request.MerchantId == 114)
            {
                var requestedAssociatesErrors = new List<FailedCommissionMerchantInfoRetrieval>();
                var getAchInfoResult = _clientRepository.GetAssociateMerchantInfo(request.AssociateId, request.MerchantId);

                if (getAchInfoResult.Any())
                {
                    foreach (var associateId in getAchInfoResult.Select(x => x.AssociateID).Distinct())
                    {
                        var missingFields = "";
                        try
                        {
                            var associateValues = getAchInfoResult.Where(x => x?.AssociateID == associateId).ToList();
                            var info = new AchInfo
                            {
                                AssociateId = associateId,
                                BankName = associateValues.Where(x => x?.MerchantVariableKey == "BankName").Select(x => x?.Value).FirstOrDefault(),
                                AccountNumber = associateValues.Where(x => x?.MerchantVariableKey == "AccountNumber").Select(x => x?.Value).FirstOrDefault(),
                                RoutingNumber = associateValues.Where(x => x?.MerchantVariableKey == "RoutingNumber").Select(x => x?.Value).FirstOrDefault(),
                            };

                            if (string.IsNullOrWhiteSpace(info.BankName)) missingFields = "BankName,";
                            if (string.IsNullOrWhiteSpace(info.AccountNumber)) missingFields += "AccountNumber,";
                            if (string.IsNullOrWhiteSpace(info.RoutingNumber)) missingFields += "RoutingNumber";

                            if (string.IsNullOrWhiteSpace(missingFields))
                            {
                                achInfoResponse.AssociateAchInfos.Add(info);
                            }
                            else
                            {
                                requestedAssociatesErrors.Add(new FailedCommissionMerchantInfoRetrieval
                                {
                                    AssociateId = associateId,
                                    Reason = $"Associate missing required field(s): {missingFields.TrimEnd(',')}."
                                });
                                return false;
                            }
                        }
                        catch (Exception)
                        {
                            requestedAssociatesErrors.Add(new FailedCommissionMerchantInfoRetrieval
                            {
                                AssociateId = associateId,
                                Reason = "Error occurred while retrieving ACH info for this associate."
                            });
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
            {
                var getAssociateInfoResult = _clientRepository.GetAssociateMerchantInfo(request.AssociateId, request.MerchantId);
                if (getAssociateInfoResult.Any())
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public string GetCommissionMerchantInfo(GetCommissionMerchantInfoRequest request)
        {
            var achInfoResponse = new GetAchInfoResponse() { AssociateAchInfos = new List<AchInfo>(), FailedRetrievals = new List<FailedCommissionMerchantInfoRetrieval>() };
            var iPayoutInfoResponse = new GetIPayoutInfoResponse() { AssociateIPayoutInfos = new List<IPayoutInfo>(), FailedRetrievals = new List<FailedCommissionMerchantInfoRetrieval>() };
            var checkInfoResponse = new GetCheckInfoResponse();


            if (request.MerchantId == 114)
            {
                iPayoutInfoResponse = null;
                var requestedAssociatesErrors = new List<FailedCommissionMerchantInfoRetrieval>();
                var getAchInfoResult = _clientRepository.GetAchInfo(request.AssociateIds);

                var associatesNotFound = request?.AssociateIds.Where(x => getAchInfoResult.All(y => y.AssociateID != x)).ToList();

                if (associatesNotFound.Any())
                {
                    requestedAssociatesErrors.AddRange(associatesNotFound.Select(id => new FailedCommissionMerchantInfoRetrieval
                    {
                        AssociateId = id,
                        Reason = "Associate ACH info not found."
                    }));
                }

                foreach (var associateId in getAchInfoResult.Select(x => x.AssociateID).Distinct())
                {
                    var missingFields = "";
                    try
                    {
                        var associateValues = getAchInfoResult.Where(x => x?.AssociateID == associateId).ToList();
                        var info = new AchInfo
                        {
                            AssociateId = associateId,
                            BankName = associateValues.Where(x => x?.MerchantVariableKey == "BankName").Select(x => x?.Value).FirstOrDefault(),
                            AccountNumber = associateValues.Where(x => x?.MerchantVariableKey == "AccountNumber").Select(x => x?.Value).FirstOrDefault(),
                            RoutingNumber = associateValues.Where(x => x?.MerchantVariableKey == "RoutingNumber").Select(x => x?.Value).FirstOrDefault(),
                            AccountType = associateValues.Where(x => x?.MerchantVariableKey == "AccountType").Select(x => x?.Value).FirstOrDefault(),
                        };

                        if (string.IsNullOrWhiteSpace(info.BankName)) missingFields = "BankName,";
                        if (string.IsNullOrWhiteSpace(info.AccountNumber)) missingFields += "AccountNumber,";
                        if (string.IsNullOrWhiteSpace(info.RoutingNumber)) missingFields += "RoutingNumber";

                        if (string.IsNullOrWhiteSpace(missingFields))
                        {
                            achInfoResponse.AssociateAchInfos.Add(info);
                        }
                        else
                        {
                            requestedAssociatesErrors.Add(new FailedCommissionMerchantInfoRetrieval
                            {
                                AssociateId = associateId,
                                Reason = $"Associate missing required field(s): {missingFields.TrimEnd(',')}."
                            });
                        }
                    }
                    catch (Exception)
                    {
                        requestedAssociatesErrors.Add(new FailedCommissionMerchantInfoRetrieval
                        {
                            AssociateId = associateId,
                            Reason = "Error occurred while retrieving ACH info for this associate."
                        });
                    }
                }

                achInfoResponse.FailedRetrievals = requestedAssociatesErrors;
            }

            if (request.MerchantId == 112)
            {
                achInfoResponse = null;
                var requestedAssociatesErrors = new List<FailedCommissionMerchantInfoRetrieval>();
                var getIPayoutInfoResult = _clientRepository.GetIPayoutInfo(request.AssociateIds);

                var associatesNotFound = request?.AssociateIds.Where(x => getIPayoutInfoResult.All(y => y?.AssociateID != x)).ToList();

                if (associatesNotFound.Any())
                {
                    requestedAssociatesErrors.AddRange(associatesNotFound.Select(id => new FailedCommissionMerchantInfoRetrieval
                    {
                        AssociateId = id,
                        Reason = "Associate IPayout info not found."
                    }));
                }

                foreach (var associateId in getIPayoutInfoResult.Select(x => x.AssociateID).Distinct())
                {
                    try
                    {
                        var associateValues = getIPayoutInfoResult.Where(x => x.AssociateID == associateId).ToList();
                        var info = new IPayoutInfo
                        {
                            AssociateId = associateId,
                            AccountNumber = associateValues.Where(x => x?.MerchantVariableKey == "AccountNumber").Select(x => x?.Value).FirstOrDefault(),
                        };

                        if (string.IsNullOrWhiteSpace(info.AccountNumber))
                        {
                            requestedAssociatesErrors.Add(new FailedCommissionMerchantInfoRetrieval
                            {
                                AssociateId = associateId,
                                Reason = $"IPayout AccountNumber not found.."
                            });
                        }
                        else
                        {
                            iPayoutInfoResponse.AssociateIPayoutInfos.Add(info);
                        }
                    }
                    catch (Exception)
                    {
                        requestedAssociatesErrors.Add(new FailedCommissionMerchantInfoRetrieval
                        {
                            AssociateId = associateId,
                            Reason = "Error occurred while retrieving IPayout info for this associate."
                        });
                    }
                }

                iPayoutInfoResponse.FailedRetrievals = requestedAssociatesErrors;
            }

            if (request.MerchantId == 3)
            {
                foreach (var associateId in request.AssociateIds)
                {
                    var summary = _associateService.GetAssociate(associateId).Result;
                    var associateData = new GetCheckInfoResponseData { AssociateName = summary.Name, AssociateAddress = summary.Address, AssociateId = associateId, AssociateType = summary.AssociateType, BackOfficeId = summary.BackOfficeId, BirthDate = summary.BirthDate, EmailAddress = summary.EmailAddress, SignupDate = summary.SignupDate };
                    checkInfoResponse.CheckInfo.Add(associateData);
                }
                var companyInfo = _companyService.GetCompany().Result;
                checkInfoResponse.Company = companyInfo;
            }

            var response = new GetCommissionMerchantInfoResponse
            {
                BatchId = request.BatchId,
                AchInfo = achInfoResponse,
                IPayoutInfo = iPayoutInfoResponse,
                CheckResponse = checkInfoResponse
            };

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public string GetSavePaymentFrameDetails(int associateId)
        {
            try
            {
                var head = new StringBuilder();

                var body = new StringBuilder();
                //var apirequest = new GetPointBalanceApiRequest { CustomerId = associateId.ToString() };
                //head.AppendLine("<link href='~/Styles/iframe.css' rel='stylesheet' />");
                head.AppendLine("<style >");
                head.AppendLine("    .msg {");
                head.AppendLine("        font-style: italic;");
                head.AppendLine("        font-weight: bold;");
                head.AppendLine("        color: #999999;");
                head.AppendLine("    }");
                head.AppendLine("    .mail {");
                head.AppendLine("        font-weight: bold;");
                head.AppendLine("        color: #169BD7;");
                head.AppendLine("    }");
                head.AppendLine("</style>");
                //head.AppendLine("<script src='~/Scripts/addcardframe.js'></script>");
                //head.AppendLine("<link href='/Styles/bootstrapCS.css' rel='stylesheet' />");
                head.AppendLine("<script src='https://code.jquery.com/jquery-3.4.1.min.js'></script>");
                head.AppendLine("<script>");
                head.AppendLine("    $(document).ready(function () {");
                head.AppendLine("        var url = window.location.href;");
                head.AppendLine("        var arr = url.split('/');");
                head.AppendLine("        var apiurl = arr[0] + '/Command/ClientAPI/Merchants/Ewallet/GetPointBalance';");

                head.AppendLine("            var jsonObjects =  {");
                head.AppendLine($"                'CustomerId': '{associateId}'");
                head.AppendLine("            };");

                head.AppendLine("        var settings = {");
                head.AppendLine("            'async': true,");
                head.AppendLine("            'crossDomain': true,");
                head.AppendLine("            'url': apiurl,");
                head.AppendLine("            'method': 'POST',");
                head.AppendLine("            'headers': {");
                head.AppendLine("                'content-type': 'application/json; charset=UTF-8',");
                head.AppendLine("                'Accept': 'application/json',");
                head.AppendLine("                'dataType': 'json',");
                head.AppendLine("                'Cache-Control': 'no-cache'");
                head.AppendLine("            },");
                head.AppendLine("            'data': JSON.stringify(jsonObjects)");
                head.AppendLine("        }");
                head.AppendLine("        $.ajax(settings).done(function (r) {");
                head.AppendLine("            if (r.Status === 0) {");
                head.AppendLine("                if (r.Data) {");
                head.AppendLine("                    if (r.Status === 0) {");
                head.AppendLine("                        $('#ewalletbal').text(r.Data.Amount);");
                head.AppendLine("                    }");
                head.AppendLine("                    else {");
                head.AppendLine("                        alert(r.Message);");
                head.AppendLine("                    }");
                head.AppendLine("                }");
                head.AppendLine("            }");
                head.AppendLine("        });");
                head.AppendLine("    });");
                head.AppendLine("</script>");
                head.AppendLine("<script>");
                head.AppendLine("    function SavePayment() {");
                head.AppendLine("        var data = {};");
                head.AppendLine("        data.card_ref = guid();");
                head.AppendLine("        data.expireMonth = 1;");
                head.AppendLine("        data.type = 'E-Wallet';");
                head.AppendLine("        data.token = 'test';");
                head.AppendLine("        DS_SavePaymentMethod(data);");
                head.AppendLine("    }");

                head.AppendLine("function guid() {");
                head.AppendLine("function s4() {");
                head.AppendLine("    return Math.floor((1 + Math.random()) * 0x10000)");
                head.AppendLine("        .toString(16)");
                head.AppendLine("        .substring(1);");
                head.AppendLine("}");
                head.AppendLine("return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();");
                head.AppendLine("}");

                head.AppendLine("</script>");
                head.AppendLine("<p class='text-center msg' id='ewalletmsg'>Your E-Wallet Balance is :</p>");
                head.AppendLine("<p class='text-center mail' id='ewalletbal'></p>");
                head.AppendLine("<div class='row' style='margin-top:25px;'>");
                head.AppendLine("    <div class='col-xs-12 text-center'>");
                head.AppendLine("        <button type='button' class='btn btn-primary' onclick='SavePayment();'>Save E-Wallet Account</button>");
                head.AppendLine("    </div>");
                head.AppendLine("</div>");

                return head.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
