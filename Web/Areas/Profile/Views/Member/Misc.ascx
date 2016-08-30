<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.MiscellaneousConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<script type="text/javascript">
  $(document).ready(function () {

   //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
   $("#IsDailyPayableIsWebRequired").bind("change", DailyPayableRequiredChangeEvent);
   DisableDailyPayableCheckBoxs();

  // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveMiscDetails").attr('disabled', 'disabled');   
    $("#btnSaveMiscDetails").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveMiscDetails").removeAttr('disabled'); 
     $("#btnSaveMiscDetails").addClass('ignoredirty');     
   }



   $('#RejectionOnValidationFailureId').focus();
       <%if (Model.MemberId > 0)
      {%>
        $(".futureEditLink").show();
        $('.currentFieldValue').attr("disabled", true);
    <%
      }%>
          
  });
   $('#BillingIswebMigrationDate').watermark(_periodFormat);
   $("#BillingIsXmlMigrationDate").watermark(_periodFormat);
  
  $('#BillingIsXmlMigrationStatusId').change(function () {
   if ($('#BillingIsXmlMigrationStatusId').val() == 3)
    {
      $('#miscBillingIsXmlCertifiedOn').datepicker('enable');
       $('#BillingIsXmlMigrationDate').removeAttr('disabled');
      }
    else
    {
      $('#miscBillingIsXmlCertifiedOn').datepicker('disable');
       $('#BillingIsXmlMigrationDate').attr('disabled', 'disabled');
       $('#miscBillingIsXmlCertifiedOn').val("");
      $('#BillingIsXmlMigrationDate').val("");
      $('#miscBillingIsXmlCertifiedOn').watermark(_dateWatermark);
      $('#BillingIsXmlMigrationDate').watermark(_periodFormat);
     }
  });
  
  $('#saveMisc').click(function () {
    if (!IsValidperiod($('#BillingIsXmlMigrationDate').val())) {
      return false;
    }
  });


   function IsValidperiod(datecheck) {
    var flag = false;
    if ((jQuery.trim(datecheck).length == 0 || (datecheck == _periodFormat))) {
      flag = true;
    }
    else {
      var arr_d1 = datecheck.split("-");

      day = arr_d1[2];
      month = arr_d1[1];
      year = arr_d1[0];
      var d = new Date();
      var curr_year = d.getFullYear();
      var curr_month = d.getMonth();
      var curr_day = d.getDate();
      var curr_period;
      var periodarray = new Array("01", "02", "03", "04");
      var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC");
      var isValidYear = /^[0-9]+$/.test(year);

      if ($.inArray(day, periodarray) <= -1 || $.inArray(month.toUpperCase(), monthArray) <= -1 || !isValidYear || year.length != 4) {
        alert("Please enter valid Date.");
        flag = false;
      }
      else {
        flag = true;
      }
    }
    return flag;
  }
</script>
<%
  using (Html.BeginForm("Misc", "Member", FormMethod.Post, new { id = "Misc" }))
  {%>
  <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div style="width: 22%;">
      <h2>
        Validations</h2>
        <%:Html.HiddenFor(miscModel => miscModel.UserCategory)%>
      <%: Html.ProfileFieldFor(miscModel => miscModel.RejectionOnValidationFailureId, "Rejection on Validation Failure", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "RejectionOnValidationFailureId" }, { "class", "mediumTextField" } }, new Dictionary<string, object> { { "style", "width: 200px;" }, { "class", "miscuatpControl" } })%>
    </div>
    <div style="width: 22%;">
        <h2>
           Online Correction Allowed</h2>
        <%:Html.ProfileFieldFor(miscModel => miscModel.IsOnlineCorrectionAllowed, "Online Correction Allowed", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
     </div>   
    <div style="width: 25%;">
      <h2>
        Supporting Documents</h2>
      <%: Html.ProfileFieldFor(miscModel => miscModel.MiscAllowedFileTypesForSupportingDocuments, "Additional File Types Accepted", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "AllowedFileTypes" }, { "id", "MiscAllowedFileTypesForSupportingDocuments" }, { "maxLength", "500" } }, new Dictionary<string, object> { { "class", "miscuatpControl mediumTextField" }, { "style", "width: 40%;" } }, null, new Dictionary<string, object> { { "style" , "width: 400px;"} })%>
    </div>
    <div style="width: 25%;">
      <h2>Future Billings Submissions</h2>  
            <%: Html.ProfileFieldFor(miscModel => miscModel.IsFutureBillingSubmissionsAllowed, "Future Billing Submissions Allowed", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%> 
    </div>
  </div>
  <div class="fieldContainer horizontalFlow">
    <div class="topLine">
      <h2>
        Output Files:</h2>
      <div class="miscuatpControl">
        <label>
          Billed Invoices:</label>
          IS-XML
                    <%: Html.ProfileFieldFor(miscModel => miscModel.BillingXmlOutput, "IS-XML", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingXmlOutput" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
                                {
                                  FieldId = "BillingXmlOutput",
                                  FieldName = "BillingXmlOutput",
                                  FieldType = 2,
                                  CurrentValue = Model.BillingXmlOutput.ToString(),
                                  FutureValue = Model.BillingXmlOutputFutureValue.ToString(),
                                  HasFuturePeriod = true,
                                  FuturePeriod = Model.BillingXmlOutputFuturePeriod
                                },null,true)%>
      </div>

      <%: Html.ProfileFieldFor(miscModel => miscModel.IsBillingDataSubmittedByThirdPartiesRequired, "Billing Invoices Submitted On Behalf of the Member", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IsBillingDataSubmittedByThirdPartiesRequired" }, { "class", "currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 30%" }, { "class", "miscuatpControl" } }, new FutureUpdate
                                {
                                  FieldId = "IsBillingDataSubmittedByThirdPartiesRequired",
                                  FieldName = "IsBillingDataSubmittedByThirdPartiesRequired",
                                  FieldType = 2,
                                  CurrentValue = Model.IsBillingDataSubmittedByThirdPartiesRequired.ToString(),
                                  FutureValue = Model.IsBillingDataSubmittedByThirdPartiesRequiredFutureValue.ToString(),
                                  HasFuturePeriod = true,
                                  FuturePeriod = Model.IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod
                                })%>


                                <div class="miscuatpControl" style="Width:25%">
        <label>
          Daily IS-XML files for Receivables IS-WEB Invoices:</label>
          
                    <%: Html.ProfileFieldFor(miscModel => miscModel.IsDailyXmlRequired, "IS-XML", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IsDailyXmlRequired" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
                                {
                                    FieldId = "IsDailyXmlRequired",
                                    FieldName = "IsDailyXmlRequired",
                                    FieldType = 2,
                                    CurrentValue = Model.IsDailyXmlRequired.ToString(),
                                    FutureValue = Model.IsDailyXmlRequiredValue.ToString(),
                                    HasFuturePeriod = true,
                                    FuturePeriod = Model.IsDailyXmlRequiredFuturePeriod
                                },null,true)%>
      </div>

    </div>
  </div>

  <!--CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 -->
  <div class="fieldContainer horizontalFlow">
    <div class="topLine">
      <h2>
        Payables Bilateral Invoices:</h2>
      
      <div class="miscuatpControl">
        <%: Html.ProfileFieldFor(miscModel => miscModel.IsDailyPayableIsWebRequired, "Daily Delivery in IS-WEB", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
        </div>

         <div class="miscuatpControl" style="Width:30%">

      <%: Html.ProfileFieldFor(miscModel => miscModel.IsDailyPayableOARRequired, "Daily Offline Archive Outputs", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
        </div>

          <div class="miscuatpControl" style="Width:25%">
         <%: Html.ProfileFieldFor(miscModel => miscModel.IsDailyPayableXmlRequired, "Daily IS-XML Files", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
        </div>
       <div class="clear" style="Width:60%">
        <label> 'Daily Offline Archive Outputs' and 'Daily IS-XML Files' can be opted for only if 'Daily Delivery in IS-WEB' is chosen </label>
       </div>
    </div>

  </div>
  <%: Html.HiddenFor(miscModel => miscModel.ContactList, new { @id = "MiscContactList" })%>
  <div class="fieldContainer horizontalFlow additionaloutput">
    <div class="topLine">
      <h2>
        Offline Archive Outputs</h2>
      <%: Html.TextBoxFor(miscModel => miscModel.MemberId, new { @class = "hidden", @id = "miscMemberId" })%>
      <% Html.RenderPartial("AdditionalOutputsControl", Model); %>
    </div>
  </div>
   <%--CMP#622: MISC Outputs Split as per Location IDs--%>
  <div class="fieldContainer horizontalFlow">
   <div class="topLine">
  <h2>
      Location Specific Output Files</h2>
      <div>
      <%:Html.ProfileFieldFor(miscModel => miscModel.RecCopyOfLocSpecificMISCOutputsAtMain, "Receive Copy of Location Specific Files at Location Main", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
      </div>
    </div>
   </div>
  <div class="fieldContainer verticalFlow">
    <div class="oneColumn topLine bottomLine">
      <h2>
        Certification and Migration Details</h2>
      <div>
        <table>
          <thead align="center" valign="middle">
            <tr>
              <td style="width: 50px;">
              </td>
              <td style="font-weight: bold; width: 150px;">
                Certification Status
              </td>
              <td style="font-weight: bold; width: 200px;">
                Certified On
              </td>
              <td style="font-weight: bold; width: 150px;">
                Migration Period
              </td>
            </tr>
          </thead>
          <tbody align="center" valign="middle">
            <tr>
              <td style="font-weight: bold; text-align: left;">
                IS-XML
              </td>
              <td>
                <%:Html.ProfileFieldFor(miscModel => miscModel.BillingIsXmlMigrationStatusId, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingIsXmlMigrationStatusId" }, { "class", "miscuatpControl" } }, null, null, null, true)%>
              </td>
              <td>
                 <%:Html.ProfileFieldFor(miscModel => miscModel.BillingIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "miscBillingIsXmlCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
              </td>
              <td>
                <%:Html.ProfileFieldFor(miscModel => miscModel.BillingIsXmlMigrationDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingIsXmlMigrationDate" }, { "class", "miscuatpControl" } }, null, null, null, true)%>
              </td>
            </tr>
             <tr>
              <td style="font-weight: bold; text-align: left;">
                IS-WEB
              </td>
              <td>
               </td>
              <td>
              </td>
              <td>
                <%:Html.ProfileFieldFor(miscModel => miscModel.BillingIswebMigrationDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingIswebMigrationDate"},{ "class", "miscuatpControl" } }, null, null, null, true)%>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  <div class="fieldContainer verticalFlow ">
    <div class="halfWidthColumn">
      <div>
        <h2>
          IS Contacts</h2>
        <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>','#divContactAssignmentSearchResult','MISC','',$('#selectedMemberId').val());">
          View/Edit</a>
      </div>
    </div>
    <div class="halfWidthColumn">
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer"> 
    <div>
      <input class="primaryButton" id="btnSaveMiscDetails" type="submit" value="Save Miscellaneous Details" />
      <div class="futureUpdatesLegend">Future Updates Pending</div>
    </div>
   
</div>
<%}  %>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
  <% Html.RenderPartial("SearchResultControl");%></div>

   <!--CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 -->
   <div class="hidden" id="DailyPayablePopUp">
    <div>Unchecking this field will also automatically uncheck dependent fields ‘Daily Offline Archive Outputs’ and ‘Daily IS-XML Files’. Click Proceed to continue with update of this field. Else click Cancel to abort update of this field.</div>
  </div>
