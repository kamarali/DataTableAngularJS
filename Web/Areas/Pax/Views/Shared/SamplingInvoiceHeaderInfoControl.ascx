<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<h2>
  Header Details</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Receivables)
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
          <%: EnumMapper.GetSettlementMethodDisplayValue((int)Model.InvoiceSmi) %></b>
      </div>
      <div>
        Invoice Number: <b>
          <%: Model.InvoiceNumber %></b>
      </div>
    </div>
    <div>
      <div>
        Billing Code: <b>
          <%: EnumMapper.GetBillingCodeDisplayValue((BillingCode) Model.BillingCode) %></b>
      </div>
      <div>
        Provisional Billing Month:
        <%if (Model.ProvisionalBillingMonth != 0)
          {%>
        <b>
          <%: string.Format("{0}-{1}", Model.ProvisionalBillingYear, new System.Globalization.DateTimeFormatInfo().GetAbbreviatedMonthName(Model.ProvisionalBillingMonth)) %></b>
        <%}
          else
          {%>
        <b>
          <%: "-" %></b>
        <%}
        %>
      </div>
      <div>
        Listing Amount: <b>
          <%: Model.ListingCurrencyDisplayText %>
          <% if (Model.BillingCode == (int)BillingCode.SamplingFormDE)
             {%>
          <%: Model.SamplingFormEDetails != null ? Model.SamplingFormEDetails.NetAmountDue.ToString(FormatConstants.TwoDecimalsFormat) : "0.00"%>
          <%}
             else
             {%>
          <%: Model.InvoiceTotalRecord.NetTotal.ToString(FormatConstants.TwoDecimalsFormat)%>
          <%}%>
        </b>
      </div>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        Listing/Evaluation to Billing Rate: <b>
          <%:Model.ExchangeRate.HasValue ? Model.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat) : string.Empty%>
          <%:Html.Hidden("ExchangeRate", Model.ExchangeRate.HasValue ? Model.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat) : string.Empty)%>
          </b>
      
      </div>
    </div>
    <div>
      <div>
        Billing Amount: <b>
          <%: Model.BillingCurrencyDisplayText %>
          <% if (Model.BillingCode == (int)BillingCode.SamplingFormDE)
             {%>
          <%: Model.SamplingFormEDetails != null ? Model.SamplingFormEDetails.NetAmountDueInCurrencyOfBilling.ToString(FormatConstants.TwoDecimalsFormat) : "0.00"%>
          <%}
             else
             {%>
          <%: Model.InvoiceTotalRecord.NetBillingAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
          <%}%></b>
      </div>
      <div>
        Invoice Status: <b>
          <%: Model.InvoiceStatus %></b>
      </div>
      <% if (Model.BillingCode == (int)BillingCode.SamplingFormDE)
         {
      %>
      <%
        }
      %>
    </div>
    <div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
