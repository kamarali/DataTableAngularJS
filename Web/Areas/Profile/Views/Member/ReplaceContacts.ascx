<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Contact>" %>

<div class="solidBox verticalFlow">
  <div>
    <div>
      <label for="currentContact">
        Current Contact Person</label>
      <%: Html.MemberContactsDropdownListFor(contactModel => contactModel.Id, Model.MemberId, "replaceoldcontact")%>
    </div>
    <div>
      <label for="newContact">
        New Contact Person</label>
      <%: Html.MemberContactsDropdownListFor(contactModel => contactModel.Id, Model.MemberId, "replacenewcontact")%>
    </div>
     <div style = "display:none">
         <%: Html.MemberEmailsDropdownListFor(contactModel => contactModel.Id, Model.MemberId, "rpalceEmail")%>
  </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="OK" onclick="ReplaceContact('<%:Url.Action("Replacecontacts", "Member", new { area = "Profile"}) %>');" />
  <input class="secondaryButton" type="submit" value="Exit" onclick="if ($('#divType').val() =='divSwitchContacts') { $('#divSwitchContacts').dialog('close'); } else { $('#divSwitchContacts').dialog('close'); }" />
</div>
<div class="clear">
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#replaceoldcontact option").each(function () {
            var a = $(this);
            $("#rpalceEmail option").each(function () {
                if ($(this).val() == a.val())
                    a.attr({ 'title': $(this).html() });
            });
        });
        $("#replacenewcontact option").each(function () {
            var a = $(this);
            $("#rpalceEmail option").each(function () {
                if ($(this).val() == a.val())
                    a.attr({ 'title': $(this).html() });
            });
        });
    });    
</script>