<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Interline Payables Analysis
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

 <%Html.BeginForm("", "", FormMethod.Post, new { id = "InterlinePayablesAnalysis" }); %>
    <h1>
        Interline Payables Analysis
    </h1>
    <div>
        <% Html.RenderPartial("InterlinePayablesAnalysisSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="InterlinePayablesAnalysisReport();" />
    </div>
    <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/SearchPageValidation.js")%>"></script> 
</asp:Content>
