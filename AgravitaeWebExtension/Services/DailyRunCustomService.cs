using AgravitaeWebExtension.Repositories;
using RPMSEwallet.Services.Interface;
using ZiplingoEngagement.Models.Associate;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Services
{
    public interface IDailyRunCustomService
    {
        List<AutoshipInfo> GetNextFiveDayAutoships();
        List<CardInfo> GetCreditCardInfoBefore30Days();
        void UpdateAssociateStatusInEwallet();
        void UpdateAssociateStatusinUnify();
       
    }
    public class DailyRunCustomService : IDailyRunCustomService
    {
        private readonly IDailyRunCustomRepository _dailyrunRepository;
        private readonly IEwalletService _ewalletService;
        private readonly IZLAssociateService _zlAssociateService;
        public DailyRunCustomService(IDailyRunCustomRepository dailyrunRepository, IEwalletService ewalletService, IZLAssociateService zlAssociateService)
        {
            _dailyrunRepository = dailyrunRepository;
            _ewalletService = ewalletService;
            _zlAssociateService = zlAssociateService;
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
        public void UpdateAssociateStatusinUnify()
        {
            try
            {
                var associateStatuses = _dailyrunRepository.GetAssociateStatuses();
                _zlAssociateService.AssociateStatusSync(associateStatuses);


            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
