<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Member>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<script type="text/javascript">
  $(document).ready(function () {

  // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#SaveSisOpss").attr('disabled', 'disabled');   
    $("#SaveSisOpss").removeClass('ignoredirty');
   }
   else
   {
     $("#SaveSisOpss").removeAttr('disabled'); 
     $("#SaveSisOpss").addClass('ignoredirty');     
   }

  $("#IsMergedFutureDateInd").hide();
   <%if (Model.IsMergedFutureValue !=null)
      {%>        
         <%if (Model.IsMerged != Model.IsMergedFutureValue )
      {%>
           <%if (!string.IsNullOrEmpty(Model.IsMergedFuturePeriod))
          {%>
            $("#IsMergedFutureDateInd").show();
          <%}%>
     <%}%>  
     <% else if ((Model.IsMerged == true && Model.IsMergedFutureValue==true ))
      {%>
        <%if (!string.IsNullOrEmpty(Model.IsMergedFuturePeriod))
          {%>
            $("#IsMergedFutureDateInd").show();
          <%}%>
       <%}%>
   <%}%>


  
 
   $('#AllowContactDetailsDownload').focus();
 <%if (Model.Id > 0)
      {%>
        $(".futureEditLink").show();
        $('.currentFieldValue').attr("disabled", true);
    <%
      }%>
  });
    
        if ($('#LegalArchivingRequired').prop('checked') == true) {
          $('#CdcCompartmentIDforInv').removeAttr('disabled');    
        }
        else{
           $('#CdcCompartmentIDforInv').attr('disabled', 'disabled');
        };

             

   $('#LegalArchivingRequired').change(function () { 
      if ($(this).prop('checked') == true) {
        $('#CdcCompartmentIDforInv').removeAttr('disabled');
      }
      else {
        $('#CdcCompartmentIDforInv').attr('disabled', 'disabled');
            $("#CdcCompartmentIDforInv").val("");            
      }

    });
   
//   $('#SaveSisOpss').click(function () {
//   alert("test");
//    if (!IsValidperiod($('#ActualMergerDate').val())) {
//      return false;
//    }
//  });

  

</script>
<%
  using (Html.BeginForm("MemberControl", "Member", FormMethod.Post, new { id = "memberControl" }))
  {
%>
<%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
   <h2>
      Merger Information </h2>

     <div>
      <div class="tallFieldContainer">
        <%:Html.ProfileFieldFor(memberControl => memberControl.IsMerged, "Merged Entity", Model.UserCategory, new Dictionary<string, object> { { "id", "IsMerged" }, { "class", "currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
                                        {
                                            FieldId = "IsMerged",
                                            FieldName = "IsMerged",
                                          FieldType = 2,
                                            CurrentValue = Model.IsMerged.ToString(),
                                            FutureValue = Model.IsMergedFutureValue.ToString(),
                                          HasFuturePeriod = true,
                                            FuturePeriod = Model.IsMergedFuturePeriod
                                        })%>
      </div>
      <%if (Model.IsMerged == true)
       {%> 
       <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Parent Member (Current Value):</label>  
          <%: Model.ParentMemberIdDisplayValue%>
       </div>
       <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Merger Effective Period (Current Value):</label>  
         <%: Model.ActualMergerDate%>       
       </div>
       <%}%>
     </div>
      <%= Html.TextBoxFor(memberControl => memberControl.ActualMergerDate, new { @id = "ActualMergerDate", @class = "hidden" })%>
      <%= Html.TextBoxFor(memberControl => memberControl.ActualMergerDateFutureValue, new { @id = "ActualMergerDateFutureValue", @class = "hidden" })%>
    

     <%= Html.TextBoxFor(memberControl => memberControl.ParentMemberId, new { @id = "ParentMemberId", @class = "hidden" })%>
     <%= Html.TextBoxFor(memberControl => memberControl.ParentMemberIdDisplayValue, new { @id = "ParentMemberIdDisplayValue", @class = "hidden" })%>
     <%= Html.TextBoxFor(memberControl => memberControl.ParentMemberIdFutureValue, new { @id = "ParentMemberIdFutureValue", @class = "hidden" })%>
     <%= Html.TextBoxFor(memberControl => memberControl.ParentMemberIdFutureDisplayValue, new { @id = "ParentMemberIdFutureDisplayValue", @class = "hidden" })%>

     <%= Html.TextBoxFor(memberControl => memberControl.Id, new { @id = "Id", @class = "hidden" })%>

   <%--  <%= Html.TextBoxFor(memberControl => memberControl.ParentMemberIdFuturePeriod, new { @id = "ParentMemberIdFuturePeriod", @class = "hidden" })%>
--%>    
      

     <div class="fieldSeparator"></div>

    <h2>
      Subscriptions</h2>
    <div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Allow Download of Contact Details in CSV Format:</label>
          <%: Html.ProfileFieldFor(memberControl => memberControl.AllowContactDetailsDownload, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Participate In Value Determination:</label>
          <%: Html.ProfileFieldFor(memberControl => memberControl.IsParticipateInValueDetermination, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Participate In Billing Value Confirmation:</label>
          <%: Html.ProfileFieldFor(memberControl => memberControl.IsParticipateInValueConfirmation, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Digital Signature Application:</label>        
              <%: Html.ProfileFieldFor(memberControl => memberControl.DigitalSignApplication, "", Model.UserCategory, new Dictionary<string, object> { { "id", "DigitalSignApplication" }, { "class", "currentFieldValue"} },null,new FutureUpdate
              {
                FieldId = "DigitalSignApplication",
                FieldName = "DigitalSignApplication",
                FieldType = 2,
                CurrentValue = Model.DigitalSignApplication.ToString(),
                FutureValue = Model.DigitalSignApplicationFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.DigitalSignApplicationFuturePeriod
              }, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Digital Signature Verification:</label>       
              <%: Html.ProfileFieldFor(memberControl => memberControl.DigitalSignVerification, "", Model.UserCategory, new Dictionary<string, object> { { "id", "DigitalSignVerification" }, { "class", "currentFieldValue" } },null,new FutureUpdate
              {
                FieldId = "DigitalSignVerification",
                FieldName = "DigitalSignVerification",
                FieldType = 2,
                CurrentValue = Model.DigitalSignVerification.ToString(),
                FutureValue = Model.DigitalSignVerificationFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.DigitalSignVerificationFuturePeriod
              }, skipContainer: true)%>
      </div>
     
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Passenger Old IDEC Member:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.PaxOldIdecMember, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Cargo Old IDEC Member:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.CgoOldIdecMember, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <%--<div class="tallFieldContainer">
        <label class="wrappingLabel">
          UATP Invoice Handled by ATCAN:</label>       
              <%: Html.ProfileFieldFor(memberControl => memberControl.UatpInvoiceHandledbyAtcan, "", Model.UserCategory, new Dictionary<string, object> { { "id", "UatpInvoiceHandledbyAtcan" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
              {
                FieldId = "UatpInvoiceHandledbyAtcan",
                FieldName = "UatpInvoiceHandledbyAtcan",
                FieldType = 2,
                CurrentValue = Model.UatpInvoiceHandledbyAtcan.ToString(),
                FutureValue = Model.UatpInvoiceHandledbyAtcanFutureValue.ToString(),
                HasFuturePeriod = true,
                FuturePeriod = Model.UatpInvoiceHandledbyAtcanFuturePeriod
              }, skipContainer: true)%>
      </div>--%>

       <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Legal Archiving:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.LegalArchivingRequired, "", Model.UserCategory, skipContainer: true)%>
      </div>     
       

      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Legal Archive Compartment ID:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.CdcCompartmentIDforInv, "", Model.UserCategory, skipContainer: true)%>
      </div>
           

    </div>    
    <div class="fieldSeparator">

    </div>
    <h2>
      Contacts</h2>
    <div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Sampling Member - Info Contact:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.EnableSampMemInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Sampling Steering Committee Member - Info Contact:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.EnableSampScMemInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Sampling Q-SMART Member - Info Contact:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.EnableSampQSmartInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          Old IDEC Steering Committee Members - Info Contact:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.EnableOldIdecScInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          First & Final - Info Contact:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.EnableFirstNFinalInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          First & Final ASG Members - Info Contact:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.EnableFnfAsgInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          First & Final AIA Members - Info Contact:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.EnableFnfAiaInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          RAWG Members - Info Contact:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.EnableRawgMemInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          IAWG Members - Info Contact:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.EnableIawgMemInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          ICH Panel Members - Info Contact:</label>
        <%: Html.ProfileFieldFor(memberControl => memberControl.EnableIchPanelInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          E-Invoicing WG - Info Contact:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.EnableEInvWgInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
      <div class="tallFieldContainer">
        <label class="wrappingLabel">
          SIS Steering Committee - Contact:</label>
         <%: Html.ProfileFieldFor(memberControl => memberControl.EnableSisScInfoContact, "", Model.UserCategory, skipContainer: true)%>
      </div>
    </div>
  </div>
  <div class="clear"></div>
</div>
<div class="buttonContainer">
  
  <input class="primaryButton" id="SaveSisOpss" type="submit" value="Save Control Data" />
  <div class="futureUpdatesLegend">Future Updates Pending</div>

</div>



<% }%>
