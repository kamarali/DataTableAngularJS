<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.LocationIcao>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Master Maintenance :: Area Related :: Add ICAO Location Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <h1>
        ICAO Location Setup
    </h1><h2>
        Add ICAO Location</h2>
    <% using (Html.BeginForm("Create", "LocationIcao", FormMethod.Post, new { @id = "LocationIcaoMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Location ICAO Code:
            </label>
            <%: Html.TextBoxFor(model => model.Id, new { @class = "alphabetsOnly upperCase", @maxLength = 4 })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Country:
            </label>
            <%: Html.CountryCodeDropdownListForICAO(model => model.CountryCode) %>
            <%: Html.ValidationMessageFor(model => model.CountryCode) %>
        </div>
        <div>
            <label>
                Description:
            </label>
            <%--SCPID : 107323 - ICAO location code - Incorrect reference to country master--%>
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateAllowedCharactersForTextAreaFields" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","LocationIcao") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/LocationIcaoValidate.js")%>" type="text/javascript"></script>
</asp:Content>
