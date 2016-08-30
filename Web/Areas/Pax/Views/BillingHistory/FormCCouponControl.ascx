<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormC>" %>
<%
  foreach (var cCouponRecord in Model.SamplingFormCDetails)
  {
%>
<table class="formattedTable">
  <thead>
    <tr>
      <td>
        Provisional Billing Month
      </td>
      <td>
        Billing Member
      </td>
      <td>
        Billed Member
      </td>
      <td>
        Provisional Invoice Number
      </td>
      <td>
        Billing Code
      </td>
      <td>
        Doc No
      </td>
      <td>
        Coupon No
      </td>
      <td>
        Batch Number
      </td>
      <td>
        Record Sequence Number
      </td>
      <td>
        Rason Code
      </td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%:string.Format("{0} {1} P{2}",
                                      System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Model.ProvisionalBillingMonth),
                                      Model.ProvisionalBillingYear,
                                      Model.ProvisionalBillingPeriod.Period)%>
      </td>
      <td>
        <%:Model.FromMemberText%>
      </td>
      <td>
        <%:Model.ProvisionalBillingMemberText%>
      </td>
      <td>
        <%: cCouponRecord.ProvisionalInvoiceNumber%>
      </td>
      <td>
        S-C
      </td>
      <td>
        <%: cCouponRecord.DocumentNumber%>
      </td>
      <td>
        <%:cCouponRecord.CouponNumber%>
      </td>
      <td>
        <%: cCouponRecord.BatchNumberOfProvisionalInvoice%>
      </td>
      <td>
        <%: cCouponRecord.RecordSeqNumberOfProvisionalInvoice%>
      </td>
      <td>
        <%: cCouponRecord.ReasonCode%>
      </td>
    </tr>
  </tbody>
</table>
<%
      if (cCouponRecord.Attachments.Count > 0)
      {
%>
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in cCouponRecord.Attachments)
                 {%>
  <a href="<%:Url.Action("BillingHistoryFormCAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
  %>
</div>
<br />
<%
               }

    }

%>
