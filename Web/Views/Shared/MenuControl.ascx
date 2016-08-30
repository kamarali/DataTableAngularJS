<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<int>>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<script type="text/javascript">
  function notImplemented() {
    alert("This is not yet implemented.");
  }
</script>
<div class="sf-menu-container">
  <ul id="sample-menu-1" class="sf-menu">
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.Passenger, Model))
       {	%>
    <li><a href="#">Passenger</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.PaxReceivables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Receivables</a>
          <ul>
            <%: Html.MenuItem("Manage Invoice", "Index", "ManageInvoice", "Pax", true, Iata.IS.Business.Security.Permissions.Menu.PaxRecManage, userPermissions: Model)%>
            <%: Html.MenuItem("Create Non-Sampling Invoice", "Create", "Invoice", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecNonSampleCreateInvoice, userPermissions: Model)%>
            <%: Html.MenuItem("Create Non-Sampling Credit Note", "Create", "CreditNote", "Pax", true, Iata.IS.Business.Security.Permissions.Menu.PaxRecNonSampleCreateCreditNote, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Sampling Form C", "Index", "FormC", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecManageFormC, userPermissions: Model)%>
            <%: Html.MenuItem("Create Sampling Form C", "Create", "FormC", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecSampleCreateFormC, userPermissions: Model)%>
            <%: Html.MenuItem("Create Sampling Form D/E", "Create", "FormDE", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecSampleCreateFormDE, userPermissions: Model)%>
            <%: Html.MenuItem("Create Sampling Form F", "Create", "FormF", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecSampleCreateFormF, userPermissions: Model)%>
            <%: Html.MenuItem("Create Sampling Form XF", "Create", "FormXF", "Pax", true, Iata.IS.Business.Security.Permissions.Menu.PaxRecSampleCreateFormXF, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "Index", "SupportingDoc", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecManageSupportingDocuments, userPermissions: Model)%>
            <%: Html.MenuItem("Correct Supporting Document Linking Errors", "Index", "UnlinkedSupportingDocument", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecCorrectSupportingDocumentsLinkingErrors, userPermissions: Model)%>

              <%: Html.MenuItem("Validation Error Correction", "Index", "PaxValidationErrorCorrection", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecValidationErrorCorrection, userPermissions: Model)%>
            <%: Html.MenuItem("Correct AutoBilling Invoices", "Index", "AutoBilling", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxRecCorrectAutoBillingInvoices, userPermissions: Model) %>
          </ul>
        </li>
        <% } %>
        <%: Html.MenuItem("Billing History and Correspondence", "Index", "BillingHistory", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxViewBillingHistoryAndCorrespondence, userPermissions: Model)%>      

        
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.PaxPayables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Payables</a>
          <ul>
            <%: Html.MenuItem("Invoice Search", "Index", "ManagePaxPayablesInvoice", "PaxPayables", true, Iata.IS.Business.Security.Permissions.Menu.PaxPaySearch, userPermissions: Model)%>
            <%: Html.MenuItem("Sampling Form C Search", "PayablesSearch", "FormCPayables", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxPaySearchSampleFormC, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "PayableSupportingDocs", "SupportingDoc", "PaxPayables", false, Iata.IS.Business.Security.Permissions.Menu.PaxPayViewSupportingDocuments, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
         <%--SCP#447047: Correspondences--%>
         <%: Html.MenuItem("Download Correspondences", "Index", "CorrespondenceTrail", "Pax", false, Iata.IS.Business.Security.Permissions.Menu.PaxDownloadCorrespondences, userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <%--Cargo--%>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.Cargo, Model))
       {	%>
    <li><a href="#">Cargo</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.CgoReceivables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Receivables</a>
          <ul>
            <%: Html.MenuItem("Manage Invoice", "Index", "CargoManageInvoice", "Cargo", true, Iata.IS.Business.Security.Permissions.Menu.CgoRecManage, userPermissions: Model)%>
            <%: Html.MenuItem("Create Invoice", "Create", "Invoice", "Cargo", false, Iata.IS.Business.Security.Permissions.Menu.CgoRecCreateInvoice, userPermissions: Model)%>
            <%: Html.MenuItem("Create Credit Note", "Create", "CreditNote", "Cargo", true, Iata.IS.Business.Security.Permissions.Menu.CgoRecCreateCreditNote, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "Index", "CgoSupportingDoc", "Cargo", false, Iata.IS.Business.Security.Permissions.Menu.CgoRecManageSupportingDocuments, userPermissions: Model)%>
            <%: Html.MenuItem("Validation Error Correction", "Index", "ValidationErrorCorrection", "Cargo", false, Iata.IS.Business.Security.Permissions.Menu.CgoRecvalidationErrorCorrection, userPermissions: Model)%>
            <%: Html.MenuItem("Correct Supporting Document Linking Errors", "Index", "UnlinkedSupportingDocument", "Cargo", false, Iata.IS.Business.Security.Permissions.Menu.CgoRecCorrectSupportingDocumentsLinkingErrors, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <%: Html.MenuItem("Billing History and Correspondence", "Index", "BillingHistory", "Cargo", false, Iata.IS.Business.Security.Permissions.Menu.CgoViewBillingHistoryAndCorrespondence, userPermissions: Model)%>       
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.CgoPayables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Payables</a>
          <ul>
            <%: Html.MenuItem("Invoice Search", "Index", "PayablesInvoiceSearch", "CargoPayables", true, Iata.IS.Business.Security.Permissions.Menu.CgoPaySearch, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "PayableSupportingDocs", "CargoSupportingDoc", "CargoPayables", false, Iata.IS.Business.Security.Permissions.Menu.CgoPayViewSupportingDocuments, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <%--SCP#447047: Correspondences--%>
        <%: Html.MenuItem("Download Correspondences", "Index", "CorrespondenceTrail", "Cargo", false, Iata.IS.Business.Security.Permissions.Menu.CgoDownloadCorrespondences, userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <%--End Cargo Menu--%>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.Miscellaneous, Model))
       {	%>
    <li><a href="#">Miscellaneous</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.MiscReceivables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Receivables</a>
          <ul>
            <%: Html.MenuItem("Manage Invoice", "Index", "ManageMiscInvoice", "Misc", true, Iata.IS.Business.Security.Permissions.Menu.MiscRecManageInvoices, userPermissions: Model)%>
            <%-- SCP401669: Misc Permissions Issue--%>
            <%: Html.MenuItem("Create Invoice", "Create", "MiscInvoice", "Misc", false, Iata.IS.Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit, userPermissions: Model)%>
            <%: Html.MenuItem("Create Credit Note", "Create", "MiscCreditNote", "Misc", true, Iata.IS.Business.Security.Permissions.Misc.Receivables.CreditNote.CreateOrEdit, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "Index", "MiscSupportingDoc", "Misc", false, Iata.IS.Business.Security.Permissions.Menu.MiscRecManageSupportingDocuments, userPermissions: Model)%>
            <%: Html.MenuItem("Correct Supporting Document Linking Errors", "Index", "UnlinkedSupportingDocument", "Misc", false, Iata.IS.Business.Security.Permissions.Menu.MiscRecCorrectSupportingDocumentsLinkingErrors, userPermissions: Model)%>
            <%-- SCP401669: Misc Permissions Issue--%>
            <%: Html.MenuItem("Validation Error Correction", "Index", "MiscValidationErrorCorrection", "Misc", true,Iata.IS.Business.Security.Permissions.Menu.MiscRecValidationErrorCorrection, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <%: Html.MenuItem("Billing History and Correspondence", "Index", "BillingHistory", "Misc", false, Iata.IS.Business.Security.Permissions.Menu.MiscViewBillingHistoryAndCorrespondence, userPermissions: Model)%>        
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.MiscPayables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Payables</a>
          <ul>
            <%: Html.MenuItem("Invoice Search", "Index", "ManageMiscPayablesInvoice", "MiscPayables", true, Iata.IS.Business.Security.Permissions.Menu.MiscPaySearch, userPermissions: Model)%>
            <%--CMP529 : Daily Output Generation for MISC Bilateral Invoices--%>
            <%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions--%>
            <%: Html.MenuItem("View Daily Bilateral Invoices", "Index", "ManageMiscDailyPayablesInvoice", "MiscPayables", true, Iata.IS.Business.Security.Permissions.Menu.MiscPayDailyBilateralDelivery, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "PayableSupportingDocs", "MiscPayableSupportingDoc", "MiscPayables", false, Iata.IS.Business.Security.Permissions.Menu.MiscPayViewSupportingDocuments, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
         <%--SCP#447047: Correspondences--%>
         <%: Html.MenuItem("Download Correspondences", "Index", "CorrespondenceTrail", "Misc", false, Iata.IS.Business.Security.Permissions.Menu.MiscDownloadCorrespondences, userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.UATP, Model))
       {	%>
    <li><a href="#">UATP</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.UATPReceivables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Receivables</a>
          <ul>
            <%: Html.MenuItem("Manage Invoice", "Index", "ManageUatpInvoice", "Uatp", true, Iata.IS.Business.Security.Permissions.Menu.UATPRecManageInvoices, userPermissions: Model)%>
            <%--<%: Html.MenuItem("Create Invoice", "Create", "UatpInvoice", "Uatp",  false, Iata.IS.Business.Security.Permissions.Menu.UATPRecCreateInvoice, userPermissions: Model)%>
            <%: Html.MenuItem("Create Credit Note", "Create", "UatpCreditNote", "Uatp", true, Iata.IS.Business.Security.Permissions.Menu.UATPRecCreateCreditNote, userPermissions: Model)%>--%>
            <%: Html.MenuItem("Manage Supporting Documents", "Index", "UatpSupportingDoc", "Uatp", false, Iata.IS.Business.Security.Permissions.Menu.UATPRecManageSupportingDocuments, userPermissions: Model)%>
            <%: Html.MenuItem("Correct Supporting Document Linking Errors", "Index", "UnlinkedSupportingDocument", "Uatp", false, Iata.IS.Business.Security.Permissions.Menu.UATPRecCorrectSupportingDocumentsLinkingErrors, userPermissions: Model)%>
            <%: Html.MenuItem("Validation Error Correction", "Index", "UatpValidationErrorCorrection", "Uatp",  true,Iata.IS.Business.Security.Permissions.Menu.UATPRecValidationErrorCorrection, userPermissions: Model)%>
            
          </ul>
        </li>
        <% } %>
        <%: Html.MenuItem("Billing History and Correspondence", "Index", "BillingHistory", "Uatp",  false, Iata.IS.Business.Security.Permissions.Menu.UATPViewBillingHistoryAndCorrespondence, userPermissions: Model)%>      
        <%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions--%>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.UATPPayables, Model))
           {	%>
        <li><a href="#" class="parentMenu">Payables</a>
          <ul>
            <%: Html.MenuItem("Invoice Search", "Index", "ManageUatpPayablesInvoice", "UatpPayables", true, Iata.IS.Business.Security.Permissions.Menu.UATPPaySearch, userPermissions: Model)%>
            <%: Html.MenuItem("Manage Supporting Documents", "PayableSupportingDocs", "UatpPayablesSupportingDoc", "UatpPayables", false, Iata.IS.Business.Security.Permissions.Menu.UATPPayViewSupportingDocuments, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
          <%--SCP#447047: Correspondences--%>
          <%: Html.MenuItem("Download Correspondences", "Index", "CorrespondenceTrail", "Uatp", false, Iata.IS.Business.Security.Permissions.Menu.UATPDownloadCorrespondences, userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.Reports, Model))
          {	%>
    <li><a href="#">Reports</a>
      <ul>
        <%: Html.MenuItem("Processing Dashboard", "ISProcessingDashboard", "ProcessingDashboard", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepProcessingDashboard, userPermissions: Model)%>
        <%: Html.MenuItem("SIS Usage Report", "SISUsageReport", "SisUsageReport", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepSisUsageReport, userPermissions: Model)%>
        <%--CMP #659: SIS IS-WEB Usage Report.--%>
        <% if (SessionUtil.MemberId != 0)
            {
        %>
        <%:Html.MenuItem("SIS IS-WEB Usage Report", "SisIsWebUsageReport", "SisUsageReport", "Reports", true,
                                Iata.IS.Business.Security.Permissions.Menu.RepSisUsageReport, userPermissions: Model)%>
        <% } %>
        <%: Html.MenuItem("Member/Contact Report", "MemberDetails", "QueryAndDownloadDetails", "Profile", true, Iata.IS.Business.Security.Permissions.Menu.RepMemberContactReports, userPermissions: Model)%>
        <%: Html.MenuItem("User Permission Report", "UserPermissionReport", "UserPermissionReport", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepUserPermissionReport, userPermissions: Model)%>
        <%: Html.MenuItem("IS and CH Calendar Report", "IsCalendar", "IsCalendar", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepISCHCalendarReport, userPermissions: Model)%>
        <%: Html.MenuItem("Invoice Deletion Audit Trail Report", "InvoiceDeletionAudit", "InvoiceDeletionAudit", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepInvoceDeletion, userPermissions: Model)%>
        <%: Html.MenuItem("Invoice Reference Data", "MemberLocation", "MemberLocation", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepInvoiceReferenceData, userPermissions: Model)%>
        <%-- <%: Html.MenuItem("Interline Billing Summary Dashboard Report", "InterlineBillingSummaryDashBoardReport","InterlineBillingSummaryDashboard","Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepISCHCalendarReport, userPermissions: Model)%> --%>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.RepFinancialController, Model))
           {	%>
        <li><a href="#" class="parentMenu">Financial Controller</a>
          <ul>
            <%: Html.MenuItem("Interline Billing Summary", "InterlineBillingSummaryReport", "InterlinePayablesAnalysis", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlInterlineBillingsummary, userPermissions: Model)%>
            <%: Html.MenuItem("Interline Payables Analysis", "InterlinePayablesAnalysis", "InterlinePayablesAnalysis", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlInterlinePayablesAnalysis, userPermissions: Model)%>
            <%: Html.MenuItem("Suspended Billings", "MemberSuspendedInvoices", "ManageSuspendedInvoices", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlSuspendedBillings, userPermissions: Model)%>
            <%: Html.MenuItem("Pending Invoices In Error", "PendingInvoicesReport", "PendingInvoices", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlPendingErrorInvoices, userPermissions: Model)%>
            <%: Html.MenuItem("Top 10 Interline Partner - Receivables", "PaxCgoMscTopTenPartnerRec", "PaxCgoMscTopTenPartner", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlTop10Receivables, userPermissions: Model)%>
            <%: Html.MenuItem("Top 10 Interline Partner - Payables", "PaxCgoMscTopTenPartnerPay", "PaxCgoMscTopTenPartner", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlTop10Payables, userPermissions: Model)%>
            <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlAccessIchWebReports, Model))
               {	%>
            <li>
              <%: Html.ActionLink("Access ICH Reports", "RedirectToIch", "Account", null, new { target = "_blank", @class = "menuItem" })%>
            </li>
            <%}
               /* CMP #645: Access ACH Settlement Reports*/ 
              if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.RepFinCtrlAccessAchSettlementReports, Model))
               {	%>  
                    <li>
                        <%: Html.ActionLink("Access ACH Settlement Reports", "RedirectToAch", "Account", null, new { target = "_blank", @class = "menuItem" })%>              
                    </li>
             <%}%>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.RepPassenger, Model))
           {	%>
        <li><a href="#" class="parentMenu">Passenger</a>
          <ul>
            <li><a href="#" class="parentMenu">Receivables</a>
              <ul>
                <%: Html.MenuItem("Interline Billing Summary", "PaxInterlineBillingSummaryReport", "ReceivablesReport","Reports",true, Iata.IS.Business.Security.Permissions.Menu.RepPaxReceivablesInterlineBillSummary, userPermissions: Model)%>
                <%: Html.MenuItem("Non Sample Rejection Analysis", "PaxRejectionAnalysisNonSamplingReport", "ReceivablesReport", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepPaxReceivablesNonSampleRejnAnalysis, userPermissions: Model)%>
                <%: Html.MenuItem("Sampling Billing Analysis", "OwSamplingReport", "ReceivablesReport", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepPaxReceivablesSamplingBillingAnalysis, userPermissions: Model)%>
                <%: Html.MenuItem("RM BM CM Summary", "ReceivablesReport", "ReceivablesReport", "Reports", false, Iata.IS.Business.Security.Permissions.Menu.RepPaxReceivablesRMBMCMSummary, userPermissions: Model)%>
                <%: Html.MenuItem("Supporting Documents Mismatch", "MismatchDocument", "SupportingMismatchDocument", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepPaxReceivablesSupportingDocMismatch, userPermissions: Model)%>
                <%: Html.MenuItem("Auto Billing Performance", "AutoBillingPerformance", "AutoBillingPerformance", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepPaxReceivablesInterlineBillSummary, userPermissions: Model)%>
              </ul>
            </li>
            <li><a href="#" class="parentMenu">Payables</a>
              <ul>
                <%: Html.MenuItem("Interline Billing Summary", "PaxInterlineBillingSummaryReport", "PayablesReport","Reports",false, Iata.IS.Business.Security.Permissions.Menu.RepPaxPayablesInterlineBillSummary, userPermissions: Model)%>
                <%: Html.MenuItem("Non Sample Rejection Analysis", "PaxRejectionAnalysisNonSamplingReport", "PayablesReport", "Reports", false, Iata.IS.Business.Security.Permissions.Menu.RepPaxPayablesNonSampleRejnAnalysis, userPermissions: Model)%>
                <%: Html.MenuItem("Sampling Billing Analysis", "IwSamplingReport", "PayablesReport", "Reports", true, Iata.IS.Business.Security.Permissions.Menu.RepPaxPayablesSamplingBillingAnalysis, userPermissions: Model)%>
                <%: Html.MenuItem("RM BM CM Summary", "PayablesReport", "PayablesReport", "Reports", false, Iata.IS.Business.Security.Permissions.Menu.RepPaxPayablesRMBMCMSummary, userPermissions: Model)%>
              </ul>
            </li>
            <%: Html.MenuItem("Correspondence Status", "PaxCorrespondenceStatus", "CorrespondenceStatus", "Reports", false, Iata.IS.Business.Security.Permissions.Menu.RepPaxCorrespondenceStatus, userPermissions: Model)%>
            <%: Html.MenuItem("BVC Details", "ConfirmationDetail", "ConfirmationDetail", "Reports", false, Iata.IS.Business.Security.Permissions.Menu.RepPaxBvcDetails, userPermissions: Model)%>
          </ul>
        </li>
        <%} %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.RepCargo, Model))
           {	%>
        <li><a href="#" class="parentMenu">Cargo</a>
          <ul>
            <li><a href="#" class="parentMenu">Receivables</a>
              <ul>
                <%:Html.MenuItem("Submission Overview", "Index", "ReceivableCargoSubmissionOverview",
                                               "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoReceivablesSubmissionOverview, userPermissions: Model)%>
                <%:Html.MenuItem("Interline Billing Summary", "ReceivablesReport",
                                               "CargoInterlineBillingSummary", "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoReceivablesInterlineBillSummary, userPermissions: Model)%>
                <%:Html.MenuItem("Rejection Analysis", "CgoRejectionAnalysisRec", "RejectionAnalysisRec",
                                               "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoReceivablesRejnAnalysis, userPermissions: Model)%>
                <%:Html.MenuItem("RM BM CM Summary", "CargoReceivablesReport", "RMBMCMSummaryReport",
                                               "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoReceivablesRMBMCMSummary, userPermissions: Model)%>                
                <%:Html.MenuItem("Supporting Documents Mismatch", "CgoMismatchDocument",
                                               "SupportingMismatchDocument", "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoReceivablesSupportingDocMismatch, userPermissions: Model)%>
              </ul>
            </li>
            <li><a href="#" class="parentMenu">Payables</a>
              <ul>
                <%:Html.MenuItem("Submission Overview", "PayableCargoSubmissionOverview",
                                               "ReceivableCargoSubmissionOverview", "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoPayablesSubmissionOverview, userPermissions: Model)%>
                <%:Html.MenuItem("Interline Billing Summary", "PayablesReport",
                                               "CargoInterlineBillingSummary", "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.
                                                   RepCargoPayablesInterlineBillSummary, userPermissions: Model)%>
                <%:Html.MenuItem("Rejection Analysis", "CgoRejectionAnalysisPay", "RejectionAnalysisRec",
                                               "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.RepCargoPayablesRejnAnalysis,
                                               userPermissions: Model)%>
                <%:Html.MenuItem("RM BM CM Summary", "CargoPayablesReport", "RMBMCMSummaryReport",
                                               "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.RepCargoPayablesRMBMCMSummary,
                                               userPermissions: Model)%>
              </ul>
            </li>
            <%:Html.MenuItem("RM BM CM Details Report", "Index", "RMBMCMDetails", "Reports", true,
                                               Iata.IS.Business.Security.Permissions.Menu.RepCargoRMBMCMDetails,
                                               userPermissions: Model)%>
            <%:Html.MenuItem("Correspondence Status", "CGOCorrespondenceStatus",
                                               "CorrespondenceStatus", "Reports", false,
                                               Iata.IS.Business.Security.Permissions.Menu.RepCargoCorrespondenceStatus,
                                               userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.RepMiscellaneous, Model))
{	%>
        <li><a href="#" class="parentMenu">Miscellaneous</a>
          <ul>
            <%:Html.MenuItem("Substitution Values Report", "MiscSubstitutionValuesReport", "Miscellaneous",
                                    "Reports", true,
                                    Iata.IS.Business.Security.Permissions.Menu.RepMiscSubstitutionValues,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Receivables Supporting Documents Mismatch", "MiscMismatchDocument", "SupportingMismatchDocument",
                                    "Reports", true,
                                    Iata.IS.Business.Security.Permissions.Menu.RepMiscReceivablesSupportingDocMismatch,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Receivable Invoice Summary Report", "MiscChargeSummary", "Miscellaneous", "Reports",
                                    true,Iata.IS.Business.Security.Permissions.Menu.RepMiscRecInvSummary,
                                        userPermissions: Model)%>
            <%:Html.MenuItem("Payables Invoice Summary Report ", "MiscChargePaySummary", "Miscellaneous",
                                        "Reports", true, 
                                        Iata.IS.Business.Security.Permissions.Menu.RepMiscPayChargeSummary,
                                         userPermissions: Model)%>
            <%:Html.MenuItem("Correspondence Status", "MiscCorrespondenceStatus", "CorrespondenceStatus",
                                    "Reports", false,
                                    Iata.IS.Business.Security.Permissions.Menu.RepMiscCorrespondenceStatus,
                                    userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
            <% if (SessionUtil.MemberId != 0)
           {	%>
        <%: Html.MenuItem("Download Offline Reports","Index","OfflineReports","Reports",true)%>
          <% } %>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.General, Model))
{	%>
    <li><a href="#">General</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.GenFileManagement, Model))
{	%>
        <li><a href="#">File Management</a>
          <ul>
            <%:Html.MenuItem("Upload File", "FileManagerUpload", "FileViaWeb", "General", false,
                                    Iata.IS.Business.Security.Permissions.Menu.GenFileManagementUploadFile,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Download File", "FileManagerDownload", "FileViaWeb", "General", false,
                                    Iata.IS.Business.Security.Permissions.Menu.GenFileManagementDownloadFile,
                                    userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <%:Html.MenuItem("Manage Suspended Invoices", "ManageSuspendedInvoices", "ManageSuspendedInvoices",
                                    "Reports", false,
                                    Iata.IS.Business.Security.Permissions.Menu.GenManageSuspendedInvoices,
                                    userPermissions: Model)%>
       <%-- CMP #630: Access to ICH Protest and Adjustment Screen--%>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.AccessIchProtestAndAdjustment, Model))
           {	%>
        <li>
          <%: Html.ActionLink("ICH Protest and Adjustment", "RedirectToIchProtestAndAdjustment", "Account", null, new { target = "_blank", @class = "menuItem" })%>
        </li>
        <%} %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.GenLegalArchive, Model))
{	%>
       <li>
         <a href="#">Legal Archive Retrieval</a>
      <ul>
            <%: Html.MenuItem("Search and Retrieve", "Search", "ArchiveRetrieval", "LegalArchive", true, Iata.IS.Business.Security.Permissions.Menu.GenLegalArchiveSearchRetrieve, userPermissions: Model)%>
            <%: Html.MenuItem("Download Retrieved Invoices", "DownloadRetrievedFiles", "ArchiveRetrieval", "LegalArchive", true, Iata.IS.Business.Security.Permissions.Menu.GenLegalArchiveDownloadRetrievedInv, userPermissions: Model)%>
      </ul>
     </li>
     <% } %>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.ProfileAndUserManagement, Model))
{	%>
    <li><a href="#">Profile and User Management</a>
      <ul>
        <%:Html.MenuItem("Create / Manage Member Profile", "Create", "Member", "Profile", false,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileCreateManageMember,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Manage Member Profile", "Manage", "Member", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileManageMemberProfile,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Create Users", "Register", "Account", string.Empty, false,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileCreateManageUsers,
                                    userPermissions: Model)%>
       <%--SCP311034 : Super User Reset Please(removed user type check) [As discuss with srini if permission is assigned then menu should be visible to user]--%>
        
        <%:Html.MenuItem("Manage Users", "SearchOrModify", "Account", string.Empty, false,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileCreateManageUsers,
                                    userPermissions: Model)%>
       
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.ProfileManageUserPermissions, Model))
{	%>
        <li><a href="#" class="parentMenu">Manage User Permissions</a>
          <ul>
            <%=Html.MenuItem("Manage Permission Template", "ManagePermissionTemplate", "Permission", "Profile",
                                    false, Iata.IS.Business.Security.Permissions.Profile.ManageUserPermissionAccess,
                                    userPermissions: Model)%>
            <%=Html.MenuItem("Assign Permission To user", "PermissionToUser", "Permission", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Profile.ManageUserPermissionAccess,
                                    userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <%:Html.MenuItem("View Profile Changes", "AuditTrail", "AuditTrail", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileViewProfileChanges,
                                    userPermissions: Model)%>

        <%--CMP#655(2.1.1)IS-WEB Display per Location--%>
        <%:Html.MenuItem("Manage Location Associations", "ManageLocationAssociation", "LocationAssociation", "Profile", true,
                                        Iata.IS.Business.Security.Permissions.Menu.ProfileLocationAssociation,
                                    userPermissions: Model)%>

        <%:Html.MenuItem("View ICH Profile Changes", "Ich", "AuditTrail", "Profile", false,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileViewIchProfileChanges,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("View ACH Profile Changes", "Ach", "AuditTrail", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileViewAchProfileChanges,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Contacts Administration", "Index", "ManageContacts", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileContactsAdministration,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Proxy Log-in", "ProxyLoginSearch", "Account", null, false,
                                    Iata.IS.Business.Security.Permissions.Menu.ProfileProxyLogin, userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOps, Model))
{	%>
    <li><a href="#">SIS Ops</a>
      <ul>
        <%:Html.MenuItem("System Monitor", "Manage", "ManageSystemMonitor", "ISOps", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsSystemMonitor,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Manage System Parameters", "ManageSystemParameter", "ManageSystemParameter",
                                    "ISOps", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageSystemParameters,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Broadcast Messages", "BroadcastMessages", "BroadcastMessages", "ISOps", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsBroadcastAlertsAndNotifications,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Upload ICH & ACH Calendar", "UploadCalendar", "Calendar", "General", false,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsUploadIchAchUploadCalendar,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Upload Member Profile Data", "UploadMemberProfileCSVData", "UploadMemberProfileCSVData", "ISOps", false,
                                        Iata.IS.Business.Security.Permissions.Menu.IsUploadMemberProfileData,
                                    userPermissions: Model)%>
        <%--SCPID : 120784 - User unable to view Download Member Profile option even though permissions have been set--%>
        <%:Html.MenuItem("Download Member Profile Info", "DownloadReportForMemberdetails",
                                    "QueryAndDownloadDetails", "Profile", true,
                                        Iata.IS.Business.Security.Permissions.Menu.IsOpsDownloadMemberProfileInfo,
                                    userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IchOps, Model))
{	%>
    <li><a href="#">ICH Ops</a>
      <ul>
        <%:Html.MenuItem("Manage Blocks", "ICHBlockingRules", "ICH", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IchOpsManageBlocks,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Manage Late Submissions", "IchLateSubmission", "LateSubmission", "LateSubmission",
                                    true, Iata.IS.Business.Security.Permissions.Menu.IchOpsManageLateSubmissions,
                                    userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.AchOps, Model))
{	%>
    <li><a href="#">ACH Ops</a>
      <ul>
        <%:Html.MenuItem("Manage Blocks", "ACHBlockingRules", "ACH", "Profile", true,
                                    Iata.IS.Business.Security.Permissions.Menu.AchOpsManageBlocks,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Manage Late Submissions", "AchLateSubmission", "LateSubmission", "LateSubmission",
                                    true, Iata.IS.Business.Security.Permissions.Menu.AchOpsManageLateSubmissions,
                                    userPermissions: Model)%>
        <!-- CMP #553: ACH Requirement for Multiple Currency Handling -->
        <%:Html.MenuItem("Allowed ACH Currencies of Clearance Setup", "Index", "AchCurrencySetUp", "Masters",
                                        true, Iata.IS.Business.Security.Permissions.Menu.AchCurrencySetUp, 
                                        userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMasters, Model))
{	%>
    <li><a href="#">Master Maintenance</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneral, Model))
{	%>
        <li><a href="#" class="parentMenu">General</a>
          <ul>
            <%:Html.MenuItem("Reason Code Setup", "Index", "ReasonCode", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneralReasonCode,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Transaction Type Setup", "Index", "TransactionType", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralTransactionTypeSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Transmitter Exception Setup", "Index", "OnBehalfInvoiceSetup", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralTransmitterExceptionSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Tolerance Setup", "Index", "Tolerance", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneralToleranceSetup,
                                    userPermissions: Model)%>
         
            <%:Html.MenuItem("Minimum Value Setup", "Index", "MinAcceptableAmount", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralMinimumValueSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Maximum Value Setup", "Index", "MaxAcceptableAmount", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralMinimumValueSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Time Limit Setup", "Index", "TimeLimit", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneralTimeLimit,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Lead Period Setup", "Index", "LeadPeriod", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneralLeadPeriod,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Miscellaneous Codes Setup", "Index", "MiscCode", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralMiscellaneousCodesSetup, userPermissions: Model)%>                        
            <%:Html.MenuItem("Member Sub Status Setup", "Index", "SisMemberSubStatus", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralMemberSubStatusSetup, userPermissions: Model)%>
            <%:Html.MenuItem("File Format Setup", "Index", "FileFormat", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneralFileFormatSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("VAT Identifier Setup", "Index", "VatIdentifier", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersGeneralVatIdentifierSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Settlement Method Setup", "Index", "SettlementMethod", "Masters", true,
                                          Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersSettlementMethodSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Language Setup", "Index", "Language", "Masters", true, 
                                       Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersGeneralLanguageSetup, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersArea, Model))
{	%>
        <li><a href="#" class="parentMenu">Area Related</a>
          <ul>
            <%:Html.MenuItem("ISO Country and DS Setup", "Index", "Country", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersAreaISOCountryAndDSSetup, userPermissions: Model)%>
            <%:Html.MenuItem("City and Airport Setup", "Index", "CityAirport", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersAreaCityAndAirportSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Area Sub Division Setup", "Index", "SubDivision", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersAreaAreaSubDivisionSetup, userPermissions: Model)%>
            <%:Html.MenuItem("UN Location Setup", "Index", "UnlocCode", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersAreaUNLocationSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("ICAO Location Setup", "Index", "LocationIcao", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersAreaICAOLocationSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("ICAO Country Setup", "Index", "CountryIcao", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersAreaICAOCountrySetup,
                                    userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersCurrency, Model))
{	%>
        <li><a href="#" class="parentMenu">Currency Related</a>
          <ul>
            <%:Html.MenuItem("ISO Currency Setup", "Index", "Currency", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersCurrencyISOCurrencySetup, userPermissions: Model)%>
            <%:Html.MenuItem("Exchange Rate Setup", "Index", "ExchangeRate", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersCurrencyExchangeRateSetup, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersPassenger, Model))
{	%>
        <li><a href="#" class="parentMenu">Passenger Related</a>
          <ul>
            <%:Html.MenuItem("Sample Digit Setup", "Index", "SampleDigit", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersPassengerSampleDigitSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Tax Code Setup", "Index", "TaxCode", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersPassengerTaxCodeSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Reason Code - RM Amount Map", "Index", "RMReasonAcceptableDiff", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersPassengerReasonCodeRmAmountMap, userPermissions: Model)%>
            <%:Html.MenuItem("EMD RFIC Setup", "Index", "Rfic", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersPassengerEMDRFICSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("EMD RFISC Setup", "Index", "Rfisc", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersPassengerEMDRFISCSetup,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("BVC Matrix Setup", "Index", "BvcMatrix", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersPassengerATPCoValidatedPMISetup, userPermissions: Model)%>
            <%:Html.MenuItem("One-Way BVC Agreements Setup", "Index", "OneWayBVCAgreement", "Masters", true, 
                                    Iata.IS.Business.Security.Permissions.Menu.
                                         IsOpsManageMastersBvcAgreementSetup, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersMiscellaneous, Model))
{	%>
        <li><a href="#" class="parentMenu">Miscellaneous Related</a>
          <ul>
            <%:Html.MenuItem("Aircraft Type Setup", "Index", "AircraftType", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersMiscellaneousAircraftType, userPermissions: Model)%>
            <%:Html.MenuItem("Unit Of Measure Code Setup", "Index", "UomCode", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersMiscellaneousUOMCode,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Tax Sub Type Setup", "Index", "TaxSubType", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersMiscellaneousTaxSubType,
                                    userPermissions: Model)%>
            <%:Html.MenuItem("Aircraft Type ICAO Setup", "Index", "AircraftTypeIcao", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.
                                        IsOpsManageMastersMiscellaneousAircraftTypeICAO, userPermissions: Model)%>
            <%:Html.MenuItem("Charge Code Type Requirement Setup", "Index", "ChargeCodeTypeRequirementSetUp", "Masters", true,
                                      Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersMiscellaneousChargeCodeTypeReqSetup, userPermissions: Model)%>
            <%:Html.MenuItem("Charge Code Type Name Setup", "Index", "ChargeCodeTypeNameSetUp", "Masters", true,
                                      Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersMiscellaneousChargeCodeTypeNameSetup, userPermissions: Model)%>
            <%:Html.MenuItem("MISC Payment Status Setup", "Index", "InvPaymentStatus", "Masters", true,
                                      Iata.IS.Business.Security.Permissions.Menu.IsOpsManageMastersMiscellaneousMiscPaymentStatusSetup, userPermissions: Model)%>
          </ul>
        </li>
        <% } %>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.Sandbox, Model))
{	%>
    <li><a href="#">Sandbox</a>
      <ul>
        <%:Html.MenuItem("Certification Parameters", "Index", "SandBox", "Masters", true,
                                    Iata.IS.Business.Security.Permissions.Menu.SandboxCertificationParameter,
                                    userPermissions: Model)%>
        <%:Html.MenuItem("Sandbox Testing Report", "SandBoxTransaction", "SandBoxTransaction", "Reports",
                                    true, Iata.IS.Business.Security.Permissions.Menu.SandboxTestingDetails,
                                    userPermissions: Model)%>
      </ul>
    </li>
    <% } %>
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.Help, Model))
{	%>
    <li><a href="#">Help</a>
      <ul>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.ViewSisOpsHelp, Model))
{	%>
        <li>
          <%:
    Html.ActionLink("SIS Ops", "IntroductionToSisOps", "Home", new {area = ""},
                    new {target = "_blank", id = "helplinkmenu", @class = "ignoredirty"})%>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.ViewICHOpsHelp, Model))
{	%>
        <li><a href="#">ICH Ops </a>
          <ul>
            <li class="separatorMenuItem">
              <%:
    Html.ActionLink("Manage Blocks Help Link", "IchOpsManageBlocks", "Home", new {area = ""},
                    new {target = "_blank", id = "helplinkmenu", @class = "ignoredirty"})%>
            </li>
            <li class="separatorMenuItem">
              <%:
    Html.ActionLink("Manage Late Submissions Help Link", "IchOpsManagLateSubmissions", "Home", new {area = ""},
                    new {target = "_blank", id = "helplinkmenu", @class = "ignoredirty"})%>
            </li>
          </ul>
        </li>
        <% } %>
        <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Menu.ViewACHOpsHelp, Model))
{	%>
        <li><a href="#">ACH Ops </a>
          <ul>
            <li class="separatorMenuItem">
              <%:
    Html.ActionLink("Manage Blocks Help Link", "AchOpsManageBlocks", "Home", new {area = ""},
                    new {target = "_blank", id = "helplinkmenu", @class = "ignoredirty"})%>
            </li>
            <li class="separatorMenuItem">
              <%:
    Html.ActionLink("Manage Late Submissions Help Link", "AchOpsManagLateSubmissions", "Home", new {area = ""},
                    new {target = "_blank", id = "helplinkmenu", @class = "ignoredirty"})%>
            </li>
             <li class="separatorMenuItem">
              <%:
    Html.ActionLink("Allowed ACH Currencies of Clearance Setup Help Link", "AchOpsAllowedAchCurrencies", "Home", new { area = "" },
                    new {target = "_blank", id = "helplinkmenu", @class = "ignoredirty"})%>
            </li>
          </ul>
        </li>
        <% } %>
      </ul>
    </li>
    <% } %>
  </ul>
</div>
<span style="position: absolute; right: 0; top: 0" class="ajaxLoader">
  <img src='<%:Url.Content("~/Content/Images/ajax-loader.gif") %>' alt="Loading..." />
</span>
<div class="clear">
</div>
