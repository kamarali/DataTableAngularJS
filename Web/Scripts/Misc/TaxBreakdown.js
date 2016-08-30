var $taxCurrent = 1;
var $taxList;
var $validateTax;
var taxFields = InitializeTaxFields();
var totalTaxAmount = 0;
var $TotalTaxAmount = '#TaxAmount';
var taxGridHeight = 150;
_percentDecimals = 3;

function InitializeTaxGrid(taxData) {
  if ($isOnView) { $("#formTaxDetails").hide(); taxGridHeight = 350; }
  
  totalTaxAmount = $($TotalTaxAmount).val(); //initialize totalTaxAmount
  $validateTax = $("#formTaxDetails").validate({
    rules: {
      SubType: { required: true },
      TaxDescription: { maxlength: 150 },        //CMP464: Extende Tax Description text length for 50 to 150 
      Percentage: { min: -99.999, max: 99.999 }
    },
    messages: {
      SubType: {
        required: "Tax Sub Type required."
      },
      Percentage: "Value should be between -99.999 to 99.999"
    },
    submitHandler: function () {
      addTax();
    },
    highlight: false
  });

  // Initialize tax grid
  $taxList = $('#taxGrid');
  $taxList.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 800,
    height: taxGridHeight,
    colNames: [taxFields.TaxId.DisplayName, taxFields.SerialNumber.DisplayName, taxFields.Description.DisplayName, taxFields.BaseAmount.DisplayName, taxFields.TaxPercent.DisplayName, taxFields.CalculatedAmount.DisplayName, taxFields.CategoryCode.DisplayName, taxFields.SubType.DisplayName],
    colModel: [
                { name: 'Id', index: 'Id', sorttype: 'int', sortable: false, formatter: buttonFmatterTax, width: 60, hidden: $isOnView },
                { name: taxFields.SerialNumber.Id, index: taxFields.SerialNumber.Id, sortable: false, width: 50 },
                { name: taxFields.Description.Id, index: taxFields.Description.Id, sortable: false, width: 250 },
                { name: taxFields.BaseAmount.Id, index: taxFields.BaseAmount.Id, sortable: false, align: 'right', formatter: taxAmountFormatter },
                /*SCP257502:  VAT NODE MISSING-Upendra*/
                { name: taxFields.TaxPercent.Id, index: taxFields.TaxPercent.Id, sortable: false, align: 'right', formatter: taxpercentageFormatter },
                { name: taxFields.CalculatedAmount.Id, index: taxFields.CalculatedAmount.Id, sortable: false, align: 'right', formatter: taxAmountFormatter },
                { name: taxFields.CategoryCode.Id, index: taxFields.CategoryCode.Id, sortable: false },
                { name: taxFields.SubType.Id, index: taxFields.SubType.Id, sortable: false }
              ]
  });

  taxData = eval(taxData);
  // Populate data in tax grid with existing tax records
  if (taxData != null) {
    for ($taxCurrent; $taxCurrent < taxData.length + 1; $taxCurrent++) {
      row = { Id: $taxCurrent, TaxDescription: taxData[$taxCurrent - 1]["Description"], Percentage: taxData[$taxCurrent - 1][taxFields.TaxPercent.Id], CalculatedAmount: taxData[$taxCurrent - 1][taxFields.CalculatedAmount.Id], SerialNumber: $taxCurrent, CategoryCode: taxData[$taxCurrent - 1][taxFields.CategoryCode.Id], Amount: taxData[$taxCurrent - 1][taxFields.BaseAmount.Id], SubType: taxData[$taxCurrent - 1][taxFields.SubType.Id] };
      $taxList.jqGrid('addRowData', $taxCurrent, row);
      addTaxFields($taxCurrent, taxData[$taxCurrent - 1]["Id"], row[taxFields.Description.Id], row[taxFields.TaxPercent.Id], row[taxFields.CalculatedAmount.Id], row[taxFields.CategoryCode.Id], row[taxFields.BaseAmount.Id], row[taxFields.SubType.Id]);
    }
  }
  $('#TaxBreakdown').bind('dialogclose', function (event) {
    setFocusAndBlur('#TaxAmount');
  });
}

// Create object for Tax field constant
function CreateField(id, displayName) {
  obj = [];
  obj.Id = id;
  obj.DisplayName = displayName;
  return obj;
}

// Initializes taxField constant which contains values for Controls and display name for grid title
function InitializeTaxFields() {
  var fields = new Array();  
  fields.TaxId = CreateField('TaxId', 'Action');
  fields.Description = CreateField('TaxDescription', 'Tax Text');
  fields.BaseAmount = CreateField('Amount', 'Tax Base Amount');
  fields.TaxPercent = CreateField('Percentage', 'Tax Percent');
  fields.CalculatedAmount = CreateField('CalculatedAmount', 'Tax Calculated Amount');
  fields.CategoryCode = CreateField('CategoryCode', 'Tax Category');
  fields.SerialNumber = CreateField('SerialNumber', 'Sr. No.');
  fields.SubType = CreateField('SubType', 'Tax Name');
  return fields;
}

// Custom formatter to display delete button in grid
function buttonFmatterTax(cellValue, options, cellObject) {
    return "<a style='cursor:hand;' target='_parent' href='javascript:deleteTaxRecord(" + cellValue + ");' title='Delete'><div class='deleteIcon ignoredirty'></div></a>";
}

// Custom formatter to display amount rounded to 2 decimal places in grid
function taxAmountFormatter(cellValue, options, cellObject) {
  if (cellValue != '')
    return Number(cellValue).toFixed(_amountDecimals);
  else
    return '';
}
/*SCP257502:  VAT NODE MISSING-Upendra*/
// Custom formatter to display percentage rounded to 3 decimal places in grid
function percentageFormatter(cellValue, options, cellObject) {
    if (cellValue != '') 
    return Number(cellValue).toFixed(_percentDecimals);
    else
        return '';
}
/*SCP257502:  VAT NODE MISSING-Upendra*/
// Custom formatter to display percentage rounded to 3 decimal places in grid
function taxpercentageFormatter(cellValue, options, cellObject) {
    if (cellValue != '')
        return Number(cellValue).toFixed(_percentDecimals);
    else
        return '';
}

// Method to delete tax record
function deleteTaxRecord(id) {

  if (confirm("Are you sure you want to delete this record?")) {
    //calculate and set total vat amount  
    var amountToBeDeducted = $('#' + taxFields.CalculatedAmount.Id + id).val();
    
    totalTaxAmount = totalTaxAmount - Number(amountToBeDeducted);

    //Delete record from grid
    $taxList.delRowData(id);

    //delete record entries from hidden fields
    var taxText = '#' + taxFields.Description.Id + id;
    $(taxText).remove();

    var taxAmount = '#' + taxFields.BaseAmount.Id + id;
    $(taxAmount).remove();

    var taxId = '#' + taxFields.TaxId.Id + id;
    $(taxId).remove();

    var taxCalculatedAmt = '#' + taxFields.CalculatedAmount.Id + id;
    $(taxCalculatedAmt).remove();

    var taxCategoryCode = '#' + taxFields.CategoryCode.Id + id;
    $(taxCategoryCode).remove();

    var taxPercent = '#' + taxFields.TaxPercent.Id + id;
    $(taxPercent).remove();

    var taxSubType = '#' + taxFields.SubType.Id + id;
    $(taxSubType).remove();

    var rowIds = $taxList.getDataIDs();

    if (!isNaN(totalTaxAmount)) {
      // For bug 7091. If after deletion of breakdown record, total tax amount becomes zero, then it should go into db as null.
      if (rowIds.length == 0 && totalTaxAmount == 0) {
        $($TotalTaxAmount).val(null);
      }
      else {
        $($TotalTaxAmount).val(totalTaxAmount.toFixed(_amountDecimals));
      }
     }      
      // Set the parent form dirty.
      $parentForm.setDirty();

    

    for (cnt = 0; cnt <= rowIds.length; cnt++) {
      try {
        $taxList.jqGrid('setCell', rowIds[cnt], 'SerialNumber', cnt + 1);
        id++;
      } catch (e) { }
    }
  }
}

function addTax() {  
  setCalculatedAmount_glbl();
  var taxText = $('#' + taxFields.Description.Id).val();
  var taxBaseAmt = $('#' + taxFields.BaseAmount.Id).val();
  var taxPercent = $('#' + taxFields.TaxPercent.Id).val();
  var taxCalculatedAmt = $('#' + taxFields.CalculatedAmount.Id).val();
  var taxCategory = $('#' + taxFields.CategoryCode.Id).val();
  var taxSubType = $('#' + taxFields.SubType.Id).val();

  if ($.trim(taxText) == '' && $.trim(taxBaseAmt) == '' && $.trim(taxPercent) == '' && ($.trim(taxCalculatedAmt) == '' || $.trim(taxCalculatedAmt) == 0) && $.trim(taxCategory) == '') {
    alert('Please enter value for at least one field.');
    return;
  }


  var categoryClone = $('#' + taxFields.TaxId.Id).clone();
  categoryClone.attr("name", taxFields.CategoryCode.Id + $taxCurrent);
  categoryClone.attr("id", taxFields.CategoryCode.Id + $taxCurrent);
  categoryClone.val(taxCategory);
  categoryClone.appendTo("#childTaxList");

  var description = $('#' + taxFields.TaxId.Id).clone();
  description.attr("name", taxFields.Description.Id + $taxCurrent);
  description.attr("id", taxFields.Description.Id + $taxCurrent);
  description.val(taxText);
  description.appendTo("#childTaxList");

  // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
  // To add Tax sub type in child tax list
  var subTypeClone = $('#' + taxFields.TaxId.Id).clone();
  subTypeClone.attr("name", taxFields.SubType.Id + $taxCurrent);
  subTypeClone.attr("id", taxFields.SubType.Id + $taxCurrent);
  subTypeClone.val(taxSubType);
  subTypeClone.appendTo("#childTaxList");
  // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

  // Change id of textboxes and append it in child tax list div
  $("#TaxDetails").children("div").children("div").children("input").each(function (i) {
      var currentElem = $(this);
      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To add the other fields in childtaxlist excluding TaxSubType, CategoryCode, Description.
      if (currentElem.attr("name") != taxFields.Description.Id && currentElem.attr("name") != taxFields.SubType.Id && currentElem.attr("name") != taxFields.CategoryCode.Id) {
        // CMP #534: Tax Issues in MISC and UATP Invoices. [End]
          var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $taxCurrent, id: currentElem.attr("id") + $taxCurrent, value: currentElem.val() });
          inputControl.appendTo('#childTaxList');
      }
  });

  //Clear values of textboxes in capture div
  $('#' + taxFields.Description.Id).val("");
  $('#' + taxFields.BaseAmount.Id).val("");
  $('#' + taxFields.TaxPercent.Id).val("");
  $('#' + taxFields.CalculatedAmount.Id).val("");
  $('#' + taxFields.CategoryCode.Id).val("");
  $('#' + taxFields.SubType.Id).val("");

  //disable Tax Calculated Amount.
  $('#' + taxFields.BaseAmount.Id).focus();
  //Add record in grid
  var row = { Id: $taxCurrent, TaxDescription: taxText, Percentage: taxPercent, Amount: taxBaseAmt, CalculatedAmount: taxCalculatedAmt, CategoryCode: taxCategory, SerialNumber: $taxCurrent, SubType: taxSubType };
  $taxList.addRowData($taxCurrent, row);

  $taxCurrent++;

  var rowIds = $taxList.getDataIDs();
  for (cnt = 0; cnt <= rowIds.length; cnt++) {
    try {
      $taxList.jqGrid('setCell', rowIds[cnt], 'SerialNumber', cnt + 1);
      id++;
    } catch (e) { }
  }

  //calculate and set total vat amount
  totalTaxAmount = Number(totalTaxAmount) + Number(taxCalculatedAmt);
  $($TotalTaxAmount).val(totalTaxAmount.toFixed(_amountDecimals));
  
  // Set the parent form dirty.
  $parentForm.setDirty();
}

//Add hidden input fields for existing tax records. Is called only for existing data.
function addTaxFields($taxCurrent, id, taxText, taxPercent, taxCalculatedAmt, taxCategoryCode, taxBaseAmt, taxSubType) {
  var tax = $("#TaxDetails").clone(true);
  tax.find('#' + taxFields.BaseAmount.Id).val(taxBaseAmt);
  tax.find('#' + taxFields.CalculatedAmount.Id).val(taxCalculatedAmt);
  tax.find('#' + taxFields.TaxId.Id).val(id);
  tax.find('#' + taxFields.TaxPercent.Id).val(taxPercent);
  tax.find('#' + taxFields.Description.Id).val(taxText);
  tax.find('#' + taxFields.CategoryCode.Id).val(taxCategoryCode);
  tax.find('#' + taxFields.SubType.Id).val(taxSubType);

  var categoryClone = $('#' + taxFields.TaxId.Id).clone();
  categoryClone.attr("name", taxFields.CategoryCode.Id + $taxCurrent);
  categoryClone.attr("id", taxFields.CategoryCode.Id + $taxCurrent);
  categoryClone.val(taxCategoryCode);
  categoryClone.appendTo("#childTaxList");

  var description = $('#' + taxFields.TaxId.Id).clone();
  description.attr("name", taxFields.Description.Id + $taxCurrent);
  description.attr("id", taxFields.Description.Id + $taxCurrent);
  description.val(taxText);
  description.appendTo("#childTaxList");

  // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
  // To add Tax sub type in child tax list
  var subTypeClone = $('#' + taxFields.TaxId.Id).clone();
  subTypeClone.attr("name", taxFields.SubType.Id + $taxCurrent);
  subTypeClone.attr("id", taxFields.SubType.Id + $taxCurrent);
  subTypeClone.val(taxSubType);
  subTypeClone.appendTo("#childTaxList");
  // CMP #534: Tax Issues in MISC and UATP Invoices. [End]


  // Change id of textboxes and append it in child tax list div
  tax.children("div").children("div").children("input").each(function (i) {
      var currentElem = $(this);
      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To add the other fields in childtaxlist excluding TaxSubType, CategoryCode, Description.
      if (currentElem.attr("name") != taxFields.Description.Id && currentElem.attr("name") != taxFields.SubType.Id && currentElem.attr("name") != taxFields.CategoryCode.Id) {
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]
          var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $taxCurrent, id: currentElem.attr("id") + $taxCurrent, value: currentElem.val() });
          inputControl.appendTo('#childTaxList');
      }
  });
}

//Close tax details modal dialogue
function closeTaxDetail() {
  $validateTax.resetForm();
  $('#TaxBreakdown').dialog('close');
  setFocusAndBlur('#TaxAmount');
}

$(document).ready(function () {
  $('#Amount').blur(function () {
    setCalculatedAmt();
  });

  $('#Percentage').blur(function () {
    setCalculatedAmt();
  });

  function setCalculatedAmt() {
    if ($.trim($('#Percentage').val()) == '' && $.trim($('#Amount').val()) == '') {
      $('#CalculatedAmount').removeAttr('readonly');
      $('#CalculatedAmount').removeAttr('value');
    }
    else
    {
      $('#CalculatedAmount').attr('readonly', 'readonly');
      // Validation not to be done if tax category = exempt or reverse charge.
      //var categoryCode = $('#CategoryCode').val();
      //if (!(categoryCode == 'Exempt' || categoryCode == 'Reverse Charge')) {
      if (!isNaN($('#Percentage').val()) && !isNaN($('#Amount').val())) {
        var calcAmt = $('#Percentage').val() / 100 * $('#Amount').val();
        $('#CalculatedAmount').val(calcAmt.toFixed(_amountDecimals));
      }
      else {
        $('#CalculatedAmount').removeAttr('value');
      }
    }
  }
});

// This function is written to be accessible within addTax() function.
function setCalculatedAmount_glbl() {
  if ($.trim($('#Percentage').val()) == '' && $.trim($('#Amount').val()) == '') {
    $('#CalculatedAmount').removeAttr('readonly');
  }
  else {
    $('#CalculatedAmount').attr('readonly', 'readonly');
    if (!isNaN($('#Percentage').val()) && !isNaN($('#Amount').val())) {
      var calcAmt = $('#Percentage').val() / 100 * $('#Amount').val();
      $('#CalculatedAmount').val(calcAmt.toFixed(_amountDecimals));
    }
    else {
      $('#CalculatedAmount').removeAttr('value');
    }
  }
}