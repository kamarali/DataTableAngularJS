

function validateCorrespondence(formid, category) {
    redirectToCorrespondenceReport(formid, category);
}


function redirectToCorrespondenceReport(formid, category) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    //in \Views\CorrespondenceStatus\CorrespondenceSearch.ascx 'charge category' is made invisible if category is not Misc, so following change
    var chargeCategoryLabel = $('#ChargeCategory').is(":visible") ? ', Charge Category:' + $('#ChargeCategory :selected').text() : '';
    //CMP526 - Passenger Correspondence Identifiable by Source Code
    var sourceCodeLabel = $('#SourceCode').is(":visible") ? ' , SourceCode:' + $('#SourceCode').val() : '';
    var sourceCode = $('#SourceCode').val();
    if (typeof sourceCode === 'undefined')
        sourceCode = '';
    var SearchCriteria = 'Correspondence Ref. No.:' + $('#CorrespondenceNumber').val() + ' , From Date:' + $('#Fromdate').val() + ', To Date:' + $('#ToDate').val() + ', Corr. Initiating Member:' + $('#InitiatingMember :selected').text() + ', Member Code:' + ($('#MemberId').val() == '' ? 'All' : $('#MemberCode').val()) + chargeCategoryLabel + ', Correspondence Status:' + $('#CorrespondenceStatusId :selected').text() + ', Correspondence Sub Status:' + $('#CorrespondenceSubStatusId :selected').text() + ', Correspondence Stage > =:' + $('#Corrstage').val() + ' , Expiring In (no of days):' + $('#Expiryindays').val() + ' , Show only Authority to bill cases:' + ($('#IsAuthorityToBillCase').prop('checked') == true ? 'Yes' : 'No' + sourceCodeLabel);
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    //getDateTimeForReports() function is defined in site.jss
    //CMP526 - Passenger Correspondence Identifiable by Source Code - pass Source Code as quesry string param    
    window.open(rootpath + "/CorrespondenceReport.aspx?fdate=" + $('#Fromdate').val() + "&tdate=" + $('#ToDate').val() + "&Refno=" + $('#CorrespondenceNumber').val() + "&InitMem=" + $('#InitiatingMember').val() + "&fmem=" + $('#MemberId').val() + "&Autho=" + $('#IsAuthorityToBillCase').prop('checked') + "&corrstatus=" + $('#CorrespondenceStatusId').val() + "&corrsub=" + $('#CorrespondenceSubStatusId').val() + "&corrstage=" + $('#Corrstage').val() + "&Expiry=" + $('#Expiryindays').val() + "&charge=" + $('#ChargeCategory').val() + "&category=" + category + "&SourceCode=" + sourceCode + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}
SetSubStatus();
function SetSubStatus() {
   
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

