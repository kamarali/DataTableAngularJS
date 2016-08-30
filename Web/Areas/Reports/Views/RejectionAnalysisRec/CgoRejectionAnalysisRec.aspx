<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Reports ::Cargo Rejection Analysis
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "CgoRejectionAnalysisRec" }); %>
    <h1>
        <span id="headers">Cargo Rejection Analysis</span>
    </h1>
    <div>
        <% Html.RenderPartial("CgoRejectionAnalysisRecSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="RejectionAnalysisReport('<%:Url.Action("ValidateBillingMonthYearSearch","ReceivablesReport")%>');" />
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/CgoRejectionAnalysisRec.js")%>"></script>   
</asp:Content>
