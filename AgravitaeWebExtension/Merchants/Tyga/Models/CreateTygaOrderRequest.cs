
namespace AgravitaeExtension.Merchants.Tyga.Models
{
    public class CreateTygaOrderRequest
    {

        public string name { get; set; }
        public string description { get; set; }
        public string orderNumber { get; set; }
        public string amount { get; set; }
        public string email { get; set; }
        public string notifyUrl { get; set; }
        public string returnUrl { get; set; }
    }


}
