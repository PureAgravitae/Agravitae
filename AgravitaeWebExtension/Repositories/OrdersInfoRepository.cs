using Dapper;
using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using AgravitaeWebExtension.Models.GenericReports;
using AgravitaeWebExtension.Models.OrdersInfo;

namespace AgravitaeWebExtension.Repositories
{
    public interface IOrdersInfoRepository
    {
        Task<IEnumerable<QtyPerSKU>> GetItemOrderHistoryCountPerAssociate(int associateID, params string[] skus);
    }
    public class OrdersInfoRepository : IOrdersInfoRepository
    {
        private readonly IDataService _dataService;

        public OrdersInfoRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public async Task<IEnumerable<QtyPerSKU>> GetItemOrderHistoryCountPerAssociate(int associateID, params string[] skus)
        {
            using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                return await dbConnection.QueryAsync<QtyPerSKU>(GetItemOrderHistoryCountQuery, new { SKUs = skus, DistributorId = associateID });
            }
        }

        private const string GetItemOrderHistoryCountQuery = @"
select 
	 od.SKU as SKU, sum(od.Qty) as Qty
from 
	ORD_Order o
join
	ORD_OrderDetail od
	on
		o.recordnumber = od.OrderNumber
where
	od.SKU in @SKUs and o.DistributorID = @DistributorId 
	and o.Status not in ('refunded', 'payment declined')
group by 
    o.DistributorID, od.SKU
";

    }
}
