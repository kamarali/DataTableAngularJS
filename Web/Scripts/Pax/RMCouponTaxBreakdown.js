var $taxCurrent = 1;
var $taxList;
var $validateTax;
var taxFields = InitializeTaxFields();
var totalTaxAmountBilled = 0, totalTaxAmountAccepted = 0, totalTaxAmountDifference = 0;

function InitializeRMTaxGrid(taxData) {
  if ($isOnView) { $("#formTaxDetails").hide(); }
  else {
    // Calculate tax amount difference
    $('#CouponTaxAmountBilled').blur(function () {
      setDifference('#CouponTaxAmountBilled', '#CouponTaxAmountAccepted', '#CouponTaxAmountDifference');
      $('#CouponTaxAmountDifference').blur();
    });

    $('#CouponTaxAmountAccepted').blur(function () {
      setDifference('#CouponTaxAmountBilled', '#CouponTaxAmountAccepted', '#CouponTaxAmountDifference');
      $('#CouponTaxAmountDifference').blur();
    });

    $validateTax = $("#formTaxDetails").validate({
      rules: {
        TaxCode: "required",
        CouponTaxAmountBilled: "required",
        CouponTaxAmountAccepted: "required"
      },
      messages: {
        TaxCode: { required: "Tax Code Required" },
        CouponTaxAmountBilled: { required: "Tax Amount Billed Required" },
        CouponTaxAmountAccepted: { required: "Tax Amount Accepted Required" }

      }, submitHandler: function () {
        addTax();
      }, highlight: false
    });
  }

  // initialize total Amounts
  totalTaxAmountBilled = $('#TaxAmountBilled').val();
  totalTaxAmountAccepted = $('#TaxAmountAccepted').val();
  totalTaxAmountDifference = $('#TaxAmountDifference').val();

  // Initialize tax grid
  $taxList = $('#taxGrid');
  $taxList.jqGrid({
    datatype: 'local',
    width: 675,
    height: 250,
    colNames: [taxFields.TaxId.DisplayName, taxFields.TaxCode.DisplayName, taxFields.TaxAmountBilled.DisplayName, taxFields.TaxAmountAccepted.DisplayName, taxFields.TaxAmountDifference.DisplayName],
    colModel: [
                { name: 'Id', index: 'Id', sorttype: 'int', sortable: false, formatter: buttonFmatter, width: 30, hidden: $isOnView },
                { name: 'TaxCode', index: 'TaxCode', sortable: false },
                { name: 'Amount', index: 'Amount', sortable: false, formatter: amountFormatter },
                { name: 'AmountAccepted', index: 'AmountAccepted', sortable: false, formatter: amountFormatter },
                { name: 'AmountDifference', index: 'AmountDifference', sortable: false, formatter: amountFormatter }
              ]
  });
  
  taxData = eval(taxData);
  // Populate data in tax grid with existing tax records
  if (taxData != null) {  
    for ($taxCurrent; $taxCurrent < taxData.length + 1; $taxCurrent++) {
      row = { Id: $taxCurrent, TaxCode: taxData[$taxCurrent - 1]["TaxCode"], Amount: taxData[$taxCurrent - 1]["Amount"], AmountAccepted: taxData[$taxCurrent - 1]["AmountAccepted"], AmountDifference: taxData[$taxCurrent - 1]["AmountDifference"] };
      $taxList.jqGrid('addRowData', $taxCurrent, row);
      addTaxFields($taxCurrent, taxData[$taxCurrent - 1]["Id"], row["TaxCode"], row["Amount"], taxData[$taxCurrent - 1]["AmountAccepted"], taxData[$taxCurrent - 1]["AmountDifference"]);
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
  fields.TaxAmountBilled = CreateField('CouponTaxAmountBilled', 'Amount Billed');
  fields.TaxAmountAccepted = CreateField('CouponTaxAmountAccepted', 'Amount Accepted');
  fields.TaxAmountDifference = CreateField('CouponTaxAmountDifference', 'Amount Difference');
  return fields;
}

// Custom formatter to display delete button in grid
function buttonFmatter(cellValue, options, cellObject) {
    return "<a style='cursor:hand;' target='_parent' href='javascript:deleteTaxRecord(" + cellValue + ");'><div class='deleteIcon ignoredirty'></div></a>";
}

// Method to delete tax record
function deleteTaxRecord(id) {
  if (confirm("Are you sure you want to delete this record?")) {

    // calculate and set total vat amount  
    deleteTotalAmount(id);

    // Delete record from grid
    $taxList.delRowData(id);

    // delete record entries from hidden fields
    var taxCode = '#' + taxFields.TaxCode.Id + id;
    $(taxCode).remove();

    var taxAmountBilled = '#' + taxFields.TaxAmountBilled.Id + id;
    $(taxAmountBilled).remove();
    var taxAmountAccepted = '#' + taxFields.TaxAmountAccepted.Id + id;
    $(taxAmountAccepted).remove();
    var taxAmountDifference = '#' + taxFields.TaxAmountDifference.Id + id;
    $(taxAmountDifference).remove();

    var taxId = '#' + taxFields.TaxId.Id + id;
    $(taxId).remove();

    // Set the parent form dirty.
    $parentForm.setDirty();
  }
}

function deleteTotalAmount(id) {
  var taxAmountBilled = $('#' + taxFields.TaxAmountBilled.Id + id).val();
  totalTaxAmountBilled = Number(totalTaxAmountBilled) - Number(taxAmountBilled);
  $('#TaxAmountBilled').val(totalTaxAmountBilled.toFixed(_amountDecimals));

  var taxAmountAccepted = $('#' + taxFields.TaxAmountAccepted.Id + id).val();
  totalTaxAmountAccepted = Number(totalTaxAmountAccepted) - Number(taxAmountAccepted);
  $('#TaxAmountAccepted').val(totalTaxAmountAccepted.toFixed(_amountDecimals));

  var taxAmountDifference = $('#' + taxFields.TaxAmountDifference.Id + id).val();
  totalTaxAmountDifference = Number(totalTaxAmountDifference) - Number(taxAmountDifference);
  $('#TaxAmountDifference').val(totalTaxAmountDifference.toFixed(_amountDecimals));
}

function addTotalAmount(taxAmountBilled, taxAmountAccepted, taxAmountDifference) {
  totalTaxAmountBilled = Number(totalTaxAmountBilled) + Number(taxAmountBilled);
  $('#TaxAmountBilled').val(totalTaxAmountBilled.toFixed(_amountDecimals));

  totalTaxAmountAccepted = Number(totalTaxAmountAccepted) + Number(taxAmountAccepted);
  $('#TaxAmountAccepted').val(totalTaxAmountAccepted.toFixed(_amountDecimals));

  totalTaxAmountDifference = Number(totalTaxAmountDifference) + Number(taxAmountDifference);
  $('#TaxAmountDifference').val(totalTaxAmountDifference.toFixed(_amountDecimals));
}

function addTax() {
    //in below code we are doing trim to taxcode SCP#50425:Tax Code XT
  var taxCode = $.trim($('#' + taxFields.TaxCode.Id).val().toUpperCase());
  var taxAmountBilled = $('#' + taxFields.TaxAmountBilled.Id).val();
  var taxAmountAccepted = $('#' + taxFields.TaxAmountAccepted.Id).val();
  var taxAmountDifference = $('#' + taxFields.TaxAmountDifference.Id).val();


  // Change id of textboxes and append it in child tax list div
  $("#divTaxDetails").children("div").children("input").each(function (i) {
    var $currentElem = $(this);
  //  if ($currentElem.attr("name") != "TaxCode") {
      var inputControl = $('<input>').attr({ type: 'text', name: $currentElem.attr("name") + $taxCurrent, id: $currentElem.attr("id") + $taxCurrent, value: $currentElem.val() });
      inputControl.appendTo('#childTaxList');
  //  }
  });

//  // Appending tax code as simple input to fix error of 'TaxCode Auto complete not working after any cloned hidden field for TaxCode is removed'
//  var taxCodeInput = $('#' + taxFields.TaxId.Id).clone();
//  taxCodeInput.attr("name", taxFields.TaxCode.Id + $taxCurrent);
//  taxCodeInput.attr("id", taxFields.TaxCode.Id + $taxCurrent);
//  taxCodeInput.val(taxCode);
//  taxCodeInput.appendTo("#childTaxList");
  
  addTotalAmount(taxAmountBilled, taxAmountAccepted, taxAmountDifference);
  
  // Clear values of textboxes in capture div
  $('#' + taxFields.TaxCode.Id).val("");
  $('#' + taxFields.TaxAmountBilled.Id).val("");
  $('#' + taxFields.TaxAmountAccepted.Id).val("");
  $('#' + taxFields.TaxAmountDifference.Id).val("");
  $('#' + taxFields.TaxId.Id).val("");

  // Add record in grid
  var row = { Id: $taxCurrent, TaxCode: taxCode, Amount: taxAmountBilled, AmountAccepted: taxAmountAccepted, AmountDifference: taxAmountDifference };
  $taxList.addRowData($taxCurrent, row);

  // Set the parent form dirty.
  $parentForm.setDirty();

  $taxCurrent++;
}

// Add hidden input fields for existing tax records
function addTaxFields(taxCurrent, id, taxCode, AmountBilled, AmountAccepted, AmountDifference) {
  var tax = $("#divTaxDetails").clone(true);
  tax.find('#' + taxFields.TaxAmountBilled.Id).val(AmountBilled);
  tax.find('#' + taxFields.TaxAmountAccepted.Id).val(AmountAccepted);
  tax.find('#' + taxFields.TaxAmountDifference.Id).val(AmountDifference);
  tax.find('#' + taxFields.TaxId.Id).val(id);
  tax.find('#' + taxFields.TaxCode.Id).val(taxCode.toUpperCase());

  // Change id of textboxes and append it in child tax list div
  tax.children("div").children("input").each(function (i) {    
    var currentElem = $(this);   
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + taxCurrent, id: currentElem.attr("id") + taxCurrent, value: currentElem.val() });
    inputControl.appendTo('#childTaxList');   
  });  
}

// Close tax details modal dialogue
function closeTaxDetail() {
  if (!$isOnView)
    $validateTax.resetForm();
  $('#divTaxBreakdown').dialog('close');
}

var rmStage = 1;
function SetStage(rejMemoStage) {
  rmStage = rejMemoStage;
}

// Calculate billed and accepted amount difference
function setDifference(billedId, acceptedId, targetControl) {
  var billedAmt = $(billedId).val();
  var acceptedAmt = $(acceptedId).val();
  var amtDifference;
  if (rmStage == 2) {
    amtDifference = acceptedAmt - billedAmt;
  }
  else
    amtDifference = billedAmt - acceptedAmt;
  if (!isNaN(amtDifference))
    $(targetControl).val(amtDifference.toFixed(_amountDecimals));
}

// Custom formatter to display amount rounded to 2 decimal places in grid
function amountFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_amountDecimals);
}

// Following code is executed when user closes Tax Breakdown Popup
$("#divTaxBreakdown").bind("dialogclose", function (event, ui) {
  // Set focus on TaxAmountDifference text box
  $('#TaxAmountDifference').focus();
});