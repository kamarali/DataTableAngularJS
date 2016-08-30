<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.ProcessingDashBoard.SisUsageReportModel>" %>
<%--CMP #659: SIS IS-WEB Usage Report.--%>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: SIS IS-WEB Usage Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% Html.BeginForm("SisIsWebUsageReport", "SisUsageReport", FormMethod.Post, new { id = "SisIsWebUsageReport" }); %>
    <h1>SIS IS-WEB Usage Report</h1>
     
    <div>
        <% Html.RenderPartial("SearchControlForIsWebUsageReport"); %>
    </div>
    <div>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Offline CSV Report" />
    </div>
     <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src='<%:Url.Content("~/Scripts/SISUsageReport.js")%>'></script>
    <script type="text/javascript">
        $(document).ready(function () {
            ValidateSISUsageReport('<%: Url.Action("IsLegalArchivingProcessCompleted", "Data", new { area = ""})%>');
        }); 
  </script>
</asp:Content>
