<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoRejectionMemo>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Rejection Memo Data Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Batch No.:</label>
        <%:Html.TextBoxFor(m => m.BatchSequenceNumber, new { @class = "integer", @maxLength = 5 })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Sequence Number:</label>
        <%:Html.TextBoxFor(m => m.RecordSequenceWithinBatch, new { @class = "integer", @maxLength = 5 })%>
      </div>
    </div>
    <div class="bottomLine">     
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
        <%:Html.TextBoxFor(m => m.ReasonCode, new { @class = "autocComplete upperCase", @maxLength = 2 })%>
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
        <%:Html.TextBoxFor(m => m.YourInvoiceNumber, new { @maxLength = 10, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label id="LabelYourRejectionNumber">
          Your Rejection Memo Number:</label>
        <%:Html.TextBoxFor(m => m.YourRejectionNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>      
      <div>
        <label for="YourInvoiceBillingYear">
          <span class="required">*</span> Your Billing Year:</label>
        <%:Html.BillingYearDropdownList("YourInvoiceBillingYear", Model.YourInvoiceBillingYear)%>
      </div>
      <div>
        <label for="YourInvoiceBillingMonth">
          <span class="required">*</span> Your Billing Month:</label>
        <%:Html.BillingMonthDropdownList("YourInvoiceBillingMonth", Model.YourInvoiceBillingMonth)%>
      </div>
      <div>
        <label for="YourInvoiceBillingPeriod">
          <span class="required">*</span> Your Billing Period:</label>
        <%:Html.StaticBillingPeriodDropdownList("YourInvoiceBillingPeriod", Model.YourInvoiceBillingPeriod)%>
      </div>
     </div>     
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> BM/CM Indicator:</label>
        <%:Html.BmCmIndicatorDropdownList("BMCMIndicatorId", Model.BMCMIndicatorId.ToString())%>
      </div>
      <div>
        <label for="YourBillingMemoNumber">
          Your BM/CM Number:</label>
        <%:Html.TextBoxFor(m => m.YourBillingMemoNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <% if (!ViewData.ContainsKey(ViewDataConstants.FromBillingHistory)) { %>
      <div>
        <br />
        <input type="button" value="Validate Linking Details" onclick="GetLinkingDetails();"
          id="btnGetLinkingData" class="primaryButton" />
      </div>
      <% } %>
    </div>
    <div class="bottomLine">
      <label>
        Reason Remarks:</label>
      <%:Html.TextAreaFor(m => m.ReasonRemarks, 10, 138, new { maxlength = "4000", @class = "notValidCharsTextarea" })%>
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
              Weight Charges
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledTotalWeightCharge, new { @class = "smallTextField amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedTotalWeightCharge, new { @class = "smallTextField amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalWeightChargeDifference, new { @class = "smallTextField amount amt_15_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Valuation Charges
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledTotalValuationCharge, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedTotalValuationCharge, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalValuationChargeDifference, new { @class = "smallTextField amount amt_15_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Other Charges
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledTotalOtherChargeAmount, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedTotalOtherChargeAmount, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalOtherChargeDifference, new { @class = "smallTextField amount amt_15_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
              <%--  <%:ScriptHelper.GenerateDialogueHtml("Other Charges", "Rejection Memo OC Capture", "divOtherChargeBreakdown", 500, 900)%>--%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              VAT Amount
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledTotalVatAmount, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedTotalVatAmount, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.TotalVatAmountDifference, new { @class = "smallTextField amount amt_15_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
              <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Rejection Memo VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
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
              <%:Html.TextBoxFor(m => m.AllowedTotalIscAmount, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedTotalIscAmount, new { @class = "smallTextField amt_15_3 amount", watermark = ControlIdConstants.NegativeAmount })%>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.TotalIscAmountDifference, new { @class = "smallTextField amt_15_3 amount", @readOnly = true, watermark = ControlIdConstants.NegativeAmount })%>
            </td>
          </tr>
          <tr>
            <td colspan="5">
              &nbsp;
            </td>
          </tr>
          <tr>
            <td align="right" colspan="3">
              <b>Net Reject Amount.</b>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(m => m.TotalNetRejectAmount, new { @class = "smallTextField pos_amt_15_3 amount", @readOnly = true })%>
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
        <a class="ignoredirty" id="attachmentBreakdown" href="#" onclick="return openAttachment();">Attachment</a>
      </div>
      <div>
        <label>
          Airline Own Use:</label>
        <%:Html.TextBoxFor(m => m.AirlineOwnUse, new { @maxLength = 20 })%>
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
<div>
  <%:Html.HiddenFor(m => m.IsLinkingSuccessful)%>
  <%:Html.HiddenFor(m => m.IsBreakdownAllowed)%>
  <%:Html.HiddenFor(m => m.CurrencyConversionFactor)%>
</div>

 