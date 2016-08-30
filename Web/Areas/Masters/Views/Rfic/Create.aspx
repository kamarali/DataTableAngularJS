<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.Rfic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: Add EMD RFIC Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        EMD RFIC Setup</h1>
    <h2>
        Add EMD RFIC</h2>
    <% using (Html.BeginForm("Create", "Rfic", FormMethod.Post, new { @id = "RficMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>RFIC Code:</label>
            <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Description:</label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Rfic") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/RficValidate.js")%>" type="text/javascript"></script>
</asp:Content>
