
function GenerateIsCalendarReport(formId) {
    var Isvalid = $("#IsCalendarReport").validate({
        rules: {

            ClearanceYear: {
                required: true
            },
            EventCategory: {
                required: true
            }
        },
        messages: {
            ClearanceYear: " Clearance Year Required.",
            EventCategory: " Event Category Required."
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

    var clearanceYear;
    var eventCategory;
    var timeZone;

    clearanceYear = $('#ClearanceYear').val();
    eventCategory = $('#EventCategory').val();
    timeZone = $('#TimeZone').val();
    //Changes to display search criteria on report
    var SearchCriteria = 'Calender Year:' + clearanceYear + ', Calender Type:' + $('#EventCategory :selected').text() + ', Time Zone:' + $('#TimeZone :selected').text();    
    //regular expression used to have multiple replaces in string
    var regPlus = RegExp("\\+", "g");
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    //+ Sign causes showing blank
    SearchCriteria = SearchCriteria.replace(regPlus, " and ");
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/IsCalendarReport.aspx?ClearanceYear=" + clearanceYear + "&EventCategory=" + eventCategory + "&TimeZone=" + timeZone + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}

