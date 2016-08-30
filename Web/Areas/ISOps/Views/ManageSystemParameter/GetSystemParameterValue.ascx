<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.AdminSystem.SystemParameters>" %>
<%@ Import Namespace="System.ComponentModel" %>
<h2>
    <%:ViewData["SystemParameterText"].ToString()%></h2>
<% using (Html.BeginForm("UpdateSystemParam", "ManageSystemParameter", FormMethod.Post, new { id = "formid" }))
   { %>
<%: Html.AntiForgeryToken() %>
<%: Html.ValidationSummary(true) %>
<%
       if (ViewData["SystemParameterKey"].ToString() == "UIParameters")
       {
%>
<fieldset class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.UIParameters.GetType().GetProperty("DefaultPageSize").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div class="editor-label">
                <%:Html.LabelFor(model => model.UIParameters.DefaultPageSize)%>
                <%:
        Html.TextBoxFor(model => model.UIParameters.DefaultPageSize, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.UIParameters.GetType().GetProperty("PageSizeOptions").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div>
                <%:Html.LabelFor(model => model.UIParameters.PageSizeOptions)%>
                <%:Html.TextBoxFor(model => model.UIParameters.PageSizeOptions, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</fieldset>
<%} %>
<%
       if (ViewData["SystemParameterKey"].ToString() == "BVCDetails")
       {%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.BVCDetails.GetType().GetProperty("AIASLANoOfRecords").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:Html.LabelFor(model => model.BVCDetails.AIASLANoOfRecords)%>
                <%:
        Html.TextBoxFor(model => model.BVCDetails.AIASLANoOfRecords, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.BVCDetails.GetType().GetProperty("AIASLATimeInSeconds").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:Html.LabelFor(model => model.BVCDetails.AIASLATimeInSeconds)%>
                <%:
        Html.TextBoxFor(model => model.BVCDetails.AIASLATimeInSeconds, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.BVCDetails.GetType().GetProperty("MaxCouponRecordsPerVCF").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:Html.LabelFor(model => model.BVCDetails.MaxCouponRecordsPerVCF)%>
                <%:
        Html.TextBoxFor(model => model.BVCDetails.MaxCouponRecordsPerVCF, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<%if (ViewData["SystemParameterKey"].ToString() == "ICHDetails")
  {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("IchOpsEmail").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.IchOpsEmail)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.IchOpsEmail, new { Style="width:200px;"})%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("ManualControlOnIchLateSubmission").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.ManualControlOnIchLateSubmission)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.ManualControlOnIchLateSubmission, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("IchWebReportEncryptionKey").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.IchWebReportEncryptionKey)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.IchWebReportEncryptionKey, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("MaxNumberOfInvoicesInIchSettlementFile").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.MaxNumberOfInvoicesInIchSettlementFile)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.MaxNumberOfInvoicesInIchSettlementFile, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("MaxNumberOfRetriesToSendIchSettlementFile").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.MaxNumberOfRetriesToSendIchSettlementFile)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.MaxNumberOfRetriesToSendIchSettlementFile, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("SisGatewayServiceUserName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.SisGatewayServiceUserName)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.SisGatewayServiceUserName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("SisGatewayServiceUsersPassword").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.SisGatewayServiceUsersPassword)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.SisGatewayServiceUsersPassword, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ICHDetails.GetType().GetProperty("WebReportSingleSignOnURL").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ICHDetails.WebReportSingleSignOnURL)%>
                <%:
        Html.TextBoxFor(model => model.ICHDetails.WebReportSingleSignOnURL, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</div>
<%
  }

%>
<%if (ViewData["SystemParameterKey"].ToString() == "ACHDetails")
  {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ACHDetails.GetType().GetProperty("AchOpsEmail").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ACHDetails.AchOpsEmail)%>
                <%:
        Html.TextBoxFor(model => model.ACHDetails.AchOpsEmail , new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ACHDetails.GetType().GetProperty("ACHRecapOverviewContact").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ACHDetails.ACHRecapOverviewContact)%>
                <%:
        Html.TextBoxFor(model => model.ACHDetails.ACHRecapOverviewContact, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ACHDetails.GetType().GetProperty("ACHMissingSubmissionContact").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ACHDetails.ACHMissingSubmissionContact)%>
                <%:
        Html.TextBoxFor(model => model.ACHDetails.ACHMissingSubmissionContact, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.ACHDetails.GetType().GetProperty("ManualControlOnACHLateSubmission").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.ACHDetails.ManualControlOnACHLateSubmission)%>
                <%:
        Html.TextBoxFor(model => model.ACHDetails.ManualControlOnACHLateSubmission, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<%if (ViewData["SystemParameterKey"].ToString() == "SIS_OpsDetails")
  {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.SIS_OpsDetails.GetType().GetProperty("SisOpsEmail").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.SIS_OpsDetails.SisOpsEmail)%>
                <%:
        Html.TextBoxFor(model => model.SIS_OpsDetails.SisOpsEmail, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<%if (ViewData["SystemParameterKey"].ToString() == "General")
  {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("BillingYearToStartWith").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.BillingYearToStartWith)%>
                <%:
        Html.TextBoxFor(model => model.General.BillingYearToStartWith, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("DefaultExpiryDateforMessages").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.DefaultExpiryDateforMessages)%>
                <%:
        Html.TextBoxFor(model => model.General.DefaultExpiryDateforMessages, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("HtmlToPdfTimeoutInMilliSeconds").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 270px;">
                <%:
        Html.LabelFor(model => model.General.HtmlToPdfTimeoutInMilliSeconds)%>
                <%:
        Html.TextBoxFor(model => model.General.HtmlToPdfTimeoutInMilliSeconds, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("FtpFileUploadMaxAttempt").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.FtpFileUploadMaxAttempt)%>
                <%:
        Html.TextBoxFor(model => model.General.FtpFileUploadMaxAttempt, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("DefaultExpiryDaysforAnnoucements").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.DefaultExpiryDaysforAnnoucements)%>
                <%:
        Html.TextBoxFor(model => model.General.DefaultExpiryDaysforAnnoucements, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("MaxLoginAllowed").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.MaxLoginAllowed)%>
                <%:
        Html.TextBoxFor(model => model.General.MaxLoginAllowed, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("MemberContactMaxRowCount").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 270px;">
                <%:
        Html.LabelFor(model => model.General.MemberContactMaxRowCount)%>
                <%:
        Html.TextBoxFor(model => model.General.MemberContactMaxRowCount, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("SuppDocMaxFileSizeFTP").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.SuppDocMaxFileSizeFTP)%>
                <%:
        Html.TextBoxFor(model => model.General.SuppDocMaxFileSizeFTP, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("AllowedDefaultAttachmentExtensions").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.AllowedDefaultAttachmentExtensions)%>
                <%:
        Html.TextBoxFor(model => model.General.AllowedDefaultAttachmentExtensions, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("PathToSchemaFiles").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.PathToSchemaFiles)%>
                <%:
        Html.TextBoxFor(model => model.General.PathToSchemaFiles, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("LogOnURL").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 270px;">
                <%:
        Html.LabelFor(model => model.General.LogOnURL)%>
                <%:
        Html.TextBoxFor(model => model.General.LogOnURL, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("SFRRootBasePath").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.SFRRootBasePath)%>
                <%:
        Html.TextBoxFor(model => model.General.SFRRootBasePath, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("FtpRootBasePath").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.FtpRootBasePath)%>
                <%:
        Html.TextBoxFor(model => model.General.FtpRootBasePath, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("IgnoreValidationOnMigrationPeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.IgnoreValidationOnMigrationPeriod)%>
                <%:
        Html.TextBoxFor(model => model.General.IgnoreValidationOnMigrationPeriod, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("MemberLogoFileLocation").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 270px;">
                <%:
        Html.LabelFor(model => model.General.MemberLogoFileLocation)%>
                <%:
        Html.TextBoxFor(model => model.General.MemberLogoFileLocation, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("YmqTimeZoneName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.YmqTimeZoneName)%>
                <%:
        Html.TextBoxFor(model => model.General.YmqTimeZoneName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("TempInvoiceOutputFiles").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.TempInvoiceOutputFiles)%>
                <%:
        Html.TextBoxFor(model => model.General.TempInvoiceOutputFiles, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("IsMultilingualAllowed").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.General.IsMultilingualAllowed)%>
                <%:
        Html.CheckBoxFor(model => model.General.IsMultilingualAllowed)%>
            </div>
            <%
              }%>
            <%--CMP#622: MISC Outputs Split as per Location IDs--%>
            <%if (!string.IsNullOrWhiteSpace(Model.General.GetType().GetProperty("CreateNilFileLocationSpecificMISCOutputs").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 280px;">
                <%:
        Html.LabelFor(model => model.General.CreateNilFileLocationSpecificMISCOutputs)%>
                <%:
        Html.TextBoxFor(model => model.General.CreateNilFileLocationSpecificMISCOutputs, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
        </div>
        <div>
            <br />
        </div>
        <div>
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<%
       if (ViewData["SystemParameterKey"].ToString() == "Atpco")
       {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("ServerName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Atpco.ServerName)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.ServerName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("UserName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Atpco.UserName)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.UserName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("Password").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Atpco.Password)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.Password, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("Port").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Atpco.Port)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.Port, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("Security").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Atpco.Security)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.Security, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("ATPCOCode").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Atpco.ATPCOCode)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.ATPCOCode, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.Atpco.GetType().GetProperty("ApplicationMode").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 300px;">
                <%:
        Html.LabelFor(model => model.Atpco.ApplicationMode)%>
                <%:
        Html.TextBoxFor(model => model.Atpco.ApplicationMode, new { Style = "width:200px;" })%>
                (PROD/TEST)
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<%if (ViewData["SystemParameterKey"].ToString() == "Ach")
  {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.Ach.GetType().GetProperty("ServerName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Ach.ServerName)%>
                <%:
        Html.TextBoxFor(model => model.Ach.ServerName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Ach.GetType().GetProperty("UserName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Ach.UserName)%>
                <%:
        Html.TextBoxFor(model => model.Ach.UserName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Ach.GetType().GetProperty("Password").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Ach.Password)%>
                <%:
        Html.TextBoxFor(model => model.Ach.Password, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.Ach.GetType().GetProperty("Port").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Ach.Port)%>
                <%:
        Html.TextBoxFor(model => model.Ach.Port, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Ach.GetType().GetProperty("Security").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Ach.Security)%>
                <%:
        Html.TextBoxFor(model => model.Ach.Security, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.Ach.GetType().GetProperty("FtpWorkingDirectory").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.Ach.FtpWorkingDirectory)%>
                <%:
        Html.TextBoxFor(model => model.Ach.FtpWorkingDirectory, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<%if (ViewData["SystemParameterKey"].ToString() == "iiNet")
  {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("ServerName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.ServerName)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.ServerName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("UserName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.UserName)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.UserName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Password").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Password)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Password)%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Port").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Port)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Port, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Security").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Security)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Security, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("iiNetFolderName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.iiNetFolderName)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.iiNetFolderName, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Description").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Description)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Description, new { Style="width:180px;"})%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("FileType").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.FileType)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.FileType, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Sender").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Sender)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Sender, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Service").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Service)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Service, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
            <%if (!string.IsNullOrWhiteSpace(Model.iiNet.GetType().GetProperty("Signature").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.iiNet.Signature)%>
                <%:
        Html.TextBoxFor(model => model.iiNet.Signature, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
        <div>
            <br />
        </div>
    </div>
    <div class="clear" />
</div>
<%} %>
<% if (ViewData["SystemParameterKey"].ToString() == "IataDetails")
   {%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.IataDetails.GetType().GetProperty("IataContactEmail").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.IataDetails.IataContactEmail)%>
                <%:
        Html.TextBoxFor(model => model.IataDetails.IataContactEmail, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
        </div>
    </div>
    <div class="clear" />
    <br />
</div>
<%} %>
<%
       if (ViewData["SystemParameterKey"].ToString() == "LegalArchivingDetails")
       {
%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("CDCArkhineoCoffreID").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.CDCArkhineoCoffreID)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.CDCArkhineoCoffreID, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("CDCArkhineoSectionID").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.CDCArkhineoSectionID)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.CDCArkhineoSectionID, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
        </div>
        <div>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("CDCArkhineoClientIDofIATA").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.CDCArkhineoClientIDofIATA)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.CDCArkhineoClientIDofIATA, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("CDCArkhineoLoginID").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.CDCArkhineoLoginID)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.CDCArkhineoLoginID, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("CDCArkhineoPassword").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.CDCArkhineoPassword)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.CDCArkhineoPassword, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
        </div>
        <div>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("ArchiveDurationinYears").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.ArchiveDurationinYears)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.ArchiveDurationinYears, new { Style = "width:200px;", maxLength = 2, max = 99, min = 0, @class = "digits integer" })%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("ExpectedARFResponseTimeinHours").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.ExpectedARFResponseTimeinHours)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.ExpectedARFResponseTimeinHours, new { Style = "width:200px;", maxLength = 2, max = 99, min = 0, @class = "digits integer" })%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("RetentionPeriodofRetrievedLAJobsinDays").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 300px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.RetentionPeriodofRetrievedLAJobsinDays)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.RetentionPeriodofRetrievedLAJobsinDays, new { Style = "width:200px;", maxLength = 2, max = 60, min = 0, @class = "digits integer" })%>
            </div>
            <%
           }%>
        </div>
        <div>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("CDCArkhineoBaseURL").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.CDCArkhineoBaseURL)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.CDCArkhineoBaseURL, new { Style = "width:200px;", maxLength = 100})%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("ArchieveRetrievalUserName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.ArchieveRetrievalUserName)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.ArchieveRetrievalUserName, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
            <%
           if (!string.IsNullOrWhiteSpace(Model.LegalArchivingDetails.GetType().GetProperty("ArchieveRetrievalPassword").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
           {%>
            <div style="width: 250px;">
                <%:
        Html.LabelFor(model => model.LegalArchivingDetails.ArchieveRetrievalPassword)%>
                <%:
        Html.TextBoxFor(model => model.LegalArchivingDetails.ArchieveRetrievalPassword, new { Style = "width:200px;", maxLength = 100 })%>
            </div>
            <%
           }%>
        </div>
    </div>
    <div class="clear" />
    <br />
</div>
<%} %>
<% if (ViewData["SystemParameterKey"].ToString() == "PurgingPeriodDetails")
   {%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("InputDataFilesPurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.InputDataFilesPurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.InputDataFilesPurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("OutputDataFilesPurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.OutputDataFilesPurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.OutputDataFilesPurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
        </div>
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("UnlinkedSupportDocFilesPurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.UnlinkedSupportDocFilesPurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.UnlinkedSupportDocFilesPurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("TemporaryFilesPurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.TemporaryFilesPurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.TemporaryFilesPurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
        </div>
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("LegalArchievePurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.LegalArchievePurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.LegalArchievePurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("LogFilesPurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.LogFilesPurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.LogFilesPurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
        </div>
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("LogFilePath").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.LogFilePath)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.LogFilePath, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("CorrReportFilesPurgingPeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.CorrReportFilesPurgingPeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.CorrReportFilesPurgingPeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
        </div>
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("ErrorInvoicePurgePeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.PurgingPeriodDetails.ErrorInvoicePurgePeriod)%>
                <%:
               Html.TextBoxFor(model => model.PurgingPeriodDetails.ErrorInvoicePurgePeriod, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
            <%--CMP529 : Daily Output Generation for MISC Bilateral Invoices--%>
            <%if (!string.IsNullOrWhiteSpace(Model.PurgingPeriodDetails.GetType().GetProperty("DailyMiscBilateralFilesPurgingPeriod").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 300px;">
                <%:
        Html.LabelFor(model => model.PurgingPeriodDetails.DailyMiscBilateralFilesPurgingPeriod)%>
                <%:
        Html.TextBoxFor(model => model.PurgingPeriodDetails.DailyMiscBilateralFilesPurgingPeriod, new { Style = "width:200px;" })%>
            </div>
            <%
              }%>
        </div>
    </div>
    <div class="clear" />
    <br />
</div>
<%} %>
<% if (ViewData["SystemParameterKey"].ToString() == "AutoBilling")
   {%>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.AutoBilling.GetType().GetProperty("UsageDataExpRespHours").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.AutoBilling.UsageDataExpRespHours)%>
                <%:
               Html.TextBoxFor(model => model.AutoBilling.UsageDataExpRespHours, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
            <%
       if (!string.IsNullOrWhiteSpace(Model.AutoBilling.GetType().GetProperty("InvoiceRangeThresholdValue").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.AutoBilling.InvoiceRangeThresholdValue)%>
                <%:
               Html.TextBoxFor(model => model.AutoBilling.InvoiceRangeThresholdValue, new { Style = "width:200px;" })%>
            </div>
            <%
       }%>
        </div>
    </div>
    <div class="clear" />
    <br />
</div>
<%} %>
<!--CMP496: Validation Parameters controls are displayed here  -->
<% if (ViewData["SystemParameterKey"].ToString() == "ValidationParams")
   {%>
<div class="solidBox dataEntry">
    <h2>
        Outcome of Mismatch on Billed Member Reference Data field</h2>
    <div class="fieldContainer horizontalFlow">
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("MemberLegalName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.MemberLegalName)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.MemberLegalName, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("TaxVATRegistration").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.TaxVATRegistration)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.TaxVATRegistration, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("AddTaxVATRegistration").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.AddTaxVATRegistration)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.AddTaxVATRegistration, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("CompanyRegistrationID").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.CompanyRegistrationID)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.CompanyRegistrationID, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("AddressLine1").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.AddressLine1)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.AddressLine1, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("AddressLine2").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.AddressLine2)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.AddressLine2, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("AddressLine3").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.AddressLine3)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.AddressLine3, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("CityName").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.CityName)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.CityName, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("CountryCode").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.CountryCode)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.CountryCode, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("PostalCode").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.PostalCode)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.PostalCode, new { Style = "width:80px;" })%>
            </div>
            <%} %>
        </div>
    </div>
    <div class="clear" />
    <br />
    <h2>
        Outcome of Mismatch on PAX and CGO RM Billed/Allowed Amounts</h2>
    <div class="fieldContainer horizontalFlow">
        <div>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("PAXRMBilledAllowedAmounts").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.PAXRMBilledAllowedAmounts)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.PAXRMBilledAllowedAmounts, new { Style = "width:80px;" })%>
            </div>
            <%} %>
            <%
       if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("CGORMBilledAllowedAmounts").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
       {%>
            <div style="width: 250px;">
                <%:
               Html.LabelFor(model => model.ValidationParams.CGORMBilledAllowedAmounts)%>
                <%:
               Html.ValidationParamDropdownListFor(model => model.ValidationParams.CGORMBilledAllowedAmounts, new { Style = "width:80px;" })%>
            </div>
            <%
       }%>
        </div>
    </div>
    <%--CMP #534: Tax Issues in MISC and UATP Invoices. [Start]--%>
    <%--To display the dropdown for Outcome of Mismatch on TaxSubType of Tax for MISC/UATP--%>
    <div class="clear" />
    <h2>
        Outcome of Mismatch on TaxSubType of Tax for MISC/UATP</h2>
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("TaxSubTypeOfTaxForMiscUatp").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:Html.LabelFor(model => model.ValidationParams.TaxSubTypeOfTaxForMiscUatp)%>
                <%:Html.ValidationParamDropdownListFor(model => model.ValidationParams.TaxSubTypeOfTaxForMiscUatp, new { Style = "width:80px;" })%>
            </div>
            <%}%>
        </div>
    </div>
    <%--CMP #534: Tax Issues in MISC and UATP Invoices. [End]--%>
    <div class="clear" />
    <%--CMP614: Source Code Validation for PAX RMs.--%>
    <h2>
        Outcome of Mismatch on PAX RM Source Codes</h2>
    <div class="fieldContainer horizontalFlow">
        <div>
            <%if (!string.IsNullOrWhiteSpace(Model.ValidationParams.GetType().GetProperty("PaxRMSourceCodes").GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().FirstOrDefault().DisplayName))
              {%>
            <div style="width: 250px;">
                <%:Html.LabelFor(model => model.ValidationParams.PaxRMSourceCodes)%>
                <%:Html.ValidationParamDropdownListFor(model => model.ValidationParams.PaxRMSourceCodes, new { Style = "width:80px;" })%>
            </div>
            <%}%>
        </div>
    </div>
    <div class="clear" />
    <br />
</div>
<%} %>
<div class="buttonContainer">
    <input id="saveParameter" class="primaryButton ignoredirty" type="submit" value="Save" />
    <input class="secondaryButton" type="button" value="Cancel" onclick="CancelProcessing();" />
</div>
<%} %>