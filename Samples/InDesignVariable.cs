using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.PrintStudio.PublishingEngine;
using Sitecore.PrintStudio.PublishingEngine.Rendering;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.PrintStudio.PublishingEngine.Helpers;
using Sitecore.Collections;


namespace Sitecore72aps.Renderers
{
    public class InDesignVariable : InDesignItemRendererBase
    {
        protected override void RenderContent(PrintContext printContext, XElement output)
        {
            XElement parentNode = new XElement((XName)"Variables");

                XElement page = this.CreateVariable(printContext, false, false);
                ++printContext.PageCount;
                this.RenderChildren(printContext, page);
                output.Add((object)page);
        }
        protected virtual XElement CreateVariable(PrintContext printContext, bool skipNumber, bool emptyNumber)
        {
            Item renderingItem = this.RenderingItem;
            XElement xelement = RenderItemHelper.CreateXElement("Variable", renderingItem, printContext.Settings.IsClient, (Item)null, true);

            if (renderingItem.Fields["Find"] != null && renderingItem.Fields["Replace"] != null)
            {
                if (printContext.Settings.IsClient && renderingItem.Fields["PageXML"] != null)
                    XElementExtensions.AddInnerXml(xelement, renderingItem.Fields["PageXML"].Value, false);

                var findElement = new XElement("Find", (object)renderingItem["Find"]);
                xelement.Add(findElement);

                if (printContext.Settings.Parameters.ContainsKey("Variables") &&
                (SafeDictionary<string, object>)printContext.Settings.Parameters["Variables"] != null)
                {
                    var variables = (SafeDictionary<string, object>)printContext.Settings.Parameters["Variables"];
                    var variable = variables.FirstOrDefault(v => v.Key == (string)renderingItem["Find"]);
                    var replaceElement = new XElement("Replace", (object)variable.Value);
                    xelement.Add(replaceElement);
                }
                else
                {
                    var replaceElement = new XElement("Replace", (object)renderingItem["Replace"]);
                    xelement.Add(replaceElement);
                }

            }

            return xelement;
        }
    }
}