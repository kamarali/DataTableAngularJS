<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.IchConfiguration>" %>
<script type="text/javascript">
  AggregatorMemberCapture();

</script>
<% using (Html.BeginForm("", "Member", FormMethod.Post))
   {%>
<div>
  <div class="fieldContainer verticalFlow">
    <div class="twoColumnWidth">
      <div>
       <%=Html.TextBoxFor(ich => ich.DisplayMemberText, new { id = "AggreagatedMemberText", style = "width:270px", @class = "autocComplete" })%>
        <%= Html.TextBoxFor(ich => ich.MemberId, new { @class = "hidden", id = "IchMemberId" })%>
        <input id="hiddenAggrMemberIdAdd" type="Hidden" />
        <input id="hiddenAggrMemberIdRemove" type="Hidden" />
        <input id="hiddenAggrSelfId" type="hidden" value="<%=Session["SelectedMemberId"]%>" />
      </div>
    </div>
    <div class="oneColumnWidth">
      <div class="buttonContainer">
        <input class="secondaryButton" id="addAggregator" value="Add" type="button" />
      </div>
    </div>
  </div>
  <div class="fieldContainer verticalFlow">
    <div class="twoColumnWidth">
      <div>
        <%=Html.ListBox("aggrAvailableMembers", (MultiSelectList)ViewData["Aggregators"], new { size = "8",  style = "width:270px" })%>
      </div>
     
      <div>
        <label class="hidden" id="aggFuturePeriodlbl">
          Future Period:</label>
        <%=Html.TextBoxFor(ich => ich.AggregatorFuturePeriod, new { @id = "aggFuturePeriod", @class = "hidden" })%>
      </div>
    
    </div>
    <div class="oneColumnWidth">
      <div class="buttonContainer">
        <input class="secondaryButton" id="removeAggregator" value="Remove" type="button" />
      </div>
    </div>
  </div>
</div>
<%} %>