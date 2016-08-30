namespace Iata.IS.Model.MemberProfile.Enums
{
  public enum ProcessingContactType
  {
    OwnProfileUpdates = 60,
    OtherMembersInvoiceReferenceDataUpdates = 61,
    MiscCorrespondence = 81,
    PayableInvoicesAvailableAlertMisc = 78,
    ICHPrimaryContact = 88,
    ICHAdviceContact = 50,
    ICHClaimConfirmationContact = 51,
    ICHClearanceInitializationContact = 52,
    ICHFinancialContact = 53,
    UatpCorrespondence = 87,
    MISCValidationErrorAlert = 79,
    PAXValidationErrorAlert = 65,
    UATPValidationErrorAlert = 84,
    ACHPrimaryContact = 89,
    PaxCorrespondence = 67,
    PaxFileReceiptAlerts = 63,
    CargoFilReceipteAlerts = 71,
    MiscFileReceiptAlerts = 77,
    UatpFileReceiptAlerts = 82,
    ValueConfirmationReportsAlerts = 68,
    AutoBillingValueDeterminationAlerts = 69,
    PaxIdec = 70,
    UatpOpenInvoicesAlert = 85,
    CgoIdec = 76,
    DsFailure = 62,
    PAXOutputAvailableAlert = 64,
    CGOOutputAvailableAlert = 72,
    MISCOutputAvailableAlert = 78,
    UATPOutputAvailableAlert = 83,
    PAXOpenInvoicesContact = 66,
    CGOOpenInvoicesContact = 74,
    MISCOpenInvoicesContact = 80,
    CargoCorrespondence = 75,
    CargoValidationAlert = 106,
    //CargoOutputAvailableAlert = 107,
    AutoBillingUnavliableInvoiceAlert = 108,
    AutoBillingThreshouldValueInvoiceAlert = 109,

    // CMP#616: New Contact Type for Correspondence Expiry Alerts
    PaxCorrespondenceExpiry = 110,
    CargoCorrespondenceExpiry = 111,
    MiscCorrespondenceExpiry = 112,
    UatpCorrespondenceExpiry = 113
  }
}
