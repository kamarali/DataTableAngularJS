<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BvcAgreement>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: BVC Agreement Master
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        BVC Agreement Setup</h2>
    <% using (Html.BeginForm("Index", "OneWayBVCAgreement", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchBvcAgreement.ascx"); %>
    </div>
    <div class="buttonContainer">
        <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete))
          {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "OneWayBVCAgreement")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchBvcAgreementGrid.ascx", ViewData["BvcAgreementGrid"]); %>
</asp:Content>
