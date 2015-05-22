using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.PrintStudio.PublishingEngine.Text;
using Sitecore.PrintStudio.PublishingEngine.Text.Parsers.Html;
using HtmlAgilityPack;
using System.Xml.Linq;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Resources.Media;
using Sitecore;
using Sitecore.PrintStudio.Configuration;
using Sitecore.PrintStudio.PublishingEngine;
using Sitecore.PrintStudio.PublishingEngine.Rendering;

namespace LaunchSitecore.pxm.parsers
{
    public class CustomImageParser : HtmlNodeParser
    {
        public override void ParseNode(HtmlNode htmlNode, XElement resultElement, ParseContext parseContext, XElement baseFormattingElement)
        {
            XElement xElement = null;
            HtmlAttribute htmlAttribute = htmlNode.Attributes["src"];
            if (htmlAttribute != null)
            {
                MediaItem mediaItem = this.GetMediaItem(htmlAttribute.Value);
                if (mediaItem != null)
                {
                    string text;
                    string text2;
                    StyleParser.ParseDimensions(htmlNode, out text, out text2);
                    if (string.IsNullOrEmpty(text))
                    {
                        text = HtmlParseHelper.ParseDimensionValue(mediaItem.InnerItem["Width"], true);
                    }
                    if (string.IsNullOrEmpty(text2))
                    {
                        text2 = HtmlParseHelper.ParseDimensionValue(mediaItem.InnerItem["Height"], true);
                    }

                    xElement = this.CreateInlineElement(text, text2);
                    XElement content = this.CreateImageElement(htmlNode, mediaItem, parseContext, text, text2);
                    xElement.Add(content);
                }
            }
            if (xElement != null)
            {
                resultElement.Add(xElement);
            }
        }
        protected virtual XElement CreateInlineElement(string width, string height)
        {
            double dblMaxwidth = 117.474999999999980;
            double dblMaxheight = 185.5;

            double dblHeight = System.Convert.ToDouble(height);
            double dblWidth = System.Convert.ToDouble(width);

            if (dblWidth > dblMaxwidth)
            {
                width = dblMaxwidth.ToString();
                dblHeight = (dblMaxwidth / dblWidth) * dblHeight;
                height = dblHeight.ToString();
            }

            if (dblHeight > dblMaxheight)
            {
                height = dblMaxheight.ToString();
                dblWidth = (dblMaxheight / dblHeight) * dblWidth;
                width = dblWidth.ToString();
            }

            XElement xElement = new XElement("Inline");
            xElement.SetAttributeValue("Type", "Graphic");
            xElement.SetAttributeValue("Height", height);
            xElement.SetAttributeValue("Width", width);
            xElement.SetAttributeValue("Position", "AboveLine");
            xElement.SetAttributeValue("Alignment", "Left");
            xElement.SetAttributeValue("Scaling", "Fit Content to Frame");
            //xElement.SetAttributeValue("FitFrame", "True");
            return xElement;
        }
        protected virtual XElement CreateImageElement(HtmlNode htmlNode, Item mediaContentItem, ParseContext parseContext, string width, string height)
        {
            Item item = parseContext.Database.GetItem(WebConfigHandler.PrintStudioEngineSettings.EngineTemplates + "P_Image");
            Item standardValues = item.Template.StandardValues;
            XElement xElement = RenderItemHelper.CreateXElement("Image", standardValues, parseContext.PrintOptions.IsClient, null);
            ParseDefinition parseDefinition = parseContext.ParseDefinitions[htmlNode];
            if (parseDefinition != null)
            {
                this.SetAttributes(xElement, htmlNode, parseDefinition);
            }
            xElement.SetAttributeValue("Height", height);
            xElement.SetAttributeValue("Width", width);
            xElement.SetAttributeValue("SitecoreMediaID", mediaContentItem.ID.Guid.ToString());
            string text = ImageRendering.CreateImageOnServer(parseContext.PrintOptions, mediaContentItem);
            text = parseContext.PrintOptions.FormatResourceLink(text);
            xElement.SetAttributeValue("LowResSrc", text);
            xElement.SetAttributeValue("HighResSrc", text);
            return xElement;
        }
        [System.Obsolete]
        protected MediaItem GetMediaItem(string sourceAttributeValue)
        {
            string mediaPath = this.GetMediaPath(sourceAttributeValue);
            MediaItem result = null;
            if (!string.IsNullOrEmpty(mediaPath))
            {
                result = (ID.IsID(mediaPath) ? Context.Database.GetItem(ID.Parse(mediaPath)) : Context.Database.GetItem(mediaPath));
            }
            return result;
        }
        private string GetMediaPath(string localPath)
        {
            int num = -1;
            string text = string.Empty;
            foreach (string current in MediaManager.Provider.Config.MediaPrefixes)
            {
                num = localPath.IndexOf(current, System.StringComparison.InvariantCultureIgnoreCase);
                if (num >= 0)
                {
                    text = current;
                    break;
                }
            }
            if (num < 0)
            {
                return string.Empty;
            }
            if (string.Compare(localPath, num, text, 0, text.Length, true, System.Globalization.CultureInfo.InvariantCulture) != 0)
            {
                return string.Empty;
            }
            string text2 = StringUtil.Divide(StringUtil.Mid(localPath, num + text.Length), '.', true)[0];
            if (text2.EndsWith("/"))
            {
                return string.Empty;
            }
            if (ShortID.IsShortID(text2))
            {
                return ShortID.Decode(text2);
            }
            char[] trimChars = new char[]
			{
				'/'
			};
            return "/sitecore/media library/" + text2.TrimStart(trimChars);
        }    
    }
}