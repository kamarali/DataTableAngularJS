var $VatFields = InitializeVatFields();
var $VatCurrent = 1;
var $VatList;
var $validateVat;
var totalVatAmount = 0;
var vatGridHeight = 150;
var vatSubTypeDefault = 'VAT';
//id of fields in form.
var $VatDescription = 'VATDescription';
var $VATCalculatedAmount = 'VATCalculatedAmount';
var $VATId = 'VATId';
var $VATBaseAmount = 'VATBaseAmount';
var $VATPercent = 'VATPercent';
var $VATSubType = 'VATSubType';
var $VATCategoryCode = 'VATCategoryCode';
var $TotalVatAmount = '#VatAmount';
_percentDecimals = 3;

// Initializes vatField constant which contains values for Controls and display name for grid title
function InitializeVatFields() {
  var field = {};
  field.VATId = CreateVatField('Id', 'Action');
  field.VATSubType = CreateVatField('SubType', 'VAT SubType');
  field.VATBaseAmount = CreateVatField('Amount', 'VAT Base Amount');
  field.VATPercent = CreateVatField('Percentage', 'VAT Percent');
  field.VATCalculatedAmount = CreateVatField('CalculatedAmount', 'VAT Calculated Amount');
  field.VATCategory = CreateVatField('CategoryCode', 'VAT Category');
  field.VATText = CreateVatField('Description', 'Description');
  field.VATSerialNo = CreateVatField('SerialNumber', 'Sr. No.');
  return field;
}

function InitializeVatGrid(vatData) {
  if ($isOnView) { $("#formVatDetails").hide();vatGridHeight = 350; }
  totalVatAmount = $($TotalVatAmount).val(); //initialize totalVatAmount

  $validateVat = $("#formVatDetails").validate({
    rules: {
      VATDescription: {
        maxlength: 50
      },
      VATPercent: { min: -99.999, max: 99.999 },
      VATBaseAmount: "required"      
    },
    messages: {
      VATBaseAmount: { required: "VAT Base Amount required." },
      VATPercent: "Value should be between -99.999 to 99.999"
    }, submitHandler: function () {
      addVat();
    }, highlight: false
  });

  //Initialize vat grid
  $VatList = $('#vatGrid');
  $VatList.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 800,
    height: vatGridHeight,
    colNames: [$VatFields.VATId.DisplayName, $VatFields.VATSerialNo.DisplayName, $VatFields.VATText.DisplayName, $VatFields.VATSubType.DisplayName, $VatFields.VATBaseAmount.DisplayName, $VatFields.VATPercent.DisplayName, $VatFields.VATCalculatedAmount.DisplayName, $VatFields.VATCategory.DisplayName],
    colModel: [
                { name: $VatFields.VATId.Id, index: $VatFields.VATId.Id, sortable: false, formatter: buttonVatDeleteFormatter, width: 65, hidden: $isOnView },
                { name: $VatFields.VATSerialNo.Id, index: $VatFields.VATSerialNo.Id, sortable: false, align: 'right' },
                { name: $VatFields.VATText.Id, index: $VatFields.VATText.Id, sortable: false, width: 300 },
                { name: $VatFields.VATSubType.Id, index: $VatFields.VATSubType.Id, sortable: false },
                { name: $VatFields.VATBaseAmount.Id, index: $VatFields.VATBaseAmount.Id, sortable: false, align: 'right', formatter: vatAmountFormatter },
                { name: $VatFields.VATPercent.Id, index: $VatFields.VATPercent.Id, sortable: false, width: 100, align: 'right', formatter: percentageFormatter },
                { name: $VatFields.VATCalculatedAmount.Id, index: $VatFields.VATCalculatedAmount.Id, align: 'right', sortable: false, formatter: vatAmountFormatter },
                { name: $VatFields.VATCategory.Id, index: $VatFields.VATCategory.Id, sortable: false}
              ]
  });
  
  vatData = eval(vatData);
  // Populate data in VAT grid with existing VAT records
  if (vatData != null) {    
    for ($VatCurrent; $VatCurrent < vatData.length + 1; $VatCurrent++) {
      row = { SerialNumber: $VatCurrent, Id: $VatCurrent, SubType: vatData[$VatCurrent - 1][$VatFields.VATSubType.Id], Amount: vatData[$VatCurrent - 1][$VatFields.VATBaseAmount.Id], Percentage: vatData[$VatCurrent - 1][$VatFields.VATPercent.Id], CalculatedAmount: vatData[$VatCurrent - 1][$VatFields.VATCalculatedAmount.Id], CategoryCode: vatData[$VatCurrent - 1][$VatFields.VATCategory.Id], Description: vatData[$VatCurrent - 1][$VatFields.VATText.Id] };
      $VatList.jqGrid('addRowData', $VatCurrent, row);
      addVatFields(vatData[$VatCurrent - 1][$VatFields.VATId.Id], row[$VatFields.VATSubType.Id], row[$VatFields.VATCategory.Id], row[$VatFields.VATBaseAmount.Id], row[$VatFields.VATCalculatedAmount.Id], row[$VatFields.VATPercent.Id], row[$VatFields.VATText.Id]);
    }
  }

  $('#' + $VATCategoryCode).change(function () {
    enableVATCalculatedAmount();
});

 //TFS#9985:Mozilla:V46: Pagination get overlapped on VAT Breakdown Capture Screen.
 $("#DerivedVatGrid_pager_center").width(297);
}

//Create object for VatField constant
function CreateVatField(id, displayName) {
  obj = [];
  obj.Id = id;
  obj.DisplayName = displayName;
  return obj;
}

function addVat() {
  setVatCalculatedAmt_glbl();
  var vatSubType = $('#' + $VATSubType).val();
 
  var vatBaseAmt = $('#' + $VATBaseAmount).val();

  var vatText = $('#'+$VatDescription).val();
  var vatPercentage = $('#' + $VATPercent).val();
  var vatCalculatedAmt = $('#' + $VATCalculatedAmount).val();
  var vatId = $('#' + $VATId).val();

  var vatCategory = $('#' + $VATCategoryCode).val();
  
 
  //for dropdowns clone only the value in textbox.
  var subType = $('#' + $VATId).clone();
  subType.attr("name", $VATSubType + $VatCurrent);
  subType.attr("id", $VATSubType + $VatCurrent);
  subType.val(vatSubType);
  subType.appendTo("#childVatList");

  var category = $('#' + $VATId).clone();
  category.attr("name", $VATCategoryCode + $VatCurrent);
  category.attr("id", $VATCategoryCode + $VatCurrent);
  category.val(vatCategory);
  category.appendTo("#childVatList");

 
  var description = $('#' + $VATId).clone();
  description.attr("name", $VatDescription + $VatCurrent);
  description.attr("id", $VatDescription + $VatCurrent);
  description.val(vatText);
  description.appendTo("#childVatList");

  // Change id of textboxes and append it in child tax list div
  $("#VatDetails").children("div").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $VatCurrent, id: currentElem.attr("id") + $VatCurrent, value: currentElem.val() });
    inputControl.appendTo('#childVatList');
  });

  //Clear values of textboxes in capture div
  $('#' + $VATSubType).val(vatSubTypeDefault);
  $('#' + $VATCategoryCode).val('');
  $('#' + $VatDescription).val('');
  $('#' + $VATBaseAmount).val('');
  $('#' + $VATPercent).val('');
  $('#' + $VATCalculatedAmount).val('');
  //disable Tax Calculated Amount.
  $('#' + $VATCalculatedAmount).attr('readonly', true);
  $('#' + $VATSubType).focus();
  //Add record in grid
  var row = { Id: $VatCurrent, SubType: vatSubType, Amount: vatBaseAmt, Percentage: vatPercentage, CalculatedAmount: vatCalculatedAmt, CategoryCode: vatCategory, Description: vatText };
  $VatList.addRowData($VatCurrent, row);
  $VatCurrent++;

  //update the serial numbers.
  var rowIds = $VatList.getDataIDs();
  for (cnt = 0; cnt <= rowIds.length; cnt++) {
    try {
      $VatList.jqGrid('setCell', rowIds[cnt], $VatFields.VATSerialNo.Id, cnt + 1);
      id++;
    } catch (e) { }
  }  
  if (vatCalculatedAmt != '') {    
    totalVatAmount = Number(totalVatAmount) + Number(vatCalculatedAmt);
  }
  else {
    if (isNaN(totalVatAmount)) {totalVatAmount = 0;}
    totalVatAmount = Number(totalVatAmount);
  }  
  $($TotalVatAmount).val(totalVatAmount.toFixed(_amountDecimals));

  // Set the parent form dirty.
  $parentForm.setDirty();
}

// Add hidden input fields for existing VAT records. Called only for existing VAT data.
function addVatFields(id, subTypeValue, categoryValue, vatBaseAmount, vatCalculatedAmt, vatPercentage, vatText) {
  var vat = $("#VatDetails").clone(true);
  vat.find('#' + $VATBaseAmount).val(vatBaseAmount);
  vat.find('#' + $VATPercent).val(vatPercentage);
  vat.find('#' + $VATCalculatedAmount).val(vatCalculatedAmt);
  vat.find('#' + $VatDescription).val(vatText);
  vat.find('#' + $VATId).val(id);

  var subType = $('#' + $VATId).clone();
  subType.attr("name", $VATSubType + $VatCurrent);
  subType.attr("id", $VATSubType + $VatCurrent);
  subType.val(subTypeValue);
  subType.appendTo("#childVatList");

  var category = $('#' + $VATId).clone();
  category.attr("name", $VATCategoryCode + $VatCurrent);
  category.attr("id", $VATCategoryCode + $VatCurrent);
  category.val(categoryValue);
  category.appendTo("#childVatList");

  // Change id of textboxes and append it in child tax list div
  vat.children("div").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $VatCurrent, id: currentElem.attr("id") + $VatCurrent, value: currentElem.val() });
    inputControl.appendTo('#childVatList');
  });

 
}

// Close VAT details modal dialog
function closeVatDetail() {
  $validateVat.resetForm();
  $('#VATBreakdown').dialog('close');
  setFocusAndBlur('#VatAmount');
}

function setVATCalculatedAmount(couponGrossValue) {
  //trim white space
  var vatValue = $('#'+$VATPercent).val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  var vatPercentage = vatValue / 100 * couponGrossValue;
  // in case of invalid entry of Base Amount or Percent, set calculated value as zero.
  if (isNaN(vatPercentage)) { vatPercentage = 0; }
  $('#'+$VATCalculatedAmount).val(vatPercentage.toFixed(_amountDecimals));
}

//Custom formatter to display delete button in grid
function buttonVatDeleteFormatter(cellValue, options, cellObject) {
    return "<a style='cursor:hand;' target='_parent' href='javascript:deleteVatRecord(" + cellValue + ");' title='Delete'><div class='deleteIcon ignoredirty'></div></a>";
}

//Custom formatter to display amount rounded to 2 decimal places in grid
function vatAmountFormatter(cellValue, options, cellObject) {  
  if (cellValue != null && cellValue != '' ) {    
    return Number(cellValue).toFixed(_amountDecimals);
  }
  else {
    // return empty string instead of null.  
    return '&nbsp;';
  }
}

// Custom formatter to display percentage rounded to 3 decimal places in grid
function percentageFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_percentDecimals);
}

//Method to delete VAT record
function deleteVatRecord(id) {
  if (confirm("Are you sure you want to delete this record?")) {
    //calculate and set total vat amount

    var amountToBeDeducted = $('#' + $VATCalculatedAmount + id).val();
    totalVatAmount = totalVatAmount - Number(amountToBeDeducted);    

    //Delete record from grid
    $VatList.delRowData(id);
    
    //delete record entries from hidden fields
    $('#' + $VATId + id).remove();
    $('#' + $VATSubType + id).remove();
    $('#' + $VATBaseAmount + id).remove();
    $('#' + $VATCalculatedAmount + id).remove();
    $('#' + $VATCategoryCode + id).remove();
    $('#' + $VATPercent + id).remove();
    $('#' + $VatDescription + id).remove();

    //update the serial numbers.
    var rowIds = $VatList.getDataIDs();

    if (!isNaN(totalVatAmount)) {
      // For bug 7091. If after deletion of breakdown record, total VAT amount becomes zero, then it should go into db as null.
      if (rowIds.length == 0 && totalVatAmount == 0) {
        $($TotalVatAmount).val(null);
      } else {
        $($TotalVatAmount).val(totalVatAmount.toFixed(_amountDecimals));
      }

      // Set the parent form dirty.
      $parentForm.setDirty();
    }

    

    for (cnt = 0; cnt <= rowIds.length; cnt++) {
      try {
        $VatList.jqGrid('setCell', rowIds[cnt], $VatFields.VATSerialNo.Id, cnt + 1);
        id++;
      } catch (e) { }
    }
  }
}

$(document).ready(function () {
  $('#' + $VATBaseAmount).blur(function () {
    setCalculatedAmt();
  });

  $('#' + $VATPercent).blur(function () {
    setCalculatedAmt();   
  });  

  function setCalculatedAmt() {
    // Validation not to be done if tax category = exempt or reverse charge.
    var categoryCode = $('#' + $VATCategoryCode).val();
    if (!(categoryCode == 'Exempt' || categoryCode == 'Reverse Charge')) {
      if (!isNaN($('#' + $VATPercent).val()) && !isNaN($('#' + $VATBaseAmount).val())) {
        var calcAmt = $('#' + $VATPercent).val() / 100 * $('#' + $VATBaseAmount).val();
        $('#' + $VATCalculatedAmount).val(calcAmt.toFixed(_amountDecimals));
      }
      else {
        $('#' + $VATCalculatedAmount).removeAttr('value');
      }
    }
  }

  $('#VATBreakdown').bind('dialogclose', function (event) {
    setFocusAndBlur('#VatAmount');
  });
});

// This function is written to be accessible within addVat() function.
function setVatCalculatedAmt_glbl() {
  // Validation not to be done if tax category = exempt or reverse charge.
  var categoryCode = $('#' + $VATCategoryCode).val();
  if (!(categoryCode == 'Exempt' || categoryCode == 'Reverse Charge')) {
    if (!isNaN($('#' + $VATPercent).val()) && !isNaN($('#' + $VATBaseAmount).val())) {
      var calcAmt = $('#' + $VATPercent).val() / 100 * $('#' + $VATBaseAmount).val();
      $('#' + $VATCalculatedAmount).val(calcAmt.toFixed(_amountDecimals));
    }
    else {
      $('#' + $VATCalculatedAmount).removeAttr('value');
    }
  }
}

function enableVATCalculatedAmount() {
  if ($('#' + $VATCategoryCode).val() == 'Exempt' || $('#' + $VATCategoryCode).val() == 'Reverse Charge') {
    $('#' + $VATCalculatedAmount).removeAttr('readonly');
    $('#' + $VATCalculatedAmount).val('');
  }
  else {
    $('#' + $VATCalculatedAmount).attr('readonly', true);
    if (!isNaN($('#' + $VATPercent).val()) && !isNaN($('#' + $VATBaseAmount).val())) {
      var calcAmt = $('#' + $VATPercent).val() / 100 * $('#' + $VATBaseAmount).val();
      $('#' + $VATCalculatedAmount).val(calcAmt.toFixed(_amountDecimals));
    }
    else {
      $('#' + $VATCalculatedAmount).removeAttr('value');
    }
  }
}