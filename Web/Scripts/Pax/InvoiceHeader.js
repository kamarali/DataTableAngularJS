/// <reference path="../jquery-1.5.1.min.js" />
/// <reference path="site.js" />
_amountDecimals = 2;
_percentDecimals = 3;

billingCategoryType = 1;
var billingMemberId;
var selectedBilledMemberId;
var GetLocationIdMethod = '/Data/GetBilledMemberLocationList';
var GetExchangeRateMethod = '/Data/GetExchangeRate';
var GetDefaultSettlementMethod = '/Data/GetDefaultSettlementMethod';
var GetDefaultCurrencyMethod = '/Data/GetDefaultCurrency';
//CMP #553: ACH Requirement for Multiple Currency Handling
var IsBillingAndBilledAchOrDualMemberUrl;
var bilateralSettlementMethodId = 3; // Defined in SettlementMethod enum. B
var IsCreditNote = 0;
var exchangeRateDefaultValue = '0.00000';
var exchangeRateOneValue = '1.00000';
var bilateralSMIList;
var adjustmentDueToProtestSettlementMethodId = 4;
var SmiX = "8";
var SmiA = "2";
var SmiM = "5";
var isAchOrDualMember = false;

function InitializeBilateralSMIs(bilateralSmi, bilateralSmiList) {
    // Form an array from the comma-separated list.
    bilateralSMIList = bilateralSmiList.split(',');
    bilateralSettlementMethodId = bilateralSmi;
}

function InitialiseInvoiceHeader(locationIdMethod, exchangeRateMethod, viewMode, settlementMethod, defaultCurrencyMethod, memberId, formId, IsBillingAndBilledAchOrDualMemberMethod, selectedBillingCurrencyId) {

  GetLocationIdMethod = locationIdMethod;
  GetExchangeRateMethod = exchangeRateMethod;
  GetDefaultSettlementMethod = settlementMethod;
  GetDefaultCurrencyMethod = defaultCurrencyMethod;
  IsBillingAndBilledAchOrDualMemberUrl = IsBillingAndBilledAchOrDualMemberMethod;
  billingMemberId = memberId;
  var smi = $('#SettlementMethodId').val();
  if ($.trim(smi) != '' && jQuery.inArray(smi, bilateralSMIList) > -1) {
    CloneCurrencyDropDown();
  }

  //CMP #553: ACH Requirement for Multiple Currency Handling
  if ($.trim(smi) != '' && (smi == SmiA || smi == SmiM) && IsBillingAndBilledAchOrDualMemberUrl != undefined) {
      CloneCurrencyDropDown();
      if (selectedBillingCurrencyId != null && selectedBillingCurrencyId != "")
          $('#BillingCurrencyId').val(selectedBillingCurrencyId);
  }


    //CMP #624: ICH Rewrite-New SMI X 
    //Description: Code put here to disable ChAgreementIndicator and ChDueDate fields in cases when SMI is other than X
    $("#ChAgreementIndicator").attr("readonly", "readonly");
    $("#ChDueDate").watermark("DD-MMM-YY");
    $("#ChDueDate").attr("readonly", "readonly");
    $("#ChDueDate").datepicker('disable');

    if ($.trim(smi) != '' && jQuery.inArray(smi, bilateralSMIList) > -1 && smi == SmiX) {
        //For Invoices/Credit Notes captured using SMI X: New field ‘CH Agreement Indicator’ is allowed
        //So making the field blank and then allowing user input.        
        $("#ChAgreementIndicator").attr("readonly", false);
        
        //For Invoices/Credit Notes captured using SMI X: New field ‘CH Due Date’ is allowed
        //So making the field blank and then allowing user input.
        $("#ChDueDate").attr("readonly", false);
        $("#ChDueDate").datepicker("enable");
    }

  if (viewMode) {

    // todo: remove call to SetPageToViewMode method when implement SetPageToViewModeEx in all pages.
    if (formId && formId.length > 0)
      SetPageToViewModeEx(viewMode, formId);
    else
      SetPageToViewMode(viewMode);

    $("#SaveInvoiceHeader").hide();
    $("#Transactions").val('View Transactions');
    $("#BilledMemberRefData").val('Billing Member\'s Reference Data');
    $("#BillingMemberRefData").val('Billed Member\'s Reference Data');
    var divActionOnInvoice = $('#divTransactions');
    $('input[type=submit]', divActionOnInvoice).show();
    $('input[type=submit]', divActionOnInvoice).attr('disabled', false);
    DisplayViewModeButtons();
  }
  else {
      $("#InvoiceForm").validate({
          rules: {
              InvoiceNumber: { required: true, ValidInvoiceNumber: true },
              ProvisionalBillingMonth: "required", //This field is checked only for form DE validation
              BilledMemberText: "required",
              SettlementMethodId: "required",
              InvoiceDate: "required",
              BillingYearMonthPeriod: "required",
              BilledMemberText: { required: true, checkMemberEquality: true },
              ListingCurrencyId: "required",
              BillingCurrencyId: { required: true, validationBillingCurrForAchOrM: true }, 
              FormDEProvisionalBillingMonth: { required: function () { isDropdownValueRequired($('#FormDEProvisionalBillingMonth')) } },
              ListingToBillingRate: { min: 0.00001, maxlength: 17 },
              ChAgreementIndicator: {
                  required: function (element) {
                      //CMP #624: ICH Rewrite-New SMI X 
                      //Description: Client side validations on new field of ‘CH Agreement Indicator’ 
                      //FRS Section 2.12 PAX/CGO IS-WEB Screens (Part 1) 
                      //Change #3: Related to new field of ‘CH Agreement Indicator’

                      var SelectedSmi = $('#SettlementMethodId').val();
                      if (SelectedSmi == SmiX) {
                          //For Invoices/Credit Notes captured using SMI “X”: New field ‘CH Agreement Indicator’ is mandatory
                          return true;
                      }
                      else if (SelectedSmi != SmiX) {
                          //For Invoices/Credit Notes captured using SMIs I, A or M and Bilateral/behave like bilateral : New field ‘CH Agreement Indicator’ is NOT allowed
                          return false;
                      }
                  }
              }
          },
          messages: {
              InvoiceNumber: GetInvoiceNumberMessage(),
              ProvisionalBillingMonth: "Provisional Billing Month Required",
              BilledMemberText: "Billed Member Required",
              SettlementMethodId: "Settlement Method Required",
              InvoiceDate: GetInvoiceDateMessage(),
              BillingYearMonthPeriod: "Billing Year/Month/Period Required",
              ListingCurrencyId: "Currency of Listing/Evaluation Required",
              BillingCurrencyId: { required: "Currency of Billing Required", validationBillingCurrForAchOrM: "Currency of Listing/Evaluation should be same as Currency of Billing" },
              FormDEProvisionalBillingMonth: "Provisional Billing Month Required",
              ListingToBillingRate: "Exchange Rate cannot be zero or negative or exceed max length.",
              ChAgreementIndicator: "CH Agreement Indicator is mandatory for Settlement Method X."
          },
          submitHandler: function (form) {
              $('#SettlementMethodId').removeAttr('disabled');
              $('#ListingCurrencyId').removeAttr('disabled');
              $('#BillingCurrencyId').removeAttr('disabled');
              $("#FormDEProvisionalBillingMonth").removeAttr('disabled');

              onSubmitHandler(form);
          }
      });

    trackFormChanges('InvoiceForm');

    $("#ChAgreementIndicator").blur(function () {
      var chAgrInd = $("#ChAgreementIndicator").val();
      chAgrInd = $.trim(chAgrInd);
      $("#ChAgreementIndicator").val(chAgrInd);
    });

    $("#ListingToBillingRate").blur(function () {
      var exchnRate = $.trim($("#ListingToBillingRate").val());
      if (exchnRate.length <= 17 && exchnRate != '' && exchnRate != 'NaN') {
        var exchnRate5 = parseFloat(exchnRate).toFixed(5);
        $("#ListingToBillingRate").val(exchnRate5);
      }
      if ($.trim(exchnRate) == '') {
        $("#ListingToBillingRate").val(exchangeRateDefaultValue);
      }
    }); 

    $("#ListingCurrencyId").bind('change', function () {
      GetExchangeRate($("#ListingCurrencyId").val(), $("#BillingCurrencyId").val());
    });

    $("#BillingCurrencyId").bind('change', function () {
      GetExchangeRate($("#ListingCurrencyId").val(), $("#BillingCurrencyId").val());
    });

    $("#BillingYearMonthPeriod").bind('change', function () {
      GetExchangeRate($("#ListingCurrencyId").val(), $("#BillingCurrencyId").val());
    });

    $("#SettlementMethodId").bind("change", function () {

        //For Invoices/Credit Notes captured using a Bilateral SMI (other than X): New field ‘CH Agreement Indicator’ is NOT allowed
        //So making the field blank and then prevent user input.
        $("#ChAgreementIndicator").attr("readonly", "readonly");

        //For Invoices/Credit Notes captured using a Bilateral SMI (other than X): New field ‘CH Due Date’ is NOT allowed
        $("#ChDueDate").watermark("DD-MMM-YY");
        $("#ChDueDate").attr("readonly", "readonly");
        $("#ChDueDate").datepicker('disable');
        //So making the field blank and then prevent user input.

        CloneCurrencyDropDown();
        PopulateDefaultCurrency();        

        //alert("in SMI Change Drop down Handler - 2");

        //alert($('#SettlementMethodId').val());

        if (jQuery.inArray($('#SettlementMethodId').val(), bilateralSMIList) == -1) { // SMI is not to be treated as Bilateral.
            GetExchangeRate($("#ListingCurrencyId").val(), $("#BillingCurrencyId").val());
        }

        if (jQuery.inArray($('#SettlementMethodId').val(), bilateralSMIList) > -1) {
            $('#ListingToBillingRate').val(exchangeRateDefaultValue);
        }

        //CMP #624: ICH Rewrite-New SMI X 
        //Description: Client side validations on new field of ‘CH Agreement Indicator’ 
        //FRS Section 2.12 PAX/CGO IS-WEB Screens (Part 1) 
        //Change #3 : Related to new field of ‘CH Agreement Indicator’ and
        //Change #4 : Related to new field of ‘CH Due Date’

        //Enum Constant - JS mapping of Cs Enum SettlementMethodValues

        if ($('#SettlementMethodId').val() == SmiX) {
            /* Both newly added fields open up, by default on SMI change */
            $("#ChAgreementIndicator").attr("readonly", false);
            $("#ChDueDate").attr("readonly", false);
            $("#ChDueDate").datepicker("enable");
        }
        if ($('#SettlementMethodId').val() != SmiX) {
            /* When newly changed SMI value is other than X blank out the fields. */
            $("#ChAgreementIndicator").val("");
            $("#ChDueDate").val("");
        }
    });

    $('#DigitalSignatureRequiredId').change(function () {
      if ($('#DigitalSignatureRequiredId').val() == "2" && $('#DigitalSignatureFlagId').val() == "1") {
        if (!confirm("Do you want to change the Digital Signature option?")) {
          $('#DigitalSignatureRequiredId').val($('#DigitalSignatureFlagId').val());
        }
      }
    });

    // Initialize the custom validators.
    validationBilledAndBillingMember();
    validationBillingCurrencyForAchOrM()
  }

  function isDropdownValueRequired(element) {
    if ($(element).attr('disabled') == false) {
      return true;
    }
    else
      return false;
  }

  function validationBilledAndBillingMember() {
    jQuery.validator.addMethod('checkMemberEquality',
      function (value, element) {
        if (element.value == $("#BillingMemberText").val()) {
          return false;
        }
        else return true;
      },
    "Billed Member should not be same as Billing member.");
  }
  validationInvoiceNumber();
}

function validationInvoiceNumber() {
  jQuery.validator.addMethod('ValidInvoiceNumber',
      function (value, element) {

        var regEx = '^[a-zA-Z0-9]+$'; // without underscore        
        var re = new RegExp(regEx);
        if (!element.value.match(re)) {          
          return false;
        }
        else { return true; }
      },
    "Invoice Number invalid.");
}

function GetExchangeRate(listingCurrency, billingCurrency) {
  var smi = $('#SettlementMethodId').val();
  //if both the curr are same then update the value as 1
  if (listingCurrency == billingCurrency) {
    $("#ListingToBillingRate").val(exchangeRateOneValue);
  }
if ($.trim(smi) != '' && jQuery.inArray(smi, bilateralSMIList) == -1) {
    if (listingCurrency != "" && billingCurrency != "") {
      $.ajax({
        type: "POST",
        url: GetExchangeRateMethod,
        data: { listingCurrencyId: listingCurrency, billingCurrencyId: billingCurrency, billingPeriod: $("#BillingYearMonthPeriod").val() },
        dataType: "json",
        success: function (response) {
          if (response.Message) {
              showClientErrorMessage(response.Message);
              /* SCP# 305858 - FW: 350-YB Tax Rejections Which Failed In SIS Validation 
              Desc: Update Total Amount in Currency of Billing.
              Exchange rate was not getting reset to 0 because of incorrect controller name. 
              old code = $("#ListingtoBillingRate").val(exchangeRateDefaultValue);
              Becasue of this header was getting saved with incorrect exchange rate. */
              $("#ListingToBillingRate").val(exchangeRateDefaultValue);
          }
          else {
            clearMessageContainer();
            $("#ListingToBillingRate").val(response);
          }
        },
        failure: function (response) {
          $("#ListingToBillingRate").val(exchangeRateDefaultValue);
        }
      });
    }
    else {
      $("#ListingToBillingRate").val(exchangeRateDefaultValue);
    }
  }
  else {      
    if (jQuery.inArray(smi, bilateralSMIList) == -1)
        $("#ListingToBillingRate").val(exchangeRateDefaultValue);
  }
}

function SetControlAccess() {
  $("#BilledMemberText").attr("readonly", "readonly");
  $("#SettlementMethodId").attr("disabled", "disabled");
  $("#InvoiceDate").attr("readonly", "readonly");
  $("#ListingCurrencyId").attr("disabled", "disabled");
  $("#BillingCurrencyId").attr("disabled", "disabled");
}

function BilledMemberText_SetAutocompleteDisplay(item) {
  var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "-" + item.CommercialName + "";
  return { label: memberCode, value: memberCode, id: item.Id };
}

function BilledMemberText_AutoCompleteValueChange(selectedMemberId, smi) {
  selectedBilledMemberId = selectedMemberId;
  if (selectedMemberId == $("#BillingMemberId").val()) {
    showClientErrorMessage("Billing Member and Billed Member should not be same");
    $("#BilledMemberId").val("");
    $("#BilledMemberText").val("");
    return false;
  }
  else {
    clearMessageContainer();
  }
  $.ajax({
    type: "POST",
    url: GetLocationIdMethod,
    data: { billedMemberId: selectedMemberId },
    dataType: "json",
    success: OnLocationListPopulated,
    failure: function (response) {
      $("#BilledMemberId").val("");
      $("#BilledMemberText").val("");
    }
  });
  
  var billingMemberValue = $("#BillingMemberId").val();
  // CMP #624: ICH Rewrite-New SMI X 
  // Description: Refer FRS Section 2.14 Change #9
  // Code added to pre-populate SMI drop down with value X.
  if (smi != null && smi == SmiX) {
      $("#SettlementMethodId").val(SmiX);
      $("#SettlementMethodId").attr("disabled", "disabled");
      CloneCurrencyDropDown();
      PopulateDefaultCurrency();      
  }
  else
  {
    $.ajax({
      type: "POST",
      url: GetDefaultSettlementMethod,
      data: { billedMemberId: selectedMemberId, billingMemberId: billingMemberValue, billingCategoryId: billingCategoryType },
      dataType: "json",
      success: function (response) {
        if (response != null) {
          $("#SettlementMethodId").val(response);
          //CMP#624 : Changes done to resolved issue TFS#9248
          smi = $("#SettlementMethodId").val();
          if (smi == SmiX) {
            $("#ChAgreementIndicator").attr("readonly", false);
            $("#ChDueDate").attr("readonly", false);
            $("#ChDueDate").datepicker("enable");
          }
          else {
            $("#ChAgreementIndicator").val("");
            $("#ChDueDate").val("");
            $("#ChAgreementIndicator").attr("readonly", true);
            $("#ChDueDate").watermark("DD-MMM-YY");
            $("#ChDueDate").attr("readonly", "readonly");
            $("#ChDueDate").datepicker('disable');
          }
          CloneCurrencyDropDown();
          PopulateDefaultCurrency();
        }
      },
      failure: function (response) {
        $("#BilledMemberId").val("");
        $("#BilledMemberText").val("");
      }
    });
  }
}

function OnLocationListPopulated(response) {
  if (response.length > 0) {
      $("#BilledMemberLocationCode").empty(); //CMP496;Populate dropdown without blank .append('<option selected="selected" value=""></option>');
      $("#MemberLocationInformation_1__MemberLocationCode"); //CMP496;Populate dropdown without blank .empty().append('<option selected="selected" value=""></option>');
    var locationCodeText;
    for (i = 0; i < response.length; i++) {      
      if (response[i].CurrencyCode != '') {
        locationCodeText = response[i].LocationCode + "-" + response[i].CityName + "-" + response[i].CountryId + "-" + response[i].CurrencyCode;
      }
      else {
        locationCodeText = response[i].LocationCode + "-" + response[i].CityName + "-" + response[i].CountryId;
      }

      if (response[i].LocationCode == "Main") {
        $("#BilledMemberLocationCode").append($("<option selected='selected'></option>").val(response[i].LocationCode).html(locationCodeText));
        $("#MemberLocationInformation_1__MemberLocationCode").append($("<option selected='selected'></option>").val(response[i].LocationCode).html(locationCodeText));
        $("#BilledMemberLocationCode").change();
      }
      else {
        $("#BilledMemberLocationCode").append($("<option></option>").val(response[i].LocationCode).html(locationCodeText));
        $("#MemberLocationInformation_1__MemberLocationCode").append($("<option></option>").val(response[i].LocationCode).html(locationCodeText));
        $("#BilledMemberLocationCode").change();
      }
    };
  }
  else {
      $("#BilledMemberLocationCode").empty(); //CMP496;Populate dropdown without blank .append('<option selected="selected" value="">Please Select<option>');
    $("#MemberLocationInformation_1__MemberLocationCode").empty();
  }
}

function setInvoiceHeaderFocus(isFromBillingHistory) {
    if (isFromBillingHistory == "True") {
        //SCP#450827 - Rejections Process
        //Set focus on Invoice Number if Invoice created from Billing History Screen.
        $('#InvoiceNumber').focus();
    }
    else {
        //SCP#450827 - Rejections Process
        //Set focus on Billed Member if Invoice created from Recievable Screen.
        $('#BilledMemberText').focus();
    }
}
function DisplayViewModeButtons() {
  var divViewModeButtons = $('.viewModeDisplayButtons');
  $('input[type=submit]', divViewModeButtons).show();
  $('input[type=submit]', divViewModeButtons).attr('disabled', false);

}

function PopulateDefaultCurrency() {

    //alert("in PopulateDefaultCurrency");
    var smi = $('#SettlementMethodId').val();


    if (selectedBilledMemberId == undefined) {
        var billedMemberId = $("#BilledMemberId").val();
        selectedBilledMemberId = billedMemberId;
    }


    if (smi != null && $.trim(smi) != '' && smi != SmiX && jQuery.inArray(smi, bilateralSMIList) == -1 && selectedBilledMemberId != undefined) {

    //alert("Making Ajax Call to Get Exchange Rate");

    $.ajax({
      type: "GET",
      url: GetDefaultCurrencyMethod,
      data: { settlementMethodId: smi, billingMemberId: billingMemberId, billedMemberId: selectedBilledMemberId },
      dataType: "json",
      success: function (response) {
        if (response) {
          // Set Clearance Currency to default currency.
          if (response == -1)
            $('#BillingCurrencyId').val("");
          else
            $('#BillingCurrencyId').val(response);
          GetExchangeRate($("#ListingCurrencyId").val(), $("#BillingCurrencyId").val());
        }

      },
      beforeSend: function (xmlRequest) {
      }
    });
  }
}

function CloneCurrencyDropDown() {

    //alert("in CloneCurrencyDropDown");
    $('#BillingCurrencyId').empty();

    //alert($('#SettlementMethodId').val());

    if (jQuery.inArray($('#SettlementMethodId').val(), bilateralSMIList) > -1 || ($('#SettlementMethodId').val() == adjustmentDueToProtestSettlementMethodId)) {
        /* SMI is Bilateral or Behave Like Bilateral, in this case all currencies will appear as currency of billing */
        //alert("IF");
        $('#ListingCurrencyId option').clone().appendTo('#BillingCurrencyId');
        $('#BillingCurrencyId').val($('#HiddenBillingCurrencyId').val());
        $('#ListingToBillingRate').removeAttr('readonly');
    }
    else {

        if (IsBillingAndBilledAchOrDualMemberUrl != undefined) {
            //CMP #553: ACH Requirement for Multiple Currency Handling
            var smi = $('#SettlementMethodId').val();
            if (smi == SmiA) {
                $('#HiddenAchBillingCurrency option').clone().appendTo('#BillingCurrencyId');
            }
            else {
                if (smi == SmiM) {
                    $.ajax({
                        type: "GET",
                        url: IsBillingAndBilledAchOrDualMemberUrl,
                        data: { billingMemberId: billingMemberId, billedMemberId: $("#BilledMemberId").val() },
                        async: false,
                        dataType: "json",
                        success: function (response) {
                            if (response) {
                                isAchOrDualMember = true;
                                $('#HiddenAchBillingCurrency option').clone().appendTo('#BillingCurrencyId');
                            }
                            else {
                                isAchOrDualMember = false;
                                /* SMI is Non bilateral, in this case only selective zonal currencies will appear as currency of billing */
                                //alert("ELSE");
                                $('#HiddenBillingCurrency option').clone().appendTo('#BillingCurrencyId');
                                $("#ListingToBillingRate").attr("readonly", "readonly");
                            }

                        }
                    });
                }
                else {
                    /* SMI is Non bilateral, in this case only selective zonal currencies will appear as currency of billing */
                    //alert("ELSE");
                    $('#HiddenBillingCurrency option').clone().appendTo('#BillingCurrencyId');
                    $("#ListingToBillingRate").attr("readonly", "readonly");
                }

            }
        }
        else {
            /* SMI is Non bilateral, in this case only selective zonal currencies will appear as currency of billing */
            //alert("ELSE");
            $('#HiddenBillingCurrency option').clone().appendTo('#BillingCurrencyId');
            $("#ListingToBillingRate").attr("readonly", "readonly");
        }
    }
}

function SetInvoiceType(isCreditNote) {
  IsCreditNote = isCreditNote;  
}

function GetInvoiceNumberMessage() {
  if (IsCreditNote) {
    return "Credit Note Number required and should be valid.";
  }
  else {
    return "Invoice Number required and should be valid.";
  }
}

function GetInvoiceDateMessage() {
  if (IsCreditNote) {
    return "Credit Note Date Required";
  }
  else {
    return "Invoice Date Required";
  }
}

// Following function is used to display popup for SourceCode vat total 
function showSourceCodeVatDialog(iurl, id) { 
  $.ajax({
    type: "POST",
    url: iurl,
    data: { sourceCodeId: id },
    success: function (response) {
      $("#AvailableVatGrid").jqGrid('clearGridData');
      for (var i = 0; i <= response.length; i++)
        $("#AvailableVatGrid").jqGrid('addRowData', i + 1, response[i]);

      $("#divAvailableVatGridResult").dialog({
        autoOpen: true,
        title: 'Source Code VAT Total',
        height: 270,
        width: 650,
        modal: true,
        resizable: false
      });
    },
    error: function (xhr, textStatus, errorThrown) {
      alert('An error occurred! ' + errorThrown);
    }
});
//TFS#10005 :Firefox: v45: Page number is overlapped in Source Code VAT.
$("#AvailableVatGrid_pager_center").width(297);
  return false;
}
