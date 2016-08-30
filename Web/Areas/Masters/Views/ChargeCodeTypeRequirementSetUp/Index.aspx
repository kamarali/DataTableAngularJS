<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.ChargeCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Charge Code Type Requirement SetUp
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Charge Code Type Requirement SetUp</h2>
  <% using (Html.BeginForm("Index", "ChargeCodeTypeRequirementSetUp", FormMethod.Post))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchChargeCodeTypeReqSetUp.ascx"); %>
  </div>
  <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete))
      {%>
    <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","ChargeCodeTypeRequirementSetUp") %>'" />
    <%
      }%>
    <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
  </div>
  <%} %>
  <h2>
    Search Results</h2>
  <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchChargeCodeTypeReqSetUpGrid.ascx", ViewData["ChargeCodeTypeReqSetupGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/ChargeCodeTypeReqSetUp.js")%>" type="text/javascript"> </script>
  <script type="text/javascript">
    $(document).ready(function () {
      InitialiseChargeCodeTypeReq('<%: Url.Action("GetChargeCodeList", "ChargeCodeTypeRequirementSetUp", new { area = "Masters"})%>');
    });
  </script>
</asp:Content>
