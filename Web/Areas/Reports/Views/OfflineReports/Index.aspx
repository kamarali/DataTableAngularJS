<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.OfflineReportLog.OfflineReportSearchCriteria>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Reports :: Download Offline Reports
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Download Offline Reports</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
    if (SessionUtil.MemberId == 0)
        Response.Redirect("~/Account/LogOn");
     using (Html.BeginForm("Index","OfflineReports", FormMethod.Get,new{id = "OfflineReportSearchForm"}))
      {
        Html.RenderPartial("OfflineReportSearchControl", Model);
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
     <%: ScriptHelper.GenerateGridDownloadScript(Url,ControlIdConstants.SearchGrid,
                       Url.Action("DownloadReport","OfflineReports",new
                       {
                       area = "Reports"
                       }))%>
  <div>
         <% Html.RenderPartial("GridControl",ViewData[ViewDataConstants.SearchGrid]);%>
  </div>
</asp:Content>


<asp:Content runat="server" ID="Script" ContentPlaceHolderID="Script">

    <script type="text/javascript" language="javascript">
        function downloadZip(methodName, value, offlineReportLogId, gridId) {
            var isDownload = window.open(methodName + '?downloadId=' + value + '&offlineReportLogId=' + offlineReportLogId);
            if (isDownload == null)
                showClientErrorMessage("Error occured while download report, please try again.");
            else {
                showClientSuccessMessage("Report downloaded  successfully.");
            }
        }

        $(document).ready(function () {
            
            $("#RequestDateTime").datepicker('setDate', '<%=ViewData[ViewDataConstants.RequestDateTime]%>');
            $('#ReportId').val('<%=ViewData[ViewDataConstants.ReportId]%>');
        });
    </script> 

</asp:Content>