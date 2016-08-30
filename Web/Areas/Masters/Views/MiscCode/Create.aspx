<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MiscCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Master Maintenance :: General :: Add Miscellaneous Codes Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Miscellaneous Codes Setup
    </h1><h2>
        Add Miscellaneous Code</h2>
    <% using (Html.BeginForm("Create", "MiscCode", FormMethod.Post, new { @id = "MiscCodeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Miscellaneous Code Group:</label>
        </div>
        <div class="editor-field">
            <%: Html.MiscCodeGroupDropdownListFor(model => model.Group,  new {style = "width:200px" })%>
            <%: Html.ValidationMessageFor(model => model.Group) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Miscellaneous Code:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @class = "alphaNumeric upperCase", @maxLength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Description:</label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 1000, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","MiscCode") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/MiscCodeValidate.js")%>" type="text/javascript"></script>

</asp:Content>
