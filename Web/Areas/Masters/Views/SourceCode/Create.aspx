<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SourceCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Source Code
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create Source Code</h2>
    <% using (Html.BeginForm("Create", "SourceCode", FormMethod.Post, new { @id = "SourceCodeMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Source Code Identifier:</label>
            <%: Html.TextBoxFor(model => model.SourceCodeIdentifier, new {@Class="number",@maxLength=2 })%>
            <%: Html.ValidationMessageFor(model => model.SourceCodeIdentifier) %>
        </div>
         <div class="editor-label">
             <label><span class="required">* </span>Transaction Type:</label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId) %>
            <%: Html.ValidationMessageFor(model => model.TransactionTypeId) %>
        </div>
         <div class="editor-label">
             <label>Utilization Type:</label>
            <%: Html.TextBoxFor(model => model.UtilizationType, new { @class = "alphaNumeric upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.UtilizationType) %>
        </div>
        <div class="editor-label">
           <label>Source Code Description:</label>
            <%: Html.TextBoxFor(model => model.SourceCodeDescription, new { @class = "alphaNumericWithSpace", @maxLength = 255 })%>
            <%: Html.ValidationMessageFor(model => model.SourceCodeDescription) %>
        </div>
        <div class="editor-label">
             <label>Include In Atpco Report:</label>
            <%: Html.CheckBoxFor(model => model.IncludeInAtpcoReport) %>
            <%: Html.ValidationMessageFor(model => model.IncludeInAtpcoReport) %>
        </div>
        <div class="editor-label">
             <label>FF Indicator:</label>
            <%: Html.CheckBoxFor(model => model.IsFFIndicator)%>
            <%: Html.ValidationMessageFor(model => model.IsFFIndicator) %>
        </div>
        <div class="editor-label">
             <label>Bilateral Code:</label>
            <%: Html.CheckBoxFor(model => model.IsBilateralCode)%>
            <%: Html.ValidationMessageFor(model => model.IsBilateralCode) %>
        </div>
        <div class="editor-label">
             <label>Rejection Level:</label>
            <%: Html.CheckBoxFor(model => model.IsRejectionLevel)%>
            <%: Html.ValidationMessageFor(model => model.IsRejectionLevel) %>
        </div>
        <div class="editor-label">
             <label>Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SourceCode") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/SourceCodeValidate.js")%>" type="text/javascript"></script>

</asp:Content>
