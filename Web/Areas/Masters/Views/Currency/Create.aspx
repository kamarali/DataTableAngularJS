<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Currency>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Currency Related :: ISO Add ISO Currency Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ISO Currency Setup
    </h1>
    <h2>
        Add ISO Currency</h2>
    <% using (Html.BeginForm("Create", "Currency", FormMethod.Post, new { @id = "CurrencyMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span> Currency Numeric Code:</label>
            <%: Html.TextBoxFor(currency => currency.Id, new { @Class = "integer", @maxLength = 5 })%>
            <%: Html.ValidationMessageFor(currency => currency.Id)%>
        </div>
        <div>
            <label>
                <span class="required">* </span>Currency Alpha Code:</label>
            <%: Html.TextBoxFor(currency => currency.Code, new { @Class = "alphabetsOnly upperCase", @maxLength = 3 })%>
            <%: Html.ValidationMessageFor(currency => currency.Code) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Currency Name:</label>
            <%: Html.TextBoxFor(currency => currency.Name, new {@maxLength = 50 })%>
            <%: Html.ValidationMessageFor(currency => currency.Name) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Currency Precision:</label>
            <%: Html.CurrencyPrecisionDropdownListFor(currency => currency.Precision, new { @Class = "integer", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(currency => currency.Precision) %>
        </div>
        <div>
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(currency => currency.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(currency => currency.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Currency") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/CurrencyValidate.js")%>" type="text/javascript"></script>
</asp:Content>
