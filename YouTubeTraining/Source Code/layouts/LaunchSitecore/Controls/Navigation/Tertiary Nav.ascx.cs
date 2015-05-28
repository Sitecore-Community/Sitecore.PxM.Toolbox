using System;
using LaunchSitecore.Configuration.SiteUI.Base;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Web.UI.WebControls;
using Sitecore.Data.Managers;
using System.Linq;
using LaunchSitecore.Configuration;
using Sitecore.PrintStudio.PublishingEngine;
using Sitecore.Data;
using Sitecore.Configuration;
using System.IO;

namespace LaunchSitecore.layouts.LaunchSitecore.Controls.Navigation
{
  public partial class Tertiary_Nav : SitecoreUserControlBase
  {    
    bool multipleLanguagesHasBeenProcessed = false;

    private string projectPath = "/sitecore/Print Studio/Print Studio Projects/Launch Sitecore Articles on Demand";

    private void Page_PreRender(object sender, EventArgs e)
    {
      Login.Text = GetDictionaryText("Login");
      Register.Text = GetDictionaryText("Register");
      MyFavorites.Text = GetDictionaryText("My Favorites");
      btnLogout.Text = GetDictionaryText("Logout");
      btnPrint.Text = "Generate PDF of Favourites";

      if (Sitecore.Context.IsLoggedIn && Sitecore.Context.Domain.Name.ToLower() == "extranet")
      {
        SetViewableLinks(true);
        LoadFavorites();
      }
      else
      {
        SetViewableLinks(false);
      }

      LoadSitesList();
      LoadLanguagesList();

      // microsites are small so we are limiting the top area.
      if (SiteConfiguration.IsMicrosite())
      {
        loginli.Visible = false;
        registerli.Visible = false;
        logoutli.Visible = false;
        favoritesli.Visible = false;
      }
    }

    private void SetViewableLinks(bool IsLoggedIn)
    {
      loginli.Visible = !IsLoggedIn;
      registerli.Visible = !IsLoggedIn;
      favoritesli.Visible = false;
      logoutli.Visible = IsLoggedIn;
    }

    private void LoadFavorites()
    {
      List<Item> items = new List<Item>();
      Sitecore.Security.Accounts.User user = Sitecore.Context.User;
      Sitecore.Security.UserProfile profile = user.Profile;
      string ItemIds = profile.GetCustomProperty("Favorites");

      foreach (string itemId in ItemIds.Split('|'))
      {
        Item item = Sitecore.Context.Database.GetItem(itemId);
        if (item != null)
          items.Add(item);
      }

      if (items.Count > 0)
      {
        favoritesli.Visible = true;
        rptFavorites.DataSource = items;
        rptFavorites.DataBind();
      }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
      Sitecore.Security.Authentication.AuthenticationManager.Logout();
      Sitecore.Web.WebUtil.Redirect("/");

      // By default in Launch Sitecore we return the user the home page on logout.  It is important to understand how the Session Provider works in 7.5+ though.
      // The Sitecore session provider pushes the session data to the xDB on session end not logout.  
      // If you want to force this is development environemnts, you can call Session.Abandon() instead of the redirect.
    }

    private void LoadSitesList()
    {
      Item contentNode = SiteConfiguration.GetHomeItem().Parent;
      List<Item> sites = new List<Item>();
      List<Item> externalsites = new List<Item>();

      foreach (Item site in contentNode.Children.ToArray().Where(item => SiteConfiguration.DoesItemExistInCurrentLanguage(item)))
      {
        if (site["Show in Sites Menu"] == "1") { sites.Add(site); }
      }

      if (SiteConfiguration.GetExternalSitesItem() != null)
      {
        foreach (Item externalsite in SiteConfiguration.GetExternalSitesItem().Children)
        {
          if (SiteConfiguration.DoesItemExistInCurrentLanguage(externalsite)) externalsites.Add(externalsite);
        }
      }

      if (sites.Count + externalsites.Count > 1) // Don't show the drop down unless there are multiple sites
      {
        SitesLink.Text = GetDictionaryText("Sites");        
        if (sites.Count > 0)
        {
          rptList.DataSource = sites;
          rptList.DataBind();
        }

        if (sites.Count > 0 && externalsites.Count > 0) { divider.Visible = true; }

        if (externalsites.Count > 0)
        {
          rptExternal.DataSource = externalsites;
          rptExternal.DataBind();
        }
      }
      else
      {
        SitesLink.Visible = false;
      }
    }

    private void LoadLanguagesList()
    {
      List<Item> languages = Configuration.SiteUI.Translation.SiteLanguage.GetAdditionalLanguages(Sitecore.Context.Language);
      if (languages.Count == 0)
      {
        LiLanguages.Visible = false;
      }
      else
      {
        LanguageLink.Text = String.Format("<img src=\"{0}\" alt\"{1}\" /> {2}", Sitecore.Resources.Images.GetThemedImageSource(Sitecore.Context.Language.GetIcon(Sitecore.Context.Database)), Sitecore.Context.Language.Name, Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName);
        rptLanguages.DataSource = languages;
        rptLanguages.DataBind();
      }
    }

    protected void rptFavorites_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        Item node = (Item)e.Item.DataItem;
        HyperLink LinkTo = (HyperLink)e.Item.FindControl("LinkTo");

        if (LinkTo != null)
        {
          LinkTo.NavigateUrl = LinkManager.GetItemUrl(node);
          LinkTo.Text = node["Menu Title"];
        }
      }
    }

    protected void rptList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        Item node = (Item)e.Item.DataItem;
        HyperLink LinkTo = (HyperLink)e.Item.FindControl("LinkTo");

        if (LinkTo != null)
        {
          LinkTo.NavigateUrl = LinkManager.GetItemUrl(node);
          LinkTo.Text = node["Site Name"];
        }
      }
    }

    protected void rptExternal_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        Item node = (Item)e.Item.DataItem;
        Link LinkTo = (Link)e.Item.FindControl("LinkTo");

        if (LinkTo != null)
        {
          LinkTo.Item = node;          
        }
      }
    }

    protected void rptLanguages_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        Item node = (Item)e.Item.DataItem;

        HyperLink LangLink = (HyperLink)e.Item.FindControl("LangLink");
        System.Web.UI.WebControls.Image LangImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("LangImage");
        Literal LangIso = (Literal)e.Item.FindControl("LangIso");

        //check if context item has a language version
        Item contextItem = Sitecore.Context.Item;
        Item itemInLanguage = Sitecore.Context.Database.GetItem(contextItem.ID, Sitecore.Globalization.Language.Parse(node.Name));

        if (node != null && SiteConfiguration.DoesItemExistInCurrentLanguage(itemInLanguage) && LangLink != null && LangImage != null && LangIso != null)
        {
          string icon = node.Appearance.Icon;
          icon = Sitecore.Resources.Images.GetThemedImageSource(icon);
          LangImage.ImageUrl = icon;
          LangIso.Text = node["iso"].ToLower();
          LangLink.NavigateUrl = Configuration.SiteUI.Translation.SiteLanguage.GetLanguageUrl(node);

          if (!multipleLanguagesHasBeenProcessed)
          {
            LiLanguages.Attributes.Add("class", "dropdown");
            LanguageLink.Text += " <b class=\"caret\"></b>";
            multipleLanguagesHasBeenProcessed = true;
          }          
        }
      }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
      Item projectItem = Sitecore.Context.Database.GetItem(this.projectPath);

      Sitecore.Security.Accounts.User user = Sitecore.Context.User;
      Sitecore.Security.UserProfile profile = user.Profile;

      if (projectItem != null)
      {
          string fileName = string.Format("{0}_{1}", projectItem.Name, DateTime.Now.Ticks.ToString());

          PrintOptions printOptions = new PrintOptions
          {
              PrintExportType = PrintExportType.Pdf,
              ResultFileName = fileName,
              UseHighRes = false
          };

          Database masterBase = Factory.GetDatabase("master");

          printOptions.Parameters.Add("articles", profile.GetCustomProperty("Favorites"));

          PrintManager printManager = new PrintManager(masterBase, Sitecore.Context.Language);
          string result = printManager.Print(projectItem.ID.ToString(), printOptions);

          if (!string.IsNullOrEmpty(result) && File.Exists(result))
          {
              var file = new FileInfo(result);
              Response.ContentType = "application/pdf";
              Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}", file.Name));
              Response.AppendHeader("Content-Length", file.Length.ToString());
              Response.TransmitFile(file.FullName);
              Response.Flush();
              Response.End();
          }
      }
    }
  }
}