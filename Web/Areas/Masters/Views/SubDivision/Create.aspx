<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.SubDivision>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Add Area Sub Division Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Area Sub Division Setup
    </h1>
    <h2>
        Add Sub Division</h2>
    <% using (Html.BeginForm("Create", "SubDivision", FormMethod.Post, new { @id = "SubDivisionMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Sub Division Code:</label>
            <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 3 })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Sub Division Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new {@maxLength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Country Name:</label>
            <%: Html.CountryCodeDropdownListFor(model => model.CountryId) %>
            <%: Html.ValidationMessageFor(model => model.CountryId) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SubDivision") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/SubDivisionValidate.js")%>" type="text/javascript"></script>
</asp:Content>
