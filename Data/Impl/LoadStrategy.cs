using System.Collections.Generic;

namespace Iata.IS.Data.Impl
{
  public class LoadStrategy
  {
    private readonly List<string> _entityList = new List<string>();
    private readonly string _entityNames = string.Empty;

    public LoadStrategy(string entityNames)
    {
      _entityNames = entityNames;
      _entityList.AddRange(entityNames.Split(',', '.'));
    }

    public List<string> EntityNames
    {
      get
      {
        return _entityList;
      }
    }

    public override string ToString()
    {
      return _entityNames;
    }

    public static class Entities
    {
      public const string Invoice = "Invoice";
      public const string Coupon = "CouponDataRecord";
      public const string CouponAttachment = "CouponAttachment";
      public const string CouponISWeb = "CouponDataRecordISWeb";
      public const string CouponAttachmentISWeb = "CouponAttachmentISWeb";
      public const string CouponTax = "CouponDataTaxRecord";
      public const string CouponVat = "CouponDataVatRecord";
      public const string CouponVatIdentifier = "CouponVatIdentifier";
      public const string CouponSourceCode = "CouponDataSourceCode";
      public const string MemberLocation = "MemberLocation";
      public const string MemberLocationInfoAddDetail = "MemberLocationInfoAddDetail";

      public const string BillingMemo = "BillingMemoRecord";
      public const string BillingMemoVat = "BillingMemoVat";
      public const string BillingMemoVatIdentifier = "BillingMemoVatIdentifier";
      public const string BillingMemoAttachments = "BillingMemoAttachments";
      public const string BillingMemoISWeb = "BillingMemoRecordISWeb";
      public const string BillingMemoAttachmentsISWeb = "BillingMemoAttachmentsISWeb";
      public const string BillingMemoCoupon = "BillingMemoCoupon";
      public const string BillingMemoCouponVat = "BillingMemoCouponVat";
      public const string BMCouponVatIdentifier = "BMCouponVatIdentifier";
      public const string BMCouponAttachments = "BMCouponAttachments";
      public const string BillingMemoCouponISWeb = "BillingMemoCouponISWeb";
      public const string BMCouponAttachmentsISWeb = "BMCouponAttachmentsISWeb";
      public const string BillingMemoCouponTax = "BillingMemoCouponTax";

      public const string CreditMemo = "CreditMemoRecord";
      public const string CreditMemoVat = "CreditMemoVat";
      public const string CreditMemoVatIdentifier = "CreditMemoVatIdentifier";
      public const string CreditMemoAttachments = "CreditMemoAttachments";
      public const string CreditMemoISWeb = "CreditMemoRecordISWeb";
      public const string CreditMemoAttachmentsISWeb = "CreditMemoAttachmentsISWeb";
      public const string CreditMemoCoupon = "CreditMemoCoupon";
      public const string CreditMemoCouponTax = "CreditMemoCouponTax";
      public const string CreditMemoCouponVat = "CreditMemoCouponVat";
      public const string CreditMemoCouponVatIdentifier = "CMCouponVatIdentifier";
      public const string CreditMemoCouponAttachments = "CMCouponAttachments";
      public const string CreditMemoCouponISWeb = "CreditMemoCouponISWeb";
      public const string CreditMemoCouponAttachmentsISWeb = "CMCouponAttachmentsISWeb";

      public const string RejectionMemo = "RejectionMemoRecord";
      public const string RejectionMemoVat = "RejectionMemoVat";
      public const string RejectionMemoCoupon = "RejectionMemoCoupon";
      public const string RejectionMemoCouponTax = "RejectionMemoCouponTax";
      public const string RejectionMemoCouponVat = "RejectionMemoCouponVat";
      public const string RejectionMemoVatIdentifier = "RejectionMemoVatIdentifier";
      public const string RejectionMemoAttachments = "RejectionMemoAttachments";
      public const string RejectionMemoISWeb = "RejectionMemoRecordISWeb";
      public const string RejectionMemoAttachmentsISWeb = "RejectionMemoAttachmentsISWeb";
      public const string RejectionMemoOtherCharge = "RejectionMemoOtherCharge";
      public const string RejectionMemoProrateLadder = "RejectionMemoProrateLadder";
      public const string RejectionMemoCouponVatIdentifier = "RMCouponVatIdentifier";
      public const string RejectionMemoCouponAttachments = "RMCouponAttachments";
      public const string RejectionMemoCouponISWeb = "RejectionMemoCouponISWeb";
      public const string RejectionMemoCouponAttachmentsISWeb = "RMCouponAttachmentsISWeb";

      public const string SourceCodeTotal = "SourceCodeTotal";
      public const string SourceCode = "SourceCodeRecord";
      public const string SourceCodeVat = "SourceCodeVatRecord";    //remaining
      public const string SourceCodeTotalVat = "SourceCodeTotalVat";
      public const string InvoiceTotal = "InvoiceTotalRecord";
      public const string InvoiceTotalVat = "InvoiceTotalVat";//remaining
      public const string InvoiceTotalVatIdentifier = "InvTotalVatIdentifierRecord";//remaining
      public const string CouponDataVatIdentifier = "CouponDataVatIdentifierRecord";//remaining
      public const string SourceCodeVatIdentifier = "SourceCodeVatIdentifierRecord";//remaining

      public const string BillingMember = "BillingMember";
      public const string BilledMember = "BilledMember";
      public const string ListingCurrency = "ListingCurrency";
      public const string AttachmentUploadedbyUser = "AttachmentUploadedByUser";
      public const string CorrespondenceOwnerInfo = "CorrespondenceOwnerInfo";

      public const string SamplingFormDRecord = "SamplingFormDRecord";
      public const string SamplingFormDAttachment = "SamplingFormDAttachment";
      public const string SamplingFormDRecordISWeb = "SamplingFormDRecordISWeb";
      public const string SamplingFormDAttachmentISWeb = "SamplingFormDAttachmentISWeb";
      public const string SamplingFormDVatIdentifier = "SamplingFormDVatIdentifier";
      public const string SamplingFormDVat = "SamplingFormDRecordVat";
      public const string SamplingFormDTax = "SamplingFormDTax";

      public const string SamplingFormEDetails = "SamplingFormEDetails";
      public const string SamplingFormEDetailVat = "SamplingFormEDetailVat";
      public const string SamplingFormEDetailVatIdentifier = "SamplingFormEDetVatIdentifier";
      public const string ProvisionalInvoiceDetails = "ProvisionalInvoiceDetails";

      public const string SamplingFormC = "SamplingFormC";
      public const string SamplingFormCDetails = "SamplingFormCDetails";
      public const string SamplingFormCListingCurrency = "SamplingFormCListingCurrency";
      public const string ProvisionalBillingMember = "ProvisionalBillingMember";
      public const string SamplingFormCFromMember = "SamplingFormCFromMember";
      public const string SamplingFormCRecordAttachment = "SamplingFormCRecordAttachment";
      public const string SamplingFormCRecordAttachmentISWeb = "SamplingFormCRecordAttachmentISWeb";
      public const string SamplingFormCSourceCodeTotal = "FormCSourceCodeTotal";

      public const string Correspondence = "Correspondence";
      public const string CorrespondencesFromMember = "CorrespondencesFromMember";
      public const string CorrespondencesToMember = "CorrespondencesToMember";
      public const string CorrespondenceAttachment = "CorrespondenceAttachment";
      public const string CorrespondenceCurrency = "CorrespondenceCurrency";

      public const string AutoBillCoupon = "AutoBillingCoupon";
      public const string AutoBillCouponTax = "AutoBillingCouponTax";
      public const string AutoBillCouponVat = "AutoBillingCouponVat";
      public const string AutoBillCouponAttach = "AutoBillCouponAttach";

      public const string CouponMarketingDetails = "CouponMarketingDetails";
    }

    public static class MiscEntities
    {
      public const string MiscInvoice = "MiscInvoice";
      public const string MiscInvoiceTaxAdditionalDetail = "MUTaxAdditionalDetail";
      public const string BilledMember = "BilledMember";
      public const string BillingMember = "BillingMember";
      public const string ChargeCategory = "ChargeCategory";
      public const string ChargeCategoryChargeCode = "ChargeCategoryChargeCode";
      public const string ChargeCategoryChargeCodeType = "ChargeCategoryChargeCodeType";
      public const string InvoiceSummary = "InvoiceSummary";
      public const string LineItem = "LineItem";
      public const string LineItemChargeCode = "LineItemChargeCode";
      public const string LineItemUomCode = "LineItemUomCode";
      public const string MiscTaxBreakdown = "MiscTaxBreakdown";
      public const string MiscUatpAttachment = "MiscUatpAttachments";
      public const string MiscUatpAttachmentISWeb = "MiscUatpAttachmentsISWeb";
      public const string MiscInvoiceAddOnCharge = "MiscInvoiceAddOnCharge";
      public const string MiscUatpInvoiceAdditionalDetail = "MUInvoiceAdditionalDetail";
      public const string MemberContact = "MemberContact";
      public const string PaymentDetail = "PaymentDetail";
      public const string ListingCurrency = "ListingCurrency";
      public const string InvoiceOwner = "InvoiceOwner";
      public const string LineItemChargeCodeType = "LineItemChargeCodeType";
      public const string LineItemDetails = "LineItemDetails";
      public const string LineItemDetailsFieldValues = "LineItemDetailsFieldValues";
      public const string LIDetFieldValuesFieldMetaData = "LIDetFieldValuesFieldMetaData";
      public const string LineItemDetailFieldValueAttrValue = "LIDFieldValueAttrValue";
      public const string LineItemDetailFieldValueParentValue = "LIDFieldValueParentValue";
      public const string LIDFMDataSource = "LIDFMDataSource";
      public const string LineItemTaxBreakdown = "LineItemTaxBreakdown";
      public const string LineItemAddOnCharges = "LineItemAddOnCharges";
      public const string LineItemDetailAdditionalDet = "LineItemDetailAdditionalDet";
      public const string LineItemAdditionalDetails = "LineItemAdditionalDetails";
      public const string LineItemTaxAdditionalDetails = "LineItemTaxAdditionalDetails";
      public const string LineItemDetailTaxAdditionalDetails = "LIDTaxAdditionalDetails";
      public const string LineItemDetailUomCode = "LineItemDetailUomCode";
      public const string LineItemDetailTaxBreakdown = "LineItemDetailTaxBreakdown";
      public const string LineItemDetailAddOnCharges = "LineItemDetailAddOnCharges";
      public const string OtherOrganizationInformation = "OOInformation";
      public const string OtherOrganizationAdditionalDetails = "OOAdditionalDetails";
      public const string OtherOrganizationContactInformations = "OOContactInformations";
      public const string MiscTaxBreakdownCountry = "MiscTaxBreakdownCountry";
      public const string LineItemTaxCountry = "LineItemTaxCountry";
      public const string LineItemDetailTaxCountry = "LineItemDetailTaxCountry";
    }
    public static class CargoEntities
    {
      public const string CargoInvoice = "CargoInvoice";
      public const string CargoInvoiceTaxAdditionalDetail = "CargoTaxAdditionalDetail";
      public const string BilledMember = "BilledMember";
      public const string BillingMember = "BillingMember";
      public const string ChargeCategory = "ChargeCategory";
      public const string MemberLocation = "MemberLocation";
      public const string Members = "Members";
      
      //  public const string BillingMemoOtherCharge = "BillingMemoOtherCharge";
      public const string AttachmentUploadedbyUser = "AttachmentUploadedByUser";
      
      public const string InvoiceSummary = "InvoiceSummary";
      public const string InvoiceTotal = "InvoiceTotalRecord";
      public const string CargoTaxBreakdown = "CargoTaxBreakdown";
      public const string CargoUatpAttachment = "CargoUatpAttachments";
      public const string CargoInvoiceAddOnCharge = "CargoInvoiceAddOnCharge";
      public const string CargoUatpInvoiceAdditionalDetail = "CargoInvoiceAdditionalDetail";
      public const string MemberContact = "MemberContact";
      public const string PaymentDetail = "PaymentDetail";
      public const string ListingCurrency = "ListingCurrency";
      public const string InvoiceOwner = "InvoiceOwner";
      public const string Correspondence = "Correspondence";
      public const string SourceCode = "SourceCode";
      
      public const string OtherOrganizationInformation = "OOInformation";
      public const string OtherOrganizationAdditionalDetails = "OOAdditionalDetails";
      public const string OtherOrganizationContactInformations = "OOContactInformations";
      public const string CargoTaxBreakdownCountry = "CargoTaxBreakdownCountry";
      public const string InvoiceTotalVat = "InvoiceTotalVat";//remaining

      public const string CargoVatIdentifier = "CargoVatIdentifier";

      public const string BillingCodeSubTotal = "BillingCodeSubTotal";
      public const string CargoBillingCodeSubTotalVat = "CargoBillingCodeSubTotalVat";

      public const string AwbRecord = "AwbRecord";
      public const string AwbRecordVat = "AwbRecordVat";
      public const string AwbOtherCharge = "AwbOtherCharge";
      public const string AwbAttachment = "AwbAttachment";

      public const string RejectionMemo = "RejectionMemoRecord";
      public const string RejectionMemoVat = "RejectionMemoVat";
      public const string RejectionMemoAttachments = "RejectionMemoAttachments";
      public const string RmAwb = "RmAwb";
      public const string RmAwbVat = "RmAwbVat";
      public const string RmAwbAttachments = "RmAwbAttachments";
      public const string RmAwbProrateLadder = "RmAwbProrateLadder";
      public const string RmAwbProrateLadderDetail = "RmAwbProrateLadderDetail";
      public const string RmAwbOtherCharges = "RmAwbOtherCharges";

      public const string BillingMemo = "CargoBillingMemoRecord";
      public const string BillingMemoVat = "CargoBillingMemoVat";
      public const string BillingMemoAttachments = "CargoBillingMemoAttachments";
      public const string BmAwb = "BMAwb";
      public const string BmAwbVat = "BMAwbVat";
      public const string BmAwbAttachments = "BmAwbAttachments";
      public const string BmAwbProrateLadder = "BmAwbProrateLadder";
      public const string BmAwbProrateLadderDetail = "BmAwbProrateLadderDetail";
      public const string BmAwbOtherCharges = "BmAwbOtherCharges";
      public const string BMAwbVatIdentifier = "BMAwbVatIdentifier";
      public const string BMAttachmentUploadedByUser = "BMAttachmentUploadedByUser";
      
      public const string CreditMemo = "CreditMemoRecord";
      public const string CreditMemoVat = "CreditMemoVat";
      public const string CreditMemoAttachments = "CreditMemoAttachments";
      public const string CmAwb = "CmAwb";
      public const string CmAwbVat = "CmAwbVat";
      public const string CmAwbAttachments = "CmAwbAttachments";
      public const string CmAwbProrateLadder = "CmAwbProrateLadder";
      public const string CmAwbProrateLadderDetail = "CmAwbProrateLadderDetail";
      public const string CmAwbVatIdentifier = "CmAwbVatIdentifier";
      public const string CmAwbOtherCharges = "CmAwbOtherCharges";
      public const string CmAttachmentUploadedByUser = "CmAttachmentUploadedByUser";

      public const string BillingMemoVatIdentifier = "CargoBillingMemoVatIdentifier";
      public const string CreditMemoVatIdentifier = "CargoCreditMemoVatIdentifier";
      public const string RmAwbVatIdentifier = "RmAwbVatIdentifier";

      public const string AwbDataVatIdentifier = "AwbDataVatIdentifier";
      public const string Invoice = "Invoice";
      public const string Currency = "Currency";
      public const string CorrespondenceAttachment = "CorrespondenceAttachment";
      // public const string RejectionMemoOtherCharge = "RejectionMemoOtherCharge";
     
    }
    public static class PaxEntities
    {
      public const string PaxInvoice = "PaxInvoice";
      public const string Members = "Members";
      public const string PrimeCoupon = "PrimeCoupon";
      public const string Correspondence = "Correspondence";
      public const string SourceCode = "SourceCode";
      public const string RejectionMemo = "RejectionMemo";
      public const string BillingMemo = "BillingMemo";
      public const string RejectionMemoCoupon = "RejectionMemoCoupon";
      public const string BillingMemoCoupon = "BillingMemoCoupon";
      public const string FormDCoupon = "FormDCoupon";
      public const string PrimecouponTax = "PrimecouponTax";
      public const string PrimecouponVat = "PrimecouponVat";
      public const string RejectionMemoVAT = "RejectionMemoVAT";
      public const string BMVAT = "BMVAT";
      public const string RMCouponTax = "RMCouponTAX";
      public const string RMCouponVAT = "RMCouponVAT";
      public const string BMCouponTax = "BMCouponTax";
      public const string BMCouponVAT = "BMCouponVAT";
      public const string PMAttachment = "PMATTACHMENT";
      public const string RMAttachment = "RMATTACHMENT";
      public const string RMCouponAttachment = "RMCouponATTACHMENT";
      public const string BMAttachment = "BMATTACHMENT";
      public const string BMCouponATTACHMENT = "BMCouponATTACHMENT";
      public const string Currency = "Currency";
    }
  }


}

