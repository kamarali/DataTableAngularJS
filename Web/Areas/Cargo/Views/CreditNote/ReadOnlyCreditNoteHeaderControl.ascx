<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.Enums" %>
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
  <%-- <div>
        Billing Code: <b>
          <%: EnumMapper.GetCgoBillingCodeDisplayValue((Iata.IS.Model.Cargo.Enums.BillingCode)(int) Iata.IS.Model.Cargo.Enums.BillingCode.CreditNote)%></b>
      </div>--%>
      <div>
        Listing Amount: 
         <b>
          <%: Model.ListingCurrencyDisplayText %>
          <%: Model.CGOInvoiceTotal.NetTotal.ToString(FormatConstants.ThreeDecimalsFormat)%></b>
      </div>
     <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        Listing to Billing Rate: <b>
          <%: Model.ExchangeRate.HasValue ? Model.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat) : string.Empty%></b>
      </div>
      <div>
        Billing Amount: <b>
         <%: Model.BillingCurrencyDisplayText %>
         <%: Model.CGOInvoiceTotal.NetBillingAmount.ToString(FormatConstants.ThreeDecimalsFormat)%></b>
      </div>
       <div>
        Credit Note Status: <b>
          <%: Model.InvoiceStatusDisplayText%></b>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
