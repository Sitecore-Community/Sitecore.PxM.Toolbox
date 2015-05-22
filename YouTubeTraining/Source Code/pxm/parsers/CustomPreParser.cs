using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using HtmlAgilityPack;
using Sitecore.PrintStudio.PublishingEngine.Helpers;
using Sitecore.PrintStudio.PublishingEngine.Text;
using Sitecore.PrintStudio.PublishingEngine.Text.Parsers.Html;
using Sitecore.Diagnostics;

namespace LaunchSitecore.pxm.parsers
{
    public class CustomPreParser : HtmlNodeParser
    {
        protected override void ParseChildNodes(HtmlNode htmlNode, XElement resultElement, ParseContext parseContext, XElement baseFormattingElement)
        {
            foreach (HtmlNode current in (System.Collections.Generic.IEnumerable<HtmlNode>)htmlNode.ChildNodes)
            {
                    this.ParseChildNode(current, resultElement, parseContext, baseFormattingElement);
                    //Log.Info("Current nodetype" + current.OriginalName, this);
                    if (current.OriginalName == "#text")
                    {
                        XElement linereturn = new XElement("SpecialCharacter");
                        linereturn.SetAttributeValue("Type", "10");
                        resultElement.Add(linereturn);
                    }                
            }
        }
    }
}