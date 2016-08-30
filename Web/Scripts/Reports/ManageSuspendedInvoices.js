var $ichAchLateSubmissionDialog;
var $ichLateSubmissionDialog;
var $achLateSubmissionDialog;
var resubmitInvoicesUrl = '/ManageSuspendedInvoices/UpdateInvoiceRemark';
var searchUrl = '/ManageSuspendedInvoices/SearchResultGridData';
var resubmissionStatusUrl = '/ManageSuspendedInvoices/GetInvoiceResubmissionStatus';
function setResubmitUrl(postUrl) {
  resubmitInvoicesUrl = postUrl;
}
function setsearchUrl(Url) {
  searchUrl = Url;
}
function setResubmissionStatusUrl(resubmissionUrl) {
  resubmissionStatusUrl = resubmissionUrl
}
function seachSuspendedInvoices(isRefreshSearch) {
  if (isRefreshSearch) {
    $('#clientSuccessMessageContainer').hide();
    $('#clientErrorMessageContainer').hide();
  }
  if ($("#ResubmissionStatusId").val() == 2) {
    $("#cb_ManageSuspendedInvoicesSearchResultGrid").attr('disabled', 'disabled');
    $("#btnUndoBilateral").show();
    $("#btnBilateral").hide();
    $("#btnBilateral").attr("disabled", false);
    $("#btnUndoBilateral").attr("disabled", false);
    $("#btnResubmit").attr("disabled", false);
  }
  else if ($("#ResubmissionStatusId").val() == 1) {
    $("#btnBilateral").attr("disabled", true);
    $("#btnUndoBilateral").attr("disabled", true);
    $("#btnResubmit").attr("disabled", true);
    $("#cb_ManageSuspendedInvoicesSearchResultGrid").removeAttr("disabled");
  }
    else if ($("#ResubmissionStatusId").val() == 3) {
        $("#btnBilateral").attr("disabled", true);
        $("#btnUndoBilateral").attr("disabled", true);
        $("#btnResubmit").attr("disabled", true);
        $("#cb_ManageSuspendedInvoicesSearchResultGrid").removeAttr("disabled");
    }
  else {
    $("#btnBilateral").show();
    $("#btnUndoBilateral").hide();
    $("#btnBilateral").attr("disabled", false);
    $("#btnUndoBilateral").attr("disabled", false);
    $("#btnResubmit").attr("disabled", false);
    $("#cb_ManageSuspendedInvoicesSearchResultGrid").removeAttr("disabled");

  }

  var fromBillingMonth = $("#fromBillingMonth").val();
  var toBillingMonth = $("#toBillingMonth").val();
  var fromBillingPeriod = $("#fromBillingPeriod").val();
  var toBillingPeriod = $("#toBillingPeriod").val();
  var billedEntityCode = $("#BilledMemberId").val();
  var fromBillingYear = $("#fromBillingYear").val();
  var toBillingYear = $("#toBillingYear").val();
  if (billedEntityCode == "")
    billedEntityCode = -1;
  if (fromBillingYear == "")
    fromBillingYear = -1;
  if (toBillingYear == "")
    toBillingYear = -1;
  var settlementMethodIndicator = $("#SettlementMethodId").val();
  var resubmissionStatus = $("#ResubmissionStatusId").val();
  var url = searchUrl + "?" + $.param({ fromBillingMonth: fromBillingMonth, toBillingMonth: toBillingMonth, fromBillingPeriod: fromBillingPeriod, toBillingPeriod: toBillingPeriod, smi: settlementMethodIndicator, resubmissionStatus: resubmissionStatus, billedEntityCode: billedEntityCode, fromBillingYear: fromBillingYear, toBillingYear: toBillingYear });
  $("#ManageSuspendedInvoicesSearchResultGrid").jqGrid('setGridParam', { url: url }).trigger("reloadGrid");

}


function showRemarkDilog(gerRemarkUrl, ids) {
  $("#hiddenInvoiceId").val(ids);

  $.ajax({
    type: "POST",
    url: gerRemarkUrl,
    dataType: "html",
    data: { invoiceId: ids },
    success: function (response) {
      $remarkDialog.dialog('open');
      $("#ResubmissionRemarks").val(response);
      $("#hiddenResubmissionRemark").val(response);
    },
    error: function (xhr, textStatus, errorThrown) {
      alert('An error occurred! ' + errorThrown);
    }
  });
  return false;
}

function SaveRemark(updateUrl) {
  var remark = $("#ResubmissionRemarks").val();
  var ids = $("#hiddenInvoiceId").val();
  $.ajax({
    type: "POST",
    url: updateUrl,
    dataType: "html",
    data: { invoiceId: ids, remark: remark },
    success: function (response) {
      alert("Remark saved successfully.");
      $remarkDialog.dialog('close');

    },
    error: function (xhr, textStatus, errorThrown) {
      alert('An error occurred! ' + errorThrown);
    }
  });
}

function MarkInvoiceAsResubmitted(chkLateSubmissionAlloedUrl) {
  var selectedInvoiceIds = $("#ManageSuspendedInvoicesSearchResultGrid").jqGrid('getGridParam', 'selarrrow');
  $.ajax({
    type: "POST",
    url: chkLateSubmissionAlloedUrl,
    data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
    success: function (response) {
      var lateSubmissionArray = response.split(",");

      if (lateSubmissionArray[0] == "True" && lateSubmissionArray[1] == "True") {
        $("#ichAchLateSubmissionMsg").html("Late submission window is open for both ICH and ACH. Please select the resubmission period.");
        showIchAchResubmissionPeriodDialog();
      }
      if (lateSubmissionArray[0] == "True" && lateSubmissionArray[1] == "False") {
        $("#ichLateSubmissionMsg").html("Latesubmission window is open for ICH. Please select the resubmission period.");
        showIchResubmissionPeriodDialog();
      }
      if (lateSubmissionArray[0] == "False" && lateSubmissionArray[1] == "True") {
        $("#achLateSubmissionMsg").html("Latesubmission window is open for ACH. Please select the resubmission period.");
        showAchResubmissionPeriodDialog();
      }
      if (lateSubmissionArray[0] == "False" && lateSubmissionArray[1] == "False") {
        resubmitInvoices(resubmitInvoicesUrl, false, false);
      }
    }
  });

}

function MarkInvoiceAsBilaterallySettled(postUrl) {
  var selectedInvoiceIds = $("#ManageSuspendedInvoicesSearchResultGrid").jqGrid('getGridParam', 'selarrrow');
  if (selectedInvoiceIds.toString() == "" || selectedInvoiceIds.length == 0)
    alert("Please select at least one invoice for bilateral settlement.");
  else {

    if (confirm("Do you want to mark this invoice as bilaterally settled?")) {
      $.ajax({
        type: "POST",
        url: postUrl,
        data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
        success: function (result) {
          if (result.IsFailed == false) {
            // Toggle message containers.
            $('#clientSuccessMessage').html(result.Message);
            $('#clientSuccessMessageContainer').show();
            $('#clientErrorMessageContainer').hide();
          }
          else {
            $('#clientErrorMessage').html(result.Message);
            $('#clientErrorMessageContainer').show();
            $('#clientSuccessMessageContainer').hide();

          }
          seachSuspendedInvoices(false);
        },
        error: function (xhr, textStatus, errorThrown) {
          alert('An error occurred! ' + errorThrown);
       }
      });
    }
  }

}

function UndoBilateral(postUrl) {
  var selectedInvoiceIds = $("#ManageSuspendedInvoicesSearchResultGrid").jqGrid('getGridParam', 'selarrrow');

  if (selectedInvoiceIds.toString() == "" || selectedInvoiceIds.length==0)
        alert("Please select at least one invoice to undo bilateral settlement.");
    else {
  $.ajax({
    type: "POST",
    url: postUrl,
    data: { selectedInvoiceIds: selectedInvoiceIds.toString() },
    success: function (result) {
      if (result.IsFailed == false) {
        // Toggle message containers.
        $('#clientSuccessMessage').html(result.Message);
        $('#clientSuccessMessageContainer').show();
        $('#clientErrorMessageContainer').hide();
      }
      else {
        $('#clientErrorMessage').html(result.Message);
        $('#clientErrorMessageContainer').show();
        $('#clientSuccessMessageContainer').hide();

      }
      seachSuspendedInvoices(false);

    },
    error: function (xhr, textStatus, errorThrown) {
      alert('An error occurred! ' + errorThrown);
    }
  });
    }


}

function resubmitInvoices(postUrl, isIchLateSubmit, isAchLateSubmit) {

  var selectedInvoiceIds = $("#ManageSuspendedInvoicesSearchResultGrid").jqGrid('getGridParam', 'selarrrow');
  if (selectedInvoiceIds.toString() == "" || selectedInvoiceIds.length==0)
    alert("Please select at least one invoice for resubmission.");
  else {
    $.ajax({
      type: "POST",
      url: postUrl,
      data: { selectedInvoiceIds: selectedInvoiceIds.toString(), isIchLateSubmit: isIchLateSubmit, isAchLateSubmit: isAchLateSubmit },
      success: function (result) {
        if (result.IsFailed == false) {
          // Toggle message containers.
          $('#clientSuccessMessage').html(result.Message);
          $('#clientSuccessMessageContainer').show();
          $('#clientErrorMessageContainer').hide();
        }
        else {
          $('#clientErrorMessage').html(result.Message);
          $('#clientErrorMessageContainer').show();
          $('#clientSuccessMessageContainer').hide();
        }
      },
      error: function (xhr, textStatus, errorThrown) {
        alert('An error occurred! ' + errorThrown);
      }
    });
  }
}

function showIchAchResubmissionPeriodDialog(postUrl) {
  $ichAchLateSubmissionDialog.dialog('open');
  return false;
}

function showIchResubmissionPeriodDialog(postUrl) {
  $ichLateSubmissionDialog.dialog('open');
  return false;
}

function showAchResubmissionPeriodDialog(postUrl) {
  $achLateSubmissionDialog.dialog('open');
  return false;
}

function saveIchAchResubmissionPEriod(postUrl) {
  var resubmissionPeriod = $("input[name='rbIchAchResubmissionPeriod']:checked").val();
  if (resubmissionPeriod == "Previous")
    resubmitInvoices(postUrl, true, true);
  if (resubmissionPeriod == "Current")
    resubmitInvoices(postUrl, false, false);
}
function saveAchResubmissionPEriod(postUrl) {
  var resubmissionPeriod = $("input[name='rbAchResubmissionPeriod']:checked").val();
  if (resubmissionPeriod == "Previous")
    resubmitInvoices(postUrl, false, true);
  if (resubmissionPeriod == "Current")
    resubmitInvoices(postUrl, false, false);
}
function saveIchResubmissionPEriod(postUrl) {
  var resubmissionPeriod = $("input[name='rbIchResubmissionPeriod']:checked").val();
  if (resubmissionPeriod == "Previous")
    resubmitInvoices(postUrl, true, false);
  if (resubmissionPeriod == "Current")
    resubmitInvoices(postUrl, false, false);
}
$ichAchLateSubmissionDialog = $('<div></div>')
.html($("#ichAchresubmissionPeriod"))
.dialog({
  autoOpen: false,
  title: 'Resubmission Period',
  height: 220,
  width: 300,
  modal: true,
  buttons: {

    Save: function () {
      saveIchAchResubmissionPEriod(resubmitInvoicesUrl);
      $(this).dialog('close');
      seachSuspendedInvoices(false);
    },
    Cancel: function () {
      $(this).dialog('close');
      seachSuspendedInvoices(false);
    }
  },
  resizable: false
});
$ichLateSubmissionDialog = $('<div></div>')
.html($("#ichresubmissionPeriod"))
.dialog({
  autoOpen: false,
  title: 'Resubmission Priod',
  height: 220,
  width: 300,
  modal: true,
  buttons: {

    Save: function () {
      saveIchResubmissionPEriod(resubmitInvoicesUrl);
      $(this).dialog('close');
      seachSuspendedInvoices(false);
    },
    Cancel: function () {
      $(this).dialog('close');
      seachSuspendedInvoices(false);
    }
  },
  resizable: false
});
$achLateSubmissionDialog = $('<div></div>')
.html($("#achresubmissionPeriod"))
.dialog({
  autoOpen: false,
  title: 'Resubmission Priod',
  height: 220,
  width: 300,
  modal: true,
  buttons: {
    Save: function () {
      saveAchResubmissionPEriod(resubmitInvoicesUrl);
      $(this).dialog('close');
      seachSuspendedInvoices(false);
    },
    Cancel: function () {
      $(this).dialog('close');
      seachSuspendedInvoices(false);
    }
  },
  resizable: false
});

function GenerateSuspendedInvoiceReport(formId) {
  var Isvalid = $("#viewSuspendedInvoces").validate({
    rules: {

      FromBillingYear: {
        required: true,
        min: 1
      },
      FromBillingMonth: {
        required: true,
        min: 1
      },
      FromBillingPeriod: {
        required: true,
        min: 1

      },
      ToBillingYear: {
        required: function (element) {
          var fy = $("#FromBillingYear").val();
          var ty = $("#ToBillingYear").val();
          if (fy != "" && ty < fy) {
            $("#ToBillingYear").val("");
            return true;
          }
          else {
            return false;
          }
        },
        min: 1
      }
            ,
      ToBillingMonth: {
        required: function (element) {
            var fy = parseInt($("#FromBillingYear").val());
            var ty = parseInt($("#ToBillingYear").val());
            var fm = parseInt($("#FromBillingMonth").val());
            var tm = parseInt($("#ToBillingMonth").val());
          if (ty == fy && tm < fm) {
            $("#ToBillingMonth").val("-1");
            return true;
          }
          else {
            return false;
          }
        },
        min: 1
      },
      ToBillingPeriod: {
        required: function (element) {
          var fy = $("#FromBillingYear").val();
          var ty = $("#ToBillingYear").val();
          var fm = $("#FromBillingMonth").val();
          var tm = $("#ToBillingMonth").val();
          var fp = $("#FromBillingPeriod").val();
          var tp = $("#ToBillingPeriod").val();
          if (ty == fy && tm == fm && tp < fp) {
            $("#ToBillingPeriod").val("-1");
            return true;
          }
          else {
            return false;
          }
        },
        min: 1
      }
    },
    messages: {
            FromBillingYear: " From Clearance Year Required.",
            FromBillingMonth: " From Clearance Month Required.",
            FromBillingPeriod: " From Clearance Period Required.",
            ToBillingYear: " To Clearance Year should be grater than or equal to From Clearance Year.",
            ToBillingMonth: " To Clearance Month should be grater than or equal to From Clearance Month.",
            ToBillingPeriod: " To Clearance Period should be grater than or equal to From Clearance Period."
    },
    submitHandler: function (form) {
      $('#errorContainer').hide();
      redirectToReport(form);
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    }
  });

}

function redirectToReport(formId) {

  var isDate;
  var fOriginalBillingYear;
  var tOriginalBillingYear;
  var fOriginalBillingMonth;
  var tOriginalBillingMonth;
  var fOriginalPeriod;
  var tOriginalPeriod;
  var SettlementMethodId;
  var suspensionEntityCode;
  var billingCategoryId;
  var iataMemberId;
  var achMemberId;
  //Changes to display search criteria on report
  var billingMonthFrom;
  var billingMonthTo;
  var clearingHouse;
  var suspendedMemberName;
  var billingCategory;
  var SearchCriteria;

  fOriginalBillingYear = $('#FromBillingYear').val();
  tOriginalBillingYear = $('#ToBillingYear').val();
  fOriginalBillingMonth = $('#FromBillingMonth').val();
  tOriginalBillingMonth = $('#ToBillingMonth').val();
  fOriginalPeriod = $('#FromBillingPeriod').val();
  tOriginalPeriod = $('#ToBillingPeriod').val();
  SettlementMethodId = $('#SettlementMethodStatusId').val();
  suspensionEntityCode = $('#BilledEntityCode').val();
  billingCategoryId = $('#BillingCategoryId').val();
  iataMemberId = $('#IataMemberId').val();
  achMemberId = $('#AchMemberId').val();

  //start--Changes to display search criteria on report
  billingMonthFrom = $('#FromBillingMonth :selected').text().toString();
  billingMonthTo = $('#ToBillingMonth :selected').text().toString();
  clearingHouse = $('#SettlementMethodStatusId :selected').text().toString();
  suspendedMemberName = (suspensionEntityCode == '') ? 'All' : $('#BilledEntityName').val().toString(); //''= Billing Member Code=Blank
  billingCategory = $('#BillingCategoryId :selected').text().toString();

  SearchCriteria = 'Billing Year From:' + fOriginalBillingYear.toString() + ', Billing Month From:' + billingMonthFrom + ', Billing Period From:' + fOriginalPeriod.toString() +
     ', Billing Year To:' + tOriginalBillingYear.toString() + ', Billing Month To:' + billingMonthTo + ', Billing Period To:' + tOriginalPeriod.toString() +
     ', Clearing House:' + clearingHouse + ', Suspended Member Code:' + suspendedMemberName + ', Billing Category:' + billingCategory;
  var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
  var regAnd = RegExp("\\&", "g");
  //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
  SearchCriteria = SearchCriteria.replace(regAnd, "and");

  //getDateTimeForReports() function is defined in site.jss
  window.open(rootpath + "/SuspendedInvoiceReport.aspx?FromBillingYear=" + fOriginalBillingYear + "&ToBillingYear=" + tOriginalBillingYear + "&FromBillingMonth=" + fOriginalBillingMonth + "&ToBillingMonth=" + tOriginalBillingMonth + "&FromBillingPeriod=" + fOriginalPeriod + "&ToBillingPeriod=" + tOriginalPeriod + "&SettlementMethodId=" + SettlementMethodId + "&SuspendedEntityCode=" + suspensionEntityCode + "&BillingCategoryId=" + billingCategoryId + "&IATAMemberId=" + iataMemberId + "&ACHMemberId=" + achMemberId + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}
$(document).ready(function () {
});

var resubmissionStarusId;

function RowSelectEvent(ids) {
  $.ajax({
    type: "POST",
    url: resubmissionStatusUrl,
    dataType: "html",
    data: { invoiceId: ids },
    success: function (result) {
      if (result == 2) {
        $("#btnUndoBilateral").show();
        $("#btnBilateral").hide();
      }
      else {
        $("#btnUndoBilateral").hide();
        $("#btnBilateral").show();
      }
    },
    error: function (xhr, textStatus, errorThrown) {
      alert('An error occurred! ' + errorThrown);
    }
  });
}

