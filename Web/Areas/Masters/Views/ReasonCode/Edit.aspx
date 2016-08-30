<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ReasonCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Reason Code
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Reason Code Setup</h1>

    <h2>Edit Reason Code</h2>

    <% using (Html.BeginForm("Edit", "ReasonCode", FormMethod.Post, new { @id = "ReasonCodeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Reason Code:</label>
            <%: Html.TextBoxFor(model => model.Code, new { @class = "alphaNumeric upperCase", @maxLength = 5 })%>
            <%: Html.ValidationMessageFor(model => model.Code) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Transaction Type:</label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId) %>
            <%: Html.ValidationMessageFor(model => model.TransactionTypeId) %>
        </div>
        <div class="editor-label">
            <label>
               Description:</label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Coupon Awb Breakdown Mandatory:</label>
            <%: Html.CheckBoxFor(model => model.CouponAwbBreakdownMandatory) %>
            <%: Html.ValidationMessageFor(model => model.CouponAwbBreakdownMandatory) %>
        </div>
        <div class="editor-label">
            <label>
                Bilateral Code:</label>
            <%: Html.CheckBoxFor(model => model.BilateralCode)%>
            <%: Html.ValidationMessageFor(model => model.BilateralCode) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive)%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ReasonCode") %>'" />
        </div>
    </fieldset>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/ReasonCodeValidate.js")%>" type="text/javascript"></script>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#Code').focus(); });   </script>
</asp:Content>

