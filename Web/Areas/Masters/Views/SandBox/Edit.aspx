<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SandBox.CertificationParameterMaster>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: SandBox
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Certification Parameters</h1>
    <h2>
        Edit Certification Parameters</h2>
    <% using (Html.BeginForm("Edit", "SandBox", FormMethod.Post, new { @id = "SandboxMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                Billing Category:</label>
            <%: Html.TextBoxFor(model => model.BillingCategory, new { @readonly = true })%>
            <%: Html.ValidationMessageFor(model => model.BillingCategory)%>
        </div>
         <br />
        <div class="editor-label">
            <label>
                File Format:</label>
            <%: Html.TextBoxFor(model => model.FileFormat, new { @readonly = true })%>
            <%: Html.ValidationMessageFor(model => model.FileFormat)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Transaction Group:</label>
            <%: Html.TextBoxFor(model => model.TransactionGroup, new { @readonly = true, style="width:250px;" })%>
            <%: Html.ValidationMessageFor(model => model.TransactionGroup)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Transaction Type:</label>
            <%: Html.TextBoxFor(model => model.TransactionType, new { @readonly = true, style="width:250px;" })%>
            <%: Html.ValidationMessageFor(model => model.TransactionType)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Minimum Transaction Count:</label>
            <%: Html.TextBoxFor(model => model.MinTransactionCount)%>
            <%: Html.ValidationMessageFor(model => model.MinTransactionCount)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Transaction Sub Type 1 Minimum Count:</label>
            <%: Html.TextBoxFor(model => model.TransactionSubType1MinCount)%>
            <%: Html.ValidationMessageFor(model => model.TransactionSubType1MinCount)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Transaction Sub Type 1:</label>
            <%: Html.TextBoxFor(model => model.TransactionSubType1Label)%>
            <%: Html.ValidationMessageFor(model => model.TransactionSubType1Label)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Transaction Sub Type 2 Minimum Count:</label>
            <%: Html.TextBoxFor(model => model.TransactionSubType2MinCount)%>
            <%: Html.ValidationMessageFor(model => model.TransactionSubType2MinCount)%>
        </div>
        <br />
        <div class="editor-label">
            <label>
                Transaction Sub Type 2:</label>
            <%: Html.TextBoxFor(model => model.TransactionSubType2Label)%>
            <%: Html.ValidationMessageFor(model => model.TransactionSubType2Label)%>
        </div>
        <br />
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SandBox") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
