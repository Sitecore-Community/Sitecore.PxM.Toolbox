namespace LaunchSitecore.pxm.workflow
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Rules;
    using Sitecore.Rules.Actions;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/SetWorkflowVariable" when creating SetWorkflowVariable class. Fix Title field.

    public class SetWorkflowVariable<T> : RuleAction<T> where T : RuleContext
    {
        public virtual string ParameterName
        {
            get;
            set;
        }
        public override void Apply([NotNull] T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Item item = ruleContext.Item;

            if (!string.IsNullOrEmpty(this.ParameterName))
            {
                if (!ruleContext.Parameters.ContainsKey(this.ParameterName))
                {
                    ruleContext.Parameters.Add(this.ParameterName, item.ID.ToString());
                    return;
                }
                ruleContext.Parameters[this.ParameterName] = item.ID.ToString();
            }  
        }
    }
}