<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% using (Html.BeginForm("Graphs", "ManageSystemMonitor", FormMethod.Post, new { id = "ManageSystemMonitorLocation" }))
   {%>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <h2>
      Processed Data Import Files Graph </h2>
      <%--<input class="primaryButton" type="button" name="btnRefreshProcessedFilesChart" value="Refresh" id="btnRefreshProcessedFilesChart" onclick="RefreshProcessedFilesChart();"/>--%>
  </div>
   
   <div id="ProcessedFilesChart">
            <p>
                <img id="chart" src='<%:Url.Action("CreateProcessedFilesChart", "ManageSystemMonitor", new { area = "ISOps"}) %>' alt="" />
            </p>
        </div>
         <div style="padding-top: 13px; float: left; width: 150px;">
         <p> <input id="CreateProcessedFilesButton" type="button" value="Refresh" class="primaryButton" onclick="CreateProcessedFiles();" /></p>
         </div>
</div>
<div class="clear">
</div>
<div  style="height:10px; " >
</div>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <h2>
      Processed Supporting Documents Files Graph</h2>
      <%--<input class="primaryButton" type="button" name="btnProcessedSupprotingChart" value="Refresh" id="btnProcessedSupprotingChart" onclick="RefreshProcessedSupprotingChart();"/>--%>
  </div>
 
   <div id="ProcessedSupprotingChart">
            <p>
                 <img id="Processedchart" src='<%:Url.Action("CreateProcessedSupprotingChart", "ManageSystemMonitor", new { area = "ISOps"}) %>' alt="" />
            </p>
   </div>
    <div style="padding-top: 13px; float: left; width: 150px;">
     <p> <input id="Button1" type="button" value="Refresh" class="primaryButton" onclick="ProcessedSupprotingFiles();" /></p>
     </div>
</div>

<div class="clear">
</div>
<%
   }%>

   <script language="javascript" type="text/javascript">
     function RefreshProcessedSupprotingChart() {
       var newImage = $('<img />');
              newImage.attr('src', '<%: Url.Action("CreateProcessedSupprotingChart", "ManageSystemMonitor", new { area = "ISOps"}) %>');
       $("#ProcessedSupprotingChart").html('')
       $('#ProcessedSupprotingChart').append(newImage);
     }
     function RefreshProcessedFilesChart() {
       var newImage = $('<img />');
       newImage.attr('src', '<%: Url.Action("CreateProcessedFilesChart", "ManageSystemMonitor", new { area = "ISOps"}) %>');
       $("#ProcessedFilesChart").html('')
       $('#ProcessedFilesChart').append(newImage);
     }

   </script>