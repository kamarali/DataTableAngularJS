<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.CorrespondenceTrailSearchCriteria>" %>
<h2>
  Correspondence Search Criteria</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow" style="width:805px">
    <div>
      <div>
        <label for="MemberCode">
          Member Code:</label>
        <%:Html.TextBoxFor(model => model.CorrBilledMemberText, new {@class="autocComplete"})%>
        <%:Html.TextBoxFor(model => model.CorrBilledMemberId, new {@class="hidden"}) %>
      </div>
      <div>
        <label for="FromDate">
          <span class="required">*</span> From Date:</label>
        <%:Html.TextBox(ControlIdConstants.CorrespondenceFromDate, Model.FromDate != null ? Model.FromDate.Value.ToString(FormatConstants.DateFormat):null, new { @class = "datePicker"})%>
      </div>
      <div>
        <label for="ToDate">
          <span class="required">*</span> To Date:</label>
        <%:Html.TextBox(ControlIdConstants.CorrespondenceToDate, Model.ToDate != null ? Model.ToDate.Value.ToString(FormatConstants.DateFormat):null, new { @class = "datePicker"})%>
      </div>
    </div>
    <div>
      <div>
        <label for="CorrespondenceInitiator">
          Correspondence Initiated By:</label>
        <%: Html.CorrespondenceReportInitiatingMemberDropdownList(ControlIdConstants.InitiatingMember, Model.InitiatingMember ?? 0)%>
      </div>
      <div>
        <label for="CorrespondenceStatus">
          Correspondence Status:</label>
        <%: Html.CorrespondenceReportStatusDropdownList(ControlIdConstants.CorrespondenceStatusId, Model.CorrespondenceStatusId)%>
      </div>
      <div>
        <label for="CorrespondenceSubStatus">
          Correspondence Sub Status:</label>
        <%: Html.CorrespondenceReportSubStatusDropdownList(ControlIdConstants.CorrespondenceSubStatusId, Model.CorrespondenceSubStatusId)%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
