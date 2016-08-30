/*
Initializes the jQuery validation plug-in settings.
*/
function initializeValidationSettings(imagePath) {
  jQuery.validator.setDefaults({
    errorPlacement: function (error, element) {
      error.attr("title", error.text());
      error.html("<img style='border-style: none;' src='" + imagePath + "' />");
      if (element.hasClass('datePicker'))
        error.insertAfter(element.next());
      else
        error.insertAfter(element);

    },
    onfocusout: function (element) {
    },
    highlight: function () { $("#errorContainer").show(); clearMessageContainer(); },
    onkeyup: false,
    submitHandler: function (form) {
      // Call onSubmitHandler() function which will disable submit buttons on form submit and will submit the form
      onSubmitHandler(form);
    },
    invalidHandler: function () {
      // Show all watermark text if validation fails
      $.watermark.showAll();
    }
  });


  // Initialize the custom validators.
  validationSetSelectMethod();
  validationSetRegexMethod();

  addClassRules();
  
  $.validator.addMethod('validateAlphabets', validateAlphabets);
  // Add a rule for validating allowed ascii characters only.
  $.validator.addMethod('allowedCharacters', validateAllowedCharacters);
  $.validator.addMethod('allowedCharactersWithExclimation', validateAllowedCharactersWithExclimation);
  $.validator.addMethod('validateDate', validateDate);
  $.validator.addMethod('allowedCharactersForTextAreaFields', validateAllowedCharactersForTextAreaFields);
  $.validator.addMethod('allowedCharactersForTextArea', validateAllowedCharactersForTextArea);

  // Add a rule for validating a billing period.
  $.validator.addMethod('billingPeriod', validatePeriod);

  $.validator.addMethod('EffectiveBillingPeriod', validateEffectivePeriod);

  // Add a rule for validating a file Type.
  $.validator.addMethod('allowedFileType', isValidFileType);

  //Added for dynamic fields data size check, to fix issue If range is 999999999999999.999 and error message is displayed as 1000000000000000
  $.validator.addMethod('dynamicFieldRange', validateDynamicDatasizeRange);

  //Added validation for dynamic field clerance period format
  $.validator.addMethod('dynamicFieldClearancePeriod', validateDFClearancePeriod);

  //Added validation for dynamic field Latitude
  $.validator.addMethod('dynamicFieldLatitude', validateDFLatitude);

  //Added validation for dynamic field Longitude
  $.validator.addMethod('dynamicFieldLongitude', validateDFLongitude);

  //Added validation for dynamic field datetimepicker
  $.validator.addMethod('dynamicFieldDateTimeValue', validateDFDateTimePicker);

  //Add a rule for validating a invoice number range.
  $.validator.addMethod('invoiceNumber', validateInvoiceNumberRange);

  // Add a rule for validating IBAN, SWIFT and Bank Account Number
  $.validator.addMethod('BankDetails', validateBankDetails);

  //Add rule for validation of Pax Textarea Field
  $.validator.addMethod('ValidatePaxTextareaField', isPaxTextareaFieldValid);

  // Add rule for validation of Aircraft Registration Number.
  $.validator.addMethod('dynamicFieldAircraftRegNo', validateAircraftRegNo);

  // Add a rule for validating AWB Serial Number.
  $.validator.addMethod('AwbSerNoCannotBeZero', validateAWBSerialNumber, "AWB Serial Number cannot be zero.");

  $("select option").each(function () {
    $(this).attr({ 'title': $(this).html() });
  });

  //Add a rule for validationg member code alpha.
  $.validator.addMethod('memberCodeAlpha', validateMemberCodeAlpha);

  $(".numeric").keypress(checkIsNumeric);
  $(".integer").keypress(checkIsInteger);
  $(".percentage").keypress(checkIsNumericForThreeDecimals);
  $(".alphabet").keypress(checkIsAlphabet);
  $(".alphabetsOnly").keypress(checkIsAlphabetsOnly);
  $(".twoChars").keypress(checkIsTwoCharacters);
  $(".alphaNumeric").keypress(checkIsAlphaNumeric);
  $(".alphaNumericWithSpace").keypress(checkIsAlphaNumericwithSpace);
  $(".popupTextField").keypress(checkIsPopupTextField);
  $(".possitiveInteger").keypress(checkIsPossitiveInteger);
  $(".validCharsTextarea").keypress(checkValidCharsOnly);

  $(".alphabetsWithSpace").keypress(checkIsAlphabetswithSpace);
}

function cancelFunction() {
  return false;
}

function validationSetSelectMethod() {
  $.validator.addMethod('selectNone',
    function (value, element) {
      if (element.value == '') {
        return false;
      }
      else return true;
    },
    "Please Select");
}

var USD = 840;
var SmiA = "2";
var SmiM = "5";

//CMP #553: ACH Requirement for Multiple Currency Handling
function validationBillingCurrencyForAchOrM() {
    jQuery.validator.addMethod('validationBillingCurrForAchOrM',
      function (value, element) {

          var smi = $('#SettlementMethodId').val();
          if (element.value != USD && (smi == SmiA || (smi == SmiM && isAchOrDualMember))) {
              if (element.value != $('#ListingCurrencyId').val()) {
                  return false;
              }
          }
          return true;
      });
}

//As per the point number 1.1.22 in IS-web specification, application should accept only ASCII range 32 to 126 characters 
//This function validates characters entered in every textbox.
function validateAllowedCharacters(value, element, params) {
  if (value != '') {
    //Ascii is defined as the characters in the range of 000-177 (octal). In regular expression, Octal number for range 32 to 126 are used 
    var re = new RegExp("^[\040-\176]*$");

    if (!value.match(re)) {
      //CMP #636: Standard Update Mobilization
      //If message is not given in the js file then default message will display.
      if (typeof this.settings.messages[element.name] === 'undefined')
        this.settings.messages[element.name] = "Field value contains invalid characters or empty";
      return false;
    }
    else
      return true;
  }
  else
    return true;
}

//As per the point number 1.1.22 in IS-web specification, application should accept only ASCII range 32 to 126 characters 
//This function validates characters entered in every textbox.
function validateAllowedCharactersWithExclimation(value, element, params) {
    
  var strValue = value.replace("!!!", "");
  if (strValue != '') {
    //Ascii is defined as the characters in the range of 000-177 (octal). In regular expression, Octal number for range 32 to 126 are used 
    var re = new RegExp("^[\040-\176]*$");


    if (!strValue.match(re)) {
      this.settings.messages[element.name] = "Field value contains invalid characters or empty";
      return false;
    }
    else
      return true;
  }
  else
    return true;
}


// Required for Aircraft Reg. No. dynamic field.
function validateAircraftRegNo(value, element, params) {
  if (value != '') {

    var re = new RegExp("^([a-z]|[A-Z]|[0-9]){1,10}$");    

    if (!value.match(re)) {
      this.settings.messages[element.name] = "Value contains invalid characters.";
      return false;
    }
    else
      return true;
  }
  else
    return true;
}

//As per the point number 1.1.22 in IS-web specification, application should accept only ASCII range 32 to 126 characters and \n and \r.
//This function validates characters entered in every textbox.
function validateAllowedCharactersForTextAreaFields(value, element, params) {
  if (value != '') {
    //Ascii is defined as the characters in the range of 000-177 (octal). In regular expression, Octal number for range 32 to 126 are used/
    //Also, allow horizontal tab, line feed, carriage return.
    var re = new RegExp("^[\011\012\015\040-\176]*$");
    
    if (!value.match(re)) {
      this.settings.messages[element.name] = "Value contains invalid characters.";
      return false;
    }
    else
      return true;
  }
  else
    return true;
}

//This function validates characters entered in TextArea.
function validateAllowedCharactersForTextArea(value, element, params) {
  if (value != '') {
    // SCP304020: TFS9360 UAT 1.6: Misc Codes Setup
    // Ascii is defined as the characters in the range of 000-177 (octal). In regular expression, Octal number for range 32 to 126 are used 
    var re = new RegExp("^[\040-\176]*$");
    if (!value.match(re)) {
      this.settings.messages[element.name] = "Value contains invalid characters.";
      return false;
    }
    else
      return true;
  }
  else
    return true;
}

//Check if Pax Textarea Field is valid or not. This function check for max allowed chars in line and max allowed lines for textarea field.
function isPaxTextareaFieldValid(value, element, params) {
  
  var isValidProrateSlip = true;
  if (value != '') {
    //Get lines entered for field
    lines = value.split(/\r\n|\r|\n/);

    var maxCharsInLine = params[0];
    var maxLines = params[1];
    //If entered no of lines are more than maximum allowed line, return false.
    if (lines.length > maxLines) {
      this.settings.messages[element.name] = "Number of allowed characters exceeded.";
      return false;
    }

    var linesWithinLimits = 0;
    //var actualLinesInProrateSlip = lines.length;
    for (i = 0; i < lines.length; i++) {
      if (lines[i].length <= maxCharsInLine) {
        linesWithinLimits++;
      }
    }

    var actualLinesInProrateSlip = linesWithinLimits;
    //Check if characters in each lines are within maximum characters allowed in one line
    for (i = 0; i < lines.length; i++) {
      if (lines[i].length > maxCharsInLine) {
        //If characters in line exceeds max characters in  one line limit, devide it by NoOfAllowedcharactersInOneLine.
        //Add quotient to no of lines.
        var quotient = lines[i].length / maxCharsInLine;
        actualLinesInProrateSlip = actualLinesInProrateSlip + quotient;

        //If actual no of lines are more than maxLines, return false.
        if (actualLinesInProrateSlip > maxLines) {
          isValidProrateSlip = false;
          break;
        }
      }
    }
  }
  if (isValidProrateSlip == false)
    this.settings.messages[element.name] = "Number of allowed characters exceeded.";
  return isValidProrateSlip;
}

function validatePeriod(value, element, params) {

      if (params[0] == "Migration") {
        if ($("#" + params[1]).val() == 3) {
          return isValidPeriod(value);
        }
        else
          return true;
    }
    if (params[0] == "WebMigration") {

        if (value == "") {
            return true;
        }
        else
            return isValidPeriod(value);
            
 
    }

  if (params[0] == "Reinstatement") {
    if ((value == "") && ($("#" + params[1]).val() == "2")) {
      return true;
    }

    if ((value != "") && ($("#" + params[1]).val() == "2")) {
      if ((isValidPeriod(value)) && (ValidateFuturePeriod($("#" + params[3])))) {
        if (confirmReinstatementPeriod()) {
          return true;
        }
        else {
          $("#" + params[3]).val("");
          return false;
        }
      }
      else {
        return false;
      }
    }
    else
      return true;
  }

  if (params[0] == "auditTrail") {
    if ($("input:radio[name=rdDateOrClearancePeriod]:checked").val() == "ClearancePeriod") {
      return isValidPeriod(value);
    }
    else
      return true;
  }
  else
    return isValidPeriod(value);
}

function validateInvoiceNumberRange(value, element, params) {
    if (($("#" + params[2]).prop('checked')) || ($("#" + params[3]).val() == 'True') || ($("#" + params[3]).val() == 'true')) {
    var prefixlength = $("#" + params[0]).val().length;
    var RangeFromlength = $("#" + params[1]).val().length;
    if (((prefixlength + RangeFromlength) > 10) || (prefixlength == 0) || (RangeFromlength == 0))
      return false;
    if ((Number($("#" + params[4]).val())) > (Number($("#" + params[1]).val())))
      return false;
    else
      return true;
  }

  return true;

}

//Added for dynamic fields data size check, to fix issue If range is 999999999999999.999 and error message is displayed as 1000000000000000
function validateDynamicDatasizeRange(value, element) {
  var minDatasize = $(element).attr('minDataSize');
  var maxDatasize = $(element).attr('maxDataSize');
  if (value != '') {
    if (minDatasize != '' && maxDatasize != '') {
      var minValue = Number(minDatasize);
      var maxValue = Number(maxDatasize);
      var elementValue = Number(value);
      if (minValue <= elementValue && elementValue <= maxValue) {
        //If range is valid, check for valid decimal places
        var allowedDecimal = $(element).attr('dataSizeDecimal');
        var numAllowedDecimal = Number(allowedDecimal);
        if (numAllowedDecimal != 0) {
          if (value.indexOf('.') != -1) {
            //if value contain decimal point, get count of digits after decimal places.
            var decimalsInValue = value.substr(value.indexOf('.') + 1, value.length);
            var decimalsInValueLength = decimalsInValue.length;

            if (decimalsInValueLength <= numAllowedDecimal)
              return true;
            else {
              this.settings.messages[element.name] = "Please enter a value with " + numAllowedDecimal + " decimals.";
              return false;
            }
          }
          //If value does not contain digits after decimal point, return true
          else
            return true;
        }
        //If field is not decimal, then return true
        else {
          if (value.indexOf('.') != -1) {
            this.settings.messages[element.name] = "Please enter integer value.";
            return false;
          }
          else
            return true;
        }
      }
      else {
        this.settings.messages[element.name] = "Please enter a value between " + minDatasize + " and " + maxDatasize;
        return false;
      }
    }
    else
      return false;
  }
  return true;
}

//Added validation for dynamic field clerance period format
function validateDFClearancePeriod(value, element) {
  var isValidPeriod = true;
  if (value != '') {
    if (value.length != 6)
      isValidPeriod = false;

    if (isValidPeriod != false) {
      var year = value.substring(0, 2);
      if (isNaN(year))
        isValidPeriod = false;
      var month = value.substring(2, 4) - 1;
      if (month < 0 || month > 11)
        isValidPeriod = false;
      else {
        period = value.substring(4, 6);
        if (period != '01' && period != '02' && period != '03' && period != '04') {
          isValidPeriod = false;
        }
      }
    }
  }
  if (isValidPeriod == false) {
    this.settings.messages[element.name] = "Invalid value. Please enter a value in format YYMMPP";
  }

  return isValidPeriod;
}

//Added validation for Effective period format YYYYMMPP
function validateEffectivePeriod(value, element) {
    var isValidPeriod = true;
    if (value != '') {
        if (value.length != 8)
            isValidPeriod = false;

        if (isValidPeriod != false) {
            var year = value.substring(0, 4);
            if (isNaN(year))
                isValidPeriod = false;
            //SCP258660 - Unable to update Reason Code - RM Amount Map master
            //comment: value.substring(4, 2) - 1;
            var month = value.substring(4, 6);
            if (month < 1 || month > 12)
                isValidPeriod = false;
            else {
                period = value.substring(6, 8);
                if (period != '01' && period != '02' && period != '03' && period != '04') {
                    isValidPeriod = false;
                }
            }
        }
    }
    if (isValidPeriod == false) {
        this.settings.messages[element.name] = "Invalid value. Please enter a value in format YYYYMMPP";
    }

    return isValidPeriod;
}


//Added validation for dynamic field Latitude format
function validateDFLatitude(value, element) {
  var isValidLatitude = true;
  if (value != '') {
    if (value.length > 9)
      isValidLatitude = false;

    //Substitute value 0 can be added
    if (value == '0')
      return true;

    var re = new RegExp("^[0-9]{1,2}\:[0-9]{2}\:[0-9]{2}[N|S]$");
    //"((\d{2,3}):(\d{2,3}):(\d{2,3})[N|W])");
    //[NS] \d{1,}(\:[0-5]\d){2}.{0,1}\d{0,},[EW] \d{1,}(\:[0-5]\d){2}.{0,1}\d{0,}

    if (!value.match(re))
      isValidLatitude = false;

  }
  if (isValidLatitude == false) {
    this.settings.messages[element.name] = "Invalid value. Please enter a value in format #n:nn:nnN/S";
  }

  return isValidLatitude;
}

//Added validation for dynamic field Longitude format
function validateDFLongitude(value, element) {
  var isValidLongitude = true;
  if (value != '') {
    if (value.length > 10)
      isValidLongitude = false;

    //Substitute value 0 can be added
    if (value == '0')
      return true;

    var re = new RegExp("^[0-9]{1,3}\:[0-9]{2}\:[0-9]{2}[E|W]$");

    if (!value.match(re))
      isValidLongitude = false;

  }
  if (isValidLongitude == false) {
    this.settings.messages[element.name] = "Invalid value. Please enter a value in format ##n:nn:nnE/W";
  }

  return isValidLongitude;
}

//Added validation for dynamic field datetimepicker
function validateDFDateTimePicker(value, element) {
    var isValid = true;
   //TFS#9910:IE-Version 11: Invalid date captured from Calender Date Picker for Miscellaneous.
   //Desc:Prior to Jquery upgradtion parseDate function excepts 'dd-M-yy' format but hence forth we are using the 'dd-M-y' format.
   var _dateFormatDF = 'dd-M-y';
  if (value != '') {
    if (value.length != 18)
      isValid = false;
    if (value.indexOf('T') != -1) {
      var dateValue = value.substring(0, value.indexOf('T'));

      try {
        //Validate date
          jQuery.datepicker.parseDate(_dateFormatDF, dateValue, null);

      }
      catch (error) {
        isValid = false;
      }
      //Validate time
      var timeValue = value.substring(value.indexOf('T') + 1, value.length);
      if (timeValue.substring(2, 3) !== ':' || timeValue.substring(5, 6) !== ':')
        isValidPeriod = false;
      var hr = timeValue.substring(0, 2);
      if (isNaN(hr))
        isValidPeriod = false;
      else {
        if (Number(hr) < 0 || Number(hr) > 23)
          isValidPeriod = false;
      }
      var min = timeValue.substring(3, 5);
      if (isNaN(min))
        isValidPeriod = false;
      else {
        if (Number(min) < 0 || Number(min) > 59)
          isValidPeriod = false;
      }
      var sec = timeValue.substring(6, 8);
      if (isNaN(sec))
        isValidPeriod = false;
      else {
        if (Number(sec) < 0 || Number(sec) > 59)
          isValidPeriod = false;
      }

    }
  }
  if (isValid == false) {
    this.settings.messages[element.name] = "Invalid Value. Please enter a value in format DD-MMM-YYYY" + "T" + _timeFormatForDateTimePicker;
  }
  return isValid;
}

function validateMemberCodeAlpha(value, element) {
  var regEx = "^\\d*[a-zA-Z][a-zA-Z0-9]*$";
  var re = new RegExp(regEx);
  return re.test(value);
}

function validationSetRegexMethod() {
  $.validator.addMethod(
    "regex",
    function (value, element, regexp) {
      var check = false;
      var re = new RegExp(regexp);
      return this.optional(element) || re.test(value);
    },
    "Please check your input."
  );
}

function checkIsNumeric(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }

  var re = new RegExp("^\\d+(?:\\.\\d{0,2})?$");

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function AllowedTypesSupportingDocuments(eventObj) {

  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^(([A-Za-z0-9-'.,]+|&[^#])*&?)$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkIsInteger(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var nbr = (window.event) ? event.keyCode : eventObj.which;
  var sKey = String.fromCharCode(nbr);

  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }

  var re = new RegExp("^\\d+$");

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkIsPossitiveInteger(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }

  var re = new RegExp("^\\d+$");
  if (sValue <= 0)
    window.event.returnValue = false;
  if (!sValue.match(re))
    window.event.returnValue = false;
}


function checkIsNumericForThreeDecimals(eventObj) {

  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^\\d+(?:\\.\\d{0,3})?$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkIsAlphaNumeric(eventObj) {

  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }

  //var regEx = /^\w+$/i; // allows underscore
  var regEx = /[a-zA-Z0-9]+$/i; // without underscore

  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkValidCharsOnly(eventObj) {

  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var code = eventObj.keyCode ? eventObj.keyCode : eventObj.which ? eventObj.which : eventObj.charCode;

  // Below check required for firefox.
  if (code == 8 || code == 9) // back space key, tab key
    return true;

  var sKey = String.fromCharCode(code);
  sValue = sValue + sKey;

  var re = new RegExp("^[\040-\176]*$");

  if (!sValue.match(re)) {
    stopEvent(eventObj);
  }
}

function stopEvent(e) {
  if (!e)
    if (window.event)
      e = window.event;
    else
      return;
  if (e.cancelBubble != null)
    e.cancelBubble = true;
  if (e.stopPropagation)
    e.stopPropagation();
  if (e.preventDefault)
    e.preventDefault();
  if (window.event)
    e.returnValue = false;
  if (e.cancel != null)
    e.cancel = true;
}  // stopEvent

function checkIsAlphaNumericwithSpace(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^[a-zA-Z0-9\040]+$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkIsAlphabetswithSpace(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^[a-zA-Z\040]+$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkIsAlphabet(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^[a-zA-Z]$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}


function checkIsAlphabetsOnly(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^[a-zA-Z]+$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function checkIsTwoCharacters(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "^[a-zA-Z0-9]{0,2}$";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

// Function to take care of max length for text area.
function maxLength(object, maxLen) {
  if (object.value.length >= maxLen) {
    event.returnValue = false;
    return false;
  }
}

// Function to take care of max length when data is pasted into field.
function maxLengthPaste(field, maxChars) {
  event.returnValue = false;
  if ((field.value.length + window.clipboardData.getData("Text").length) > maxChars) {
    return false;
  }
  event.returnValue = true;
}

function checkIsPopupTextField(eventObj) {
  var obj = eventObj.currentTarget;
  var sValue = obj.value;
  var sKey = String.fromCharCode(window.event.keyCode);
  if (document.selection.createRange().text == sValue) {
    sValue = sKey;
  } else {
    sValue = sValue + sKey;
  }
  var regEx = "[^`]";
  var re = new RegExp(regEx);

  if (!sValue.match(re))
    window.event.returnValue = false;
}

function addClassRules() {

  //Add class rule for validation rule to check for characters in ascii range 32 to 126
  jQuery.validator.addClassRules("validateCharacters", {
    allowedCharacters: true
  });

  //Add class rule for validation rule to check for characters in ascii range 32 to 126
  jQuery.validator.addClassRules("validateCharactersForTextArea", {
    allowedCharactersForTextArea: true
  });

  //Add class rule for validation rule to check for alphabets.
  jQuery.validator.addClassRules("alphabetsOnly", {
    validateAlphabets: true
  });

  jQuery.validator.addClassRules("datePicker", {
    validateDate: true
  });

  jQuery.validator.addClassRules("dynamicFieldAircraftRegNo", {
    dynamicFieldAircraftRegNo: true
  });

  jQuery.validator.addClassRules("amt_16_3", {
    min: -9999999999999.99,
    max: 9999999999999.99
  });

  jQuery.validator.addClassRules("amt_6_3", {
    min: -999.999,
    max: 999.999
  });

  jQuery.validator.addClassRules("amt_11_3", {
    min: -99999999.999,
    max: 99999999.999
  });

  jQuery.validator.addClassRules("pos_amt_11_3", {
    min: 0,
    max: 99999999.999
  });

  jQuery.validator.addClassRules("neg_amt_11_3", {
    min: -99999999.999,
    max: 0
  });

  jQuery.validator.addClassRules("amt_10_3", {
    min: -9999999.99,
    max: 9999999.99
});

  jQuery.validator.addClassRules("amt_5_3", {
     min: -99.999,
     max: 99.999  
  });
  
  jQuery.validator.addClassRules("amt_12_3", {
    min: -999999999.99,
    max: 999999999.99
  });

  jQuery.validator.addClassRules("amt_15_3", {
    min: -999999999999.999,
    max: 999999999999.999
  });

  jQuery.validator.addClassRules("exchangeRate", {
    min: -99999999999.99999,
    max: 99999999999.99999
  });

  jQuery.validator.addClassRules("amt_percent", {
    min: -99.999,
    max: 99.999
  });

  jQuery.validator.addClassRules("requiredDigits", {
    required: true, digits: true
  });

  jQuery.validator.addClassRules("pos_amt_12_3", {
    min: 0,
    max: 999999999.99
  });

  jQuery.validator.addClassRules("pos_amt_15_3", {
    min: 0,
    max: 999999999999.999
  });

  jQuery.validator.addClassRules("neg_amt_15_3", {
    min: -999999999999.999,
    max: 0
  });

  jQuery.validator.addClassRules("neg_amt_12_3", {
    min: -999999999.99,
    max: 0
  });

  jQuery.validator.addClassRules("num_5_2", {
    min: -999.99,
    max: 999.99
  });

  jQuery.validator.addClassRules("pos_num_7_3", {
    min: 0,
    max: 9999.999
  });

  jQuery.validator.addClassRules("num_18_3", {
    min: -999999999999999,
    max: 999999999999999
  });

  jQuery.validator.addClassRules("num_18_4", {
    min: -99999999999999.9999, /*TODO: put correct values here*/
    max: 99999999999999.9999
  });

  jQuery.validator.addClassRules("pos_num_18_4", {
    min: 0.0001,
    max: 99999999999999.9999
  });

  jQuery.validator.addClassRules("amt_14_3", {
    min: -99999999999999.999,
    max: 99999999999999.999
  });
  
  $.validator.addMethod(
  "alphaNumeric",
  function (value, element) {
    var re = new RegExp(/^\w+$/i);
    return this.optional(element) || re.test(value);
  },
  "Please enter valid input"
  );


  $.validator.addMethod(
  "alphaNumericChar",
  function (value, element) {
      //Ascii is defined as the characters in the range of 000-177 (octal). In regular expression, Octal number for range 32 to 126 are used 
      var re = new RegExp("^[\040-\176]*$");
      return this.optional(element) || re.test(value);
  },
  "Please enter valid input"
  );

  $.validator.addMethod("multiemail", function (value, element) {
    if (this.optional(element)) // return true on optional element 
      return true;

    var emails = value.split(';');
    valid = true;
    for (var i in emails) {
      value = $.trim(emails[i]);
      valid = valid && jQuery.validator.methods.email.call(this, value, element);
    }
    return valid;
  }, 'One or more email addresses are invalid');

  $.validator.addMethod("checkDigit", function (value, element) {
    if (this.optional(element)) // return true on optional element 
      return true;

    var checkDigit = value;
    valid = false;
    if ((checkDigit >= 0 && checkDigit <= 6) || checkDigit == 9)
      valid = true;
    return valid;
  }, 'Valid values for check digit are 0-6, 9');

  $.validator.addMethod("fimCouponNumber", function (value, element) {
      if (this.optional(element)) // return true on optional element 
          return true;

      var FimBmCmIndicatar = $("#FIMBMCMIndicatorId").val();
      var fimCouponNo = value;
      valid = false;
      if (FimBmCmIndicatar == 3 || FimBmCmIndicatar == 4) {
          valid = true;
      }
        //SCP 111527: Problem rejection coupon number
        //Desc: Incomplete check for validation is corrected.
        //Date: 25-Apr-2013
      if (fimCouponNo == 1 || fimCouponNo == 2 || fimCouponNo == 9 || fimCouponNo == 3 || fimCouponNo == 4)
          valid = true;
      return valid;
  }, 'Valid values for FIM Coupon Number are 1, 2, 9');

  /* CMP #596: Length of Member Accounting Code to be Increased to 12 
  Desc: Field is mandatory with a minimum length of 3 and maximum length of 12.
  Ref: FRS Section 3.1 Point 10 */
  $.validator.addMethod(
  "minMemPrefix",
  function (value, element) {
      if (value.length == 4 && Number(value) >= 0) {
            if (Number(value) >= 3600) {
                return true;
            }
            else {
                return false;
            }
        }
          return true;
  },
  "4 numeric codes should have a value 3600 or greater"
  );

}


function SetPageToViewMode(viewMode) {

  if (viewMode) {
    $(':input').not(':button').attr('disabled', 'disabled');
    //Changes to remove dark background for readonly fields in payables
    $(':input').not(':button').removeAttr('readOnly');
    $('select').attr('disabled', 'disabled');
    //$('textarea').attr('disabled', 'disabled');
    $('textarea').removeAttr("disabled");
    $('textarea').attr('readOnly', 'readOnly');
    $('input[type=checkbox]').attr('disabled', 'disabled');
    $('.primaryButton').hide();
    $('input[type=submit]').hide();
    $('.datePicker').datepicker('disable');

    $isOnView = true; // for tax, vat, attachment pop ups
  }
}

function SetPageToViewModeEx(viewMode, formId) {
  if (viewMode) {
    $(':input', formId).not(':button').attr('disabled', 'disabled');
    //Changes to remove dark background for readonly fields in payables
    $(':input', formId).not(':button').removeAttr('readOnly');
    $('select', formId).attr('disabled', 'disabled');
    $('textarea', formId).removeAttr('disabled');
    $('textarea', formId).attr('readOnly', 'true');
    $('input[type=checkbox]', formId).attr('disabled', 'disabled');
    $('.primaryButton', formId).hide();
    $('input[type=submit]', formId).hide();
    $('.datePicker', formId).datepicker('disable');

    $isOnView = true; // for tax, vat, attachment pop ups
  }
}

function SetPageModeToCreateMode(createMode) {
  if (createMode) {
    $('input[type=text]:not(.populated)').removeAttr('value');
    $('#CheckDigit').val('');
    $("#AttachmentIndicatorOriginal").val("No");
  }
}

function SetPageModeToClone(cloneMode) {
  if (cloneMode)
    $("#AttachmentIndicatorOriginal").val("No");
}

function ResetForm(formId) {
  $(':input', formId)
 .not(':button, :submit, :reset, :hidden')
 .val('')
 .removeAttr('checked')
 .removeAttr('selected');
}

function prefixAsterisk(controlId) {
  $(controlId + " span").remove();
  $(controlId).html("<span style=color:red>*</span> " + $(controlId).html());
}

function removeAsteriskSpan(controlId) {
  $(controlId + " span").remove();
}


function isLeapYear(year) {
  return !(year % 4) && (year % 100) || !(year % 400) ? true : false;
}

function isValidDate(year, month, day) {
  //assumption: day- min: 1; max: 31
  var allowedDays = new Array(31, 30, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);

  if (!isLeapYear(year) && month == 2 && day > 28)
    return false;

  if (isLeapYear(year) && month == 2 && day > 29)
    return false;

  if (day > allowedDays[month - 1])
    return false;

  return true;
}

function isValidPeriod(value) {
  var tokenArray = value.split("-");
  if (tokenArray.length != 3) {
    return false;
  }
  else {
    var period = tokenArray[2];
    var month = tokenArray[1];
    var year = tokenArray[0];

    var periodArray = new Array("01", "02", "03", "04");
    var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC");
    var isValidYear = /^[0-9]+$/.test(year);

    if ($.inArray(period, periodArray) <= -1 || $.inArray(month.toUpperCase(), monthArray) <= -1 || !isValidYear || year.length != 4) {
      return false;
    }

    return true;
  }

}

function isValidFileType(value) {
  if (value == "")
    return true;
  //var re = new RegExp("^(([A-Za-z0-9-'.,]+|&[^#])*&?)$");
  var re = new RegExp("^[\.]([a-zA-Z]{1}[0-9a-zA-Z]{0,5})+((,{1}[.][a-zA-Z]{1}[0-9a-zA-Z]{0,5}){0,9})+$");
  if (!value.match(re))
    return false;
  else
    return true;
}

function checkForNumeric(eventObj) {

  var evtobj = window.event ? window.event : eventObj;
  var unicode = evtobj.keyCode ? evtobj.keyCode : evtobj.charCode;

  // for backspace
  if (unicode == 8)
    return true;

  var sKey = String.fromCharCode(unicode);
  var re = new RegExp("^\\d+$");

  if (!sKey.match(re)) {
    if (window.event)
      evtobj.returnValue = false;
    else
      evtobj.preventDefault();
  }
}

// Function for validating Bank details entered for a location
function validateBankDetails(value, element) {
  var validBankDetails = true;
  if (value != '') {

    var re = new RegExp("^([ ]|[a-z]|[A-Z]|[0-9]){1,80}$");

    if (!value.match(re))
      validBankDetails = false;

  }
  return validBankDetails;
}

// Function for validating AWB Serial Number.
function validateAWBSerialNumber(value, element) {
 
  if (value != '' && value != null) {
    var awbSerialNo = value.substring(0, 7);
    if (awbSerialNo == 0) {
      return false;
    }

    return true; // valid
  }

  return true;
}

function validateAlphabets(value, element, params) {
  if (value != '') {
    //Ascii is defined as the characters in the range of 000-177 (octal). In egular expression, Octal number for range 32 to 126 are used 
    var re = new RegExp("^[A-Za-z]*$");

    if (!value.match(re)) {
      this.settings.messages[element.name] = "Please enter alphabets only.";
      return false;
    }
    else
      return true;
  }
  else
    return true;
}

function validateDate(value, element, params) {
    if (value != '' && value != 'DD-MMM-YY') {
    var _dateFormat = 'dd-M-y';
    var allowedFormats = ['dd-mm-yy', 'd-mm-yy', 'dd-m-yy', 'd-m-yy', 'd-M-yy', _dateFormat, 'd-m-y', 'dd-m-y', 'd-mm-y', 'dd-mm-y', 'd-M-y'];

    // Check if any of the date formats is valid.
    var parsedDate = isValidDateFormat(allowedFormats, value);

    if (parsedDate == undefined) {
      this.settings.messages[element.name] = "Invalid date.";
      return false;
    } 
    else {
      $("#"+element.id).datepicker('setDate', parsedDate);
      return true;
    }   
  }
  else {
    return true;
  }
}
