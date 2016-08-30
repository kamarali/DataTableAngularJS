<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Location>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<script type="text/javascript">

 $('#CountryId').change(function () {
    $('#SubDivisionName').val("");
     $('#SubDivisionName').flushCache();
     $('#CountryIdDisplayValue').val('');
     $('#CountryIdFutureDisplayValue').val($('#CountryId option:selected').text());
    });

      $('#IsUatpLocation').change(function() {
      if(this.checked )
      {
        $('#IsActive').prop('checked', true);
        $("#IsActive").attr('disabled', 'disabled');
        $('#fileSpecLocReq').prop('checked', false);
        $("#fileSpecLocReq").attr('disabled', 'disabled');
         $("#LocSpecific").hide();
      }
      else {
      $('#IsActive').removeAttr("disabled");
       $('#fileSpecLocReq').removeAttr("disabled");
        $("#LocSpecific").show();
      }
      
    });
    $(document).ready(function () {
    // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveLocation").attr('disabled', 'disabled');   
    $("#btnSaveLocation").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveLocation").removeAttr('disabled'); 
     $("#btnSaveLocation").addClass('ignoredirty');     
   }

    
    $('#selLocationCode').change(function() {
     viewLocationDetails('<%:Url.Action("GetMemberLocationDetails", "Member", new { area = "Profile" })%>','selLocationCode');
    });
    $("#LocationIdFlag").val("0");
    
    $('#selLocationCode').focus();
     $('#IsActive').prop('checked', true);
      <%
        if (Model.Id > 0)
        {%>
        $(".futureEditLink").show();
        $('.currentFieldValue').attr("disabled", true);
        $('#IsUatpLocation').prop('checked', true);
        $("#IsUatpLocation").attr('disabled', 'disabled');
    <%
        }%>
    registerAutocomplete('SubDivisionName', 'SubDivisionName', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = "" })%>', 0, true, null, '', '', '#CountryId');
    SetLocationDataUrl('<%:Url.Action("GetMemberLocationList", "Member", new { area = "Profile" , selectedMemberId = Model.MemberId})%>');
    });
 

  
</script>
<%
    Html.BeginForm("Location", "Member", FormMethod.Post, new { id = "location" });%>
    <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <h2>
                    Search Existing Location:</h2>
                <%:Html.LocationIdDropdownListFor(memberLocation => memberLocation.LocationCode, Model.MemberId, new { @id = "selLocationCode", @class = "locationField" })%>
            </div>
            <div class="buttonContainer">
                <input class="primaryButton" type="button" value="View" id="btnView" onclick="viewLocationDetails('<%:Url.Action("GetMemberLocationDetails", "Member", new { area = "Profile" })%>','selLocationCode');" />
                <input class="secondaryButton" type="button" value="Add Location" onclick="AddLocationDetails();" />
            </div>
        </div>
        <div class="topLine">
            <h2>
                Location Details</h2>
            <%:Html.TextBoxFor(memberLocation => memberLocation.LocationIdFlag, new { @class = "hidden", id = "LocationIdFlag" })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.MemberLegalName,
                                                        "Member Legal Name",
                                             SessionUtil.UserCategory,
                                                                 new Dictionary<string, object> { { "maxLength", 100 }, { "id", "MemberLegalName" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                   FieldId = "MemberLegalName",
                                                   FieldName = "MemberLegalName",
                                                   FieldType = 1,
                                                   CurrentValue = Model.MemberLegalName,
                                                   FutureValue = Model.MemberLegalNameFutureValue,
                                                 HasFuturePeriod = true,
                                                   FuturePeriod = Model.MemberLegalNameFuturePeriod,
                                                   // TFS_Bug8929: CMP597: System updating null value in Member legal Name 
                                                   IsFieldMandatory = true
                                               })%>
            <%:Html.TextBoxFor(memberLocation => memberLocation.Id, new { @class = "hidden", id = "locationId" })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.MemberCommercialName,
                                             "Member Commercial Name",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", "100" } })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.RegistrationId,
                                             "Company Registration ID",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 25 }, { "id", "RegistrationId" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "RegistrationId",
                                                 FieldName = "RegistrationId",
                                                 FieldType = 1,
                                                 CurrentValue = Model.RegistrationId,
                                                 FutureValue = Model.RegistrationIdFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.RegistrationIdFuturePeriod
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.TaxVatRegistrationNumber,
                                             "Tax/VAT Registration #",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 25 }, { "id", "TaxVatRegistrationNumber" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "TaxVatRegistrationNumber",
                                                 FieldName = "TaxVatRegistrationNumber",
                                                 FieldType = 1,
                                                 CurrentValue = Model.TaxVatRegistrationNumber,
                                                 FutureValue = Model.TaxVatRegistrationNumberFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.TaxVatRegistrationNumberFuturePeriod
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.AdditionalTaxVatRegistrationNumber,
                                             "Add. Tax/VAT Registration #",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 25 }, { "id", "AdditionalTaxVatRegistrationNumber" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "AdditionalTaxVatRegistrationNumber",
                                                 FieldName = "AdditionalTaxVatRegistrationNumber",
                                                 FieldType = 1,
                                                 CurrentValue = Model.AdditionalTaxVatRegistrationNumber,
                                                 FutureValue = Model.AdditionalTaxVatRegistrationNumberFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.AdditionalTaxVatRegistrationNumberFuturePeriod
                                               })%>
        </div>
        <div class="topLine">
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.AddressLine1,
                                             "Address Line1",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 70 }, { "id", "AddressLine1" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "AddressLine1",
                                                 FieldName = "AddressLine1",
                                                 FieldType = 1,
                                                 CurrentValue = Model.AddressLine1,
                                                 FutureValue = Model.AddressLine1FutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.AddressLine1FuturePeriod,
                                                 IsFieldMandatory=true
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.AddressLine2,
                                             "Address Line2",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 70 }, { "id", "AddressLine2" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "AddressLine2",
                                                 FieldName = "AddressLine2",
                                                 FieldType = 1,
                                                 CurrentValue = Model.AddressLine2,
                                                 FutureValue = Model.AddressLine2FutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.AddressLine2FuturePeriod
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.AddressLine3,
                                             "Address Line3",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 70 }, { "id", "AddressLine3" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "AddressLine3",
                                                 FieldName = "AddressLine3",
                                                 FieldType = 1,
                                                 CurrentValue = Model.AddressLine3,
                                                 FutureValue = Model.AddressLine3FutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.AddressLine3FuturePeriod
                                               })%>
        </div>
        <div>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.CityName,
                                             "City Name",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 50 }, { "id", "CityName" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "CityName",
                                                 FieldName = "CityName",
                                                 FieldType = 1,
                                                 CurrentValue = Model.CityName,
                                                 FutureValue = Model.CityNameFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.CityNameFuturePeriod,
                                                 IsFieldMandatory=true
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.PostalCode,
                                             "Postal Code",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 50 }, { "id", "PostalCode" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "PostalCode",
                                                 FieldName = "PostalCode",
                                                 FieldType = 1,
                                                 CurrentValue = Model.PostalCode,
                                                 FutureValue = Model.PostalCodeFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.PostalCodeFuturePeriod
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.CountryId,
                                             "Country Name",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "id", "CountryId" }, { "class", "currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "CountryId",
                                                 FieldName = "CountryId",
                                                 FieldType = 8,
                                                 CurrentValue = Model.CountryId,
                                                 CurrentDisplayValue = Model.CountryIdDisplayValue,
                                                 FutureValue = Model.CountryIdFutureValue,
                                                 FutureDisplayValue = Model.CountryIdFutureDisplayValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.CountryIdFuturePeriod,
                                                 IsFieldMandatory=true
                                               })%>
        </div>
        <div class="bottomLine">
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.SubDivisionName,
                                             "Subdivision Name",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", 50 }, { "id", "SubDivisionName" }, { "class", "autocComplete currentFieldValue" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "SubDivisionName",
                                                 FieldName = "SubDivisionName",
                                                 FieldType = 4,
                                                 CurrentValue = Model.SubDivisionName,
                                                 FutureValue = Model.SubDivisionNameFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.SubDivisionNameFuturePeriod
                                               })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.IsUatpLocation, "UATP Location", SessionUtil.UserCategory, new Dictionary<string, object> { { "disabled", "disable" } })%>
         
                Active : <br />
                 <%: Html.ProfileFieldFor(memberLocation => memberLocation.IsActive, "Active",
                                              SessionUtil.UserCategory,
                                              new Dictionary<string, object> { { "id", "IsActive" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
                                {
                                    FieldId = "IsActive",
                                    FieldName = "IsActive",
                                  FieldType = 2,
                                    CurrentValue = Model.IsActive != null ? Model.IsActive.ToString() : string.Empty,
                                    FutureValue = Model.IsActiveFutureValue != null ? Model.IsActiveFutureValue.ToString() : string.Empty,
                                  HasFuturePeriod = true,
                                    FuturePeriod = Model.IsActiveFuturePeriod != null ? Model.IsActiveFuturePeriod.ToString() : string.Empty
                                },null,true)%>


          
        </div>
    </div> <br />
    <%--CMP#622: MISC Outputs Split as per Location IDs--%>
   <div class= "fieldContainer horizontalFlow" id= "LocSpecific">
        
            <h2>
                Miscellaneous Output Files Specific to this Location</h2>
            <div>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.FileSpecificLocReq,
                                             "Files Specific to this Location Required",
                                             SessionUtil.UserCategory,
                                                new Dictionary<string, object> { { "id", "fileSpecLocReq" } }
                                             )%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.LociiNetAccountId,
                                             "iiNet Account ID for this Location",
                                             SessionUtil.UserCategory,
                                                new Dictionary<string, object> { { "class", "alphaNumeric" }, { "id", "lociiNetAccId" }, { "maxLength", "50" }, { "style", "width:180px" } })%>
            </div>
            <div class="bottomLine">
            If files specific to this Location are required and an iiNet Account ID is not defined, they will be delivered to the Main Miscellaneous iiNet Account (if defined)
            </div>
               
    </div> <br />
    

    <div class="fieldContainer horizontalFlow">
        <h2>
            Invoice Footer</h2>
        <div class="bottomLine">
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.LegalText,
                                             "The below text will appear on invoices billed from this location",
                                             SessionUtil.UserCategory,
                                                       new Dictionary<string, object> { { "id", "LegalText" }, { "class", "currentFieldValue" }, { "maxLength", 700 }, { "rows", "5" }, { "cols", "170" } },
                                             null,
                                             new FutureUpdate
                                               {
                                                 FieldId = "LegalText",
                                                 FieldName = "LegalText",
                                                 FieldType = 5,
                                                 CurrentValue = Model.LegalText,
                                                 FutureValue = Model.LegalTextFutureValue,
                                                 HasFuturePeriod = true,
                                                 FuturePeriod = Model.LegalTextFuturePeriod
                                               },
                                             new Dictionary<string, object> { { "style", "width: 700px;" } })%>
        </div>
    </div>
    <div class="clear">
    </div>
    <div class="fieldContainer horizontalFlow">
        <h2>
            Bank Details for Bilateral Settlement</h2>
        <div>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.BankAccountName, "Bank Account Name", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "80" }, { "style", "width:180px" } })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.BankAccountNumber,
                                             "Bank Account Number",
                                             SessionUtil.UserCategory,
                                                                 new Dictionary<string, object> { { "class", "alphaNumeric" }, { "maxLength", "80" }, { "style", "width:180px" } })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.BankName,
                                             "Bank Name",
                                             SessionUtil.UserCategory,
                                             new Dictionary<string, object> { { "maxLength", "100" }, { "id", "locBankName" } })%>
        </div>
        <div>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.BranchCode, "Branch Code", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "80" }, { "style", "width:180px" } })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.BankCode, "Bank Code", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "80" }, { "style", "width:180px" } })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.CurrencyId, "Currency Code", SessionUtil.UserCategory)%>
        </div>
        <div>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.Iban, "IBAN", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "80" }, { "style", "width:180px" } })%>
            <%:Html.ProfileFieldFor(memberLocation => memberLocation.Swift, "SWIFT", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "alphaNumeric" }, { "maxLength", "11" }, { "style", "width:180px" } })%>
            <%:Html.Hidden("MemberId", Model.MemberId)%>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
<div class="buttonContainer">
    <div>
        <input class="primaryButton" id="btnSaveLocation" type="submit" value="Save Location" />
    </div>
    <div class="futureUpdatesLegend">
        Future Updates Pending</div>
</div>
<%
    Html.EndForm();%>
