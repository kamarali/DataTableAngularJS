<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.CargoConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<script type="text/javascript">
  $(document).ready(function () {

   // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveCargoDetails").attr('disabled', 'disabled');   
    $("#btnSaveCargoDetails").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveCargoDetails").removeAttr('disabled'); 
     $("#btnSaveCargoDetails").addClass('ignoredirty');     
   }

    $('#RejectionOnValidationFailureId').focus();
    
 <%
   if (Model.MemberId > 0)
   {%>
    $(".futureEditLink").show();
    $('.currentFieldValue').attr("disabled", true);
<%
   }%>

//  $('#cgoPrimeBillingIsIdecCertifiedOn').datepicker('disable');
//  $('#cgoPrimeBillingIsxmlCertifiedOn').datepicker('disable');
//  $('#cgoRmIsIdecCertifiedOn').datepicker('disable');
//  $('#cgoRmIsXmlCertifiedOn').datepicker('disable');
//  $('#cgoBmIsIdecCertifiedOn').datepicker('disable');
//  $('#cgoBmIsXmlCertifiedOn').datepicker('disable');
//  $('#cgoCmIsIdecCertifiedOn').datepicker('disable');
//  $('#cgoCmIsXmlCertifiedOn').datepicker('disable');
  });
</script>
<%
   using (Html.BeginForm("Cgo", "Member", FormMethod.Post, new { id = "cgo" }))
   {%>
   <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer verticalFlow">
    <div style="width: 24%;">
      <h2>
        Validations</h2>
          <%:Html.HiddenFor(cargoDetails => cargoDetails.UserCategory)%>
      <%:Html.ProfileFieldFor(cargoDetails => cargoDetails.RejectionOnValidationFailureId,
                                            "Rejection on Validation Failure",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "id", "RejectionOnValidationFailureId" }, { "class", "mediumTextField" } },
                                            new Dictionary<string, object> {{ "class", "miscuatpControl wrappingLabel" } })%>

      
     </div>
      <div style="width: 24%;">
             <h2>
                Online Correction Allowed</h2>
        <%:Html.ProfileFieldFor(cargoDetails => cargoDetails.IsOnlineCorrectionAllowed, "Online Correction Allowed", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
     </div>     

    <div style="width: 30%;">
      <h2>
        Supporting Documents</h2>
      <%:Html.ProfileFieldFor(cargoDetails => cargoDetails.CgoAllowedFileTypesForSupportingDocuments,
                                            "Additional File Types Accepted",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "class", "AllowedFileTypes" }, { "id", "CgoAllowedFileTypesForSupportingDocuments" }, { "maxLength", "500" } },
                                            new Dictionary<string, object> { { "class", "miscuatpControl mediumTextField" }, { "style", "width: 40%;" } })%>
    </div>
    
  </div>
  <div class="fieldContainer horizontalFlow additionaloutput">
  <div class="topLine miscuatpControl">
      <h2>
        Output Files</h2>
      <div style="width: 250px">
        <label>
          Billed Invoices:</label>
        IS-IDEC
        <%:Html.ProfileFieldFor(cargoDetails => cargoDetails.BillingInvoiceIdecOutput,
                                            "IS-IDEC",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "id", "BillingInvoiceIdecOutput" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "BillingInvoiceIdecOutput",
                                                FieldName = "BillingInvoiceIdecOutput",
                                                FieldType = 2,
                                                CurrentValue = Model.BillingInvoiceIdecOutput.ToString(),
                                                FutureValue = Model.BillingInvoiceIdecOutputFutureValue.ToString(),
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.BillingInvoiceIdecOutputFuturePeriod
                                              },
                                            null,
                                            true)%>
        IS-XML
        <%:Html.ProfileFieldFor(cargoDetails => cargoDetails.BillingInvoiceXmlOutput,
                                            "IS-XML",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "id", "BillingInvoiceXmlOutput" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "BillingInvoiceXmlOutput",
                                                FieldName = "BillingInvoiceXmlOutput",
                                                FieldType = 2,
                                                CurrentValue = Model.BillingInvoiceXmlOutput.ToString(),
                                                FutureValue = Model.BillingInvoiceXmlOutputFutureValue.ToString(),
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.BillingInvoiceXmlOutputFuturePeriod
                                              },
                                            null,
                                            true)%>
      </div>
      <%:Html.HiddenFor(cargoDetails => cargoDetails.CgoOldIdecMember)%>
      <%: Html.ProfileFieldFor(cargoDetails => cargoDetails.DownConvertISTranToOldIdeccgo, "Down Converted IS Transaction To Old IDEC Format", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width:250px;" } })%>
    </div>
  </div>
  <div class="fieldContainer horizontalFlow additionaloutput">
    <div class="topLine">
      <h2>
        Offline Archive Outputs</h2>
      <%:Html.TextBoxFor(cargoDetails => cargoDetails.MemberId, new { @class = "hidden", id = "cgoMemberId" })%>
      <%
     Html.RenderPartial("AdditionalOutputsControl", Model);%>
    </div>
  </div>
  <div class="fieldContainer verticalFlow">
      <div class="topLine bottomLine miscuatpControl ">
        <h2>
          Certification and Migration Details</h2>
        <%
     Html.RenderPartial("~/Areas/Profile/Views/Shared/CargoMigration.ascx");%>
      </div>
    <div>
    
</div>
    
    </div>
  <div class="fieldContainer verticalFlow">
      <div class="halfWidthColumn">
        <div>
          <h2>
            IS Contacts</h2>
          <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile" ,selectedMemberId= Model.MemberId})%>','#divContactAssignmentSearchResult','CGO','<%:Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile" })%>',$('#selectedMemberId').val());">
            View/Edit</a>
        </div>
      </div>
      
      <div class="clear">
  </div>
    </div>
      <%:Html.HiddenFor(cargoDetails => cargoDetails.ContactList, new { @id = "CgoDetailsContactList" })%>
    <div class="clear">
    </div>
  </div>
  
<div class="buttonContainer">
  
  <input class="primaryButton" id = "btnSaveCargoDetails" type="submit" value="Save Cargo Details" />
  <div class="futureUpdatesLegend">Future Updates Pending</div>
 
</div>
<%
  }%>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
  <% Html.RenderPartial("SearchResultControl");%></div>
