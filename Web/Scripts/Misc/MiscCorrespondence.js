//CMP #661: Popup Before Sending a Correspondence
// set default flag false here, which will further used to show confirmation popup after validation on submitHandler
var _isSendClicked = false;

function InitialiseCorrespondenceHeader() {
  $("#UploadAttachment").bind("click", openAttachment);
  $("#AmountToBeSettled").bind("blur", CheckNonZeroValue);

  $("#CorrespondenceHeader").validate({
    rules: {
      Subject: "required",
      AmountToBeSettled: "required",
      CurrencyId: "required",
      ToEmailId: "required",
      CorrespondenceStatus: "required",
      CorrespondenceDetails: "required",
      ToAdditionalEmailIds: "multiemail"
    },
    messages: {
      Subject: "Subject Required",
      AmountToBeSettled: "Amount To Be Settled Required",
      CurrencyId: "Currency Code Required",
      ToEmailId: "To E-mail ID Required",
      CorrespondenceStatus: "Correspondence Status Required",
      CorrespondenceDetails: "Correspondence Text Required"
    },
    submitHandler: function (form) {
      //CMP #661: Popup Before Sending a Correspondence
      // check if "Send" correspondence button clicked from screen, then only show the confirmation popup
      if (_isSendClicked) {
        $('#confirm_correspondence_send').dialog({
          title: "Confirm Correspondence Send",
          closeOnEscape: false,
          height: 130, width: 300, modal: true,
          resizable: false,
          open: function () {
            $(this).closest('.ui-dialog').find('.ui-dialog-buttonpane button')[1].focus();
          },
          buttons: {
            Save: {
              class: 'primaryButton',
              text: 'Yes',
              click: function () {
                $(this).dialog("close");

                $("#CurrencyId").removeAttr('disabled');
                $("#CorrespondenceDate").removeAttr('readonly');
                $("#ToEmailId").removeAttr('disabled');
                $('#SaveCorrespondenceAsReadyToSubmit').attr('disabled', true);
                $('#SaveCorrespondence').attr('disabled', true);
                $('#SendCorrespondence').attr('disabled', true);

                // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
                onSubmitHandler(form);
                // form.submit();
              }
            },
            Cancel: {
              class: 'secondaryButton',
              text: 'No',
              click: function () {
                $(this).dialog("close");
              }
            }
          }
        }
      );
      }
      else { // if "Save" and "Ready To Submit"  correspondence button clicked, then perform normal flow
        $("#CurrencyId").removeAttr('disabled');
        $("#CorrespondenceDate").removeAttr('readonly');
        $("#ToEmailId").removeAttr('disabled');
        $('#SaveCorrespondenceAsReadyToSubmit').attr('disabled', true);
        $('#SaveCorrespondence').attr('disabled', true);
        $('#SendCorrespondence').attr('disabled', true);

        // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
        onSubmitHandler(form);
        // form.submit();
      }
    }
  });

  trackFormChanges('CorrespondenceHeader');
}

function CheckNonZeroValue() {
  var amount = $("#AmountToBeSettled").val();
  
  if (!isNaN(amount)) {
    if (amount == 0) {
      if (!confirm("Amount To Be Settled Is Zero. Are you sure?")) {
        $("#AmountToBeSettled").focus();
        return;
      }
    }
    $("#AmountToBeSettled").val(Number(amount).toFixed(2));
  }
}

function downloadPDF(actionUrl) {
  $.ajax({
    type: "GET",
    url: actionUrl,    
    dataType: "json",
    success: function (response) {      
    },
    failure: function (response) {
    }
  });
}

function CheckIfBMExists(isAuthorityToBill, isBMExistsAction, redirectUrl) {
  // If Authority to bill is set, and if BM exists, do not allow user to reply to correspondence.
  if (isAuthorityToBill == 'True') {
    $.ajax({
      type: "POST",
      url: isBMExistsAction,
      data: { correspondenceRefNumber: $('#CorrespondenceNumber').val() },
      dataType: "json",
      success: function (response) {
        if (response.IsFailed == false) {
          location.href = redirectUrl;
          return;
        }
        alert(response.Message);
        return;
      }
    });
  }
  else {
    location.href = redirectUrl;
    return;
  }
}