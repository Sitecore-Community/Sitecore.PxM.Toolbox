// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationIndicatorShape.cs" company="Sitecore Corporation">
//   Copyright (C) 2012 by Sitecore Corporation
// </copyright>
// <summary>
//   Defines a custom rendering for path shape.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomAPS.Renderers
{
  using System;
  using System.Xml.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.PrintStudio.PublishingEngine;
  using Sitecore.PrintStudio.PublishingEngine.Rendering;
  using Sitecore.Data.Items;

  /// <summary>
  /// Defines a custom rendering for path shape.
  /// </summary>
  public class LocationIndicatorShape : XmlShapeRenderer
  {
    /// <summary>
    /// Renders the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <param name="output">The output.</param>
    protected override void RenderContent(PrintContext printContext, XElement output)
    {
      try
      {
        XElement temp = new XElement("temp");
        base.RenderContent(printContext, temp);

        // Find the indicator element
        var locationIndicator = temp.Element("Path");
        if (locationIndicator != null)
        {
          // Fetch the dynamic content item (City)
          Item contentItem = this.GetDataItem(printContext);

          /*string contentItemID = base.RenderingItem["Item Reference"];

          if (contentItem == null)
          {
              contentItem = printContext.Database.GetItem(contentItemID);
          }
            
          Log.Info("Content Item ID" + contentItemID, this);
           */
          Log.Info("Content Item" + contentItem.ID.ToString(), this);
          if (contentItem != null && !string.IsNullOrEmpty(contentItem["Coordinates"]))
          {
            string[] coordinates = contentItem["Coordinates"].Split('|');

            // Set the x and y coordinates
            if (locationIndicator.HasAttributes)
            {
              locationIndicator.SetAttributeValue("X", coordinates[0]);
              locationIndicator.SetAttributeValue("Y", coordinates[1]);

              output.Add(locationIndicator);
            }
          }
        }
      }
      catch (Exception exc)
      {
        Log.Error("Rendering the location indicator", exc, this);
      }
    }
  }
}
