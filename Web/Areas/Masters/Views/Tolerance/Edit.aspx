<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Tolerance>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Edit Tolerance
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Tolerance Setup
    </h1><h2>
        Edit Tolerance</h2>
    <% using (Html.BeginForm("Edit", "Tolerance", FormMethod.Post, new { @id = "ToleranceMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Billing Category:</label>
            <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId) %>
            <%: Html.ValidationMessageFor(model => model.BillingCategoryId) %>
        </div>
        <div class="editor-label">
            <label>
                   <span class="required">* </span>Effective From Period:
           </label>
            <%: Html.TextBox(ControlIdConstants.EffectiveFromPeriod, Model.EffectiveFromPeriod.ToString("dd-MMM-yyyy"), new { @class = "upperCase" })%>
        </div>
        <div class="editor-label">
            <label>
                    <span class="required">* </span>Effective To Period:
            </label>
            <%: Html.TextBox(ControlIdConstants.EffectiveToPeriod, Model.EffectiveToPeriod.ToString("dd-MMM-yyyy"), new { @class = "upperCase" })%>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Clearing House:</label>
            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.ClearingHouse) %>
        </div>
         <div class="editor-label">
            <label>
                <span class="required">* </span>Rounding Tolerance:</label>
            <%: Html.TextBoxFor(model => model.RoundingTolerance, new { @class = "digit", @maxLength = 8 })%>
            <%: Html.ValidationMessageFor(model => model.RoundingTolerance) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Summation Tolerance:</label>
            <%: Html.TextBoxFor(model => model.SummationTolerance, new { @class = "digit", @maxLength = 8 })%>
            <%: Html.ValidationMessageFor(model => model.SummationTolerance) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Type:</label>
            <%: Html.TextBoxFor(model => model.Type, new { @class = "alphabet upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.Type) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Tolerance") %>'" />
        </div>
    </fieldset>
    <% } %>
   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/ToleranceValidate.js")%>" type="text/javascript"></script>
<script type="text/javascript">
  $(document).ready(function () {
      $('#EffectiveFromPeriod').watermark('PP-MMM-YYYY');
      $('#EffectiveToPeriod').watermark('PP-MMM-YYYY');
  });
</script>
</asp:Content>
