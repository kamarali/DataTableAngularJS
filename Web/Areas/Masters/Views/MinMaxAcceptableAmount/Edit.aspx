<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MinMaxAcceptableAmount>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Edit Minimum / Maximum Value Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <h1>
       Minimum / Maximum Value Setup
    </h1>
    <h2>
        Edit Minimum / Maximum Value</h2>
    <% using (Html.BeginForm("Edit", "MinMaxAcceptableAmount", FormMethod.Post, new { @id = "MinMaxAcceptableAmountMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Transaction Type:</label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId) %>
            <%: Html.ValidationMessageFor(model => model.TransactionTypeId) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Min:</label>
            <%: Html.TextBoxFor(model => model.Min, new { @class = "amt_10_3 amount", @maxLength = 11 })%>
            <%: Html.ValidationMessageFor(model => model.Min) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Max:</label>
            <%: Html.TextBoxFor(model => model.Max, new { @class = "amt_10_3 amount", @maxLength = 11 })%>
            <%: Html.ValidationMessageFor(model => model.Max) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Clearing House:</label>
            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.ClearingHouse) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","MinMaxAcceptableAmount") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
 <script src="<%:Url.Content("~/Scripts/Masters/MinMaxAcceptableAmountValidate.js")%>"
        type="text/javascript"></script>
</asp:Content>
