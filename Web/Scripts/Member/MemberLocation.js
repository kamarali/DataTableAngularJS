



function redirectToReport(formId) {

    var memId;
    var locId;

    memId = $('#ParticipantID').val();
    locId = $('#locationId').val();

    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    var SearchCriteria = 'Member Code:' + ((memId == '') ? 'All' : $('#ParticipantText').val().toString()) + ', Location ID:' + ((locId == '') ? 'All' : $('#locationId').val());
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/MemberLocationReport.aspx?memId=" + memId + "&locId=" + locId + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports());
}





