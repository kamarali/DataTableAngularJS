<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<![if IE 7]>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/json2.js")%>"></script>
  <![endif]>
<script type="text/javascript">
    $(document).ready(function () {
       // initInvoiceStausTabValidations();        
        $('#filestatuslink').focus();
        $("#isInvoiceTabClicked").val('<%: ViewData["IsInvoiceTabClicked"] %>');
        $("#PasswordQuestion").val("2");
        $("#isOpen").val('<%= ViewData["IsLateSubOpen"] %>');

        if ('<%: ViewData["ResubmitIsOpsOnly"] %>' == "true") {
            $('#rs').show();
        }
        else {
            $('#rs').hide();
        }

    });
     
        
    // Following function is used to Delete selected Invoices
    function DeleteSelectedInvoices()
    {
        // Get selected Invoices Id on InvoiceStatusGrid  
    	var selectedInvoiceIds = $("#ISSearchResultListGrid").jqGrid('getGridParam','selarrrow');
         $("#Delete").attr('disabled', 'disabled');
        // If Invoice is not selected popup an alert, else call DeleteSelectedInvoices() action which will
        // delete selected Invoices  
         if (selectedInvoiceIds != "" && selectedInvoiceIds.length > 0 )
        {
            // Give an Confirm box whether user want to delete selected Invoices, If true, then delete.
            if (confirm("Are you sure you want to delete selected Invoice(s)?"))
            {
                // Give synchronous Ajax call to "DeleteSelectedInvoices" action
                $.ajax({
                    type: "POST",
                    url: '<%: Url.Action("DeleteSelectedInvoices", "ProcessingDashboard", new { area = "Reports"}) %>',
                    data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
                    success: function (response) {
                        // Clear ActionResultGrid data                                    
                        $("#ISInvoiceActionResultsGrid").jqGrid('clearGridData');

                        // Iterate through response receieved and add rows to ActionResult grid
                        for(var i=0;i<=response.length;i++) 
                          $("#ISInvoiceActionResultsGrid").jqGrid('addRowData',i+1,response[i]); 

                        // Popup an dialog 
                        $("#divInvoiceStatusActionResult").dialog({
                            autoOpen: true,
                            title: 'Results of Invoice Action',
                            height: 300,
                            width: 550,
                            modal: true,
                            resizable: false
                        });
                        // On Success call "refreshInvoiceStatusGrid()" function which will refresh the Invoice grid
                        refreshInvoiceStatusGrid();
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        alert('An error occurred! ' + errorThrown);
                    },
                async: false
            });
            }
        }// end if()
        else
        {
            alert('Please select atleast one Invoice.');
        }// end else         
         $("#Delete").removeAttr('disabled');
    }// end DeleteSelectedInvoices()

     // Following function is used to Resubmit selected Invoices
    function ResubmitSelectedInvoices()
    {
        // Get selected Invoices Id on InvoiceStatusGrid  
    	var selectedInvoiceIds = $("#ISSearchResultListGrid").jqGrid('getGridParam','selarrrow');

        // If Invoice is not selected popup an alert, else call ResubmitSelectedInvoices() action which will
        // Resubmit selected Invoices  
    	if (selectedInvoiceIds != "" && selectedInvoiceIds.length > 0)
        {
            // Give an Confirm box whether user want to Resubmit selected Invoices, If true, then delete.
            if (confirm("Are you sure you want to Resubmit selected Invoice(s)?"))
            {
                // Give synchronous Ajax call to "ResubmitSelectedInvoices" action
                $.ajax({
                    type: "POST",
                    url: '<%: Url.Action("ResubmitSelectedInvoices", "ProcessingDashboard", new { area = "Reports"}) %>',
                    data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
                    success: function (response) {
                        // Clear ActionResultGrid data                                    
                        $("#ISInvoiceActionResultsGrid").jqGrid('clearGridData');

                        // Iterate through response receieved and add rows to ActionResult grid
                        for(var i=0;i<=response.length;i++) 
                          $("#ISInvoiceActionResultsGrid").jqGrid('addRowData',i+1,response[i]); 

                        // Popup an dialog 
                        $("#divInvoiceStatusActionResult").dialog({
                            autoOpen: true,
                            title: 'Results of Invoice Action',
                            height: 300,
                            width: 550,
                            modal: true,
                            resizable: false
                        });
                        // On Success call "refreshInvoiceStatusGrid()" function which will refresh the Invoice grid
                        refreshInvoiceStatusGrid();
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        alert('An error occurred! ' + errorThrown);
                    },
                async: false
            });
            }
        }// end if()
        else
        {
            alert('Please select atleast one Invoice.');
        }// end else

    }// end ResubmitSelectedInvoices()

    // Following function is used to increment Billing period for selected Invoices
    function IncrementInvoiceBillingPeriod()
    {
        // Get selected Invoices Id on InvoiceStatusGrid  
    	var selectedInvoiceIds = $("#ISSearchResultListGrid").jqGrid('getGridParam','selarrrow');

        // If Invoice is not selected popup an alert, else call IncrementInvoiceBillingPeriod() action which will
        // increment Billing period for selected Invoices  
        if(selectedInvoiceIds != "" && selectedInvoiceIds.length > 0 )
        {
           // Give synchronous Ajax call to "IncrementInvoiceBillingPeriod" action
            $.ajax({
                type: "POST",
                url: '<%: Url.Action("IncrementInvoiceBillingPeriod", "ProcessingDashboard", new { area = "Reports"}) %>',
                data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
                success: function (response) {
                    // Clear ActionResultGrid data                                    
                    $("#ISInvoiceActionResultsGrid").jqGrid('clearGridData');

                    // Iterate through response receieved and add rows to ActionResult grid
                    for(var i=0;i<=response.length;i++) 
                      $("#ISInvoiceActionResultsGrid").jqGrid('addRowData',i+1,response[i]); 

                    // Popup an dialog 
                    $("#divInvoiceStatusActionResult").dialog({
                    autoOpen: true,
                    title: 'Results of Invoice Action',
                    height: 270,
                    width: 450,
                    modal: true,
                    resizable: false
                    });
                    // On Success call "refreshInvoiceStatusGrid()" function which will refresh the Invoice grid
                    refreshInvoiceStatusGrid();
                },
                async: false
            });
        }// end if()
        else
        {
            alert('Please select atleast one Invoice.');
        }// end else

    }// end IncrementInvoiceBillingPeriod()

    // Following function is used to mark Invoices for LateSubmission
    function MarkInvoicesForLateSubmission()
    {
         if($("#isOpen").val().toLowerCase() == "false")
         {
            alert("Late Submission Window Is Closed");
            return;
         }
        // Get selected Invoices Id on InvoiceStatusGrid  
    	var selectedInvoiceIds = $("#ISSearchResultListGrid").jqGrid('getGridParam','selarrrow');
        // If Invoice is not selected popup an alert, else call MarkInvoiceForLateSubmission() action which will
    	// set IsLateSubmitted flag true for selected Invoices  
    	if (selectedInvoiceIds != "" && selectedInvoiceIds.length > 0)
        {
            // Give Ajax call to "MarkInvoiceForLateSubmission" action
            $.ajax({
                type: "POST",
                url: '<%: Url.Action("MarkInvoiceForLateSubmission", "ProcessingDashboard", new { area = "Reports"}) %>',
                data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
                success: function (response) {                                    
                    
                    $("#ISInvoiceActionResultsGrid").jqGrid('clearGridData');
                    for(var i=0;i<=response.length;i++) 
                      $("#ISInvoiceActionResultsGrid").jqGrid('addRowData',i+1,response[i]); 

                    $("#divInvoiceStatusActionResult").dialog({
                    autoOpen: true,
                    title: 'Results of Invoice Action',
                    height: 270,
                    width: 550,
                    modal: true,
                    resizable: false
                    });
                    // On Success call "refreshInvoiceStatusGrid()" function which will refresh the Invoice grid
                    refreshInvoiceStatusGrid();
                },
                async: false
            });
        }// end if()
        else
        {
            alert('Please select atleast one Invoice.');
        }// end else
        
    }// end MarkInvoicesForLateSubmission()

    function GetSelectedRecordId(ids) {
        var flag = 0;
        
        selectedTransactionIds = jQuery('#ISSearchResultListGrid').getGridParam('selarrrow');
        
        if (selectedTransactionIds && selectedTransactionIds != null && selectedTransactionIds.length >= 1) {
            for (i = 0; i < selectedTransactionIds.length; i++) {
                selectedTransactionId = selectedTransactionIds[i];
                var gridRow = $("#ISSearchResultListGrid").getRowData(selectedTransactionId);
                var billingMonth = gridRow.BillingPeriod;
                var monthArray = billingMonth.split('-');
                
                if(monthArray.length == 2) {
                    flag = 1;
                }
                
                if(monthArray[2] == 04) {
                    flag = 1;
                }
            }
            if(flag == 1) {
                $('#IncrementInvoice').attr('disabled','disabled');
                
                // Buttons which have disabled attribute are not grayed out in Firefox,
                // so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
                if ($.browser.mozilla) {
                    $("#IncrementInvoice").removeClass('primaryButton');
                    $("#IncrementInvoice").addClass('disabledButtonClassForMozilla');
                }
            }
            else {
                $('#IncrementInvoice').removeAttr('disabled');
                
                // If we remove "disabled" attribute from button and browser is Firefox,
                // add "primaryButton" class and remove "disabledButtonClassForMozilla" class 
                if ($.browser.mozilla) {
                    $("#IncrementInvoice").removeClass('disabledButtonClassForMozilla');
                    $("#IncrementInvoice").addClass('primaryButton');
                }
            }
        }
        else
        {
            $('#IncrementInvoice').removeAttr('disabled');

            // If we remove "disabled" attribute from button and browser is Firefox,
            // add "primaryButton" class and remove "disabledButtonClassForMozilla" class 
            if ($.browser.mozilla) {
                $("#IncrementInvoice").removeClass('disabledButtonClassForMozilla');
                $("#IncrementInvoice").addClass('primaryButton');
            }
        }
    }

    function DownloadCsv()
    {
        $("#searchCriteriaCsvServerSide").val( $("#searchCriteriaCsv").val());
        $("#GeneRateCsvFrm").submit();
    }
           
</script>
<div>
<h2>
    Search Criteria
  </h2>
 <div>
  <form id="ProcessingDashboardInvoiceStatusForm">

    <input type="hidden" id="isOpen" name="isOpen" />
    <% Html.RenderPartial("ProcessingDashboardSearchCriteriaControl", ViewData["ProcessingDashboardSearch"]); %>

</form>
  </div>
   
    <br />
    <br />
    <h2>
    Search Results
  </h2>
  <div>
  
    <% Html.RenderPartial("InvoiceStatusSearchResultControl", ViewData["ISSearchResultListGrid"]);%>
    <div class="buttonContainer" style = "clear :both">
    <%
        if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.MarkforLateSubmission))
      {%>
            <input class="primaryButton" type="submit"  value="Mark for Late Submission" onclick="javascript:MarkInvoicesForLateSubmission();" />
    <%
      }%>
       <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.IncrBillingPeriod))
      {%>
            <input class="primaryButton" type="submit" id="IncrementInvoice" value="Increment Billing Period" onclick="javascript:IncrementInvoiceBillingPeriod()" />
    <%
     }%>
    
    
    <% using (Html.BeginForm("GenerateInvoiceStatusCsv", "ProcessingDashboard", FormMethod.Post, new { id = "GeneRateCsvFrm" }))
   {%>
     <input type="hidden" id="searchCriteriaCsv" name="searchCriteriaCsv" />
      <input type="hidden" id="searchCriteriaCsvServerSide" name="searchCriteriaCsvServerSide" />
     <input type="hidden" id="billingPeriodCsv" name="billingPeriodCsv" />
     <input type="hidden" id="searchTypeCsv" name="searchTypeCsv" />
            <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.Download))
              {%>
     <input class="secondaryButton" type="button" id="bb" value="Download" onclick="DownloadCsv();"  />     
   
    <%
  }%>
            <%
   }%>
  
<input class="secondaryButton" type="button" id="rs" value="Resubmit" onclick="javascript:ResubmitSelectedInvoices()"  />
   

 <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Reports.ProcessingDashboard.Delete))
      {%>
              <input class="secondaryButton" type="button" id="Delete" value="Delete" onclick="javascript:DeleteSelectedInvoices()" />
     <% }%>
   
  </div>
  </div>  
  <% Html.RenderPartial("~/Areas/Reports/Views/Shared/DashboardLegend.ascx");%>
</div>
<div id="divInvoiceStatusActionResult" class="hidden">
  <% Html.RenderPartial("~/Areas/Reports/Views/Shared/ProcessingDashboardInvoiceActionResultsControl.ascx",ViewData["InvoiceStatusActionResultList"]);%>
 </div>
 <p id="confirmBox" style="width:70px;" class="hidden">
Would you like to include the details related to processing dates/time stamps in 
the download?</p>



