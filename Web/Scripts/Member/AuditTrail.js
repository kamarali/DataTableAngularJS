

function redirectToReport(formId) {
    var isDate;
    var fdate;
    var tdate;
    if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'ClearancePeriod') {
        fdate = $('#auditTrailFromPeriod').val();
        tdate = $('#auditTrailToPeriod').val();
        isDate = 0;
    }
    else {
        fdate = $('#auditTrailFromDate').val();
        tdate = $('#auditTrailToDate').val();
        isDate = 1;
    }

    var newArray = new Array();
    var elementList = '';
    var elements = $('input[name=group]')

    for (i = 0; i < elements.length; i++) {
        if (elements[i].type == 'checkbox') {
            elementList = elementList + "!" + elements[i].id + "|" + elements[i].checked;
        }
    }

    var user = $('#User').val();
    var rootpath = location.href.substring(0, location.href.indexOf("/Profile"));
    window.open(rootpath + "/AuditTrailReportView.aspx?fdate=" + fdate + "&tdate=" + tdate + "&user=" + user + "&elist=" + elementList + "&isdate=" + isDate + " ", null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}

function redirectToIchAchReport(reportType) {
    var isDate;
    var fdate;
    var tdate;
    if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'ClearancePeriod') {
        fdate = $('#auditTrailFromPeriod').val();
        tdate = $('#auditTrailToPeriod').val();
        isDate = 0;
    }
    else {
        fdate = $('#auditTrailFromDate').val();
        tdate = $('#auditTrailToDate').val();
        isDate = 1;
    }

    var elementList;
    var user = $('#User').val();
    var mId = $('#Id').val();

    var rootpath = location.href.substring(0, location.href.indexOf("/Profile"));
    window.open(rootpath + "/AuditTrailReportView.aspx?fdate=" + fdate + "&tdate=" + tdate + "&user=" + user + "&elist=" + elementList + "&isdate=" + isDate + "&mid=" + mId + "&rtype=" + reportType + "", null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}

function selectAllCheckBox() {
    var button = $("#selectAll");
    if (button.val() == "Select All") {
        $('input[name=group]').prop('checked', true);
        button.attr('value', 'Unselect All');
    }
    else {
        $('input[name=group]').prop('checked', false);
        button.attr('value', 'Select All');
    }
}

//Function to fill user dropdwon based on given member id.
function getMemberUser(memberId) {
    $.ajax({
        type: "POST",
        url: "GetMemberUsers", //'<%: Url.Action("GetMemberUsers", "AuditTrail", new { area = "Profile"}) %>',
        dataType: "json",
        data: { id: memberId },
        success: function (response) {
            // this will give us an array of objects
            var subgroups = response;

            var objSelect = $('#User');
            $('option', objSelect).remove();

            var myOptions = '<option value="">Please Select</option>'; ;
            // iterate over subgroups
            for (var x = 0; x < subgroups.length; x++) {
                var twt = subgroups[x];
                myOptions = myOptions + '<option  value=\"' + twt.UserID + '\">' + twt.FirstName + '</option>';
            }
            objSelect.html(myOptions);
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('An error occurred! ' + errorThrown);
        }
    });
}


function validateAuditTrail(reportType, formId) {

    auditTrailValidator = $("#" + formId).validate({
        rules: {

            FromPeriod: {
                billingPeriod: ["auditTrail", $('input:radio[name=rdDateOrClearancePeriod]:checked').val()],
                required: function (element) {
                    if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'ClearancePeriod') {
                        return true;
                    } else {
                        return false;
                    }
                }
            },

            ToPeriod: {
                billingPeriod: ["auditTrail", $('input:radio[name=rdDateOrClearancePeriod]:checked').val()],
                required: function (element) {
                    if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'ClearancePeriod') {
                        return true;
                    } else {
                        return false;
                    }
                }
            },
            FromDate: {
                required: function (element) {
                    if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'Date') {
                        return true;
                    } else {
                        return false;
                    }
                }
            },

            ToDate: {
                required: function (element) {
                    if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'Date') {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        },
        messages: {
            FromPeriod: "Invalid From Period",
            ToPeriod: "Invalid To Period",
            FromDate: "From Date required",
            ToDate: "To Date required"
        },
        submitHandler: function (form) {
            $('#errorContainer').hide();
            if (reportType == "member") {
                if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'ClearancePeriod') {
                    if (validatePeriodRange($('#auditTrailFromPeriod').val(), $('#auditTrailToPeriod').val()))
                        redirectToReport(form.id);

                    else
                        alert("From period must be less than or equal to the To period");
                }
                else if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'Date') {
                    var dateComparisonResult = validateDateRange('auditTrailFromDate', 'auditTrailToDate');
                    if (dateComparisonResult) {
                        redirectToReport(form.id);
                    }
                    else {
                        alert("From date must be less than or equal to the To Date");
                    }
                }
                else
                    redirectToReport(form.id);

            }
            else {
                if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'ClearancePeriod') {
                    if (validatePeriodRange($('#auditTrailFromPeriod').val(), $('#auditTrailToPeriod').val()))
                        redirectToIchAchReport(reportType);
                    else {
                        alert("From period must be less than or equal to the To period");
                    }
                }
                else if ($('input:radio[name=rdDateOrClearancePeriod]:checked').val() == 'Date') {
                    if (validateDateRange('auditTrailFromDate', 'auditTrailToDate'))
                        redirectToIchAchReport(reportType);
                    else {
                        alert("From date must be less than or equal to the To Date");
                    }
                }
                else
                    redirectToIchAchReport(reportType);
            }

            $.watermark.showAll();
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }
    });
}

function validatePeriodRange(fromPeriod, toPeriod) {
    var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC");
    var fromtokenArray = fromPeriod.split("-");
    var fromperiod = fromtokenArray[2];
    var frommonth = fromtokenArray[1];
    var fromyear = fromtokenArray[0];
    var totokenArray = toPeriod.split("-");
    var toperiod = totokenArray[2];
    var tomonth = totokenArray[1];
    var toyear = totokenArray[0];

    if (fromyear > toyear)
        return false;
    else {
        if (fromyear == toyear) {
            if ($.inArray(frommonth.toUpperCase(), monthArray) > $.inArray(tomonth.toUpperCase(), monthArray))
                return false;
            else {
                if ($.inArray(frommonth.toUpperCase(), monthArray) == $.inArray(tomonth.toUpperCase(), monthArray)) {
                    if (fromperiod > toperiod)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
        }
        else
            return true;
    }
}

function validateDateRange(startDateId, endDateId) {
    var startDateVal = $('#' + startDateId).datepicker("getDate");
    var endDateVal = $('#' + endDateId).datepicker("getDate");
    return endDateVal >= startDateVal;
}



function redirectToInvoideDeletionReport(formId) {




    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    var SearchCriteria = 'Billing Category:' + (($('#BillingCategoryId option:selected').text() == '-1') ? 'All' : $('#BillingCategoryId option:selected').text().toString()) + ', Billing Year:' + (($('#BillingYear').val() == '') ? 'All' : $('#BillingYear').val());
    SearchCriteria = SearchCriteria + ', Billing Month:' + (($('#BillingMonth').val() == '') ? 'All' : $('#BillingMonth').val().toString()) + ', Billing Period:' + (($('#BillingPeriod').val() == '-1') ? 'All' : $('#BillingPeriod').val());
    SearchCriteria = SearchCriteria + ', Billed Member:' + (($('#BilledMember').val() == '') ? 'All' : $('#BilledMember').val().toString()) + ', Invoice No.:' + (($('#InvoiceNo').val() == '') ? 'All' : $('#InvoiceNo').val());
    SearchCriteria = SearchCriteria + ', Deleted From Date:' + (($('#DeletionDateFrom').val() == '') ? 'NULL' : $('#DeletionDateFrom').val().toString()) + ', Deleted To Date.:' + (($('#DeletionDateTo').val() == '') ? 'NULL' : $('#DeletionDateTo').val());
    SearchCriteria = SearchCriteria + ', Deleted By:' + (($('#DeletedBy').val() == '') ? 'NULL' : $('#DeletedBy').val().toString());
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/InvoiceDeletionAuditReport.aspx?BillingCat=" + $('#BillingCategoryId option:selected').val() + "&Year=" + $('#BillingYear').val() + "&Month=" + $('#BillingMonth').val() + "&Period=" + $('#BillingPeriod').val() + "&MemId=" + $('#BilledMemberId').val() + "&Invoice=" + $('#InvoiceNo').val() + "&DateFrom=" + $('#DeletionDateFrom').val() + "&DateTo=" + $('#DeletionDateTo').val() + "&DeletedBy=" + $('#DeletedBy').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports());
}


function ValidateInvoideDeletionReport(formId, validateBillYearMonthPeriodUrl) {


    var BillingCategory = $('#BillingCategoryId option:selected').val();
    var BillingYear = $('#BillingYear').val();
    var BillingMonth = $('#BillingMonth').val();
    var DeletedFrom = $('#DeletionDateFrom').val();
    var DeletedTo = $('#DeletionDateTo').val();

    if (BillingCategory == 0) {
        showClientErrorMessage('Please Select Billing Category');
        return false;
    }

    if (BillingYear == '') {
        showClientErrorMessage('Please Select Billing Year');
        return false;
    }

    if (BillingMonth == '') {
        showClientErrorMessage('Please Select Billing Month');
        return false;
    }

    
    if (DeletedFrom != '' && DeletedTo != '') {
        var dateComparisonResult = validateDateRange('DeletionDateFrom', 'DeletionDateTo');
        
        if (!dateComparisonResult) {
            showClientErrorMessage('Deleted To Date should not be earlier than Deleted From Date');
            return false;
        }
    }
    $('#clientErrorMessageContainer').hide();
    
    $.ajax({
        url: validateBillYearMonthPeriodUrl,
        data: { month: $('#BillingMonth :selected').val(), year: $('#BillingYear').val(), period: $('#BillingPeriod').val()},
        success: function (errMsg) { 
          if(errMsg != "Valid Parameters"){
            alert(errMsg);
          }
          else{
    redirectToInvoideDeletionReport(formId);
          }
        dataType: "text"
        },
        
      });
    

}















