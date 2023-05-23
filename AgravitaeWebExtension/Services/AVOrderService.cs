using AgravitaeAgravitaeWebExtension.Hooks;
using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using System.Data.SqlClient;

namespace AgravitaeAgravitaeWebExtension.Services
{
    public interface IAVOrderService
    {
        Task<LineItem[]> AddAdditionalItems(NewOrderDetail order, ILogger<SubmitOrderHook> logger);
        Task<List<CustomFields>> GetItemCustomFields(int itemId);
    }
    public class AVOrderService : IAVOrderService
    {
        private readonly IAssociateService _associateService;
        private readonly IItemService _itemService;
        private readonly IDataService _dataService;
        public AVOrderService(IDataService dataService, IAssociateService associateService, IItemService itemService)
        {
            _dataService = dataService;
            _associateService = associateService;
            _itemService = itemService;
        }

        public async Task<LineItem[]> AddAdditionalItems(NewOrderDetail order, ILogger<SubmitOrderHook> logger)
        {
            var lineItems = order.LineItems.ToList();
            try
            {
                if (order.ShipAddress.State.ToUpper().Equals("CA"))
                {   
                    var associateType = await _associateService.GetAssociate(order.AssociateId);
                    //Prop65Sticker
                    var promotionalItems = await _itemService.GetLineItemById(47, 1, "USD", "en", 1, (int)order.OrderType, associateType.AssociateType, order.StoreId, "us");
                    if (promotionalItems == null) throw new Exception($"Cannot find item '{47}'");
                    lineItems.Add(promotionalItems);
                }
                else
                {
                    lineItems = order.LineItems.ToList();
                }

                return lineItems.ToArray();
            }
            catch (Exception ex)
            {
                logger.LogError($"There was an error adding Prop65 item, for the Customer account: {order.AssociateId},  " + ex.Message);

                return lineItems.ToArray();
            }
        }

        public async Task<List<CustomFields>> GetItemCustomFields(int itemId)
        {
            await using var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result);            

            var query = @"SELECT * FROM INV_CustomFields WHERE ItemID = @ItemId";

            return dbConnection.Query<CustomFields>(query, new { ItemId = itemId }).ToList();
        }
    }
}
