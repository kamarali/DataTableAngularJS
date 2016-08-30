
var $AddChargeFields = InitializeAddChargeFields();
var $AddChargeCurrent = 1;
var $AddChargeList;
var totalAddChargeAmount = 0;
var $validateAddCharge;
var addChargeGridHeight = 150;

var $AddChargeName = '#Name';
var $ChargeableAmount = '#ChargeableAmount';
var $Percentage = '#AddChargePercentage';
var $Amount = '#AddChargeAmount';
var $AddChargeId = '#AddChargeId';
var $LineItemNos = '#ChargeForLineItemNumber';
var $isForLineItem=false;
var $TotalAddOnCharge = '#TotalAddChargeAmount';

// SCP278884: Add/Deduct Charge field
var invoiceSummaryTotalAmount = 0;
var $TotalAmountControl = '#InvoiceSummary_TotalAmount';

//Initializes AddChargeFields constant which contains data field and display names in grid.
function InitializeAddChargeFields() {  
  var field = {};
  field.AddChargeId = CreateAddChargeField('Id', 'Action');
  field.AddChargeSerialNo = CreateAddChargeField('SerialNumber', 'Sr. No.');
  field.AddChargeName = CreateAddChargeField('Name', 'Add/Deduct Charge Name');
  field.AddChargeBaseAmount = CreateAddChargeField('ChargeableAmount', 'Base Amount');
  field.AddChargePercent = CreateAddChargeField('Percentage', 'Percent');
  field.AddChargeCalculatedAmount = CreateAddChargeField('Amount', 'Calculated Amount');
  field.AddChargeLineItemNumbers = CreateAddChargeField('ChargeForLineItemNumber', 'Line Item Numbers');
  return field;
}

function InitializeAddChargeGrid(addChargeData, isForLineItem) {

  $isForLineItem = isForLineItem;
  if ($isOnView) { $("#formAddChargeDetails").hide();addChargeGridHeight = 350; }
  totalAddChargeAmount = $($TotalAddOnCharge).val(); // Initialize totalAddChargeAmount
  // SCP278884: Add/Deduct Charge field
  invoiceSummaryTotalAmount = $($TotalAmountControl).val(); // Initialize InvoiceSummaryTotalAmount

  $validateAddCharge = $('#formAddChargeDetails').validate({
    rules: {
      Name: {
        required: true
        //        alphaNumeric: true
        //maxlength: 30
      },
      AddChargePercentage: { min: -999.99, max: 999.99 },
      AddChargeAmount: { required: true }
    },
    messages: {
      Name: { required: "Add/Deduct Charge Name Required" },
      AddChargeAmount: { required: "Calculated Amount Required" }
    },
    submitHandler: function () {
      addAddCharge();
    },
    highlight: false
  });

  //Initialize grid
  $AddChargeList = $('#AddChargeGrid');
  $AddChargeList.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 800,
    height: addChargeGridHeight,
    colNames: [$AddChargeFields.AddChargeId.DisplayName, $AddChargeFields.AddChargeSerialNo.DisplayName, $AddChargeFields.AddChargeName.DisplayName, $AddChargeFields.AddChargeBaseAmount.DisplayName, $AddChargeFields.AddChargePercent.DisplayName, $AddChargeFields.AddChargeCalculatedAmount.DisplayName, $AddChargeFields.AddChargeLineItemNumbers.DisplayName],
    colModel: [
                { name: $AddChargeFields.AddChargeId.Id, index: $AddChargeFields.AddChargeId.Id, sortable: false, formatter: buttonAddChargeDeleteFormatter, width: 65, hidden: $isOnView },
                { name: $AddChargeFields.AddChargeSerialNo.Id, index: $AddChargeFields.AddChargeSerialNo.Id, sortable: false, align: 'right' },
                { name: $AddChargeFields.AddChargeName.Id, index: $AddChargeFields.AddChargeName.Id, sortable: false, width: 300 },
                { name: $AddChargeFields.AddChargeBaseAmount.Id, index: $AddChargeFields.AddChargeBaseAmount.Id, sortable: false, formatter: amountFormatter, align: 'right' },
                { name: $AddChargeFields.AddChargePercent.Id, index: $AddChargeFields.AddChargePercent.Id, sortable: false, formatter: percentageFormatter, align:'right' },
                { name: $AddChargeFields.AddChargeCalculatedAmount.Id, index: $AddChargeFields.AddChargeCalculatedAmount.Id, sortable: false, align: 'right', formatter: amountFormatter },
                { name: $AddChargeFields.AddChargeLineItemNumbers.Id, index: $AddChargeFields.AddChargeLineItemNumbers.Id, sortable: false, width: 200, align: 'right', hidden: $isForLineItem }
              ]
  });

  addChargeData = eval(addChargeData);
  // Populate data in Add/Charge grid with existing Add/Charge records
  if (addChargeData != null) {
    for ($AddChargeCurrent; $AddChargeCurrent < addChargeData.length + 1; $AddChargeCurrent++) {
      row = { Id: $AddChargeCurrent, Name: addChargeData[$AddChargeCurrent - 1][$AddChargeFields.AddChargeName.Id], Amount: addChargeData[$AddChargeCurrent - 1][$AddChargeFields.AddChargeCalculatedAmount.Id], Percentage: addChargeData[$AddChargeCurrent - 1][$AddChargeFields.AddChargePercent.Id], ChargeableAmount: addChargeData[$AddChargeCurrent - 1][$AddChargeFields.AddChargeBaseAmount.Id], ChargeForLineItemNumber: addChargeData[$AddChargeCurrent - 1][$AddChargeFields.AddChargeLineItemNumbers.Id], SerialNumber: $AddChargeCurrent };
      $AddChargeList.jqGrid('addRowData', $AddChargeCurrent, row);
      addAddChargeFields(addChargeData[$AddChargeCurrent - 1][$AddChargeFields.AddChargeId.Id], row[$AddChargeFields.AddChargeName.Id], row[$AddChargeFields.AddChargeBaseAmount.Id], row[$AddChargeFields.AddChargePercent.Id], row[$AddChargeFields.AddChargeCalculatedAmount.Id], row[$AddChargeFields.AddChargeLineItemNumbers.Id]);
    }
  }
}

//Create object for AddOnChargeField constant
function CreateAddChargeField(id, displayName) {
  obj = [];
  obj.Id = id;
  obj.DisplayName = displayName;
  return obj;
}

function addAddCharge() {
  setCalculatedAmtOnAdd();
  var addChargeName = $($AddChargeName).val();
  var addChargeBaseAmt = $($ChargeableAmount).val();
  var addChargePercent = $($Percentage).val();
  var addChargeCalcAmt = $($Amount).val();
  var addChargeId = $($AddChargeId).val();
  var addChargeLineItemNos = $($LineItemNos).val();
  
  //Clone vat div containing details entered in capture screen
  var AddCharge = $("#AddChargeDetails").clone(true);
  //Change id of textboxes and append it in child vat list div
  AddCharge.children("div").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $AddChargeCurrent, id: currentElem.attr("id") + $AddChargeCurrent, value: currentElem.val() });
    inputControl.appendTo('#childAddChargeList');
  });

  
  //Clear values of textboxes in capture div
  $($AddChargeName).val('');
  $($ChargeableAmount).val('');
  $($Percentage).val('');
  $($Amount).val('');
  $($LineItemNos).val('');
  $($Amount).removeAttr('readonly');
  //Add record in grid
  var row = { Id: $AddChargeCurrent, Name: addChargeName, ChargeableAmount: addChargeBaseAmt, Percentage: addChargePercent, Amount: addChargeCalcAmt, ChargeForLineItemNumber: addChargeLineItemNos };
  $AddChargeList.addRowData($AddChargeCurrent, row);
  $AddChargeCurrent++;

  //update the serial numbers.
  var rowIds = $AddChargeList.getDataIDs();
  for (cnt = 0; cnt <= rowIds.length; cnt++) {
    try {
      $AddChargeList.jqGrid('setCell', rowIds[cnt], $AddChargeFields.AddChargeSerialNo.Id, cnt + 1);
      id++;
    } catch (e) { }
  }

  totalAddChargeAmount = Number(totalAddChargeAmount) + Number(addChargeCalcAmt);
  $($TotalAddOnCharge).val(totalAddChargeAmount.toFixed(_amountDecimals));
  // SCP278884: Add/Deduct Charge field
  invoiceSummaryTotalAmount = Number(invoiceSummaryTotalAmount) + Number(addChargeCalcAmt);
  $($TotalAmountControl).val(invoiceSummaryTotalAmount.toFixed(_amountDecimals));

  $parentForm.setDirty();
}

// Add hidden input fields for existing VAT records. Called only for existing VAT data.
//addAddChargeFields(addChargeData[$AddChargeCurrent - 1][field.AddChargeId.Id], row[$AddChargeFieldselds.AddChargeBaseAmount.Id], row[$AddChargeFields.AddChargePercent.Id], row[$AddChargeFields.AddChargeCalculatedAmount.Id], row[$AddChargeFields.AddChargeLineItemNumbers.Id]);

function addAddChargeFields(id, addChargeName,baseAmt, percent, calcAmt, lineItemNos) {
  var addCharge = $("#AddChargeDetails").clone(true);
  
  addCharge.find($AddChargeId).val(id);
  addCharge.find($ChargeableAmount).val(baseAmt);
  addCharge.find($Percentage).val(percent);
  addCharge.find($Amount).val(calcAmt);
  addCharge.find($LineItemNos).val(lineItemNos);
  addCharge.find($AddChargeName).val(addChargeName);

  addCharge.children("div").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $AddChargeCurrent, id: currentElem.attr("id") + $AddChargeCurrent, value: currentElem.val() });
    inputControl.appendTo('#childAddChargeList');
  });
  
}

function closeAddChargeDetail() {
  $validateAddCharge.resetForm();
  $('#AddChargeBreakdown').dialog('close');
  setFocusAndBlur('#TotalAddChargeAmount');
}

//Custom formatter to display delete button in grid
function buttonAddChargeDeleteFormatter(cellValue, options, cellObject) {
    return "<a style='cursor:hand;' target='_parent' href='javascript:deleteAddCharge(" + cellValue + ");'><div class='deleteIcon ignoredirty'></div></a>";
}

//Custom formatter to display amount rounded to 2 decimal places in grid
function amountFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_amountDecimals);
}

//Custom formatter to display percentage rounded to 3 decimal places in grid
function percentageFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_percentDecimals);
}

//Method to delete add charge record
function deleteAddCharge(id) {
  if (confirm("Are you sure you want to delete this record?")) {
    //calculate and set total add charge amount

    var amountToBeDeducted = $($Amount + id).val();
    totalAddChargeAmount = totalAddChargeAmount - Number(amountToBeDeducted);
    // SCP278884: Add/Deduct Charge field
    invoiceSummaryTotalAmount = invoiceSummaryTotalAmount - Number(amountToBeDeducted);

    //Delete record from grid
    $AddChargeList.delRowData(id);

    //delete record entries from hidden fields
    $($AddChargeId + id).remove();
    $($AddChargeName + id).remove();
    $($Amount + id).remove();
    $($ChargeableAmount + id).remove();
    $($LineItemNos + id).remove();
    $($Percentage + id).remove();

    //update the serial numbers.
    var rowIds = $AddChargeList.getDataIDs();

    if (!isNaN(totalAddChargeAmount)) {
      if (rowIds.length == 0 && totalAddChargeAmount == 0) {
        $($TotalAddOnCharge).val(null);
      } else {
        $($TotalAddOnCharge).val(totalAddChargeAmount.toFixed(_amountDecimals));

      }
      // SCP278884: Add/Deduct Charge field
      $($TotalAmountControl).val(invoiceSummaryTotalAmount.toFixed(_amountDecimals));
      $parentForm.setDirty();
    }

    for (cnt = 0; cnt <= rowIds.length; cnt++) {
      try {
        $AddChargeList.jqGrid('setCell', rowIds[cnt], $AddChargeFields.AddChargeSerialNo.Id, cnt + 1);
        id++;
      } catch (e) { }
    }
  }
}

$(document).ready(function () {
  $($ChargeableAmount).blur(function () {
    if (!isNaN($($ChargeableAmount).val())) {
      //setAmount($ChargeableAmount, 2);
      setCalculatedAmt();      
    }
  });

  $($Percentage).blur(function () {
    if (!isNaN($($Percentage).val())) {
      // setAmount($Percentage, 3);
      setCalculatedAmt();      
    }
  });

  $('#AddChargeBreakdown').bind('dialogclose', function (event) {
    setFocusAndBlur('#TotalAddChargeAmount');
  });

  $($Amount).focus(function () {
    if ($.trim($($Percentage).val()) == '' && $.trim($($ChargeableAmount).val()) == '') {
      $($Amount).removeAttr('readonly');
    }
    else {
      $($Amount).attr('readonly', 'readonly');
    }
  });
});

function setAmount(fieldId, decimalPlaces) {
  if (!isNaN($(fieldId).val()))
    $(fieldId).val(Number($(fieldId).val()).toFixed(decimalPlaces));
}

function setCalculatedAmt() {
  if ($.trim($($Percentage).val()) == '' && $.trim($($ChargeableAmount).val()) == '') {
    $($Amount).removeAttr('readonly');
    $($Amount).removeAttr('value');
  }
  else {
    $($Amount).attr('readonly', 'readonly');
    if (!isNaN($($Percentage).val()) && !isNaN($($ChargeableAmount).val())) {
      var calcAmt = $($Percentage).val() / 100 * $($ChargeableAmount).val();
      $($Amount).val(calcAmt.toFixed(_amountDecimals));
    }
    else {
      $($Amount).removeAttr('value');
    }
  }
}

function setCalculatedAmtOnAdd() {
  if ($.trim($($Percentage).val()) == '' && $.trim($($ChargeableAmount).val()) == '') {
    $($Amount).removeAttr('readonly');
  }
  else {
    $($Amount).attr('readonly', 'readonly');
    if (!isNaN($($Percentage).val()) && !isNaN($($ChargeableAmount).val())) {
      var calcAmt = $($Percentage).val() / 100 * $($ChargeableAmount).val();
      $($Amount).val(calcAmt.toFixed(_amountDecimals));
    }
    else {
      $($Amount).removeAttr('value');
    }
  }
}
