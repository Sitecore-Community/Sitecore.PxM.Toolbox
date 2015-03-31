// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TocIndex.cs" company="Sitecore Corporation">
//   Copyright (C) 2012 by Sitecore Corporation
// </copyright>
// <summary>
//   Defines the toc index class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomAPS.Renderers
{
  using System.Collections.Generic;
  using Sitecore.PrintStudio.JetstreamDemo.Rendering.Common;
  using Sitecore.PrintStudio.PublishingEngine;
  using CustomAPS.Renderers.Common;
  using System.Linq;
    using Sitecore.Data.Items;

  /// <summary>
  /// Defines the toc index class.
  /// </summary>
  public class TocIndex : XmlContentRenderer
  {
    /// <summary>
    /// Gets or sets the name of the index.
    /// </summary>
    /// <value>
    /// The name of the index.
    /// </value>
    public string IndexValue { get; set; }

    /// <summary>
    /// Gets or sets the replace var.
    /// </summary>
    /// <value>
    /// The replace var.
    /// </value>
    public string ReplaceVar { get; set; }

    /// <summary>
    /// Parses the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <returns>
    /// The content.
    /// </returns>
    ///
    public string IndexName { get; set; }

    /// <summary>
    /// Parses the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <returns>
    /// The content.
    /// </returns>
    /// 
    public string ReplaceNameField { get; set; }

    /// <summary>
    /// Parses the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <returns>
    /// The content.
    /// </returns>
    /// 
    protected override string ParseContent(PrintContext printContext)
    {
      string content = base.ParseContent(printContext);

      IDictionary<string, string> variables = new Dictionary<string, string>();

      int tocIndex = 2;
      if (printContext.Settings.Parameters.ContainsKey(this.IndexValue))
      {
        tocIndex = (int)printContext.Settings.Parameters[this.IndexValue];
      }

      Item dataItem = this.GetDataItem(printContext);
      string fieldContents = dataItem[ReplaceNameField];

      variables.Add(this.ReplaceVar, tocIndex.ToString());

      SetVariableName(ref content, IndexName, fieldContents);

      ReplaceVariables(ref content, variables);

      printContext.Settings.Parameters[this.IndexValue] = tocIndex + 1;

      return content;
    }
    internal static void ReplaceVariables(ref string input, IDictionary<string, string> variables)
    {
        input = variables.Aggregate(input, (current, variable) => current.Replace(variable.Key, variable.Value));
    }
    internal static void SetVariableName(ref string input, string IndexName, string fieldContents)
    {
        input = input.Replace(IndexName, fieldContents);
    }
  }
}
