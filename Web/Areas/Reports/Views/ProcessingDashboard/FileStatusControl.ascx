<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript">
    var $Filedialog;
    $(document).ready(function () {

   // initFileStatusTabValidations();   
      
        $("#isWindowOpen").val('<%= ViewData["IsLateSubOpen"] %>')

        $("#isInvoiceTabClicked").val('false');

        $Filedialog = $('<div></div>')
		.html($("#divFileDetail"))
		.dialog({
		    autoOpen: false,
		    title: 'File(s) With Error',
		    height: 350,
		    width: 800,
		    modal: true,
		    resizable: false,
		    close: function (event, ui) {
		        $("#SelectedFiles option").remove();
		    },
		    open: function () {
		        var id = $("#SelectedFiles :selected").val();
		        if (id) {
		            var postUrl = $("#warningUrlName").val();
		            var url = postUrl + "?" + $.param({ fileId: id });
		            // fill warning dialog grid with by default selected dropdown value
		            $("#InvoiceDetailWarningGrid").jqGrid('setGridParam', { url: url }).trigger("reloadGrid");

		        }
		    }
		});

    });

  // Following function is used to mark Files for LateSubmission
function MarkFilesForLateSubmission() {

    if ($("#isWindowOpen").val().toLowerCase() == "false") {
        alert("Late Submission Window Is Closed");
        return;
    }
        // Get selected Files' Ids From FileStatusGrid  
          var selectedFileIds = $("#FSSearchResultListGrid").jqGrid('getGridParam', 'selarrrow');
        // If No File is selected popup an alert, else call MarkFileForLateSubmission() action which will
        // set IsLateSubmitted flag true for Invoices   in Selected Files
          if (selectedFileIds != "" && selectedFileIds.length > 0) {

              $("#selectedFilesForLateSubmission").val(selectedFileIds.toString())

                  $("#SelectedFiles option").remove();
                  var getIds = selectedFileIds.toString();
                  var countDropdownItem=0;
                  var fileNames;
                  var options = $("#SelectedFiles");
                  var sp = getIds.split(',');
                  var fileName="";

                  for (var i = 0; i <= sp.length; i++) {
                      var ret = jQuery("#FSSearchResultListGrid").jqGrid('getRowData', sp[i]);
                      // if 20= completed dont add in dd. if file format is isidec 1 or isxml 2 then only add in dd
                      if (ret.FileStatusId != 20 && (ret.FileFormatId == 1 || ret.FileFormatId == 2)) {
                          if (ret.FileStatusId == 18) {
                              // check if whole fire to be rejected on validation failure
                              // if some invoice in period error allow late submission
                              if (ret.RejectOnValidationFailure != 1 || ret.InvoicesInPeriodError > 0) {
                                  options.append($("<option />").val(sp[i]).text(ret.FileName));
                                  countDropdownItem = countDropdownItem + 1;
                              }
                              else {
                                  fileName = fileName + "," + ret.FileName;
                              }
                          }
                      }
                  }

                  // if no file is to be rejected if having any validation error then process furthre else 
                  // display alert.
                  if (fileName=="") {
                      var id = $("#SelectedFiles :selected").val();
                      // set no file items present in dropdown to show on warning dialog
                      $("#fileErrorCount").html("<b>" + countDropdownItem + "</b>");
                      // if no file with error dont show warning dilouge else dispaly it
                      if (id)
                          $Filedialog.dialog('open');
                      else
                          SubmitForLateSubmission();
                  }
                  else
                      alert("Following file(s) can not be marked for late submission. Please uncheck : " + fileName.substring(1));
             // }      
        } // end if()
        else {
          alert('Please select atleast one File.');
        } // end else

  } // end MarkInvoicesForLateSubmission()


  function SubmitForLateSubmission() {

      $("#SelectedFiles option").remove();
      $Filedialog.dialog("close");

      // Get selected Files' Ids From FileStatusGrid  
      var selectedFileIds = $("#selectedFilesForLateSubmission").val();
    
      $.ajax({
          type: "POST",
          url: '<%:Url.Action("MarkFileForLateSubmission", "ProcessingDashboard", new {area = "Reports"})%>',
          data: { selectedFileIds: selectedFileIds },
          success: function (response) {

              $("#FSFileActionResultsGrid").jqGrid('clearGridData');
              for (var i = 0; i <= response.length; i++)
                  $("#FSFileActionResultsGrid").jqGrid('addRowData', i + 1, response[i]);
              // Show "Late Submission Already Requested" column only when LateSubmission Grid details are displayed
              $("#FSFileActionResultsGrid").jqGrid('showCol', 'NumberOfAlreadyRequested');

              $("#divFileStatusActionResult").dialog({
                  autoOpen: true,
                  title: 'Results of File Action',
                  height: 270,
                  width: 850,
                  modal: true,
                  resizable: false
              });
              // On Success call "refreshInvoiceStatusGrid()" function which will refresh the Invoice grid
              refreshFileStatusGrid();
          },
          async: false
      }
      );
  } // end SubmitForLateSubmission()

  function FileDailogClose() {
      $("#SelectedFiles option").remove();
      $Filedialog.dialog("close");
  }

  // Following function is used to increment Billing period for Invoices within selected Files
  function IncrementBillingPeriodForInvoicesWithinFile() {
      // Get selected Files Id on FileStatusGrid  
      var selectedFileIds = $("#FSSearchResultListGrid").jqGrid('getGridParam', 'selarrrow');

      // If File is not selected popup an alert, else call IncrementInvoiceBillingPeriodWithinFile() action which will
      // increment Billing period for Invoices within selected Files  
      if (selectedFileIds != "" && selectedFileIds.length > 0 ) {

          // Give synchronous Ajax call to "IncrementBillingPeriodForInvoicesWithinFile" action
          $.ajax({
              type: "POST",
              url: '<%:Url.Action("IncrementBillingPeriodForInvoicesWithinFile", "ProcessingDashboard",
                                 new {area = "Reports"})%>',
              data: { selectedFileIds: selectedFileIds.toString() },
              success: function (response) {
                  // Clear FileActionResultGrid data                                    
                  $("#FSFileActionResultsGrid").jqGrid('clearGridData');

                  // Iterate through response receieved and add rows to ActionResult grid
                  for (var i = 0; i <= response.length; i++)
                      $("#FSFileActionResultsGrid").jqGrid('addRowData', i + 1, response[i]);

                  // Hide "Late Submission Already Requested" column when IncrementBillingPeriod details are displayed.
                  $("#FSFileActionResultsGrid").jqGrid('hideCol', 'NumberOfAlreadyRequested'); 
                                      
                  // Popup an dialog 
                  $("#divFileStatusActionResult").dialog({
                      autoOpen: true,
                      title: 'Results of File Action',
                      height: 270,
                      width: 760,
                      modal: true,
                      resizable: false
                  });

                  // On Success call "refreshFileStatusGrid()" function which will refresh the File grid
                  refreshFileStatusGrid();
              },
              async: false
          });
      } // end if()
      else {
          alert('Please select atleast one File.');
      } // end else

  } // end IncrementBillingPeriodForInvoicesWithinFile()

  function DeleteFiles() { 
      // Get selected Files Id on FileStatusGrid  
      var selectedFileIds = $("#FSSearchResultListGrid").jqGrid('getGridParam', 'selarrrow');
      $("#DeleteFile").attr('disabled', 'disabled');
      // If File is not selected popup an alert, else call DeleteFiles() action which will delete selected files.
      if (selectedFileIds != "" && selectedFileIds.length > 0) {

          if (confirm('Are you sure you want to delete this File(s)?')) {
              // Give synchronous Ajax call to "DeleteFiles" action
              $.ajax({
                  type: "POST",
                  url: '<%:Url.Action("DeleteFiles", "ProcessingDashboard", new {area = "Reports"})%>',
                  data: { selectedFileIds: selectedFileIds.toString() },
                  success: function (response) {
                      // Clear FileDeleteActionResultGrid data                                    
                      $("#FileDeleteActionStatusResultsGrid").jqGrid('clearGridData');

                      // Iterate through response receieved and add rows to ActionResult grid
                      for (var i = 0; i <= response.length; i++)
                          $("#FileDeleteActionStatusResultsGrid").jqGrid('addRowData', i + 1, response[i]);

                      // Popup an dialog 
                      $("#divFileStatusDeleteActionResult").dialog({
                      autoOpen: true,
                      title: 'Results of File Delete Action',
                      height: 270,
                      width: 830,
                      modal: true,
                      resizable: false
                      });

                      // On Success call "refreshFileStatusGrid()" function which will refresh the File grid
                      refreshFileStatusGrid();
                  },
                  async: false
              });
          }// end if()  
      } // end if()
      else {
          alert('Please select atleast one File.');
      } // end else     
      $("#DeleteFile").removeAttr('disabled');
     
  } // end DeleteFiles()
       
</script>

<%--CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Call hooked to generate script for new action (image) column added. --%>
<%:ScriptHelper.GenerateProgressBarGridScript(Url, ControlIdConstants.PDFileStatusGrid)%>

<div>
<h2>
    Search Criteria
  </h2>
  <div>
   <form id="ProcessingDashboardFileStatusForm">
   <input type="hidden" id="isWindowOpen" name="isWindowOpen" />
    <%Html.RenderPartial("ProcessingDashboardSearchCriteriaControl", ViewData["ProcessingDashboardSearch"]);%>    
   </form>
  </div>
  <h2>
    Search Results
  </h2>
  <div>
    <%
        Html.RenderPartial("FileStatusSearchResultControl", ViewData["FSSearchResultModel"]);%>
  </div>
  <div class="buttonContainer">
     <input type="hidden" id="selectedFilesForLateSubmission" name="selectedFilesForLateSubmission" />
    <input type="hidden" id="warningUrlName" name="urlname" value='<%:Url.Action("GetFileDetailForWarningDialogue", "ProcessingDashboard", new {area = "Reports"})%>' />
    <%
        if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.MarkforLateSubmission))
        {%>
             <input class="primaryButton" type="submit"  value="Mark for Late Submission" onclick="MarkFilesForLateSubmission()" disabled="disabled"/>
    <%
        }%>
    
          <input class="primaryButton"  type="button" value="Increment Billing Period"  id="fileStatusIncrementBillingPeriod" disabled="disabled" />
   

   
   
    <%
        using (
            Html.BeginForm("GenerateFileStatusCsv", "ProcessingDashboard", FormMethod.Post,
                           new {id = "GeneRateFileCsvFrm"}))
        {%>
     <input type="hidden" id="fileSearchCriteriaCsv" name="fileSearchCriteriaCsv" />
     <input type="hidden" id="fileBillingPeriodCsv" name="fileBillingPeriodCsv" />
     <input type="hidden" id="fileSearchTypeCsv" name="fileSearchTypeCsv" />
    
    <%
            if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.Download))
            {%>
                <input class="secondaryButton" type="submit" value="Download"  />
            <%
            }%>
            <% } %>
    
    
  <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.Delete))
      {%>
          <input class="secondaryButton" type="button" id="DeleteFile" value="Delete" onclick="DeleteFiles()" />
    <%
      }%>
    
  </div>
</div>

<div id="divFileStatusActionResult" class="hidden">
  <% Html.RenderPartial("~/Areas/Reports/Views/Shared/ProcessingDashboardFileActionResultsControl.ascx", ViewData["FileStatusActionResultList"]);%>
 </div>
 <div id="divFileStatusDeleteActionResult" class="hidden">
  <% Html.RenderPartial("~/Areas/Reports/Views/Shared/ProcessingDashboardFileDeleteActionStatusControl.ascx", ViewData["FileStatusDeleteActionResultList"]);%>
 </div>
 <div id="divFileDetail" class="removelisterror">
  <% Html.RenderPartial("FileDetailControl");%>
</div>
