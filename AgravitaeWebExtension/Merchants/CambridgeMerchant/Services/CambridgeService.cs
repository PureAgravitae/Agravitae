using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using WebExtension.Merchants.CambridgeMerchant.Models;
using WebExtension.Merchants.CambridgeMerchant.Services.Models;
using WebExtension.Merchants.EwalletMerchant.Models;

namespace WebExtension.Merchants.CambridgeMerchant.Services
{
    public interface ICambridgeService
    {
        //CambridgeSettingsResponse GetCambridgeSettingsInfo();
        IBANValidationResponse IBANValidation(IBANValidationRequest req);
        List<BankSearchResponse> BankSearch(BankSearchRequest req);
        GetBeneRulesResponse GetBenefisaryRules(GetBenefisaryRulesRequest req);
        List<GetCountriesResponse> GetCountries();
        List<GetCitiesResponse> GetCities(GetCitiesRequest req);
        List<GetRegionsResponse> GetRegions(GetRegionsRequest req);
        List<GetCurrenciesResponse> GetCurrencies();
        string GetApiToken();
        CreateBenefisaryResponse CreateBenefisary(CreateBenefisaryRequest req);
        GetBenefisaryResponse GetBenefisary(GetBenefisaryRequest req);
        SearchBenefisaryResponse SearchBenefisary(SearchBenefisaryRequest req);
        //MoneyOutPaymentTransferResponse MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest req);
        GetPaymentOrderResponse GetPaymentOrder(GetPaymentOrderRequest req);
        List<GetAssociatePaymentOrderResponse> GetAssociatePaymentOrder(GetAssociatePaymentOrderRequest req);
        BenefisaryDetailsResponse GetBenefisaryDetails(GetBenefisaryRequest req);
        DeleteBenefisaryResponse DeleteBenefisary(GetBenefisaryRequest req);
        CreateBenefisaryResponse EditBenefisary(CreateBenefisaryRequest req);
        int SetActiveCommissionMerchant(SetActiveCommissionMerchantRequest req);
        MoneyOutPaymentTransferResponse MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest req);
        GetPaymentRateInfoResponse GetPamentRateInfo(GetPaymentRateInfoRequest req);
        bool BenefisarySetDefault(BenefisarySetDefaultRequest rObject);
    }
    public class CambridgeService : ICambridgeService
    {
        private readonly ICambridgeRepository _cambridgeRepository;
        //private readonly ILoggingService _loggingService;
        public CambridgeService(ICambridgeRepository cambridgeRepository)
        {
            //_loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _cambridgeRepository = cambridgeRepository ?? throw new ArgumentNullException(nameof(cambridgeRepository));
        }
        //public CambridgeSettingsResponse GetCambridgeSettingsInfo()
        //{
        //    return _cambridgeRepository.GetCambridgeSettingsInfo();
        //}
        public IBANValidationResponse IBANValidation(IBANValidationRequest req)
        {
            return _cambridgeRepository.IBANValidation(req);
        }
        public List<BankSearchResponse> BankSearch(BankSearchRequest req)
        {
            return _cambridgeRepository.BankSearch(req);
        }
        public GetBeneRulesResponse GetBenefisaryRules(GetBenefisaryRulesRequest req)
        {
            return _cambridgeRepository.GetBenefisaryRules(req);
        }
        public List<GetCountriesResponse> GetCountries()
        {
            return _cambridgeRepository.GetCountries();
        }
        public List<GetCitiesResponse> GetCities(GetCitiesRequest req)
        {
            return _cambridgeRepository.GetCities(req);
        }

        public List<GetRegionsResponse> GetRegions(GetRegionsRequest req)
        {
            return _cambridgeRepository.GetRegions(req);
        }

        public List<GetCurrenciesResponse> GetCurrencies()
        {
            return _cambridgeRepository.GetCurrencies();
        }
        public string GetApiToken()
        {
            return _cambridgeRepository.GetApiToken();
        }
        public List<GetAssociatePaymentOrderResponse> GetAssociatePaymentOrder(GetAssociatePaymentOrderRequest req)
        {
            return _cambridgeRepository.GetAssociatePaymentOrder(req);
        }
        public CreateBenefisaryResponse CreateBenefisary(CreateBenefisaryRequest req)
        {
            return _cambridgeRepository.CreateBenefisary(req, false);
        }
        public CreateBenefisaryResponse EditBenefisary(CreateBenefisaryRequest req)
        {
            return _cambridgeRepository.CreateBenefisary(req, true);
        }
        public GetBenefisaryResponse GetBenefisary(GetBenefisaryRequest req)
        {
            return _cambridgeRepository.GetBenefisary(req);
        }
        public GetPaymentOrderResponse GetPaymentOrder(GetPaymentOrderRequest req)
        {
            return _cambridgeRepository.GetPaymentOrder(req);
        }
        public SearchBenefisaryResponse SearchBenefisary(SearchBenefisaryRequest req)
        {
            return _cambridgeRepository.SearchBenefisary(req);
        }
        //public MoneyOutPaymentTransferResponse MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest req)
        //{
        //    return _cambridgeRepository.MoneyOutPaymentTransferToCambridge(req);
        //}
        public BenefisaryDetailsResponse GetBenefisaryDetails(GetBenefisaryRequest req)
        {
            return _cambridgeRepository.GetBenefisaryDetails(req);
        }
        public DeleteBenefisaryResponse DeleteBenefisary(GetBenefisaryRequest req)
        {
            return _cambridgeRepository.DeleteBenefisary(req);
        }
        public int SetActiveCommissionMerchant(SetActiveCommissionMerchantRequest req)
        {
            return _cambridgeRepository.SetActiveCommissionMerchant(req);
        }
        public MoneyOutPaymentTransferResponse MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest req)
        {
            return _cambridgeRepository.MoneyOutPaymentTransferToCambridge(req);
        }
        public GetPaymentRateInfoResponse GetPamentRateInfo(GetPaymentRateInfoRequest req)
        {
            return _cambridgeRepository.GetPamentRateInfo(req);
        }
        public bool BenefisarySetDefault(BenefisarySetDefaultRequest req)
        {
            return _cambridgeRepository.BenefisarySetDefault(req);
        }
    }
}
