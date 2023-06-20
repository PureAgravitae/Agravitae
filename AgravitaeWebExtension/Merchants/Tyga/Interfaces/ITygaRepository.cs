using System.Threading.Tasks;
using AgravitaeExtension.Merchants.Tyga.Models;


namespace AgravitaeExtension.Merchants.Tyga.Interfaces
{
    public interface ITygaRepository
    {
        string CreateOrder(string payorId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string cardNumber, string transactionNumber, string authorizationCode, string discoNotes);
        Task SaveErrorLogResponse(int associateId, int orderId, string message, string error);
        Task CreateTygaOrderLogs(int associateId, int orderId, CreateTygaOrderResponse request);
        Task UpdateTygaOrderLogs(TygaPaymentResponse req);
        Task<TygaOrder> GetTygaOrderbyOrderId(string orderId);
        Task<TygaSettings> GetTygaSettings();
    }
}
