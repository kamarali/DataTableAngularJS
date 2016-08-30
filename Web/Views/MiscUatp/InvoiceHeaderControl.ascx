<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
  Header</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Receivables)
          {%>
        <label>
          <span>*</span> Billed Member:</label>
          <!--  CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete textboxWidth" })%>
        <%}
          else
          {%>
        <label>
          Billing Member:</label>
        <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Increasing field size by specifying in-line width
        Ref: 3.5 Table 19 Row 8,10 -->
        
        <%:Html.TextBoxFor(invoice => invoice.BillingMemberText, new { @readOnly = true, @class = "textboxWidth" })%>
        <%}%>
        <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
        <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
        <%:Html.HiddenFor(invoice => invoice.IsCreatedFromBillingHistory)%>
        <%:Html.HiddenFor(invoice => invoice.AttachmentIndicatorOriginal) %>
        <%--CMP#648: Exchange rate should be remain empty if SMI is bilateral or like bilateral.--%>
        <%:Html.Hidden("NonBilateralSMICurrenyList", EnumMapper.GetBillingCurrencyList().Aggregate(string.Empty, (current, item) => current + ("option1 value='" + item.Value + "'option2" + item.Text + "option3")))%>
        <%-- SCP0000:Impact on MISC/UATP rejection linking due to purging --%>
        <%:Html.HiddenFor(invoice => invoice.ExpiryDatePeriod)%>
        <%if (Model.ExpiryDatePeriod != null && Model.ExpiryDatePeriod==new DateTime(1973,1,1))
        {%>
         <%:Html.Hidden("IsPurged",true)%>
        <%}
        else
        {%>
        <%:Html.Hidden("IsPurged",false)%>
        <%}%>
      </div>
      <div>
        <label for="InvoiceNumber">
          <span>*</span>
          <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
          Number:</label>
        <%:Html.TextBoxFor(invoice => invoice.InvoiceNumber, new { maxLength = 10, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          <span>*</span>
          <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
          Date:</label>
        <%: Html.TextBox(ControlIdConstants.InvoiceDate, Model.InvoiceDate.ToString(FormatConstants.DateFormat), new { @class = "datePicker populated" })%>
      </div>
      <div>
        <label for="ChargeCategory">
          <span>*</span> Charge Category:</label>
        <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
        <%: Html.ChargeCategoryDropdownList(ControlIdConstants.ChargeCategory, Model.ChargeCategoryId, Model.BillingCategory,
        isIncludeInactive: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View) ? true : false)%>
      </div>
      <% if (Model.BillingCategory == BillingCategoryType.Misc)
         {%>
      <div>
        <label for="Airport">
          Location (Airport/City Code):</label>
        <%:Html.TextBoxFor(invoice => invoice.LocationCode, new { maxLength = 5, @class = "alphaNumeric upperCase" })%>
      </div>
      <%}%>
    </div>
    <div>
      <div>
        <label for="PONumber">
          P.O. Number:</label>
        <%: Html.TextBoxFor(invoice => invoice.PONumber, new { maxLength = 35 })%>
      </div>
      <div>
        <!--CMP#IS-WEB Display per Location ID 2.5	MISC IS-WEB RECEIVABLES - INVOICE CAPTURE SCREEN-->
        <span>*</span> 
        <%: ScriptHelper.GenerateDialogueHtml("Billing Member's Location ID:", "Billing Member Reference Data", "BillingMemberReference", 510, 700)%>
        <%var defaultLocation = (Model.BillingCategory == BillingCategoryType.Misc) ? "Main" : "UATP";  %>
        <%: Html.MemberLocationIdDropdownList(ControlIdConstants.BillingMemberLocationCode, Model.BillingMemberLocationCode, Model.BillingMemberId, Model.BillingCategory, MemberType.Billing, new { @class = "mediumTextField" }, defaultLocation, ViewData[ViewDataConstants.PageMode] != null && (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Edit || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false, (Model.MemberLocationInformation != null && Model.MemberLocationInformation.Count == 2) ? Model.MemberLocationInformation[0] : null)%>
      </div>
      <div>
        <%: ScriptHelper.GenerateDialogueHtml("Billed Member's Location ID:", "Billed Member Reference Data", "BilledMemberReference", 420, 700, "BilledMemberLocLink")%>
        <%: Html.MemberLocationIdDropdownList(ControlIdConstants.BilledMemberLocationCode, Model.BilledMemberLocationCode, Model.BilledMemberId, Model.BillingCategory, MemberType.Billed, new { @class = "mediumTextField" }, defaultLocation, ViewData[ViewDataConstants.PageMode] != null && (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Edit || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false, (Model.MemberLocationInformation != null &&  Model.MemberLocationInformation.Count == 2) ? Model.MemberLocationInformation[1] : null)%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Contact of Billing Member:", "Contact of Billing Member - Additional Details", "BillingAdditionalDetails", 490, 650)%><br />
        <%: Html.TextBoxFor(invoice => invoice.BillingMemberContactName, new { @class = "smallTextField" })%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Contact of Billed Member:", "Contact of Billed Member - Additional Details", "BilledAdditionalDetails", 490, 650)%><br />
        <%: Html.TextBoxFor(invoice => invoice.BilledMemberContactName, new { @class = "smallTextField" })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          <span>*</span> Settlement Method:</label>
        <%:Html.SettlementMethodDropdownList(ControlIdConstants.SettlementMethodIdDropDown, Model.SettlementMethodDisplayText, Model.InvoiceType, TransactionMode.Transactions)%>
        <%: Html.Hidden(ControlIdConstants.SettlementMethodIdDropDown + "ForView", Model.SettlementMethodId)%>
      </div>
      <div>
        <label>
          <span>*</span> Billing Year/Month/Period:</label>
        <% if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
           { %>
        <%: Html.BillingYearMonthPeriodDropdown(ControlIdConstants.BillingYearMonthPeriodDropDown, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod)%>
        <% }
           else
           { %>
        <%: Html.BillingYearMonthPeriodTextBox(ControlIdConstants.BillingYearMonthPeriodDropDown, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod)%>
        <% } %>
      </div>
      <div id="divListingCurrecy">
        <label for="ListingCurrencyId">
          <span>*</span> Currency of Billing:</label>
        <%:Html.MiscBillingCurrencyDropdownList(ControlIdConstants.ListingCurrencyId, Model.ListingCurrencyId.HasValue ? Model.ListingCurrencyId.Value : 0, Model.BillingMemberId)%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Tax Amount:", "Tax Breakdown", "TaxBreakdown", 500, 900)%><br />
        <%:Html.TextBox(ControlIdConstants.TotalMiscTaxAmount, Model.InvoiceSummary.TotalTaxAmount, new { id = "TaxAmount", @class = "smallTextField num_18_3 amount", @readOnly = true })%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Amount:", "VAT Breakdown", "VATBreakdown", 750, 950)%><br />
        <%:Html.TextBox(ControlIdConstants.TotalMiscVatAmount, Model.InvoiceSummary.TotalVatAmount, new { id = "VatAmount", @class = "smallTextField num_18_3 amount", @readOnly = true })%>
      </div>
    </div>
    <div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Add/Deduct Charge:", "Add/Deduct Charge Breakdown", "AddChargeBreakdown", 500, 900)%><br />
        <%:Html.TextBox(ControlIdConstants.TotalAddOnChargeAmount, Model.InvoiceSummary.TotalAddOnChargeAmount, new { id = "TotalAddChargeAmount", @class = "smallTextField num_18_3 amount", @readOnly = true })%>
      </div>
      <div>
        <label for="Total">
          Total Amount in Billing Currency:</label>
        <%:Html.TextBox(ControlIdConstants.AmountInBillingCurrency, Model.InvoiceSummary.TotalAmount, new { @readOnly = true, @class = "num_18_3 amount" })%>
      </div>
      <%--CMP#624 : Change 3=> Swaping position Clearance currency and exchange rate--%>
      <%--CMP#648 : Show Clearance currency for MISC if SMI like Bileateral--%>
      <div id="divClearanceCurrency">
        <label for="BillingCurrencyId">
          <span id = "RequiredindBC">*</span> Currency of Clearance:</label>
        <%:Html.BillingCurrencyDropdownList(ControlIdConstants.BillingCurrencyId, Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, null, Model.SettlementMethodId)%>
      </div>

      <div id="divExchangeRate">
        <label>
          <span id = "RequiredindEX">*</span> Exchange Rate:</label>
        <%--<%:Html.TextBox(ControlIdConstants.ListingToBillingRate, Model.ListingToBillingRate.ToString(FormatConstants.ExchangeRateEditFormat), new { @readOnly = true })%>--%>
        <%--280744 - MISC UATP Exchange Rate population/validation during error Desc: Function to display exchange rate as NULL/blank instead of 0.--%>
        <%:Html.TextBox(ControlIdConstants.ListingToBillingRate, Model.ListingToBillingRate.HasValue ? Model.ListingToBillingRate.Value.ToString(FormatConstants.ExchangeRateEditFormat) : string.Empty, new { @readOnly = true })%>
      </div>

      <div class="hidden">
        <%:Html.BillingCurrencyDropdownList(ControlIdConstants.BillingCurrencyId, Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, new { id = "ClearanceCurrency" }, Model.SettlementMethodId)%>
        <%:Html.HiddenFor(invoice => invoice.BillingCurrencyId, new { id = "HiddenBillingCurrencyId" })%>
        <%:Html.HiddenFor(invoice => invoice.IsValidationFlag)%>
        <%--CMP #553: ACH Requirement for Multiple Currency Handling--%>
        <%:Html.BillingAchCurrencyDropdownList("HiddenAchBillingCurrency", Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, new { id = "HiddenAchBillingCurrency" }, Model.SettlementMethodId)%>
      </div>
      <div id="divTotalAmountInClearanceCurrency">
        <label for="AmountInClearanceCurrency">
          Total Amount in Clearance Currency:</label>        
        <%:Html.TextBox(ControlIdConstants.AmountInClearanceCurrency, Model.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue ? Model.InvoiceSummary.TotalAmountInClearanceCurrency.Value == 0.0M ? (decimal?)null : Model.InvoiceSummary.TotalAmountInClearanceCurrency.Value : (decimal?)null, new { @readOnly = true, @class = "num_18_3 amount" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label for="DigitalSignature">
          Digital Signature:</label>
        <%:Html.DigitalSignatureDropdownList(ControlIdConstants.DigitalSignatureRequiredId, Model.DigitalSignatureRequiredId.ToString())%>
        <%:Html.Hidden("DigitalSignatureFlagId", ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId])%>
      </div>
       
        <!-- CMP #624: ICH Rewrite-New SMI X 
        Change #1: New fields ‘CH Agreement Indicator’ and ‘CH Due Date’ -->
	    <div>
		    <label>CH Agreement Indicator:</label>
		    <%:Html.TextBoxFor(invoice => invoice.ChAgreementIndicator, new { maxLength = 5 })%>
	    </div>
	    <div>
		    <label>CH Due Date:</label>
             <%: Html.TextBox(ControlIdConstants.ChDueDate, (Model.ChDueDate.HasValue == true ? Model.ChDueDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
	    </div>  
  
      <% if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.Edit || (string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
         {%>
      <div>
        <label for="InvoiceOwnerDisplayText">
          <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
          Owner:</label>
        <%:Html.TextBoxFor(invoice => invoice.InvoiceOwnerDisplayText, new { @readOnly = true })%>
        <%:Html.HiddenFor(invoice => invoice.InvoiceOwnerId)%>
      </div>
      <%
         }%>
           <%if (SystemParameters.Instance.General.IsMultilingualAllowed)
        {%>
            <div>
                <label>Invoice Template Language: </label>
                <%: Html.LanguageDropdownList(ControlIdConstants.InvTemplateLanguage,
                                                                Model.InvTemplateLanguage) %>
            </div>
        <%}%>        
    </div>
    <div class="payment hidden">
      <h2>
        Payment Details</h2>
      <div>
        <label>
          Payment Terms Code:</label>
        <%: Html.TextBoxFor(invoice => invoice.PaymentDetail.PaymentTermsType, new { @class = "alphaNumeric", maxLength = 25 })%>
      </div>
      <div class="paymentField">
        <div>
          <label>
            Payment due by&nbsp;</label>
          <div class="paymentFieldTextWithin">
            <label>
              within&nbsp;</label>
          </div>
        </div>
        <div>
          <%: Html.TextBox(ControlIdConstants.NetDueDate, Model.PaymentDetail.NetDueDate != null ? Model.PaymentDetail.NetDueDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
          <div>
            <%: Html.TextBoxFor(invoice => invoice.PaymentDetail.NetDueDays, new { @class = "numeric amountTextfield", maxLength = 3 })%>
            &nbsp;days
          </div>
        </div>
      </div>
      <div class="discountField">
        <div>
          <label>
            Get discount of
          </label>
          <%: Html.TextBoxFor(invoice => invoice.PaymentDetail.DiscountPercent, new { @class = "num_5_2 amountTextfield", roundTo = 2 })%>
          <label>
            % in case payment made by &nbsp;</label>
          <div class="paymentFieldText">
            <label style="display: inline">
              within&nbsp;</label>
          </div>
        </div>
        <div>
          <%: Html.TextBox(ControlIdConstants.DiscountDueDate, Model.PaymentDetail.DiscountDueDate != null ? Model.PaymentDetail.DiscountDueDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
          <div>
            <%: Html.TextBoxFor(invoice => invoice.PaymentDetail.DiscountDueDays, new { @class = "numeric amountTextfield", maxLength = 3 })%>
            &nbsp;days
          </div>
        </div>
      </div>
    </div>
    <div class="payment hidden">
      <div>
        <label>
          Remarks:</label>
        <%: Html.TextAreaFor(invoice => invoice.PaymentDetail.Description, 4, 80, new { id = "Remarks" })%>
      </div>
    </div>
      <%
      if (Model.OtherOrganizationInformations != null && Model.OtherOrganizationInformations.Count() > 0 && (Model.InvoiceStatus == InvoiceStatusType.Presented || Model.InvoiceStatus == InvoiceStatusType.ProcessingComplete || Model.InvoiceStatus == InvoiceStatusType.Claimed || Model.InvoiceStatus == InvoiceStatusType.ReadyForBilling))
       { %>
         <%Html.RenderPartial("~/Views/MiscUatp/PaymentDetailsOO.ascx", Model.OtherOrganizationInformations[0]);%>
     <%  }
       else
      { %>
         <%Html.RenderPartial("~/Views/MiscUatp/PaymentDetails.ascx", new Location { Currency = new Currency() });%>
     <%  } %>
  
    <!-- Hide following control in case of UATP invoice -->
    <% if (Model.InvoiceType != InvoiceType.CreditNote)
       { %>
    <div>
      <div>
        <label for="OriginalInvoice">
          Original Invoice:</label>
        <%:Html.RadioButtonFor(invoice => invoice.InvoiceType, InvoiceType.Invoice, new { id = "radOrgInvoice" })%>
      </div>
      <div>
        <label for="RejectionInvoice">
          Rejection Invoice:</label>
        <%:Html.RadioButtonFor(invoice => invoice.InvoiceType, InvoiceType.RejectionInvoice, new { id = "radRejInvoice" })%>
      </div>
      <div>
        <label for="Correspondence Invoice:">
          Correspondence Invoice:</label>
        <%:Html.RadioButtonFor(invoice => invoice.InvoiceType, InvoiceType.CorrespondenceInvoice, new { id = "radCpdInvoice" })%>
      </div>
    </div>
    <div id="divRejectionInvoiceInfo" class="hidden">
      <div>
        <label>
          <span>*</span> Rejected Invoice #:</label>
        <%: Html.TextBoxFor(invoice => invoice.RejectedInvoiceNumber, new { maxLength = 10 })%>
        <span class="hidden rejInvLoader">
          <img src='<%:Url.Content("~/Content/Images/busy.gif") %>' alt="Loading..." />
        </span>
      </div>
      <% if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
         {%>
      <div>
        <label>
          <span>*</span> Billed In:</label>
        <%: Html.BilledInDropdown(ControlIdConstants.BilledIn, Model.SettlementYear, Model.SettlementMonth, htmlAttributes: new { @class = "rejInvNoDep" })%>
        <%: Html.HiddenFor(invoice => invoice.SettlementMonth)%>
        <%: Html.HiddenFor(invoice => invoice.SettlementYear)%>
      </div>
      <%
         }
         else
         {%>
      <div>
        <label>
          <span>*</span> Billed In:</label>
        <%: Html.BilledInTextBox(ControlIdConstants.BilledIn, Model.SettlementYear, Model.SettlementMonth)%>
      </div>
      <%
         }%>
      <div>
        <label>
          <span>*</span> Period:</label>
        <%:Html.StaticBillingPeriodDropdownList("SettlementPeriod", Model.SettlementPeriod, false, new { @class = "rejInvNoDep" })%>
      </div>
      <div>
        <label>
          <span>*</span> Rejection Stage:</label>
        <%:Html.RejectionStageDropdownListFor(invoice => invoice.RejectionStage, TransactionMode.MiscUatpInvoice, new { @class = "rejInvNoDep" })%>
      </div>
    </div>
    <div id="divCorrespondenceInvoiceInfo" class="hidden">
      <div>
        <label>
          <span>*</span> Correspondence Reference #:</label>
        <%: Html.TextBox(ControlIdConstants.CorrespondenceRefNo, Model.CorrespondenceRefNo.HasValue ? Model.CorrespondenceRefNo.Value.ToString(FormatConstants.CorrespondenceNumberFormat): string.Empty, new { @class = "numeric", maxLength = 11 })%>
        <span class="hidden corrInvLoader">
          <img src='<%:Url.Content("~/Content/Images/busy.gif") %>' alt="Loading..." />
        </span>
      </div>
      <div>
        <label>
          <span>*</span> Rejected Invoice #:</label>
        <%: Html.TextBoxFor(invoice => invoice.RejectedInvoiceNumber, new { maxLength = 10, disabled = true, id = "CorrespondenceRejInvoiceNo" })%>
      </div>
      <div>
        <label>
          Authority to Bill received:</label>
        <%:Html.RadioButton(ControlIdConstants.IsExpired, "", new { id = "radCpdAbtr", disabled = true })%>
      </div>
      <div>
        <label>
          Expired:</label>
        <%:Html.RadioButton(ControlIdConstants.IsExpired, "", new { id = "radCpdExpired", disabled = true })%>
        <%:Html.HiddenFor(invoice => invoice.IsAuthorityToBill)%>
      </div>
    </div>
    <div class="bottomLine">
    </div>
    <%} %>
    <%else
       { %>
    <%:Html.HiddenFor(invoice => invoice.InvoiceType)%>
    <%} %>
    <!-- Hide above control in case of UATP invoice -->
    <h2>
      Notes</h2>
    <div id="MainNote">
      <div>
        <label>
          Note:</label>
        <%: Html.TextBox(ControlIdConstants.NoteDropdown, string.Empty, new { @class = "autocComplete" })%>
      </div>
      <div class="dynamicTextArea">
        <label>
          Note Description:</label>
        <%:Html.TextArea("txtNoteDesc", new { rows = 3, cols = 80, @class = "noteDescRequired" })%>
        <span>
          <img class="linkImage" id="AddNote" title="Add fields" alt="Add fields" src='<%:Url.Content("~/Content/Images/plus.png") %>' /></span>
      </div>
    </div>
    <div id="MainAddDetail" class="topLine">
      <div>
        <label>
          Additional Details:</label>
          <%--SCP140271 - incorrect data in eInvoice are validated by SI--%>
        <%: Html.TextBox(ControlIdConstants.AdditionalDetailDropdown, string.Empty, new { @class = "autocComplete additionalDetailRequired", maxLength = 30 })%>
      </div>
      <div class="dynamicTextArea">
        <label>
          Additional Details Description:</label>
        <%:Html.TextArea(ControlIdConstants.AdditionalDetailDescription, new { rows = 3, cols = 80, @class = "addDetailDescRequired" })%>
        <span>
          <img class="linkImage" id="AddDetail" title="Add fields" alt="Add fields" src='<%:Url.Content("~/Content/Images/plus.png") %>' /></span>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
      {%>
    <div class="topLine">
      <div>
        <label>
          Attachment Indicator - Original:</label>
        <%:Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal, new { @class = "populated", @readOnly = true })%>
      </div>
      <div>
        <label>
          Attachment Indicator - Validated:</label>
        <%: Html.TextBox(ControlIdConstants.AttachmentIndicatorValidated, Model.AttachmentIndicatorValidated == true? "Yes" : "No")%>
      </div>
      <div>
        <label>
          Number of Attachments:</label>
        <%: Html.TextBoxFor(invoice => invoice.AttachmentNumber)%>
      </div>
      <div>
        <label>
          IS Validation:</label>
        <%: Html.TextBoxFor(invoice => invoice.IsValidationFlag)%>
      </div>
      <div>
        <label>
          Suspended Flag:</label>
        <%: Html.TextBox(ControlIdConstants.SuspendedInvoiceFlag, Model.SuspendedInvoiceFlag == true? "Yes" : string.Empty)%>
      </div>
    </div>
    <%} %>
    <div class="clear">
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div id="AdditionalDetails">
</div>
<div id="AddDetailTemplate" class="hidden">
  <div id="AddTl">
    <div>
      <label>
        Additional Details:</label>
      <%: Html.TextBox(ControlIdConstants.AdditionalDetailDropdownTemplate, string.Empty, new { @class = "autocComplete additionalDetailRequired" })%>
    </div>
    <div class="dynamicTextArea">
      <label>
        Additional Details Description:</label>
      <%:Html.TextArea(ControlIdConstants.AdditionalDetailDescriptionTemplate, new { rows = 3, cols = 80, @class = "addDetailDescRequired" })%>
      <span>
        <img class="linkImage" title="Remove fields" alt="Remove fields" src='<%:Url.Content("~/Content/Images/minus.png") %>' /></span>
    </div>
  </div>
</div>
<div id="NoteTemplate" class="hidden">
  <div id="NoteTl">
    <div>
      <label>
        Note:</label>
      <%: Html.TextBox(ControlIdConstants.NoteDropdownTemplate, string.Empty, new { @class = "autocComplete" })%>
    </div>
    <div class="dynamicTextArea">
      <label>
        Note Description:</label>
      <%:Html.TextArea(ControlIdConstants.NoteDescriptionTemplate, new { rows = 3, cols = 80, @class = "noteDescRequired" })%>
      <span>
        <img class="linkImage" title="Remove fields" alt="Remove fields" src='<%:Url.Content("~/Content/Images/minus.png") %>' /></span>
    </div>
  </div>
</div>
<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAddChargeList" class="hidden">
</div>
<div id="childContactList" class="hidden">
</div>
<div id="billingMemberReferenceContainer" class="hidden">
</div>
<div id="billedMemberReferenceContainer" class="hidden">
</div>
<div>
  <%: Html.HiddenFor(invoice => invoice.InvoiceSummary.TotalLineItemAmount)%>
</div>
