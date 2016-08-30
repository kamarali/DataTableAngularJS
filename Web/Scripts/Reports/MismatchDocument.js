
function redirectToMismatchDocReport(formId, category) {
    //Changes to display search criteria on report
    var SearchCriteria = 'Billing Month:' + $('#BillingMonth :selected').text() + ', Period No:' + $('#BillingPeriod').val() + ', Billing Year:' + $('#BillingYear').val() + ', Settlement Method:' + $('#SettlementMethodStatusId :selected').text() + ', Invoice Number:' + $('#InvoiceNo').val() + ' , Member Code:' + ($('#MemberId').val() == '' ? 'All' : $('#MemberCode').val().toString());
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports")); 
    
    if (category == 1) {
        window.open(rootpath + "/SupportingMismatchDoc.aspx?bmonth=" + $('#BillingMonth').val() + "&bPeriod=" + $('#BillingPeriod').val() + "&bYear=" + $('#BillingYear').val() + "&SetelmentMethodId=" + $('#SettlementMethodStatusId').val() + "&InNo=" + $('#InvoiceNo').val() + "&AirCode=" + $('#MemberId').val() + "&category=" + category + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
    }
    else if (category == undefined || category == 2) {
        //getDateTimeForReports() function is defined in site.js
        window.open(rootpath + "/CargoSupportingMismatchDoc.aspx?bmonth=" + $('#BillingMonth').val() + "&bPeriod=" + $('#BillingPeriod').val() + "&bYear=" + $('#BillingYear').val() + "&SetelmentMethodId=" + $('#SettlementMethodStatusId').val() + "&InNo=" + $('#InvoiceNo').val() + "&AirCode=" + $('#MemberId').val() + "&category=" + 2 + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
    }
    //CMP# 519 Miscellaneous Supp Doc Mismatch Report
    else if (category == undefined || category == 3) {
        //getDateTimeForReports() function is defined in site.js
        window.open(rootpath + "/MiscSupportingMismatchDoc.aspx?bmonth=" + $('#BillingMonth').val() + "&bPeriod=" + $('#BillingPeriod').val() + "&bYear=" + $('#BillingYear').val() + "&SetelmentMethodId=" + $('#SettlementMethodStatusId').val() + "&InNo=" + $('#InvoiceNo').val() + "&AirCode=" + $('#MemberId').val() + "&category=" + 2 + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
    }
}

function ValidateMismatchDocument(formId, category, validateBillYearMonthPeriodUrl) {
  
 
    $("#MismatchDoc").validate({
        rules: {
            BillingMonth: "required",
            BillingYear :  "required"
            },
        messages: {
            BillingMonth: "Billing Month Required",
            BillingYear  : "Billing Year Required"
        },
        submitHandler: function (form) {
            $('#errorContainer').hide();
            
      $.ajax({
        url: validateBillYearMonthPeriodUrl,
        data: { month: $('#BillingMonth :selected').val(), year: $('#BillingYear').val(), period: $('#BillingPeriod').val() },
        success: function (errMsg) {
          if (errMsg != "Valid Parameters") {
            alert(errMsg);
          }
          else {
            redirectToMismatchDocReport(formId,category);
          }


        },
        dataType: "text"
      });

        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }

    });
    }
    

