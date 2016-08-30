<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<div class="searchCriteria">
<!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Increasing field size from 85% to 120% to keep layout intact -->
  <div class="solidBox" style="width: 120%" >
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.SupportingDocBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, Model.SupportingDocumentTypeId)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(m => m.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            <span>*</span> Billed Member:</label>
          <%:Html.HiddenFor(m => m.BilledMemberId)%>
          <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: Increasing field size by specifying in-line width
            Ref: 3.5 Table 19 Row 1 -->
          <%:Html.TextBoxFor(m => m.BilledMemberText, new { @class = "autocComplete textboxWidth" })%>
        </div>
        <div>
          <label>
            Invoice Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
        <div>
          <label for="ChargeCategory">
            Charge Category:</label>
            <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
          <%: Html.ChargeCategoryDropdownList(ControlIdConstants.ChargeCategory, Model.ChargeCategoryId, Iata.IS.Model.Enums.BillingCategoryType.Misc, isIncludeInactive: true)%>
        </div>
      </div>
      <div>
        <div>
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator Original:
          </label>
          <%:Html.SupportingDocAttachmentIndicatorDropdownList(m => m.AttachmentIndicatorOriginal,"Misc")%>
        </div>
        <div>
          <label for="IsMismatchCases">
            Mismatch Cases:
          </label>
          <%:Html.CheckBoxFor(m => m.IsMismatchCases)%>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="button" onclick="resetSearchCriteria();" value="Clear" />
</div>
<div class="clear">
</div>
