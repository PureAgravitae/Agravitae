
using Newtonsoft.Json;

namespace AgravitaeExtension.Merchants.Tyga.Models
{
    public class CreateTygaOrderResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public CreateOrderResponseData Data { get; set; }
    }
    public class CreateOrderResponseData
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("paymentUrl")]
        public string PaymentUrl { get; set; }
    }

}
