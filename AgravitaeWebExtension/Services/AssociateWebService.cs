using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension;
using AgravitaeWebExtension.Repositories;
using ZiplingoEngagement.Models.Orders;

namespace AgravitaeWebExtension.Services
{
    public interface IAssociateWebService
    {
        List<ShipMethods> GetShipMethods();
    }
    public class AssociateWebService : IAssociateWebService
    {
        private readonly IAssociateWebRepository _associateWebRepository;
        private readonly IAssociateService _associateService;
        public AssociateWebService(IAssociateService associateService, IAssociateWebRepository associateWebRepository)
        {
            _associateWebRepository = associateWebRepository ?? throw new ArgumentNullException(nameof(associateWebRepository));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
        }
        public List<ShipMethods> GetShipMethods()
        {
            return _associateWebRepository.GetShipMethods();
        }
    }
}
