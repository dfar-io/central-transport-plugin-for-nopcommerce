using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.CentralTransport.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using static Nop.Services.Shipping.GetShippingOptionRequest;

namespace Nop.Plugin.Shipping.CentralTransport
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class CentralTransportComputationMethod :
        BasePlugin, IShippingRateComputationMethod
    {
        private readonly CentralTransportSettings _settings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IRateQuoteService _rateQuoteService;
        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        
        public CentralTransportComputationMethod(
            CentralTransportSettings settings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            IRateQuoteService rateQuoteService,
            IMeasureService measureService,
            IShippingService shippingService
        )
        {
            _settings = settings;
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _rateQuoteService = rateQuoteService;
            _measureService = measureService;
            _shippingService = shippingService;
        }

        public override string GetConfigurationPageUrl()
        {
            return
            $"{_webHelper.GetStoreLocation()}Admin/CentralTransport/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _settingService.SaveSetting(CentralTransportSettings.Default());


            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.AccessCode, "Access Code"
            );
            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.AccessCodeHint,
                "Code provided to your company by Central Transport."
            );

            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.CustomerNumber, "Customer Number"
            );
            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.CustomerNumberHint,
                "Central Transport customer account number."
            );

            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.ShipmentClass, "Shipment Class"
            );
            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.ShipmentClassHint,
                "Shipment class to use for rate quotes."
            );

            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.MinimumWeightLimitInLbs,
                "Minimum Weight Limit (in lbs)"
            );
            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.MinimumWeightLimitInLbsHint,
                "Weight limit to start checking for Central Transport weights"
            );

            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.ShippingOptionName,
                "Shipping Option Name"
            );
            _localizationService.AddOrUpdatePluginLocaleResource(
                CentralTransportLocales.ShippingOptionNameHint,
                "Name to show customer for shipping option."
            );

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<CentralTransportSettings>();

            // Delete all plugin locales using reflection
            foreach (var field in typeof(CentralTransportLocales).GetFields())
            {
                _localizationService.DeletePluginLocaleResource(
                    (string)field.GetValue(null)
                );
            }

            base.Uninstall();
        }

        public ShippingRateComputationMethodType ShippingRateComputationMethodType
            => ShippingRateComputationMethodType.Realtime;

        public IShipmentTracker ShipmentTracker => null;

        public GetShippingOptionResponse GetShippingOptions(
            GetShippingOptionRequest getShippingOptionRequest
        )
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(
                    nameof(getShippingOptionRequest)
                );

            // Validate that 'lb' measure weight exists in system
            var lbsMeasureWeight =
                _measureService.GetMeasureWeightBySystemKeyword("lb");
            if (lbsMeasureWeight == null)
            {
                return new GetShippingOptionResponse {
                    Errors = new[] { "Cannot perform weight conversion, unable " +
                    "to find 'lb' measure weight. Make sure a measure weight " +
                    "with 'lb' system keyword exists." }
                };
            }
            
            if (!getShippingOptionRequest.Items?.Any() ?? true)
            {
                return new GetShippingOptionResponse {
                    Errors = new[] { "No shipment items" }
                };
            }

            // Check that destination zip/postal was provided
            var destinationZip =
                getShippingOptionRequest.ShippingAddress?.ZipPostalCode;
            if (destinationZip == null)
            {
                return new GetShippingOptionResponse {
                    Errors = new[]
                    {
                        "Destination shipping zip/postal code is not set"
                    }
                };
            }
            
            var weight = _shippingService.GetTotalWeight(
                getShippingOptionRequest
            );
            var weightInLbs = _measureService.ConvertFromPrimaryMeasureWeight(
                weight, lbsMeasureWeight);

            // If under minimum, don't provide as option
            if (weightInLbs < _settings.MinimumWeightLimitInLbs)
            {
                return new GetShippingOptionResponse();
            }
            
            decimal rateQuote;
            
            try {
                rateQuote = _rateQuoteService.GetRateQuote(
                    destinationZip,
                    decimal.ToInt32(decimal.Round(weightInLbs))
                );
            }
            catch (NopException ex) {
                return new GetShippingOptionResponse {
                    Errors = new[] { $"{ex.Message}" }
                };
            }

            var response = new GetShippingOptionResponse();
            response.ShippingOptions.Add(new ShippingOption()
            {
                Name = _settings.ShippingOptionName,
                Rate = rateQuote
            });
            return response;
        }

        public decimal? GetFixedRate(
            GetShippingOptionRequest getShippingOptionRequest
        )
        {
            return 0.0M;
        }
    }
}