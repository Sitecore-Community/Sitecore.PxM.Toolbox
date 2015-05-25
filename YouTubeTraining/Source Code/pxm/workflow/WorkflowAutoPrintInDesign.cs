namespace LaunchSitecore.pxm.workflow
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.PrintStudio.PublishingEngine;
    using Sitecore.PrintStudio.PublishingEngine.Helpers;
    using Sitecore.Rules;
    using Sitecore.Rules.Actions;
    using System;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/WorkflowAutoPrintInDesign" when creating WorkflowAutoPrintInDesign class. Fix Title field.

    public class WorkflowAutoPrintInDesign<T> : RuleAction<T> where T : RuleContext
    {
        public ID ProjectId
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public override void Apply([NotNull] T ruleContext)
        {
            // Execute action
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Item item = ruleContext.Item;

            Logger.LogMessage("Workflow print to indesign started:" + item.Name + "_" + item.Language + ".pdf");
            //Logger.LogMessage("Workflow parameters:" + ruleContext.Parameters.Keys.ToString());

            if (this.ProjectId != ID.Null)
            {
                try
                {
                    string @string = StringUtil.GetString(new string[]
					{
						this.FileName,
						this.ProjectId.ToString() + DateTime.Now.Ticks
					});
                    PrintOptions printOptions = new PrintOptions
                    {
                        PrintExportType = PrintExportType.Pdf,
                        ResultFileName = FileName + item.Name + "_" + item.Language + ".pdf",
                        UseHighRes = false,
                        Parameters = ruleContext.Parameters
                    };
                    PrintManager printManager = new PrintManager(item.Database, item.Language);
                    printManager.PrintAsync(this.ProjectId.ToString(), printOptions);
                }
                catch (Exception ex)
                {
                    Logger.LogMessage(ex.Message);
                }
            }
        }
    }
}