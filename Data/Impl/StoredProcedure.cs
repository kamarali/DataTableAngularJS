using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.Impl
{
  public abstract class StoredProcedure
  {
    public OracleParameterCollection Parameters;

    public void AddInputParameters(OracleParameter[] inParams)
    {
      this.Parameters.AddRange(inParams);
    }

    public abstract string Name { get; }

    public abstract List<SPResultObject> GetResultSpec();
  }

  public class GetInvoiceSP : StoredProcedure
  {
    public override string Name
    {
      get
      {
        return "PROC_LOAD_PAX_INVOICE";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.Entities.Invoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BilledMember, ParameterName="R_CUR_Billed_Member_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMember, ParameterName="R_CUR_BILLING_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.MemberLocation, ParameterName="R_CUR_MEMBER_LOCATION_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.Coupon, ParameterName="R_CUR_COUPON_RECORD_O"},
                new SPResultObject{ EntityName = LoadStrategy.Entities.CouponTax, ParameterName = "R_CUR_COUPON_RECORD_TAX_O"}, 
                new SPResultObject{ EntityName = LoadStrategy.Entities.CouponVat, ParameterName = "R_CUR_COUPON_RECORD_VAT_O"}, 

                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemo, ParameterName="R_CUR_BILLING_MEMO_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemoVat, ParameterName="R_CUR_BILLING_MEMO_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemoCoupon, ParameterName="R_CUR_BILING_MEMO_COUPON_O"},                
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemoCouponTax, ParameterName="R_CUR_BILING_MEMO_COUPON_TAX_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemoCouponVat, ParameterName="R_CUR_BILING_MEMO_COUPON_VAT_O"},

                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemo, ParameterName="R_CUR_REJECTION_MEMO_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoCoupon, ParameterName="R_CUR_REJECTION_MEMO_COUPON_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoVat, ParameterName="R_CUR_REJECTION_MEMO_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoCouponTax, ParameterName="R_CUR_REJ_MEMO_COUPON_TAX_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoCouponVat, ParameterName="R_CUR_REJ_MEMO_COUPON_VAT_O"},
                

                //new SPResultObject{EntityName = LoadStrategy.Entities.SourceCode,ParameterName = "R_CUR_SourceCode_O"}, 

                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemo, ParameterName="R_CUR_CREDIT_MEMO_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoVat, ParameterName="R_CUR_CREDIT_MEMO_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoCoupon, ParameterName="R_CUR_CREDIT_MEMO_COUPON_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoCouponTax, ParameterName="R_CUR_CREDIT_MEMO_COUPON_TAX_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoCouponVat, ParameterName="R_CUR_CREDIT_MEMO_COUPON_VAT_O"},

                new SPResultObject{ EntityName=LoadStrategy.Entities.SamplingFormDRecord, ParameterName="R_CUR_FORM_D_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.SamplingFormDVat, ParameterName="R_CUR_FORM_D_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.SamplingFormDTax, ParameterName="R_CUR_FORM_D_TAX_O"},

                
                new SPResultObject{ EntityName=LoadStrategy.Entities.ProvisionalInvoiceDetails, ParameterName="R_CUR_PROV_INVOICE_DET_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.SamplingFormEDetails, ParameterName="R_CUR_FORM_E_DET_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.SamplingFormEDetailVat, ParameterName="R_CUR_FORM_E_DET_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotal, ParameterName="R_CUR_INVOICE_TOTAL_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotalVat, ParameterName="R_CUR_INVOICE_TOTAL_VAT_O"},
                
                new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeTotal, ParameterName = "R_CUR_SOURCE_CODE_TOTAL_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeTotalVat, ParameterName="R_CUR_SOURCE_CODE_TOTAL_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.ListingCurrency, ParameterName="R_CUR_LISTING_CURRENCY_O"},

                new SPResultObject{ EntityName=LoadStrategy.Entities.CouponAttachment, ParameterName="R_CUR_COUPON_RECORD_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemoAttachments, ParameterName="R_CUR_BILLING_MEMO_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BMCouponAttachments, ParameterName="R_CUR_BM_COUPON_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoAttachments, ParameterName="R_CUR_CREDIT_MEMO_ATTACHMENT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoCouponAttachments, ParameterName="R_CUR_CM_COUPON_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoAttachments, ParameterName="R_CUR_REJECTION_MEMO_ATTACH_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoCouponAttachments, ParameterName="R_CUR_RM_COUPON_ATTACH_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.SamplingFormDAttachment, ParameterName="R_CUR_FORM_D_ATTACH_O"},

                new SPResultObject{ EntityName=LoadStrategy.Entities.AutoBillCoupon, ParameterName="R_CUR_AUTOBILL_CPN_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.AutoBillCouponTax, ParameterName="R_CUR_AUTOBILL_CPNTAX_O"} ,
                new SPResultObject{ EntityName=LoadStrategy.Entities.AutoBillCouponVat, ParameterName="R_CUR_AUTOBILL_CPNVAT_O"} ,
                new SPResultObject{ EntityName=LoadStrategy.Entities.AutoBillCouponAttach, ParameterName="R_CUR_AUTOBILL_CPNATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CouponMarketingDetails, ParameterName="R_CUR_CPN_MKT_DTL_O"} 
           });
    }

  }

  public class GetPaxOldIdecInvoicesSP : StoredProcedure
  {
    /// <summary>
    /// Gets the procedure name.
    /// </summary>
    public override string Name
    {
      get
      {
        return "PROC_LOAD_PAX_INVOICES_OLDIDEC";
      }
    }

    /// <summary>
    /// Gets the result spec.
    /// </summary>
    /// <returns></returns>
    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.Entities.Invoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BilledMember, ParameterName="R_CUR_Billed_Member_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMember, ParameterName="R_CUR_BILLING_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.Coupon, ParameterName="R_CUR_COUPON_RECORD_O"},
                
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotal, ParameterName="R_CUR_INVOICE_TOTAL_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotalVat, ParameterName="R_CUR_INVOICE_TOTAL_VAT_O"},
                
                new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeTotal, ParameterName = "R_CUR_SOURCE_CODE_TOTAL_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeTotalVat, ParameterName="R_CUR_SOURCE_CODE_TOTAL_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.ListingCurrency, ParameterName="R_CUR_LISTING_CURRENCY_O"},
            });
    }

  }
    //cargo
  public class GetCargoOldIdecInvoicesSP : StoredProcedure
  {
      /// <summary>
      /// Gets the procedure name.
      /// </summary>
      public override string Name
      {
          get
          {
              return "PROC_LOAD_CGO_INVOICES_OLDIDEC";
          }
      }

      /// <summary>
      /// Gets the result spec.
      /// </summary>
      /// <returns></returns>
      public override List<SPResultObject> GetResultSpec()
      {
          return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.Entities.Invoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BilledMember, ParameterName="R_CUR_BILLED_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingMember, ParameterName="R_CUR_BILLING_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.AwbRecord, ParameterName="R_CUR_COUPON_RECORD_O"},
                
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.InvoiceTotal, ParameterName="R_CUR_INVOICE_TOTAL_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.InvoiceTotalVat, ParameterName="R_CUR_INVOICE_TOTAL_VAT_O"},
                
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingCodeSubTotal, ParameterName = "R_CUR_BILLING_CODE_TOTAL_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CargoBillingCodeSubTotalVat, ParameterName="R_CUR_BILLING_CODE_TOTAL_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.ListingCurrency, ParameterName="R_CUR_LISTING_CURRENCY_O"},
            });
      }

  }

  public class GetInvoicesSP : GetInvoiceSP
  {
    public override string Name
    {
      get
      {
        return "PROC_GET_INVOICES";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.Entities.Invoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.Coupon, ParameterName="R_CUR_CouponDataRecord_O"},
                new SPResultObject{EntityName = LoadStrategy.Entities.CouponTax, ParameterName = "R_CUR_CouponDataTaxRecord_O"}, 
                new SPResultObject{EntityName = LoadStrategy.Entities.CouponVat, ParameterName = "R_CUR_CouponDataVatRecord_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.MemberLocation, ParameterName="R_CUR_MemberLocation_O"},
                new SPResultObject{EntityName=LoadStrategy.Entities.SourceCodeTotal, ParameterName = "R_CUR_SourceCodeTotal_O"}, 
                new SPResultObject{EntityName = LoadStrategy.Entities.SourceCode,ParameterName = "R_CUR_SourceCode_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.Entities.BillingMemo, ParameterName="R_CUR_BillingMemo_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemo, ParameterName="R_CUR_CreditMemo_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemo, ParameterName="R_CUR_RejectionMemo_O"},
                //new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoCoupon, ParameterName="R_CUR_RM_COUPN_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotal, ParameterName="R_CUR_InvoiceTotal_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotalVat, ParameterName="R_CUR_InvoiceTotalVat_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotalVatIdentifier, ParameterName="R_CUR_INVTotalVatIdentifier_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CouponDataVatIdentifier, ParameterName="R_CUR_CPNDataVatIdentifier_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeVatIdentifier, ParameterName="R_CUR_SRCCodeVatIdentifier_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeVat, ParameterName="R_CUR_SourceCodeVatRecord_O"},
            });
    }
  }
  public class GetInvoiceHeaderSP : StoredProcedure
  {
    public const string ListingCurrencyEntity = "Currency";
    public const string BilledMemberEntity = "Member";

    public override string Name
    {
      get
      {
        return "PROC_GET_INV_HEADER";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.Entities.Invoice, IsMain=true, ParameterName="R_CUR_INV_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.InvoiceTotal, ParameterName="R_CUR_INV_TOT_O"},            
                new SPResultObject{ EntityName=ListingCurrencyEntity, ParameterName="R_CUR_CURRENCY_O"},
                new SPResultObject{ EntityName=BilledMemberEntity, ParameterName="R_CUR_MEM_O"},    
            });
    }

  }

  public class GetRMLinkingDetailsSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_RM_LINKING_DETAILS"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {                            
                new SPResultObject{EntityName = Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName,ParameterName = "R_CUR_MEMO_AMOUNTS_O"},
                new SPResultObject{EntityName = Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName,ParameterName = "R_CUR_MEMO_DET_O"}
            });
    }
  }

  public class GetCgoRMLinkingDetailsSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_CGO_RM_LINKING_DET"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new [] 
            {                            
                new SPResultObject{EntityName = Cargo.Impl.RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName,ParameterName = "R_CUR_MEMO_AMOUNTS_O"},
                new SPResultObject{EntityName = Cargo.Impl.RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName,ParameterName = "R_CUR_MEMO_DET_O"}
            });
    }
  }

  public class GetFormFLinkingDetailsSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_FORM_F_LINKING_DET"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {                            
                new SPResultObject{EntityName = Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName,ParameterName = "R_CUR_MEMO_AMOUNTS_O"},
                new SPResultObject{EntityName = Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName,ParameterName = "R_CUR_MEMO_DET_O"}
            });
    }
  }

  public class GetLinkedMemoAmountDetailsSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_PAX_MEMO_AMT_DETAIL"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
              new SPResultObject{EntityName = Pax.Impl.RejectionMemoRecordRepositoryConstants.RejMemoAmountsParameterName,ParameterName = "R_CUR_MEMO_AMOUNTS_O"}
            });
    }
  }

  public class GetCgoLinkedMemoAmountDetailsSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_CGO_MEMO_AMT_DETAIL"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new[] 
            {
              new SPResultObject{EntityName = Cargo.Impl.RejectionMemoRecordRepositoryConstants.RejMemoAmountsParameterName,ParameterName = "R_CUR_MEMO_AMOUNTS_O"}
            });
    }
  }

  public class GetRMCouponLinkingDetailsSp : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_PAX_RM_CPN_LINKIN_DET"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {                            
                new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponAmountsParameterName,ParameterName = "R_CUR_COUPON_AMOUNTS_O"},
                new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponListParameterName,ParameterName = "R_CUR_COUPON_LIST_O"},
                new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponTaxParameterName,ParameterName = "R_CUR_TAX_O"}
            });
    }
  }

  public class GetRMAwbLinkingDetailsSp : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_CGO_RM_CPN_LINKIN_DET"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new[] 
            {                            
                new SPResultObject{EntityName = Cargo.Impl.RMAwbRepositoryConstants.AwbAmountsParameterName, ParameterName = "R_CUR_AWB_AMOUNTS_O"},
                new SPResultObject{EntityName = Cargo.Impl.RMAwbRepositoryConstants.AwbListParameterName,ParameterName = "R_CUR_AWB_LIST_O"},
                new SPResultObject{EntityName = Cargo.Impl.RMAwbRepositoryConstants.AwbOtherChargeParameterName,ParameterName = "R_CUR_OC_AMT_O"}
            });
    }
  }

  /// <summary>
  /// Used on Form F.
  /// </summary>
  public class GetSamplingCouponLinkingDetailsSp : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_SMPL_CPN_LINKIN_DET"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {                            
                new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponAmountsParameterName,ParameterName = "R_CUR_COUPON_AMOUNTS_O"},
                new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponListParameterName,ParameterName = "R_CUR_COUPON_LIST_O"},
                new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponTaxParameterName,ParameterName = "R_CUR_TAX_O"}
            });
    }
  }

  public class GetLinkedCouponAmountDetailsSp : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_PAX_RM_CPN_DETAILS"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
              new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponAmountsParameterName,ParameterName = "R_CUR_COUPON_AMOUNTS_O"},
              new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponTaxParameterName,ParameterName = "R_CUR_TAX_O"}
            });
    }
  }

  public class GetLinkedAwbAmountDetailsSp : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_CGO_RM_AWB_DETAILS"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new[] 
            {
              new SPResultObject{EntityName = Cargo.Impl.RMAwbRepositoryConstants.AwbAmountsParameterName,ParameterName = "R_CUR_AWB_AMOUNTS_O"},
              new SPResultObject{EntityName =  Cargo.Impl.RMAwbRepositoryConstants.AwbOtherChargeParameterName,ParameterName = "R_CUR_OC_AMT_O"}
            });
    }
  }

  public class GetSamplingLinkedCouponAmountDetailsSp : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_GET_SMPL_CPN_DETAILS"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
              new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponAmountsParameterName,ParameterName = "R_CUR_COUPON_AMOUNTS_O"},
              new SPResultObject{EntityName = Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponTaxParameterName,ParameterName = "R_CUR_TAX_O"}
            });
    }
  }

  public class GetMiscInvoiceSP : StoredProcedure
  {
    public override string Name
    {
      get
      {
        return "PROC_LOAD_MU_INVOICES";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscInvoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.BilledMember, ParameterName="R_CUR_BILLED_MEMBER_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.BillingMember, ParameterName="R_CUR_BILLING_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.MemberLocation, ParameterName="R_CUR_MEMBER_LOC_INFO_O"},    
                new SPResultObject{ EntityName=LoadStrategy.Entities.MemberLocationInfoAddDetail, ParameterName="R_CUR_MEM_LOC_INFO_ADD_DET_O"},    
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ChargeCategory, ParameterName="R_CUR_CHARGE_CATEGORY_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.InvoiceSummary, ParameterName="R_CUR_INVOICE_SUMMARY_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItem, ParameterName="R_CUR_LINE_ITEM_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemChargeCode, ParameterName="R_CUR_LINE_ITEM_CHARGE_CODE_O"},   
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemUomCode, ParameterName="R_CUR_LINE_ITEM_UOM_CODE_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscTaxBreakdown, ParameterName="R_CUR_MISC_TAX_BREAKDOWN_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscUatpAttachment, ParameterName="R_CUR_ATTACHMENTS_O"},

                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge, ParameterName="R_CUR_ADD_ON_CHARGE_O"},   
                //new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail, ParameterName="R_CUR_ADDITIONAL_DETAIL_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MemberContact, ParameterName="R_CUR_MEMBER_CONTACT_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.PaymentDetail, ParameterName="R_CUR_PAYMENT_DETAIL_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ListingCurrency, ParameterName="R_CUR_LISTING_CURRENCY_O"},  
                new SPResultObject{ EntityName=LoadStrategy.Entities.Correspondence, ParameterName="R_CUR_CORRESPONDENCE_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondencesFromMember, ParameterName="R_CUR_CORR_FROM_MEMBER_O"},   
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondencesToMember, ParameterName="R_CUR_CORR_TO_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondenceAttachment, ParameterName="R_CUR_CORR_ATTACHMENT_O"},            
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondenceCurrency, ParameterName="R_CUR_CORR_CURRENCY_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.InvoiceOwner, ParameterName="R_CUR_INVOICE_OWNER_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ChargeCategoryChargeCode, ParameterName="R_CUR_CC_CHARGE_CODE_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType, ParameterName="R_CUR_CC_CHARGE_CODE_TYPE_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail, ParameterName="R_CUR_TAX_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail, ParameterName="R_CUR_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscTaxBreakdownCountry, ParameterName="R_CUR_TAX_COUNTRY_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.OtherOrganizationInformation, ParameterName="R_CUR_OTHER_ORG_INFO_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails, ParameterName="R_CUR_OTHER_ORG_ADD_DET_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.OtherOrganizationContactInformations, ParameterName="R_CUR_OTHER_ORG_INFO_CONTACT_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemChargeCodeType, ParameterName="R_CUR_LI_CHARGE_CODE_TYPE_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemAdditionalDetails, ParameterName="R_CUR_LINE_ITEM_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemAddOnCharges, ParameterName="R_CUR_LINE_ITEM_AOC_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemTaxBreakdown, ParameterName="R_CUR_LINE_ITEM_TAX_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails, ParameterName="R_CUR_LINE_ITEM_TAX_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemTaxCountry, ParameterName="R_CUR_LINE_ITEM_TAX_COUNTRY_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetails, ParameterName="R_CUR_LINE_ITEM_DETAIL_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailUomCode, ParameterName="R_CUR_LINE_ITEM_DET_UOM_CODE_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailAdditionalDet, ParameterName="R_CUR_LINE_ITEM_DET_ADD_DET_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailsFieldValues, ParameterName="R_CUR_LINE_ITEM_DET_FV_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData, ParameterName="R_CUR_LI_DET_FV_FM_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown, ParameterName="R_CUR_LI_DET_TAX_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailTaxAdditionalDetails, ParameterName="R_CUR_LI_DET_TAX_ADD_DET_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailTaxCountry, ParameterName="R_CUR_LI_DET_TAX_COUNTRY_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, ParameterName="R_CUR_LI_DET_AOC_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue, ParameterName="R_CUR_LI_DET_FV_ATTR_VAL_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue, ParameterName="R_CUR_LI_DET_FV_PARENT_VAL_O"},  
                 new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
                  new SPResultObject{EntityName = LoadStrategy.MiscEntities.LIDFMDataSource,ParameterName = "R_CUR_LID_FM_DATA_SOURCE_O"}, 
            });
    }
  }

  public class GetMiscIsWebInvoiceSP : StoredProcedure
  {
      public override string Name
      {
          get
          {
              return "PROC_LOAD_MISC_ISWEB_INVOICES";
          }
      }

      public override List<SPResultObject> GetResultSpec()
      {
          return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscInvoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.BilledMember, ParameterName="R_CUR_BILLED_MEMBER_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.BillingMember, ParameterName="R_CUR_BILLING_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.MemberLocation, ParameterName="R_CUR_MEMBER_LOC_INFO_O"},    
                new SPResultObject{ EntityName=LoadStrategy.Entities.MemberLocationInfoAddDetail, ParameterName="R_CUR_MEM_LOC_INFO_ADD_DET_O"},    
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ChargeCategory, ParameterName="R_CUR_CHARGE_CATEGORY_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.InvoiceSummary, ParameterName="R_CUR_INVOICE_SUMMARY_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItem, ParameterName="R_CUR_LINE_ITEM_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemChargeCode, ParameterName="R_CUR_LINE_ITEM_CHARGE_CODE_O"},   
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemUomCode, ParameterName="R_CUR_LINE_ITEM_UOM_CODE_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscTaxBreakdown, ParameterName="R_CUR_MISC_TAX_BREAKDOWN_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscUatpAttachment, ParameterName="R_CUR_ATTACHMENTS_O"},

                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge, ParameterName="R_CUR_ADD_ON_CHARGE_O"},   
                
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MemberContact, ParameterName="R_CUR_MEMBER_CONTACT_O"},            
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.PaymentDetail, ParameterName="R_CUR_PAYMENT_DETAIL_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ListingCurrency, ParameterName="R_CUR_LISTING_CURRENCY_O"},  
                new SPResultObject{ EntityName=LoadStrategy.Entities.Correspondence, ParameterName="R_CUR_CORRESPONDENCE_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondencesFromMember, ParameterName="R_CUR_CORR_FROM_MEMBER_O"},   
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondencesToMember, ParameterName="R_CUR_CORR_TO_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondenceAttachment, ParameterName="R_CUR_CORR_ATTACHMENT_O"},            
                new SPResultObject{ EntityName=LoadStrategy.Entities.CorrespondenceCurrency, ParameterName="R_CUR_CORR_CURRENCY_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.InvoiceOwner, ParameterName="R_CUR_INVOICE_OWNER_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ChargeCategoryChargeCode, ParameterName="R_CUR_CC_CHARGE_CODE_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType, ParameterName="R_CUR_CC_CHARGE_CODE_TYPE_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail, ParameterName="R_CUR_TAX_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail, ParameterName="R_CUR_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.MiscTaxBreakdownCountry, ParameterName="R_CUR_TAX_COUNTRY_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.OtherOrganizationInformation, ParameterName="R_CUR_OTHER_ORG_INFO_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails, ParameterName="R_CUR_OTHER_ORG_ADD_DET_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.OtherOrganizationContactInformations, ParameterName="R_CUR_OTHER_ORG_INFO_CONTACT_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemChargeCodeType, ParameterName="R_CUR_LI_CHARGE_CODE_TYPE_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemAdditionalDetails, ParameterName="R_CUR_LINE_ITEM_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemAddOnCharges, ParameterName="R_CUR_LINE_ITEM_AOC_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemTaxBreakdown, ParameterName="R_CUR_LINE_ITEM_TAX_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails, ParameterName="R_CUR_LINE_ITEM_TAX_ADD_DET_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemTaxCountry, ParameterName="R_CUR_LINE_ITEM_TAX_COUNTRY_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetails, ParameterName="R_CUR_LINE_ITEM_DETAIL_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailUomCode, ParameterName="R_CUR_LINE_ITEM_DET_UOM_CODE_O"},
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailAdditionalDet, ParameterName="R_CUR_LINE_ITEM_DET_ADD_DET_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailsFieldValues, ParameterName="R_CUR_LINE_ITEM_DET_FV_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData, ParameterName="R_CUR_LI_DET_FV_FM_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown, ParameterName="R_CUR_LI_DET_TAX_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailTaxAdditionalDetails, ParameterName="R_CUR_LI_DET_TAX_ADD_DET_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailTaxCountry, ParameterName="R_CUR_LI_DET_TAX_COUNTRY_O"}, 
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, ParameterName="R_CUR_LI_DET_AOC_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue, ParameterName="R_CUR_LI_DET_FV_ATTR_VAL_O"},  
                new SPResultObject{ EntityName=LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue, ParameterName="R_CUR_LI_DET_FV_PARENT_VAL_O"},  
                new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
                new SPResultObject{EntityName = LoadStrategy.MiscEntities.LIDFMDataSource,ParameterName = "R_CUR_LID_FM_DATA_SOURCE_O"}, 
            });
      }
  }

  public class GetLineItemSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_MISC_LINE_ITEM_DET"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItem,IsMain=true,ParameterName = "R_CUR_LINE_ITEM_O"},
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemChargeCode,ParameterName = "R_CUR_LINE_ITEM_CHARGE_CODE_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemUomCode,ParameterName = "R_CUR_LINE_ITEM_UOM_CODE_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemChargeCodeType,ParameterName = "R_CUR_LI_CHARGE_CODE_TYPE_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemDetails,ParameterName = "R_CUR_LINE_ITEM_DET_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemDetailsFieldValues,ParameterName = "R_CUR_LI_DET_FIELD_VALUE_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData,ParameterName = "R_CUR_LI_DET_FIELD_METADATA_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemTaxBreakdown,ParameterName = "R_CUR_LINE_ITEM_TAX_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemAddOnCharges,ParameterName = "R_CUR_LINE_ITEM_AOC_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemAdditionalDetails,ParameterName = "R_CUR_LINE_ITEM_ADD_DETAILS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemDetailUomCode,ParameterName = "R_CUR_LINE_ITEM_DETAIL_UOM_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,ParameterName = "R_CUR_LINE_ITEM_DETAIL_TAX_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemDetailAddOnCharges,ParameterName = "R_CUR_LINE_ITEM_DETAIL_AOC_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.MiscEntities.LineItemDetailAdditionalDet,ParameterName = "R_CUR_LINE_ITEM_DE_ADD_DET_O"} 
            });
    }
  }
  public class GetMiscCorrespondence : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_MISC_CORRESPONDENCE"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.Correspondence,IsMain=true,ParameterName = "R_CUR_CORR_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceAttachment,ParameterName = "R_CUR_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondencesToMember,ParameterName = "R_CUR_TO_MEMBER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondencesFromMember,ParameterName = "R_CUR_FROM_MEMBER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceCurrency,ParameterName = "R_CUR_CURRENCY_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceOwnerInfo,ParameterName = "R_CUR_CORR_USER_O"},
            });
    }
  }

  public class GetCorrespondence : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_PAX_CORRESPONDENCE"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.Correspondence,IsMain=true,ParameterName = "R_CUR_CORR_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondencesToMember,ParameterName = "R_CUR_TO_MEMBER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondencesFromMember,ParameterName = "R_CUR_FROM_MEMBER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceCurrency,ParameterName = "R_CUR_CURRENCY_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceAttachment,ParameterName = "R_CUR_ATTACHMENTS_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceOwnerInfo,ParameterName = "R_CUR_CORR_USER_O"},
            });
    }
  }

  public class GetFirstCorrespondence : StoredProcedure
  {
      public override string Name
      {
          get { return "PROC_GET_FIRST_PAX_CORR_DET"; }
      }

      public override List<SPResultObject> GetResultSpec()
      {
          return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.Correspondence,IsMain=true,ParameterName = "R_CUR_CORR_O"}
            });
      }
  }

  public class GetCargoCorrespondence : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_CGO_CORRESPONDENCE"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.Correspondence,IsMain=true,ParameterName = "R_CUR_CORR_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondencesToMember,ParameterName = "R_CUR_TO_MEMBER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondencesFromMember,ParameterName = "R_CUR_FROM_MEMBER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceCurrency,ParameterName = "R_CUR_CURRENCY_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceAttachment,ParameterName = "R_CUR_ATTACHMENTS_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceOwnerInfo,ParameterName = "R_CUR_CORR_USER_O"},
            });
    }
  }

  public class GetRejectionMemoSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_REJECTION_MEMO"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemo,IsMain=true,ParameterName = "R_CUR_REJECTION_MEMO"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoVat,ParameterName = "R_CUR_REJECTION_MEMO_VAT"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoVatIdentifier,ParameterName = "R_CUR_RM_VAT_IDENTIFIER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoAttachments,ParameterName = "R_CUR_RM_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoCoupon,ParameterName = "R_CUR_RM_COUPON_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoCouponVat,ParameterName = "R_CUR_RM_COUPON_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoCouponVatIdentifier,ParameterName = "R_CUR_RM_COUPON_VAT_ID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoCouponAttachments,ParameterName = "R_CUR_RM_COUPON_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoCouponTax,ParameterName = "R_CUR_RM_COUPON_TAX_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
            });
    }
  }

  public class GetCgotRejectionMemoSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_CGO_REJECTION_MEMO"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RejectionMemo,IsMain=true,ParameterName = "R_CUR_REJECTION_MEMO_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RejectionMemoVat,ParameterName = "R_CUR_REJECTION_MEMO_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.RejectionMemoVatIdentifier,ParameterName = "R_CUR_RM_VAT_IDENTIFIER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RejectionMemoAttachments,ParameterName = "R_CUR_RM_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwb,ParameterName = "R_CUR_RM_AWB_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbVat,ParameterName = "R_CUR_RM_AWB_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbVatIdentifier,ParameterName = "R_CUR_RM_AWB_VAT_ID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbAttachments,ParameterName = "R_CUR_RM_AWB_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbOtherCharges,ParameterName = "R_CUR_RM_AWB_OTHER_CHARGES_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AttachmentUploadedbyUser,ParameterName = "R_CUR_AWB_ATTACH_USER_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbProrateLadder, ParameterName="R_CUR_RM_AWB_PRORATE_LADDER_O"}
            });
    }
  }


  public class GetCreditMemoSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_CREDIT_MEMO"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemo,IsMain=true,ParameterName = "R_CUR_CREDIT_MEMO_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoVat,ParameterName = "R_CUR_CREDIT_MEMO_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoVatIdentifier,ParameterName = "R_CUR_CM_VAT_IDENTIFIER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoAttachments,ParameterName = "R_CUR_CM_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCoupon,ParameterName = "R_CUR_CM_COUPON_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCouponVat,ParameterName = "R_CUR_CM_COUPON_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCouponVatIdentifier,ParameterName = "R_CUR_CM_COUPON_VAT_ID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCouponAttachments,ParameterName = "R_CUR_CM_COUPON_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCouponTax,ParameterName = "R_CUR_CM_COUPON_TAX_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"},
            });
    }
  }


  public class GetBillingMemoSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_BILLING_MEMO"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemo,ParameterName = "R_CUR_BILLING_MEMO_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemoVat,ParameterName = "R_CUR_BILLING_MEMO_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemoVatIdentifier,ParameterName = "R_CUR_BM_VAT_IDENTIFIER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemoAttachments,ParameterName = "R_CUR_BM_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemoCoupon,ParameterName = "R_CUR_BM_COUPON_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemoCouponVat,ParameterName = "R_CUR_BM_COUPON_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.BMCouponVatIdentifier,ParameterName = "R_CUR_BM_COUPON_VAT_ID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.BMCouponAttachments,ParameterName = "R_CUR_BM_COUPON_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.BillingMemoCouponTax,ParameterName = "R_CUR_BM_COUPON_TAX_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}, 
            });
    }
  }

  public class GetPrimeCouponSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_PRIME_COUPON"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.Coupon,IsMain=true,ParameterName = "R_CUR_PRIME_COUPON_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CouponTax,ParameterName = "R_CUR_PRIME_COUPON_TAX_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CouponVat,ParameterName = "R_CUR_PRIME_COUPON_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CouponDataVatIdentifier,ParameterName = "R_CUR_PRIME_COUPON_VAT_VATID_O"},            
                            new SPResultObject{EntityName = LoadStrategy.Entities.CouponAttachment,ParameterName = "R_CUR_PRIME_COUPON_ATTACH_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser,ParameterName = "R_CUR_COUPON_ATTACH_USER_O"},
            });
    }

  }
  public class GetSamplingFormCSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_FORM_C_DETAILS"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormC,IsMain=true,ParameterName = "R_CUR_FORM_C_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCListingCurrency,ParameterName = "R_CUR_FORM_C_LISTING_CUR_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.ProvisionalBillingMember,ParameterName = "R_CUR_PROV_BILLING_MEMBER_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCFromMember,ParameterName = "R_CUR_FROM_MEMBER_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCDetails,ParameterName = "R_CUR_FORM_C_DETAIL_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCRecordAttachment, ParameterName = "R_CUR_FORM_C_DETAIL_ATTHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser, ParameterName = "R_CUR_COUPON_ATTACH_USER_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCSourceCodeTotal, ParameterName = "R_CUR_SOURCE_CODE_TOTAL_O"},
            });
    }
  }

  public class GetFormDRecordSP : StoredProcedure
  {
    public override string Name
    {
      get { return "PROC_LOAD_FORM_D_RECORD"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormDRecord,IsMain=true,ParameterName = "R_CUR_FORM_D_RECORD_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormDAttachment,ParameterName = "R_CUR_FORM_D_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormDTax,ParameterName = "R_CUR_FORM_D_TAX_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormDVat,ParameterName = "R_CUR_FORM_D_VAT_O"},            
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormDVatIdentifier,ParameterName = "R_CUR_FORM_D_VAT_VAT_ID_O"},
                             new SPResultObject{EntityName = LoadStrategy.Entities.AttachmentUploadedbyUser, ParameterName = "R_CUR_COUPON_ATTACH_USER_O"}
            });
    }
  }
}

public class GetPaxAuditSP : StoredProcedure
{
  public override string Name
  {
    get { return "PROC_GET_PAX_INV_AUDIT_TRAIL"; }
  }

  public override List<SPResultObject> GetResultSpec()
  {
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.PaxInvoice,IsMain=true,ParameterName = "R_CUR_INV_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.PrimeCoupon,ParameterName = "R_CUR_PM_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RejectionMemo,ParameterName = "R_CUR_RM_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BillingMemo,ParameterName = "R_CUR_BM_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemo, ParameterName = "R_CUR_CM_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RejectionMemoCoupon,ParameterName = "R_CUR_RM_CP_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BillingMemoCoupon,ParameterName = "R_CUR_BM_CP_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCoupon,ParameterName = "R_CUR_CM_CP_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.Correspondence,ParameterName = "R_CUR_COR_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.FormDCoupon,ParameterName = "R_CUR_FRM_D_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.Members,ParameterName = "R_CUR_MEMBERS_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.PrimecouponTax,ParameterName = "R_CUR_PM_TAX_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.PrimecouponVat,ParameterName = "R_CUR_PM_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RejectionMemoVAT,ParameterName = "R_CUR_RM_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BMVAT,ParameterName = "R_CUR_BM_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RMCouponTax,ParameterName = "R_CUR_RM_CP_TAX_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RMCouponVAT,ParameterName = "R_CUR_RM_CP_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BMCouponTax,ParameterName = "R_CUR_BM_CP_TAX_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BMCouponVAT,ParameterName = "R_CUR_BM_CP_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.PMAttachment,ParameterName = "R_CUR_PM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RMAttachment,ParameterName = "R_CUR_RM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.RMCouponAttachment,ParameterName = "R_CUR_RM_CP_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BMAttachment,ParameterName = "R_CUR_BM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.BMCouponATTACHMENT,ParameterName = "R_CUR_BM_CP_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoAttachments,ParameterName = "R_CUR_CM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CreditMemoCouponAttachments,ParameterName = "R_CUR_CM_CP_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormDAttachment,ParameterName = "R_CUR_FRMD_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.CorrespondenceAttachment,ParameterName = "R_CUR_COR_ATTACHMENT_O"},

                            new SPResultObject{EntityName = LoadStrategy.PaxEntities.Currency,ParameterName = "R_CUR_CURRENCY_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormC,ParameterName = "R_CUR_FRM_C_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCDetails,ParameterName = "R_CUR_FRM_C_DET_O"},
                            new SPResultObject{EntityName = LoadStrategy.Entities.SamplingFormCRecordAttachment,ParameterName = "R_CUR_FRM_C_ATTACHMENT_O"}
                            
            });
    }
  }


}



public class GetCargoInvoiceSP : StoredProcedure
{
  public override string Name
  {
    get
    {
      return "PROC_LOAD_CGO_INVOICE";
    }
  }

  public override List<SPResultObject> GetResultSpec()
  {
    return new List<SPResultObject>(new SPResultObject[] { 
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.Invoice, IsMain=true, ParameterName="R_CUR_INVOICE_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BilledMember, ParameterName="R_CUR_BILLED_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingMember, ParameterName="R_CUR_BILLING_MEMBER_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.MemberLocation, ParameterName="R_CUR_MEMBER_LOCATION_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CargoVatIdentifier, ParameterName="R_CUR_CGO_VAT_IDENTIFIER_O"},

                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingCodeSubTotal, ParameterName="R_CUR_BILLINGCODETOTAL_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CargoBillingCodeSubTotalVat, ParameterName="R_CUR_BILLINGCODETOTAL_VAT_O"},

                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.AwbRecord, ParameterName="R_CUR_AWB_RECORD_O"},
                new SPResultObject{ EntityName = LoadStrategy.CargoEntities.AwbRecordVat, ParameterName = "R_CUR_AWB_RECORD_VAT_O"}, 
                new SPResultObject{ EntityName = LoadStrategy.CargoEntities.AwbOtherCharge, ParameterName = "R_CUR_AWB_RECORD_OC_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.AwbAttachment, ParameterName="R_CUR_AWB_RECORD_ATTACH_O"},


                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RejectionMemo, ParameterName="R_CUR_REJECTION_MEMO_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RejectionMemoVat, ParameterName="R_CUR_REJECTION_MEMO_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RejectionMemoAttachments, ParameterName="R_CUR_REJECTION_MEMO_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RmAwb, ParameterName="R_CUR_REJECTION_MEMO_AWB_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RmAwbVat, ParameterName="R_CUR_REJ_MEMO_AWB_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RmAwbAttachments, ParameterName="R_CUR_RM_AWB_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RmAwbProrateLadder, ParameterName="R_CUR_RM_AWB_PRORATE_LADDER_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.RmAwbOtherCharges, ParameterName="R_CUR_RM_AWB_OC_O"},

                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingMemo, ParameterName="R_CUR_BILLING_MEMO_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingMemoVat, ParameterName="R_CUR_BILLING_MEMO_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BillingMemoAttachments, ParameterName="R_CUR_BILLING_MEMO_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BmAwb, ParameterName="R_CUR_BILLING_MEMO_AWB_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BmAwbVat, ParameterName="R_CUR_BM_AWB_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BmAwbAttachments, ParameterName="R_CUR_BM_AWB_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BmAwbProrateLadder, ParameterName="R_CUR_BM_AWB_PRORATE_LADDER_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.BmAwbOtherCharges, ParameterName="R_CUR_BM_AWB_OC_O"},
              
                new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemo, ParameterName="R_CUR_CREDIT_MEMO_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CreditMemoVat, ParameterName="R_CUR_CREDIT_MEMO_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CreditMemoAttachments, ParameterName="R_CUR_CREDIT_MEMO_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CmAwb, ParameterName="R_CUR_CREDIT_MEMO_AWB_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CmAwbVat, ParameterName="R_CUR_CM_AWB_VAT_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CmAwbAttachments, ParameterName="R_CUR_CM_AWB_ATTACH_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CmAwbProrateLadder, ParameterName="R_CUR_CM_AWB_PRORATE_LADDER_O"},
                 new SPResultObject{ EntityName=LoadStrategy.CargoEntities.CmAwbOtherCharges, ParameterName="R_CUR_CM_AWB_OC_O"},

                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.InvoiceTotal, ParameterName="R_CUR_INVOICE_TOTAL_O"},
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.InvoiceTotalVat, ParameterName="R_CUR_INVOICE_TOTAL_VAT_O"},

                
                new SPResultObject{ EntityName=LoadStrategy.CargoEntities.ListingCurrency, ParameterName="R_CUR_LISTING_CURRENCY_O"},
                
               // new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeTotal, ParameterName = "R_CUR_SOURCE_CODE_TOTAL_O"}, 
               // new SPResultObject{ EntityName=LoadStrategy.Entities.SourceCodeTotalVat, ParameterName="R_CUR_SOURCE_CODE_TOTAL_VAT_O"},
                

                
              
               // new SPResultObject{ EntityName=LoadStrategy.Entities.BMCouponAttachments, ParameterName="R_CUR_BM_COUPON_ATTACH_O"},
               // new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoAttachments, ParameterName="R_CUR_CREDIT_MEMO_ATTACHMENT_O"},
               // new SPResultObject{ EntityName=LoadStrategy.Entities.CreditMemoCouponAttachments, ParameterName="R_CUR_CM_COUPON_ATTACH_O"},
               //// new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoAttachments, ParameterName="R_CUR_REJECTION_MEMO_ATTACH_O"}, 
               // new SPResultObject{ EntityName=LoadStrategy.Entities.RejectionMemoCouponAttachments, ParameterName="R_CUR_RM_COUPON_ATTACH_O"}, 
               
            });
  }

}

public class GetCargoBillingMemoSP : StoredProcedure
{
  public override string Name
  {
    get { return "PROC_LOAD_CARGO_BILLING_MEMO"; }
  }

  public override List<SPResultObject> GetResultSpec()
  {
    return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemo,ParameterName = "R_CUR_BILLING_MEMO_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemoVat,ParameterName = "R_CUR_BILLING_MEMO_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemoVatIdentifier,ParameterName = "R_CUR_BM_VAT_IDENTIFIER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemoAttachments,ParameterName = "R_CUR_BM_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwb,ParameterName = "R_CUR_BM_AWB_RECORD_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbOtherCharges,ParameterName = "R_CUR_BM_AWB_RECORD_OC_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbVat,ParameterName = "R_CUR_BM_AWB_RECORD_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BMAwbVatIdentifier,ParameterName = "R_CUR_BM_AWB_VAT_VATID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbAttachments,ParameterName = "R_CUR_BM_AWB_RECORD_ATTACH_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BMAttachmentUploadedByUser,ParameterName = "R_CUR_BM_AWB_ATTACH_USER_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbProrateLadder,ParameterName = "R_CUR_BM_AWB_PRORATE_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbProrateLadderDetail,ParameterName = "R_CUR_BM_AWB_PRO_DET_O"},
                             
            });
  }
}

public class GetCargoCreditMemoSP : StoredProcedure
{
  public override string Name
  {
    get { return "PROC_LOAD_CARGO_CREDIT_MEMO"; }
  }

  public override List<SPResultObject> GetResultSpec()
  {
    return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CreditMemo,ParameterName = "R_CUR_CREDIT_MEMO_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CreditMemoVat,ParameterName = "R_CUR_CREDIT_MEMO_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CreditMemoVatIdentifier,ParameterName = "R_CUR_CM_VAT_IDENTIFIER_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CreditMemoAttachments,ParameterName = "R_CUR_CM_ATTACHMENTS_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwb,ParameterName = "R_CUR_CM_AWB_RECORD_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbOtherCharges,ParameterName = "R_CUR_CM_AWB_RECORD_OC_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbVat,ParameterName = "R_CUR_CM_AWB_RECORD_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbVatIdentifier,ParameterName = "R_CUR_CM_AWB_VAT_VATID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbAttachments,ParameterName = "R_CUR_CM_AWB_RECORD_ATTACH_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAttachmentUploadedByUser,ParameterName = "R_CUR_CM_AWB_ATTACH_USER_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbProrateLadder,ParameterName = "R_CUR_CM_AWB_PRORATE_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbProrateLadderDetail,ParameterName = "R_CUR_CM_AWB_PRO_DET_O"},
                             
            });
  }
}

public class GetCargoBmAwbSP : StoredProcedure
{
    public override string Name
    {
        get { return "PROC_LOAD_BM_AWB_RECORD"; }
    }

    public override List<SPResultObject> GetResultSpec()
    {
        return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwb,ParameterName = "R_CUR_BM_AWB_RECORD_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbOtherCharges,ParameterName = "R_CUR_BM_AWB_RECORD_OC_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbVat,ParameterName = "R_CUR_BM_AWB_RECORD_VAT_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BMAwbVatIdentifier,ParameterName = "R_CUR_BM_AWB_VAT_VATID_O"}, 
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbAttachments,ParameterName = "R_CUR_BM_AWB_RECORD_ATTACH_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BMAttachmentUploadedByUser,ParameterName = "R_CUR_BM_AWB_ATTACH_USER_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbProrateLadder,ParameterName = "R_CUR_BM_AWB_PRORATE_O"},
                             
            });
    }
}
public class GetAwbRecordSP : StoredProcedure
{
  public override string Name
  {
    get { return "PROC_LOAD_AWB_RECORD"; }
  }

  public override List<SPResultObject> GetResultSpec()
  {
    return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbRecord,IsMain=true,ParameterName = "R_CUR_AWB_RECORD_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbOtherCharge,ParameterName = "R_CUR_AWB_RECORD_OC_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbRecordVat,ParameterName = "R_CUR_AWB_RECORD_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbDataVatIdentifier,ParameterName = "R_CUR_AWB_RECORD_VAT_VATID_O"},            
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbAttachment,ParameterName = "R_CUR_AWB_RECORD_ATTACH_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AttachmentUploadedbyUser,ParameterName = "R_CUR_AWB_ATTACH_USER_O"},
            });
  }

}

public class GetCargoAuditSP : StoredProcedure
{
  public override string Name
  {
    get { return "PROC_GET_CGO_INV_AUDIT_TRAIL"; }
  }

  public override List<SPResultObject> GetResultSpec()
  {
    {
      return new List<SPResultObject>(new SPResultObject[] 
            {
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CargoInvoice,IsMain=true,ParameterName = "R_CUR_INV_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbRecord,ParameterName = "R_CUR_AWB_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RejectionMemo,ParameterName = "R_CUR_RM_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemo,ParameterName = "R_CUR_BM_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CreditMemo, ParameterName = "R_CUR_CM_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwb,ParameterName = "R_CUR_RM_AWB_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwb,ParameterName = "R_CUR_BM_AWB_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwb,ParameterName = "R_CUR_CM_AWB_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.Correspondence,ParameterName = "R_CUR_COR_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.Members,ParameterName = "R_CUR_MEMBERS_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbRecordVat,ParameterName = "R_CUR_AWB_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RejectionMemoVat,ParameterName = "R_CUR_RM_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemoVat,ParameterName = "R_CUR_BM_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbVat,ParameterName = "R_CUR_RM_AWB_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbVat,ParameterName = "R_CUR_BM_AWB_VAT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.AwbAttachment,ParameterName = "R_CUR_AWB_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RejectionMemoAttachments,ParameterName = "R_CUR_RM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.RmAwbAttachments,ParameterName = "R_CUR_RM_AWB_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BillingMemoAttachments,ParameterName = "R_CUR_BM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.BmAwbAttachments,ParameterName = "R_CUR_BM_AWB_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CreditMemoAttachments,ParameterName = "R_CUR_CM_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CmAwbAttachments,ParameterName = "R_CUR_CM_AWB_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.CorrespondenceAttachment,ParameterName = "R_CUR_COR_ATTACHMENT_O"},
                            new SPResultObject{EntityName = LoadStrategy.CargoEntities.Currency,ParameterName = "R_CUR_CURRENCY_O"}
                            
            });
    }
  }


}