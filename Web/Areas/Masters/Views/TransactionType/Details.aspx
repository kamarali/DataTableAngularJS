<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.TransactionType>" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Transaction Type details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Transaction Type Setup
    </h1><h2>
        Transaction Type Details</h2>
    <fieldset class="solidBox dataEntry">
        <div>
            <div>
                Name:
                <%: Model.Name %><br /></div>
            <div>
                <br />Billing Category Code:
                <%: (BillingCategoryType)Model.BillingCategoryCode%></div>
            <div>
                <br />Description:<%: Model.Description %></div>
            <div>
                <br />Last Updated By:
                <%: Model.LastUpdatedBy %></div>
            <div>
                <br />Last Updated On:
                <%: String.Format("{0:g}", Model.LastUpdatedOn) %></div>
            <div>
                <br />Active:
                <%: Model.IsActive %></div>
            <div class="buttonContainer">
                <br />
                <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","TransactionType") %>'" />
            </div>
        </div>
    </fieldset>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
