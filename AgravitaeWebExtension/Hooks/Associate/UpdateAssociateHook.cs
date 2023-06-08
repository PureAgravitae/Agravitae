﻿using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using DirectScale.Disco.Extension.Hooks.Associates;
using AgravitaeWebExtension.Repositories;
using AgravitaeWebExtension.Services;

using ZiplingoEngagement.Services.Interface;
using RPMSEwallet.Services.Interface;

namespace AgravitaeWebExtension.Hooks.Associate
{
    public class UpdateAssociateHook : IHook<UpdateAssociateHookRequest, UpdateAssociateHookResponse>
    {
        private readonly IEwalletService _ewalletService;
        private readonly IAssociateService _associateService;
        private readonly ICustomLogRepository _customLogRepository;
        private readonly IZLAssociateService _zlassociateServices;




        //private readonly IAssociateWebService _customAssociateService;

        public UpdateAssociateHook
        (
            IEwalletService ewalletService,
            IAssociateService associateService, 
            ICustomLogRepository customLogRepository,
            IZLAssociateService zlassociateservices
        //IAssociateWebService customAssociateService
        )
        {
            _ewalletService = ewalletService ?? throw new ArgumentNullException(nameof(ewalletService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _customLogRepository = customLogRepository ?? throw new ArgumentNullException(nameof(customLogRepository));
            _zlassociateServices = zlassociateservices ?? throw new ArgumentNullException(nameof(zlassociateservices));

            //_customAssociateService = customAssociateService;
        }

        public async Task<UpdateAssociateHookResponse> Invoke(UpdateAssociateHookRequest request, Func<UpdateAssociateHookRequest, Task<UpdateAssociateHookResponse>> func)
        {
            var oldAssociateType = request.OldAssociateInfo.AssociateBaseType;
            var newAssociateType = request.UpdatedAssociateInfo.AssociateBaseType;
            var associateId = request.UpdatedAssociateInfo.AssociateId;

            var result = await func(request);

            try
            {
                //if (oldAssociateType == 5 &&
                //    (newAssociateType == 1 || newAssociateType == 3 || newAssociateType == 6))
                //{
                //    _customAssociateService.PlaceInBinaryTree(associateId);
                //}

                  var OldAssociateType = await _associateService.GetAssociateTypeName(oldAssociateType);
                var UpdatedAssociateType = await _associateService.GetAssociateTypeName(newAssociateType);
                if (request.OldAssociateInfo.AssociateBaseType != request.UpdatedAssociateInfo.AssociateBaseType)
                {
                  await  _zlassociateServices.AssociateTypeChange(associateId, OldAssociateType, UpdatedAssociateType, newAssociateType);
                }
                var associate = await _associateService.GetAssociate(associateId);
                await _zlassociateServices.UpdateContact(associate);
                await _ewalletService.UpdateCustomer(associate);
            }
            catch (Exception ex)
            {
                _customLogRepository.CustomErrorLog(request.OldAssociateInfo.AssociateBaseType, request.UpdatedAssociateInfo.AssociateBaseType, "", "Error : " + ex.Message);
            }

            return result;
        }
    }
}