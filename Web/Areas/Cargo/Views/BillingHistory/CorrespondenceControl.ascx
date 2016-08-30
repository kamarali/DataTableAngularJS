<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoCorrespondence>" %>

<%Html.RenderPartial("~/Views/Correspondence/AuditTrailCorrespondenceAcceptanceDetails.ascx", Model); %>

<table class="formattedTable">
  <thead>
    <tr>
      <td>
        From Member
      </td>
      <td>
        To Member
      </td>
      <td>
        Correspondence Date
      </td>
      <td>
        Correspondence Ref. No.
      </td>
      <td>
        Correspondence Stage
      </td>
      <td>
        Authority To Bill
      </td>
      <td colspan="2">
        Amount to be Settled
      </td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%: Model.FromMember!= null ? string.Format("{0}-{1}-{2}", Model.FromMember.MemberCodeAlpha, Model.FromMember.MemberCodeNumeric, Model.FromMember.CommercialName) : string.Empty%>
      </td>
      <td>
        <%: Model.FromMember!= null ? string.Format("{0}-{1}-{2}", Model.ToMember.MemberCodeAlpha, Model.ToMember.MemberCodeNumeric, Model.ToMember.CommercialName) : string.Empty %>
      </td>
      <td>
        <%: Model.CorrespondenceDate.ToString(FormatConstants.DateFormat) %>
      </td>
      <td>
        <%: Model.CorrespondenceNumber.HasValue ? Model.CorrespondenceNumber.Value.ToString(FormatConstants.CorrespondenceNumberFormat) : string.Empty%>
      </td>
      <td>
        <%: Html.DisplayFor(m => m.CorrespondenceStage)%>
      </td>
      <td>
        <%: Model.AuthorityToBill ? "Yes" :"No" %>
      </td>
      <td colspan="2">
        <%: Model.Currency != null? Model.Currency.Code : string.Empty%>
        <%: Html.DisplayFor(m => m.AmountToBeSettled)%>
      </td>
    </tr>
  </tbody>
</table>

<div>
  <b>Correspondence Details</b>:
  <%: Html.DisplayFor(m => m.CorrespondenceDetails)%>
</div>

<%
  if (Model.Attachments.Count > 0)
  {
%>
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
    foreach (var attachment in Model.Attachments)
    {%>
  <a href="<%:Url.Action("BillingHistoryCorrAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
    }
  %>
</div>
<br />
<%
  }
%>