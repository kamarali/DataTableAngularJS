<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ProcessingDashboardInvoiceDetail>" %>
<style type="text/css">
  .boldText {
    font-weight: bold;
  }
</style>
<fieldset class="solidBox horizontalFlowForDetails">
  <div>
    <div>
      <label class="boldText">
        Billing Period:</label>
      <span class="readonlyValue">
        <%: Model.BillingPeriod ?? String.Empty %>
      </span>
    </div>
    <div>
      <label class="boldText">
        Clearance Type:</label>
      <span class="readonlyValue">
        <%: Model.SettleMethodIndicator ?? String.Empty %>
      </span>
    </div>
    <div>
      <label class="boldText">
        Billed Member:</label>
        <!-- CMP#596 Increasing length of billed member code on Additional invoice details popup -->
      <span class="readonlyValue" style="width: 160px;">
        <%: Model.BilledMemberCode %>
      </span>
    </div>
    <div>
    <!-- CMP#596 Increasing length of billed member code on Additional invoice details popup -->
      <label class="boldText" style="padding-left: 40px;">
        Billed Member Name:</label>
      <span class="readonlyValue" style="padding-left: 40px; width: 150px;">
        <%: Model.BilledMemberName %>
      </span>
    </div>
    <div>
      <label class="boldText">
        Invoice No.
      </label>
      <span class="readonlyValue">
        <%: Model.InvoiceNo %>
      </span>
    </div>
    <div>
      <label for="invoiceDate" class="boldText">
        Invoice Date:</label>
      <span class="readonlyValue">
        <%: Model.FormatedInvoiceDate %>
      </span>
    </div>
    <div>
      <label for="chargeCategory" class="boldText">
        Billing Category:</label>
      <span class="readonlyValue">
        <%: Model.BillingCategory %>
      </span>
    </div>
    <div>
      <label class="boldText">
        Invoice Currency:</label>
      <span class="readonlyValue">
        <%: Model.InvoiceCurrency %>
      </span>
    </div>
    <div>
      <label class="boldText">
        Invoice Amount:</label>
      <span class="readonlyValue">
        <%: Model.InvoiceAmount %>
      </span>
    </div>
  </div>
</fieldset>
<table>
  <tr>
    <td style="text-align: right">
      Received in IS:
    </td>
    <td>
      <%:Model.FormatedReceivedInISDate %>
    </td>
    <td>
    </td>
  </tr>
  <tr>
    <td style="text-align: right">
      Source:
    </td>
    <td>
      <%:Model.Source %>
    </td>
    <td>
    </td>
  </tr>
  <tr>
    <td style="text-align: right">
      Validation:
    </td>
    <td>
      <%: Model.ValidationStatus %>
    </td>
    <td>
      &nbsp; &nbsp;
    </td>
    <td>
      <%: Model.FormatedValidationStatusDate %>
    </td>
  </tr>
  <tr>
    <td style="text-align: right">
      Billing Value Confirmation:
    </td>
    <td>
      <%: Model.ValueConfirmationStatus %>
    </td>
    <td>
      &nbsp; &nbsp;
    </td>
    <td>
      <%: Model.FormatedValueConfirmationStatusDate %>
    </td>
  </tr>
  <tr>
    <td style="text-align: right">
      Digital Signature:
    </td>
    <td>
      <%: Model.DigitalSignatureStatus %>
    </td>
    <td>
      &nbsp; &nbsp;
    </td>
    <td>
      <%: Model.FormatedDigitalSignatureStatusDate %>
    </td>
  </tr>
  <tr>
    <td style="text-align: right">
      Settlement File to ICH:
    </td>
    <td>
      <%: Model.SettlementFileStatus %>
    </td>
    <td>
      &nbsp; &nbsp;
    </td>
    <td>
      <%: Model.FormatedSettlementFileStatusDate %>
    </td>
  </tr>
  <tr>
    <td style="text-align: right">
      Presented to Billed Member:
    </td>
    <td>
      <%: Model.PresentedStatus %>
    </td>
    <td>
      &nbsp; &nbsp;
    </td>
    <td>
      <%: Model.FormatedPresentedStatusDate %>
    </td>
  </tr>
</table>
