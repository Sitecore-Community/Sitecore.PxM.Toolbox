using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.PrintStudio.PublishingEngine;
using Sitecore.PrintStudio.PublishingEngine.Rendering;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;


namespace Sitecore72aps.Renderers
{
    public class FitTextToFrame : XmlTextFrameRenderer
    {
        protected override void RenderContent(PrintContext printContext, XElement output)
        {
            XElement baseXml = new XElement("base");
            base.RenderContent(printContext, baseXml);

            XElement textFrame = baseXml.Element("TextFrame");
            Item dataItem = GetDataItem(printContext);

            XElement xElement = RenderItemHelper.CreateXElement("TextFrame", base.RenderingItem, printContext.Settings.IsClient, dataItem);
            this.SetAttributes(xElement);

            output.Add(xElement);

            XAttribute xAttribute = output.Attribute("ParagraphStyle");

            string ParagraphStyle = base.RenderingItem["ParagraphStyle"];

            int ContentLength = dataItem.Fields[this.ContentFieldName].ToString().Length;
            decimal textFrameWidthDecimal = Convert.ToDecimal(base.RenderingItem["Width"].ToString());
            int textFrameWidthInt = Convert.ToInt32(textFrameWidthDecimal);

            XElement format = RenderItemHelper.CreateXElement("Format", base.RenderingItem, printContext.Settings.IsClient);

            int fontsize = textFrameWidthInt / ContentLength;
            fontsize = fontsize * 5;
            string fontsizeattrib = fontsize.ToString();

            format.SetAttributeValue("FontSize", fontsizeattrib);

            format.SetValue(dataItem.Fields[this.ContentFieldName].ToString());

            XElement result = RenderItemHelper.CreateXElement("ParagraphStyle", base.RenderingItem, printContext.Settings.IsClient);
            result.SetAttributeValue("Style", ParagraphStyle);

            result.Add(format);

            xElement.Add(result);

            this.RenderChildren(printContext, xElement);
        }
    }
}