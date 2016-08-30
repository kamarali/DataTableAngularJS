<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Country>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Edit ISO Country and DS Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ISO Country and DS Setup</h1>
    <h2>
        Edit ISO Country</h2>
    <% using (Html.BeginForm("Edit", "Country", FormMethod.Post, new { @id = "CountryMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        
        <div>
            <label>
                <span class="required">* </span>Country Code:
            </label>
            <%: Html.TextBoxFor(country => country.Id, new { @class = "alphabetsOnly upperCase", @maxLength = 2, @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(country => country.Id) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Country Name:
            </label>
            <%: Html.TextBoxFor(country => country.Name, new {@maxLength = 50 })%>
            <%: Html.ValidationMessageFor(country => country.Name) %>
        </div>        
        <div>
            <label>
               Legal Format for Digital Signature:
            </label>
            <%: Html.DropDownListFor(country => country.DsFormat, ViewData["DsFormatList"] as SelectList)%> 
            <%: Html.ValidationMessageFor(country => country.DsFormat) %>
        </div>
        <div>
            <label>
                Digital Signature Supported?:
            </label>
            <%: Html.CheckBoxFor(country => country.DsSupportedByAtos)%>
            <%: Html.ValidationMessageFor(country => country.DsSupportedByAtos) %>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(country => country.IsActive) %>
            <%: Html.ValidationMessageFor(country => country.IsActive) %>
        </div>
        <div class="clear">
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Country") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/CountryValidate.js")%>" type="text/javascript"></script>
</asp:Content>
