// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlParagraphStyleRenderer.cs" company="Sitecore Corporation">
//   Copyright (C) 2012 by Sitecore Corporation
// </copyright>
// <summary>
//   Renders paragraph style element markup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomAPS.Renderers.Common
{
  using System.Xml.Linq;
  using Sitecore.PrintStudio.PublishingEngine;
  using Sitecore.PrintStudio.PublishingEngine.Rendering;

  /// <summary>
  /// Renders paragraph style element markup.
  /// </summary>
  public class XmlParagraphStyleRenderer : XmlElementRenderer
  {
    /// <summary>
    /// Renders the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <param name="output">The output.</param>
    protected override void RenderContent(PrintContext printContext, XElement output)
    {
      // Create a temp node to serve as a container
      var temp = new XElement("temp");
      
      // Call the base to generate the element xml
      base.RenderContent(printContext, temp);

      // Select the generated paragraph style element
      var paragraph = temp.Element(this.Tag);        
      if (paragraph != null)
      {
        // Get the data item assigned to the snippet
        var dataItem = this.GetDataItem(printContext);
        if (dataItem != null && !string.IsNullOrEmpty(this.RenderingItem["Item Field"]))
        {
          // Fetch the value for the field point in the ParagraphStyle element and add it as a CDATA
          paragraph.AddFirst(new XCData(dataItem[this.RenderingItem["Item Field"]]));
        }

        output.Add(paragraph);
      }
    }
  }
}
