using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System;

namespace WebExtension.Merchants.CambridgeMerchant.Services
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
                EnvironmentType envType = _settings.ExtensionContext().Result.EnvironmentType;
                if (envType == EnvironmentType.Live)
                {
                    return true;
                }

                return false;
            }
        }

        public string ClientCode
        {
            get
            {
                if (!IsLive)
                {
                    return "280086";
                }

                return "280086";
            }
        }

        public string ClientId
        {
            get
            {
                if (!IsLive)
                {
                    return "280086_API_User";
                }

                return "280086_API_User";
            }
        }

        public string Message
        {
            get
            {
                if (!IsLive)
                {
                    return "Cambridge";
                }

                return "Cambridge";
            }
        }

        public string SettlementAccountId
        {
            get
            {
                if (!IsLive)
                {
                    return "001";
                }

                return "001";
            }
        }

        public string SettlementMethod
        {
            get
            {
                if (!IsLive)
                {
                    return "EFT";
                }

                return "EFT";
            }
        }

        public string SettlementCurrency
        {
            get
            {
                if (!IsLive)
                {
                    return "USD";
                }

                return "USD";
            }
        }

        public string Key
        {
            get
            {
                if (!IsLive)
                {
                    return "7042c7fb7e884a6e8ba776e13545c94dWZdEchFkt5LWDNc2";
                }

                return "7042c7fb7e884a6e8ba776e13545c94dWZdEchFkt5LWDNc2";
            }
        }

        public string IssuerID
        {
            get
            {
                if (!IsLive)
                {
                    return "280086_API_User";
                }

                return "280086_API_User";
            }
        }

        public string ApiUrl
        {
            get
            {
                if (!IsLive)
                {
                    return "https://beta.cambridgelink.com";
                }

                return "https://cambridgelink.com";
            }
        }
        public string PartnerKey
        {
            get
            {
                    return "5241fb90f8454890a0fa299f5a3ee75aHExqr3RZV1QU3_Dg";
            }
        }
        public string PartnerIssuerID
        {
            get
            {
                return "ZIPLINGO";
            }
        }
    }
}
