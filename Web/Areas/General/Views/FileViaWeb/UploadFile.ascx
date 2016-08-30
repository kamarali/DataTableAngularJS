<%@ Control Language="C#"  Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.IsInputFile>" %>
<div>
   <form id="ajaxUploadForm" action="<%= Url.Action("AjaxUpload", "FileViaWeb")%>" method="post" enctype="multipart/form-data">
    <%: Html.AntiForgeryToken() %>
  <div class="searchCriteria">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow" style="height: 80px;">
        <div class="buttonContainer">
          <h2>
            All files must be in compressed format with a .zip extension.</h2>
          <br />
          <div style="float: left; width: 580px;">

          <b>File Name:</b>
            

          <input type="file" name="file" id="uploadedFile" />

            <input class="primaryButton" id="ajaxUploadButton" type="submit"  value="Submit" />

            
          </div>
        </div>
      </div>
    </div>
  </div>
  </form>
   <% using (Html.BeginForm("FileManagerUpload", "FileViaWeb", FormMethod.Post, new { @id = "FileUploadManager" }))
       {%>
  <%: Html.AntiForgeryToken() %>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteria">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div style="float: left">
                <span class="required">* </span><span> File Submission From Date:</span>
            <br />
            
            <%:Html.TextBox("FileSubmissionFrom", Model.FileSubmissionFrom.ToString(FormatConstants.DateFormat), new { @class = "datePicker", @id = "FileSubmissionFrom" })%>
          </div>
          <div style="float: left">
                <span class="required">* </span><span> File Submission To Date:</span>
            <br />
            <%:Html.TextBox("FileSubmissionTo", Model.FileSubmissionTo.ToString(FormatConstants.DateFormat), new { @class = "datePicker", @id = "FileSubmissionTo" })%>
          </div>
          <div style="float: left">
              <span>Billing Period: </span>
            <br />
            <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.BillingPeriod)%>
            
          </div>
          <div style="float: left">
            <span>Billing Month: </span>
            <br />
          <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.BillingMonth)%>
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
          <div style="float: left">
        <span>
          <%= Iata.IS.Web.AppSettings.FileType%>
        </span>
        <br />
        <%= Html.FileFormatTypeDropdownListFor(searchCriteria => searchCriteria.FileFormatId,false,false)%> </div>
        <div style="float: left"><span>
          File Name:
        </span>
        <br />
        <%= Html.TextBoxFor(searchCriteria => searchCriteria.FileName,new {@Style="width:300px;"})%></div>
        </div>
        <br /><br />
        <input class="primaryButton" type="submit" name="btnSearch" value="Search" id="btnSearch" />
      </div>
    </div>
  </div>
   <% } %>
</div>
<br />
<div>

  <h2>Search Results</h2>
       
    <%--CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Call hooked to generate script for new action (image) column added. --%>
    <%:ScriptHelper.GenerateProgressBarGridScript(Url, ControlIdConstants.UploadFileStatusGrid)%>

    <%Html.RenderPartial("FileUploadGrid", ViewData["FileUploadGridData"]); %>

  
</div>
