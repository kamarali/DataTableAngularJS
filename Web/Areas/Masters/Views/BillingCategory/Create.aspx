<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.BillingCategory>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Billing Category
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create Billing Category</h2>
    <% using (Html.BeginForm("Create", "BillingCategory", FormMethod.Post, new { @id = "BillingCategoryMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Billing Category Code:
            </label>
            <%: Html.TextBoxFor(model => model.CodeIsxml, new { @maxlength=25})%>
            <%: Html.ValidationMessageFor(model => model.CodeIsxml) %>
        </div>
        <div class="editor-label">
           <label>
                Description:
            </label>
            <%: Html.TextAreaFor(model => model.Description,3,60, new { @maxlength=255})%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
         <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","BillingCategory") %>'" />
        </div>
        
    </fieldset>
    <% } %>
   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
     <script src="<%:Url.Content("~/Scripts/Masters/BillingCategoryValidate.js")%>" type="text/javascript"></script>
</asp:Content>
