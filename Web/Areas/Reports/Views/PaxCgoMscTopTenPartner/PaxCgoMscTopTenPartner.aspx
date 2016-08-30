<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports ::<% =ViewData["ReportPageName"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <%Html.BeginForm("", "", FormMethod.Post, new { id = "PaxCgoMscTopTenPartner" }); %>
    <h1>
        <span id="headers"><% =ViewData["ReportPageName"]%></span>
    </h1>
    <div>
        <% Html.RenderPartial("PaxCgoMscTopTenPartnerSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="PaxCgoMscTopTenPartnerReport();" />
    </div>
    <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<%--<script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/CgoRejectionAnalysisRec.js")%>"></script>   --%>
 <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/SearchPageValidation.js")%>"></script>   
</asp:Content>
