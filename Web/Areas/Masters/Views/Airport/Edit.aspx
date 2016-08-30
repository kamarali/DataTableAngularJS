<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Airport>" %>

<%@ Import Namespace="Iata.IS.Web.UIModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Edit ICAO Airport Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ICAO Airport Setup</h1>
    <h2>
        Edit ICAO Airport</h2>
    <% using (Html.BeginForm("Edit", "Airport", FormMethod.Post, new { @id = "AirportMaster" }))
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
            <%: Html.TextBoxFor(model => model.Id, new { @maxLength = 4, @class = "alphabetsOnly upperCase", @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Airport Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @maxLength = 50, @class = "alphaNumericWithSpace" })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div>
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive)%>
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
    <script type="text/javascript">
        $(document).ready(function () {            
            $("#Id").attr("disabled", "disabled"); 
     });
    </script>
</asp:Content>
