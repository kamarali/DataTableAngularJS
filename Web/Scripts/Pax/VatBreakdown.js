var $VatFields = InitializeVatFields();
var $VatCurrent = 1;
var $VatList; var $validateVat;
var totalVatAmount = 0;
var isRMVat = false;

//Initializes vatField constant which contains values for Controls and display name for grid title
function InitializeVatFields() {
  var field = {};
  field.VATId = CreateVatField('VatId', 'Action');
  field.VATIdentifier = CreateVatField('VatIdentifierId', 'VAT Identifier');
  field.VATLabel = CreateVatField('VatLabel', 'VAT Label');
  field.VATText = CreateVatField('VatText', 'VAT Text');
  field.VATBaseAmount = CreateVatField('VatBaseAmount', 'VAT Base Amount');
  field.VATPercentage = CreateVatField('VatPercentage', 'VAT Percentage');
  field.VATCalculatedAmount = CreateVatField('VatCalculatedAmount', 'VAT Calculated Amount');
  return field;
}

//Create object for VatField constant
function CreateVatField(id, displayName) {
  obj = [];
  obj.Id = id;
  obj.DisplayName = displayName;
  return obj;
}

function addVat() {
  var VATIdentifier = $('#' + $VatFields.VATIdentifier.Id).val();
  var VATIdentifierText = $('#' + $VatFields.VATIdentifier.Id + " option:selected").text();
  var vatLabel = $('#' + $VatFields.VATLabel.Id).val();
  var vatText = $('#' + $VatFields.VATText.Id).val();
  var vatBaseAmount = $('#' + $VatFields.VATBaseAmount.Id).val();
  var vatPercentage = $('#' + $VatFields.VATPercentage.Id).val();
  var vatCalculatedAmount = $('#' + $VatFields.VATCalculatedAmount.Id).val();
  var vatId = $('#' + $VatFields.VATId.Id).val();
  
  
  // Change id of textboxes and append it in child tax list div
  $("#divVatDetails").children("div").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $VatCurrent, id: currentElem.attr("id") + $VatCurrent, value: currentElem.val() });
    inputControl.appendTo('#childVatList');
  });

  var identifier = $('#' + $VatFields.VATId.Id).clone();
  identifier.attr("name", $VatFields.VATIdentifier.Id + $VatCurrent);
  identifier.attr("id", $VatFields.VATIdentifier.Id + $VatCurrent);
  identifier.val(VATIdentifier);
  identifier.appendTo("#childVatList");

  //Clear values of textboxes in capture div
  $('#' + $VatFields.VATIdentifier.Id).val('');
  $('#' + $VatFields.VATLabel.Id).val('');
  $('#' + $VatFields.VATText.Id).val('');
  $('#' + $VatFields.VATBaseAmount.Id).val('');
  $('#' + $VatFields.VATPercentage.Id).val('');
  $('#' + $VatFields.VATCalculatedAmount.Id).val('');

  //Add record in grid
  var row = { Id: $VatCurrent, Identifier: VATIdentifierText, VatLabel: vatLabel, VatText: vatText, VatBaseAmount: vatBaseAmount, VatPercentage: vatPercentage, VatCalculatedAmount: vatCalculatedAmount };
  $VatList.addRowData($VatCurrent, row);
  $VatCurrent++;

  //calculate and set total vat amount
  if (isRMVat == false) {
    totalVatAmount = Number(totalVatAmount) + Number(vatCalculatedAmount);
    $('#VatAmount').val(totalVatAmount.toFixed(_amountDecimals));
  }
  else {
  }

  // Set the parent form dirty.
  $parentForm.setDirty();
}

function InitializeVatGrid(vatData) {
  // If pageMode == Create/Edit, register control events and validation rules, for View mode hide Vat details panel
  if ($isOnView) {
    $("#formVatDetails").hide();
  }
  else {

    if ($('#VatBaseAmount').val() == 0.00) {
      $("#VatBaseAmount").val('');
    }

    if ($("#VatPercentage").val() == 0.00) {
      $("#VatPercentage").val('');
    }

    $validateVat = $("#formVatDetails").validate({
      rules: {
        VatIdentifierId: {
          selectNone: true
        },
        VatLabel: { required: true,
          maxlength: 5,
          alphaNumeric: true
        },
        VatText: { required: true,
          maxlength: 50
        },
        VatBaseAmount: { required: true, min: -99999999999.9, max: 99999999999.9 },
        VatPercentage: { required: true, min: -99.999, max: 99.999 }
      },
      messages: {
        VatIdentifierId: "VAT Identifier Required",
        VatLabel: {
          required: "VAT Label Required",
          maxlength: "Please enter no more than 5 characters."
        },
        VatText: {
          required: "VAT Text Required",
          maxlength: "Please enter no more than 50 characters."
        },
        VatBaseAmount: { required: "VAT Base Amount Required", min: "Value out of bounds.", max: "Value out of bounds" },
        VatPercentage: { required: "VAT Percentage Required", min: "Value should be between -99.999 and 99.999", max: "Value should be between -99.999 and 99.999" }
      }, submitHandler: function () {
        addVat();




      }, highlight: false
    });

    $("#VatPercentage").blur(function () {
      //trim the whitespace
       var couponGrossValue = $('#VatBaseAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');

      /* SCP# 391037 - Clarification required to send Reverse VAT in Cargo IS IDEC file.
      Desc: Allowing values 0 as valid input. 
      if ($("#VatPercentage").val() == 0.00) {
        $("#VatPercentage").val('');
        return;
      }*/

      setVATCalculatedAmount(couponGrossValue);
    });

    $("#VatBaseAmount").blur(function () {
        /* SCP# 391037 - Clarification required to send Reverse VAT in Cargo IS IDEC file.
        Desc: Allowing values 0 as valid input. 
        if ($('#VatBaseAmount').val() == 0.00) {
        $("#VatBaseAmount").val('');
        return;
      }*/
      //trim the whitespace
      var couponGrossValue = $('#VatBaseAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      //var intRegex = /^\d+$/;
      setVATCalculatedAmount(couponGrossValue);
    });  //end blur
  }
  totalVatAmount = $('#VatAmount').val(); //initialize totalVatAmount

  //Initialize tax grid
  $VatList = $('#vatGrid');
  $VatList.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 870,
    height: 215,
    colNames: [$VatFields.VATId.DisplayName, $VatFields.VATIdentifier.DisplayName, $VatFields.VATLabel.DisplayName, $VatFields.VATText.DisplayName, $VatFields.VATBaseAmount.DisplayName, $VatFields.VATPercentage.DisplayName, $VatFields.VATCalculatedAmount.DisplayName],
    colModel: [
                { name: 'Id', index: 'Id', sorttype: 'int', sortable: false, formatter: buttonVatDeleteFormatter, width: 30, hidden: $isOnView },
                { name: 'Identifier', index: 'Identifier', sorttype: 'int', sortable: false },
                { name: 'VatLabel', index: 'VatLabel', sortable: false },
                { name: 'VatText', index: 'VatText', sortable: false, width: 300 },
                { name: 'VatBaseAmount', index: 'VATBaseAmount', align: 'right', sortable: false, formatter: amountFormatter },
                { name: 'VatPercentage', index: 'VATPercentage', align: 'right', sortable: false, formatter: percentageFormatter },
                { name: 'VatCalculatedAmount', index: 'VatCalculatedAmount', align: 'right', sortable: false, formatter: amountFormatter }
              ]
  });

  vatData = eval(vatData);
  // Populate data in tax grid with existing tax records
  if (vatData != null) {
    var vatIdentifierText = '';
    for ($VatCurrent; $VatCurrent < vatData.length + 1; $VatCurrent++) {
      vatIdentifierText = '';
      if (vatData[$VatCurrent - 1]["Identifier"] == '' && vatData[$VatCurrent - 1]["VatIdentifierId"] != null) {
        vatIdentifierText = $('#' + $VatFields.VATIdentifier.Id + " option[value = " + vatData[$VatCurrent - 1]["VatIdentifierId"] + "]").text();
      }
      else {
        vatIdentifierText = vatData[$VatCurrent - 1]["Identifier"];
      }
      row = { Id: $VatCurrent, Identifier: vatIdentifierText, VatLabel: vatData[$VatCurrent - 1]["VatLabel"], VatText: vatData[$VatCurrent - 1]["VatText"], VatBaseAmount: vatData[$VatCurrent - 1]["VatBaseAmount"], VatPercentage: vatData[$VatCurrent - 1]["VatPercentage"], VatCalculatedAmount: vatData[$VatCurrent - 1]["VatCalculatedAmount"] };
      $VatList.jqGrid('addRowData', $VatCurrent, row);
      addVatFields($VatCurrent, vatData[$VatCurrent - 1]["Id"], vatData[$VatCurrent - 1]["VatIdentifierId"], row["VatLabel"], row["VatText"], row["VatBaseAmount"], row["VatPercentage"], row["VatCalculatedAmount"]);
    }
  }
}

//Add hidden input fields for existing tax records
function addVatFields(localUId, id, vatIdentifierId, vatLabel, vatText, vatBaseAmount, vatPercentage, vatCalculatedAmount) {
  var vat = $("#divVatDetails").clone(true);
  vat.find('#' + $VatFields.VATLabel.Id).val(vatLabel);
  vat.find('#' + $VatFields.VATText.Id).val(vatText);
  vat.find('#' + $VatFields.VATBaseAmount.Id).val(vatBaseAmount);
  vat.find('#' + $VatFields.VATPercentage.Id).val(vatPercentage);
  vat.find('#' + $VatFields.VATCalculatedAmount.Id).val(vatCalculatedAmount);
  vat.find('#' + $VatFields.VATId.Id).val(id);
   

  // Change id of textboxes and append it in child tax list div
  vat.children("div").children("div").children("input").each(function (i) {
    var currentElem = $(this);
    var inputControl = $('<input>').attr({ type: 'text', name: currentElem.attr("name") + $VatCurrent, id: currentElem.attr("id") + $VatCurrent, value: currentElem.val() });
    inputControl.appendTo('#childVatList');
  });

  var identifier = $('#' + $VatFields.VATId.Id).clone();
  identifier.attr("name", $VatFields.VATIdentifier.Id + $VatCurrent);
  identifier.attr("id", $VatFields.VATIdentifier.Id + $VatCurrent);
  identifier.val(vatIdentifierId);
  identifier.appendTo("#childVatList");

}

//Close tax details modal dialogue
function closeVatDetail() {
  if (!$isOnView)
    $validateVat.resetForm();
  $("#VatBaseAmount").val('');
  $("#VatPercentage").val('');
  $('#divVatBreakdown').dialog('close');
}

function setVATCalculatedAmount(couponGrossValue) {
  //trim white space
  var vatValue = $("#VatPercentage").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  var vatPercentage = vatValue / 100 * couponGrossValue;
  // in case of invalid entry of Base Amount or Percent, set calculated value as zero.
  if (isNaN(vatPercentage)) { vatPercentage = 0; }
  $("#VatCalculatedAmount").val(vatPercentage.toFixed(_amountDecimals));
}

// Custom formatter to display delete button in grid
function buttonVatDeleteFormatter(cellValue, options, cellObject) {
    return "<a style='cursor:hand;' target='_parent' href='javascript:deleteVatRecord(" + cellValue + ");' title='Delete'><div class='deleteIcon ignoredirty'></div></a>";
}

// Custom formatter to display amount rounded to 2 decimal places in grid
function amountFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_amountDecimals);
}

// Custom formatter to display percentage rounded to 3 decimal places in grid
function percentageFormatter(cellValue, options, cellObject) {
  return Number(cellValue).toFixed(_percentDecimals);
}

// Method to delete tax record
function deleteVatRecord(id) {
  if (confirm("Are you sure you want to delete this record?")) {
    // calculate and set total vat amount
    if (isRMVat == false) {
      var amountToBeDeducted = $('#' + $VatFields.VATCalculatedAmount.Id + id).val();
      totalVatAmount = totalVatAmount - Number(amountToBeDeducted);

      if (!isNaN(totalVatAmount)) {
        $('#VatAmount').val(totalVatAmount.toFixed(_amountDecimals));

        // Set the parent form dirty.
        $parentForm.setDirty();
      }
    }

    // Delete record from grid
    $VatList.delRowData(id);

    //delete record entries from hidden fields
    $('#' + $VatFields.VATId.Id + id).remove();
    $('#' + $VatFields.VATIdentifier.Id + id).remove();
    $('#' + $VatFields.VATLabel.Id + id).remove();
    $('#' + $VatFields.VATText.Id + id).remove();
    $('#' + $VatFields.VATBaseAmount.Id + id).remove();
    $('#' + $VatFields.VATPercentage.Id + id).remove();
    $('#' + $VatFields.VATCalculatedAmount.Id + id).remove();
  }
}

/* SCP# 391037 - Clarification required to send Reverse VAT in Cargo IS IDEC file.
Desc: Allowing values 0 as valid input. 
/* Default VAT Amount to 0 if invalid value given 
$(document).ready(function () {
   
  $('#VatBaseAmount').blur(function () {
    if ($("#VatBaseAmount").val() == 0.00) {
      $("#VatBaseAmount").val('');
      return;
    }
  });

  $('#VatPercentage').blur(function () {
    if ($("#VatPercentage").val() == 0.00) {
      $("#VatPercentage").val('');
      return;
    }
  });
});*/

function InitializeRMVatGrid(vatData) {
  InitializeVatGrid(vatData);
  isRMVat = true;
}

// Following code is executed when user closes VAT Breakdown Popup
$("#divVatBreakdown").bind("dialogclose", function (event, ui) {
  // Set focus on VatAmount text box
  $('#VatAmount').focus();
});