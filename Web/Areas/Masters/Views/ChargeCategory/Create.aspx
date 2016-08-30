<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.ChargeCategory>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Charge Category
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create Charge Category</h2>
    <% using (Html.BeginForm("Create", "ChargeCategory", FormMethod.Post, new { @id = "ChargeCategoryMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Charge Category Name:
            </label>
            <%: Html.TextBoxFor(chargeCategory => chargeCategory.Name, new { @maxLength = 25 })%>
            <%: Html.ValidationMessageFor(chargeCategory => chargeCategory.Name) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Billing Category:
            </label>
            <%: Html.BillingCategoryDropdownListFor(chargeCategory => chargeCategory.BillingCategoryId) %>
            <%: Html.ValidationMessageFor(chargeCategory => chargeCategory.BillingCategoryId) %>
        </div>
        <div class="editor-label">
            <label>
                Description:
            </label>
            <%: Html.TextAreaFor(chargeCategory => chargeCategory.Description, 3, 60, new { @maxLength = 255 })%>
            <%: Html.ValidationMessageFor(chargeCategory => chargeCategory.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(chargeCategory => chargeCategory.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(chargeCategory => chargeCategory.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ChargeCategory") %>'" />
        </div>
    </fieldset>
    <% } %>
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/ChargeCategoryValidate.js")%>" type="text/javascript"></script>

</asp:Content>
