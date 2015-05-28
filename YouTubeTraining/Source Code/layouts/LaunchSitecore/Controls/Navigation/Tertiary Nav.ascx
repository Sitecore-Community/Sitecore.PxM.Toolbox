<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tertiary Nav.ascx.cs" Inherits="LaunchSitecore.layouts.LaunchSitecore.Controls.Navigation.Tertiary_Nav" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<ul class="nav nav-pills pull-right anav">
 <li id="loginli" runat="server"><asp:HyperLink NavigateUrl="/login" ID="Login" runat="server" /></li>
 <li id="registerli" runat="server"><asp:HyperLink NavigateUrl="/register" ID="Register" runat="server" /></li>
 <li id="logoutli" runat="server"><asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click" /></li>

 <li class="dropdown" id="favoritesli" runat="server">
  <a class="dropdown-toggle" data-toggle="dropdown" href="#">
   <asp:Literal ID="MyFavorites" runat="server" /><b class="caret"></b></a>
  <ul class="dropdown-menu pull-right">
   <asp:Repeater ID="rptFavorites" runat="server" OnItemDataBound="rptFavorites_ItemDataBound">
    <ItemTemplate><li><asp:HyperLink ID="LinkTo" runat="server" /></li></ItemTemplate>
   </asp:Repeater>
      <li class="divider" id="divider2"></li>
   <li><asp:LinkButton ID="btnPrint" runat="server" OnClick="btnPrint_Click" /></li>
  </ul>
 </li>

 <li class="dropdown">
  <a class="dropdown-toggle" data-toggle="dropdown" href="#">
   <asp:Literal ID="SitesLink" runat="server" /><b class="caret"></b></a>
  <ul class="dropdown-menu pull-right">
   <asp:Repeater ID="rptList" runat="server" OnItemDataBound="rptList_ItemDataBound">
    <ItemTemplate><li><asp:HyperLink ID="LinkTo" runat="server" /></li></ItemTemplate>
   </asp:Repeater>
   <li class="divider" id="divider" runat="server" visible="false"></li>
   <asp:Repeater ID="rptExternal" runat="server" OnItemDataBound="rptExternal_ItemDataBound">
    <ItemTemplate><li><sc:Link ID="LinkTo" runat="server" Field="Site Link" /></li></ItemTemplate>
   </asp:Repeater>
  </ul>
 </li>

 <li id="LiLanguages" runat="server">
  <asp:HyperLink ID="LanguageLink" runat="server" CssClass="dropdown-toggle" data-toggle="dropdown" NavigateUrl="#" />
  <ul class="dropdown-menu pull-right" style="min-width: 80px;">
   <asp:Repeater ID="rptLanguages" runat="server" OnItemDataBound="rptLanguages_ItemDataBound">
    <ItemTemplate>
     <li>
      <asp:HyperLink ID="LangLink" runat="server">
       <asp:Image ID="LangImage" runat="server" />&nbsp;<asp:Literal ID="LangIso" runat="server" />
      </asp:HyperLink>
     </li>
    </ItemTemplate>
   </asp:Repeater>
  </ul>
 </li>
</ul>


