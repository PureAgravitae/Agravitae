using AgravitaeWebExtension.Models;
using Dapper;
using DirectScale.Disco.Extension.Services;
using System.Data.SqlClient;

namespace AgravitaeWebExtension.Repositories
{
    public interface IAssociateWebRepository
    {
        List<ShipMethods> GetShipMethods();
    }
    public class AssociateWebRepository : IAssociateWebRepository
    {
        private readonly IDataService _dataService;
        public AssociateWebRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }
        public List<ShipMethods> GetShipMethods()
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().GetAwaiter().GetResult()))
            {
                var queryStatement = "select recordnumber as ShipMethodId, MethodName as ShipMethodName ,CASE  WHEN CAST(last_modified AS DATE) >= CAST(GETDATE() - 1 AS DATE) THEN 'true' ELSE 'false' END AS isUpdated from dbo.ORD_ShippingMethods";

                return dbConnection.Query<ShipMethods>(queryStatement).ToList();
            }
        }
    }
}
