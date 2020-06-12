using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Shipping.CentralTransport.Models;
using Nop.Services.Messages;

namespace Nop.Plugin.Shipping.CentralTransport.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CentralTransportController : BasePluginController
    {
        private readonly CentralTransportSettings _settings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        public CentralTransportController(
            CentralTransportSettings settings,
            ISettingService settingService,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService
        )
        {
            _settings = settings;
            _settingService = settingService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(
                StandardPermissionProvider.ManageShippingSettings
            ))
            {
                return AccessDeniedView();
            }

            return View(
                "~/Plugins/Shipping.CentralTransport/Views/Configure.cshtml",
                _settings.ToModel()
            );
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(
                StandardPermissionProvider.ManageShippingSettings
            ))
            {
                return Content("Access denied");
            }

            _settingService.SaveSetting(
                CentralTransportSettings.FromModel(model)
            );

            _notificationService.SuccessNotification(
                _localizationService.GetResource("Admin.Plugins.Saved")
            );

            return Configure();
        }
    }
}