/// <reference path="site.js" />
var $VatFields = InitializeVatFields();
var $validateVat;
var totalVatAmount = 0;

$(document).ready(function () {
  $('input[type=text]:not(.populated)').removeAttr('value');
  InitializeValidation();
});

function InitializeValidation(vatData) {
  totalVatAmount = $('#VatAmount').val(); //initialize totalVatAmount
  $validateVat = $("#formVatDetails").validate({
    rules: {
      VatIdentifierId: {
        selectNone: true
      },
      VatLabel: { maxlength: 50 },
      VatText: { required: true, maxlength: 50 },
      VatBaseAmount: { required: true, min: -999999999.9, max: 999999999.9 },
      VatPercentage: { min: -99.999, max: 99.999 }
    },
    messages: {
      VatIdentifierId: "VAT Identifier Required",
      VatText: { required: "VAT Text Required" },
      VatBaseAmount: { required: "VAT Base Amount Required" },
      VatPercentage: "Value should be between -99.999 and 99.999" 
    }, submitHandler: function () {
      addVat();
    }, highlight: function () { $("#errorContainer").show(); $(".serverErrorMessage").hide(); $(".serverSuccessMessage").hide(); },
    onkeyup: false
  });

  trackFormChanges('formVatDetails');

  $("#VatPercentage").blur(function () {
    //trim the whitespace
    var couponGrossValue = $('#VatBaseAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    setVATCalculatedAmount(couponGrossValue);
  }); //end blur

  $("#VatBaseAmount").blur(function () {
    //trim the whitespace
    var couponGrossValue = $('#VatBaseAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    setVATCalculatedAmount(couponGrossValue);
  }); //end blur

}

function setVATCalculatedAmount(couponGrossValue) {
  //trim white space
  var vatValue = $("#VatPercentage").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  var vatPercentage = 0.00;

  if (vatValue != '') {
    if (!isNaN(Number(vatValue))) {
      vatAmount = Number($("#VatPercentage").val());
      vatPercentage = vatAmount / 100 * couponGrossValue;
      if (!isNaN(vatAmount))
        $("#VatPercentage").val(vatAmount.toFixed(3));

      grossValue = Number(couponGrossValue);
      if (!isNaN(grossValue))
        $('#VatBaseAmount').val(grossValue.toFixed(2));
      else
        $('#VatBaseAmount').val("0.00");
    }
    if (!isNaN(vatPercentage) )
      $("#VatCalculatedAmount").val(vatPercentage.toFixed(2));
  }
  else
    $("#VatCalculatedAmount").val('');
}

function addVat() {
  $("#errorContainer").hide();
  var VATIdentifier = $('#' + $VatFields.VATIdentifier.Id).val();
  var VATIdentifierText = $('#' + $VatFields.VATIdentifier.Id + " option:selected").text();
  var vatLabel = $('#' + $VatFields.VATLabel.Id).val();
  var vatText = $('#' + $VatFields.VATText.Id).val();
  var vatBaseAmount = $('#' + $VatFields.VATBaseAmount.Id).val();
  var vatPercentage = $('#' + $VatFields.VATPercentage.Id).val();
  var vatCalculatedAmount = $('#' + $VatFields.VATCalculatedAmount.Id).val();
  var invoiceId = $('#InvoiceId').val();

  var vatObject;

  if (vatPercentage == '' && vatCalculatedAmount == '') {
    vatObject = {
      VatIdentifierId: VATIdentifier,
      VatLabel: vatLabel,
      VatText: vatText,
      VatBaseAmount: vatBaseAmount,
      VatPercentage: vatPercentage,
      VatCalculatedAmount: vatCalculatedAmount,
      ParentId: invoiceId
    };
  }
  else {
    vatObject = {
      VatIdentifierId: VATIdentifier,
      VatLabel: vatLabel,
      VatText: vatText,
      VatBaseAmount: vatBaseAmount,
      VatPercentage: vatPercentage,
      VatCalculatedAmount: vatCalculatedAmount,
      ParentId: invoiceId
    };
  }
  if (vatObject != '')
  addInvoiceVat(vatObject);

  // Clear values of textboxes in capture div
  $('#' + $VatFields.VATIdentifier.Id).val('');
  $('#' + $VatFields.VATLabel.Id).val('');
  $('#' + $VatFields.VATText.Id).val('');
  $('#' + $VatFields.VATBaseAmount.Id).val('');
  $('#' + $VatFields.VATPercentage.Id).val('');
  $('#' + $VatFields.VATCalculatedAmount.Id).val('');

  // Add record in grid
}

// Initializes vatField constant which contains values for Controls and display name for grid title
function InitializeVatFields() {
  var field = {};
  field.VATId = CreateVatField('VatId', '');
  field.VATIdentifier = CreateVatField('VatIdentifierId', 'VAT Identifier');
  field.VATLabel = CreateVatField('VatLabel', 'VAT Label');
  field.VATText = CreateVatField('VatText', 'VAT Text');
  field.VATBaseAmount = CreateVatField('VatBaseAmount', 'VAT Base Amount');
  field.VATPercentage = CreateVatField('VatPercentage', 'VAT Percentage');
  field.VATCalculatedAmount = CreateVatField('VatCalculatedAmount', 'VAT Calculated Amount');
  return field;
}

// Create object for VatField constant
function CreateVatField(id, displayName) {
  obj = [];
  obj.Id = id;
  obj.DisplayName = displayName;
  return obj;
}

function copyAvailableVat() {
  rowId = $("#AvailableVatGrid").getGridParam('selrow');
  rowNUmber = $('#InvoiceVatGrid').getGridParam('selarrrow');
  if (rowId) {
    var rowData = $("#AvailableVatGrid").getRowData(rowId);
    var invoiceId = invoiceId = $('#InvoiceId').val();
    var VatIdentifierID = $('#' + $VatFields.VATIdentifier.Id + " option:contains('" + rowData["Identifier"] + "')").val();
    var vatObject = {
      VatIdentifierId: VatIdentifierID,
      VatLabel: rowData["VatLabel"],
      VatText: rowData["VatText"],
      VatBaseAmount: rowData["VatBaseAmount"],
      VatPercentage: rowData["VatPercentage"],
      VatCalculatedAmount: rowData["VatCalculatedAmount"],
      ParentId: invoiceId
    };

    addInvoiceVat(vatObject);
  }
  else {
    alert("Please select a record from Derived VAT List");
  }

}

function addInvoiceVat(datarow) {
    //debugger;
  var jsonData = JSON.stringify(datarow);
  var invId = $('#InvoiceId').val(); //nk
  $.ajax({
    url: 'Vat',
    type: 'POST',
    data: { from: jsonData, invoiceId: invId },
    dataType: 'json',
    error: function (error) {
        var returnString = error.responseText;
        if (returnString.indexOf("Invalid characters were entered") > -1) {            
            showClientErrorMessage("Invalid characters were entered. Operation did not complete.");
        } else {
            showClientErrorMessage("Error while saving record.");
        }      
    },
    success: function (result) {
      if (result.IsFailed == false) {
        if (result.isRedirect) {
          location.href = result.RedirectUrl;
        }
        showClientSuccessMessage(result.Message);
        $("#InvoiceVatGrid").trigger("reloadGrid");
      }
      else {
        showClientErrorMessage(result.Message);
      }      
    }
  });
}

function copyUnappliedVat() {
  rowId = $("#UnappliedAmountVatGrid").getGridParam('selrow');
  rowNUmber = $('#InvoiceVatGrid').getGridParam('selarrrow');
  if (rowId) {
    var rowData = $("#UnappliedAmountVatGrid").getRowData(rowId);
    var invoiceId = invoiceId = $('#InvoiceId').val();
    var VatIdentifierID = $('#' + $VatFields.VATIdentifier.Id + " option:contains('" + rowData["VatIdentifierText"].toUpperCase() + "')").val();
    var vatObject = {
      VatIdentifierId: VatIdentifierID,
      VatLabel: ' ',
      VatText: 'VAT Not Applicable',
      VatBaseAmount: rowData["NonAppliedAmount"],
      ParentId: invoiceId
    };

    addInvoiceVat(vatObject);
  }
  else {
    alert("Please select a record from List of Amounts on which VAT Has Not Been Applied");
  }
}

function InitializeViewOnlyInvoiceVat(viewMode) {
  if (viewMode) {
    SetPageToViewModeEx(viewMode, '#formVatDetails');
    $('.primaryButton').hide();
    $('#formVatDetails').hide();
  }
}

function DisplayNullableAmountFormatter(cellValue, options, cellObject) {
  if (cellValue == '')
    return '';
  else {
    var decimalValue = Number(cellValue).toFixed(2);
    decimalValue = decimalValue + '';
    var formattedVal = addThousandSeparator(decimalValue);
    return formattedVal;
  }
}

//
function DisplayNullablePercentFormatter(cellValue, options, cellObject) {
  if (cellValue == '')
    return '';
  else {
    var decimalValue = Number(cellValue).toFixed(3);
    decimalValue = decimalValue + '';
    var formattedVal = addThousandSeparator(decimalValue);
    return formattedVal;
  }
}

//Function to add thousand separator to value
function addThousandSeparator(someNum){
  while (someNum.match(/^(.*\d)(\d{3}(\.|,|$).*$)/)){
     someNum = someNum.replace(/^(.*\d)(\d{3}(\.|,|$).*$)/, '$1,$2');
             }
   return someNum;
}

          