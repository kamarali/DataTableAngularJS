<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Contact>" %>
<div class="fieldContainer verticalFlow">
  <div>
    <% Html.RenderPartial("SearchContactAssignment");%></div>
  <div class="halfWidthColumn" id="divSearchContactAssignment">
  </div>
</div>
<div>
  <%: Html.HiddenFor(contact => contact.ContactList, new { @id = "contactsassignContactList" })%>
</div>
