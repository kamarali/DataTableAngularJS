function deleteRecordMethod(methodName, value, gridId, _selectedMemberId) {
  
  $('#successMessageContainer').hide();
  
  if (confirm("Are you sure you want to delete this record?")) {
    $.ajax({
      type: "POST",
      url: methodName,
      data: { id:value, selectedMemberId: _selectedMemberId },
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


function deleteRecord(methodName, value, gridId) {
    $('#successMessageContainer').hide();

    if (confirm("Are you sure you want to delete this record?")) {
        $.ajax({
            type: "POST",
            url: methodName + "/" + value,
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


function activateRecord(methodName, value, gridId) {
    $('#successMessageContainer').hide();

    if (confirm("Are you sure you want to activate this record?")) {
        $.ajax({
            type: "POST",
            url: methodName + "/" + value,
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
                    showClientSuccessMessage("Record activated successfully.");
                }
                else {
                    showClientSuccessMessage("Record activated successfully.");
                }
                $(gridId).trigger("reloadGrid");
            }
        });
    }
}

//CMP #553: ACH Requirement for Multiple Currency Handling
function activateAchCurrency(methodName, achCurrencyUrl, value, gridId) {
    $('#successMessageContainer').hide();

    if (confirm("Are you sure you want to activate this record?")) {

        $.ajax({
            type: "GET",
            url: achCurrencyUrl,
            data: { currencyCodeNum: value },
            dataType: "json",
            success: function (data) {
                if (data == true) {
                    $.ajax({
                        type: "POST",
                        url: methodName + "/" + value,
                        success: function (result) {
                            $('#errorContainer').hide();
                            if (result.IsFailed == false) {
                                if (result.isRedirect) {
                                    location.href = result.RedirectUrl;
                                }
                                // Toggle message containers.          
                                showClientSuccessMessage("Record activated successfully.");
                            }
                            else {
                                showClientSuccessMessage("Record activated successfully.");
                            }
                            $(gridId).trigger("reloadGrid");
                        }
                    });
                } //End If
                else {
                    alert('Activation failed as the Currency of Clearance is Inactive as per ISO Currency Setup');
                }
            }
        });
    }
}

function dactivateRecord(methodName, value, gridId) {
    $('#successMessageContainer').hide();

    if (confirm("Are you sure you want to deactivate this record?")) {
        $.ajax({
            type: "POST",
            url: methodName + "/" + value,
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
                    showClientSuccessMessage("Record deactivated successfully.");
                }
                else {
                    if (result.IsFailed && methodName.contains("ActiveDeactiveMemberSubStatus")) {

                        showClientErrorMessage(result.Message);

                    } else {
                        showClientSuccessMessage("Record deactivated successfully.");
                    }
                   
                }
                $(gridId).trigger("reloadGrid");
            }
        });
    }
}