<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<%--CMP #675: Progress Status Bar for Processing of Billing Data Files. 
    Desc: Newly added script referred here, ut is used for rendering progress bar UI after applying appropriate css classes. --%>
<script src="<%=Url.Content("~/Scripts/ProgressBar.js")%>" type="text/javascript"></script>

<% using (Html.BeginForm("CurrentStats", "ManageSystemMonitor", FormMethod.Post, new { id = "CurrentStats" }))
   {%>

<script language="javascript" type="text/javascript">
  function formatJobCompleteFileName(cellValue, options, rowObject) {
      var fileId = rowObject.FileId;
      var isPurged = rowObject.IsPurged;

        var strFun = "javascript:PostData";
        
        var funcCall = strFun + "('" + fileId + "');";

        var linkHtml = cellValue;
       
        if (isPurged == "True") {
          linkHtml = cellValue;         
        }
        else {
          linkHtml = "<a href=" + funcCall + ">" + cellValue + '</a>';         
         }

        return linkHtml;
    }

    function formatJobCompleteIspurged(cellValue, options, rowObject) {

        var isPurged = rowObject.IsPurged;    

      if (isPurged == "True") {
        linkHtml = "Yes";
      }
      else {
        linkHtml = "No";
      }
      return linkHtml;
    }


    function formatFileName(cellValue, options, rowObject) {

        var fileId = rowObject.FileId;

      var strFun = "javascript:PostData";
      
      var funcCall = strFun + "('" + fileId + "');";

      var linkHtml = "<a href=" + funcCall + ">" + cellValue + '</a>';
      return linkHtml;
    }

    function PostData(datatosend) {

        var actionMethod = '<%:Url.Action("DownloadFile", "ManageSystemMonitor", new { area = "ISOps" })%>';       
        var myForm = document.createElement("form");
        myForm.method = "post";
        myForm.action = actionMethod;
        var myInput = document.createElement("input");

        myInput.setAttribute("name", "FileToDownload");
        myInput.setAttribute("value", datatosend);

        myForm.appendChild(myInput);
        document.body.appendChild(myForm);
        myForm.submit();
        document.body.removeChild(myForm);
    }
    function IsWebResponseGrid_GetCellBoldValue(cellValue, options, rowObject) {
        if (cellValue.toUpperCase().indexOf('TOTAL') != -1) {
            return '<b>' + cellValue + '</b>';
        }
        return cellValue;

    }

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
    Desc: Hiding pop-up div on page load, default behaviour. */
    $(document).ready(function () {

        $dialog = $('<div></div>')
            .html($("#FileProgressDiv"))
            .dialog({
                autoOpen: false,
                title: 'Processing Progress Status',
                height: 60,
                width: 240,
                modal: true,
                resizable: false
            });
    });

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files 
    Desc: Client side function executing on image click. */
    function ShowFileProgressStatus(IsFileLogId) {

        var actionMethod = '<%:Url.Action("GetFileProgressDetails", "Data", new { area = "" })%>';
        $.ajax({
            type: "POST",
            url: actionMethod,
            data: { fileLogId: IsFileLogId },
            dataType: "json",
            success: function (response) {

                if (response.IsSuccess == true) {
                    showFileProgressStatusDialog(response.Process, response.State, response.Position);
                }
                else {
                    alert('Unable to retriever File Processing Progress Details or File Status may have recently changed, please refresh grid and then try again.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.statusText);
                alert(thrownError);
            }
        });
    }

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files
    Desc: Function to create File Status Dialog on this page. */
    function showFileProgressStatusDialog(Process_Name, Process_State, Queue_Position) {

        if (Process_Name == 'NONE' || Process_State == 'NONE') {
            alert('Unable to retriever File Processing Progress Details or File Status may have recently changed, please refresh grid and then try again.');
            return false;
        }
        if (Process_State == 'PENDING' && Queue_Position == '-1') {
            alert('Unable to retriever File Processing Progress Details or File Status may have recently changed, please refresh grid and then try again.');
            return false;
        }

        $dialog = $('<div></div>')
		    .html($("#FileProgressDiv"))
		    .dialog({
		        autoOpen: true,
		        title: 'Processing Progress Status',
		        height: 200,
		        width: 700,
		        modal: true,
		        resizable: false
		    });

        /* Common logic to render progress bar. */
        renderProgressBar(Process_Name, Process_State, Queue_Position);
        return false;
    }

</script>
  
<div class="solidBox" style="width: 101%;">
    <div class="fieldContainer horizontalFlow" style="height: 250px;">
        <div style="width: 550px; float: left;">

           <h2>Pending Jobs</h2>

            <%--CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Call hooked to generate script for new action (image) column added. --%>
            <%:ScriptHelper.GenerateProgressBarGridScript(Url, ControlIdConstants.SMCurrentStatsGrid)%>

            <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.PendingJobGrid]); %>
        </div>
    </div>

    <%--CMP #675: Progress Status Bar for Processing of Billing Data Files--%>
    <div id="FileProgressDiv">
           <%
            Html.RenderPartial("ProgressBar"); %>
    </div>

</div>
<div style="height: 10px;">
</div>
<div class="solidBox" style="width: 101%;">
    <div class="fieldContainer horizontalFlow" style="height: 250px;">
        <div style="width: 550px;">
            <h2>
                Completed Jobs</h2>
            <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CompletedJobGrid]); %>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox" style="width: 101%;">
    <div class="fieldContainer horizontalFlow" style="height: 250px;">
        <div style="width: 550px;">
            <h2>
                Outstanding BVC Response From ATPCO</h2>
            <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.OutstandingItemsGrid]); %>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<!--
<div class="solidBox" style="width: 101%;">
  <div class="fieldContainer horizontalFlow" style="height: 250px;">
    <div style="width: 350px; float: left;">
      <h2>
        IS-WEB Response Time Statistics</h2>
      <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.ISWEBResponseGrid]); %>
    </div>
  </div>
</div>

<div style="height: 10px;">
</div>
-->
<div class="solidBox" style="width: 101%;">
    <div class="fieldContainer horizontalFlow" style="height: 300px;">
        <div style="width: 33%; float: left;">
            <h2>
                System Alerts</h2>
            <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SystemAlertsJobGrid]); %>
        </div>
        <div style="width: 34%; float:left">
            <h2>
                Logged-in Users By Member</h2>
            <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.LoggedInUsersJobGrid]); %>
        </div>
        <div style="width: 33%;">
            <h2>IS-WEB Response Time Stats in Percentile (Last 24 Hrs)</h2>
            
            <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.ISWEBResponseGrid]); %>
        </div>
    </div>
</div>
<script language="javascript" type="text/javascript">
    function ViewLoggedInUsers() {
        $("#dialogLoggedInUsers").dialog({
            autoOpen: true,
            title: 'Logged-In Users By Region',
            height: 430,
            width: 600,
            modal: true,
            resizable: true
        });

        var url = '<%:Url.Action("UserCountByRegion", "ManageSystemMonitor", new { area = "ISOps"}) %>';
        $("#modalIFrame").attr('src', url);
        $("#modalIFrame").attr('width', 560);
        $("#modalIFrame").attr('height', 370);

    }

</script>
<%}%>
