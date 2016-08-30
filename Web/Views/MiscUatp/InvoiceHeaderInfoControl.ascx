<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<h2>
  <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
  Header
</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <%:Model.InvoiceType == InvoiceType.CreditNote? "Credit Note" : "Invoice"%>
        Number: <b>
          <%: Model.InvoiceNumber %></b>
      </div>
      <div>
        <%: Model.InvoiceType == InvoiceType.CreditNote? "Credit Note" : "Invoice"%>
        Date: <b>
          <%: Model.InvoiceDate.ToString(FormatConstants.ReadOnlyHeaderDateFormat) %></b>
      </div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Receivables)
         {%>
        Billed Member: <b>
          <%:string.Format("{0}-{1:D3}", Model.BilledMember.MemberCodeAlpha, Model.BilledMember.MemberCodeNumeric)%></b>
        <%
         }
         else
         {%> 
         Billing Member: <b>
          <%:string.Format("{0}-{1:D3}", Model.BillingMember.MemberCodeAlpha, Model.BillingMember.MemberCodeNumeric)%></b>
         <%
         }%>
      </div>
      <div>
        Charge Category: <b>
          <%: Model.ChargeCategory.Name %></b>
      </div>
      <div>
        Billing Amount: <b>
          <%: string.Format("{0} {1}", Model.ListingCurrencyDisplayText, Model.InvoiceSummary.TotalAmount.ToString(FormatConstants.ThreeDecimalsFormat))%></b>
      </div>
    </div>
    <div>
      <div>
        <%:Model.InvoiceType == InvoiceType.CreditNote? "Credit Note" : "Invoice"%>
        Status: <b>
          <%: Model.InvoiceStatusDisplayText %></b>
      </div>
      <div>
        Billing Period: <b>
          <%: Model.DisplayBillingPeriod %></b><%:Html.HiddenFor(invoice => invoice.AttachmentIndicatorOriginal) %>
      </div>
    </div>
    <div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
