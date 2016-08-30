﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RejectionMemo>" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<h2>
  Header Details</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Receivables)
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
          <%:Model.Invoice.DisplayBillingPeriod%></b>
      </div>
      <div>
        Settlement Method: <b>
          <%:EnumMapper.GetSettlementMethodDisplayValue((int) Model.Invoice.InvoiceSmi)%></b>
      </div>
      <div>
        Invoice Number: <b>
          <%:Model.Invoice.InvoiceNumber%></b>
      </div>
    </div>
    <div>
      <div>
        Billing Code: <b>
          <%:EnumMapper.GetBillingCodeDisplayValue((BillingCode) Model.Invoice.BillingCode)%></b>
      </div>
      <div>
        Provisional Billing Month:
        <%
          if (Model.Invoice.ProvisionalBillingMonth != 0)
          {%>
        <b>
          <%: string.Format("{0}-{1}", Model.Invoice.ProvisionalBillingYear, new System.Globalization.DateTimeFormatInfo().GetAbbreviatedMonthName(Model.Invoice.ProvisionalBillingMonth)) %></b>
        <%
          }
          else
          {%>
        <b>
          <%: "-" %></b>
        <%
          }
        %>
      </div>
      <div>
        Listing Amount: <b>
          <%: Model.Invoice.ListingCurrencyDisplayText %>
          <%: Model.Invoice.ListingAmount.ToString(FormatConstants.TwoDecimalsFormat) %></b>
      </div>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        Listing/Evaluation to Billing Rate: <b>
          <%:Model.Invoice.ExchangeRate.HasValue? Model.Invoice.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat): string.Empty%></b>
      </div>
    </div>
    <div>
      <div>
        Billing Amount: <b>
          <%: Model.Invoice.BillingCurrencyDisplayText %>
          <%: Model.Invoice.BillingAmount.ToString(FormatConstants.TwoDecimalsFormat)%></b>
      </div>
      <div>
        Rejection Memo No.: <b>
          <%: Model.RejectionMemoNumber %></b>
      </div>
      <div>
        Source Code: <b>
          <%: Model.SourceCodeId %></b>
      </div>
      <div>
        Reason Code: <b>
          <%: Model.ReasonCode %></b>
      </div>
    </div>
    <div>
      <div>
        Invoice Status: <b>
          <%: Model.Invoice.DisplayInvoiceStatus %></b>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
