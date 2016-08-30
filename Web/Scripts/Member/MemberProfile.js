var $memberLogoToUpload = false;
var $ImageUploaddialog;
var DisplayDetailsPostUrl = '/Member/GetContactDetails';
var memContactsDatagetUrl = 'Member/getContactList';
var MemberLocationDataUrl = '/Member/GetMemberLocationList';
var CheckIfUserUrl = '/Member/GetUserByEmailId';
var UserCitySubdivisionNameUrl = '/Member/GetUserCityNameAndSubDivisionName';
var locationValidator;
var contactValidator;

$(document).ready(function () {
    function CommercialName_SetAutocompleteDisplay(item) {
        var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "(" + item.CommercialName + ")";
        return { label: memberCode, value: memberCode, id: item.Id };
    }

});

function setCheckIfUserUrl(IfUserUrl) {
  CheckIfUserUrl = IfUserUrl;
}

function SetUserCitySubdivisionNameUrl(UserCitySubdivisionUrl) {
  UserCitySubdivisionNameUrl = UserCitySubdivisionUrl;
}

function SetPostUrl(postUrl) {
  DisplayDetailsPostUrl = postUrl;
}

function SetcontactsDataUrl(contactsDataUrl) {
  memContactsDatagetUrl = contactsDataUrl;
}

function SetLocationDataUrl(locationDataUrl) {
  MemberLocationDataUrl = locationDataUrl;
}

function setCheckIfUserUrl(IfUserUrl) {
  CheckIfUserUrl = IfUserUrl;
}

function SetUserCitySubdivisionNameUrl(UserCitySubdivisionUrl) {
  UserCitySubdivisionNameUrl = UserCitySubdivisionUrl;
}

function IsValidperiod(datecheck) {
    var flag = false;
    if ((jQuery.trim(datecheck).length == 0 || (datecheck == _periodFormat))) {
        flag = true;
    }
    else {
        var arr_d1 = datecheck.split("-");

        day = arr_d1[2];
        month = arr_d1[1];
        year = arr_d1[0];
        var d = new Date();
        var curr_year = d.getFullYear();
        var curr_month = d.getMonth();
        var curr_day = d.getDate();
        var curr_period;
        var periodarray = new Array("01", "02", "03", "04");
        var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC");
        var isValidYear = /^[0-9]+$/.test(year);

        if ($.inArray(day, periodarray) <= -1 || $.inArray(month.toUpperCase(), monthArray) <= -1 || !isValidYear || year.length != 4) {
            alert("Please enter valid  period format in Merger Effective Period(New Value).");
            flag = false;
        }
        else {
            flag = true;
        }
    }
    return flag;
}

function popupFutureUpdateDialog(callerId, fieldType, hasPeriod, calendarImgPath, isFieldMandatory) {

  $('#futureValueDialog').dialog({
    title: function () {
      // CMP#689: Flexible CH Activation Options
      // for field type 19 & 20 (additional condition of 16 & 17) change the title
      if (fieldType == 19 || fieldType == 20) {
        return 'Please Select Change Timing';
      }
      else {
        return 'Please Specify Effective Date OR Period';
      }
    },
    modal: true,
    resizable: false,
    beforeClose: function (event, ui) {
      if (fieldType == 2) {
        if (callerId == '#IsMerged') {
          $('#futureMergerFromValue').removeAttr('disabled');
          $('#MemberTextFutureValue').removeAttr('disabled');
        }
      }
    },
    buttons: {
      Save: {
        class: 'primaryButton',
        text: function () {
          // CMP#689: Flexible CH Activation Options
          // for field type 19 & 20 (additional condition of 16 & 17) change the Save button text
          if (fieldType == 19 || fieldType == 20) {
            return 'OK';
          }
          else {
            return 'Save';
          }
        },
        click: function () {
          var valueChanged;
          var checkedradio = $('input[name=changeTimingRB]:radio:checked').val();

          if (fieldType == 19 || fieldType == 20) {
            if (checkedradio == 2) {
              if (!ValidateFuturePeriod("#futurePeriod")) {
                $('#futurePeriod').focus();
                return false;
              }
            }
          }
          else {
            if (hasPeriod) {
              if (!ValidateFuturePeriod("#futurePeriod")) {
                $('#futurePeriod').focus();
                return false;
              }
            }
            else {
              if (!ValidateFutureDate("#futureDate")) { $('#futureDate').focus(); return false; }
            }
          }



          // Update the future value in the appropriate hidden field.
          if (fieldType == 1) {

            if (isFieldMandatory == 1) {
              if ($.trim($('#futureTextValue').val()) == '') {
                alert("Please enter new value");
                return false;
              }
            }

            // validate special characters entered in textbox.
            if ($.trim($('#futureTextValue').val()) != '') {
              var re = new RegExp("^[\040-\176]*$");
              if (!$.trim($('#futureTextValue').val()).match(re)) {
                alert("Field value contains invalid characters.");
                return false;
              }
            }

            //TFS 9732 :Future Date or period text boxes are accepting Scripts in Member Profile
            var futureTextinputed = $('#futureTextValue').val();
            var sanitizedText = futureTextinputed.replace(/<script[^>]*?>.*?<\/script>/gi, '').replace(/<[\/\!]*?[^<>]*?>/gi, '').replace(/<style[^>]*?>.*?<\/style>/gi, '').replace(/<![\s\S]*?--[ \t\n\r]*>/gi, '');
            $('#futureTextValue').val(sanitizedText);

            valueChanged = $(callerId).val() != $('#futureTextValue').val();
            $(callerId + 'FutureValue').val($.trim($('#futureTextValue').val()));
            $(callerId + 'FuturePeriod').val($.trim($('#futurePeriod').val()));
          }
          else if (fieldType == 2) {

            //Here Logic implemented for CMP-409 by Upendra Y
            if (callerId == '#IsMerged') {

              $('#futureMergerFromValue').removeAttr('disabled');
              $('#MemberTextFutureValue').removeAttr('disabled');

              valueChanged_IsMerge = $(callerId).prop('checked') != $('#FutureIsMergedValue').prop('checked');

              if (valueChanged_IsMerge) {
                valueChanged = true;
              }
              else {

                if ($(callerId).prop('checked') == true && $('#FutureIsMergedValue').prop('checked') == true) {
                  valueChanged_Parent = $('#ParentMemberId').val() != $('#MemberIdFutureValue').val();
                  if (valueChanged_Parent) {
                    valueChanged = true;
                  }
                  else {
                    valueChanged_ParentMember = $('#ParentMemberIdFutureValue').val() != $('#MemberIdFutureValue').val();
                    valueChanged_MergerDate = $('#ActualMergerDateFutureValue').val() != $('#futureMergerFromValue').val();
                    valueChanged_MergerPeriod = $(callerId + 'FuturePeriod').val() != $('#futurePeriod').val();

                    if (valueChanged_ParentMember || valueChanged_MergerDate || valueChanged_MergerPeriod) {
                      valueChanged = true;
                    }
                  }
                }
              }

              if ($('#FutureIsMergedValue').prop('checked') == true) {
                if ($('#MemberIdFutureValue').val() == '' || $('#futureMergerFromValue').val() == '') {
                  alert("If Is Merged (New Value) is checked then Merger Effective From(New Value) and Parent Member(New Value) can't be blank!");
                  return false;
                }
                else {
                  if (!IsValidperiod($('#futureMergerFromValue').val())) {
                    $('#futureMergerFromValue').focus();
                    return false;

                  }
                  else {

                    $(callerId + 'FutureValue').val($('#FutureIsMergedValue').prop('checked'));
                    $(callerId + 'FuturePeriod').val($.trim($('#futurePeriod').val()));
                    $(callerId + 'FutureValue').prop('checked', $('#FutureIsMergedValue').prop('checked'));


                    $('#ActualMergerDateFutureValue').val($('#futureMergerFromValue').val());
                    $('#ParentMemberIdDisplayValue').val($('#MemberTextValue').val());
                    $('#ParentMemberIdFutureDisplayValue').val($('#MemberTextFutureValue').val());
                    $('#ParentMemberIdFutureValue').val($('#MemberIdFutureValue').val());
                  }
                }
              }
              else {
                $('#ActualMergerDateFutureValue').val(null);
                $('#ParentMemberIdFutureDisplayValue').val(null);
                $('#ParentMemberIdFutureValue').val(null);
                $('#IsMergedFutureValue').val(null);
                $(callerId + 'FuturePeriod').val($.trim($('#futurePeriod').val()));
              }
            }
            else {
              valueChanged = $(callerId).prop('checked') != $('#checkboxFutureValue').prop('checked');
              $(callerId + 'FutureValue').val($('#checkboxFutureValue').prop('checked'));
              $(callerId + 'FuturePeriod').val($.trim($('#futurePeriod').val()));
              $(callerId + 'FutureDate').val($('#futureDate').val());
              $(callerId + 'FutureValue').prop('checked', $('#checkboxFutureValue').prop('checked'));
              if (callerId == "#IsParticipateInAutoBilling") {
                if ($(callerId + 'FutureValue').val() == "true") {
                  $('#InvoiceNumberRangePrefix').removeAttr("readonly");
                  $('#InvoiceNumberRangeFrom').removeAttr("readonly");
                  $('#InvoiceNumberRangeTo').removeAttr("readonly");
                  $(".cuttOffTimeEditLink").show();
                  $(".listingCurrencyEditLink").show();
                  $(".isIsrFileRequiredEditLink").show();
                }
              }
            }
          }

          else if (fieldType == 4) {

            //TFS 9732 :Future Date or period text boxes are accepting Scripts in Member Profile
            var futureTextinputed = $('#AutoSubdivisionFutureValue').val();
            var sanitizedText = futureTextinputed.replace(/<script[^>]*?>.*?<\/script>/gi, '').replace(/<[\/\!]*?[^<>]*?>/gi, '').replace(/<style[^>]*?>.*?<\/style>/gi, '').replace(/<![\s\S]*?--[ \t\n\r]*>/gi, '');
            $('#AutoSubdivisionFutureValue').val(sanitizedText);

            valueChanged = $(callerId).val() != $('#AutoSubdivisionFutureValue').val();
            $(callerId + 'FutureValue').val($('#AutoSubdivisionFutureValue').val());
            $(callerId + 'FuturePeriod').val($.trim($('#futurePeriod').val()));
          }
          else if (fieldType == 5) {

            valueChanged = $(callerId).val() != $('#futureTextAreaValue').val();
            if ($('#futureTextAreaValue').val().length > 700) {
              alert("Please do not enter more than 700 characters");
              return false;
            }

            // validate special characters entered in textbox.
            if ($.trim($('#futureTextAreaValue').val()) != '') {
              var re = new RegExp("^[\011\012\015\040-\176]*$");
              if (!$.trim($('#futureTextAreaValue').val()).match(re)) {
                alert("Field value contains invalid characters.");
                return false;
              }
            }

            //TFS 9732 :Future Date or period text boxes are accepting Scripts in Member Profile
            var futureTextinputed = $('#futureTextAreaValue').val();
            var sanitizedText = futureTextinputed.replace(/<script[^>]*?>.*?<\/script>/gi, '').replace(/<[\/\!]*?[^<>]*?>/gi, '').replace(/<style[^>]*?>.*?<\/style>/gi, '').replace(/<![\s\S]*?--[ \t\n\r]*>/gi, '');
            $('#futureTextAreaValue').val(sanitizedText);

            $(callerId + 'FutureValue').val($('#futureTextAreaValue').val());
          }
          else if (fieldType == 6) {
            valueChanged = $(callerId).val() != $('#AutoSponsoredbyTextFutureValue').val();
            $(callerId + 'FutureValue').val($('#AutoSponsoredbyTextFutureValue').val());
            $(callerId + 'FutureValueId').val($('#AutoSponsoredIdFutureValue').val());
          }
          else if (fieldType == 7) {
            valueChanged = $(callerId).val() != $('#AutoAggregatedbyTextFutureValue').val();
            $(callerId + 'FutureValue').val($('#AutoAggregatedbyTextFutureValue').val());
            $(callerId + 'FutureValueId').val($('#AutoAggregatedIdFutureValue').val());
          }
          else if ((fieldType == 8) || (fieldType == 9) || (fieldType == 10) || (fieldType == 11) || (fieldType == 12) || (fieldType == 13) || (fieldType == 14) || (fieldType == 15) || (fieldType == 16) || (fieldType == 17) || (fieldType == 18)) {

            if ((fieldType == 8 || fieldType == 11 || fieldType == 12) && ($.trim($('#dropdownFutureValue').val()) == '')) {
              if (isFieldMandatory == 1) {
                alert("Please enter new value");
                return false;
              }
            }
            //SCP221813 - Auto Billing issue(If ListingCurrencyId is not present in database, will update from ListingCurrencyIdFutureValue).
            if (fieldType == 14 && !$("#IsParticipateInAutoBilling").prop('checked')) {
              $(callerId).val($('#dropdownFutureValue').val());
            }
            //If membership status dropdown is caller, then current db value for membership status should be displayed in current value dropdown
            //Caller should not be displayed as disabled field
            if ((fieldType == 16) || (fieldType == 17) || (fieldType == 18)) {
              if (fieldType == 16) $(callerId).val($('#hdnIchMembershipStatus').val());
              if (fieldType == 17) $(callerId).val($('#hdnAchMembershipStatus').val());
              $(callerId).removeAttr('disabled');
              if (fieldType == 18) {
                if ($('#hdnIsMembershipStatus').val() == "") {
                  $('#hdnIsMembershipStatus').val("2");
                }
                $(callerId).val($('#hdnIsMembershipStatus').val());

                $(".statusEditLink").show();
                $(callerId).removeAttr('disabled');
              }

              //If membership status dropdown is caller, then current db value for membership status should be displayed in current value dropdown
              //Caller should not be displayed as disabled field
              if ((fieldType == 16) || (fieldType == 17) || (fieldType == 18)) {
                if (fieldType == 16) $(callerId).val($('#hdnIchMembershipStatus').val());
                if (fieldType == 17) $(callerId).val($('#hdnAchMembershipStatus').val());
                if (fieldType == 18) $(callerId).val($('#hdnIsMembershipStatus').val());

                if ($('#dropdownFutureValue').val() == '') {
                  alert("Please select new value");
                  return false;
                }
              }
            }
            //Set Current field display value
            valueChanged = $(callerId).val() != $('#dropdownFutureValue').val();
            $(callerId + 'FutureValue').val($('#dropdownFutureValue').val());

            $(callerId + 'DisplayValue').val($("#currentDropdownValue option:selected").text());
            $(callerId + 'FutureDisplayValue').val($("#dropdownFutureValue option:selected").text());


          }
          else if (fieldType == 19 || fieldType == 20) {

            if (checkedradio == 1) {
              //enable the caller field here otherwise it will not update
              $(callerId).attr("disabled", false);
              $(callerId + 'FutureValue').val("");
              // restrict the user to enter a future date as compared to the system date .
              if (fieldType == 19) $("#ichEntryDate").datepicker("option", "maxDate", new Date());
              if (fieldType == 20) $("#achEntryDate").datepicker("option", "maxDate", new Date());
            }
            else if (checkedradio == 2) {

              if (fieldType == 19) $(callerId).val($('#hdnIchMembershipStatus').val());
              if (fieldType == 20) $(callerId).val($('#hdnAchMembershipStatus').val());

              //Set Current field display value
              valueChanged = $(callerId).val() != $('#dropdownFutureValue').val();
              $(callerId + 'FutureValue').val($('#dropdownFutureValue').val());
              $(callerId + 'DisplayValue').val($("#currentDropdownValue option:selected").text());
              $(callerId + 'FutureDisplayValue').val($("#dropdownFutureValue option:selected").text());

              // allow the user to enter a future date as compared to the system date (in addition to current or past dates).
              if (fieldType == 19) $("#ichEntryDate").datepicker("option", "maxDate", null);
              if (fieldType == 20) $("#achEntryDate").datepicker("option", "maxDate", null);

              //disable the caller field here 
              $(callerId).attr("disabled", true);

            }

            if (fieldType == 19) {
              //Zone Should be enabled and become immediately updateable
              $('#IchZoneId').attr("disabled", false);
              $('#FutureEditLinkFor_IchZoneId').hide();
              //Category Should be enabled and become immediately updateable
              $('#IchCategoryId').attr("disabled", false);
              $('#FutureEditLinkFor_IchCategoryId').hide();
            }
            if (fieldType == 20) {
              //Category Should be enabled
              $('#AchCategoryId').attr("disabled", false);
            }

          }
          // Get the future period/date value and update the hidden fields.

          if (hasPeriod) {

            if (callerId == '#IsMerged') {
              $('ParentMemberIdFuturePeriod').val($('#futurePeriod').val());
            }
            else {
              $(callerId + 'FuturePeriod').val($('#futurePeriod').val());
            }
          }
          else {
            $(callerId + 'FutureDate').val($('#futureDate').val());
          }

          // Show the future dated value indicator (exclamation image).
          if (valueChanged) {
            $(callerId + 'FutureDateInd').show();
          }
          else {
            $(callerId + 'FutureDateInd').hide();
            if (callerId == "#IsParticipateInAutoBilling") {
              $('#InvoiceNumberRangePrefix').val("");
              $('#InvoiceNumberRangeFrom').val("");
              $('#InvoiceNumberRangeTo').val("");
              $('#InvoiceNumberRangePrefix').attr('readonly', 'true');
              $('#InvoiceNumberRangeFrom').attr('readonly', 'true');
              $('#InvoiceNumberRangeTo').attr('readonly', 'true');
              $(".cuttOffTimeEditLink").show();
              $(".listingCurrencyEditLink").show();
              $(".isIsrFileRequiredEditLink").show();
            }
          }

          // Set the form as dirty and close the dialog.
          $parentForm.setDirty();
          $(this).dialog("close");
        }
      },
      Cancel: {
        class: 'secondaryButton', text: 'Cancel',
        click: function () {
          if (fieldType == 2) {
            if (callerId == '#IsMerged') {
              $('#futureMergerFromValue').removeAttr('disabled');
              $('#MemberTextFutureValue').removeAttr('disabled');
            }
          }
          if (fieldType == 16 || fieldType == 19) { //CMP#689: Flexible CH Activation Options
            $(callerId).val($('#hdnIchMembershipStatus').val());
            $(callerId).removeAttr('disabled');
            $(callerId).change();
          }
          if (fieldType == 17 || fieldType == 20) { //CMP#689: Flexible CH Activation Options
            $(callerId).val($('#hdnAchMembershipStatus').val());
            $(callerId).removeAttr('disabled');
            $(callerId).change();
          }
          if (fieldType == 18) {
            $(callerId).val($('#hdnIsMembershipStatus').val());
            $(callerId).removeAttr('disabled');
            $(callerId).change();
          }
          $(this).dialog("close");
        }
      }
    },
    open: function () {
      // Display the current value and the future value (from the hidden field) in the popup.

      //Get attributes of current value fields
      var maxLengthattr = $(callerId).attr("maxLength");
      var classattr = $(callerId).attr("class");
      var rowsattr = $(callerId).attr("rows");
      var colsattr = $(callerId).attr("cols");

      if (fieldType == 1) {
        $('#futureTextValue').attr({ 'class': classattr, 'maxLength': maxLengthattr });
        if ($(callerId).val() != ".doc,.xls etc.")
          $('#currentTextValue').val($(callerId).val());
        if (callerId == "#PaxAllowedFileTypesForSupportingDocuments" || callerId == "#CgoAllowedFileTypesForSupportingDocuments" || callerId == "#MiscAllowedFileTypesForSupportingDocuments" || callerId == "#UatpAllowedFileTypesForSupportingDocuments") {
          if (($(callerId + 'FutureValue').val()) == "") {
            $('#futureTextValue').watermark(".doc,.xls etc.");
            $('#futureTextValue').val(".doc,.xls etc.");

          }
          else
            $('#futureTextValue').val($(callerId + 'FutureValue').val());
        }
        else {
          $.watermark.hide("#futureTextValue");
          $('#futureTextValue').watermark('');
          $('#futureTextValue').val($(callerId + 'FutureValue').val());
        }

        // Assign current value field attributes to future text value field
        $('#futureTextValue').removeClass('currentFieldValue');

        $('.textField, #currentTextValue, #futureTextValue').show();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();

      }
      else if (fieldType == 2) {

        if (callerId == '#IsMerged') {

          $('#currentIsMergedValue').prop('checked', $(callerId).prop('checked'));

          if ($(callerId).prop('checked')) {
            $('#FutureIsMergedValue').prop('checked', false);
          }
          else {
            $('#FutureIsMergedValue').prop('checked', true);
          }

          //Populate Effective Merger Date 
          $('#currentMergerFromValue').val($('#ActualMergerDate').val());
          $('#futureMergerFromValue').val($('#ActualMergerDateFutureValue').val());

          // Populate Member field value
          $('#MemberTextValue').val($('#ParentMemberIdDisplayValue').val());
          $('#MemberTextFutureValue').val($('#ParentMemberIdFutureDisplayValue').val());
          $('#MemberIdFutureValue').val($('#ParentMemberIdFutureValue').val());

          //show Member List and Effective Merger Date
          $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').show();
          $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
          $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').show();
          $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').show();
        }
        else {
          $('#currentCheckboxValue').prop('checked', $(callerId).prop('checked'));

          if ($(callerId).prop('checked'))
            $('#checkboxFutureValue').prop('checked', false);
          else
            $('#checkboxFutureValue').prop('checked', true);

          $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
          $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').show();
          $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
          $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        }
        //Assign current value field attributes to checkboxFutureValue field                
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();

      }
      else if (fieldType == 4) {
        $('#currentAutoSubdivisionValue').val($(callerId).val());
        $('#AutoSubdivisionFutureValue').val($(callerId + 'FutureValue').val());

        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').show();
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();
      }


      else if (fieldType == 5) {
        $('#currentTextAreaValue').val($(callerId).val());
        $('#futureTextAreaValue').val($(callerId + 'FutureValue').val());

        //Assign current value field attributes to futuretextvalue field 
        $('#futureTextAreaValue').attr({ 'class': classattr, 'maxLength': maxLengthattr });
        $('#futureTextAreaValue').removeClass('currentFieldValue');


        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').show();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();
      }
      else if (fieldType == 6) {
        $('#currentAutoSponsoredbyTextValue').val($(callerId).val());
        $('#AutoSponsoredbyTextFutureValue').val($(callerId + 'FutureValue').val());
        $('#AutoSponsoredIdFutureValue').val($(callerId + 'FutureValueId').val());

        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').show();
        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();
      }
      else if (fieldType == 7) {
        $('#currentAutoAggregatedbyTextValue').val($(callerId).val());
        $('#AutoAggregatedbyTextFutureValue').val($(callerId + 'FutureValue').val());
        $('#AutoAggregatedIdFutureValue').val($(callerId + 'FutureValueId').val());

        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').show();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();
      }
      else if ((fieldType == 8) || (fieldType == 9) || (fieldType == 10) || (fieldType == 11) || (fieldType == 12) || (fieldType == 13) || (fieldType == 14) || (fieldType == 15) || (fieldType == 16) || (fieldType == 17) || (fieldType == 18)) {
        $("#currentDropdownValue").remove();
        $("#dropdownFutureValue").remove();
        $('br').remove();
        $('<br />').appendTo('.dropdownCurrentValue');
        $('<br />').appendTo('.dropdownNewValue');
        $(callerId).clone().attr('id', 'currentDropdownValue').appendTo('.dropdownCurrentValue');
        $(callerId).removeAttr('disabled');
        $(callerId).clone().attr('id', 'dropdownFutureValue').appendTo('.dropdownNewValue');
        $(callerId).attr('disabled', 'disabled');

        if ((fieldType == 16) || (fieldType == 17) || (fieldType == 18)) {
          if (fieldType == 16) $('#currentDropdownValue').val($('#hdnIchMembershipStatus').val());
          if (fieldType == 17) $('#currentDropdownValue').val($('#hdnAchMembershipStatus').val());
          if (fieldType == 18) $('#currentDropdownValue').val($('#hdnIsMembershipStatus ').val());

          $("#dropdownFutureValue option:contains('Terminated')").attr('selected', 'selected');
          //Disable Current and Future value fields for membership status fields
          $('#currentDropdownValue').attr('disabled', 'disabled');
          $('#dropdownFutureValue').attr('disabled', 'disabled');
        }
        else {
          $('#currentDropdownValue').val($(callerId).val());
          $('#dropdownFutureValue').val($(callerId + 'FutureValue').val());
        }
        $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').show();
        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        //CMP#689: Flexible CH Activation Options
        $('#secondryTitle').hide();
        $('.radioButton, #changeTimingRB').hide();
      }
      /////////////////////////////CMP#689: Flexible CH Activation Options//Start//////////////////////////////////////////////////////
      //here fieldType 19 and 20 is additional case of 16 and 17
      else if (fieldType == 19 || fieldType == 20) {

        //ByDefault set the first radio button selected (i.e. Immediate Option)
        $("input[name=changeTimingRB][value=2]:radio").prop("checked", true);

        displayDefaultFutureUpdateInputScreen(callerId, fieldType);


        // On radio button selection change event perfom the further functinality
        $("input[name=changeTimingRB]:radio").on("change", function () {

          if ($(this).val() == "2") { // If Future Period option selected from radio button do the following :

            displayDefaultFutureUpdateInputScreen(callerId, fieldType);

          } else {// If Immediate option selected from radio button do the following :

            //remove the current and future dropdown
            $("#currentDropdownValue").remove();
            $("#dropdownFutureValue").remove();

            //hide the all other field other then radio button
            $('#secondryTitle').hide();
            $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').hide();
            $('#futurePeriodContainer').hide();

          }
        });


        $('.AutoAggregatedField, #currentAutoAggregatedbyTextValue, #AutoAggregatedbyTextFutureValue').hide();
        $('.AutoSponsoredField, #currentAutoSponsoredbyTextValue, #AutoSponsoredbyTextFutureValue').hide();
        $('.AutoSubdivisionField, #currentAutoSubdivisionValue, #AutoSubdivisionFutureValue').hide();
        $('.textField, #currentTextValue, #futureTextValue').hide();
        $('.checkboxField, #currentCheckboxValue, #checkboxFutureValue').hide();
        $('.textAreaField, #currentTextAreaValue, #futureTextAreaValue').hide();
        $('.statusField, #currentStatusValue, #futureStatusValue').hide();
        $('.MemberListField, #MemberTextValue, #MemberTextFutureValue').hide();
        $('.textFieldMergerFrom, #currentMergerFromValue, #futureMergerFromValue').hide();
        $('.checkboxIsMerged, #currentIsMergedValue, #FutureIsMergedValue').hide();
        $('.radioButton, #changeTimingRB').show();
      }
      /////////////////////////////CMP#689: Flexible CH Activation Options//End//////////////////////////////////////////////////////

      // Conditionally show the period or the date picker.
      if (hasPeriod) {
        $('#futurePeriodContainer').show();
        if ($(callerId + 'FuturePeriod').val())
          $('#futurePeriod').val($(callerId + 'FuturePeriod').val());
        else
          $('#futurePeriod').val($("#nextPeriod").val())

        $('#futurePeriod').watermark(_periodFormat);
        $('#futureDateContainer').hide();
      }
      else {
        $("#futureDate").datepicker("option", "minDate", +1);
        $('#futureDateContainer').show();

        if ($(callerId + 'FutureDate').val() != "") {
          $('#futureDate').val($(callerId + 'FutureDate').val());
        }
        else {
          $('#futureDate').val("");
          $("#futureDate").watermark(_dateWatermark);
        }


        $('#futurePeriodContainer').hide();
      }
    },//TFS#9959 - IE:Version 11: Future Date Calendar Pop up is not opening.
    close: function () { 
            $(this).dialog("destroy"); 
        }
  });

  return false;
}

///CMP#689: Flexible CH Activation Options//Start
// This function is used for fieldtype 19 & 20 only to display additional case of fieldtype 16 & 17
function displayDefaultFutureUpdateInputScreen(callerId, fieldType) {

  //refresh the page control
  $("#currentDropdownValue").remove();
  $("#dropdownFutureValue").remove();
  $('br').remove();
  $('<br />').appendTo('.dropdownCurrentValue');
  $('<br />').appendTo('.dropdownNewValue');
  $(callerId).clone().attr('id', 'currentDropdownValue').appendTo('.dropdownCurrentValue');
  $(callerId).removeAttr('disabled');
  $(callerId).clone().attr('id', 'dropdownFutureValue').appendTo('.dropdownNewValue');
  $(callerId).attr('disabled', 'disabled');

  // show the secondary title, current and future dropdown textbox and Future period textbox
  $('#secondryTitle').show();
  $('.dropdownField, #currentDropdownValue, #dropdownFutureValue').show();
  $('#futurePeriodContainer').show();

  //populate the future period value
  if ($(callerId + 'FuturePeriod').val())
    $('#futurePeriod').val($(callerId + 'FuturePeriod').val());
  else
    $('#futurePeriod').val($("#nextPeriod").val())

  $('#futurePeriod').watermark(_periodFormat);


  //populate the currentdropdown value from hidden field
  if (fieldType == 19) $('#currentDropdownValue').val($('#hdnIchMembershipStatus').val());
  if (fieldType == 20) $('#currentDropdownValue').val($('#hdnAchMembershipStatus').val());

  //populate the futuredropdown value from callerId future value
  $('#dropdownFutureValue').val($(callerId + 'FutureValue').val());
  //Disable Current and Future value fields for membership status fields
  $('#currentDropdownValue').attr('disabled', 'disabled');
  $('#dropdownFutureValue').attr('disabled', 'disabled');
}

function browse() {
  $('#divBrowse').dialog({ title: 'Browse', height: 100, width: 400, modal: true, resizable: false });
}

function GetMembervalidation() {

  $('#clientSuccessMessageContainer').hide();
  $("#FetchMember").validate({
    rules: {
      DisplayCommercialName: "required"
    },
    messages: {
      DisplayCommercialName: "Member is required"
    }
  });
}

function onValidationFailed() {
  $.watermark.showAll();
  $('#errorContainer').show();
  $('#clientErrorMessageContainer').hide();
  $('#clientSuccessMessageContainer').hide();
}

function initMemberDetailsTabValidations() {
    $("#divPostdatedPeriod").hide();

  if (($("#memberTerminationDate").val() != "") && ($("#memberTerminationDate").val() != _dateWatermark)) {
    $('#memberTerminationDate').datepicker('disable');
  }

  $("#statusHistory").appendTo('#divIsMembershipStatus');

  $("#MemberDetails").validate({
    rules: {
        MemberCodeAlpha: { required: true, allowedCharacters: true },
      MemberCodeNumeric: { required: true, allowedCharacters: true, minlength: 3, maxlength: 12, minMemPrefix: true },
      LegalName: { required: true, allowedCharacters: true },
      CommercialName: { required: true, allowedCharacters: true },
      IsMembershipStatusId: "required",
      "DefaultLocation.CountryId": "required",
      "DefaultLocation.AddressLine1": { required: true, allowedCharacters: true },
      "DefaultLocation.AddressLine2": { allowedCharacters: true },
      "DefaultLocation.AddressLine3": { allowedCharacters: true },
      "DefaultLocation.PostalCode": { allowedCharacters: true },
      "DefaultLocation.CityName": {
        required: true,
        minlength: 3,
        allowedCharacters: true
      },
      "DefaultLocation.RegistrationId": { allowedCharacters: true },
      "DefaultLocation.TaxVatRegistrationNumber": { allowedCharacters: true },
      "DefaultLocation.AdditionalTaxVatRegistrationNumber": { allowedCharacters: true },
      "DefaultLocation.BankAccountName": { allowedCharacters: true },
      "DefaultLocation.BankAccountNumber": { allowedCharacters: true },
      "DefaultLocation.BankName": { allowedCharacters: true },
      "DefaultLocation.BranchCode": { allowedCharacters: true },
      "DefaultLocation.BankCode": { allowedCharacters: true },
      "DefaultLocation.Iban": { allowedCharacters: true },
      "DefaultLocation.Swift": { allowedCharacters: true },
      EntryDate: {
        required: function (element) {
          var value = $("#IsMembershipStatusId").val();
          if (value == 1) {
            return true;
          }
          else {
            return false;
          }
        }
      },
      TerminationDate: {
        required: function (element) {
          var value = $("#IsMembershipStatusId").val();
          if (value == 4) {
            return true;
          } else {
            return false;
          }
        }
},
      IsMembershipSubStatusId : "required",
      "DefaultLocation.Iban": {
        BankDetails: ["DefaultLocation_Iban"]
      },
      "DefaultLocation.Swift": {
        BankDetails: ["DefaultLocation_Swift"]
      },
      "DefaultLocation.BankAccountNumber": {
        BankDetails: ["DefaultLocation_BankAccountNumber"]
      }
    },
    messages: {
      MemberCodeAlpha: "Member Code Alpha required(at least 1 alphabet)",
      MemberCodeNumeric: {
        required: "Member Code Numeric required",
        minlength: "Field is mandatory with a minimum length of 3 and maximum length of 12",
        maxlength: "Field is mandatory with a minimum length of 3 and maximum length of 12"
      },
      LegalName: "Member Legal Name required",
      CommercialName: "Member Commercial Name required",
      IsMembershipStatusId: "IS Membership Status required",
      "DefaultLocation.CountryId": "Country Name required",
      "DefaultLocation.CityName": "City Name required(Minimum 3 chars)",
      "DefaultLocation.AddressLine1": "Address Line 1 required",
      EntryDate: "Entry Date required",
      TerminationDate: "Termination Date required",
      IsMembershipSubStatusId: "IS Membership Sub Status required",
      "DefaultLocation.Iban": "Invalid IBAN Value",
      "DefaultLocation.Swift": "Invalid SWIFT Value",
      "DefaultLocation.BankAccountNumber": "Invalid Bank Account Number Value"


    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });
  $("#IsOpsComments").bind("keypress", function () { maxLength(this, 500) });
  $("#IsOpsComments").bind("paste", function () { maxLengthPaste(this, 500) });
  $("#memberLogoUpload").val("");
}

function initLocationsTabValidations() {
  locationValidator = $("#location").validate({
    rules: {
      MemberLegalName: { required: true, allowedCharacters: true },
      MemberCommercialName: { required: true, allowedCharacters: true },
      CommercialName: { required: true, allowedCharacters: true },
      CountryId: "required",
      AddressLine1: { required: true, allowedCharacters: true },
      RegistrationId: { allowedCharacters: true },
      TaxVatRegistrationNumber: { allowedCharacters: true },
      AdditionalTaxVatRegistrationNumber: { allowedCharacters: true },
      AddressLine2: { allowedCharacters: true },
      AddressLine3: { allowedCharacters: true },
      PostalCode: { allowedCharacters: true },
      CityName: { required: true, minlength: 3, allowedCharacters: true },
      BankAccountName: { allowedCharacters: true },
      BankAccountNumber: { allowedCharacters: true },
      BankName: { allowedCharacters: true },
      BranchCode: { allowedCharacters: true },
      BankCode: { allowedCharacters: true },
      Iban: { allowedCharacters: true },
      Swift: { allowedCharacters: true },
      LegalText: { allowedCharactersForTextAreaFields: true }


    },
    messages: {
      MemberLegalName: "Member Legal Name required",
      MemberCommercialName: "Member Commercial Name required",
      CommercialName: "Commercial Name required",
      CountryId: "Country Name required",
      AddressLine1: "Address Line 1 required",
      CityName: "City Name required(Minimum 3 chars)"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });
  $("#LegalText").bind("keypress", function () { maxLength(this, 700) });
  $("#LegalText").bind("paste", function () { maxLengthPaste(this, 700) });
}

function initContactsTabValidations() {

  if ($('#contactId').val() == "") {
    $('#conIsActive').attr({ "checked": "checked" });
  }

  if (($("#UserCategory").val() != 'IchOps') && ($("#UserCategory").val() != 'AchOps')) {
    $("#conIsActive").removeAttr('disabled');
  }

  contactValidator = $("#contacts").validate({
    rules: {
      FirstName: { required: true, allowedCharacters: true },
      EmailAddress:
      {
        required: true,
        email: true,
        allowedCharacters: true
      }
    },
    messages: {
      FirstName: "First Name required",
      EmailAddress: "Invalid Email address"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });
}

function initEBillingTabValidations() {
  $("#eBilling").validate({
    rules: {
      PayableLegalArchievingPeriod: {
        required: function (element) {
            var value = $("#IsPayableLegalArchievingOptional").prop('checked');
          if (value == true) {
            return true;
          } else {
            return false;
          }

        },
        LegalText: { allowedCharacters: true }

      },
      RecievableLegalArchievingPeriod: {
        required: function (element) {
            var value = $("#IsRecievableLegalArchievingOptional").prop('checked');
          if (value == true) {
            return true;
          } else {
            return false;
          }
        }
      }
    },
    messages: {
      PayableLegalArchievingPeriod: "Invalid Legal Archiving Period - payables",
      RecievableLegalArchievingPeriod: "Invalid Legal Archiving Period - receivables"
    },
    submitHandler: saveMemberData,
    invalidHandler: onValidationFailed
  });

  $("#LegalText").bind("keypress", function () { maxLength(this, 700) });
  $("#LegalText").bind("paste", function () { maxLengthPaste(this, 700) });

  var IsLegalArchievingRequired = $("#IsLegalArchievingRequired").val();
  if (IsLegalArchievingRequired == 'True') {
    $('#IsPayableLegalArchievingOptional').removeAttr('disabled');
    $('#IsRecievableLegalArchievingOptional').removeAttr('disabled');
    $('#PayableLegalArchievingPeriod').removeAttr('readOnly');
    $('#RecievableLegalArchievingPeriod').removeAttr('readOnly');
  }
}

function initPassengerTabValidations() {
  $("#PaxAllowedFileTypesForSupportingDocuments").watermark(".doc,.xls etc.");
  $("#NonSamplePrimeBillingIsIdecMigratedDate").watermark(_periodFormat);
  $("#NonSamplePrimeBillingIsxmlMigratedDate").watermark(_periodFormat);
  $("#SamplingProvIsIdecMigratedDate").watermark(_periodFormat);
  $("#SamplingProvIsxmlMigratedDate").watermark(_periodFormat);
  $("#NonSampleRmIsIdecMigratedDate").watermark(_periodFormat);
  $("#NonSampleRmIsXmlMigratedDate").watermark(_periodFormat);
  $("#NonSampleBmIsIdecMigratedDate").watermark(_periodFormat);
  $("#NonSampleBmIsXmlMigratedDate").watermark(_periodFormat);
  $("#NonSampleCmIsIdecMigratedDate").watermark(_periodFormat);
  $("#NonSampleCmIsXmlMigratedDate").watermark(_periodFormat);
  $("#SampleFormCIsIdecMigratedDate").watermark(_periodFormat);
  $("#SampleFormCIsxmlMigratedDate").watermark(_periodFormat);
  $("#SampleFormDeIsIdecMigratedDate").watermark(_periodFormat);
  $("#SampleFormDeIsxmlMigratedDate").watermark(_periodFormat);
  $("#SampleFormFxfIsIdecMigratedDate").watermark(_periodFormat);
  $("#SampleFormFxfIsxmlMigratedDate").watermark(_periodFormat);

  $('#NonSamplePrimeBillingIswebMigratedDate').watermark(_periodFormat);
  $('#SamplingProvIswebMigratedDate').watermark(_periodFormat);
  $('#NonSampleRmIswebMigratedDate').watermark(_periodFormat);
  $('#NonSampleBmIswebMigratedDate').watermark(_periodFormat);
  $('#NonSampleCmIswebMigratedDate').watermark(_periodFormat);
  $('#SampleFormCIswebMigratedDate').watermark(_periodFormat);
  $('#SampleFormDeIswebMigratedDate').watermark(_periodFormat);
  $('#SampleFormFxfIswebMigratedDate').watermark(_periodFormat);
  $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').datepicker('disable');
  $('#PaxSamplingProvIsIdecCerfifiedOn').datepicker('disable');
  $('#PaxSamplingProvIsxmlCertifiedOn').datepicker('disable');
  $('#PaxNonSampleRmIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSampleRmIsXmlCertifiedOn').datepicker('disable');
  $('#PaxNonSampleBmIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSampleBmIsxmlCertifiedOn').datepicker('disable');
  $('#PaxNonSampleCmIsIdecCertifiedOn').datepicker('disable');
  $('#PaxNonSampleCmIsXmlCertifiedOn').datepicker('disable');
  $('#PaxSampleFormCIsIdecCertifiedOn').datepicker('disable');
  $('#PaxSampleFormCIsxmlCertifiedOn').datepicker('disable');
  $('#PaxSampleFormDeIsIdecCertifiedOn').datepicker('disable');
  $('#PaxSampleFormDeIsxmlCertifiedOn').datepicker('disable');
  $('#PaxSampleFormFxfIsIdecCertifiedOn').datepicker('disable');
  $('#PaxSampleFormFxfIsxmlCertifiedOn').datepicker('disable');

  $('#NonSamplePrimeBillingIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#NonSamplePrimeBillingIsxmlMigratedDate').attr('disabled', 'disabled');
  $('#SamplingProvIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#SamplingProvIsxmlMigratedDate').attr('disabled', 'disabled');
  $('#NonSampleRmIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#NonSampleRmIsXmlMigratedDate').attr('disabled', 'disabled');
  $('#NonSampleBmIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#NonSampleBmIsXmlMigratedDate').attr('disabled', 'disabled');
  $('#NonSampleCmIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#NonSampleCmIsXmlMigratedDate').attr('disabled', 'disabled');
  $('#SampleFormCIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#SampleFormCIsxmlMigratedDate').attr('disabled', 'disabled');
  $('#SampleFormDeIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#SampleFormDeIsxmlMigratedDate').attr('disabled', 'disabled');
  $('#SampleFormFxfIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#SampleFormFxfIsxmlMigratedDate').attr('disabled', 'disabled');

  if (($("#IsParticipateInValueDetermination").val() == 'True') && ($("#paxMemberId").val() != "0")) {
    $("#IsParticipateInAutoBilling").attr('disabled', 'disabled');
    $(".autoBillingEditLink").show();
  }
  else {
    $(".autoBillingEditLink").hide();
  }

  if ($("#PaxOldIdecMember").val() == 'True') {
    $('#DownConvertISTranToOldIdec').removeAttr('disabled');
  }
  else {
    $('#DownConvertISTranToOldIdec').attr('disabled', 'disabled');
  }


  if (($("#SamplingCareerTypeId").val() == "1") && ($("#paxMemberId").val() != "0")) {
    $(".provBillingFileEditLink").hide();
    $("#IsConsolidatedProvisionalBillingFileRequiredFutureDateInd").hide();
  }
  else if (($("#SamplingCareerTypeId").val() != "1") && ($("#paxMemberId").val() != "0")) {
    $(".provBillingFileEditLink").show();
  }

  setPaxMigrationfields();

  $("#pax").validate({
      rules: {
          NonSamplePrimeBillingIsIdecMigratedDate: {
              billingPeriod: ["Migration", "NonSamplePrimeBillingIsIdecMigrationStatusId"]
          },
          NonSamplePrimeBillingIsxmlMigratedDate: {
              billingPeriod: ["Migration", "NonSamplePrimeBillingIsxmlMigrationStatusId"]
          },
          SamplingProvIsIdecMigratedDate: {
              billingPeriod: ["Migration", "SamplingProvIsIdecMigrationStatusId"]
          },
          SamplingProvIsxmlMigratedDate: {
              billingPeriod: ["Migration", "SamplingProvIsxmlMigrationStatusId"]
          },
          NonSampleRmIsXmlMigratedDate: {
              billingPeriod: ["Migration", "NonSampleRmIsXmlMigrationStatusId"]
          },
          NonSampleRmIsIdecMigratedDate: {
              billingPeriod: ["Migration", "NonSampleRmIsIdecMigrationStatusId"]
          },
          NonSampleBmIsIdecMigratedDate: {
              billingPeriod: ["Migration", "NonSampleBmIsIdecMigrationStatusId"]
          },
          NonSampleBmIsXmlMigratedDate: {
              billingPeriod: ["Migration", "NonSampleBmIsXmlMigrationStatusId"]
          },
          NonSampleCmIsIdecMigratedDate: {
              billingPeriod: ["Migration", "NonSampleCmIsIdecMigrationStatusId"]
          },
          NonSampleCmIsXmlMigratedDate: {
              billingPeriod: ["Migration", "NonSampleCmIsXmlMigrationStatusId"]
          },
          SampleFormCIsIdecMigratedDate: {
              billingPeriod: ["Migration", "SampleFormCIsIdecMigrationStatusId"]
          },
          SampleFormCIsxmlMigratedDate: {
              billingPeriod: ["Migration", "SampleFormCIsxmlMigrationStatusId"]
          },
          SampleFormDeIsIdecMigratedDate: {
              billingPeriod: ["Migration", "SampleFormDeIsIdecMigrationStatusId"]
          },
          SampleFormDeIsxmlMigratedDate: {
              billingPeriod: ["Migration", "SampleFormDEisxmlMigrationStatusId"]
          },
          SampleFormFxfIsIdecMigratedDate: {
              billingPeriod: ["Migration", "SampleFormFxfIsIdecMigrationStatusId"]
          },
          SampleFormFxfIsxmlMigratedDate: {
              billingPeriod: ["Migration", "SampleFormFxfIsxmlMigratedStatusId"]
          },

          NonSamplePrimeBillingIswebMigratedDate: {
              billingPeriod: ["WebMigration", "NonSamplePrimeBillingIswebMigratedDate"]
          },
          SamplingProvIswebMigratedDate: {
              billingPeriod: ["WebMigration", "SamplingProvIswebMigratedDate"]
          },
          NonSampleRmIswebMigratedDate: {
              billingPeriod: ["WebMigration", "NonSampleRmIswebMigratedDate"]
          },
          NonSampleBmIswebMigratedDate: {
              billingPeriod: ["WebMigration", "NonSampleBmIswebMigratedDate"]
          },
          NonSampleCmIswebMigratedDate: {
              billingPeriod: ["WebMigration", "NonSampleCmIswebMigratedDate"]
          },
          SampleFormCIswebMigratedDate: {
              billingPeriod: ["WebMigration", "SampleFormCIswebMigratedDate"]
          },
          SampleFormDeIswebMigratedDate: {
              billingPeriod: ["WebMigration", "SampleFormDeIswebMigratedDate"]
          },
          SampleFormFxfIswebMigratedDate: {
              billingPeriod: ["WebMigration", "SampleFormFxfIswebMigratedDate"]
          },
          NonSamplePrimeBillingIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSamplePrimeBillingIsIdecMigrationStatusId");
              }
          },
          NonSamplePrimeBillingIsxmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSamplePrimeBillingIsxmlMigrationStatusId");
              }
          },
          SamplingProvIsIdecCerfifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SamplingProvIsIdecMigrationStatusId");
              }
          },
          SamplingProvIsxmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SamplingProvIsxmlMigrationStatusId");
              }
          },

          NonSampleRmIsXmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSampleRmIsXmlMigrationStatusId");
              }
          },
          NonSampleRmIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSampleRmIsIdecMigrationStatusId");
              }
          },
          NonSampleBmIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSampleBmIsIdecMigrationStatusId");
              }
          },
          NonSampleBmIsxmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSampleBmIsXmlMigrationStatusId");
              }
          },
          NonSampleCmIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSampleCmIsIdecMigrationStatusId");
              }
          },
          NonSampleCmIsXmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("NonSampleCmIsXmlMigrationStatusId");
              }
          },
          SampleFormCIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SampleFormCIsIdecMigrationStatusId");
              }
          },
          SampleFormCIsxmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SampleFormCIsxmlMigrationStatusId");
              }
          },
          SampleFormDeIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SampleFormDeIsIdecMigrationStatusId");
              }
          },
          SampleFormDeIsxmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SampleFormDEisxmlMigrationStatusId");
              }
          },
          SampleFormFxfIsIdecCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SampleFormFxfIsIdecMigrationStatusId");
              }
          },
          SampleFormFxfIsxmlCertifiedOn: {
              required: function (element) {
                  return isMigrationDateValid("SampleFormFxfIsxmlMigratedStatusId");
              }
          },
          ListingCurrencyId: {
              required: function (element) {
                  return isValidCurrency("IsParticipateInAutoBilling");
              }
          },
          ListingCurrencyIdFutureValue: {
              required: function (element) {                 
//                  var isValueDet = $("#IsParticipateInValueDetermination").val();
//                  var isParticipate = $("#IsParticipateInAutoBilling").val();
//                  var isParti = $("#IsParticipateInAutoBilling").attr('checked');
//                  var isParticipate = $("#IsParticipateInAutoBillingFutureValue").val();
                  var old = $("#ListingCurrencyId").val();
                  var New = $("#ListingCurrencyIdFutureValue").val();

                  //SCP221813 - Auto Billing issue.
                  if (($("#IsParticipateInAutoBilling").prop('checked')) || ($("#IsParticipateInAutoBillingFutureValue").val().toLowerCase() == 'true')) {
                      if ((old == '' && New == '')) {
                          return true;
                      } else {
                          return false;
                      }
                  } else {
                      return false;
                  }

              }
          },
          PaxAllowedFileTypesForSupportingDocuments: "allowedFileType",
          InvoiceNumberRangePrefix: {
              required: function (element) {
                  if (($("#IsParticipateInAutoBilling").prop('checked')) || ($("#IsParticipateInAutoBillingFutureValue").val() == 'True') || ($("#IsParticipateInAutoBillingFutureValue").val() == 'true')) {
                      return true;
                  } else {
                      return false;
                  }
              }
          },

          CutOffTime: {
              required: function (element) {
                  if (($("#IsParticipateInAutoBilling").prop('checked')) || ($("#IsParticipateInAutoBillingFutureValue").val() == 'True') || ($("#IsParticipateInAutoBillingFutureValue").val() == 'true')) {
                      return true;
                  } else {
                      return false;
                  }
              }
          },

          InvoiceNumberRangeFrom: {
              invoiceNumber: ["InvoiceNumberRangePrefix", "InvoiceNumberRangeFrom", "IsParticipateInAutoBilling", "IsParticipateInAutoBillingFutureValue"]
          },

          InvoiceNumberRangeTo: {
              invoiceNumber: ["InvoiceNumberRangePrefix", "InvoiceNumberRangeTo", "IsParticipateInAutoBilling", "IsParticipateInAutoBillingFutureValue", "InvoiceNumberRangeFrom"]
          }
      },
      messages: {
          ListingCurrencyId: "Listing Currency required",
          ListingCurrencyIdFutureValue: "Listing Currency required",
          InvoiceNumberRangeFrom: "Invoice Number Range - From required when auto billing enabled and Length should not be greater than 10",
          InvoiceNumberRangeTo: "Invoice Number Range - To required when auto billing enabled and Length should not be greater than 10 and Should be greater than 'Invoice Number Range From'",
          InvoiceNumberRangePrefix: "Invoice Number Prefix required when auto billing enabled",
          NonSamplePrimeBillingIsIdecMigratedDate: "Invalid Prime Billing IS-IDEC Migration Period",
          NonSamplePrimeBillingIsxmlMigratedDate: "Invalide Prime Billing IS-XML Migration Period",
          SamplingProvIsIdecMigratedDate: "Invalid Sampling Prov IS-IDEC Migration Period",
          SamplingProvIsxmlMigratedDate: "Invalid Sampling Prov IS-XML Migration Period",
          NonSampleRmIsXmlMigratedDate: "Invalid RM IS-XML Migration Period",
          NonSampleRmIsIdecMigratedDate: "Invalid RM IS-IDEC Migration Period",
          NonSampleBmIsIdecMigratedDate: "Invalid BM IS-IDEC Migration Period",
          NonSampleBmIsXmlMigratedDate: "Invalid BM IS-XML Migration Period",
          NonSampleCmIsIdecMigratedDate: "Invalid CM IS-IDEC Migration Period",
          NonSampleCmIsXmlMigratedDate: "Invalid CM IS-XML Migration Period",
          SampleFormCIsIdecMigratedDate: "Invalid Sampling form C IS-IDEC Migration Period",
          SampleFormCIsxmlMigratedDate: "Invalid Sampling form C IS-XML Migration Period",
          SampleFormDeIsIdecMigratedDate: "Invalid Sampling form D/E IS-IDEC Migration Period",
          SampleFormDeIsxmlMigratedDate: "Invalid Sampling form D/E IS-XML Migration Period",
          SampleFormFxfIsIdecMigratedDate: "Invalid Sampling form F/X IS-IDEC Migration Period",
          SampleFormFxfIsxmlMigratedDate: "Invalid Sampling form F/X IS-XML Migration Period",
          NonSamplePrimeBillingIsIdecCertifiedOn: "Invalid Billing IS-IDEC Migration Date",
          NonSamplePrimeBillingIsxmlCertifiedOn: "Invalid Billing IS-XML Migration Date",
          SamplingProvIsIdecCerfifiedOn: "Invalid Sampling Prov IS-IDEC Migration Date",
          SamplingProvIsxmlCertifiedOn: "Invalid Sampling Prov IS-XML Migration Date",
          NonSampleRmIsXmlCertifiedOn: "Invalid RM IS-XML Migration Date",
          NonSampleRmIsIdecCertifiedOn: "Invalid RM IS-IDEC Migration Date",
          NonSampleBmIsIdecCertifiedOn: "Invalid BM IS-IDEC Migration Date",
          NonSampleBmIsxmlCertifiedOn: "Invalid BM IS-XML Migration Date",
          NonSampleCmIsIdecCertifiedOn: "Invalid CM IS-IDEC Migration Date",
          NonSampleCmIsXmlCertifiedOn: "Invalid CM IS-XML Migration Date",
          SampleFormCIsIdecCertifiedOn: "Invalid Sampling form C IS-IDEC Migration Date",
          SampleFormCIsxmlCertifiedOn: "Invalid Sampling form C IS-XML Migration Date",
          SampleFormDeIsIdecCertifiedOn: "Invalid Sampling form D/E IS-IDEC Migration Date",
          SampleFormDeIsxmlCertifiedOn: "Invalid Sampling dorm D/E IS-XML Migration Date",
          SampleFormFxfIsIdecCertifiedOn: "Invalid Sampling form F/X IS-IDEC Migration Date",
          SampleFormFxfIsxmlCertifiedOn: "Invalid Sampling form F/X IS-XML Migration Date",
          PaxAllowedFileTypesForSupportingDocuments: "Invalid File Type",
          NonSamplePrimeBillingIswebMigratedDate: "Invalid Prime Billing IS-WEB Migration Period",
          SamplingProvIswebMigratedDate: "Invalid Sampling Prov IS-WEB Migration Period",
          NonSampleRmIswebMigratedDate: "Invalid RM IS-WEB Migration Period",
          NonSampleBmIswebMigratedDate: "Invalid BM IS-WEB Migration Period",
          NonSampleCmIswebMigratedDate: "Invalid CM IS-WEB Migration Period",
          SampleFormCIswebMigratedDate: "Invalid Sampling form C IS-WEB Migration Period",
          SampleFormDeIswebMigratedDate: "Invalid Sampling form D/E IS-WEB Migration Period",
          SampleFormFxfIswebMigratedDate: "Invalid Sampling form F/X IS-WEB Migration Period"

      },
      submitHandler: saveMemberDataAlongWithContact,
      invalidHandler: onValidationFailed
  });

  enableDisableControlOnIndex("#SamplingCareerType", "#IsConsolidatedProvisionalBillingFileRequired");
  enableDisableControlOnChecking("#IsAutoBillingEnabled", "#InvoiceNumberRangePrefix", "#InvoiceNumberRangeFrom", "#InvoiceNumberRangeTo");

  function enableDisableControlOnIndex(dropdownIndexId, checkBoxId) {
    $(dropdownIndexId).change(function () {
      var value = $(dropdownIndexId).val();
      if (value != 1) {
        $(checkBoxId).attr({ "checked": "checked" });
      }
    });
  }
}

function initCgoTabValidations() {
  $("#CgoAllowedFileTypesForSupportingDocuments").watermark(".doc,.xls etc.");
  $("#PrimeBillingIsIdecMigratedDate").watermark(_periodFormat);
  $("#PrimeBillingIsxmlMigratedDate").watermark(_periodFormat);
  $("#RmIsIdecMigratedDate").watermark(_periodFormat);
  $("#RmIsXmlMigratedDate").watermark(_periodFormat);
  $("#BmIsIdecMigratedDate").watermark(_periodFormat);
  $("#BmIsXmlMigratedDate").watermark(_periodFormat);
  $("#CmIsIdecMigratedDate").watermark(_periodFormat);
  $("#CmIsXmlMigratedDate").watermark(_periodFormat);
  $('#PrimeBillingIswebMigratedDate').watermark(_periodFormat);
  $('#RmIswebMigratedDate').watermark(_periodFormat);
  $('#BmIswebMigratedDate').watermark(_periodFormat);
  $('#CmIswebMigratedDate').watermark(_periodFormat);

  $('#cgoPrimeBillingIsIdecCertifiedOn').datepicker('disable');
  $('#cgoPrimeBillingIsxmlCertifiedOn').datepicker('disable');
  $('#cgoRmIsIdecCertifiedOn').datepicker('disable');
  $('#cgoRmIsXmlCertifiedOn').datepicker('disable');
  $('#cgoBmIsIdecCertifiedOn').datepicker('disable');
  $('#cgoBmIsXmlCertifiedOn').datepicker('disable');
  $('#cgoCmIsIdecCertifiedOn').datepicker('disable');
  $('#cgoCmIsXmlCertifiedOn').datepicker('disable');

  $('#PrimeBillingIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#PrimeBillingIsxmlMigratedDate').attr('disabled', 'disabled');
  $('#RmIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#RmIsXmlMigratedDate').attr('disabled', 'disabled');
  $('#BmIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#BmIsXmlMigratedDate').attr('disabled', 'disabled');
  $('#CmIsIdecMigratedDate').attr('disabled', 'disabled');
  $('#CmIsXmlMigratedDate').attr('disabled', 'disabled');

  setCgoMigrationFields();

  if ($("#CgoOldIdecMember").val() == 'True') {
    $('#DownConvertISTranToOldIdeccgo').removeAttr('disabled');
  }
  else {
    $('#DownConvertISTranToOldIdeccgo').attr('disabled', 'disabled');
  }

  $("#cgo").validate({
    rules: {
      CgoAllowedFileTypesForSupportingDocuments: "allowedFileType",
      IsIdecOutputVersion: "required",
      PrimeBillingIsIdecMigratedDate: {
        billingPeriod: ["Migration", "PrimeBillingIsIdecMigrationStatusId"]
      },
      PrimeBillingIsxmlMigratedDate: {
        billingPeriod: ["Migration", "PrimeBillingIsxmlMigrationStatusId"]
      },
      RmIsIdecMigratedDate: {
        billingPeriod: ["Migration", "RmIsIdecMigrationStatusId"]
      },
      RmIsXmlMigratedDate: {
        billingPeriod: ["Migration", "RmIsXmlMigrationStatusId"]
      },
      BmIsIdecMigratedDate: {
        billingPeriod: ["Migration", "BmIsIdecMigrationStatusId"]
      },
      BmIsXmlMigratedDate: {
        billingPeriod: ["Migration", "BmIsXmlMigrationStatusId"]
      },
      CmIsIdecMigratedDate: {
        billingPeriod: ["Migration", "CmIsIdecMigrationStatusId"]
      },
      CmIsXmlMigratedDate: {
        billingPeriod: ["Migration", "CmIsXmlMigrationStatusId"]
      },
      PrimeBillingIswebMigratedDate: {
        billingPeriod: ["WebMigration", "PrimeBillingIswebMigratedDate"]
      },
      RmIswebMigratedDate: {
        billingPeriod: ["WebMigration", "RmIswebMigratedDate"]
      },
      BmIswebMigratedDate: {
        billingPeriod: ["WebMigration", "BmIswebMigratedDate"]
      },
      CmIswebMigratedDate: {
        billingPeriod: ["WebMigration", "CmIswebMigratedDate"]
      },
      PrimeBillingIsIdecCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("PrimeBillingIsIdecMigrationStatusId");
        }
      },
      PrimeBillingIsxmlCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("PrimeBillingIsxmlMigrationStatusId");
        }
      },
      RmIsIdecCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("RmIsIdecMigrationStatusId");
        }
      },
      RmIsXmlCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("RmIsXmlMigrationStatusId");
        }
      },
      BmIsIdecCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("BmIsIdecMigrationStatusId");
        }
      },
      BmIsXmlCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("BmIsXmlMigrationStatusId");
        }
      },
      CmIsIdecCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("CmIsIdecMigrationStatusId");
        }
      },
      CmIsXmlCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("CmIsXmlMigrationStatusId");
        }
      }
    },
    messages: {
      IsIdecOutputVersion: "IS-IDEC Output Version is required",
      PrimeBillingIsIdecMigratedDate: "Invalid Prime Billing IS-IDEC Migration Period",
      PrimeBillingIsxmlMigratedDate: "Invalid Prime Billing IS-XML Migration Period",
      RmIsIdecMigratedDate: "Invalid RM IS-IDEC Migration Period",
      RmIsXmlMigratedDate: "Invalid RM IS-XML Migration Period",
      BmIsIdecMigratedDate: "Invalid BM IS-IDEC Migration Period",
      BmIsXmlMigratedDate: "Invalid BM IS-XML Migration Period",
      CmIsIdecMigratedDate: "Invalid CM IS-IDEC Migration Period",
      CmIsXmlMigratedDate: "Invalid CM IS-XML Migration Period",
      PrimeBillingIsIdecCertifiedOn: "Invalid Prime Billing IS-IDEC Migration Date",
      PrimeBillingIsxmlCertifiedOn: "Invalid Prime Billing IS-XML Migration Date",
      RmIsIdecCertifiedOn: "Invalid RM IS-IDEC Migration Date",
      RmIsXmlCertifiedOn: "Invalid RM IS-XML Migration Date",
      BmIsIdecCertifiedOn: "Invalid BM IS-IDEC Migration Date",
      BmIsIdecCertifiedOn: "Invalid BM IS-XML Migration Date",
      CmIsIdecCertifiedOn: "Invalid CM Is-IDEC Migration Date",
      CmIsXmlCertifiedOn: "Invalid CM IS-XML Migration Date",
      CgoAllowedFileTypesForSupportingDocuments: "Invalid File Type",

      PrimeBillingIswebMigratedDate: "Invalid Prime Billing IS-WEB Migration Period",
      RmIswebMigratedDate: "Invalid RM IS-WEB Migration Period",
      BmIswebMigratedDate: "Invalid BM IS-WEB Migration Period",
      CmIswebMigratedDate: "Invalid CM IS-WEB Migration Period"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });

  /// Unused.
  function enableControlOnCheck(checkBoxId) {
    var indicatorChkBox = document.getElementById(checkBoxId);
    $(indicatorChkBox).attr({ "checked": "checked" });
  }
}

function initMiscTabValidations() {

  $("#BillingIsXmlMigrationDate").watermark(_periodFormat);
  $("#MiscAllowedFileTypesForSupportingDocuments").watermark(".doc,.xls etc.");
  $('#miscBillingIsXmlCertifiedOn').datepicker('disable');
  $('#BillingIsXmlMigrationDate').attr('disabled', 'disabled');
  $('#errorContainer').hide();
  $('#BillingIswebMigrationDate').watermark(_periodFormat);

  setMiscMigrationFields();

  $("#Misc").validate({
    rules: {
      MiscAllowedFileTypesForSupportingDocuments: "allowedFileType",
      BillingIsXmlMigrationDate: {
        billingPeriod: ["Migration", "BillingIsXmlMigrationStatusId"]
      },
      BillingIswebMigrationDate: {
        billingPeriod: ["WebMigration", "BillingIswebMigrationDate"]
      },

      BillingIsXmlCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("BillingIsXmlMigrationStatusId");
        }
      }
    },
    messages: {
      BillingIsXmlCertifiedOn: "Invalid Date When Billing IS-XML Migrated",
      BillingIsXmlMigrationDate: "Invalid Period When Billing IS-XML Migrated",
      BillingIswebMigrationDate: "Invalid Period When Billing IS-WEB Migrated",
      MiscAllowedFileTypesForSupportingDocuments: "Invalid File Type"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });

  /// Unused.
  function enableControlOnCheck(checkBoxId) {
    var indicatorChkBox = document.getElementById(checkBoxId);
    $(indicatorChkBox).attr({ "checked": "checked" });
  }

  /// Unused.
  function SetPageWaterMark() {
    $("#MiscAllowedFileTypesForSupportingDocuments").watermark(".doc,.xls etc.");
    $("#miscBillingIsXmlMigrationDate").watermark(_periodFormat);
  }
}

function initUatpTabValidations() {
  $("#UatpAllowedFileTypesForSupportingDocuments").watermark(".doc,.xls etc.");
  $("#BillingIsXmlMigrationDateUATP").watermark(_periodFormat);
  $('#uatpBillingIsXmlCertifiedOn').datepicker('disable');
  $('#BillingIsXmlMigrationDateUATP').attr('disabled', 'disabled');
  $("#BillingIswebMigrationDate").watermark(_periodFormat);
  $.watermark.showAll();
  setUatpMigrationFields();
  $("#uatp").validate({
    rules: {
      UatpAllowedFileTypesForSupportingDocuments: "allowedFileType",
      BillingIsXmlMigrationDateUATP: {
        billingPeriod: ["Migration", "BillingIsXmlMigrationStatusId"]
      },
      BillingIswebMigrationDate: {
        billingPeriod: ["WebMigration", "BillingIswebMigrationDate"]
      },
      BillingIsXmlCertifiedOn: {
        required: function (element) {
          return isMigrationDateValid("BillingIsXmlMigrationStatusId");
        }
      }
    },
    messages: {
      BillingIsXmlCertifiedOn: "Invalide Date When Billing IS-XML Migrated",
      BillingIsXmlMigrationDateUATP: "Invalid Period When Billing IS-XML Migrated",
      BillingIswebMigrationDate: "Invalid Period When Billing IS-WEB Migrated",
      UatpAllowedFileTypesForSupportingDocuments: "Invalid File Type"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });

  function enableDisableControlOnCheck(checkBoxId1, checkboxId2) {
      if ($(checkBoxId1).prop('checked') == true) {
      $(checkboxId2).attr({ "checked": "checked" });
    }
    else {
        $(checkboxId2).prop({ "checked": false });
    }
  }

  /// Unused.
  function SetPageWaterMark() {
    $("#errorContainer").hide();
    $("#UatpAllowedFileTypesForSupportingDocuments").watermark(".doc,.xls etc.");
  }

}

function initICHTabValidations() {
  $("#ichReinstatementPeriod").watermark(_periodFormat);
  $('#futurePeriod').watermark(_periodFormat);
  $('#aggFuturePeriod').watermark(_periodFormat);
  $('#spnFuturePeriod').watermark(_periodFormat);

  // Restrict user to enter only current as past values in entry date
  $("#ichEntryDate").datepicker("option", "maxDate", new Date());

  $("#ichStatusHistory").appendTo('#divIchMembershipStatus');

  if (($("#ichTerminationDate").val() != "") && ($("#ichTerminationDate").val() != _dateWatermark)) {
    $('#ichTerminationDate').datepicker('disable');
  }

  $("#Ich").validate({
    rules: {
      IchMemberShipStatusId: "required",
      IchCategoryId: "required",
      IchZoneId: "required",
      IchWebReportOptionsId: "required",
      AggregatedTypeId: {
        required: function (element) {
          var value = $("#AggregatedByText").val();

          if (value != "") {
            return true;
          } else {
            return false;
          }
        }
      },
      StatusChangedDate: {
        required: function (element) {
          var value = $("#IchMemberShipStatusId").val();

          if (value == 2) {
            return true;
          } else {
            return false;
          }
        }
      },
      DefaultSuspensionDate: {
        required: function (element) {
          var value = $("#IchMemberShipStatusId").val();
          if (value == 2) {
            return true;
          } else {
            return false;
          }
        }
      },
      EntryDate: {
        required: function (element) {
          //CMP-689-Flexible CH Activation Options
          // validation added for the entry date if ichmembership status changed from Not A Member/ Terminated to Live
          var value = $("#IchMemberShipStatusId").val();
          var futurevalue = $("#IchMemberShipStatusIdFutureValue").val();

          if ($('#IchMemberShipStatusId').val() == $('#hdnIchMembershipStatus').val()) {
            return false;
          }
          else if ($('#IchMemberShipStatusId').val() == 1 && $('#hdnIchMembershipStatus').val() != 2) {
            return true;
          }
          else if (futurevalue ==1 && (value == 3 || value == 4)) {
            return true;
          }
          else {
            return false;
          }
        }
      },
      TerminationDate: {
        required: function (element) {
          var value = $("#IchMemberShipStatusId").val();
          if (value == 3 && ($('#IchZoneId').val() == "")) {
            return true;
          } else {
            return false;
          }
        }
      },
      ReinstatementPeriod: {
        billingPeriod: ["Reinstatement", "IchMemberShipStatusId", "hdnIchMembershipStatus", "ichReinstatementPeriod"]
      }
    },
    messages: {
      IchMemberShipStatusId: "Ich MemberShip status is required",
      IchCategoryId: "Ich Category is required",
      IchZoneId: "Ich Zone is required",
      IchWebReportOptionsId: "ICH WebReport options required",
      StatusChangedDate: "Suspension period From required",
      DefaultSuspensionDate: "Suspension Defaulting Period From required",
      EntryDate: "Entry Date required",
      ReinstatementPeriod: "Reinstatement Period should be valid('YYYY-MMM-PP')",
      TerminationDate: "Termination Date required",
      AggregatedTypeId: "Aggregated type Required"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });

  /// Unused.
  function datetimeCheck(element) {
    var datecheck = $(element).val();
    var arr_d1 = datecheck.split("-");
    var flag = false;
    day = arr_d1[0];
    month = arr_d1[1];
    year = arr_d1[2];
    var d = new Date();
    var curr_year = d.getFullYear();
    var periodarray = new Array("01", "02", "03", "04");
    var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC");

    for (var a = 0; a < periodarray.length; a++) {
      if (periodarray[a] == day) {
        for (var i = 0; i < monthArray.length; i++) {
          if (monthArray[i] == month.toUpperCase()) {
            if (year == curr_year) {
              flag = true;
            }
          }
        }
      }
    }

    if (flag == false) {
      alert("Please Enter valid Date.");
      return flag;
    }
  }
}

function initACHTabValidations() {
  $("#achReinstatementPeriod").watermark(_periodFormat);
  $("#achEntryDate").datepicker("option", "maxDate", new Date());

  if (($("#achTerminationDate").val() != "") && ($("#achTerminationDate").val() != _dateWatermark)) {
    $('#achTerminationDate').datepicker('disable');
  }

  $("#achStatusHistory").appendTo('#divAchMembershipStatus');

  $("#ach").validate({
    rules: {
      AchMembershipStatusId: "required",
      AchCategoryId: "required",

      StatusChangedDate: {
        required: function (element) {
          var value = $("#AchMembershipStatusId").val();

          if (value == 2) {
            return true;
          } else {
            return false;
          }
        }
      },
      DefaultSuspensionDate: {
        required: function (element) {
          var value = $("#AchMembershipStatusId").val();
          if (value == 2) {
            return true;
          } else {
            return false;
          }
        }
      },
      EntryDate: {
        required: function (element) {
          //CMP-689-Flexible CH Activation Options
          // validation added for the entry date if ichmembership status changed from Not A Member/ Terminated to Live
          var value = $("#AchMembershipStatusId").val();
          var futurevalue = $("#AchMembershipStatusIdFutureValue").val();

          if ($('#AchMembershipStatusId').val() == $('#hdnAchMembershipStatus').val()) {
            return false;
          }
          else if (value == 1 && ($('#hdnAchMembershipStatus').val() == 0 || $('#hdnAchMembershipStatus').val() == 3 || $('#hdnAchMembershipStatus').val() == 4) && $("#hdnAchReinstatement").val() == 0) {
            return true;
          }
          else if (futurevalue == 1 && (value == 3 || value == 4)) {
            return true;
          }
          else {
            return false;
          }
        }
      },
      TerminationDate: {
        required: function (element) {
          var value = $("#AchMembershipStatusId").val();
          if (value == 3 && $('#hdnAchMembershipStatus').val() == 0) {
            return true;
          } else {
            return false;
          }
        }
      },
      ReinstatementPeriod: {
        billingPeriod: ["Reinstatement", "AchMembershipStatusId", "hdnAchMembershipStatus", "achReinstatementPeriod"]

      }
    },
    messages: {
      AchMembershipStatusId: "Ach MemberShip status is required",
      AchCategoryId: "Ach Category is required",
      StatusChangedDate: "Invalid Suspension period from",
      DefaultSuspensionDate: "Invalid Suspension defaulting period from",
      EntryDate: "Invalid Entry date",
      TerminationDate: "Invalid Termination date",
      ReinstatementPeriod: "Reinstatement Period should be valid('YYYY-MMM-PP')"
    },
    submitHandler: saveMemberDataAlongWithContact,
    invalidHandler: onValidationFailed
  });

  var IchPax = $("#InterClearanceInvoiceSubmissionPatternPaxId").val();
  if (IchPax != null) {
    if (IchPax.length < 4) {
      var len = 4 - (IchPax.length);
      for (var i = 1; i <= len; i++) {
        IchPax = 0 + "" + IchPax;
      }
    }
    var letters = IchPax.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#IchPaxPeriod" + a)[0].checked = true;
      }
    }
  }

  var IchCgo = $("#InterClearanceInvoiceSubmissionPatternCgoId").val();
  if (IchCgo != null) {
    if (IchCgo.length < 4) {
      var len = 4 - (IchCgo.length);
      for (var i = 1; i <= len; i++) {
        IchCgo = 0 + "" + IchCgo;
      }

    }
    var letters = IchCgo.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#IchCgoPeriod" + a)[0].checked = true;
      }
    }
  }

  var IchMisc = $("#InterClearanceInvoiceSubmissionPatternMiscId").val();
  if (IchMisc != null) {
    if (IchMisc.length < 4) {
      var len = 4 - (IchMisc.length);
      for (var i = 1; i <= len; i++) {
        IchMisc = 0 + "" + IchMisc;
      }
    }
    var letters = IchMisc.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#IchMiscPeriod" + a)[0].checked = true;
      }
    }
  }

  var IchUatp = $("#InterClearanceInvoiceSubmissionPatternUatpId").val();
  if (IchUatp != null) {
    if (IchUatp.length < 4) {
      var len = 4 - (IchUatp.length);
      for (var i = 1; i <= len; i++) {
        IchUatp = 0 + "" + IchUatp;
      }
    }
    var letters = IchUatp.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#IchUatpPeriod" + a)[0].checked = true;
      }
    }
  }

  var AchPax = $("#AchClearanceInvoiceSubmissionPatternPaxId").val();
  if (AchPax != null) {
    if (AchPax.length < 4) {
      var len = 4 - (AchPax.length);
      for (var i = 1; i <= len; i++) {
        AchPax = 0 + "" + AchPax;
      }
    }
    var letters = AchPax.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#AchPaxPeriod" + a)[0].checked = true;
      }
    }
  }

  var AchCgo = $("#AchClearanceInvoiceSubmissionPatternCgoId").val();
  if (AchCgo != null) {
    if (AchCgo.length < 4) {
      var len = 4 - (AchCgo.length);
      for (var i = 1; i <= len; i++) {
        AchCgo = 0 + "" + AchCgo;
      }
    }
    var letters = AchCgo.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#AchCgoPeriod" + a)[0].checked = true;
      }
    }
  }

  var AchMisc = $("#AchClearanceInvoiceSubmissionPatternMiscId").val();
  if (AchMisc != null) {
    if (AchMisc.length < 4) {
      var len = 4 - (AchMisc.length);
      for (var i = 1; i <= len; i++) {
        AchMisc = 0 + "" + AchMisc;
      }
    }
    var letters = AchMisc.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#AchMiscPeriod" + a)[0].checked = true;
      }
    }
  }

  var AchUatp = $("#AchClearanceInvoiceSubmissionPatternUatpId").val();
  if (AchUatp != null) {
    if (AchUatp.length < 4) {
      var len = 4 - (AchUatp.length);
      for (var i = 1; i <= len; i++) {
        AchUatp = 0 + "" + AchUatp;
      }
    }
    var letters = AchUatp.split('');
    for (var i = 0; i < letters.length; i++) {
      if (letters[i] == "1") {
        var a = i + 1;
        $("#AchUatpPeriod" + a)[0].checked = true;
      }
    }
  }

  $("#IchPaxPeriod1").change(function () {
    GetValueofControlOnCheck("#IchPaxPeriod1", "#IchPaxPeriod2", "#IchPaxPeriod3", "#IchPaxPeriod4", "#InterClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#IchPaxPeriod2").change(function () {
    GetValueofControlOnCheck("#IchPaxPeriod1", "#IchPaxPeriod2", "#IchPaxPeriod3", "#IchPaxPeriod4", "#InterClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#IchPaxPeriod3").change(function () {
    GetValueofControlOnCheck("#IchPaxPeriod1", "#IchPaxPeriod2", "#IchPaxPeriod3", "#IchPaxPeriod4", "#InterClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#IchPaxPeriod4").change(function () {
    GetValueofControlOnCheck("#IchPaxPeriod1", "#IchPaxPeriod2", "#IchPaxPeriod3", "#IchPaxPeriod4", "#InterClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#IchCgoPeriod1").change(function () {
    GetValueofControlOnCheck("#IchCgoPeriod1", "#IchCgoPeriod2", "#IchCgoPeriod3", "#IchCgoPeriod4", "#InterClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#IchCgoPeriod2").change(function () {
    GetValueofControlOnCheck("#IchCgoPeriod1", "#IchCgoPeriod2", "#IchCgoPeriod3", "#IchCgoPeriod4", "#InterClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#IchCgoPeriod3").change(function () {
    GetValueofControlOnCheck("#IchCgoPeriod1", "#IchCgoPeriod2", "#IchCgoPeriod3", "#IchCgoPeriod4", "#InterClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#IchCgoPeriod4").change(function () {
    GetValueofControlOnCheck("#IchCgoPeriod1", "#IchCgoPeriod2", "#IchCgoPeriod3", "#IchCgoPeriod4", "#InterClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#IchMiscPeriod1").change(function () {
    GetValueofControlOnCheck("#IchMiscPeriod1", "#IchMiscPeriod2", "#IchMiscPeriod3", "#IchMiscPeriod4", "#InterClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#IchMiscPeriod2").change(function () {
    GetValueofControlOnCheck("#IchMiscPeriod1", "#IchMiscPeriod2", "#IchMiscPeriod3", "#IchMiscPeriod4", "#InterClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#IchMiscPeriod3").change(function () {
    GetValueofControlOnCheck("#IchMiscPeriod1", "#IchMiscPeriod2", "#IchMiscPeriod3", "#IchMiscPeriod4", "#InterClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#IchMiscPeriod4").change(function () {
    GetValueofControlOnCheck("#IchMiscPeriod1", "#IchMiscPeriod2", "#IchMiscPeriod3", "#IchMiscPeriod4", "#InterClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#IchUatpPeriod1").change(function () {
    GetValueofControlOnCheck("#IchUatpPeriod1", "#IchUatpPeriod2", "#IchUatpPeriod3", "#IchUatpPeriod4", "#InterClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#IchUatpPeriod2").change(function () {
    GetValueofControlOnCheck("#IchUatpPeriod1", "#IchUatpPeriod2", "#IchUatpPeriod3", "#IchUatpPeriod4", "#InterClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#IchUatpPeriod3").change(function () {
    GetValueofControlOnCheck("#IchUatpPeriod1", "#IchUatpPeriod2", "#IchUatpPeriod3", "#IchUatpPeriod4", "#InterClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#IchUatpPeriod4").change(function () {
    GetValueofControlOnCheck("#IchUatpPeriod1", "#IchUatpPeriod2", "#IchUatpPeriod3", "#IchUatpPeriod4", "#InterClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#AchPaxPeriod1").change(function () {
    GetValueofControlOnCheck("#AchPaxPeriod1", "#AchPaxPeriod2", "#AchPaxPeriod3", "#AchPaxPeriod4", "#AchClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#AchPaxPeriod2").change(function () {
    GetValueofControlOnCheck("#AchPaxPeriod1", "#AchPaxPeriod2", "#AchPaxPeriod3", "#AchPaxPeriod4", "#AchClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#AchPaxPeriod3").change(function () {
    GetValueofControlOnCheck("#AchPaxPeriod1", "#AchPaxPeriod2", "#AchPaxPeriod3", "#AchPaxPeriod4", "#AchClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#AchPaxPeriod4").change(function () {
    GetValueofControlOnCheck("#AchPaxPeriod1", "#AchPaxPeriod2", "#AchPaxPeriod3", "#AchPaxPeriod4", "#AchClearanceInvoiceSubmissionPatternPaxId");
  });

  $("#AchCgoPeriod1").change(function () {
    GetValueofControlOnCheck("#AchCgoPeriod1", "#AchCgoPeriod2", "#AchCgoPeriod3", "#AchCgoPeriod4", "#AchClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#AchCgoPeriod2").change(function () {
    GetValueofControlOnCheck("#AchCgoPeriod1", "#AchCgoPeriod2", "#AchCgoPeriod3", "#AchCgoPeriod4", "#AchClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#AchCgoPeriod3").change(function () {
    GetValueofControlOnCheck("#AchCgoPeriod1", "#AchCgoPeriod2", "#AchCgoPeriod3", "#AchCgoPeriod4", "#AchClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#AchCgoPeriod4").change(function () {
    GetValueofControlOnCheck("#AchCgoPeriod1", "#AchCgoPeriod2", "#AchCgoPeriod3", "#AchCgoPeriod4", "#AchClearanceInvoiceSubmissionPatternCgoId");
  });

  $("#AchMiscPeriod1").change(function () {
    GetValueofControlOnCheck("#AchMiscPeriod1", "#AchMiscPeriod2", "#AchMiscPeriod3", "#AchMiscPeriod4", "#AchClearanceInvoiceSubmissionPatternMiscId");
  });


  $("#AchMiscPeriod2").change(function () {
    GetValueofControlOnCheck("#AchMiscPeriod1", "#AchMiscPeriod2", "#AchMiscPeriod3", "#AchMiscPeriod4", "#AchClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#AchMiscPeriod3").change(function () {
    GetValueofControlOnCheck("#AchMiscPeriod1", "#AchMiscPeriod2", "#AchMiscPeriod3", "#AchMiscPeriod4", "#AchClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#AchMiscPeriod4").change(function () {
    GetValueofControlOnCheck("#AchMiscPeriod1", "#AchMiscPeriod2", "#AchMiscPeriod3", "#AchMiscPeriod4", "#AchClearanceInvoiceSubmissionPatternMiscId");
  });

  $("#AchUatpPeriod1").change(function () {
    GetValueofControlOnCheck("#AchUatpPeriod1", "#AchUatpPeriod2", "#AchUatpPeriod3", "#AchUatpPeriod4", "#AchClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#AchUatpPeriod2").change(function () {
    GetValueofControlOnCheck("#AchUatpPeriod1", "#AchUatpPeriod2", "#AchUatpPeriod3", "#AchUatpPeriod4", "#AchClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#AchUatpPeriod3").change(function () {
    GetValueofControlOnCheck("#AchUatpPeriod1", "#AchUatpPeriod2", "#AchUatpPeriod3", "#AchUatpPeriod4", "#AchClearanceInvoiceSubmissionPatternUatpId");
  });

  $("#AchUatpPeriod4").change(function () {
    GetValueofControlOnCheck("#AchUatpPeriod1", "#AchUatpPeriod2", "#AchUatpPeriod3", "#AchUatpPeriod4", "#AchClearanceInvoiceSubmissionPatternUatpId");
  });

  function GetValueofControlOnCheck(checkBoxId1, checkBoxId2, checkBoxId3, checkBoxId4, textBoxId) {
    var Index1 = 0;
    var Index2 = 0;
    var Index3 = 0;
    var Index4 = 0;

    if ($(checkBoxId1).is(':checked')) {
      Index1 = 1;
    }

    if ($(checkBoxId2).is(':checked')) {
      Index2 = 1;
    }

    if ($(checkBoxId3).is(':checked')) {
      Index3 = 1;
    }

    if ($(checkBoxId4).is(':checked')) {
      Index4 = 1;
    }

    var value = (Index1 + "" + Index2 + "" + Index3 + "" + Index4);
    $(textBoxId).val(value);
  }
}

function initTechnicalTabValidations() {
  if ($("#IspaxDeliveryMethodId").val() == 3) {
    $("#IspaxOutputServerIpAddress").removeAttr('readOnly');
    $("#IspaxOutputServerUserId").removeAttr('readOnly');
    $("#IspaxOutputServerPassword").removeAttr('readOnly');
    $("#IspaxOutputServerPassword").removeAttr('readOnly');
    $("#PaxIiNetFolder").removeAttr('readOnly');
    $("#PaxAccountId").removeAttr('readOnly');

  }
  if ($("#IscgoDeliveryMethodId").val() == 3) {
    $("#IscgoOutputServerIpAddress").removeAttr('readOnly');
    $("#IscgoOutputServerUserId").removeAttr('readOnly');
    $("#IscgoOutputServerPassword").removeAttr('readOnly');
    $("#CgoIiNetFolder").removeAttr('readOnly');
    $("#CgoAccountId").removeAttr('readOnly');
  }
  if ($("#IsmiscDeliveryMethodId").val() == 3) {
    $("#IsmiscOutputServerIpAddress").removeAttr('readOnly');
    $("#IsmiscOutputServerUserId").removeAttr('readOnly');
    $("#IsmiscOutputServerPassword").removeAttr('readOnly');
    $("#MiscIiNetFolder").removeAttr('readOnly');
    $("#MiscAccountId").removeAttr('readOnly');
  }
  if ($("#IsuatpDeliveryMethodId").val() == 3) {
    $("#IsuatpOutputServerIpAddress").removeAttr('readOnly');
    $("#IsuatpOutputServerUserId").removeAttr('readOnly');
    $("#IsuatpOutputServerPassword").removeAttr('readOnly');
    $("#UatpIiNetFolder").removeAttr('readOnly');
    $("#UatpAccountId").removeAttr('readOnly');
  }


  $("#technical").validate({
    rules: {
      IspaxOutputServerIpAddress: {
        required: function (element) {
          var value = $("#IspaxDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IspaxOutputServerUserId: {
        required: function (element) {
          var value = $("#IspaxDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IspaxOutputServerPassword: {
        required: function (element) {
          var value = $("#IspaxDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      PaxIiNetFolder: {
        required: function (element) {
          var value = $("#IspaxDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      PaxAccountId: {
        required: function (element) {
          var value = $("#IspaxDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IscgoOutputServerIpAddress: {
        required: function (element) {
          var value = $("#IscgoDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IscgoOutputServerUserId: {
        required: function (element) {
          var value = $("#IscgoDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IscgoOutputServerPassword: {
        required: function (element) {
          var value = $("#IscgoDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      CgoIiNetFolder: {
        required: function (element) {
          var value = $("#IscgoDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      CgoAccountId: {
        required: function (element) {
          var value = $("#IscgoDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IsmiscOutputServerIpAddress: {
        required: function (element) {
          var value = $("#IsmiscDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IsmiscOutputServerUserId: {
        required: function (element) {
          var value = $("#IsmiscDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IsmiscOutputServerPassword: {
        required: function (element) {
          var value = $("#IsmiscDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      MiscIiNetFolder: {
        required: function (element) {
          var value = $("#IsmiscDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      MiscAccountId: {
        required: function (element) {
          var value = $("#IsmiscDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IsuatpOutputServerIpAddress: {
        required: function (element) {
          var value = $("#IsuatpDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IsuatpOutputServerUserId: {
        required: function (element) {
          var value = $("#IsuatpDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      IsuatpOutputServerPassword: {
        required: function (element) {
          var value = $("#IsuatpDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      UatpIiNetFolder: {
        required: function (element) {
          var value = $("#IsuatpDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      },
      UatpAccountId: {
        required: function (element) {
          var value = $("#IsuatpDeliveryMethodId").val();
          if (value != 3) {
            return false;
          } else {
            return true;
          }
        }
      }
    },
    messages: {
      IspaxOutputServerIpAddress: "IS Passenger Output Server Ip Address required",
      IspaxOutputServerUserId: "IS Passenger Output Server User Id required",
      IspaxOutputServerPassword: "IS Passenger Output Server Password required",
      IscgoOutputServerIpAddress: "IS Cargo Output Server Ip Address required",
      IscgoOutputServerUserId: "IS Cargo Output Server User Id required",
      IscgoOutputServerPassword: "IS Cargo Output Server Password required",
      IsmiscOutputServerIpAddress: "IS Miscellaneous Output Server Ip Address required",
      IsmiscOutputServerUserId: "IS Miscellaneous Output Server User Id required",
      IsmiscOutputServerPassword: "IS Miscellaneous Output Server Password required",
      IsuatpOutputServerIpAddress: "IS Uatp Output Server Ip Address required",
      IsuatpOutputServerUserId: "IS Uatp Output Server User Id required",
      IsuatpOutputServerPassword: "IS Uatp Output Server Password required"
    },
    submitHandler: saveMemberData,
    invalidHandler: onValidationFailed
  });

  readonlyFunction("#IspaxDeliveryMethod", "#IspaxOutputServerIpAddress", "#IspaxOutputServerUserId", "#IspaxOutputServerPassword");
  readonlyFunction("#IscgoDeliveryMethod", "#IscgoOutputServerIpAddress", "#IscgoOutputServerUserId", "#IscgoOutputServerPassword");
  readonlyFunction("#IsmiscDeliveryMethod", "#IsmiscOutputServerIpAddress", "#IsmiscOutputServerUserId", "#IsmiscOutputServerPassword");
  readonlyFunction("#IsuatpDeliveryMethod", "#IsuatpOutputServerIpAddress", "#IsuatpOutputServerUserId", "#IsuatpOutputServerPassword");

  function readonlyFunction(dropdownId, textboxId1, textboxId2, textboxId3) {
    $(dropdownId).change(function () {
      var indicatorDropdown = document.getElementById(dropdownId);
      var value = $(dropdownId).val();
      if (value != 4) {
        $(textboxId1).attr('readonly', 'readonly');
        $(textboxId2).attr('readonly', 'readonly');
        $(textboxId3).attr('readonly', 'readonly');
      }
    });
  }

  enableDisableControlOnIndex("#IspaxDeliveryMethod", "#IspaxOutputServerIpAddress", "#IspaxOutputServerUserId", "#IspaxOutputServerPassword");
  enableDisableControlOnIndex("#IscgoDeliveryMethod", "#IscgoOutputServerIpAddress", "#IscgoOutputServerUserId", "#IscgoOutputServerPassword");
  enableDisableControlOnIndex("#IsmiscDeliveryMethod", "#IsmiscOutputServerIpAddress", "#IsmiscOutputServerUserId", "#IsmiscOutputServerPassword");
  enableDisableControlOnIndex("#IsuatpDeliveryMethod", "#IsuatpOutputServerIpAddress", "#IsuatpOutputServerUserId", "#IsuatpOutputServerPassword");

  function enableDisableControlOnIndex(dropdownIndexId, textBoxId1, textBoxId2, textBoxId3) {
    $(dropdownIndexId).change(function () {
      var value = $(dropdownIndexId).val();
      if (value == 4) {
        $(textBoxId1).removeAttr("readonly");
        $(textBoxId2).removeAttr("readonly");
        $(textBoxId3).removeAttr("readonly");
      }
      else {
        $(textBoxId1).val("");
        $(textBoxId2).val("");
        $(textBoxId3).val("");
        $(textBoxId1).attr('readonly', 'true');
        $(textBoxId2).attr('readonly', 'true');
        $(textBoxId3).attr('readonly', 'true');
      }
    });
  }
}

function initMemberControlsTabValidations() {
    $("#memberControl").validate({
        rules: {

            CdcCompartmentIDforInv: {
                required: function (element) {
                    var value = $("#CdcCompartmentIDforInv").val();
                    if (value == "") {
                        return true;
                    }
                    else {
                        return false;
                    }
                },
                maxlength: 100
            }
        },
        messages: {
            CdcCompartmentIDforInv: "If Legal Archiving is checked, this filed is required (Max Length 100) "    
        },
        submitHandler: saveMemberData,
        invalidHandler: onValidationFailed
    });
}


function saveMemberData(form) {
  $('.currentFieldValue').removeAttr("disabled");
  var formData = $("#" + form.id).serializeArray();
  $('.currentFieldValue').attr("disabled", true);
  $(".futureEditLink").show();

  var formData = $("#" + form.id).serializeArray();
  $('.currentFieldValue').removeAttr("disabled");
  $('.currentFieldValue').attr("disabled", true);
  $(".futureEditLink").show();
  $.ajax({
      url: form.action,
      type: "POST",
      data: formData,
      success: function (result) {
          $('#errorContainer').hide();

          if (result.IsFailed == false) {
              $('#clientSuccessMessage').html(result.Message);
              $('#clientSuccessMessageContainer').show();
              $('#clientErrorMessageContainer').hide();

              // Reset the dirtyForm status.
              $parentForm.resetDirty();

              if (form.id == "eBilling") {
                  $("#BillingCountiesToAdd").val('');
                  $('#BillingCountiesToRemove').val('');
                  $("#BilledCountiesToAdd").val('');
                  $('#BilledCountiesToRemove').val('');

                  $("#hiddenBillingCountryIdAdd").val('');
                  $('#hiddenBillingCountryIdRemove').val('');
                  $("#hiddenCountryIdAdd").val('');
                  $('#hiddenCountryIdRemove').val('');
                  $('#eBillingMemberId').val(result.Id);
              }
              if (result.IsAlert == true) {
                  alert(result.AlertMessage);
              }
          }
          else {
              $('#clientErrorMessage').html(result.Message);
              //Spira IN008343              
              if (typeof result.Message === 'undefined') {
                  //ID:2015-SIS-005, Point:2
                  var returnString = result;
                  if (returnString.indexOf("Invalid characters were entered") > -1) {
                      $('#clientErrorMessage').html('Invalid characters were entered. Operation did not complete.');
                  } else {
                      $('#clientErrorMessage').html('Session seems to be expired. Please log in again');
                  }
              }
              $('#clientErrorMessageContainer').show();
              $('#clientSuccessMessageContainer').hide();
          }
          if (result.isRedirect)
              window.location.href = result.RedirectUrl;
      }
  });
}

function saveMemberDataAlongWithContact(form) {
  $('#successMessageContainer').hide();
  var newArray = new Array();
  var contactList = '';
  var tabname = "#" + form.id + " :checkbox";
  var elements = $(tabname);
  for (i = 0; i < elements.length; i++) {
    if (elements[i].type == 'checkbox') {
      var index = elements[i].id.indexOf("_");
      if (index >= 0)
        contactList = contactList + "!" + elements[i].id + "|" + elements[i].checked;
    }
  }


  $("#" + form.id + "ContactList").val(contactList);
  $('.currentFieldValue').removeAttr("disabled");

  //SCP180697: Issue to be fixed in Profile Location edit screen.
  $('#IsUatpLocation').removeAttr('disabled');
  var formData = $("#" + form.id).serializeArray();

  $("#IsUatpLocation").attr('disabled', 'disabled');
  $('.currentFieldValue').attr("disabled", true);

  $.ajax({
    url: form.action,
    type: "POST",
    data: formData,
    success: function (result) {
      $('#errorContainer').hide();

      if (result.IsFailed == false) {
        // Toggle message containers.
        $('#clientSuccessMessage').html(result.Message);
        $('#clientSuccessMessageContainer').show();
        $('#clientErrorMessageContainer').hide();

        // Reset the dirtyForm status.
        $parentForm.resetDirty();

        $(".futureEditLink").show();

        var memberShipStatus, memberId;
        if (form.id == "uatp") {
          if (result.Value == "True")
            $(".ignoreUATPEditLink").show();
          else
            $(".ignoreUATPEditLink").hide();

          if (result.FutureFieldValue == "True") {
            $(".BillingDataSubmittedByThirdPartyEditLink").show();
          }
          else {
            $(".BillingDataSubmittedByThirdPartyEditLink").hide();
            $("#IsBillingDataSubmittedByThirdPartiesRequired").attr({ "checked": false });
            $("#IsBillingDataSubmittedByThirdPartiesRequiredFutureDateInd").hide();
          }
        }

        if (form.id == "location") {

          for (i = 0; i < formData.length; i++) {
            if (formData[i].name == "Id") {
              $('#locationId').val(result.Id);
            }
          }

          //Refresh location dropdown list.
          resetLocationCode(result.Id);

          // Sync the 'Member Name' with 'Member Commercial Name' of Main location.
          if ($('#DisplayCommercialName').length > 0 && result.DisplayCommercialName)
            $('#DisplayCommercialName').val(result.DisplayCommercialName);

        }

        if (form.id == "pax") {
          for (i = 0; i < formData.length; i++) {
            if (formData[i].name == "MemberId") {
              $('#paxMemberId').val(result.Id);
            }

          }
          $("#IsConsolidatedProvisionalBillingFileRequiredFutureValue").val(result.Value);
          if ($('#SamplingCareerTypeId').val() != 1) {
            $(".provBillingFileEditLink").show();
          }
          if ($("#IsParticipateInValueDetermination").val() == 'True') {
            $(".autoBillingEditLink").show();
          }
          else {
            $(".autoBillingEditLink").hide();
            $("#IsParticipateInAutoBillingFutureDateInd").hide();
          }
        }

        if (form.id == "ach") {

          for (i = 0; i < formData.length; i++) {
            if (formData[i].name == "PaxExceptionMemberAddList") {
              if (formData[i].value > 0) {
                $('#PaxExceptionFuturePeriod').show();
                $('#paxExcLabel').show();
              }
            }

            if (formData[i].name == "CgoExceptionMemberAddList") {
              if (formData[i].value > 0) {
                $('#CgoExceptionFuturePeriod').show();
                $('#cgoExcLabel').show();
              }
            }

            if (formData[i].name == "MiscExceptionMemberAddList") {
              if (formData[i].value > 0) {
                $('#MiscExceptionFuturePeriod').show();
                $('#miscExcLabel').show();
              }
            }

            if (formData[i].name == "UatpExceptionMemberAddList") {
              if (formData[i].value > 0) {
                $('#UatpExceptionFuturePeriod').show();
                $('#uatpExcLabel').show();
              }
            }
          }
        }


        if (form.id == "contacts") {
          for (i = 0; i < formData.length; i++) {
            if (formData[i].name == "Id") {
              $('#contactId').val(result.Id);
            }
          }
          var selectedRow = $("#ContactsGrid > tbody > tr[id=" + result.Id + "]");
          if (selectedRow.length > 0) {
            //$("#ContactsGrid").trigger("reloadGrid").setSelection(result.Id, true);
            $("#ContactsGrid").trigger("reloadGrid").jqGrid('setSelection', result.Id, true);

            //Update existing contact
            editSelectOption('#replaceoldcontact', result.Id, result.Value);
            editSelectOption('#replacenewcontact', result.Id, result.Value);
            editSelectOption('#copyoldcontact', result.Id, result.Value);
            editSelectOption('#copynewcontact', result.Id, result.Value);
          }
          else {
            $("#ContactsGrid").trigger("reloadGrid");

            //Add new contact
            if (result.Value == " ") {
              addSelectOption('#replaceoldcontact', result.Id, $("#FirstNameHidden").val() + " " + $("#LastNameHidden").val());
              addSelectOption('#replacenewcontact', result.Id, $("#FirstNameHidden").val() + " " + $("#LastNameHidden").val());
              addSelectOption('#copyoldcontact', result.Id, $("#FirstNameHidden").val() + " " + $("#LastNameHidden").val());
              addSelectOption('#copynewcontact', result.Id, $("#FirstNameHidden").val() + " " + $("#LastNameHidden").val());
            }
            else {
              addSelectOption('#replaceoldcontact', result.Id, result.Value);
              addSelectOption('#replacenewcontact', result.Id, result.Value);
              addSelectOption('#copyoldcontact', result.Id, result.Value);
              addSelectOption('#copynewcontact', result.Id, result.Value);
            }
          }
        }

        if (form.id == "MemberDetails") {
          //Set hidden field value to current status for member
          $('#hdnIsMembershipStatus').val($('#IsMembershipStatusId').val());
          $('#createSuperUser').attr("disabled", false);
          var isToEmailSend = false;
          for (i = 0; i < formData.length; i++) {
            if (formData[i].name == "IsMembershipStatusId") {
              memberShipStatus = formData[i].value;
            }
            if (formData[i].name == "Id") {
              memberId = formData[i].value;
            }
          }
          if ((memberId == "" || memberId == "0") && (memberShipStatus != "1"))
            isToEmailSend = emailConfirmation();
          else
            isToEmailSend = false;

          var isLogoToUpload = $memberLogoToUpload;
          if (isLogoToUpload) {
            result.isRedirect = false;
            $('#imageUploadForm').attr('action', result.OtherUrl);
          }
          if (isToEmailSend) SendEmail(isLogoToUpload);
          else {
            if (isLogoToUpload) {
              $("#submitImage").trigger("click");
            }
          }
        }

        if (form.id == "Ich") {
                 
          // check to show aggregator fuutre date ind 
          //ID : 43358 - Member profile XML from SIS to ICH - Aggregator Function
          //Show indicator only when value is set for future
          if ($("#AggregatorFuturePeriod").val() != '')//parseInt($('#AggregatorAddCount').val()) > 0)// && parseInt($('#AggregatorsSelected option').length) > 0)
            $("#AggregatorFutureDateInd").show();
          else
            $("#AggregatorFutureDateInd").hide();

          // check to show sponsor fuutre date ind parseInt($('#SponsorrAddCount').val()) > 0 && 
          //ID : 43358 - Member profile XML from SIS to ICH - Aggregator Function
          //Show indicator only when value is set for future
          if ($("#SponsororFuturePeriod").val() != '')//parseInt($('#spnFuturePeriod').val()) > 0)// && parseInt($('#SponsororsSelected option').length) > 0)
            $("#SponsoredFutureDateInd").show();
          else
            $("#SponsoredFutureDateInd").hide();

          for (i = 0; i < formData.length; i++) {
            if (formData[i].name == "SponsororAddList") {
              if (formData[i].value > 0) {
                $('#spnFuturePeriod').show();
                $('#spnFuturePeriodlbl').show();
              }
            }

            if (formData[i].name == "AggregatorAddList") {

              if (formData[i].value > 0) {
                $('#aggFuturePeriod').show();
                $('#aggFuturePeriodlbl').show();

              }
            }

          }

          $("#SponsororAddList").val('');
          $('#SponsororDeletedList').val('');
          $("#AggregatorAddList").val('');
          $('#AggregatorDeleteList').val('');

          $("#hiddenMemberIdAdd").val('');
          $('#hiddenMemberIdRemove').val('');
          $("#hiddenAggrMemberIdAdd").val('');
          $('#hiddenAggrMemberIdRemove').val('');

          $('#hdnIchMembershipStatus').val($('#IchMemberShipStatusId').val());
          $('#currIchMemberId').val(result.Id);

          $("#AggregatorAddCount").val('0');
          $('#AggrAvailMembersDisabled').val('-1');
          $('#SponsAvailMembersDisabled').val('-1');

          if ($("#ichReinstatementPeriod").val() != '') {
            $("#ReinstFutureDateInd").show();
          }
          else {
            $("#ReinstFutureDateInd").hide();
          }

          //CMP-689-Flexible CH Activation Options
          if (($('#IchMemberShipStatusId').val() == 3 || $('#IchMemberShipStatusId').val() == 4) && $('#IchMemberShipStatusId' + 'FutureValue').val() == 1) {
            $('#divIchEntryDate').hide();
          }
          else {
            $('#divIchEntryDate').show();
          }
        }

        if (form.id == "ach") {
          $('#hdnAchMembershipStatus').val($('#AchMembershipStatusId').val());
          $("#PaxExceptionMemberAddList").val('');
          $('#PaxExceptionMemberDeleteList').val('');
          $("#CgoExceptionMemberAddList").val('');
          $('#CgoExceptionMemberDeleteList').val('');
          $("#MiscExceptionMemberAddList").val('');
          $('#MiscExceptionMemberDeleteList').val('');
          $("#UatpExceptionMemberAddList").val('');
          $('#UatpExceptionMemberDeleteList').val('');

          $("#hiddenpaxMemberIdAdd").val('');
          $('#hiddenpaxMemberIdRemove').val('');
          $("#hiddencgoMemberIdAdd").val('');
          $('#hiddencgoMemberIdRemove').val('');
          $("#hiddenmiscMemberIdAdd").val('');
          $('#hiddenmiscMemberIdRemove').val('');
          $("#hiddenuatpMemberIdAdd").val('');
          $('#hiddenuatpMemberIdRemove').val('');

          if ($("#achReinstatementPeriod").val() != '') {
            $("#achReinstFutureDateInd").show();
          }
          else {
            $("#achReinstFutureDateInd").hide();
          }

          //CMP-689-Flexible CH Activation Options
          if (($('#AchMembershipStatusId').val() == 3 || $('#AchMembershipStatusId').val() == 4) && $('#AchMembershipStatusId' + 'FutureValue').val() == 1) {
            $('#divAchEntryDate').hide();
          }
          else {
            $('#divAchEntryDate').show();
          }
        }
      }
      else {

        $('#clientErrorMessage').html(result.Message);
        //Spira IN008343
        if (typeof result.Message === 'undefined') {
          //ID:2015-SIS-005, Point:2
          var returnString = result;
          if (returnString.indexOf("Invalid characters were entered") > -1) {
            $('#clientErrorMessage').html('Invalid characters were entered. Operation did not complete.');
          } else {
            $('#clientErrorMessage').html('Session seems to be expired. Please log in again');
          }
        }
        $('#clientErrorMessageContainer').show();
        $('#clientSuccessMessageContainer').hide();
      }


      if (result.isRedirect) {
        window.location.href = result.RedirectUrl;
      }
    }
  });

}

function emailConfirmation() {
  //Fix TFS issue #2946
  if (confirm("Do you wish to send an email notification/alert to ICH/ACH operations team for configuring ICH/ACH specific profile elements? \nClick  ‘OK’, to send the email notification/alert. \nClick ‘Cancel’ to proceed with the SAVE without sending email notification/alert.")) {
    return true;
  }
  return false;
}

function SendEmail(isLogoToUpload) {
  $.ajax({
    type: "POST",
    url: urlEmailSend + "/" + isLogoToUpload,
    success: function (result) {
        if (result && isLogoToUpload) {
            $("#submitImage").trigger("click");
      }
    }
  })
}

function isMigrationDateValid(controlId) {

  var value = $("#" + controlId).val();
  if (value == 3) {
    return true;
  } else {
    return false;
  }
}

function isValidCurrency(controlId) {
    var value = $("#" + controlId).prop('checked');
  if (value == true) {
    return true;
  } else {
    return false;
  }

}

function showImageDialog() {
  multi_selector = new MultiSelector(document.getElementById('files_list'), 1);
  multi_selector.addElement(document.getElementById('memberLogoUpload'));
  $ImageUploaddialog.dialog('open');

  return false;
}

function OnOKMemeberLogoUpload() {

  $memberLogoToUpload = $("#memberLogoUpload")[0].value.length > 0;
  if (!$memberLogoToUpload) alert("Please select a logo to upload.");
  else { alert("Logo will be saved when the Member Details are saved. Please allow sometime for the new logo to take effect."); $ImageUploaddialog.dialog('close'); }


  //  $memberLogoToUpload = $("#memberLogoUpload")[0].value.length > 0;
  //  if (!$memberLogoToUpload) {
  //      alert("Please select a logo to upload.");
  //  }
  //  else {
  //      var index = $("#memberLogoUpload")[0].value.lastIndexOf('.');
  //      var filetype = $("#memberLogoUpload")[0].value.substr(index);
  //      if (filetype.toLowerCase() != ".png" && filetype.toLowerCase() != ".gif" && filetype.toLowerCase() != ".jpg")
  //       {
  //           alert("File format should be .jpg or .png or .gif");
  //           $ImageUploaddialog.dialog('close');
  //      }
  //      else { alert("Logo will be saved when the Member Details are saved. Please allow sometime for the new logo to take effect."); $ImageUploaddialog.dialog('close'); }
  //  }
}

function OnCancelMemeberLogoUpload() {
  $("#memberLogoUpload").val("");
  $ImageUploaddialog.dialog('close');
}

function viewLocationDetails(postUrl, controlId) {
  $("#LocationIdFlag").val("0");
  var LocationIdVal = $('#' + controlId).val();
  clearMessageContainer();

  //CMP#622: MISC Outputs Split as per Location IDs
  if ($("#selLocationCode :selected").text() == "Main" || $("#selLocationCode :selected").text() == "UATP") {
      $("#LocSpecific").hide();
  }
  else {
      $("#LocSpecific").show();
  }

  if (LocationIdVal == "") {

    if (controlId == 'selLocationCode') {
      $('.dataEntry input[type="text"]').attr('value', '');
      $('select').val('');
      $('textarea').attr('val', '');
      OnLocationDetailsPopulated(null);
    }
    else if (controlId == 'LocationId') {
      OnContactLocationDetailsPopulated(null);
    }
  }
  else {

    $.ajax({
      type: "POST",
      url: postUrl,
      data: { locationId: LocationIdVal },
      dataType: "json",
      success: function (result) {

        if (controlId == 'selLocationCode') {
          OnLocationDetailsPopulated(result);
        }
        else if (controlId == 'LocationId') {
          OnContactLocationDetailsPopulated(result);
        }
      },
      failure: function (response) {
        alert(response.Id);
      }
    });
  }
}

function OnContactLocationDetailsPopulated(response) {
  if (response == null) {
    $('#conCountryId').removeAttr('disabled');
    $("#conAddressLine1").removeAttr('disabled');
    $("#conAddressLine2").removeAttr('disabled');
    $("#conAddressLine3").removeAttr('disabled');
    $("#conCityName").removeAttr('disabled');
    $("#conSubDivisionName").removeAttr('disabled');
    $("#conPostalCode").removeAttr('disabled');

    $("#conCountryId").val("");
    $("#conAddressLine1").val("");
    $("#conAddressLine2").val("");
    $("#conAddressLine3").val("");
    $("#conCityName").val("");
    $("#SubDivisionCode").val("");
    $("#conSubDivisionName").val("");
    $("#conPostalCode").val("");

  }
  else {
    $("#conCountryId").val("");
    $("#conCountryId").attr('disabled', 'disabled');
    $("#conAddressLine1").val("");
    $("#conAddressLine1").attr('disabled', true);
    $("#conAddressLine2").val("");
    $("#conAddressLine2").attr('disabled', true);
    $("#conAddressLine3").val("");
    $("#conAddressLine3").attr('disabled', true);
    $("#conCityName").val("");
    $("#conCityName").attr('disabled', true);
    $("#conSubDivisionName").val("");
    $("#conSubDivisionName").attr('disabled', true);
    $("#conPostalCode").val("");
    $("#conPostalCode").attr('disabled', true);

    if (response.AddressLine1 == null)
      $("#conAddressLine1").val("");
    else
      $("#conAddressLine1").val(response.AddressLine1);

    if (response.AddressLine2 == null)
      $("#conAddressLine2").val("");
    else
      $("#conAddressLine2").val(response.AddressLine2);

    if (response.AddressLine3 == null)
      $("#conAddressLine3").val("");
    else
      $("#conAddressLine3").val(response.AddressLine3);

    if (response.CityName == null)
      $("#conAddressLine3").val("");
    else
      $("#conAddressLine3").val(response.CityName);

    if (response.SubDivisionName == null)
      $("#conSubDivisionName").val("");
    else
      $("#conSubDivisionName").val(response.SubDivisionName);

    if (response.PostalCode == null)
      $("#conPostalCode").val("");
    else
      $("#conPostalCode").val(response.PostalCode);

    if (response.CountryId == null)
      $("#conCountryId").val("");
    else
      $("#conCountryId").val(response.CountryId);

    if (response.CityName == null)
      $("#conCityName").val("");
    else
      $("#conCityName").val(response.CityName);
  }
}

function OnLocationDetailsPopulated(response) {
  locationValidator.resetForm();
  if (response == null) {
    $(".futureEditLink").hide();
    AddLocationDetails();
  }
  else {
    var LocationId = response.LocationCode;
    $("#selLocationCode option:contains(" + response.LocationCode + ")").attr('selected', 'selected');
    //SCP180697: Issue to be fixed in Profile Location edit screen
    $("#IsUatpLocation").attr('disabled', 'disabled');

    if (LocationId == "Main") {
        $("#IsActive").attr('disabled', 'disabled');
        $('#IsUatpLocation').prop('checked', false);
      $("#IsUatpLocation").attr('disabled', 'disabled');
    }
    else if (LocationId == "UATP") {
        $('#IsActive').prop('checked', true);
        $("#IsActive").attr('disabled', 'disabled');
     // $('#IsUatpLocation').removeAttr('disabled');
    }
    else {
        $('#IsActive').removeAttr('disabled');
        //SCP136581:Active indicator IS-Web
        $('#IsActive').removeAttr('disabled');
     // if ($('#selLocationCode').find(":contains('UATP')").length == 0)
     // $('#IsUatpLocation').removeAttr('disabled');
    //  else {
        $('#IsUatpLocation').prop('checked', false);
        $("#IsUatpLocation").attr('disabled', 'disabled');
    //}
    }

    if (response.LocationCode == "UATP")
        $("#IsUatpLocation").prop('checked', true);

    if (response.Id == null)
      $("#locationId").val("");
    else
      $("#locationId").val(response.Id);

    if (response.MemberLegalName == null)
      $("#MemberLegalName").val("");
    else
      $("#MemberLegalName").val(response.MemberLegalName);

    if (response.MemberCommercialName == null)
      $("#MemberCommercialName").val("");
    else
      $("#MemberCommercialName").val(response.MemberCommercialName);

    if (response.AdditionalTaxVatRegistrationNumber == null)
      $("#AdditionalTaxVatRegistrationNumber").val("");
    else
      $("#AdditionalTaxVatRegistrationNumber").val(response.AdditionalTaxVatRegistrationNumber);

    if (response.TaxVatRegistrationNumber == null)
      $("#TaxVatRegistrationNumber").val("");
    else
      $("#TaxVatRegistrationNumber").val(response.TaxVatRegistrationNumber);

    if (response.RegistrationId == null)
      $("#RegistrationId").val("");
    else
      $("#RegistrationId").val(response.RegistrationId);

    //CountryId

    if (response.CountryId == null)
      $("#CountryId").val("");
    else
      $("#CountryId").val(response.CountryId);

    if (response.AddressLine1 == null)
      $("#AddressLine1").val("");
    else
      $("#AddressLine1").val(response.AddressLine1);

    if (response.AddressLine2 == null)
      $("#AddressLine2").val("");
    else
      $("#AddressLine2").val(response.AddressLine2);

    if (response.AddressLine3 == null)
      $("#AddressLine3").val("");
    else
      $("#AddressLine3").val(response.AddressLine3);

    if (response.CityName == null)
      $("#CityName").val("");
    else
      $("#CityName").val(response.CityName);

    if (response.BankName == null)
      $("#locBankName").val("");
    else
      $("#locBankName").val(response.BankName);

    if (response.SubDivisionName == null)
      $("#SubDivisionName").val("");
    else
      $("#SubDivisionName").val(response.SubDivisionName);

    if (response.PostalCode == null)
      $("#PostalCode").val("");
    else
      $("#PostalCode").val(response.PostalCode);

    if (response.LegalText == null)
      $("#LegalText").val("");
    else
      $("#LegalText").val(response.LegalText);

    if (response.Iban == null)
      $("#Iban").val("");
    else
      $("#Iban").val(response.Iban);

    if (response.Swift == null)
      $("#Swift").val("");
    else
      $("#Swift").val(response.Swift);

    if (response.BankCode == null)
      $("#BankCode").val("");
    else
      $("#BankCode").val(response.BankCode);

    if (response.BranchCode == null)
      $("#BranchCode").val("");
    else
      $("#BranchCode").val(response.BranchCode);

    if (response.BankAccountNumber == null)
      $("#BankAccountNumber").val("");
    else
      $("#BankAccountNumber").val(response.BankAccountNumber);

    if (response.BankAccountName == null)
      $("#BankAccountName").val("");
    else
      $("#BankAccountName").val(response.BankAccountName);

    if (response.CurrencyId == null)
      $("#CurrencyId").val("");
    else
      $("#CurrencyId").val(response.CurrencyId);

    if (response.LocationCode == null) {
      $("#LocationCode").val("");
      $("#selLocationCode").val("");
    }

    else {
      $("#LocationCode").val(response.LocationCode);
      $("#selLocationCode option:contains(" + response.LocationCode + ")").attr('selected', 'selected');
      $("#selLocationCode").val(response.Id);
    }

    //CMP#622: MISC Outputs Split as per Location IDs 
    if (response.FileSpecificLocReq == null) {
        $("#fileSpecLocReq").prop('checked', false);
     }
    else {
        $("#fileSpecLocReq").prop('checked', response.FileSpecificLocReq);
    }

    if (response.LociiNetAccountId == null) {
      $("#lociiNetAccId").val("");
     }
     else {
      $("#lociiNetAccId").val(response.LociiNetAccountId);
    }

    //Country ID
    if (response.CountryIdFuturePeriod == null) {
      $("#CountryIdFuturePeriod").val("");
      $("#CountryIdFutureDateInd").hide();
    }
    else {
      $("#CountryIdFuturePeriod").val(response.CountryIdFuturePeriod);
      $("#CountryIdFutureDateInd").show();
    }

    if (response.CountryIdFutureValue == null) {
      $("#CountryIdFutureValue").val("");
      $("#CountryIdFutureDisplayValue").val("");
    }
    else {
      $("#CountryIdFutureValue").val(response.CountryIdFutureValue);
      $("#CountryIdFutureDisplayValue").val(response.CountryIdFutureDisplayValue);
    }

        // Member Legal Name
        if (response.MemberLegalNameFuturePeriod == null) {
            $("#MemberLegalNameFuturePeriod").val("");
            $("#MemberLegalNameFutureDateInd").hide();
        }
        else {
            $("#MemberLegalNameFuturePeriod").val(response.MemberLegalNameFuturePeriod);
            $("#MemberLegalNameFutureDateInd").show();
        }

        if (response.MemberLegalNameFutureValue == null)
            $("#MemberLegalNameFutureValue").val("");
        else
            $("#MemberLegalNameFutureValue").val(response.MemberLegalNameFutureValue);
    //Registration Id




    if (response.RegistrationIdFuturePeriod == null) {
      $("#RegistrationIdFuturePeriod").val("");
      $("#RegistrationIdFutureDateInd").hide();
    }
    else {
      $("#RegistrationIdFuturePeriod").val(response.RegistrationIdFuturePeriod);
      $("#RegistrationIdFutureDateInd").show();
    }

    if (response.RegistrationIdFutureValue == null)
      $("#RegistrationIdFutureValue").val("");
    else
      $("#RegistrationIdFutureValue").val(response.RegistrationIdFutureValue);

    //TAX/VAT Registration Number
    if (response.TaxVatRegistrationNumberFuturePeriod == null) {
      $("#TaxVatRegistrationNumberFuturePeriod").val("");
      $("#TaxVatRegistrationNumberFutureDateInd").hide();
    }
    else {
      $("#TaxVatRegistrationNumberFuturePeriod").val(response.TaxVatRegistrationNumberFuturePeriod);
      $("#TaxVatRegistrationNumberFutureDateInd").show();
    }

    if (response.TaxVatRegistrationNumberFutureValue == null)
      $("#TaxVatRegistrationNumberFutureValue").val("");
    else
      $("#TaxVatRegistrationNumberFutureValue").val(response.TaxVatRegistrationNumberFutureValue);

    //Additional Tax/Vat RegistrationNumber
    if (response.AdditionalTaxVatRegistrationNumberFuturePeriod == null) {
      $("#AdditionalTaxVatRegistrationNumberFuturePeriod").val("");
      $("#AdditionalTaxVatRegistrationNumberFutureDateInd").hide();
    }
    else {
      $("#AdditionalTaxVatRegistrationNumberFuturePeriod").val(response.AdditionalTaxVatRegistrationNumberFuturePeriod);
      $("#AdditionalTaxVatRegistrationNumberFutureDateInd").show();
    }

    if (response.AdditionalTaxVatRegistrationNumberFutureValue == null)
      $("#AdditionalTaxVatRegistrationNumberFutureValue").val("");
    else
      $("#AdditionalTaxVatRegistrationNumberFutureValue").val(response.AdditionalTaxVatRegistrationNumberFutureValue);

    //AddressLine1
    if (response.AddressLine1FuturePeriod == null) {
      $("#AddressLine1FuturePeriod").val("");
      $("#AddressLine1FutureDateInd").hide();
    }
    else {
      $("#AddressLine1FuturePeriod").val(response.AddressLine1FuturePeriod);
      $("#AddressLine1FutureDateInd").show();
    }

    if (response.AddressLine1FutureValue == null)
      $("#AddressLine1FutureValue").val("");
    else
      $("#AddressLine1FutureValue").val(response.AddressLine1FutureValue);

    //AddressLine2




    if (response.AddressLine2FuturePeriod == null) {
      $("#AddressLine2FuturePeriod").val("");
      $("#AddressLine2FutureDateInd").hide();
    }
    else {
      $("#AddressLine2FuturePeriod").val(response.AddressLine2FuturePeriod);
      $("#AddressLine2FutureDateInd").show();
    }

    if (response.AddressLine2FutureValue == null)
      $("#AddressLine2FutureValue").val("");
    else
      $("#AddressLine2FutureValue").val(response.AddressLine2FutureValue);






    //Address Line 3
    if (response.AddressLine3FuturePeriod == null) {
      $("#AddressLine3FuturePeriod").val("");
      $("#AddressLine3FutureDateInd").hide();
    }
    else {
      $("#AddressLine3FuturePeriod").val(response.AddressLine3FuturePeriod);
      $("#AddressLine3FutureDateInd").show();
    }

    if (response.AddressLine3FutureValue == null)
      $("#AddressLine3FutureValue").val("");
    else
      $("#AddressLine3FutureValue").val(response.AddressLine3FutureValue);

    //City Name
    if (response.CityNameFuturePeriod == null) {
      $("#CityNameFuturePeriod").val("");
      $("#CityNameFutureDateInd").hide();
    }
    else {
      $("#CityNameFuturePeriod").val(response.CityNameFuturePeriod);
      $("#CityNameFutureDateInd").show();
    }

    if (response.CityNameFutureValue == null)
      $("#CityNameFutureValue").val("");
    else
      $("#CityNameFutureValue").val(response.CityNameFutureValue);

    //Subdivision Name
    if (response.SubDivisionNameFuturePeriod == null) {
      $("#SubDivisionNameFuturePeriod").val("");
      $("#SubDivisionNameFutureDateInd").hide();
    }
    else {
      $("#SubDivisionNameFuturePeriod").val(response.SubDivisionNameFuturePeriod);
      $("#SubDivisionNameFutureDateInd").show();
    }

    if (response.SubDivisionNameFutureValue == null)
      $("#SubDivisionNameFutureValue").val("");
    else
      $("#SubDivisionNameFutureValue").val(response.SubDivisionNameFutureValue);

    //Postal Code
    if (response.PostalCodeFuturePeriod == null) {
      $("#PostalCodeFuturePeriod").val("");
      $("#PostalCodeFutureDateInd").hide();
    }
    else {
      $("#PostalCodeFuturePeriod").val(response.PostalCodeFuturePeriod);
      $("#PostalCodeFutureDateInd").show();
    }

    if (response.PostalCodeFutureValue == null)
      $("#PostalCodeFutureValue").val("");
    else
      $("#PostalCodeFutureValue").val(response.PostalCodeFutureValue);

    //Legal Text

    if (response.LegalTextFuturePeriod == null) {
      $("#LegalTextFuturePeriod").val("");
      $("#LegalTextFutureDateInd").hide();
    }
    else {
      $("#LegalTextFuturePeriod").val(response.LegalTextFuturePeriod);
      $("#LegalTextFutureDateInd").show();
    }

    if (response.LegalTextFutureValue == null)
      $("#LegalTextFutureValue").val("");
    else
      $("#LegalTextFutureValue").val(response.LegalTextFutureValue);

    $(".futureEditLink").show();
    $('.currentFieldValue').attr("disabled", true);

   
    if (response.IsActive == null)
        $('#IsActive').prop('checked', false);
    else
        $('#IsActive').prop('checked', response.IsActive);

        // CMP597 : Not able to update Active checkbox.
        if (response.IsActiveFuturePeriod == null) {
            $("#IsActiveFuturePeriod").val("");
            $("#IsActiveFutureDateInd").hide();
        }
        else {
            $("#IsActiveFuturePeriod").val(response.IsActiveFuturePeriod);
            $("#IsActiveFutureDateInd").show();
        }
        if (response.IsActiveFutureValue == null)
            $("#IsActiveFutureValue").val("");
        else
            $("#IsActiveFutureValue").val(response.IsActiveFutureValue);

        // CMP597 : Not able to update Active checkbox.
        if (LocationId == "Main" || LocationId == "UATP") {
            $('#FutureEditLinkFor_IsActive').hide();
        }
  }
}

function AddLocationDetails() {
  //CMP#622: MISC Outputs Split as per Location IDs
  $("#LocSpecific").show();
  $("#LocationIdFlag").val("1");
  $("#locationId").val("");
  $("#MemberLegalName").val("");
  $("#MemberCommercialName").val("");
  $("#AdditionalTaxVatRegistrationNumber").val("");
  $("#TaxVatRegistrationNumber").val("");
  $("#RegistrationId").val("");
  $("#CountryId").val("");
  $("#AddressLine1").val("");
  $("#AddressLine2").val("");
  $("#AddressLine3").val("");
  $("#CityName").val("");
  $("#SubDivisionName").val("");
  $("#PostalCode").val("");
  $("#LegalText").val("");
  $("#Iban").val("");
  $("#Swift").val("");
  $("#BankCode").val("");
  $("#locBankName").val("");
  $("#BranchCode").val("");
  $("#BankAccountNumber").val("");
  $("#BankAccountName").val("");
  $("#CurrencyId").val("");
  $("#selLocationCode").val("");
  $('#IsActive').removeAttr('disabled');
  //CMP#622: MISC Outputs Split as per Location IDs 
  $("#fileSpecLocReq").prop('checked', false);
  $("#lociiNetAccId").val("");
  if ($("#selLocationCode").find(":contains('UATP')").length > 0) {
      $('#IsUatpLocation').prop('checked', false);
    $("#IsUatpLocation").attr('disabled', 'disabled');
  }
  else {
    $('#IsUatpLocation').removeAttr('disabled');
  }

  /*Future Update related fields*/
  $("#RegistrationIdFuturePeriod").val("");
  $("#RegistrationIdFutureValue").val("");
    $("#MemberLegalNameFuturePeriod").val("");
    $("#MemberLegalNameFutureValue").val("");
  $("#CountryIdFuturePeriod").val("");
  $("#CountryIdFutureValue").val("");
  $("#TaxVatRegistrationNumberFuturePeriod").val("");
  $("#TaxVatRegistrationNumberFutureValue").val("");
  $("#AdditionalTaxVatRegistrationNumberFuturePeriod").val("");
  $("#AdditionalTaxVatRegistrationNumberFutureValue").val("");
  $("#AddressLine1FuturePeriod").val("");
  $("#AddressLine1FutureValue").val("");
  $("#AddressLine2FuturePeriod").val("");
  $("#AddressLine2FutureValue").val("");
  $("#AddressLine3FuturePeriod").val("");
  $("#AddressLine3FutureValue").val("");
  $("#CityNameFuturePeriod").val("");
  $("#CityNameFutureValue").val("");
  $("#SubDivisionNameFuturePeriod").val("");
  $("#SubDivisionNameFutureValue").val("");
  $("#PostalCodeFuturePeriod").val("");
  $("#PostalCodeFutureValue").val("");
  $("#LegalTextFuturePeriod").val("");
  $("#LegalTextFutureValue").val("");
  // CMP597 : Not able to update Active checkbox.
  $("#IsActiveFuturePeriod").val("");
  $("#IsActiveFutureValue").val("");

  // Make future update fields editable since while adding new location
  $('.currentFieldValue').removeAttr("disabled");
  $("#RegistrationIdFutureDateInd").hide();
    $("#MemberLegalNameFutureDateInd").hide();
  $("#TaxVatRegistrationNumberFutureDateInd").hide();
  $("#AdditionalTaxVatRegistrationNumberFutureDateInd").hide();
  $("#AddressLine1FutureDateInd").hide();
  $("#AddressLine2FutureDateInd").hide();
  $("#AddressLine3FutureDateInd").hide();
  $("#CityNameFutureDateInd").hide();
  $("#SubDivisionNameFutureDateInd").hide();
  $("#PostalCodeFutureDateInd").hide();
  $("#LegalTextFutureDateInd").hide();
  $("#CountryIdFutureDateInd").hide();
  $(".futureEditLink").hide();
  setFocus('#MemberLegalName');
}

function enableDisableControlOnChecking(checkBoxId, textBoxId1, textBoxId2, textBoxId3) {
  $(checkBoxId).change(function () {
      if ($(this).prop('checked') == true) {

      $(textBoxId1).removeAttr("readonly");
      $(textBoxId2).removeAttr("readonly");
      $(textBoxId3).removeAttr("readonly");
    }
    else {
      $(textBoxId1).val("");
      $(textBoxId2).val("");
      $(textBoxId3).val("");
      $(textBoxId1).attr('readonly', 'true');
      $(textBoxId2).attr('readonly', 'true');
      $(textBoxId3).attr('readonly', 'true');
    }
  });
}

function AddNewContact() {
  if (contactValidator != null) {
    contactValidator.resetForm();
  }
  $("#StaffId").removeAttr('readonly');
  $("#conIsActive").removeAttr('readonly');
  $("#conIsActive").removeAttr('disabled');
  //SCP333083: XML Validation Failure for A30-XB - SIS Production
  $("#EmailAddress").removeAttr('readonly');
  $("#Salutation").removeAttr('readonly');
  $("#FirstName").removeAttr('readonly');
  $("#LastName").removeAttr('readonly');
  $("#PositionOrTitle").removeAttr('readonly');
  $("#Division").removeAttr('readonly');
  $("#Department").removeAttr('readonly');
  $("#LocationId").removeAttr('readonly');
  $("#conAddressLine1").removeAttr('readonly');
  $("#conAddressLine2").removeAttr('readonly');
  $("#conAddressLine3").removeAttr('readonly');
  $("#conCityName").removeAttr('readonly');
  $("#conCountryId").removeAttr('readonly');
  $("#conSubDivisionName").removeAttr('readonly');
  $("#SitaAddress").removeAttr('readonly');
  $("#conPostalCode").removeAttr('readonly');
  $("#FaxNumber").removeAttr('readonly');
  $("#PhoneNumber1").removeAttr('readonly');
  $("#PhoneNumber2").removeAttr('readonly');
  $("#MobileNumber").removeAttr('readonly');

  $("#StaffId").val("");
  $("#Salutation").val("");
  $("#FirstName").val("");
  $("#LastName").val("");
  $("#ContactTypes").val("");
  $("#EmailAddress").val("");
  $("#PositionOrTitle").val("");
  $("#LocationId").val("");
  $("#conCountryId").val("");
  $("#conAddressLine1").val("");
  $("#conAddressLine2").val("");
  $("#conAddressLine3").val("");
  $("#conCityName").val("");
  $("#SubDivisionCode").val("");
  $("#conSubDivisionName").val("");
  $("#conPostalCode").val("");
  $("#PhoneNumber1").val("");
  $('#Division').val("");
  $('#Department').val("");
  $("#PhoneNumber2").val("");
  $("#MobileNumber").val("");
  $("#FaxNumber").val("");
  $("#SitaAddress").val("");
  $("#contactId").val("");
  $('#conIsActive').attr({ "checked": "checked" });
  $('#conCountryId').removeAttr('disabled');
  $("#conAddressLine1").removeAttr('disabled');
  $("#conAddressLine2").removeAttr('disabled');
  $("#conAddressLine3").removeAttr('disabled');
  $("#conCityName").removeAttr('disabled');
  $("#SubDivisionCode").removeAttr('disabled');
  $("#conSubDivisionName").removeAttr('disabled');
  $("#conPostalCode").removeAttr('disabled');
  $("#PhoneNumber1").removeAttr('disabled');
  $("#PhoneNumber2").removeAttr('disabled');
  $("#MobileNumber").removeAttr('disabled');
  $("#FaxNumber").removeAttr('disabled');
  $("#SitaAddress").removeAttr('disabled');
  $('#conCountryId').removeAttr('disabled');
  $('#StaffId').removeAttr('disabled');
  $('#Salutation').removeAttr('disabled');
  $('#FirstName').removeAttr('disabled');
  $('#LastName').removeAttr('disabled');
  $('#PositionOrTitle').removeAttr('disabled');
  $('#IsActive').removeAttr('disabled');
  $('#Division').removeAttr('disabled');
  $('#Department').removeAttr('disabled');
  $('#LocationId').removeAttr('disabled');
  setFocus('#EmailAddress');
}

function DisplayDetails(id) {
  if (id == null) {
  }
  else {
    $.ajax({
      type: "POST",
      url: DisplayDetailsPostUrl,
      data: { contactId: id },
      dataType: "json",
      success: OnContactDetailsPopulated,
      failure: function (response) {
        alert(response.d);
      }
    });
  }
}

function SelectFirstRow() {
  //If last contact deleted successfully then clear the form elements showing information of last record.
  var recs = $("#ContactsGrid").getGridParam("records");
  if (recs == 0)
    clear_form_elements(".tempEdit");
  else {
     var selecRow = jQuery('#ContactsGrid').getGridParam('selrow');
    if (selecRow == null) {            
        var row = jQuery('#ContactsGrid').getDataIDs();        
        var record = $("#ContactsGrid").jqGrid('setSelection', row[0]);
    }
    else {
        $("#ContactsGrid").jqGrid('setSelection', selecRow);        
    }
  }
}

// Clear elements in the given element name.
function clear_form_elements(selector) {
  $(selector).find(':input').each(function () {
    switch (this.type) {
      case 'hidden':
      case 'select-multiple':
      case 'select-one':
      case 'text':
        $(this).val('');
        break;
      case 'checkbox':
      case 'radio':
        this.checked = false;
        break;
    }
  });
}

function OnContactDetailsPopulated(response) {
  if (contactValidator != null) {
    contactValidator.resetForm();
  }

  notIsUser();

  if (response.Id == null)
    $("#contactId").val("");
  else
    $("#contactId").val(response.Id);

  if (response.FirstName == null) {
    checkIfUser(CheckIfUserUrl, UserCitySubdivisionNameUrl, response.EmailAddress, response.IsActive);
  }
  else {
    if (response.StaffId == null)
      $("#StaffId").val("");
    else
      $("#StaffId").val(response.StaffId);

    if (response.Salutation == null)
      $("#Salutation").val("");
    else
      $("#Salutation").val(response.Salutation);

    if (response.FirstName == null)
      $("#FirstName").val("");
    else
      $("#FirstName").val(response.FirstName);

    if (response.LastName == null)
      $("#LastName").val("");
    else
      $("#LastName").val(response.LastName);

    if (response.LastName == null)
      $("#LastName").val("");
    else
      $("#LastName").val(response.LastName);

    $("#conIsActive").prop('checked', response.IsActive);

    if (response.EmailAddress == null) {
      $("#EmailAddress").val("");
      $("#hiddemEmail").val("");
    }
    else {
      $("#EmailAddress").val(response.EmailAddress);
      $("#hiddemEmail").val(response.EmailAddress);
    }

    if (response.PositionOrTitle == null)
      $("#PositionOrTitle").val("");
    else
      $("#PositionOrTitle").val(response.PositionOrTitle);

    if (response.LocationId == 0) {
      $("#LocationId").val("");

      if (($("#UserCategory").val() != 'IchOps') && ($("#UserCategory").val() != 'AchOps')) {
        $('#conCountryId').removeAttr('disabled');
        $("#conAddressLine1").removeAttr('disabled');
        $("#conAddressLine2").removeAttr('disabled');
        $("#conAddressLine3").removeAttr('disabled');
        $("#conCityName").removeAttr('disabled');
        $("#SubDivisionCode").removeAttr('disabled');
        $("#conSubDivisionName").removeAttr('disabled');
        $("#conPostalCode").removeAttr('disabled');
      }
    }
    else {
      $("#conCountryId").val("");
      $("#conCountryId").attr('disabled', 'disabled');
      $("#LocationId").val(response.LocationId);
      $("#conAddressLine1").val("");
      $("#conAddressLine1").attr('disabled', true);
      $("#conAddressLine2").val("");
      $("#conAddressLine2").attr('disabled', true);
      $("#conAddressLine3").val("");
      $("#conAddressLine3").attr('disabled', true);
      $("#conCityName").val("");
      $("#conCityName").attr('disabled', true);
      $("#SubDivisionCode").val("");
      $("#SubDivisionCode").attr('disabled', true);
      $("#conSubDivisionName").val("");
      $("#conSubDivisionName").attr('disabled', true);
      $("#conPostalCode").val("");
      $("#conPostalCode").attr('disabled', true);
    }


    if (response.CountryId == 0)
      $("#conCountryId").val("");

    else
      $("#conCountryId").val(response.CountryId);

    if (response.AddressLine1 == null)
      $("#conAddressLine1").val("");
    else
      $("#conAddressLine1").val(response.AddressLine1);

    if (response.AddressLine2 == null)
      $("#conAddressLine2").val("");
    else
      $("#conAddressLine2").val(response.AddressLine2);

    if (response.AddressLine3 == null)
      $("#conAddressLine3").val("");
    else
      $("#conAddressLine3").val(response.AddressLine3);

    if (response.CityName == null)
      $("#conCityName").val("");
    else
      $("#conCityName").val(response.CityName);

    if (response.SubDivisionCode == null)
      $("#SubDivisionCode").val("");
    else
      $("#SubDivisionCode").val(response.SubDivisionCode);

    if (response.SubDivisionName == null)
      $("#conSubDivisionName").val("");
    else
      $("#conSubDivisionName").val(response.SubDivisionName);

    if (response.PostalCode == null)
      $("#conPostalCode").val("");
    else
      $("#conPostalCode").val(response.PostalCode);

    if (response.PhoneNumber1 == null)
      $("#PhoneNumber1").val("");
    else
      $("#PhoneNumber1").val(response.PhoneNumber1);

    if (response.PhoneNumber2 == null)
      $("#PhoneNumber2").val("");
    else
      $("#PhoneNumber2").val(response.PhoneNumber2);

    if (response.MobileNumber == null)
      $("#MobileNumber").val("");
    else
      $("#MobileNumber").val(response.MobileNumber);

    if (response.FaxNumber == null)
      $("#FaxNumber").val("");
    else
      $("#FaxNumber").val(response.FaxNumber);

    if (response.SitaAddress == null)
      $("#SitaAddress").val("");
    else
      $("#SitaAddress").val(response.SitaAddress);

    if (response.Department == null)
      $("#Department").val("");
    else
      $("#Department").val(response.Department);

    if (response.Division == null)
      $("#Division").val("");
    else
      $("#Division").val(response.Division);
  }

}

function optionExistsInList(lst, val) {
  response = false;
  for (var i = 0; i < lst.length; i++) {
    if (lst.options[i].value == val) {
      response = true;
      break;
    }
  }
  return response;
}

function SponsorMemberCapture() {
    $("#Add").click(function () {
        var select = document.getElementById('availableMembers');
        if ($("#MemberId").val() > 0) {

            var member = $("#MemberId").val();
            var memberName = $("#SponsordMemberText").val();
            var isMemberAlreadyAdded = optionExistsInList(select, member);

            if ((member != $("#hiddenSelfId").val()) && (!isMemberAlreadyAdded)) {
                var tmpMemberId = $("#hiddenMemberIdAdd").val();
                if (tmpMemberId != "") {
                    tmpMemberId = tmpMemberId + "," + member;
                }
                else {
                    tmpMemberId = member;
                }
                $("#hiddenMemberIdAdd").val(tmpMemberId);

                // remove from remove list
                var tmpMemberRemoveId = "";
                var list = $("#hiddenMemberIdRemove").val().split(",");
                for (i = 0; i < list.length; i++) {
                    if (list[i] != member) {
                        if (tmpMemberRemoveId == "")
                            tmpMemberRemoveId = list[i]
                        else
                            tmpMemberRemoveId = tmpMemberRemoveId + "," + list[i];
                    }
                }
                $("#hiddenMemberIdRemove").val(tmpMemberRemoveId);

                var newOption = document.createElement('option');
                newOption.text = memberName; newOption.value = member;
                newOption.id = member;
                select.options.add(newOption);
            }

            // add to select count
            $("#SponsorrAddCount").val(parseInt($("#SponsorrAddCount").val()) + 1);

            $("#MemberId").val("");
            $("#SponsordMemberText").val("");
        }

    });

  $("#remove").click(function () {
      var member = $("#availableMembers").val(); //$("#availableMembers option:selected").val();

      var tmpMemberId = $("#hiddenMemberIdRemove").val();
      if (tmpMemberId != "") {
          tmpMemberId = tmpMemberId + "," + member;
      }
      else {
          tmpMemberId = member;
      }
      $("#hiddenMemberIdRemove").val(tmpMemberId);

      // remove from add list
      var tmpMemberAddId = "";
      var list = $("#hiddenMemberIdAdd").val().split(",");
      for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
              if (tmpMemberAddId == "")
                  tmpMemberAddId = list[i]
              else
                  tmpMemberAddId = tmpMemberAddId + "," + list[i];
          }
      }
      $("#hiddenMemberIdAdd").val(tmpMemberAddId);

      // remove from add list
      var tmpMemberAddId = "";
      var list = $("#hiddenMemberIdAdd").val().split(",");
      for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
              if (tmpMemberAddId == "")
                  tmpMemberAddId = list[i]
              else
                  tmpMemberAddId = tmpMemberAddId + "," + list[i];
          }
      }

      // minus from cnt list for ...
      var v = $('#SpnDisableValues').val();
      if (v.indexOf(member) == -1) {
          $("#SponsorrAddCount").val(parseInt($("#SponsorrAddCount").val()) - 1);
      }

      $("#hiddenMemberIdAdd").val(tmpMemberAddId);
      $("#availableMembers option:selected").remove();
  });
}

function UpdateSponsors(currPeriod, currMonth, currYear) {
  if ($('#spnFuturePeriod').is(':visible')) {
    if (!ValidateFuturePeriod("#spnFuturePeriod")) {
      return false;
    }
  }

  // add cnt list
  if ($("#SponsAvailMembersDisabled").val() == "-1") {
    $("#SponsAvailMembersDisabled").val($('#availableMembersDisabled option').length);
  }

  $("#SponsororAddList").val($("#hiddenMemberIdAdd").val());
  $("#SponsororDeleteList").val($("#hiddenMemberIdRemove").val());






  $("#SponsororFuturePeriod").val($("#spnFuturePeriod").val());









  if (($("#SponsororFuturePeriod").val() != "") && ($("#SponsororFuturePeriod").val() != _periodFormat)) {
    $("#SponsoredFutureDateInd").show();
  }
  else {
    $('#availableMembersDisabled option').each(function (i, option) { $(option).remove(); })
    jQuery('#availableMembers option').clone().appendTo('#availableMembersDisabled');
  }

  $('#SponsororsSelected option').each(function (i, option) { $(option).remove(); })
  jQuery('#availableMembers option').clone().appendTo('#SponsororsSelected');

  //Add data in hidden listbox which will be used for displaying data which user has set but not yet saved into database



  //When future period is not specified, update the sponsored member list in listbox displayed on ICH tab

  var count = document.getElementById("availableMembers").options.length;

  if (count > 0)
    $('#SponseredByText').attr('readonly', 'true');
  else
    $('#SponseredByText').removeAttr("readonly");

  setFutureUpdateFieldValue("#availableMembersDisabled", "#availableMembers", "#SponsororFutureValue");
  // Set the form as dirty and close the dialog.
  $parentForm.setDirty();
  return true;
}

function UpdateAggregators(currPeriod, currMonth, currYear) {
  if ($('#aggFuturePeriod').is(':visible')) {
    if (!ValidateFuturePeriod("#aggFuturePeriod")) {
      return false;
    }
  }


  // add cnt list
  if ($("#AggrAvailMembersDisabled").val() == "-1") {

    $("#AggrAvailMembersDisabled").val($('#aggrAvailableMembersDisabled option').length);
  }

  $("#AggregatorAddList").val($("#hiddenAggrMemberIdAdd").val());
  $("#AggregatorDeleteList").val($("#hiddenAggrMemberIdRemove").val());
  $("#AggregatorFuturePeriod").val($("#aggFuturePeriod").val());

  if (($("#AggregatorFuturePeriod").val() != "") && ($("#AggregatorFuturePeriod").val() != _periodFormat)) {
    $("#AggregatorFutureDateInd").show();
  }
  else {
    $('#aggrAvailableMembersDisabled option').each(function (i, option) { $(option).remove(); })
    jQuery('#aggrAvailableMembers option').clone().appendTo('#aggrAvailableMembersDisabled');
  }

  $('#AggregatorsSelected option').each(function (i, option) { $(option).remove(); })
  jQuery('#aggrAvailableMembers option').clone().appendTo('#AggregatorsSelected');


  setFutureUpdateFieldValue("#aggrAvailableMembersDisabled", "#aggrAvailableMembers", "#AggregatorFutureValue");

  // Set the form as dirty and close the dialog.
  $parentForm.setDirty();
  return true;
}

function AggregatorMemberCapture() {
  $("#addAggregator").click(function () {
    var select = document.getElementById('aggrAvailableMembers');
    if ($("#IchMemberId").val() > 0) {
      var member = $("#IchMemberId").val();
      var memberName = $("#AggreagatedMemberText").val();
      var isMemberAlreadyAdded = optionExistsInList(select, member);

      if ((member != $("#hiddenAggrSelfId").val()) && (!isMemberAlreadyAdded)) {
        var tmpMemberId = $("#hiddenAggrMemberIdAdd").val();
        if (tmpMemberId != "") {
          tmpMemberId = tmpMemberId + "," + member;
        }
        else {
          tmpMemberId = member;
        }
        $("#hiddenAggrMemberIdAdd").val(tmpMemberId);

        // remove from remove list
        var tmpMemberRemoveId = "";
        var list = $("#hiddenAggrMemberIdRemove").val().split(",");
        for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
            if (tmpMemberRemoveId == "")
              tmpMemberRemoveId = list[i]
            else
              tmpMemberRemoveId = tmpMemberRemoveId + "," + list[i];
          }
        }
        $("#hiddenAggrMemberIdRemove").val(tmpMemberRemoveId);

        var newOption = document.createElement('option');
        newOption.text = memberName;
        newOption.value = member;
        newOption.id = member;
        select.options.add(newOption);

        // add to select count
        $("#AggregatorAddCount").val(parseInt($("#AggregatorAddCount").val()) + 1);

      }
      $("#IchMemberId").val("");
      $("#AggreagatedMemberText").val("");
    }
  });


  $("#removeAggregator").click(function () {

    var member = $("#aggrAvailableMembers").val();

    var tmpMemberId = $("#hiddenAggrMemberIdRemove").val();
    if (tmpMemberId != "") {
      tmpMemberId = tmpMemberId + "," + member;
    }
    else {
      tmpMemberId = member;
    }
    $("#hiddenAggrMemberIdRemove").val(tmpMemberId);

    // remove from add list
    var tmpMemberAddId = "";
    var list = $("#hiddenAggrMemberIdAdd").val().split(",");
    for (i = 0; i < list.length; i++) {
      if (list[i] != member) {
        if (tmpMemberAddId == "")
          tmpMemberAddId = list[i]
        else
          tmpMemberAddId = tmpMemberAddId + "," + list[i];
      }
    }


    $("#aggrAvailableMembers option:selected").remove();

    // minus from cnt list for ..
    var v = $('#AggDisableValues').val();
    // check removing item should not be from disable agg list
    if (v.indexOf(member) == -1) {
      $("#AggregatorAddCount").val(parseInt($("#AggregatorAddCount").val()) - 1);
    }

    $("#hiddenAggrMemberIdAdd").val(tmpMemberAddId);
  });
}

function AddMember() {
  var billingCategory = $("#BillingCategory").val();

  if (billingCategory == 'pax') {
    if ($("#exceptionMemberId").val() > 0) {
      var selectPaxMember = document.getElementById('exceptionMemberspax');
      var member = $("#exceptionMemberId").val();
      var isMemberAlreadyAdded = optionExistsInList(selectPaxMember, member);
      if ((member != $("#hiddenSelfId").val()) && (!isMemberAlreadyAdded)) {
        var memberName = $("#exceptionMemberText").val();
        var tmpMemberId = $("#hiddenpaxMemberIdAdd").val();
        if (tmpMemberId != "") {
          tmpMemberId = tmpMemberId + "," + member;
        }
        else {
          tmpMemberId = member;
        }
        $("#hiddenpaxMemberIdAdd").val(tmpMemberId);

        // remove from remove list
        var tmpMemberRemoveId = "";
        var list = $("#hiddenpaxMemberIdRemove").val().split(",");
        for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
            if (tmpMemberRemoveId == "")
              tmpMemberRemoveId = list[i]
            else
              tmpMemberRemoveId = tmpMemberRemoveId + "," + list[i];
          }
        }
        $("#hiddenpaxMemberIdRemove").val(tmpMemberRemoveId);

        var newOption = document.createElement('option');
        newOption.text = memberName; newOption.value = member;
        newOption.id = member;
        selectPaxMember.options.add(newOption);
      }
      $("#exceptionMemberId").val("");
    }
  }

  if (billingCategory == 'cgo') {
    if ($("#exceptionMemberId").val() > 0) {
      var selectCgoMember = document.getElementById('exceptionMemberscgo');
      var member = $("#exceptionMemberId").val();
      var isMemberAlreadyAdded = optionExistsInList(selectCgoMember, member);
      if ((member != $("#hiddenSelfId").val()) && (!isMemberAlreadyAdded)) {

        var memberName = $("#exceptionMemberText").val();
        var tmpMemberId = $("#hiddencgoMemberIdAdd").val();
        if (tmpMemberId != "") {
          tmpMemberId = tmpMemberId + "," + member;
        }
        else {
          tmpMemberId = member;
        }
        $("#hiddencgoMemberIdAdd").val(tmpMemberId);

        // remove from remove list
        var tmpMemberRemoveId = "";
        var list = $("#hiddencgoMemberIdRemove").val().split(",");
        for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
            if (tmpMemberRemoveId == "")
              tmpMemberRemoveId = list[i]
            else
              tmpMemberRemoveId = tmpMemberRemoveId + "," + list[i];
          }
        }
        $("#hiddencgoMemberIdRemove").val(tmpMemberRemoveId);

        var newOption = document.createElement('option');
        newOption.text = memberName; newOption.value = member;
        newOption.id = member;
        selectCgoMember.options.add(newOption);
      }
      $("#exceptionMemberId").val("");
    }
  }

  if (billingCategory == 'misc') {
    if ($("#exceptionMemberId").val() > 0) {
      var selectMiscMember = document.getElementById('exceptionMembersmisc');
      var member = $("#exceptionMemberId").val();
      var isMemberAlreadyAdded = optionExistsInList(selectMiscMember, member);
      if ((member != $("#hiddenSelfId").val()) && (!isMemberAlreadyAdded)) {
        var memberName = $("#exceptionMemberText").val();
        var tmpMemberId = $("#hiddenmiscMemberIdAdd").val();
        if (tmpMemberId != "") {
          tmpMemberId = tmpMemberId + "," + member;
        }
        else {
          tmpMemberId = member;
        }
        $("#hiddenmiscMemberIdAdd").val(tmpMemberId);

        // remove from remove list
        var tmpMemberRemoveId = "";
        var list = $("#hiddenmiscMemberIdRemove").val().split(",");
        for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
            if (tmpMemberRemoveId == "")
              tmpMemberRemoveId = list[i]
            else
              tmpMemberRemoveId = tmpMemberRemoveId + "," + list[i];
          }
        }
        $("#hiddenmiscMemberIdRemove").val(tmpMemberRemoveId);


        var newOption = document.createElement('option');
        newOption.text = memberName; newOption.value = member;
        newOption.id = member;
        selectMiscMember.options.add(newOption);
      }
      $("#exceptionMemberId").val("");
    }
  }
  if (billingCategory == 'uatp') {
    if ($("#exceptionMemberId").val() > 0) {
      var selectUatpMember = document.getElementById('exceptionMembersuatp');
      var member = $("#exceptionMemberId").val();
      var isMemberAlreadyAdded = optionExistsInList(selectUatpMember, member);
      if ((member != $("#hiddenSelfId").val()) && (!isMemberAlreadyAdded)) {
        var memberName = $("#exceptionMemberText").val();
        var tmpMemberId = $("#hiddenuatpMemberIdAdd").val();
        if (tmpMemberId != "") {
          tmpMemberId = tmpMemberId + "," + member;
        }
        else {
          tmpMemberId = member;
        }
        $("#hiddenuatpMemberIdAdd").val(tmpMemberId);

        // remove from remove list
        var tmpMemberRemoveId = "";
        var list = $("#hiddenuatpMemberIdRemove").val().split(",");
        for (i = 0; i < list.length; i++) {
          if (list[i] != member) {
            if (tmpMemberRemoveId == "")
              tmpMemberRemoveId = list[i]
            else
              tmpMemberRemoveId = tmpMemberRemoveId + "," + list[i];
          }
        }
        $("#hiddenuatpMemberIdRemove").val(tmpMemberRemoveId);

        var newOption = document.createElement('option');
        newOption.text = memberName; newOption.value = member;
        newOption.id = member;
        selectUatpMember.options.add(newOption);
      }
      $("#exceptionMemberId").val("");
    }
  }
  $("#exceptionMemberText").val("");
}

function DeleteMember() {
  var billingCategory = $("#BillingCategory").val();

  if (billingCategory == 'pax') {
    var member = $("#exceptionMemberspax option:selected").val();
    var tmpMemberId = $("#hiddenpaxMemberIdRemove").val();
    if (tmpMemberId != "") {
      tmpMemberId = tmpMemberId + "," + member;
    }
    else {
      tmpMemberId = member;
    }
    $("#hiddenpaxMemberIdRemove").val(tmpMemberId);

    // remove from add list
    var tmpMemberAddId = "";
    var list = $("#hiddenpaxMemberIdAdd").val().split(",");
    for (i = 0; i < list.length; i++) {
      if (list[i] != member) {
        if (tmpMemberAddId == "")
          tmpMemberAddId = list[i]
        else
          tmpMemberAddId = tmpMemberAddId + "," + list[i];
      }
    }
    $("#hiddenpaxMemberIdAdd").val(tmpMemberAddId);

    $("#exceptionMemberspax option:selected").remove();
  }


  if (billingCategory == 'cgo') {

    var member = $("#exceptionMemberscgo option:selected").val();
    var tmpMemberId = $("#hiddencgoMemberIdRemove").val();
    if (tmpMemberId != "") {
      tmpMemberId = tmpMemberId + "," + member;
    }
    else {
      tmpMemberId = member;
    }
    $("#hiddencgoMemberIdRemove").val(tmpMemberId);

    // remove from add list
    var tmpMemberAddId = "";
    var list = $("#hiddencgoMemberIdAdd").val().split(",");
    for (i = 0; i < list.length; i++) {
      if (list[i] != member) {
        if (tmpMemberAddId == "")
          tmpMemberAddId = list[i]
        else
          tmpMemberAddId = tmpMemberAddId + "," + list[i];
      }
    }
    $("#hiddencgoMemberIdAdd").val(tmpMemberAddId);

    $("#exceptionMemberscgo option:selected").remove();
  }

  if (billingCategory == 'misc') {

    var member = $("#exceptionMembersmisc option:selected").val();
    var tmpMemberId = $("#hiddenmiscMemberIdRemove").val();
    if (tmpMemberId != "") {
      tmpMemberId = tmpMemberId + "," + member;
    }
    else {
      tmpMemberId = member;
    }
    $("#hiddenmiscMemberIdRemove").val(tmpMemberId);

    // remove from add list
    var tmpMemberAddId = "";
    var list = $("#hiddenmiscMemberIdAdd").val().split(",");
    for (i = 0; i < list.length; i++) {
      if (list[i] != member) {
        if (tmpMemberAddId == "")
          tmpMemberAddId = list[i]
        else
          tmpMemberAddId = tmpMemberAddId + "," + list[i];
      }
    }
    $("#hiddenmiscMemberIdAdd").val(tmpMemberAddId);

    $("#exceptionMembersmisc option:selected").remove();
  }

  if (billingCategory == 'uatp') {

    var member = $("#exceptionMembersuatp option:selected").val();
    var tmpMemberId = $("#hiddenuatpMemberIdRemove").val();
    if (tmpMemberId != "") {
      tmpMemberId = tmpMemberId + "," + member;
    }
    else {
      tmpMemberId = member;
    }
    $("#hiddenuatpMemberIdRemove").val(tmpMemberId);

    // remove from add list
    var tmpMemberAddId = "";
    var list = $("#hiddenuatpMemberIdAdd").val().split(",");
    for (i = 0; i < list.length; i++) {
      if (list[i] != member) {
        if (tmpMemberAddId == "")
          tmpMemberAddId = list[i]
        else
          tmpMemberAddId = tmpMemberAddId + "," + list[i];
      }
    }
    $("#hiddenuatpMemberIdAdd").val(tmpMemberAddId);

    $("#exceptionMembersuatp option:selected").remove();
  }
}

function showExceptionDialog(billingCategory, currPeriod, currMonth, currYear) {
  $('#divExceptionMemberList').dialog({
    title: 'Settlement with ICH?',
    modal: true,
    resizable: false,
    open: function (event, ui) {

      $("#BillingCategory").val(billingCategory);

      if (billingCategory == 'pax') {
        $("#PaxExceptionFuturePeriod").watermark(_periodFormat);
        var periodValue = $('#PaxExceptionFuturePeriod').val();
        if (!periodValue || periodValue == _periodFormat) {
          //if ($('#exceptionMemberspaxDisabled option').length > 0 || $('exceptionMemberspaxFutureDateInd').is(':visible'))
          if ($('exceptionMemberspaxFutureDateInd').is(':visible'))
            $('#PaxExceptionFuturePeriod').val($('#nextPeriod').val());

          if ($('#PaxExceptionFuturePeriod').is(':visible') && $('#PaxExceptionFuturePeriod').val() == '') {
            $('#PaxExceptionFuturePeriod').val($('#nextPeriod').val());
          }
        }


        $("#exceptionMemberspax").show();
        $("#exceptionMembersmisc").hide();
        $("#exceptionMembersuatp").hide();
        $("#exceptionMemberscgo").hide();
        $("#paxExceptionsdiv").show();
        $("#cgoExceptionsdiv").hide();
        $("#miscExceptionsdiv").hide();
        $("#uatpExceptionsdiv").hide();

        $('#exceptionMemberspax option').each(function (i, option) { $(option).remove(); })
        jQuery('#exceptionMemberspaxSelected option').clone().appendTo('#exceptionMemberspax');
      }
      if (billingCategory == 'cgo') {
        $("#CgoExceptionFuturePeriod").watermark(_periodFormat);

        var periodValue = $('#CgoExceptionFuturePeriod').val();
        if (!periodValue || periodValue == _periodFormat) {
          //if ($('#exceptionMemberscgoDisabled option').length > 0 || $('exceptionMemberscgoFutureDateInd').is(':visible'))
          if ($('exceptionMemberscgoFutureDateInd').is(':visible'))
            $('#CgoExceptionFuturePeriod').val($('#nextPeriod').val());
          if ($('#CgoExceptionFuturePeriod').is(':visible') && $('#CgoExceptionFuturePeriod').val() == '') {
            $('#CgoExceptionFuturePeriod').val($('#nextPeriod').val());
          }
        }
        $("#exceptionMembersmisc").hide();
        $("#exceptionMembersuatp").hide();
        $("#exceptionMemberspax").hide();
        $("#exceptionMemberscgo").show();
        $("#paxExceptionsdiv").hide();
        $("#cgoExceptionsdiv").show();
        $("#miscExceptionsdiv").hide();
        $("#uatpExceptionsdiv").hide();

        $('#exceptionMemberscgo option').each(function (i, option) { $(option).remove(); })
        jQuery('#exceptionMemberscgoSelected option').clone().appendTo('#exceptionMemberscgo');
      }


      if (billingCategory == 'misc') {
        $("#MiscExceptionFuturePeriod").watermark(_periodFormat);
        var periodValue = $('#MiscExceptionFuturePeriod').val();
        if (!periodValue || periodValue == _periodFormat) {
          //if ($('#exceptionMembersmiscDisabled option').length > 0 || $('exceptionMembersmiscFutureDateInd').is(':visible'))
          if ($('exceptionMembersmiscFutureDateInd').is(':visible'))
            $('#MiscExceptionFuturePeriod').val($('#nextPeriod').val());
          if ($('#MiscExceptionFuturePeriod').is(':visible') && $('#MiscExceptionFuturePeriod').val() == '') {
            $('#MiscExceptionFuturePeriod').val($('#nextPeriod').val());
          }
        }
        $("#exceptionMembersuatp").hide();
        $("#exceptionMemberspax").hide();
        $("#exceptionMemberscgo").hide();
        $("#exceptionMembersmisc").show();
        $("#paxExceptionsdiv").hide();
        $("#cgoExceptionsdiv").hide();
        $("#miscExceptionsdiv").show();
        $("#uatpExceptionsdiv").hide();

        $('#exceptionMembersmisc option').each(function (i, option) { $(option).remove(); })
        jQuery('#exceptionMembersmiscSelected option').clone().appendTo('#exceptionMembersmisc');
      }
      if (billingCategory == 'uatp') {
        $("#UatpExceptionFuturePeriod").watermark(_periodFormat);
        var periodValue = $('#UatpExceptionFuturePeriod').val();
        if (!periodValue || periodValue == _periodFormat) {
          //if ($('#exceptionMembersuatpDisabled option').length > 0 || $('exceptionMembersuatpFutureDateInd').is(':visible'))
          if ($('exceptionMembersuatpFutureDateInd').is(':visible'))
            $('#UatpExceptionFuturePeriod').val($('#nextPeriod').val());
          if ($('#UatpExceptionFuturePeriod').is(':visible') && $('#UatpExceptionFuturePeriod').val() == '') {
            $('#UatpExceptionFuturePeriod').val($('#nextPeriod').val());
          }
        }
        $("#exceptionMemberspax").hide();
        $("#exceptionMemberscgo").hide();
        $("#exceptionMembersmisc").hide();
        $("#exceptionMembersuatp").show();
        $("#paxExceptionsdiv").hide();
        $("#cgoExceptionsdiv").hide();
        $("#miscExceptionsdiv").hide();
        $("#uatpExceptionsdiv").show();

        $('#exceptionMembersuatp option').each(function (i, option) { $(option).remove(); })
        jQuery('#exceptionMembersuatpSelected option').clone().appendTo('#exceptionMembersuatp');

      }
    },

    buttons: {

      Save: {
        className: 'primaryButton',
        text: 'Save',
        click: function () {
          
          $("#BillingCategory").val(billingCategory);



          if ($('#PaxExceptionFuturePeriod').is(':visible')) {
            if (!ValidateFuturePeriod('#PaxExceptionFuturePeriod')) {
              return false;
            }

          }


          if ($('#CgoExceptionFuturePeriod').is(':visible')) {
            if (!ValidateFuturePeriod('#CgoExceptionFuturePeriod')) {
              return false;
            }
          }
          if ($('#MiscExceptionFuturePeriod').is(':visible')) {
            if (!ValidateFuturePeriod('#MiscExceptionFuturePeriod')) {
              return false;
            }
          }

          if ($('#UatpExceptionFuturePeriod').is(':visible')) {
            if (!ValidateFuturePeriod('#UatpExceptionFuturePeriod')) {
              return false;
            }

          }

          if (billingCategory == 'pax') {
            $("#PaxExceptionMemberAddList").val($("#hiddenpaxMemberIdAdd").val());
            $("#PaxExceptionMemberDeleteList").val($("#hiddenpaxMemberIdRemove").val());
            $("#hidPaxExceptionFuturePeriod").val($("#PaxExceptionFuturePeriod").val());
            if (($("#hidPaxExceptionFuturePeriod").val() != "") && ($("#hidPaxExceptionFuturePeriod").val() != _periodFormat)) {
              $("#exceptionMemberspaxFutureDateInd").show();
            }
            else {
              $('#exceptionMemberspaxDisabled option').each(function (i, option) { $(option).remove(); })
              jQuery('#exceptionMemberspax option').clone().appendTo('#exceptionMemberspaxDisabled');
            }

            $('#exceptionMemberspaxSelected option').each(function (i, option) { $(option).remove(); })
            jQuery('#exceptionMemberspax option').clone().appendTo('#exceptionMemberspaxSelected');

            setFutureUpdateFieldValue("#exceptionMemberspaxDisabled", "#exceptionMemberspax", "#PaxExceptionFutureValue");
          }
          if (billingCategory == 'cgo') {
            $("#CgoExceptionMemberAddList").val($("#hiddencgoMemberIdAdd").val());
            $("#CgoExceptionMemberDeleteList").val($("#hiddencgoMemberIdRemove").val());
            $("#hidCgoExceptionFuturePeriod").val($("#CgoExceptionFuturePeriod").val());
            if (($("#hidCgoExceptionFuturePeriod").val() != "") && ($("#hidCgoExceptionFuturePeriod").val() != _periodFormat)) {
              $("#exceptionMemberscgoFutureDateInd").show();
            }
            else {
              $('#exceptionMemberscgoDisabled option').each(function (i, option) { $(option).remove(); })
              jQuery('#exceptionMemberscgo option').clone().appendTo('#exceptionMemberscgoDisabled');
            }

            $('#exceptionMemberscgoSelected option').each(function (i, option) { $(option).remove(); })
            jQuery('#exceptionMemberscgo option').clone().appendTo('#exceptionMemberscgoSelected');

            setFutureUpdateFieldValue("#exceptionMemberscgoDisabled", "#exceptionMemberscgo", "#CgoExceptionFutureValue");


          }
          if (billingCategory == 'misc') {
            $("#MiscExceptionMemberAddList").val($("#hiddenmiscMemberIdAdd").val());
            $("#MiscExceptionMemberDeleteList").val($("#hiddenmiscMemberIdRemove").val());
            $("#hidMiscExceptionFuturePeriod").val($("#MiscExceptionFuturePeriod").val());
            if (($("#hidMiscExceptionFuturePeriod").val() != "") && ($("#hidMiscExceptionFuturePeriod").val() != _periodFormat)) {
              $("#exceptionMembersmiscFutureDateInd").show();
            }
            else {
              $('#exceptionMembersmiscDisabled option').each(function (i, option) { $(option).remove(); })
              jQuery('#exceptionMembersmisc option').clone().appendTo('#exceptionMembersmiscDisabled');
            }

            $('#exceptionMembersmiscSelected option').each(function (i, option) { $(option).remove(); })
            jQuery('#exceptionMembersmisc option').clone().appendTo('#exceptionMembersmiscSelected');

            setFutureUpdateFieldValue("#exceptionMembersmiscDisabled", "#exceptionMembersmisc", "#MiscExceptionFutureValue");
          }
          if (billingCategory == 'uatp') {
            $("#UatpExceptionMemberAddList").val($("#hiddenuatpMemberIdAdd").val());
            $("#UatpExceptionMemberDeleteList").val($("#hiddenuatpMemberIdRemove").val());
            $("#hidUatpExceptionFuturePeriod").val($("#UatpExceptionFuturePeriod").val());
            if (($("#hidUatpExceptionFuturePeriod").val() != "") && ($("#hidUatpExceptionFuturePeriod").val() != _periodFormat)) {
              $("#exceptionMembersuatpFutureDateInd").show();
            }
            else {
              $('#exceptionMembersuatpDisabled option').each(function (i, option) { $(option).remove(); })
              jQuery('#exceptionMembersuatp option').clone().appendTo('#exceptionMembersuatpDisabled');
            }

            $('#exceptionMembersuatpSelected option').each(function (i, option) { $(option).remove(); })
            jQuery('#exceptionMembersuatp option').clone().appendTo('#exceptionMembersuatpSelected');

            setFutureUpdateFieldValue("#exceptionMembersuatpDisabled", "#exceptionMembersuatp", "#UatpExceptionFutureValue");
          }
          $("#exceptionMemberText").val("");

          // Set the form as dirty and close the dialog.
          $parentForm.setDirty();
          $(this).dialog('close');
        }
      },
      Close: {
        className: 'secondaryButton',
        text: 'Close',
        click: function () {
          $(this).dialog('close');
        }
      }

    }
  });


  return false;
}

function generateAuditTrailReport(formId) {
  var newArray = new Array();
  var elementList = '';
  var tabname = "#" + formId + " :checkbox";
  var elements = $(tabname);
  for (i = 0; i < elements.length; i++) {
    if (elements[i].type == 'checkbox') {
      elementList = elementList + "!" + elements[i].id + "|" + elements[i].checked;
    }
  }
  $("#" + formId + "ElementList").val(elementList);
  var formData = $("#" + formId).serializeArray();

  $.ajax({
    url: '/AuditTrail/AuditTrail',
    type: "POST",
    data: formData,
    success: function (result) {
      if (result.isRedirect)
        window.location.href = result.RedirectUrl;

    }
  });
}

function getmembercontactsData(contactTypeCategoryval, columns) {
  $.ajax({
    type: "POST",
    data: { contactTypeCategory: contactTypeCategoryval, columns: columns },
    url: memContactsDatagetUrl,
    dataType: "json",
    failure: function (response) {
      alert(response.d);
    }
  });
}

function MigrationPeriodValidation(textboxval) {
  var datecheck = $(textboxval).val();
  var arr_d1 = datecheck.split("-");
  var flag = false;
  day = arr_d1[2];
  month = arr_d1[1];
  year = arr_d1[0];
  var d = new Date();
  var curr_year = d.getFullYear();
  var periodarray = new Array("01", "02", "03", "04");
  var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN",
"JUL", "AUG", "SEP", "OCT", "NOV", "DEC");

  for (var a = 0; a < periodarray.length; a++) {
    if (periodarray[a] == day) {
      for (var i = 0; i < monthArray.length; i++) {
        if (monthArray[i] == month.toUpperCase()) {
          if (year == curr_year) {
            flag = true;
          }
        }
      }
    }
  }

  if (flag == false) {
    alert("Please Enter valid Date.");
    return false;
  }
  return flag;
}

// Function used for validating future period value
function ValidateFuturePeriod(element) {
  var datecheck = $(element).val();
  var arr_d1 = datecheck.split("-");
  var flag = true;
  var validDay = false;
  var validMonth = false;
  var isBackDatedPeriod = false;
  day = arr_d1[2];
  month = arr_d1[1];
  year = arr_d1[0];

  var curr_period = $('#currPeriod').val();
  var curr_month = $('#currMonth').val();
  var curr_year = $('#currYear').val();

  var periodarray = new Array("01", "02", "03", "04");
  var monthArray = new Array("MON", "JAN", "FEB", "MAR", "APR", "MAY", "JUN",
"JUL", "AUG", "SEP", "OCT", "NOV", "DEC");

  for (var a = 0; a < periodarray.length; a++) {
    if (periodarray[a] == day) {
      validDay = true;
      for (var i = 0; i < monthArray.length; i++) {
        if (monthArray[i] == month.toUpperCase()) {
          validMonth = true;
          //Validate whether year value entered is greater than or equal to current year value
          if (year < curr_year) {
            flag = false;
            isBackDatedPeriod = true;
          }

          if ((flag == true) && (year == curr_year) && (i < curr_month)) {
            flag = false;
            isBackDatedPeriod = true;
          }
          if ((flag == true) && (year == curr_year) && (i == curr_month) && ((a + 1) <= curr_period)) {
            flag = false;
            isBackDatedPeriod = true;
          }
        }
      }
    }
  }

  //Indicates 
  if ((validDay == false) || (validMonth == false)) {
    flag = false;
  }
  if (flag == false) {
    if (isBackDatedPeriod == true)
      alert("Please enter future period");
    else
      alert("Effective period should be specified as YYYY-MMM-PP");
  }
  return flag;
}

//function for confirming future reinstatement period
function confirmReinstatementPeriod() {
  if (confirm("Member would be reinstated from specified reinstatement period.Do you want to continue?")) {
    return true;
  }

  return false;
}

// function for displaying future update details on image icon click
function displayFutureUpdateDetails(fieldValue, hasFuturePeriod, fieldType) {
    $('#divfutureUpdateInformation').dialog({
        title: 'Future Value',
        modal: true,
        resizable: false,
        width: 100,
        open: function () {
          //CMP-689-Flexible CH Activation Options
          // added two new fieldtype 19 and 20 which are extension of 16 and 17 fieldtype
          if ((fieldType == 8) || (fieldType == 9) || (fieldType == 10) || (fieldType == 11) || (fieldType == 12) || (fieldType == 13) || (fieldType == 14) || (fieldType == 15) || (fieldType == 16) || (fieldType == 17) || (fieldType == 18) || (fieldType == 19) || (fieldType == 20)) {
                $("#updatedValue").html($(fieldValue + 'FutureDisplayValue').val());
            }
            else if (fieldType == 2) {
                var value = $(fieldValue + 'FutureValue').val();
                if (value != "") {
                    $("#updatedValue").html(value);
                }
                else {
                    $("#updatedValue").html("false");
                };

            }
            else if (fieldType == 'Reinstatement') {
                $("#updatedValue").html($(fieldValue).val());
                $("#effectiveFrom").text($(fieldValue).val());
            }
            else {
                $("#updatedValue").html($(fieldValue + 'FutureValue').val());
            }

            if (hasFuturePeriod == 1) {
                $("#effectiveFrom").text($(fieldValue + 'FuturePeriod').val());
            }
            else if (hasFuturePeriod == 0) {
                $("#effectiveFrom").text($(fieldValue + 'FutureDate').val());
            }

        }
    });
}

// Function used for validating future Date value
function ValidateFutureDate(element) {
  return isValidDate(element, _dateFormat)
}

function isValidDate(controlName, format) {
  var isValid = true;
  var dateValue = $(controlName).val();
  if (dateValue) {
    try {
      jQuery.datepicker.parseDate(format, dateValue, null);
    }
    catch (error) {
      isValid = false;
    }
  } else {
    isValid = false;
  }

  if (isValid == false) {
    alert("Please Enter valid date value.");
  }
  return isValid;
}

function copyContacts(postUrl) {

  var oldcontactId = $('#copyoldcontact').val();
  var newcontactId = $('#copynewcontact').val();

  if (oldcontactId == "" || newcontactId == "")
    alert("Please select contact");
  else if (oldcontactId == newcontactId)
    alert("Copy Contact Assignments of user and New Contact Person can not be same");
  else {
    $.ajax({
      type: "POST",
      url: postUrl,
      data: { oldContactId: oldcontactId, newContactId: newcontactId },
      dataType: "json",
      success: function (result) {
        if (result == false) {
          alert("User specified as “Copy contact assignments of user’ is  not assigned as a contact for any role");
        }
        else {
          alert("All the contact assignments of " + $("#copyoldcontact option:selected").text() + "  have been successfully copied to " + $("#copynewcontact option:selected").text());
        }
      }
    });
  }
}

function ReplaceContact(postUrl) {

  var oldcontactId = $('#replaceoldcontact').val();
  var newcontactId = $('#replacenewcontact').val();

  if (oldcontactId == "" || newcontactId == "")
    alert("Please select contact");
  else if (oldcontactId == newcontactId)
    alert("Current Contact person and New Contact Person can not be same");
  else {
    $.ajax({
      type: "POST",
      url: postUrl,
      data: { oldContactId: oldcontactId, newContactId: newcontactId },
      dataType: "json",
      success: function (result) {
        if (result == false) {
          alert("Specified current user not assigned as a contact for any role");
        }
        else {
          alert($("#replaceoldcontact option:selected").text() + " has been successfully replaced by " + $("#replacenewcontact option:selected").text() + " as the new contact person");
        }
      }
    });
  }
}

function SiftListItems(sourceId, destinationId, addList, removeList) {
  var countryId = "";
  $("#" + sourceId + "  option:selected").each(function (i, selected) {
    if (countryId != undefined)
      countryId = countryId + $(selected).val() + ",";
    else
      countryId = $(selected).val() + ",";
  });

  countryId = countryId.slice(0, -1);



  var tmpcountryId = $("#" + addList).val();
  if (tmpcountryId != "") {
    tmpcountryId = tmpcountryId + "," + countryId;
  }
  else {
    tmpcountryId = countryId;
  }
  $("#" + addList).val(tmpcountryId);

  // remove from remove list
  var tmpcountryRemoveId = "";
  var list1 = $("#" + removeList).val();
  var list = list1.split(",");
  for (i = 0; i < list.length; i++) {
    if (list[i] != countryId) {
      if (tmpcountryRemoveId == "")
        tmpcountryRemoveId = list[i]
      else
        tmpcountryRemoveId = tmpcountryRemoveId + "," + list[i];
    }
  }
  $("#" + removeList).val(tmpcountryRemoveId);

  $("#" + sourceId + "  option:selected").appendTo("#" + destinationId);

}

function SiftListItemsBack(sourceId, destinationId, addList, removeList) {
  var countryId = "";
  $("#" + sourceId + "  option:selected").each(function (i, selected) {
    if (countryId != undefined)
      countryId = countryId + $(selected).val() + ",";
    else
      countryId = $(selected).val() + ",";

  });

  countryId = countryId.slice(0, -1);
  var tmpcountryId = $("#" + removeList).val();
  if (tmpcountryId != "") {
    tmpcountryId = tmpcountryId + "," + countryId;
  }
  else {
    tmpcountryId = countryId;
  }
  $("#" + removeList).val(tmpcountryId);

  // remove from add list
  var tmpcountryAddId = "";
  var list = $("#" + addList).val().split(",");
  for (i = 0; i < list.length; i++) {
    if (list[i] != countryId) {
      if (tmpcountryAddId == "")
        tmpcountryAddId = list[i]
      else
        tmpcountryAddId = tmpcountryAddId + "," + list[i];
    }
  }
  $("#" + addList).val(tmpcountryAddId);
  $("#" + sourceId + "  option:selected").appendTo("#" + destinationId);
}

function SaveBillingDsRequiredCountries(currPeriod, currMonth, currYear) {
  var itemList = $("#BillingMemberDSSupportedByAtosTo" + " option");

  if ($('#BillingFuturePeriod').is(':visible')) {
    if (!ValidateFuturePeriod("#BillingFuturePeriod")) {
      return false;
    }
    else {
      $("#DSReqCountriesAsBillingFuturePeriod").val($("#BillingFuturePeriod").val());
      if ($("#DSReqCountriesAsBillingFuturePeriod").val() != "") {
        $("#BillingMemberFutureDateInd").show();
      }
    }
  }
  else {
    $('#BillingMemberDSSupportedByAtosToDisabled option').each(function (i, option) { $(option).remove(); })
    jQuery('#BillingMemberDSSupportedByAtosTo option').clone().appendTo('#BillingMemberDSSupportedByAtosToDisabled');
  }
  $("#BillingCountiesToAdd").val($("#hiddenBillingCountryIdAdd").val());
  $("#BillingCountiesToRemove").val($("#hiddenBillingCountryIdRemove").val());

  $('#BillingMemberDSSupportedByAtosFromHidden option').each(function (i, option) { $(option).remove(); })
  jQuery('#BillingMemberDSSupportedByAtosFrom option').clone().appendTo('#BillingMemberDSSupportedByAtosFromHidden');

  $('#BillingMemberDSSupportedByAtosToHidden option').each(function (i, option) { $(option).remove(); })
  jQuery('#BillingMemberDSSupportedByAtosTo option').clone().appendTo('#BillingMemberDSSupportedByAtosToHidden');

  setFutureUpdateFieldValue("#BillingMemberDSSupportedByAtosToDisabled", "#BillingMemberDSSupportedByAtosTo", "#BillingFutureValue");

  /*JqueryUI: BillingDsRequiredCountries Dialog Save Issue*/
  //$(this).dialog('close');

  return true;
}

function SaveBilledDsRequiredCountries(currPeriod, currMonth, currYear) {

  var itemList = $("#BilledMemberDSSupportedByAtosTo" + " option");

  if ($('#BilledFuturePeriod').is(':visible')) {
    if (!ValidateFuturePeriod("#BilledFuturePeriod")) {
      return false;
    }
    else {
      $("#DSReqCountriesAsBilledFuturePeriod").val($("#BilledFuturePeriod").val());
      if ($("#DSReqCountriesAsBilledFuturePeriod").val() != "") {
        $("#BilledMemberFutureDateInd").show();
      }
    }
  }
  else {
    $('#BilledMemberDSSupportedByAtosToDisabled option').each(function (i, option) { $(option).remove(); })
    jQuery('#BilledMemberDSSupportedByAtosTo option').clone().appendTo('#BilledMemberDSSupportedByAtosToDisabled');
  }
  $("#BilledCountiesToAdd").val($("#hiddenCountryIdAdd").val());
  $("#BilledCountiesToRemove").val($("#hiddenCountryIdRemove").val());

  $('#BilledMemberDSSupportedByAtosFromHidden option').each(function (i, option) { $(option).remove(); })
  jQuery('#BilledMemberDSSupportedByAtosFrom option').clone().appendTo('#BilledMemberDSSupportedByAtosFromHidden');

  $('#BilledMemberDSSupportedByAtosToHidden option').each(function (i, option) { $(option).remove(); })
  jQuery('#BilledMemberDSSupportedByAtosTo option').clone().appendTo('#BilledMemberDSSupportedByAtosToHidden');

  setFutureUpdateFieldValue("#BilledMemberDSSupportedByAtosToDisabled", "#BilledMemberDSSupportedByAtosTo", "#BilledFutureValue");

  /*JqueryUI: BillingDsRequiredCountries Dialog Save Issue*/
  // $(this).dialog('close');
  return true;
}

// SCP156578 - ICH informational contacts not visible through ICH Tab / IS Contacts
function searchContactTypes(postUrl, divId, contactTypeCategory, saveContactAssignmentUrl) {

  var groupId = "", subGroupId = "", typeId = "", columns = "";

  groupId = $('#GroupId').val();
  subGroupId = $('#SubGroupId').val();
  typeId = $('#TypeId').val();
  $('#divType').val(divId);
  if (contactTypeCategory == 'PAX') {
      columns = "ValueConfirmation_" + $('#IsParticipateInValueConfirmation').prop('checked') + "|" + "AutoBilling_" + $('#IsParticipateInAutoBilling:checked').prop('checked') + "|" + "ValueDetermination_" + $('#IsParticipateInValueDetermination:checked').prop('checked') + "|" + "EnableOldIdec_" + $('#paxIsEnableOldIdecPassengerBilling').prop('checked') + "|";
  }
  else if (contactTypeCategory == 'UATP') {
      columns = "ATCAN_" + $('#UatpInvoiceHandledbyAtcan').prop('checked') + "|";
  }
  else if (contactTypeCategory == 'CGO') {
      columns = "EnableOldIdec_" + $('#cgoIsEnableOldIdecPassengerBilling').prop('checked') + "|";
  }
  else if (contactTypeCategory == 'E_BILLING') {
      columns = "DsServiceRequired_" + $('#IsDigitalSignatureRequired').prop('checked') + "|";
  }


  $("#divContactAssignmentSearchResult").dialog({
    title: 'Contact Assignments',
    width: 670,
    modal: true,
    resizable: false,
    open: function () {
      $("#divContactAssignmentSearchResult").show();
    },
    close: function () {
      $("#divContactAssignmentSearchResult").hide();
      //unload grid to load grid for new search criteria.
      $.jgrid.gridUnload('#list');
    }
  });

  //Get post data.
  var postData = {
    contactTypeCategory: contactTypeCategory,
    groupId: groupId,
    subGroupId: subGroupId,
    typeId: typeId,
    columns: columns

};

  loadContactGrid(postUrl, postData);
  return false;
}

function loadContactGrid(postUrl, postData) {

  $("#btnSaveAssignment").hide();
  $("#hdnIsDataChanged").val('0');
  curContactGridPage = 1;
  //ajax call start
  $.ajax({
    type: "POST",
    url: postUrl,
    data: postData,
    dataType: "json",
    success: function (result) {
      var searchResults = jQuery.parseJSON(result);
      var colD = searchResults[0]['colData'];
      var colN = searchResults[0]['colNames'];
      var colM = searchResults[0]['colModel'];
      var colNo = searchResults[0]['colCount'];
      var width = 450;
      if (colNo < 3) {
        alert("There are no contact types for given criteria.");
        return;
      }
      if (colNo > 3)
        width = colNo * 125;

      jQuery("#list").jqGrid({
        url: postUrl.replace(/GetMyGridDataJson/gi, 'GetMyGridData'),
        data: postData,
        datatype: 'json',
        mtype: 'POST',
        colNames: colN,
        colModel: colM,
        pager: jQuery('#pager'),
        height: 250,
        width: width,
        rowNum: 10,
        //rowList: [5, 10, 20, 50],
        sortname: "",
        sortorder: "",
        scrollOffset: 50,
        viewrecords: true,
        loadComplete: function () {

          //If 'allowToDownloadCSVFile' is set for member and search result contains records then only show the download button
          var recs = $("#list").getGridParam("records");
          if (recs > 0)
            $("#btnSaveAssignment").show();

          $("#list").closest(".ui-jqgrid-bdiv").css({ 'overflow-y': 'scroll' });
          $("#list input:checkbox").bind("change", function () {
            $("#hdnIsDataChanged").val('1');

            //Modify to set flag that ICH contact type is changed or not.
            var ichChangeElement = $("#hdnIsICHTypeChanged");
            if (ichChangeElement.val() == "false") {
              var contactTypeId = $(this).parent("td").attr("aria-describedby");
              contactTypeId = contactTypeId.replace(/list_/, '');
              var ichContactTypeIdList = ',50,51,52,53,88,';
              if (ichContactTypeIdList.indexOf(',' + contactTypeId + ',') > -1) {
                ichChangeElement.val("true");
              }
            }

          });

          var grid = $("#list");

          var ids = grid.getDataIDs();

          for (var i = 0; i < ids.length; i++) {

            var id = ids[i];
            var val = $("#list").getCell(id, 'EMAIL_ADDRESS');

            grid.setCell(id, 'FIRST_NAME', '', '', { title: val });
          }

        },
        onPaging: function (pgButton) {

          //get next page number
          var nextPg = $("#list").getGridParam("page");

          //If page is not  dirty then change page.
          if ($("#hdnIsDataChanged").val() == '0') {
            curContactGridPage = nextPg;
            return;
          }

          $('<div></div>').html($("<P>Data has Changed. <BR/>Do you want to change the page without saving these changes?</P>")).dialog({
            modal: true,
            buttons: [{
              text: "Yes",
              //'class': "primaryButton",
              click: function () {
                $("#hdnIsDataChanged").val('0');
                curContactGridPage = nextPg;
                $("#list").setGridParam({ page: curContactGridPage }).trigger("reloadGrid");
                $(this).dialog("close");
                return;
              }
            }, {
              text: "No",
              //'class': "secondaryButton",
              click: function () { $(this).dialog("close"); }
            }],
            open: function () {
              $("div[role=dialog] button:contains('Yes')").attr("class", "primaryButton");
              $("div[role=dialog] button:contains('No')").attr("class", "secondaryButton");

            }
          });

          $("#list").setGridParam({ page: curContactGridPage }); //Workaround - jqGrid still increments the page num even when we return stop so we have to reset it (and track the current page num)    
          return 'stop';

        }
      }).navGrid($('#pager'), { edit: false, add: false, del: false, refresh: true, search: false });
    },
    error: function (x, e) {
      alert(x.readyState + " " + x.status + " " + e.msg);
    }
  });
  //ajax call end
}

function saveContactAssignment(saveContactAssignmentUrl) {

  //If page is not  dirty then change page.
  if ($("#hdnIsDataChanged").val() == '0') {
    alert("There is no change in data.");
    return;
  }

  var contactList = "";
  $("#list tr:gt(0)").each(function () {
    var rowData = "";
    $(this).find("td:gt(1)").each(function () {
      var contactTypeId = $(this).attr("aria-describedby");
      if (contactTypeId != "list_EMAIL_ADDRESS") {
        contactTypeId = contactTypeId.replace(/list_/, '');
        rowData = rowData + contactTypeId + '_' + $(this).find('input:checkbox').prop('checked') + ",";
      }
    });

    var r = rowData.length - 1;
    if (r > 0 && rowData[r] == ',')
    { rowData = rowData.substring(0, r); }
    contactList = contactList + $(this).children('td:eq(0)').text() + "!" + rowData + "|";
  });

  var r = contactList.length - 1;
  if (r > 0 && contactList[r] == '|')
  { contactList = contactList.substring(0, r); }

  if (contactList != "") {

    $.ajax({
      type: "POST",
      url: saveContactAssignmentUrl,
      data: { contactList: contactList, ichContactTypes: $("#hdnIsICHTypeChanged").val() },
      dataType: "json",
      success: function (result) {
        alert(result.Message);
        $("#hdnIsDataChanged").val('0');
        $("#hdnIsICHTypeChanged").val("false");
      },
      failure: function (response) {
        alert(response.d);
      }
    });
  }
}

function showStatusHistoryDialog(divId, memberType, url) {
  $.ajax({
    type: "POST",
    url: url,
    data: { memberType: memberType },
    dataType: "json",
    success: function (response) {
      var searchResults = jQuery.parseJSON(response);
      var myOptions = "";
      if (searchResults.length > 1) {
        $StatusHistoryDialog =
		$(divId).dialog({
		  title: 'Status Change History',
		  modal: true,
		  buttons: {
		    Close: {
		      className: 'secondaryButton',
		      text: 'Close',
		      click: function () {
		        $(this).dialog('close');
		      }
		    }
		  },
		  resizable: false
		});
        var objhead = searchResults[0];
        var rowHtml = '<tr class="ui-jqgrid-labels">';
        for (var key in objhead) {
          rowHtml = rowHtml + '<th class="ui-state-default ui-th-column ui-th-ltr">' + objhead[key] + '</th>';
        }
        myOptions = myOptions + rowHtml + '</tr>';

        for (var i = 1; i < searchResults.length; i++) {
          var obj = searchResults[i];
          var rowHtml = '<tr class="ui-widget-content jqgrow ui-row-ltr">';
          for (var key in obj) {
            rowHtml = rowHtml + '<td>' + obj[key] + '</td>';

          }
          myOptions = myOptions + rowHtml + '</tr>';
        }

      }
      else
        alert("No status history available.");
      myOptions = '<table class="ui-jqgrid-htable" border="1" width="100%">' + myOptions + '<table>';
      $(divId).html(myOptions);
    }

  });

}

function checkIfUser(postUrl, postUrl1, emaild, IsActive) {

  $.ajax({
    url: postUrl,
    type: "POST",
    data: { emailId: emaild },
    success: function (response) {
      if (IsActive == null) {
        if ((response) && (response.IsFailed != undefined) && (response.IsFailed = true)) {
          // $('#clientErrorMessage').html(response.Message);
          // $('#clientErrorMessageContainer').show();
          // $('#clientSuccessMessageContainer').hide();
          alert(response.Message);


        }
        else if ((response) && (response.Email != undefined)) {

          $('#EmailAddress').val(response.Email);
          $("#StaffId").val(response.StaffID);
          $("#StaffId").attr('disabled', true);

          if (($("#UserCategory").val() != 'IchOps') && ($("#UserCategory").val() != 'AchOps')) {
            $("#conIsActive").removeAttr('disabled');
          }


        $("#conIsActive").prop('checked', !response.IsLocked);
          $("#conIsActive").val(!response.IsLocked);


          if (response.IsLocked == true)
            $("#conIsActive").attr('disabled', true);

          else {
            if (IsActive != null) {
                $("#conIsActive").prop('checked', IsActive);
            }
          }

          $("#Salutation").val(response.Salutation);

          $("#FirstName").val(response.FirstName);
          $("#FirstNameHidden").val(response.FirstName);

          $("#LastName").val(response.LastName);
          $("#LastNameHidden").val(response.LastName);

          $("#PositionOrTitle").val(response.PositionOrTitle);

          $("#Division").val(response.Division);

          $("#Department").val(response.Department);

          $("#LocationId").val(response.Location.LocationID);

          $("#conAddressLine1").val(response.Location.Address1);

          $("#conAddressLine2").val(response.Location.Address2);

          $("#conAddressLine3").val(response.Location.Address3);

          $("#conCityName").val(response.Location.CityName);

          $('#conCountryId').val(response.Location.COUNTRY_CODE);

          $('#conSubDivisionName').val(response.Location.SUB_DIVISION_CODE);

          $("#SitaAddress").val(response.Location.SITA_Adress);

          $("#conPostalCode").val(response.Location.POSTAL_CODE);

          $("#FaxNumber").val(response.Phone.Fax);

          $("#PhoneNumber1").val(response.Phone.PhoneNumber1);

          $("#PhoneNumber2").val(response.Phone.PhoneNumber2);
          $("#MobileNumber").val(response.Phone.MobileNumber);

          $("#Salutation").attr('disabled', true);
          $("#FirstName").attr('disabled', true);
          $("#LastName").attr("disabled", true);
          $("#PositionOrTitle").attr('disabled', true);
          $("#Division").attr('disabled', true);
          $("#Department").attr('disabled', true);
          $("#LocationId").attr('disabled', true);
          $("#conAddressLine1").attr('disabled', true);
          $("#conAddressLine2").attr('disabled', true);
          $("#conAddressLine3").attr('disabled', true);
          $("#conCityName").attr('disabled', true);
          $("#conCountryId").attr('disabled', true);
          $("#conSubDivisionName").attr('disabled', true);
          $("#SitaAddress").attr('disabled', true);
          $("#conPostalCode").attr('disabled', true);
          $("#FaxNumber").attr('disabled', true);
          $("#PhoneNumber1").attr('disabled', true);
          $("#PhoneNumber2").attr('disabled', true);
          $("#MobileNumber").attr('disabled', true);
        }

        else {
          if (confirm("Contact being configured is not an existing IS user.\r\n\r\nSelect 'OK' to proceed with contact creation.\r\nSelect 'Cancel' to correct contact email id.")) {
            notIsUser();
          }
          else {
            notIsUser();
            $("#EmailAddress").focus();
          }

        }
      }

      else {
        if ((response) && (response.Email != undefined)) {

          $('#EmailAddress').val(response.Email);
          $("#StaffId").val(response.StaffID);
          $("#StaffId").attr('disabled', true);

          if (($("#UserCategory").val() != 'IchOps') && ($("#UserCategory").val() != 'AchOps')) {
            $("#conIsActive").removeAttr('disabled');
          }


        $("#conIsActive").prop('checked', !response.IsLocked);
          $("#conIsActive").val(!response.IsLocked);


          if (response.IsLocked == true)
            $("#conIsActive").attr('disabled', true);

          else {
            if (IsActive != null) {
                $("#conIsActive").prop('checked', IsActive);
            }
          }

          $("#Salutation").val(response.Salutation);

          $("#FirstName").val(response.FirstName);
          $("#FirstNameHidden").val(response.FirstName);

          $("#LastName").val(response.LastName);
          $("#LastNameHidden").val(response.LastName);

          $("#PositionOrTitle").val(response.PositionOrTitle);

          $("#Division").val(response.Division);

          $("#Department").val(response.Department);

          $("#LocationId").val(response.Location.LocationID);

          $("#conAddressLine1").val(response.Location.Address1);

          $("#conAddressLine2").val(response.Location.Address2);

          $("#conAddressLine3").val(response.Location.Address3);

          $("#conCityName").val(response.Location.CityName);

          $('#conCountryId').val(response.Location.COUNTRY_CODE);

          $('#conSubDivisionName').val(response.Location.SUB_DIVISION_CODE);

          $("#SitaAddress").val(response.Location.SITA_Adress);

          $("#conPostalCode").val(response.Location.POSTAL_CODE);

          $("#FaxNumber").val(response.Phone.Fax);

          $("#PhoneNumber1").val(response.Phone.PhoneNumber1);

          $("#PhoneNumber2").val(response.Phone.PhoneNumber2);
          $("#MobileNumber").val(response.Phone.MobileNumber);

          //SCP333083: XML Validation Failure for A30-XB - SIS Production
          $("#EmailAddress").attr('readonly', 'true');
          $("#Salutation").attr('disabled', true);
          $("#FirstName").attr('disabled', true);
          $("#LastName").attr("disabled", true);
          $("#PositionOrTitle").attr('disabled', true);
          $("#Division").attr('disabled', true);
          $("#Department").attr('disabled', true);
          $("#LocationId").attr('disabled', true);
          $("#conAddressLine1").attr('disabled', true);
          $("#conAddressLine2").attr('disabled', true);
          $("#conAddressLine3").attr('disabled', true);
          $("#conCityName").attr('disabled', true);
          $("#conCountryId").attr('disabled', true);
          $("#conSubDivisionName").attr('disabled', true);
          $("#SitaAddress").attr('disabled', true);
          $("#conPostalCode").attr('disabled', true);
          $("#FaxNumber").attr('disabled', true);
          $("#PhoneNumber1").attr('disabled', true);
          $("#PhoneNumber2").attr('disabled', true);
          $("#MobileNumber").attr('disabled', true);
        }

        else {
          if (confirm("Contact being configured is not an existing IS user.\r\n\r\nSelect 'OK' to proceed with contact creation.\r\nSelect 'Cancel' to correct contact email id.")) {
            notIsUser();
          }
          else {
            notIsUser();
            $("#EmailAddress").focus();
          }

        }
      }

    }
  });
}

function setFutureUpdateFieldValue(element1, element2, futureValueElement) {
    /* SCP79517: ICH Tab - Future Value does not reflect updates for removed entities in Aggregatored / Sponsored lists 
	Date: 26-02-2013
	Desc: Future update pop-up should show the value(set of values) which will be in effect from future period. 
		  Old logic is updated to display future list as is. Future list is laready well populated by the caller function.
    */
    var futureUpdateSponsororList = "";
    $(element2 + " option").each(function () {
        var metaid = $(this).val();
        futureUpdateSponsororList = futureUpdateSponsororList + $(this).text() + ",<br/>";
    });

    var length = futureUpdateSponsororList.length;
    if (length >= 6)
        length = length - 6;
    if (length > 1)
        futureUpdateSponsororList = futureUpdateSponsororList.substring(0, length);
    $('' + futureValueElement + '').val(futureUpdateSponsororList);
}

// Add option to given dropdown list
function addSelectOption(elementName, value, text) {
  var option = $(elementName);
  if (option.length > 0)
    option.append($(document.createElement("option")).attr("value", value).text(text));
}

// Update option in given dropdown list
function editSelectOption(elementName, value, text) {
  var option = $(elementName + " option[value=" + value + "]");
  if (option.length > 0)
    option.text(text);
}

function notIsUser() {
  if (($("#UserCategory").val() != 'IchOps') && ($("#UserCategory").val() != 'AchOps')) {

    $("#StaffId").removeAttr('disabled');
    $("#conIsActive").removeAttr('disabled');
    //SCP333083: XML Validation Failure for A30-XB - SIS Production
    $("#EmailAddress").removeAttr('readonly');
    $("#Salutation").removeAttr('disabled');
    $("#FirstName").removeAttr('disabled');
    $("#LastName").removeAttr('disabled');
    $("#PositionOrTitle").removeAttr('disabled');
    $("#Division").removeAttr('disabled');
    $("#Department").removeAttr('disabled');
    $("#LocationId").removeAttr('disabled');
    $("#conAddressLine1").removeAttr('disabled');
    $("#conAddressLine2").removeAttr('disabled');
    $("#conAddressLine3").removeAttr('disabled');
    $("#conCityName").removeAttr('disabled');
    $("#conCountryId").removeAttr('disabled');
    $("#conSubDivisionName").removeAttr('disabled');
    $("#SitaAddress").removeAttr('disabled');
    $("#conPostalCode").removeAttr('disabled');
    $("#FaxNumber").removeAttr('disabled');
    $("#PhoneNumber1").removeAttr('disabled');
    $("#PhoneNumber2").removeAttr('disabled');
    $("#MobileNumber").removeAttr('disabled');

    if ($("#LocationId").val() == "") {

      $('#conCountryId').removeAttr('disabled');
      $("#conAddressLine1").removeAttr('disabled');
      $("#conAddressLine2").removeAttr('disabled');
      $("#conAddressLine3").removeAttr('disabled');
      $("#conCityName").removeAttr('disabled');
      $("#SubDivisionCode").removeAttr('disabled');
      $("#conSubDivisionName").removeAttr('disabled');
      $("#conPostalCode").removeAttr('disabled');
    }
    else {
      $("#conCountryId").attr('disabled', 'disabled');
      $("#conAddressLine1").attr('disabled', true);
      $("#conAddressLine2").attr('disabled', true);
      $("#conAddressLine3").attr('disabled', true);
      $("#conCityName").attr('disabled', true);
      $("#SubDivisionCode").attr('disabled', true);
      $("#conSubDivisionName").attr('disabled', true);
      $("#conPostalCode").attr('disabled', true);
    }
  }
}

function resetLocationCode(selectedId) {
  $.ajax({
    type: "POST",
    url: MemberLocationDataUrl,
    dataType: "json",
    success: function (response) {
      var locationList = response;
      var objSelect = $('#selLocationCode');


      var myOptions = "";
      // iterate over locationList
      if (locationList.length > 0) {
        $('option', objSelect).remove();
        myOptions = '<option value="">Please Select</option>';
        for (var x = 0; x < locationList.length; x++) {
          var twt = locationList[x];
          if (selectedId == twt.Id)
            myOptions = myOptions + '<option title=\"' + twt.LocationCode + '\" value=\"' + twt.Id + '\" selected=\"selected\">' + twt.LocationCode + '</option>';
          else
            myOptions = myOptions + '<option title=\"' + twt.LocationCode + '\" value=\"' + twt.Id + '\">' + twt.LocationCode + '</option>';
        }
      }

      objSelect.html(myOptions);

    }
  });
}

// Function to get contact sub groups for given group ids.
function getSubGroups(posturl, postdata) {
  $.ajax({
    type: "POST",
    url: posturl,
    dataType: "json",
    data: { groupIds: postdata },
    success: function (response) {
      var subgroups = response;
      var objSelect = $('#SubGroupId');
      $('option', objSelect).remove();

      var myOptions = "";
      // iterate over subgroups
      if (subgroups.length > 0) {
        myOptions = '<option value=\"\">Please Select</option>';
        for (var x = 0; x < subgroups.length; x++) {
          var twt = subgroups[x];
          myOptions = myOptions + '<option title=\"' + twt.Name + '\" value=\"' + twt.Id + '\">' + twt.Name + '</option>';
        }
      }
      else {
        myOptions = '<option value=\"\">N/A</option>';
      }
      objSelect.html(myOptions);
    },
    error: function (xhr, textStatus, errorThrown) {
      alert('An error occurred! ' + errorThrown);
    }
  });
}

function setPaxMigrationfields() {

  if ($('#NonSamplePrimeBillingIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').datepicker('enable');
      $('#NonSamplePrimeBillingIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSamplePrimeBillingIsIdecCertifiedOn').datepicker('disable');
    $('#NonSamplePrimeBillingIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#NonSamplePrimeBillingIsxmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').datepicker('enable');
      $('#NonSamplePrimeBillingIsxmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSamplePrimeBillingIsxmlCertifiedOn').datepicker('disable');
    $('#NonSamplePrimeBillingIsxmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SamplingProvIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSamplingProvIsIdecCerfifiedOn').datepicker('enable');
      $('#SamplingProvIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSamplingProvIsIdecCerfifiedOn').datepicker('disable');
    $('#SamplingProvIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SamplingProvIsxmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSamplingProvIsxmlCertifiedOn').datepicker('enable');
      $('#SamplingProvIsxmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSamplingProvIsxmlCertifiedOn').datepicker('disable');
    $('#SamplingProvIsxmlMigratedDate').attr('disabled', 'disabled');

  }


  if ($('#NonSampleRmIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSampleRmIsIdecCertifiedOn').datepicker('enable');
      $('#NonSampleRmIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSampleRmIsIdecCertifiedOn').datepicker('disable');
    $('#NonSampleRmIsIdecMigratedDate').attr('disabled', 'disabled');
  }


  if ($('#NonSampleRmIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSampleRmIsXmlCertifiedOn').datepicker('enable');
      $('#NonSampleRmIsXmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSampleRmIsXmlCertifiedOn').datepicker('disable');
    $('#NonSampleRmIsXmlMigratedDate').attr('disabled', 'disabled');
  }


  if ($('#NonSampleBmIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSampleBmIsIdecCertifiedOn').datepicker('enable');
      $('#NonSampleBmIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSampleBmIsIdecCertifiedOn').datepicker('disable');
    $('#NonSampleBmIsIdecMigratedDate').attr('disabled', 'disabled');
  }


  if ($('#NonSampleBmIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSampleBmIsxmlCertifiedOn').datepicker('enable');
      $('#NonSampleBmIsXmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSampleBmIsxmlCertifiedOn').datepicker('disable');
    $('#NonSampleBmIsXmlMigratedDate').attr('disabled', 'disabled');

  }


  if ($('#NonSampleCmIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSampleCmIsIdecCertifiedOn').datepicker('enable');
      $('#NonSampleCmIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSampleCmIsIdecCertifiedOn').datepicker('disable');
    $('#NonSampleCmIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#NonSampleCmIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxNonSampleCmIsXmlCertifiedOn').datepicker('enable');
      $('#NonSampleCmIsXmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxNonSampleCmIsXmlCertifiedOn').datepicker('disable');
    $('#NonSampleCmIsXmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SampleFormCIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSampleFormCIsIdecCertifiedOn').datepicker('enable');
      $('#SampleFormCIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSampleFormCIsIdecCertifiedOn').datepicker('disable');
    $('#SampleFormCIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SampleFormCIsxmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSampleFormCIsxmlCertifiedOn').datepicker('enable');
      $('#SampleFormCIsxmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSampleFormCIsxmlCertifiedOn').datepicker('disable');
    $('#SampleFormCIsxmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SampleFormDeIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSampleFormDeIsIdecCertifiedOn').datepicker('enable');
      $('#SampleFormDeIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSampleFormDeIsIdecCertifiedOn').datepicker('disable');
    $('#SampleFormDeIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SampleFormDEisxmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSampleFormDeIsxmlCertifiedOn').datepicker('enable');
      $('#SampleFormDeIsxmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSampleFormDeIsxmlCertifiedOn').datepicker('disable');
    $('#SampleFormDeIsxmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SampleFormFxfIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSampleFormFxfIsIdecCertifiedOn').datepicker('enable');
      $('#SampleFormFxfIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSampleFormFxfIsIdecCertifiedOn').datepicker('disable');
    $('#SampleFormFxfIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#SampleFormFxfIsxmlMigratedStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#PaxSampleFormFxfIsxmlCertifiedOn').datepicker('enable');
      $('#SampleFormFxfIsxmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#PaxSampleFormFxfIsxmlCertifiedOn').datepicker('disable');
    $('#SampleFormFxfIsxmlMigratedDate').attr('disabled', 'disabled');
  }

}

function setCgoMigrationFields() {

  if ($('#PrimeBillingIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoPrimeBillingIsIdecCertifiedOn').datepicker('enable');
      $('#PrimeBillingIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoPrimeBillingIsIdecCertifiedOn').datepicker('disable');
    $('#PrimeBillingIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#PrimeBillingIsxmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoPrimeBillingIsxmlCertifiedOn').datepicker('enable');
      $('#PrimeBillingIsxmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoPrimeBillingIsxmlCertifiedOn').datepicker('disable');
    $('#PrimeBillingIsxmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#RmIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoRmIsIdecCertifiedOn').datepicker('enable');
      $('#RmIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoRmIsIdecCertifiedOn').datepicker('disable');
    $('#RmIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#RmIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoRmIsXmlCertifiedOn').datepicker('enable');
      $('#RmIsXmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoRmIsXmlCertifiedOn').datepicker('disable');
    $('#RmIsXmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#BmIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoBmIsIdecCertifiedOn').datepicker('enable');
      $('#BmIsIdecMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoBmIsIdecCertifiedOn').datepicker('disable');
    $('#BmIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#BmIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoBmIsXmlCertifiedOn').datepicker('enable');
      $('#BmIsXmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoBmIsXmlCertifiedOn').datepicker('disable');
    $('#BmIsXmlMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#CmIsIdecMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoCmIsIdecCertifiedOn').datepicker('enable');
      $('#CmIsIdecMigratedDate').removeAttr('disabled');

    }
  }
  else {
    $('#cgoCmIsIdecCertifiedOn').datepicker('disable');
    $('#CmIsIdecMigratedDate').attr('disabled', 'disabled');
  }

  if ($('#CmIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#cgoCmIsXmlCertifiedOn').datepicker('enable');
      $('#CmIsXmlMigratedDate').removeAttr('disabled');
    }
  }
  else {
    $('#cgoCmIsXmlCertifiedOn').datepicker('disable');
    $('#CmIsXmlMigratedDate').attr('disabled', 'disabled');

  }
}

function setMiscMigrationFields() {
  if ($('#BillingIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#miscBillingIsXmlCertifiedOn').datepicker('enable');
      $('#BillingIsXmlMigrationDate').removeAttr('disabled');
    }
  }
  else {
    $('#miscBillingIsXmlCertifiedOn').datepicker('disable');
    $('#BillingIsXmlMigrationDate').attr('disabled', 'disabled');
  }
}

function setUatpMigrationFields() {
  if ($('#BillingIsXmlMigrationStatusId').val() == 3) {
    if ($('#UserCategory').val() == 'SisOps') {
      $('#uatpBillingIsXmlCertifiedOn').datepicker('enable');
      $('#BillingIsXmlMigrationDateUATP').removeAttr('disabled');
    }
  }
  else {
    $('#uatpBillingIsXmlCertifiedOn').datepicker('disable');
    $('#BillingIsXmlMigrationDateUATP').attr('disabled', 'disabled');

  }
}

function SelectAllIchPattern() {
  var button = $("#selectAllIch");
  if (button.val() == "Select All") {
      $("#IchPaxPeriod1").prop('checked', true);
      $("#IchPaxPeriod2").prop('checked', true);
      $("#IchPaxPeriod3").prop('checked', true);
      $("#IchPaxPeriod4").prop('checked', true);
    GetValueofOnCheck("#IchPaxPeriod1", "#IchPaxPeriod2", "#IchPaxPeriod3", "#IchPaxPeriod4", "#InterClearanceInvoiceSubmissionPatternPaxId");
    $("#IchCgoPeriod1").prop('checked', true);
    $("#IchCgoPeriod2").prop('checked', true);
    $("#IchCgoPeriod3").prop('checked', true);
    $("#IchCgoPeriod4").prop('checked', true);
    GetValueofOnCheck("#IchCgoPeriod1", "#IchCgoPeriod2", "#IchCgoPeriod3", "#IchCgoPeriod4", "#InterClearanceInvoiceSubmissionPatternCgoId");
    $("#IchMiscPeriod1").prop('checked', true);
    $("#IchMiscPeriod2").prop('checked', true);
    $("#IchMiscPeriod3").prop('checked', true);
    $("#IchMiscPeriod4").prop('checked', true);
    GetValueofOnCheck("#IchMiscPeriod1", "#IchMiscPeriod2", "#IchMiscPeriod3", "#IchMiscPeriod4", "#InterClearanceInvoiceSubmissionPatternMiscId");
    $("#IchUatpPeriod1").prop('checked', true);
    $("#IchUatpPeriod2").prop('checked', true);
    $("#IchUatpPeriod3").prop('checked', true);
    $("#IchUatpPeriod4").prop('checked', true);
    GetValueofOnCheck("#IchUatpPeriod1", "#IchUatpPeriod2", "#IchUatpPeriod3", "#IchUatpPeriod4", "#InterClearanceInvoiceSubmissionPatternUatpId");
    button.attr('value', 'Unselect All');
  }
  else {
      $("#IchPaxPeriod1").prop('checked', false);
      $("#IchPaxPeriod2").prop('checked', false);
      $("#IchPaxPeriod3").prop('checked', false);
      $("#IchPaxPeriod4").prop('checked', false);
    GetValueofOnCheck("#IchPaxPeriod1", "#IchPaxPeriod2", "#IchPaxPeriod3", "#IchPaxPeriod4", "#InterClearanceInvoiceSubmissionPatternPaxId");
    $("#IchCgoPeriod1").prop('checked', false);
    $("#IchCgoPeriod2").prop('checked', false);
    $("#IchCgoPeriod3").prop('checked', false);
    $("#IchCgoPeriod4").prop('checked', false);
    GetValueofOnCheck("#IchCgoPeriod1", "#IchCgoPeriod2", "#IchCgoPeriod3", "#IchCgoPeriod4", "#InterClearanceInvoiceSubmissionPatternCgoId");
    $("#IchMiscPeriod1").prop('checked', false);
    $("#IchMiscPeriod2").prop('checked', false);
    $("#IchMiscPeriod3").prop('checked', false);
    $("#IchMiscPeriod4").prop('checked', false);
    GetValueofOnCheck("#IchMiscPeriod1", "#IchMiscPeriod2", "#IchMiscPeriod3", "#IchMiscPeriod4", "#InterClearanceInvoiceSubmissionPatternMiscId");
    $("#IchUatpPeriod1").prop('checked', false);
    $("#IchUatpPeriod2").prop('checked', false);
    $("#IchUatpPeriod3").prop('checked', false);
    $("#IchUatpPeriod4").prop('checked', false);
    GetValueofOnCheck("#IchUatpPeriod1", "#IchUatpPeriod2", "#IchUatpPeriod3", "#IchUatpPeriod4", "#InterClearanceInvoiceSubmissionPatternUatpId");
    button.attr('value', 'Select All');
  }

}

function SelectAllAchPattern() {
  var button = $("#selectAllAch");
  if (button.val() == "Select All") {
    $("#AchPaxPeriod1").prop('checked', true);
    $("#AchPaxPeriod2").prop('checked', true);
    $("#AchPaxPeriod3").prop('checked', true);
    $("#AchPaxPeriod4").prop('checked', true);
    GetValueofOnCheck("#AchPaxPeriod1", "#AchPaxPeriod2", "#AchPaxPeriod3", "#AchPaxPeriod4", "#AchClearanceInvoiceSubmissionPatternPaxId");
    $("#AchCgoPeriod1").prop('checked', true);
    $("#AchCgoPeriod2").prop('checked', true);
    $("#AchCgoPeriod3").prop('checked', true);
    $("#AchCgoPeriod4").prop('checked', true);
    GetValueofOnCheck("#AchCgoPeriod1", "#AchCgoPeriod2", "#AchCgoPeriod3", "#AchCgoPeriod4", "#AchClearanceInvoiceSubmissionPatternCgoId");
    $("#AchMiscPeriod1").prop('checked', true);
    $("#AchMiscPeriod2").prop('checked', true);
    $("#AchMiscPeriod3").prop('checked', true);
    $("#AchMiscPeriod4").prop('checked', true);
    GetValueofOnCheck("#AchMiscPeriod1", "#AchMiscPeriod2", "#AchMiscPeriod3", "#AchMiscPeriod4", "#AchClearanceInvoiceSubmissionPatternMiscId");
    $("#AchUatpPeriod1").prop('checked', true);
    $("#AchUatpPeriod2").prop('checked', true);
    $("#AchUatpPeriod3").prop('checked', true);
    $("#AchUatpPeriod4").prop('checked', true);
    GetValueofOnCheck("#AchUatpPeriod1", "#AchUatpPeriod2", "#AchUatpPeriod3", "#AchUatpPeriod4", "#AchClearanceInvoiceSubmissionPatternUatpId");
    button.attr('value', 'Unselect All');
  }
  else {
      $("#AchPaxPeriod1").prop('checked', false);
      $("#AchPaxPeriod2").prop('checked', false);
      $("#AchPaxPeriod3").prop('checked', false);
      $("#AchPaxPeriod4").prop('checked', false);
    GetValueofOnCheck("#AchPaxPeriod1", "#AchPaxPeriod2", "#AchPaxPeriod3", "#AchPaxPeriod4", "#AchClearanceInvoiceSubmissionPatternPaxId");
    $("#AchCgoPeriod1").prop('checked', false);
    $("#AchCgoPeriod2").prop('checked', false);
    $("#AchCgoPeriod3").prop('checked', false);
    $("#AchCgoPeriod4").prop('checked', false);
    GetValueofOnCheck("#AchCgoPeriod1", "#AchCgoPeriod2", "#AchCgoPeriod3", "#AchCgoPeriod4", "#AchClearanceInvoiceSubmissionPatternCgoId");
    $("#AchMiscPeriod1").prop('checked', false);
    $("#AchMiscPeriod2").prop('checked', false);
    $("#AchMiscPeriod3").prop('checked', false);
    $("#AchMiscPeriod4").prop('checked', false);
    GetValueofOnCheck("#AchMiscPeriod1", "#AchMiscPeriod2", "#AchMiscPeriod3", "#AchMiscPeriod4", "#AchClearanceInvoiceSubmissionPatternMiscId");
    $("#AchUatpPeriod1").prop('checked', false);
    $("#AchUatpPeriod2").prop('checked', false);
    $("#AchUatpPeriod3").prop('checked', false);
    $("#AchUatpPeriod4").prop('checked', false);
    GetValueofOnCheck("#AchUatpPeriod1", "#AchUatpPeriod2", "#AchUatpPeriod3", "#AchUatpPeriod4", "#AchClearanceInvoiceSubmissionPatternUatpId");
    button.attr('value', 'Select All');
  }
}


function GetValueofOnCheck(checkBoxId1, checkBoxId2, checkBoxId3, checkBoxId4, textBoxId) {
  var Index1 = 0;
  var Index2 = 0;
  var Index3 = 0;
  var Index4 = 0;

  if ($(checkBoxId1).is(':checked')) {
    Index1 = 1;
  }

  if ($(checkBoxId2).is(':checked')) {
    Index2 = 1;
  }

  if ($(checkBoxId3).is(':checked')) {
    Index3 = 1;
  }

  if ($(checkBoxId4).is(':checked')) {
    Index4 = 1;
  }

  var value = (Index1 + "" + Index2 + "" + Index3 + "" + Index4);
  $(textBoxId).val(value);
}

//CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
function DailyPayableRequiredChangeEvent() {
    if (!$(this).is(":checked")) {

    if($('#IsDailyPayableOARRequired').is(":checked") || $('#IsDailyPayableXmlRequired').is(":checked"))
    {
        //‘Daily Delivery in IS-WEB’ is updated from “True” to “False” then warning popup should open with below text.
        // "i.Unchecking this field will also automatically uncheck dependent fields ‘Daily Offline Archive Outputs’ and ‘Daily IS-XML Files’.
        // Click Proceed to continue with update of this field. Else click Cancel to abort update of this field."
        $('#DailyPayablePopUp').dialog({ title: "Warning", height: 130, width: 500, modal: true, resizable: false, closeOnEscape: false,
            buttons: {
                Proceed: {
                    class: 'primaryButton',
                    text: 'Proceed',
                    click: function () {
                        $('#IsDailyPayableOARRequired').attr("disabled", true);
                        $('#IsDailyPayableOARRequired').prop('checked', false);

                        $('#IsDailyPayableXmlRequired').attr("disabled", true);
                        $('#IsDailyPayableXmlRequired').prop('checked', false);

                        $(this).dialog("close");
                    }
                },
                Cancel: {
                    class: 'secondaryButton',
                    text: 'Cancel',
                    click: function () {
                        $('#IsDailyPayableIsWebRequired').prop('checked', true);
                        $(this).dialog("close");
                    }
                },
            },
            close: function (event, ui) {
                $('#IsDailyPayableIsWebRequired').prop('checked', true);
                $(this).dialog("close");
            }
        });
      }
    }
    else {
        /*If ‘Daily Delivery in IS-WEB’ is updated from “Flase” to “True” then 
           i.‘Daily IS-XML Files:’ should be updated from “False” to “True”
           ii.‘Daily Delivery in IS-WEB’ should be updated from “False” to “True”
        */
      $('#IsDailyPayableOARRequired').removeAttr("disabled");
      $('#IsDailyPayableXmlRequired').removeAttr("disabled");
      $('#IsDailyPayableOARRequired').prop('checked', true);
      $('#IsDailyPayableXmlRequired').prop('checked', true);
    }
}


//CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
function DisableDailyPayableCheckBoxs() {
    //These checkbox should be disabled if ‘Daily Delivery in IS-WEB’ is “False” (i.e. users should not be able to update them)
    //These checkbox should be enabled only if ‘Daily Delivery in IS-WEB’ is “True” (i.e. users should be able to update them)
    if ($("#IsDailyPayableIsWebRequired").is(":checked")) {
        $('#IsDailyPayableOARRequired').removeAttr("disabled");
        $('#IsDailyPayableXmlRequired').removeAttr("disabled");
       
    }
    else {
        $('#IsDailyPayableOARRequired').attr("disabled", true);
        $('#IsDailyPayableXmlRequired').attr("disabled", true);
    }
}
