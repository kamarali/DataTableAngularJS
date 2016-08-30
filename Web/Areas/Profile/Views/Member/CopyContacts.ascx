<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Contact>" %>

<div class="solidBox verticalFlow">
  <div>
    <div>
      <label for="CopyContactAssignmentsOfUser">
        Copy Contact Assignments of User</label>
      <%: Html.MemberContactsDropdownListFor(contactModel => contactModel.Id, Model.MemberId, "copyoldcontact")%>
    </div>
    <div>
      <label for="newContact">
        New Contact Person</label>
      <%: Html.MemberContactsDropdownListFor(contactModel => contactModel.Id, Model.MemberId, "copynewcontact")%>
    </div>
    <div style = "display:none">
         <%: Html.MemberEmailsDropdownListFor(contactModel => contactModel.Id, Model.MemberId, "copyEmail")%>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="OK" onclick="copyContacts('<%:Url.Action("Copycontacts", "Member", new { area = "Profile"}) %>');" />
  <input class="secondaryButton" type="button" value="Exit"  onclick="if ($('#divType').val() =='divCopyContacts') { $('#divCopyContacts').dialog('close'); } else { $('#divCopyContacts').dialog('close'); }" />
</div>
<div class="clear">
</div>

<script type="text/javascript">
    $(document).ready(function () {
        $("#copyoldcontact option").each(function () {
            var a = $(this);
            $("#copyEmail option").each(function () {
                if ($(this).val() == a.val())
                    a.attr({ 'title': $(this).html() });
            });
        });
        $("#copynewcontact option").each(function () {
            var a = $(this);
            $("#copyEmail option").each(function () {
                if ($(this).val() == a.val())
                    a.attr({ 'title': $(this).html() });
            });
        });
    });    
</script>