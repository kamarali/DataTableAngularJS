<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Location>" %>
<div class = "payment hidden">
  <div>
    <label>
      Bank Name:</label>
    <%:Html.TextBoxFor(location => location.BankName, new { disabled = true }) %>
  </div>
  <div>
    <label>
      IBAN:</label>
    <%:Html.TextBoxFor(location => location.Iban, new { disabled = true }) %>
  </div>
  <div>
    <label>
      SWIFT:</label>
    <%:Html.TextBoxFor(location => location.Swift, new { disabled = true })%>
  </div>
  <div>
    <label>
      Bank Code:</label>
    <%:Html.TextBoxFor(location => location.BankCode, new { disabled = true })%>
  </div>
  <div>
    <label>
      Branch Code:</label>
    <%:Html.TextBoxFor(location => location.BranchCode, new { disabled = true })%>
  </div>  
</div>
<div class = "bottomLine payment hidden">
  <div>
    <label>
      Bank Account Number:</label>
    <%:Html.TextBoxFor(location => location.BankAccountNumber, new { disabled = true })%>
   </div>
  <div>
    <label>
      Bank Account Name:</label>
    <%:Html.TextBoxFor(location => location.BankAccountName, new { disabled = true })%>
  </div>
  <div>
    <label>
      Currency Code:</label>
    <%:Html.TextBoxFor(location => location.Currency.Code, new { disabled = true })%>
  </div>
</div>
