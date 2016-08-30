<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RejectionMemo>" %>
<h2>
  Rejection Memo Data Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Batch No.:</label>
        <%:Html.TextBoxFor(m => m.BatchSequenceNumber, new { max = 99999, min = 0, @class = "digits integer", @maxLength = 5 })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Sequence Number:</label>
        <%:Html.TextBoxFor(m => m.RecordSequenceWithinBatch, new { max = 99999, min = 0, @class = "digits integer", @maxLength = 5 })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Source Code:</label>
        <%:Html.TextBoxFor(m => m.SourceCodeId, new { @class = "autocComplete" })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Rejection Memo Stage:</label>
        <%:Html.RejectionStageDropdownList(ControlIdConstants.RejectionStage, Model.RejectionStage)%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Rejection Memo Number:</label>
        <%:Html.TextBoxFor(m => m.RejectionMemoNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Reason Code:</label>
        <%:Html.TextBoxFor(m => m.ReasonCode, new { @class = "autocComplete upperCase" })%>
        <%:Html.HiddenFor(m => m.CouponAwbBreakdownMandatory)%>
      </div>
      <div>
        <label>
          Our Reference:</label>
        <%:Html.TextBoxFor(m => m.OurRef, new { @maxLength = 20 })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Your Invoice Number:</label>
        <%:Html.TextBoxFor(m => m.YourInvoiceNumber, new { @maxLength = 10 })%>
        <%:Html.HiddenFor(m => m.YourInvoiceNumber)%>
      </div>
      <div>
        <label id="LabelYourRejectionNumber">
          Your Rejection Memo Number:</label>
        <%:Html.TextBoxFor(m => m.YourRejectionNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label for="BillingYear">
          <span class="required">*</span> Your Billing Year:</label>        
        <%:Html.BillingYearDropdownList("YourInvoiceBillingYear", Model.YourInvoiceBillingYear)%>
      </div>
      <div>
        <label for="BillingMonth">
          <span class="required">*</span> Your Billing Month:</label>        
        <%:Html.BillingMonthDropdownList("YourInvoiceBillingMonth", Model.YourInvoiceBillingMonth)%>
      </div>
      <div>
        <label for="BillingMonthPeriod">
          <span class="required">*</span> Your Billing Period:</label>
        <%:Html.StaticBillingPeriodDropdownList("YourInvoiceBillingPeriod", Model.YourInvoiceBillingPeriod)%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          FIM/BM/CM Indicator:</label>
        <%:Html.FimBmCmIndicatorDropdownList("FIMBMCMIndicatorId", Model.FIMBMCMIndicatorId.ToString())%>
      </div>
      <div>
        <label>
          FIM/Billing Memo Number:</label>
        <%:Html.TextBoxFor(m => m.FimBMCMNumber, new { @maxLength = 11 })%>
      </div>
      <div>
        <label>
          FIM Coupon Number:</label>
        <%:Html.TextBoxFor(m => m.FimCouponNumber, new { @class = "integer", @maxLength = 2 })%>
      </div>
      <%
        if (!ViewData.ContainsKey(ViewDataConstants.FromBillingHistory))
        {
%>
      <div>
        <br />
        <input type="button" value="Validate Linking Details" onclick="GetLinkingDetails();" id="btnGetLinkingData" class="primaryButton" />
      </div>
      <%
        }
%>
    </div>
    <div class="bottomLine">
      <label>
        Reason Remarks:</label>
      <%:Html.TextAreaFor(m => m.ReasonRemarks, 10, 80, new { maxlength = "4000", @class = "notValidCharsTextarea" })%>
    </div>
    <div class="bottomLine">
      <table cellpadding="2" cellspacing="0" id="amountFieldsTable" class="amountFieldsTable">
        <thead align="center" valign="middle" style="width: 50%">
          <tr>
            <td>
            </td>
            <td style="width: 130px; font-weight: bold;">
              Billed
            </td>
            <td style="width: 130px; font-weight: bold;">
              Accepted
            </td>
            <td colspan="2" align="left" style="width: 200px; font-weight: bold;">
              Difference
            </td>
          </tr>
        </thead>
        <tbody align="center" valign="middle">
          <tr>
            <td style="font-weight: bold;" align="left">
              Gross
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.TotalGrossAmountBilled,
                                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.TotalGrossAcceptedAmount,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalGrossDifference,
                                                        new { @class = "smallTextField amount amt_12_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Tax
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.TotalTaxAmountBilled,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.TotalTaxAmountAccepted,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalTaxAmountDifference,
                                                        new { @class = "smallTextField amount amt_12_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
          <tr>
            <td>
            </td>
            <td style="font-weight: bold;">
              Allowed
            </td>
            <td style="font-weight: bold;">
              Accepted
            </td>
            <td colspan="2" style="font-weight: bold;" align="left">
              &nbsp;&nbsp;&nbsp;&nbsp; Difference
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              ISC
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AllowedIscAmount,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedIscAmount,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.IscDifference,
                                                        new { @class = "smallTextField amount amt_12_3", @readOnly = true, watermark = ControlIdConstants.NegativeAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              UATP
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AllowedUatpAmount,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedUatpAmount,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.UatpAmountDifference, new { @class = "smallTextField amount amt_12_3", @readOnly = true, watermark = ControlIdConstants.NegativeAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Handling Fee
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AllowedHandlingFee,
                                                        new { @class = "smallTextField breakDownDerived amount amt_10_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedHandlingFee,
                                                        new { @class = "smallTextField breakDownDerived amount amt_10_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.HandlingFeeAmountDifference,
                                                        new { @class = "smallTextField amount amt_10_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Other Commission
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AllowedOtherCommission,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedOtherCommission,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.OtherCommissionDifference, new { @class = "smallTextField amount amt_12_3", @readOnly = true, watermark = ControlIdConstants.NegativeAmount })%>
            </td>
          </tr>
          <tr>
            <td colspan="5">
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              VAT
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.TotalVatAmountBilled,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.TotalVatAmountAccepted,
                                                        new { @class = "smallTextField breakDownDerived amount amt_12_3", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalVatAmountDifference,
                                                        new { @class = "smallTextField amount amt_12_3", @readOnly = true, watermark = ControlIdConstants.NegativeAmount })%>
              <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Rejection Memo VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
            </td>
          </tr>
          <tr>
            <td align="right" colspan="3">
              <b>Net Reject Amount.</b>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.TotalNetRejectAmount,
                                        new { @class = "smallTextField amount", @readOnly = true, min = 0, max = 9999999999999.9, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div>
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
        <label>
          Airline Own Use:</label>
        <%:Html.TextBoxFor(m => m.AirlineOwnUse, new { @maxLength = 11 })%>
      </div>
      <div>
        <%:Html.HiddenFor(m => m.IsLinkingSuccessful)%>
        <%:Html.HiddenFor(m => m.IsBreakdownAllowed)%>
        <%:Html.HiddenFor(m => m.CurrencyConversionFactor)%>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
    {%>
  <div class="fieldSeparator" id="payablesSeparatorDiv">
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
      <%:Html.TextBoxFor(m => m.NumberOfAttachments)%>
    </div>
    <div>
      <label for="isValidationFlag">
        IS Validation Flag:</label>
      <%:Html.TextBoxFor(m => m.ISValidationFlag)%>
    </div>
        <div>
      <label for="isRejectionFlag">
        IS Rejection Flag:</label>
      <%:Html.TextBoxFor(m => m.IsRejectionFlag)%>
    </div>
  </div>
  <%
    }%>
  </div>
  <div class="clear">
  </div>
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div>
