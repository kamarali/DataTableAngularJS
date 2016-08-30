<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.OfflineReportLog.OfflineReportSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            Report:</label>
          <%:Html.OfflineReportDropdown("ReportId")%>
        </div>
        <div>
        <label for="FromDate">
           UTC Date of Report Request:</label>     
          <%:Html.TextBox("RequestDateTime",Model.RequestDateTime != null ? Model.RequestDateTime.Value.ToString(FormatConstants.DateFormat) : null,new
{
@class = "datePicker"
})%>          
      </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>

<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
</div>
<div class="clear">
</div>
