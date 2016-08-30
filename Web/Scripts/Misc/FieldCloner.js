// This js can be used to clone div section on "+" click.
// Note: This can not be used in case where multiple div's are to be cloned. 

var clickCount = 0; var NewDivIdsArray = [];

var _divToDuplicate;
var _childDivIdPrefix;
var _firstControlIdPrefix;
var _secondControlIdPrefix;
var _thirdControlIdPrefix;
var _divToInsertAfter;
var _maxCount;
var _thirdControlIdLength;
// This is id of the control on click event of which div will get duplicated.
var _buttonId;
var _maxlength;

var _url = '/Data/GetAdditionalDetails?q=&extraparam1=1';
var _detailType = 1;
var _detailLevel = 1;

var Name = 'Name';
var Description = 'Description';

function InitializeParameters(divToDuplicate, childDivIdPrefix, firstControlIdPrefix, secondControlIdPrefix, thirdControlIdPrefix, divToInsertAfter, buttonId, maxCount, maxlength, data, url, detailType, detailLevel) {
  
  _divToDuplicate = divToDuplicate;
  _childDivIdPrefix = childDivIdPrefix;
  _firstControlIdPrefix = firstControlIdPrefix;
  _secondControlIdPrefix = secondControlIdPrefix;
  _thirdControlIdPrefix = thirdControlIdPrefix;
  _divToInsertAfter = divToInsertAfter;
  _maxCount = maxCount;
  _thirdControlIdLength = thirdControlIdPrefix.length;
  _buttonId = buttonId;
  _maxlength = maxlength;
  _url = url;
  _detailType = detailType;
  _detailLevel = detailLevel;

  registerAutocomplete(firstControlIdPrefix, null, url, 0, false, null, detailType, detailLevel, null);

  // bind existing data.
  data = eval(data);
  if (data != null && data[0] != null) {
    if(data[0][Name] != null)
      $('#' + firstControlIdPrefix).val(data[0][Name]);
    if (data[0][Description] != null)
      $('#' + secondControlIdPrefix).val(data[0][Description]);
    rowCounter = 1;
    for (rowCounter; rowCounter < data.length; rowCounter++) {
      // assign values to controls.
      if (data[rowCounter][Name] != null || data[rowCounter][Description] != null)
        AddFields();

      if (data[rowCounter][Name] != null)
        $('#' + firstControlIdPrefix + clickCount).val(data[rowCounter][Name]);

      if (data[rowCounter][Description] != null)
        $('#' + secondControlIdPrefix + clickCount).val(data[rowCounter][Description]);
    }
  }
  
  $.validator.addMethod('additionalDetail', isValidAdditionalDetails, "Additional Details Required.");
  $.validator.addClassRules("additionalDetailRequired", {
    additionalDetail: true
  });

  $.validator.addMethod('addDetailDescription', isValidAddDetailsDesc, "Additional Details Description Required.");
  $.validator.addClassRules("addDetailDescRequired", {
    addDetailDescription: true
  });
}

function AddFields() {

  var divTemplate = $(_divToDuplicate).clone(true);
  
  divTemplate[0].children[0].id = _childDivIdPrefix + ++clickCount;
  divTemplate[0].children[0].children[0].children[1].id = _firstControlIdPrefix + clickCount;
  divTemplate[0].children[0].children[1].children[1].id = _secondControlIdPrefix + clickCount;
  divTemplate[0].children[0].children[1].children[2].id = _thirdControlIdPrefix + clickCount;

  NewDivIdsArray.push(clickCount);

  if (NewDivIdsArray.length == 1)
    $($(divTemplate).html()).insertAfter(_divToInsertAfter);

  else {
    var id = NewDivIdsArray[NewDivIdsArray.length - 2];
    $($(divTemplate).html()).insertAfter('#' + _childDivIdPrefix + id);
  }

  registerAutocomplete(_firstControlIdPrefix + clickCount, null, _url, 0, false, null, _detailType, _detailLevel, null);

  $('#' + divTemplate[0].children[0].id).attr('name', _childDivIdPrefix + clickCount);
  $('#' + divTemplate[0].children[0].children[0].children[1].id).attr('name', _firstControlIdPrefix + clickCount);
  //SCP140271 - incorrect data in eInvoice are validated by SI
  $('#' + divTemplate[0].children[0].children[0].children[1].id).attr('maxLength', 30);
  $('#' + divTemplate[0].children[0].children[1].children[1].id).attr('name', _secondControlIdPrefix + clickCount);
  $('#' + divTemplate[0].children[0].children[1].children[2].id).attr('name', _thirdControlIdPrefix + clickCount);

  $('#' + divTemplate[0].children[0].children[1].children[1].id).bind("keypress", function () { maxLength(this, _maxlength) });
  $('#' + divTemplate[0].children[0].children[1].children[1].id).bind("paste", function () { maxLengthPaste(this, _maxlength) });
  
  if (NewDivIdsArray.length == (_maxCount - 1))
    $(_buttonId).hide();

  $('#' + _thirdControlIdPrefix + clickCount).bind("click", RemoveFields);
}

function RemoveFields() {
  var id = this.id.substring(_thirdControlIdLength);
  var i;
  for (i = 0; i < NewDivIdsArray.length; i++) {
    if (NewDivIdsArray[i] == id) {
      NewDivIdsArray.splice(i, 1);
      break;
    }
  }
  $(this.parentNode.parentNode).remove();
  $(_buttonId).show();
}

function RemoveDynamicFields(control) {
  var id = control.id.substring(_thirdControlIdLength);
  var i;
  for (i = 0; i < NewDivIdsArray.length; i++) {
    if (NewDivIdsArray[i] == id) {
      NewDivIdsArray.splice(i, 1);
      break;
    }
  }
  $(control.parentNode.parentNode.parentNode).remove();
  $(_buttonId).show();
}

function BindEventsForFieldClone() {
  $("#AddDetail").bind("click", AddFields);
}

function isValidAdditionalDetails(value, element) {
  
  var elementName = $(element).attr("id");
  var elementNameLength = elementName.length;

  // AdditionalDetailDropdown contains 24 characters.
  var detailNumber = elementName.substr(24, elementNameLength - 24);

  var additionalDetailDescIdConst = "AdditionalDetailDescription";
  var additionalDetailDescId = additionalDetailDescIdConst + detailNumber;
  // If description is empty..
  var $additionalDetailDescId = $('#' + additionalDetailDescId);
  if ($additionalDetailDescId.val() == undefined || $additionalDetailDescId.val() == '') {
    return true;
  }
  else if(value == ''){
    return false;
  }
    
  return true;
}

function isValidAddDetailsDesc(value, element) {  
  var elementName = $(element).attr("id");
  var elementNameLength = elementName.length;

  // AdditionalDetailDescription contains 27 characters.
  var detailNumber = elementName.substr(27, elementNameLength - 27);

  var additionalDetailIdConst = "AdditionalDetailDropdown";
  var additionalDetailId = additionalDetailIdConst + detailNumber;
  // If description is empty..
  var $additionalDetailId = $('#' + additionalDetailId);
  if ($additionalDetailId.val() == undefined || $additionalDetailId.val() == '') {
    return true;
  }
  else if (value == '') {
    return false;
  }

  return true;
}