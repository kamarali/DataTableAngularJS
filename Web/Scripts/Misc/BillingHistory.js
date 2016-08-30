var corrUrl, invUrl, auditTrailUrl, billingMemoUrl;
function InitialiseBillingHistory(corrURL, invoiceURL, auditTrailURL, billingMemoURL, isTransactionOutsideTimeLimitMethod, isCorrespondenceOutsideTimeLimitMethod, isCorrespondenceInvoiceExistsForCorrespondence) {
    corrUrl = corrURL;
    invUrl = invoiceURL;
    _isTransactionOutsideTimeLimitMethod = isTransactionOutsideTimeLimitMethod;
    _isCorrespondenceOutsideTimeLimitMethod = isCorrespondenceOutsideTimeLimitMethod;
    _isCorrespondenceInvoiceExistsForCorrespondence = isCorrespondenceInvoiceExistsForCorrespondence;
    SetTransaction();
    $("#CorrespondenceStatusId").bind("change", SetSubStatus);
    $("#BillingTypeId").bind("change", function () {
        SetTransaction();
    });

    $("#corrSearchCriteria").validate({
        rules: {
            FromDate: "required",
            ToDate: "required",
            CorrespondenceStatusId: "required"
        },
        messages: {
            FromDate: "From Date Required",
            ToDate: "To Date Required",
            CorrespondenceStatusId: "Correspondence Status Required"
        }
    });

    //  SCP 000000  : Handled Elmah Issue "The data reader is incompatible with the specified 'ISDataContext.MiscBillingHistorySearchResult'"
    //  Description : In cases when user do not select member code from auto complete dropdown list, 0 value was getting passed as billed member id in SP. 
    //  Because of SP generates ORA-01403: no data found exception. 
    //  Handled the same by additng JS validation check on billed member Id field instead of billed member code field.
    $("#invoiceSearchCriteria").validate({
        rules: {
            BillingYearMonth: "required",
            BillingPeriod: "required",
            //BilledMemberCode: "required",
            BilledMemberId: {
                required: true,
                min: 1,
                number: true
            },
            BillingTypeId: "required",
            AssociatedLocation: "required"
        },
        messages: {
            BillingYearMonth: "Billing Year / Month Required",
            BillingPeriod: "Billing Period Required",
            //BilledMemberCode: "Member Code Required",
            BilledMemberId: "Please select Member Code",
            BillingTypeId: "Billing Type Required",
            AssociatedLocation: "Billed from/to Location ID required"
        }
    });

    //CMP #655: IS-WEB Display per Location ID          
    $("#AssociatedLocation option").each(function (i, selected) {
        var selectedLocation = $("#MemberLocation").val();
        var selectedLocationArray = selectedLocation.split(',');
        if (selectedLocation == '') {
            $(selected).attr('selected', 'selected');
        } else {
            var found = $.inArray(selected.text, selectedLocationArray) > -1;
            if (found) {
                $(selected).attr('selected', 'selected');
            }
        }
    });

    var firstOption = $("#AssociatedLocation option:selected:first").attr('title');
    $("#AssociatedLocation option:selected:first").filter(function () {
        $(this).removeAttr('selected');
        return $(this).text() == firstOption;
    }).attr('selected', true);

    var title = "At least one Location ID must be selected for successful search results. If no Location IDs are shown here, it means that you are not associated with any Location of your organization. Please contact your organization’s user(s) who have access to the Location Association module to review and associate you with appropriate Location(s).";
    $("#AssociatedLocation").attr("title", title);
    $("#AssociatedLocation option").attr("title", title);

}

$("#Search").bind('click', function () {
    BindSelectedLocation();
});
 
function BindSelectedLocation() {
    var selectedLocationIds = '';
    $("#AssociatedLocation option:selected").each(function () {
        selectedLocationIds = selectedLocationIds + ',' + $(this).text();
    });
    $("#MemberLocation").val(selectedLocationIds);
}



// Following function is used to check whether From date is less than To date.
$("#FromDate").change(function () {
  var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
  if (!dateComparisonResult) {
    alert("From date must be lesser than to date");
    $("#FromDate").val('');
  }
});

// Following function is used to check whether To date is greater than From date.
$("#ToDate").change(function () {
  var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
  if (!dateComparisonResult) {
    alert("To date must be greater than from date");
    $("#ToDate").val('');
  }
});

function SetTransaction()
{  
if ($("#BillingTypeId").val() == "" || $("#BillingTypeId").val() == "2") {
      $("#TransactionStatusId").val('');
      $("#TransactionStatusId").attr('disabled', 'disabled');
      $("#AcceptTransactionButton").attr('disabled', 'disabled');
    }
    else
      $("#TransactionStatusId").removeAttr('disabled');
    $("#AcceptTransactionButton").removeAttr('disabled');
  }

  function resetForm(formId) {
    $(':input', formId)
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
    $("#BillingPeriod").val("-1");
    $("#BilledMemberId").val("0");
  }

  function ShowDetails(correspondenceUrl, invoiceUrl) {
      selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');
      

    var gridRow = $("#BHSearchResultsGrid").getRowData(selectedInvoiceId);
    if (gridRow.DisplayCorrespondenceStatus != '')
      location.href = corrUrl + '/' + selectedInvoiceId;
    else
      location.href = invUrl + '/' + selectedInvoiceId;
  }

  function ShowCorrespondence() {

      selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');

      if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId!=null) {
      location.href = corrUrl + '/' + selectedInvoiceId;
    }
    else {

    }
  }

//  function ShowAuditTrail() {
//    var bhGrid = jQuery('#BHSearchResultsGrid');
//    selectedInvoiceId = bhGrid.getGridParam('selrow');
//    var rowData = bhGrid.getRowData(selectedInvoiceId);
//    if (selectedInvoiceId && selectedInvoiceId != 0) {
//      //location.href = auditTrailUrl + '/' + selectedInvoiceId;
//      location.href = auditTrailUrl + '/' + rowData[14];
//    }
//  }

  function CreateBillingMemo() {
      selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');
      

    var gridRow = $("#BHSearchResultsGrid").getRowData(selectedInvoiceId);

    var correspondenceNumber = gridRow.TransactionNumber;

    if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId!=null) {
      location.href = billingMemoUrl + '/' + selectedInvoiceId + '/' + correspondenceNumber;
    }
    else {

    }
}

   function GetSelectedRecordId(ids) {  
        if(isinvoiceSearch)
       {                
        $("#CreateBillingMemo").attr('disabled', 'disabled');

        selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');

        var gridRow = $("#BHSearchResultsGrid").getRowData(selectedInvoiceId);

        if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId!=null && $('#BillingTypeId').val() == '1' && ((gridRow.ClearingHouse == 'I' && gridRow.RejectionStage == "1") || (gridRow.ClearingHouse == 'A' && gridRow.RejectionStage == "2"))) {
          $("#CorrespondenceButton").removeAttr('disabled');      
        }
        else {
          $("#CorrespondenceButton").attr('disabled', 'disabled');
        }

        if($('#BillingTypeId').val() == '2')
        {
          $("#AcceptTransactionButton").attr('disabled', 'disabled');
        }
       }
       else {
           selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');           

        var gridRow = $("#BHSearchResultsGrid").getRowData(selectedInvoiceId);

        if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId != null && gridRow.DisplayCorrespondenceStatus != "Closed" && (gridRow.AuthorityToBill == "Yes" || gridRow.DisplayCorrespondenceStatus == "Expired") && gridRow.BillingMemberId != loggedInMemberId) {
          $("#CreateBillingMemo").removeAttr('disabled');      
        }
         else {
          $("#CreateBillingMemo").attr('disabled', 'disabled');
        }
          $("#AcceptTransactionButton").attr('disabled', 'disabled');          
      
        }      
  }

  //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
  //Concate Curreny with amount for total net amount column
  function ConcateCurreny(cellValue, options, rowObject) {
      //SCP# 429409 - Invoice amounts in MISC Billing History screen displayed incorrectly
      try {
          /* Attempting to format cell value, e.g. -> 1234.056 will be formated as 1,234.056 */
          var splitedCellVal = cellValue.split('.');
          NonDecimalPart = splitedCellVal[0];
          DecimalPart = splitedCellVal[1];
          var rgx = /(\d+)(\d{3})/;
          while (rgx.test(NonDecimalPart)) {
              NonDecimalPart = NonDecimalPart.replace(rgx, '$1' + ',' + '$2');
          }
          return rowObject.NetAmtCurrency + " " + NonDecimalPart + '.' + DecimalPart;
      }
      catch (e) {
          /* Failover code - Do not format - Keep working as is like day one. */
          /* Problem while formating the numeric value, so returning cell value as is (like day one) */
          return rowObject.NetAmtCurrency + " " + cellValue;
      }      
  }

  function SetAuthorityToBill(cellValue, options, rowObject) {
    if (isinvoiceSearch)
      return "";

    if (cellValue.toString().toLowerCase() === 'true') return "Yes";
    return "NA";
  }

  // If Invoice is being searched, set No of days to expire as null.
  function SetNoOfDaysToExpireAsBlank(cellValue, options, rowObject) {
    if (isinvoiceSearch) {
      return "";
    }
    else {
      return cellValue;
    }
  }  

  SetSubStatus();
  function SetSubStatus() {
   //CMP527: Add new sub status  "Accepted By Correspondence Initiator"
    var selectedId = $("#CorrespondenceSubStatusId").val();
    $("#CorrespondenceSubStatusId").empty();
    var status = $("#CorrespondenceStatusId").val();
    if (status == "1") {
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("1").html("Received"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("3").html("Saved"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("2").html("Responded"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("4").html("Ready For Submit"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("7").html("Pending"));
    }
    else if (status == "2") {
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("Please Select"));
    }
    else if (status == "3") {
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("5").html("Billed"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("6").html("Due To Expiry"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("8").html("Accepted By Correspondence Initiator"));
    }
    else if (status == "-1") {
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("All"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("1").html("Received"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("3").html("Saved"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("2").html("Responded"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("4").html("Ready For Submit"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("5").html("Billed"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("6").html("Due To Expiry"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("7").html("Pending"));
      $("#CorrespondenceSubStatusId").append($("<option></option>").val("8").html("Accepted By Correspondence Initiator"));
    }
      
    //TFS#9984 : Forefox:v45 - By selecting value from "Correspondence Status" related value is not updated in "Correspondence Sub Status".
    //Note:Again Changes are done as per TFS#9992.
    if ($("#CorrespondenceSubStatusId > option[value='" + selectedId + "']").length == 0) {
          $("#CorrespondenceSubStatusId").val($("#CorrespondenceSubStatusId option:first").val());
          return;
      }
    else {
          $("#CorrespondenceSubStatusId").val(selectedId);
      }
  
  }

  var _isTransactionOutsideTimeLimitMethod;
  function IsTransactionOutsideTimeLimit(invoiceId, correspondenceStatusId, authorityToBill, correspondenceDate, correspondenceRefNumber) {
      $.ajax({
          type: "POST",
          url: _isTransactionOutsideTimeLimitMethod,
          data: { invoiceId: invoiceId, correspondenceStatusId: correspondenceStatusId, authorityToBill: authorityToBill, correspondenceDate: correspondenceDate, correspondenceRefNumber: correspondenceRefNumber },
          dataType: "json",
          success: function (response) {
              if (response.IsFailed == false) {
                  //check correspondence invoice is exist or not for correspondence reference.
                  IsCorrespondenceInvoiceExists(correspondenceRefNumber, response.RedirectUrl);
                  return;
              }
              alert(response.Message);
              return;
          }
      });
 }


 //SCP186155 - Same BM ,RAm5.2.2.5(check correspondence invoice is exist or not).
 var _isCorrespondenceInvoiceExistsForCorrespondence;
    function IsCorrespondenceInvoiceExists(correspondenceRefNumber, responseUrl) {
        $.ajax({
            type: "POST",
            url: _isCorrespondenceInvoiceExistsForCorrespondence,
            data: { correspondenceRefNumber: correspondenceRefNumber },
            dataType: "json",
            success: function (response) {
                if (response.IsFailed == true) {
                    alert(response.Message);
                   return;
                }
               location.href = responseUrl;
                return;
            }
        });
}

  var _isCorrespondenceOutsideTimeLimitMethod;
  function IsCorrespondenceOutsideTimeLimit(invoiceId) {
    $.ajax({
      type: "POST",
      url: _isCorrespondenceOutsideTimeLimitMethod,
      data: { invoiceId: invoiceId},
      dataType: "json",
      success: function (response) {
        if (response.IsFailed == false) {
          location.href = response.RedirectUrl;
          return;
        }
        alert(response.Message);
        return;
      }
    });
}

//CMP508:Audit Trail Download with Supporting Documents
function DownloadFile(instantDownloadUrl, enqueueUrl) {
    var includeSuppDocs = $('#IncludeSuppDocs').prop('checked');
    if (!includeSuppDocs) {
        location.href = instantDownloadUrl;
    }
      else {
        //SCP310398 - SRM:Exception occurred in Report Download Service. - SIS Production - 10Nov
        checkUserSessionsForAjaxRequest();

        $.ajax({
            type: "POST",
            url: enqueueUrl,
            dataType: "json",
            success: function (response) {
              // SCP227747: Cargo Invoice Data Download
              if (response.IsFailed) {
                showClientErrorMessage(response.Message);
              }
              else {
                showClientSuccessMessage(response.Message);
              }
            }
        });
    }
}