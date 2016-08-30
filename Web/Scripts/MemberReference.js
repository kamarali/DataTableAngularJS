// BILLING member variables. Do not use for billed member.
var previousBillingLocationValue;
var legalName;
var companyRegId;
var addressLine1;
var addressLine2;
var addressLine3;
var cityName;
var footerDataLine;
var taxVatRegistration;
var addTaxVatRegistration;
var postalCode;
var countryName;
var subDivName;
var isBillingLegalTextSet = false;
var isDetailSetFromBlankLocationCode = false;
var memberOrgNameControlId;
var memberOrgNameOldData;

// BILLED member variables. Do not use for billing member.
var previousBilledLocationValue;
var billedLegalName;
var billedCompanyRegId;
var billedAddressLine1;
var billedAddressLine2;
var billedAddressLine3;
var billedCityName;
var billedFooterDataLine;
var billedTaxVatRegistration;
var billedAddTaxVatRegistration;
var billedPostalCode;
var billedCountryName;
var billedSubDivName;

var GetMemberLocationMethod = '/Data/GetMemberLocationDetails';
var GetSubDivisionCodeMethod = '/Data/GetSubDivisionNameList';
var InvoiceId = '';

var $validateBillingMemberReference;
var $validateBilledMemberReference;
function InitReferenceData(locationIdMethod, invoiceId, subDivisionCodeMethod) {
  registerAutocomplete('MemberLocationInformation_0__SubdivisionName', 'MemberLocationInformation_0__SubdivisionName', subDivisionCodeMethod, 0, true, null, '', '', '#BillingMemberCountryCode');
  registerAutocomplete('MemberLocationInformation_1__SubdivisionName', 'MemberLocationInformation_1__SubdivisionName', subDivisionCodeMethod, 0, true, null, '', '', '#BilledMemberCountryCode');
  GetMemberLocationMethod = locationIdMethod;
  GetSubDivisionCodeMethod = subDivisionCodeMethod;
  InvoiceId = invoiceId;

  var billingLocationCode = $("#BillingMemberLocationCode").val();
  var billedLocationCode = $("#BilledMemberLocationCode").val();
  $('#MemberLocationInformation_0__MemberLocationCode').val(billingLocationCode);
  $('#MemberLocationInformation_1__MemberLocationCode').val(billedLocationCode);

  if ($("#BilledMemberLocationCode").val() != "")
    SetControlAccessibility(true, false);

  if ($("#BillingMemberLocationCode").val() != "")
    SetControlAccessibility(true, true);

  EnableBilledLocationLink();
  // doing this, as while binding to model, this is saved with a blank space if empty.(the field is not nullable.)
  $('#MemberLocationInformation_0__OrganizationName').val($.trim($('#MemberLocationInformation_0__OrganizationName').val()));
  $('#MemberLocationInformation_1__OrganizationName').val($.trim($('#MemberLocationInformation_1__OrganizationName').val()));
  //debugger;
  if (billingLocationCode != '')
    GetMemberLocationForCreateMode(billingLocationCode, $("#BillingMemberId").val(), true, true);
  // else
  closeBillingReferenceDataPopup(true);
  if (billedLocationCode != '')
    GetMemberLocationForCreateMode(billedLocationCode, $("#BilledMemberId").val(), false, true);
  //  else
  closeBilledReferenceDataPopup(true);

  // Register Control events and Validation rules only if pageMode is Create or Edit

  $validateBillingMemberReference = $("#BillingMemberReferenceForm").validate({
    rules: {
      "MemberLocationInformation[0].OrganizationName": { required: true, allowedCharactersWithExclimation:true },
      "MemberLocationInformation[0].TaxRegistrationId": { allowedCharacters: true },
      "MemberLocationInformation[0].CompanyRegistrationId": { allowedCharacters: true },
      "MemberLocationInformation[0].AdditionalTaxVatRegistrationNumber": { allowedCharacters: true },

      BillingMemberCountryCode: "required",
      "MemberLocationInformation[0].AddressLine1": { required: true, allowedCharacters: true },
      "MemberLocationInformation[0].AddressLine2": { allowedCharacters: true },
      "MemberLocationInformation[0].AddressLine3": { allowedCharacters: true },
      "MemberLocationInformation[0].CityName": { required: true, allowedCharacters: true },
      "MemberLocationInformation[0].PostalCode": { allowedCharacters: true },
      "MemberLocationInformation[0].LegalText": { allowedCharactersForTextAreaFields: true }

    },
    highlight: false,
    submitHandler: function () { closeBillingReferenceDataPopup(); closeDialog('#BillingMemberReference'); },
    
    invalidHandler: function (form, validator) {
           var errorCount = validator.numberOfInvalids();
           if (errorCount > 0) {
               $('#MemberLocationInformation_0__OrganizationName').val($('#MemberLocationInformation_0__OrganizationName').val().replace("!!!",""));
           }
       }
    
  });

  $("#MemberLocationInformation_0__LegalText").bind("keypress", function () { maxLength(this, 700) });
  $("#MemberLocationInformation_0__LegalText").bind("paste", function () { maxLengthPaste(this, 700) });
    
  $validateBilledMemberReference = $("#BilledMemberReferenceForm").validate({
    rules: {
      "MemberLocationInformation[1].OrganizationName": { required: true, allowedCharactersWithExclimation:true },
      "MemberLocationInformation[1].TaxRegistrationId": { allowedCharacters: true },
      "MemberLocationInformation[1].CompanyRegistrationId": { allowedCharacters: true },
      "MemberLocationInformation[1].AdditionalTaxVatRegistrationNumber": { allowedCharacters: true },
      BilledMemberCountryCode: "required",
      "MemberLocationInformation[1].AddressLine1": { required: true, allowedCharacters: true },
      "MemberLocationInformation[1].AddressLine2": { allowedCharacters: true },
      "MemberLocationInformation[1].AddressLine3": { allowedCharacters: true },
      "MemberLocationInformation[1].CityName": { required: true, allowedCharacters: true },
      "MemberLocationInformation[1].PostalCode": { allowedCharacters: true }
    },
    messages: {
      "MemberLocationInformation[1].OrganizationName": "Company Legal Name Required",
      BilledMemberCountryCode: "Country Required",
      "MemberLocationInformation[1].AddressLine1": "AddressLine 1 Required",
      "MemberLocationInformation[1].CityName": "City Required"
       
    },
    highlight: false,
    submitHandler: function () {
      closeBilledReferenceDataPopup(); closeDialog('#BilledMemberReference');
    },
       invalidHandler: function (form, validator) {
           var errorCount = validator.numberOfInvalids();
           if (errorCount > 0) {
               $('#MemberLocationInformation_1__OrganizationName').val($('#MemberLocationInformation_1__OrganizationName').val().replace("!!!",""));
           }
       }
  });

  $('#MemberLocationInformation_1__OrganizationName').keyup(function() {
            var len = $(this).val().length;
            if (len > 100) {
                this.value = this.value.substring(0, 100);
            }
        });
    
 $('#MemberLocationInformation_0__OrganizationName').keyup(function() {
            var len = $(this).val().length;
            if (len > 100) {
                this.value = this.value.substring(0, 100);
            }
        });

  $('#BillingMemberCountryCode').change(function () {
    $('#MemberLocationInformation_0__SubdivisionName').val("");
    $('#MemberLocationInformation_0__SubdivisionName').flushCache();
  });

  $('#BilledMemberCountryCode').change(function () {
    $('#MemberLocationInformation_1__SubdivisionName').val("");
    $('#MemberLocationInformation_1__SubdivisionName').flushCache();
  });

  $('#BillingMemberReference').bind('dialogopen', function () {
    //debugger;
    // copy the value from main dropdown to popup dropdown.
    var $MemberLocationCode = $('#MemberLocationInformation_0__MemberLocationCode');
    $MemberLocationCode.val($('#BillingMemberLocationCode').val());
    $MemberLocationCode.change();

    // copy values from cloned fields to popup.
    //if ($MemberLocationCode.val() != '') {
    // Done so as to clear all fields when blank location code is selected.
    if (isDetailSetFromBlankLocationCode == true)
      copyFromCloneToDialog(true);
    //}
    else { // Copy only footer from clone.
      $('#MemberLocationInformation_0__LegalText').val($('#MemberLocationInformation_0__LegalText1').val());
    }
      $('#MemberLocationInformation_0__OrganizationName').val($('#MemberLocationInformation_0__OrganizationName').val().replace("!!!",""));
  });

  $('#BilledMemberReference').bind('dialogopen', function () {
    // copy the value from main dropdown to popup dropdown.
    var $MemberLocationCode = $('#MemberLocationInformation_1__MemberLocationCode');
    $MemberLocationCode.val($('#BilledMemberLocationCode').val());
    $MemberLocationCode.change();
    //debugger;
    // copy values from cloned fields to popup.
    //if ($MemberLocationCode.val() != '') {
    copyFromCloneToDialog(false);
    //}
     $('#MemberLocationInformation_1__OrganizationName').val($('#MemberLocationInformation_1__OrganizationName').val().replace("!!!",""));
  });

  $('#BillingMemberReference').bind('dialogclose', function () {
    $validateBillingMemberReference.resetForm();
     
  });

  $('#BilledMemberReference').bind('dialogclose', function () {
    $validateBilledMemberReference.resetForm();
      
  });

  $("#BillingMemberClear").bind('click', function () {
    $('#MemberLocationInformation_0__MemberLocationCode').val('');
    ResetDivInputs('#BillingMemberReference');
    SetControlAccessibility(false, true);
    clearMemoryData(true);
  });

  $("#BilledMemberClear").bind('click', function () {
    $('#MemberLocationInformation_1__MemberLocationCode').val('');
    ResetDivInputs('#BilledMemberReference');
    SetControlAccessibility(false, false);
    clearMemoryData(false);

  });

  // Billed member
  $("#BilledMemberLocationCode").bind('change', function () {

    PopulatedBilledMemberLocationDetails();
    EnableBilledLocationLink();
  });

  //Billing member
  $("#BillingMemberLocationCode").bind('change', function () {
    //debugger;
    if ($("#BillingMemberLocationCode").val() == "") {
      SetControlAccessibility(false, true);
      ResetDivInputs('#BillingMemberReference');
    }
    else {
      var LocationCode = $("#BillingMemberLocationCode").val();
      //debugger;
      GetMemberLocationForCreateMode(LocationCode, $("#BillingMemberId").val(), true);
    }
    $('#MemberLocationInformation_0__MemberLocationCode').val($("#BillingMemberLocationCode").val());
  });


  function copyFromCloneToDialog(isBillingMember) {
    var isBlankLocationCode;
    if (isBillingMember == true) {
      isBlankLocationCode = $("#MemberLocationInformation_0__MemberLocationCode").val() == "";
    }
    else {
      isBlankLocationCode = $("#MemberLocationInformation_1__MemberLocationCode").val() == "";
    }

    //debugger;
    if (isBillingMember == true) {
      $ReferenceDiv = $("#BillingMemberReference");

      $('#BillingMemberCountryCode').val($('#BillingMemberCountryCode1').val());
      $('#BillingMemberDSRequired').val($('#BillingMemberDSRequired1').val());
      $('#MemberLocationInformation_0__LegalText').val($('#MemberLocationInformation_0__LegalText1').val());

    }
    else {
      $ReferenceDiv = $("#BilledMemberReference");

      $('#BilledMemberCountryCode').val($('#BilledMemberCountryCode1').val());
      $('#BilledMemberDSRequired').val($('#BilledMemberDSRequired1').val());
      //   $('#MemberLocationInformation_1__LegalText').val($('#MemberLocationInformation_1__LegalText1').val());
    }
    // set all textbox values
    $ReferenceDiv.find("input[type=text]").each(function (i) {
      var $currentElem = $(this);
      var currentElemId = $currentElem.attr("id");

      $currentElem.attr('disabled', false);
      $currentElem.val($('#' + currentElemId + '1').val());
      if (isBlankLocationCode == false)
        $currentElem.attr('disabled', true);
    });
  }

  function clearMemoryData(isBillingMember) {
    if (isBillingMember == true) {
      legalName = '';
      companyRegId = '';
      addressLine1 = '';
      addressLine2 = '';
      addressLine3 = '';
      cityName = '';
      footerDataLine = '';
      taxVatRegistration = '';
      addTaxVatRegistration = '';
      postalCode = '';
      countryName = '';
      subDivName = '';
    }
    else {
      billedLegalName = '';
      billedCompanyRegId = '';
      billedAddressLine1 = '';
      billedAddressLine2 = '';
      billedAddressLine3 = '';
      billedCityName = '';
      billedFooterDataLine = '';
      billedTaxVatRegistration = '';
      billedAddTaxVatRegistration = '';
      billedPostalCode = '';
      billedCountryName = '';
      billedSubDivName = '';
    }
  }

  var $BillingMemberLocationCode = $("#MemberLocationInformation_0__MemberLocationCode");
  $BillingMemberLocationCode.focus(function () {
    previousBillingLocationValue = $BillingMemberLocationCode.val();

    if (previousBillingLocationValue == "") {
      // store values in js variables.
      copyToMemory(true); // This is done so that if user selects a blank location again, he will see his entered values.
    }
  }).change(function () {

    if ($BillingMemberLocationCode.val() == "") { // current value of dropdown
      //debugger;
      SetControlAccessibility(false, true);
      ResetDivInputs('#BillingMemberReference');
      if (isDetailSetFromBlankLocationCode == true)
        copyFromMemoryToDialog(true);
    }
    else {
      var LocationCode = $BillingMemberLocationCode.val();
      //debugger;
      GetMemberLocationForCreateMode(LocationCode, $("#BillingMemberId").val(), true);
    }
  });

  var $BilledMemberLocationCode = $("#MemberLocationInformation_1__MemberLocationCode");
  $BilledMemberLocationCode.focus(function () {
    previousBilledLocationValue = $BilledMemberLocationCode.val();

    if (previousBilledLocationValue == "") {
      // store values in js variables.
      copyToMemory(false); // This is done so that if user selects a blank location again, he will see his entered values.
    }
  }).change(function () {
    if ($BilledMemberLocationCode.val() == "") {
      SetControlAccessibility(false, false);
      ResetDivInputs('#BilledMemberReference');
      copyFromMemoryToDialog(false);
    }
    else {
      var LocationCode = $BilledMemberLocationCode.val();
      GetMemberLocationForCreateMode(LocationCode, $("#BilledMemberId").val(), false);
    }
  });
} // end InitReferenceData

function copyToMemory(isBillingMember) {
  if (isBillingMember == true) {
    legalName = $('#MemberLocationInformation_0__OrganizationName').val();
    companyRegId = $('#MemberLocationInformation_0__CompanyRegistrationId').val();
    addressLine1 = $('#MemberLocationInformation_0__AddressLine1').val();
    addressLine2 = $('#MemberLocationInformation_0__AddressLine2').val();
    addressLine3 = $('#MemberLocationInformation_0__AddressLine3').val();
    cityName = $('#MemberLocationInformation_0__CityName').val();
    footerDataLine = $('#MemberLocationInformation_0__LegalText').val();
    taxVatRegistration = $('#MemberLocationInformation_0__TaxRegistrationId').val();
    addTaxVatRegistration = $('#MemberLocationInformation_0__AdditionalTaxVatRegistrationNumber').val();
    postalCode = $('#MemberLocationInformation_0__PostalCode').val();
    countryName = $('#BillingMemberCountryCode').val();
    subDivName = $('#MemberLocationInformation_0__SubdivisionName').val();
  }
  else {
    billedLegalName = $('#MemberLocationInformation_1__OrganizationName').val();
    billedCompanyRegId = $('#MemberLocationInformation___CompanyRegistrationId').val();
    billedAddressLine1 = $('#MemberLocationInformation_1__AddressLine1').val();
    billedAddressLine2 = $('#MemberLocationInformation_1__AddressLine2').val();
    billedAddressLine3 = $('#MemberLocationInformation_1__AddressLine3').val();
    billedCityName = $('#MemberLocationInformation_1__CityName').val();
    //  billedFooterDataLine = $('#MemberLocationInformation_1__LegalText').val();
    billedTaxVatRegistration = $('#MemberLocationInformation_1__TaxRegistrationId').val();
    billedAddTaxVatRegistration = $('#MemberLocationInformation_1__AdditionalTaxVatRegistrationNumber').val();
    billedPostalCode = $('#MemberLocationInformation_1__PostalCode').val();
    billedCountryName = $('#BilledMemberCountryCode').val();
    billedSubDivName = $('#MemberLocationInformation_1__SubdivisionName').val();
  }
}

function copyFromMemoryToDialog(isBillingMember) {
  if (isBillingMember == true) {
    $('#MemberLocationInformation_0__OrganizationName').val(legalName);
    $('#MemberLocationInformation_0__CompanyRegistrationId').val(companyRegId);
    $('#MemberLocationInformation_0__AddressLine1').val(addressLine1);
    $('#MemberLocationInformation_0__AddressLine2').val(addressLine2);
    $('#MemberLocationInformation_0__AddressLine3').val(addressLine3);
    $('#MemberLocationInformation_0__CityName').val(cityName);
    $('#MemberLocationInformation_0__LegalText').val(footerDataLine);
    $('#MemberLocationInformation_0__TaxRegistrationId').val(taxVatRegistration);
    $('#MemberLocationInformation_0__AdditionalTaxVatRegistrationNumber').val(addTaxVatRegistration);
    $('#MemberLocationInformation_0__PostalCode').val(postalCode);
    $('#BillingMemberCountryCode').val(countryName);
    $('#MemberLocationInformation_0__SubdivisionName').val(subDivName);
  }
  else {
    $('#MemberLocationInformation_1__OrganizationName').val(billedLegalName);
    $('#MemberLocationInformation_1__CompanyRegistrationId').val(billedCompanyRegId);
    $('#MemberLocationInformation_1__AddressLine1').val(billedAddressLine1);
    $('#MemberLocationInformation_1__AddressLine2').val(billedAddressLine2);
    $('#MemberLocationInformation_1__AddressLine3').val(billedAddressLine3);
    $('#MemberLocationInformation_1__CityName').val(billedCityName);
    //    $('#MemberLocationInformation_1__LegalText').val(billedFooterDataLine);
    $('#MemberLocationInformation_1__TaxRegistrationId').val(billedTaxVatRegistration);
    $('#MemberLocationInformation_1__AdditionalTaxVatRegistrationNumber').val(billedAddTaxVatRegistration);
    $('#MemberLocationInformation_1__PostalCode').val(billedPostalCode);
    $('#BilledMemberCountryCode').val(billedCountryName);
    $('#MemberLocationInformation_1__SubdivisionName').val(billedSubDivName);
  }
}

function GetMemberLocationForCreateMode(LocationCode, MemberId, isbilling, shouldClone) {
  //debugger;
  $.ajax({
    type: "GET",
    url: GetMemberLocationMethod,
    data:
        {
          locationCode: LocationCode,
          invoiceId: InvoiceId,
          isBillingMember: isbilling,
          memberId: MemberId
        },
    dataType: "json",
    success: function (response) {
      //debugger;      
      // copy from location form to cloned hidden fields.
      // if (shouldClone == true) {
      //      if (isbilling == true) {
      //       closeBillingReferenceDataPopup(true);
      //  isBillingLegalTextSet = false;
      //    }
      //    else
      //      closeBilledReferenceDataPopup(true);
      //     }

      OnLocationDetailsPopulated(response, isbilling);

      // If location is blank, fetch from clone. User has to press . or dash to suppress the footer.
      //SCPID : 107966 - SIS Member Profile - invoice footer detail
      //Trim the val of LegalText1 for the check to happen correctly in all browsers
      if (isbilling == true && $.trim($("#MemberLocationInformation_0__LegalText1").val()) != '') {
          $("#MemberLocationInformation_0__LegalText").val($("#MemberLocationInformation_0__LegalText1").val());

          
      }
    },
    failure: function (response) {

      alert('No Location information can be accessed.');
    }
  });
}

function OnLocationDetailsPopulated(response, billingMember) {
  //  debugger;
  if (response) {
    if (billingMember) {
      //   debugger;
      //f(footerDataLine == '')
      //$("#MemberLocationInformation_0__LegalText").val(response.LegalText ? response.LegalText : '');
      //else
      //$('#MemberLocationInformation_0__LegalText').val(footerDataLine);

      //if (response.LegalText == '' || response.LegalText == null || $("#MemberLocationInformation_0__LegalText").val() == '') {
      //isBillingLegalTextSet = false;
      //}
      // Done so as to not override the footer entered by user. Set the footer information only once when the location code is selected.
      //debugger;
        //   if (isBillingLegalTextSet == false) {

        $("#MemberLocationInformation_0__LegalText").val(response.LegalText ? response.LegalText : '');
        
      //  }
      //   else {
      //$("#MemberLocationInformation_0__LegalText").val(footerDataLine);
      // copy footer from clone.
      //    $('#MemberLocationInformation_0__LegalText').val($('#MemberLocationInformation_0__LegalText1').val());
      //   }
      //if(response.LegalText != null && response.LegalText != '')
      // isBillingLegalTextSet = true;
      //}

      $("#MemberLocationInformation_0__OrganizationName").val(response.OrganizationName ? response.OrganizationName : '');
      $("#MemberLocationInformation_0__TaxRegistrationId").val(response.TaxRegistrationId ? response.TaxRegistrationId : '');
      $("#MemberLocationInformation_0__CompanyRegistrationId").val(response.CompanyRegistrationId ? response.CompanyRegistrationId : '');
      $("#MemberLocationInformation_0__AddressLine3").val(response.AddressLine3 ? response.AddressLine3 : '');
      $("#MemberLocationInformation_0__CityName").val(response.CityName ? response.CityName : '');
      $("#MemberLocationInformation_0__AddressLine2").val(response.AddressLine2 ? response.AddressLine2 : '');
      $("#MemberLocationInformation_0__PostalCode").val(response.PostalCode ? response.PostalCode : '');
      $("#MemberLocationInformation_0__AddressLine1").val(response.AddressLine1 ? response.AddressLine1 : '');
      $("#BillingMemberCountryCode").val(response.CountryCode ? response.CountryCode : '');
      $("#MemberLocationInformation_0__AdditionalTaxVatRegistrationNumber").val(response.AdditionalTaxVatRegistrationNumber ? response.AdditionalTaxVatRegistrationNumber : '');
      $("#MemberLocationInformation_0__SubdivisionCode").val(response.SubdivisionCode ? response.SubdivisionCode : '');

      SetControlAccessibility(true, true);
      
    }
    else {
      //   $("#MemberLocationInformation_1__LegalText").val(response.LegalText ? response.LegalText : '');
      $("#MemberLocationInformation_1__OrganizationName").val(response.OrganizationName ? response.OrganizationName : '');
      $("#MemberLocationInformation_1__TaxRegistrationId").val(response.TaxRegistrationId ? response.TaxRegistrationId : '');
      $("#MemberLocationInformation_1__CompanyRegistrationId").val(response.CompanyRegistrationId ? response.CompanyRegistrationId : '');
      $("#MemberLocationInformation_1__AddressLine3").val(response.AddressLine3 ? response.AddressLine3 : '');
      $("#MemberLocationInformation_1__CityName").val(response.CityName ? response.CityName : '');
      $("#MemberLocationInformation_1__AddressLine2").val(response.AddressLine2 ? response.AddressLine2 : '');
      $("#MemberLocationInformation_1__PostalCode").val(response.PostalCode ? response.PostalCode : '');
      $("#MemberLocationInformation_1__AddressLine1").val(response.AddressLine1 ? response.AddressLine1 : '');
      $("#BilledMemberCountryCode").val(response.CountryCode ? response.CountryCode : '');
      $("#MemberLocationInformation_1__AdditionalTaxVatRegistrationNumber").val(response.AdditionalTaxVatRegistrationNumber ? response.AdditionalTaxVatRegistrationNumber : '');
      $("#MemberLocationInformation_1__SubdivisionCode").val(response.SubdivisionCode ? response.SubdivisionCode : '');

      SetControlAccessibility(true, false);
    }
}
//TFS#9993 : Firefox: v45- Billed Member's Location Id details are not removed for PAX/CGO.
    else if (response == null) {
        if (billingMember) {

            $("#MemberLocationInformation_0__LegalText").val('');
            $("#MemberLocationInformation_0__OrganizationName").val('');
            $("#MemberLocationInformation_0__TaxRegistrationId").val('');
            $("#MemberLocationInformation_0__CompanyRegistrationId").val('');
            $("#MemberLocationInformation_0__AddressLine3").val('');
            $("#MemberLocationInformation_0__CityName").val('');
            $("#MemberLocationInformation_0__AddressLine2").val('');
            $("#MemberLocationInformation_0__PostalCode").val('');
            $("#MemberLocationInformation_0__AddressLine1").val('');
            $("#BillingMemberCountryCode").val('');
            $("#MemberLocationInformation_0__AdditionalTaxVatRegistrationNumber").val('');
            $("#MemberLocationInformation_0__SubdivisionCode").val('');
            SetControlAccessibility(true, true);
        } else {

            $("#MemberLocationInformation_1__OrganizationName").val('');
            $("#MemberLocationInformation_1__TaxRegistrationId").val('');
            $("#MemberLocationInformation_1__CompanyRegistrationId").val('');
            $("#MemberLocationInformation_1__AddressLine3").val('');
            $("#MemberLocationInformation_1__CityName").val('');
            $("#MemberLocationInformation_1__AddressLine2").val('');
            $("#MemberLocationInformation_1__PostalCode").val('');
            $("#MemberLocationInformation_1__AddressLine1").val('');
            $("#BilledMemberCountryCode").val('');
            $("#MemberLocationInformation_1__AdditionalTaxVatRegistrationNumber").val('');
            $("#MemberLocationInformation_1__SubdivisionCode").val('');
            SetControlAccessibility(true, false);
        }
    } 

}

function SetControlAccessibility(setdisable, isBilling) {
  if (setdisable) {
    if (isBilling) {
      $('#BillingMemberReference input[type=text]').attr('disabled', 'disabled');
      $('#BillingMemberReference select[id!=MemberLocationInformation_0__MemberLocationCode]').attr('disabled', 'disabled');
      // $('#BillingMemberReference textarea').attr('disabled', 'disabled');
    }
    else {
      $('#BilledMemberReference input[type=text]').attr('disabled', 'disabled');
      $('#BilledMemberReference select[id!=MemberLocationInformation_1__MemberLocationCode]').attr('disabled', 'disabled');
      //   $('#BilledMemberReference textarea').attr('disabled', 'disabled');
    }
  }
  else {
    if (isBilling) {
      $('#BillingMemberReference input[type=text]').removeAttr('disabled');
      $('#BillingMemberReference select').removeAttr('disabled');
      //  $('#BillingMemberReference textarea').removeAttr('disabled');
    }
    else {
      $('#BilledMemberReference input[type=text]').removeAttr('disabled');
      $('#BilledMemberReference select').removeAttr('disabled');
      //   $('#BilledMemberReference textarea').removeAttr('disabled');
    }
  }
}

function closeBillingReferenceDataPopup(isInitialize) {
  // debugger;
  if (isInitialize == null || isInitialize != true)
    $('#BillingMemberLocationCode').val($('#MemberLocationInformation_0__MemberLocationCode').val());
  $("#billingMemberReferenceContainer").html('');


  if ($('#MemberLocationInformation_0__MemberLocationCode').val() == '') {
    isDetailSetFromBlankLocationCode = true;
  }
  else {
    isDetailSetFromBlankLocationCode = false;
  }

  isBillingLegalTextSet = true;
  copyToMemory(true);
  var cloneBillingMemRef = $("#BillingMemberReference").clone(true);
  cloneBillingMemRef.find("input[type=text]").each(function (i) {
    var $currentElem = $(this);
    $currentElem.attr("name", $currentElem.attr("name") + '1');
    $currentElem.attr("id", $currentElem.attr("id") + '1');
    $currentElem.attr("disabled", false);
    $currentElem.appendTo("#billingMemberReferenceContainer"); //append within form
  });

  //cloning for dropdown. (separately cloning for issue in FF.)
  var countryClone = $('#MemberLocationInformation_0__OrganizationName').clone(true);
  countryClone.attr("name", "BillingMemberCountryCode1");
  countryClone.attr("id", "BillingMemberCountryCode1");
  countryClone.attr("disabled", false);
  countryClone.val($('#BillingMemberCountryCode').val());
  countryClone.appendTo("#billingMemberReferenceContainer"); //append within form
    
    
  var countryNameClone = $('#MemberLocationInformation_0__OrganizationName').clone(true);
  countryNameClone.attr("name", "BillingMemberCountryName");
  countryNameClone.attr("id", "BillingMemberCountryName");
  countryNameClone.attr("disabled", false);
  countryNameClone.val($('#BillingMemberCountryCode option:selected').text());
  countryNameClone.appendTo("#billingMemberReferenceContainer"); //append within form

  //cloning for textarea. (separately cloning for issue in FF.)
  var footerClone = $('#MemberLocationInformation_0__LegalText').clone(true);
  footerClone.attr("name", "MemberLocationInformation[0].LegalText1");
  footerClone.attr("id", "MemberLocationInformation_0__LegalText1");
  footerClone.attr("disabled", false);
  footerClone.val($('#MemberLocationInformation_0__LegalText').val());
  
  footerClone.appendTo("#billingMemberReferenceContainer"); //append within form

  //  }
}

function closeBilledReferenceDataPopup(isInitialize) {

  if (isInitialize == null || isInitialize != true)
    $('#BilledMemberLocationCode').val($('#MemberLocationInformation_1__MemberLocationCode').val());
  $("#billedMemberReferenceContainer").html('');

  //if ($('#MemberLocationInformation_1__MemberLocationCode').val() == '') {
  copyToMemory(false);
  var cloneBilledMemRef = $("#BilledMemberReference").clone(true);
  cloneBilledMemRef.find("input[type=text]").each(function (i) {
    var $currentElem = $(this);
    $currentElem.attr("name", $currentElem.attr("name") + '1');
    $currentElem.attr("id", $currentElem.attr("id") + '1');
    $currentElem.attr("disabled", false);
    $currentElem.appendTo("#billedMemberReferenceContainer"); //append within form
  });

  //cloning for dropdown. (separately cloning for issue in FF.)
  
  var countryClone = $('#MemberLocationInformation_1__OrganizationName').clone(true);
  countryClone.attr("name", "BilledMemberCountryCode1");
  countryClone.attr("id", "BilledMemberCountryCode1");
  countryClone.attr("disabled", false);
  countryClone.val($('#BilledMemberCountryCode').val());
  countryClone.appendTo("#billedMemberReferenceContainer"); //append within form

  var countryNameClone = $('#MemberLocationInformation_1__OrganizationName').clone(true);
  countryNameClone.attr("name", "BilledMemberCountryName");
  countryNameClone.attr("id", "BilledMemberCountryName");
  countryNameClone.attr("disabled", false);
  countryNameClone.val($('#BilledMemberCountryCode option:selected').text());
  countryNameClone.appendTo("#billedMemberReferenceContainer"); //append within form

  //}
}

function closeMemberReferenceDialog(divName, controlIdToSetFocus) {
  $(divName).dialog('close');
  // Set focus on control
  $(controlIdToSetFocus).focus();
}


function AddExclimationInOrganizationName(orgNameDb, controlId) {
    var orgNameDbWithoutEx = orgNameDb.replace("!!!", "");
    var orgNameWithoutEx = $(controlId).val();
    if (orgNameDbWithoutEx == orgNameWithoutEx) {
        $(controlId).val(orgNameDb);
    } else {
        len = orgNameWithoutEx.length;
        if (orgNameWithoutEx.indexOf("!!!") == -1 && len > 50) {
            var strOrgName1 = orgNameWithoutEx.substring(0, 50);
            var strOrgName2 = orgNameWithoutEx.substring(50, len);
            $(controlId).val(strOrgName1 + "!!!" + strOrgName2);
        } else {
            $(controlId).val(orgNameWithoutEx);
        }
    }
}

// Following code is executed when user closes Billing Member Location Popup
$("#BillingMemberReference").bind("dialogclose", function (event, ui) {
  // Set focus on BillingMember Location dropdown box
  $('#BillingMemberLocationCode').focus();
});

// Following code is executed when user closes Billed Member Location Popup
$("#BilledMemberReference").bind("dialogclose", function (event, ui) {
  // Set focus on BilledMember Location dropdown box
  $('#BilledMemberLocationCode').focus();
});

function PopulatedBilledMemberLocationDetails() {
  //debugger;
  if ($("#BilledMemberLocationCode").val() == "") {
    SetControlAccessibility(false, false);
    ResetDivInputs('#BilledMemberReference');
  }
  else {
    var LocationCode = $("#BilledMemberLocationCode").val();
    GetMemberLocationForCreateMode(LocationCode, $("#BilledMemberId").val(), false);
  }
  $('#MemberLocationInformation_1__MemberLocationCode').val($("#BilledMemberLocationCode").val());
}

// for clearing values.
function ResetDivInputs(divId) {
  $(divId + ' :input').not(':submit').not(':button').removeAttr('value');
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
