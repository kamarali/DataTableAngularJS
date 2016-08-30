<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SystemMonitor.OutputFile>" %>
<% using (Html.BeginForm("ResendFile", "ManageSystemMonitor", FormMethod.Post, new { id = "ManageSystemMonitorResending" }))
   {%>
<script type="text/javascript">
    $(document).ready(function () {
        $('#FileSubmissionFrom').focus();
        registerAutocomplete('FileMemberName', 'FileMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null, null, null, null, function (selectedId) { Member_AutoCompleteValueChange(selectedId); });
        $('#fileStatus').hide();

        //CMP#622: MISC Outputs Split as per Location IDs       
        registerAutocompleteWithOutCache('MiscLocationCode', 'MiscLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#FileMemberId', null);
        $("#FileMemberName").bind("change", function () {
          $('#MiscLocationCode').val('');
        });
    });

    //CMP#622: MISC Outputs Split as per Location IDs
    function Member_AutoCompleteValueChange(selectedId) {
        $('#MiscLocationCode').val('');        
        return true;
    }
    

     function formatViewFTPLog(cellValue, options, rowObject) {
       var fileId = "'" + cellValue + "'";
       var linkHtml = '<a href="#" onclick=showDialog(' + fileId + ')> View </a>';
       return linkHtml;
     }     

     function formatFileName(cellValue, options, rowObject) {

       /* CMP#622: Grid Column Index updated in order to get correct formatter applied. */
         var fileId = rowObject.FileId;
         var isPurged = rowObject.IsPurged;
         //var fileId = "'" + fileId + "'";

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

     function formatIspurged(cellValue, options, rowObject) {

       /* CMP#622: Grid Column Index updated in order to get correct formatter applied. */
         var isPurged = rowObject.IsPurged;
       if (isPurged == "True") {
           linkHtml = "Yes";
       }
       else {
           linkHtml = "No";
       }
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
     };
        
</script>
<h2>
  Search Criteria</h2>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style="float: left">
          <span>File Submission From Date:</span>
          <br />
          <%:Html.TextBox("FileSubmissionFrom", Model.FileSubmissionFrom !=null ? Convert.ToDateTime(Model.FileSubmissionFrom).ToString(FormatConstants.DateFormat) : string.Empty, new { @class = "datePicker", @id = "FileSubmissionFrom" })%>
        </div>
        <div style="float: left">
          <span>File Submission To Date:</span>
          <br />
          <%:Html.TextBox("FileSubmissionTo", Model.FileSubmissionTo != null ? Convert.ToDateTime(Model.FileSubmissionTo).ToString(FormatConstants.DateFormat) : string.Empty, new { @class = "datePicker", @id = "FileSubmissionTo" })%>
        </div>
        <div style="float: left">
          <span>Billing Period: </span>
          <br />
          <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.ProvisionalBillingPeriod)%>
        </div>
        <div style="float: left">
          <span>Billing Month: </span>
          <br />
          <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.ProvisionalBillingMonth)%>
        </div>
        <div style="float: left">
          <span>Billing Year: </span>
          <br />
          <%: Html.BillingYearDropdownListFor(searchCriteria => searchCriteria.ProvisionalBillingYear)%>
        </div>
      </div>
      <div>
        <div style="float: left">
          <span>Member: </span>
          <br />
          <%:Html.HiddenFor(searchCriteria => searchCriteria.FileMemberId, new { style = "width:200px;" })%>
          <%:Html.TextBoxFor(searchCriteria => searchCriteria.FileMemberName, new { @class = "autocComplete textboxWidth" })%>
        </div>
        <div>
          <span>File Status: </span>
          <br />
         <%-- //SCP#391788 - “validation Completed” in file status.
             //Desc: A file status "Validation Completed" has no use in dropdown list on resend tab of system monitor.--%>
          <%: Html.OutputFileStatusDropdownListFor(searchCriteria => searchCriteria.FileStatusId, new { style = "width:150px;" }, true)%>
        </div>
        <div style="float: left">
          <span>File Type: </span>
          <br />
          <%= Html.sysMonitorFileFormatTypeDropdownListFor(searchCriteria => searchCriteria.FileFormatId, true,  new { style = "width:150px;" })%>
        </div>
        <div style="float: left">
          <span>File Name: </span>
          <br />
          <%= Html.TextBoxFor(searchCriteria => searchCriteria.FileName,new {@Style="width:300px;"})%></div>
      </div>
      <div>
      <div style="float: left">
          <span>Location ID: </span>
          <br />          
          <%:Html.TextBoxFor(searchCriteria => searchCriteria.MiscLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>           
      </div>
      </div>
      <br />
      <br />
      <input class="primaryButton" type="button" name="btnSearch" value="Search" id="btnSearch"
        onclick="SearchFiles();" />
    </div>
  </div>
</div>
<div>
  <h2>
    Search Results</h2>
  <%Html.RenderPartial("ResendFileGrid", ViewData["SearchResultOutpuFileGrid"]); %>
</div>
<br />
<br />
<input class="primaryButton" type="button" name="btnresendFiles" value="Resend Selected Files"
  id="Button1" onclick="ResendFiles();" />
 <div id="fileStatus">
   <div id="divftpLog" style="overflow:auto;" />
     
</div>

<%}%>
<script src="<%:Url.Content("~/Scripts/Site.js")%>" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
  function validateDateRange(startDateId, endDateId) {
    var startDateVal = $('#' + startDateId).datepicker("getDate");
    var endDateVal = $('#' + endDateId).datepicker("getDate");

    return endDateVal >= startDateVal;
  }
  function SearchFiles() {
    var sdate = $("#FileSubmissionFrom").val();
    var edate = $("#FileSubmissionTo").val();
    if (sdate != null && sdate != '' && edate != null && edate != '') {
      if (!validateDateRange('FileSubmissionFrom', 'FileSubmissionTo')) {
        showClientErrorMessage(" 'File Submission From Date' must be less than or equal to the 'File Submission To Date'");
        return false;
      }
    }
    // Create searchCriteria in JSON format which contains values selected for Search criteria

    var provisionalBillingYear = $("#ProvisionalBillingYear option:selected").val();
    var provisionalBillingMonth = $("#ProvisionalBillingMonth option:selected").val();
    var provisionalBillingPeriod = $("#ProvisionalBillingPeriod option:selected").val();
    var fileMemberId = $("#FileMemberId").val();
    var fileStatusId = $("#FileStatusId option:selected").val();
    var fileFormatId = $("#FileFormatId option:selected").val();
    var fileName = $("#FileName").val();
    var miscLocationCode = $("#MiscLocationCode").val();
    var fileSubmissionFrom = $("#FileSubmissionFrom").val();
    var fileSubmissionTo = $("#FileSubmissionTo").val();
    if (provisionalBillingYear == null || provisionalBillingYear == '') {
      provisionalBillingYear = 0;
    }
    if (fileFormatId == null || fileFormatId == '') {
      fileFormatId = 0;
    }
    if (fileStatusId == null || fileStatusId == '') {
      fileStatusId = 0;
    }
    if (fileMemberId == null || fileMemberId == '') {
      fileMemberId = 0;
    }
    var postUrl = '<%: Url.Action("ResendFileSearchGridData", "ManageSystemMonitor", new { area = "ISOps"}) %>';
    var url = postUrl + "?" + $.param({ provisionalBillingYear: provisionalBillingYear,
      provisionalBillingMonth: provisionalBillingMonth,
      provisionalBillingPeriod: provisionalBillingPeriod,
      fileMemberId: fileMemberId,
      fileStatusId: fileStatusId,
      fileFormatId: fileFormatId,
      fileName: fileName,
      miscLocationCode: miscLocationCode,
      fileSubmissionFrom: fileSubmissionFrom,
      fileSubmissionTo: fileSubmissionTo
    });
    $("#SearchOutpuFileGrid").jqGrid('setGridParam', { url: url }).trigger("reloadGrid", [{ page: 1}]);
  }       
  
  function ResendFiles() {
    var selectedFileIds = $("#SearchOutpuFileGrid").jqGrid('getGridParam', 'selarrrow');
    if (selectedFileIds.length == 0) {
      showClientErrorMessage(" Please Select At Least One File To Resend.");
      return false;
    }
    $.ajax({    
      type: "POST",
      dataType: "json",
      url: '<%: Url.Action("ResendSelectedFiles", "ManageSystemMonitor", new { area = "ISOps"}) %>',
      data: { selectedFileIDs: selectedFileIds.toString() },
      success: function (response) {
        if (response.IsFailed) {
          if (response.Message) {
            showClientErrorMessage(response.Message);
          }
        } else {
          if (response.Message) {
            SearchFiles();
            showClientSuccessMessage(response.Message);
          }
        }
      },
      async: false
    }
      );
  }


  function showDialog(ftpId) {
    $('#divftpLog').html('');
    $('#fileStatus').show();
    $dialog = $('<div></div>')
		.html($("#fileStatus"))
		.dialog({
		  autoOpen: true,
		  title: 'FTP File Log',
		  height: 400,
		  width: 570,
		  modal: true,
		  resizable: true
		});
		GetFtpLog(ftpId);
    return false;

  }

  function GetFtpLog(fileId) {

    $.ajax({
      type: "POST",
      dataType: "json",
      url: '<%: Url.Action("GetFtpFileLog", "ManageSystemMonitor", new { area = "ISOps"}) %>',
      data: { fileId: fileId.toString() },
      success: function (response) {
      // Render FTP LOG
        $('#divftpLog').html(response.Message);

      },
      async: false
    }
      );

  }


</script>
