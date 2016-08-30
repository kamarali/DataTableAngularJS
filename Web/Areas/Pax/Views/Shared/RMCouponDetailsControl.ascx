<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RMCoupon>" %>
<h2>
  Rejection Memo Coupon</h2>
  <input type="hidden" value="<%:Model.RejectionMemoRecord.SourceCodeId%>" id="SourceCodeId" />
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div id="serialNoDiv">
        <label for="SerialNo">
          Serial Number:
        </label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.SerialNo, new { @readOnly = true })%>
      </div>
      <div>
        <label for="TicketOrFimIssuingAirline">
          <span>*</span> Ticket Issuing Airline:
        </label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.TicketOrFimIssuingAirline, new { @class = "autocComplete populated", maxlength = 4 })%>
      </div>
      <div>
        <label for="TicketOrFimCouponNumber">
          <span>*</span> Coupon Number:
        </label>
        <%:Html.NonFimCouponNumberDropdownList("TicketOrFimCouponNumber", Model.TicketOrFimCouponNumber)%>
      </div>
      <div>
        <label for="TicketDocOrFimNumber">
          <span>*</span> Ticket/Document Number:
        </label>
        <%--CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured--%>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.TicketDocOrFimNumber, new { max = 9999999999, min = 0, @class = "digits integer", maxLength = 10 })%>
      </div>
      <div>
        <label for="checkDigit">
          <span>*</span> Check Digit:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.CheckDigit, new { @class = "checkDigit linkingPopulated", maxLength = 1 })%>
      </div>
      <div>
        <%
          //SCP131535: SAMPLE FORM F - COUPON ENTRY FAILS 
          // Added condtion to check billing code.
          if (Model.RejectionMemoRecord.IsLinkingSuccessful == true && Model.RejectionMemoRecord.SourceCodeId != 44 && (Convert.ToBoolean(ViewData["IsAwbLinkingRequired"]) || Model.RejectionMemoRecord.Invoice.BillingCode != 0))
          {%>
        <input class="primaryButton" type="button" value="View" id="FetchButton" />
        <%
          }%>
      </div>
    </div>
    <div>
      <div>
        <label for="fromAirportOfCoupon">
          From Airport of Coupon:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.FromAirportOfCoupon, new { maxLength = 4, @class = "upperCase linkingPopulated" })%>
      </div>
      <div>
        <label for="toAirportOfCoupon">
          To Airport of Coupon:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ToAirportOfCoupon, new { maxLength = 4, @class = "upperCase linkingPopulated" })%>
      </div>
      <div>
        <label for="settlementAuthorizationCode">
          Settlement Authorization Code:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.SettlementAuthorizationCode, new { maxLength = 14, @class = "linkingPopulated" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="AgreementIndicatorSupplied">
          Agreement Indicator - Supplied:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.AgreementIndicatorSupplied, new { maxLength = 2, @class = "linkingPopulated" })%>
      </div>
      <div>
        <label for="AgreementIndicatorValidated">
          Agreement Indicator - Validated:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.AgreementIndicatorValidated, new { maxLength = 2, @class = "linkingPopulated" })%>
      </div>
      <div>
        <label for="OriginalPmi">
          Original PMI:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.OriginalPmi, new { @class = "alphabet upperCase linkingPopulated", maxLength = 1 })%>
      </div>
      <div>
        <label for="ValidatedPmi">
          Validated PMI:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ValidatedPmi, new { maxLength = 1, @class = "linkingPopulated" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div class="bottomLine">
      <table border="0" cellpadding="2" cellspacing="0" class="amountFieldsTable">
        <thead align="center" valign="middle" style="width: 50%">
          <tr>
            <td>
              &nbsp;
            </td>
            <td>
              &nbsp;
            </td>
            <td style="width: 90px; font-weight: bold;">
              Billed
            </td>
            <td>
              &nbsp;
            </td>
            <td style="width: 90px; font-weight: bold;">
              Accepted
            </td>
            <td style="font-weight: bold;">
              Difference
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
        </thead>
        <tbody align="center" valign="middle">
          <tr>
            <td style="font-weight: bold;" align="left">
              Gross
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.GrossAmountBilled,
                                        new { @class = "smallTextField amountTextfield amt_12_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.GrossAmountAccepted, new { @class = "amountTextfield smallTextField amt_12_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.GrossAmountDifference, new { @class = "smallTextField amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Tax
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.TaxAmountBilled, new { @class = "smallTextField amountTextfield amt_12_3 amount linkingPopulated", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.TaxAmountAccepted, new { @class = "smallTextField amountTextfield amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.TaxAmountDifference, new { @class = "smallTextField amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              <%:ScriptHelper.GenerateDialogueHtml("Tax Breakdown", "Rejection Memo Coupon Tax Capture", "divTaxBreakdown", 500, 700)%>
            </td>
          </tr>
          <tr>
            <td>
              &nbsp;
            </td>
            <td colspan="2" style="font-weight: bold;">
              Allowed
            </td>
            <td colspan="2" style="font-weight: bold;">
              Accepted
            </td>
            <td style="font-weight: bold;">
              Difference
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td>
              &nbsp;
            </td>
            <td>
              %
            </td>
            <td>
              Amount
            </td>
            <td>
              %
            </td>
            <td>
              Amount
            </td>
            <td>
              Amount
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              ISC
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedIscPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.NegativePercentage })%>
            </td>
            <td>
              <%--<%if (Model.RejectionMemoRecord.SourceCodeId != 91 && Model.RejectionMemoRecord.SourceCodeId != 92 && Model.RejectionMemoRecord.SourceCodeId != 93)
                {%>
              <%:Html.TextBoxFor(coupon => coupon.AllowedIscAmount, new{ @class = "smallTextField amountTextfield amt_12_3 amount linkingPopulated", @readOnly = true})%>
              <%
                } else {%>

                <%:Html.TextBoxFor(coupon => coupon.AllowedIscAmount, new{@class ="smallTextField amountTextfield amt_12_3 amount linkingPopulated"})%>

                <%} %>--%>
                 <%:Html.TextBoxFor(coupon => coupon.AllowedIscAmount, new{@class ="smallTextField amountTextfield amt_12_3 amount linkingPopulated"})%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedIscPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.NegativePercentage })%>
            </td>
            <td>
            <%--<%if (Model.RejectionMemoRecord.SourceCodeId != 91 && Model.RejectionMemoRecord.SourceCodeId != 92 && Model.RejectionMemoRecord.SourceCodeId != 93)
                {%>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedIscAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount", @readOnly = true})%>
              <%
                } else {%>

                <%:Html.TextBoxFor(coupon => coupon.AcceptedIscAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount"})%>

                <%} %>--%>
                <%:Html.TextBoxFor(coupon => coupon.AcceptedIscAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount"})%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.IscDifference, new { @class = "smallTextField amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              UATP
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedUatpPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.NegativePercentage })%>
            </td>
            <td>
            <%--<%if (Model.RejectionMemoRecord.SourceCodeId != 91 && Model.RejectionMemoRecord.SourceCodeId != 92 && Model.RejectionMemoRecord.SourceCodeId != 93)
                {%>
              <%:Html.TextBoxFor(coupon => coupon.AllowedUatpAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount linkingPopulated", @readOnly = true })%>
              <%
                } else {%>
              <%:Html.TextBoxFor(coupon => coupon.AllowedUatpAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount linkingPopulated"})%>
                <%} %>--%>
                <%:Html.TextBoxFor(coupon => coupon.AllowedUatpAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount linkingPopulated"})%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedUatpPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.NegativePercentage })%>
            </td>
            <td>
           <%-- <%if (Model.RejectionMemoRecord.SourceCodeId != 91 && Model.RejectionMemoRecord.SourceCodeId != 92 && Model.RejectionMemoRecord.SourceCodeId != 93)
                {%>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedUatpAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount", @readOnly = true })%>
              <%
                } else {%>
                <%:Html.TextBoxFor(coupon => coupon.AcceptedUatpAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount"})%>
                <%} %>--%>
                <%:Html.TextBoxFor(coupon => coupon.AcceptedUatpAmount, new { @class = "smallTextField amountTextfield amt_12_3 amount"})%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.UatpDifference, new { @class = "smallTextField amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Handling Fee
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedHandlingFee,
                                        new { @class = "smallTextField amountTextfield amt_10_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedHandlingFee, new { @class = "smallTextField amountTextfield amt_10_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.HandlingDifference, new { @class = "smallTextField amt_10_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Other Comm.
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedOtherCommissionPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.NegativePercentage })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedOtherCommission,
                                        new { @class = "smallTextField amountTextfield amt_12_3 amount", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedOtherCommissionPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.NegativePercentage })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedOtherCommission, new { @class = "smallTextField amountTextfield amt_12_3 amount", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.OtherCommissionDifference, new { @class = "smallTextField amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td colspan="7">
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              VAT
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.VatAmountBilled,
                                        new { @class = "smallTextField amountTextfield amt_12_3 amount", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.VatAmountAccepted, new { @class = "smallTextField amountTextfield amt_12_3 amount", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.VatAmountDifference, new { @class = "smallTextField amt_12_3 amount", @readOnly = true, id = "VatAmountDifference" })%>
            </td>
            <td>
              <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Rejection Memo Coupon VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Net Reject Amt.
            </td>
            <td colspan="4">
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.NetRejectAmount, new { @class = "smallTextField amt_12_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div class="clear">
  </div>
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <div>
        <label for="reasonCode">
          Reason Code:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ReasonCode, new { maxLength = 2 })%>
      </div>
      <div>
        <label for="AirlineOwnUse">
          Airline Own Use:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.AirlineOwnUse, new { maxLength = 20 })%>
      </div>
      <div>
        <label for="ReferenceField1">
          Reference Field 1:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ReferenceField1, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField2">
          Reference Field 2:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ReferenceField2, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField3">
          Reference Field 3:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ReferenceField3, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField4">
          Reference Field 4:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ReferenceField4, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField5">
          Reference Field 5:
        </label>
        <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ReferenceField5, new { maxLength = 20 })%>
      </div>
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
        <%:Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal, new { @class = "populated", @readOnly = true })%>
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
      <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.NumberOfAttachments)%>
    </div>
    <div>
      <label for="isValidationFlag">
        IS Validation Flag:</label>
      <%:Html.TextBoxFor(couponRejectionBreakdown => couponRejectionBreakdown.ISValidationFlag)%>
    </div>
  </div>
  <%
    }%>
  </div>
  <div class="clear">
  </div>
  <%:Html.TextAreaFor(couponRejectionBreakdown => couponRejectionBreakdown.ProrateSlipDetails, new { @class = "hidden", @id = "hiddenprorateSlip" })%>
</div>
<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div>
