<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.DailyOutputFileDownloadSearch>" %>
<% using (Html.BeginForm("FileManagerDailyOutputDownload", "FileViaWeb", FormMethod.Post, new { id = "DailyOpFileDownloadSearch" }))
   { %>
   <%: Html.AntiForgeryToken() %>
<h2>
  Search Criteria for Daily MISC Bilateral Files to Billed Members: </h2>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style="float: left">
          <span>* </span> Delivery Date From
          <br />
          <%:Html.TextBox(ControlIdConstants.DeliveryDateFrom, Model.DeliveryDateFrom != null ? Model.DeliveryDateFrom.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
        </div>
        <div style="float: left">
          <span>* </span> Delivery Date To
          <br />
          <%:Html.TextBox(ControlIdConstants.DeliveryDateTo, Model.DeliveryDateTo != null ? Model.DeliveryDateTo.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
        </div>
        <div style="width: 370px">
          <span>File Type </span>
          <br />
          <%= Html.FileFormatTypeDropdownListFor(searchCriteria => searchCriteria.FileFormatId, true, false, new { style = "width:300px" }, true)%>
        </div>  
        <%--CMP#622: MISC Outputs Split as per Location IDs--%>
        <div style="float: left">
            <label id="lblLocId"> <span>* </span>Location ID:</label>
             <%:Html.ListBox("DailyAssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new { @style = "width: 120px;height:80px;" })%>
             <%: Html.HiddenFor(m => m.MiscLocCode)%>    

       <%--   <%:Html.TextBoxFor(dailyOutputFileDownloadSearch => dailyOutputFileDownloadSearch.MiscLocCode, new { @class = "autocComplete", style = "width:120px;", id = "MiscLocCode" })%>--%>
        </div>      
      </div>      
      <input class="primaryButton" type="submit" id="submitDailyOpSearch" value="Search" />
    </div>
  </div>
</div>
<br />
<div>
  <h2>
    Search Results</h2>
  <%Html.RenderPartial("FileDownloadGrid", ViewData["FileDownloadGridData"]); %>
</div>
<% } %>
