var corrUrl, invUrl, auditTrailUrl, initiateCorrUrl, memoType, clearSearchUrl, billingCode, initiateDuplicateRejectionUrl, checkIfBMExists;
var $validateCorrTrail;

function InitialiseBillingHistory() {

  $("#CorrespondenceStatusId").bind("change", SetSubStatus);


  $validateCorrTrail = $("#corrSearchCriteria").validate({
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

  SetControlValues(true);

  $("#FromDate").change(function () {
    var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
    if (!dateComparisonResult) {
      alert("From date must be lesser than to date");
      $("#FromDate").val('');
    }
  });

  $("#ToDate").change(function () {
    var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
    if (!dateComparisonResult) {
      alert("To date must be greater than from date");
      $("#ToDate").val('');
    }
  });

}

function SetTransControl() {

}

function SetControlValues(isFromPageLoad) {

  SetTransControl();
}


function resetForm(formId) {

  ResetSearch(formId);
}


function ResetSearch(formId) {

  $.ajax({
    type: "POST",
    url: clearSearchUrl,
    data: { entity: formId },
    dataType: "json",
    success: function (response) {

      if (response) {
        return;
      }
    },
    failure: function (response) {
      var failed = 'failed';
    }
  });
}

function GetSelectedRecordId(ids) {


}

function SetAuthorityToBill(cellValue, options, rowObject) {
    if (cellValue.toString().toLowerCase() === 'true') return "Yes";
  return "NA";
}

function SetMemberCode(cellValue, options, rowObject) {
  rowObject
  if (cellValue.toString().toLowerCase() === 'true') return "Yes";
  return "NA";
}

SetSubStatus();

function SetSubStatus() {
    //CMP527: Add new sub status  "Accepted By Correspondence Initiator"
  var selectedId = $("#CorrespondenceSubStatusId").val();
  $("#CorrespondenceSubStatusId").empty();
  var status = $("#CorrespondenceStatusId").val();
  if (status == "1") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("All"));
    $("#CorrespondenceSubStatusId").append($("<option selected></option>").val("1").html("Received"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("3").html("Saved"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("2").html("Responded"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("4").html("Ready For Submit"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("7").html("Pending"));
  }
  else if (status == "2") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("Please Select"));
  }
  else if (status == "3") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("All"));
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


SetCorrespondenceOwner();

function SetCorrespondenceOwner() {

}

function validateDateRange(startDateId, endDateId) {
  var startDateVal = $('#' + startDateId).datepicker("getDate");
  var endDateVal = $('#' + endDateId).datepicker("getDate");

  return endDateVal >= startDateVal;
}

function validateMaxDateRange() {

  var d1 = $('#FromDate').datepicker("getDate");
  var d2 = $('#ToDate').datepicker("getDate");
  var dayDiff = Math.ceil((d2 - d1) / (1000 * 60 * 60 * 24));
  //  var months;
  //  months = (d2.getFullYear() - d1.getFullYear()) * 12;
  //  months -= d1.getMonth() + 1;
  //  months += d2.getMonth();
  //  months = months + 2;

  //  var fm = d1.getMonth();
  //  var fy = d1.getFullYear();
  //  var sm = d2.getMonth();
  //  var sy = d2.getFullYear();
  //  var months = Math.abs(((fy - sy) * 12) + fm - sm);
  //  var firstBefore = d1 > d2;
  //  d1.setFullYear(sy);
  //  d1.setMonth(sm);
  //  firstBefore ? d1 < d2 ? months-- : "" : d2 < d1 ? months-- : "";

  if (dayDiff > 90) {
      alert("Maximum allowed date range is 90 Days");
    return false;
  }
  return true;


}

function CorrTrailReportRequest() {
  //SCP310398 - SRM:Exception occurred in Report Download Service. - SIS Production - 10Nov
  checkUserSessionsForAjaxRequest();

  var selectedTransactionIds = jQuery('#CorrespondenceTrailSearchGrid').getGridParam('selarrrow');
 
  var selectedTransactionString = selectedTransactionIds.join();
  $.ajax({
    type: "POST",
    url: RequestForCorrespondenceTrailReportUrl,
    data: { transactionIds: selectedTransactionString },
    dataType: "json",
    success: function (result) {
      if (result.IsFailed == false) {
        showClientSuccessMessage(result.Message);
      }
      else {
        showClientErrorMessage(result.Message);
      }
    }
  });
}


function CorrTrailReportRequestAll() {
  //SCP310398 - SRM:Exception occurred in Report Download Service. - SIS Production - 10Nov
  checkUserSessionsForAjaxRequest();

  var recordcount = jQuery('#CorrespondenceTrailSearchGrid').getGridParam("reccount");
  $.ajax({
    type: "POST",
    url: RequestForCorrespondenceTrailReportAllUrl,
    data: { recordCount: recordcount },
    dataType: "json",
    success: function (result) {
      if (result.IsFailed == false) {
        showClientSuccessMessage(result.Message);
      }
      else {
        showClientErrorMessage(result.Message);
      }

    }
  });
}


function ResetCorrespondence() {
  $validateCorrTrail.resetForm();
  $("#CorrBilledMemberText").val('');
  $("#CorrespondenceStatusId").val('-1');
  $("#CorrespondenceSubStatusId").val('-1');
  //TFS#10003:Firefox: v45: "Clear" button removing value from "Correspondence Initiating Member" for PAX. 
  $("#CorrespondenceOwnerId").val($("#CorrespondenceOwnerId option:first").val());
  $("#InitiatingMember").val($("#InitiatingMember option:first").val());  
  $("#ToDate").datepicker('setDate', new Date());
  $("#FromDate").datepicker('setDate', new Date());
  SetSubStatus();
  jQuery('#CorrespondenceTrailSearchGrid').clearGridData();
  clearMessageContainer();
}


