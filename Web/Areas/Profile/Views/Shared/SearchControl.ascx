<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.AuditTrail>" %>
<script type="text/javascript">
  $(document).ready(function () {
    $('#auditTrailToPeriod').watermark(_periodFormat);
    $('#auditTrailFromPeriod').watermark(_periodFormat);

    $(function () {
      $('#FromDate').hide();
      $('#ToDate').hide();
      $('input:radio[name=rdDateOrClearancePeriod]').click(function () {
        if ($(this).attr("id") == 'rdClearancePeriod') {
          $('#FromPeriod').show();
          $('#ToPeriod').show();
          $('#FromDate').hide();
          $('#ToDate').hide();
        } else {
          $('#FromPeriod').hide();
          $('#ToPeriod').hide();
          $('#FromDate').show();
          $('#ToDate').show();
        }
      });

    });

  });
</script>
<div class="solidBox ">
  <div>
    <div class="fieldContainer horizontalFlow">
      <%:Html.HiddenFor(model=>model.ElementList) %>
      <input type="hidden" id="userCategory" value='<%: ViewData["userCategory"]%>' />
      <div>        
        <input type="radio" id="rdClearancePeriod" name="rdDateOrClearancePeriod" value="ClearancePeriod" checked="checked" />Clearance
        Period
        <input type="radio" id="rdDate" name="rdDateOrClearancePeriod" value="Date" />Date
      </div>
    </div>
    <div class="fieldContainer horizontalFlow">
      <div id="FromDate">
        <div>
          <label for="fromDate">
            <span class="required">* </span>From Date:</label>
          <%: Html.TextBoxFor(model => model.FromDate, new { @id = "auditTrailFromDate", @class = "datePicker", @readOnly = true })%>
        </div>
        <div>
          <label for="toDate">
            <span class="required">* </span> To Date:</label>
          <%: Html.TextBoxFor(model => model.ToDate, new { @id = "auditTrailToDate", @class = "datePicker", @readOnly = true })%>
        </div>
      </div>
      <div id="FromPeriod">
        <div>
          <label for="fromPeriod">
            <span class="required">* </span>From Period:</label>
          <%: Html.TextBoxFor(model => model.FromPeriod, new { @id = "auditTrailFromPeriod" })%>
        </div>
        <div>
          <label for="toPeriod">
            <span class="required">* </span>To Period:</label>
          <%: Html.TextBoxFor(model => model.ToPeriod, new { @id = "auditTrailToPeriod" })%>
        </div>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
  <br />
  <div class=" fieldContainer horizontalFlow">
    <div>
      <label>
        User:</label>
      <%:Html.UserDropdownListFor(model => model.User, Convert.ToInt32( ViewData["ReportTypeCategory"]))%>
    </div>
  </div>
  <br />
  <%
    bool isNotVisible = Convert.ToBoolean(ViewData["IsGroupNotVisible"]);
    if (!isNotVisible)
    {%>
  <div class=" fieldContainer horizontalFlow">
     <br /><label>
        Element Group:</label>
    <table border="1" width="20%">
      <%
        var searchCritieraDataTable = EnumMapper.GetElementGroupType();
      %>
      <%
        for (int i = 0; i < searchCritieraDataTable.Count; i++)
        {
      %>
      <tr>
        <td>
          <%:searchCritieraDataTable.ElementAt(i).Text%>
        </td>
        <td>
          <input iscontact="true" type="checkbox" name="group" id="<%:searchCritieraDataTable.ElementAt(i).Value%>" />
        </td>
      </tr>
      <%
        }
      %>
    </table>
  </div>
  <%
}%>
  <div class="clear">
  </div>
</div>
