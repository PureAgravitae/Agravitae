using AgravitaeWebExtension.Repositories;
using ZiplingoEngagement.Models.Associate;

namespace AgravitaeWebExtension.Services
{
    public interface IDailyRunCustomService
    {
        List<AutoshipInfo> GetNextFiveDayAutoships();
        List<CardInfo> GetCreditCardInfoBefore30Days();
    }
    public class DailyRunCustomService : IDailyRunCustomService
    {
        private readonly IDailyRunCustomRepository _dailyrunRepository;
        public DailyRunCustomService(IDailyRunCustomRepository dailyrunRepository)
        {
            _dailyrunRepository = dailyrunRepository;
        }
        public List<AutoshipInfo> GetNextFiveDayAutoships()
        {
            return _dailyrunRepository.GetNextFiveDayAutoships();
        }
        public List<CardInfo> GetCreditCardInfoBefore30Days()
        {
            return _dailyrunRepository.GetCreditCardInfoBefore30Days();
        }
    }
}
