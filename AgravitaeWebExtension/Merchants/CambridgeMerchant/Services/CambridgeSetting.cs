using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Services
{
    public interface ICambridgeSetting
    {
        bool IsLive { get; }
        string ClientCode { get; }
        string ClientId { get; }
        string Message { get; }
        string SettlementAccountId { get; }
        string SettlementMethod { get; }
        string SettlementCurrency { get; }
        string Key { get; }
        string IssuerID { get; }
        string ApiUrl { get; }
        string PartnerKey { get; }
        string PartnerIssuerID { get; }
    }


    public class CambridgeSetting : ICambridgeSetting
    {
        private readonly ISettingsService _settings;

        public CambridgeSetting(ISettingsService settingsService)
        {
            _settings = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }

        //public int BackupMerchantId => _settings.GetVariable("IPayout_BackupMerchantId", CompanyId, 0);

        public bool IsLive
        {
            get
            {
                return false;
            }
        }

        public string ClientCode
        {
            get
            {

                return "";
            }
        }

        public string ClientId
        {
            get
            {
                

                return "";
            }
        }

        public string Message
        {
            get
            {

                return "";
            }
        }

        public string SettlementAccountId
        {
            get
            {
               

                return "";
            }
        }

        public string SettlementMethod
        {
            get
            {
                

                return "";
            }
        }

        public string SettlementCurrency
        {
            get
            {

                return "";
            }
        }

        public string Key
        {
            get
            {

                return "";
            }
        }

        public string IssuerID
        {
            get
            {

                return "";
            }
        }

        public string ApiUrl
        {
            get
            {

                return "";
            }
        }
        public string PartnerKey
        {
            get
            {
                    return "";
            }
        }
        public string PartnerIssuerID
        {
            get
            {
                return "";
            }
        }
    }
}
