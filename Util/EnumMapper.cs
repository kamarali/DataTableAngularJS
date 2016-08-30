using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;
using Iata.IS.Model;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Business.Reports.OfflineReportManger;

namespace Iata.IS.Web.Util
{
  public class EnumMapper
  {

    private static IOfflineReportLogManager _offlineReportLogManager;

    /// <summary>
    /// 
    /// </summary>
    private static IReferenceManager _referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

    /// <summary>
    /// HandlingTypeValues List.
    /// </summary>
    public static readonly Dictionary<string, string> HandlingTypeValues =
    new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.HandlingFeeType));

    public static readonly Dictionary<int, string> InvoiceStatusValues =
    new Dictionary<int, string>(_referenceManager.GetInvoiceStatusDList());

    public static readonly Dictionary<int, string> CgoInvoiceStatusValues =
    new Dictionary<int, string>(_referenceManager.GetCgoInvoiceStatusList());

    public static Dictionary<int, string> RejectionOnValidatonFailure =
    new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.ValidationFailureRej));

    public static Dictionary<string, string> TaxCategories = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.TaxCategory));

    public static readonly Dictionary<int, string> SettlementMethodValues = new Dictionary<int, string>(_referenceManager.GetSettlementMethodList());

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public static readonly Dictionary<int, string> BilateralSettlementMethodValues = new Dictionary<int, string>(_referenceManager.GetSmisTreatedAsBilateralList());

    public static readonly Dictionary<string, string> ResubmissionStatusValues =
    new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.ResubmissionStatus));

    public static readonly Dictionary<string, string> OtherChargeDic =
new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.CgoOtherChargeCode));
    // public static readonly Dictionary<int, string> OtherChargeDic = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.CgoOtherChargeCode));
    /// <summary>
    ///   Retrieves mapping for Ach Member Status
    /// </summary>
    public static Dictionary<int, string> AchCategory = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.AchMemberCategory));


    public static List<SelectListItem> GetHandlingFeeTypeList()
    {
      return HandlingTypeValues.Select(handlingFeeType => new SelectListItem
      {
        Value = handlingFeeType.Key,
        Text = handlingFeeType.Value
      }).ToList();
    }

    public static readonly Dictionary<string, string> FileSubmissionMethods
        = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.FileSubmissionMethod));
    /// <summary>
    ///   Returns File submission method list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetSubmissionMethodList()
    {
      return FileSubmissionMethods.Select(fileSubmissionMethod =>
          new SelectListItem
          {
            Value = fileSubmissionMethod.Key,
            Text = fileSubmissionMethod.Value
          }).ToList();
    }

    //CMP559 : Add Submission Method Column to Processing Dashboard

    public static Dictionary<string, string> InvoiceSubmissionMethod = new Dictionary<string, string>
                                                                        {
                                                                          { "IS-IDEC", "1" },
                                                                          { "IS-XML", "2" },
                                                                          { "IS-WEB", "3" },
                                                                           { "Auto-Billing", "4" },
                                                                        };

    public static List<SelectListItem> GetInvoiceSubmissionMethodList()
    {
        return InvoiceSubmissionMethod.Select(s => new SelectListItem
        {
            Text = s.Key,
            Value = s.Value
        }).ToList();
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public static Dictionary<string, string> DailyDeliveryStatus = new Dictionary<string, string>
                                                                         {
                                                                             {"Not Required", "0"},
                                                                             {"Pending", "1"},
                                                                             {"Completed", "2"},
                                                                         };
    public static List<SelectListItem> GetDailyDeliveryStatusList()
    {
        return DailyDeliveryStatus.Select(s => new SelectListItem
                                                   {
                                                       Text = s.Key,
                                                       Value = s.Value
                                                   }).ToList();
    }


    public static List<SelectListItem> GetDigitalSignatureList()
    {
      return EnumList.DigitalSignatureRequiredValues.Select(digitalSignature => new SelectListItem
      {
        Value = Convert.ToInt32(digitalSignature.Key).ToString(),
        Text = digitalSignature.Value
      }).ToList();
    }

    /// <summary>
    ///   Returns billing currency list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetBillingCurrencyList()
    {
      return EnumList.BillingCurrencyValues.Select(billingCurreny => new SelectListItem
                                                                     {
                                                                       Value = Convert.ToInt32(billingCurreny.Key).ToString(),
                                                                       Text = billingCurreny.Value
                                                                     }).ToList();
    }

    /// <summary>
    /// Gets the billing currency display value.
    /// </summary>
    /// <param name="billingCurrency">The billing currency.</param>
    /// <returns></returns>
    public static string GetBillingCurrencyDisplayValue(BillingCurrency? billingCurrency)
    {
      // Billing Currency can be null. If null then empty string will be returned.
      return billingCurrency == null ? string.Empty : EnumList.BillingCurrencyValues[billingCurrency.Value];
    }

    /// <summary>
    ///   Returns billing code list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetBillingCodeList()
    {
        return EnumList.BillingCodeValuesList.Select(billingCode => new SelectListItem
                                                                        {
                                                                            Value =
                                                                                Convert.ToInt32(billingCode.Key).
                                                                                ToString(),
                                                                            Text = billingCode.Value
                                                                        }).ToList();
    }

      /// <summary>
    /// Returns Carog Billing Code list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetCgoBillingCodeList()
    {
        return EnumList.CgoBillingCodeValues.Select(billingCode => new SelectListItem
        {
            Value = Convert.ToInt32(billingCode.Key).ToString(),
            Text = billingCode.Value
        }).ToList();
    }

    /// <summary>
    ///   Returns settlement method list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetSettlementMethodList(InvoiceType invoiceType)
    {
      if (invoiceType == InvoiceType.CreditNote)
      {
        return SettlementMethodValues.Select(settlementMethod => new SelectListItem
        {
          Value = Convert.ToInt32(settlementMethod.Key).ToString(),
          Text = settlementMethod.Value
        }).ToList().Where(item => item.Value != Convert.ToInt32(SMI.NoSettlement).ToString()).ToList();
      }
      else
      {
        return SettlementMethodValues.Select(settlementMethod => new SelectListItem
        {
          Value = Convert.ToInt32(settlementMethod.Key).ToString(),
          Text = settlementMethod.Value
        }).ToList().Where(item => item.Value != Convert.ToInt32(SMI.AdjustmentDueToProtest).ToString() && item.Value != Convert.ToInt32(SMI.NoSettlement).ToString()).ToList();
      }
    }

    public static List<SelectListItem> GetSettlementMethodList()
    {
      return SettlementMethodValues.Select(settlementMethod => new SelectListItem
                                                                          {
                                                                            Value = Convert.ToInt32(settlementMethod.Key).ToString(),
                                                                            Text = settlementMethod.Value
                                                                          }).ToList();
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public static List<SelectListItem> GetBilateralSettlementMethodList()
    {
        return BilateralSettlementMethodValues.Select(settlementMethod => new SelectListItem
        {
            Value = Convert.ToInt32(settlementMethod.Key).ToString(),
            Text = settlementMethod.Value
        }).ToList();
    }





    /// <summary>
    ///   Returns handling fee type list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetRejectionMemoStageList()
    {
      return EnumList.RejectionStageValues.Select(stage => new SelectListItem
      {
        Value = Convert.ToInt32(stage.Key).ToString(),
        Text = stage.Value
      }).ToList();
    }

    /// <summary>
    ///   Returns handling rejection reason code to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetRejectionReasonCodeList()
    {
        return EnumList.RejectionStageValues.Select(stage => new SelectListItem
        {
            Value = Convert.ToInt32(stage.Key).ToString(),
            Text = stage.Value
        }).ToList();
    }
    /// <summary>
    ///   Returns handling fee type list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetMiscRejectionMemoStageList()
    {
      return EnumList.MiscRejectionStageValues.Select(stage => new SelectListItem
      {
        Value = Convert.ToInt32(stage.Key).ToString(),
        Text = stage.Value
      }).ToList();
    }

    /// <summary>
    ///   Returns display value for given billing code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetBillingCodeDisplayValue(Iata.IS.Model.Pax.Enums.BillingCode key)
    {
      return EnumList.BillingCodeValues[key];
    }

    /// <summary>
    ///   Returns display value for given billing code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetCgoBillingCodeDisplayValue(Iata.IS.Model.Cargo.Enums.BillingCode key)
    {
        return EnumList.CgoBillingCodeValues[key];
    }
    
    /// <summary>
    ///   Returns display value for given settlement method value.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetSettlementMethodDisplayValue(int key)
    {
      return SettlementMethodValues[key];
    }


    /// <summary>
    ///   Returns invoice status list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetInvoiceStatusList()
    {
      return InvoiceStatusValues.Select(invoiceStatus => new SelectListItem
                                                                    {
                                                                      Value = Convert.ToInt32(invoiceStatus.Key).ToString(),
                                                                      Text = invoiceStatus.Value
                                                                    }).ToList();
    }

    /// <summary>
    ///   Returns invoice status list to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetCgoInvoiceStatusList()
    {
      return CgoInvoiceStatusValues.Select(invoiceStatus => new SelectListItem
      {
        Value = Convert.ToInt32(invoiceStatus.Key).ToString(),
        Text = invoiceStatus.Value
      }).ToList();
    }

    /// <summary>
    ///   Returns invoice status list for Form C to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetFormCInvoiceStatusList()
    {
      return EnumList.InvoiceStatusValuesForFormC.Select(invoiceStatus => new SelectListItem
                                                                            {
                                                                              Value = Convert.ToInt32(invoiceStatus.Key).ToString(),
                                                                              Text = invoiceStatus.Value
                                                                            }).ToList();
    }

    /// <summary>
    ///   Returns display value for given invoice status.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetInvoiceStatusDisplayValue(int key)
    {
      return InvoiceStatusValues[key];
    }


    /// <summary>
    ///   Returns display value for given output file delevery name.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetRejectionOnValidationFailureDisplayValue(int key)
    {
      return RejectionOnValidatonFailure[key];
    }

    /// <summary>
    ///   Returns Rejection On Validation Failure to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetRejectionOnValidationFailureList()
    {
      return RejectionOnValidatonFailure.Select(rejectionOnValidationFailure => new SelectListItem
                                                                                  {
                                                                                    Value = Convert.ToInt32(rejectionOnValidationFailure.Key).ToString(),
                                                                                    Text = rejectionOnValidationFailure.Value
                                                                                  }).ToList();
    }

    public static Dictionary<int, string> SamplingCarrierType = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.SamplingCarrierType));

    /// <summary>
    ///   Returns Sampling Carrier Type to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetSamplingCareerTypeList()
    {
      return SamplingCarrierType.Select(samplingcarrierType => new SelectListItem
                                                                 {
                                                                   Value = Convert.ToInt32(samplingcarrierType.Key).ToString(),
                                                                   Text = samplingcarrierType.Value
                                                                 }).ToList();
    }

    /// <summary>
    ///   Returns display value for given billing code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetSamplingCareerTypeDisplayValue(int key)
    {
      return SamplingCarrierType[key];
    }




    public static List<SelectListItem> GetTaxCategoryList()
    {
      return TaxCategories.Select(taxCategory => new SelectListItem
      {
        Value = taxCategory.Value,
        Text = taxCategory.Value
      }).ToList();
    }

    public static Dictionary<int, string> VatSubTypes = new Dictionary<int, string>(_referenceManager.GetTaxSubTypeList());

    public static List<SelectListItem> GetVatSubTypes()
    {
      return VatSubTypes.Select(vatSubType => new SelectListItem
      {
        Value = vatSubType.Value,
        Text = vatSubType.Value,
        Selected = vatSubType.Value == "VAT"
      }).ToList();
    }

    // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
    /// <summary>
    /// To make the list of Tax sub types to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetTaxSubTypes()
    {
      return _referenceManager.GetTaxSubTypeListForTaxTypeTax().Select(taxSubTypes => new SelectListItem
      {
        Value = taxSubTypes.Value,
        Text = taxSubTypes.Value
      }).ToList();
    }
    // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

    public static Dictionary<int, string> MigrationStatus = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.MigrationStatus));


    /// <summary>
    ///   Returns Migration Status to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetMigrationStatusList()
    {
      return MigrationStatus.Select(migrationStatus => new SelectListItem
                                                         {
                                                           Value = Convert.ToInt32(migrationStatus.Key).ToString(),
                                                           Text = migrationStatus.Value
                                                         }).ToList();
    }

    /// <summary>
    /// Returns display value for given billing code.
    /// </summary>
    /// <param name="migrationStatuskey">The migration status key.</param>
    /// <returns></returns>
    public static string GetMigrationStatusDisplayValue(int migrationStatuskey)
    {
      return MigrationStatus[migrationStatuskey];
    }

    public static Dictionary<int, string> IchMemberStatus = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.IchMemberStatus));


    /// <summary>
    /// Returns display value for given billing code.
    /// </summary>
    /// <param name="ichMemberShipStatusKey">The ICH member ship status key.</param>
    /// <returns></returns>
    public static string GetIchMemberShipStatusDisplayValue(int ichMemberShipStatusKey)
    {
      return IchMemberStatus[ichMemberShipStatusKey];
    }

    /// <summary>
    ///   Returns ICH Member Status to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetIchMembershipStatusList()
    {
      return IchMemberStatus.Select(memberStatus => new SelectListItem
                                                      {
                                                        Value = Convert.ToInt32(memberStatus.Key).ToString(),
                                                        Text = memberStatus.Value
                                                      }).ToList();
    }

    /// <summary>
    /// Retrieves ICH Zone and corresponding display value.
    /// </summary>
    public static Dictionary<int, string> IchZone = new Dictionary<int, string>(_referenceManager.GetIchZoneList());


    /// <summary>
    ///  Returns ICH Zone to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetIchZoneList()
    {
      return IchZone.Select(zoneStatus => new SelectListItem
                                            {
                                              Value = Convert.ToInt32(zoneStatus.Key).ToString(),
                                              Text = zoneStatus.Value
                                            }).ToList();
    }

    /// <summary>
    ///   Retrieves ICH Category and corresponding display value.
    /// </summary>
    public static Dictionary<int, string> IchCategory = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.IchMemberCategory));


    /// <summary>
    /// Returns ICH Category to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetIchCategoryList()
    {
      return IchCategory.Select(ichCategory => new SelectListItem
                                                 {
                                                   Value = Convert.ToInt32(ichCategory.Key).ToString(),
                                                   Text = ichCategory.Value
                                                 }).ToList();
    }

    /// <summary>
    ///   Holds mapping of Aggregated Type and corresponding display value.
    /// </summary>
    public static Dictionary<AggregatedType, string> AggregatedType = new Dictionary<AggregatedType, string>
                                                                        {
                                                                          { Model.MemberProfile.Enums.AggregatedType.Type1, "1" },
                                                                          { Model.MemberProfile.Enums.AggregatedType.Type2, "2" },
                                                                          { Model.MemberProfile.Enums.AggregatedType.Type3, "3" },
                                                                        };

    /// <summary>
    ///   Returns Aggregated Type List to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetaggregatedTypeList()
    {
      return AggregatedType.Select(aggregatedType => new SelectListItem
                                                       {
                                                         Value = Convert.ToInt32(aggregatedType.Key).ToString(),
                                                         Text = aggregatedType.Value
                                                       }).ToList();
    }

    /// <summary>
    ///   Holds mapping of Ich Web Reports Options and corresponding display value.
    /// </summary>
    public static Dictionary<IchWebReportOptions, string> IchWebReports = new Dictionary<IchWebReportOptions, string>
                                                                            {
                                                                              { IchWebReportOptions.Option0, "0" },
                                                                              { IchWebReportOptions.Option1, "1" },
                                                                              { IchWebReportOptions.Option2, "2" },
                                                                              { IchWebReportOptions.Option3, "3" },
                                                                              { IchWebReportOptions.Option4, "4" },
                                                                              { IchWebReportOptions.Option5, "5" },
                                                                              { IchWebReportOptions.Option6, "6" },
                                                                              { IchWebReportOptions.Option7, "7" },
                                                                              { IchWebReportOptions.Option8, "8" },
                                                                              { IchWebReportOptions.Option9, "9" },
                                                                            };

    /// <summary>
    ///   Returns Ich Web reports option List to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetIchWebReportList()
    {
      return IchWebReports.Select(ichWebReportsOptions => new SelectListItem
                                                            {
                                                              Value = Convert.ToInt32(ichWebReportsOptions.Key).ToString(),
                                                              Text = ichWebReportsOptions.Value
                                                            }).ToList();
    }

    /// <summary>
    ///   Get membership status values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetMembershipStatusList()
    {
      return MemberShipStatus.Select(memberStatus => new SelectListItem
                                                       {
                                                         Value = Convert.ToInt32(memberStatus.Key).ToString(),
                                                         Text = memberStatus.Value
                                                       }).ToList();
    }

    /// <summary>
    ///   Holds mapping of Member Status
    /// </summary>
    public static Dictionary<MemberStatus, string> MemberShipStatus = new Dictionary<MemberStatus, string>
                                                                        {
                                                                          { MemberStatus.Active, "Active" },
                                                                          { MemberStatus.Pending, "Pending" },
                                                                          { MemberStatus.Restricted, "Restricted" },
                                                                          { MemberStatus.Basic, "Basic" },
                                                                          { MemberStatus.Terminated, "Terminated" },
                                                                        };

    /// <summary>
    ///   Get membership sub status values
    /// </summary>
    /// <returns></returns>
    public static  List<SelectListItem> GetMembershipSubStatusList()
    {
      return (new Dictionary<int, string>(_referenceManager.GetAllMemberSubStatus())).Select(memberSubStatus => new SelectListItem
      {
        Value = Convert.ToInt32(memberSubStatus.Key).ToString(),
        Text = memberSubStatus.Value
      }).ToList();

      //CMP605: We have found two different ISmemebrship status in IS-WEB and SIS usage report
      //return MemberShipSubStatus.Select(memberSubStatus => new SelectListItem
      //{
      //  Value = Convert.ToInt32(memberSubStatus.Key).ToString(),
      //  Text = memberSubStatus.Value
      //}).ToList();
    }

    //CMP605: We have found two different ISmemebrship status in IS-WEB and SIS usage report
    //  private static Dictionary<int, string> MemberShipSubStatus = new Dictionary<int, string>(_referenceManager.GetAllMemberSubStatus());

    ///// <summary>
    /////   Holds mapping of Member sub Status
    ///// </summary>
    //public static Dictionary<MemberSubStatus, string> MemberShipSubStatus = new Dictionary<MemberSubStatus, string>
    //                                                                    {
    //                                                                      { MemberSubStatus.None, "None1" },
    //                                                                      { MemberSubStatus.ISPA, "ISPA1" },
    //                                                                      { MemberSubStatus.ISUA, "ISUA1" },
    //                                                                      { MemberSubStatus.TOU, "TOU1" },
    //                                                                      { MemberSubStatus.TOUEF, "TOU-E&F1" },
    //                                                                      { MemberSubStatus.Terminated, "Terminated1" },
    //                                                                    };

    /// <summary>
    /// Returns display value for given billing code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetMembershipSubStatusDisplayValue(int? key)
    {
      var memberShipSubStatusLocal = new Dictionary<int, string>(_referenceManager.GetAllMemberSubStatus());

      return key != null ? memberShipSubStatusLocal[(int)key] : string.Empty;
    }

    /// <summary>
    ///   Get ACH Membership status values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetAchMembershipStatusList()
    {
      return AchMemberShipStatus.Select(achMemberStatus => new SelectListItem
                                                             {
                                                               Value = Convert.ToInt32(achMemberStatus.Key).ToString(),
                                                               Text = achMemberStatus.Value
                                                             }).ToList();
    }

    /// <summary>
    ///   Holds mapping for Ach Member Status
    /// </summary>
    /// 
    public static Dictionary<int, string> AchMemberShipStatus =
      new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.AchMemberStatus));


    /// <summary>
    ///   Returns display value for given  Category.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>                                                                             
    public static string GetachCategoryDisplayValue(int key)
    {
      return AchCategory[key];
    }
    /// <summary>
    ///   Get ACH Category values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetachCategoryList()
    {
      return AchCategory.Select(achCategory => new SelectListItem
                                                 {
                                                   Value = Convert.ToInt32(achCategory.Key).ToString(),
                                                   Text = achCategory.Value
                                                 }).ToList();
    }

    /// <summary>
    /// Get Contact type values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetTypeOfContactTypeList()
    {
      return TypeOfContactType.Select(contactType => new SelectListItem
      {
        Value = Convert.ToInt32(contactType.Key).ToString(),
        Text = contactType.Value
      }).ToList();
    }



    /// <summary>
    /// Holds mapping for Contact Type
    /// </summary>
    public static Dictionary<TypeOfContactType, string> TypeOfContactType = new Dictionary<TypeOfContactType, string>
                                                                  {
                                                                    { Model.MemberProfile.Enums.TypeOfContactType.Informational, "Informational" },
                                                                    { Model.MemberProfile.Enums.TypeOfContactType.Processing, "Processing" },
                                                                   

                                                                  };
    /// <summary>
    ///   Returns display value for given output file delivery name.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetOutputFileDeliveryDisplayValue(int key)
    {
      return OutputFileDelivery[key];
    }

    /// <summary>
    ///   Get Output File Delivery values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetOutputFileDeliveryList()
    {
      return OutputFileDelivery.Select(outputFileDeliveryMethods => new SelectListItem
                                                                      {
                                                                        Value = Convert.ToInt32(outputFileDeliveryMethods.Key).ToString(),
                                                                        Text = outputFileDeliveryMethods.Value
                                                                      }).ToList();
    }

    /// <summary>
    ///   Holds mapping for Output File Delivery
    /// </summary>
    public static Dictionary<int, string> OutputFileDelivery = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.OutputFileDeliveryMethod));




    /// <summary>
    ///   Holds mapping for Delivery Preferences
    /// </summary>
    public static Dictionary<int, string> DeliveryPreferences = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.DeliveryPreference));


    /// <summary>
    ///   Returns display value for given output delivery preference.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetDeliveryPreferencesDisplayValue(int key)
    {
      return DeliveryPreferences[key];
    }

    /// <summary>
    ///   Get Delivery Preferences values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetDeliveryPreferencesList()
    {
      return DeliveryPreferences.Select(deliveryPreferences => new SelectListItem
                                                                 {
                                                                   Value = Convert.ToInt32(deliveryPreferences.Key).ToString(),
                                                                   Text = deliveryPreferences.Value
                                                                 }).ToList();
    }





    /// <summary>
    ///   Get Salutation values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetSalutationList()
    {
      return Saluation.Select(saluation => new SelectListItem
                                             {
                                               Value = Convert.ToInt32(saluation.Key).ToString(),
                                               Text = saluation.Value
                                             }).ToList();
    }

    /// <summary>
    ///   Returns display value for given salutation key.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetSaluationDisplayValue(Salutation key)
    {
      return Saluation[key];
    }

    /// <summary>
    ///   Holds mapping for Salutations
    /// </summary>
    public static Dictionary<Salutation, string> Saluation = new Dictionary<Salutation, string>
                                                               {
                                                                 {0,"Please Select"},
                                                                 { Salutation.Mr, "Mr" },
                                                                 { Salutation.Miss, "Miss" },
                                                                 { Salutation.Mrs, "Mrs" },
                                                                 { Salutation.Dr, "Dr" },
                                                                 { Salutation.Ms, "MS"},
                                                               };






    public static readonly Dictionary<int, string> SaluationDic = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.Salutation));
    public static List<SelectListItem> GetSalutationMiscCode()
    {
      return SaluationDic.Select(salutation => new SelectListItem
      {
        Value = Convert.ToInt32(salutation.Key).ToString(),
        Text = salutation.Value
      }).ToList();
    }



    /// <summary>
    ///   Get Contact Status values
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetContactStatusList()
    {
      return ContactStatus.Select(contactStatus => new SelectListItem
                                                     {
                                                       Value = Convert.ToInt32(contactStatus.Key).ToString(),
                                                       Text = contactStatus.Value
                                                     }).ToList();
    }

    /// <summary>
    ///   Holds mapping for Salutations
    /// </summary>
    public static Dictionary<ContactStatus, string> ContactStatus = new Dictionary<ContactStatus, string>
                                                                      {
                                                                        { Model.MemberProfile.Enums.ContactStatus.Active, "Active" },
                                                                        { Model.MemberProfile.Enums.ContactStatus.Inactive, "Inactive" },
                                                                      };


    /// <summary>
    ///   Returns Element Group list to display in Html Table format.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetElementGroupType()
    {
      return GroupElement.Select(elementGroup => new SelectListItem
      {
        Value = Convert.ToInt32(elementGroup.Key).ToString(),
        Text = elementGroup.Value
      }).ToList();
    }

    public static List<SelectListItem> GetInvoiceDownloadOptions(InvoiceDownloadOptions downloadOptions = InvoiceDownloadOptions.PaxPayables)
    {
      return EnumList.InvoiceDownloadOptionsValues.Where(downloadOption => (downloadOptions & downloadOption.Key) == downloadOption.Key)
        .Select(downloadOption => new SelectListItem
        {
          Value = Convert.ToInt32(downloadOption.Key).ToString(),
          Text = downloadOption.Value
        }).ToList();
    }

    public static readonly Dictionary<int, string> BillingTypeValues = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.BillingType));
    public static List<SelectListItem> GetBillingTypeList()
    {
      return BillingTypeValues.Select(billingType => new SelectListItem
      {
        Value = Convert.ToInt32(billingType.Key).ToString(),
        Text = billingType.Value
      }).ToList();
    }


    public static readonly Dictionary<string, string> CorrespondenceStatusValues
      = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.CorrespondenceStatus));
    public static List<SelectListItem> GetCorrespondenceStatusList()
    {
      return CorrespondenceStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static readonly Dictionary<string, string> CorrespondenceSubStatusValues
          = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.CorrespondenceSubStatus));
    public static List<SelectListItem> GetCorrespondenceSubStatusList()
    {
      return CorrespondenceSubStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static readonly Dictionary<string, string> FimBmCmIndicatorValues
        = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.FIMBMCMIndicator));
    public static List<SelectListItem> GetFimBmCmIndicatorList()
    {
      return FimBmCmIndicatorValues.Select(fimBmCm => new SelectListItem
      {
        Value = Convert.ToInt32(fimBmCm.Key).ToString(),
        Text = fimBmCm.Value
      }).ToList();
    }

    public static readonly Dictionary<string, string> BmCmIndicatorValues
       = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.BMCMIndicator));
    public static List<SelectListItem> GetBmCmIndicatorList()
    {
      return BmCmIndicatorValues.Select(bmCm => new SelectListItem
      {
        Value = Convert.ToInt32(bmCm.Key).ToString(),
        Text = bmCm.Value
      }).ToList();
    }

    public static List<SelectListItem> GetPaxCorrespondenceStatusList()
    {
      return EnumList.CorrespondenceStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }


    public static List<SelectListItem> GetPaxCorrespondenceSubStatusList()
    {
      return EnumList.CorrespondenceSubStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetCorrespondenceInitiatingMemberList()
    {
      return EnumList.CorrespondenceInitiatingMemberValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static readonly Dictionary<string, string> TransactionStatusValues
        = new Dictionary<string, string>(_referenceManager.GetMiscCodeString(MiscGroups.TransactionStatus));
    public static List<SelectListItem> GetTransactionStatusList()
    {
      return TransactionStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static readonly Dictionary<int, string> ContactTypeValues = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.ContactType));

    /// <summary>
    /// Gets the contact type list.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetContactTypeList()
    {
      return ContactTypeValues.Select(corrStatus => new SelectListItem
      {
        Value = corrStatus.Value,
        Text = corrStatus.Value
      }).ToList();
    }

    public static readonly Dictionary<int, string> BillingCategoryValues
     = new Dictionary<int, string>(_referenceManager.GetBillingCategoryList());
    public static List<SelectListItem> GetBillingCategorysList()
    {
      return BillingCategoryValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetClearanceTypesList()
    {
      return EnumList.ClearanceTypeValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetMonthsList()
    {
      return EnumList.MonthValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetInvoicePeriodList()
    {
      return EnumList.InvoicePeriodValues.Select(corrStatus => new SelectListItem
      {
        Value = corrStatus.Value,
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetSupportingDocTypeList()
    {
      return EnumList.SupportingDocTypeValues.Select(sdType => new SelectListItem
      {
        Value = Convert.ToInt32(sdType.Key).ToString(),
        Text = sdType.Value
      }).ToList();
    }

    public static List<SelectListItem> GetSupportingDocAttachIndicatorList()
    {
      return EnumList.SupportingDocAttachmentIndicatorValues.Select(sdType => new SelectListItem
      {
        Value = Convert.ToInt32(sdType.Key).ToString(),
        Text = sdType.Value
      }).ToList();
    }

    ///This is returns validated PMI dropdown list.
    public static List<SelectListItem> GetPaxCouponRecordList()
    {
      return EnumList.PaxCouponRecordValue.Select(corrStatus => new SelectListItem
      {
        Value = corrStatus.Value,
        Text = corrStatus.Value
      }).ToList();
    }

    /// <summary>
    /// Following method is used to retrieve values from CargoMemotypes enum
    /// </summary>
    /// <returns>Select List of Cargo memo types</returns>
    public static List<SelectListItem> GetCargoMemoList()
    {
      return EnumList.CargoMemoTypes.Select(memoType => new SelectListItem
      {
        Value = Convert.ToInt32(memoType.Key).ToString(),
        Text = memoType.Value
      }).ToList();
    }

    public static List<SelectListItem> GetMemoList()
    {
      return EnumList.MemoTypes.Select(memoType => new SelectListItem
      {
        Value = Convert.ToInt32(memoType.Key).ToString(),
        Text = memoType.Value
      }).ToList();
    }

    public static List<SelectListItem> GetMemoListForReport()
    {
      return EnumList.MemoTypesForReport.Select(memoType => new SelectListItem
      {
        Value = Convert.ToInt32(memoType.Key).ToString(),
        Text = memoType.Value
      }).ToList();
    }

    public static readonly Dictionary<int, string> FileStatusValues = new Dictionary<int, string>(_referenceManager.GetFileStatusList());

    public static List<SelectListItem> GetFileStatusList()
    {
      return FileStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetFileStatusListForProcessingDashboard()
    {
      /* SCP# 318756 - File status missing
       * Desc: Added 23 - Validation Completed status in drop down */
      var fileStatusIds = new string[] { "1", "4", "5", "18", "20", "23" };
      return FileStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).Where(f => fileStatusIds.Contains(f.Value)).ToList();
    }

    public static readonly Dictionary<int, string> FileFormatValues
  = new Dictionary<int, string>(_referenceManager.GetFileFormatList());
    public static List<SelectListItem> GetFileFormatList()
    {
      return FileFormatValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    public static List<SelectListItem> GetFileFormatListForProcessingDashboard()
    {
      var fileFormatIds = new string[] { "1", "2", "7" };
      return FileFormatValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).Where(f => fileFormatIds.Contains(f.Value)).ToList();
    }

    public static Dictionary<ElementGroupType, string> GroupElement = new Dictionary<ElementGroupType, string>
                                                                      {
                                                                        { ElementGroupType.MemberDetails, "Member Details" },
                                                                        { ElementGroupType.Locations, "Locations" },
                                                                        { ElementGroupType.Contacts, "Contacts" },
                                                                        { ElementGroupType.EBilling, "e-Billing" },
                                                                        { ElementGroupType.Pax, "Passenger" },
                                                                        { ElementGroupType.Cgo, "Cargo" },
                                                                        { ElementGroupType.Miscellaneous, "Miscellaneous" },
                                                                        { ElementGroupType.Uatp, "UATP" },
                                                                        { ElementGroupType.Ich, "ICH" },
                                                                        { ElementGroupType.Ach, "ACH" },
                                                                        { ElementGroupType.Technical, "Technical" }
                                                                        
                                                                      };

    /// <summary>
    ///   Returns display value for given ich zone code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetIchZoneDisplayValue(int key)
    {
      return IchZone[key];
    }

    /// <summary>
    ///   Returns display value for given ich category code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetIchCategoryDisplayValue(int key)
    {
      return IchCategory[key];
    }

    /// <summary>
    ///   Returns display value for given  aggregated code.
    /// </summary>
    /// <param name = "key"></param>
    /// <returns></returns>
    public static string GetAggregatedTypeDisplayValue(AggregatedType key)
    {
      return AggregatedType[key];
    }

    public static List<SelectListItem> GetResubmissionStatusList()
    {
      return ResubmissionStatusValues.Select(resubmissionStatus => new SelectListItem
      {
        Value = resubmissionStatus.Key,
        Text = resubmissionStatus.Value
      }).ToList();
    }



    /// <summary>
    ///   Holds mapping of User Category and corresponding display value.
    /// </summary>
    public static Dictionary<UserCategory, string> UserCategoryList = new Dictionary<UserCategory, string>
                                                                                  {
                                                                                    {
                                                                                      Model.MemberProfile.Enums.UserCategory.SisOps,
                                                                                      "IS- Admin/SIS Ops" 
                                                                                    },
                                                                                    {
                                                                                      Model.MemberProfile.Enums.UserCategory.IchOps,
                                                                                      "ICH Ops User"
                                                                                    },
                                                                                    {
                                                                                      Model.MemberProfile.Enums.UserCategory.AchOps,
                                                                                      "ACH Ops User"
                                                                                    },
                                                                                    {
                                                                                      Model.MemberProfile.Enums.UserCategory.Member,
                                                                                      "Member User"
                                                                                    },
                                                                                  };

    /// <summary>
    ///   Returns User Category to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetUserCategoryList()
    {
      return UserCategoryList.Select(listUserCategory => new SelectListItem
      {
        Value = Convert.ToUInt32(listUserCategory.Key).ToString(),
        Text = listUserCategory.Value

      }).ToList();
    }

    /// <summary>
    ///   Holds mapping of Invoice Payment Status Applicable For display value.
    /// </summary>
    public static Dictionary<InvPaymentStatusApplicableFor, string> InvPaymentApplicableForList = new Dictionary<InvPaymentStatusApplicableFor, string>
                                                                                  {
                                                                                    {
                                                                                      Model.Enums.InvPaymentStatusApplicableFor.BillingMember,
                                                                                      "Billing Member" 
                                                                                    },
                                                                                    {
                                                                                      Model.Enums.InvPaymentStatusApplicableFor.BilledMember,
                                                                                      "Billed Member"
                                                                                    }
                                                                                  };


    /// <summary>
    ///   Returns Invoice Payment Status Applicable For to display in dropdown.
    /// </summary>
    /// <returns></returns>
    public static List<SelectListItem> GetInvPaymentStatusApplicableForList()
    {
        return InvPaymentApplicableForList.Select(listInvPaymentStatusApplicable => new SelectListItem
        {
            Value = Convert.ToUInt32(listInvPaymentStatusApplicable.Key).ToString(),
            Text = listInvPaymentStatusApplicable.Value

        }).ToList();
    }




    public static readonly Dictionary<int, string> UserStatusDic = new Dictionary<int, string>(_referenceManager.GetMiscCode(MiscGroups.UserStatus));
    public static List<SelectListItem> GetUserStatusMiscCode()
    {
      return UserStatusDic.Select(status => new SelectListItem
      {
        Value = Convert.ToInt32(status.Key).ToString(),
        Text = status.Value
      }).ToList();
    }

    public static List<SelectListItem> GetInvoiceTypeList()
    {
      return EnumList.InvoiceTypeDictionary.Select(invoiceType => new SelectListItem
      {
        Value = Convert.ToInt32(invoiceType.Key).ToString(),
        Text = invoiceType.Value
      }).ToList();
    }

    public static List<SelectListItem> GetSettlementMethodStatusList()
    {
      return EnumList.SettlementMethodStatusValues.Select(corrStatus => new SelectListItem
      {
        Value = Convert.ToInt32(corrStatus.Key).ToString(),
        Text = corrStatus.Value
      }).ToList();
    }

    /// <summary>
    /// To get Error Type list
    /// </summary>
    public static List<SelectListItem> GetErrorTypeList()
    {
        return EnumList.ErrorTypeValues.Select(corrStatus => new SelectListItem
        {
            Value = Convert.ToInt32(corrStatus.Key).ToString(),
            Text = corrStatus.Value
        }).ToList();
    }

    /// <summary>
    /// To get Other Charge code list
    /// </summary>

    //public static List<SelectListItem> GetOtherChargeCodeMiscCode()
    //{
    //  return OtherChargeDic.Select(otherCharge => new SelectListItem
    //  {
    //    Value = Convert.ToInt32(otherCharge.Key).ToString(),
    //    Text = otherCharge.Value
    //  }).ToList();
    //}
    public static List<SelectListItem> GetOtherChargeCodeMiscCode()
    {
      return OtherChargeDic.Select(otherCharge => new SelectListItem
      {
        Value = otherCharge.Key,
        Text = otherCharge.Value
      }).ToList();
    }

      /// <summary>
      /// To get Applicable minimum field list
      /// </summary>
      /// <returns></returns>
     public static List<SelectListItem> GetApplicableMinimumField()
     {
         return EnumList.ApplicableAmountFieldValues.Select(othercharge => new SelectListItem()
                                                         {
                                                             Value = Convert.ToInt32(othercharge.Key).ToString(),
                                                             Text = othercharge.Value
                                                         }).ToList();
     }

     // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
     /// <summary>
     /// To get Product Id List.
     /// </summary>
     /// <returns> Product Id list</returns>
     public static List<SelectListItem> GetProductIdList()
     {
       return EnumList.ProductIdList.Select(productId => new SelectListItem()
       {
         Value = productId.Value,
         Text = productId.Value
       }).ToList();
     }
    // CMP # 533: RAM A13 New Validations and New Charge Code [End]


		 /// <summary>
		 /// This function is used to get list of offline report name.
		 /// </summary>
		 /// <returns></returns>
		 public static List<SelectListItem> GetOfflineReportList()
		 {
		 	try
		 	{
		 		_offlineReportLogManager = Ioc.Resolve<IOfflineReportLogManager>(typeof (IOfflineReportLogManager));

		 		var offlineReportNames = (from of in _offlineReportLogManager.GetAllOfflineReport()
		 		                          where of.IsActive
		 		                          select new SelectListItem
		 		                                 	{
		 		                                 		Value = of.Id.ToString(),
		 		                                 		Text = of.Name
		 		                                 	}).ToList();
        Ioc.Release(_offlineReportLogManager);
		 		return offlineReportNames;
		 	}
		 	catch
		 	{
					return new List<SelectListItem>();
		 	}
		 }
  }
}
