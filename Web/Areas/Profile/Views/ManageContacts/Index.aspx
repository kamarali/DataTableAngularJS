<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Iata.IS.Model.MemberProfile.ContactType>>" %>

<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Profile and User Management ::   Contacts Administration
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
   Manage Contact Type</h1>
    <% using (Html.BeginForm("Index", "ManageContacts", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Profile/Views/Shared/SearchContacts.ascx"); %>
    </div>
    <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","ManageContacts") %>'" />
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
     
    <%Html.RenderPartial("~/Areas/Profile/Views/Shared/SearchContactsGrid.ascx", ViewData["ContactTypeGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>