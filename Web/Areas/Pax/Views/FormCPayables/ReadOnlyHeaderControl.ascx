<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormC>" %>
<h2>
  Header Details</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
      <div>
        Provisional Billing Month: <b>
          <%: string.Format("{0}-{1}", Model.ProvisionalBillingYear, new System.Globalization.DateTimeFormatInfo().GetAbbreviatedMonthName(Model.ProvisionalBillingMonth)) %></b>
      </div>
      <div>
         <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Receivables)
           {%>
         Provisional Billing Member: <b>
          <%:string.Format("{0}-{1}", Model.ProvisionalBillingMember.MemberCodeAlpha, Model.ProvisionalBillingMember.MemberCodeNumeric)%></b>
          <%
           }
           else
           {%>
           From Member: <b>
          <%:string.Format("{0}-{1}", Model.FromMember.MemberCodeAlpha, Model.FromMember.MemberCodeNumeric)%></b>
          <%
           }%>
      </div>
      <div>
        Billing Code: <b>
          <%: EnumMapper.GetBillingCodeDisplayValue(Iata.IS.Model.Pax.Enums.BillingCode.SamplingFormC) %></b>
      </div>
      <%if (Model.NilFormCIndicator == "N")
        {%>
      <div>
        Total Gross Amount/ALF: <b>
          <%:string.Format("{0} {1}", Model.ListingCurrency.Code, Model.TotalGrossAmount.ToString(FormatConstants.TwoDecimalsFormat))%></b>
      </div>
      <%
        }%>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
