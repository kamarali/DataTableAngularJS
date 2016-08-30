<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Base.InvoiceBase>" %>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
  <%:Html.TextAreaFor(invoice => invoice.ResubmissionRemarks, new { @maxlength = 200, @rows = 5, @cols = 40 })%>
  <%:Html.HiddenFor(invoice=>invoice.Id,new{@id="hiddenInvoiceId"}) %>
  <%:Html.HiddenFor(invoice => invoice.ResubmissionRemarks, new { @id = "hiddenResubmissionRemark" })%>
  </div>
</div>
<div class="clear"></div>
