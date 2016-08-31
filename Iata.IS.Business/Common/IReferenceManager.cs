using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Business.Common
{
  /// <summary>
  /// 
  /// </summary>
  public interface IReferenceManager
  {
    /// <summary>
    /// Gets the reason code list.
    /// </summary>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <returns></returns>
    //IList<ReasonCode> GetReasonCodeList(int billingCode, int sourceCode);
    IList<ReasonCode> GetReasonCodeList(int transactionTypeId);
    /// <summary>
    /// Get reason code list.
    /// </summary>
    /// <param name="withDuplicate">if true then it will get distinct code and Description list else get all codes</param>
    /// <returns></returns>
    IList<ReasonCode> GetReasonCodeList(bool withDuplicate = false);

    IList<UserQuestion> GetUserQuestionList(int categoryId);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IList<Currency> GetCurrencyList();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IList<Model.Common.TransactionType> GetTransactionTypeList();

    IList<Model.Common.FileFormat> GetFileFormatTypeList();

    /// <summary>
    /// Gets the rfic list.
    /// </summary>
    /// <returns></returns>
    IList<Rfic> GetRficList();

    IList<MiscCodeGroup> GetMiscCodeGroupList();

    /// <summary>
    /// Gets the member status list.
    /// </summary>
    /// <returns></returns>
    IList<SisMemberStatus> GetMemberStatusList();

    /// <summary>
    /// CMP603:Member Profile-Changes in IS Membership Sub Status
    /// Get all MemberShip Sub status list from MST_SIS_MEMBER_SUB_STATUS table
    /// Author : Vinod Patil
    /// Date : 12 Feb 2014
    /// </summary>
    /// <returns>Dictionary object of int and string pair list</returns>
    Dictionary<int, string> GetAllMemberSubStatus();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IList<TaxCode> GetTaxCodeList();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IList<CityAirport> GetAirportsCodeList();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transactionTypeId"></param>
    /// <returns></returns>
    IList<SourceCode> GetSourceCodeList(int transactionTypeId);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IList<SourceCode> GetSourceCodeList();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MemberLocationInformation GetBillingLocationForInvoice(string invoiceId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MemberLocationInformation GetBilledLocationForInvoice(string invoiceId);

    /// <summary>
    /// This method returns country list.
    /// </summary>
    /// <returns>exchange rate</returns>
    IList<Country> GetCountryList();

    /// <summary>
    /// This method returns language list.
    /// </summary>
    IList<Language> GetLanguageList();

    /// <summary>
    /// This method returns subdivision code list.
    /// </summary>
    IList<SubDivision> GetSubDivisionList();

    /// <summary>
    /// This method returns exchange rate for selected listing currency and billing currency 
    /// </summary>
    /// <param name="listingCurrencyId">The listing currency id</param>
    /// <param name="billingCurrency">BillingCurrency enum </param>
    /// <param name="billingYear"></param>
    /// <param name="billingMonth"></param>
    /// <returns>exchange rate</returns>
    double GetExchangeRate(int listingCurrencyId, BillingCurrency billingCurrency, int billingYear, int billingMonth);

    /// <summary>
    /// Gets the source code details.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <returns>The Source Code object if source code exist else null.</returns>
    SourceCode GetSourceCodeDetails(int sourceCode);

    /// <summary>
    /// This method retrieves list of VAT identifiers available in database
    /// </summary>
    /// <returns>List of Vat identifier objects</returns>
    IList<VatIdentifier> GetVatIdentifierList(BillingCategoryType billingCategoryType);

    /// <summary>
    /// Gets the cgo vat identifier list.
    /// </summary>
    /// <param name="isOcApplicable">The is oc applicable.</param>
    /// <returns></returns>
    IList<CgoVatIdentifier> GetCgoVatIdentifierList(bool? isOcApplicable);

    /// <summary>
    /// Gets the vat identifier id.
    /// </summary>
    /// <param name="vatIdentifier">The vat identifier.</param>
    /// <param name="billingCategoryType">Type of the billing category.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    int GetVatIdentifierId(string vatIdentifier, BillingCategoryType billingCategoryType);


    /// <summary>
    /// Get Attachment Active file server details
    /// </summary>
    /// <returns></returns>
    FileServer GetActiveAttachmentServer();

    /// <summary>
    /// Get Unlinked Documents Active file server details
    /// </summary>
    /// <returns></returns>
    FileServer GetActiveUnlinkedDocumentsServer();

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name="currencyId">The currency id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidCurrency(int? currencyId);

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name="invoice">The invoice object.</param>
    /// <param name="currencyId">The currency id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidCurrency(InvoiceBase invoice, int? currencyId);

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name="currencyCode">The currency code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidCurrencyCode(string currencyCode);

    /// <summary>
    /// Determines whether [is valid currency code] [the specified invoice].
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="currencyCode">The currency code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid currency code] [the specified invoice]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidCurrencyCode(InvoiceBase invoice, string currencyCode);

    /// <summary>
    /// Validates the tax code.
    /// </summary>
    /// <param name="taxCode">The tax code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool IsValidTaxCode(string taxCode);

    /// <summary>
    /// Validates the tax code against the tax code dictionary in invoce object.
    /// </summary>
    /// <param name="invoice">Invoice object.</param>
    /// <param name="taxCode">The tax code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool IsValidTaxCode(InvoiceBase invoice, string taxCode);

    /// <summary>
    /// Determines whether [is valid airport code] [the specified airport code].
    /// </summary>
    /// <param name="airportCode">The airport code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid airport code] [the specified airport code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidAirportCode(string airportCode);

    /// <summary>
    /// Determines whether [is valid source code] [the specified source code id].
    /// </summary>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid source code] [the specified source code id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidSourceCode(int sourceCodeId, int transactionTypeId);

    /// <summary>
    /// Determines whether [is valid source code] [the specified invoice].
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid source code] [the specified invoice]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidSourceCode(InvoiceBase invoice, int sourceCodeId, int transactionTypeId);

    /// <summary>
    /// Determines whether [is valid reason code] [the specified reason code id].
    /// </summary>
    /// <param name="reasonCode">The reason code.</param>
    /// <param name="transactionTypeId"></param>
    /// <returns>
    /// 	<c>true</c> if [is valid reason code] [the specified reason code id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidReasonCode(string reasonCode, int transactionTypeId);

    bool IsValidRficCode(string rficCodeId);

    bool IsValidRfiscCode(string rfiscCodeId, string rficCodeId);

    /// <summary>
    /// Determines whether [is valid reason code] [the specified reason code id].
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="reasonCode">The reason code.</param>
    /// <param name="transactionTypeId"></param>
    /// <returns>
    /// 	<c>true</c> if [is valid reason code] [the specified reason code id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidReasonCode(InvoiceBase invoice, string reasonCode, int transactionTypeId);

    /// <summary>
    /// Determines whether [is valid vat identifier] [the specified vat identifier id].
    /// </summary>
    /// <param name="vatIdentifierId">The vat identifier id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid vat identifier] [the specified vat identifier id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidVatIdentifier(int vatIdentifierId);

    /// <summary>
    /// Determines whether [is valid country code] [the specified country code].
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid country code] [the specified country code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidCountryCode(string countryCode);

    /// <summary>
    /// Determines whether [is valid AircraftType code] [the specified AircraftType code].
    /// </summary>
    /// <param name = "aircraftTypeCode">The aircraftTypeCode code.</param>
    /// <returns>
    /// <c>true</c> if [is valid aircraftTypeCode code] [the specified aircraftTypeCode code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidAircraftTypeCode(string aircraftTypeCode);

    /// <summary>
    /// Determines whether [is valid AircraftTypeIcao code] [the specified AircraftTypeIcao code].
    /// </summary>
    /// <param name = "aircraftTypeIcaoCode">The AircraftTypeIcao code.</param>
    /// <returns>
    /// <c>true</c> if [is valid AircraftTypeIcao code] [the specified AircraftTypeIcao code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidAircraftTypeIcaoCode(string aircraftTypeIcaoCode);

    /// <summary>
    /// Determines whether [is valid source code] [the specified source code id].
    /// </summary>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid source code] [the specified source code id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidSourceCode(int sourceCodeId, out int transactionTypeId);

    /// <summary>
    /// Determines whether [is valid source code] [the specified invoice].
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid source code] [the specified invoice]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidSourceCode(InvoiceBase invoice, int sourceCodeId, out int transactionTypeId);

    /// <summary>
    /// Adds the is input file.
    /// </summary>
    /// <param name="isInputFile">The is input file.</param>
    /// <returns></returns>
    IsInputFile AddIsInputFile(IsInputFile isInputFile);


    /// <summary>
    /// Updates the is input file.
    /// </summary>
    /// <param name="isInputFile">The is input file.</param>
    /// <returns></returns>
    IsInputFile UpdateIsInputFile(IsInputFile isInputFile);

    /// <summary>
    /// Gets the is input file.
    /// </summary>
    /// <param name="isInputFileName">Name of the is input file.</param>
    /// <returns></returns>
    IsInputFile GetIsInputFile(string isInputFileName);

    /// <summary>
    /// Gets all is input file.
    /// </summary>
    /// <param name="isInputFileName">Name of the is input file.</param>
    /// <returns></returns>
    List<IsInputFile> GetAllIsInputFile(string isInputFileName);

    /// <summary>
    /// Determines whether [is valid net  amount] [the specified net amount].
    /// </summary>
    /// <param name="netAmount">The net amount.</param>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <param name="invoice">The invoicebase object</param>
    /// <param name="iExchangeRate">Exchange rate</param>
    /// <param name="validateMaxAmount">Valid max amount</param>
    /// <param name="validateMinAmount">Valid min amount</param>
    /// <param name="applicableMinimumField">Applicable min amount</param>
    /// <param name="rejectionReasonCode">Rejection reason code</param>
    /// <param name="iMinAcceptableAmount">Min acccepatable amount</param>
    /// <param name="iMaxAcceptableAmount">Max acceptable amount</param>
    ///<param name="isCorrespondence"></param>
    ///<param name="correspondence"></param>
    ///<param name="isCreditNote"></param>
    ///<returns>
    ///	<c>true</c> if [is valid net  amount] [the specified net amount]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidNetAmount(double netAmount, TransactionType transactionType, int? listingCurrencyId,
                          InvoiceBase invoice, ExchangeRate iExchangeRate = null, bool validateMinAmount = true,
                          bool validateMaxAmount = true, MinAcceptableAmount iMinAcceptableAmount = null,
                          ApplicableMinimumField? applicableMinimumField = null, string rejectionReasonCode = null, MaxAcceptableAmount iMaxAcceptableAmount = null, bool isCorrespondence = false, CorrespondenceBase correspondence = null, bool isCreditNote = false, bool isRejectionMemo = false);

    /// <summary>
    /// Gets the type of the transaction time limit transaction.
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns></returns>
    TimeLimit GetTransactionTimeLimitTransactionType(TransactionType transactionType, string clearingHouse);

    [Obsolete]
    TimeLimit GetTransactionTimeLimitTransactionType(Model.Enums.TransactionType transactionType, int settlementMethodId);
    /// <summary>
    /// Gets the additional detail list based on type (i.e. Additional Detail or notes).
    /// </summary>
    /// <param name="additionalDetailType">Type of the additional detail.</param>
    /// <param name="additionalDetailLevel">The additional detail level.</param>
    /// <returns></returns>
    IList<AdditionalDetail> GetAdditionalDetailList(AdditionalDetailType additionalDetailType, AdditionalDetailLevel additionalDetailLevel);

    /// <summary>
    /// Gets the charge category.
    /// </summary>
    /// <returns></returns>
    IList<ChargeCategory> GetChargeCategoryList(BillingCategoryType billingCategory);

    /// <summary>
    /// Overloaded method of GetChargeCategoryList() with isIncludeInactive parameter
    /// CMP609: MISC Changes Required as per ISW2 
    /// Gets the charge category.
    /// </summary>
    /// <param name = "billingCategory">Billing Category</param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns>IList of ChargeCategory</returns>
    IList<ChargeCategory> GetChargeCategoryList(BillingCategoryType billingCategory, bool isIncludeInactive);

    /// <summary>
    /// Gets the charge code.
    /// </summary>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    IList<ChargeCode> GetChargeCodeList(int chargeCategoryId);

    /// <summary>
    /// Gets the city airport.
    /// </summary>
    /// <returns></returns>
    IList<CityAirport> GetCityAirportList();

    /// <summary>
    /// Gets the uom code list.
    /// </summary>
    /// <returns></returns>
    IList<UomCode> GetUomCodeList();

    /// <summary>
    /// Gets the charge code detail.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    ChargeCode GetChargeCodeDetail(int chargeCodeId);

    /// <summary>
    /// Gets the type of the charge code.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    IList<ChargeCodeType> GetChargeCodeType(int chargeCodeId, bool isActiveChargeCodeType = false);

    /// <summary>
    /// Determines whether given cityAirport id exists in master.
    /// </summary>
    /// <param name="cityAirportCode">The city airport code.</param>
    /// <returns>
    /// true if cityAirport available in master otherwise, false.
    /// </returns>
    bool IsValidCityAirport(string cityAirportCode);

    /// <summary>
    /// Determines whether given Invoiceopcode exists in mst_misc_codes master.
    /// </summary>
    /// <param name = "invoiceOpCode"></param>
    /// <returns>
    /// true if cityAirport available in master otherwise, false.
    /// </returns>
    //CMP #636: Standard Update Mobilization
    bool IsValidMiscCode(string invoiceOpCode);

    /// <summary>
    /// Determines whether specified city code is valid city code.
    /// </summary>
    /// <param name="cityCode">The city code.</param>
    /// <returns>
    /// true if specified city code is valid city code; otherwise, false.
    /// </returns>
    bool IsValidCityCode(string cityCode);

    /// <summary>
    /// This method will get all contact type groups
    /// </summary>
    /// <returns>List of contact type groups</returns>
    List<ContactTypeGroup> GetContactTypeGroupList();

    /// <summary>
    /// This method will get contact type sub groups
    /// </summary>
    /// <param name="groupIdList">group id list</param>
    /// <returns>List of contact type subgroups</returns>
    List<ContactTypeSubGroup> GetContactTypeSubGroupList(List<int> groupIdList);

    /// <summary>
    /// Gets all contact type subgroups
    /// </summary>
    /// <returns></returns>
    List<ContactTypeSubGroup> GetAllContactTypeSubGroupList();

    /// <summary>
    /// Gets the rejection time limit in months.
    /// </summary>
    /// <param name="settlementMethod"></param>
    /// <param name="billingCategory"></param>
    /// <returns></returns>
    int GetRejectionTimeLimit(int settlementMethod, BillingCategoryType billingCategory);

    Dictionary<int, string> GetMiscCode(MiscGroups miscGroupName);
    Dictionary<string, string> GetMiscCodeString(MiscGroups miscGroupName);
    string GetDisplayValue(MiscGroups miscGroup, string miscCodeValue);
    string GetDisplayValue(MiscGroups miscGroup, int miscCodeValue);

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    string GetSettlementMethodDisplayValue(int settlementMethodId);

    /// <summary>
    /// Gets the invoice status display value.
    /// </summary>
    /// <param name="fileStatusId">The file status id.</param>
    /// <returns></returns>
    string GetFileStatusDisplayValue(int fileStatusId);

    /// <summary>
    /// Gets the invoice status display value.
    /// </summary>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <returns></returns>
    string GetInvoiceStatusDisplayValue(int invoiceStatusId);

    /// <summary>
    /// Gets the invoice status D list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetInvoiceStatusDList();

    /// <summary>
    /// Gets the invoice status list for Cargo.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetCgoInvoiceStatusList();

    /// <summary>
    /// Gets the settlement method list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetSettlementMethodList();

    /// <summary>
    /// Gets the file status list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetFileStatusList();


    /// <summary>
    /// Gets the file status list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetTaxSubTypeList();

    // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
    /// <summary>
    /// Gets the Tax Sub Type list when Tax Type is Tax.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetTaxSubTypeListForTaxTypeTax();
    // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

    /// <summary>
    /// Gets the Billing Category list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetBillingCategoryList();

    /// <summary>
    /// Gets the File Format list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetFileFormatList();

    /// <summary>
    /// Gets the IchZone list.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetIchZoneList();

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    string GetSettlementMethodDisplayValueForOutput(int settlementMethodId);

    /// <summary>
    /// Gets the settlement method list for parsing.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, string> GetBillingCategoryListForParsing();

    /// <summary>
    /// Gets the settlement method list for parsing.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, string> GetSettlementMethodListForParsing();

    /// <summary>
    /// Gets the settlement method list for parsing.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, string> GetSettlementMethodsForParsing();

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name="ichZone">The ich zone.</param>
    /// <returns></returns>
    string GetIchZoneDisplayValue(int ichZone);

    /// <summary>
    /// Gets the reason code.
    /// </summary>
    /// <param name="reasonCode">The reason code.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <returns></returns>
    ReasonCode GetReasonCode(string reasonCode, int transactionTypeId);

    /// <summary>
    /// Get clearing house from settlement method
    /// </summary>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    string GetClearingHouseFromSMI(int settlementMethodId);

    /// <summary>
    /// Gets the clearing house enum.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method id.</param>
    ClearingHouse GetClearingHouseEnum(int settlementMethodId);

    /// <summary>
    /// Add entry in is-file log and make the status of the file as Available for Download  
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="billingCategoryType"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="fileFormatType"></param>
    /// <param name="filePath"></param>
    /// <param name="fileSenderRecieverType"></param>
    /// <param name="isConsolidatedProvFile"></param>
    void AddOutputFileEntry(int memberId,
                          BillingCategoryType billingCategoryType,
                          BillingPeriod billingPeriod,
                          FileFormatType fileFormatType,
                          string filePath,
                          FileSenderRecieverType fileSenderRecieverType,
                          bool isConsolidatedProvFile = false);

    /// <summary>
    /// Gets the type of the transaction time limit transaction.
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <returns></returns>
    TimeLimit GetTransactionTimeLimitTransactionType(TransactionType transactionType, int settlementMethodId, DateTime yourBillingPeriod);


    TimeLimit GetTransactionTimeLimitTransactionType(Model.Enums.TransactionType transactionType, DateTime yourBillingPeriod);

    /// <summary>
    /// Determines whether given Country ICAO code is valid or not.
    /// </summary>
    /// <param name="countryCodeIcao">The country code.</param>
    /// <returns>
    /// true if country ICAO is valid; otherwise, false.
    /// </returns>
    bool IsValidCountryIcaoCode(string countryCodeIcao);

    /// <summary>
    /// Determines whether given Country ICAO code is valid or not.
    /// </summary>
    /// <param name="locationIcao">The location ICAO.</param>
    /// <returns>
    /// true if country ICAO is valid; otherwise, false.
    /// </returns>
    bool IsValidLocationIcaoCode(string locationIcao);


    /// <summary>
    /// Get IS_FILE_LOG files as per Status ID
    /// Author : Vinod Patil
    /// Date   :28 March 2011
    /// </summary>
    /// <param name="status"> int file status  </param>
    /// <returns> Collection of Files listing </returns>
    IQueryable<IsInputFile> GetIsInputFileByStatus(int status);

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    string GetSettlementMethodDisplayValueForSearchResult(int settlementMethodId);

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// The month/year portion of the time limit for the subsequent/follow-up transaction is calculated by adding ‘X’ number of months to the month of the previous transaction/uplift.
    /// The period portion of the time limit for the subsequent transaction will always be considered as 4.
    /// PAX - Prim billing rejection 1 and Misc - rejection 1
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="rejectionMemo">The rejection memo.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="currentBillingPeriod"></param>
    /// <returns>
    /// true if transaction in time limit ; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodA(TransactionType transactionType, int settlementMethodId, RejectionMemo rejectionMemo, InvoiceBase invoice, BillingPeriod? currentBillingPeriod);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of months is added to the month of the uplift/service delivery.
    /// The last calendar date of this month (newly calculated month) will be the time limit.
    /// The MISC invoice or PAX/CGO invoice containing the transaction should be validated successfully
    /// (i.e. status set as ‘Ready for Billing’) on/before the time limit date.
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// true if transaction in time limit ; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodA1(TransactionType transactionType, int settlementMethodId, InvoiceBase invoice);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of months is added to the date of successful validation of the previous MISC invoice or PAX/CGO invoice 
    /// containing the previous transaction (i.e. invoice status set as ‘Ready for Billing’). 
    /// The MISC rejection invoice or the PAX/CGO invoice containing the rejection should be validated successfully 
    /// (i.e. status set as ‘Ready for Billing’) on/before the time limit date.
    /// </summary>
    /// <param name="transactionType"></param>
    /// <param name="settlementMethodId"></param>
    /// <param name="invoice"></param>
    /// <returns></returns>
    bool IsTransactionInTimeLimitMethodA2(TransactionType transactionType, int settlementMethodId, InvoiceBase invoice);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// Closure date of the 4th period of the billing month of the final rejection is determined (from the IS Calendar).
    /// The actual period of the last rejection has no significance. ‘X’ number of months is added to that date to determine
    /// the time limit for the 1st correspondence.
    /// </summary>
    /// <param name="transactionType">Correspondence transaction type.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <param name="invoice">Invoice of correspondence#1 (i.e. 3rd rejection invoice)</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodB(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice, DateTime currentTransactionDate, ref bool isTimeLimitRecordFound);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of months is added to the date of successful validation of the final MISC rejection invoice or
    /// PAX/CGO invoice containing the final rejection (i.e. invoice status set as ‘Ready for Billing’).
    /// This shall be the time limit for the 1st correspondence.
    /// e.g. MISC Rejection Invoice #1  Correspondence #1
    /// PAX 3rd Rejection  Correspondence #1
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <param name="invoice">Misc final rejection/PAX containing final rejection stage.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodB1(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice, DateTime currentTransactionDate);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of months is added to the previous correspondence’s transmission/sending date.
    /// e.g.
    /// 1. MISC 1st correspondence  2nd correspondence
    /// 2. MISC 2nd correspondence  3rd correspondence
    /// 3. PAX 3rd correspondence  4th correspondence
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">PAX/Misc correspondence date.</param>
    /// <param name="previousCorrespondenceDate">Previous PAX/Misc correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodC(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, DateTime previousCorrespondenceDate);

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// The month/year portion of the time limit for the subsequent/follow-up transaction is calculated by adding ‘X’
    /// number of months to the month of the last correspondence that has not been responded to within time limits;
    /// or to the month of the correspondence that has an authority to bill.
    /// The period portion of the time limit for the subsequent transaction will always be considered as 4.
    /// e.g.
    /// 1. MISC 3rd Correspondence (expired)  Correspondence invoice
    /// 2. PAX 2nd Correspondence (having authority to bill)  BM
    /// 3. MISC 4th Correspondence (having authority to bill)  Correspondence invoice
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">PAX/Misc correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodD(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate);

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// ‘X’ number of months is added to the last correspondence’s transmission/sending date.
    /// The last calendar date of this month (newly calculated month) will be the time limit for the
    /// MISC correspondence invoice or the PAX/CGO invoice containing the BM. The MISC correspondence invoice or
    /// the PAX/CGO invoice containing the BM should be validated successfully (i.e. status set as ‘Ready for Billing’)
    /// on/before the time limit date.
    /// e.g.
    /// 1. MISC 3rd Correspondence (expired)  Correspondence invoice
    /// 2. PAX 2nd Correspondence (having authority to bill)  BM
    /// 3. MISC 4th Correspondence (having authority to bill)  Correspondence invoice
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">PAX/Misc correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodD1(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of months is added to the month of the final rejection. 
    /// The last calendar date of this month (newly calculated month) will be the time limit for the 1st correspondence. 
    /// The actual period of the last rejection has no significance.
    /// e.g. 
    /// 1. PAX 3rd Rejection  Correspondence #1
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="invoice">Invoice with final rejection.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodE(TransactionType transactionType, int settlementMethodId, InvoiceBase invoice, ref bool isTimeLimitRecordFound);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of months is added to the month of the final rejection. 
    /// The last calendar date of this month (newly calculated month) will be the time limit for the 1st correspondence. 
    /// The actual period of the last rejection has no significance.
    /// e.g. 
    /// 1. 2nd Rejection Invoice -> Correspondence #1
    ///   Billing period of 2nd Rejection: 2010-Jan-P2
    ///   Value of time limit: 90 days
    ///   Last date of Jan-2010: 31-Jan-2010
    ///   Time limit for the 1st Correspondence is calculated as 01-May-2010 (31-Jan-2010 + 90 days)
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="invoice">Invoice with final rejection.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodF(TransactionType transactionType, int settlementMethodId, InvoiceBase invoice, ref bool isTimeLimitRecordFound);

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ‘X’ number of days is added to the previous correspondence’s transmission/sending date.
    /// e.g.
    /// 1. Correspondence #2  Correspondence #3
    /// Date of Correspondence #2: 04-Feb-2013
    /// Vale of time limit: 60
    /// Time limit for the 3rd Correspondence is calculated as 05-Apr-2013 (04-Feb-2013 + 60 days)
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <param name="previousCorrespondenceDate">The previous correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodG(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, DateTime previousCorrespondenceDate);

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// The month/year portion of the time limit for the subsequent/follow-up transaction is calculated by adding ‘X’ number of months to the month of the previous transaction/uplift.
    /// The period portion of the time limit for the subsequent transaction will always be considered as 4.
    /// PAX - Prim billing rejection 1 and Misc - rejection 1
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// true if transaction in time limit ; otherwise, false.
    /// </returns>
    bool IsTransactionInTimeLimitMethodA(TransactionType transactionType, int settlementMethodId, MiscUatpInvoice invoice);

    /// <summary>
    /// Checks if the transaction is in time limit or not. Used in Pax to display warning message.
    /// </summary>
    /// <param name="transactionType"></param>
    /// <param name="settlementMethodId">SMI of the invoice being rejected.</param>
    /// <param name="billingYear">Billing Year of the invoice being rejected.</param>
    /// <param name="billingMonth">Billing month of the invoice being rejected.</param>
    /// <param name="billingPeriod">Billing period of the invoice being rejected.</param>
    /// <returns></returns>
    bool IsTransactionInTimeLimitMethodH(TransactionType transactionType, int settlementMethodId, int billingYear,
                                         int billingMonth, int billingPeriod);

    /// <summary>
    /// Gets the time limit method expiry date, for method C and D
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns></returns>
    DateTime GetTimeLimitMethodExpiryDate(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice);

    DateTime GetTimeLimitMethodExpiryDateMethodG(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice);
    /// <summary>
    /// Gets the time limit expiry date for method D1.
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <param name="invoice">invoice.</param>
    /// <returns></returns>
    DateTime GetTimeLimitMethodD1ExpiryDate(TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice);

    /// <summary>
    /// Get vat identifier id from vat identifier code
    /// </summary>
    /// <param name="vatIdentifierId"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    string GetVatIdentifierCode(int vatIdentifierId);

    /// <summary>
    /// Gets the is input file.
    /// </summary>
    /// <param name="isInputFileId"></param>
    /// <returns></returns>
    IsInputFile GetIsInputFileFromIsFileId(Guid isInputFileId);

    /// <summary>
    /// Determines whether [is valid country code] [the specified country code].
    /// </summary>
    /// <param name="invoiceBase"></param>
    /// <param name="countryCode">The country code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid country code] [the specified country code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidCountryCode(InvoiceBase invoiceBase, string countryCode);

    /// <summary>
    /// CMP496: Get country name by country code. 
    /// </summary>
    /// <param name="countryCode">valid country code</param>
    /// <returns></returns>
    string GetCountryNameByCode(string countryCode);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="billingMember"></param>
    /// <param name="billedMember"></param>
    /// <returns></returns>
    string GetClearingHouseForInvoice(InvoiceBase invoice, Member billingMember, Member billedMember);

    SMI GetDefaultSettlementMethodForMembers(int billingMemberId, int billedMemberId, int billingCategoryId);

    void AddIsFtpLog(IsftpLog isftpLog);

    bool IsTransactionInTimeLimitMethodA(TransactionType transactionType, int settlementMethodId,
                                                CargoRejectionMemo rejectionMemo, InvoiceBase invoice,
                                                BillingPeriod? currentBillingPeriod);

    IsftpLog GetFTPLog(Guid isFileId);

    /// <summary>
    /// Get UOM Code type list
    /// </summary>
    /// <returns></returns>
    IList<MiscCode> GetUomCodeTypeList();

    /// <summary>
    /// Gets the clearing house enum.
    /// </summary>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    ClearingHouse GetClearingHouseToFetchCurrentBillingPeriod(int settlementMethodId);

    IList<FileFormat> SysMonitorGetFileFormatTypeList();

    /// <summary>
    /// Get Cargo vat identifier code from vat identifier id
    /// </summary>
    /// <param name="vatIdentifierId"></param>
    /// <returns></returns>
    string GetCgoVatIdentifierCode(int vatIdentifierId);
    string GetChargeCategoryName(int chargeCategoryId);

      /// <summary>
      /// Check if the given SMI is to be treated like 'Bilateral' SMI.
      /// </summary>
      /// <param name="settlementMethodId">The settlement method id.</param>
      /// <returns>True if SMI is to be treated like Bilateral, false otherwise.</returns>
      /// CMP #624: ICH Rewrite-New SMI X 
      bool IsSmiLikeBilateral(int settlementMethodId, bool isSmiXbehaveLikeBilateral);

    /// <summary>
    /// Check if SMI is valid for Late Submission    
    /// </summary>
    /// <param name="settlementMethodId">SMI</param>
    /// <returns>True/False</returns>
      bool IsValidSmiForLateSubmission(int settlementMethodId);

    /// <summary>
    /// Returns 'Bilateral' SMI for all SMIs that are to be treated like Bilateral. To be used in case statements.
    /// </summary>
    /// <param name="smi"></param>
    /// <returns></returns>
    SMI GetBilateralSmi(int smi);

    /// <summary>
    /// Get SMI list which are are to be treated like 'Bilateral', including Bilateral SMI.
    /// </summary>
    /// <returns></returns>
    List<int> GetSMIsToBeTreatedBilateral();

    /// <summary>
    /// Gets the lead period.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="yourBillingPeriod">Your billing period.</param>
    /// <returns></returns>
    LeadPeriod GetLeadPeriod(string clearingHouse, int billingCategoryId, DateTime yourBillingPeriod);

    DateTime GetExpiryDatePeriodMethod(Model.Enums.TransactionType transactionType, InvoiceBase invoice,
                                       BillingCategoryType billingCategory, string samplingIndicator,
                                       BillingPeriod? currentBillingPeriod, bool isCorrespondence = false,
                                       CorrespondenceBase correspondenceBase = null);

    bool IsValidMemberNumbericCode(string memberCodeNumeric);

    bool IsValidMemberAlphaCode(string memberCodeAlpha);

    DateTime GetExpiryDatePeriodForClosedCorrespondence(InvoiceBase invoice, BillingCategoryType billingCategory, string samplingIndicator, BillingPeriod? currentBillingPeriod);

    /// <summary>
    /// To Validate abillSourCode
    /// </summary>
    /// <param name="sourceCodes"></param>
    /// <param name="sourceCodeId"></param>
    /// <param name="transactionTypeId"></param>
    /// <returns></returns>
    bool IsValidABillSourceCode(IList<SourceCode> sourceCodes, int sourceCodeId, int transactionTypeId);

    /// <summary>
    /// Gets subdivision code list based on country code.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    List<string> GetSubdivisionCodesByCountryCode(string countryCode);

    /// <summary>
    /// Gets Country Icao List
    /// </summary>
    /// <returns></returns>
    IList<CountryIcao> GetCountryIcaoList();

    /// <summary>
    /// Get debugLog object 
    /// </summary>
    /// <param name="logDate"></param>
    /// <param name="logMethodName"></param>
    /// <param name="logClassName"></param>
    /// <param name="logCategory"></param>
    /// <param name="logText"></param>
    /// <param name="logUserId"></param>
    /// /// <param name="refId"></param>
    /// <returns></returns>
    DebugLog GetDebugLog(DateTime logDate, string logMethodName, string logClassName, string logCategory, string logText, int logUserId, string refId);

    //SCP210204: IS-WEB Outage
    /// <summary>
    /// Inserts the log debug data.
    /// </summary>
    /// <param name="log">The log.</param>
    void InsertLogDebugData(DebugLog log);

    /// <summary>
    /// Add debug log to database
    /// </summary>
    /// <param name="log">DebugLog object having the details to be logged </param>
    void LogDebugData(DebugLog log);

    /// CMP523 - Source Code in RMBMCM Summary Report
    IList<SourceCode> GetSourceCodesList(int transactionType);

    //CMP508-Audit Trail Download with Supporting Documents
    /// <summary>
    /// Enqueue message in AQ_TRAN_TRAIL_REPORT
    /// </summary>
    /// <param name="message">Message containing values to be enqueued</param>
    // SCP227747: Cargo Invoice Data Download
    // Change return type of method to return boolean value.
    bool EnqueTransactionTrailReport(ReportDownloadRequestMessage message);

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    Dictionary<int, string> GetSmisTreatedAsBilateralList();

    //SCP219674 : InvalidAmountToBeSettled Validation
    /// <summary>
    /// Validate Correspondence Amount to be Settled
    /// </summary>
    /// <param name="billingMemoInvoice"> Invoice Base </param>
    /// <param name="bmNetAmountBilled">Billing Memo Net Amount Billed</param>
    /// <param name="corrCurrencyId">Correspondence Currency Id</param>
    /// <param name="corrAmountToBeSettled">Correspondence Amount to be Settled</param>
    /// <returns>true/false</returns>
    bool ValidateCorrespondenceAmounttobeSettled(InvoiceBase billingMemoInvoice,
                                                 ref decimal bmNetAmountBilled,
                                                 int corrCurrencyId,
                                                 decimal corrAmountToBeSettled,
                                                 InvoiceBase originalInvoice = null);
    /// <summary>
    /// This function is used to get list of charge category for master charge code type name.
    /// </summary>
    /// <param name="billingCategory"></param>
    /// <param name="isIncludeInactive"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    IList<ChargeCategory> GetChargeCategoriesForMstChargeCodeType(bool isActiveChargeCodeTypeReq);

    /// <summary>
    /// This function is used to get list of charge code for master charge code type name.
    /// </summary>
    /// <param name="billingCategory"></param>
    /// <param name="isIncludeInactive"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    IList<ChargeCode> GetChargeCodeListForMstChargeCodeType(int chargeCategoryId, bool isActiveChargeCodeTypeReq);

    //CMP #641: Time Limit Validation on Third Stage PAX Rejections
    /// <summary>
    /// Gets the transaction time limit for pax rm stage three.
    /// </summary>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="yourBillingPeriod">Your billing period.</param>
    /// <returns></returns>
    TimeLimit GetTransactionTimeLimitForPaxRmStageThree(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime yourBillingPeriod);


    /// <summary>
    /// ID : 362066 - Supporting Document Missing
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    bool IsRecordExistIn_Aq_SanityCheck(string fileName);

    /// <summary>
    /// This function is used to get currency code name based on currency code.
    /// CMP-553-ACH Requirement for Multiple Currency Handling
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    string GetCurrencyCodeName(Int32 currencyCode);

    /// <summary>
    /// Gets all the Currency except USD with order by Asc.
    /// </summary>
    /// <returns>List of currency class objects</returns>
    /// CMP #553: ACH Requirement for Multiple Currency Handling
    IList<Currency> GetCurrencyListForAchCurrencySetUp();

    /// <summary>
    /// This function is used to get is file log data based on billed member and location id
    /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <param name="locationCode"></param>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    IsDailyOutputProcessLog GetDailyOutputProcessLogData(int billedMemberId, string locationCode, DateTime targetDate);

  }
}
