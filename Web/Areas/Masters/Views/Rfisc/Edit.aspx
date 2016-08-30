<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.Rfisc>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: Edit EMD RFISC Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        EMD RFISC Setup</h1>
    <h2>
        Edit EMD RFISC</h2>
    <% using (Html.BeginForm("Edit", "Rfisc", FormMethod.Post, new { @id = "RfiscMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <div class="editor-label">
                <label>
                    <span class="required">* </span>RFISC Code:</label>
                <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 3, @readonly = "readonly" })%>
                <%: Html.ValidationMessageFor(model => model.Id)%>
            </div>
            <div class="editor-label">
                <label>
                    <span class="required">* </span>RFIC Code:</label>
                <%= Html.RficDropdownListFor(model => model.RficId)%>
                <%: Html.ValidationMessageFor(model => model.RficId) %>
            </div>
            <div class="editor-label">
                <label>
                    Group Name:</label>
                <%: Html.TextBoxFor(model => model.GroupName, new {@maxLength = 50 })%>
                <%: Html.ValidationMessageFor(model => model.GroupName) %>
            </div>
            <div class="editor-label">
                <label>
                    <span class="required">* </span>Commercial Name:</label>
                <%: Html.TextBoxFor(model => model.CommercialName, new {@maxLength = 50 })%>
                <%: Html.ValidationMessageFor(model => model.CommercialName) %>
            </div>
            <div class="editor-label">
                <label>
                    Active:</label>
                <%: Html.CheckBoxFor(model => model.IsActive) %>
                <%: Html.ValidationMessageFor(model => model.IsActive) %>
            </div>
            <div class="buttonContainer">
                <input type="submit" value="Save" class="primaryButton" />
                <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Rfisc") %>'" />
            </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/RfiscValidate.js")%>" type="text/javascript"></script>
</asp:Content>
