// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserMessageTextFrame.cs" company="Sitecore Corporation">
//   Copyright (C) 2012 by Sitecore Corporation
// </copyright>
// <summary>
//   Outputs user details and a personal message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LaunchSitecore.pxm.renderers
{
    using System;
    using System.Xml.Linq;
    using Sitecore;
    using Sitecore.Diagnostics;
    using Sitecore.PrintStudio.PublishingEngine;
    using Sitecore.PrintStudio.PublishingEngine.Rendering;

  /// <summary>
  /// Outputs user details and a personal message.
  /// </summary>
  public class UserMessageTextFrame : XmlTextFrameRenderer
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
        var tempElement = new XElement("temp");
        base.RenderContent(printContext, tempElement);

        var textFrame = tempElement.Element("TextFrame");
        if (textFrame != null)
        {

          var userFirstName = Context.User.Profile.FullName;
          if (string.IsNullOrEmpty(userFirstName))
          {
            userFirstName = "Example User";
          }

          // Set text variable to match user points value
          var textParagraph = textFrame.Element("ParagraphStyle");
          if (textParagraph != null)
          {
            var textValue = textParagraph.Value;
            textParagraph.ReplaceNodes(
              new XCData(textValue.Replace("$name", userFirstName).ToString())
              );
          }

          output.Add(textFrame);
        }
      }
      catch (Exception exc)
      {
        Log.Error("Rendering the user message text", exc, this);
      }
    }
  }
}