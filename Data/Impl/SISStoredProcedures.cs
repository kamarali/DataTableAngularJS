namespace Iata.IS.Data.Impl
{
  internal static class SisStoredProcedures
  {
    public static StoredProcedure GetInvoice = new GetInvoiceSP();
    
    public static StoredProcedure GetInvoices = new GetInvoicesSP();
    public static StoredProcedure GetPaxOldIdecInvoices = new GetPaxOldIdecInvoicesSP();
    public static StoredProcedure GetCargoOldIdecInvoices = new GetCargoOldIdecInvoicesSP();
    public static StoredProcedure GetInvoiceHeader = new GetInvoiceHeaderSP();
    public static StoredProcedure GetRMLinkingDetails = new GetRMLinkingDetailsSP();
    public static StoredProcedure GetLinkedMemoAmountDetails = new GetLinkedMemoAmountDetailsSP();
    public static StoredProcedure GetRMCouponLinkingDetails = new GetRMCouponLinkingDetailsSp();
    public static StoredProcedure GetLinkedCouponAmountDetails = new GetLinkedCouponAmountDetailsSp();
    public static StoredProcedure GetMiscInvoice = new GetMiscInvoiceSP();
    public static StoredProcedure GetMiscIsWebInvoice = new GetMiscIsWebInvoiceSP();
    public static StoredProcedure GetLineItem = new GetLineItemSP();
    public static StoredProcedure GetMiscCorrespondence = new GetMiscCorrespondence();
    public static StoredProcedure GetCorrespondence = new GetCorrespondence();
    public static StoredProcedure GetFirstCorrespondence = new GetFirstCorrespondence();
    public static StoredProcedure GetRejectionMemo = new GetRejectionMemoSP();
    public static StoredProcedure GetCreditMemo = new GetCreditMemoSP();
    public static StoredProcedure GetBillingMemoOrBMCoupon = new GetBillingMemoSP();
    public static StoredProcedure GetSamplingFormC = new GetSamplingFormCSP();
    public static StoredProcedure GetFormDRecord = new GetFormDRecordSP();
    public static StoredProcedure GetPrimeCoupon = new GetPrimeCouponSP();
    public static StoredProcedure GetBillingMemo = new GetBillingMemoSP();
    public static StoredProcedure GetAuditPaxInvoice = new GetPaxAuditSP();
    #region Sampling RM Stored Procedures
    public static StoredProcedure GetSamplingCouponLinkingDetails = new GetSamplingCouponLinkingDetailsSp();
    public static StoredProcedure GetSamplingLinkedCouponAmountDetails = new GetSamplingLinkedCouponAmountDetailsSp();
    public static StoredProcedure GetFormFLinkingDetails = new GetFormFLinkingDetailsSP();
    #endregion

    #region Cargo Stored Procedures
    public static StoredProcedure GetCargoBillingMemo = new GetCargoBillingMemoSP();
    public static StoredProcedure GetCargoBmAwb = new GetCargoBmAwbSP();
    public static StoredProcedure GetCargoInvoice = new GetCargoInvoiceSP();
    public static StoredProcedure GetCgoRejectionMemo = new GetCgotRejectionMemoSP();
    public static StoredProcedure GetAuditCargoInvoice = new GetCargoAuditSP();
    public static StoredProcedure GetAwbRecord = new GetAwbRecordSP();
    public static StoredProcedure GetCargoCorrespondence = new GetCargoCorrespondence();
    public static StoredProcedure GetCargoCreditMemo = new GetCargoCreditMemoSP();

    // RM linking 
    public static StoredProcedure GetCgoRMLinkingDetails = new GetCgoRMLinkingDetailsSP();
    public static StoredProcedure GetCgoLinkedMemoAmountDetails = new GetCgoLinkedMemoAmountDetailsSP();
    public static StoredProcedure GetRMAwbLinkingDetails = new GetRMAwbLinkingDetailsSp();
    public static StoredProcedure GetLinkedAwbAmountDetails = new GetLinkedAwbAmountDetailsSp();
    #endregion
  }
}

