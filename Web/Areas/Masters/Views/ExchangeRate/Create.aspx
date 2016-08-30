<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ExchangeRate>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Currency Related :: Add Exchange Rate Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Exchange Rate Setup</h1><h2>
        Add Exchange Rate</h2>
    <% using (Html.BeginForm("Create", "ExchangeRate", FormMethod.Post, new { @id = "ExchangeRateMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">        
        <div class="editor-label">
            <label>
                <span class="required">* </span>Currency:</label>
            <%: Html.CurrencyDropdownListFor(exchangeRate => exchangeRate.CurrencyId) %>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.CurrencyId) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective From:</label>
            <%:Html.TextBox("EffectiveFromDate",null, new { @class = "datePicker", @id = "EffectiveFromDate"})%>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.EffectiveFromDate) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective To:</label>
            <%:Html.TextBox("EffectiveToDate", null, new { @class = "datePicker", @id = "EffectiveToDate" })%>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.EffectiveToDate) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Five Day Rate USD:</label>
            <%: Html.TextBoxFor(exchangeRate => exchangeRate.FiveDayRateUsd, new { @class = "exchangeRate", @maxLength = 16 })%>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.FiveDayRateUsd) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Five Day Rate GBP:</label>
            <%: Html.TextBoxFor(exchangeRate => exchangeRate.FiveDayRateGbp, new { @class = "exchangeRate", @maxLength = 16 })%>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.FiveDayRateGbp)%>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Five Day Rate EUR:</label>
            <%: Html.TextBoxFor(exchangeRate => exchangeRate.FiveDayRateEur, new { @class = "exchangeRate", @maxLength = 16 })%>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.FiveDayRateEur) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(exchangeRate => exchangeRate.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(exchangeRate => exchangeRate.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ExchangeRate") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/ExchangeRateValidate.js")%>" type="text/javascript"></script>

</asp:Content>
