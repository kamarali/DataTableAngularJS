<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.OldIdecParticipation>" %>
<script type="text/javascript">

  $(document).ready(function () {
    registerAutocomplete('MemberText', 'MemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
  });
</script>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Member:</label>
            <%: Html.TextBoxFor(model => model.MemberText, new { @id = "MemberText", @Class = "autocComplete" })%>
            <%: Html.HiddenFor(model => model.MemberId, new { @id = "MemberId" })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
