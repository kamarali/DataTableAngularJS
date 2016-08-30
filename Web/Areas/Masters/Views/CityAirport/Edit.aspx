<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.CityAirport>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Edit City and Airport Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        City and Airport Setup</h1>
    <h2>
        Edit City and Airport</h2>
    <% using (Html.BeginForm("Edit", "CityAirport", FormMethod.Post, new { @id = "CityAirportMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>City Airport Code:
            </label>
            <%--SCP205281:Error when searching City/Airport Master Table --%>
            <%: Html.TextBoxFor(model => model.CityAirportCodeDisplayName, new { @maxlength = 4, @class = "alphabetsOnly upperCase", @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        
        <div>
            <label>
                <span class="required">* </span>City Name:
            </label>
            <%: Html.TextBoxFor(model => model.Name, new {@maxlength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Country Name:
            </label>
            <%: Html.CountryCodeDropdownListFor(model => model.CountryId)%>
            <%: Html.ValidationMessageFor(model => model.CountryId) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Main City Code:
            </label>
            <%: Html.TextBoxFor(model => model.MainCity, new { @maxlength = 4, @class = "alphabetsOnly upperCase" })%>
            <%: Html.ValidationMessageFor(model => model.MainCity) %>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","CityAirport") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/CityAirportValidate.js")%>" type="text/javascript"></script>
</asp:Content>
