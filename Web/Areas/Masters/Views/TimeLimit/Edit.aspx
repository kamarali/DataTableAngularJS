<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.TimeLimit>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Master Maintenance :: General :: Edit Time Limit Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Time Limit Setup</h1>
  <h2>
    Edit Time Limit</h2>
  <% using (Html.BeginForm("Edit", "TimeLimit", FormMethod.Post, new { @id = "TimeLimitMaster" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <%: Html.ValidationSummary(true) %>
  <fieldset class="solidBox dataEntry">    
    <div class="editor-label">
      <label>
        <span class="required">* </span>Time Limit:
      </label>
      <%: Html.TextBoxFor(model => model.Limit, new { @class = "integer", @maxLength = 3 })%>
      <%: Html.ValidationMessageFor(model => model.Limit) %>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Effective From Period:
      </label>
      <%: Html.TextBox(ControlIdConstants.EffectiveFromPeriod, Model.EffectiveFromPeriod.ToString(FormatConstants.DateFormatFullYear), new { @class = "datePickerMaster" })%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Effective To Period:
      </label>
      <%: Html.TextBox(ControlIdConstants.EffectiveToPeriod, Model.EffectiveToPeriod.ToString(FormatConstants.DateFormatFullYear), new { @class = "datePickerMaster" })%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Settlement Method:
      </label>
      <%: Html.SettlementMethodDropdownListFor(model => model.SettlementMethodId, InvoiceType.CreditNote, Iata.IS.Web.Util.TransactionMode.Transactions)%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Transaction Type:
      </label>
      <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId)%>
      <%: Html.ValidationMessageFor(model => model.TransactionTypeId) %>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>Calculation Method:
      </label>
      <%: Html.TextBoxFor(model => model.CalculationMethod, new { @class = "alphaNumeric upperCase", @maxLength = 2 })%>
      <%: Html.ValidationMessageFor(model => model.CalculationMethod) %>
    </div>
    <div class="editor-label">
      <label>
        Active:
      </label>
      <%: Html.CheckBoxFor(model => model.IsActive) %>
      <%: Html.ValidationMessageFor(model => model.IsActive) %>
    </div>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton" />
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","TimeLimit") %>'" />
    </div>
  </fieldset>
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/TimeLimitValidate.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          _dateFormat = 'dd-M-yy';
      });
  </script>
</asp:Content>
