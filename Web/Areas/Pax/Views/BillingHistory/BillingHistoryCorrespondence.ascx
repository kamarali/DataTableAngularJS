<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Correspondence>" %>
<table class="formattedTable">
  <thead>
    <tr>
      <td rowspan="2">
        From Member
      </td>
      <td rowspan="2">
        To Member
      </td>
      <td rowspan="2">
        Correspondence Date
      </td>
      <td rowspan="2">
        Correspondence Ref. No.
      </td>
      <td rowspan="2">
        Charge Category
      </td>
      <td rowspan="2">
        Authority To Bill
      </td>
      <td colspan="2">
        Amount to be Settled
      </td>
    </tr>
  </thead>
    <tbody>
    <tr>
      <td rowspan="2">
        <%: Html.Encode(string.Format("{0}-{1}-{2}", Model.FromMember.MemberCodeAlpha, Model.FromMember.MemberCodeNumeric, Model.FromMember.CommercialName))%>
      </td>
      <td rowspan="2">
        <%: Html.Encode(string.Format("{0}-{1}-{2}", Model.ToMember.MemberCodeAlpha, Model.ToMember.MemberCodeNumeric, Model.ToMember.CommercialName))%>
      </td> 
      <td rowspan="2">
        <%: Html.Encode(Model.CorrespondenceDate.ToString(FormatConstants.DateFormat))%>
      </td>
      <td rowspan="2">        
          <%: Model.CorrespondenceNumber.HasValue ? Model.CorrespondenceNumber.Value.ToString(FormatConstants.CorrespondenceNumberFormat) : string.Empty%>
      </td>
      <td rowspan="2">
        <%: Html.DisplayFor(m => m.Invoice.ChargeCategory.Name)%>
      </td>
      <td rowspan="2">
        <%: Html.Encode(Model.AuthorityToBill? "Yes" :"No")%>
      </td>
      <td colspan="2">
      <%: Html.Encode(Model.Currency.Code)%> <%: Html.DisplayFor(m => m.AmountToBeSettled)%>
      </td>
    </tr>
  </tbody>
</table>
<div>
  <b>
    Correspondence Details</b>:
  <%: Html.DisplayFor(m => m.CorrespondenceDetails)%>
</div>