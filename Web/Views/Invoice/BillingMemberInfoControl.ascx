<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Base.InvoiceBase>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<h2>
  Billing Member's Reference Data</h2>
<form id="BillingMemberReferenceForm" action="" method="post">
<div class="solidBox dataEntry" id="ReferenceDataFields">
  <div class="fieldContainer verticalFlow">
    <div class="oneColumn">
      <div>
        <label for="LocationId">
          Location ID:</label>
        <%
            string defaultLocation = (Model.BillingCategory == BillingCategoryType.Misc) ? "Main" : "UATP";%>
        <%:Html.MemberLocationIdDropdownList(ControlIdConstants.BillingMemberReferenceLocationCode,
                                                        Model.MemberLocationInformation[0].MemberLocationCode,
                                                        Model.BillingMemberId,
                                                        Model.BillingCategory,
                                                        MemberType.Billing,
                                                        new {@class = "mediumTextField populated"},
                                                        defaultLocation,
                                                        ViewData[ViewDataConstants.PageMode] != null &&
                                                        (ViewData[ViewDataConstants.PageMode].ToString() ==
                                                         PageMode.Edit ||
                                                         ViewData[ViewDataConstants.PageMode].ToString() ==
                                                         PageMode.View)
                                                            ? true
                                                            : false,
                                                        (Model.MemberLocationInformation != null &&
                                                         Model.MemberLocationInformation.Count == 2)
                                                            ? Model.MemberLocationInformation[0]
                                                            : null)%>
        <%--CMP #655: IS-WEB Display per Location ID--%>
        <% if (Model.BillingCategory != BillingCategoryType.Misc) {%>
        <input type="button" value="Clear Data" class="primaryButton" id="BillingMemberClear" />
        <%
            }%>
      </div>
    </div>
    <div class="oneColumn">
      <div class="fieldSeparator">
      </div>
    </div>
  </div>
  <div id="divData">
    <div class="fieldContainer verticalFlow">
      <div class="halfWidthColumn">
        <div>
          <label for="OrganizationName1">
            <span>*</span> Company Legal Name:</label>
            <!--
            SCP:47630 - Unable to submit "Miscellaneous" rejection
            set textbox length to 100 chars 
            -->
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].OrganizationName, new { @class = "largeTextField populated", maxLength = 100 })%>
        </div>
        <div>
          <label for="CompanyRegistrationID">
            Company Registration ID:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].CompanyRegistrationId, new { maxLength = 25 })%>
        </div>
      </div>
      <div class="halfWidthColumn">
        <div>
          <label for="TaxRegistrationID">
            Tax/VAT Registration #:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].TaxRegistrationId, new { maxLength = 25 })%>
        </div>
        <div>
          <label for="AdditionalTaxVatRegistrationNumber">
            Add. Tax/VAT Registration #:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].AdditionalTaxVatRegistrationNumber, new { maxLength = 25 })%>
        </div>
      </div>
      <div class="oneColumn">
        <div class="fieldSeparator">
        </div>
      </div>
    </div>
    <div class="fieldContainer verticalFlow">
      <div class="halfWidthColumn">
        <div>
          <label for="Address1">
            <span>*</span> Address Line 1:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].AddressLine1, new { @class = "largeTextField", maxLength = 70 })%>
        </div>
        <div>
          <label for="Address2">
            Address Line 2:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].AddressLine2, new { @class = "largeTextField", maxLength = 70 })%>
        </div>
        <div>
          <label for="Address3">
            Address Line 3:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].AddressLine3, new { @class = "largeTextField", maxLength = 70 })%>
        </div>
        <div>
          <label for="CityName">
            <span>*</span> City:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].CityName, new { maxLength = 50 })%>
        </div>
      </div>
      <div class="halfWidthColumn">
        <div>
          <label for="CountryCode">
            <span>*</span> Country:</label>
          <%:Html.CountryCodeDropdownList(ControlIdConstants.BillingMemberCountryCode, Model.MemberLocationInformation[0].CountryCode)%>
        </div>
        <div>
          <label for="SubdivisionName">
            State/Province/Region:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].SubdivisionName, new { @class = "autocComplete" })%>
        </div>
        <div>
          <label for="PostalCode">
            Postal Code:</label>
          <%:Html.TextBoxFor(model => model.MemberLocationInformation[0].PostalCode, new { @class = "smallTextField", maxLength = 50 })%>
        </div>        
      </div>
    </div>
    <div class="fieldContainer verticalFlow">
      <div class="oneColumn">
        <div>
          <label for="InvoiceFooterDataLine1">
            Invoice Footer Information:</label>
          <%:Html.TextAreaFor(model => model.MemberLocationInformation[0].LegalText, new { @class = "largeTextField notValidCharsTextarea" })%>
        </div>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" value="Ok" type="submit" id="BillingMemberClose" />
  <input class="secondaryButton ignoredirty" value="Cancel" type="button" onclick="closeMemberReferenceDialog('#BillingMemberReference', '#BillingMemberLocationCode')" />
</div>
</form>
