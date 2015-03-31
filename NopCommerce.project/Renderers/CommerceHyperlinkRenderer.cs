// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HyperlinkRenderer.cs" company="Sitecore Corporation">
//   Copyright (C) 2014 by Sitecore Corporation
// </copyright>
// <summary>
//   The hyperlink renderer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomAPS.Renderers
{
  using System;
  using System.Collections.Generic;
  using System.Xml.Linq;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.PrintStudio.PublishingEngine;
  using Sitecore.PrintStudio.PublishingEngine.Rendering;
    using System.Web;

  /// <summary>
  /// The hyperlink renderer.
  /// </summary>
  public class HyperlinkRenderer : XmlTextFrameRenderer
  {
    /// <summary>
    /// The data item.
    /// </summary>
    private Item dataItem;

    /// <summary>
    /// Gets or sets the link text.
    /// </summary>
    /// <value>
    /// The link text.
    /// </value>
    public string LinkText { get; set; }
    public string baseUrl { get; set; }
    public string path { get; set; }
    public string FieldName { get; set; }

    /// <summary>
    /// The begin render.
    /// </summary>
    /// <param name="printContext">
    /// The print context.
    /// </param>
    protected override void BeginRender(PrintContext printContext)
    {
      base.BeginRender(printContext);
      this.dataItem = this.GetDataItem(printContext);
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <param name="textFrameNode">The text frame node.</param>
    /// <returns>
    /// The content as collection of <see cref="XElement" />.
    /// </returns>
    protected override IEnumerable<XElement> GetContent(PrintContext printContext, XElement textFrameNode)
    {
      try
      {
        var paragraphStyleAttribute = textFrameNode.Attribute("ParagraphStyle");
        var paragraphStyle = paragraphStyleAttribute != null && !string.IsNullOrEmpty(paragraphStyleAttribute.Value) ? paragraphStyleAttribute.Value : "NormalParagraphStyle";
        
        var textBlock = new XElement("ParagraphStyle");
        textBlock.SetAttributeValue("Style", paragraphStyle);
        textBlock.Add(new XCData(System.Web.HttpUtility.UrlDecode(this.LinkText)));

        return new List<XElement>
        {
          textBlock
        };
      }
      catch (Exception exc)
      {
        Log.Error(exc.Message, this);
      }

      return null;
    }

    /// <summary>
    /// Sets the attributes.
    /// </summary>
    /// <param name="textFrameNode">The text frame node.</param>
    protected override void SetAttributes(XElement textFrameNode)
    {
      base.SetAttributes(textFrameNode);

      if (this.dataItem != null)
      {
        Field contentField = this.dataItem.Fields[this.ContentFieldName];
        if (contentField != null)
        {
          textFrameNode.SetAttributeValue("HLInfo", "URL");
          textFrameNode.SetAttributeValue("HLink", "True");
          textFrameNode.SetAttributeValue("HLLinkTo", GenerateQrCodeUrl(this.baseUrl, System.Web.HttpUtility.UrlDecode(this.path)));
        }
      }
    }
    private string GenerateQrCodeUrl(string domain, string path)
    {
        Item contentItem = this.dataItem;
        string ExternalID = contentItem[this.FieldName];
        string url;
        url = String.Format("http://{0}{1}{2}?sc_itemid={3}", domain, path, ExternalID, contentItem.ID.ToString());

        return url;
    }
  }
}