using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch.Utilities;
using Sitecore.PrintStudio.PublishingEngine.Rendering;
using Sitecore.ContentSearch;
using Sitecore.Buckets.Util;
using Sitecore.PrintStudio.PublishingEngine.Helpers;
using Sitecore.Data.Items;

namespace CustomAPS.Renderers
{
    public class SearchXMLRepeat : InDesignItemRendererBase
    {
        /// <summary>
        /// Gets or sets the name of the child data key.
        /// </summary>
        /// <value>
        /// The name of the child data key.
        /// </value>
        public string ChildDataKeyName { get; set; }

        /// <summary>
        /// Preliminary render action invoked before RenderContent <see cref="RenderContent"/>.
        /// </summary>
        /// <summary>
        /// Renders the content.
        /// </summary>
        /// <param name="printContext">The print context.</param>
        /// <param name="output">The output.</param>
        protected override void RenderContent(Sitecore.PrintStudio.PublishingEngine.PrintContext printContext, System.Xml.Linq.XElement output)
        {
            if (!string.IsNullOrEmpty(this.ChildDataKeyName))
            {
                printContext.Settings.Parameters[this.ChildDataKeyName] = this.DataSource;
            }

            if (!string.IsNullOrEmpty(this.RenderingItem["Search Query"]))
            {
                string searches = this.RenderingItem["Search Query"];
                using (var context = ContentSearchManager.GetIndex("sitecore_master_index").CreateSearchContext())
                {

                    var items = LinqHelper.CreateQuery(context, SearchStringModel.ParseDatasourceString(searches))
                                                                              .Select(toItem => toItem.GetItem());

                    //This gives us our IQueryable

                    foreach (var item in items)
                    {
                        this.DataSource = item.ID.ToString();
                        if (!string.IsNullOrEmpty(this.ChildDataKeyName))
                        {
                            printContext.Settings.Parameters[this.ChildDataKeyName] = item.ID.ToString();
                        }
                        Logger.LogMessage("Found item: " + item.Name);
                        RenderChildren(printContext, output);
                    }
                }
            }

        }
    }
}
