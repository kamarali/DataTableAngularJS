//variable is used to check which button has been clicked(Generate Report/ Export to CSV)
var _checkExportbutton = false;

function redirectToReport(formId, redirectUrl) {
    var cleMonth;
    var perNo;
    var billiAirNumber;
    var billdAirNumber;
    var invNumber;
    var issAirNumber;
    var origPMI;
    var validPMI;
    var agreIndSupplied;
    var agreIndValidated;
    var atpcoReasCode;
    var memId;

    cleMonth = $('#ClearanceMonth').val();
    perNo = $('#BillingPeriod').val();
    billiAirNumber = $('#BillingAirlineNumber').val();
    billdAirNumber = $('#BilledAirlineNumber').val();
    invNumber = $('#invoiceNumber').val();
    issAirNumber = $('#IssuingAirline').val();
    origPMI = $('#originalPMI').val();
    validPMI = $('#ValidatedPMI').val();
    agreIndSupplied = $('#agreIndSupplied').val();
    agreIndValidated = $('#agreIndValidated').val();
    atpcoReasCode = $('#atpcoReasonCode').val();
    memId = $('#MmbrId').val();
    bYear = $('#billingYear').val();

    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    var SearchCriteria = 'Clearance Month:' + $('#ClearanceMonth :selected').text() + ', Period No:' + perNo + ', Billing Year:' + bYear + ', Billing Airline:' + (billiAirNumber == '' ? 'All' : $('#BillingAirlineNumberCode').val()) + ', Billed Airline:' + (billdAirNumber == '' ? 'All' : $('#BilledAirlineNumberCode').val()) + ', Invoice Number:' + invNumber + ' , Issuing Airline:' + (issAirNumber == '' ? 'All' : $('#IssuingAirline').val()) + ', Original PMI:' + origPMI + ' , Validated PMI:' + validPMI + ' , Agreement Indicator - Supplied:' + agreIndSupplied + ' , Agreement Indicator - Validated:' + agreIndValidated + ' , ATPCO Reason Code:' + atpcoReasCode;
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

//    if (_checkExportbutton) {
       // Download Report     
        window.open(redirectUrl + "?clearanceMonth=" + cleMonth + "&periodNo=" + perNo + "&biligAirlineNo=" + billiAirNumber + "&biledAirlineNo=" + billdAirNumber + "&invoiceNo=" + invNumber + "&issuAirline=" + issAirNumber + "&originalPMI=" + origPMI + "&validatedPMI=" + validPMI + "&agrmntIndiSupplied=" + agreIndSupplied + "&agrmntIndiValidated=" + agreIndValidated + "&atpcoReasCd=" + atpcoReasCode + "&billingYear=" + bYear + "&memberId=" + memId);
//      }
//    else {
//        //getDateTimeForReports() function is defined in site.jss    
//        window.open(rootpath + "/ConfirmationDetailReport.aspx?cleMonth=" + cleMonth + "&perNo=" + perNo + "&billiAirNumber=" + billiAirNumber + "&billdAirNumber=" + billdAirNumber + "&invNumber=" + invNumber + "&issAirNumber=" + issAirNumber + "&origPMI=" + origPMI + "&validPMI=" + validPMI + "&agreIndSupplied=" + agreIndSupplied + "&agreIndValidated=" + agreIndValidated + "&atpcoReasCode=" + atpcoReasCode + "&bYear=" + bYear + "&memId=" + memId + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports());
//      }

}

function Pax_ConfirmationReport(formId, id, redirectUrl, validateBillYearMonthPeriodUrl) {

//    if (id == "Button2") { _checkExportbutton = true; }
//    else if (id == "Button1") { _checkExportbutton = false; };

    var Isvalid = $("#ConfirmationDetailReport").validate({
        rules: {

            ClearanceMonth: {
                required: true
                //min: 1
            },
            BillingPeriod: {
                required: true
            },
            billingYear: {
                required: true
            },
            BillingAirlineNumberCode: {
                required: function (element) {
                    var blig = $("#BillingAirlineNumber").val();
                    var bild = $("#BilledAirlineNumber").val();
                    var mId = $("#MmbrId").val();

                    if (blig == mId && bild == mId) {
                        $("#BillingAirlineNumberCode").val(" ");
                        $("#BilledAirlineNumberCode").val(" ");
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                // min: 1
            },

            BilledAirlineNumberCode: {
                required: function (element) {
                    var blig = $("#BillingAirlineNumber").val();
                    var bild = $("#BilledAirlineNumber").val();
                    var mId = $("#MmbrId").val();

                    if (blig != "" && bild != "") {
                        if ((blig == bild && blig != mId && bild != mId) || (blig != bild && blig != mId && bild != mId)) {
                            $("#BillingAirlineNumberCode").val(" ");
                            $("#BilledAirlineNumberCode").val(" ");
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
                //min: 1
            }
        },

        messages: {
            ClearanceMonth: " Clearance month required.",
            BillingAirlineNumberCode: "Billing and Billied Member can not be same.",
            BilledAirlineNumberCode: " Billing and Billed Member both can not be other than Logged in Member"
        },
        submitHandler: function (form) {
          $('#errorContainer').hide();
          $.ajax({
            url: validateBillYearMonthPeriodUrl,
            data: { month: $('#ClearanceMonth :selected').val(), year: $('#billingYear').val(), period: $('#BillingPeriod').val() },
            success: function (errMsg) {
              if (errMsg != "Valid Parameters") {
                alert(errMsg);
              }
              else {
                redirectToReport(form, redirectUrl);
              }
            },
            dataType: "text"
          });
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

}

