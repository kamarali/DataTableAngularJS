<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.ChargeCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Charge Code Type Requirement SetUp
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Edit Charge Code Type Requirement</h2>
  <% using (Html.BeginForm("Edit", "ChargeCodeTypeRequirementSetUp", FormMethod.Post, new { @id = "ChargeCodeTypeReqMaster" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <%: Html.ValidationSummary(true) %>
  <fieldset class="solidBox dataEntry">
    <div class="editor-label">
      <label>
        <span class="required">* </span>Charge Category:
      </label>
      <%: Html.ChargeCategoryTextBox("chargeCategoryId", Model.ChargeCategoryId, new { @readOnly = true})%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Charge Code:
      </label>
      <%: Html.TextBoxFor(model => model.Name, new { @readOnly = true })%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Charge Code Type Requirement:</label>
      <%: Html.ChargeCodeTypeReqDropdownList(model => model.IsChargeCodeTypeRequired) %>
    </div>
    <div class="editor-label">
      <label>
        Active:</label>
      <%: Html.CheckBoxFor(model => model.IsActiveChargeCodeType)%>
    </div>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton" />
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ChargeCodeTypeRequirementSetUp") %>'" />
    </div>
  </fieldset>
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/ChargeCodeTypeReqSetUp.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
     $(document).ready(function () {
      InitialiseChargeCodeTypeReq('<%: Url.Action("GetChargeCodeList", "ChargeCodeTypeRequirementSetUp", new { area = "Masters"})%>');
     });
  </script>
</asp:Content>
