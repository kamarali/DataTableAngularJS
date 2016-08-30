<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>" %>
<h2>
  Form D Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          <span>*</span> Source Code:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.SourceCodeId, new { @readOnly = true, @class = "autocComplete populated" })%>
      </div>
      <div>
        <label>
          <span>*</span> Ticket Issuing Airline:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.TicketIssuingAirline, new { @class = "autocComplete populated" })%>
      </div>
      <div>
        <label>
          <span>*</span> Coupon Number:</label>
        <%:Html.NonFimCouponNumberDropdownList("CouponNumber", Model.CouponNumber)%>
      </div>
      <div>
        <label>
          <span>*</span> Ticket/Document Number:</label>
          <%--CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured--%>
        <%: Html.TextBoxFor(formDDetails => formDDetails.TicketDocNumber, new { maxLength = 10, @class = "numeric" })%>
      </div>
      <div>
        <input class="primaryButton" type="button" value="Retrieve" id="FetchButton" />
      </div>
    </div>
    <div>
      <div>
        <label>
          <span>*</span> Provisional Invoice Number:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ProvisionalInvoiceNumber, new { maxLength = 10 })%>
      </div>
      <div>
        <label>
          <span>*</span> Batch Number of Provisional Invoice:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.BatchNumberOfProvisionalInvoice, new { maxLength = 5, @class = "numeric" })%>
      </div>
      <div>
        <label>
          <span>*</span> Sequence Number of Prov. Invoice:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.RecordSeqNumberOfProvisionalInvoice, new { maxLength = 5, @class = "numeric" })%>
      </div>
      <div>
        <label>
          <span>*</span> Provisional Gross/ALF Amount:</label>
        <%: Html.TextBoxFor(invoice => invoice.ProvisionalGrossAlfAmount, new { min = -999999999.99, max = 999999999.99, @class = "amount" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label>
          Evaluated Gross Amount:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.EvaluatedGrossAmount, new { min = -999999999.99, max = 999999999.99, @class = "amount" })%>
      </div>
      <div>
        <label>
          Evaluated ISC:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.IscPercent, new { @class = "percentageTextfield amt_percent percent" })%>&nbsp;%&nbsp;<%: Html.TextBoxFor(formDDetails => formDDetails.IscAmount, new { @class = "amountTextfield amount", @readOnly = true })%>&nbsp;Amount
      </div>
      <div>
        <label>
          Evaluated Tax Amount:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.TaxAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -9999999999.99, max = 9999999999.99 })%>
        <%:ScriptHelper.GenerateDialogueHtml("Tax Breakdown", "Sampling Form D Tax Capture", "divTaxBreakdown", 500, 500)%>
      </div>
      <div>
        <label>
          Evaluated Handling Fee Amount:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.HandlingFeeAmount, new { min = -999999999.9, max = 999999999.9, @class = "amount" })%>
      </div>
      <div>
        <label>
          Evaluated Other Commission:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.OtherCommissionPercent, new { @class = "percentageTextfield amt_percent percent" })%>&nbsp;%&nbsp;<%: Html.TextBoxFor(formDDetails => formDDetails.OtherCommissionAmount, new { @class = "amountTextfield amount", min = -999999999.99, max = 999999999.99 })%>&nbsp;Amount
      </div>
      </div>
      <div>
      <div>
        <label>
          Evaluated UATP:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.UatpPercent, new { @class = "percentageTextfield amt_percent percent" })%>&nbsp;%&nbsp;<%: Html.TextBoxFor(formDDetails => formDDetails.UatpAmount, new { @class = "amountTextfield amount", @readOnly = true })%>&nbsp;Amount
      </div>
      <div>
        <label>
          Evaluated VAT Amount:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.VatAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -999999999.99, max = 999999999.99 })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Sampling Form D VAT Capture", "divVatBreakdown", 500, 900)%>
      </div>
      <div>
        <label>
          <span>*</span> Evaluated Net Amount:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.EvaluatedNetAmount, new { @readOnly = true, min = 0, max = 999999999.99, @class = "amount" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label>
          Agreement Indicator - Supplied:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.AgreementIndicatorSupplied, new { maxLength = 2 })%>
      </div> 
      <div>
        <label>
          Agreement Indicator - Validated:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.AgreementIndicatorValidated, new { maxLength = 2 })%>
      </div>     
      <div>
        <label>
          Original PMI:</label>
        <%: Html.StaticOriPMIDropdownListFor(formDDetails => formDDetails.OriginalPmi)%>
      </div>   
      <div>
        <label>
          Validated PMI:</label>
        <%: Html.StaticPMIDropdownListFor(formDDetails => formDDetails.ValidatedPmi)%>
      </div>    
      <div>
        <label>
          Prorate Methodology:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ProrateMethodology, new { maxLength = 3 })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          Reason Code:</label>
          <%:Html.TextBoxFor(formDDetails => formDDetails.ReasonCode, new { maxLength = 2, @class = "upperCase" })%>
      </div>
      <div>
        <label>
          Airline Own Use:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.AirlineOwnUse, new { maxLength = 20 })%>
      </div>
      <div>
        <label for="airlineownuse1">
          Reference Field 1:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ReferenceField1, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="airlineownuse2">
          Reference Field 2:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ReferenceField2, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="airlineownuse3">
          Reference Field 3:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ReferenceField3, new { maxLength = 10 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="airlineownuse4">
          Reference Field 4:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ReferenceField4, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="airlineownuse5">
          Reference Field 5:</label>
        <%: Html.TextBoxFor(formDDetails => formDDetails.ReferenceField5, new { maxLength = 20 })%>
      </div>
    </div>
    <div class="topLine">
      <div>
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
        <%:ScriptHelper.GenerateDialogueHtml("Prorate Slip", "Add/Edit Prorate Slip Details", "divProrateSlip", 400, 970)%>
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
          <%:Html.TextBoxFor(formDDetails => formDDetails.NumberOfAttachments)%>
        </div>
      </div>
    <% }%>
    <div class="clear">
    </div>
  </div>
  <div class="clear">
  </div>
</div>

<%:Html.TextAreaFor(formDDetails => formDDetails.ProrateSlipDetails, new { @class = "hidden", @id = "hiddenprorateSlip" })%>

<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="hidden">
</div>
