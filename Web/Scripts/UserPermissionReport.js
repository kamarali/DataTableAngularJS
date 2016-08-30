function redirectToUserPermission(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    var searchCriteria = null;
    if ($('#UserCategoryId').val() == undefined) {
         searchCriteria = 'User Name:' + ($('#UserName').val() == '' ? 'All' : $('#UserName').val());
    }
     else if (($('#UserCategoryId').val() != undefined) && ($('#MemberName').val() == "" )) {
        searchCriteria = 'User Category:' + $('#UserCategoryId :selected').text() + ', User Name:' + ($('#UserName').val() == '' ? 'All' : $('#UserName').val());
    }
     else {
         searchCriteria = 'User Category:' + $('#UserCategoryId :selected').text() + ', Member:' + $('#MemberName').val() + ', User Name:' + ($('#UserName').val() == '' ? 'All' : $('#UserName').val()); 
    }
    var regAnd = RegExp("\\&", "g");
    //"&" Sign causing value to be truncated on getting querystring, so replaced & with 'and'
    searchCriteria = searchCriteria.replace(regAnd, "and");
    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/UserPermissionReport.aspx?UserCatId=" + $('#UserCategoryId').val() + "&MemId=" + $('#MemberId').val() + "&Email=" + $('#UserName').val() + "&SearchCriteria=" + searchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}


function ValidateUserPermissionReport(formId) {

    $("#userPermissionReport").validate({
        rules: {

            UserCategoryId: {
                required: true

            },
            MemberId: {
                required: function (element) {
                    if ($('#UserCategoryId').val() == "4" && $('#MemberId').val() == '' && $('#UserName').val() == '') {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            },
            UserName: {
                required: function (element) {
                    if ($('#UserCategoryId').val() == "4" && $('#MemberId').val() == '' && $('#UserName').val() == '') {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        },
        messages: {

            UserCategoryId: "User Category is required.",
            MemberId: "Either Member or User Name is mandatory for Member User Report.",
            UserName: "Either Member or User Name is mandatory for Member User Report."
        },

        submitHandler: function (form) {            
                $('#errorContainer').hide();
                redirectToUserPermission(form.id);
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

