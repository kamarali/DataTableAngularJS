<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.PassengerConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<script type="text/javascript">
  $(document).ready(function () {

  //SCP221813 - Auto Billing issue.
   $('#btnSavePaxDetails').click(function ()
    {
       if ($("#pax" ).valid() && !$("#IsParticipateInAutoBilling").prop('checked') &&  $("#ListingCurrencyId").val() == '') 
        {
          $("#ListingCurrencyId").val($('#ListingCurrencyIdFutureValue').val());
        }
    });
  // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSavePaxDetails").attr('disabled', 'disabled');   
    $("#btnSavePaxDetails").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSavePaxDetails").removeAttr('disabled'); 
     $("#btnSavePaxDetails").addClass('ignoredirty');     
   }

  $('#RejectionOnValidationFailureId').focus();
   <%
     if (Model.MemberId > 0)
     {%>
    $(".futureEditLink").show();
    $('.currentFieldValue').attr("disabled", true);
  <%
     }%>
   $('#InvoiceNumberRangeTo').attr('maxLength',10);
   $('#InvoiceNumberRangeFrom').attr('maxLength',10);
    $('#IsAutomatedVcDetailsReportRequired').attr('disabled', 'disabled');
    $('#IsParticipateInAutoBilling').attr('disabled', 'disabled');
    $('#InvoiceNumberRangePrefix').attr('readonly', 'true');
    $('#InvoiceNumberRangeFrom').attr('readonly', 'true');
    $('#InvoiceNumberRangeTo').attr('readonly', 'true');
    $('#CutOffTime').attr('readonly', 'true');
    $('#IsIsrFileRequired').attr('disabled', 'disabled');
    $('#ListingCurrencyId').attr('disabled', 'disabled');
      
    $('#IsParticipateInAutoBilling').change(function () {
   
    if ($("#paxMemberId").val()=="0")
    {
      if ($(this).prop('checked') == true) {
        $('#InvoiceNumberRangePrefix').removeAttr("readonly");
        $('#InvoiceNumberRangeFrom').removeAttr("readonly");
        $('#InvoiceNumberRangeTo').removeAttr("readonly");
        $('#CutOffTime').removeAttr("readonly");
        $('#IsIsrFileRequired').removeAttr("disabled");
        $('#ListingCurrencyId').removeAttr("disabled");
      }
      else {
        $('#InvoiceNumberRangePrefix').val("");
        $('#InvoiceNumberRangeFrom').val("");
        $('#InvoiceNumberRangeTo').val("");
        $('#CutOffTime').val("");
        $('#ListingCurrencyId').val("");
        $('#InvoiceNumberRangePrefix').attr('readonly', 'true');
        $('#InvoiceNumberRangeFrom').attr('readonly', 'true');
        $('#InvoiceNumberRangeTo').attr('readonly', 'true');
        $('#CutOffTime').attr('readonly', 'true');
        $('#IsIsrFileRequired').attr('disabled', 'disabled');
        $('#ListingCurrencyId').attr('disabled', 'disabled');
        $("#IsIsrFileRequired").attr({ "checked": false });
      }
      }
      

    });

    <%
     if ((Model.IsParticipateInAutoBilling) || (Model.IsParticipateInAutoBillingFutureValue==true))
     {%>
        $('#InvoiceNumberRangePrefix').removeAttr("readonly");
        $('#InvoiceNumberRangeFrom').removeAttr("readonly");
        $('#InvoiceNumberRangeTo').removeAttr("readonly");
        $(".cuttOffTimeEditLink").show();
        $(".listingCurrencyEditLink").show();
        $(".isIsrFileRequiredEditLink").show();
      <%
     }%>
       <%
     else
     {%>
      
        $('#InvoiceNumberRangePrefix').val("");
        $('#InvoiceNumberRangeFrom').val("");
        $('#InvoiceNumberRangeTo').val("");
        $('#CutOffTime').val("");
        $('#ListingCurrencyId').val("");
        $('#InvoiceNumberRangePrefix').attr('readonly', 'true');
        $('#InvoiceNumberRangeFrom').attr('readonly', 'true');
        $('#InvoiceNumberRangeTo').attr('readonly', 'true');
        $('#CutOffTime').attr('readonly', 'true');
        $('#IsIsrFileRequired').attr('disabled', 'disabled');
        $('#ListingCurrencyId').attr('disabled', 'disabled');
        $("#IsIsrFileRequired").attr({ "checked": false });
         $(".cuttOffTimeEditLink").hide();
        $(".listingCurrencyEditLink").hide();
        $(".isIsrFileRequiredEditLink").hide();
      <%
     }%>
   
       <%
     if (Model.IsParticipateInValueDetermination)
     {%>
        <%
       if (Model.IsParticipateInAutoBillingFutureValue == null)
       {%>
            $("#IsParticipateInAutoBilling").removeAttr('disabled');
         <%
       }%>
      <%
     }%>
     <%
     else
     {%>
     
        $("#IsParticipateInAutoBilling").attr('disabled', 'disabled');
        $("#IsParticipateInAutoBilling").attr({ "checked": false });
        $("#IsParticipateInAutoBillingFutureDateInd").hide();

        $('#InvoiceNumberRangePrefix').val("");
        $('#InvoiceNumberRangeFrom').val("");
        $('#InvoiceNumberRangeTo').val("");
        $('#CutOffTime').val("");
        $('#ListingCurrencyId').val("");
        $('#InvoiceNumberRangePrefix').attr('readonly', 'true');
        $('#InvoiceNumberRangeFrom').attr('readonly', 'true');
        $('#InvoiceNumberRangeTo').attr('readonly', 'true');
        $('#CutOffTime').attr('readonly', 'true');
        $('#IsIsrFileRequired').attr('disabled', 'disabled');
        $('#ListingCurrencyId').attr('disabled', 'disabled');
        $("#IsIsrFileRequired").attr({ "checked": false });
         $(".cuttOffTimeEditLink").hide();
        $(".listingCurrencyEditLink").hide();
        $(".isIsrFileRequiredEditLink").hide();
         $("#ListingCurrencyIdFutureDateInd").hide();
          $("#CutOffTimeFutureDateInd").hide();
           $("#IsIsrFileRequiredFutureDateInd").hide();
        
    <%
     }%>

       <%
     if (Model.IsParticipateInValueConfirmation)
     {%> 
        $("#IsAutomatedVcDetailsReportRequired").removeAttr('disabled');
      <%
     }%>
     <%
     else
     {%>
        $("#IsAutomatedVcDetailsReportRequired").attr('disabled', 'disabled');
        $("#IsAutomatedVcDetailsReportRequired").attr({ "checked": false });
       
     <%
     }%>
     $('#SamplingCareerTypeId').change(function () {
     if ($("#paxMemberId").val()=="0"){
       if(($('#SamplingCareerTypeId').val()== 1) || ($('#SamplingCareerTypeId').val()== 3))
       {
         $('#IsConsolidatedProvisionalBillingFileRequired').attr('disabled', 'disabled');
         $("#IsConsolidatedProvisionalBillingFileRequired").attr({ "checked": false });
       }
       else
       {
          $('#IsConsolidatedProvisionalBillingFileRequired').removeAttr('disabled');
       }
     }
     else
     {
       if(($('#SamplingCareerTypeId').val()== 1) || ($('#SamplingCareerTypeId').val()== 3))
       {
         $(".provBillingFileEditLink").hide();
         $("#IsConsolidatedProvisionalBillingFileRequiredFutureDateInd").hide();
          $("#IsConsolidatedProvisionalBillingFileRequired").attr({ "checked": false });
       }
       else
       {
        $(".provBillingFileEditLink").show();
        if ($("#IsConsolidatedProvisionalBillingFileRequiredFutureValue").val() !="")
          $("#IsConsolidatedProvisionalBillingFileRequiredFutureDateInd").show();
       }
     }
  });
  
     });

</script>
<%
    using (Html.BeginForm("Pax", "Member", FormMethod.Post, new { id = "pax" }))
    {%>
    <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
    <div class="fieldContainer verticalFlow">
        <div style="width: 24%;">
            <h2>
                Validations</h2>
            <%:Html.HiddenFor(paxDetails => paxDetails.UserCategory)%>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.RejectionOnValidationFailureId, "Rejection on Validation Failure", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "RejectionOnValidationFailureId" }, { "class", "mediumTextField" } }, new Dictionary<string, object> {{ "class", "miscuatpControl" } })%>
            <h2>Future Billings Submissions</h2>  
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.IsFutureBillingSubmissionsAllowed, "Future Billing Submissions Allowed", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%> 
            <h2>
                Online Correction Allowed</h2>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.IsOnlineCorrectionAllowed, "Online Correction Allowed", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
               
           <h2>
                Supporting Documents</h2>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.PaxAllowedFileTypesForSupportingDocuments, "Additional File Types Accepted",SessionUtil.UserCategory,new Dictionary<string, object>{{"class","AllowedFileTypes"},{"id","PaxAllowedFileTypesForSupportingDocuments"},{"maxLength","500"}})%>
        </div>
    
        <div style="border-left: 1px dotted #888; padding-left: 10px; width: 24%;">
            <h2>
                Sampling</h2>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.SamplingCareerTypeId, "Sampling Carrier", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "mediumTextField" } })%>
            <h2>
                Billing Value Confirmation (BVC)</h2>
            <%:Html.HiddenFor(paxDetails => paxDetails.IsParticipateInValueConfirmation)%>
            <label>
                Participate In Billing Value Confirmation</label>
            <div class="subscriptionRequired">
                <%:Model.IsParticipateInValueConfirmationDisplay%></div>
        </div>
        <div class="oneColumnWidth" style="border-left: 1px dotted #888; padding-left: 10px;
            width: 45%;">
            <h2>
                Value Determination and Auto Billing</h2>
            <div class="horizontalFlow fieldContainer">
                <div>
                    <div style="width: 50%;">
                        <%:Html.HiddenFor(paxDetails => paxDetails.IsParticipateInValueDetermination)%>
                        <label>
                            Participate In Value Determination</label>
                        <div class="subscriptionRequired">
                            <%:Model.IsParticipateInValueDeterminationDisplay%></div>
                    </div>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.IsParticipateInAutoBilling, "Participate In Auto Billing", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IsParticipateInAutoBilling" }, { "class", "currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
{
  FieldId = "IsParticipateInAutoBilling",
  FieldName = "IsParticipateInAutoBilling",
  FieldType = 2,
  CurrentValue = Model.IsParticipateInAutoBilling.ToString(),
  FutureValue = Model.IsParticipateInAutoBillingFutureValue.ToString(),
  HasFuturePeriod = false,
  FutureDate = Model.IsParticipateInAutoBillingFutureDate,
  EditLinkClass ="autoBillingEditLink"
})%>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.InvoiceNumberRangePrefix, "Invoice Number Range-Prefix", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 50%;" }, { "class", "alphaNumeric" } })%>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.InvoiceNumberRangeFrom, "Invoice Number-From", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 50%;" }, { "class", "numericOnly" } })%>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.InvoiceNumberRangeTo, "Invoice Number-To", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 50%;" }, { "class", "numericOnly" } })%>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.IsIsrFileRequired, "ISR File Required", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IsIsrFileRequired" }, { "class", "currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
{
  FieldId = "IsIsrFileRequired",
  FieldName = "IsIsrFileRequired",
  FieldType = 2,
  CurrentValue = Model.IsIsrFileRequired.ToString(),
  FutureValue = Model.IsIsrFileRequiredFutureValue.ToString(),
  HasFuturePeriod = true,
  FuturePeriod = Model.IsIsrFileRequiredFuturePeriod,
  EditLinkClass = "isIsrFileRequiredEditLink"
})%>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.CutOffTime, "Cut Off Time(Hours)", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "CutOffTime" }, { "class", "smallTextField currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
{
  FieldId = "CutOffTime",
  FieldName = "CutOffTime",
  FieldType = 1,
  CurrentValue = Model.CutOffTime.ToString(),
  FutureValue = Model.CutOffTimeFutureValue.ToString(),
  HasFuturePeriod = true,
  FuturePeriod = Model.CutOffTimeFuturePeriod,
  EditLinkClass = "cuttOffTimeEditLink"
})%>
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.ListingCurrencyId, "Currency Of Listing", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "ListingCurrencyId" }, { "class", "currentFieldValue" } }, new Dictionary<string, object> { { "style", "width: 50%;" } }, new FutureUpdate
{
  FieldId = "ListingCurrencyId",
  FieldName = "ListingCurrencyId",
  FieldType = 14,
  CurrentValue = Model.ListingCurrencyId.ToString(),
  CurrentDisplayValue = Model.ListingCurrencyIdDisplayValue,
  FutureValue = Model.ListingCurrencyIdFutureValue.ToString(),
  FutureDisplayValue = Model.ListingCurrencyIdFutureDisplayValue,
  HasFuturePeriod = true,
  FuturePeriod = Model.ListingCurrencyIdFuturePeriod,
  EditLinkClass ="listingCurrencyEditLink"
})%>
                </div>
            </div>
        </div>
    </div>
    <div class="fieldContainer horizontalFlow">
        <div class="topLine bottomLine miscuatpControl">
            <h2>
                Output Files:</h2>
            <div style="width: 250px">
                <label>
                    Billed Invoices:</label>
                <div>
                    IS-IDEC
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.BillingInvoiceIdecOutput, "IS-IDEC", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingInvoiceIdecOutput" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "BillingInvoiceIdecOutput",
  FieldName = "BillingInvoiceIdecOutput",
  FieldType = 2,
  CurrentValue = Model.BillingInvoiceIdecOutput.ToString(),
  FutureValue = Model.BillingInvoiceIdecOutputFutureValue.ToString(),
  HasFuturePeriod = true,
  FuturePeriod = Model.BillingInvoiceIdecOutputFuturePeriod
},null,true)%>
                    IS-XML
                    <%: Html.ProfileFieldFor(paxDetails => paxDetails.BillingInvoiceXmlOutput, "IS-XML", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingInvoiceXmlOutput" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
                                {
                                  FieldId = "BillingInvoiceXmlOutput",
                                  FieldName = "BillingInvoiceXmlOutput",
                                  FieldType = 2,
                                  CurrentValue = Model.BillingInvoiceXmlOutput.ToString(),
                                  FutureValue = Model.BillingInvoiceXmlOutputFutureValue.ToString(),
                                  HasFuturePeriod = true,
                                  FuturePeriod = Model.BillingInvoiceXmlOutputFuturePeriod
                                },null,true)%>
                </div>
            </div>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.IsConsolidatedProvisionalBillingFileRequired, "Addnl. Sampling Prov. Billing File (Monthly)", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingInvoiceXmlOutput" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
                                {
                                  FieldId = "IsConsolidatedProvisionalBillingFileRequired",
                                  FieldName = "IsConsolidatedProvisionalBillingFileRequired",
                                  FieldType = 2,
                                  CurrentValue = Model.IsConsolidatedProvisionalBillingFileRequired.ToString(),
                                  FutureValue = Model.IsConsolidatedProvisionalBillingFileRequiredFutureValue.ToString(),
                                  HasFuturePeriod = true,
                                  FuturePeriod = Model.IsConsolidatedProvisionalBillingFileRequiredFuturePeriod,
                                  EditLinkClass = "provBillingFileEditLink"
                                }, new Dictionary<string, object> { { "class", "wrappingLabel" } })%>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.IsAutomatedVcDetailsReportRequired, "BVC Details Report", SessionUtil.UserCategory)%>
            <%:Html.HiddenFor(paxDetails => paxDetails.PaxOldIdecMember)%>
            <%: Html.ProfileFieldFor(paxDetails => paxDetails.DownConvertISTranToOldIdec, "Down Converted IS Transaction To Old IDEC Format", SessionUtil.UserCategory, null, null, null, new Dictionary<string, object> { { "class", "wrappingLabel" } })%>
        </div>
    </div>
    <div class="fieldContainer verticalFlow">
        <div class="oneColumn">
            <h2>
                Offline Archive Outputs</h2>
            <%:Html.TextBoxFor(paxDetails => paxDetails.MemberId, new { @class = "hidden", id = "paxMemberId" })%>
            <div class="additionaloutput">
                <%
        Html.RenderPartial("AdditionalOutputsControl", Model);%>
            </div>
        </div>
    </div>
    <div class="fieldContainer verticalFlow">
        <div class="topLine bottomLine miscuatpControl">
            <h2>
                Certification and Migration Details</h2>
            <%
        Html.RenderPartial("~/Areas/Profile/Views/Shared/PassengerMigration.ascx");%>
        </div>
    </div>   
     <div class="fieldContainer verticalFlow ">
        <div class="halfWidthColumn">
            <div>
                <h2>
                    IS Contacts</h2>
                <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>','#divContactAssignmentSearchResult','PAX','', $('#selectedMemberId').val());">
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
        <input class="primaryButton" id="btnSavePaxDetails" type="submit" value="Save Passenger Details" />
    </div>
    <div class="futureUpdatesLegend">
        Future Updates Pending</div>
   
    <div class="subscriptionRequired" style="margin-left: 5px;">
        Subscription required to activate the optional service.</div>
</div>
<div>
    <%:Html.HiddenFor(paxDetails => paxDetails.ContactList, new { @id ="paxContactList" })%>
</div>
<% }%>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
    <% Html.RenderPartial("SearchResultControl");%></div>
