using Nop.Core.Configuration;
using Nop.Plugin.Shipping.CentralTransport.Models;

namespace Nop.Plugin.Shipping.CentralTransport
{
    public class CentralTransportSettings : ISettings
    {
        public string AccessCode { get; private set; }
        public string CustomerNumber { get; private set; }
        public string ShipmentClass { get; private set; }
        public decimal MinimumWeightLimitInLbs { get; private set; }
        public string ShippingOptionName { get; private set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AccessCode) &&
                       !string.IsNullOrWhiteSpace(CustomerNumber) &&
                       !string.IsNullOrWhiteSpace(ShipmentClass) &&
                       !string.IsNullOrWhiteSpace(ShippingOptionName);
            }
        }

        public static CentralTransportSettings FromModel(
            ConfigurationModel model
        ) {
            return new CentralTransportSettings()
            {
                AccessCode = model.AccessCode,
                CustomerNumber = model.CustomerNumber,
                ShipmentClass = model.ShipmentClass,
                MinimumWeightLimitInLbs = model.MinimumWeightLimitInLbs,
                ShippingOptionName = model.ShippingOptionName
            };
        }

        public static CentralTransportSettings Default()
        {
            return new CentralTransportSettings()
            {
                ShippingOptionName = "Ground Freight"
            };
        }

        public ConfigurationModel ToModel()
        {
            return new ConfigurationModel()
            {
                AccessCode = AccessCode,
                CustomerNumber = CustomerNumber,
                ShipmentClass = ShipmentClass,
                MinimumWeightLimitInLbs = MinimumWeightLimitInLbs,
                ShippingOptionName = ShippingOptionName
            };
        }
    }
}