using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Shipping.CentralTransport.Models
{
    public class ConfigurationModel : BaseSearchModel
    {
        [Required]
        [NopResourceDisplayName(CentralTransportLocales.AccessCode)]
        public string AccessCode { get; set; }

        [Required]
        [NopResourceDisplayName(CentralTransportLocales.CustomerNumber)]
        public string CustomerNumber { get; set; }

        [Required]
        [NopResourceDisplayName(CentralTransportLocales.ShipmentClass)]
        public string ShipmentClass { get; set; }

        [Required]
        [NopResourceDisplayName(CentralTransportLocales.MinimumWeightLimitInLbs)]
        public decimal MinimumWeightLimitInLbs { get; set; }

        [Required]
        [NopResourceDisplayName(CentralTransportLocales.ShippingOptionName)]
        public string ShippingOptionName { get; set; }

        public IList<SelectListItem> AvailableShipmentClasses { get; private set; }

        public ConfigurationModel()
        {
            AvailableShipmentClasses = new List<SelectListItem>
            { 
                new SelectListItem("50", "50"),
                new SelectListItem("55", "55"),
                new SelectListItem("60", "60"),
                new SelectListItem("65", "65"),
                new SelectListItem("70", "70"),
                new SelectListItem("77.5", "77.5"),
                new SelectListItem("85", "85"),
                new SelectListItem("92.5", "92.5"),
                new SelectListItem("100", "100"),
                new SelectListItem("110", "110"),
                new SelectListItem("125", "125"),
                new SelectListItem("150", "150"),
                new SelectListItem("175", "175"),
                new SelectListItem("200", "200"),
                new SelectListItem("250", "250"),
                new SelectListItem("300", "300"),
                new SelectListItem("400", "400"),
                new SelectListItem("500", "500")
            };
        }
    }
}