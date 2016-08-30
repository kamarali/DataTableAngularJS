// Following variable is declared to get whether the page mode is view. If page mode is view it will be set to true from respective pages.
var $isOnView = false;
// Following variable is used to set Billing type i.e. Receivables or Payables
var billingType = '';

$(function () {
    $('.ajaxLoader')
    .hide()  // hide it initially 
    .ajaxStart(function () {
        _isAjaxError = false;
        $(this).show();
        // Disable submit button when ajax request is being processed to prevent double click of submit button.
        $('.buttonContainer input[type=submit]').attr('disabled', true);
    })
    .ajaxStop(function () {
        $(this).hide();
        // Enable submit button on page when ajax request completes.
        $('.buttonContainer input[type=submit]').attr('disabled', _isAjaxError);
    });

    $("ul.sf-menu").supersubs({
        minWidth: 15,
        maxWidth: 20,
        extraWidth: 1
      }).superfish();

      // SCP304020: UAT 1.6: Misc Codes Setup
      // SCP315028: no email id in correspondence
      if ($('textarea').hasClass('textAreaTrimText')) {
        $('.textAreaTrimText').each(function (i, obj) {
          $(obj).val($(obj).val().replace(/^\s+|\s+$/g, ''));
        });
      }

    $('textarea:not(.notValidCharsTextarea)', 'form.validCharacters').addClass('validCharsTextarea');

    $('.showDropdownTooltip').live("mouseover", function (event) {
        if (event.srcElement != null) {
            var value = $("#" + event.srcElement.id + " option:selected").text();
            $(this).attr('title', value);
        }
    });
    // Initialize the validation plug-in.
    initializeValidationSettings(_errorIcon);

    // Set jqGrid default values.
    initializeGridSettings();

    // Format all the date controls on the page.
    formatDateControls();

    // Format for date control master pages
    formatDateControlsMaster();

    // Set the dialog defaults.
    $.extend($.ui.dialog.prototype.options, {
        closeOnEscape: false,
        modal: true,
        bgiframe: true,
        resizable: false
    });

    // Decorate the '*' on the mandatory fields with red color.
    highlightMandatory();

    initializeUnsavedChangesWarning();

    // Format the decimals.
    formatDecimals();

    // On change of grid page size option, store the value in session.
    $(".ui-pg-selbox").change(function () { onPageSizeOptionChange(this) });
    //  On change of jqGrid navigation text box current page option, store the value in session.    
    $(".ui-pg-input").change(function () { onCurrentPageNumberChange(this.value) });

    //  On change navigator text box of jqGrid with click next navigator icon, current page option, store the value in session.  
    $("#next").click(function () {
        var textvalue = $('.ui-pg-input').val();
        textvalue = parseInt(textvalue) + 1;
        onCurrentPageNumberChange(textvalue)
    });

    //  On change navigator text box of jqGrid with click last navigator icon, current page option, store the value in session.  
    $("#last").click(function () {
        var textvalue = $("#sp_1").text();
        onCurrentPageNumberChange(textvalue)
    });

    //  On change navigator text box of jqGrid with click previous navigator icon, current page option, store the value in session.  
    $("#prev").click(function () {
        var textvalue = $('.ui-pg-input').val();
        textvalue = parseInt(textvalue) - 1;
        onCurrentPageNumberChange(textvalue)
    });

    //  On change navigator text box of jqGrid with click first navigator icon, current page option, store the value in session.  
    $("#first").click(function () {
        var textvalue = 1;
        onCurrentPageNumberChange(textvalue)
    });

    // Fix the billing type in the bread crumb.
    fixBreadCrumbBillingType();

    // For debugging purpose only.
    $('#appHeader').dblclick(function (event) {
        if (_skyMajic === 1) {
            if ((event.metaKey === true) && (event.shiftKey === true)) {
                window.location.href = _changeBillingMemberUrl;
            }
        }
    });

    $(':input.validCharsTextarea', 'form.validCharacters').each(function (i) {
        
        if ($(this).val() != null) {
            var value = $(this).val().replace(/\r\n|\r|\n/, '');
            $(this).val(value);
        }
    });
    //For All textboxes in form with class validCharacters, add validation rule to check for characters in ASCII range 32 to 126
    $(':input', 'form.validCharacters').addClass('validateCharacters');

    // To give focus to the first non-hidden text field on the first form.
    $('form:first *:input[type!=hidden]:first').focus();

    // For debugging purpose only.
    $('#appHeader').dblclick(function (event) {
        if (_skyMajic === 1) {
            if ((event.metaKey === true) && (event.shiftKey === true)) {
                window.location.href = _changeBillingMemberUrl;
            }
        }
    });
});

function formatDecimals() {
  // Round all the decimals.
  roundAmounts();
  roundPercents();

  // On tab out of amount and percent fields, round the values to a fixed number of decimal places.
  $('.amount:not([readonly=readonly]), #content').blur(function () { roundNumbers(this, _amountDecimals) });
  $('.percent, #content').blur(function () { roundNumbers(this, _percentDecimals) });

  // For fields having specific decimal places to be rounded to.
  $('*[roundTo]:not([readonly=readonly])', '#content').blur(function () { roundNumbers(this, $(this).attr('roundTo')) });
}

function highlightMandatory() {
  $("span:contains('*')").css('color', 'red');
}

function setFocus(fieldSelector) {
  $(fieldSelector).focus();
}

function setFocusAndBlur(fieldSelector) {
  $(fieldSelector).blur();
  $(fieldSelector).focus();
}

function initializeUnsavedChangesWarning() {
  $('#pendingChangesDialog .cancel').click(function () {
      try {
          $('#pendingChangesDialog').dialog('close');
      } catch (error) { }
  });
}

function trackFormChanges(formId) {
  // if (_showUnsavedDataWarning == 1) {
  if ($parentForm != undefined) {
    $parentForm.resetDirty();
  }

  initializeParentForm(formId);
  $parentForm.dirtyForms();
  // }
}

function initializeParentForm(parentFormId) {
  $parentForm = $("form#" + parentFormId);
}

function formatDateControls() {
  // Convert to a date picker control and add a watermark.
    var $datePickers = $(".datePicker");

    _dateFormat = 'dd-M-y';
  // Set the focus onto next control on close event of datepicker.
  $datePickers.datepicker({ dateFormat: _dateFormat, showOn: 'both', buttonImage: _calendarIcon, buttonImageOnly: true, onClose: function (dateText, inst) { sanitizeDate(dateText, inst);  } });

  $datePickers.watermark(_dateWatermark);
  $datePickers.attr('maxlength', 11);

  // Convert to a date-time picker control and add a watermark.
  var $dateTimePickers = $(".dateTimePicker");
  $dateTimePickers.datetimepicker({ showSecond: true, dateFormat: _dateFormat, timeFormat: _timeFormatForDateTimePicker, separator: 'T', buttonImageOnly: true, showOn: 'both', buttonImage: _calendarIcon });
  $dateTimePickers.watermark(_dateTimeWatermark);
}

function sanitizeDate(currentDate, inst) {
  
  if (inst._keyEvent == false) {
    return;
  }

  var $datePicker = $('#' + inst.id);
  var _dateFormat = 'dd-M-y';
  var allowedFormats = ['dd-mm-yy', 'd-mm-yy', 'dd-m-yy', 'd-m-yy', 'd-M-yy', _dateFormat, 'd-m-y', 'dd-m-y', 'd-mm-y', 'dd-mm-y', 'd-M-y'];
  
  // Check if any of the date formats is valid.
  var parsedDate = isValidDateFormat(allowedFormats, currentDate);

  if (parsedDate == undefined) {
    $datePicker.val('');
  } else {
    $datePicker.datepicker('setDate', parsedDate);
    return true;
  }
}

function isValidDateFormat(dateFormats, currentDate) {

  for (var i = 0; i < dateFormats.length; i++) {
    var format1 = dateFormats[i];
    var format2 = dateFormats[i].replace(/-/g, "/");
    var format3 = dateFormats[i].replace(/-/g, ".");
    var parsedDate;

    try {
      parsedDate = $.datepicker.parseDate(format1, currentDate);
    } catch (e) {
      try {
        parsedDate = $.datepicker.parseDate(format2, currentDate);
      } catch (e) {
        try {
          parsedDate = $.datepicker.parseDate(format3, currentDate);
        } catch (e) {
          // alert(e.Description);
        }
      }
    }

    if (parsedDate != undefined) {
      break;
    }
  }

  return parsedDate;
}

function initializeGridSettings() {
  $.jgrid.defaults = $.extend($.jgrid.defaults, {
    autoencode: true
  });
}

function roundAmounts() {
  roundGenericAmounts();
  roundSpecificAmounts();
}

function roundSpecificAmounts() {
  $('*[roundTo]').each(function () {
    var roundToPlaces = $(this).attr('roundTo');
    if (!isNaN(roundToPlaces) && $(this).val() != '') {
      var amountVal = Number($(this).val());
      $(this).val(amountVal.toFixed(roundToPlaces));
    }
  });
}

function roundGenericAmounts() {
  $('.amount').each(function () {
    roundNumbers(this, _amountDecimals);
  });
}

function roundPercents() {
  $('.percent').each(function () {
    roundNumbers(this, _percentDecimals);
  });
}

function roundNumbers(element, decimals) {
  var elemValue = $(element).val();
  if (!isNaN(elemValue) && elemValue != '') {
    var value = Number(elemValue);
    $(element).val(value.toFixed(decimals));
  }
}
// ID : 296572 - Submission and Assign permission to user doesn't match !
//show both success and error at same time. 
function showDualClientMessage(successMsg, errorMsg) {
    $('#clientSuccessMessage').html(successMsg);
    $('#clientSuccessMessageContainer').show();
    $('#clientErrorMessage').html(errorMsg);
    $('#clientErrorMessageContainer').show();
}

function showClientSuccessMessage(message) {
  $('#clientSuccessMessage').html(message);
  $('#clientSuccessMessageContainer').show();
  $('#clientErrorMessageContainer').hide();
  $("#errorMessageContainer").hide();
  $('#serverSuccessMessage').hide();
  $('#serverErrorMessage').hide();
  $("#errorContainer").hide();
}

function showClientErrorMessage(message) {
  $('#clientErrorMessage').html(message);
  $('#clientSuccessMessageContainer').hide();
  $('#successMessageContainer').hide();
  $('#clientErrorMessageContainer').show();
  $("#errorMessageContainer").hide();
  $('#serverSuccessMessage').hide();
  $('#serverErrorMessage').hide();
  $("#errorContainer").hide();
}

function clearMessageContainer() {
  $('#clientSuccessMessageContainer').hide();
  $('#clientErrorMessageContainer').hide();
  $('#successMessageContainer').hide();
  $('#errorMessageContainer').hide();
}

function onPageSizeOptionChange(element) {
  $.ajax({
    type: "POST",
    url: _setPageSizeMethod,
    data: "pageSize=" + element.value
  });
}
// Post the change value on session with JsonResult for maintain the current page by click edit and navigator icon of jqGrid.
function onCurrentPageNumberChange(pageIndex) {
  $.ajax({
    type: "POST",
    url: _setCurrentPageMethod,
    data: "currentPage=" + pageIndex,
    autoChange: true
  });
}

// Following function is used to disable Submit button on form submit and submit the form
function onSubmitHandler(form) {
  // Disable submit button when page is submitted to prevent double click of submit button.
  $('.buttonContainer input[type=submit]').attr('disabled', true);
  form.submit();
}

function setAjaxError() {
  _isAjaxError = true;
}
function closeDialog(dialogId) {
  $(dialogId).dialog('close');
}

function changeAction(actionName) {
  $("form").attr("action", actionName);
  return true;
}

function sanitizeAmount(fieldSelector, precision) {
  var $field = $(fieldSelector);
  var value = $field.val();
  var precision = precision == undefined ? _amountDecimals : precision;
  if (isNaN(value) || value == '')
    $field.watermark();
  else
    $field.val(Number(value).toFixed(precision));
}

function SetPageWaterMark() {
  $("input[watermark=positiveAmount]").watermark('0.00  ');
  $("input[watermark=negativeAmount]").watermark('-0.00  ');
  $("input[watermark=percentage]").watermark('0.000  ');
  $("input[watermark=negativePercentage]").watermark('-0.000  ');
}

function SetMiscPageWaterMark() {
  $("input[watermark=fourDecimalPlaces]").watermark('0.0000  ');
  $("input[watermark=positiveAmount]").watermark('0.000  ');
  $("input[watermark=negativeFourDecimalPlaces]").watermark('-0.0000  ');
  $("input[watermark=negativeAmount]").watermark('-0.000  ');
}

function SetCargoPageWaterMark() {
  $("input[watermark=positiveAmount]").watermark('0.000  ');
  $("input[watermark=negativeAmount]").watermark('-0.000  ');
  $("input[watermark=negativePercentage]").watermark('-0.000  ');
  $("input[watermark=percentage]").watermark('0.000  ');
}

function fixBreadCrumbBillingType() {
  var $billingTypeBreadCrumbNode = $('div#breadCrumbs span.siteMapPath span.link:contains("Receivables")');
  if ($billingTypeBreadCrumbNode.length == 1 && _billingType != 'Receivables') {
    $billingTypeBreadCrumbNode.text(_billingType);
  }
}

function adjustGridContainer() {
  var grid = $(this);
  var gridWidth = grid.width();
  var contentWidth = $('#content').width();

  if (gridWidth < contentWidth) {
    grid.closest('.gridContainer').removeClass('gridContainer');
  }
}

function PadZerosToAwbSerialNumberAndCheckDigitField(i, l, s) {
  var o = i.toString();
  if (!s) { s = '0'; }
  while (o.length < l) {
    o = s + o;
  }
  return o;
}

function formatDateControlsMaster() {
    // Convert to a date picker control and add a watermark.
    var $datePickers = $(".datePickerMaster");
    var _dateWatermark = 'DD-MMM-YYYY';
    _dateFormat = 'dd-M-yy';
    // Set the focus onto next control on close event of datepicker.
    $datePickers.datepicker({ dateFormat: _dateFormat, showOn: 'both', buttonImage: _calendarIcon, buttonImageOnly: true, onClose: sanitizeDateMaster });
    $datePickers.watermark(_dateWatermark);
    $datePickers.attr('maxlength', 11);
  }

  function formatDateControlsForToleranceAndMinMaxMaster() {
    // Convert to a date picker control and add a watermark.
    var $datePickers = $(".datePickerMasterForToleranceAndMinMax");
    var _dateWatermark = 'PP-MMM-YYYY';
    _dateFormat = 'dd-M-yy';
    // Set the focus onto next control on close event of datepicker.
    $datePickers.datepicker({ dateFormat: _dateFormat, showOn: 'both', buttonImage: _calendarIcon, buttonImageOnly: true, onClose: sanitizeDateMaster });
    $datePickers.watermark(_dateWatermark);
    $datePickers.attr('maxlength', 11);
  }

function sanitizeDateMaster(currentDate, inst) {
  
    if (inst._keyEvent == false) {
        return;
    }
    
    var $datepickerMaster = $('#' + inst.id);
    var _dateFormat = 'dd-M-yy';
    var allowedFormats = ['dd-mm-yy', 'd-mm-yy', 'dd-m-yy', 'd-m-yy', 'd-M-yy', _dateFormat, 'd-m-y', 'dd-m-y', 'd-mm-y', 'dd-mm-y', 'd-M-y'];
   
     // Check if any of the date formats is valid.
    var parsedDate = isValidDateFormat(allowedFormats, currentDate);

    if (parsedDate == undefined) {
      $datepickerMaster.val('');
    } else {
      $datepickerMaster.datepicker('setDate', parsedDate);
      return true;
    }
}

// This function returns browser independent date time format for reports in Thu Apr 12 2012 16:06:18, as suggested by Robin
function getDateTimeForReports() {
    var currentDT = new Date();
    var FormattedDateTm = GetStringDayOfWeek(currentDT.getDay()).substring(0, 3) + ' ' + GetStringMonth(currentDT.getMonth()).substring(0, 3) + ' ' + currentDT.getDate() + ' ' + currentDT.getFullYear() + ' ' + GetTime(currentDT);
    return FormattedDateTm;
}

function GetStringDayOfWeek(nDOW) {
    var weekday = new Array(7);
    weekday[0] = "Sunday";
    weekday[1] = "Monday";
    weekday[2] = "Tuesday";
    weekday[3] = "Wednesday";
    weekday[4] = "Thursday";
    weekday[5] = "Friday";
    weekday[6] = "Saturday";
    return weekday[nDOW];
}

function GetStringMonth(nMonth) {
    var cMonth = new Array(12);
    cMonth[0] = "January";
    cMonth[1] = "February";
    cMonth[2] = "March";
    cMonth[3] = "April";
    cMonth[4] = "May";
    cMonth[5] = "June";
    cMonth[6] = "July";
    cMonth[7] = "August";
    cMonth[8] = "September";
    cMonth[9] = "October";
    cMonth[10] = "November";
    cMonth[11] = "December";
    return cMonth[nMonth];
}

//get date in hh:mm:ss format
function GetTime(myDate) {
    myDate = new Date(myDate);
    //'0' is added in case digit is single
    return (myDate.getHours() < 10 ? '0' + myDate.getHours() : myDate.getHours()) + ':' +
        (myDate.getMinutes() < 10 ? '0' + myDate.getMinutes() : myDate.getMinutes()) + ':' +
        (myDate.getSeconds() < 10 ? '0' + myDate.getSeconds() : myDate.getSeconds());
  }

  //SCP310398 - SRM:Exception occurred in Report Download Service. - SIS Production - 10Nov
  //SCP305855 - UAT: Session expired when rasing a RM- Spira Case 9766
  function checkUserSessionsForAjaxRequest() {
    $.ajax({
      type: "POST",
      url: _sessionCheckUrl,
      dataType: "JSON",
      async: false,
      success: function (response) {
        if (response == 'Session_Expired') {
          location.href = _loginUrl;
        }
      }
    });
  }
