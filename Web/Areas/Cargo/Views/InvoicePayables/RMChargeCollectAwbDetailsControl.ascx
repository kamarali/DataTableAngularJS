<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.RMAwb>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Charge Collect AWB</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div id="serialNoDiv">
        <label for="BdSerialNumber">
          Serial Number:
        </label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.BdSerialNumber, new { @readOnly = true })%>
      </div>
      <div>
        <label for="AwbIssueingAirline">
          <span>*</span> AWB Issuing Airline:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.AwbIssueingAirline,new { @class = "autocComplete populated", maxlength = 4 })%>
      </div>
      <div>
        <label for="AwbDate">
          <span>*</span> AWB Serial Number & Check Digit:</label>
        <%:Html.TextBox(ControlIdConstants.AwbSerialNumber, string.Format("{0}{1}", Model.AwbSerialNumber.ToString().PadLeft(7, '0'), Model.AwbCheckDigit), new { @class = "", @maxLength = 8, @minLength = 8 })%>
      </div>
        <%
          if (Model.RejectionMemoRecord.IsLinkingSuccessful == true)
          {%>
      <div id = "FetchButtonDiv">
        <input class="primaryButton" type="button" value="View" id="FetchButton" />
      </div>
      <%
          }%>
      <div>
        <label for="flightDate">
          <span>*</span> AWB Issuing Date:</label>
        <%: Html.TextBox(ControlIdConstants.AwbDate, (Model.AwbDate.HasValue ? Model.AwbDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker linkingPopulated" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ConsignmentOriginId">
          Origin:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ConsignmentOriginId, new { @class = "upperCase alphabetsOnly linkingPopulated", maxLength = 4 })%>
      </div>
      <div>
        <label for="ConsignmentDestinationId">
          Destination:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ConsignmentDestinationId, new { @class = "upperCase alphabetsOnly linkingPopulated", maxLength = 4 })%>
      </div>
      <div>
        <label for="CarriageFromId">
          <span>*</span> From:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.CarriageFromId, new { @class = "upperCase alphabetsOnly linkingPopulated", maxlength = 4 })%>
      </div>
      <div>
        <label for="CarriageToId">
          <span>*</span> To (or point of Transfer):</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.CarriageToId, new { @class = "upperCase alphabetsOnly linkingPopulated", maxlength = 4 })%>
      </div>
      <div>
        <label for="TransferDate">
          <span>*</span> Transfer Date:</label>
        <%: Html.TextBox(ControlIdConstants.TransferDate, (Model.TransferDate.HasValue ? Model.TransferDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker linkingPopulated" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="CurrencyAdjustmentIndicator">
          Currency Adjustment Indicator:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.CurrencyAdjustmentIndicator, new { @class = "alphabetsOnly upperCase", maxLength = 3 })%>
      </div>
      <div>
        <label for="billedWeight">
          Billed Weight:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.BilledWeight, new { @class = "integer", maxlength = 6 })%>
      </div>
      <div>
        <label for="KGLBIndicator">
          KG/LB Indicator:</label>
        <%:Html.KgLbIndDropdownList(awbRecord => awbRecord.KgLbIndicator)%>
      </div>
      <div>
        <label for="provisoreqSpa">
          Proviso/Req/SPA:</label>
        <%:Html.ProvisoreqspaDropdownList(awbRecord => awbRecord.ProvisionalReqSpa)%>
      </div>
      <div>
        <label for="proratePercentage">
          Prorate %:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ProratePercentage, new { @class = "integer", maxlength = 2, min = 1, max = 99.99 })%>
      </div>      
    </div>
    <div>
      <div>
        <label for="PartShipmentIndicator">
          Part Shipment:</label>        
        <%: Html.CheckBox("chkPartShipment", Model.PartShipmentIndicator == "P")%>
        <%: Html.HiddenFor(awbRecord => awbRecord.PartShipmentIndicator)%>
      </div>
      <div>
        <label for="CcaIndicator">
          CCA Indicator:</label>
        <%:Html.CheckBoxFor(awbRecord => awbRecord.CcaIndicator)%>
      </div>
      <div>
        <label for="OurReference">
          Our Reference:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.OurReference, new { maxLength = 20 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div id="divReasonCode">
      <div>
        <label for="reasonCode">
          Reason Code:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ReasonCode, new { maxLength = 2, @class = "upperCase" })%>
      </div>
      <div>
        <label for="ReferenceField1">
          Reference Field 1:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField1, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField2">
          Reference Field 2:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField2, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField3">
          Reference Field 3:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField3, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField4">
          Reference Field 4:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField4, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField5">
          <%--CMP#658--%>
        Weight Indicator (Reference Field 5):</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField5, new { maxLength = 20 })%>
      </div>
      <div>
        <label for="AirlineOwnUse">
          Airline Own Use:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.AirlineOwnUse, new { maxLength = 20 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
  
  
  
    <div>
      <table cellpadding="2" cellspacing="0" id="amountFieldsTable" class="amountFieldsTable">
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
            <td style="font-weight: bold;width: 90px">
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
              Weight Charges
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledWeightCharge, new { @class = "smallTextField amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
             <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedWeightCharge, new { @class = "amountTextfield amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.WeightChargeDiff, new { @class = "smallTextField amount", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
             <td>
              &nbsp;
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Valuation Charges
            </td>
             <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledValuationCharge, new { @class = "amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
             <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedValuationCharge, new { @class = "amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.ValuationChargeDiff, new { @class = "smallTextField amount", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
            </td>
          </tr>
          <tr>
            <td style="font-weight: bold;" align="left">
              Other Charges
            </td>
             <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledOtherCharge, new { @class = "amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
             <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedOtherCharge, new { @class = "amountTextfield amount  amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.OtherChargeDiff, new { @class = "smallTextField amount", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
              <%:ScriptHelper.GenerateDialogueHtml("Other Charges", "RM AWB Charge Collect Other Charge Capture", "divOtherCharge", 500, 900)%>
            </td>
          </tr>
          <tr>            
            <td style="font-weight: bold;" align="left">
              VAT Amount
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.BilledVatAmount, new { @class = "amountTextfield amt_11_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
              &nbsp;
            </td>
            <td>
              <%:Html.TextBoxFor(m => m.AcceptedVatAmount, new { @class = "amountTextfield amt_11_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>           
            <td colspan="2" align="left">
              <%:Html.TextBoxFor(m => m.VatAmountDifference, new { @class = "smallTextField amount", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>
              <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "RM AWB Charge Collect VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
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
              Amt. Subjected to ISC
            </td>
            <td>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedAmtSubToIsc, new { @class = "smallTextField amountTextfield amt_11_3 amount linkingPopulated", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedAmtSubToIsc, new { @class = "smallTextField amountTextfield amt_11_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
            </td>
            <td>
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
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.Percentage })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AllowedIscAmount, new { @class = "smallTextField amountTextfield amt_11_3 amount linkingPopulated", @readOnly = true })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedIscPercentage,
                                        new { @class = "vSmallTextField percentageTextfield amt_percent percent", watermark = ControlIdConstants.Percentage })%>
            </td>
            <td>
              <%:Html.TextBoxFor(coupon => coupon.AcceptedIscAmount, new { @class = "smallTextField amountTextfield amt_11_3 amount", @readOnly = true })%>
            </td>
            <td align="left">
              <%:Html.TextBoxFor(coupon => coupon.IscAmountDifference, new { @class = "smallTextField amt_11_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>
         
           <tr>
            <td style="font-weight: bold;" align="left">
              Net Reject Amt.
            </td>
            <td colspan="4">
            </td>
            <td align="left">
              <%:Html.TextBoxFor(coupon => coupon.NetRejectAmount, new { @class = "smallTextField amt_11_3 amount", @readOnly = true })%>
            </td>
            <td>
              &nbsp;
            </td>
          </tr>        
        </tbody>
      </table>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="attachmentIndicatorOriginal">
          Attachment Indicator-Original:
        </label>
        <%: Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal)%>
        <a class="ignoredirty" id="attachmentBreakdown" href="#" onclick="return openAttachment();">Attachment</a>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Prorate Ladder", "Add/Edit Prorate Ladder Details", "divProrateLadder", 510, 970, "prorateLadder")%>
        <%: Html.HiddenFor(awb => awb.TotalProrateAmount) %>
        <%: Html.HiddenFor(awb => awb.ProrateCalCurrencyId)%>
        <%:Html.HiddenFor(awb => awb.OtherChargeVatSumAmount)%>
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
      <%:Html.TextBoxFor(awbRecord => awbRecord.NumberOfAttachments)%>
    </div>
    <div>
      <label for="isValidationFlag">
        IS Validation Flag:</label>
      <%:Html.TextBoxFor(awbRecord => awbRecord.ISValidationFlag)%>
    </div>
  </div>
  <%
    }%>
  </div>
  <div class="clear">
  </div>
</div>
<div id="childOtherChargeList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="hidden">
</div>
<div id="childProrateLadderList" class="hidden">
</div>
