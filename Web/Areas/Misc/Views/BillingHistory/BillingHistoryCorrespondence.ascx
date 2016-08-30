<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscCorrespondence>" %>
<%--  CMP527: Show comments if closed by initatior--%>
<%Html.RenderPartial("~/Views/Correspondence/AuditTrailCorrespondenceAcceptanceDetails.ascx", Model); %>
<table class="formattedTable">
  <thead>
    <tr>
     <%--TFS#9987:Mozilla:V46: Table alignment is not proper when genearting audit trail for Correspondence Stage 1-MISC
     Desc:class rowspan=2 removed from td--%> 
      <td>
        From Member
      </td>
      <td>
        To Member
      </td>
      <td>
        Correspondence<br /> Date
      </td>
      <td>
        Correspondence<br /> Ref. No.
      </td>
      <td>
        Charge Category
      </td>
      <td>
        Authority<br /> To Bill
      </td>
      <td colspan="2">
        Amount<br /> to be Settled
      </td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%: string.Format("{0}-{1}-{2}", Model.FromMember.MemberCodeAlpha, Model.FromMember.MemberCodeNumeric, Model.FromMember.CommercialName) %>
      </td>
      <td>
        <%: string.Format("{0}-{1}-{2}", Model.ToMember.MemberCodeAlpha, Model.ToMember.MemberCodeNumeric, Model.ToMember.CommercialName) %>
      </td>
      <td>
        <%: Model.CorrespondenceDate.ToString(FormatConstants.DateFormat) %>
      </td>
      <td>        
        <%: Model.CorrespondenceNumber.HasValue ? Model.CorrespondenceNumber.Value.ToString(FormatConstants.CorrespondenceNumberFormat) : string.Empty%>
      </td>
      <td>
        <%: Html.DisplayFor(m => m.Invoice.ChargeCategory.Name)%>
      </td>
      <td>
        <%: Model.AuthorityToBill ? "Yes" :"No" %>
      </td>
      <td colspan="2">
        <%: Model.Currency.Code %>
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
    foreach (var document in Model.Attachments)
    {  
  %>
  <a href="<%:Url.Action("CorrespondenceAttachmentDownload","Correspondence", new {attachmentId = document.Id}) %>">
    <%:document.OriginalFileName%></a><br />
  <%
    }
  %>
</div>
<%
  }
%>
