var _expandGroupUrl;
var groupDiv;
var groupHiddenFielId;
_amountDecimals = 3;
var lineItemDetailNotExpectedValue;
var _getSubdivisionCodesMethod;

function InitialiseLineItemDetail(expandGroupUrl, populateSubdivCodesUrl) {
  _expandGroupUrl = expandGroupUrl;
  _getSubdivisionCodesMethod = populateSubdivCodesUrl;
  //Added to create range using min and max for dynamic fields
  jQuery.validator.autoCreateRanges = true;

  // If pageMode == Create/Edit, register control events and validation rules
  if ($isOnView != true) {
    $("#Quantity").blur(function () {
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        setLIneDetailTotal();
      }
    });

    $("#ScalingFactor").blur(function () {
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        setLIneDetailTotal();
      }
    });

    $("#UnitPrice").blur(function () {
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        setLIneDetailTotal();
      }
    });

    $("#ChargeAmount").blur(function () {
      CalculateLineDetailNetTotal();
    });

    $("#TaxAmount").blur(function () {
      CalculateLineDetailNetTotal();
    });

    $("#VatAmount").blur(function () {
      CalculateLineDetailNetTotal();
    });

    $("#TotalAddChargeAmount").blur(function () {
      CalculateLineDetailNetTotal();
    });

    $('.primaryButton').click(function () {      
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        setLIneDetailTotal();
      }
      CalculateLineDetailNetTotal();
    });

  if ($('#MinimumQuantityFlag').prop('checked')) {
      $('#ChargeAmount').attr('readonly', false);
    };
  $.validator.addMethod("ValidateStartDate", ValidateStartDate, "Service Start Date should not be greater than or equal to Service End Date. If service is provided on the same day, please enter Service End Date only.");

  $('#LineItemDetailForm').validate({
    rules: {
      StartDay: {
        required: function (element) {
          if (($('#StartDay').val() == '' && $('#ServiceStartDateDropdown').val() == '')) {
            return false;
          }

          return !($('#StartDay').val() != '' && $('#ServiceStartDateDropdown').val() == '');
        }
      },
      EndDay: {
        required: function (element) {
          return $('#EndDay').attr('disabled') == '';
        }
      },
      ServiceStartDateDropdown: {
        required: function () {
          if (($('#StartDay').val() == '' && $('#ServiceStartDateDropdown').val() == '')) {
            return false;
          }

          return !($('#StartDay').val() == '' && $('#ServiceStartDateDropdown').val() != '');

        },
        ValidateStartDate: true,
        validStartDate: true
      },
      ServiceEndDateDropdown: {
        required: function () {
          return $('#ServiceEndDateDropdown').attr('disabled') == '';
        },
        validEndDate: true
      },
      Description: {
        required: true,
        maxlength: 240
      },
      ScalingFactor: {
      required: true,
      digits: true
      },
      UnitPrice: "required",
      Quantity: "required",
      UomCodeId: "required",
      ChargeAmount: { required: function () {
        return $('#MinimumQuantityFlag:checked').val() != undefined;
      }
      }
    },
    messages: {
      StartDay: "Enter the Service Start Day or set the month to 'Please Select'.",
      ServiceStartDateDropdown: {
        required: "Service Start Date Required."
      },
      EndDay: { required: "Service End Date Required." },
      ServiceEndDateDropdown: { required: "Service End Date Required." },
      Description: "Description Required and should be of maximum 240 characters.",
      Quantity: "Quantity Required and should be within 0.0001 and 99999999999999.9999.",
      UomCodeId: { required: "UOM Code Required." },
      UnitPrice: "Enter a valid unit price (dot should be used as a decimal separator).",
      ChargeAmount: { required: "Line Total Required" },
      //SCP220346: Inward Billing-XML file mandatory field
      ScalingFactor: "Scaling factor is invalid (valid values 1-99999)."
    },
    invalidHandler: function (form, validator) {
      $.watermark.showAll();
    },
    submitHandler: function (form) {
      CheckForValidationForParentValue();
      if (!ValidateProductId()) return false;

      $('#EndDay').removeAttr('disabled');
      $('#ServiceEndDateDropdown').removeAttr('disabled');
      $('#Save').attr('disabled', true);
      $('#SaveDuplicate').attr('disabled', true);
      $('#SaveReturn').attr('disabled', true);
      $('#First').attr('disabled', true);
      $('#Last').attr('disabled', true);
      $('#Next').attr('disabled', true);
      $('#Previous').attr('disabled', true);
      
      $('#UomCodeId').attr('disabled', false);
      // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
      onSubmitHandler(form);
    }
  });
    setFocusAndBlur('#StartDay');
    trackFormChanges('LineItemDetailForm');
  }
  // to check for valid date-month combinations.
  validationStartDateMethod();
  validationEndDateMethod();

  if ($('.fieldContainer').has('.recommended').length) {
    $('#RecommendedFootNote', "#content").removeClass('hidden');
  }
}

function CheckForValidationForParentValue() {
  var errorString = '';
  //Check validation for Service start date is within service date range of LineItemDetail
  var isValid = ValidateServiceStartDateRange();
  if (isValid == false)
    errorString = errorString + "Service Start Date is not in Service Date range of LineItem.";

  //Check validation for Service end date is within service date range of LineItemDetail
  isValid = ValidateServiceEndDateRange();
  if (isValid == false) {
    if (errorString != '')
      errorString += '\n';
    errorString = errorString + "Service End Date is not in Service Date range of LineItem.";
  }

  if (errorString != '')
    alert(errorString);
}

function validationStartDateMethod() {
  $.validator.addMethod(
    "validStartDate",
    function (value, element) {
      var yearMonthTokens = value.split('-');
      var year = yearMonthTokens[1];
      var month = yearMonthTokens[0];
      var day = $('#StartDay').val();
      return this.optional(element) || isValidDate(year, month, day);
    },
    "Invalid date."
);
}

function validationEndDateMethod() {
  $.validator.addMethod(
    "validEndDate",
    function (value, element) {
      var yearMonthTokens = value.split('-');
      var year = yearMonthTokens[1];
      var month = yearMonthTokens[0];
      var day = $('#EndDay').val();
      return this.optional(element) || isValidDate(year, month, day);
    },
    "Invalid date."
);
}

function ValidateStartDate(value) {
  var selectedStartMonth = $('#ServiceStartDateDropdown').val();
  var selectedStartVal = selectedStartMonth.split('-');
  var selectedStartDay = $('#StartDay').val();
  var startDate = new Date(selectedStartVal[1], selectedStartVal[0] - 1, selectedStartDay);

  var selectedendMonth = $('#ServiceEndDateDropdown').val();
  var selectedendVal = selectedendMonth.split('-');
  var selectedendDay = $('#EndDay').val();
  var endDate = new Date(selectedendVal[1], selectedendVal[0] - 1, selectedendDay);
  if (endDate <= startDate)
    return false;
  else
    return true;
}

function CheckForParentValue(value) {
  var parentProductId = $('#LineItemProductId').val();
  var productId = $('#ProductId').val();
  if (parentProductId != '' && parentProductId != productId) {
    return false;
  }
  else
    return true;
}

//Check validation for Service end date is within service date range of LineItemDetail
function ValidateServiceEndDateRange() {
  var selectedendMonth = $('#ServiceEndDateDropdown').val();
  var selectedendVal = selectedendMonth.split('-');
  var selectedendDay = $('#EndDay').val();
  var endDate = new Date(selectedendVal[1], selectedendVal[0] - 1, selectedendDay);

  var selectedParentEndDate = $('#LineItemServiceEndDate').val().split('-');
  var parentEndDate = new Date(selectedParentEndDate[2], selectedParentEndDate[1] - 1, selectedParentEndDate[0]);

  if (endDate > parentEndDate) {
    return false;
  }
  else
    return true;

}

//Check validation for Service start date is whithin service date range of LineItemDetail
function ValidateServiceStartDateRange() {
  var selectedStartMonth = $('#ServiceStartDateDropdown').val();
  var selectedStartVal = selectedStartMonth.split('-');
  var selectedStartDay = $('#StartDay').val();
  var startDate = new Date(selectedStartVal[1], selectedStartVal[0] - 1, selectedStartDay);

  var selectedParentStartDate = $('#LineItemServiceStartDate').val().split('-');
  var parentStartDate = new Date(selectedParentStartDate[2], selectedParentStartDate[1] - 1, selectedParentStartDate[0]);

  if (startDate < parentStartDate) {
    return false;
  }
  else
    return true;
}

$('#MinimumQuantityFlag').bind('change', OnChangeMinimumQuanitityFlag);

function OnChangeMinimumQuanitityFlag() {  
  if ($(this).prop('checked')) {
    //do the stuff that you would do when 'checked' 
   //TFS#9934 - IE:Version 11: "Line Detail Total" is disabled when "Minimum Quantity Flag" is true.
    $('#ChargeAmount').attr('readonly', false);
    $('#ChargeAmount').val('');
    CalculateLineDetailNetTotal();
    return;
  }
  //Here do the stuff you want to do when 'unchecked' 

  $('#ChargeAmount').attr('readonly', true);

  setLIneDetailTotal();
}

function setLIneDetailTotal() {
  if ($('#MinimumQuantityFlag:checked').val() == undefined) {
    //trim white space
    var qty = Number($("#Quantity").val().replace(/^\s\s*/, '').replace(/\s\s*$/, ''));
    var unitPrice = Number($("#UnitPrice").val().replace(/^\s\s*/, '').replace(/\s\s*$/, ''));
    var scalingFactor = Number($("#ScalingFactor").val().replace(/^\s\s*/, '').replace(/\s\s*$/, ''));

    var total = 0;
    // in case of invalid entry of Base Amount or Percent, set calculated value as zero.
    if (isNaN(qty) || isNaN(unitPrice) || isNaN(scalingFactor)) {
      total = 0;
    }
    else {
      total = (qty * unitPrice) / scalingFactor;

      if (scalingFactor == 0)
        total = 0;
    }

    $("#ChargeAmount").val(total.toFixed(_amountDecimals));
    CalculateLineDetailNetTotal();
  }
  $.watermark.show('#ChargeAmount');
}

function CheckServiceDateOnCreateMode() {

  if ($('#LineItemServiceStartDate').val() == '') {
    $('#StartDay').val('');
    $('#StartDay').attr('disabled', 'disabled');
    $('#ServiceStartDateDropdown').attr('disabled', 'disabled');
  }
  else {


    $('#StartDay').addClass('populated');
  }

  if ($('#LineItemServiceEndDate').val() != '' && $('#LineItemServiceStartDate').val() == '') {
    $('#EndDay').attr('disabled', 'disabled');
    $('#ServiceEndDateDropdown').attr('disabled', 'disabled');
  }
}

function ClearDefaultValuesOnCreateMode(retainStartDate) {
  var divStaticFields = $('#staticFields');
  if (retainStartDate == 0) {
    $('input[type=text]:not(.populated):not(.datePicker)', divStaticFields).removeAttr('value');
  }
  else {
    $('input[type=text]:not(.populated, #StartDay):not(.datePicker)', divStaticFields).removeAttr('value');
    if (retainStartDate != -1)
      $('#StartDay').val(retainStartDate);
  }
}
function OnAddDetailClick() {
  AppendAddDetailTemplate();
}

function DefaultValuesOnCloneMode() {
  if ($('#EndDay').attr('disabled') == '') {
    $("#EndDay").val('');
    $("#EndDay").focus();
  }
}
function CalculateLineDetailNetTotal() {
  CalculateLineDetailNetTotalAmount("#TotalNetAmount", "#ChargeAmount", "#TaxAmount", "#VatAmount", "#TotalAddChargeAmount");
  $.watermark.show('#ChargeAmount');
}

function CalculateLineDetailNetTotalAmount(lineDetailNetTotalControl, lineDetailTotalControl, taxAmountControl, vatAmountControl, additionalChargeControl) {
  var lineDetailTotal = 0;
  var taxAmount = 0;
  var vatAmount = 0;
  var additionalChargeAmount = 0;

  if (!isNaN(Number($(lineDetailTotalControl).val())))
    lineDetailTotal = Number($(lineDetailTotalControl).val());

  if (!isNaN(Number($(taxAmountControl).val())))
    taxAmount = Number($(taxAmountControl).val());

  if (!isNaN(Number($(vatAmountControl).val())))
    vatAmount = Number($(vatAmountControl).val());

  if (!isNaN(Number($(additionalChargeControl).val())))
    additionalChargeAmount = Number($(additionalChargeControl).val());

  var lineDetailNetTotal = lineDetailTotal + taxAmount + vatAmount + additionalChargeAmount;
  $(lineDetailNetTotalControl).val(lineDetailNetTotal.toFixed(_amountDecimals));
}

// Show hide optional fields in group
function ShowHideOptionalField(caller) {
  var divGroup = caller.parentNode.parentNode.parentNode;
  var src = $(caller).attr('src');
  $("div.optional", divGroup).each(function (index) {
    var hideDiv = false; var hasValueTxt = false; var hasValueDdl = false; var hasValueDt = false; var hasValueDatetime = false;
    var currentDiv = $(this);
    if ($(":text:not(.datePicker):not(.dateTimePicker)", currentDiv).length > 0) {
      if ($(":text:not(.datePicker):not(.dateTimePicker)[value = '']", currentDiv).length != $(":text:not(.datePicker):not(.dateTimePicker)", currentDiv).length)
        hasValueTxt = true;
    }

    if ($("select", currentDiv).length > 0) {
      if ($("select option:selected[value = '']", currentDiv).length != $("select", currentDiv).length)
        hasValueDdl = true;
    }

    if ($(":text.datePicker", currentDiv).length > 0) {
      if ($(":text.datePicker[value = 'DD-MMM-YY']", currentDiv).length != $(":text.datePicker", currentDiv).length)
        hasValueDt = true;
    }

    if ($(":text.dateTimePicker", currentDiv).length > 0) {
      if ($(":text.dateTimePicker[value = 'DD-MMM-YYThh:mm:ss']", currentDiv).length != $(":text.dateTimePicker", currentDiv).length)
        hasValueDatetime = true;
    }

    if (hasValueTxt == false && hasValueDdl == false && hasValueDt == false && hasValueDatetime == false)
      hideDiv = true;

    if (hideDiv == true) {
      if (src.match('Next'))
        $(this).show();
      else
        $(this).hide();
    }
  });


  if (src.match('Next'))
    $(caller).attr('src', src.replace('Next', 'Down'));
  else
    $(caller).attr('src', src.replace('Down', 'Next'));
}

function AddGroup(caller) {
  try {
    
    groupDiv = $(caller.parentNode.parentNode.parentNode);
    groupHiddenFielId = $("[id*=DFGroupId]", groupDiv).attr("id");
    var groupId = $('#' + groupHiddenFielId).val();
    var lineItemChargeCodeId = $('#LineItemChargeCodeId').val();
    var lineItemChargeCodeTypeId = $('#LineItemChargeCodeTypeId').val() ? $('#LineItemChargeCodeTypeId').val() : 0;

    // Get current count of group to be added.
    if (mandatoryDynamicGroups[groupId].OccurrencesRendered == mandatoryDynamicGroups[groupId].MaxOccurrenceCount) {
      alert('No more occurrences of the group can be added.');
    }
    else {

      mandatoryDynamicGroups[groupId].CurrentIndex = mandatoryDynamicGroups[groupId].CurrentIndex + 1;
      var currentCount = mandatoryDynamicGroups[groupId].CurrentIndex;

      // Make a Ajax call to get group html from server.
      $.ajax({
        type: "Post",
        url: _expandGroupUrl,
        data: { chargeCodeId: lineItemChargeCodeId, chargeCodeTypeId: lineItemChargeCodeTypeId, groupId: groupId, groupCurrentIndex: currentCount, isOptionalGroup: false },
        dataType: "json",
        success: InsertGroupHtml
      });
    }
  }
  catch (e) {
  }
}

var optionalGroupId;
function AddOptionalGroup() {
  try {
    optionalGroupId = $('#OptionalGroupDropdownList').val();
    if (optionalGroupId != '') {
      $('#AddOptionaGroup').attr('disabled', true);
      var lineItemChargeCodeId = $('#LineItemChargeCodeId').val();
      var lineItemChargeCodeTypeId = $('#LineItemChargeCodeTypeId').val() ? $('#LineItemChargeCodeTypeId').val() : 0;


      // Get current count of group to be added.
      mandatoryDynamicGroups[optionalGroupId].CurrentIndex = mandatoryDynamicGroups[optionalGroupId].CurrentIndex + 1;
      var currentCount = mandatoryDynamicGroups[optionalGroupId].CurrentIndex;

      // Make a Ajax call to get group html from server.
      $.ajax({
        type: "Post",
        url: _expandGroupUrl,
        data: { chargeCodeId: lineItemChargeCodeId, chargeCodeTypeId: lineItemChargeCodeTypeId, groupId: optionalGroupId, groupCurrentIndex: currentCount, isOptionalGroup: true },
        dataType: "json",
        success: InsertOptionalGroupHtml
      });

    }
    else
      alert('Please select optional group.');
  }
  catch (e) {
      $('#AddOptionaGroup').attr('disabled', false);
  }
}

function RemoveGroup(caller) {
//var UserChoice = confirm("Do you want to delete this?");
//  if (UserChoice == true) {
  groupDiv = $(caller.parentNode.parentNode.parentNode).get(0);
  var groupDivId = groupDiv.id;
  groupHiddenFielId = $("[id*=DFGroupId]", groupDiv).attr("id");
  var groupId = $('#' + groupHiddenFielId).val();

  $(groupDiv).remove();

  UpdateMandatoryDynamicGroupCount(groupId, groupDivId);
//}
}

//Remove optional group 
function RemoveOptionalGroup(caller) {
//  var UserChoice = confirm("Do you want to delete this?");
//  if (UserChoice == true) {
  groupDiv = $(caller.parentNode.parentNode.parentNode).get(0);
  var groupDivId = groupDiv.id;
  groupHiddenFielId = $("[id*=DFGroupId]", groupDiv).attr("id");
  var optionalGroupId = $('#' + groupHiddenFielId).val();

  $(groupDiv).remove();

  UpdateMandatoryDynamicGroupCount(optionalGroupId, groupDivId);

  //If group has max numeber of instances rendered on UI, then on click of remove group button, add option of group to dropdown and remove
  //option from hidden dropdown
  if (mandatoryDynamicGroups[optionalGroupId].OccurrencesRendered < mandatoryDynamicGroups[optionalGroupId].MaxOccurrenceCount) {
    $('#OptionalGroupDropdownList').append($("#HiddenOptionalGroupDropdownList option[value='" + optionalGroupId + "']"));
    $("#HiddenOptionalGroupDropdownList option[value='" + optionalGroupId + "']").remove();
    $('#OptionalGroupDropdownList').val('');
//  }
  }
}

function InsertGroupHtml(response) {
  if (response.Html.length > 0) {
    var groupId = $('#' + groupHiddenFielId).val();
    var divId = mandatoryDynamicGroups[groupId].UIDivId[mandatoryDynamicGroups[groupId].UIDivId.length - 1];
    // Insert response html after group div.
    var grpHtml = response.Html;
    $(grpHtml).insertAfter('#' + divId);

    if (response.FunctionName != '') {
      var functionName = response.FunctionName;
      window[functionName]();
    }
    // Update groupId occurrence in group array.
    AddMandatoryDynamicGroupInfo(groupId, 0, response.GroupHtmlDivId, true);
    formatDateControls();
    highlightMandatory();
    HideOptionalFieldsOnAjaxCall(response.GroupHtmlDivId);
  }

}

//Add html for optional group
function InsertOptionalGroupHtml(response) {
  if (response.Html.length > 0) {
    var isFirstInstanceofGroup;
    if (mandatoryDynamicGroups[optionalGroupId].UIDivId.length == 0)
      isFirstInstanceofGroup = true;
    // Insert response html after group div.
    var grpHtml = response.Html;
    if (isFirstInstanceofGroup) {
      $('#divOptionalGroup').append(grpHtml);
    }
    else {
      var divId = mandatoryDynamicGroups[optionalGroupId].UIDivId[mandatoryDynamicGroups[optionalGroupId].UIDivId.length - 1];
      $(grpHtml).insertAfter('#' + divId);
    }
    if (response.FunctionName != '') {
      var functionName = response.FunctionName;
      window[functionName]();
    }
    // Update groupId occurrence in group array.
    AddMandatoryDynamicGroupInfo(optionalGroupId, 0, response.GroupHtmlDivId, true);

    //If group has max numeber of instances rendered on UI, then remove option of group from dropdown and add
    //this option to hidden dropdown
    if (mandatoryDynamicGroups[optionalGroupId].OccurrencesRendered == mandatoryDynamicGroups[optionalGroupId].MaxOccurrenceCount) {
      $('#HiddenOptionalGroupDropdownList').append($("#OptionalGroupDropdownList option[value='" + optionalGroupId + "']"));
      $("#OptionalGroupDropdownList option[value='" + optionalGroupId + "']").remove();
    }
    formatDateControls();
    highlightMandatory();
}
  //TFS#9951 :Firefox: v47 :Miscellaneous:Create Line Item Detail:Incorrect behaviour of Buttons
  $('#AddOptionaGroup').attr('disabled', false);
}

function HideOptionalFieldsOnAjaxCall(divAddedId) {
  var grpDiv = $('#' + divAddedId);
  $("#" + divAddedId + " .expandLinkImage").each(function (i) {
    var currentDiv = $(this).get(0);
    ShowHideOptionalField(currentDiv);
  });

}

function HideOptionalFieldsOnPageRender() {

  $(".expandLinkImage").each(function (i) {
    var currentDiv = $(this).get(0);
    ShowHideOptionalField(currentDiv);
  });

}

var mandatoryDynamicGroups = new Array();

//Initialize array with group information
function AddMandatoryDynamicGroupInfo(grpId, maxOccurrence, grpDivId, hasCurrentIndexIncremented) {

  if (!mandatoryDynamicGroups.hasOwnProperty(grpId)) {
    var grp = [];
    grp.GroupId = grpId;
    grp.MaxOccurrenceCount = maxOccurrence;
    grp.OccurrencesRendered = 1;
    grp.CurrentIndex = 1;
    grp.UIDivId = [];
    if (grpDivId != '')
      grp.UIDivId.push(grpDivId);
    mandatoryDynamicGroups[grpId] = grp;
  }
  else {
    mandatoryDynamicGroups[grpId].OccurrencesRendered = mandatoryDynamicGroups[grpId].OccurrencesRendered + 1;
    if (hasCurrentIndexIncremented == null || hasCurrentIndexIncremented != true)
      mandatoryDynamicGroups[grpId].CurrentIndex = mandatoryDynamicGroups[grpId].CurrentIndex + 1;
    mandatoryDynamicGroups[grpId].UIDivId.push(grpDivId);
  }
}

function AddOptionalDynamicGroupInfo(grpId, maxOccurrence, grpDivId) {

  if (!mandatoryDynamicGroups.hasOwnProperty(grpId)) {
    var grp = [];
    grp.GroupId = grpId;
    grp.MaxOccurrenceCount = maxOccurrence;
    grp.OccurrencesRendered = 0;
    grp.CurrentIndex = 0;
    grp.UIDivId = [];
    if (grpDivId != '')
      grp.UIDivId.push(grpDivId);
    mandatoryDynamicGroups[grpId] = grp;
  }
  else {
    mandatoryDynamicGroups[grpId].OccurrencesRendered = mandatoryDynamicGroups[grpId].OccurrencesRendered + 1;
    mandatoryDynamicGroups[grpId].CurrentIndex = mandatoryDynamicGroups[grpId].CurrentIndex + 1;
    mandatoryDynamicGroups[grpId].UIDivId.push(grpDivId);
  }
}

//Initialise group details in array for optional group
function InitializeOptionalDynamicGroupInfo(grpId, maxOccurrence) {

  if (!mandatoryDynamicGroups.hasOwnProperty(grpId)) {
    var grp = [];
    grp.GroupId = grpId;
    grp.MaxOccurrenceCount = maxOccurrence;
    grp.OccurrencesRendered = 0;
    grp.CurrentIndex = 0;
    grp.UIDivId = [];
    mandatoryDynamicGroups[grpId] = grp;
  }
  //In edit mode, if UI rendered instances of group are equal to max occurrence of group, then remove it from dropdown
  if (mandatoryDynamicGroups[grpId].OccurrencesRendered == mandatoryDynamicGroups[grpId].MaxOccurrenceCount) {
    $('#HiddenOptionalGroupDropdownList').append($("#OptionalGroupDropdownList option[value='" + grpId + "']"));
    $("#OptionalGroupDropdownList option[value='" + grpId + "']").remove();
  }
}

function UpdateMandatoryDynamicGroupCount(grpId, groupDivId) {

  if (mandatoryDynamicGroups.hasOwnProperty(grpId)) {
    mandatoryDynamicGroups[grpId].OccurrencesRendered = mandatoryDynamicGroups[grpId].OccurrencesRendered - 1;

    var i; for (i = 0; i < mandatoryDynamicGroups[grpId].UIDivId.length; i++) {
      if (mandatoryDynamicGroups[grpId].UIDivId[i] == groupDivId) {
        mandatoryDynamicGroups[grpId].UIDivId.splice(i, 1);
        break;
      }
    }
  }
}

function ValidateProductId() {
  var isValid = CheckForParentValue();
  if (isValid == false) {
    if (!confirm("Product Id in LineItem is different. Do you want to continue?")) {
      $("#ProductId").focus();
      return false;
    }
    else {
      return true;
    }

  }
  return true;
}


function PopulateSubdivisionCodes(caller) {
  var parentGroupDiv = $(caller.parentNode.parentNode).get(0);
  var parentGroupDivId = parentGroupDiv.id;
  var $subdivisionCode = $("#" + parentGroupDivId + " .SubdivisionCode");

  var countryCodeValue = $(caller).val();
  if (countryCodeValue != '') {
    $.ajax({
      type: "GET",
      url: _getSubdivisionCodesMethod,
      data: { countryCode: countryCodeValue },
      success: function (result) {
        if (result != null) {
          // populate subdivision code dropdown.
          $subdivisionCode.empty().append('<option selected="selected" value="">Please Select</option>');
          for (i = 0; i < result.length; i++) {
            $subdivisionCode.append($("<option></option>").val(result[i]).html(result[i]));
          }
        }
      }
    });
  }
  else {
    $subdivisionCode.empty().append('<option selected="selected" value="">Please Select</option>');
  }
}

$(document).ready(function () {
  $(".CountryCode").live('change', function () {
    PopulateSubdivisionCodes(this);
  });
})