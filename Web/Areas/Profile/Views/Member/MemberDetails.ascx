<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Member>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.Calendar" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<script type="text/javascript">
  function resetForm() {
    $(':input', '#MemberDetails')
      .not(':button, :submit, :reset, :hidden')
      .val('')
      .removeAttr('selected');
    $("#IsMembershipStatusId").val("-1");
    $("#IsMembershipSubStatusId").val("1");
    $("#DefaultLocation_CountryId").val("-1");
    $("#DefaultLocation_CurrencyId").val("-1");
    $("#IataMemberStatus").prop("checked", false);
    $("#IchMemberStatus").prop("checked", false);
    $("#AchMemberStatus").prop("checked", false);
    window.location.href = "Create";
  }
  var urlEmailSend = '<%:Url.Action("MailSender", "Member",new {selectedMemberId = Model.Id})%>';
  $ImageUploaddialog = $('<div></div>')
    .html($("#divImageUpload"))
    .dialog({
      autoOpen: false,
      title: 'Logo',
      height: 130,
      width: 280,
      minWidth:280,
      minHeight:130,
      modal: true,
      resizable: false
    });

  $(document).ready(function () {

    /*JqueryUI: Validation call for Member details Fields.*/
    initMemberDetailsTabValidations();

    $('#hdnIsMembershipStatus').val(<% =ViewData["memberStatus"]%>); 
    //CMP#603 Member Profile-Changes in IS Membership Sub Status
   // $("#IsMembershipSubStatusId").attr('disabled', 'disabled');
    //$("#IsMembershipSubStatusId").val("1");
    //$('#MemberCodeNumeric').focus();

     if (($('#IsMembershipStatusId').val() == 1)) {
      $('#divEntryDate').show();
      //CMP#603 Member Profile-Changes in IS Membership Sub Status
      // $('#IsMembershipSubStatusId').removeAttr('disabled');
    } else {
      $('#divEntryDate').hide();
    }

    if (($('#IsMembershipStatusId').val() == 4)) {
      $('#divTerminationDate').show();
    } else {
      $('#divTerminationDate').hide();
    }

  <%
    if (Model.Id > 0)
    {%>
    $(".futureEditLink").show();
    $('.currentFieldValue').attr("disabled", true);
    $('#createSuperUser').attr("disabled", false);
<%
    }%>

  <%
    if ((Model.Id > 0) && (Model.IsMembershipStatusIdFutureValue != null))
    {%>
      $(".statusEditLink").show();
  <%
    }%>

    registerAutocomplete('DefaultLocation_SubDivisionName', 'DefaultLocation_SubDivisionName', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = "" })%>', 0, true, null, '', '', '#DefaultLocation_CountryId');

    // If logged in user is not SIS Ops then entry date and termination date datepicker controls should be disabled
    <%
    if (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps) {%>
    $('#memberEntryDate').removeClass('datePicker');
    $('#memberEntryDate').addClass('smallTextField');
    $('#memberTerminationDate').removeClass('datePicker');
    $('#memberTerminationDate').addClass('smallTextField');
    <%
    }%>

    $('#DefaultLocation_CountryId').change(function () {
      $('#DefaultLocation_SubDivisionName').val("");
      $('#DefaultLocation_SubDivisionName').flushCache();
    });

    $('#IsMembershipStatusId').change(function () {
      if ($('#IsMembershipStatusId').val() == 4) {
        if (($('#IsMembershipStatusId').val() != $('#hdnIsMembershipStatus').val()) && (!SaveConfirmation())) {
          ResetMemberStatusOnCancel();         
        } 
        else if (($('#IsMembershipStatusId').val() != $('#hdnIsMembershipStatus').val()) && ($('#DefaultLocation_CountryId').val() != "")) {
          $('.IsMembershipStatus').attr("disabled", true);
          popupFutureUpdateDialog('#IsMembershipStatusId', 18, 1, '<%: Url.Content("~/Content/Images/calendar.gif") %>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>','<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>');
        }
        else {
        //CMP#603 Member Profile-Changes in IS Membership Sub Status
          //$("#IsMembershipSubStatusId").val("");
          //$("#IsMembershipSubStatusId").attr('disabled', 'disabled');
          $('#divTerminationDate').show();
          $('#divEntryDate').hide();
        }
      } else if (($('#IsMembershipStatusId').val() == 1)) {
      //CMP#603 Member Profile-Changes in IS Membership Sub Status
       // $('#IsMembershipSubStatusId').removeAttr('disabled');
        $('#divEntryDate').show();
        $('#divTerminationDate').hide();
      } else {
      //CMP#603 Member Profile-Changes in IS Membership Sub Status
       // $("#IsMembershipSubStatusId").val("");
       //$("#IsMembershipSubStatusId").attr('disabled', 'disabled');
        $('#divTerminationDate').hide();
        $('#divEntryDate').hide();
      }
    });

  });

  function SaveConfirmation() {
    if (confirm("Do you want to Continue?")) {
      return true;
    }

    return false;
  }

  function ResetMemberStatusOnCancel() {
    if ($('#hdnIsMembershipStatus').val() == 0) {
      $('#IsMembershipStatusId')[$('#hdnIsMembershipStatus').val()][$('#hdnIsMembershipStatus').val()].selected = true;
    }
    else {
      $('#IsMembershipStatusId').val($('#hdnIsMembershipStatus').val());
      $('#IsMembershipStatusId').change();
    }
  }
</script>
<% using (Html.BeginForm("MemberDetails", "Member", FormMethod.Post, new { id = "MemberDetails" }))
   {%>
   <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <h2>
                Member Information</h2>
            <%:Html.HiddenFor(memberDetails => memberDetails.Id, new { id = "memberId" })%>
            <%:Html.HiddenFor(memberDetails => memberDetails.IchMemberStatusId)%>
            <%:Html.HiddenFor(memberDetails => memberDetails.AchMemberStatusId)%>
            <%:Html.HiddenFor(memberDetails => memberDetails.DefaultLocation.IsActive)%>
            <%--SCP176737: Member prefix code not case sensitive(Added alphaNumeric upperCase class on Member Prefix input field)--%>
            <%--CMP #596: Length of Member Accounting Code to be Increased to 12 
                Desc: Field is mandatory with a minimum length of 3 and maximum length of 12.
                Ref: FRS Section 3.1 Point 9 and 10 --%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.MemberCodeNumeric,
                                            "Member Prefix",
                                            SessionUtil.UserCategory,
                                                   new Dictionary<string, object> { { "minLength", 3 }, { "maxLength", 12 }, { "class", "alphaNumeric upperCase minMemPrefix" } },
                                            new Dictionary<string, object> { { "id", "divSuperUser" } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.MemberCodeAlpha,
                                            "Member Designator",
                                            SessionUtil.UserCategory,
                                                 new Dictionary<string, object> { { "maxLength", 2 }, { "class", "alphaNumeric upperCase" } })%>
            <%--CMP597: TFS_Bug_8930 IS WEB -Memebr legal name is not a future update field from SIS ops login--%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.LegalName, "Member Legal Name", SessionUtil.UserCategory,
                                    new Dictionary<string, object> { { "maxLength", 100 }, { "id", "LegalName" }, { "class", "currentFieldValue" } },
                                    null, new FutureUpdate
                                              {
                                                FieldId = "LegalName",
                                                FieldName = "LegalName",
                                                FieldType = 1,
                                                CurrentValue = Model.LegalName,
                                                FutureValue = Model.LegalNameFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.LegalNameFuturePeriod,
                                                IsFieldMandatory = true
                                              })%>

            <%:Html.ProfileFieldFor(memberDetails => memberDetails.CommercialName,
                                            "Member Commercial Name",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 100 } })%>
            <div style="width: 160px;">
                <%
       if ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.Member))
       {%>
                <img alt='Click to upload member logo...' id="imgMemebrLogo" onclick="showImageDialog()"
                    src='<%:Model.Id > 0
                           ? Url.Action("GetMemberLogo", "Member", new { area = "Profile", memberId = Model.Id })
                           : Url.Content("~/Content/Images/no_member_logo.gif")%>' style="cursor: pointer;"
                    title='Click to upload member logo...' />
                <%
       }
       else
       {%>
                <img alt='Member logo...' id="imgMemebrLogoReadOnly" src='<%:Model.Id> 0
                           ? Url.Action("GetMemberLogo", "Member", new { area = "Profile", memberId = Model.Id })
                           : Url.Content("~/Content/Images/no_member_logo.gif")%>' />
                <%
       }%>
            </div>
        </div>
        <div class="bottomLine">
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.IsOpsComments,
                                            "Comments",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 500 }, { "rows", 3 }, { "cols", 156 } })%>
        </div>
        <div>
            <h2>
                Membership Details</h2>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.IsMembershipStatusId,
                                            "IS Membership Status",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "id", "IsMembershipStatusId" }, { "class", "IsMembershipStatus" } },
                                            new Dictionary<string, object> { { "id", "divIsMembershipStatus" }, { "style", "width: 23%;" } },
                                            new FutureUpdate
                                              {
                                                FieldId = "IsMembershipStatusId",
                                                FieldName = "IsMembershipStatusId",
                                                FieldType = 18,
                                                CurrentValue = Model.IsMembershipStatusId.ToString(),
                                                CurrentDisplayValue = Model.IsMembershipStatusIdDisplayValue,
                                                FutureValue = Model.IsMembershipStatusIdFutureValue.ToString(),
                                                FutureDisplayValue = Model.IsMembershipStatusIdFutureDisplayValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.IsMembershipStatusIdFuturePeriod,
                                                EditLinkClass = "statusEditLink"
                                              })%>
            <input id="hdnIsMembershipStatus" type="hidden" />
            <img alt='View Membership Status History' class="imglegend" id="statusHistory" onclick="showStatusHistoryDialog('#divMemStatusHistory','MEM','<%:Url.Action("GetMemberHistory", "Member", new { area = "Profile",selectedMemberId= Model.Id })%>')"
                src='<%:Url.Content("~/Content/Images/MemberStatusDetails.png")%>' style="cursor: pointer;"
                title='View Membership Status History' />
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.IsMembershipSubStatusId, "IS Membership Sub Status", SessionUtil.UserCategory)%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.EntryDate,
                                            "IS Entry Date",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "class", "datePicker" }, { "id", "memberEntryDate" }, { "readOnly", "true" } },
                                            new Dictionary<string, object> { { "id", "divEntryDate" }, { "style", "width: 13%;" } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.TerminationDate,
                                            "IS Termination Date",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "class", "datePicker" }, { "id", "memberTerminationDate" }, { "readOnly", "true" } },
                                            new Dictionary<string, object> { { "id", "divTerminationDate" }, { "style", "width: 13%;" } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.IataMemberStatus,
                                            "IATA Membership",
                                            SessionUtil.UserCategory,
                                            null,
                                            new Dictionary<string, object> { { "style", "width: 13%;" } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.IchMemberStatus, "ICH Member", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 13%;" } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.AchMemberStatus, "ACH Member", SessionUtil.UserCategory, null, new Dictionary<string, object> { { "style", "width: 13%;" } })%>
        </div>
        <%--CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users - Commented if--%>
        <%--<%
       if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
       {%>--%>
        <div class="topLine">
            <h2>
                Main Location Details</h2>
       <%--CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users - Commented if--%>
         <%--   <%
       }%>--%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.RegistrationId,
                                            "Company Registration ID",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 25 }, { "id", "DefaultLocation_RegistrationId" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_RegistrationId",
                                                FieldName = "DefaultLocation.RegistrationId",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.RegistrationId,
                                                FutureValue = Model.DefaultLocation.RegistrationIdFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.RegistrationIdFuturePeriod
                                              })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.TaxVatRegistrationNumber,
                                            "Tax/VAT Registration #",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 25 }, { "id", "DefaultLocation_TaxVatRegistrationNumber" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_TaxVatRegistrationNumber",
                                                FieldName = "DefaultLocation.TaxVatRegistrationNumber",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.TaxVatRegistrationNumber,
                                                FutureValue = Model.DefaultLocation.TaxVatRegistrationNumberFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.TaxVatRegistrationNumberFuturePeriod
                                              })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.AdditionalTaxVatRegistrationNumber,
                                            "Add. Tax/VAT Registration #",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 25 }, { "id", "DefaultLocation_AdditionalTaxVatRegistrationNumber" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_AdditionalTaxVatRegistrationNumber",
                                                FieldName = "DefaultLocation.AdditionalTaxVatRegistrationNumber",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.AdditionalTaxVatRegistrationNumber,
                                                FutureValue = Model.DefaultLocation.AdditionalTaxVatRegistrationNumberFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.AdditionalTaxVatRegistrationNumberFuturePeriod
                                              })%>
<%--CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users - Commented if--%>
<%--            <%
       if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
       {%>--%>
        </div>
<%--CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users - Commented if--%>
<%--        <%
       }%>--%>
        <div>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.AddressLine1,
                                            "Address Line1",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 70 }, { "id", "DefaultLocation_AddressLine1" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_AddressLine1",
                                                FieldName = "DefaultLocation.AddressLine1",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.AddressLine1,
                                                FutureValue = Model.DefaultLocation.AddressLine1FutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.AddressLine1FuturePeriod,
                                                IsFieldMandatory = true
                                              })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.AddressLine2,
                                            "Address Line2",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 70 }, { "id", "DefaultLocation_AddressLine2" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_AddressLine2",
                                                FieldName = "DefaultLocation.AddressLine2",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.AddressLine2,
                                                FutureValue = Model.DefaultLocation.AddressLine2FutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.AddressLine2FuturePeriod
                                              })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.AddressLine3,
                                            "Address Line3",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 70 }, { "id", "DefaultLocation_AddressLine3" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_AddressLine3",
                                                FieldName = "DefaultLocation.AddressLine3",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.AddressLine3,
                                                FutureValue = Model.DefaultLocation.AddressLine3FutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.AddressLine3FuturePeriod
                                              })%>
        </div>
        <div class="bottomLine">
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.CityName,
                                            "City Name",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 50 }, { "id", "DefaultLocation_CityName" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_CityName",
                                                FieldName = "DefaultLocation.CityName",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.CityName,
                                                FutureValue = Model.DefaultLocation.CityNameFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.CityNameFuturePeriod,
                                                IsFieldMandatory = true
                                              })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.PostalCode,
                                            "Postal Code",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 50 }, { "id", "DefaultLocation_PostalCode" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_PostalCode",
                                                FieldName = "DefaultLocation.PostalCode",
                                                FieldType = 1,
                                                CurrentValue = Model.DefaultLocation.PostalCode,
                                                FutureValue = Model.DefaultLocation.PostalCodeFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.PostalCodeFuturePeriod
                                              })%>
            <%:Html.TextBoxFor(memberDetails => memberDetails.DefaultLocation.Id, new { @class = "hidden" })%>
            <%:Html.TextBoxFor(memberDetails => memberDetails.DefaultLocation.LocationCode, new { @class = "hidden" })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.CountryId,
                                            "Country Name",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "id", "DefaultLocation_CountryId" }, { "class", "currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_CountryId",
                                                FieldName = "DefaultLocation.CountryId",
                                                FieldType = 8,
                                                CurrentValue = Model.DefaultLocation.CountryId,
                                                CurrentDisplayValue = Model.DefaultLocation.CountryIdDisplayValue,
                                                FutureValue = Model.DefaultLocation.CountryIdFutureValue,
                                                FutureDisplayValue = Model.DefaultLocation.CountryIdFutureDisplayValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.CountryIdFuturePeriod,
                                                IsFieldMandatory = true
                                              })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.SubDivisionName,
                                            "Subdivision Name",
                                            SessionUtil.UserCategory,
                                            new Dictionary<string, object> { { "maxLength", 50 }, { "id", "DefaultLocation_SubDivisionName" }, { "class", "autocComplete currentFieldValue" } },
                                            null,
                                            new FutureUpdate
                                              {
                                                FieldId = "DefaultLocation_SubDivisionName",
                                                FieldName = "DefaultLocation.SubDivisionName",
                                                FieldType = 4,
                                                CurrentValue = Model.DefaultLocation.SubDivisionName,
                                                FutureValue = Model.DefaultLocation.SubDivisionNameFutureValue,
                                                HasFuturePeriod = true,
                                                FuturePeriod = Model.DefaultLocation.SubDivisionNameFuturePeriod
                                              })%>
                                             
        </div>
        <%
       if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
       {%>
        <div>
            <h2>
                Bank Details for Bilateral Settlement</h2>
            <%
       }%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.BankAccountName,
                                            "Bank Account Name",
                                            SessionUtil.UserCategory,
                                                               new Dictionary<string, object> { { "maxLength", 80 } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.BankAccountNumber,
                                            "Bank Account Number",
                                            SessionUtil.UserCategory,
                                                                new Dictionary<string, object> {{"class", "alphaNumeric" }, { "maxLength", "80" }, { "style", "width:180px" } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.BankName, "Bank Name", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", 80 } })%>
            <%
       if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
       {%>
        </div>
        <%
       }%>
        <div>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.BranchCode, "Branch Code", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", 80 } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.BankCode, "Bank Code", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", 80 } })%>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.CurrencyId, "Currency Code", SessionUtil.UserCategory)%>
        </div>
        <div>
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.Iban, "IBAN", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", 80 } })%>
            
            <%:Html.ProfileFieldFor(memberDetails => memberDetails.DefaultLocation.Swift, "SWIFT", SessionUtil.UserCategory, new Dictionary<string, object> { { "class", "alphaNumeric" }, { "maxLength", "11" }, { "style", "width:180px" } })%>
        </div>
        <div class="topLine" style="padding-bottom: 10px;">
            <div>
                <h2>
                    IS Contacts</h2>
                <%:Html.HiddenFor(memberDetails => memberDetails.ContactList, new { @id = "MemberDetailsContactList" })%>
                <a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile",selectedMemberId= Model.Id })%>','#divContactAssignmentSearchResult','MEMBER','<%:Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile" })%>');">
                    View/Edit</a>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
    <%if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
      { %>
    <input class="primaryButton ignoredirty" id="saveMemDetails" type="submit" value="Save Member Details" />
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Profile.CreateOrManageMemberAccess))
      { %>
    <input class="secondaryButton" id="clearButton" type="button" onclick="resetForm();"
        value="Clear" />
    <%} %>
    <%}%>    
    <%
     if ((Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Profile.CreateOrManageUsersAccess)) && ((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.SisOps)))
       {%>
    <input class="secondaryButton ignoredirty" id="createSuperUser" type="button" value="Create Super User"
        disabled="disabled" onclick="javascript:location.href = '<%:Url.Action("Register","Account",new {area = "", SuperUsercreation = 1, SelectedMemberId = Model.Id}) %>'" />
    <%
       }%>
    <%if ((SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) && (SessionUtil.UserCategory != Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps))
      {%>
    <div class="futureUpdatesLegend">
        Future Updates Pending</div>
    <%
      }%>
    <%} %>
</div>
<div class="clear">
</div>
<div id="divImageUpload" class="memberLogo-dialog">
    <% Html.RenderPartial("~/Areas/Profile/Views/Shared/MemberLogo.ascx", Model.Id);%></div>
<div id="divMemStatusHistory" class="hidden ichStatus-dialog">
</div>
<div id="divContactAssignmentSearchResult" class="contactAssignment hidden">
    <% Html.RenderPartial("SearchResultControl");%></div>
