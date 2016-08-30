<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.SettlementMethod>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Edit SettlementMethod Value Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <h1>
       Minimum Value Setup
    </h1>
    <h2>
        Edit Minimum Value</h2>
    <% using (Html.BeginForm("Edit", "SettlementMethod", FormMethod.Post, new { @id = "SettlementMethodMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editText">
            <label>
                <span class="required">* </span>Settlement Method Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @maxlength = 1, readOnly = true })%>
            <%: Html.ValidationMessageFor(model => model.Name)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Description:</label>
            <%: Html.TextBoxFor(model => model.Description)%>
            <%: Html.ValidationMessageFor(model => model.Description)%>
        </div>
        <div class="editText">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer editText">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SettlementMethod") %>'" />
        </div>
        
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Masters/SettlementMethodValidate.js")%>" ></script>
</asp:Content>
