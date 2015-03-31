using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.PrintStudio.PublishingEngine.Rendering;
using Sitecore.PrintStudio.PublishingEngine.Helpers;
using Sitecore.Data.Items;

namespace CustomAPS.Renderers
{
    public class LookupRepeater : InDesignItemRendererBase
    {
        /// <summary>
        /// Gets or sets the data sources.
        /// </summary>
        /// <value>
        /// The data sources.
        /// </value>
        public string DataSources { get; set; }

        /// <summary>
        /// Gets or sets the repeat count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public string Count { get; set; }

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
        /// <param name="printContext">The print context.</param>
        protected override void BeginRender(Sitecore.PrintStudio.PublishingEngine.PrintContext printContext)
        {
            if (!string.IsNullOrEmpty(this.RenderingItem["Item Field"]))
            {
               
                var dataItem = this.GetDataItem(printContext);
                var data = dataItem[this.RenderingItem["Item Field"]];

                if (!string.IsNullOrEmpty(this.RenderingItem["Item Selector"]))
                {
                    var xpath = this.RenderingItem["Item Selector"];
                    if (!string.IsNullOrEmpty(xpath))
                    {
                        var items = dataItem.Axes.SelectItems(xpath);
                        if (items != null)
                        {
                            //this.DataSources = string.Join("|", items.Select(t => t.ID.ToString()).ToArray());
                            foreach (Item lookup in items)
                            {
                                this.DataSources = lookup[this.RenderingItem["Item Field"]];
                            }
                        }
                    }
                }

                Logger.Info("found DataSources: " + this.DataSources);
                Logger.Info("found DataSource: " + this.DataSource);

            }
        }

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

            if (!string.IsNullOrEmpty(this.DataSources))
            {
                foreach (var dataSource in this.DataSources.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    this.DataSource = dataSource;
                    if (!string.IsNullOrEmpty(this.ChildDataKeyName))
                    {
                        printContext.Settings.Parameters[this.ChildDataKeyName] = dataSource;
                    }

                    RenderChildren(printContext, output);
                }

                return;
            }

            this.RenderChildren(printContext, output);
        }
    }
}
