using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using TransactionStatus = Iata.IS.Model.Pax.Enums.TransactionStatus;

namespace Iata.IS.Model.MiscUatp
{
  /// <summary>
  /// Represents a miscellaneous invoice.
  /// </summary>
  public class MiscUatpInvoice : InvoiceBase
  {
    private BillingPeriod _settlementPeriod;

    /// <summary>
    /// Gets or sets the tax invoice number.
    /// </summary>
    /// <value>The tax invoice number.</value>
    public string TaxInvoiceNumber { get; set; }

    /// <summary>
    /// Gets or sets the tax point date.
    /// </summary>
    /// <value>The tax point date.</value>
    public DateTime TaxPointDate { get; set; }

    /// <summary>
    /// Gets or sets the charge category id.
    /// </summary>
    /// <value>The charge category id.</value>
    public int ChargeCategoryId { get; set; }

    /// <summary>
    /// Navigation property for charge category.
    /// </summary>
    public ChargeCategory ChargeCategory { get; set; }

    /// <summary>
    /// Display Name for Charge Category. 
    /// </summary>
    /// <remarks>
    /// Returns Name property of ChargeCategory
    /// returns empty string if ChargeCategory is null.
    /// </remarks>
    public string ChargeCategoryDisplayName {
      get
      {
        return ChargeCategory != null ? ChargeCategory.Name : string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the PO number.
    /// </summary>
    /// <value>The PO number.</value>
    public string PONumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is attachment indicator original.
    /// </summary>
    /// <value>
    /// true if this instance is attachment indicator original; otherwise, false.
    /// </value>
    public int AttachmentIndicatorOriginal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is attachment indicator validated.
    /// </summary>
    /// <value>
    /// true if this instance is attachment indicator validated; otherwise, false.
    /// </value>
    public bool? AttachmentIndicatorValidated { get; set; }

    /// <summary>
    /// Gets or sets the attachment number.
    /// </summary>
    /// <value>The attachment number.</value>
    public int? AttachmentNumber { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the additional details.
    /// </summary>
    /// <value>The additional details.</value>
    public List<MiscUatpInvoiceAdditionalDetail> AdditionalDetails { get; set; }
    
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    /// <value>The attachments.</value>
    public List<MiscUatpAttachment> Attachments { get; set; }
    
    /// <summary>
    /// Gets or sets the line items.
    /// </summary>
    /// <value>The line items.</value>
    public List<LineItem> LineItems { get; private set; }

    /// <summary>
    /// Gets or sets the invoice add on charges.
    /// </summary>
    /// <value>The add on charges.</value>
    public List<InvoiceAddOnCharge> AddOnCharges { get; private set; }

    /// <summary>
    /// Gets or sets the misc tax.
    /// </summary>
    /// <value>The misc tax.</value>
    public List<MiscUatpInvoiceTax> TaxBreakdown { get; private set; }

    /// <summary>
    /// Gets or sets the billing member contact information's.
    /// </summary>
    /// <value>The billing member contact information's.</value>
    public List<ContactInformation> MemberContacts { get; private set; }

    /// <summary>
    /// Gets or sets the name of the billing member contact.
    /// </summary>
    /// <value>The name of the billing member contact.</value>
    public string BillingMemberContactName { get; set; }

    /// <summary>
    /// Gets or sets the name of the billed member contact.
    /// </summary>
    /// <value>The name of the billed member contact.</value>
    public string BilledMemberContactName { get; set; }

    /// <summary>
    /// Gets or sets the rejected or correspondence invoice number.
    /// </summary>
    /// <value>The rejected invoice number.</value>
    public string RejectedInvoiceNumber { get; set; }

    /// <summary>
    /// Gets or sets the correspondence ref number.
    /// </summary>
    /// <value>The correspondence ref number.</value>
    public long? CorrespondenceRefNo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is authority to bill.
    /// </summary>
    /// <value>true if this instance is authority to bill; otherwise, false.</value>
    public bool IsAuthorityToBill { get; set; }

    /// <summary>
    /// Gets or sets the rejection stage.
    /// </summary>
    /// <value>The rejection stage.</value>
    public int RejectionStage { get; set; }

    /// <summary>
    /// Gets or sets the settlement period.
    /// </summary>
    /// <value>The settlement period.</value>
    public int SettlementPeriod { get; set; }

    /// <summary>
    /// Gets or sets the settlement month.
    /// </summary>
    /// <value>The settlement month.</value>
    public int SettlementMonth { get; set; }

    /// <summary>
    /// Gets or sets the settlement year.
    /// </summary>
    /// <value>The settlement year.</value>
    public int SettlementYear { get; set; }

    public BillingPeriod SettlementBillingPeriod
    {
      get
      {
        _settlementPeriod.Year = SettlementYear;
        _settlementPeriod.Month = SettlementMonth;
        _settlementPeriod.Period = SettlementPeriod;

        return _settlementPeriod;
      }
    }
    /// <summary>
    /// Gets or sets the location code.
    /// </summary>
    /// <value>The location code.</value>
    public string LocationCode { get; set; }

    /// <summary>
    /// Gets or sets the location code ICAO.
    /// </summary>
    /// <value>The location code ICAO.</value>
    public string LocationCodeIcao { get; set; }

    /// <summary>
    /// Navigational property for <see cref="MiscCorrespondence"/>
    /// </summary>
    public List<MiscCorrespondence> Correspondences { get; set; }

    /// <summary>
    /// Gets or sets the type of the invoice.
    /// </summary>
    /// <value>The type of the invoice.</value>
    public InvoiceType InvoiceType
    {
      get
      {
        return (InvoiceType)InvoiceTypeId;
      }
      set
      {
        InvoiceTypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the invoice type id.
    /// </summary>
    /// <value>The invoice type id.</value>
    public int InvoiceTypeId { get; set; }

    /// <summary>
    /// Gets the invoice type display text.
    /// </summary>
    /// <value>The invoice type display text.</value>
    public string InvoiceTypeDisplayText
    {
     get
     {
       return EnumList.InvoiceTypeDictionary.ContainsKey(InvoiceType) ? EnumList.InvoiceTypeDictionary[InvoiceType] : string.Empty;
     } 
    }

    /// <summary>
    /// Gets or sets IS-Validation flag for duplication of invoice or time limit. Organization
    /// </summary>
    /// <value>The is validation flag.</value>
    public string IsValidationFlag { get; set; }

    public string OtherOrganizationContactName { get; set; }

    public List<OtherOrganizationInformation> OtherOrganizationInformations { get; private set; }


    /// <summary>
    /// Gets the billing amount from Invoice Summary object.
    /// </summary>
    /// <value>The billing amount.</value>
    /// <remarks>
    /// This property is wrapper over TotalAmount property of <see cref="InvoiceSummary"/>.
    /// If InvoiceSummary property of MiscUatpInvoice is null, 0.0 will be return.
    /// </remarks>
    public decimal BillingAmount
    {
      get
      {
        return InvoiceSummary != null ? InvoiceSummary.TotalAmount : 0.0M;
      }
    }

    /// <summary>
    /// Gets the clearance amount from Invoice Summary object.
    /// </summary>
    /// <value>The clearance amount.</value>
    /// <remarks>
    /// This property is wrapper over TotalAmountInClearanceCurrency property of <see cref="InvoiceSummary"/>.
    /// If InvoiceSummary property of MiscUatpInvoice is null, 0.0 will be return.
    /// </remarks>
    /// CMP#648: Convert Clearance Amount into nullable. In case of Bilateral SMI it can hold null.
    public decimal? ClearanceAmount
    {
      get
      {
        return InvoiceSummary != null && InvoiceSummary.TotalAmountInClearanceCurrency.HasValue ? InvoiceSummary.TotalAmountInClearanceCurrency.Value : (decimal?) null;
      }
      set
      {
        InvoiceSummary.TotalAmountInClearanceCurrency = ClearanceAmount == 0 ? null : ClearanceAmount;
      }
    }

    //If an invoice is created using billing history screen
    public bool IsCreatedFromBillingHistory { set; get; }

    public IsValidationExceptionSummary isValidationExceptionSummary { get; set; }

    public List<ValidationExceptionSummary> ValidationExceptionSummary { get; set; }

    /// <summary>
    /// This flag will be set for Credit Note invoice
    /// </summary>
    public bool IsCreditNote { get; set; }

    /// <summary>
    /// Gets or sets the name of the location.
    /// </summary>
    /// <value>The name of the location.</value>
    public string LocationName { get; set; }

    /// <summary>
    /// To be used for parsing.
    /// </summary>
    public bool IsExchangeRateProvidedInXmlFile { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int InclusionStatusId { get; set; }

    public DateTime IsWebFileGenerationDate { get; set; }

    public DateTime? ExpiryDatePeriod { get; set; }

    /// <summary>
    /// These Charge Categories are used while validating parsed invoice Line Item.
    /// </summary>
    public List<ChargeCategory> ValidChargeCategoryList { get; set; }

    /// <summary>
    /// These Field Charge Code Mappings are used while validating LineItem
    /// </summary>
    public List<FieldChargeCodeMapping> ValidFieldChargeCodeMapping { get; set; }

    public MiscUatpInvoice()
    {
      AdditionalDetails = new List<MiscUatpInvoiceAdditionalDetail>();
      LineItems = new List<LineItem>();
      Attachments = new List<MiscUatpAttachment>();
      AddOnCharges = new List<InvoiceAddOnCharge>();
      TaxBreakdown = new List<MiscUatpInvoiceTax>();
      MemberContacts = new List<ContactInformation>();
      Correspondences = new List<MiscCorrespondence>();
      MemberLocationInformation = new List<MemberLocationInformation>();
      OtherOrganizationInformations = new List<OtherOrganizationInformation>();
    }

    /// <summary>
    ///  CMP #624: ICH Rewrite-New SMI X 
    ///  Used for mandatory check validation.
    /// </summary>
    public bool IsClearanceCurrencyInXmlFile { get; set; }

    //CMP#502: [3.6] IS-WEB: Save of Invoice Header of Rejection Invoices
    public string RejectionReasonCode { get; set; }

  }
}
