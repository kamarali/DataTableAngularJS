<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<h2>
  Header Details</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
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
        Billing Period: <b>
          <%: Model.DisplayBillingPeriod %></b>
      </div>
      <div>
        Settlement Method: <b>
          <%: EnumMapper.GetSettlementMethodDisplayValue((int)Model.InvoiceSmi) %>
        </b>
      </div>
      <div>
        Credit Note No.: <b>
          <%: Model.InvoiceNumber %></b>
      </div>
    </div>
    <div>
      <div>
        Billing Code: <b>
          <%: EnumMapper.GetBillingCodeDisplayValue((BillingCode) Model.BillingCode) %></b>
      </div>
      <div>
        Listing Amount: <b>
          <%: Model.ListingCurrencyDisplayText %>
          <%: Model.ListingAmount.ToString(FormatConstants.TwoDecimalsFormat)%></b>
      </div>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        Listing/Evaluation to Billing Rate: <b>
          <%: Model.ExchangeRate.HasValue ? Model.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat):string.Empty%></b>
      </div>
      <div>
        Billing Amount: <b>
          <%: Model.BillingCurrencyDisplayText %>
          <%: Model.BillingAmount.ToString(FormatConstants.TwoDecimalsFormat)%></b>
      </div>
    </div>
    <div>
      <div>
        Credit Note Status: <b>
          <%: Model.DisplayInvoiceStatus %></b>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
