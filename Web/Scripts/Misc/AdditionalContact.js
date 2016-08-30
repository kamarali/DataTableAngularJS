var $contactCurrent = 1;
var maxAllowedContactGridRows = 10;
var $validateContact;
var $validateBilledContact;
var contactFields = InitializeContactFields();
var $billingContactList = $('#contactGrid');
var $billedContactList = $('#billedContactGrid');
var BillingContactType = 'BillingContactType';
var BillingContactDescription = 'BillingContactDescription';
var BillingContactValue = 'BillingContactValue';
var BillingContactId = 'BillingContactId';
var BilledContactType = 'BilledContactType';
var BilledContactDescription = 'BilledContactDescription';
var BilledContactValue = 'BilledContactValue';
var BilledContactId = 'BilledContactId';
var MemberType = 'MemberTypeId';
var additionalContactGridHeight = 150;
var $billingContactCurrent = 1;
var $billedContactCurrent = 1;
var warningMessageText = 'Cannot add more than ' + maxAllowedContactGridRows + ' contact details.';

function InitializeAdditionalContactGrid(contactData, gridId, isBillingMember) {

  if (isBillingMember == true) {
    if ($isOnView) { $("#formContactDetails").hide();additionalContactGridHeight = 350;}
    validateBillingMemberForm();
  }
  else {
    if ($isOnView) { $("#formBilledContactDetails").hide(); }
    validateBilledMemberForm();
  }

  // Initialize contactDetails grid.  
  $(gridId).jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 610,
    height: additionalContactGridHeight,
    colNames: [contactFields.Id.DisplayName, contactFields.Type.DisplayName, contactFields.Value.DisplayName,contactFields.Description.DisplayName, contactFields.IsBillingMember.DisplayName],
    colModel: [
                { name: contactFields.Id.Id, index: contactFields.Id.Id, sorttype: 'int', sortable: false, formatter: buttonFmatterAddlContact, width: 20, hidden: $isOnView },
                { name: contactFields.Type.Id, index: contactFields.Type.Id, sortable: false, width: 50 },
                { name: contactFields.Value.Id, index: contactFields.Value.Id, sortable: false, width: 110 },
                { name: contactFields.Description.Id, index: contactFields.Description.Id, sortable: false, width: 130 },
                { name: contactFields.IsBillingMember.Id, index: contactFields.IsBillingMember.Id, sortable: false, hidden:true }
              ]
  });

  contactData = eval(contactData);
  // Populate data in contact grid with existing contact records.
  if (contactData != null) {
    $contactCurrent = 1;
    for ($contactCurrent; $contactCurrent < contactData.length + 1; $contactCurrent++) {
      //MemberType : 1- Billed member, 2- Billing member.
      if (contactData[$contactCurrent - 1][MemberType] == 2) {
        row = { Id: $contactCurrent, Type: contactData[$contactCurrent - 1][contactFields.Type.Id], Value: contactData[$contactCurrent - 1][contactFields.Value.Id], Description: contactData[$contactCurrent - 1][contactFields.Description.Id], IsBillingMember: true };
        $billingContactList.jqGrid('addRowData', $contactCurrent, row);
        AddBillingMemberContactFields($contactCurrent, contactData[$contactCurrent - 1][contactFields.Id.Id], row[contactFields.Type.Id], row[contactFields.Value.Id], row[contactFields.Description.Id]);
        $billingContactCurrent++;
      }
      else if (contactData[$contactCurrent - 1][MemberType] == 1) {
        row = { Id: $contactCurrent, Type: contactData[$contactCurrent - 1][contactFields.Type.Id], Value: contactData[$contactCurrent - 1][contactFields.Value.Id], Description: contactData[$contactCurrent - 1][contactFields.Description.Id], IsBillingMember: false };
        $billedContactList.jqGrid('addRowData', $contactCurrent, row);
        // Add billed member contact fields.
        AddBilledMemberContactFields($contactCurrent, contactData[$contactCurrent - 1][contactFields.Id.Id], row[contactFields.Type.Id], row[contactFields.Value.Id], row[contactFields.Description.Id]);
        $billedContactCurrent++;
      }
    }
    // disable the input fields if  max length is passed.
    var rowIds = $billingContactList.getDataIDs();
    if (rowIds.length >= maxAllowedContactGridRows) {
      disableBillingInputFields();
    }

    rowIds = $billedContactList.getDataIDs();
    if (rowIds.length >= maxAllowedContactGridRows) {
      disableBilledInputFields();
    }
  }
}

function validateBillingMemberForm() {
  $validateContact = $("#formContactDetails").validate({
    rules: {
      BillingContactType: { required: true },
      BillingContactValue: { required: true, maxlength: 255 },
      BillingContactDescription: {
        maxlength: 80
      }

    },
    messages: {
      BillingContactType: { required: "Contact Type Required" },
      BillingContactValue: { required: "Contact Value Required" }
    },
    submitHandler: function () {
      AddBillingContact();
    }, highlight: false
  });
}

function validateBilledMemberForm() {

  $validateBilledContact = $("#formBilledContactDetails").validate({
    rules: {
      BilledContactType: "required",
      BilledContactValue: {required:true},
      BilledContactDescription: {
        maxlength: 80
      }
    },
    messages: {
      BilledContactType: "Contact Type Required",
      BilledContactValue: {required: "Contact Value Required"}
    },
    submitHandler: function () {
      AddBilledContact();
    }, highlight: false
  });  
}

//Create object for Tax field constant
function CreateField(id, displayName) {
  obj = [];
  obj.Id = id;
  obj.DisplayName = displayName;
  return obj;
}

// Initializes taxField constant which contains values for Controls and display name for grid title
function InitializeContactFields() {
  var fields = new Array();
  fields.Id = CreateField('Id', 'Action');
  fields.Type = CreateField('Type', 'Contact Type');
  fields.Value = CreateField('Value', 'Contact Value');
  fields.Description = CreateField('Description', 'Description');
  fields.IsBillingMember = CreateField('IsBillingMember', 'Is Billing Member'); // Will be hidden.  
  return fields;
}

//Custom formatter to display delete button in grid
function buttonFmatterAddlContact(cellValue, options, cellObject) {
  var isBillingMemberFlag = cellObject[contactFields.IsBillingMember.Id];  
  return "<a style='cursor:hand;' target='_parent' href=javascript:deleteAdditionalContactRecord(" + cellValue + ","+ isBillingMemberFlag+");><div class='deleteIcon ignoredirty'></div></a>";
}

//Custom formatter to display amount rounded to 2 decimal places in grid
function amountFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_amountDecimals);
}

//Custom formatter to display amount rounded to 3 decimal places in grid
function percentFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_percentDecimals);
}

//Method to delete contactDetails record
function deleteAdditionalContactRecord(id, isBillingMember) {
  
  if (confirm("Are you sure you want to delete this record?")) {
    
    //delete record entries from hidden fields
    if (isBillingMember == true) {
      $contactList = $billingContactList;
      RemoveBillingContactHiddenFields(id);
    }
    else {
      $contactList = $billedContactList;
      RemoveBilledContactHiddenFields(id);
    }
    
    $contactList.delRowData(id);
    $parentForm.setDirty();

    var rowIds = $contactList.getDataIDs();
    for (cnt = 0; cnt <= rowIds.length; cnt++) {
      try {
        $contactList.jqGrid('setCell', rowIds[cnt], 'SerialNumber', cnt + 1);
        id++;
      } catch (e) { }
    }

    if (rowIds.length < maxAllowedContactGridRows) {
      if (isBillingMember == true) {
        enableBillingInputFields();
      }
      else {
        enableBilledInputFields();
      }
    }
  }
}


function RemoveBillingContactHiddenFields(id) {
  var description = '#' + BillingContactDescription + id;
  $(description).remove();

  var type = '#' + BillingContactType + id;
  $(type).remove();

  var value = '#' + BillingContactValue + id;
  $(value).remove();

  var contactId = '#' + BillingContactId + id;
  $(contactId).remove();
}

function RemoveBilledContactHiddenFields(id) {
  var description = '#' + BilledContactDescription + id;
  $(description).remove();

  var type = '#' + BilledContactType + id;
  $(type).remove();

  var value = '#' + BilledContactValue + id;
  $(value).remove();

  var contactId = '#' + BilledContactId + id;
  $(contactId).remove();
}

function AddBillingContact() {

  var rowIds = $billingContactList.getDataIDs();
  if (rowIds.length >= maxAllowedContactGridRows) {
    alert(warningMessageText);
    return;
  }
 
  var type = $('#' + BillingContactType).val();
  var valueField = $('#' + BillingContactValue).val();
  var descriptionField = $('#' + BillingContactDescription).val();

  var typeClone = $('#' + BillingContactId).clone();
  typeClone.attr("name", BillingContactType + $billingContactCurrent);
  typeClone.attr("id", BillingContactType + $billingContactCurrent);
  typeClone.val(type);
  typeClone.appendTo("#childContactList");

  var contactDetails = $("#AdditionalContactDetails").clone(true);
  contactDetails.children("div").children("div").children("input").each(function (i) {
    var $currentElem = $(this);

    $currentElem.attr("name", $currentElem.attr("name") + $billingContactCurrent);
    $currentElem.attr("id", $currentElem.attr("id") + $billingContactCurrent);
    $currentElem.appendTo("#childContactList");
  });
  
  var $textAreaClone = $('#' + BillingContactId).clone(true);
  $textAreaClone.attr("name", BillingContactDescription + $billingContactCurrent);
  $textAreaClone.attr("id", BillingContactDescription + $billingContactCurrent);
  $textAreaClone.val(descriptionField);
  $textAreaClone.appendTo("#childContactList");
  
  //Clear values of textboxes in capture div
  clearForm();
  
  //Check if max Occurrence i.e. 10 is reached
  var rowIds = $billingContactList.getDataIDs();
  if (rowIds.length >= maxAllowedContactGridRows) {
    alert(warningMessageText);
    return;
  }
  
  //Add record in grid with IsBillingMember as true
  var row = { Id: $billingContactCurrent, Description: descriptionField, Value: valueField, Type: type, SerialNumber: $billingContactCurrent, IsBillingMember: true };
  $billingContactList.addRowData($billingContactCurrent, row);

  $billingContactCurrent++;

  var rowIds = $billingContactList.getDataIDs();
  for (cnt = 0; cnt <= rowIds.length; cnt++) {
    try {
      $billingContactList.jqGrid('setCell', rowIds[cnt], 'SerialNumber', cnt + 1);
      id++;
    } catch (e) { }
  }
  rowIds = $billingContactList.getDataIDs();  
  if (rowIds.length >= maxAllowedContactGridRows) {
    disableBillingInputFields();
  }
  
  // Validate is reset to fix issue: Press "Add" without entering any mandatory information. The system highlights the mandatory field with a red 'X' mark. 
  //After this whenever the contact type drop down is clicked, the red 'X' mark appears besides the drop down. 
  $validateContact.resetForm();

  $parentForm.setDirty();
}


function AddBilledContact() {

  var rowIds = $billedContactList.getDataIDs();
  if (rowIds.length >= maxAllowedContactGridRows) {
    alert(warningMessageText);
    return;
  }
  
  var type = $('#' + BilledContactType).val();
  var valueField = $('#' + BilledContactValue).val();
  var descriptionField = $('#' + BilledContactDescription).val();

  var typeClone = $('#' + BilledContactId).clone();
  typeClone.attr("name", BilledContactType + $billedContactCurrent);
  typeClone.attr("id", BilledContactType + $billedContactCurrent);
  typeClone.val(type);
  typeClone.appendTo("#childContactList");

  var contactDetails = $("#BilledContactDetails").clone(true);
  contactDetails.children("div").children("div").children("input").each(function (i) {
    var $currentElem = $(this);
    $currentElem.attr("name", $currentElem.attr("name") + $billedContactCurrent);
    $currentElem.attr("id", $currentElem.attr("id") + $billedContactCurrent);
    $currentElem.appendTo("#childContactList");
  });

  var $textAreaClone = $('#' + BilledContactId).clone(true);
  $textAreaClone.attr("name", BilledContactDescription + $billedContactCurrent);
  $textAreaClone.attr("id", BilledContactDescription + $billedContactCurrent);
  $textAreaClone.val(descriptionField);
  $textAreaClone.appendTo("#childContactList");

  // Clear values of textboxes in capture div.
  $('#' + BilledContactDescription).val("");
  $('#' + BilledContactType).val("");
  $('#' + BilledContactValue).val("");

  //Check if max Occurrence i.e. 10 is reached
  var rowIds = $billedContactList.getDataIDs();
  if (rowIds.length > maxAllowedContactGridRows) {
    alert(warningMessageText);
    return;
  }

  // Add record in grid with IsBillingMember as false.
  var row = { Id: $billedContactCurrent, Description: descriptionField, Value: valueField, Type: type, SerialNumber: $billedContactCurrent, IsBillingMember: false };
  $billedContactList.addRowData($billedContactCurrent, row);
  $billedContactCurrent++;

  for (cnt = 0; cnt <= rowIds.length; cnt++) {
    try {
      $billedContactList.jqGrid('setCell', rowIds[cnt], 'SerialNumber', cnt + 1);
      id++;
    } catch (e) { }
  }
  rowIds = $billedContactList.getDataIDs();  
  if (rowIds.length >= maxAllowedContactGridRows) {
    disableBilledInputFields();
  }
  // Validate is reset to fix issue: Press "Add" without entering any mandatory information. The system highlights the mandatory field with a red 'X' mark. 
  //After this whenever the contact type drop down is clicked, the red 'X' mark appears besides the drop down. 
  $validateBilledContact.resetForm();

  $parentForm.setDirty();
}

// Add hidden input fields for existing contactDetails records. Is called only for existing data.
function AddBillingMemberContactFields($contactCurrent, id, type, value, description) {
  var contactDetails = $("#AdditionalContactDetails").clone(true);
  contactDetails.find('#' + BillingContactValue).val(value);
  contactDetails.find('#' + BillingContactId).val(id);  
  contactDetails.find('#' + BillingContactDescription).val(description);

  var typeClone = $('#' + BillingContactId).clone();
  typeClone.attr("name", BillingContactType + $contactCurrent);
  typeClone.attr("id", BillingContactType + $contactCurrent);
  typeClone.val(type);
  typeClone.appendTo("#childContactList");

  contactDetails.children("div").children("div").children("input,textarea").each(function (i) {
    var $currentElem = $(this);
    $currentElem.attr("name", $currentElem.attr("name") + $contactCurrent);
    $currentElem.attr("id", $currentElem.attr("id") + $contactCurrent);
    $currentElem.appendTo("#childContactList");
  });
}

// Add hidden input fields for existing contactDetails records. Is called only for existing data.
function AddBilledMemberContactFields($contactCurrent, id, type, value, description) {
  var contactDetails = $("#BilledContactDetails").clone(true);
  contactDetails.find('#' + BilledContactValue).val(value);
  contactDetails.find('#' + BilledContactId).val(id);  
  contactDetails.find('#' + BilledContactDescription).val(description);

  var typeClone = $('#' + BilledContactId).clone();
  typeClone.attr("name", BilledContactType + $contactCurrent);
  typeClone.attr("id", BilledContactType + $contactCurrent);
  typeClone.val(type);
  typeClone.appendTo("#childContactList");

  contactDetails.children("div").children("div").children("input,textarea").each(function (i) {
    var $currentElem = $(this);
    $currentElem.attr("name", $currentElem.attr("name") + $contactCurrent);
    $currentElem.attr("id", $currentElem.attr("id") + $contactCurrent);
    $currentElem.appendTo("#childContactList");
  });
}

// Close contactDetails details modal dialogue
function closeContactDetail() {
  $validateContact.resetForm();
  $('#BillingAdditionalDetails').dialog('close');
  // Set focus on BilledMember contact TextBox
  $("#BillingMemberContactName").focus();
}

// Following code is executed when user closes Billing Member Contact Popup
$("#BillingAdditionalDetails").bind("dialogclose", function (event, ui) {
  // Set focus on BillingMember contact TextBox
  $("#BillingMemberContactName").focus();
});

function closeBilledContactDetail() {
  $validateBilledContact.resetForm();
  $('#BilledAdditionalDetails').dialog('close');    
  // Set focus on BilledMember contact TextBox
  $("#BilledMemberContactName").focus();
}

// Following code is executed when user closes Billed Member Contact Popup
$("#BilledAdditionalDetails").bind("dialogclose", function (event, ui) {
  // Set focus on BilledMember contact TextBox
  $("#BilledMemberContactName").focus();
});

function clearForm() {
  $('#' + BillingContactDescription).val("");
  $('#' + BillingContactType).val("");
  $('#' + BillingContactValue).val("");
}


function disableBillingInputFields() {
  $('#' + BillingContactDescription).attr('readonly', 'readonly');
  $('#' + BillingContactType).attr('readonly', 'readonly');
  $('#' + BillingContactType).attr('disabled', true);
  $('#' + BillingContactValue).attr('readonly', 'readonly');    
  $('#AddContactButton').attr('disabled', true);
}

function disableBilledInputFields() {
  $('#' + BilledContactDescription).attr('readonly', 'readonly');
  $('#' + BilledContactType).attr('readonly', 'readonly');
  $('#' + BilledContactType).attr('disabled', true);
  $('#' + BilledContactValue).attr('readonly', 'readonly');
  $('#AddBilledContactButton').attr('disabled', true);
}

function enableBillingInputFields() {
  $('#' + BillingContactDescription).removeAttr('readonly');
  $('#' + BillingContactType).removeAttr('readonly');
  $('#' + BillingContactType).removeAttr('disabled');
  $('#' + BillingContactValue).removeAttr('readonly');
  $('#AddContactButton').removeAttr('disabled');
}

function enableBilledInputFields() {
  $('#' + BilledContactDescription).removeAttr('readonly');
  $('#' + BilledContactType).removeAttr('readonly');
  $('#' + BilledContactType).removeAttr('disabled');
  $('#' + BilledContactValue).removeAttr('readonly');
  $('#AddBilledContactButton').removeAttr('disabled');
}