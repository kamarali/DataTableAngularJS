<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.UatpConfiguration>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<script type="text/javascript">
    $(document).ready(function ()
    {
    // Control here Save button on the basis of Selected Member ID 
   if($('#selectedMemberId').val() ==0)
   { 
    $("#btnSaveUatpDetails").attr('disabled', 'disabled');   
    $("#btnSaveUatpDetails").removeClass('ignoredirty');
   }
   else
   {
     $("#btnSaveUatpDetails").removeAttr('disabled'); 
     $("#btnSaveUatpDetails").addClass('ignoredirty');     
   }


        $('#RejectionOnValidationFailureId').focus();   
        <%if (Model.MemberId == 0)
          {
            if((Model.IsDigitalSignatureRequired)||(Model.IsDigitalSignatureRequiredFutureValue))
              {%>
                $("#ISUatpInvIgnoreFromDsproc").removeAttr('disabled');
            <%}
            else
              {%>
                $("#ISUatpInvIgnoreFromDsproc").attr('disabled', 'disabled');
                $("#ISUatpInvIgnoreFromDsproc").attr('checked',false);
            <%}
        }
        if (Model.MemberId > 0)
          {
            if((Model.IsDigitalSignatureRequired)||(Model.IsDigitalSignatureRequiredFutureValue))
              {%>
                $(".ignoreUATPEditLink").show();
            <%}
            else
              {%>
                $(".ignoreUATPEditLink").hide();
            <%}
            if((Model.UatpInvoiceHandledbyAtcan)||(Model.UatpInvoiceHandledbyAtcanFutureValue.HasValue))
              {%>
                //$(".BillingDataSubmittedByThirdPartyEditLink").show();
            <%}
            else
              {%>
                //$(".BillingDataSubmittedByThirdPartyEditLink").hide();
            <%}%>
            $(".futureEditLink").show();
            $(".currentFieldValue").attr("disabled", true);
        <%}%>
    });
    
     $('#BillingIswebMigrationDate').watermark(_periodFormat);
     $("#BillingIsXmlMigrationDateUATP").watermark(_periodFormat);
     $('#BillingIsXmlMigrationStatusId').change(function () 
    { 
        if ($('#BillingIsXmlMigrationStatusId').val() == 3)
        {
            $('#uatpBillingIsXmlCertifiedOn').datepicker('enable');
            $('#BillingIsXmlMigrationDateUATP').removeAttr('disabled');
        }
        else
        {
            $('#uatpBillingIsXmlCertifiedOn').datepicker('disable');    
            $('#BillingIsXmlMigrationDateUATP').attr('disabled', 'disabled');
            $('#uatpBillingIsXmlCertifiedOn').val("");
            $('#BillingIsXmlMigrationDateUATP').val("");
            $('#BillingIsXmlMigrationDateUATP').watermark(_periodFormat);
            $('#uatpBillingIsXmlCertifiedOn').watermark(_dateWatermark);
        }
    });
</script>
<%
    using (Html.BeginForm("Uatp", "Member", FormMethod.Post, new { id = "uatp" }))
    {%>
    <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div style="width: 24%;">
            <h2>
                Validations</h2>
                <%:Html.HiddenFor(uatpModel => uatpModel.UserCategory)%>
            <%: Html.ProfileFieldFor(uatpModel => uatpModel.RejectionOnValidationFailureId, "Rejection on Validation Failure", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "RejectionOnValidationFailureId" }, { "class", "mediumTextField" } }, new Dictionary<string, object> { { "style", "width: 200px;" }, { "class", "miscuatpControl" } })%>
        </div>
        <div style="width: 24%;">
            <h2>
                Online Correction Allowed</h2>
             <%:Html.ProfileFieldFor(uatpModel => uatpModel.IsOnlineCorrectionAllowed, "Online Correction Allowed", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 300px;" } })%>
        </div>   
        <div style="width: 30%;">
            <h2>
                Supporting Documents</h2>
            <%: Html.ProfileFieldFor(uatpModel => uatpModel.UatpAllowedFileTypesForSupportingDocuments, "Additional File Types Accepted", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "AllowedFileTypes" }, { "id", "UatpAllowedFileTypesForSupportingDocuments" }, { "maxLength", "500" } }, new Dictionary<string, object> { { "class", "miscuatpControl mediumTextField" }, { "style", "width: 40%;" } }, null, new Dictionary<string, object> { { "style", "width: 400px;" } })%>
        </div>
        </div>
    <div class="fieldContainer horizontalFlow">
        <div class="topLine">
            <h2>
                Legal Services</h2>
            <%--<div class="wrappingLabel">
                <%:Html.HiddenFor(uatpModel => uatpModel.UatpInvoiceHandledbyAtcan)%>
                <label>
                    Handling of UATP invoice by ATCAN:</label>
                <div class="subscriptionRequired">
                    <%:Model.UatpInvoiceHandledbyAtcanDisplay %></div>
            </div>--%>
            <%: Html.ProfileFieldFor(uatpModel => uatpModel.ISUatpInvIgnoreFromDsproc, "Ignore UATP Invoices from DS Process", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "currentFieldValue" }, { "id", "ISUatpInvIgnoreFromDsproc" } }, null, new FutureUpdate
{
  FieldId = "ISUatpInvIgnoreFromDsproc",

  FieldName = "ISUatpInvIgnoreFromDsproc",
  FieldType = 2,
  CurrentValue = Model.ISUatpInvIgnoreFromDsproc.ToString(),
  FutureValue = Model.ISUatpInvIgnoreFromDsprocFutureValue.ToString(),
  HasFuturePeriod = true,
  FuturePeriod = Model.ISUatpInvIgnoreFromDsprocFuturePeriod,
  EditLinkClass = "ignoreUATPEditLink"
}, new Dictionary<string, object> { { "class", "legalDiv" } })%>
        </div>
    </div>
    <div class="fieldContainer horizontalFlow">
        <div class="topLine">
            <h2>
                Billing Data Output</h2>
            <div class="miscuatpControl">
                <label>
                    Billed Invoices:</label>
                IS-XML
                <%: Html.ProfileFieldFor(uatpModel => uatpModel.BillingxmlOutput, "IS-XML", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingXmlOutput" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
                                {
                                  FieldId = "BillingxmlOutput",
                                  FieldName = "BillingxmlOutput",
                                  FieldType = 2,
                                  CurrentValue = Model.BillingxmlOutput.ToString(),
                                  FutureValue = Model.BillingxmlOutputFutureValue.ToString(),
                                  HasFuturePeriod = true,
                                  FuturePeriod = Model.BillingxmlOutputFuturePeriod
                                },null,true)%>
            </div>
            <%: Html.ProfileFieldFor(uatpModel => uatpModel.IsBillingDataSubmittedByThirdPartiesRequired, "Billing Invoices Submitted On Behalf of the Member", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "IsBillingDataSubmittedByThirdPartiesRequired" }, { "class", "currentFieldValue" }, { "disabled", "disable" } }, new Dictionary<string, object> { { "style", "width: 30%" }, { "class", "miscuatpControl" } }, new FutureUpdate
                                {
                                  FieldId = "IsBillingDataSubmittedByThirdPartiesRequired",
                                  FieldName = "IsBillingDataSubmittedByThirdPartiesRequired",
                                  FieldType = 2,
                                  CurrentValue = Model.IsBillingDataSubmittedByThirdPartiesRequired.ToString(),
                                  FutureValue = Model.IsBillingDataSubmittedByThirdPartiesRequiredFutureValue.ToString(),
                                  HasFuturePeriod = true,
                                  FuturePeriod = Model.IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod
                                })%>
        </div>
    </div>
    <div class="fieldContainer horizontalFlow additionaloutput">
        <div class="topLine">
            <h2>
                Offline Archive Outputs</h2>
            <%: Html.TextBoxFor(uatpModel => uatpModel.MemberId, new { @class = "hidden", @id = "uatpMemberId" })%>
            <% Html.RenderPartial("AdditionalOutputsControl", Model); %>
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
                                <%:Html.ProfileFieldFor(uatpModel => uatpModel.BillingIsXmlMigrationStatusId, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingIsXmlMigrationStatusId" }, { "class", "miscuatpControl" } }, null, null, null, true)%>
                            </td>
                            <td>
                                <%:Html.ProfileFieldFor(uatpModel => uatpModel.BillingIsXmlCertifiedOn, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "uatpBillingIsXmlCertifiedOn" }, { "class", "datePicker" }, { "readOnly", "true" } }, null, null, null, true)%>
                            </td>
                            <td>
                                <%:Html.ProfileFieldFor(uatpModel => uatpModel.BillingIsXmlMigrationDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingIsXmlMigrationDateUATP" }, { "class", "miscuatpControl" } }, null, null, null, true)%>
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
                                <%:Html.ProfileFieldFor(uatpModel => uatpModel.BillingIswebMigrationDate, null, SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "BillingIswebMigrationDate" }, { "class", "miscuatpControl" } }, null, null, null, true)%>
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
                <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.MemberId}) %>','#divContactAssignmentSearchResult','UATP','',$('#selectedMemberId').val());">
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
        <input class="primaryButton" id="btnSaveUatpDetails" type="submit" value="Save UATP Details" />
        <div class="futureUpdatesLegend">
            Future Updates Pending</div>
    </div>
    
    <div class="subscriptionRequired" style="margin-left: 5px;">
        Subscription required to activate the optional service.</div>
</div>
<%}  %>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
    <% Html.RenderPartial("SearchResultControl");%></div>
