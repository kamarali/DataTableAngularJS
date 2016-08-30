<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Airport>" %>

<%@ Import Namespace="Iata.IS.Web.UIModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Add ICAO Airport Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ICAO Airport Setup</h1>
    <h2>
        Add ICAO Airport</h2>
    <% using (Html.BeginForm("Create", "Airport", FormMethod.Post, new { @id = "AirportMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Country:
            </label>
            <%: Html.CountryCodeDropdownListFor(model => model.CountryCode)%>
            
            <%: Html.ValidationMessageFor(model => model.CountryCode) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Airport ICAO Code:</label>
            <%: Html.TextBoxFor(model => model.Id, new { @maxLength = 4, @class = "alphabetsOnly upperCase" })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Airport Icao Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @class = "alphaNumericWithSpace", @maxLength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div>
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Airport") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/AirportValidate.js")%>" type="text/javascript"></script>
</asp:Content>
