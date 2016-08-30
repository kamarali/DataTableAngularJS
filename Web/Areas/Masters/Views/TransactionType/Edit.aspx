<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.TransactionType>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Transaction Type
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1>
        Transaction Type Setup
    </h1><h2>
        Edit Transaction Type</h2>
    <% using (Html.BeginForm("Edit", "TransactionType", FormMethod.Post, new { @id = "TransactionTypeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <div class="editor-label">
                <label>
                    <span class="required">* </span>Transaction Type:</label>
                <%: Html.TextBoxFor(model => model.Name, new {@class="upperCase", @maxLength=50})%>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
            <div class="editor-label">
                <label>
                    <span class="required">* </span>Billing Category:</label>
                <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryCode,"BillingCategory") %>
                <%: Html.ValidationMessageFor(model => model.BillingCategoryCode) %>
            </div>
            <div class="editor-label">
                <label>
                    <span class="required">* </span>Description:</label>
                <!--SCP304020: UAT 1.6: Misc Codes Setup-->
                <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "upperCase validateCharactersForTextArea textAreaTrimText" })%>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </div>
            <div class="editor-label">
                <label>
                    Active:</label>
                <%: Html.CheckBoxFor(model => model.IsActive) %>
                <%: Html.ValidationMessageFor(model => model.IsActive) %>
            </div>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","TransactionType") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/TransactionTypeValidate.js")%>" type="text/javascript"></script>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#Name').focus(); });   </script>
</asp:Content>
