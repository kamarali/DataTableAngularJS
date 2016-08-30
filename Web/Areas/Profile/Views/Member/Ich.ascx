<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.IchConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.Calendar" %>
<script type="text/javascript">

  var $dialog, isSuspendedtoLive = false, isTerminatedtoLive = false;

  $(document).ready(function () {

  // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveIchDetails").attr('disabled', 'disabled');   
    $("#btnSaveIchDetails").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveIchDetails").removeAttr('disabled'); 
     $("#btnSaveIchDetails").addClass('ignoredirty');     
   }

  $('#IchMemberShipStatusId').focus();
    // get the disable agg member list and check while removing form list
    // that its only from future update not from disable aggr :(ugly code but works...
    var list = " ";
    $('#aggrAvailableMembersDisabled option').each(function() {
       list = list + " " + $(this).val();    
    });

    $('#AggDisableValues').val(list);
    // get length of future item list
    var cnt = parseInt($('#aggrAvailableMembers option').length) -  parseInt($('#aggrAvailableMembersDisabled option').length);
    if(cnt >= 0) {
      $('#AggregatorAddCount').val(cnt);
    }

    //sponsor
    var spnlist=" ";
    $('#availableMembersDisabled option').each(function() {
       spnlist = spnlist + " " + $(this).val();    
    });
    $('#SpnDisableValues').val(spnlist);
    
    // get length of future item list
    var cntSp = parseInt($('#availableMembers option').length) -  parseInt($('#availableMembersDisabled option').length);
    if(cntSp >= 0) {
      $('#SponsorrAddCount').val(cntSp);
    }
        
    $('#SponseredByText').flushCache();
    $('#AggregatedByText').flushCache();
    $('#divAggregatorMemberList').bind('dialogclose', function(event) { 
      $('#aggrAvailableMembers option').each(function(i, option){ $(option).remove(); });
      $('#hiddenAggrMemberIdAdd').val($("#AggregatorAddList").val());
      $('#hiddenAggrMemberIdRemove').val($("#AggregatorDeleteList").val());
      $('#AggreagatedMemberText').val("");
    }); 

    $('#divSponsordMemberList').bind('dialogclose', function(event) { 
      $('#availableMembers option').each(function(i, option){ $(option).remove();
      $('#hiddenMemberIdAdd').val($("#SponsororAddList").val());
      $('#hiddenMemberIdRemove').val($("#SponsororDeleteList").val());
      $('#SponsordMemberText').val("");
      });
    });

    setFutureUpdateFieldValue("#availableMembersDisabled", "#availableMembers","#SponsororFutureValue");
    setFutureUpdateFieldValue("#aggrAvailableMembersDisabled", "#aggrAvailableMembers","#AggregatorFutureValue");
 
    <%if (Model.MemberId > 0)
      {%>
        $(".futureEditLink").show();
        $('.currentFieldValue').attr("disabled", true);
    <%}%>
    
     <%if ((Model.MemberId > 0) && (Model.IchMemberShipStatusIdFutureValue != null))
      {%>
        $(".statusEditLink").show();
    <%}%>

    <%if (!string.IsNullOrEmpty(Model.SponsororFuturePeriod))
      {%>
        $("#SponsoredFutureDateInd").show(); 
      <%} else {%>
    $('#SponsoredFutureDateInd').hide();
    <%}%>

      <%if (!string.IsNullOrEmpty(Model.AggregatorFuturePeriod))
      {%>
        $("#AggregatorFutureDateInd").show();     
      <%} else {%>
    $('#AggregatorFutureDateInd').hide();
     <%}%>

      <%if (Model.IsAggregator)
      {%>
          $('.aggregatorviewedit').show();
     <%} else {%>
    $('.aggregatorviewedit').hide();
     <%}%>

     <%if (string.IsNullOrEmpty(Model.AggregatedByText))
      {%>
      $('#AggregatedTypeId').attr('disabled', 'disabled');
     
    <%}%>

     <%if ((Model.MemberId > 0) && (Model.ISSponsororMember))
    {%>
    $('#spnFuturePeriod').show();
    $('#spnFuturePeriodlbl').show();
      <%}%>
      <%if ((Model.MemberId > 0) && (Model.ISAggregatorMember == true))
      {%>
        $('#aggFuturePeriod').show();
        $('#aggFuturePeriodlbl').show();
      <%}%>
        
      <%if (Model.ReinstatementPeriod.HasValue)
      {%>
          $('#ReinstFutureDateInd').show();
      <%}%> 
    $('#IsAggregator').change(function () {
      if ($(this).prop('checked') == true) {
        $('#AggregatedByText').attr('readonly', 'true');
        $('.aggregatorviewedit').show();
      }
      else {
        $('#AggregatedByText').removeAttr("readonly");
        $('.aggregatorviewedit').hide();
      }
    });

    $('#AggregatedByText').blur(function () {
      var aggregatedByVal = $('#AggregatedByText').val();
      if($('#AggregatedByText').val()== "") {
        $('#IsAggregator').removeAttr('disabled');
      }
      else {
        $('#IsAggregator').attr({ "checked": false });
        $('#IsAggregator').attr('disabled', 'disabled');
      }
    });

    $('#hdnIchMembershipStatus').val(<% = ViewData["memberStatus"] %>); 
    $('#divSuspensionPeriodFrom').hide();
    $('#divDefaultSuspensionPeriodFrom').hide();
    $('#divIchTerminationDate').hide();
    $('#divReinstatement').hide();
    $('#divIchEntryDate').hide();  
    
    //Display current status related dates depending upon value present in status dates
    <%if (Model.EntryDate != null)
      {%>
       $('#divSuspensionPeriodFrom').hide();
       $('#divDefaultSuspensionPeriodFrom').hide();
       $('#divReinstatement').hide();
       $('#divIchEntryDate').show();
       $('#divIchTerminationDate').hide();
    <%}%>
    <%else if (Model.TerminationDate != null)
      {%>
       $('#divSuspensionPeriodFrom').hide();
       $('#divDefaultSuspensionPeriodFrom').hide();
       $('#divReinstatement').hide();
       $('#divIchEntryDate').hide();
       $('#divIchTerminationDate').show();

    <%}%>
    <%else if (Model.StatusChangedDate != null)
      {%>
       $('#divSuspensionPeriodFrom').show();
       $('#divDefaultSuspensionPeriodFrom').show();
       $('#divReinstatement').show();
       $('#divIchEntryDate').hide();
       $('#divIchTerminationDate').hide();
    <%}%>
     <%else if (Model.ReinstatementPeriod != null)
      {%>
//       $('#divSuspensionPeriodFrom').hide();
//       $('#divDefaultSuspensionPeriodFrom').hide();
//        $('#divReinstatement').show();

//       $('#divIchEntryDate').hide();
//       $('#divIchTerminationDate').hide();
    <%}%>
   
    registerAutocomplete('SponsordMemberText', 'MemberId', '<%=Url.Action("GetIchMemberList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('AggreagatedMemberText', 'IchMemberId', '<%=Url.Action("GetIchMemberList", "Data", new { area = "" })%>', 0, true, null);

    $('#IchMemberShipStatusId').change(function (event) {
	    //IchMemberShipStatusId => New Status
	    //hdnIchMembershipStatus => Old Status
	    //Live = 1; Suspended = 2; Terminated = 3 and NotAMember = 4
	
	    // Do all when New status is anything but "NotAMember"
	    if ($('#IchMemberShipStatusId').val() != 4) {
            /* SCP101407: FW: XML Validation Failure for 450-9B - SIS Production
               Description: Zone and Category are made immediate update fields for first edit now. Redirection after creating new member is stopped. 
               This code is to enable drop down for Zone and Category fields, if no value already present for them. */
            if($('#IchZoneId').val() == "") {
                $('#IchZoneId').attr("disabled", false);
                $('#FutureEditLinkFor_IchZoneId').hide();
            }
            if($('#IchCategoryId').val() == "") {
                $('#IchCategoryId').attr("disabled", false);
                $('#FutureEditLinkFor_IchCategoryId').hide();
            }

		    // Live from Suspended - Not allowed
		    if ($('#IchMemberShipStatusId').val() == 1 && $('#hdnIchMembershipStatus').val() == 2) {
		      alert("Current Status of member is suspended. Status Suspended cannot be changed to status Live");
		      ResetMemberStatusOnCancel();
		    }         

        ////////////////////CMP#689: Flexible CH Activation Options//Start////////////////////////////
        // Live from Terminated or NotAMember   
        if ($('#IchMemberShipStatusId').val() == 1) {
          if (($('#IchMemberShipStatusId').val() != $('#hdnIchMembershipStatus').val())) {
            $('.IchMembershipStatus').attr("disabled", true);
            
            //If current IchMembershipStatus is 'Not a member' , '#hdnIchMembershipStatus' value will be 0 from database
            //update the value to 4 to carry it further on UI as a 'Not A Member'
            if ($('#hdnIchMembershipStatus').val()=="0")  $('#hdnIchMembershipStatus').val(4);
          
            $('#IchMemberShipStatusId'+ 'FutureValue').val($('#IchMemberShipStatusId').val());
                
            popupFutureUpdateDialog('#IchMemberShipStatusId', 19, 1, '<%= Url.Content("~/Content/Images/calendar.gif") %>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');
              
          $("#EntryDate").watermark(_dateWatermark);
          $('#divIchEntryDate').show();
          $('#divReinstatement').hide();
          $('#divSuspensionPeriodFrom').hide();
          $('#divDefaultSuspensionPeriodFrom').hide();;
          $('#divIchTerminationDate').hide();
          $('#hdnReinstatement').val(0);
          isTerminatedtoLive = true;

          }
        }	
        ////////////////////CMP#689: Flexible CH Activation Options//End////////////////////////////
		
		    // Suspended From Live or Terminated
            if ($('#IchMemberShipStatusId').val() == 2) {
              if (($('#IchMemberShipStatusId').val() != $('#hdnIchMembershipStatus').val()) && (!SaveConfirmation())) {
			    ResetMemberStatusOnCancel();
              }
              else {
                $('#divSuspensionPeriodFrom').show();
                $('#divDefaultSuspensionPeriodFrom').show();
                $('#divReinstatement').show();
                $('#hdnReinstatement').val(1);
                $('#divIchEntryDate').hide();
                $('#divIchTerminationDate').hide();
              }
            }
		
		    // Terminated From Live or Suspended        
            if ($('#IchMemberShipStatusId').val() == 3) {
              if (($('#IchMemberShipStatusId').val() != $('#hdnIchMembershipStatus').val()) && (!SaveConfirmation())) {
                ResetMemberStatusOnCancel();         
              }
              else if (($('#IchMemberShipStatusId').val() != $('#hdnIchMembershipStatus').val()) && ($('#IchZoneId').val() > 0)) {
                $('.IchMembershipStatus').attr("disabled", true);
                /* SCP88742: ICH Member Profile XB-B41 - Not a Member status issue 
                   Date: 02-Mar-2013
                   Desc: Hidden variable added to model for keeping track of changed status.
                */
                $('#IchMemberShipStatusIdSelectedOnUi').val($('#hdnIchMembershipStatus').val());
                popupFutureUpdateDialog('#IchMemberShipStatusId', 16 , 1, '<%= Url.Content("~/Content/Images/calendar.gif") %>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');
              }
              else {
                $('#divIchTerminationDate').show();
                $('#divSuspensionPeriodFrom').hide();
                $('#divDefaultSuspensionPeriodFrom').hide();
                $('#divIchEntryDate').hide();
                $('#divReinstatement').hide();
                $('#hdnReinstatement').val(0);
                $("#TerminationDate").watermark(_dateWatermark);
              }
            }		
	    }	
        else {
        $('#divSuspensionPeriodFrom').hide();
        $('#divDefaultSuspensionPeriodFrom').hide();
        $('#divIchTerminationDate').hide();
        $('#divReinstatement').hide();
        $('#divIchEntryDate').hide();
      } 
    });

    });

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


  function showMembersDialog() {
    $('#MemberId').val("");
    $SponsordMembersdialog = 
      $('#divSponsordMemberList').dialog({
        title: 'Sponsored Members',
        modal: true,
        minWidth: 500,
        resizable: false,
        open: function () {
          var periodValue = $('#spnFuturePeriod').val();
          if (!periodValue || periodValue == _periodFormat) {
            //if ($('#availableMembersDisabled option').length > 0 || $('#SponsoredFutureDateInd').is(':visible')) {
            if ($('SponsoredFutureDateInd').is(':visible')) {
             $('#spnFuturePeriod').val($('#nextPeriod').val());
            }
          }
          
          $("#spnFuturePeriod").watermark(_periodFormat);
          $('#availableMembers option').each(function(i, option){ $(option).remove(); })
          
          jQuery('#SponsororsSelected option').clone().appendTo('#availableMembers'); 

          // recheck again list
          var cnt = parseInt($('#SponsororsSelected option').length) -  parseInt($('#availableMembersDisabled option').length);
         
          if (cnt >= 0) {
            $('#SponsorrAddCount').val(cnt);
          }
        },
        buttons: {
        
        Save: {
        className: 'primaryButton',
        text: 'Save',
        click: function () {
            if (UpdateSponsors('<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>')) $(this).dialog('close');      
          }
        },
        
        Close: {
        className: 'secondaryButton',
        text: 'Close',
        click: function () {
          $(this).dialog('close');
        }
        }
        
        }
       
         
      });
      
      return false;
  }

  function showAggregatorDialog() {
    $('#IchMemberId').val("");
    $AggregatorMembersdialog = 
      $("#divAggregatorMemberList").dialog({
        title: 'Aggregated Members',
        modal: true,
        minWidth: 500,
        resizable: false,
        open: function () {
          var periodValue = $('#aggFuturePeriod').val();
          
          if(!periodValue || periodValue == _periodFormat) { 
            //if ($('#aggrAvailableMembersDisabled option').length > 0 || $('#AggregatorFutureDateInd').is(':visible')) {
            if ($('AggregatorFutureDateInd').is(':visible')) {
              $('#aggFuturePeriod').val($('#nextPeriod').val());
            }
          }
          $("#aggFuturePeriod").watermark(_periodFormat);
          $('#aggrAvailableMembers option').each(function(i, option){ $(option).remove(); })
          jQuery('#AggregatorsSelected option').clone().appendTo('#aggrAvailableMembers'); 

          // recheck again list
          var cnt = parseInt($('#AggregatorsSelected option').length) -  parseInt($('#aggrAvailableMembersDisabled option').length);
          if(cnt >= 0) $('#AggregatorAddCount').val(cnt);
         
        }, 

            buttons: {
        
        Save: {
        className: 'primaryButton',
        text: 'Save',
        click: function () {
           if (UpdateAggregators('<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>')) $(this).dialog('close');
        }
        },
          Close: {
        className: 'secondaryButton',
        text: 'Close',
        click: function () {
          $(this).dialog('close');
        }
        }
        }
      });

    return false;
  }

  function SaveConfirmation()
  {
    if (confirm("Do you want to Continue?")) {
      return true;
    }

    return false;
  }

  function ResetMemberStatusOnCancel()
  {
    if ($('#hdnIchMembershipStatus').val() == 0) {
      $('#IchMemberShipStatusId')[$('#hdnIchMembershipStatus').val()][$('#hdnIchMembershipStatus').val()].selected = true;
    }
    else {
      $('#IchMemberShipStatusId').val($('#hdnIchMembershipStatus').val());
      $('#IchMemberShipStatusId').change();
    }
  }
 
</script>
<%
  using (Html.BeginForm("ich", "Member", FormMethod.Post, new { id = "Ich" }))
  {%>
  <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <h2>
      Member Details</h2>
    <div>
      <input type="hidden" value="0" id="SponsorrAddCount" />
      <input type="hidden" value="" id="AggDisableValues" />
      <input type="hidden" value="" id="SpnDisableValues" />
      <input type="hidden" value="-1" id="SponsAvailMembersDisabled" />
      <%= Html.TextBoxFor(ich => ich.MemberId, new { @class = "hidden", id = "currIchMemberId" })%>
      <%= Html.TextBoxFor(ich => ich.ISSponsororMember, new { @class = "hidden"})%>
      <%: Html.ProfileFieldFor(ich => ich.IchMemberShipStatusId, "ICH Membership Status", SessionUtil.UserCategory,
            new Dictionary<string, object> { { "id", "IchMemberShipStatusId" }, { "class", "IchMembershipStatus" } },
                          new Dictionary<string, object> { { "id", "divIchMembershipStatus" } }, new FutureUpdate
                      {
                        FieldId = "IchMemberShipStatusId",
                        FieldName = "IchMemberShipStatusId",
                        FieldType = Convert.ToInt32(ViewData["IchMemberShipStatusIdFieldType"]),
                        CurrentValue = Model.IchMemberShipStatusId.ToString(),
                        CurrentDisplayValue = Model.IchMemberShipStatusIdDisplayValue,
                        FutureValue = Model.IchMemberShipStatusIdFutureValue.ToString(),
                        FutureDisplayValue = Model.IchMemberShipStatusIdFutureDisplayValue,
                        HasFuturePeriod = true,
                        FuturePeriod = Model.IchMemberShipStatusIdFuturePeriod,
                        EditLinkClass = "statusEditLink"
                      })%>
      <img alt='View Membership Status History' class="imglegend" id="ichStatusHistory"
        onclick="showStatusHistoryDialog('#divIchStatusHistory','ICH','<%=Url.Action("GetIchMemberHistory", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>')"
        src='<%:Url.Content("~/Content/Images/MemberStatusDetails.png") %>' style="cursor: pointer;"
        title='View Membership Status History' />
        <div>
        
      <%: Html.ProfileFieldFor(ich => ich.StatusChangedDate, "ICH Suspension Period From",SessionUtil.UserCategory,new Dictionary<string, object>{{"id","ichStatusChangedDate"}},new Dictionary<string, object>{{"id","divSuspensionPeriodFrom"}},null,null)%>
      </div>
      <div style="width:25%">
      <%: Html.ProfileFieldFor(ich => ich.DefaultSuspensionDate, "ICH Suspension Defaulting Period From", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "ichDefaultSuspensionDate" } }, new Dictionary<string, object> { { "id", "divDefaultSuspensionPeriodFrom" },{"class","wrappingLabel"} })%>
      </div>
      <div>
      <%: Html.ProfileFieldFor(ich => ich.ReinstatementPeriod, "ICH Reinstatement Period", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "ichReinstatementPeriod" } }, new Dictionary<string, object> { { "id", "divReinstatement" }})%>
      </div>
      <div>
      <%: Html.ProfileFieldFor(ich => ich.EntryDate, "Entry Date", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "ichEntryDate" }, { "class", "datePicker" } }, new Dictionary<string, object> { { "id", "divIchEntryDate" } })%>
      </div>
      <div>
      <%: Html.ProfileFieldFor(ich => ich.TerminationDate, "Termination Date", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "ichTerminationDate" }, { "class", "datePicker" }, { "readOnly", "true" } }, new Dictionary<string, object> { { "id", "divIchTerminationDate" } })%>
      </div>
      
    </div>
    <div>
      <input id="hdnReinstatement" type="Hidden" />
      <input id="hdnIchMembershipStatus" type="Hidden" />
      <%:Html.ProfileFieldFor(ich => ich.IchZoneId, "Zone", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IchZoneId" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
              {
                FieldId = "IchZoneId",
                FieldName = "IchZoneId",
                FieldType = 11,
                CurrentValue = Model.IchZoneId.ToString(),
                CurrentDisplayValue = Model.IchZoneIdDisplayValue,
                FutureValue = Model.IchZoneIdFutureValue.ToString(),
                FutureDisplayValue = Model.IchZoneIdFutureDisplayValue,
                HasFuturePeriod = true,
                FuturePeriod = Model.IchZoneIdFuturePeriod,
                IsFieldMandatory = true
              })%>
      <%:Html.ProfileFieldFor(ich => ich.IchCategoryId, "Category", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IchCategoryId" }, { "class", "currentFieldValue mediumTextField" } }, new Dictionary<string, object> { { "style", "width: 35%;" } }, new FutureUpdate
              {
                FieldId = "IchCategoryId",
                FieldName = "IchCategoryId",
                FieldType = 12,
                CurrentValue = Model.IchCategoryId.ToString(),
                CurrentDisplayValue = Model.IchCategoryIdDisplayValue,
                FutureValue = Model.IchCategoryIdFutureValue.ToString(),
                FutureDisplayValue = Model.IchCategoryIdFutureDisplayValue,
                HasFuturePeriod = true,
                FuturePeriod = Model.IchCategoryIdFuturePeriod,
                IsFieldMandatory = true
              })%>
      <%: Html.ProfileFieldFor(ich => ich.IsEarlyCallDay, "Is Early Call Day Applicable",SessionUtil.UserCategory)%>
      <%--CMP #625: New Fields in ICH Member Profile Update XML--%>
      <%: Html.ProfileFieldFor(ich => ich.IchAccountId, "ICH iiNet Account ID", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "50" } })%>
    </div>
    <div class="fieldSeparator">
    </div>
  </div>
  <div class="fieldContainer horizontalFlow">
    <div style="width: 45%;">
      <h2>
        Aggregator</h2>
      <%:Html.ProfileFieldFor(ich => ich.IsAggregator, "Aggregator", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IsAggregator" }, { "class", "currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
{
  FieldId = "IsAggregator",
  FieldName = "IsAggregator",
  FieldType = 2,
  CurrentValue = Model.IsAggregator.ToString(),
  FutureValue = Model.IsAggregatorFutureValue.ToString(),
  HasFuturePeriod = true,
  FuturePeriod = Model.IsAggregatorFuturePeriod})%>
      <div style="width: 50%;">
        <label>
          List of Aggregated Members:</label>
        <div class="twoColumnWidth">
          <div>
            <%=Html.ListBox("aggrAvailableMembersDisabled", (MultiSelectList)ViewData["currentAggregatorMembers"], new { size = "8", disabled = true , style="width:170px"})%>
            <%=Html.ListBox("AggregatorsSelected", (MultiSelectList)ViewData["Aggregators"], new {@class="hidden", size = "8", disabled = true, style="width:275px" })%>
            <img id="AggregatorFutureDateInd" src="<%=Url.Content("~/Content/Images/Exclamation.gif") %>"
              alt="" onclick="displayFutureUpdateDetails('#Aggregator', 1, 19);" class="hidden" />
            <input type="hidden" id="AggregatorFutureValue" />
          </div>
        </div>
        <%if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
          {%>
        <a href="#" class="aggregatorviewedit ignoredirty hidden" onclick="return showAggregatorDialog()">
          Add/Edit</a>
        <%
          }%>
      </div>
    </div>
    <div style="border-left: 1px dotted #888; padding-left: 45px; width: 45%;">
      <h2>
        Sponsor</h2>
      <div style="width: 50%;">
        <div>
          <label>
            List of Sponsored Members:</label>
          <div class="twoColumnWidth">
            <div>
              <%=Html.ListBox("availableMembersDisabled", (MultiSelectList)ViewData["currentSponsordMembers"], new { size = "8", disabled = true, style="width:175px"})%>
              <!--This list will be used for temporarily storing the data which is added by user in pop-up while adding sponsored members -->
              <%=Html.ListBox("SponsororsSelected", (MultiSelectList)ViewData["Members"], new { @class = "hidden", size = "8", disabled = true, style="width:275px" })%>
              <img id="SponsoredFutureDateInd" src="<%=Url.Content("~/Content/Images/Exclamation.gif") %>"
                alt="" onclick="displayFutureUpdateDetails('#Sponsoror', 1, 19);" />
              <input type="hidden" id="SponsororFutureValue" />
            </div>
          </div>
          <%if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps))
            {%>
          <a class="ignoredirty" href="#" onclick="return showMembersDialog()">Add/Edit</a>
          <%
            }%>
        </div>
      </div>
    </div>
  </div>
  <div class="fieldContainer horizontalFlow" style="clear: both;">
    <div style="width: 45%;">
      <h2>
        Aggregated By</h2>
      <%= Html.TextBoxFor(ichModel => ichModel.AggregatedById, new { @id = "AggregatedById", @class = "hidden" })%>
      <%= Html.Hidden("AggregatedByTextFutureValueId", Model.AggregatedByTextFutureValueId)%>
      <%: Html.ProfileFieldFor(ichModel => ichModel.AggregatedByText, "Aggregated By", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "50" }, { "id", "AggregatedByText" }, { "class", "currentFieldValue mediumTextField" }, { "readOnly", "true" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
              {
                FieldId = "AggregatedByText",
                FieldName = "AggregatedByText",
                FieldType = 7,
                CurrentValue = Model.AggregatedByText,
                FutureValue = Model.AggregatedByTextFutureValue,
                HasFuturePeriod = true,
                FuturePeriod = Model.AggregatedByTextFuturePeriod,
                AggregatedById = Model.AggregatedById
              })%>
      <%: Html.ProfileFieldFor(ichModel => ichModel.AggregatedTypeId, "Aggregated Type", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "AggregatedTypeId" }, { "class", "currentFieldValue" }}, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
              {
                FieldId = "AggregatedTypeId",
                FieldName = "AggregatedTypeId",
                FieldType = 15,
                CurrentDisplayValue = Model.AggregatedTypeIdDisplayValue,
                FutureValue = Model.AggregatedTypeIdFutureValue.ToString(),
                FutureDisplayValue = Model.AggregatedTypeIdFutureDisplayValue,
                HasFuturePeriod = true,
                FuturePeriod = Model.AggregatedTypeIdFuturePeriod
              })%>
    </div>
    <div style="border-left: 1px dotted #888; padding-left: 45px; width: 45%;">
      <h2>
        Sponsored</h2>
      <div class="twoColumnWidth">
        <%= Html.TextBoxFor(ichModel => ichModel.SponsoredById, new { @id = "SponsoredById", @class = "hidden" })%>
        <%= Html.Hidden("SponseredByTextFutureValueId", Model.SponseredByTextFutureValueId)%>
        <%:Html.ProfileFieldFor(ichModel => ichModel.SponseredByText, "Sponsored By", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "50" }, { "id", "SponseredByText" }, { "class", "currentFieldValue mediumTextField" }, { "readOnly", "true" } }, null, new FutureUpdate
{
  FieldId = "SponseredByText",
  FieldName = "SponseredByText",
  FieldType = 6,
  CurrentValue = Model.SponseredByText,
  FutureValue = Model.SponseredByTextFutureValue,
  HasFuturePeriod = true,
  FuturePeriod = Model.SponseredByTextFuturePeriod,
  SponsoredById = Model.SponsoredById})%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
  </div>
  <div class="fieldContainer horizontalFlow" style="clear: both;">
    <h2>
      Migration (F12 Submission Permitted For The Following Billing Categories For The
      Member)</h2>
    <div>
      <%: Html.ProfileFieldFor(ichModel => ichModel.CanSubmitPaxInF12Files, "Passenger", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CanSubmitPaxInF12Files" }, { "class", "currentFieldValue" } },null, new FutureUpdate
              {
                FieldId = "CanSubmitPaxInF12Files",
                FieldName = "CanSubmitPaxInF12Files",
                FieldType = 2,
                CurrentValue = Model.CanSubmitPaxInF12Files.ToString(),
                FutureValue = Model.CanSubmitPaxInF12FilesFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.CanSubmitPaxInF12FilesFuturePeriod
              })%>
      <%: Html.ProfileFieldFor(ichModel => ichModel.CanSubmitCargoInF12Files, "Cargo", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CanSubmitCargoInF12Files" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
              {
                FieldId = "CanSubmitCargoInF12Files",
                FieldName = "CanSubmitCargoInF12Files",
                FieldType = 2,
                CurrentValue = Model.CanSubmitCargoInF12Files.ToString(),
                FutureValue = Model.CanSubmitCargoInF12FilesFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.CanSubmitCargoInF12FilesFuturePeriod
              })%>
      <%: Html.ProfileFieldFor(ichModel => ichModel.CanSubmitMiscInF12Files, "Miscellaneous", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CanSubmitMiscInF12Files" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
              {
                FieldId = "CanSubmitMiscInF12Files",
                FieldName = "CanSubmitMiscInF12Files",
                FieldType = 2,
                CurrentValue = Model.CanSubmitMiscInF12Files.ToString(),
                FutureValue = Model.CanSubmitMiscInF12FilesFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.CanSubmitMiscInF12FilesFuturePeriod
              })%>
      <%: Html.ProfileFieldFor(ichModel => ichModel.CanSubmitUatpinF12Files, "UATP", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CanSubmitUatpinF12Files" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
              {
                FieldId = "CanSubmitUatpinF12Files",
                FieldName = "CanSubmitUatpinF12Files",
                FieldType = 2,
                CurrentValue = Model.CanSubmitUatpinF12Files.ToString(),
                FutureValue = Model.CanSubmitUatpinF12FilesFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.CanSubmitUatpinF12FilesFuturePeriod
              })%>
    </div>
  </div>
  <div class="fieldContainer horizontalFlow">
    <div class="topLine bottomLine">
      <%: Html.ProfileFieldFor(ichModel => ichModel.IchWebReportOptionsId, "ICH Report Options",SessionUtil.UserCategory)%>
      <%: Html.ProfileFieldFor(ichModel => ichModel.IchOpsComments, "Comments", SessionUtil.UserCategory, new Dictionary<string, object> { { "rows", "5" }, { "cols", "100" }, { "maxlength", "500" } })%>
    </div>
  </div>
  <div class="fieldContainer verticalFlow ">
    <div class="halfWidthColumn">
      <div>
        <h2>
          IS Contacts</h2>
        <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%=Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>','#divContactAssignmentSearchResult','ICH','<%=Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile"}) %>',$('#selectedMemberId').val());">
          View/Edit</a>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
  <input id="hiddenSponseredMember" type="Hidden" />
</div>
<%if (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.Member)
  { %>
<div class="buttonContainer">
 
  <input class="primaryButton" id="btnSaveIchDetails" type="submit" value="Save ICH Details" />
  <div class="futureUpdatesLegend">
    Future Updates Pending</div>
 
</div>
<%} %>
<%--/* SCP88742: ICH Member Profile XB-B41 - Not a Member status issue 
            Date: 02-Mar-2013
            Desc: Hidden variable added to model for keeping track of changed status.
        */--%>
<%: Html.HiddenFor(ichModel => ichModel.IchMemberShipStatusIdSelectedOnUi)%>
<%= Html.HiddenFor(ichModel => ichModel.SponsororAddList)%>
<%= Html.HiddenFor(ichModel => ichModel.SponsororDeleteList)%>
<%= Html.HiddenFor(ichModel => ichModel.AggregatorAddList)%>
<%= Html.HiddenFor(ichModel => ichModel.AggregatorDeleteList)%>
<%= Html.HiddenFor(ichModel => ichModel.SponsororFuturePeriod)%>
<%= Html.HiddenFor(achModel => achModel.AggregatorFuturePeriod)%>
<%= Html.HiddenFor(ichModel => ichModel.AggregatorAddCount)%>
<%= Html.HiddenFor(achModel => achModel.AggrAvailMembersDisabled)%>
<%} %>
<div id="divSponsordMemberList" class="hidden ml-dialog">
  <% Html.RenderPartial("~/Areas/Profile/Views/Shared/MemberList.ascx");%></div>
<div id="divAggregatorMemberList" class="hidden ag-dialog">
  <% Html.RenderPartial("~/Areas/Profile/Views/Shared/AggregatorMemberList.ascx");%></div>
<div id="divIchStatusHistory" class="hidden ichStatus-dialog">
</div>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
  <% Html.RenderPartial("SearchResultControl");%></div>
