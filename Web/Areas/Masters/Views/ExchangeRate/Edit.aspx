<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ExchangeRate>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Currency Related :: Edit Exchange Rate Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Exchange Rate Setup</h1><h2>
        Edit Exchange Rate</h2>
    <% using (Html.BeginForm("Edit", "ExchangeRate", FormMethod.Post, new { @id = "ExchangeRateMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Currency:</label>
            <%: Html.CurrencyDropdownListFor(model => model.CurrencyId) %>
            <%: Html.ValidationMessageFor(model => model.CurrencyId) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective From:</label>
            <%:Html.TextBox("EffectiveFromDate", Model.EffectiveFromDate.ToString(FormatConstants.DateFormat), new { @class = "datePicker", @id = "EffectiveFromDate" })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveFromDate) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective To:</label>
            <%:Html.TextBox("EffectiveToDate", Model.EffectiveToDate.ToString(FormatConstants.DateFormat), new { @class = "datePicker", @id = "EffectiveToDate" })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveToDate) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Five Day Rate USD:</label>
            <%: Html.TextBoxFor(model => model.FiveDayRateUsd, new { @class = "exchangeRate", @maxLength = 16 })%>
            <%: Html.ValidationMessageFor(model => model.FiveDayRateUsd) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Five Day Rate GBP:</label>
            <%: Html.TextBoxFor(model => model.FiveDayRateGbp, new { @class = "exchangeRate", @maxLength = 16 })%>
            <%: Html.ValidationMessageFor(model => model.FiveDayRateGbp) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Five Day Rate EUR:</label>
            <%: Html.TextBoxFor(model => model.FiveDayRateEur, new { @class = "exchangeRate", @maxLength = 16 })%>
            <%: Html.ValidationMessageFor(model => model.FiveDayRateEur) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
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
