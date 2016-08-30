<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoRejectionMemo>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Header Details</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
      <div>
       <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Receivables)
         {%>
        Billed Member: <b>
       
        <%:string.Format("{0}-{1:D3}", Model.Invoice.BilledMember.MemberCodeAlpha, Model.Invoice.BilledMember.MemberCodeNumeric)%></b>
        <%
         }
         else
         {%> 
         Billing Member: <b>
         <%:string.Format("{0}-{1:D3}", Model.Invoice.BillingMember.MemberCodeAlpha, Model.Invoice.BillingMember.MemberCodeNumeric)%></b>
         <%
         }%>
      </div>
      <div>
        Billing Period: <b>
          <%: Model.Invoice.DisplayBillingPeriod %></b>
      </div>
      <div>
        Settlement Method: <b>
         <%:EnumMapper.GetSettlementMethodDisplayValue((int)Model.Invoice.InvoiceSmi) %>
        </b>
      </div>
      <div>
        <%--<%:Model.InvoiceType == InvoiceType.CreditNote? "Credit Note" : "Invoice"%>--%> Invoice Number: <b>
          <%: Model.Invoice.InvoiceNumber%></b>
      </div>
    </div>
    <div>
      <%--<div>
        Billing Code: <b>
          <%: EnumMapper.GetBillingCodeDisplayValue((BillingCode) Model.BillingCode) %></b>
      </div>--%>
      <div>
        Listing Amount: <b>
          <%: Model.Invoice.ListingCurrencyDisplayText %>
         <%: Model.Invoice.CGOInvoiceTotal.NetTotal.ToString(FormatConstants.ThreeDecimalsFormat)%></b>
      </div>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        Listing/Evaluation to Billing Rate: <b>
          <%:Model.Invoice.ExchangeRate.HasValue ? Model.Invoice.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat):string.Empty %></b>
      </div>
      <div>
        Billing Amount: <b>
          <%: Model.Invoice.BillingCurrencyDisplayText %>
          <%: Model.Invoice.CGOInvoiceTotal.NetBillingAmount.ToString(FormatConstants.ThreeDecimalsFormat)%></b>
      </div>
       <div>
       Invoice Status: <b>
          <%: Model.Invoice.InvoiceStatusDisplayText%></b>
      </div>
    </div>
    <div>     
      <div>
       Memo Number: <b>
          <%: Model.RejectionMemoNumber%></b>
      </div>    
       <div>
        Reason Code: <b>
          <%: Model.ReasonCode%></b>
      </div>        
    </div>
  </div>
  <div class="clear">
  </div>
</div>
