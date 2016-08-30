<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.TaxCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: Edit Tax Code Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Tax Code Setup</h1>
    <h2>
        Edit Tax Code</h2>
    <% using (Html.BeginForm("Edit", "TaxCode", FormMethod.Post, new { @id = "TaxCodeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Tax Code:
            </label>
            <%: Html.TextBoxFor(model => model.Id, new { @maxLength = 3, @class = "alphaNumeric upperCase", @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Tax Code Type:
            </label>
            <%: Html.TextBoxFor(model => model.TaxCodeTypeId, new { @maxLength = 1, @class = "alphabet upperCase" })%>
            <%: Html.ValidationMessageFor(model => model.TaxCodeTypeId) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Description:
            </label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Description, new { @maxLength = 255, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","TaxCode") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/TaxCodeValidate.js")%>" type="text/javascript"></script>
</asp:Content>
