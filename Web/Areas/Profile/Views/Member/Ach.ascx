<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.AchConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.Calendar" %>

<script type="text/javascript">
  var $dialog, isAchSuspendedtoLive = false, isAchTerminatedtoLive = false;  

  $(document).ready(function () {  
  // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveAchDetails").attr('disabled', 'disabled');   
    $("#btnSaveAchDetails").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveAchDetails").removeAttr('disabled'); 
     $("#btnSaveAchDetails").addClass('ignoredirty');     
   }

  $('#AchMembershipStatusId').focus();
   <%{ if ((Model.MemberId > 0) &&(Model.HasPaxExceptionMembers))%>
  
    var periodValue = $('#PaxExceptionFuturePeriod').val();
    if(!periodValue || periodValue == _periodFormat)
       $('#PaxExceptionFuturePeriod').val($('#nextPeriod').val());
    $('#PaxExceptionFuturePeriod').show();
    $('#paxExcLabel').show();
    <%}%>
     <%{ if ((Model.MemberId > 0) &&(Model.HasCgoExceptionMembers))%>
   var periodValue = $('#CgoExceptionFuturePeriod').val();
    if(!periodValue || periodValue == _periodFormat)
       $('#CgoExceptionFuturePeriod').val($('#nextPeriod').val());
   $('#CgoExceptionFuturePeriod').show();
    $('#cgoExcLabel').show();
    <%}%>
     <%{ if ((Model.MemberId > 0) &&(Model.HasMiscExceptionMembers))%>
    var periodValue = $('#MiscExceptionFuturePeriod').vallist
    if(!periodValue || periodValue == _periodFormat)
       $('#MiscExceptionFuturePeriod').val($('#nextPeriod').val());
   $('#MiscExceptionFuturePeriod').show();
   $('#miscExcLabel').show();
    <%}%>
     <%{ if ((Model.MemberId > 0) &&(Model.HasUatpExceptionMembers))%>
    var periodValue = $('#UatpExceptionFuturePeriod').val();
    if(!periodValue || periodValue == _periodFormat)
       $('#UatpExceptionFuturePeriod').val($('#nextPeriod').val());
    $('#UatpExceptionFuturePeriod').show();
    $('#uatpExcLabel').show();
    <%}%>

  //If logged is member user then ACH Clearence and Inter clearance sections would be read-only
   <%if (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.Member)
     {%>
  $("#divAchClearance").attr('disabled', 'disabled');
  $("#divInterClearence").attr('disabled', 'disabled');
  $('.achClearance').attr('disabled', 'disabled');
  <%
     }%>
 setFutureUpdateFieldValue("#exceptionMemberspaxDisabled", "#exceptionMemberspax","#PaxExceptionFutureValue");
 setFutureUpdateFieldValue("#exceptionMemberscgoDisabled", "#exceptionMemberscgo","#CgoExceptionFutureValue");
 setFutureUpdateFieldValue("#exceptionMembersmiscDisabled", "#exceptionMembersmisc","#MiscExceptionFutureValue");
 setFutureUpdateFieldValue("#exceptionMembersuatpDisabled", "#exceptionMembersuatp","#UatpExceptionFutureValue");

 registerAutocomplete('exceptionMemberText', 'exceptionMemberId', '<%:Url.Action("GetDualMemberList", "Data", new { area = "" })%>', 0, true, null);
$('#SponseredByText').flushCache();
 $('#AggregatedByText').flushCache();

   <%if ((Model.MemberId > 0) && (Model.AchMembershipStatusIdFutureValue != null))
      {%>

        $(".statusEditLink").show();
     
    <%}%>

  $('#divExceptionMemberList').bind('dialogclose', function(event) { 
  $('#exceptionMembersmisc option').each(function(i, option){ $(option).remove(); });
  $('#exceptionMembersuatp option').each(function(i, option){ $(option).remove(); });
  $('#exceptionMemberscgo option').each(function(i, option){ $(option).remove(); });
  $('#exceptionMemberspax option').each(function(i, option){ $(option).remove(); });

  $('#hiddenpaxMemberIdAdd').val($("#PaxExceptionMemberAddList").val());
  $('#hiddenpaxMemberIdRemove').val($("#PaxExceptionMemberDeleteList").val());

  $('#hiddencgoMemberIdAdd').val($("#CgoExceptionMemberAddList").val());
  $('#hiddencgoMemberIdRemove').val($("#CgoExceptionMemberDeleteList").val());

  $('#hiddenmiscMemberIdAdd').val($("#MiscExceptionMemberAddList").val());
  $('#hiddenmiscMemberIdRemove').val($("#MiscExceptionMemberDeleteList").val());

  $('#hiddenuatpMemberIdAdd').val($("#UatpExceptionMemberAddList").val());
  $('#hiddenuatpMemberIdRemove').val($("#UatpExceptionMemberDeleteList").val());
 
 }); 

   $('#hdnAchMembershipStatus').val(<% = ViewData["achMemberStatus"] %>); 
 
    $('#divAchSuspensionPeriodFrom').hide();
    $('#divAchDefaultSuspensionPeriodFrom').hide();
    $('#divAchTerminationDate').hide();
    $('#divAchReinstatement').hide();
    $('#divAchEntryDate').hide();  

     <%if (Model.ReinstatementPeriod.HasValue)
      {%>
          $('#achReinstFutureDateInd').show();
      <%}%> 

    //Display current status related dates depending upon value present in status dates
    <%if (Model.EntryDate != null)
      {%>
       $('#divAchSuspensionPeriodFrom').hide();
       $('#divAchDefaultSuspensionPeriodFrom').hide();
       $('#divAchReinstatement').hide();
       $('#divAchEntryDate').show();
       $('#divAchTerminationDate').hide();
    <%}%>
    <%else if (Model.TerminationDate != null)
      {%>
      $('#divAchSuspensionPeriodFrom').hide();
       $('#divAchDefaultSuspensionPeriodFrom').hide();
       $('#divAchReinstatement').hide();
       $('#divAchEntryDate').hide();
       $('#divAchTerminationDate').show();
    <%}%>
    <%else if (Model.StatusChangedDate != null)
      {%>
       $('#divAchSuspensionPeriodFrom').show();
       $('#divAchDefaultSuspensionPeriodFrom').show();
       $('#divAchReinstatement').show();
       $('#divAchEntryDate').hide();
       $('#divAchTerminationDate').hide();
    <%}%>
     <%else if (Model.ReinstatementPeriod != null)
      {%>
//       $('#divAchSuspensionPeriodFrom').hide();
//       $('#divAchDefaultSuspensionPeriodFrom').hide();
//       $('#divAchReinstatement').show();
//       $('#divAchEntryDate').hide();
//       $('#divAchTerminationDate').hide();
    <%}%>

   <%
    if (Model != null)
    {%>
    
    <%
    if (!string.IsNullOrEmpty(Model.PaxExceptionFuturePeriod))
      {%>
   
          $("#exceptionMemberspaxFutureDateInd").show();
      <%
      }
      if (!string.IsNullOrEmpty(Model.CgoExceptionFuturePeriod))
      {%>
          $("#exceptionMemberscgoFutureDateInd").show();
      <%
      }
       if (!string.IsNullOrEmpty(Model.MiscExceptionFuturePeriod))
      {%>
          $("#exceptionMembersmiscFutureDateInd").show();
      <%
      }
      
      if (!string.IsNullOrEmpty(Model.UatpExceptionFuturePeriod))
      {%>
          $("#exceptionMembersuatpFutureDateInd").show();
      <%
      } 
    }%>

    if($('#AchClearanceInvoiceSubmissionPatternPaxId').val() == "1111" && $('#AchClearanceInvoiceSubmissionPatternCgoId').val() == "1111" && $('#AchClearanceInvoiceSubmissionPatternMiscId').val() == "1111" && $('#AchClearanceInvoiceSubmissionPatternUatpId').val() == "1111")
         $('#selectAllAch').attr('value', 'Unselect All');

     if($('#InterClearanceInvoiceSubmissionPatternPaxId').val() == "1111" && $('#InterClearanceInvoiceSubmissionPatternCgoId').val() == "1111" && $('#InterClearanceInvoiceSubmissionPatternMiscId').val() == "1111" && $('#InterClearanceInvoiceSubmissionPatternUatpId').val() == "1111")
          $('#selectAllIch').attr('value', 'Unselect All');

  });

  $AchMemberStatusdialog = $('<div></div>')
    .html($("#divAchMemberStatus"))
    .dialog({
      autoOpen: false,
      title: 'Member Status',
      modal: true,
      buttons: {
        Close: {
        className: 'secondaryButton',
        text: 'Close',
        click: function () {
          $(this).dialog('close');
        }
        }
      },
      resizable: false
    });

  function showAchStatusHistoryDialog() {
    $.ajax({
      type: "POST",
      url: '<%: Url.Action("GetIchMemberHistory", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>',
      data:{memberType:"ACH"},
      dataType: "json"  
    }); 
    $StatusHistoryDialog = 
      $("#divAchStatusHistory").dialog({
        title: 'Status Change History',
        modal: true,
        resizable: false
    });
  }

  function isValidDate(controlName, format) {
    var isValid = true;
    
    try {
      jQuery.datepicker.parseDate(format, jQuery('#' + controlName).val(), null);
    }
    catch(error){
      isValid = false;
    }

    return isValid;
  }

    $('#AchMembershipStatusId').change(function () { 
      if ($('#AchMembershipStatusId').val() != 4) {

        // Live from Suspended
        if ($('#AchMembershipStatusId').val() == 1 && $('#hdnAchMembershipStatus').val() == 2) {
          alert("Current Status of member is suspended.Status Suspended cannot be changed to status Live");
          ResetMemberStatusOnCancel();
        }

        
        ////////////////////CMP#689: Flexible CH Activation Options//Start////////////////////////////
        // Live from Terminated or NotAMember   
        if ($('#AchMembershipStatusId').val() == 1) {
          if (($('#AchMembershipStatusId').val() != $('#hdnAchMembershipStatus').val())) {
            $('.AchMembershipStatus').attr("disabled", true);
            
            //If current AchMembershipStatus is 'Not a member' , '#hdnAchMembershipStatus' value will be 0 from database
            //update the value to 4 to carry it further on UI as a 'Not A Member'
            if ($('#hdnAchMembershipStatus').val()=="0")  $('#hdnAchMembershipStatus').val(4);
          
            $('#AchMembershipStatusId'+ 'FutureValue').val($('#AchMembershipStatusId').val());
                
            popupFutureUpdateDialog('#AchMembershipStatusId', 20, 1, '<%= Url.Content("~/Content/Images/calendar.gif") %>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');
              
          $('#divAchEntryDate').show();
          $("#achEntryDate").watermark(_dateWatermark);
          $('#divAchReinstatement').hide();
          $('#divAchSuspensionPeriodFrom').hide();
          $('#divAchDefaultSuspensionPeriodFrom').hide();
          $('#divAchTerminationDate').hide();
           $('#hdnAchReinstatement').val(0);
           isAchTerminatedtoLive = true;

          }
        }	
        ////////////////////CMP#689: Flexible CH Activation Options//End////////////////////////////

        //  Suspended
        if ($('#AchMembershipStatusId').val() == 2) {
          if (($('#AchMembershipStatusId').val() != $('#hdnAchMembershipStatus').val()) && (!SaveAchConfirmation())) {
            ResetMemberStatusOnCancel();
          }
          else
          {
            $('#divAchSuspensionPeriodFrom').show();
            $('#divAchDefaultSuspensionPeriodFrom').show();
            $('#divAchEntryDate').hide();
            $('#divAchReinstatement').show();
            $('#hdnAchReinstatement').val(1);
            $('#divAchTerminationDate').hide();
          }
        }
        // Terminated
        if ($('#AchMembershipStatusId').val() == 3) { 
          if (($('#AchMembershipStatusId').val() != $('#hdnAchMembershipStatus').val()) && (!SaveAchConfirmation())) {
            ResetMemberStatusOnCancel();
          }
          else if (($('#AchMembershipStatusId').val() != $('#hdnAchMembershipStatus').val()) && ($('#AchCategoryId').val() > 0))
          {
            $('.AchMembershipStatus').attr("disabled", true);
            popupFutureUpdateDialog('#AchMembershipStatusId', 17 , 1, '<%: Url.Content("~/Content/Images/calendar.gif") %>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');
          }
          else {
            $('#divAchTerminationDate').show();
            $('#divAchSuspensionPeriodFrom').hide();
            $('#divAchDefaultSuspensionPeriodFrom').hide();
            $('#divAchEntryDate').hide();
            $('#divAchReinstatement').hide();
            $('#hdnAchReinstatement').val(0);    
            $("#achTerminationDate").watermark(_dateWatermark);  
          }
        }
      }
      else {
        $('#divAchSuspensionPeriodFrom').hide();
        $('#divAchDefaultSuspensionPeriodFrom').hide();
        $('#divAchTerminationDate').hide();
        $('#divAchReinstatement').hide();
        $('#divAchEntryDate').hide();  
      }
    });

    function SaveAchConfirmation() {
      if (confirm("Do you want to Continue?")) {
        return true;
      }
      
      return false;
    }

    function ResetMemberStatusOnCancel() {
     if ($('#hdnAchMembershipStatus').val() == 0) {
         $('#AchMembershipStatusId')[$('#hdnAchMembershipStatus').val()][$('#hdnAchMembershipStatus').val()].selected = true;
      }
      else {
        $('#AchMembershipStatusId').val($('#hdnAchMembershipStatus').val());
        $('#AchMembershipStatusId').change();
      }
    }
 
</script>

<%
  using (Html.BeginForm("Ach", "Member", FormMethod.Post, new { id = "ach" }))
  {%>
  <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <h2>
      Member Details</h2>
    <div>
    <%: Html.TextBoxFor(ach => ach.MemberId, new { @class = "hidden", id = "currAchMemberId" })%>

       <%: Html.ProfileFieldFor(ach => ach.AchMembershipStatusId, "ACH Membership Status", SessionUtil.UserCategory,
            new Dictionary<string, object> { { "id", "AchMembershipStatusId" }, { "class", "AchMembershipStatus" } },
                      new Dictionary<string, object> { { "id", "divAchMembershipStatus" } }, new FutureUpdate
                      {
                        FieldId = "AchMembershipStatusId",
                        FieldName = "AchMembershipStatusId",
                        FieldType = Convert.ToInt32(ViewData["AchMembershipStatusIdFieldType"]),
                        CurrentValue = Model.AchMembershipStatusId.ToString(),
                        CurrentDisplayValue = Model.AchMembershipStatusIdDisplayValue,
                        FutureValue = Model.AchMembershipStatusIdFutureValue.ToString(),
                        FutureDisplayValue = Model.AchMembershipStatusIdFutureDisplayValue,
                        HasFuturePeriod = true,
                        FuturePeriod = Model.AchMembershipStatusIdFuturePeriod,
                        EditLinkClass = "statusEditLink"
                      })%>
      <img alt='View Membership Status History' class="imglegend" id="achStatusHistory" onclick="showStatusHistoryDialog('#divAchStatusHistory','ACH','<%:Url.Action("GetIchMemberHistory", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>')" src='<%:Url.Content("~/Content/Images/MemberStatusDetails.png") %>' style="cursor: pointer;" title='View Membership Status History' />
 
      <%: Html.ProfileFieldFor(ach => ach.StatusChangedDate, "ACH Suspension set From",SessionUtil.UserCategory,new Dictionary<string, object>{{"id","achStatusChangedDate"}},new Dictionary<string, object>{{"id","divAchSuspensionPeriodFrom"}},null,null)%>
    
      <%: Html.ProfileFieldFor(ach => ach.DefaultSuspensionDate, "ACH Suspension Applicable From", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "achDefaultSuspensionDate" } }, new Dictionary<string, object> { { "id", "divAchDefaultSuspensionPeriodFrom" }})%>

      <%: Html.ProfileFieldFor(ach => ach.ReinstatementPeriod, "ACH Reinstatement Period", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "achReinstatementPeriod" } }, new Dictionary<string, object> { { "id", "divAchReinstatement" }})%>
        
      <%: Html.ProfileFieldFor(ach => ach.EntryDate, "Entry Date", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "achEntryDate" }, { "class", "datePicker" } }, new Dictionary<string, object> { { "id", "divAchEntryDate" } })%>

      <%: Html.ProfileFieldFor(ach => ach.TerminationDate, "Termination Date", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "achTerminationDate" }, { "class", "datePicker" }, { "readOnly", "true" } }, new Dictionary<string, object> { { "id", "divAchTerminationDate" } })%>
    </div>

    <input id="hdnAchReinstatement" type="Hidden" />
    <input id="hdnAchMembershipStatus" type="Hidden" />
    <%:Html.ProfileFieldFor(achModel=>achModel.AchCategoryId,"Category",SessionUtil.UserCategory) %>
    <div class="fieldContainer verticalFlow bottomLine topLine" style="padding-left: 0; padding-right: 0">
      <div class="secondColumnWidth">
        <h2>
          ACH clearance Invoice Submission Pattern</h2>
        <div id="divAchClearance">
          <table border="0" cellpadding="2" cellspacing="0">
            <tbody align="center" valign="middle">
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    PAX</label>
                  <%:Html.TextBoxFor(achModel => achModel.AchClearanceInvoiceSubmissionPatternPaxId, new { @class="hidden"})%>
                </td>
                <td>
                  <%: Html.CheckBox("AchPaxPeriod1", new { @class = "achClearance" })%>Period1 </td>
                <td>
                  <%: Html.CheckBox("AchPaxPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("AchPaxPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("AchPaxPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    CGO</label>
                  <%:Html.TextBoxFor(achModel => achModel.AchClearanceInvoiceSubmissionPatternCgoId, new { @class = "hidden" })%>
                </td>
                <td>
                  <%: Html.CheckBox("AchCgoPeriod1", new { @class = "achClearance" })%>Period1 </td>
                <td>
                  <%: Html.CheckBox("AchCgoPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("AchCgoPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("AchCgoPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    MISC</label>
                  <%:Html.TextBoxFor(achModel => achModel.AchClearanceInvoiceSubmissionPatternMiscId, new { @class="hidden"})%>
                </td>
                <td>
                  <%: Html.CheckBox("AchMiscPeriod1", new { @class = "achClearance" })%>Period1 </td>
                <td>
                  <%: Html.CheckBox("AchMiscPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("AchMiscPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("AchMiscPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    UATP</label>
                  <%:Html.TextBoxFor(achModel => achModel.AchClearanceInvoiceSubmissionPatternUatpId, new { @class = "hidden" })%>
                </td>
                <td>
                  <%: Html.CheckBox("AchUatpPeriod1", new { @class = "achClearance" ,@name="achClearance"})%>Period1 </td>
                <td>
                  <%: Html.CheckBox("AchUatpPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("AchUatpPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("AchUatpPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <%if(SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps || SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps )
{%>
              <tr>
                <td>
                  <input type="button" id="selectAllAch" value="Select All"  onclick="SelectAllAchPattern();"  class="secondaryButton"/>
                </td>
              </tr>
              <%
}%>
            </tbody>
          </table>
        </div>
      </div>
      <div class="halfWidthColumn">
        <h2>
          Inter clearance Invoice Submission Pattern</h2>
        <div id="divInterClearence">
          <table border="0" cellpadding="2" cellspacing="0">
            <tbody align="center" valign="middle">
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    PAX</label>
                  <%:Html.TextBoxFor(achModel => achModel.InterClearanceInvoiceSubmissionPatternPaxId, new { @class = "hidden" })%>
                </td>
                <td>
                  <%: Html.CheckBox("IchPaxPeriod1", new { @class = "achClearance" ,@name="IchClearance"})%>Period1 </td>
                <td>
                  <%: Html.CheckBox("IchPaxPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("IchPaxPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("IchPaxPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    CGO</label>
                  <%:Html.TextBoxFor(achModel => achModel.InterClearanceInvoiceSubmissionPatternCgoId, new { @class = "hidden" })%>
                </td>
                <td>
                  <%: Html.CheckBox("IchCgoPeriod1", new { @class = "achClearance" })%>Period1 </td>
                <td>
                  <%: Html.CheckBox("IchCgoPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("IchCgoPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("IchCgoPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    MISC</label>
                  <%:Html.TextBoxFor(achModel => achModel.InterClearanceInvoiceSubmissionPatternMiscId, new { @class = "hidden" })%>
                </td>
                <td>
                  <%: Html.CheckBox("IchMiscPeriod1", new { @class = "achClearance" })%>Period1 </td>
                <td>
                  <%: Html.CheckBox("IchMiscPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("IchMiscPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("IchMiscPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
              <tr>
                <td style="font-weight: bold;">
                  <label>
                    UATP</label>
                  <%:Html.TextBoxFor(achModel => achModel.InterClearanceInvoiceSubmissionPatternUatpId, new { @class = "hidden" })%>
                </td>
                <td>
                  <%: Html.CheckBox("IchUatpPeriod1", new { @class = "achClearance"})%>Period1 </td>
                <td>
                  <%: Html.CheckBox("IchUatpPeriod2", new { @class = "achClearance" })%>Period2 </td>
                <td>
                  <%: Html.CheckBox("IchUatpPeriod3", new { @class = "achClearance" })%>Period3 </td>
                <td>
                  <%: Html.CheckBox("IchUatpPeriod4", new { @class = "achClearance" })%>Period4 </td>
              </tr>
                <%if(SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps || SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps )
{%>
                <tr>
                <td>
                  <input type="button" id="selectAllIch" value="Select All" onclick="SelectAllIchPattern();"  class="secondaryButton"/>
                </td>
              </tr>
              <%
}%>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  
    <%: Html.ProfileFieldFor(achModel => achModel.AchOpsComments,"Comments",SessionUtil.UserCategory,new Dictionary<string, object>{{"rows","3"},{"cols","150"},{"maxLength","150"}},new Dictionary<string, object>{{"class","fieldContainer"}}) %>
    <div class="fieldContainer verticalFlow topLine" style="padding-left: 0; padding-right: 0">
      <h2>
        Exceptions (Settlement Via ICH For Dual Clearing House Members)</h2>
      <div style="width: 25%">
        <label>
          PAX</label>
          <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%:Html.ListBox("exceptionMemberspaxDisabled", (MultiSelectList)ViewData["currentPaxExceptionMembers"], new { size = "4", disabled = true, @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMemberspaxSelected", (MultiSelectList)ViewData["paxexceptionMemberList"], new { @class = "hidden", size = "8", disabled = true })%>
        <img id="exceptionMemberspaxFutureDateInd" src="<%:Url.Content("~/Content/Images/Exclamation.gif") %>" alt="" onclick="displayFutureUpdateDetails('#PaxException', 1, 19);"
          class="hidden" />
          <%if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
{%>
        <a class="ignoredirty" href="#" onclick="return showExceptionDialog('pax','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');">Add/Edit</a>
        <%
}%>
        <input type="hidden" id="PaxExceptionFutureValue" />
      </div>
      <div style="width: 25%">
        <label>
          CGO</label>
          <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%:Html.ListBox("exceptionMemberscgoDisabled", (MultiSelectList)ViewData["currentCgoExceptionMembers"], new { size = "4", disabled = true, @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMemberscgoSelected", (MultiSelectList)ViewData["cgoexceptionMemberList"], new { @class = "hidden", size = "8", disabled = true })%>
        <img id="exceptionMemberscgoFutureDateInd" src="<%:Url.Content("~/Content/Images/Exclamation.gif") %>" alt="" onclick="displayFutureUpdateDetails('#CgoException', 1, 19);"
          class="hidden" />
          <%if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
{%>
        <a class="ignoredirty" href="#" onclick="return showExceptionDialog('cgo','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');">Add/Edit</a>
        <%
}%>
        <input type="hidden" id="CgoExceptionFutureValue" />
      </div>
      <div style="width: 25%">
        <label>
          MISC</label>
        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%:Html.ListBox("exceptionMembersmiscDisabled", (MultiSelectList)ViewData["currentMiscExceptionMembers"], new { size = "4", disabled = true, @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMembersmiscSelected", (MultiSelectList)ViewData["miscexceptionMemberList"], new { @class = "hidden", size = "8", disabled = true })%>
        <img id="exceptionMembersmiscFutureDateInd" src="<%:Url.Content("~/Content/Images/Exclamation.gif") %>" alt="" onclick="displayFutureUpdateDetails('#MiscException', 1, 19);"
          class="hidden" />
          <%if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
{%>
        <a class="ignoredirty" href="#" onclick="return showExceptionDialog('misc','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');">Add/Edit</a>
        <%
}%>
        <input type="hidden" id="MiscExceptionFutureValue" />
      </div>
      <div style="width: 25%">
        <label>
          UATP</label>
        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%:Html.ListBox("exceptionMembersuatpDisabled", (MultiSelectList)ViewData["currentUatpExceptionMembers"], new { size = "4", disabled = true, @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMembersuatpSelected", (MultiSelectList)ViewData["uatpexceptionMemberList"], new { @class = "hidden", size = "8", disabled = true })%>
        <img id="exceptionMembersuatpFutureDateInd" src="<%:Url.Content("~/Content/Images/Exclamation.gif") %>" alt="" onclick="displayFutureUpdateDetails('#UatpException', 1, 19);"
          class="hidden" />
          <%if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
{%>
        <a class="ignoredirty" href="#" onclick="return showExceptionDialog('uatp','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');">Add/Edit</a>
        <%
}%>
        <input type="hidden" id="UatpExceptionFutureValue" />
      </div>
    </div>
    <div class="fieldContainer horizontalFlow topLine" style="padding-left: 0; padding-right: 0">
      <div>
        <h2>
          IS Contacts</h2>
        <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>','#divContactAssignmentSearchResult','ACH','<%:Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile"}) %>',$('#selectedMemberId').val());">
          View/Edit</a>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<%if (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.Member)
  { %>
<div class="buttonContainer"> 
  <input class="primaryButton" id="btnSaveAchDetails" type="submit" value="Save ACH Details" /> 
  <div class="futureUpdatesLegend">Future Updates Pending</div>   
</div>
<%} %>
<div>
  <%: Html.HiddenFor(achModel => achModel.ContactList, new { @id = "achContactList" })%>
</div>
<div>
  <%: Html.HiddenFor(achModel => achModel.PaxExceptionMemberAddList)%>
  <%: Html.HiddenFor(achModel => achModel.PaxExceptionMemberDeleteList)%>
  <%: Html.HiddenFor(achModel => achModel.CgoExceptionMemberAddList)%>
  <%: Html.HiddenFor(achModel => achModel.CgoExceptionMemberDeleteList)%>
  <%: Html.HiddenFor(achModel => achModel.MiscExceptionMemberAddList)%>
  <%: Html.HiddenFor(achModel => achModel.MiscExceptionMemberDeleteList)%>
  <%: Html.HiddenFor(achModel => achModel.UatpExceptionMemberAddList)%>
  <%: Html.HiddenFor(achModel => achModel.UatpExceptionMemberDeleteList)%>
  <%: Html.HiddenFor(achModel => achModel.PaxExceptionFuturePeriod, new { id = "hidPaxExceptionFuturePeriod" })%>
  <%: Html.HiddenFor(achModel => achModel.CgoExceptionFuturePeriod, new { id = "hidCgoExceptionFuturePeriod" })%>
  <%: Html.HiddenFor(achModel => achModel.MiscExceptionFuturePeriod, new {id = "hidMiscExceptionFuturePeriod" })%>
  <%: Html.HiddenFor(achModel => achModel.UatpExceptionFuturePeriod, new {id = "hidUatpExceptionFuturePeriod" })%>
  <%: Html.HiddenFor(achModel => achModel.HasPaxExceptionMembers)%>
  <%: Html.HiddenFor(achModel => achModel.HasCgoExceptionMembers)%>
  <%: Html.HiddenFor(achModel => achModel.HasMiscExceptionMembers)%>
  <%: Html.HiddenFor(achModel => achModel.HasUatpExceptionMembers)%>
</div>
<%} %>
<div id="divExceptionMemberList" class="hidden sn-dialog">
  <% Html.RenderPartial("~/Areas/Profile/Views/member/ACHExceptionsControl.ascx");%>
</div>
<div id="divAchStatusHistory" class="hidden achStatus-dialog">
</div>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
  <% Html.RenderPartial("SearchResultControl");%></div>
