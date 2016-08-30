<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.CountryIcao>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Edit ICAO Country Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ICAO Country Setup</h1>
    <h2>
        Edit ICAO Country</h2>
    <% using (Html.BeginForm("Edit", "CountryIcao", FormMethod.Post, new { @id = "CountryIcaoMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Country ICAO Code:
            </label>
            <%: Html.TextBoxFor(country => country.CountryCodeIcao, new { @class = "alphabetsOnly upperCase", @maxLength = 2, @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(country => country.Id) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Country Name:
            </label>
            <%: Html.TextBoxFor(country => country.Name, new { @class = "alphabetsWithSpace upperCase", @maxLength = 50 })%>
            <%: Html.ValidationMessageFor(country => country.Name) %>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(country => country.IsActive)%>
            <%: Html.ValidationMessageFor(country => country.IsActive) %>
        </div>
        <div class="clear">
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","CountryIcao") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/CountryIcaoValidate.js")%>" type="text/javascript"></script>
</asp:Content>
