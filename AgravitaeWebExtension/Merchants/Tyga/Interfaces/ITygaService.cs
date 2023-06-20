using DirectScale.Disco.Extension;
using System.Threading.Tasks;
using AgravitaeExtension.Merchants.Tyga.Models;


namespace AgravitaeExtension.Merchants.Tyga.Interfaces
{
    public interface ITygaService
    {

        Task<CreateTygaOrderResponse> CreateOrder(string methodname, CreateTygaOrderRequest request);
        Task SaveErrorLogResponse(int associateId, int orderId, string message, string error);
        Task CreateTygaOrderLogs(int associateId, int orderId, CreateTygaOrderResponse request);
        Task<PaymentStatus> UpdateOrderStatus(TygaPaymentResponse request);
    }
}
