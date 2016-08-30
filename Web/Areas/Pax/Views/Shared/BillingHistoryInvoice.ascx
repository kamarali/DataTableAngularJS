<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<h2>
  Invoices</h2>
<form id = "BHSelectInvoiceForm" action= "">
<div class="solidBox dataEntry">
  <div class="fieldContainer verticalFlow">
    <div>
      <div>
        <label for="ddlInvoice">
          Select Invoice:
        </label>
        <select id="ddlInvoice">
        </select>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input type="button" id="btnOk" value="Ok" onclick="callMemo();" class="primaryButton" />
  <input type="button" id="btnClose" value="Cancel" onclick="closeDialog('#divBillingHistoryInvoice');" class="secondaryButton" />
</div>
</form>
<script type="text/javascript">
  function callMemo() {
    if ($("#ddlInvoice").val() != "") {
      var url = '';
      if (memoType == 'RejectionMemo') {
        if (billingCode == 5)
          url = '<%: Url.Action("RMCreate","FormF", new {invoiceId = "replaceInvoiceId" })%>'.replace("replaceInvoiceId", $("#ddlInvoice").val());
        else if (billingCode == 6)
          url = '<%: Url.Action("RMCreate","FormXF", new {invoiceId = "replaceInvoiceId" })%>'.replace("replaceInvoiceId", $("#ddlInvoice").val());
        else if (billingCode == 0)
          url = '<%: Url.Action("RMCreate","Invoice", new {invoiceId = "replaceInvoiceId" })%>'.replace("replaceInvoiceId", $("#ddlInvoice").val());
        
        document.location.href = url;        
      }
      else if (memoType == 'BillingMemo') {
        document.location.href = '<%: Url.Action("BMCreate","Invoice", new {invoiceId = "replaceInvoiceId" })%>'.replace("replaceInvoiceId", $("#ddlInvoice").val());
      }
    }
    else
      alert("Please select Invoice");
  }
</script>

