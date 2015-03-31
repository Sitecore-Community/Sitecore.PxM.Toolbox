using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.PrintStudio.PublishingEngine.Rendering;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.PrintStudio.PublishingEngine;
using Sitecore.Commerce;
using Sitecore.Commerce.Services.Prices;
using Sitecore.Obec.StarterKit.Services;

namespace CustomAPS.Renderers
{
    public class GetPrice : XmlTextFrameRenderer 
    {

        protected override void RenderContent(PrintContext printContext, XElement output)
        {
            string ListPriceKey = "List";

            XElement baseXml = new XElement("base");
            base.RenderContent(printContext, baseXml);

            XElement textFrame = baseXml.Element("TextFrame");
            Item dataItem = GetDataItem(printContext);

            XElement xElement = RenderItemHelper.CreateXElement("TextFrame", base.RenderingItem, printContext.Settings.IsClient, dataItem);
            this.SetAttributes(xElement);

            output.Add(xElement);

            XAttribute xAttribute = output.Attribute("ParagraphStyle");
            string text = (xAttribute != null && !string.IsNullOrEmpty(xAttribute.Value)) ? xAttribute.Value : "H1 Orange";

            Field fieldname = dataItem.Fields[this.ContentFieldName];
            DateField dateField = fieldname;

            var request = new GetProductPricesRequest(dataItem["ExternalID"]);
            var response = new PricingServiceProvider();
            var price = response.GetProductPrices(request).Prices[ListPriceKey].Amount;
            //var price = response.Prices.ContainsKey(ListPriceKey) ? response.Prices[ListPriceKey].Amount : decimal.Zero;

            //var price = this.pricingService.GetProductPrice("11");

            IEnumerable<XElement> result = this.FormatText(text, price.ToString("c"));

            xElement.Add(result);

            this.RenderChildren(printContext, xElement);
        }
    }
}
