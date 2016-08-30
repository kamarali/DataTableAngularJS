function deleteRecord(methodName, value, gridId) {

    $('#successMessageContainer').hide();

    var maxCouponAllowed = 1;
    var $couponList = $('#searchGrid');
    var rowIds = $couponList.getDataIDs();

    if (rowIds.length > maxCouponAllowed) {
        flag = 0;
        if (confirm("Are you sure you want to delete this record?")) {
            $.ajax({
                type: "POST",
                url: methodName + "/" + value,
                data: { isLastCoupon : false },
                success: function (result) {
                    $('#errorContainer').hide();
                    if (result.IsFailed == false) {
                        if (result.isRedirect) {
                            location.href = result.RedirectUrl;
                        }
                        if (result.LineItemDetailExpected) {
                            alert("Line item detail expected.");
                        }

                        //For contacts tab after successful delete remove that contact from other dropdown list.
                        if (methodName == "DeleteContact") {
                            $("#replaceoldcontact option[value=" + value + "]").remove();
                            $("#replacenewcontact option[value=" + value + "]").remove();
                            $("#copyoldcontact option[value=" + value + "]").remove();
                            $("#copynewcontact option[value=" + value + "]").remove();
                        }

                        // Toggle message containers.          
                        showClientSuccessMessage(result.Message);
                    }
                    else {
                        showClientErrorMessage(result.Message);
                    }
                    $(gridId).trigger("reloadGrid");
                }
            });
        }
    }
    else {
        flag = 1;
        if (confirm("Deleting this record will also delete the invoice permanently!\n\nAre you sure you want to proceed?")) {
            $.ajax({
                type: "POST",
                url: methodName + "/" + value,
                data: { isLastCoupon: true },
                success: function (result) {
                    $('#errorContainer').hide();
                    if (result.IsFailed == false) {
                        if (result.isRedirect) {
                            location.href = result.RedirectUrl;
                        }
                        if (result.LineItemDetailExpected) {
                            alert("Line item detail expected.");
                        }

                        //For contacts tab after successful delete remove that contact from other dropdown list.
                        if (methodName == "DeleteContact") {
                            $("#replaceoldcontact option[value=" + value + "]").remove();
                            $("#replacenewcontact option[value=" + value + "]").remove();
                            $("#copyoldcontact option[value=" + value + "]").remove();
                            $("#copynewcontact option[value=" + value + "]").remove();
                        }

                        // Toggle message containers.          
                        showClientSuccessMessage(result.Message);
                    }
                    else {
                        showClientErrorMessage(result.Message);
                    }
                    $(gridId).trigger("reloadGrid");
                }
            });
        }
    }
}