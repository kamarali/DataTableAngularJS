<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.ChargeCodeType>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Charge Code Type Name SetUp
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Edit Charge Code Type Name</h2>
  <% using (Html.BeginForm("Edit", "ChargeCodeTypeNameSetUp", FormMethod.Post, new { @id = "ChargeCodeTypeNameMaster" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <fieldset class="solidBox dataEntry">
    <div class="editor-label">
      <label>
        <span class="required">* </span>Charge Category:
      </label>
      <%: Html.ChargeCategoryTextBox("chargeCategoryName", Model.ChargeCategoryId, new { @readOnly = true, style = "width:140px" })%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Charge Code:
      </label>
      <%: Html.ChargeCodeTextBox("chargeCodeName", Model.ChargeCodeId, new { @readOnly = true, style = "width:140px" })%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Charge Code Type Name:</label>
      <%: Html.TextBoxFor(model => model.Name, new { style = "width:140px" })%>
    </div>
    <div class="editor-label">
      <label>
        Active:</label>  
      <%: Html.CheckBoxFor(model => model.IsActive)%>
    </div> 
    <%: Html.HiddenFor(model => model.ChargeCodeId) %>
    <%: Html.HiddenFor(model => model.ChargeCategoryId) %>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton" />
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ChargeCodeTypeNameSetUp") %>'" />
    </div>
  </fieldset>
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/ChargeCodeTypeNameSetUp.js")%>" type="text/javascript"></script>
   <script type="text/javascript">
     $(document).ready(function () {
       InitialiseChargeCodeTypeName('<%: Url.Action("GetChargeCodeList", "ChargeCodeTypeNameSetUp", new { area = "Masters"})%>');
     });
  </script>
</asp:Content>
