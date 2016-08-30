<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.IchConfiguration>" %>
<script type="text/javascript">
  SponsorMemberCapture(); 
</script>
<% using (Html.BeginForm("GetMemberList", "Member", FormMethod.Post))
   {%>
<div>
  <div class="fieldContainer verticalFlow">
    <div class="twoColumnWidth">
      <div>
        <%: Html.TextBoxFor(ich => ich.DisplayMemberText, new { id = "SponsordMemberText",  style = "width:270px", @class = "autocComplete" })%>
        <%: Html.TextBoxFor(ich => ich.MemberId, new { @class = "hidden", id = "MemberId" })%>
        <input id="hiddenMemberIdAdd" type="hidden" />
        <input id="hiddenMemberIdRemove" type="hidden" />
        <input id="hiddenSelfId" type="hidden" value="<%: Session["SelectedMemberId"]%>" />
      </div>
    </div>
    <div class="oneColumnWidth">
      <div class="buttonContainer">
        <input class="secondaryButton" id="Add" value="Add" type="button" />
      </div>
    </div>
  </div>
  <div class="fieldContainer verticalFlow">
    <div class="twoColumnWidth">
      <div>
        <%: Html.ListBox("availableMembers", (MultiSelectList)ViewData["Members"], new { size = "8" ,  style = "width:270px"})%>
      </div>
      <div>
        <label class="hidden" id="spnFuturePeriodlbl">
          Future Period:</label>
        <%: Html.TextBoxFor(ich => ich.SponsororFuturePeriod, new { id = "spnFuturePeriod", @class = "hidden" })%>
      </div>
    </div>
    <div class="oneColumnWidth">
      <div class="buttonContainer">
        <input class="secondaryButton" id="remove" value="Remove" type="button" />
      </div>
    </div>
  </div>
</div>
<%} %>