<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="solidBox dataEntry">
  <div class="fieldContainer">
    <div>
      Please provide a Common Rejection Reason Code to be used in the rejected Line Item(s).
      <br />
      These may be modified later by editing the individual Line Item(s) once the Invoice
      Header has been saved.
      <br />
    </div>
    <div class="clear">
    </div>
  </div>
  <div class="fieldContainer">
    <div class="clear">
    </div>
    <div>
      <label for="Amount">
        <span>*</span> Rejection Reason Code:</label>
      <%: Html.TextBox("RejectionReasonCodeText", "", new { @class = "autocComplete upperCase" })%>
     
      <br />
    </div>
    <div class="clear">
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Save Invoice Header"
    id="SaveInvoiceHeader2" onclick="ValidateRejectionReasoneCode();" />
  <input class="secondaryButton" type="button" value="Close" onclick="closeRejectionReasonCodeDetail();" />
</div>
