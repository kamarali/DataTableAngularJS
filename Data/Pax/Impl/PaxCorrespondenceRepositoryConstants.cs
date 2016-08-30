namespace Iata.IS.Data.Pax.Impl
{
  public class PaxCorrespondenceRepositoryConstants
  {
    #region GetCorrespondenceLS Parameters

    public const string CorrespondenceId = "CORRESPONDENCE_ID_I";
    public const string CorrespondenceNumber = "CORRESPONDENCE_NO_I";
    public const string CorrespondenceStage = "CORRESPONDENCE_STAGE_I";
    #endregion

    public const string UpdatePaxCorrespondenceMethodName = "UpdatePaxCorrespondenceStatus";
    public const string BillingPeriodParameterName = "BILLING_PERIOD_I";

    public const string GetCorresRefNoMethodName = "GetCorresRefNo";
    public const string MemberIdParameterName = "MEMBER_ID_I";
    public const string ResultParameterName = "CORR_REF_NO_O";

    #region Enqueue Correspondence For Report Download

    /*SCP# 120094 - Pax Correspondence Download- Download All button 
    Desc: Comma seperated field in queue had size limit of 4000, now it is updated (to CLOB) to accomodate value of any length. Below are the constants related to SP.
    Date: 10-May-2013
     */
    public const string MemberIdParameterNameForEnqueDownloadRequest = "MEMBER_ID_I";
    public const string UserIdParameterNameForEnqueDownloadRequest = "USER_ID_I";
    public const string CorrespondenceIdsParameterNameForEnqueDownloadRequest = "COREESPONDENCE_NOS_I";
    public const string DownloadUrlForEnqueDownloadRequest = "DOWNLOAD_URL_I";
    public const string EnqueueCorrespondenceReportForDownloadFunctionName = "enqueueCorrespondenceReportForDownload";

    #endregion

    //SCP106534: ISWEB No-02350000768 
    //Desc: Constants to call Procedure to create a correspondence.
    //Date: 20/06/2013
    #region Create Pax Correspondence

    public const string ProcCreatePaxCorrespondenceSP = "ProcCreatePaxCorrespondence";

    public const string CORRESPONDENCE_NO_I = "CORRESPONDENCE_NO_I";
    public const string CORRESPONDENCE_DATE_I = "CORRESPONDENCE_DATE_I";
    public const string CORRESPONDENCE_STAGE_I = "CORRESPONDENCE_STAGE_I";
    public const string FROM_MEMBER_ID_I = "FROM_MEMBER_ID_I";
    public const string TO_MEMBER_ID_I = "TO_MEMBER_ID_I";
    public const string TO_EMAILID_I = "TO_EMAILID_I";
    public const string AMOUNT_TO_SETTLED_I = "AMOUNT_TO_SETTLED_I";
    public const string OUR_REFERENCE_I = "OUR_REFERENCE_I";
    public const string YOUR_REFERENCE_I = "YOUR_REFERENCE_I";
    public const string SUBJECT_I = "SUBJECT_I";
    public const string CORRESPONDENCE_STATUS_I = "CORRESPONDENCE_STATUS_I";
    public const string AUTHORITY_TO_BILL_I = "AUTHORITY_TO_BILL_I";
    public const string NO_OF_DAYS_TO_EXPIRE_I = "NO_OF_DAYS_TO_EXPIRE_I";
    public const string LAST_UPDATED_BY_I = "LAST_UPDATED_BY_I";
    public const string LAST_UPDATED_ON_I = "LAST_UPDATED_ON_I";
    public const string FROM_EMAILID_I = "FROM_EMAILID_I";
    public const string CURRENCY_CODE_NUM_I = "CURRENCY_CODE_NUM_I";
    public const string CORRESPONDENCE_DETAILS_I = "CORRESPONDENCE_DETAILS_I";
    public const string CORRESPONDENCE_OWNER_ID_I = "CORRESPONDENCE_OWNER_ID_I";
    public const string CORRESPONDENCE_SUB_STATUS_I = "CORRESPONDENCE_SUB_STATUS_I";
    /* CMP#657: Retention of Additional Email Addresses in Correspondences
       Adding code to get email ids from initiator and non-initiator and removing
       additional email field*/
    public const string ADDITIONAL_EMAIL_INITIATOR = "ADDITIONAL_EMAIL_INITIATOR";
    public const string ADDITIONAL_EMAIL_NON_INITIATOR = "ADDITIONAL_EMAIL_NON_INITIATOR";
    public const string INVOICE_ID_I = "INVOICE_ID_I";
    public const string CORRESPONDENCE_SENT_ON_I = "CORRESPONDENCE_SENT_ON_I";
    public const string EXPIRY_DATE_I = "EXPIRY_DATE_I";
    public const string EXPIRY_DATEPERIOD_I = "EXPIRY_DATEPERIOD_I";
    public const string BM_EXPIRY_PERIOD_I = "BM_EXPIRY_PERIOD_I";
    public const string PAX_RM_IDS_I = "PAX_RM_IDS_I";
    public const string PAX_CORR_ATTACHMENT_IDS_I = "PAX_CORR_ATTACHMENT_IDS_I";
    public const string CORRESPONDENCE_ID_O = "CORRESPONDENCE_ID_O";
    public const string OPERATION_STATUS_INDICATOR_O = "OPERATION_STATUS_INDICATOR_O";
    //CMP526 - Passenger Correspondence Identifiable by Source Code
    public const string SOURCE_CODE_I = "SOURCE_CODE_I";

      #endregion

  }
}