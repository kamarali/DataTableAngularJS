<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="solidBox fieldContainer horizontalFlow" style="height: 20px;">
  <!-- <div class = "fieldContainer horizontalFlow" style="text-align:justify;">Status Key: -->
  <!--<div style = "float:left; vertical-align:text-top;">Status Key:</div> -->
  <div>
    <div class="dashboardLegend" style="width: 80px;">
      <b><span style="vertical-align: sub;">Status Key:</span></b>
    </div>
    <div class="dashboardLegend" style="width: 120px;">
      <img src='<%:Url.Content("~/Content/Images/status_failed.png") %>' class="imglegend" alt="Failed" style="vertical-align: sub;" />
      <span style="vertical-align: sub;">Failed</span>
    </div>
    <div class="dashboardLegend" style="width: 130px;">
      <img src='<%:Url.Content("~/Content/Images/status_pending.png") %>' class="imglegend" alt="Pending" style="vertical-align: sub;" />
      <span style="vertical-align: sub;">Pending</span>
    </div>
    <div class="dashboardLegend" style="width: 210px;">
      <img src='<%:Url.Content("~/Content/Images/status_succesfully_completed.png") %>' class="imglegend" alt="Successfully Completed"
        style="vertical-align: sub;" /> <span style="vertical-align: sub;">Successfully Completed</span>
    </div>
    <div class="dashboardLegend">
      <img src='<%:Url.Content("~/Content/Images/status_suspended_member.png") %>' class="imglegend" alt="Suspended member" style="vertical-align: sub;" />
      <span style="vertical-align: sub;">Suspended member</span>
    </div>
    <div class="dashboardLegend">
      <img src='<%:Url.Content("~/Content/Images/status_late_submission.png") %>' class="imglegend" alt="Late Submission" style="vertical-align: sub;" />
      <span style="vertical-align: sub;">Late Submission</span>
    </div>
  </div>
  <!-- </div> -->
</div>
