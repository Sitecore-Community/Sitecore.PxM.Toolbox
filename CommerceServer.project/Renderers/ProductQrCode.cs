// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlightQrCode.cs" company="Sitecore Corporation">
//   Copyright (C) 2012 by Sitecore Corporation
// </copyright>
// <summary>
//   Defines the flight QR code class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomAPS.Renderers
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml.Linq;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Links;
  using Sitecore.PrintStudio.PublishingEngine;
  using Sitecore.PrintStudio.PublishingEngine.Rendering;
  using Sitecore.Web;

  /// <summary>
  /// Defines the product QR code class.
  /// </summary>
  public class ProductQrCode : InDesignItemRendererBase
  {
    /// <summary>
    /// the base url used to make the qr code link
    /// </summary>
    public string baseUrl { get; set; }

    /// <summary>
    /// Gets or sets the base URL item.
    /// </summary>
    /// <value>
    /// The base URL item.
    /// </value>
    public string path { get; set; }

    /// <summary>
    /// Gets or sets the cabin class.
    /// </summary>
    /// <value>
    /// The cabin class.
    /// </value>
    public string FieldName { get; set; }

    /// <summary>
    /// Begins the render.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <summary>
    /// Renders the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <param name="output">The output.</param>
    protected override void RenderContent(PrintContext printContext, XElement output)
    {
      XElement imageNode = RenderItemHelper.CreateXElement("Image", this.RenderingItem, printContext.Settings.IsClient, null);
      string imageFilePath = string.Empty;

      Item contentItem = this.GetDataItem(printContext);

      string ExternalID = contentItem[this.FieldName];
      if (contentItem != null && !string.IsNullOrEmpty(this.baseUrl))
      {
        try
        {
            string url = this.GenerateQrCodeUrl(this.baseUrl, this.path, ExternalID);
          imageFilePath = ImageRendering.CreateQrOnServer(printContext.Settings, this.RenderingItem, url);
          string qrcodeFilePath = this.GetQrImageFilePath(contentItem, printContext.Settings);

          File.Copy(imageFilePath, qrcodeFilePath, true);
          imageFilePath = qrcodeFilePath;
        }
        catch (Exception exc)
        {
          Log.Error(exc.Message, this);
          imageFilePath = string.Empty;
        }
      }

      imageFilePath = printContext.Settings.FormatResourceLink(imageFilePath);

      imageNode.SetAttributeValue("LowResSrc", imageFilePath);
      imageNode.SetAttributeValue("HighResSrc", imageFilePath);

      output.Add(imageNode);
      this.RenderChildren(printContext, imageNode);
    }

    /// <summary>
    /// Generates the qr code.
    /// </summary>
    /// <param name="contentItem">The content item.</param>
    /// <returns>
    /// The qr code.
    /// </returns>
    private string GenerateQrCodeUrl(string domain, string path, string ExternalID)
    {
        string url;
        url = String.Format("http://{0}{1}{2}", domain, path, ExternalID);

      return url;
    }

    /// <summary>
    /// Gets the qr image file path.
    /// </summary>
    /// <param name="contentItem">The content item.</param>
    /// <param name="printOptions">The print options.</param>
    /// <returns>
    /// The qr image file path.
    /// </returns>
    private string GetQrImageFilePath(Item contentItem, PrintOptions printOptions)
    {
      string fileName;

        fileName = contentItem["ExternalID"];

      return Path.Combine(printOptions.CacheFolder, fileName + ".png");
    }
  }
}
