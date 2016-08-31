namespace Iata.IS.Business.Security.Permissions
{
    public struct Menu
    {
        #region Home
        public const int Home = 10;
        #endregion

        #region Passenger
        public const int Passenger = 11;

        public const int PaxReceivables = 111;
        
        //SCP419601: PAX permissions issue
        public const int PaxRecNonSampleCreateInvoice = 103040;

        public const int PaxRecNonSampleCreateCreditNote = 103035;

        public const int PaxRecSampleCreateFormC = 103055;

        public const int PaxRecSampleCreateFormDE = 103060;

        public const int PaxRecSampleCreateFormF = 103065;

        public const int PaxRecSampleCreateFormXF = 103070;

        public const int PaxRecManage = 11117;

        public const int PaxRecManageFormC = 11118;

        public const int PaxRecManageSupportingDocuments = 11119;

        public const int PaxRecCorrectSupportingDocumentsLinkingErrors = 11120;

        public const int PaxRecValidationErrorCorrection = 11121;

        public const int PaxPayables = 112;

        public const int PaxPayNonSampleInvoice = 11211;

        public const int PaxPayNonSampleCreditNote = 11212;

        public const int PaxPaySampleFormC = 11213;

        public const int PaxPaySampleFormDE = 11214;

        public const int PaxPaySampleFormF = 11215;

        public const int PaxPaySampleFormXF = 11216;

        public const int PaxPaySearch = 11217;

        public const int PaxPaySearchSampleFormC = 11218;

        public const int PaxPayViewSupportingDocuments = 11219;

        public const int PaxViewBillingHistoryAndCorrespondence = 113;

        // SCP#447047: Correspondences
        public const int PaxDownloadCorrespondences = 114;

        public const int PaxRecCorrectAutoBillingInvoices = 11122;
        #endregion

        #region Cargo
        public const int Cargo = 13;

        public const int CgoReceivables = 131;

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        public const int CgoRecCreateInvoice = 1311120;

        public const int CgoRecCreateCreditNote = 1311230;

        public const int CgoRecManage = 13113;

        public const int CgoRecManageSupportingDocuments = 13114;

        public const int CgoRecvalidationErrorCorrection = 13115;

        public const int CgoRecCorrectSupportingDocumentsLinkingErrors = 13116;
        
        public const int CgoPayables = 132;

        public const int CgoPayInvoice = 13211;

        public const int CgoPaySearch = 13212;

        public const int CgoPayViewSupportingDocuments = 13213;

        public const int CgoViewBillingHistoryAndCorrespondence = 133;

        public const int CgoDownloadCorrespondences = 134;
        
        #endregion

        #region MISC
        public const int Miscellaneous = 12;

        public const int MiscReceivables = 121;

        public const int MiscRecCreateInvoice = 12111;

        public const int MiscRecCreateCreditNote = 12112;

        public const int MiscRecManageSupportingDocuments = 12113;
        public const int MiscRecManageInvoices = 12120;
        public const int MiscRecCorrectSupportingDocumentsLinkingErrors = 12114;

        //SCP401669: Misc Permissions Issue
        public const int MiscRecValidationErrorCorrection = 12115;

        public const int MiscPayables = 122;

        public const int MiscPayInvoice = 12211;

        public const int MiscPayCreditNote = 12212;

        public const int MiscPaySearch = 12213;

        public const int MiscPayViewSupportingDocuments = 12214;

        public const int MiscViewBillingHistoryAndCorrespondence = 123;

        // SCP#447047: Correspondences
        public const int MiscDownloadCorrespondences = 124;

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        public const int MiscPayDailyBilateralDelivery = 12215;

        #endregion

        #region UATP
        public const int UATP = 14;

        public const int UATPReceivables = 141;

        public const int UATPRecCreateInvoice = 14111;

        public const int UATPRecCreateCreditNote = 14112;

        public const int UATPRecManageInvoices = 14113;

        public const int UATPRecManageSupportingDocuments = 14114;

        public const int UATPRecValidationErrorCorrection = 14115;
        
        public const int UATPRecCorrectSupportingDocumentsLinkingErrors = 14116;

        public const int UATPPayables = 142;

        public const int UATPPayInvoice = 14211;

        public const int UATPPayCreditNote = 14212;

        public const int UATPPaySearch = 14213;

        public const int UATPPayViewSupportingDocuments = 14214;

        public const int UATPViewBillingHistoryAndCorrespondence = 143;

        public const int UATPDownloadCorrespondences = 144;
        #endregion

        #region General
        public const int General = 20;

        public const int GenFileManagement = 201;

        public const int GenFileManagementUploadFile = 2011;

        public const int GenFileManagementDownloadFile = 2012;

        public const int GenViewISandCHCalendar = 202;

        public const int GenManageSuspendedInvoices = 203;

        public const int GenLegalArchive = 204;

        public const int GenLegalArchiveSearchRetrieve = 2041;

        public const int GenLegalArchiveDownloadRetrievedInv = 2042;


        #endregion

        #region Reports
        public const int Reports = 21;

        public const int RepProcessingDashboard = 211;

        public const int RepSisUsageReport = 212;

        public const int RepMemberContactReports = 213;

        public const int RepISCHCalendarReport = 230;

        public const int RepInvoiceReferenceData = 231;

        public const int RepFinancialController = 214;

        public const int RepPassenger = 215;

        public const int RepCargo = 217;

        public const int RepMiscellaneous = 216;

        public const int RepUserPermissionReport = 218;

        public const int RepInvoceDeletion = 219;

        public const int RepFinCtrlInterlineBillingsummary = 21430;

        public const int RepFinCtrlInterlinePayablesAnalysis = 21431;

        public const int RepFinCtrlSuspendedBillings = 21411;

        public const int RepFinCtrlPendingErrorInvoices = 21432;

        public const int RepFinCtrlTop10Receivables = 21433;

        public const int RepFinCtrlTop10Payables = 21434;

        /* CMP #645: Access ACH Settlement Reports*/
        public const int RepFinCtrlAccessAchSettlementReports = 21413;

        public const int RepFinCtrlAccessIchWebReports = 21412;

        public const int RepPaxReceivables = 21511;

        public const int RepPaxPayables = 21512;

        public const int RepPaxCorrespondenceStatus = 21513;
      
        public const int RepPaxBvcDetails = 21520;

        public const int RepPaxReceivablesInterlineBillSummary = 2151120;

        public const int RepPaxReceivablesNonSampleRejnAnalysis = 2151121;

        public const int RepPaxReceivablesSamplingBillingAnalysis = 2151122;

        public const int RepPaxReceivablesRMBMCMSummary = 21514;

        public const int RepPaxReceivablesSupportingDocMismatch = 21519;

        public const int RepPaxPayablesInterlineBillSummary = 2151220;

        public const int RepPaxPayablesNonSampleRejnAnalysis = 2151221;

        public const int RepPaxPayablesSamplingBillingAnalysis = 2151222;
        
        public const int RepPaxPayablesRMBMCMSummary = 21516;

        public const int RepCargoReceivables = 21711;

        public const int RepCargoPayables = 21712;

        public const int RepCargoRMBMCMDetails = 21713;

        public const int RepCargoCorrespondenceStatus = 21714;

        public const int RepCargoReceivablesSubmissionOverview = 2171120;

        public const int RepCargoReceivablesInterlineBillSummary = 2171121;

        public const int RepCargoReceivablesRejnAnalysis = 2171122;

        public const int RepCargoReceivablesRMBMCMSummary = 2171123;

        public const int RepCargoReceivablesSupportingDocMismatch = 2171124;

        public const int RepCargoPayablesSubmissionOverview = 2171220;

        public const int RepCargoPayablesInterlineBillSummary = 2171221;

        public const int RepCargoPayablesRejnAnalysis = 2171222;

        public const int RepCargoPayablesRMBMCMSummary = 2171223;
        
        public const int RepMiscSubstitutionValues = 21611;

        public const int RepMiscReceivablesSupportingDocMismatch = 21615; 

        public const int RepMiscRecInvSummary = 21612;

        public const int RepMiscPayChargeSummary = 21614;

        public const int RepMiscCorrespondenceStatus = 21613;

        public const int RepPaxReceivablesBillingValueConfAnalysis = 21517;

        public const int RepPaxPayablesBillingValueConfAnalysis = 21518;

        public const int RepIata = 218;

        public const int RepIataOldIdecStatistics = 21811;

        public const int RepIataOldIdecInvalidPeriod = 21812;

        public const int RepIataOldIdecReceiptDetails = 21813;

        public const int RepSisOps = 219;

        #endregion

        #region Profile And User Management
        public const int ProfileAndUserManagement = 22;

        public const int ProfileCreateManageMember = 221;

        public const int ProfileManageMemberProfile = 222;

        public const int ProfileCreateManageUsers = 223;

        public const int ProfileManageUserPermissions = 224;

        public const int ProfileViewProfileChanges = 225;

        public const int ProfileViewIchProfileChanges = 226;

        public const int ProfileViewAchProfileChanges = 227;

        public const int ProfileContactsAdministration = 228;

        //CMP#655(2.1.1)IS-WEB Display per Location
        public const int ProfileLocationAssociation = 232;

        #endregion

        #region ISOPS
        //Masters - Start
        public const int IsOps = 30;
        public const int IsOpsManageMasters = 301;
        public const int IsOpsManageMastersArea = 3011;
        public const int IsOpsManageMastersAreaISOCountryAndDSSetup = 30111;
        public const int IsOpsManageMastersAreaCityAndAirportSetup = 30112;
        public const int IsOpsManageMastersAreaUNLocationSetup = 30114;
        public const int IsOpsManageMastersAreaAreaSubDivisionSetup = 30139;
        public const int IsOpsManageMastersAreaICAOCountrySetup = 30113;
        public const int IsOpsManageMastersAreaICAOLocationSetup = 30115;
        public const int IsOpsManageMastersAreaICAOAirportSetup = 30119;
        public const int IsOpsManageMastersCargo = 3012;
        public const int IsOpsManageMastersCargoReasonCodeRmAmountMap = 30131;
        public const int IsOpsManageMastersCurrency = 3013;
        public const int IsOpsManageMastersCurrencyISOCurrencySetup = 30116;
        public const int IsOpsManageMastersCurrencyExchangeRateSetup = 30120;
        public const int IsOpsManageMastersGeneral = 3014;
        public const int IsOpsManageMastersGeneralTimeLimit = 30121;
        public const int IsOpsManageMastersGeneralReasonCode = 30124;
        public const int IsOpsManageMastersGeneralMinimumValueSetup = 30126;
        public const int IsOpsManageMastersGeneralToleranceSetup = 30127;
        public const int IsOpsManageMastersGeneralTransmitterExceptionSetup = 30128;
        public const int IsOpsManageMastersGeneralLinkedMemberSetup = 30129;
        public const int IsOpsManageMastersGeneralVatIdentifierSetup = 30135;
        public const int IsOpsManageMastersGeneralFileFormatSetup = 30136;
        public const int IsOpsManageMastersGeneralTransactionTypeSetup = 30137;
        public const int IsOpsManageMastersGeneralMemberStatusSetup = 30138;
        public const int IsOpsManageMastersGeneralMemberSubStatusSetup = 30140;
        public const int IsOpsManageMastersGeneralMiscellaneousCodesSetup = 30144;
        public const int IsOpsManageMastersGeneralLeadPeriod = 30145;
        public const int IsOpsManageMastersSettlementMethodSetup = 30146;

        public const int IsOpsManageMastersMiscellaneous = 3015;
        public const int IsOpsManageMastersMiscellaneousTaxSubType = 30141;
        public const int IsOpsManageMastersMiscellaneousAircraftType = 30117;
        public const int IsOpsManageMastersMiscellaneousAircraftTypeICAO = 30118;
        public const int IsOpsManageMastersMiscellaneousUOMCode = 30143;
        public const int IsOpsManageMastersPassenger = 3016;
        public const int IsOpsManageMastersPassengerTaxCodeSetup = 30122;
        public const int IsOpsManageMastersPassengerReasonCodeRmAmountMap = 30130;
        public const int IsOpsManageMastersPassengerATPCoValidatedPMISetup = 30132;
        public const int IsOpsManageMastersPassengerEMDRFICSetup = 30133;
        public const int IsOpsManageMastersPassengerEMDRFISCSetup = 30134;
        public const int IsOpsManageMastersPassengerSampleDigitSetup = 30142;

        //SCP212072 - Unable to update Language Master.
        public const int IsOpsManageMastersGeneralLanguageSetup = 30147;

        //CMP #636: Standard Update Mobilization.
        public const int IsOpsManageMastersMiscellaneousChargeCodeTypeReqSetup = 30155;
        public const int IsOpsManageMastersMiscellaneousChargeCodeTypeNameSetup = 30156;

        //CMP #692: Misc Payment Status.
        public const int IsOpsManageMastersMiscellaneousMiscPaymentStatusSetup = 30157;
        
        /// <summary>
        /// CMP#538
        /// </summary>
        public const int IsOpsManageMastersBvcAgreementSetup = 30161;
        //Masters - End

        public const int IsOpsManageSystemParameters = 302;

        public const int IsOpsSystemMonitor = 303;

        public const int IsOpsBroadcastAlertsAndNotifications = 304;

        public const int IsOpsUploadIchAchUploadCalendar = 305;

        public const int IsUploadMemberProfileData = 306;

        public const int IsOpsDownloadMemberProfileInfo = 324;

      

        public const int ProfileProxyLogin = 229;

        //CMP #630: Access to ICH Protest and Adjustment Screen
        public const int AccessIchProtestAndAdjustment = 2051;
        #endregion

        #region ICHOps

        public const int IchOps = 31;

        public const int IchOpsManageLateSubmissions = 311;

        public const int IchOpsProcessingDashboard = 312;

        public const int IchOpsManageBlocks = 313;
        #endregion

        #region ACHOps
        public const int AchOps = 32;

        public const int AchOpsManageLateSubmissions = 321;

        public const int AchOpsProcessingDashboard = 322;

        public const int AchOpsManageBlocks = 323;

        //CMP #553: ACH Requirement for Multiple Currency Handling
        public const int AchCurrencySetUp = 632311;
        #endregion


        #region Sandbox Testing
        public const int Sandbox = 33;

        public const int SandboxCertificationParameter = 331;

        public const int SandboxTestingDetails = 332;
  
        #endregion


        #region Help
        public const int Help = 34;

        public const int ViewSisOpsHelp = 341;

        public const int ViewACHOpsHelp = 342;

        public const int ViewICHOpsHelp = 343;

        #endregion

    }
}
