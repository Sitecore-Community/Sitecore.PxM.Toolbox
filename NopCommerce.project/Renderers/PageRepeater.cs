// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageRepeater.cs" company="Sitecore Corporation">
//   Copyright (C) 2014 by Sitecore Corporation
// </copyright>
// <summary>
//   Repeates a single page based on the datasource item collection count.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomAPS.Renderers
{
  using System.Linq;
  using System.Web;
  using System.Xml.Linq;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.SearchTypes;
  using Sitecore.ContentSearch.Utilities;
  using Sitecore.PrintStudio.PublishingEngine;
  using Sitecore.PrintStudio.PublishingEngine.Rendering;

  /// <summary>
  /// Repeates a single page based on the datasource item collection count.
  /// </summary>
  public class PageRepeater : XmlPageRenderer
  {
    /// <summary>
    /// Gets or sets the name of the child data key.
    /// </summary>
    /// <value>
    /// The name of the child data key.
    /// </value>
    public string ChildDataKeyName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PageRepeater" /> is repeatable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if repeatable; otherwise, <c>false</c>.
    /// </value>
    public bool Repeatable { get; set; }

    /// <summary>
    /// Gets or sets the data query.
    /// </summary>
    /// <value>
    /// The data query.
    /// </value>
    public string QueryFieldName { get; set; }

    /// <summary>
    /// Renders the content.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <param name="output">The output.</param>
    protected override void RenderContent(PrintContext printContext, XElement output)
    {
      if (this.RenderingItem["Skip"].Equals("1") || !this.Repeatable || SitecoreHelper.HasScriptReference(this.RenderingItem))
      {
        base.RenderContent(printContext, output);
        return;
      }

      if (!string.IsNullOrEmpty(this.QueryFieldName))
      {
        var pages = this.RenderingItem.Parent;
        var searchCriteria = pages.Axes.GetDescendant("Search Criteria");

        var searchQuery = searchCriteria[HttpUtility.UrlDecode(this.QueryFieldName)];
        if (!string.IsNullOrEmpty(searchQuery))
        {
          using (var context = ContentSearchManager.GetIndex("sitecore_master_index").CreateSearchContext())
          {
            var items = LinqHelper.CreateQuery<SitecoreUISearchResultItem>(context, SearchStringModel.ParseDatasourceString(searchQuery)).Select(toItem => toItem.GetItem());
            foreach (var item in items)
            {
              this.DataSource = item.ID.ToString();
              if (!string.IsNullOrEmpty(this.ChildDataKeyName))
              {
                printContext.Settings.Parameters[this.ChildDataKeyName] = item.ID.ToString();
              }

              this.RenderPage(printContext, output, true);
            }

            return;
          }
        }
      }

      this.RenderPage(printContext, output);
    }

    /// <summary>
    /// Renders the page.
    /// </summary>
    /// <param name="printContext">The print context.</param>
    /// <param name="output">The output.</param>
    /// <param name="skipNumber">if set to <c>true</c> [skip number].</param>
    /// <param name="emptyNumber">if set to <c>true</c> [empty number].</param>
    private void RenderPage(PrintContext printContext, XElement output, bool skipNumber = false, bool emptyNumber = false)
    {
      XElement pageContent = this.CreatePage(printContext, skipNumber, emptyNumber);
      printContext.PageCount++;

      this.RenderChildren(printContext, pageContent);
      output.Add(pageContent);
    }
  }
}
