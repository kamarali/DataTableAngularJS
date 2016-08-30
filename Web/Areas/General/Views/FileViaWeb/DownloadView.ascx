<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.IsInputFile>" %>
<% using (Html.BeginForm("FileManagerDownload", "FileViaWeb", FormMethod.Post))
   { %>
<legend>
  <%= System.Configuration.ConfigurationManager.AppSettings["SearchCriteriaHeader1"]%></legend>
<%if (ViewData.ContainsKey("DownloadFileError")) %>
<%{ %>
<font color="red">
  <%= Html.Encode(ViewData["DownloadFileError"])%></font>
<%} %>
<h2>
  Search Criteria</h2>
<div class="searchCriteria" style="height:150px;">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style="float: left">
          <span>
            <%= Iata.IS.Web.AppSettings.BillingMonthFrom%>
          </span>
          <br />
          <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.BillingMonthFrom)%>
        </div>
        <div style="float: left">
          <span>
            <%= Iata.IS.Web.AppSettings.BillingPeriodFrom%>
          </span>
          <br />
          <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.BillingPeriodFrom)%>
        </div>
        <div style="float: left">
          <span>
            <%= Iata.IS.Web.AppSettings.BillingMonthTo%>
          </span>
          <br />
          <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.BillingMonthTo)%>
        </div>
        <div style="float: left">
          <span>
            <%= Iata.IS.Web.AppSettings.BillingPeriodTo%>
          </span>
          <br />
          <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.BillingPeriodTo)%>
        </div>
        <div style="float: left">
          <span>
            <%= Iata.IS.Web.AppSettings.BillingYear%>
          </span>
          <br />
          <%: Html.BillingYearDropdownListFor(searchCriteria => searchCriteria.BillingYear)%>
        </div>
      </div>
      <div>
        <div style="width: 375px">
      <span>
        <%= Iata.IS.Web.AppSettings.FileType%>
      </span>
      <br />
      <%= Html.FileFormatTypeDropdownListFor(searchCriteria => searchCriteria.FileFormatId, true,false, new { style = "width:300px" }, false)%>
      </div>
        <%--CMP#622: MISC Outputs Split as per Location IDs--%>
        <div style="width: 250px">
            <label id="locationId">Location ID:</label>
            <%:Html.ListBox("AssociatedLocation", (MultiSelectList)TempData["AssociatedLocation"], new { @style = "width: 120px;height:80px;" })%>
            <%: Html.HiddenFor(m => m.MiscLocationCode)%>            
        <%--  <%:Html.TextBoxFor(searchCriteria => searchCriteria.MiscLocationCode, new { @class = "autocComplete", style = "width:120px;" })%> --%>
        </div>
      </div>     
      <input class="primaryButton" type="submit" id="submitOutputFileSearch" value="Search"  style=" margin-top : -50px;"/>
    </div>
  </div>
</div>
<br />
<%--<div>
  <h2>
    Search Results</h2>
  <%Html.RenderPartial("FileDownloadGrid", ViewData["FileDownloadGridData"]); %>
</div>--%>
<% } %>
