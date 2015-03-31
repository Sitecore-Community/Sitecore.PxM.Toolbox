using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Xml.Xsl;
using Sitecore.Commerce;
using Sitecore.Commerce.Services.Prices;

namespace CustomAPS.XSLmethods
{
    class GetPrice : Sitecore.Xml.Xsl.XslHelper
    {
        public string GetExternalPrice(string ExternalID)
        {
            string ListPriceKey = "List";

            var request = new GetProductPricesRequest(ExternalID);
            var response = new PricingServiceProvider();
            var price = response.GetProductPrices(request).Prices[ListPriceKey].Amount;

            return price.ToString("c");
        }
    }
}
