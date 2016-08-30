<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormCRecord>" %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          <span>*</span> Source Code:</label>
        <%:Html.TextBoxFor(formCCoupon => formCCoupon.SourceCodeId, new { @readOnly = true, @class = "autocComplete populated" })%>
      </div>
      <div>
        <label>
          <span>*</span> Tkt. Iss. Arln:</label>
        <%:Html.TextBox("TicketIssuingAirline", Model.TicketIssuingAirline, new { @class = "autocComplete", maxlength = 4 })%>
      </div>
      <div>
        <label>
          <span>*</span> Cpn.No.:</label>
        <%:Html.NonFimCouponNumberDropdownList("CouponNumber", Model.CouponNumber)%>
      </div>
      <div>
        <label>
          <span>*</span> Tkt./Doc. No:</label>
          <%--CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured--%>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.DocumentNumber, new { maxLength = 10, @class = "requiredDigits" })%>
      </div>
      <div>
        <input class="primaryButton" type="button" value="Retrieve" id="FetchButton" />
      </div>
    </div>
    <div>
      <div>
        <label>
          <span>*</span> Prov. Inv. No.:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.ProvisionalInvoiceNumber, new { maxLength = 10 })%>
      </div>
      <div>
        <label>
          <span>*</span> Batch No.:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.BatchNumberOfProvisionalInvoice, new { maxLength = 5, @class = "requiredDigits" })%>
      </div>
      <div>
        <label>
          <span>*</span> Sequence No.:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.RecordSeqNumberOfProvisionalInvoice, new { maxLength = 5, @class = "requiredDigits" })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          Currency of Listing/Evaluation:</label>
        <%: Html.TextBox(ControlIdConstants.ListingCurrency, Model.SamplingFormC.ListingCurrency.Code, new { @readOnly = true, @class = "populated" })%>
      </div>
      <div>
        <label>
          Gross Amount/ALF:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.GrossAmountAlf, new { @class = "amt_12_3 amount" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label>
          Electronic Ticket Indicator:</label>
        <%:Html.CheckBoxFor(formCCoupon => formCCoupon.ElectronicTicketIndicator)%>
      </div>
      <div>
        <label>
          Agreement Indicator - Supplied:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.AgreementIndicatorSupplied, new { maxLength = 2, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          Agreement Indicator - Validated:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.AgreementIndicatorValidated, new { maxLength = 2, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          Original PMI:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.OriginalPmi, new { @class = "alphabet upperCase", maxLength = 1 })%>
      </div>
      <div>
        <label>
          Validated PMI:</label>
        <%: Html.TextBoxFor(formCCoupon => formCCoupon.ValidatedPmi, new { @class = "alphabet upperCase", maxLength = 1 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label>
          <span>*</span> Rejection Reason Code:</label>
        <%:Html.TextBoxFor(formCCoupon => formCCoupon.ReasonCode, new { @class = "autocComplete upperCase" })%>
      </div>
      <div id="divAttachments">
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
          {%>
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator - Original:
          </label>
        <%
          }
          else
          {%>
          <label for="attachmentIndicatorOriginal">
            <span>*</span> Attachment Indicator:
          </label>
          <%} %>
        <%:Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal)%>
        <a class="ignoredirty" href="#" onclick="return openAttachment();">Attachment</a>
      </div>
      <div>
        <div>
          <label>
            Remarks:</label>
          <%: Html.TextAreaFor(formCCoupon => formCCoupon.Remarks, 10, 80, null)%>
        </div>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
    {%>
      <div class="fieldSeparator">
      </div>
      <div id="divPayables">
        <div>
          <label for="attachmentIndicatorValidated">
            Attachment Indicator - Validated:</label>
          <%:Html.TextBox(ControlIdConstants.AttachmentIndicatorValidated, Model.AttachmentIndicatorValidated == true ? "Yes" : "No")%>
        </div>
        <div>
          <label for="numberOfAttachments">
            Number Of Attachments:</label>
          <%:Html.TextBoxFor(formCCoupon => formCCoupon.NumberOfAttachments)%>
        </div>
      </div>
    <% }%>
    <div class="clear">
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div id="childAttachmentList" class="">
</div>
