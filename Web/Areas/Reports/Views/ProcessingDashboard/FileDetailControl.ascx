<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript">
   
    $(document).ready(function () {

    });

    function GetFileDetail(postUrl) {

        // Create URL to call "GetInvoiceAndFileGridData" action passing it filter criteria
        var id = $("#SelectedFiles :selected").val();
        var url = postUrl + "?" + $.param({ fileId: id});
        $("#InvoiceDetailWarningGrid").jqGrid('setGridParam', { url: url }).trigger("reloadGrid");
    }
  
</script>
<div>
  <div>
     <select name="SelectedFiles" style="width:270px;" id="SelectedFiles" onchange="GetFileDetail('<%: Url.Action("GetFileDetailForWarningDialogue", "ProcessingDashboard", new { area = "Reports"}) %>');">
     </select>
    <span id="fileErrorCount"></span> <span>File(s) With Error</span>
  </div>
  <div>
    <% Html.RenderPartial("FileStatusLateSubmissionWarningGrid", ViewData["InvoiceDetailWarning"]); %>
  </div>
  <div style=" margin-top:15px;">
    <div class="buttonContainer">
    <span>Do you want to continue?</span>
    <input class="primaryButton" type="submit" value="Yes" onclick="SubmitForLateSubmission()"/>
    <input class="secondaryButton" type="submit" value="No" onclick="FileDailogClose()" />
  </div>
  </div>
</div>

