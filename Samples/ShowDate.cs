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
    public class ShowDate : XmlTextFrameRenderer
    {
        public string Format { get; set; }

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
            string text = (xAttribute != null && !string.IsNullOrEmpty(xAttribute.Value)) ? xAttribute.Value : "NormalParagraphStyle";

            Field fieldname = dataItem.Fields[this.ContentFieldName];
            DateField dateField = fieldname;

            string dateoutput = dateField.DateTime.ToString(Format);

            IEnumerable<XElement> result = this.FormatText(text, dateoutput);

            xElement.Add(result);

            this.RenderChildren(printContext, xElement);
        }
    }
}