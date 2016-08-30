/// <reference path="../jquery-1.5.1.min.js" />
/// <reference path="../site.js" />

var GetLocationIdMethod = '/Data/GetMiscUatpBilledMemberLocationList';
var GetExchangeRateMethod = '/Data/GetExchangeRate';
var GetDefaultSettlementMethod = '/Data/GetDefaultSettlementMethod';
var GetMemberLocationDetails = '/Data/GetLocationDetails';
var GetRejectionInvoiceDetailsMethod = '/Misc/MiscInvoice/GetRejectionInvoiceDetails';
var GetCorrespondenceInvoiceDetailsMethod = '/Misc/MiscInvoice/GetCorrespondenceInvoiceDetails';
var GetDefaultCurrencyMethod = '/Data/GetDefaultCurrency';
var GetRejectionStageForSmi = '/Data/GetRejectionStageForSmi';
//CMP #553: ACH Requirement for Multiple Currency Handling
var IsBillingAndBilledAchOrDualMemberUrl;
var isRejectionOutsideTimeLimitMethod;
var bilateralSettlementMethodId = 3; // Defined in SettlementMethod enum. B
var adjustmentDueToProtestSettlementMethodId = 4; // Defined in SettlementMethod enum. R
var smiI = 1;// Defined in SettlementMethod enum. ICH
var smiA = 2;// Defined in SettlementMethod enum. ACH
var smiM = 5; // Defined in SettlementMethod enum. MITA
var smiX = 8; // Defined in SettlementMethod enum. X
var maxNoteOccurrence = 10;
var maxAddDetailOccurrence = 20;
var billingMemberId;
var selectedBilledMemberId;
var currentYear;
var currentMonth;
var billingCategoryId;
var rejectionBillingPeriod;
var creditNoteInvoiceTypeId = 2;
var IsCreditNote = 1;
_amountDecimals = 3;
var isOnEditInvoice = false;
//var isFrombillingHistory = false;

var enableExchangeRate;
var currentBilledIn;
var currentBillingCurrency;
var invoiceId;
var isUpdateOperation = false;
var bilateralSMIList;
var isAchOrDualMember = false;

function InitializeBilateralSMIs(bilateralSmi, bilateralSmiList) {

  // Form an array from the comma-separated list.
  bilateralSMIList = bilateralSmiList.split(',');
  bilateralSettlementMethodId = bilateralSmi;
}

function BilledMemberText_AutoCompleteValueChange(selectedMemberId,smi, callFromCreatePage) {
    selectedBilledMemberId = selectedMemberId;
  
  if ($('#IsCreatedFromBillingHistory').val() != "True") {
    $.ajax({
      type: "POST",
      url: GetLocationIdMethod,
      data: { billedMemberId: selectedMemberId, billingCategory: billingCategoryId },
      dataType: "json",
      success: function (response) {
        OnLocationListPopulated(response);
        PopulatedBilledMemberLocationDetails(); // populate the billed member location details.
      },
      failure: function (response) {
        $("#BilledMemberId").val("");
        $("#BilledMemberText").val("");
      }
    });
  }

  var billingMemberValue = $("#BillingMemberId").val();
//SIT:9585,9592  
if (jQuery.inArray(smi, bilateralSMIList) > -1)
      return;
  if (smi != smiX) {
      $.ajax({
        type: "POST",
        url: GetDefaultSettlementMethod,
        data: { billedMemberId: selectedMemberId, billingMemberId: billingMemberValue, billingCategoryId: billingCategoryId },
        dataType: "json",
        success: function (response) {

          if (response != null) {
            $('#SettlementMethodId').val(response);
            TogglePaymentDetailsDisplay();
            PopulateRejectionInvDetails();
            PopulateCorrespondenceInvDetails();
            CloneCurrencyDropDown();
            SetClearanceCurrency();
            InitializeFieldsBilateral();
            PopulateDefaultCurrency();
            //CMP#624 : Changes done to resolved issue TFS#9248 
            PopulateSmiXfields();
          }
        },
        failure: function (response) {
          $("#BilledMemberId").val("");
          $("#BilledMemberText").val("");
        }
      });
  }
}

function EnableBilledLocationLink() {
  if ($('#BilledMemberText').val() == '') {
    //disable link
    $('#BilledMemberLocLink').attr('disabled', true);
    $('#BilledMemberLocLink').attr('title', 'Please enter Billed Member first.');
  }
  else {
    //enable link.    
    $('#BilledMemberLocLink').attr('disabled', false);
    $('#BilledMemberLocLink').attr('title', '');
  }
}

function OnLocationListPopulated(response) {
    if (response && response.BilledLocations && response.BilledLocations.length > 0) {
        if (billingCategoryId == 4) {
            $("#BilledMemberLocationCode").empty().append('<option selected="selected" value=""></option>');
            $("#MemberLocationInformation_1__MemberLocationCode").empty().append('<option selected="selected" value=""></option>');
        }
        else {
            $("#BilledMemberLocationCode").empty(); //CMP496;Populate dropdown without blank .append('<option selected="selected" value=""></option>');
            $("#MemberLocationInformation_1__MemberLocationCode").empty(); //CMP496;Populate dropdown without blank .append('<option selected="selected" value=""></option>');
         }
    var locationCodeText;
    for (i = 0; i < response.BilledLocations.length; i++) {
      if (response.BilledLocations[i].CurrencyCode != '') {
        locationCodeText = response.BilledLocations[i].LocationCode + "-" + response.BilledLocations[i].CityName + "-" + response.BilledLocations[i].CountryId + "-" + response.BilledLocations[i].CurrencyCode;
      }
      else {
        locationCodeText = response.BilledLocations[i].LocationCode + "-" + response.BilledLocations[i].CityName + "-" + response.BilledLocations[i].CountryId;
      }
      if (response.BilledLocations[i].LocationCode == response.DefaultLocation) {
        $("#BilledMemberLocationCode").append($("<option selected='selected'></option>").val(response.BilledLocations[i].LocationCode).html(locationCodeText));
        $("#MemberLocationInformation_1__MemberLocationCode").append($("<option selected='selected'></option>").val(response.BilledLocations[i].LocationCode).html(locationCodeText));
      }
      else {
        $("#BilledMemberLocationCode").append($("<option></option>").val(response.BilledLocations[i].LocationCode).html(locationCodeText));
        $("#MemberLocationInformation_1__MemberLocationCode").append($("<option></option>").val(response.BilledLocations[i].LocationCode).html(locationCodeText));
      }
    };
  }
  else {
      if (billingCategoryId == 4) {
          $("#BilledMemberLocationCode").empty().append('<option selected="selected" value="">Please Select<option>');
      }
      else {
          $("#BilledMemberLocationCode").empty(); //CMP496;Populate dropdown without blank .append('<option selected="selected" value="">Please Select<option>');
      }
       $("#MemberLocationInformation_1__MemberLocationCode").empty();
  }
}

function InitializeCurrentYearMonth(month, year) {
  currentMonth = month;
  currentYear = year;
}

function InitiliazeCorrespondenceFields(invoiceIdentifier, isUpdate) {
  invoiceId = invoiceIdentifier;
  isUpdateOperation = isUpdate;
}

function InitialiseInvoiceHeader(locationIdMethod, exchangeRateMethod, rejectionDetailsMethod, correspondenceDetails, viewMode, settlementMethod, locationDetailsMethod, memberId, billingCategory, defaultCurrencyMethod, rejectionStageForSmi, isRejOutsideTLMethod, IsBillingAndBilledAchOrDualMemberMethod, selectedBillingCurrencyId) {
    if (!IsCreditNote) {
        SetAuthorityToBillRadioButtons();
    }

    billingMemberId = memberId;
    billingCategoryId = billingCategory;
    GetLocationIdMethod = locationIdMethod;
    GetExchangeRateMethod = exchangeRateMethod;
    GetDefaultSettlementMethod = settlementMethod;
    GetRejectionInvoiceDetailsMethod = rejectionDetailsMethod;
    GetCorrespondenceInvoiceDetailsMethod = correspondenceDetails;
    GetDefaultCurrencyMethod = defaultCurrencyMethod;
    GetRejectionStageForSmi = rejectionStageForSmi;
    GetMemberLocationDetails = locationDetailsMethod;
    isRejectionOutsideTimeLimitMethod = isRejOutsideTLMethod;
    IsBillingAndBilledAchOrDualMemberUrl = IsBillingAndBilledAchOrDualMemberMethod;
    GetPaymentDetails();
    CloneCurrencyDropDown();
    SetClearanceCurrency();
    InitializeFieldsBilateral();
    EnableBilledLocationLink();
    EnableDisableSMiDropdown();

    var smi = $('#SettlementMethodId').val();

    //CMP #553: ACH Requirement for Multiple Currency Handling
    if ($.trim(smi) != '' && (smi == smiA || smi == smiM) && IsBillingAndBilledAchOrDualMemberUrl != undefined) {
        if (selectedBillingCurrencyId != null && selectedBillingCurrencyId != "")
            $('#BillingCurrencyId').val(selectedBillingCurrencyId);
            CalculateNetTotal();
    }


    if ($('#radCpdAbtr').is(':checked') && isOnEditInvoice == true && $isOnView == false) {
        PopulateCorrespondenceInvDetails();
    }

    if ($('#radRejInvoice').is(':checked') && isOnEditInvoice == true && $isOnView == false) {
        PopulateRejectionInvDetails();
    }

    var smi = $('#SettlementMethodId').val();


    $('#PaymentDetail_DiscountPercent').watermark('0.00  ');
    if (IsRadioChecked("#radRejInvoice"))
        OnRejectionInvoiceClick();
    else if (IsRadioChecked("#radCpdInvoice"))
        OnCorrespondenceInvoiceClick();
    if ($isOnView) {
        SetPageToViewMode(viewMode);
        $("#SaveInvoiceHeader").hide();
    }
    else {

        if (IsRadioChecked("#radRejInvoice") && !$('#RejectionStage').prop('readonly') && $('#RejectionStage').val() == 2) {
            $('#ListingToBillingRate').removeAttr('readonly');
        }

        if (IsRadioChecked("#radCpdInvoice") && $("#IsValidationFlag").val() != '') {
            $('#ListingToBillingRate').removeAttr('readonly');
        }

        SetBillingHistoryCorrespondenceInvoice();
        validationInvoiceNumber();
        validationBillingCurrencyForAchOrM();
        $("#InvoiceForm").validate({
            rules: {
                InvoiceNumber: { required: true, ValidInvoiceNumber: true },
                BilledMemberText: "required",
                SettlementMethodId: "required",
                InvoiceDate: "required",
                BillingYearMonthPeriod: "required",
                BilledMemberText: "required",
                BillingMemberLocationCode: "required",
                LocationCode: { required: function (element) {
                    if ($('#LocationCode').prop("readonly") != true && $('#LocationCode').val() == "" && $('#IsLineItemExistHid').val() == 'true')
                    { return true; }
                    else
                    { return false; }
                }
                },
                ListingCurrencyId: { required: function (element) {
                    if ($('#IsCreatedFromBillingHistory').val() == "True") {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                },
                ListingToBillingRate: { min: function (element) {
                    var selectedSMI = $("#SettlementMethodId").val();
                    if (jQuery.inArray($("#SettlementMethodId").val(), bilateralSMIList) > -1) {
                        return 0.00000;
                    }
                    else {
                        return 0.00001;
                    }
                }, maxlength: 17
                },
                RejectedInvoiceNumber: {
                    required: function (element) {
                        return IsRejInvoiceRequired();
                    }
                },

                BilledIn: {
                    required: function (element) {
                        return IsRadioChecked("#radRejInvoice");
                    }
                },
                SettlementPeriod: {
                    required: function (element) {
                        return IsRadioChecked("#radRejInvoice");
                    }
                },
                RejectionStage: {
                    required: function (element) {
                        return IsRadioChecked("#radRejInvoice");
                    }
                },

                CorrespondenceRefNo: {
                    required: function (element) {
                        return IsRadioChecked("#radCpdInvoice");
                    }
                },
                ChargeCategoryId: { required: function (element) {
                    if ($('#IsCreatedFromBillingHistory').val() == "True") {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                },
                BillingCurrencyId: { required: function (element) {
                    var selectedSMI = $("#SettlementMethodId").val();
                    if (jQuery.inArray(selectedSMI, bilateralSMIList) > -1  || $('#IsCreatedFromBillingHistory').val() == "True") {
                        return false;
                    }
                    else {
                        return true;
                    }
                },
                validationBillingCurrForAchOrM: true 
                },
                ChAgreementIndicator: {
                    required: function (element) {
                        //CMP #624: ICH Rewrite-New SMI X 
                        //Description: Client side validations on new field of ‘CH Agreement Indicator’ 
                        //FRS Section 2.12 PAX/CGO IS-WEB Screens (Part 1) 
                        //Change #3: Related to new field of ‘CH Agreement Indicator’

                        //Enum Constant - JS mapping of Cs Enum SettlementMethodValues
                        var SmiI = "1";
                        var SmiA = "2";
                        var SmiM = "5";
                        var SmiX = "8";

                        var SelectedSmi = $('#SettlementMethodId').val();
                        if (SelectedSmi == smiX) {
                            //For Invoices/Credit Notes captured using SMI “X”: New field ‘CH Agreement Indicator’ is mandatory
                            return true;
                        }
                        else if (SelectedSmi != smiX) {
                            //For Invoices/Credit Notes captured using a Bilateral SMI (other than X): New field ‘CH Agreement Indicator’ is NOT allowed
                            return false;
                        }
                    }
                }
            },

            messages: {
                InvoiceNumber: GetInvoiceNumberMessage(),
                BilledMemberText: "Billed Member Required",
                LocationCode: "Location Code Required",
                SettlementMethodId: "Settlement Method Required",
                InvoiceDate: GetInvoiceDateMessage(),
                BillingMemberLocationCode: "This is mandatory field. If no Location IDs are shown here, it means that you are not associated with any Location of your organization; and you will not be able to proceed with Invoice/Credit Note capture. Please contact your organization’s user(s) who have access to the Location Association module to review and associate you with appropriate Location(s).",
                BillingYearMonthPeriod: "Billing Year/Month/Period Required",
                ListingCurrencyId: "Currency of Billing Required",
                RejectedInvoiceNumber: "Rejection Invoice Number Required",
                BilledIn: "Billed In Year-Month Required",
                SettlementPeriod: "Settlement Period Required",
                RejectionStage: "Invalid Rejection Stage",
                CorrespondenceRefNo: "Correspondence Ref Number Required",
                CorrespondenceRejInvoiceNo: "Rejection Invoice Number Required",
                ChargeCategoryId: "Charge Category Required",
                BillingCurrencyId: { required: "Currency of Clearance Required", validationBillingCurrForAchOrM: "Currency of Billing should be same as Currency of Clearance" },
                ListingToBillingRate: "Exchange Rate cannot be zero or negative or exceed max length.",
                ChAgreementIndicator: "CH Agreement Indicator is mandatory for Settlement Method X."
            },
            invalidHandler: function () { $.watermark.showAll(); },
            submitHandler: function (form) {
                $('#BilledIn').removeAttr('disabled');
                $('#RejectedInvoiceNumber').removeAttr('disabled');
                $('#SettlementPeriod').removeAttr('disabled');
                $('#RejectionStage').removeAttr('disabled');
                $('#radCpdAbtr').removeAttr('disabled');
                $('#radCpdExpired').removeAttr('disabled');

                $('#ChargeCategoryId').removeAttr('disabled');
                $('#radOrgInvoice').removeAttr('disabled');
                $('#radRejInvoice').removeAttr('disabled');
                $('#radCpdInvoice').removeAttr('disabled');
                $('#InvoiceDate').removeAttr('disabled');
                $('#SaveInvoiceHeader').attr('disabled', true);
                $('#AddLineItem').attr('disabled', true);
                $('#UploadAttachment').attr('disabled', true);

                //For correspondence invoice created from billing history
                $('#CorrespondenceRejInvoiceNo').removeAttr('disabled');
                $('#CorrespondenceRefNo').removeAttr('disabled');
                $('#radCpdInvoice').removeAttr('disabled');
                $('#InvoiceSummary_TotalAmount').removeAttr('disabled');
                $('#SettlementMethodId').removeAttr('disabled');
                $('#InvoiceSummary_TotalAmount').removeAttr('disabled');
                $('#SettlementPeriod').removeAttr('disabled');
                $('#ListingCurrencyId').removeAttr('disabled');
                $('#BillingCurrencyId').removeAttr('disabled');

                warnUserForOutsideTimeLimitRejection(form);
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
            //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
            currentSMI = $("#SettlementMethodId").val();

            if (exchnRate.length <= 17 && exchnRate != '' && exchnRate != 'NaN') {
                var exchnRate5 = parseFloat(exchnRate).toFixed(5);
                $("#ListingToBillingRate").val(exchnRate5);
                CalculateNetTotal();
            }

            if ($.trim(exchnRate) == '' || isNaN(exchnRate)) {
                if (jQuery.inArray(currentSMI, bilateralSMIList) > -1) {
                    $("#ListingToBillingRate").val("");
                    $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
                }
                else {
                    $("#ListingToBillingRate").val("0.00000");
                }
            }

        });

        $("#SaveInvoiceHeader").click(function () {
            if ($('#IsCreatedFromBillingHistory').val() == "False") {
                CalculateNetTotal();
            }
        });

        $("#TaxAmount, #VatAmount, #TotalAddChargeAmount").blur(function () {
            CalculateNetTotal();
        });

        $("#Remarks").bind("keypress", function () { maxLength(this, 80) });
        $("#Remarks").bind("paste", function () { maxLengthPaste(this, 80) });

        $("#txtNoteDesc").bind("keypress", function () { maxLength(this, 500) });
        $("#txtNoteDesc").bind("paste", function () { maxLengthPaste(this, 500) });

        $("#AdditionalDetailDescription").bind("keypress", function () { maxLength(this, 80) });
        $("#AdditionalDetailDescription").bind("paste", function () { maxLengthPaste(this, 80) });

        $('#BillingMemberLocationCode').change(function () {
            GetPaymentDetails();
        });
        //CMP#502 : 
        $("#RejectionReasonCodeText").bind("change", onBlankRejectionReasonCode);

        $('#RejectionStage').change(function () {
            OnRejectionStageChange();
        });
        // CMP#624:2.17 : Change 17 : Disallow change of SMI from X to another value
        $('#radOrgInvoice').change(function () {
            $('#ListingCurrencyId').removeAttr('disabled');
            EnableDisableSMiDropdown();
        });
        $('#radCpdInvoice').change(function () {
            $('#ListingCurrencyId').removeAttr('disabled');
            EnableDisableSMiDropdown();
        });
        $('#radRejInvoice').change(function () {
            EnableDisableSMiDropdown();
        });

        $('#DigitalSignatureRequiredId').change(function () {
            if ($('#DigitalSignatureRequiredId').val() == "2" && $('#DigitalSignatureFlagId').val() == "1") {
                if (!confirm("Do you want to change the Digital Signature option?")) {
                    $('#DigitalSignatureRequiredId').val($('#DigitalSignatureFlagId').val());
                }
            }
        });
    }
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

//SCP#450827 - Rejections Process
//Set focus on Invoice Number if Invoice created from Billing History Screen.
//Set focus on Billed Member if Invoice created from Recievable Screen.
function setInvoiceHeaderFocus(isFromBillingHistory) {
    if (isFromBillingHistory == "True") {
        $('#InvoiceNumber').focus();
    }
    else {
        $('#BilledMemberText').focus();
    }
}

function InitializeExhangeRateField(invoiceType) {
  if (invoiceType == creditNoteInvoiceTypeId) {
      $("#ListingToBillingRate").attr('readonly', false);
    $('#ListingToBillingRate').bind("blur", UpdateExchangeRateForCreditNote);
  }
  else {
    $("#ListingToBillingRate").attr('readonly', true);
}
}

function UpdateExchangeRateForCreditNote() {
  var exchangeRate = $.trim($('#ListingToBillingRate').val());
  //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
  currentSMI = $("#SettlementMethodId").val();

  if ($.trim(exchangeRate) == '') {
      if (jQuery.inArray(currentSMI, bilateralSMIList) > -1 ) {
          $("#ListingToBillingRate").val("");
          $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
      }
      else {
          $("#ListingToBillingRate").val("0.00000");
      }
  }
  
  /*
  if (exchangeRate == '') {
    $('#ListingToBillingRate').val('0.00000');
  }
  if (isNaN(exchangeRate)) {
    $('#ListingToBillingRate').val('0.00000');
  }
  */
  CalculateNetTotal();
}

function InitializeFieldsBilateral() {
  var valueToCompare;
  if ($isOnView == true)
    valueToCompare = $("#SettlementMethodIdForView").val();
  else
    valueToCompare = $("#SettlementMethodId").val();
  /*
  if (jQuery.inArray(valueToCompare, bilateralSMIList) > -1){ // SMI is to be treated as Bilateral.
    $("#divExchangeRate").hide();
    $("#divClearanceCurrency").hide();
    $('#divTotalAmountInClearanceCurrency').hide();
  }
  else {
    $("#divExchangeRate").show();
    $("#divClearanceCurrency").show();
    $('#divTotalAmountInClearanceCurrency').show();
  }*/
}

function SetAuthorityToBillRadioButtons() {
  if ($('#IsAuthorityToBill').val().toLowerCase() == "true") {
    $('#radCpdAbtr').attr('checked', 'checked');
    $('#radCpdExpired').removeAttr('checked');
  }
  else {
    $('#radCpdExpired').attr('checked', 'checked');
    $('#radCpdAbtr').removeAttr('checked');
  }
}

function SetBillingHistoryCorrespondenceInvoice() {
  if ($('#IsCreatedFromBillingHistory').val() == "True") {
    $('#CorrespondenceRejInvoiceNo').attr('disabled', true);
    $('#RejectedInvoiceNumber').attr('disabled', true);
    $('#CorrespondenceRefNo').attr('disabled', true);
    $('#radOrgInvoice').attr('disabled', true);
    $('#radRejInvoice').attr('disabled', true);
    $('#BilledMemberText').attr('disabled', true);
    $('#ChargeCategoryId').attr('disabled', true);
    //$('#ListingCurrencyId').attr('disabled', true);

    $('#radCpdExpired').attr('disabled', true);
    $('#radCpdAbtr').attr('disabled', true);

    $('#radCpdInvoice').attr('disabled', true);
  }
}

function OnOriginalInvoiceClick() {
  $("#divRejectionInvoiceInfo").hide();
  $("#divCorrespondenceInvoiceInfo").hide();
  ClearRejectionInvoiceData();
  ClearCorrespondenceInvoiceData();
  PopulateExchangeRate();
}

function GetPaymentDetails() {
  var locationCode = $('#BillingMemberLocationCode').val();
  if ($isOnView == true) {
    locationCode = locationCode.split('-')[0];
  }
  
  $.ajax({
    type: "POST",
    url: GetMemberLocationDetails,
    data: { locationId: locationCode, memberId: billingMemberId },
    dataType: "json",
    success: function (response) {
      if (response) {
        setPaymentDetailFields(response);
      }
    }
  });
}

function CallRejectInvoice(invoiceId, lineItemIds, searchType, actionUrl) {
    $.ajax({
        type: "POST",
        url: actionUrl,
        data: { lineItemId: lineItemIds, searchType: searchType },
        dataType: "json",
        success: function (response) {
            if (response.isRedirect) {
                location.href = response.RedirectUrl;
            }
            else {
                if (response.Message) {
                    showClientErrorMessage(response.Message);
                }
            }

        }
    });
}

function setPaymentDetailFields(locationDetails) {
  if (locationDetails.BankName)
    $('#BankName').val(locationDetails.BankName);
  else
    $('#BankName').val('');

  if (locationDetails.Iban)
    $('#Iban').val(locationDetails.Iban);
  else
    $('#Iban').val('');

  if (locationDetails.Swift)
    $('#Swift').val(locationDetails.Swift);
  else
    $('#Swift').val('');

  if (locationDetails.BankCode)
    $('#BankCode').val(locationDetails.BankCode);
  else
    $('#BankCode').val('');

  if (locationDetails.BranchCode)
    $('#BranchCode').val(locationDetails.BranchCode);
  else
    $('#BranchCode').val('');

  if (locationDetails.BankAccountNumber)
    $('#BankAccountNumber').val(locationDetails.BankAccountNumber);
  else
    $('#BankAccountNumber').val('');

  if (locationDetails.BankAccountName)
    $('#BankAccountName').val(locationDetails.BankAccountName);
  else
    $('#BankAccountName').val('');

  if (locationDetails.Currency && locationDetails.Currency.Code)
    $('#Currency_Code').val(locationDetails.Currency.Code);
  else
    $('#Currency_Code').val('');
}

function IsRadioChecked(id) {
  var value = $(id).prop('checked');
  if (value) {
    return true;
  } else {
    return false;
  }
}

function IsRejInvoiceRequired() {
  var value = $("#radRejInvoice").prop('checked') || $("#radCpdInvoice").prop('checked');
  if (value) {
    return true;
  } else {
    return false;
  }
}

function GetExchangeRate(listingCurrency, billingCurrency, billingPeriod) {
    var currentSMI = $("#SettlementMethodId").val();
  if ((typeof ($varView) == 'undefined') || ($varView != 'View')) {
    if (listingCurrency != "" && billingCurrency != "" && billingPeriod != "") {
        $.ajax({
            type: "POST",
            url: GetExchangeRateMethod,
            data: { listingCurrencyId: listingCurrency, billingCurrencyId: billingCurrency, billingPeriod: billingPeriod },
            dataType: "json",
            success: function (response) {

                if (response.Message) {
                    showClientErrorMessage(response.Message);
                    //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
                    var exchnRate = $.trim($("#ListingToBillingRate").val());
                    if (exchnRate == 'undefined' || $.trim(exchnRate) == '' || isNaN(exchnRate)) {
                        if (jQuery.inArray(currentSMI, bilateralSMIList) > -1) {
                            $("#ListingToBillingRate").val("");
                            $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
                        }
                        else {
                            $("#ListingToBillingRate").val("0.00000");
                        }
                    }
                }
                else {
                    $('#clientErrorMessageContainer').hide();
                    $("#ListingToBillingRate").val(response);
                    // Recalculate the net total.
                    CalculateNetTotal();
                }
            },
            failure: function (response) {
                //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
                var exchnRate = $.trim($("#ListingToBillingRate").val());
                if ($.trim(exchnRate) == '') {
                    if (jQuery.inArray(currentSMI, bilateralSMIList) > -1) {
                        $("#ListingToBillingRate").val("");
                        $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
                    }
                    else {
                        $("#ListingToBillingRate").val("0.00000");
                    }
                }
            }
        });
    }
    else {
        //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
        currentSMI = $("#SettlementMethodId").val();
        var billingCurrencyValue = $('#BillingCurrencyId').val();
        var exchangeRate = $.trim($('#ListingToBillingRate').val());
        if ($.trim(exchangeRate) == '') {
            if (jQuery.inArray(currentSMI, bilateralSMIList) > -1) {
                $("#ListingToBillingRate").val("");
                $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
            }
            else {
                $("#ListingToBillingRate").val("0.00000");
            }
        }
        else if (billingCurrencyValue == "") {
            $("#ListingToBillingRate").val("");
            $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
        }
    }
  }
}

var noteUrl = '/Data/GetAdditionalDetails?q=&extraparam1=2';
var addDetailType = 2;
var addDetailLevel = 1;
var addNoteclick = 0; var AddNoteArr = [];
function AppendAddNoteTemplate() {

  var divTemplate = $("#NoteTemplate").clone(true);
  divTemplate.id = "NoteTemplate" + ++addNoteclick;
  divTemplate[0].children[0].id = "AddNote" + addNoteclick;
  divTemplate[0].children[0].children[0].children[1].id = "NoteDropdown" + addNoteclick;
  divTemplate[0].children[0].children[1].children[1].id = "txtNoteDesc" + addNoteclick;
  divTemplate[0].children[0].children[1].children[2].id = "RemoveNote" + addNoteclick;

  AddNoteArr.push(addNoteclick);

  if (AddNoteArr.length == 1)
    $($(divTemplate).html()).insertAfter("#MainNote");

  else {
    var id = AddNoteArr[AddNoteArr.length - 2];
    $($(divTemplate).html()).insertAfter("#AddNote" + id);

    if (AddNoteArr.length == (maxNoteOccurrence - 1))
      $("#AddNote").hide();
  }

  registerAutocomplete("NoteDropdown" + addNoteclick, null, noteUrl, 0, false, null, addDetailType, addDetailLevel, null);

  $('#' + divTemplate.id).attr('name', "NoteTemplate" + addNoteclick);
  $('#' + divTemplate[0].children[0].id).attr('name', "AddNote" + addNoteclick);
  $('#' + divTemplate[0].children[0].children[0].children[1].id).attr('name', "NoteDropdown" + addNoteclick);
  $('#' + divTemplate[0].children[0].children[1].children[1].id).attr('name', "txtNoteDesc" + addNoteclick);

  $('#' + divTemplate[0].children[0].children[1].children[1].id).bind("keypress", function () { maxLength(this, 500) });
  $('#' + divTemplate[0].children[0].children[1].children[1].id).bind("paste", function () { maxLengthPaste(this, 500) });

  $('#' + divTemplate[0].children[0].children[1].children[2].id).attr('name', "RemoveNote" + addNoteclick);

  $("#RemoveNote" + addNoteclick).bind("click", RemoveCurrentNote);
}

function RemoveCurrentNote(e) {
  var id = this.id.substring(10);
  var i;
  for (i = 0; i < AddNoteArr.length; i++) {
    if (AddNoteArr[i] == id) {
      AddNoteArr.splice(i, 1);
      break;
    }
  }

  $(this.parentNode.parentNode).remove();
  $("#AddNote").show();
}

function BindEventOnCreateInvoice() {
    
 
  $("#AddNote").bind("click", AppendAddNoteTemplate);
  $("#radOrgInvoice").bind("click", OnOriginalInvoiceClick);
  $("#radRejInvoice").bind("click", OnRejectionInvoiceClick);
  $("#radCpdInvoice").bind("click", OnCorrespondenceInvoiceClick);

  $("#SettlementMethodId").bind("change", TogglePaymentDetailsDisplay);
  $("#SettlementMethodId").bind("change", CloneCurrencyDropDown);
  $("#SettlementMethodId").bind("change", TogglePaymentDetailsDisplay);

  $("#SettlementMethodId").bind("change", InitializeFieldsBilateral);
 
  $('#RejectedInvoiceNumber').bind("blur", PopulateRejectionInvDetails);
  $('#CorrespondenceRefNo').bind("blur", PopulateCorrespondenceInvDetails);
  $('#radCpdAbtr').bind("click", SetIsAuthorityToBill);
  $('#radCpdExpired').bind("click", SetIsAuthorityToBill);
  // Listing currency is the billing currency in case of miscellaneous invoice.
  // Billing Currency is the clearance currency. 
  $('#ListingCurrencyId').bind("change", SetClearanceCurrency);
  $('#SettlementMethodId').bind("change", SetClearanceCurrency);

  $('#SettlementMethodId').bind("change", GetCorrDetailsAfterSmiChanged);
 
  $("#ListingCurrencyId, #BillingCurrencyId, #BilledIn, #RejectionStage").bind('change', PopulateExchangeRate);

  $("#BillingYearMonthPeriod").bind('change', OnBillingMonthChange);
  //CMP#648: Clearance Information in MISC Invoice PDFs
  //Desc: Hide due to inconsistancy function of fields 
  //$("#SettlementMethodId").bind("change", PopulateExchangeRate);
  //$("#SettlementMethodId").bind("change", PopulateDefaultCurrency);
  $("#BilledMemberText").bind("change", EnableBilledLocationLink);
  $('#SettlementMethodId').bind("change", PopulateSmiXfields);
  $('#SettlementMethodId').bind("change", ResetCCurrencyAndExchangeRatefields);

  CalculateNetTotal();
}

function OnBillingMonthChange() {
  // Done to populate the exchange rate as per original month.
  if ($('#radCpdAbtr').is(':checked') && isOnEditInvoice == true) {
    PopulateCorrespondenceInvDetails();
  }
  else {
    PopulateExchangeRate();
  }
}

//CMP#648: Clearance Information in MISC Invoice PDFs
function ResetCCurrencyAndExchangeRatefields() {
    $("#HiddenBillingCurrencyId").val("");
    $('#BillingCurrencyId').val("");

    var selectedSMI = 0;
    if ($isOnView == true)
        selectedSMI = $("#SettlementMethodIdForView").val();
    else
        selectedSMI = $("#SettlementMethodId").val();

    var requiredindBC = $("#RequiredindBC");
    var requiredindEX = $("#RequiredindEX");
    if (jQuery.inArray(selectedSMI, bilateralSMIList) > -1) {
        $("#ListingToBillingRate").val('');
        $("#ListingToBillingRate").attr('readonly', false);
        $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
        requiredindBC.hide();
        requiredindEX.hide();
    }
    else {
        requiredindBC.show();
        requiredindEX.show();
        $("#ListingToBillingRate").val('0.00000'); 
    }
}

function SetClearanceCurrency() {
    if (jQuery.inArray($("#SettlementMethodId").val(), bilateralSMIList) > -1) { // SMI is to be treated as Bilateral.
        $('#BillingCurrencyId').val($('#ListingCurrencyId').val());

    }
    if (jQuery.inArray($("#SettlementMethodId").val(), bilateralSMIList) > -1) { // SMI is to be treated as Bilateral.
        $('#BillingCurrencyId').val($('#HiddenBillingCurrencyId').val());

    }
}

function SetIsAuthorityToBill() {

  if ($('#radCpdAbtr').is(':checked')) {
    $('#IsAuthorityToBill').val(true);
  }
  else
    $('#IsAuthorityToBill').val(false);
}

function TogglePaymentDetailsDisplay() {
  var smi;
  if ($isOnView) {
    // In view mode, fetch the value of Settlement Method Id from 'SettlementMethodIdForView' instead of 'SettlementMethodId' control 
    // because it contains text not actual id.
    smi = $('#SettlementMethodIdForView').val();
  }
  else {
    smi = $('#SettlementMethodId').val();
  }
  if (smi == bilateralSettlementMethodId) {
    $(".payment").show();
  }
  else
    $(".payment").hide();
}

//Display Rejection stage 1 and 2 only for ACH, for ICH, BIlateral and ACH using IATA rules display rejection stage 1 only.
function PopulateRejectionStageValues() {
    var smi = $('#SettlementMethodId').val();
    if ($.trim(smi) != '') {
        $.ajax({
            type: "Post",
            url: GetRejectionStageForSmi,
            data: { settlementMethod: smi },
            dataType: "json",
            success: function (response) {
                if (response) {
                    PopulateRejectionStageData(response);
                }
            }
        });
    }
}

//Populate rejection stages
function PopulateRejectionStageData(response) {
    if (response.length > 0) {

        $("#RejectionStage").empty();

        //Add option label for dropdown
        $("#RejectionStage").append($("<option></option>").val('').html('Please Select'));
        for (i = 0; i < response.length; i++) {
            $("#RejectionStage").append($("<option ></option>").val(response[i].Value).html(response[i].Text));
        };

    }
}

function PopulateExchangeRate() {
  var billingPeriod;
  var smi = $('#SettlementMethodId').val();

  if (IsRadioChecked("#radRejInvoice")) {
    if (currentBilledIn != null)
      billingPeriod = currentBilledIn;
    else {
      if ($("#RejectionStage").val() == "2" && !$("#ListingToBillingRate").prop("readonly")) {
        billingPeriod = $("#BillingYearMonthPeriod").val();
      }
      else
        billingPeriod = $("#BilledIn").val();
    }
  }
  else {
    billingPeriod = $("#BillingYearMonthPeriod").val();
  }

  if (IsRadioChecked("#radCpdInvoice")) {
    if (currentBilledIn != null)
      billingPeriod = currentBilledIn;
    // Creation of Correspondence invoice.
    else if ($('#SettlementYear').val() != 0) {
      billingPeriod = $('#SettlementYear').val() + '-' + $('#SettlementMonth').val() + '-' + $('#SettlementPeriod').val();
    }
    else {
      
      // If created from billing history and stage 1 rejection, original invoice not found.
      if ($('#IsCreatedFromBillingHistory').val() == "True") {
        $('#IsValidationFlag').val('EX');
        $('#ListingCurrencyId').attr('disabled', false);
        $('#ListingToBillingRate').removeAttr('readonly');
      }

      billingPeriod = $("#BillingYearMonthPeriod").val();
    }
}
//SIT:9608
if ($isOnView == false) {
    //SIT:9591
    if (smi == smiA || smi == smiI || smi == smiM || smi == adjustmentDueToProtestSettlementMethodId) {
        GetExchangeRate($("#ListingCurrencyId").val(), $("#BillingCurrencyId").val(), billingPeriod);
    }
    else {
        if ($("#ListingCurrencyId").val() == $("#BillingCurrencyId").val()) {
            $("#ListingToBillingRate").val("1.00000");
        }
        else {
            //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
            if (jQuery.inArray(smi, bilateralSMIList) > -1) {
                $("#ListingToBillingRate").val("");
                $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
            }
            else {
                $("#ListingToBillingRate").val("0.00000");
            }
        }
    }
}
}

function OnAddNoteClick() {
  AppendAddNoteTemplate();
}

function OnAddDetailClick() {
  AppendAddDetailTemplate();
}

function OnCorrespondenceInvoiceClick() {
  // Rejection Invoice Numbers (for both correspondence and rejection invoices) map to the same property in model.
  // And hence have the same name. Hence change name of the other rejection invoice number field. (Names are used while validating form.)
  $('#RejectedInvoiceNumber').attr('name', 'RejInvNo'); // dummy name
  $('#CorrespondenceRejInvoiceNo').attr('name', 'RejectedInvoiceNumber');
  $("#divCorrespondenceInvoiceInfo").show();
  $("#divRejectionInvoiceInfo").hide();
  ClearRejectionInvoiceDataForCorrespondence();
//  PopulateExchangeRate();
}

function ClearCorrespondenceInvoiceData() {
  if ($('#IsCreatedFromBillingHistory').val() == "False") {
    $('#CorrespondenceRefNo').val("");
    $('#radCpdInvoice').prop('checked', false);
    $('#CorrespondenceRejInvoiceNo').val("");
  }
}

function ClearRejectionInvoiceData() {
  
  $('#RejectedInvoiceNumber').val("");
  $('#BilledIn').val("");
  $('#RejectionStage').val("");
  $('#SettlementPeriod').val("");
  $('#SettlementMonth').val("");
  $('#SettlementYear').val("");
  if ($isOnView == false) {
    $('#BilledIn').attr('disabled', false);
    $('#SettlementPeriod').attr('disabled', false);
    $('#RejectionStage').attr('disabled', false);
  }
}

function ClearRejectionInvoiceDataForCorrespondence() {
  
  $('#RejectedInvoiceNumber').val("");
  $('#BilledIn').val("");
  $('#RejectionStage').val("");
  
  if ($isOnView == false) {
    $('#BilledIn').attr('disabled', false);
    $('#SettlementPeriod').attr('disabled', false);
    $('#RejectionStage').attr('disabled', false);
  }
}

function OnRejectionInvoiceClick() {
  $('#CorrespondenceRejInvoiceNo').attr('name', 'RejInvNo'); // dummy name
  $('#RejectedInvoiceNumber').attr('name', 'RejectedInvoiceNumber');
  $("#divRejectionInvoiceInfo").show();
  $("#divCorrespondenceInvoiceInfo").hide();
  if ($isOnView == false) {
    $('#BilledIn').attr('disabled', false);
    $('#SettlementPeriod').attr('disabled', false);
    $('#RejectionStage').attr('disabled', false);
  }

  ClearCorrespondenceInvoiceData();
}

function CalculateNetTotal() {
//  if ($('#IsCreatedFromBillingHistory').val() == "False") {
    CalculateNetTotalWithParams("#InvoiceSummary_TotalAmount", '#InvoiceSummary_TotalAmountInClearanceCurrency', "#TaxAmount", "#VatAmount", "#TotalAddChargeAmount", "#ListingToBillingRate", "#InvoiceSummary_TotalLineItemAmount");
//  }
}

function CalculateNetTotalWithParams(NetTotalInBillingCurControl, NetTotalInClearanceCurControl, taxAmountControl, vatAmountControl, addonChargeControl, listingToBilllingRateControl, lineTotalControl) {
  
  var tax;
  var vat;
  var addonCharge;
  var exchangeRate;
  var totalInBillingCurrency;
  var totalInClearanceCurrency;
  // Line total of all the line items in Invoice.
  var lineTotal;

  if (!isNaN(Number($(taxAmountControl).val())))
    tax = Number($(taxAmountControl).val());
  else
    tax = 0;

  if (!isNaN(Number($(vatAmountControl).val())))
    vat = Number($(vatAmountControl).val());
  else
    vat = 0;

  if (!isNaN(Number($(addonChargeControl).val())))
    addonCharge = Number($(addonChargeControl).val());
  else
    addonCharge = 0;

  if (!isNaN(Number($(listingToBilllingRateControl).val())))
    exchangeRate = Number($(listingToBilllingRateControl).val());
  else
    exchangeRate = 1.0;

  if (!isNaN(Number($(lineTotalControl).val())))
    lineTotal = Number($(lineTotalControl).val());
  else
      lineTotal = 0;
  
  var totalInBillingCurrency = lineTotal + tax + vat + addonCharge;
  if ($('#radCpdAbtr').is(':checked') && totalInBillingCurrency == 0 && $(NetTotalInBillingCurControl).val() != 0)
    totalInBillingCurrency =  Number($(NetTotalInBillingCurControl).val());
  var totalInClearanceCurrency = 0;
  
  var billingCurrencyValue = $('#BillingCurrencyId').val();
  if (exchangeRate != 0 && billingCurrencyValue != '') {
   
      totalInClearanceCurrency = totalInBillingCurrency / exchangeRate;
  }

  
  if (!isNaN(totalInBillingCurrency))
    $(NetTotalInBillingCurControl).val(totalInBillingCurrency.toFixed(_amountDecimals));

  if (billingCurrencyValue == '') {
        $(NetTotalInClearanceCurControl).val('');
    }
    else if (!isNaN(totalInClearanceCurrency))
        $(NetTotalInClearanceCurControl).val(totalInClearanceCurrency.toFixed(_amountDecimals));
    

   //CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.
    var exchnRate = $.trim($("#ListingToBillingRate").val());
    var smi = $("#SettlementMethodId").val();
    if (exchnRate == '' || exchnRate == 'NaN') {
 
        if (jQuery.inArray(smi, bilateralSMIList) > -1) {
            $("#ListingToBillingRate").val('');
            $(NetTotalInClearanceCurControl).val('');
        }
    }

}

function CloneCurrencyDropDown() {
    var billingCurrencyValue = $('#BillingCurrencyId').val();
    $('#BillingCurrencyId').empty();
    
    var valueToCompare = $("#SettlementMethodId").val();
    //CMP#648: Clone currencies.
    if (jQuery.inArray(valueToCompare, bilateralSMIList) > -1 || valueToCompare == adjustmentDueToProtestSettlementMethodId || valueToCompare == smiX) {// SMI is to be treated as Bilateral.
        $('#ListingCurrencyId option').clone().appendTo('#BillingCurrencyId');
        if (billingCurrencyValue == null || billingCurrencyValue == '')
            $('#BillingCurrencyId').val("");
        else
            $('#BillingCurrencyId').val(billingCurrencyValue);
    }
    else {

        //CMP #553: ACH Requirement for Multiple Currency Handling
        var smi = $('#SettlementMethodId').val();
        if (smi == smiA) {
            //Remove Please select option from list due to maintain existing behavior.
            //TFS#9945:IE:Version 11- Intermediate issue "Exchange rate for given currencies is not available". 
            $('#HiddenAchBillingCurrency Option[value=""]').remove(); 
            $('#HiddenAchBillingCurrency option').clone().appendTo('#BillingCurrencyId');
        }
        else {
            if (smi == smiM) {
                $.ajax({
                    type: "GET",
                    url: IsBillingAndBilledAchOrDualMemberUrl,
                    data: { billingMemberId: billingMemberId, billedMemberId: $("#BilledMemberId").val() },
                    dataType: "json",
                    async: false,
                    success: function (response) {
                        if (response) {
                            //Remove Please select option from list due to maintain existing behavior.
                            //TFS#9945:IE:Version 11- Intermediate issue "Exchange rate for given currencies is not available". 
                            $('#HiddenAchBillingCurrency Option[value=""]').remove();
                            $('#HiddenAchBillingCurrency option').clone().appendTo('#BillingCurrencyId');
                            isAchOrDualMember = true;
                        }
                        else {
                            SetCurrencyForNonBilateral(billingCurrencyValue);
                            isAchOrDualMember = false;
                        }

                    }
                });
            }
            else {
                SetCurrencyForNonBilateral(billingCurrencyValue);
            }
        }
    }
}

function SetCurrencyForNonBilateral(billingCurrencyValue)
{
 var list = $('#NonBilateralSMICurrenyList').val();

        do {
            //CMP#648:Populate dropdown without primary currency for SMI I,A,M :<option selected="selected" value=""></option>
            list = list.replace("option1", "<option").replace("option2", ">").replace("option3", "</option>");
        }
        while (list.search("option1") != -1);

        //SCP369502: New Release Issue
        try {
            $('#BillingCurrencyId').html(list);
            
        }
        catch (e) {
        }

        /* SCP# 366828 - Selecting Default Currency from drop down */
        //TFS#9945 :IE-Version 11: Intermediate issue "Exchange rate for given currencies is not available".
        //Desc:Set first value as selected value if  "billingCurrencyValue" is null or "". 
        if (billingCurrencyValue == "" || billingCurrencyValue == null) {
            $("#BillingCurrencyId").val($("#BillingCurrencyId option:first").val());
        } else {
            $('#BillingCurrencyId').val(billingCurrencyValue);
        }
       
        
        

        if (IsCreditNote == false) {
            PopulateExchangeRate();
        }
        //$('#ClearanceCurrency option').clone().appendTo('#BillingCurrencyId');
}

function PopulateRejectionInvDetails() {
  var $RejectedInvoiceNumber = $('#RejectedInvoiceNumber', '#content');
  var invoiceNumber = $RejectedInvoiceNumber.val();
  var smi = $('#SettlementMethodId', '#content').val();
  var billedMember = $("#BilledMemberId", '#content').val();
 
  _settlementMonth = null;
  if ($('#SettlementMonth').val() != '0' && $('#SettlementMonth').val() != '') {
      _settlementMonth = $('#SettlementMonth').val();
  }

  _settlementYear = null;
  if ($('#SettlementYear').val() != '0' && $('#SettlementYear').val() != '') {
      _settlementYear = $('#SettlementYear').val();
  }

  _settlementPeriod = null;
  if ($('#SettlementPeriod').val() != '0' && $('#SettlementPeriod').val() != '') {
      _settlementPeriod = $('#SettlementPeriod').val();
  }
 
  if ($.trim(smi) != '' && $.trim($RejectedInvoiceNumber.val()) != '' && $.trim(billedMember) != '') {
    $.ajax({
      type: "Post",
      url: GetRejectionInvoiceDetailsMethod,
      data: { billingMemberId: billingMemberId, billedMemberId: billedMember, rejectionInvoiceNumber: invoiceNumber, settlementMethod: smi, settlementMonth: _settlementMonth, settlementYear: _settlementYear, settlementPeriod: _settlementPeriod },
      dataType: "json",
      success: function (response) {
        if (response) {
          
          ProcessRejectionInvoiceDetails(response);
        }
        $(".rejInvLoader").hide();
        $("#SaveInvoiceHeader").attr("disabled", false);
      },
      beforeSend: function (xmlRequest) {
        $(".rejInvLoader").show();
        $(".rejInvNoDep").attr("disabled", true);
        $("#SaveInvoiceHeader").attr("disabled", true);
      }
    });
  }
}

function PopulateDefaultCurrency() {
    var smi = $('#SettlementMethodId').val();

    if (selectedBilledMemberId == undefined) {
        var billedMemberId = $("#BilledMemberId").val();
        selectedBilledMemberId = billedMemberId;
    }

      if ($.trim(smi) != '' && smi != smiX && jQuery.inArray(smi, bilateralSMIList) == -1 && selectedBilledMemberId != null) { //check only for empty smi and non bilateral.    
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

          PopulateExchangeRate();
        }
      },
      beforeSend: function (xmlRequest) {
      }
    });
  }
}

function ProcessRejectionInvoiceDetails(response) {
  currentBilledIn = response.CurrentBilledIn;
  currentBillingCurrency = response.CurrentBillingCurrencyId;

  if (response.BillingPeriod != null && response.BillingPeriod == 0) // Rejected invoice NOT found
  {
    // make fields editable
    $('#BilledIn').removeAttr('disabled');
    $('#SettlementPeriod').removeAttr('disabled');
    $('#RejectionStage').removeAttr('disabled');
    $('#ListingCurrencyId').removeAttr('disabled');

    OnRejectionStageChange();
    
    //Enable ExchangeRate field, if Original Invoice not found in DB
    $('#ListingToBillingRate').removeAttr('readonly');

  }
  else if (response.ErrorMessage && response.ErrorMessage != '') {
    alert(response.ErrorMessage);
    $('#BilledIn').val('');
    $('#SettlementPeriod').val('');
    $('#RejectionStage').val('');
    $('#RejectedInvoiceNumber').val('');
    $('#BilledIn').removeAttr('disabled');
    $('#SettlementPeriod').removeAttr('disabled');
    $('#RejectionStage').removeAttr('disabled');
    return false;
  }
  else {
    // The credit note rejection confirmation message should be shown on the create invoice page only if user
    // has navigated to it through menu and not through Billing history/Payables.
    if (response.AlertMessage && response.AlertMessage != '' && $('#IsCreatedFromBillingHistory').val() == 'False' && isOnEditInvoice == false) { // credit note rejection.
      if (confirm(response.AlertMessage) == true) {
        populateRejectionFields(response, currentBillingCurrency);
      }
      else {
        // clear rejection fields.
        $('#RejectedInvoiceNumber').val('');
        $('#BilledIn').val('');
        $('#SettlementPeriod').val('');
        $('#RejectionStage').val('');
      }
    }
    else {
      populateRejectionFields(response, currentBillingCurrency);
    }
  }
}

function populateRejectionFields(response, currentBillingCurrency) {
   
  var smi = $('#SettlementMethodId').val();
  if (response.BillingYear != null && response.BillingMonth != null) {
    if (currentBillingCurrency != null)
      $('#ListingCurrencyId').val(currentBillingCurrency);

    if (response.DisableBillingCurrency)
      $('#ListingCurrencyId').attr('disabled', true);
    else
        $('#ListingCurrencyId').attr('disabled', false);

    if (smi == smiA || smi == smiI || smi == smiM) {
      PopulateExchangeRate();
    }
    if (isBilledInContainsValue(response.BillingYear, response.BillingMonth)) {
      $('#BilledIn').val(response.BillingYear + '-' + response.BillingMonth);
      $('#BilledIn').attr('disabled', true);
    }
    else {
      $('#BilledIn').val('');
      $('#BilledIn').attr('disabled', false);
    }
  }
  if (response.BillingPeriod != null) {
    $('#SettlementPeriod').val(response.BillingPeriod);
    $('#SettlementPeriod').attr('disabled', true);
  }
  if (response.RejectionStage != null) {
    $('#RejectionStage').val(response.RejectionStage);
    $('#RejectionStage').attr('disabled', true);

  }

  SetBilledInValue(); // used in case of Edit Invoice
}

function isBilledInContainsValue(year, month) {
  if ($('#BilledIn option[value=' + year + '-' + month + ']').length > 0)
    return true;
  return false;
}
// used in case of Edit invoice
function SetBilledInValue() {
  
  if ($('#BilledIn').val() == '')
    $('#BilledIn').val($('#SettlementYear').val() + '-' + $('#SettlementMonth').val());
}

function PopulateCorrespondenceInvDetails() {
  if ($('#IsCreatedFromBillingHistory').val() == "True") return;

  var corrRefNumber = $('#CorrespondenceRefNo').val();
  var billedMemberId = $('#BilledMemberId').val();

  corrRefNumber = $.trim(corrRefNumber);

  if (corrRefNumber != '' && $.trim(billedMemberId) != '') {
    if (isNaN(corrRefNumber)) {
      alert('Invalid Value for Correspondence Reference Number');
      $('#CorrespondenceRefNo').val('');
    }
    else {
        $.ajax({
            type: "POST",
            url: GetCorrespondenceInvoiceDetailsMethod,
            data: { correspondenceRefNumber: corrRefNumber, billedMemberId: billedMemberId, invoiceId: invoiceId, isUpdateOperation: isUpdateOperation },
            dataType: "json",
            success: function (response) {
                $(".corrInvLoader").hide();
                if (response) {
                    ProcessCorrespondenceInvoiceDetails(response);
                }
                $("#radOrgInvoice").attr("disabled", false);
                $("#radRejInvoice").attr("disabled", false);
                $("#radCpdInvoice").attr("disabled", false);

                $("#SaveInvoiceHeader").attr("disabled", false);
                $("#CorrespondenceRefNo").attr("disabled", false);
            },
            beforeSend: function (xmlRequest) {
                $(".corrInvLoader").show();
                $("#CorrespondenceRefNo").attr("disabled", true);
                $("#SaveInvoiceHeader").attr("disabled", true);

                $("#radOrgInvoice").attr("disabled", true);
                $("#radRejInvoice").attr("disabled", true);
                $("#radCpdInvoice").attr("disabled", true);
            }
        });
    }
  }
  if (corrRefNumber == '') {
    $('#CorrespondenceRejInvoiceNo').val('');
  }
}

function ProcessCorrespondenceInvoiceDetails(response) {
    var currentSmi = $('#SettlementMethodId').val();

    currentBillingCurrency = response.CurrentBillingCurrencyCode;
    if ($.trim(response.ErrorMessage) != '') // Rejected invoice NOT found
    {
        // make fields editable
        $('#CorrespondenceRejInvoiceNo').attr('readonly', true);
        $('#radCpdAbtr').attr('disabled', true);
        $('#radCpdExpired').attr('disabled', true);
        alert(response.ErrorMessage);
        $('#CorrespondenceRefNo').val('');
        $('#CorrespondenceRejInvoiceNo').val('');

    }
    else {
        currentBilledIn = response.CurrentBilledIn;
        $('#CorrespondenceRejInvoiceNo').val(response.RejectedInvoiceNumber);


        $('#IsAuthorityToBill').val(response.IsAuthorityToBill);

        SetAuthorityToBillRadioButtons();

        $('#CorrespondenceRejInvoiceNo').attr('readonly', true);
        $('#radCpdAbtr').attr('disabled', true);
        $('#radCpdExpired').attr('disabled', true);
       // if (response.DisableBillingCurrency && currentSmi != smiX)
      //      $('#ListingCurrencyId').attr('disabled', true);
      //  else
        //CMP #553: ACH Requirement for Multiple Currency Handling
        $('#ListingCurrencyId').attr('disabled', false);

        if (response.EnableExchangeRate) {

            $('#ListingToBillingRate').removeAttr('readonly');
            $('#IsValidationFlag').val('EX');
        }
        else {
            //SCP# 420308 - Creating Correspondence Invoice
            //Because of incorrect variable used below, IS-Web screen used to hang up.
            //Incorrect/Old Code -if (smi == smiA || smi == smiI || smi == smiM) {
            if(currentSmi == smiA || currentSmi == smiI || currentSmi == smiM) {
                $('#ListingToBillingRate').attr('readonly', true);
                $('#IsValidationFlag').val('');
            }
        }
        //CMP #553: ACH Requirement for Multiple Currency Handling
//        if (currentBillingCurrency != null && currentBillingCurrency != 0) {
//            $('#ListingCurrencyId').val(currentBillingCurrency);
//        }
        //SCP# 420308 - Creating Correspondence Invoice
        //Because of incorrect variable used below, IS-Web screen used to hang up.
        //if (smi == smiA || smi == smiI || smi == smiM) {
        if(currentSmi == smiA || currentSmi == smiI || currentSmi == smiM) {
            PopulateExchangeRate();
        }
    }
}

var $NoteDropdown = '#NoteDropdown';
var $NoteDescription = '#txtNoteDesc';

var NoteName = "Name";
var NoteDescription = "Description";

// url: Action method that returns additional detail list
// detailType = 1: Additional Detail; 2: Notes
function InitializeNoteFields(noteData, url, detailType, detailLevel) {
  registerAutocomplete('NoteDropdown', null, url, 0, false, null, detailType, detailLevel, null);
  noteUrl = url;
  addDetailType = detailType;
  addDetailLevel = detailLevel;
  // bind existing data.
  noteData = eval(noteData);
  if (noteData != null && noteData[0] != null) {
    if (noteData[0][NoteName] != null)
      $($NoteDropdown).val(noteData[0][NoteName]);
    if (noteData[0][NoteDescription] != null)
      $($NoteDescription).val(noteData[0][NoteDescription]);

    for (noteCounter = 1; noteCounter < noteData.length; noteCounter++) {
      // assign values to controls.
      if (noteData[noteCounter][NoteName] != null || noteData[noteCounter][NoteDescription] != null)
        AppendAddNoteTemplate();

      if (noteData[noteCounter][NoteName] != null)
        $($NoteDropdown + addNoteclick).val(noteData[noteCounter][NoteName]);

      if (noteData[noteCounter][NoteDescription] != null)
        $($NoteDescription + addNoteclick).val(noteData[noteCounter][NoteDescription]);
    }
  }

  $.validator.addMethod('noteDescription', isValidNoteDesc, "Note Description Required.");
  $.validator.addClassRules("noteDescRequired", {
    noteDescription: true
  });
}

// values before edit.
var originalSmi;
var originalBillingMonth;
var originalBillingYear;
var originalSettlementYear;
var originalSettlementMonth;
var originalRejectionStage;
var originalSettlementPeriod;

function InitializeEditMode(isLineItemExists, attachmentCount) {
  originalSmi = $('#SettlementMethodId').val();
  originalRejectionStage = $('#RejectionStage').val();
  originalBillingMonthYear = $('#BillingYearMonthPeriod').val();
  originalBilledIn = $('#BilledIn').val();
  originalBillingYear = originalBillingMonthYear.split('-')[0];
  originalBillingMonth = originalBillingMonthYear.split('-')[1];
  // Original BilledIn is null in case of CreditNote, so if not null then split the value else skip
  if (originalBilledIn != null) {
    originalSettlementYear = originalBilledIn.split('-')[0];
    originalSettlementMonth = originalBilledIn.split('-')[1];
    originalSettlementPeriod = $('#SettlementPeriod').val();
  }

  if (isLineItemExists || attachmentCount > 0) {
    //make certain fields readonly.
    $('#BilledMemberText').attr('readonly', 'readonly');
    $('#InvoiceDate').attr('readonly', 'readonly');
    $('#InvoiceDate').datepicker('disable');
    $('#RejectedInvoiceNumber').attr('readonly', true);

    $('#ChargeCategoryId').attr('disabled', true);
    $('#radOrgInvoice').attr('disabled', true);
    $('#radRejInvoice').attr('disabled', true);
    $('#radCpdInvoice').attr('disabled', true);
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

function InitRejectionInvoiceDetail() {
  $('#BilledIn').attr('disabled', true);
  $('#SettlementPeriod').attr('disabled', true);
  $('#RejectionStage').attr('disabled', true);
}


function InitializeLocationCode(isLocationCodePresent) {
  if (isLocationCodePresent) {
    $('#LocationCode').attr('readonly', true);
    $('#LocationCode').val('');
  }
  else {
    $('#LocationCode').removeAttr('readOnly');
  }
}

function OnRejectionStageChange() {
  
  if ($('RejectedInvoiceNumber').val() != '' && $('#RejectionStage').val() == "2") {
    $('#ListingToBillingRate').removeAttr('readonly');
    $('#IsValidationFlag').val('EX');
    $('#ListingCurrencyId').removeAttr('disabled');
  }
  else {
    $('#IsValidationFlag').val('');
    $('#ListingToBillingRate').attr('readonly', true);
  }
}

function isDirty(smi, rejStage, billingYear, billingMonth, settlementYear, settlementMonth, settlementPeriod) {
  if (originalSmi != smi || originalRejectionStage != rejStage || billingYear != originalBillingYear
  || billingMonth != originalBillingMonth || settlementYear != originalSettlementYear || settlementMonth != originalSettlementMonth || settlementPeriod != originalSettlementPeriod)
    return true;
  else
    return false;
}

function warnUserForOutsideTimeLimitRejection(form) {
  var shouldSave = true;

  //CMP#678: Time Limit Validation on Last Stage MISC Rejections 
  // check if it is a rejection invoice
  var smi = $('#SettlementMethodId').val();
  var rejStage = $('#RejectionStage').val();

  if (IsRadioChecked("#radRejInvoice") == false) {
    onSubmitHandler(form);
    return;
  }
  //CMP#678: Time Limit Validation on Last Stage MISC Rejections 
  //This popup should still be shown (if applicable) when an SMI “A” Original Invoice is rejected beyond the Time Limit (R1)
  if (smi == smiA && rejStage == '1') {
      var billingMonthYear = $('#BillingYearMonthPeriod').val();
      var billedIn = $('#BilledIn').val();
      var billingYear = billingMonthYear.split('-')[0];
      var billingMonth = billingMonthYear.split('-')[1];
      var settlementYear = billedIn.split('-')[0];
      var settlementMonth = billedIn.split('-')[1];
      var settlementPeriod = $('#SettlementPeriod').val();
      if (isDirty(smi, rejStage, billingYear, billingMonth, settlementYear, settlementMonth, settlementPeriod) == false) {
          onSubmitHandler(form);
          return;
      }

      $.ajax({
          type: "POST",
          url: isRejectionOutsideTimeLimitMethod,
          data: { rejectionStage: rejStage, settlementYear: settlementYear, settlementMonth: settlementMonth, settlementMethodId: smi, billingYear: billingYear, billingMonth: billingMonth, settlementPeriod: settlementPeriod },
          dataType: "json",
          success: function (response) {
              // rejection outside time limit.
              if (response == true) {
                  if (confirm('The rejection is outside time limit. Do you want to continue with the Save operation?') == true) {
                      shouldSave = true;
                  }
                  else
                      shouldSave = false;
              }
              else {
                  shouldSave = true;
              }
          },
          complete: function () { if (shouldSave == true) onSubmitHandler(form); }
      });
  }
  else {
      onSubmitHandler(form);
      return;
  }
}

function isValidNoteDesc(value, element) {
  var elementName = $(element).attr("id");
  var elementNameLength = elementName.length;

  // txtNoteDesc contains 11 characters.
  var noteNumber = elementName.substr(11, elementNameLength - 11);

  var noteIdConst = "NoteDropdown";
  var noteId = noteIdConst + noteNumber;
  // If description is empty..
  var $noteId = $('#' + noteId);
  if ($noteId.val() == undefined || $noteId.val() == '') {
    return true;
  }
  else if (value == '') {
    return false;
  }

  return true;
}

function PopulateSmiXfields() {
  EnableDisableSMiDropdown();
  var valueToCompare;
  if ($isOnView == true)
    valueToCompare = $("#SettlementMethodIdForView").val();
  else
    valueToCompare = $("#SettlementMethodId").val();

  if (valueToCompare == smiX) {
//    if (isFrombillingHistory == true) {
//      $('#SettlementMethodId').attr('disabled', true);
//    }
    $("#divExchangeRate").show();
    $("#divClearanceCurrency").show();
    $('#divTotalAmountInClearanceCurrency').show();
    if ($isOnView == false) {
      $("#ListingToBillingRate").removeAttr('readonly');
      $('#ListingCurrencyId').removeAttr('disabled');
      $("#ListingToBillingRate").removeAttr('readonly');
      $("#ChAgreementIndicator").removeAttr('readonly');
      $("#ChDueDate").removeAttr('readonly');
      $("#ChDueDate").datepicker("enable");
      var hiddenBillingCurrency = $("#HiddenBillingCurrencyId").val();
      $('#BillingCurrencyId').val(hiddenBillingCurrency);
    }
    else {
      $("#ChAgreementIndicator").attr("readonly", "readonly");
      $("#ChDueDate").attr("readonly", "readonly");
      $("#ChDueDate").datepicker('disable');
  }
  }
  else {
      if ($isOnView == false) {
      $("#ListingToBillingRate").attr('readonly', true);
      if (IsCreditNote) {
        InitializeExhangeRateField(creditNoteInvoiceTypeId);
      }
      $("#ChAgreementIndicator").val("");
      $("#ChAgreementIndicator").attr("readonly", "readonly");
      $("#ChDueDate").val("");
      $("#ChDueDate").watermark("DD-MMM-YY");
      $("#ChDueDate").attr("readonly", "readonly");
      $("#ChDueDate").datepicker('disable');
    }
  else {
      $("#ChAgreementIndicator").attr("readonly", "readonly");
      $("#ChDueDate").attr("readonly", "readonly");
      $("#ChDueDate").datepicker('disable');
  }
  SetExchangeRateFieldforBilateralSMI();
}
}

//CMP#648: Clearance Information in MISC Invoice PDFs
function SetExchangeRateFieldforBilateralSMI() {
    var selectedSMI = 0;
    if ($isOnView == true)
        selectedSMI = $("#SettlementMethodIdForView").val();
    else
        selectedSMI = $("#SettlementMethodId").val();

    var exchangeRate = $("#ListingToBillingRate").val();
    var requiredindBC = $("#RequiredindBC");
    var requiredindEX = $("#RequiredindEX");
    if (jQuery.inArray(selectedSMI, bilateralSMIList) > -1) {
        var exchnRate = $.trim($("#ListingToBillingRate").val());

        if (exchnRate.length <= 17 && exchnRate != '' && exchnRate != 'NaN') {
            var exchnRate5 = parseFloat(exchnRate).toFixed(5);
            $("#ListingToBillingRate").val(exchnRate5);
            CalculateNetTotal();
        }
        else {
            $('#InvoiceSummary_TotalAmountInClearanceCurrency').val('');
        }

        $("#ListingToBillingRate").attr('readonly', false);
        requiredindBC.hide();
        requiredindEX.hide();
        var billingCurrencyValue = $('#BillingCurrencyId').val();
        if (billingCurrencyValue == '') {
            $("#ListingToBillingRate").val('');
        }
    }
    else {
        $("#ListingToBillingRate").attr('readonly', true);
        if (IsCreditNote) {
            InitializeExhangeRateField(creditNoteInvoiceTypeId);
        }
        requiredindBC.show();
        requiredindEX.show();
    }
 }

// CMP#624:2.17 : Change 17 : Disallow change of SMI from X to another value
function EnableDisableSMiDropdown() {
  var valueToCompare = $("#SettlementMethodId").val();
  if (valueToCompare == smiX) {
    if ($isOnView == false && isOnEditInvoice == true) {
      if ($('#radOrgInvoice').is(':checked')) {
        $('#SettlementMethodId').removeAttr('disabled');
      }
      else {
        $('#SettlementMethodId').attr('disabled', true);
      }
    }
    if ($isOnView == true) {
      $('#SettlementMethodId').attr('disabled', true);
    } 
  }
  else {
    if ($isOnView == true) {
      $('#SettlementMethodId').attr('disabled', true);
    }
    else {
      $('#SettlementMethodId').removeAttr('disabled');
    }
  }

}

function GetCorrDetailsAfterSmiChanged() {
  var valueToCompare;
  if ($isOnView == true)
    return;
  else
    valueToCompare = $("#SettlementMethodId").val();

  if (IsRadioChecked("#radCpdInvoice")) {
    PopulateCorrespondenceInvDetails();

    if ($('#IsCreatedFromBillingHistory').val() == "True" && $('#ListingCurrencyId').attr('disabled') == false && valueToCompare != smiX) {
      var corrRefNumber = $('#CorrespondenceRefNo').val();
      var billedMemberId = $('#BilledMemberId').val();

      corrRefNumber = $.trim(corrRefNumber);

      if (corrRefNumber != '' && $.trim(billedMemberId) != '') {
        if (isNaN(corrRefNumber)) {
          alert('Invalid Value for Correspondence Reference Number');
          $('#CorrespondenceRefNo').val('');
        }
        else {
          $.ajax({
            type: "POST",
            url: GetCorrespondenceInvoiceDetailsMethod,
            data: { correspondenceRefNumber: corrRefNumber, billedMemberId: billedMemberId, invoiceId: invoiceId, isUpdateOperation: isUpdateOperation },
            dataType: "json",
            success: function (response) {
              $(".corrInvLoader").hide();
              if (response) {
                ProcessCorrespondenceInvoiceDetails(response);
                PopulateExchangeRate();
              }
              $("#SaveInvoiceHeader").attr("disabled", false);

            },
            beforeSend: function (xmlRequest) {
              $(".corrInvLoader").show();
              $("#CorrespondenceRefNo").attr("disabled", true);
              $("#SaveInvoiceHeader").attr("disabled", true);

              $("#radOrgInvoice").attr("disabled", true);
              $("#radRejInvoice").attr("disabled", true);
              $("#radCpdInvoice").attr("disabled", true);
            }
          });
        }
      }
      if (corrRefNumber == '') {
        $('#CorrespondenceRejInvoiceNo').val('');
      }
    }
  }
}

//CMP#502 : [3.5] Rejection Reason for MISC Invoices
var rejectionDialog;

function onRejectionReasonCodeChange(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode
  $('#RejectionReasonCode').val('');
  $('#RejectionReasonCode').val(selectedValue.split('-')[0]);
  $('#RejectionReasonCodeText').val(selectedValue);
}

function warnUserForRejectionReasoneCode() {
  rejectionDialog = $('#divRejectionReasonCode').dialog({
    closeOnEscape: false,
    title: 'Misc Rejection Reason Code',
    height: 250,
    width: 600,
    modal: true,
    resizable: false
  });
}

function closeRejectionReasonCodeDetail() {
  rejectionDialog.dialog("close");
}

function ValidateRejectionReasoneCode() {
  var reasonCode = $('#RejectionReasonCode').val();
  if (reasonCode == "" || reasonCode == "undefined") {
    alert("Rejection Reason Code is mandatory");
  }
  else {
    var InvoiceHeaderform = $("#InvoiceForm");
    InvoiceHeaderform.validate();
    if (InvoiceHeaderform.valid()) {
      InvoiceHeaderform.submit();
      closeRejectionReasonCodeDetail();
    }
  }
}

function ValidateForm() {
  var InvoiceHeaderform = $("#InvoiceForm");
  InvoiceHeaderform.validate();
  if (InvoiceHeaderform.valid()) {
    if ($('#IsCreatedFromBillingHistory').val() == "True") {
      warnUserForRejectionReasoneCode();
    }
    else {
      InvoiceHeaderform.submit();
    }
  }
}

function onBlankRejectionReasonCode() {
  // Split selectedValue parameter to retrieve reasonCode
  $('#RejectionReasonCode').val('');
}