<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ProcessingDashboardSearchEntity>" %>
<script type="text/javascript">
  function validateDateRange(startDateId, endDateId) {
    var startDateVal = $('#' + startDateId).datepicker("getDate");
    var endDateVal = $('#' + endDateId).datepicker("getDate");   
    if (startDateVal == null || endDateVal == null) {
      return false;
    }     
    return endDateVal >= startDateVal;
}


$(document).ready(function () {
    $('#rdQuickSearch').focus();
    
    registerAutocomplete('BilledMember', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('BillingMember', 'BillingMemberId', '<%:Url.Action("GetAggregatSponsordMemberList", "Data", new { area = "" })%>', 0, true, null);

    $('#detailSearch').hide();
    $('input:radio[name=searchOption]').click(function () {
        if ($(this).attr("id") == 'rdDetailSearch') {
            $('#detailSearch').show();
            $('#quickSearch').hide();
            //  $('#Search').click();
            // Clear grid data  
            if ($("#isInvoiceTabClicked").val() == "true")
            jQuery("#ISSearchResultListGrid").jqGrid("clearGridData", true);
            else
            jQuery("#FSSearchResultListGrid").jqGrid("clearGridData", true);



            //jQuery("#ISSearchResultListGrid").trigger('reloadGrid');

        } else {
            jQuery("#ISSearchResultListGrid").jqGrid("clearGridData", true);
            $('#detailSearch').hide();
            $('#quickSearch').show();
            searchInvoiceAndFileGridData('<%:Url.Action("GetInvoiceAndFileGridData", "ProcessingDashboard", new { area = "Reports"}) %>');
            //$('#Search').click();

        }
    });



    if ($("#DisableBillingTextBox").val() == "true")
        $("#BillingMember").attr("disabled", "disabled");

    //SCP109185: IS DASHBOARD mandatory fields
    $("#rdDetailSearch").click(function () {
      if ('<%: Convert.ToString(Session["IsSISOpsUser"]) %>' == "true") {
        $('#mandateYear').hide();
        $('#mandateMonth').hide();
      }
      else {
        $('#mandateYear').show();
        $('#mandateMonth').show();
      }
    });

});

         
    // Following function is executed when user clicks on Search button by adding search criteria
  function searchInvoiceAndFileGridData(postUrl) {
      // check here for show claim failed invoices

      var formID = '';
      if ($("#isInvoiceTabClicked").val() == "true") {
          formID = 'ProcessingDashboardInvoiceStatusForm';
      }
      else {
          formID = 'ProcessingDashboardFileStatusForm';
      }

      $("#" + formID).validate({
          rules: {
              BillingYear: {
                  required: function () {
                    if ($('#rdDetailSearch').is(':checked')) {
                      //SCP109185: IS DASHBOARD mandatory fields
                      if ('<%: Convert.ToString(Session["IsSISOpsUser"]) %>' == "true") {
                        return false;
                          }
                      return true;
                      }
                      else
                          return false;
                  }
              },
              BillingMonth: {
                  required: function () {
                    if ($('#rdDetailSearch').is(':checked')) {
                      //SCP109185: IS DASHBOARD mandatory fields
                      if ('<%: Convert.ToString(Session["IsSISOpsUser"]) %>' == "true") {
                        return false;
                      }
                      return true;
                    }
                      else
                          return false;
                  }
              }
          },
          messages: {
              BillingYear: "Billing Year required",
              BillingMonth: "Billing Month required"
          },
          invalidHandler: function () {
              $('#errorContainer').show();
              $('#clientErrorMessageContainer').hide();
              $('#clientSuccessMessageContainer').hide();
              $.watermark.showAll();
          }
      }).form();

      if (!$("#" + formID).valid()) {
          return false;
      }

      $('#errorContainer').hide();

      if ($("#isInvoiceTabClicked").val() != "true") {
          var sdate = $("#FileGeneratedDateFrom").val();
          var edate = $("#FileGeneratedDateTo").val();
          if (sdate != "" && edate != "") {
              if (!validateDateRange('FileGeneratedDateFrom', 'FileGeneratedDateTo')) {
                  alert("From date must be less than or equal to the To Date");
                  return false;
              }
          }
      }

    var ShowClaimFailed;
    if ($("#IsShowClaimFailed").prop('checked') == true) {
        ShowClaimFailed = true;
    }
    else {
        ShowClaimFailed = false;
    }

    // Create searchCriteria in JSON format which contains values selected for Search criteria
    var searchCriteria = {
        BillingYear: $("#BillingYear option:selected").val(),
        BillingMonth: $("#BillingMonth option:selected").val(),
        BillingPeriod: $("#BillingPeriod option:selected").val(),
        BillingMemberId: $("#BillingMemberId").val(),
        BilledMemberId: $("#BilledMemberId").val(),
        SettlementMethod: $("#SettlementMethod option:selected").val(),
        BillingCategory: $("#BillingCategory option:selected").val(),
        InvoiceStatus: $("#InvoiceStatus option:selected").val(),
        InvoiceNo: $("#InvoiceNo").val(),
        FileStatus: $("#FileStatus option:selected").val(),
        FileFormat: $("#FileFormat option:selected").val(),
        FileName: $("#FileName").val(),
        FileGeneratedDateFrom: $("#FileGeneratedDateFrom").val(),
        FileGeneratedDateTo: $("#FileGeneratedDateTo").val(),
        UniqueInvoiceNo: $("#UniqueInvoiceNo").val(),
        IsShowClaimFailed: ShowClaimFailed,
        //CMP559 : Add Submission Method Column to Processing Dashboard
        SubmissionMethodId: $("#SubmissionMethodId option:selected").val(),
        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        DailyDeliverystatusId: $("#DailyDeliverystatusId option:selected").val()
    };

    // Convert searchCriteria object to JSON.
    var searchCriteriaObject = $.toJSON(searchCriteria);

    // Get which radio button is clicked for billing period. i.e. Current or Previous 
    var billingPeriod = $("input[name='rbClearancePeriod']:checked").val();

    // Get which SearchType radio button is clicked. i.e. QuickSearch or DetailedSearch
    var searchType = $("input[name='searchOption']:checked").val();

    // set value to generate csv for invoice and file status tab
    $("#searchCriteriaCsv").val(searchCriteriaObject);
    $("#billingPeriodCsv").val(billingPeriod);
    $("#searchTypeCsv").val(searchType);
    $("#fileSearchCriteriaCsv").val(searchCriteriaObject);
    $("#fileBillingPeriodCsv").val(billingPeriod);
    $("#fileSearchTypeCsv").val(searchType);

    // Create URL to call "GetInvoiceAndFileGridData" action passing it filter criteria
    var url = postUrl + "?" + $.param({ billingPeriod: billingPeriod, isInvoiceTabClicked: $("#isInvoiceTabClicked").val(), searchType: searchType, searchCriteria: searchCriteriaObject });
    // Get tab clicked by user i.e. Invoice or File, depending on which we will reload the grid
    if ($("#isInvoiceTabClicked").val() == "true") 
        $("#ISSearchResultListGrid").jqGrid('setGridParam', { url: url, page: 1 }).trigger("reloadGrid");    
    else 
        $("#FSSearchResultListGrid").jqGrid('setGridParam', { url: url, page: 1 }).trigger("reloadGrid");    
 }    // end searchInvoiceAndFileGridData()

</script>
<script src="<%:Url.Content("~/Scripts/Site.js")%>" type="text/javascript"></script>

<div class="searchCriteria">
    <div>
        <input type="radio" id="rdQuickSearch" name="searchOption" value="QuickSearch" checked="checked"  />Quick
        Search
        <input type="radio" id="rdDetailSearch" name="searchOption" value="DetailedSearch"  />Detail
        Search
    </div>
    <div style="height: 10px">
    </div>
    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Increasing field size from 85% to 120% to keep layout intact -->
    <div class="solidBox" style="width: 120%" id="detailSearch">
        <div class="fieldContainer horizontalFlow">
            <div>
                                     <div>
                    <label>
                        <span class="required" id="mandateYear">* </span>Clearance Year</label>
                    <% if (ViewData["Status"] == "InvoiceStatus")
                       {%>
                    <%: Html.InvoiceYearDropdownListFor(searchCriteria => searchCriteria.BillingYear)%>
                    <% }
                       else
                       { %>
                    <%: Html.IsFileLogYearDropdownListFor(searchCriteria => searchCriteria.BillingYear)%>
                    <%}%>
                </div>
                                     <div>
                    <label>
                        <span class="required" id="mandateMonth">* </span>Clearance Month</label>
                        <%--SCP109185: IS DASHBOARD mandatory fields--%>
                        <% if (Session["IsSISOpsUser"] == "true")
                           {%>
                            <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.BillingMonth)%>
                        <% }else{ %>
                            <%: Html.ClearanceMonthDropdownListFor(searchCriteria => searchCriteria.BillingMonth)%>
                         <%}%>
                </div>
                                     <div>
                    <label>
                        Period</label>
                    <%: Html.StaticBillingPeriodDropdownList("BillingPeriod", Model.BillingPeriod.ToString(), TransactionMode.ProcessingDashboard)%>
                </div>
                                     <div>
                    <label>
                        Billing Member</label>
                    <%: Html.HiddenFor(searchCriteria => searchCriteria.DisableBillingTextBox)%>
                    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                    Desc: Increasing field size by specifying in-line width
                    Ref: 3.1 Table 1 Row 1, 3.2 Table 7 Row 1, 3.3 Table 11 Row 1, 3.5 Table 19 Row 15,
                    3.1 Table 1 Row 3, 3.2 Table 7 Row 3, 3.3 Table 11 Row 3, 3.5 Table 19 Row 17 -->
                    <%: Html.TextBoxFor(searchCriteria => searchCriteria.BillingMember, new { @class = "textboxWidth" })%>
                    <%: Html.HiddenFor(searchCriteria => searchCriteria.BillingMemberId)%>
                </div>
                <%  if (ViewData["Status"] == "InvoiceStatus")
                    {%>
                                     <div>
                    <!-- Added the Billing Category template to this control. Needs to be changed -->
                    <label>
                        Billed Member</label>
                    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                    Desc: Increasing field size by specifying in-line width
                    Ref: 3.1 Table 1 Row 2, 3.2 Table 7 Row 2, 3.3 Table 11 Row 2, 3.5 Table 19 Row 16 -->
                    <%: Html.TextBoxFor(searchCriteria => searchCriteria.BilledMember, new { @class = "textboxWidth" })%>
                    <%: Html.HiddenFor(searchCriteria => searchCriteria.BilledMemberId)%>
                </div>
                                     <div>
                    <%
                        string UserCategory = string.Empty;
                        if (Convert.ToInt32(ViewData["UserCategory"]) == (int)Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps)
                            UserCategory = "ICH";
                        else if (Convert.ToInt32(ViewData["UserCategory"]) == (int)Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps)
                            UserCategory = "ACH";
                    %>
                    <label>
                        Clearance Type</label>
                    <%: Html.ClearanceTypesDropdownListFor(searchCriteria => searchCriteria.SettlementMethod, UserCategory)%>
                </div>
                <% }%>
                                     <div>
                    <label>
                        Billing Category</label>
                    <%: Html.BillingCategoryDropdownListFor(searchCriteria => searchCriteria.BillingCategory,"SettlementMethod")%>
                </div>
                <%  if (ViewData["Status"] == "InvoiceStatus")
                    {%>
                                     <div>
                    <label>
                        Invoice No.</label>
                    <%: Html.TextBoxFor(searchCriteria => searchCriteria.InvoiceNo)%>
                </div>
                                     
                 <% }%>
                <%  if (ViewData["Status"] == "InvoiceStatus")
                    {%>
                                     <div>
                    <label>
                        Invoice Status</label>
                    <%: Html.InvoiceStatusDropdownListFor(searchCriteria => searchCriteria.InvoiceStatus,true)%>
                </div>
                <%if (ViewData["UniqueInvoiceNoShowClaimFailedCasesOnly"] == "SIS_ICH_OpsUserOnly")
                  {%>	
                  <div>
                    <label>Unique Invoice No.</label>
                    <%:Html.TextBoxFor(searchCriteria => searchCriteria.UniqueInvoiceNo)%>
                </div>
                <%--CMP559 : Add Submission Method Column to Processing Dashboard--%>
                <div>
                    <label>Submission Method</label>
                    <%: Html.InvoiceSubmissionMethodDropdownList("SubmissionMethodId", Model.SubmissionMethodId.ToString())%>
                </div>  
                <%--CMP529 : Daily Output Generation for MISC Bilateral Invoices--%>
                <% if (Convert.ToInt32(ViewData["UserCategory"]) != (int)Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps && Convert.ToInt32(ViewData["UserCategory"]) != (int)Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps)
                   {%>
                <div>
                    <label>Daily Delivery Status</label>
                    <%:Html.DailyDeliveryStatusDropdownList("DailyDeliverystatusId", Model.DailyDeliverystatusId.ToString())%>
                </div>
                <%
                   }%>              
                <div>
                    <label>Show Claim Failed cases only</label>
                    <%:Html.CheckBoxFor(searchCriteria => searchCriteria.IsShowClaimFailed)%>
                </div>
                <%
                  }

                  else
                    {%>
                   
                   <%--CMP559 : Add Submission Method Column to Processing Dashboard--%>
                    <div>
                        <label>Submission Method</label>
                        <%:Html.InvoiceSubmissionMethodDropdownList("SubmissionMethodId", Model.SubmissionMethodId.ToString())%>
                    </div>	 
                    <%--CMP529 : Daily Output Generation for MISC Bilateral Invoices--%>
                     <%
                      if (Convert.ToInt32(ViewData["UserCategory"]) != (int) Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps && Convert.ToInt32(ViewData["UserCategory"]) != (int) Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps)
                      {%>
                                        <div>
                                            <label>Daily Delivery Status</label>
                                            <%:Html.DailyDeliveryStatusDropdownList("DailyDeliverystatusId",
                                                                               Model.DailyDeliverystatusId.ToString())%>
                                        </div>  
                                 
                                     <%
                      }
                    }%>                  
                <% }
                    else
                    {%>
                                     <div>
                    <label>
                        File Format</label>
                    <%: Html.FileFormatTypeDropdownListFor(searchCriteria => searchCriteria.FileFormat, false,true)%>
                </div>
                                     <div>
                    <label>
                        File Status</label>
                    <%: Html.FileStatusDropdownListFor(searchCriteria => searchCriteria.FileStatus)%>
                </div>                
                                     <div>
                    <label>
                        File Name</label>
                    <%: Html.TextBoxFor(searchCriteria => searchCriteria.FileName)%>
                </div>
                                     <div>
                    <label>
                        From Date</label>
                    <%:Html.TextBox("FileGeneratedDateFrom", Model.FileGeneratedDateFrom != null ? Model.FileGeneratedDateFrom.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker", @id = "FileGeneratedDateFrom" })%>          
                 </div>
                                     <div>
                    <label>
                        To Date</label>
                        <%:Html.TextBox("FileGeneratedDateTo", Model.FileGeneratedDateTo != null ? Model.FileGeneratedDateTo.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker", @id = "FileGeneratedDateTo" })%>          
                </div>
                <%}%>
            </div>
        </div>
        <div class="clear" />
    </div>
    <div class="solidBox" style="width: 25%" id="quickSearch">
        <div class="fieldContainer horizontalFlow">
            <%: Html.Label("")%>Billing Period
           <%-- <input type="radio" id="rbClearancePeriod" name="rbClearancePeriod" value="Current" checked="checked"  />Current
            <input type="radio" id="rbClearancesPeriod" name="rbClearancePeriod" value="Previous"  />Previous--%>
           <%: Html.RadioButton("rbClearancePeriod", "Current", true)%>Current
            <%: Html.RadioButton("rbClearancePeriod", "Previous", false)%>Previous
        </div>
    </div>
    
       <div class="buttonContainer" style="float: left">
        <input class="primaryButton" id="Search" type="button" value="Search" onclick="searchInvoiceAndFileGridData('<%:Url.Action("GetInvoiceAndFileGridData", "ProcessingDashboard", new { area = "Reports"}) %>');" />
    </div>
    <div class="clear" />
</div>

