using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.PrintStudio.PublishingEngine;
using Sitecore.PrintStudio.PublishingEngine.Rendering;

namespace CustomAPS.Renderers
{
    class CommerceServerXmlImageFrameRenderer : InDesignItemRendererBase
    {
        protected override void RenderContent(PrintContext printContext, XElement output)
        {
            Item dataItem = this.GetDataItem(printContext);
            XElement xElement = RenderItemHelper.CreateXElement("ImageFrame", base.RenderingItem, printContext.Settings.IsClient, dataItem);
            xElement.SetAttributeValue("SitecoreFieldname", base.RenderingItem["item field"]);
            xElement.SetAttributeValue("SitecoreMediaID", base.RenderingItem["medialibrary reference"]);
            xElement.SetAttributeValue("ItemReferenceID", base.RenderingItem["item reference"]);
            xElement.SetAttributeValue("RenderingID", base.RenderingItem["xml renderer"]);
            xElement.SetAttributeValue("ItemReferenceDisplayName", (dataItem != null) ? dataItem.DisplayName : string.Empty);
            output.Add(xElement);
            this.RenderChildren(printContext, xElement);
        }
        protected override void BeginRender(PrintContext printContext)
        {
            if (string.IsNullOrEmpty(base.DataSource))
            {
                base.DataSource = base.RenderingItem["item reference"];
            }
        }
    }
}
