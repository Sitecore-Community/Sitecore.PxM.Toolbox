namespace LaunchSitecore.pxm.workflow
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Rules;
    using Sitecore.Rules.Actions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/BuildPXMproject" when creating BuildPXMproject class. Fix Title field.

    public class BuildPXMproject<T> : RuleAction<T> where T : RuleContext
    {
        public static readonly string ItemReferenceFieldName = "Item Reference";
        public static readonly string DataKeyFieldName = "Data Key";

        public ID ProjectId
        {
            get;
            set;
        } 
        public override void Apply([NotNull] T ruleContext)
        {
            // Execute action
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Item item = ruleContext.Item;

            Database contextDb = Sitecore.Context.ContentDatabase;

            //string[] parameters = ruleContext.Parameters;

            Item workFlowItem = ruleContext.Item;

            Assert.IsNotNull(ProjectId, "Workflow Parameters field needs to have a Print Studio Project ID");

            Item APSProjectTemplateItem = contextDb.GetItem(ProjectId);

            Item NewProjectID = APSProjectTemplateItem.Duplicate(workFlowItem.Name);

            NewProjectID.Editing.BeginEdit();
            NewProjectID.Name = workFlowItem.Name;
            NewProjectID.Appearance.DisplayName = workFlowItem.Name;
            NewProjectID.Editing.EndEdit();

            //Item[] pages = NewProjectID.Axes.GetDescendants().Where;

            Item[] updateSnippets = NewProjectID.Axes.GetDescendants().Where(
                c => !string.IsNullOrEmpty(c[ItemReferenceFieldName])).ToArray();

            foreach (KeyValuePair<string, object> datakey in ruleContext.Parameters)
            {
                foreach (Item child in updateSnippets)
                {
                    string datakeystring = datakey.Key;
                    Item datakeyvalue = contextDb.GetItem(datakey.Value.ToString());

                    UpdateInitialItemReferenceField(child, datakeyvalue, datakeystring);
                    //UpdateChildrenItemReferenceField(child, datakeyvalue, datakeystring);
                }
            }
        }
        private static void UpdateChildrenItemReferenceField(Item updateSnippet, Item newReferenceItem, string datakey)
        {
            updateSnippet.Children.ToList().ForEach(
                c => UpdateItemReferenceField(c, newReferenceItem, datakey));
        }

        private static void UpdateInitialItemReferenceField(Item updateSnippet, Item newReferenceItem, string datakey)
        {
            if (!string.IsNullOrEmpty(updateSnippet[ItemReferenceFieldName]) && updateSnippet[DataKeyFieldName] == datakey)
            {
                updateSnippet.Editing.BeginEdit();
                updateSnippet[ItemReferenceFieldName] = newReferenceItem.ID.ToString();
                updateSnippet.Editing.EndEdit();
            }
            UpdateChildrenItemReferenceField(updateSnippet, newReferenceItem, datakey);
        }
        private static void UpdateItemReferenceField(Item updateSnippet, Item newReferenceItem, string datakey)
        {
            if (!string.IsNullOrEmpty(updateSnippet[ItemReferenceFieldName]))
            {
                updateSnippet.Editing.BeginEdit();
                updateSnippet[ItemReferenceFieldName] = newReferenceItem.ID.ToString();
                updateSnippet.Editing.EndEdit();
            }
            UpdateChildrenItemReferenceField(updateSnippet, newReferenceItem, datakey);
        }
    }
}