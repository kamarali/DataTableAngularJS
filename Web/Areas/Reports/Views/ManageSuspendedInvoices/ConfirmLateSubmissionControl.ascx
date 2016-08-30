<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="solidBox">
<div>
<label id="lateSubmissionMsg"></label>

</div>
  <div class="fieldContainer horizontalFlow">
    <%: Html.Label("")%>Resubmission Period
    <%: Html.RadioButton("rbResubmissionPeriod", "Current", true)%>Current
    <%: Html.RadioButton("rbResubmissionPeriod", "Previous", false)%>Previous
  </div>
</div>
