//SCP271100: - Manage Users - Screen
//Convert mandatory field member code into optional field.
//Now member code and email address either is mandatory.
var AssociationTypeValue = 0;
var saveLocationAssociationURL = '/Profile/LocationAssociation/SaveLocationAssociation';

function SetSaveAssoLocationURL(url) {
    saveLocationAssociationURL = url;
}


function UserSearchOrModify(formId) {
    $("#SearchOrModify").validate({
        rules: {
            UserCategoryId: {
                required: function (element) {
                    if ($('#ddVisible').val() == '0') {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }

        },
        messages: {
            UserCategoryId: " Please Select User Category."
        },
        submitHandler: function (form) {
            if ($('#ddVisible').val() == '1') {
                if ($('#MemberName').val() == ''  && $.trim($('#Email').val()) == '') {
                    $('#clientErrorMessage').html("Either Member Code or Email Address is required.");
                    $('#clientErrorMessageContainer').show();
                }
                else {
                    $('#errorContainer').hide();
                    $('#clientErrorMessageContainer').hide();
                    $('#clientSuccessMessageContainer').hide();
                    onSubmitHandler(form);
                }
            }
            else {
                $('#errorContainer').hide();
                $('#clientErrorMessageContainer').hide();
                $('#clientSuccessMessageContainer').hide();
                onSubmitHandler(form);
            }
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });
}

function ManageUsersValidate(formId) {

  $("#ManageUsers").validate({
  submitHandler: function (form) {
      if ($('#ddVisible').val() == '1') {
          if (($('#MemberName').val() == '' || $('#MemberName').val() == '0') &&  $('#Email').val() == '') {
              $('#clientErrorMessage').html("Either member code or email address is required in search criteria.");
              $('#clientErrorMessageContainer').show();
          }
          else {
              $('#errorContainer').hide();
              $('#clientErrorMessageContainer').hide();
              $('#clientSuccessMessageContainer').hide();
              onSubmitHandler(form);
          }
      }
      else {
          $('#errorContainer').hide();
          $('#clientErrorMessageContainer').hide();
          $('#clientSuccessMessageContainer').hide();
          onSubmitHandler(form);
      }
  },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    }
  });
}

//CMP #655: IS-WEB Display per Location ID
function LocationAssociation(userId,memberId,emailId,url) {
    $("#hdnUserID").val(userId);
    $("#hdnEmailId").val(emailId);
    $("#hdnMemberId").val(memberId);
    $('#LocAssociationType').css({ 'display': 'none' });
    $('#locationListBox').css("display", "none");
      
    $.ajax({
        type: "Get",
        url: url,
        data: { userId: userId, memberId: memberId },
        dataType: "json",
        success: function (response) {

            $("#AssociatedLocation").html('');
            $("#UnAssociatedLocation").html('');

            if (response.length > 0) {

                AssociationTypeValue = response[0].AssociationType;
                if (AssociationTypeValue == '0') {
                    $("#radNone").attr('checked', 'checked');
                    $("#radNone").focus();
                    $('#LocAssociationType').css({ 'display': 'block' });
                    $('#locationListBox').css("display", "none");
                } else if (AssociationTypeValue == '1') {
                    $("#radAllLocation").attr('checked', 'checked');
                    $("#radAllLocation").focus();
                    $('#LocAssociationType').css({ 'display': 'block' });
                    $('#locationListBox').css("display", "none");
                } else {
                    $("#radSpecificLocation").attr('checked', 'checked');
                    $("#radSpecificLocation").focus();
                    $('#locationListBox').css({ 'display': 'block' });
                    $('#LocAssociationType').css({ 'display': 'block' });
                }
            }

            for (var i = 0, l = response.length; i < l; i++) {
                if (response[i].UserContactId == '0') {
                    // Unassigned List Box                        
                    var inHTML = '<option value="' + response[i].LocationId + '">' + response[i].LocationName + '</option>';
                    $("#UnAssociatedLocation").append(inHTML);
                }
                else {
                    // Assigned List Box
                    var inHTML = '<option value="' + response[i].LocationId + '">' + response[i].LocationName + '</option>';
                    $("#AssociatedLocation").append(inHTML);
                }
            }
        },
        failure: function (response) {
            showClientErrorMessage(response.Message);
        }
    });

    var userName = 'User : <b>' + emailId + '</b>';
    $("#lblUserName").html(userName);

    $("#dialogLocationAssociation").dialog({
        autoOpen: true,
        title: 'Manage Location Association',
        height: 400,
        width: 490,
        modal: true,
        resizable: false,
        close: function () {
            $('#LocAssociationType').css({ 'display': 'none' });
            $('#locationListBox').css("display", "none");
        }
    });

}

//CMP #655: IS-WEB Display per Location ID
$(document).ready(function () {
    $("#dialogLocationAssociation").hide();
});

$("#add").click(function () {
         var selected = $("#UnAssociatedLocation").find(':selected').val();
        $("#UnAssociatedLocation option:selected").appendTo("#AssociatedLocation");
        SortListItems("AssociatedLocation");
        $("#AssociatedLocation").val(selected);  
    });

    //If you want to move all item from availableFields to selectedFields
    $("#addAll").click(function () {
        $("#UnAssociatedLocation option").appendTo("#AssociatedLocation");
        SortListItems("AssociatedLocation");
    });

    //If you want to remove selected item from selectedFields to availableFields
    $("#remove").click(function () {
        var selected = $("#AssociatedLocation").find(':selected').val();
        $("#AssociatedLocation option:selected").each(function () {
            $(this).appendTo("#UnAssociatedLocation");
        });
        SortListItems("UnAssociatedLocation");
        $("#UnAssociatedLocation").val(selected);
    });

    //If you want to remove all items from selectedFields to availableFields
    $("#removeAll").click(function () {
        $("#AssociatedLocation option").appendTo("#UnAssociatedLocation");
        SortListItems("UnAssociatedLocation");
    });

    $("#btnCancel").click(function () {
        $("#dialogLocationAssociation").dialog("close");
        $('#LocAssociationType').css({ 'display': 'none' });
        $('#locationListBox').css("display", "none");
    });



    function SortListItems(ListBoxId) {

        var Location = ["MAIN", "UATP"];

        var listIntLocation = $("#" + ListBoxId + " option");
        var listStringLocation = listIntLocation;

        for (var i = listStringLocation.length - 1; i >= 0; --i) {
            var itemText = listStringLocation[i].innerHTML;
            itemText = itemText.split('-')[0];

            var found = $.inArray(itemText.toUpperCase(), Location) > -1;

            if (!found) {

                listStringLocation = jQuery.grep(listStringLocation, function (value) {
                    return value != listStringLocation[i];
                });

            }
        }

        for (var i = listIntLocation.length - 1; i >= 0; --i) {
            var itemText = listIntLocation[i].innerHTML;
            itemText = itemText.split('-')[0];
            var found = $.inArray(itemText.toUpperCase(), Location) > -1;

            if (found) {
                listIntLocation = jQuery.grep(listIntLocation, function (value) {
                    return value != listIntLocation[i];
                });

            }
        }

        listStringLocation.sort(function (a, b) {
            var firstItem = a.innerHTML.split('-')[0];
            var secondItem = b.innerHTML.split('-')[0];
            if (firstItem > secondItem) return 1;
            else if (firstItem < secondItem) return -1;
        });


        listIntLocation.sort(function (a, b) {
            var firstItem = a.innerHTML.split('-')[0];
            var secondItem = b.innerHTML.split('-')[0];
            if (parseInt(firstItem) > parseInt(secondItem)) return 1;
            else if (parseInt(firstItem) < parseInt(secondItem)) return -1;

        });
        $("#" + ListBoxId).empty().append(listStringLocation);
        $("#" + ListBoxId).append(listIntLocation);
    }





    $("input[name='AssociationType']").change(function () {
        AssociationTypeValue = $(this).val();

        if (AssociationTypeValue == "2") {
            $('#locationListBox').css({ 'display': 'block' });
        } else {
            $('#locationListBox').css("display", "none");
        }

    });



    $("#btnSave").bind('click', function () {
        var selectedLocationIds = '';

        if (AssociationTypeValue == 2) {

            $("#AssociatedLocation").each(function () {
                $('option', this).each(function () {
                    selectedLocationIds = selectedLocationIds + ',' + $(this).val();
                });
            });
            if (selectedLocationIds == '') {
                alert('At least one Location ID should be associated with the User');
                return false;
            }
        }

        var userId = $("#hdnUserID").val();
        var excludedLocIds = '';
        var paramEmailId = $("#hdnEmailId").val();
        var paramMemberId = $("#hdnMemberId").val();

        $.ajax({
            type: "POST",
            url: saveLocationAssociationURL,
            data: { locationSelectedIds: selectedLocationIds, excludedLocIds: excludedLocIds, userId: userId, associtionType: AssociationTypeValue, emailId: paramEmailId, memberId: paramMemberId },
            dataType: "json",
            success: function (response) {
                if (response.Message) {
                    if (response.IsFailed == false) {
                        showClientSuccessMessage(response.Message);


                    } else {
                        showClientErrorMessage(response.Message);
                        if (typeof result.Message === 'undefined') {
                            showClientErrorMessage('Session seems to be expired. Please log in again');
                        }
                    }
                }
                else {
                    showClientErrorMessage('Session seems to be expired. Please log in again');
                }
            },
            failure: function (response) {
                showClientErrorMessage(response.Message);
            }
        });

        $("#dialogLocationAssociation").dialog("close");
        $('#LocAssociationType').css({ 'display': 'none' });
        $('#locationListBox').css("display", "none");
    });
    // End Code CMP #655: IS-WEB Display per Location ID