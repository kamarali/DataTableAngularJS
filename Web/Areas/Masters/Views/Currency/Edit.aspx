<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Currency>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Currency Related :: ISO Edit ISO Currency Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ISO Currency Setup
    </h1>
    <h2>
        Edit ISO Currency</h2>
    <% using (Html.BeginForm("Edit", "Currency", FormMethod.Post, new { @id = "CurrencyMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Currency Numeric Code:</label>
            <%: Html.TextBoxFor(currency => currency.Id, new { @Class = "integer", @maxLength = 5, @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(currency => currency.Id)%>
        </div>
        <div>
            <label>
                <span class="required">* </span>Currency Alpha Code:</label>
            <%: Html.TextBoxFor(model => model.Code, new { @Class = "alphabetsOnly upperCase", @maxLength = 3 })%>
            <%: Html.ValidationMessageFor(model => model.Code) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Currency Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new {@maxLength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Currency Precision:</label>
            <%: Html.CurrencyPrecisionDropdownListFor(model => model.Precision, new { @Class = "integer", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.Precision) %>
        </div>
        <div>
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="clear">
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
