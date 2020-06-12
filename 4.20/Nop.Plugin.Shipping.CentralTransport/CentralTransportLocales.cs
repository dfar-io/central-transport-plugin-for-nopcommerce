using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.CentralTransport
{
    public static class CentralTransportLocales
    {
        private const string Base = "Plugins.Shipping.CentralTransport";

        public const string AccessCode = Base + ".Fields.AccessCode";
        public const string AccessCodeHint = AccessCode + ".Hint";

        public const string CustomerNumber = Base + ".Fields.CustomerNumber";
        public const string CustomerNumberHint = CustomerNumber + ".Hint";

        public const string ShipmentClass = Base + ".Fields.ShipmentClass";
        public const string ShipmentClassHint = ShipmentClass + ".Hint";

        public const string MinimumWeightLimitInLbs = Base + ".Fields.MinimumWeightLimitInLbs";
        public const string MinimumWeightLimitInLbsHint = MinimumWeightLimitInLbs + ".Hint";

        public const string ShippingOptionName = Base + ".Fields.ShippingOptionName";
        public const string ShippingOptionNameHint = ShippingOptionName + ".Hint";
    }
}