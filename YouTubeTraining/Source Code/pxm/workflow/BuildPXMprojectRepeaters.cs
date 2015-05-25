namespace LaunchSitecore.pxm.workflow
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Rules;
    using Sitecore.Rules.Actions;
    using System.Linq;
    using System.Text;
    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/BuildPXMprojectRepeaters" when creating BuildPXMprojectRepeaters class. Fix Title field.

    public class BuildPXMprojectRepeaters<T> : RuleAction<T> where T : RuleContext
    {
        public static readonly string ItemReferenceFieldName = "Item Reference";
        public static readonly string DataKeyFieldName = "Data Key";

        public ID pxmdocid
        {
            get;
            set;
        }
        public override void Apply([NotNull] T ruleContext)
        {
            // Execute action
            // Execute action
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Item pxmproject = ruleContext.Item;

            Database contextDb = Sitecore.Context.ContentDatabase;

            //string[] parameters = ruleContext.Parameters;

            Item pxmProject = ruleContext.Item;

            Assert.IsNotNull(pxmdocid, "Workflow Parameters field needs to have a Print Studio Project ID");

            Item APSDocumentTemplateItem = contextDb.GetItem(pxmdocid);

            Item pxmProjectDocsFolder = pxmProject.Axes.GetDescendant("Books/P_Book/Documents");

            Item pxmProjectDoc = APSDocumentTemplateItem.CopyTo(pxmProjectDocsFolder, "Document");

            int buildLevel = 1;

            FindRepeaterAndBuildChildItems(pxmProjectDoc, buildLevel);
        }
        private static void FindRepeaterAndBuildChildItems(Item currentItem, int buildLevel)
        {
            Item repeater = currentItem.Axes.SelectSingleItem(".//*[@@templatekey = 'xml repeat' and contains(@parameters,'BuildLevel=" + buildLevel.ToString() + "')]");

            // Get a list of the original children of the repeater

            Item[] repeaterChildren = repeater.Children.ToArray();

            string repeaterContentReference = repeater["Item Reference"];
            string repeaterItemSelector = repeater["Item Selector"];
            string repeaterChildDataKeyName = System.Web.HttpUtility.ParseQueryString(repeater["Parameters"]).Get("ChildDataKeyName");

            Item repeaterContentReferenceItem = Sitecore.Context.ContentDatabase.GetItem(repeaterContentReference);

            Item[] repeaterContentReferenceItemChildren = repeaterContentReferenceItem.Children.ToArray();

            int sortOrder = 1000;

            // Iterate through all the content references found by the repeater
            foreach (Item reference in repeaterContentReferenceItemChildren)
            {
                // Make a copy of the repeater children
                foreach (Item page in repeaterChildren)
                {
                    Item newPage = page.Duplicate();

                    newPage.Editing.BeginEdit();
                    newPage.Name = reference.Name + "-" + page.Name;
                    newPage.Appearance.Sortorder = sortOrder;
                    newPage.Editing.EndEdit();

                    sortOrder++;

                    if (newPage.TemplateName == "XML Repeat")
                    {
                        BuildChildItems(newPage, 2);
                    }

                    Item[] updateSnippets = newPage.Children.Where(c => !string.IsNullOrEmpty(c[DataKeyFieldName])).ToArray();

                    // Set the Item Reference on the snippet based on the Data Key
                    foreach (Item child in updateSnippets)
                    {
                        UpdateInitialItemReferenceField(child, reference, repeaterChildDataKeyName);
                    }
                }

            }

            // Delete the original children of the repeater (they have no item reference set)
            foreach (Item original in repeaterChildren)
            {
                original.Delete();
            }

            repeater.Editing.BeginEdit();
            repeater[DataKeyFieldName] = "";
            repeater[ItemReferenceFieldName] = "";
            repeater["Item Selector"] = "";
            repeater.Editing.EndEdit();
        }
        private static void BuildChildItems(Item repeater, int buildLevel)
        {
            Item[] repeaterChildren = repeater.Children.ToArray();

            string repeaterContentReference = repeater["Item Reference"];
            string repeaterItemSelector = repeater["Item Selector"];
            string repeaterChildDataKeyName = System.Web.HttpUtility.ParseQueryString(repeater["Parameters"]).Get("ChildDataKeyName");

            Item repeaterContentReferenceItem = Sitecore.Context.ContentDatabase.GetItem(repeaterContentReference);

            Item[] repeaterContentReferenceItemChildren = repeaterContentReferenceItem.Children.ToArray();

            int sortOrder = 1000;

            foreach (Item reference in repeaterContentReferenceItemChildren)
            {
                // Make a copy of the repeater children
                foreach (Item page in repeaterChildren)
                {
                    Item newPage = page.Duplicate();

                    newPage.Editing.BeginEdit();
                    newPage.Name = reference.Name + "-" + page.Name;
                    newPage.Appearance.Sortorder = sortOrder;
                    newPage.Editing.EndEdit();

                    sortOrder++;

                    Item[] updateSnippets = newPage.Children.Where(c => !string.IsNullOrEmpty(c[DataKeyFieldName])).ToArray();

                    // Set the Item Reference on the snippet based on the Data Key
                    foreach (Item child in updateSnippets)
                    {
                        UpdateInitialItemReferenceField(child, reference, repeaterChildDataKeyName);
                    }
                }

            }

            // Delete the original children of the repeater (they have no item reference set)
            foreach (Item original in repeaterChildren)
            {
                original.Delete();
            }

            repeater.Editing.BeginEdit();
            repeater[DataKeyFieldName] = "";
            repeater[ItemReferenceFieldName] = "";
            repeater.Editing.EndEdit();

        }
        private static void UpdateInitialItemReferenceField(Item updateSnippet, Item newReferenceItem, string datakey)
        {
            if (updateSnippet[DataKeyFieldName] == datakey)
            {
                updateSnippet.Editing.BeginEdit();
                updateSnippet[ItemReferenceFieldName] = newReferenceItem.ID.ToString();
                updateSnippet[DataKeyFieldName] = "";
                updateSnippet.Editing.EndEdit();
            }
            UpdateChildrenItemReferenceField(updateSnippet, newReferenceItem, datakey);
        }
        private static void UpdateChildrenItemReferenceField(Item updateSnippet, Item newReferenceItem, string datakey)
        {
            updateSnippet.Children.ToList().ForEach(
                c => UpdateItemReferenceField(c, newReferenceItem, datakey));
        }
        private static void UpdateItemReferenceField(Item updateSnippet, Item newReferenceItem, string datakey)
        {
            updateSnippet.Editing.BeginEdit();
            updateSnippet[ItemReferenceFieldName] = newReferenceItem.ID.ToString();
            updateSnippet.Editing.EndEdit();

            UpdateChildrenItemReferenceField(updateSnippet, newReferenceItem, datakey);
        }
    }
}