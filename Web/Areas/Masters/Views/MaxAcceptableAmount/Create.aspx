<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MaxAcceptableAmount>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Add Maximum Value Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Maximum Value Setup
    </h1><h2>
        Add Maximum Value</h2>
    <% using (Html.BeginForm("Create", "MaxAcceptableAmount", FormMethod.Post, new { @id = "MaxAcceptableAmountMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
    <div class="editText">
            <label>
                <span class="required">* </span>Effective From period:</label>
            <%: Html.TextBoxFor(model => model.EffectiveFromPeriod, new { @class = "upperCase", @id = "EffectiveFromPeriod" })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveFromPeriod)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Effective To period:</label>
            <%: Html.TextBoxFor(model => model.EffectiveToPeriod, new { @class = "upperCase", @maxLength = 11 })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveToPeriod)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Transaction Type:</label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId) %>
            <%: Html.ValidationMessageFor(model => model.TransactionTypeId) %>
        </div>
        
        <div class="editText">
            <label>
                <span class="required">* </span>Amount:</label>
            <%: Html.TextBoxFor(model => model.Amount, new { @class = "amt_10_3 amount", @maxLength = 11 })%>
            <%: Html.ValidationMessageFor(model => model.Amount) %>
        </div>
        
        <div class="editText">
            <label>
                <span class="required">* </span>Clearing House:</label>
            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.ClearingHouse) %>
        </div>
        <div class="editText">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer editText">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","MaxAcceptableAmount") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Masters/MaxAcceptableAmountValidate.js")%>" ></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/MinAcceptableAmount.js")%>"></script>
       <script type="text/javascript">
           $(document).ready(function () {
             BindEventOnCreateMinAccepatableAmount();
             $('#EffectiveFromPeriod').watermark('PP-MMM-YYYY');
             $('#EffectiveToPeriod').watermark('PP-MMM-YYYY');
           });
    </script>
</asp:Content>
