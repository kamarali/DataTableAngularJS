<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.IchZone>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ich Zone
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Edit Ich Zone</h2>
    <% using (Html.BeginForm("Edit", "IchZone", FormMethod.Post, new { @id = "IchZoneMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Ich Zone:</label>
            <%: Html.TextBoxFor(model => model.Zone, new { @class = "alphabet upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.Zone) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Clearance Currency:</label>
            <%: Html.TextBoxFor(model => model.ClearanceCurrency, new { @class = "alphabetsOnly upperCase", @maxLength = 3 })%>
            <%: Html.ValidationMessageFor(model => model.ClearanceCurrency)%>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Description:</label>
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @class = "alphaNumericWithSpace", @maxLength = 255 })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive)%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","IchZone") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/IchZoneValidate.js")%>" type="text/javascript"></script>

</asp:Content>
