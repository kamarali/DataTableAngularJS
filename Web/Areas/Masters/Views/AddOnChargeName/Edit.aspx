<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.AddOnChargeName>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add On Charge Name
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
    <h2>
        Edit Add On Charge Name</h2>
   <% using (Html.BeginForm("Edit", "AddOnChargeName", FormMethod.Post, new { @id = "AddOnChargeNameMaster" }))
       {%>
    {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Add On Charge Name:
            </label>
            <%: Html.TextBoxFor(model => model.Name, new { @class = "alphaNumericWithSpace", @maxLength = 30 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Billing Category:
            </label>
            <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId) %>
            <%: Html.ValidationMessageFor(model => model.BillingCategoryId) %>
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
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","AddOnChargeName") %>'" />
        </div>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/AddOnChargeNameValidate.js")%>" type="text/javascript"></script>
</asp:Content>
