using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using AgravitaeWebExtension.Merchants.CambridgeMerchant.Models;
using AgravitaeWebExtension.Merchants.CambridgeMerchant.Services.Models;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Data.SqlClient;
using Dapper;
using System.Collections;
using DirectScale.Disco.Extension;
using AgravitaeWebExtension.Merchants.EwalletMerchant.Models;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Services
{
    public interface ICambridgeRepository
    {
        IBANValidationResponse IBANValidation(IBANValidationRequest req);
        List<BankSearchResponse> BankSearch(BankSearchRequest req);
        GetBeneRulesResponse GetBenefisaryRules(GetBenefisaryRulesRequest req);
        List<GetCountriesResponse> GetCountries();
        List<GetCitiesResponse> GetCities(GetCitiesRequest req);
        List<GetRegionsResponse> GetRegions(GetRegionsRequest req);
        List<GetCurrenciesResponse> GetCurrencies();
        string GetApiToken();
        List<GetAssociatePaymentOrderResponse> GetAssociatePaymentOrder(GetAssociatePaymentOrderRequest req);
        CreateBenefisaryResponse CreateBenefisary(CreateBenefisaryRequest req, bool isEdit);
        GetBenefisaryResponse GetBenefisary(GetBenefisaryRequest req);
        GetPaymentOrderResponse GetPaymentOrder(GetPaymentOrderRequest req);
        SearchBenefisaryResponse SearchBenefisary(SearchBenefisaryRequest req);
        MoneyOutPaymentTransferResponse MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest req);
        CurrencyRate GetCurrencyRate(string Currency);
        void LogCurrencyRate(CurrencyRate currencyRate);
        BenefisaryDetailsResponse GetBenefisaryDetails(GetBenefisaryRequest req);
        DeleteBenefisaryResponse DeleteBenefisary(GetBenefisaryRequest req);
        int SetActiveCommissionMerchant(SetActiveCommissionMerchantRequest request);
        GetPaymentRateInfoResponse GetPamentRateInfo(GetPaymentRateInfoRequest req);
        bool BenefisarySetDefault(BenefisarySetDefaultRequest req);

    }

    public class CambridgeRepository : ICambridgeRepository
    {
        private readonly IDataService _dataService;
        private readonly IMoneyOutService _moneyOutService;
        private readonly ISettingsService _settingsService;
        private readonly ICambridgeSetting _cambridgeSetting;

        public CambridgeRepository(IDataService dataService, IMoneyOutService moneyOutService, ISettingsService settingsService, ICambridgeSetting cambridgeSetting)
        {
            //_loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _moneyOutService = moneyOutService ?? throw new ArgumentNullException(nameof(moneyOutService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _cambridgeSetting = cambridgeSetting ?? throw new ArgumentNullException(nameof(cambridgeSetting));
        }

        public CambridgeSettingsResponse GetCambridgeAccessTokenInfo()
        {
            DirectScale.Disco.Extension.EnvironmentType envType = _settingsService.ExtensionContext().Result.EnvironmentType;
            CambridgeSettingsResponse objSettings = new CambridgeSettingsResponse();
            if (envType == EnvironmentType.Sandbox)
            {
                objSettings.AccessToken = "BFFA0348E2EDE397EEECCC4D1B30AA3F0146C37CF15C52E41FF0C8CB2A6C2B03EEFFB611652482F7CAAC2F7D49F26A166BB488D47A820D47E70FAA670449EA2762241D825C2322468FDE7E26140DB781BBDB784A427E01E0ECC69EC40EB49A46612BDC63E296D777AC0F608D1622022444261991B1B89F152577C21AE98C05571E0D4EDD2C55C4B0947FEB2B3442319134334E0DBF8041FDBAD8807FF0E93FD7CD037D66752F4611482E19EB23326489";
            }
            else
            {
                objSettings.AccessToken = GetApiToken();
            }
            return objSettings;
        }


        public IBANValidationResponse IBANValidation(IBANValidationRequest req)
        {

            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/ibanvalidation");
                var parameters = new
                {
                    iban = req.IBANNumber
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var ibanStatus = (bool)contentJSON["content"]["isValid"];
                    if (ibanStatus)
                    {
                        return new IBANValidationResponse
                        {
                            isValid = (bool)contentJSON["content"]["isValid"],
                            accountNumber = (string)contentJSON["content"]["accountNumber"],
                            branchName = (string)contentJSON["content"]["branchName"],
                            bankName = (string)contentJSON["content"]["bankName"],
                            iban = (string)contentJSON["content"]["iban"],
                            branchCode = (string)contentJSON["content"]["branchCode"],
                            routingNumber = (string)contentJSON["content"]["routingNumber"],
                            swiftBIC = (string)contentJSON["content"]["swiftBIC"],
                            bankAddress = (string)contentJSON["content"]["bankAddress"],
                            postalCode = (string)contentJSON["content"]["postalCode"],
                            countryName = (string)contentJSON["content"]["countryName"],
                            countryCode = (string)contentJSON["content"]["country"],
                            bankCity = (string)contentJSON["content"]["bankCity"],
                        };
                    }
                    else
                    {
                        return new IBANValidationResponse
                        {
                            isValid = (bool)contentJSON["content"]["isValid"]
                        };
                    }

                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new IBANValidationResponse
                    {
                        error = "Record Not Found"
                    };
                }
            }

        }

        public GetBeneRulesResponse GetBenefisaryRules(GetBenefisaryRulesRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/template-guide?templateType=Bene&destinationcountry=" + req.Destinationcountry + "&bankcountry=" + req.Bankcountry + "&bankCurrency=" + req.BankCurrency + "&classification=" + req.Classification + "&paymentMethods=" + req.PaymentMethod);

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"]["templateGuide"]["rules"];
                    var rules = new List<GetBenefisaryRulesResponse>();
                    foreach (var item in searchResult)
                    {
                        rules.Add(new GetBenefisaryRulesResponse
                        {
                            Id = (string)item["id"],
                            ErrorMessage = (string)item["errorMessage"],
                            IsRequired = (string)item["isRequired"],
                            RegEx = (string)item["regEx"],
                            validationRules = String.Join(" ", item["validationRules"]),
                            valueSet = String.Join(" ", item["valueSet"] ?? ""),
                            isRequiredInValueSet = (string)item["isRequiredInValueSet"],
                            defaultValue = (string)item["defaultValue"],
                            error = (string)item["error"]
                        });
                    }
                    var regulatoryInfo = contentJSON["content"]["templateGuide"]["regulatoryRules"];
                    var regulatoryRules = new List<GetBenefisaryRulesResponse>();
                    foreach (var item in regulatoryInfo)
                    {
                        regulatoryRules.Add(new GetBenefisaryRulesResponse
                        {
                            Id = (string)item["id"],
                            ErrorMessage = (string)item["errorMessage"],
                            IsRequired = (string)item["isRequired"],
                            RegEx = (string)item["regEx"],
                            validationRules = String.Join(" ", item["validationRules"]),
                            valueSet = String.Join(" ", item["valueSet"] ?? ""),
                            isRequiredInValueSet = (string)item["isRequiredInValueSet"],
                            defaultValue = (string)item["defaultValue"],
                            error = (string)item["error"]
                        });
                    }
                    return new GetBeneRulesResponse
                    {
                        Rules = rules,
                        RegulatoryRules = regulatoryRules
                    };
                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new GetBeneRulesResponse
                    {
                        error = "Record Not Found"
                    };
                }
            }
        }

        public List<BankSearchResponse> BankSearch(BankSearchRequest req)
        {
            var res = new List<BankSearchResponse>();

            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/banks?country=" + req.BankCountry + "&query=" + req.BankName);

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"]["data"]["rows"];
                    foreach (var item in searchResult)
                    {
                        res.Add(new BankSearchResponse
                        {
                            bankName = (string)item["institutionName"],
                            address1 = (string)item["address1"],
                            address2 = (string)item["address2"],
                            city = (string)item["city"],
                            postalCode = (string)item["postalCode"],
                            country = (string)item["country"],
                            nationalBankCode = (string)item["nationalBankCode"],
                            swiftBIC = (string)item["swiftBIC"],
                            branchName = (string)item["branchName"]
                        });
                    }

                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    res.Add(new BankSearchResponse
                    {
                        error = "Record Not Found"
                    });
                }
            }

            return res;
        }

        public List<GetCountriesResponse> GetCountries()
        {
            var res = new List<GetCountriesResponse>();

            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/countries");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"];
                    foreach (var item in searchResult)
                    {
                        res.Add(new GetCountriesResponse
                        {
                            countryName = (string)item["countryName"],
                            defaultCurrency = (string)item["defaultCurrency"],
                            country = (string)item["country"]
                        });
                    }

                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    res.Add(new GetCountriesResponse
                    {
                        error = "Record Not Found"
                    });
                }
            }

            return res;
        }

        public List<GetCitiesResponse> GetCities(GetCitiesRequest req)
        {
            var res = new List<GetCitiesResponse>();
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/" + req.CountryName + "/" + req.RegionName + "/cities");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"];
                    foreach (var item in searchResult)
                    {
                        if ((string)item["city"] != "")
                        {
                            res.Add(new GetCitiesResponse
                            {
                                cityName = (string)item["city"]
                            });
                        }
                    }

                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    res.Add(new GetCitiesResponse
                    {
                        error = "Record Not Found"
                    });
                }
            }
            return res;
        }

        public List<GetRegionsResponse> GetRegions(GetRegionsRequest req)
        {
            var res = new List<GetRegionsResponse>();

            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/regions?country=" + req.CountryName);

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"]["regions"];
                    foreach (var item in searchResult)
                    {
                        if ((string)item["name"] != "")
                        {
                            res.Add(new GetRegionsResponse
                            {
                                regionName = (string)item["name"]
                            });
                        }
                    }

                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    res.Add(new GetRegionsResponse
                    {
                        error = "Record Not Found"
                    });
                }
            }

            return res;
        }

        public string GetPartnerLevelAccessCode()
        {
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.UtcNow;
            var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
            var exp = (int)issueTime.AddMinutes(30).Subtract(utc0).TotalSeconds;

            TokenModel Treq = new TokenModel();
            Treq.Issuedat = iat;
            Treq.Expiration = exp;
            Treq.Audience = "cambridgefx";
            Treq.Subject = "";

            using (var httpClient = new HttpClient())
            {
                var url = new Uri("http://jwtbuilder.jamiekurtz.com/tokens");
                var Token = "";
                var parameters = new
                {
                    alg = "HS256",
                    key = _cambridgeSetting.PartnerKey,
                    claims = new
                    {
                        aud = Treq.Audience,
                        exp = Treq.Expiration,
                        iat = Treq.Issuedat,
                        iss = _cambridgeSetting.PartnerIssuerID,
                        sub = Treq.Subject
                    }
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    Token = (string)contentJSON["token"];
                }
                return Token;
            }
        }

        public string GetPartnerLevelToken()
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/partner/oauth2/token");
                var Token = "";
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var parameters = new
                {
                    assertion = GetPartnerLevelAccessCode(),
                    grant_type = "urn:ietf:params:oauth:grant-type:jwt-bearer"
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    Token = (string)contentJSON["access_code"];
                }
                return Token;
            }
        }

        public string GetClientLevelAccessCode()
        {
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.UtcNow;
            var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
            var exp = (int)issueTime.AddMinutes(30).Subtract(utc0).TotalSeconds;

            TokenModel Treq = new TokenModel();
            Treq.Issuedat = iat;
            Treq.Expiration = exp;
            Treq.Audience = "cambridgefx";
            Treq.Subject = "";

            using (var httpClient = new HttpClient())
            {
                var url = new Uri("http://jwtbuilder.jamiekurtz.com/tokens");
                var Token = "";
                var parameters = new
                {
                    alg = "HS256",
                    key = _cambridgeSetting.Key,
                    claims = new
                    {
                        aud = Treq.Audience,
                        exp = Treq.Expiration,
                        iat = Treq.Issuedat,
                        iss = _cambridgeSetting.IssuerID,
                        sub = Treq.Subject
                    }
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    Token = (string)contentJSON["token"];
                }
                return Token;
            }
        }

        public string GetApiToken()
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/partner/oauth2/userToken");
                var Token = "";
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetPartnerLevelToken());
                var parameters = new
                {
                    assertion = GetClientLevelAccessCode(),
                    grant_type = "urn:ietf:params:oauth:grant-type:jwt-bearer"
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    Token = (string)contentJSON["access_code"];
                }
                return Token;
            }
        }

        public List<GetCurrenciesResponse> GetCurrencies()
        {
            var res = new List<GetCurrenciesResponse>();

            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/payCurrencies?product=Bene");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"]["all"];
                    foreach (var item in searchResult)
                    {
                        res.Add(new GetCurrenciesResponse
                        {
                            curr = (string)item["curr"],
                            desc = (string)item["desc"]
                        });
                    }

                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    res.Add(new GetCurrenciesResponse
                    {
                        error = "Record Not Found"
                    });
                }
            }

            return res;
        }

        public List<GetAssociatePaymentOrderResponse> GetAssociatePaymentOrder(GetAssociatePaymentOrderRequest req)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var query = @"SELECT BeneficiaryID,OrderPaymentNumber,OrderPaymentdate FROM Client.AssociateBeneficiaryOrderPaymentInfo WHERE AssociateId = @AssociateId";
                return dbConnection.Query<GetAssociatePaymentOrderResponse>(query, new { AssociateId = req.AssociateID }).ToList() ?? new List<GetAssociatePaymentOrderResponse>();
            }
        }

        public BenefisaryDetailsResponse GetBenefisaryDetails(GetBenefisaryRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/templates/" + _cambridgeSetting.ClientId + "_" + req.AssociateID);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var bene = contentJSON["content"]["bene"];
                    return new BenefisaryDetailsResponse
                    {
                        benefisaryId = (string)bene["id"],
                        beneContactName = (string)bene["beneContactName"],
                        destinationCountry = (string)bene["destinationCountry"],
                        bankCurrency = (string)bene["bankCurrency"],
                        beneClassification = (string)bene["beneClassification"],
                        beneCountry = (string)bene["beneCountry"],
                        beneRegion = (string)bene["beneRegion"],
                        beneAddress1 = (string)bene["beneAddress1"],
                        beneAddress2 = (string)bene["beneAddress2"],
                        beneCity = (string)bene["beneCity"],
                        benePostal = (string)bene["benePostal"],
                        beneEmail = (string)bene["beneEmail"],
                        bankName = (string)bene["bankName"],
                        bankCountry = (string)bene["bankCountry"],
                        bankRegion = (string)bene["bankRegion"],
                        bankCity = (string)bene["bankCity"],
                        bankPostal = (string)bene["bankPostal"],
                        paymentReference = (string)bene["paymentReference"],
                        iban = (string)bene["iban"],
                        beneCountryName = (string)bene["beneCountryName"],
                        accountHolderCountryName = (string)bene["accountHolderCountryName"],
                        bankCountryName = (string)bene["bankCountryName"],
                        bankCurrencyDesc = (string)bene["bankCurrencyDesc"],
                        accountNumber = (string)bene["accountNumber"],
                        routingCode = (string)bene["routingCode"],
                        swiftBicCode = (string)bene["swiftBicCode"],
                        sendPayTracker = (bool)bene["sendPayTracker"],
                        bankAddressLine1 = (string)bene["bankAddressLine1"],
                        bankAddressLine2 = (string)bene["bankAddressLine2"],
                        regulatoryFields = (string)bene["regulatoryFields"],
                        preferredMethod = (string)bene["preferredMethod"],
                        paymentMethods = (string)bene["paymentMethods"],
                        BeneIdentifier = (string)bene["beneIdentifier"],
                        DestinationCountry = (string)bene["destinationCountry"],
                        BeneficiaryPhoneNumber = (string)bene["beneficiaryPhoneNumber"],
                        PurposeOfPayment = (string)bene["purposeOfPayment"],
                        Regulatory = bene["regulatory"],
                        BeneCountryName = (string)bene["beneCountryName"],
                        localAccountNumber = (string)bene["localAccountNumber"]
                    };
                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new BenefisaryDetailsResponse
                    {
                        error = "Record Not Found"
                    };
                }
            }

        }

        public DeleteBenefisaryResponse DeleteBenefisary(GetBenefisaryRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/templates/" + _cambridgeSetting.ClientId + "_" + req.AssociateID);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    DeleteBenefisary(req.AssociateID);
                    return new DeleteBenefisaryResponse
                    {
                        BeneficiaryDeleteStatus = "Benefisary has been deleted"
                    };
                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new DeleteBenefisaryResponse
                    {
                        error = "Record Not Found"
                    };
                }
            }

        }

        public void DeleteBenefisary(string AssociateId)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {

                var parameters = new
                {
                    beneficiaryID = _cambridgeSetting.ClientId + "_" + AssociateId
                };
                var sql = @"DELETE FROM Client.AssociateBeneficiaryInfo WHERE BeneficiaryID = @beneficiaryID";
                dbConnection.Execute(sql, parameters);
            }
        }

        public CreateBenefisaryResponse CreateBenefisary(CreateBenefisaryRequest req, bool isEdit)
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/templates/" + _cambridgeSetting.ClientId + "_" + req.AssociateId);
                var parameters = new
                {
                    accountHolderName = req.AccountHolderName,
                    templateIdentifier = _cambridgeSetting.ClientId + "_" + req.AssociateId,
                    destinationCountry = req.DestinationCountry,
                    bankCurrency = req.BankCurrency,
                    classification = req.Classification,
                    paymentMethods = req.PaymentMethods,
                    preferredMethod = req.PreferredMethod,
                    accountNumber = req.AccountNumber,
                    localAccountNumber = req.LocalAccountNumber,
                    routingCode = req.BankRoutingCode,
                    routingCode2 = "",
                    localRoutingCode = req.LocalRoutingCode,
                    accountHolderAddress1 = req.AccountHolderAddress1,
                    accountHolderAddress2 = req.AccountHolderAddress2,
                    accountHolderCity = req.AccountHolderCity,
                    accountHolderPostal = req.AccountHolderPostal,
                    accountHolderCountry = req.AccountHolderCountry,
                    accountHolderRegion = req.AccountHolderRegion,
                    accountHolderPhoneNumber = req.AccountHolderPhone,
                    accountHolderEmail = req.AccountHolderEmail,
                    sendPayTracker = req.SendPayTracker,
                    iban = "",
                    bankName = req.BankName,
                    swiftBicCode = req.SwiftBicCode,
                    bankAddressLine1 = req.BankAddress1,
                    bankAddressLine2 = req.BankAddress2,
                    bankCity = req.BankCity,
                    bankPostal = req.BankPostal,
                    bankCountry = req.BankCountry,
                    bankRegion = req.BankRegion,
                    paymentReference = "Commission Payout",
                    internalPaymentAlert = req.AccountHolderEmail,
                    externalPaymentAlert = req.AccountHolderEmail,
                    regulatory = req.RegulatoryInfo
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var benifisaryId = (string)contentJSON["content"]["templateId"];
                    if (isEdit)
                    {
                        UpdateBenifisary(_cambridgeSetting.ClientId + "_" + req.AssociateId, req.AssociateId, req.AccountNumber, req.AccountHolderEmail);
                    }
                    else
                    {
                        InsertBenifisary(_cambridgeSetting.ClientId + "_" + req.AssociateId, req.AssociateId, req.AccountNumber, req.AccountHolderEmail);
                    }

                    //Set Associate Custom Money Out Merchant
                    SetActiveCommissionMerchantRequest reqobj = new SetActiveCommissionMerchantRequest();
                    reqobj.AssociateId = Convert.ToInt32(req.AssociateId);
                    reqobj.MerchantId = 9001;
                    SetActiveCommissionMerchant(reqobj);

                    return new CreateBenefisaryResponse
                    {
                        benefisaryId = benifisaryId
                    };
                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new CreateBenefisaryResponse
                    {
                        error = ""
                    };
                }
            }

        }

        public GetPaymentRateInfoResponse GetPamentRateInfo(GetPaymentRateInfoRequest req)
        {
            GetPaymentRateInfoResponse objres = new GetPaymentRateInfoResponse();
            using (var httpClient = new HttpClient())
            {
                var orderPaymentNumber = "";
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/quotes-payment");
                GetBenefisaryRequest objReq = new GetBenefisaryRequest();
                objReq.AssociateID = Convert.ToString(req.AssociateID);
                var beneInfo = GetBenefisaryDetails(objReq);
                var payMethod = "";
                if (beneInfo.preferredMethod == "W")
                {
                    payMethod = "Wire";
                }
                else if (beneInfo.preferredMethod == "E")
                {
                    payMethod = "EFT";
                }
                else
                {
                    payMethod = "Link Balance";
                }

                var parameters = new
                {
                    beneficiaryId = _cambridgeSetting.ClientId + "_" + req.AssociateID,
                    paymentMethod = payMethod,
                    amount = req.Amount,
                    lockSide = "Settlement",
                    paymentCurrency = beneInfo.bankCurrency,
                    settlementCurrency = _cambridgeSetting.SettlementCurrency,
                    settlementAccountId = _cambridgeSetting.SettlementAccountId,
                    settlementMethod = _cambridgeSetting.SettlementMethod,
                    paymentReference = "",
                    remitterId = "",
                    purposeOfPayment = "Payout transfer",
                    deliveryDate = DateTime.Now.ToString("yyyy-MM-dd")
                };

                var paymentList = new ArrayList();
                paymentList.Add(parameters);
                var paymentRequest = new
                {
                    payments = paymentList
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var paymentResult = contentJSON["content"]["paymentSummary"];
                    var fessResult = contentJSON["content"]["fees"];
                    foreach (var item in paymentResult)
                    {
                        objres.CurrencyRate = (double)item["rate"];
                        objres.PaymentCurrency = (string)item["paymentCurrency"];
                        objres.InitialWithdrawlAmount = (double)item["amountTotal"];
                        objres.PayoutAmount = (double)item["settlementAmount"];
                        objres.SettlementCurrency = (string)item["settlementCurrency"];
                    }
                    foreach (var item in fessResult)
                    {
                        objres.InternationalTransferFees = (double)item["amount"];
                        objres.FeeCurrency = (string)item["currency"];
                    }
                }
            }
            return objres;
        }

        public bool BenefisarySetDefault(BenefisarySetDefaultRequest req)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().ToString()))
            {
                var parameters = new
                {
                    req.associateId,
                    req.benifisaryId
                };
                var sql1 = @"
                    UPDATE Client.AssociateBeneficiaryInfo SET IsDefault = 0 WHERE associateId = @associateId;
                ";
                var sql2 = @"
                    UPDATE Client.AssociateBeneficiaryInfo SET IsDefault = 1 WHERE BeneficiaryID = @benifisaryId;
                ";
                dbConnection.Execute(sql1, parameters);
                return dbConnection.Execute(sql2, parameters) == 1;
            }
        }


        public MoneyOutPaymentTransferResponse MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var orderPaymentNumber = "";
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/quotes-payment");
                GetBenefisaryRequest objReq = new GetBenefisaryRequest();
                objReq.AssociateID = Convert.ToString(req.AssociateID);
                var beneInfo = GetBenefisaryDetails(objReq);
                var payMethod = "";
                if (beneInfo.preferredMethod == "W")
                {
                    payMethod = "Wire";
                }
                else if (beneInfo.preferredMethod == "E")
                {
                    payMethod = "EFT";
                }
                else
                {
                    payMethod = "Link Balance";
                }

                var parameters = new
                {
                    beneficiaryId = _cambridgeSetting.ClientId + "_" + req.AssociateID,
                    paymentMethod = payMethod,
                    amount = req.Amount,
                    lockSide = "Settlement",
                    paymentCurrency = beneInfo.bankCurrency,
                    settlementCurrency = _cambridgeSetting.SettlementCurrency,
                    settlementAccountId = _cambridgeSetting.SettlementAccountId,
                    settlementMethod = _cambridgeSetting.SettlementMethod,
                    paymentReference = "",
                    purposeOfPayment = "Payout transfer",
                    remitterId = req.RemitterId,
                    deliveryDate = DateTime.Now.ToString("yyyy-MM-dd")
                };

                var paymentList = new ArrayList();
                paymentList.Add(parameters);
                var paymentRequest = new
                {
                    payments = paymentList
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var bookResult = contentJSON["links"];
                    foreach (var item in bookResult)
                    {
                        if ((string)item["rel"] == "BOOK_PAYMENT")
                        {
                            var bookPaymentLink = (string)item["uri"];
                            var orderNumber = BookOrder(bookPaymentLink);
                            orderPaymentNumber = orderNumber;

                            if (orderPaymentNumber == "")
                            {
                                var stringContentReCall = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
                                httpClient.DefaultRequestHeaders.Accept.Clear();
                                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                                HttpResponseMessage responseReCall = httpClient.PostAsync(url, stringContentReCall).Result;
                                var contentsReCall = responseReCall.Content.ReadAsStringAsync().Result;
                                if (responseReCall.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contentsReCall))
                                {
                                    var contentJSONReCall = JObject.Parse(contentsReCall);
                                    var bookResultReCall = contentJSONReCall["links"];
                                    foreach (var itemReCall in bookResultReCall)
                                    {
                                        if ((string)itemReCall["rel"] == "BOOK_PAYMENT")
                                        {
                                            var bookPaymentLinkReCall = (string)itemReCall["uri"];
                                            var orderNumberReCall = BookOrder(bookPaymentLinkReCall);
                                            orderPaymentNumber = orderNumberReCall;
                                        }
                                    }
                                }

                            }

                            InsertOrderPaymentNumber(req.AssociateID, orderNumber);
                        }
                    }

                    var Result = contentJSON["content"]["paymentSummary"];
                    foreach (var item in Result)
                    {
                        var settlementAmount = (string)item["settlementAmount"];
                        var amountTotal = (string)item["amountTotal"];
                    }

                    return new MoneyOutPaymentTransferResponse
                    {
                        orderPaymentNumber = orderPaymentNumber
                    };
                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new MoneyOutPaymentTransferResponse
                    {
                        orderPaymentNumber = orderPaymentNumber,
                        error = ""
                    };
                }
            }
        }

        public void UpdateBenifisary(string benifisaryId, string associateId, string accountNumber, string associateEmail)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {

                var parameters = new
                {
                    Date = DateTime.Now,
                    BenifisaryId = benifisaryId,
                    AssociateId = associateId,
                    AccountNumber = accountNumber,
                    AssociateEmail = associateEmail
                };
                var sql = @"UPDATE Client.AssociateBeneficiaryInfo SET last_modified = @Date, BeneficiaryID = @BenifisaryId, AccountNumber = @AccountNumber, BeneficiaryEmailID = @AssociateEmail WHERE AssociateID = @AssociateId";
                dbConnection.Execute(sql, parameters);
            }
        }

        public void InsertBenifisary(string benifisaryId, string associateId, string accountNumber, string associateEmail)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {

                var parameters = new
                {
                    Date = DateTime.Now,
                    BenifisaryId = benifisaryId,
                    AssociateId = associateId,
                    AccountNumber = accountNumber,
                    AssociateEmail = associateEmail
                };

                var sql = @"INSERT INTO Client.AssociateBeneficiaryInfo (last_modified, [AssociateID], [BeneficiaryID], [AccountNumber],[BeneficiaryEmailID]) VALUES (@Date, @AssociateId, @BenifisaryId, @AccountNumber,@AssociateEmail)";
                dbConnection.Execute(sql, parameters);
            }
        }

        public GetBenefisaryResponse GetBenefisary(GetBenefisaryRequest req)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var query = @"SELECT BeneficiaryID,BeneficiaryEmailID,orderPaymentNumber FROM Client.AssociateBeneficiaryInfo WHERE AssociateId = @AssociateId";
                return dbConnection.QueryFirstOrDefault<GetBenefisaryResponse>(query, new { AssociateId = req.AssociateID }) ?? new GetBenefisaryResponse();
            }
        }

        public GetPaymentOrderResponse GetPaymentOrder(GetPaymentOrderRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/orders/" + req.OrderPaymentNumber);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    GetPaymentOrderResponse obj = new GetPaymentOrderResponse();
                    var contentJSON = JObject.Parse(contents);
                    var paymentResult = contentJSON["content"]["payments"];
                    foreach (var item in paymentResult)
                    {
                        obj.amount = (string)item["amount"];
                        obj.payeeName = (string)item["payeeName"];
                        obj.accountType = (string)item["accountType"];
                        obj.currency = (string)item["currency"];
                        obj.approvalStatus = (string)item["approvalStatus"];
                        obj.feeAmount = (string)item["feeAmount"];
                        obj.feeCurrency = (string)item["feeCurrency"];
                        obj.estimateCostAmount = (string)item["estimateCostAmount"];
                        obj.estimateCostCurrency = (string)item["estimateCostCurrency"];
                        obj.method = (string)item["method"];
                    }
                    var orderDetailResult = contentJSON["content"]["orderDetail"];
                    if (orderDetailResult != null)
                    {
                        obj.entryDate = (string)orderDetailResult["entryDate"];
                        obj.ordNum = (string)orderDetailResult["ordNum"];
                        obj.buy = (string)orderDetailResult["buy"];
                        obj.buyAmount = (string)orderDetailResult["buyAmount"];
                        obj.sell = (string)orderDetailResult["sell"];
                        obj.sellAmount = (string)orderDetailResult["sellAmount"];
                        obj.exchange = (string)orderDetailResult["exchange"];
                        obj.ourAction = (string)orderDetailResult["ourAction"];
                    }
                    return obj;
                }
                else
                {
                    var errorJSON = JObject.Parse(contents);
                    return new GetPaymentOrderResponse
                    {
                        error = "Order Not Found"
                    };
                }
            }
        }

        public SearchBenefisaryResponse SearchBenefisary(SearchBenefisaryRequest req)
        {
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(_cambridgeSetting.ApiUrl + "/api/" + _cambridgeSetting.ClientCode + "/0/benes?q=Status:A");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    var searchResult = contentJSON["content"]["data"]["rows"];
                    foreach (var item in searchResult)
                    {
                        var emailId = (string)item["email"];
                        if (emailId == req.BeneficiaryEmailID)
                        {
                            return new SearchBenefisaryResponse
                            {
                                BeneficiarySearchStatus = true
                            };
                        }
                    }
                    return new SearchBenefisaryResponse
                    {
                        BeneficiarySearchStatus = false
                    };
                }
                else
                {
                    dynamic err = JObject.Parse(contents);
                    return new SearchBenefisaryResponse
                    {
                        BeneficiarySearchStatus = false,
                        error = "Record Not Found"
                    };
                }
            }

        }


        public void InsertOrderPaymentNumber(int associateId, string orderNumber)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new
                {
                    Date = DateTime.Now,
                    OrderPaymentDate = DateTime.Now,
                    OrderPaymentNumber = orderNumber,
                    AssociateID = associateId,
                    BeneficiaryID = _cambridgeSetting.ClientId + "_" + associateId,
                    OrderDescription = "MoneyOut Payout"
                };

                var sql = @"INSERT INTO Client.AssociateBeneficiaryOrderPaymentInfo (last_modified,OrderPaymentDate,[AssociateID], [BeneficiaryID],[OrderDescription],[OrderPaymentNumber]) VALUES (@Date, @OrderPaymentDate, @AssociateID, @BeneficiaryID, @OrderDescription, @OrderPaymentNumber)";
                dbConnection.Execute(sql, parameters);
            }
        }

        public CurrencyRate GetCurrencyRate(string currency)
        {
            var parameters = new { currency };
            var query = @"SELECT CurrencyCode, ExchangeRate, last_modified FROM Currency WHERE CurrencyCode = @currency";

            using (var connection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                return connection.QueryFirst<CurrencyRate>(query, parameters);
            }
        }

        public void LogCurrencyRate(CurrencyRate currencyRate)
        {
            var parameters = new
            {
                currencyRate.CurrencyCode,
                Rate = currencyRate.ExchangeRate,
                StartDate = currencyRate.Date,
                EndDate = DateTime.Now.ToString()
            };

            var query = @"INSERT INTO client.CurrencyLog (CurrencyCode, Rate, StartDate, EndDate) VALUES (@CurrencyCode, @Rate, @StartDate, @EndDate)";

            using (var connection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                connection.Execute(query, parameters);
            }


        }

        public string BookOrder(string bookPaymentLink)
        {
            var Result = "";
            using (var httpClient = new HttpClient())
            {
                var url = new Uri(bookPaymentLink);
                var paymentRequest = new
                {
                };
                var stringContent = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("CMG-AccessToken", GetCambridgeAccessTokenInfo().AccessToken);
                HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result;
                var contents = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(contents))
                {
                    var contentJSON = JObject.Parse(contents);
                    Result = contentJSON["content"]["orderNumber"].ToString();
                }
            }
            return Result;
        }

        public void UpdateOrderPaymentNumber(int associateId, string orderNumber)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new
                {
                    Date = DateTime.Now,
                    OrderNumber = orderNumber
                };

                var sql = @"UPDATE Client.AssociateBeneficiaryInfo SET last_modified=@Date,orderPaymentNumber= @OrderNumber WHERE BeneficiaryID=" + _cambridgeSetting.ClientId + "_" + associateId + " )";
                dbConnection.Execute(sql, parameters);
            }
        }

        public int SetActiveCommissionMerchant(SetActiveCommissionMerchantRequest request)
        {
            var result = _moneyOutService.AutoProvisionAccount(request.AssociateId, request.MerchantId).Result;
            if (result.MerchantId != 0)
            {
                return 1;
            }
            return 0;
        }

    }
}
