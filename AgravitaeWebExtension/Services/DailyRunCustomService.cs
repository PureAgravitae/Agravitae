using AgravitaeWebExtension.Repositories;
using RPMSEwallet.Services.Interface;
using ZiplingoEngagement.Models.Associate;

namespace AgravitaeWebExtension.Services
{
    public interface IDailyRunCustomService
    {
        List<AutoshipInfo> GetNextFiveDayAutoships();
        List<CardInfo> GetCreditCardInfoBefore30Days();
        void UpdateAssociateStatusInEwallet();
    }
    public class DailyRunCustomService : IDailyRunCustomService
    {
        private readonly IDailyRunCustomRepository _dailyrunRepository;
        private readonly IEwalletService _ewalletService;
        public DailyRunCustomService(IDailyRunCustomRepository dailyrunRepository, IEwalletService ewalletService)
        {
            _dailyrunRepository = dailyrunRepository;
            _ewalletService = ewalletService;
        }
        public List<AutoshipInfo> GetNextFiveDayAutoships()
        {
            return _dailyrunRepository.GetNextFiveDayAutoships();
        }
        public List<CardInfo> GetCreditCardInfoBefore30Days()
        {
            return _dailyrunRepository.GetCreditCardInfoBefore30Days();
        }
        public void UpdateAssociateStatusInEwallet()
        {
            try
            {
                var associateStatuses = _dailyrunRepository.GetAssociateStatuses();

                if (associateStatuses != null)
                {
                    foreach (var Associateids in associateStatuses)
                    {
                        if (Associateids.CurrentStatusId == 2 || Associateids.CurrentStatusId == 5)
                        {
                            _ewalletService.ChangeCustomerStatus(Associateids.AssociateID, 0);
                        }
                        else
                        {
                            _ewalletService.ChangeCustomerStatus(Associateids.AssociateID, 1);
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
