var taxCurrent = 1;
var $taxList;
var $validateTax;
var taxFields = InitializeTaxFields();
var totalTaxAmount = 0;
var isCreditMemo = false;
_amountDecimals = 2;

function InitializeTaxGrid(taxData) {
    
  if ($isOnView) { $("#formTaxDetails").hide(); }
  totalTaxAmount = $('#TaxAmount').val(); //initialize totalTaxAmount

  // Following condition registers control events and validation rules if pageMode is Create or Edit and not for View mode
  if (!$isOnView) {
    $validateTax = $("#formTaxDetails").validate({
      rules: {
        TaxCode: {
          required: true,
          maxlength: 2
        },
        Amount: "required"
      },
      messages: {
        TaxCode: {
          required: "Tax Code Required",
          maxlength: "Please enter no more than 2 characters."
        }
      },
      submitHandler: function () {
        addTax();
      }, highlight: false
    });

    if (!isCreditMemo) {
      $("#Amount").blur(function () {
        if ($('#Amount').val() == 0.00) {
          $("#Amount").val('');
          return;
        }
      });
    }
  }

  // Initialize tax grid
  $taxList = $('#taxGrid');
  $taxList.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 475,
    height: 250,
    colNames: [taxFields.TaxId.DisplayName, taxFields.TaxCode.DisplayName, taxFields.TaxAmount.DisplayName],
    colModel: [
                { name: 'Id', index: 'Id', sorttype: 'int', sortable: false, formatter: buttonFmatter, width: 30, hidden: $isOnView },
                { name: 'TaxCode', index: 'TaxCode', sortable: false },
                { name: 'Amount', index: 'Amount', sortable: false, align: 'right', formatter: amountFormatter }
              ]
  });

  taxData = eval(taxData);
  // Populate data in tax grid with existing tax records
  if (taxData != null) {
    for (taxCurrent; taxCurrent < taxData.length + 1; taxCurrent++) {
      row = { Id: taxCurrent, TaxCode: taxData[taxCurrent - 1]["TaxCode"], Amount: taxData[taxCurrent - 1]["Amount"] };
      $taxList.jqGrid('addRowData', taxCurrent, row);
      addTaxFields(taxCurrent, taxData[taxCurrent - 1]["Id"], row["TaxCode"], row["Amount"]);
    }
  }
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
  fields.TaxCode = CreateField('TaxCode', 'Tax Code');
  fields.TaxAmount = CreateField('Amount', 'Amount');
  return fields;
}

// Custom formatter to display delete button in grid
function buttonFmatter(cellValue, options, cellObject) {
  return "<a style='cursor:hand;' target='_parent' href='javascript:deleteRecord(" + cellValue + ");' title='Delete'><div class='deleteIcon ignoredirty'></div></a>";
}

// Custom formatter to display amount rounded to 2 decimal places in grid
function amountFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(2);
}

// Method to delete tax record
function deleteRecord(id) {
    if (confirm("Are you sure you want to delete this record?")) {
    // calculate and set total vat amount  
    var amountToBeDeducted = $('#' + taxFields.TaxAmount.Id + id).val();
    totalTaxAmount = totalTaxAmount - Number(amountToBeDeducted);

    if (!isNaN(totalTaxAmount)) {
      $('#TaxAmount').val(totalTaxAmount.toFixed(_amountDecimals));

      // Set the parent form dirty.
      $parentForm.setDirty();
    }

    // Delete record from grid
    $taxList.delRowData(id);

    //delete record entries from hidden fields
    var taxCode = '#' + taxFields.TaxCode.Id + id;
    $(taxCode).remove();

    var taxAmount = '#' + taxFields.TaxAmount.Id + id;
    $(taxAmount).remove();

    var taxId = '#' + taxFields.TaxId.Id + id;
    $(taxId).remove();
  }
  // Set focus on TaxCode control
  formTaxDetails.TaxCode.focus();
}

function addTax() {    
    //in below code we are doing trim to taxcode SCP#50425:Tax Code XT
  var taxCode = $.trim($('#TaxCode').val().toUpperCase());
  var amount = $('#Amount').val();
  
   // Change id of textboxes and append it in child tax list div
  $("#divTaxDetails").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + taxCurrent, id: currentElem.attr("id") + taxCurrent, value: currentElem.val() });
    inputControl.appendTo('#childTaxList');
  });

  // Clear values of textboxes in capture div
  $('#' + taxFields.TaxCode.Id).val("");
  $('#' + taxFields.TaxAmount.Id).val("");
  $('#' + taxFields.TaxId.Id).val("");

  // Add record in grid
  var row = { Id: taxCurrent, TaxCode: taxCode, Amount: amount };
  $taxList.addRowData(taxCurrent, row);

  taxCurrent++;

  // calculate and set total vat amount
  totalTaxAmount = Number(totalTaxAmount) + Number(amount);
  $('#TaxAmount').val(totalTaxAmount.toFixed(_amountDecimals));

  // Set the parent form dirty.
  $parentForm.setDirty();

  // Set focus on TaxCode control
  //formTaxDetails.TaxCode.focus();
  $('#TaxCode').focus();
}

// Add hidden input fields for existing tax records
function addTaxFields(taxIndex, id, taxCode, Amount) {
  var tax = $("#divTaxDetails").clone(true);
  tax.find('#' + taxFields.TaxCode.Id).val(taxCode.toUpperCase());
  tax.find('#' + taxFields.TaxAmount.Id).val(Amount);
  tax.find('#' + taxFields.TaxId.Id).val(id);
 
  // Change id of textboxes and append it in child tax list div
  tax.children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + taxIndex, id: currentElem.attr("id") + taxIndex, value: currentElem.val() });
    inputControl.appendTo('#childTaxList');
  });
}

// Close tax details modal dialogue
function closeTaxDetail() {
  if (!$isOnView)
    $validateTax.resetForm(); 
  $('#divTaxBreakdown').dialog('close');
}

// Following code is executed when user closes Tax Breakdown Popup
$("#divTaxBreakdown").bind("dialogclose", function (event, ui) {
  // Set focus on TaxAmount text box
  $('#TaxAmount').focus();
});


