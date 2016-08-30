<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.ChargeCodeType>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Charge Code Type Name SetUp
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Charge Code Type Name SetUp</h2>
  <% using (Html.BeginForm("Index", "ChargeCodeTypeNameSetUp", FormMethod.Post))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchChargeCodeTypeNameSetUp.ascx"); %>
  </div>
  <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete))
      {%>
    <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","ChargeCodeTypeNameSetUp") %>'" />
    <%
}%>
    <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
  </div>
  <%} %>
  <h2>
    Search Results</h2>
  <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchChargeCodeTypeNameSetUpGrid.ascx", ViewData["ChargeCodeTypeNameSetupGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/ChargeCodeTypeNameSetUp.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      InitialiseChargeCodeTypeName('<%: Url.Action("GetChargeCodeList", "ChargeCodeTypeNameSetUp", new { area = "Masters"})%>', true);
    });
  </script>
</asp:Content>
