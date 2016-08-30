<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.BvcMatrix>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: Add BVC Matrix Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        BVC Matrix Setup</h1>
    <h2>
        Add BVC Matrix</h2>
    <% using (Html.BeginForm("Create", "BvcMatrix", FormMethod.Post, new { @id = "BvcMatrixMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                Validated PMI:</label>
            <%: Html.TextBoxFor(model => model.ValidatedPmi, new { @class = "alphaNumeric upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.ValidatedPmi) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective From:</label>
            <%:Html.TextBox("EffectiveFrom", null, new { @class = "EffectiveBillingPeriod", @id = "EffectiveFrom", @maxLength = 8 })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveFrom) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective To:</label>
            <%:Html.TextBox("EffectiveTo", null, new { @class = "EffectiveBillingPeriod", @id = "EffectiveTo", @maxLength = 8 })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveTo) %>
        </div>
        <div class="editor-label">
            <label>
                Fare Amount:</label>
            <%: Html.CheckBoxFor(model => model.IsFareAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsFareAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Hf Amount:</label>
            <%: Html.CheckBoxFor(model => model.IsHfAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsHfAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Tax Amount:</label>
            <%: Html.CheckBoxFor(model => model.IsTaxAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsTaxAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Isc Percentage:</label>
            <%: Html.CheckBoxFor(model => model.IsIscPercentage)%>
            <%: Html.ValidationMessageFor(model => model.IsIscPercentage) %>
        </div>
        <div class="editor-label">
            <label>
                Uatp Percentage:</label>
            <%: Html.CheckBoxFor(model => model.IsUatpPercentage)%>
            <%: Html.ValidationMessageFor(model => model.IsUatpPercentage) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","BvcMatrix") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/BvcMatrixValidate.js")%>" type="text/javascript"></script>
    <script type="text/javascript">
        $('#EffectiveFrom').watermark("YYYYMMPP");
        $('#EffectiveTo').watermark("YYYYMMPP");
    </script>
</asp:Content>
