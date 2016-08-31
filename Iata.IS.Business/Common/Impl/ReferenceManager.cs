using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Pax.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Data.SystemMonitor;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using log4net;
using NVelocity;
using TransactionType = Iata.IS.Model.Common.TransactionType;
using Castle.Core.Smtp;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Data.MiscUatp;
using System.Configuration;

namespace Iata.IS.Business.Common.Impl
{
  /// <summary>
  /// </summary>
  public class ReferenceManager : IReferenceManager
  {
    /// <summary>
    /// 
    /// </summary>
    private static readonly IRepository<SettlementMethod> StaticSettlementMethodRepository = Ioc.Resolve<IRepository<SettlementMethod>>(typeof(IRepository<SettlementMethod>));
    // Logger instance.
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private const string SamplingIndicatorYes = "Y";
    private const string SamplingIndicatorNo = "N";
    private const int DefaultTimeLimit = 6;


    
    /// <summary>
    /// Reason code repository.
    /// </summary>
    public IReasonCodeRepository ReasonCodeRepository { get; set; }

    //SCP210204: IS-WEB Outage (Logging)
    public ILogDebugDataRepository LogDebugDataRepository { get; set; }

    public IRepository<Member> MemberRepository { get; set; }

    /// <summary>
    /// Currency Repository.
    /// </summary>
    public IRepository<Currency> CurrencyRepository { get; set; }

    /// <summary>
    /// Transaction Type Repository.
    /// </summary>
    public IRepository<TransactionType> TransactionTypeRepository { get; set; }

    /// <summary>
    /// Gets or sets the RFIC repository.
    /// </summary>
    public IRepository<Rfic> RficRepository { get; set; }

    public IRepository<Rfisc> RfiscRepository { get; set; }

    public IRepository<MiscCodeGroup> MiscCodeGroupRepository { get; set; }

    public IMiscInvoiceRepository MiscUatpInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the sis member status repository.
    /// </summary>
    public IRepository<SisMemberStatus> SisMemberStatusRepository { get; set; }

    /// <summary>
    /// Get the instance of MemberSip Sub Status repository
    /// </summary>
    public IRepository<SisMemberSubStatus> SisMemberSubStatusRepository { get; set; }

    /// <summary>
    /// Source code repository.
    /// </summary>
    public IRepository<SourceCode> SourceCodeRepository { get; set; }

    /// <summary>
    /// Tax code Repository.
    /// </summary>
    public IRepository<TaxCode> TaxCodeRepository { get; set; }

    /// <summary>
    /// Airport Repository.
    /// </summary>
    public IRepository<CityAirport> AirportCodeRepository { get; set; }

    /// <summary>
    /// MinAcceptableAmount Repository.
    /// </summary>
    public IRepository<MinAcceptableAmount> MinAcceptableAmountRepository { get; set; }

    /// <summary>
    /// MaxAcceptableAmount Repository.
    /// </summary>
    public IRepository<MaxAcceptableAmount> MaxAcceptableAmountRepository { get; set; }

    /// <summary>
    /// MemberLocation information repository.
    /// </summary>
    public IRepository<MemberLocationInformation> MemberLocInfoRepository { get; set; }

    /// <summary>
    /// Country Repository.
    /// </summary>
    public IRepository<Country> CountryRepository { get; set; }
      
    /// <summary>
    /// language Repository.
    /// </summary>
    public IRepository<Language> LanguageRepository { get; set; }


    /// <summary>
    /// AircraftType Repository.
    /// </summary>
    public IRepository<AircraftType> AircraftTypeRepository { get; set; }

    /// <summary>
    /// AircraftTypeIcao Repository.
    /// </summary>
    public IRepository<AircraftTypeIcao> AircraftTypeIcaoRepository { get; set; }

    /// <summary>
    /// Vat identifier Repository.
    /// </summary>
    public IRepository<VatIdentifier> VatIdentifierRepository { get; set; }

    /// <summary>
    /// Gets or sets the cgo vat identifier repository.
    /// </summary>
    /// <value>
    /// The cgo vat identifier repository.
    /// </value>
    public IRepository<CgoVatIdentifier> CgoVatIdentifierRepository { get; set; }

    /// <summary>
    /// Exchange Repository.
    /// </summary>
    public IRepository<ExchangeRate> ExchangRateRepository { get; set; }

    /// <summary>
    /// File Server Repository
    /// </summary>
    public IRepository<FileServer> FileServerRepository { get; set; }

    /// <summary>
    /// IsInputFile Repository.
    /// </summary>
    private IRepository<IsInputFile> InputFileRepository { get; set; }

    /// <summary>
    /// TimeLimit Repository.
    /// </summary>
    public IRepository<TimeLimit> TimeLimitRepository { get; set; }

    /// <summary>
    /// Lead Period Repository.
    /// </summary>
    public IRepository<LeadPeriod> LeadPeriodRepository { get; set; }

    /// <summary>
    /// ChargeCategoryrepository.
    /// </summary>
    /// <value>The charge category repository.</value>
    public IRepository<ChargeCategory> ChargeCategoryRepository { get; set; }

    /// <summary>
    /// UomCode repository.
    /// </summary>
    /// <value>The UOM code repository.</value>
    public IRepository<UomCode> UomCodeRepository { get; set; }

    /// <summary>
    /// ChargeCode repository.
    /// </summary>
    /// <value>The charge code repository.</value>
    public IRepository<ChargeCode> ChargeCodeRepository { get; set; }

    /// <summary>
    /// ChargeCode repository.
    /// </summary>
    /// <value>The charge code repository.</value>
    public IRepository<ChargeCodeType> ChargeCodeTypeRepository { get; set; }

    /// <summary>
    /// CityAirport repository.
    /// </summary>
    /// <value>The city airport repository.</value>
    public IRepository<CityAirport> CityAirportRepository { get; set; }

    /// <summary>
    /// Gets or sets the contact type group repository.
    /// </summary>
    public IRepository<ContactTypeGroup> ContactGroupRepository { get; set; }

    /// <summary>
    /// Gets or sets the contact type sub group repository.
    /// </summary>
    public IRepository<ContactTypeSubGroup> ContactSubGroupRepository { get; set; }

    /// <summary>
    /// Gets or sets the additional detail repository.
    /// </summary>
    /// <value>The additional detail repository.</value>
    public IRepository<AdditionalDetail> AdditionalDetailRepository { get; set; }

    /// <summary>
    /// Gets or sets the misc code repository.
    /// </summary>
    /// <value>The misc code repository.</value>
    public IMiscCodeRepository MiscCodeRepository { get; set; }

    /// <summary>
    /// Gets or sets the settlement method repository.
    /// </summary>
    /// <value>The settlement method repository.</value>
    public IRepository<SettlementMethod> SettlementMethodRepository { get; set; }

    /// <summary>
    /// Gets or sets the invoice status repository.
    /// </summary>
    /// <value>The invoice status repository.</value>
    public IRepository<InvoiceStatus> InvoiceStatusRepository { get; set; }

    /// <summary>
    /// Gets or sets the file status repository.
    /// </summary>
    /// <value>The file status repository.</value>
    public IRepository<FileStatus> FileStatusRepository { get; set; }

    /// <summary>
    /// Gets or sets the tax sub type repository.
    /// </summary>
    /// <value>The tax sub type repository.</value>
    public IRepository<TaxSubType> TaxSubTypeRepository { get; set; }

    /// <summary>
    /// Gets or sets the billing category repository.
    /// </summary>
    /// <value>The billing category repository.</value>
    public IRepository<BillingCategory> BillingCategoryRepository { get; set; }

    /// <summary>
    /// Gets or sets the FileFormat repository.
    /// </summary>
    /// <value>The FileFormat repository.</value>
    public IRepository<FileFormat> FileFormatRepository { get; set; }

    /// <summary>
    /// Gets or sets the IchZone repository.
    /// </summary>
    /// <value>The IchZone repository.</value>
    public IRepository<IchZone> IchZoneRepository { get; set; }

    /// <summary>
    /// Gets or sets the country ICAO repository.
    /// </summary>
    /// <value>
    /// The country ICAO repository.
    /// </value>
    public IRepository<CountryIcao> CountryIcaoRepository { get; set; }

    /// <summary>
    /// Gets or sets the location ICAO repository.
    /// </summary>
    /// <value>
    /// The location ICAO repository.
    /// </value>
    public IRepository<LocationIcao> LocationIcaoRepository { get; set; }

    public ICalendarManager CalendarManager { get; set; }

    public ISystemMonitorRepository SystemMonitorRepo { get; set; }


    public IRepository<IsftpLog> IsFtpLogRepository { get; set; }

    public IRepository<UserQuestion> UserQuestionRepository { get; set; }

    public IRepository<SubDivision> SubdivisionRepository { get; set; }

    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
    public IRepository<IsDailyOutputProcessLog> IsDailyOutputProcessLogRep { get; set; }

    private UnitOfWork unitOfWork = new UnitOfWork(new ObjectContextAdapter());

    public ReferenceManager()
    {
      InputFileRepository = new Repository<IsInputFile>(unitOfWork);
    }

    //public IRepository<UserQuestion> UserQuestionRepository { get; set; }

    private static readonly List<int> SMIsTreatedAsBilateral = new List<int>(GetSMIsTreatedAsBilateral());

    private const string BilateralSmiDescription = "bilateral";

    const string BillingDateFormat = "yyyyMMdd";

    public IList<UserQuestion> GetUserQuestionList(int categoryId)
    {
      var userQues = UserQuestionRepository.Get(userQue => userQue.IsActive && userQue.CategoryId == categoryId);

      return userQues.ToList();
    }
    /// <summary>
    /// Gets the reason code list.
    /// </summary>
    /// <param name = "transactionTypeId">The transaction type id.</param>
    /// <returns></returns>
    public IList<ReasonCode> GetReasonCodeList(int transactionTypeId)
    {
      var reasonCodes = ReasonCodeRepository.Get(reasonCode => reasonCode.IsActive && reasonCode.TransactionTypeId == transactionTypeId);

      return reasonCodes.ToList();
    }

    /// <summary>
    /// Gets the reason code.
    /// </summary>
    /// <param name = "reasonCode">The reason code.</param>
    /// <param name = "transactionTypeId">The transaction type id.</param>
    /// <returns></returns>
    public ReasonCode GetReasonCode(string reasonCode, int transactionTypeId)
    {
      var rsCode = ReasonCodeRepository.First(rCode => rCode.IsActive && rCode.Code == reasonCode && rCode.TransactionTypeId == transactionTypeId);

      return rsCode;
    }

    public IList<ReasonCode> GetReasonCodeList(bool withDuplicate = false)
    {
      // SCP 121308 : Reason Codes in PAX billing history screen appear multiple times.
      //old code : var reasonCodes = ReasonCodeRepository.GetAllReasonCodes().Where(reasonCode => reasonCode.IsActive).OrderBy(reasonCode => reasonCode.Code);
      var reasonCodes =
        ReasonCodeRepository.GetAllReasonCodes().Where(reasonCode => reasonCode.IsActive).OrderBy(reasonCode => reasonCode.Code);

      //Get only distinct reason codes and descriptions.
      if(!withDuplicate)
      {
        reasonCodes.AsEnumerable().Distinct(new ReasonCodeComparer());
      }
  
      return reasonCodes.ToList();
    }

    /// <summary>
    /// Gets all the Currency
    /// </summary>
    /// <returns>List of currency class objects</returns>
    public IList<Currency> GetCurrencyList()
    {
      // Get the list of all the Currency
      var currencies = CurrencyRepository.Get(currency => currency.IsActive);

      return currencies.ToList();
    }

    /// <summary>
    /// Gets all the Currency except USD with order by Asc.
    /// </summary>
    /// <returns>List of currency class objects</returns>
    /// CMP #553: ACH Requirement for Multiple Currency Handling
    public IList<Currency> GetCurrencyListForAchCurrencySetUp()
    {
        // Get the list of all the Currency
        var currencies = CurrencyRepository.Get(currency => currency.IsActive && currency.Id != 840).OrderBy(curr=>curr.Code);

        return currencies.ToList();
    }

    /// <summary>
    /// Gets all the Transaction types
    /// </summary>
    /// <returns>List of TransactionType class objects</returns>
    public IList<TransactionType> GetTransactionTypeList()
    {
      // Get the list of all the transaction type
      var transactiontypes = TransactionTypeRepository.Get(transactiontype => transactiontype.IsActive && transactiontype.IsActive);

      return transactiontypes.ToList();
    }

    public IList<FileFormat> GetFileFormatTypeList()
    {
      // Get the list of all the transaction type
      var fileformattypes = FileFormatRepository.Get(fileformat => fileformat.IsActive);

      return fileformattypes.ToList();
    }
    /// <summary>
    /// Gets the RFIC list.
    /// </summary>
    /// <returns></returns>
    public IList<Rfic> GetRficList()
    {
      var rficList = RficRepository.Get(rfic => rfic.IsActive && rfic.IsActive);

      return rficList.ToList();
    }

    /// <summary>
    /// Gets the misc code group list.
    /// </summary>
    /// <returns></returns>
    public IList<MiscCodeGroup> GetMiscCodeGroupList()
    {
      var miscCodeGroupList = MiscCodeGroupRepository.Get(group => group.IsActive && group.IsActive);
      return miscCodeGroupList.ToList();
    }

    /// <summary>
    /// Gets the member status list.
    /// </summary>
    /// <returns></returns>
    public IList<SisMemberStatus> GetMemberStatusList()
    {
      // Get the list of all the Currency
      var memberStatusList = SisMemberStatusRepository.Get(status => status.IsActive && status.IsActive);

      return memberStatusList.ToList();
    }

    /// <summary>
    /// CMP603:Member Profile-Changes in IS Membership Sub Status
    /// Get all MemberShip Sub status list from MST_SIS_MEMBER_SUB_STATUS table
    /// Author : Vinod Patil
    /// Date : 12 Feb 2014
    /// </summary>
    /// <returns>Dictionary object of int and string pair list</returns>
    public Dictionary<int, string> GetAllMemberSubStatus()
    {
      //var memberSubStatuses = SisMemberSubStatusRepository.GetAll();
      
      //SCP#416211 - Issue to be fixed in Member Profile screen
      var memberSubStatuses = SisMemberSubStatusRepository.Get(status=>status.IsActive);

      var memberSubStatusesList = memberSubStatuses.ToDictionary(sub => sub.Id, sub => sub.Description);

      return memberSubStatusesList;
    }

    /// <summary>
    /// List all the Source Code by passing the Transaction Type Id
    /// </summary>
    /// <param name = "transactionTypeId">Transaction Type ID for which source code list should be fetched</param>
    /// <returns>List of source code class objects</returns>
    public IList<SourceCode> GetSourceCodeList(int transactionTypeId)
    {
      // Get the list of Source Code by passing the Transaction Type Id
      // if transaction type is RejectionMemo, then search source codes for 3 transaction types, RejectionMemo1, RejectionMemo2, RejectionMemo3.
      var sourceCodes = transactionTypeId == (int)Model.Enums.TransactionType.RejectionMemo1
                          ? SourceCodeRepository.Get(
                            sc =>
                            sc.IsActive &&
                            (sc.TransactionTypeId == (int)Model.Enums.TransactionType.RejectionMemo1 || sc.TransactionTypeId == (int)Model.Enums.TransactionType.RejectionMemo2 ||
                             sc.TransactionTypeId == (int)Model.Enums.TransactionType.RejectionMemo3))
                          : (transactionTypeId == (int)Model.Enums.TransactionType.Coupon || transactionTypeId == (int)Model.Enums.TransactionType.CreditMemo ||
                             transactionTypeId == (int)Model.Enums.TransactionType.BillingMemo)
                              ? SourceCodeRepository.Get(sc => sc.IsActive && sc.TransactionTypeId == transactionTypeId)
                              : SourceCodeRepository.Get(sc => sc.IsActive && sc.TransactionTypeId == transactionTypeId);

      return sourceCodes.ToList();
    }

    /// CMP523 - Source Code in RMBMCM Summary Report
    /// <summary>
    /// List all the Source Code by passing the Transaction Type Id
    /// corresponding to that transaction type
    /// </summary>
    /// <param name = "transactionTypeId">Transaction Type ID for which source code list should be fetched</param>
    /// <returns>List of source code class objects</returns>
    public IList<SourceCode> GetSourceCodesList(int transactionTypeId)
    {
      // Get the list of Source Code by passing the Transaction Type Id
      // if transaction type is RejectionMemo, then search source codes for 3 transaction types, RejectionMemo1, RejectionMemo2, RejectionMemo3.
      switch (transactionTypeId)
      {
        case 4:
          var sourceCodeRM = SourceCodeRepository.Get(sc => sc.IsActive &&
                                                           (sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo1 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo2 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo3 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.SamplingFormF ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.SamplingFormXF)).AsEnumerable().OrderBy(sc => sc.IsBilateralCode);
          
          return sourceCodeRM.ToList();
          
        case 5:
          var sourceCodeBM = SourceCodeRepository.Get(sc => sc.IsActive &&
                                                           (sc.TransactionTypeId == (int)Model.Enums.TransactionType.BillingMemo ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.PasNsBillingMemoDueToAuthorityToBill ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.PasNsBillingMemoDueToExpiry)).AsEnumerable().OrderBy(sc => sc.IsBilateralCode);
          return sourceCodeBM.ToList();
          
        case 6:
          var sourceCodeCM = SourceCodeRepository.Get(sc => sc.IsActive &&
                                                           (sc.TransactionTypeId == (int)Model.Enums.TransactionType.CreditMemo)).AsEnumerable().OrderBy(sc => sc.IsBilateralCode);
          return sourceCodeCM.ToList();
          
        case -1:
          var sourceCodeAll = SourceCodeRepository.Get(sc => sc.IsActive &&
                                                           (sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo1 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo2 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo3 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.SamplingFormF ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.SamplingFormXF ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.BillingMemo ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.PasNsBillingMemoDueToAuthorityToBill ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.PasNsBillingMemoDueToExpiry ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.CreditMemo)).AsEnumerable().OrderBy(sc => sc.IsBilateralCode);
          return sourceCodeAll.ToList();
        default: 
          var sourceCodes = SourceCodeRepository.Get(sc => sc.IsActive &&
                                                           (sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo1 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo2 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.RejectionMemo3 ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.SamplingFormF ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.SamplingFormXF ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.BillingMemo ||
                                                            sc.TransactionTypeId == (int) Model.Enums.TransactionType.PasNsBillingMemoDueToAuthorityToBill ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.PasNsBillingMemoDueToExpiry ||
                                                            sc.TransactionTypeId == (int)Model.Enums.TransactionType.CreditMemo)).AsEnumerable().OrderBy(sc => sc.IsBilateralCode);
          return sourceCodes.ToList();
      }
    }

    public IList<SourceCode> GetSourceCodeList()
    {
      return SourceCodeRepository.GetAll().ToList();
    }

    /// <summary>
    /// Lists all the Tax Code List
    /// </summary>
    /// <returns></returns>
    public IList<TaxCode> GetTaxCodeList()
    {
      var taxCodes = TaxCodeRepository.Get(taxCode => taxCode.IsActive);

      return taxCodes.ToList();
    }

    /// <summary>
    /// Lists all the Airports Code List
    /// </summary>
    /// <returns></returns>
    public IList<CityAirport> GetAirportsCodeList()
    {
      var airportCodes = AirportCodeRepository.Get(airport => airport.IsActive);

      return airportCodes.ToList();
    }

    /// <summary>
    /// Gets the Billing Member's Location for specific invoice Id provided.
    /// </summary>
    /// <param name = "invoiceId"></param>
    /// <returns></returns>
    public MemberLocationInformation GetBillingLocationForInvoice(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();

      // Get Billing Member Location details by passing invoiceId
      var memberLocation = MemberLocInfoRepository.Single(invoice => invoice.InvoiceId == invoiceGuid);

      return memberLocation;
    }

    /// <summary>
    /// Gets the Billed Member's Location for specific invoice Id provided.
    /// </summary>
    /// <param name = "invoiceId"></param>
    /// <returns></returns>
    public MemberLocationInformation GetBilledLocationForInvoice(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();

      // Get Billed Member Location details by passing invoiceId
      var memberLocation = MemberLocInfoRepository.Single(locationInformation => locationInformation.InvoiceId == invoiceGuid);

      return memberLocation;
    }

    /// <summary>
    /// Gets all the Countries
    /// </summary>
    /// <returns></returns>
    public IList<Country> GetCountryList()
    {
      // Get the list of all the Currency
      var countries = CountryRepository.Get(country => country.IsActive).OrderBy(country => country.Name);

      return countries.ToList();
    }

    /// <summary>
    /// Gets All Languages.
    /// </summary>
    /// <returns></returns>
    public IList<Language> GetLanguageList()
    {
      // Get the list of all the Currency
      var languages = LanguageRepository.GetAll();

      return languages.ToList();
    }

    /// <summary>
    /// Gets all the subdivision Code list.
    /// </summary>
    /// <returns></returns>
    public IList<SubDivision> GetSubDivisionList()
    {
      // Get the list of all the subdivision codes
      // TODO: Get data from repository and remove below hardcoded data.

      var subDivisionList = new List<SubDivision> { new SubDivision { Id = "1", Name = "New York", Code = "NY" } };

      return subDivisionList;
    }

    /// <summary>
    /// Gets list of subdivision codes based on country code.
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    public List<string> GetSubdivisionCodesByCountryCode(string countryCode)
    {
      var subdivisionList = SubdivisionRepository.Get(div => div.CountryId.ToLower() == countryCode.ToLower() && div.IsActive == true).OrderBy(div => div.Id);
      return subdivisionList.Select(div => div.Id).ToList();
    }

    /// <summary>
    /// This method retrieves list of VAT identifiers available in database
    /// </summary>
    /// <returns>List of Vat identifier objects</returns>
    public IList<VatIdentifier> GetVatIdentifierList(BillingCategoryType billingCategoryType)
    {
      // Get list of all VatIdentifires.
      var vatIdentifiers = VatIdentifierRepository.Get(vatIdentifier => vatIdentifier.IsActive && vatIdentifier.BillingCategoryCode == (int)billingCategoryType);

      return vatIdentifiers.ToList();
    }

    public IList<CgoVatIdentifier> GetCgoVatIdentifierList(bool? isOcApplicable)
    {
      // Get list of all VatIdentifires.
      var cgovatIdentifiers = CgoVatIdentifierRepository.Get(vatIdentifier => vatIdentifier.IsActive);
      if (isOcApplicable != null)
      {
        cgovatIdentifiers = cgovatIdentifiers.Where(vatIdentifier => vatIdentifier.IsActive && vatIdentifier.IsOcApplicable == isOcApplicable);
      }

      return cgovatIdentifiers.ToList();
    }
    /// <summary>
    /// Gets the exchange rate.
    /// </summary>
    /// <param name = "listingCurrencyId">The listing currency id.</param>
    /// <param name = "billingCurrency">The billing currency enum.</param>
    /// <param name = "billingYear">Invoice billing year</param>
    /// <param name = "billingMonth">Invoice billing month</param>
    /// <returns>exchange rate</returns>
    public double GetExchangeRate(int listingCurrencyId, BillingCurrency billingCurrency, int billingYear, int billingMonth)
    {
      var exchangeRateValue = 0d;

      if (listingCurrencyId == (int)billingCurrency)
      {
        // If Billing Currency is same as listing currency, then return 
        // exchange rate as 1. No Need to go to master table.
        return 1;
      }
      // Get exchange rate for given billing year/month, first date of billing year/month.
      var billingDate = new DateTime(billingYear, billingMonth, 1);

      // if Billing Currency is CAD, then get exchange rate between
      // USD to CAD, irrespective of Listing Currency.
      // note: since USD to CAD rate is not present, we fetch CAD to USD and reciprocate it.
      if (billingCurrency == BillingCurrency.CAD)
      {
        // Get exchange rate 
        var exchangeRateCAD = ExchangRateRepository.First(exchangeRates => exchangeRates.CurrencyId == (int)billingCurrency && exchangeRates.EffectiveFromDate <= billingDate && exchangeRates.EffectiveToDate >= billingDate);

        // Check if exchange rate for given listing currency is valid.
        if (exchangeRateCAD == null || exchangeRateCAD.FiveDayRateUsd == 0)
        {
          // Todo : define new exception message.
          throw new ISBusinessException(ErrorCodes.InvalidBillingCurrency);
        }

        return 1 / exchangeRateCAD.FiveDayRateUsd;
      }

      // Get exchange rate for given listing currency id and first date of billing year/month between exchange rates effective from date and effective to date.
      var exchangeRate = ExchangRateRepository.First(exchangeRates => exchangeRates.CurrencyId == listingCurrencyId && exchangeRates.EffectiveFromDate <= billingDate && exchangeRates.EffectiveToDate >= billingDate);

      // Check if exchange rate for given listing currency is valid.
      if (exchangeRate == null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidListingCurrency);
      }

      // Get five days exchange rate value for given Billing Currency.
      switch (billingCurrency)
      {
        case BillingCurrency.USD:
          exchangeRateValue = exchangeRate.FiveDayRateUsd;
          break;
        case BillingCurrency.EUR:
          exchangeRateValue = exchangeRate.FiveDayRateEur;
          break;
        case BillingCurrency.GBP:
          exchangeRateValue = exchangeRate.FiveDayRateGbp;
          break;
        case BillingCurrency.CAD:
          // Return USD rate when Billing Currency is CAD.
          exchangeRateValue = exchangeRate.FiveDayRateUsd;
          break;
        default:
          break;
      }

      return exchangeRateValue;

    }

    /// <summary>
    /// Gets the source code details.
    /// </summary>
    /// <param name = "sourceCode">The source code.</param>
    /// <returns>The Source Code object if source code exist else null.</returns>
    public SourceCode GetSourceCodeDetails(int sourceCode)
    {
      return SourceCodeRepository.First(sourceCodeObject => sourceCodeObject.IsActive && sourceCodeObject.SourceCodeIdentifier == sourceCode);
    }

    /// <summary>
    /// Get active file server details for attachments
    /// </summary>
    /// <returns></returns>
    public FileServer GetActiveAttachmentServer()
    {
      return FileServerRepository.Single(fileServerObject => fileServerObject.IsActive && fileServerObject.Status == 1 && fileServerObject.ServerType == ReferenceManagerConstants.ServerTypeAttachment);
    }

    /// <summary>
    /// Get active file server details for unlinked supporting documents
    /// </summary>
    /// <returns></returns>
    public FileServer GetActiveUnlinkedDocumentsServer()
    {
      return
        FileServerRepository.Single(
          fileServerObject => fileServerObject.IsActive && fileServerObject.Status == 1 && fileServerObject.ServerType == ReferenceManagerConstants.ServerTypeUnlinkedDocuments);
    }

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name = "currencyId">The currency id.</param>
    /// <returns>
    /// <c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidCurrency(int? currencyId)
    {
      if (currencyId == null)
      {
        return false;
      }
      return CurrencyRepository.GetCount(currency => currency.IsActive && currency.Id == currencyId) > 0;
    }

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name = "invoice">The invoice object.</param>
    /// <param name = "currencyId">The currency id.</param>
    /// <returns>
    /// <c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidCurrency(InvoiceBase invoice, int? currencyId)
    {
      var result = false;
      if (currencyId.HasValue)
      {
        if (invoice.ValidCurrencyCodes != null)
        {
          if (invoice.ValidCurrencyCodes.Values.Contains(currencyId.Value))
          {
            result = true;
          }
        }
        else
        {
          result = IsValidCurrency(currencyId);
        }
      }
      return result;
    }

    /// <summary>
    /// Validates the tax code.
    /// </summary>
    /// <param name = "taxCode">The tax code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool IsValidTaxCode(string taxCode)
    {
      var result = true;
      var countTaxCode = TaxCodeRepository.GetCount(taxCodeObject => taxCodeObject.IsActive && taxCodeObject.Id == taxCode);

      if (countTaxCode == 0)
      {
        result = false;
      }
      return result;
    }

    /// <summary>
    /// Validates the tax code against the tax code dictionary in invoce object.
    /// </summary>
    /// <param name = "invoice">Invoice object.</param>
    /// <param name = "taxCode">The tax code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool IsValidTaxCode(InvoiceBase invoice, string taxCode)
    {
      var result = false;

      if (invoice.ValidTaxCodes != null)
      {
        if (invoice.ValidTaxCodes.ContainsKey(taxCode) && invoice.ValidTaxCodes[taxCode])
        {
          result = true;
        }
      }
      else
      {
        result = IsValidTaxCode(taxCode);
      }
      return result;
    }

    /// <summary>
    /// Determines whether [is valid airport code] [the specified airport code].
    /// </summary>
    /// <param name = "airportCode">The airport code.</param>
    /// <returns>
    /// <c>true</c> if [is valid airport code] [the specified airport code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidAirportCode(string airportCode)
    {
      return AirportCodeRepository.GetCount(airportRecord => airportRecord.IsActive && airportRecord.Id == airportCode) > 0;
    }

    /// <summary>
    /// Determines whether [is valid source code] [the specified source code id].
    /// </summary>
    /// <param name = "sourceCodeId">The source code id.</param>
    /// <param name = "transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// <c>true</c> if [is valid source code] [the specified source code id]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidSourceCode(int sourceCodeId, int transactionTypeId)
    {
      return SourceCodeRepository.GetCount(sourceCode => sourceCode.IsActive && sourceCode.SourceCodeIdentifier == sourceCodeId && sourceCode.TransactionTypeId == transactionTypeId) > 0;
    }

    /// <summary>
    /// Determines whether [is valid source code] [the specified invoice].
    /// </summary>
    /// <param name = "invoice">The invoice.</param>
    /// <param name = "sourceCodeId">The source code id.</param>
    /// <param name = "transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// <c>true</c> if [is valid source code] [the specified invoice]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidSourceCode(InvoiceBase invoice, int sourceCodeId, int transactionTypeId)
    {
      bool result;
      if (invoice.ValidSourceCodes != null)
      {
        result = invoice.ValidSourceCodes.Count(sourceCode => sourceCode.IsActive && sourceCode.SourceCodeIdentifier == sourceCodeId && sourceCode.TransactionTypeId == transactionTypeId) > 0;
      }
      else
      {
        result = IsValidSourceCode(sourceCodeId, transactionTypeId);
      }
      return result;
    }

    /// <summary>
    /// Determines whether [is valid source code] [the specified invoice].
    /// </summary>
    /// <param name = "invoice">The invoice.</param>
    /// <param name = "sourceCodeId">The source code id.</param>
    /// <param name = "transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// <c>true</c> if [is valid source code] [the specified invoice]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidSourceCode(InvoiceBase invoice, int sourceCodeId, out int transactionTypeId)
    {
      var result = false;
      transactionTypeId = 0;

      if (sourceCodeId == 0)
      {
        return false;
      }

      if (invoice.ValidSourceCodes != null)
      {
        var code = invoice.ValidSourceCodes.FirstOrDefault(sourceCode => sourceCode.IsActive && sourceCode.SourceCodeIdentifier == sourceCodeId);
        if (code != null)
        {
          transactionTypeId = code.TransactionTypeId;
          result = true;
        }
      }
      else
      {
        result = IsValidSourceCode(sourceCodeId, out transactionTypeId);
      }
      return result;
    }

    /// <summary>
    /// Determines whether [is valid source code] [the specified source code id].
    /// </summary>
    /// <param name = "sourceCodeId">The source code id.</param>
    /// <param name = "transactionTypeId">The transaction type id.</param>
    /// <returns>
    /// <c>true</c> if [is valid source code] [the specified source code id]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidSourceCode(int sourceCodeId, out int transactionTypeId)
    {
      var sourceCode = SourceCodeRepository.First(sourceCodeRecord => sourceCodeRecord.IsActive && sourceCodeRecord.SourceCodeIdentifier == sourceCodeId);
      transactionTypeId = 0;
      if (sourceCode != null)
      {
        transactionTypeId = sourceCode.TransactionTypeId;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name = "currencyCode">The currency code.</param>
    /// <returns>
    /// <c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidCurrencyCode(string currencyCode)
    {
      return CurrencyRepository.GetCount(currency => currency.IsActive && currency.Code == currencyCode) > 0;
    }

    /// <summary>
    /// Determines whether RFIC COde is a valid code
    /// </summary>
    /// <param name="rficCodeId"></param>
    /// <returns>
    /// <c>true</c> if [is valid RFIC code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidRficCode(string rficCodeId)
    {
      return RficRepository.GetCount(rficType => rficType.IsActive && rficType.Id == rficCodeId) > 0;
    }

    /// <summary>
    /// Determines whether RFIsC COde is a valid code
    /// </summary>
    /// <param name="rfiscCodeId"></param>
    /// <param name="rficCodeId"></param>
    /// <returns>
    /// <c>true</c> if [is valid RFISC code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidRfiscCode(string rfiscCodeId, string rficCodeId = null)
    {
      if (rficCodeId != null)
      {
        return RfiscRepository.GetCount(rfiscType => rfiscType.IsActive && rfiscType.Id == rfiscCodeId && rfiscType.RficId == rficCodeId) > 0;
      }
      return RfiscRepository.GetCount(rfiscType => rfiscType.IsActive && rfiscType.Id == rfiscCodeId) > 0;
    }

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name = "invoice">The invoice.</param>
    /// <param name = "currencyCode">The currency code.</param>
    /// <returns>
    /// <c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidCurrencyCode(InvoiceBase invoice, string currencyCode)
    {
      var result = false;

      if (invoice.ValidCurrencyCodes != null)
      {
        if (invoice.ValidCurrencyCodes.ContainsKey(currencyCode))
        {
          result = true;
        }
      }
      else
      {
        result = IsValidCurrencyCode(currencyCode);
      }
      return result;
    }

    /// <summary>
    /// Determines whether [is valid reason code] [the specified reason code].
    /// </summary>
    /// <param name = "reasonCode">The reason code.</param>
    /// <param name = "transactionTypeId"></param>
    /// <returns>
    /// <c>true</c> if [is valid reason code] [the specified reason code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidReasonCode(string reasonCode, int transactionTypeId)
    {
      return
        ReasonCodeRepository.GetCount(
          reasonCodeRecoord => reasonCodeRecoord.IsActive && reasonCodeRecoord.Code.Equals(reasonCode) && reasonCodeRecoord.TransactionTypeId == transactionTypeId) >
        0;
    }

    /// <summary>
    /// Determines whether [is valid reason code] [the specified reason code id].
    /// </summary>
    /// <param name = "invoice">The invoice.</param>
    /// <param name = "reasonCode">The reason code.</param>
    /// <param name = "transactionTypeId"></param>
    /// <returns>
    /// <c>true</c> if [is valid reason code] [the specified reason code id]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidReasonCode(InvoiceBase invoice, string reasonCode, int transactionTypeId)
    {
      bool result;
      if (invoice.ValidReasonCodes != null)
      {
        result =
          invoice.ValidReasonCodes.Count(
            reasonCodeRecoord => reasonCodeRecoord.IsActive && reasonCodeRecoord.Code.Equals(reasonCode) && reasonCodeRecoord.TransactionTypeId == transactionTypeId) >
          0;
      }
      else
      {
        result = IsValidReasonCode(reasonCode, transactionTypeId);
      }
      return result;
    }

    /// <summary>
    /// Determines whether [is valid vat identifier] [the specified vat identifier id].
    /// </summary>
    /// <param name = "vatIdentifierId">The vat identifier id.</param>
    /// <returns>
    /// <c>true</c> if [is valid vat identifier] [the specified vat identifier id]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidVatIdentifier(int vatIdentifierId)
    {
      return VatIdentifierRepository.GetCount(vatIdentifierRecord => vatIdentifierRecord.IsActive && vatIdentifierRecord.Id == vatIdentifierId) > 0;
    }

    /// <summary>
    /// Determines whether [is valid country code] [the specified country code].
    /// </summary>
    /// <param name = "countryCode">The country code.</param>
    /// <returns>
    /// <c>true</c> if [is valid country code] [the specified country code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidCountryCode(string countryCode)
    {
      return CountryRepository.GetCount(countryRecord => countryRecord.IsActive && countryRecord.Id == countryCode) > 0;
    }

    /// <summary>
    /// CMP496: Get country name by country code. 
    /// </summary>
    /// <param name="countryCode">valid country code</param>
    /// <returns></returns>
    public string GetCountryNameByCode(string countryCode)
    {
        var country = CountryRepository.First(countryRecord => countryRecord.IsActive && countryRecord.Id == countryCode);

        return country!= null ? country.Name : string.Empty;
    }

      /// <summary>
    /// Determines whether [is valid AircraftType code] [the specified AircraftType code].
    /// </summary>
    /// <param name = "aircraftTypeCode">The aircraftTypeCode code.</param>
    /// <returns>
    /// <c>true</c> if [is valid aircraftTypeCode code] [the specified aircraftTypeCode code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidAircraftTypeCode(string aircraftTypeCode)
    {
      return AircraftTypeRepository.GetCount(aircraftTypeRecord => aircraftTypeRecord.IsActive && aircraftTypeRecord.Id == aircraftTypeCode) > 0;
    }

    /// <summary>
    /// Determines whether [is valid member numberic code] [the specified member code numeric].
    /// </summary>
    /// <param name="memberCodeNumeric">The member code numeric.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid member numberic code] [the specified member code numeric]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidMemberNumbericCode(string memberCodeNumeric)
    {
      return MemberRepository.GetCount(member => member.MemberCodeNumeric == memberCodeNumeric) > 0;
    }

    public bool IsValidMemberAlphaCode(string memberCodeAlpha)
    {
      return MemberRepository.GetCount(member => member.MemberCodeAlpha == memberCodeAlpha) > 0;
    }
    /// <summary>
    /// Determines whether [is valid AircraftTypeIcao code] [the specified AircraftTypeIcao code].
    /// </summary>
    /// <param name = "aircraftTypeIcaoCode">The AircraftTypeIcao code.</param>
    /// <returns>
    /// <c>true</c> if [is valid AircraftTypeIcao code] [the specified AircraftTypeIcao code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidAircraftTypeIcaoCode(string aircraftTypeIcaoCode)
    {
      return AircraftTypeIcaoRepository.GetCount(aircraftTypeIcaoRecord => aircraftTypeIcaoRecord.IsActive && aircraftTypeIcaoRecord.Id == aircraftTypeIcaoCode) > 0;
    }

    /// <summary>
    /// Determines whether [is valid country code] [the specified country code].
    /// </summary>
    /// <param name="invoiceBase"></param>
    /// <param name="countryCode">The country code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid country code] [the specified country code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidCountryCode(InvoiceBase invoiceBase, string countryCode)
    {
      var result = false;
      if (invoiceBase.ValidCountryCodes != null)
      {
        if (invoiceBase.ValidCountryCodes.ContainsKey(countryCode))
          result = true;
      }
      else
      {
        result = IsValidCountryCode(countryCode);
      }
      return result;
    }

    /// <summary>
    /// Determines whether given Country ICAO code is valid or not.
    /// </summary>
    /// <param name = "countryCodeIcao">The country code.</param>
    /// <returns>
    /// true if country ICAO is valid; otherwise, false.
    /// </returns>
    public bool IsValidCountryIcaoCode(string countryCodeIcao)
    {
      return CountryIcaoRepository.GetCount(countryRecord => countryRecord.IsActive && countryRecord.CountryCodeIcao == countryCodeIcao) > 0;
    }

    /// <summary>
    /// Determines whether given Country ICAO code is valid or not.
    /// </summary>
    /// <param name = "locationIcao">The location ICAO.</param>
    /// <returns>
    /// true if country ICAO is valid; otherwise, false.
    /// </returns>
    public bool IsValidLocationIcaoCode(string locationIcao)
    {
      return LocationIcaoRepository.GetCount(countryRecord => countryRecord.IsActive && countryRecord.Id == locationIcao) > 0;
    }


    /// <summary>
    /// Adds the is FTP Log
    /// </summary>
    /// <param name = "isInputFile">The is input file.</param>
    /// <returns></returns>
    public void AddIsFtpLog(IsftpLog isftpLog)
    {
      IsFtpLogRepository.Add(isftpLog);
      UnitOfWork.CommitDefault();
    }


    /// <summary>
    /// Adds the is input file.
    /// </summary>
    /// <param name = "isInputFile">The is input file.</param>
    /// <returns></returns>
    public IsInputFile AddIsInputFile(IsInputFile isInputFile)
    {
      InputFileRepository.Add(isInputFile);
      unitOfWork.Commit();
      //UnitOfWork.CommitDefault();
      return isInputFile;
    }


    public IsftpLog GetFTPLog(Guid isFileId)
    {
      var ftpLog = IsFtpLogRepository.Get(f => f.IsfileLogId == isFileId);
      ftpLog = ftpLog.OrderByDescending(f => f.LastUpdatedOn);
      return Enumerable.FirstOrDefault(ftpLog);
    }

    /// <summary>
    /// Get IS_FILE_LOG files as per Status ID
    /// Author : Vinod Patil
    /// Date   :28 March 2011
    /// </summary>
    /// <param name = "status"> int file status  </param>
    /// <returns> Collection of Files listing </returns>
    public IQueryable<IsInputFile> GetIsInputFileByStatus(int status)
    {
      var isInputFileinDb = InputFileRepository.Get(input => input.FileStatusId == status);
      return isInputFileinDb;
    }

    ///// <summary>
    ///// Gets the is input file.
    ///// </summary>
    ///// <param name = "isInputFile">The is input file.</param>
    ///// <returns></returns>
    //public void AddIsFtpLog(IsftpLog isftpLog)
    //{
    //  IsFtpLogRepository.Add(isftpLog);
    //  UnitOfWork.CommitDefault();
    //}

    /// <summary>
    /// Updates the is input file.
    /// </summary>
    /// <param name = "isInputFile">The is input file.</param>
    /// <returns></returns>
    public IsInputFile UpdateIsInputFile(IsInputFile isInputFile)
    {
      InputFileRepository.Update(isInputFile);
      //UnitOfWork.CommitDefault();
      unitOfWork.Commit();
      return isInputFile;
    }

    /// <summary>
    /// Gets the is input file.
    /// </summary>
    /// <param name="isInputFileId"></param>
    /// <returns></returns>
    public IsInputFile GetIsInputFileFromIsFileId(Guid isInputFileId)
    {
      var isInputFileinDb = InputFileRepository.Single(isInputFileRecord => isInputFileRecord.Id == isInputFileId);
      return isInputFileinDb;
    }

    /// <summary>
    /// This function is used to get is file log data based on billed member and location id
    /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <param name="locationCode"></param>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    public IsDailyOutputProcessLog GetDailyOutputProcessLogData(int billedMemberId, String locationCode, DateTime targetDate)
    {
        IsDailyOutputProcessLog isDailyOutputProcessLogData;

        //Get daily output process log data based on target date, Location code and billed member
        if (String.IsNullOrEmpty(locationCode))
        {
            isDailyOutputProcessLogData =
                IsDailyOutputProcessLogRep.Get(
                    idopl =>
                    idopl.MemberId == billedMemberId && idopl.TargetDate < targetDate && idopl.LocationId == null).
                    OrderByDescending(isf => isf.TargetDate).Take(1).FirstOrDefault();
        }
        else
        {
            isDailyOutputProcessLogData =
                IsDailyOutputProcessLogRep.Get(
                    idopl =>
                    idopl.MemberId == billedMemberId && idopl.TargetDate < targetDate &&
                    idopl.LocationId == locationCode).
                    OrderByDescending(isf => isf.TargetDate).Take(1).FirstOrDefault();
        }

        return isDailyOutputProcessLogData;
    }

    /// <summary>
    /// Gets the is input file.
    /// </summary>
    /// <param name = "isInputFileName">Name of the is input file.</param>
    /// <returns></returns>
    public IsInputFile GetIsInputFile(string isInputFileName)
    {
      var isInputFileinDb = InputFileRepository.Single(isInputFileRecord => isInputFileRecord.FileName.ToUpper() == isInputFileName.ToUpper());
      return isInputFileinDb;
    }

    /// <summary>
    /// Gets all is input file.
    /// </summary>
    /// <param name = "isInputFileName">Name of the is input file.</param>
    /// <returns></returns>
    public List<IsInputFile> GetAllIsInputFile(string isInputFileName)
    {
        var isInputFileinDb = InputFileRepository.GetAll().Where(isInputFileRecord => isInputFileRecord.FileName.ToUpper() == isInputFileName.ToUpper()).ToList();
        return isInputFileinDb;
    }

    ///// <summary>
    ///// Get IS_FILE_LOG files as per Status ID
    ///// Author : Vinod Patil
    ///// Date   :28 March 2011
    ///// </summary>
    ///// <param name = "status"> int file status  </param>
    ///// <returns> Collection of Files listing </returns>
    //public IList<IsInputFile> GetIsInputFileByStatus(int status)
    //{
    //  var isInputFileinDb = InputFileRepository.Get(input => input.FileStatusId == status);
    //  return isInputFileinDb.ToList();
    //}

    /// <summary>
    /// Get vat identifier id from vat identifier code
    /// </summary>
    /// <param name = "vatIdentifier">The vat identifier.</param>
    /// <param name = "billingCategoryType">Type of the billing category.</param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public int GetVatIdentifierId(string vatIdentifier, BillingCategoryType billingCategoryType)
    {
      var vatRecord = VatIdentifierRepository.First(vat => vat.IsActive && vat.Identifier == vatIdentifier && vat.BillingCategoryCode == (int)billingCategoryType);

      if ((vatRecord != null))
      {
        return vatRecord.Id;
      }
      return 0;
    }

    /// <summary>
    /// Get Cargo vat identifier code from vat identifier id
    /// </summary>
    /// <param name = "vatIdentifierId"></param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public string GetCgoVatIdentifierCode(int vatIdentifierId)
    {
      //var vatRecord = VatIdentifierRepository.First(vat => vat.IsActive && vat.Id == vatIdentifierId);

      var vatRecord = CgoVatIdentifierRepository.First(vat => vat.IsActive && vat.Id == vatIdentifierId);

      if ((vatRecord != null))
      {
        return vatRecord.Identifier;
      }
      return string.Empty;
    }

    /// <summary>
    /// Get vat identifier code from vat identifier id
    /// </summary>
    /// <param name = "vatIdentifierId"></param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public string GetVatIdentifierCode(int vatIdentifierId)
    {
      var vatRecord = VatIdentifierRepository.First(vat => vat.IsActive && vat.Id == vatIdentifierId);

      if ((vatRecord != null))
      {
        return vatRecord.Identifier;
      }
      return string.Empty;
    }

    /// <summary>
    /// Determines whether [is valid net  amount] [the specified net  amount].
    /// </summary>
    /// <param name = "netAmount">The net  amount.</param>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "listingCurrencyId">The listing currency id.</param>
    /// <param name = "invoice"></param>
    /// <param name="iExchangeRate"></param>
    /// <param name="validateMinAmount"></param>
    /// <param name="validateMaxAmount"></param>
    /// <param name="iMinAcceptableAmount"></param>
    ///<param name="applicableMinimumField"></param>
    /// <param name="rejectionReasonCode"></param>
    /// <param name="iMaxAcceptableAmount"></param>
    ///<param name="isCorrespondence"></param>
    ///<param name="correspondence"></param>
    ///<returns>
    /// <c>true</c> if [is valid net  amount] [the specified net  amount]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidNetAmount(double netAmount, Model.Enums.TransactionType transactionType, int? listingCurrencyId, InvoiceBase invoice, ExchangeRate iExchangeRate = null, bool validateMinAmount = true, bool validateMaxAmount = true, MinAcceptableAmount iMinAcceptableAmount = null, ApplicableMinimumField? applicableMinimumField = null, string rejectionReasonCode = null, MaxAcceptableAmount iMaxAcceptableAmount = null, bool isCorrespondence = false, CorrespondenceBase correspondence = null, bool isCreditNote = false, bool isRejectionMemo = false)
    {
      var billingDate = GetBillingDate(invoice);
      var transactionDate = isCorrespondence ? correspondence != null ? correspondence.CorrespondenceDate : DateTime.Now : GetTransactionDate(invoice);

      MinAcceptableAmount minAcceptableAmount = null;
      MaxAcceptableAmount maxAcceptableAmount = null;
      if (iMinAcceptableAmount == null && applicableMinimumField.HasValue)
      {
        // If iMinAcceptableAmount is null then only get clearing house using GetClearingHouse().
          var clearingHouse = GetClearingHouseFromSMI(invoice.SettlementMethodId);
        var applicationId = Convert.ToInt32(applicableMinimumField.Value);
        if (ValidationCache.Instance.ValidMinAcceptableAmounts == null)
        {
          if (string.IsNullOrWhiteSpace(rejectionReasonCode))
          {
            minAcceptableAmount = MinAcceptableAmountRepository.First(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.ApplicableMinimumFieldId == applicationId && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);
          }
          else
          {
            minAcceptableAmount = MinAcceptableAmountRepository.First(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.ApplicableMinimumFieldId == applicationId && rec.RejectionReasonCode.ToUpper().CompareTo(rejectionReasonCode.ToUpper()) == 0 && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);

            if (minAcceptableAmount == null)
            {
              minAcceptableAmount = MinAcceptableAmountRepository.First(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.ApplicableMinimumFieldId == applicationId && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);
            }

          }
        }
        else
        {
          if (string.IsNullOrWhiteSpace(rejectionReasonCode))
          {
            minAcceptableAmount = ValidationCache.Instance.ValidMinAcceptableAmounts.Find(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.ApplicableMinimumFieldId == (int)applicableMinimumField.Value && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);
          }
          else
          {

            var validMinAcceptableAmounts = ValidationCache.Instance.ValidMinAcceptableAmounts.FindAll(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.ApplicableMinimumFieldId == (int)applicableMinimumField.Value && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);

            foreach (var validMinAcceptableAmount in validMinAcceptableAmounts)
            {
              if (validMinAcceptableAmount != null && !string.IsNullOrWhiteSpace(validMinAcceptableAmount.RejectionReasonCode))
              {
                if (validMinAcceptableAmount.RejectionReasonCode.ToUpper().CompareTo(rejectionReasonCode.ToUpper()) == 0)
                {
                  minAcceptableAmount = validMinAcceptableAmount;
                  break;
                }
              }
            }
            if (minAcceptableAmount == null && validMinAcceptableAmounts.Count > 0)
            {
              minAcceptableAmount = validMinAcceptableAmounts.Find(rec => rec.RejectionReasonCode == null);
            }
          }
        }
      }
      else
      {
        minAcceptableAmount = iMinAcceptableAmount;
      }

      if (iMaxAcceptableAmount == null)
      {
          var clearingHouse = GetClearingHouseFromSMI(invoice.SettlementMethodId);
        if (ValidationCache.Instance.ValidMaxAcceptableAmounts == null)
        {
          maxAcceptableAmount =
                MaxAcceptableAmountRepository.First(
                  rec =>
                  rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.EffectiveToPeriod >= transactionDate && rec.EffectiveFromPeriod <= transactionDate);

        }
        else
        {
          maxAcceptableAmount = ValidationCache.Instance.ValidMaxAcceptableAmounts.Find(
                rec => rec.IsActive &&
                       rec.TransactionTypeId == (int)transactionType &&
                       rec.ClearingHouse == clearingHouse && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);

        }
      }
      else
      {
        maxAcceptableAmount = iMaxAcceptableAmount;
      }

      // For Rejection Memo validate only net reject amount if database entries are not present for min-max amount check
      if(minAcceptableAmount == null && maxAcceptableAmount == null && isRejectionMemo && applicableMinimumField.HasValue && Convert.ToInt32(applicableMinimumField.Value) != (int)ApplicableMinimumField.TotalNetRejectAmount)
      {
        return true;
      }

      var exchangeRate = iExchangeRate ??
                         ExchangRateRepository.First(rec => rec.IsActive && rec.CurrencyId == listingCurrencyId && rec.EffectiveFromDate <= billingDate && rec.EffectiveToDate >= billingDate);
      if (invoice.BillingCategory == BillingCategoryType.Misc && (transactionType == Model.Enums.TransactionType.MiscRejection1 || transactionType == Model.Enums.TransactionType.MiscRejection2))
      {
          var miscUatpInvoice = (MiscUatpInvoice)invoice;
          DateTime rejbillingDate;
          var cultureInfo = new CultureInfo("en-US");
          cultureInfo.Calendar.TwoDigitYearMax = 2099;
          const string billingDateFormat = "yyyyMMdd";
          //To search exchange rate for the billing month.
          DateTime.TryParseExact(string.Format("{0}{1}{2}", miscUatpInvoice.SettlementYear, miscUatpInvoice.SettlementMonth.ToString("00"), "01"), billingDateFormat, cultureInfo, DateTimeStyles.None, out rejbillingDate);
          exchangeRate = iExchangeRate ??
                         ExchangRateRepository.First(rec => rec.IsActive && rec.CurrencyId == listingCurrencyId && rec.EffectiveFromDate <= rejbillingDate && rec.EffectiveToDate >= rejbillingDate);
      }
      //No entry in the database so the validation returns true.
      if (exchangeRate == null)
      {
        return true;
      }

      // If only Min Amount has  to be validated
      if (validateMinAmount && validateMaxAmount == false)
      {
        return minAcceptableAmount == null ? netAmount >= 0 : minAcceptableAmount.Amount <= (netAmount / exchangeRate.FiveDayRateUsd);
      }

      // If only Max Amount has  to be validated
      if (validateMaxAmount && validateMinAmount == false)
      {
        return maxAcceptableAmount == null ? true : maxAcceptableAmount.Amount >= (netAmount / exchangeRate.FiveDayRateUsd);
      }
     
      // if both Min and Max amount has to be validated
      return ValidateBothAmounts(minAcceptableAmount, maxAcceptableAmount, exchangeRate, netAmount, isCreditNote);
    }

    /// <summary>
    /// To validate both Minimum and Max amounts
    /// </summary>
    /// <param name="minAcceptableAmount"></param>
    /// <param name="maxAcceptableAmount"></param>
    /// <param name="exchangeRate"></param>
    /// <param name="netAmount"></param>
    /// <param name="isCreditNote"></param>
    /// <returns></returns>
    private bool ValidateBothAmounts(MinAcceptableAmount minAcceptableAmount, MaxAcceptableAmount maxAcceptableAmount, ExchangeRate exchangeRate, double netAmount, bool isCreditNote)
    {
      bool isMinAmountValid = false, isMaxAmountValid = false;
      if (minAcceptableAmount == null)
      {
        if (isCreditNote)
          isMinAmountValid = netAmount <= 0;
        else
        {
            isMinAmountValid = true;// netAmount >= 0;
        }
      }
      else
      {
        isMinAmountValid = minAcceptableAmount.Amount <= (netAmount / exchangeRate.FiveDayRateUsd);
      }
      if (maxAcceptableAmount == null)
      {
        isMaxAmountValid = true;
      }
      else
      {
        isMaxAmountValid = maxAcceptableAmount.Amount >= (netAmount / exchangeRate.FiveDayRateUsd);
      }

      return isMinAmountValid && isMaxAmountValid;
    }

    private DateTime GetBillingDate(InvoiceBase invoice)
    {
      DateTime billingDate;

      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";

      //To search exchange rate for the billing month.
      DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear, invoice.BillingMonth.ToString("00"), "01"), billingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate);
      return billingDate;
    }

    private DateTime GetTransactionDate(InvoiceBase invoice)
    {
      DateTime billingDate;

      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";

      //To search exchange rate for the billing month.
      DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear, invoice.BillingMonth.ToString("00"), invoice.BillingPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate);
      return billingDate;
    }

    protected static bool IsDualMember(Member member)
    {
      return (member.IchMemberStatusId == (int)IchMemberShipStatus.Live || member.IchMemberStatusId == (int)IchMemberShipStatus.Suspended) &&
             (member.AchMemberStatusId == (int)AchMembershipStatus.Live || member.AchMemberStatusId == (int)AchMembershipStatus.Suspended);
    }
    
    // CMP#624 : 2.22-Change#4 - Update of ‘Clearing House’ flag
    //protected static bool IsDirectSettlementMember(Member member)
    //{
    //   // return ((member.IchMemberStatusId == 0 && member.AchMemberStatusId == 0) || member.IchMemberStatusId == (int)IchMemberShipStatus.Terminated || member.AchMemberStatusId == (int)AchMembershipStatus.Terminated);
    //    return ((member.IchMemberStatusId == 0 || member.IchMemberStatusId == (int)IchMemberShipStatus.NotAMember) &&
    //         (member.AchMemberStatusId == 0 || member.AchMemberStatusId == (int)AchMembershipStatus.NotAMember)) ||
    //        (member.IchMemberStatusId == (int)IchMemberShipStatus.Terminated &&
    //         member.AchMemberStatusId == (int)AchMembershipStatus.Terminated) ||
    //        (member.IchMemberStatusId == (int)IchMemberShipStatus.Terminated &&
    //         (member.AchMemberStatusId == 0 || member.AchMemberStatusId == (int)AchMembershipStatus.NotAMember)) ||
    //        ((member.IchMemberStatusId == 0 || member.IchMemberStatusId == (int)IchMemberShipStatus.NotAMember) &&
    //         member.AchMemberStatusId == (int)AchMembershipStatus.Terminated);
    //}

    protected static bool IsAchMember(Member member)
    {
      return (member.AchMemberStatusId == (int)AchMembershipStatus.Live || member.AchMemberStatusId == (int)AchMembershipStatus.Suspended);
    }

    protected static bool IsIchMember(Member member)
    {
      return (member.IchMemberStatusId == (int)IchMemberShipStatus.Live || member.IchMemberStatusId == (int)IchMemberShipStatus.Suspended);
    }

    /// <summary>
    /// Get clearing house from settlement method
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="billingMember"></param>
    /// <param name="billedMember"></param>
    /// <returns></returns>
    public string GetClearingHouseForInvoice(InvoiceBase invoice, Member billingMember, Member billedMember)
    {
      
      var settlementMethodId = (int) GetDefaultSettlementMethodForMembers(billingMember.Id, billedMember.Id,invoice.BillingCategoryId);
       
      var clearingHouse = string.Empty;

      // Set clearing house only if Invoice's settlement method is I or A or M.
     
      if (invoice != null && (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.Ach || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules))
      {
        if (invoice.BillingMember != null && invoice.BilledMember != null)
        {
          Logger.Info(string.Format("GetClearingHouseForInvoice: Billing Member :{0} Billed Member :{1}", invoice.BillingMember.MemberCodeNumeric, invoice.BilledMember.MemberCodeNumeric));
          Logger.Info(string.Format("GetClearingHouseForInvoice: Parent Billing Member :{0} Parent Billed Member :{1}", billingMember.MemberCodeNumeric, billedMember.MemberCodeNumeric));
       
          switch (settlementMethodId)
          {
            case (int)SMI.Ich:
              clearingHouse = ReferenceManagerConstants.ClearingHouseIch;
              break;
            case (int)SMI.Ach:
              clearingHouse = ReferenceManagerConstants.ClearingHouseAch;
              break;
            case (int)SMI.AchUsingIataRules:
              if (IsAchMember(billingMember) && IsAchMember(billedMember))
              {
                clearingHouse = ReferenceManagerConstants.ClearingHouseAch;
              }
              else if (IsAchMember(billingMember) && IsIchMember(billedMember))
              {
                clearingHouse = ReferenceManagerConstants.ClearingHouseBoth;
              }
              else if (IsAchMember(billingMember) && IsDualMember(billedMember))
              {
                clearingHouse = ReferenceManagerConstants.ClearingHouseAch;
              }
              else if (IsDualMember(billingMember) && IsAchMember(billedMember))
              {
                clearingHouse = ReferenceManagerConstants.ClearingHouseAch;
              }
              else if (IsDualMember(billingMember) && IsDualMember(billedMember))
              {
                clearingHouse = ReferenceManagerConstants.ClearingHouseAch;
              }
              break;
          }
        }
      }// End if

      // CMP#624 : 2.22-Change#4 - Update of ‘Clearing House’ flag
      if (invoice != null && invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        clearingHouse = ReferenceManagerConstants.ClearingHouseIch;
      }

      Logger.Info("Clearing House : " + clearingHouse);
      return clearingHouse;
    }

    /// <summary>
    /// Return default value for settlement indicator for given combination of billing and billed members
    /// </summary>
    /// <param name="billingMemberId">Billing Member Id</param>
    /// <param name="billedMemberId">Billed Member Id</param>
    /// <param name="billingCategoryId">Billing Category Id</param>
    /// <returns></returns>
    public SMI GetDefaultSettlementMethodForMembers(int billingMemberId, int billedMemberId, int billingCategoryId)
    {
        var settlementMethod = SMI.Bilateral;
        var billingMember = MemberRepository.Get(m => m.Id == billingMemberId).FirstOrDefault();
        var billedMember = MemberRepository.Get(m => m.Id == billedMemberId).FirstOrDefault();

        if (billingMember != null && billedMember != null)
        {
            var invoiceManager = Ioc.Resolve<IInvoiceManager>(typeof(IInvoiceManager));

            //SCP304880 - SRM: Invoices not presented in UAT
            if (invoiceManager.CheckIfSettlementIsBilateral(billingMember, billedMember))
            {
                settlementMethod = SMI.Bilateral;
            }
             // Both the members are DUAL.
            else if (IsDualMember(billingMember) && IsDualMember(billedMember))
            {
              var memberManager = Ioc.Resolve<IMemberManager>(typeof (IMemberManager));
              var achExceptionsBillingMember = memberManager.GetExceptionMembers(billingMember.Id, billingCategoryId, false).ToList();

              settlementMethod = achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == billedMember.Id) > 0 ? SMI.Ich : SMI.Ach;
              Ioc.Release(memberManager);
            }

              // Billing member is Dual.
            else if (IsDualMember(billingMember))
            {
                settlementMethod = IsIchMember(billedMember)
                                       ? SMI.Ich
                                       : IsAchMember(billedMember) ? SMI.Ach : SMI.Bilateral;
            }
                // Billing member is ICH.
            else if (IsIchMember(billingMember))
            {
                settlementMethod = SMI.Ich;
            }

                // Billing member is only ACH.
            else if (IsAchMember(billingMember))
            {
                if (IsDualMember(billedMember))
                {
                    settlementMethod = SMI.Ach;
                }
                else if (IsAchMember(billedMember))
                {
                    settlementMethod = SMI.Ach;
                }
                else if (IsIchMember(billedMember))
                {
                    settlementMethod = SMI.AchUsingIataRules;
                }
            }
        }
        return settlementMethod;
    }


    /// <summary>
    /// Get clearing house from settlement method
    /// </summary>
    /// <param name = "settlementMethodId"></param>
    /// <returns></returns>
    public string GetClearingHouseFromSMI(int settlementMethodId)
    {
        var clearingHouse = string.Empty;
        if (settlementMethodId == (int)SMI.Ach)
        {
            clearingHouse = "A";
        }
        else //if (settlementMethodId == (int)SMI.Ich || IsSmiLikeBilateral(settlementMethodId) || settlementMethodId == (int)SMI.AchUsingIataRules)
        {
            clearingHouse = "I";
        }
        return clearingHouse;
    }

    /// <summary>
    /// Gets clearing house from SMI.
    /// </summary>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    private string GetClearingHouseForValidation(int settlementMethodId)
    {
      var clearingHouse = string.Empty;
      if (settlementMethodId == (int)SMI.Ach)
      {
        clearingHouse = "A";
      }
      else //if (settlementMethodId == (int)SMI.Ich || IsSmiLikeBilateral(settlementMethodId) || settlementMethodId == (int)SMI.AchUsingIataRules)
      {
        clearingHouse = "I";
      }

      return clearingHouse;
    }

    /// <summary>
    /// Gets the clearing house enum.
    /// </summary>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    public ClearingHouse GetClearingHouseToFetchCurrentBillingPeriod(int settlementMethodId)
    {
      if (settlementMethodId == (int)SMI.Ach || settlementMethodId == (int)SMI.AchUsingIataRules)
      {
        return ClearingHouse.Ach;
      }
      return ClearingHouse.Ich;
    }

    /// <summary>
    /// Gets the clearing house enum.
    /// </summary>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    public ClearingHouse GetClearingHouseEnum(int settlementMethodId)
    {
      return GetClearingHouseFromSMI(settlementMethodId) == "A" ? ClearingHouse.Ach : ClearingHouse.Ich;
    }

    /// <summary>
    /// Gets the type of the transaction time limit transaction.
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "clearingHouse"></param>
    /// <returns></returns>
    public TimeLimit GetTransactionTimeLimitTransactionType(Model.Enums.TransactionType transactionType, string clearingHouse)
    {
      var timeLimit = TimeLimitRepository.Single(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse);
      return timeLimit;
    }

    /// <summary>
    /// Gets the type of the transaction time limit transaction.
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId"></param>
    /// <param name = "yourBillingPeriod"></param>
    /// <returns></returns>
    public TimeLimit GetTransactionTimeLimitTransactionType(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime yourBillingPeriod)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      settlementMethodId = GetSettlementMethodforTimeLimit(settlementMethodId);
      var timeLimit = TimeLimitRepository.First(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId && yourBillingPeriod >= rec.EffectiveFromPeriod && yourBillingPeriod <= rec.EffectiveToPeriod);
      return timeLimit;
    }

    public TimeLimit GetTransactionTimeLimitTransactionType(Model.Enums.TransactionType transactionType, DateTime yourBillingPeriod)
    {
      var timeLimit = TimeLimitRepository.First(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && yourBillingPeriod >= rec.EffectiveFromPeriod && yourBillingPeriod <= rec.EffectiveToPeriod);
      return timeLimit;
    }

    /// <summary>
    /// Gets the lead period.
    /// </summary>
    /// <param name="clearingHouse"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <returns></returns>
    public LeadPeriod GetLeadPeriod(string clearingHouse, int billingCategoryId, DateTime yourBillingPeriod)
    {
      var leadPeriod = LeadPeriodRepository.First(rec => rec.IsActive && rec.ClearingHouse == clearingHouse && rec.BillingCategoryId == billingCategoryId && yourBillingPeriod >= rec.EffectiveFromPeriod && yourBillingPeriod <= rec.EffectiveToPeriod);
      return leadPeriod;
    }

    [Obsolete]
    public TimeLimit GetTransactionTimeLimitTransactionType(Model.Enums.TransactionType transactionType, int settlementMethodId)
    {
        settlementMethodId = GetSettlementMethodforTimeLimit(settlementMethodId);
      var timeLimit = TimeLimitRepository.First(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId);
      return timeLimit;
    }

    /// <summary>
    /// Checks if the transaction is in time limit or not. Used in Pax to display warning message.
    /// </summary>
    /// <param name="transactionType"></param>
    /// <param name="settlementMethodId">SMI of the invoice being rejected.</param>
    /// <param name="billingYear">Billing Year of the invoice being rejected.</param>
    /// <param name="billingMonth">Billing month of the invoice being rejected.</param>
    /// <param name="billingPeriod">Billing period of the invoice being rejected.</param>
    /// <returns></returns>
    public bool IsTransactionInTimeLimitMethodH(Model.Enums.TransactionType transactionType, int settlementMethodId, int billingYear, int billingMonth, int billingPeriod)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var effectiveBillingPeriod = new DateTime(billingYear, billingMonth, billingPeriod);
      TimeLimit timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);

      if (timeLimit == null)
      {
        return true;
      }

      try
      {
        var startDate = new DateTime(billingYear, billingMonth, 1);
        var endDate = startDate.AddMonths(timeLimit.Limit);
        var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);
        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        return currentBillingPeriod <= endBillingPeriod;
      }
      catch (Exception)
      {
        return true;
      }
    }

    public bool IsSmiLikeBilateral(int settlementMethodId, bool isSmiXbehaveLikeBilateral)
    {
        /* E.g. ->
         * Case 1: settlementMethodId = "any Non bilateral" and isSmiXbehaveLikeBilateral = true (default/explicitly provided as input)
         * if -> becomes true and input is not bilateral smi so return is false.
         * 
         * Case 2: settlementMethodId = "any bilateral other than X" and isSmiXbehaveLikeBilateral = true/False (default/explicitly provided as input)
         * alwasys return becomes true
         * 
         * Case 3: settlementMethodId = "SMI X" and isSmiXbehaveLikeBilateral = true (default/explicitly provided as input)
         * if -> becomes true and so SMI X will be treated as a Bilateral SMI, return is true.
         * 
         * Case 4: settlementMethodId = "SMI X" and isSmiXbehaveLikeBilateral = false
         * if -> becomes false.
         * in else -> SMI X is bilateral but later part of condition is false.
         * Hence SMI X will not be treated as a Bilateral, return is false.
         * 
         */
        if (isSmiXbehaveLikeBilateral)
        {
            return SMIsTreatedAsBilateral.Contains(settlementMethodId);
        }
        else
        {
            return (SMIsTreatedAsBilateral.Contains(settlementMethodId) &&
                    settlementMethodId != (int) SMI.IchSpecialAgreement);
        }
    }

    /// <summary>
    /// Check if SMI is valid for Late Submission    
    /// </summary>
    /// <param name="settlementMethodId">SMI</param>
    /// <returns>True/False</returns>
    public bool IsValidSmiForLateSubmission(int settlementMethodId)
    {
      /*
       * SCP359227 - SRM: SMI - P in period error
      The following SMIs will be considered eligible for Late Submission:
       1)     SMI “I”, using the ICH calendar
       2)     SMI “X”, using the ICH calendar, even though this is treated as a Bilateral SMI as per master Settlement Method Setup
       3)     SMI “A”, using the ACH calendar
       4)     SMI “M”, using the ACH calendar
       * 
       After eliminating the SMIs listed above, the following SMIs will be considered ineligible for Late Submission.
       a)     SMI “R”, even though this is not treated as a Bilateral SMI as per master Settlement Method Setup
       b)    Any other SMI that is treated as a Bilateral SMI as per master Settlement Method Setup. Currently SMIs “B” and “P” fall into this category. 
             This is not a fixed list. Any other SMI added by IATA in the future as a Bilateral SMI will automatically be considered in this category

       PAX Form Cs submitted using SMI “N” follows a different flow. For these, the calendar is not used, and master Sample Digit Setup is used.
       */
      switch (settlementMethodId)
      {
        case (int)SMI.Ich:
        case (int)SMI.Ach:
        case (int)SMI.IchSpecialAgreement:
        case (int)SMI.AchUsingIataRules:
          return true;    
        default:
          return false;
      }
    }

    public bool IsTransactionInTimeLimitMethodA(Model.Enums.TransactionType transactionType, int settlementMethodId, MiscUatpInvoice invoice)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      TimeLimit timeLimit;
      var effectiveBillingPeriod = new DateTime(invoice.SettlementYear, invoice.SettlementMonth, invoice.SettlementPeriod);
      if (ValidationCache.Instance.ValidTimeLimits != null)
      {
        //Get the time limit from the validation cache.
        timeLimit = ValidationCache.Instance.ValidTimeLimits.Find(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId && rec.EffectiveFromPeriod <= effectiveBillingPeriod && rec.EffectiveToPeriod >= effectiveBillingPeriod);
      }
      else
      {
        //Get the time limit from the database. 
        timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      }
      if (timeLimit == null)
      {
        return true;
      }

      var startDate = new DateTime(invoice.SettlementYear, invoice.SettlementMonth, 1);
      var endDate = startDate.AddMonths(timeLimit.Limit);
      var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      return currentBillingPeriod <= endBillingPeriod;
    }

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// The month/year portion of the time limit for the subsequent/follow-up transaction is calculated by adding ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â¹ÃƒÆ’Ã¢â‚¬Â¦ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œXÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¾ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ number of months to the month of the previous transaction/uplift.
    /// The period portion of the time limit for the subsequent transaction will always be considered as 4.
    /// E. g. 
    /// 1. PAX Prime -> RM Stage 1
    /// 2. MISC invoice  MISC Rejection Invoice #1
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "rejectionMemo">The rejection memo.</param>
    /// <param name = "invoice">The invoice.</param>
    /// <param name="currentBillingPeriod"></param>
    /// <returns>
    /// true if transaction in time limit ; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodA(Model.Enums.TransactionType transactionType, int settlementMethodId, RejectionMemo rejectionMemo, InvoiceBase invoice, BillingPeriod? currentBillingPeriod)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      //Also useful for -
      //FRS Section 2.14 Change #3: Update of TL flag in ‘IS Validation Flag’ for PAX/CGO RMs
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      TimeLimit timeLimit;

      DateTime effectiveBillingPeriod;
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";

      if (!DateTime.TryParseExact(string.Format("{0}{1}{2}", rejectionMemo.YourInvoiceBillingYear.ToString("0000"), rejectionMemo.YourInvoiceBillingMonth.ToString("00"), rejectionMemo.YourInvoiceBillingPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out effectiveBillingPeriod))
      {
        return true;
      }

      //var effectiveBillingPeriod = new DateTime(rejectionMemo.YourInvoiceBillingYear, rejectionMemo.YourInvoiceBillingMonth, rejectionMemo.YourInvoiceBillingPeriod);
      if (invoice.ValidTimeLimits != null)
      {
        //Get the time limit from the validation cache
        timeLimit = invoice.ValidTimeLimits.Find(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId && rec.EffectiveFromPeriod <= effectiveBillingPeriod && rec.EffectiveToPeriod >= effectiveBillingPeriod);
      }
      else
      {
        //Get the time limit from the database 
        timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      }

      if (timeLimit == null)
      {
        return true;
      }

      try
      {
        var startDate = new DateTime(rejectionMemo.YourInvoiceBillingYear, rejectionMemo.YourInvoiceBillingMonth, 1);
        var endDate = startDate.AddMonths(timeLimit.Limit);
        var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);
        if (currentBillingPeriod == null)
        {
            var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
            currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        }
          return currentBillingPeriod <= endBillingPeriod;
      }
      catch (Exception)
      {
        return true;
      }

    }

    /// <summary>
    /// Gets the expiry date period method A.
    /// </summary>
    /// <param name="transactionType">Type of the next transaction.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="samplingIndicator">The sampling indicator.</param>
    /// <param name="currentBillingPeriod">The current billing period.</param>
    /// <param name="isCorrespondence">if set to <c>true</c> [is correspondence].</param>
    /// <param name="correspondenceBase">The correspondence base.</param>
    /// <returns></returns>
    public DateTime GetExpiryDatePeriodMethod(Model.Enums.TransactionType transactionType, InvoiceBase invoice, BillingCategoryType billingCategory, string samplingIndicator, BillingPeriod? currentBillingPeriod, bool isCorrespondence = false, CorrespondenceBase correspondenceBase = null)
    {
      TimeLimit timeLimit;
      LeadPeriod leadPeriod;
      DateTime billingDate;
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      if (invoice.SubmissionMethodId != (int)SubmissionMethod.IsWeb)
      {
        if (!DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear.ToString("0000"), invoice.BillingMonth.ToString("00"), invoice.BillingPeriod.ToString("00")), BillingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate))
          // Records with this value are not purged.
          return DateTime.MinValue;
      }

      DateTime effectiveBillingPeriod;
      if (isCorrespondence && correspondenceBase != null)
      {
        // For correspondence, correspondence Date will be compared with Effective From-To Period.
        effectiveBillingPeriod = new DateTime(correspondenceBase.CorrespondenceDate.Year, correspondenceBase.CorrespondenceDate.Month, correspondenceBase.CorrespondenceDate.Day);
      }
      else
      {
        // Period of current transaction.
        effectiveBillingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
      }

      int settlementMethodId = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      if (IsSmiLikeBilateral(invoice.SettlementMethodId,false))
      {
        settlementMethodId = Convert.ToInt32(SMI.Bilateral);
      }

      if (invoice.ValidTimeLimits != null)
      {
        //Get the time limit from the validation cache
        timeLimit = invoice.ValidTimeLimits.Find(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId && rec.EffectiveFromPeriod <= effectiveBillingPeriod && rec.EffectiveToPeriod >= effectiveBillingPeriod);
      }
      else
      {
        //Get the time limit from the database 
        timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      }

      // get lead period.
      string clearingHouse = GetClearingHouseFromSMI(settlementMethodId);
      if (invoice.ValidLeadPeriods != null)
      {
        leadPeriod = invoice.ValidLeadPeriods.Find(rec => rec.IsActive && rec.ClearingHouse == clearingHouse && rec.BillingCategoryId == (int)billingCategory && effectiveBillingPeriod >= rec.EffectiveFromPeriod && effectiveBillingPeriod <= rec.EffectiveToPeriod);
      }
      else
      {
        leadPeriod = GetLeadPeriod(clearingHouse, (int)billingCategory, effectiveBillingPeriod);
      }

      if (timeLimit == null)
      {
        SendTimeLimitAlert(GetSettlementMethodforTimeLimit(settlementMethodId), transactionType, effectiveBillingPeriod,invoice);
        timeLimit = new TimeLimit() { Limit = 6 };
      }

      if (leadPeriod == null)
      {
          SendLeadPeriodAlert(clearingHouse, billingCategory, samplingIndicator, effectiveBillingPeriod, invoice);
        leadPeriod = new LeadPeriod { Period = 2 };
      }

      try
      {
        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        if (currentBillingPeriod == null) currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        var currentBillingPeriodDate = new DateTime(currentBillingPeriod.Value.Year, currentBillingPeriod.Value.Month, currentBillingPeriod.Value.Period);
        DateTime expiryPeriodDate;
        // For Misc./Uatp correspondence with SMI 'ACH', time limit is in days.
        // Calculation Method - F/G.
        if ((transactionType == Model.Enums.TransactionType.MiscCorrespondence || transactionType == Model.Enums.TransactionType.UatpCorrespondence || transactionType == Model.Enums.TransactionType.MiscOtherCorrespondence || transactionType == Model.Enums.TransactionType.UatpOtherCorrespondence) && settlementMethodId == (int)SMI.Ach)
        {
          var lastDayOfMonthDate = new DateTime(currentBillingPeriodDate.Year, currentBillingPeriodDate.Month, DateTime.DaysInMonth(currentBillingPeriodDate.Year, currentBillingPeriodDate.Month));
          expiryPeriodDate = lastDayOfMonthDate.AddDays(timeLimit.Limit).AddMonths(leadPeriod.Period);
        }
        else
          expiryPeriodDate = currentBillingPeriodDate.AddMonths(timeLimit.Limit + leadPeriod.Period);

        // Check if Correspondence is not null and Correspondence BMExpiry is Set then
        // if expiryPeriodDateWithLead (calculated based on expiry date time limit) > bmExpiryPeriodWithLead (calculated based on BM expiry Period) then set pergePeriod as expiryPeriodDate.
        // else set pergePeriod as bmExpiryPeriodWithLead.
        if (correspondenceBase != null && correspondenceBase.BMExpiryPeriod.HasValue)
        {
          // If day = 4 if day in expiryPeriodDate is greater than 4
          var expiryPeriodDateWithLead = new DateTime(expiryPeriodDate.Year,expiryPeriodDate.Month,4);
          var bmExpiryPeriodWithLead = correspondenceBase.BMExpiryPeriod.Value.AddMonths(leadPeriod.Period);
          expiryPeriodDate = (expiryPeriodDateWithLead < bmExpiryPeriodWithLead) ? bmExpiryPeriodWithLead : expiryPeriodDateWithLead;
        }// End 

        return expiryPeriodDate;
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat(string.Format("Error occurred while calculating expiry date for purging for invoice {0}, billingCategory {1}:", invoice.InvoiceNumber, invoice.BillingCategoryId), exception);
        // Records with this value are not purged.
        return DateTime.MinValue;
      }
    }

    public DateTime GetExpiryDatePeriodForClosedCorrespondence(InvoiceBase invoice, BillingCategoryType billingCategory, string samplingIndicator, BillingPeriod? currentBillingPeriod)
    {
      LeadPeriod leadPeriod;
      DateTime billingDate;
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      if (invoice.SubmissionMethodId != (int)SubmissionMethod.IsWeb)
      {
        if (!DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear.ToString("0000"), invoice.BillingMonth.ToString("00"), invoice.BillingPeriod.ToString("00")), BillingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate))
          // Records with this value are not purged.
          return DateTime.MinValue;
      }

      // Get period of previous transaction.
      var effectiveBillingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);

      int settlementMethodId = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      if (IsSmiLikeBilateral(invoice.SettlementMethodId,false))
      {
        settlementMethodId = Convert.ToInt32(SMI.Bilateral);
      }

      // get lead period.
      string clearingHouse = GetClearingHouseFromSMI(settlementMethodId);
      if (invoice.ValidLeadPeriods != null)
      {
        leadPeriod = invoice.ValidLeadPeriods.Find(rec => rec.IsActive && rec.ClearingHouse == clearingHouse && rec.BillingCategoryId == (int)billingCategory && effectiveBillingPeriod >= rec.EffectiveFromPeriod && effectiveBillingPeriod <= rec.EffectiveToPeriod);
      }
      else
      {
        leadPeriod = GetLeadPeriod(clearingHouse, (int)billingCategory, effectiveBillingPeriod);
      }

      if (leadPeriod == null)
      {
          SendLeadPeriodAlert(clearingHouse, billingCategory, samplingIndicator, effectiveBillingPeriod, invoice);
        leadPeriod = new LeadPeriod { Period = 2 };
      }

      try
      {
        if (currentBillingPeriod == null)
        {
            var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
            currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        }
        var currentBillingPeriodDate = new DateTime(currentBillingPeriod.Value.Year, currentBillingPeriod.Value.Month, currentBillingPeriod.Value.Period);
        var expiryPeriodDate = currentBillingPeriodDate.AddMonths(leadPeriod.Period);

        return expiryPeriodDate;
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat(string.Format("Error occurred while calculating expiry date for purging for closed correspondence. Invoice number {0}, billingCategory {1}:", invoice.InvoiceNumber, invoice.BillingCategoryId), exception);
        // Records with this value are not purged.
        return DateTime.MinValue;
      }
    }

    public bool IsTransactionInTimeLimitMethodA(Model.Enums.TransactionType transactionType, int settlementMethodId, CargoRejectionMemo rejectionMemo, InvoiceBase invoice, BillingPeriod? currentBillingPeriod)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      //Also useful for -
      //FRS Section 2.14 Change #3: Update of TL flag in ‘IS Validation Flag’ for PAX/CGO RMs
      if(settlementMethodId== (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int) SMI.Ich;
      }

      TimeLimit timeLimit;

      DateTime effectiveBillingPeriod;
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";

      if (!DateTime.TryParseExact(string.Format("{0}{1}{2}", rejectionMemo.YourInvoiceBillingYear.ToString("0000"), rejectionMemo.YourInvoiceBillingMonth.ToString("00"), rejectionMemo.YourInvoiceBillingPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out effectiveBillingPeriod))
      {
        return true;
      }

      //var effectiveBillingPeriod = new DateTime(rejectionMemo.YourInvoiceBillingYear, rejectionMemo.YourInvoiceBillingMonth, rejectionMemo.YourInvoiceBillingPeriod);
      if (invoice.ValidTimeLimits != null)
      {
        //Get the time limit from the validation cache
        timeLimit = invoice.ValidTimeLimits.Find(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId && rec.EffectiveFromPeriod <= effectiveBillingPeriod && rec.EffectiveToPeriod >= effectiveBillingPeriod);
      }
      else
      {
        //Get the time limit from the database 
        timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      }

      if (timeLimit == null)
      {
        return true;
      }

      try
      {
        var startDate = new DateTime(rejectionMemo.YourInvoiceBillingYear, rejectionMemo.YourInvoiceBillingMonth, 1);
        var endDate = startDate.AddMonths(timeLimit.Limit);
        var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);

        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        if (currentBillingPeriod == null) currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        return currentBillingPeriod <= endBillingPeriod;
      }
      catch (Exception)
      {
        return true;
      }

    }


    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to the month of the uplift/service delivery.
    /// The last calendar date of this month (newly calculated month) will be the time limit.
    /// The MISC invoice or PAX/CGO invoice containing the transaction should be validated successfully
    /// (i.e. status set as ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“Ready for BillingÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢) on/before the time limit date.
    /// e.g 
    /// MISC service delivery  Invoice
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "invoice">The invoice.</param>
    /// <returns>
    /// true if transaction in time limit ; otherwise, false.
    /// </returns>
    [Obsolete]
    public bool IsTransactionInTimeLimitMethodA1(Model.Enums.TransactionType transactionType, int settlementMethodId, InvoiceBase invoice)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId);
      if (timeLimit == null)
      {
        return true;
      }

      var clearingHouse = GetClearingHouseFromSMI(settlementMethodId) == "I" ? ClearingHouse.Ich : ClearingHouse.Ach;
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var startDate = calendarManager.GetBillingPeriod(clearingHouse, invoice.InvoiceBillingPeriod.Year, invoice.InvoiceBillingPeriod.Month, invoice.InvoiceBillingPeriod.Period).EndDate;
      var limitDate = startDate.AddMonths(timeLimit.Limit);
      var endDate = new DateTime(limitDate.Year, limitDate.Month, DateTime.DaysInMonth(limitDate.Year, limitDate.Month));
      return (DateTime.UtcNow <= endDate);

    }


    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â¹ÃƒÆ’Ã¢â‚¬Â¦ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œXÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¾ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ number of months is added to the date of successful validation of the previous MISC invoice or PAX/CGO invoice
    /// containing the previous transaction (i.e. invoice status set as ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â¹ÃƒÆ’Ã¢â‚¬Â¦ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œReady for BillingÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¾ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢).
    /// The MISC rejection invoice or the PAX/CGO invoice containing the rejection should be validated successfully
    /// (i.e. status set as ÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â¹ÃƒÆ’Ã¢â‚¬Â¦ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œReady for BillingÃƒÆ’Ã†â€™Ãƒâ€ Ã¢â‚¬â„¢ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¡ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€¦Ã‚Â¾ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â¢) on/before the time limit date.
    /// </summary>
    /// <param name = "transactionType"></param>
    /// <param name = "settlementMethodId"></param>
    /// <param name = "invoice"></param>
    /// <returns>
    /// true if transaction in time limit ; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodA2(Model.Enums.TransactionType transactionType, int settlementMethodId, InvoiceBase invoice)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      //Also useful for -
      //FRS Section 2.14 Change #3: Update of TL flag in ‘IS Validation Flag’ for PAX/CGO RMs
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      TimeLimit timeLimit;
      var effectiveBillingPeriod = invoice.ValidationDate;
      if (invoice.ValidTimeLimits != null)
      {
        //Get time limit from the validation cache
        timeLimit = invoice.ValidTimeLimits.Find(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.SettlementMethodId == settlementMethodId && rec.EffectiveFromPeriod <= effectiveBillingPeriod && rec.EffectiveToPeriod >= effectiveBillingPeriod);
      }
      else
      {
        //Get time limit from the database
        timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      }

      if (timeLimit == null)
      {
        return true;
      }
      DateTime invoiceBillingDate;
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;

      var startDate = new DateTime(invoice.ValidationDate.Year, invoice.ValidationDate.Month, 1);
      var endDate = startDate.AddMonths(timeLimit.Limit);
      var billingDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);
      var compareValue = DateTime.Compare(billingDate, endDate);
      //If billing date is less than or equal to enddate, transaction is in timelimit.
      return (compareValue < 0 || compareValue == 0);

    }

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// Closure date of the 4th period of the billing month of the final rejection is determined (from the IS Calendar).
    /// The actual period of the last rejection has no significance. ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to that date to determine
    /// the time limit for the 1st correspondence.
    /// </summary>
    /// <param name = "transactionType">Correspondence transaction type.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">The correspondence date.</param>
    /// <param name = "invoice">Invoice of correspondence#1 (i.e. 3rd rejection invoice)</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodB(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice, DateTime currentTransactionDate, ref bool isTimeLimitRecordFound)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, correspondenceDate);
      if (timeLimit == null)
      {
          /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
          Desc: In cases when no time limit record is available, user is restricted from new transaction creation. 
          Prior ro this code was returning true, so as to allow creation of new transaction. */
          isTimeLimitRecordFound = false;
          return false;
      }

      // Closure date of 3rd rejection invoice Rejection#3 -> Correspondence #1
      var clearingHouse = GetClearingHouseFromSMI(settlementMethodId) == "A" ? ClearingHouse.Ach : ClearingHouse.Ich;
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      // Get end date of the fourth period of the billing month, year.
      var startDate = calendarManager.GetBillingPeriod(clearingHouse, invoice.InvoiceBillingPeriod.Year, invoice.InvoiceBillingPeriod.Month, 4).EndDate;
      var endDate = startDate.AddMonths(timeLimit.Limit);

      return (currentTransactionDate <= endDate); // invoice.BillingMonth <= endDate.Month && invoice.BillingYear <= endDate.Year);
    }

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to the date of successful validation of the final MISC rejection invoice or
    /// PAX/CGO invoice containing the final rejection (i.e. invoice status set as ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“Ready for BillingÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢).
    /// This shall be the time limit for the 1st correspondence.
    /// e.g. MISC Rejection Invoice #1  Correspondence #1
    /// PAX 3rd Rejection  Correspondence #1
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">The correspondence date.</param>
    /// <param name = "invoice">Misc final rejection/PAX containing final rejection stage.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodB1(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice, DateTime currentTransactionDate)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var effectiveBillingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      if (timeLimit == null)
      {
        return true;
      }

      // Successful validation of invoice Rejection Invoice .
      var startDate = invoice.ValidationDate;
      var endDate = startDate.AddMonths(timeLimit.Limit);
      return (currentTransactionDate <= endDate);
    }

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to the previous correspondenceÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢s transmission/sending date.
    /// e.g.
    /// 1. MISC 1st correspondence  2nd correspondence
    /// 2. MISC 2nd correspondence  3rd correspondence
    /// 3. PAX 3rd correspondence  4th correspondence
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">PAX/Misc correspondence date.</param>
    /// <param name = "previousCorrespondenceDate">Previous PAX/Misc correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodC(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, DateTime previousCorrespondenceDate)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, previousCorrespondenceDate);
      if (timeLimit == null)
      {
        return true;
      }

      var endDate = correspondenceDate.AddMonths(timeLimit.Limit);
      return (correspondenceDate <= endDate);
    }

    /// <summary>
    /// Gets the time limit method expiry date.
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">The correspondence date.</param>
    /// <returns></returns>
    public DateTime GetTimeLimitMethodExpiryDate(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, correspondenceDate);
      if (timeLimit == null)
      {
        // Send failure alert when time limit entry not found in database
        SendTimeLimitAlert(settlementMethodId, transactionType, correspondenceDate,invoice);

        return correspondenceDate.AddMonths(DefaultTimeLimit);
      }

      return correspondenceDate.AddMonths(timeLimit.Limit);
    }

    public DateTime GetTimeLimitMethodExpiryDateMethodG(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, correspondenceDate);
      if (timeLimit == null)
      {
        // Send failure alert when time limit entry not found in database
        SendTimeLimitAlert(settlementMethodId, transactionType, correspondenceDate,invoice);

        return correspondenceDate.AddMonths(DefaultTimeLimit);
      }

      return correspondenceDate.AddDays(timeLimit.Limit);
    }

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// The month/year portion of the time limit for the subsequent/follow-up transaction is calculated by adding ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢
    /// number of months to the month of the last correspondence that has not been responded to within time limits;
    /// or to the month of the correspondence that has an authority to bill.
    /// The period portion of the time limit for the subsequent transaction will always be considered as 4.
    /// e.g. 
    /// 1. MISC 3rd Correspondence (expired)  Correspondence invoice
    /// 2. PAX 2nd Correspondence (having authority to bill)  BM
    /// 3. MISC 4th Correspondence (having authority to bill)  Correspondence invoice
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">PAX/Misc correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodD(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, correspondenceDate);
      if (timeLimit == null)
      {
        return true;
      }

      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      var endDate = correspondenceDate.AddMonths(timeLimit.Limit);
      var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);
      return (currentBillingPeriod <= endBillingPeriod);
    }

    /// <summary>
    /// Time limit is calculated as a period of a clearance month.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to the last correspondenceÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢s transmission/sending date.
    /// The last calendar date of this month (newly calculated month) will be the time limit for the
    /// MISC correspondence invoice or the PAX/CGO invoice containing the BM. The MISC correspondence invoice or
    /// the PAX/CGO invoice containing the BM should be validated successfully (i.e. status set as ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“Ready for BillingÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢)
    /// on/before the time limit date.
    /// e.g.
    /// 1. MISC 3rd Correspondence (expired)  Correspondence invoice
    /// 2. PAX 2nd Correspondence (having authority to bill)  BM
    /// 3. MISC 4th Correspondence (having authority to bill)  Correspondence invoice
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">PAX/Misc correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodD1(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, correspondenceDate);
      if (timeLimit == null)
      {
        return true;
      }

      var limitDate = correspondenceDate.AddMonths(timeLimit.Limit);
      var endDate = new DateTime(limitDate.Year, limitDate.Month, DateTime.DaysInMonth(limitDate.Year, limitDate.Month));
      return (DateTime.UtcNow <= endDate);
    }

    /// <summary>
    /// Gets the time limit expiry date for method D1.
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">The correspondence date.</param>
    /// <returns></returns>
    public DateTime GetTimeLimitMethodD1ExpiryDate(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, InvoiceBase invoice)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, correspondenceDate);
      if (timeLimit == null)
      {
        // Send failure alert when time limit entry not found in database
        SendTimeLimitAlert(settlementMethodId, transactionType, correspondenceDate,invoice);

        return correspondenceDate.AddMonths(DefaultTimeLimit);
      }

      var limitDate = correspondenceDate.AddMonths(timeLimit.Limit);
      var endDate = new DateTime(limitDate.Year, limitDate.Month, DateTime.DaysInMonth(limitDate.Year, limitDate.Month));
      return endDate;
    }

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to the month of the final rejection. 
    /// The last calendar date of this month (newly calculated month) will be the time limit for the 1st correspondence. 
    /// The actual period of the last rejection has no significance.
    /// e.g. 
    /// 1. PAX 3rd Rejection  Correspondence #1
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "invoice">Invoice with final rejection.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodE(Model.Enums.TransactionType transactionType, int settlementMethodId, InvoiceBase invoice, ref bool isTimeLimitRecordFound)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var effectiveBillingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      if (timeLimit == null)
      {
          /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
            Desc: In cases when no time limit record is available, user is restricted from new transaction creation. 
            Prior ro this code was returning true, so as to allow creation of new transaction. */
          isTimeLimitRecordFound = false;
          return false;
      }

      //var clearingHouse = GetClearingHouseFromSMI(settlementMethodId) == "I" ? ClearingHouse.Ich : ClearingHouse.Ach;
      //var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      //var startDate = calendarManager.GetBillingPeriod(clearingHouse, invoice.InvoiceBillingPeriod.Year, invoice.InvoiceBillingPeriod.Month, invoice.InvoiceBillingPeriod.Period).EndDate;

      var limitDate = effectiveBillingPeriod.AddMonths(timeLimit.Limit);
      var endDate = new DateTime(limitDate.Year, limitDate.Month, DateTime.DaysInMonth(limitDate.Year, limitDate.Month));
      return (DateTime.UtcNow <= endDate);

    }

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of months is added to the month of the final rejection. 
    /// The last calendar date of this month (newly calculated month) will be the time limit for the 1st correspondence. 
    /// The actual period of the last rejection has no significance.
    /// e.g. 
    /// 1. 2nd Rejection Invoice -> Correspondence #1
    /// Billing period of 2nd Rejection: 2010-Jan-P2
    /// Value of time limit: 90 days
    /// Last date of Jan-2010: 31-Jan-2010
    /// Time limit for the 1st Correspondence is calculated as 01-May-2010 (31-Jan-2010 + 90 days)
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "invoice">Invoice with final rejection.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodF(Model.Enums.TransactionType transactionType, int settlementMethodId, InvoiceBase invoice, ref bool isTimeLimitRecordFound)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      var effectiveBillingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, effectiveBillingPeriod);
      if (timeLimit == null)
      {
          /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
            Desc: In cases when no time limit record is available, user is restricted from new transaction creation. 
            Prior ro this code was returning true, so as to allow creation of new transaction. */
          isTimeLimitRecordFound = false;
          return false;
      }

      //var clearingHouse = GetClearingHouseFromSMI(settlementMethodId) == "I" ? ClearingHouse.Ich : ClearingHouse.Ach;
      //var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      //var startDate = calendarManager.GetBillingPeriod(clearingHouse, invoice.InvoiceBillingPeriod.Year, invoice.InvoiceBillingPeriod.Month, invoice.InvoiceBillingPeriod.Period).EndDate;
      //var lastDateOfMonth = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));

      var lastDateOfMonth = new DateTime(effectiveBillingPeriod.Year, effectiveBillingPeriod.Month, DateTime.DaysInMonth(effectiveBillingPeriod.Year, effectiveBillingPeriod.Month));

      var endDate = lastDateOfMonth.AddDays(timeLimit.Limit);
      return (DateTime.UtcNow <= endDate);

    }

    /// <summary>
    /// Time limit is calculated as a calendar date.
    /// ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã¢â‚¬Â¹Ãƒâ€¦Ã¢â‚¬Å“XÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢ number of days is added to the previous correspondenceÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¡Ãƒâ€šÃ‚Â¬ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã…Â¾Ãƒâ€šÃ‚Â¢s transmission/sending date.
    /// e.g.
    /// 1. Correspondence #2  Correspondence #3
    /// Date of Correspondence #2: 04-Feb-2013
    /// Vale of time limit: 60
    /// Time limit for the 3rd Correspondence is calculated as 05-Apr-2013 (04-Feb-2013 + 60 days)
    /// </summary>
    /// <param name = "transactionType">Type of the transaction.</param>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <param name = "correspondenceDate">The correspondence date.</param>
    /// <param name = "previousCorrespondenceDate">The previous correspondence date.</param>
    /// <returns>
    /// true if transaction in time limit; otherwise, false.
    /// </returns>
    public bool IsTransactionInTimeLimitMethodG(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime correspondenceDate, DateTime previousCorrespondenceDate)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      if (settlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        settlementMethodId = (int)SMI.Ich;
      }

      // For all SMIs to be treated as Bilateral, the master time limit should be queried with 'Bilateral' SMI.
      if (IsSmiLikeBilateral(settlementMethodId, false))
      {
        settlementMethodId = Convert.ToInt32(SMI.Bilateral);
      }

      var timeLimit = GetTransactionTimeLimitTransactionType(transactionType, settlementMethodId, previousCorrespondenceDate);
      if (timeLimit == null)
      {
        return true;
      }

      var endDate = correspondenceDate.AddDays(timeLimit.Limit);
      return (DateTime.UtcNow <= endDate);
    }


    /// <summary>
    /// Gets the additional detail list based on type (i.e. Additional Detail or notes).
    /// </summary>
    /// <param name = "additionalDetailType">Type of the additional detail.</param>
    /// <param name = "additionalDetailLevel">The additional detail level.</param>
    /// <returns></returns>
    public IList<AdditionalDetail> GetAdditionalDetailList(AdditionalDetailType additionalDetailType, AdditionalDetailLevel additionalDetailLevel)
    {
      return
        AdditionalDetailRepository.Get(addtionalDetail => addtionalDetail.IsActive && addtionalDetail.TypeId == (int)additionalDetailType && addtionalDetail.LevelId == (int)additionalDetailLevel).
          ToList();
    }

    /// <summary>
    /// Gets the charge category.
    /// </summary>
    /// <param name = "billingCategory"></param>
    /// <returns></returns>
    public IList<ChargeCategory> GetChargeCategoryList(BillingCategoryType billingCategory)
    {
      var billingCategoryValue = (int)billingCategory;
      var chargeCategories = ChargeCategoryRepository.Get(chargeCategory => chargeCategory.IsActive && chargeCategory.BillingCategoryId == billingCategoryValue);
      //   TFS_Bug_8992: CMP609: Incorrect alphabetic order of Charge category in Billing History screen
      // & TFS_Bug_8986: CMP609: System not displaying charge category in order
      return chargeCategories.OrderBy(cCat => cCat.Name).ToList();
    }

    /// <summary>
    /// Overloaded method of GetChargeCategoryList() with one more parameter 'isIncludeInactive'
    /// CMP609: MISC Changes Required as per ISW2 
    /// Gets the charge category.
    /// </summary>
    /// <param name = "billingCategory">Billing Category</param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns>IList of ChargeCategory</returns>
    public IList<ChargeCategory> GetChargeCategoryList(BillingCategoryType billingCategory, bool isIncludeInactive)
    {
      //   TFS_Bug_8992: CMP609: Incorrect alphabetic order of Charge category in Billing History screen
      // & TFS_Bug_8986: CMP609: System not displaying charge category in order
      return isIncludeInactive && billingCategory == BillingCategoryType.Misc
             ? ChargeCategoryRepository.Get(chargeCategory => chargeCategory.BillingCategoryId == (int)billingCategory).OrderBy(cCat => cCat.Name).ToList()
             : ChargeCategoryRepository.Get(chargeCategory => chargeCategory.BillingCategoryId == (int)billingCategory && chargeCategory.IsActive).OrderBy(cCat => cCat.Name).ToList();
    }


    /// <summary>
    /// This function is used to get list of charge category for master charge code type name.
    /// </summary>
    /// <param name="billingCategory"></param>
    /// <param name="isIncludeInactive"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public IList<ChargeCategory> GetChargeCategoriesForMstChargeCodeType(bool isActiveChargeCodeTypeReq)
    {
      var chargeCategoryList = (from chCat in ChargeCategoryRepository.GetAll().ToList()
                                join chCode in ChargeCodeRepository.GetAll().ToList()
                                on chCat.Id equals chCode.ChargeCategoryId
                                where chCode.IsChargeCodeTypeRequired != null && chCat.IsActive 
                                      && chCode.IsActive
                                      && chCat.BillingCategoryId == (int)BillingCategoryType.Misc
                                      && (isActiveChargeCodeTypeReq ? chCode.IsActiveChargeCodeType : true)
                                select chCat).Distinct().OrderBy(c=>c.Name).ToList();

      return chargeCategoryList;
    }

    /// <summary>
    /// This function is used to get list of charge category for master charge code type name.
    /// </summary>
    /// <param name="billingCategory"></param>
    /// <param name="isIncludeInactive"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public IList<ChargeCode> GetChargeCodeListForMstChargeCodeType(int chargeCategoryId, bool isActiveChargeCodeTypeReq)
    {
      return ChargeCodeRepository.Get(c => c.ChargeCategoryId == chargeCategoryId && c.IsActive && c.IsChargeCodeTypeRequired != null && (isActiveChargeCodeTypeReq ? c.IsActiveChargeCodeType : true)).OrderBy(cc => cc.Name).ToList();
    }

    /// <summary>
    /// Gets the charge code.
    /// </summary>
    /// <param name = "chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    public IList<ChargeCode> GetChargeCodeList(int chargeCategoryId)
    {
      var chargeCodes = ChargeCodeRepository.Get(chargecode => chargecode.IsActive && chargecode.ChargeCategoryId == chargeCategoryId).OrderBy(cc => cc.Name).ToList();
      return chargeCodes;
    }


    /// <summary>
    /// Gets the charge code.
    /// </summary>
    /// <param name = "chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    public string GetChargeCategoryName(int chargeCategoryId)
    {
      try
      {

        string chargeCategoryName = ChargeCategoryRepository.First(l => l.Id == chargeCategoryId && l.IsActive).Name;
        return chargeCategoryName;
      }
      catch (Exception)
      {

        return null;
      }
    }

    /// <summary>
    /// Gets the type of the charge code.
    /// </summary>
    /// <param name = "chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    //CMP #636:Standard Update Mobilization
    public IList<ChargeCodeType> GetChargeCodeType(int chargeCodeId, bool isActiveChargeCodeType = false)
    {
      IList<ChargeCodeType> chargeCodeTypes = new List<ChargeCodeType>();

      //If true then charge code type requirement and charge code type should be true else get all charge code type based on charge code id. 
      if (isActiveChargeCodeType)
      {
        chargeCodeTypes = (from chCode in ChargeCodeRepository.GetAll()
                           join chCodeType in ChargeCodeTypeRepository.GetAll()
                           on chCode.Id equals chCodeType.ChargeCodeId
                           where chCode.IsActiveChargeCodeType && chCodeType.IsActive
                                 && chCodeType.ChargeCodeId == chargeCodeId
                           select chCodeType).Distinct().OrderBy(cctype => cctype.Name).ToList();
      }
      else
      {
        chargeCodeTypes = ChargeCodeTypeRepository.Get(chargecodeType => chargecodeType.ChargeCodeId == chargeCodeId).OrderBy(cctype => cctype.Name).ToList();
      }
      return chargeCodeTypes;
    }

    /// <summary>
    /// Gets the city airport.
    /// </summary>
    /// <returns></returns>
    public IList<CityAirport> GetCityAirportList()
    {
      var cityAirports = CityAirportRepository.Get(cityAirport => cityAirport.IsActive);
      return cityAirports.ToList();
    }

    /// <summary>
    /// Gets the UOM code list.
    /// </summary>
    /// <returns></returns>
    public IList<UomCode> GetUomCodeList()
    {
      //Display UOMCode for uomcodetype Base
      int uomCodeTypeBaseName;
      var uomCodeTypeList = MiscCodeRepository.GetMiscCodes(MiscGroups.UomCodeType);
      try
      {
        var uomCodeTypeBase = uomCodeTypeList.Single(code => code.Description.ToLower().Contains("base"));
        uomCodeTypeBaseName = Convert.ToInt32(uomCodeTypeBase.Name);
      }
      catch (Exception)
      {
        uomCodeTypeBaseName = 4;
      }

      var umoCodes = UomCodeRepository.Get(uomCode => uomCode.IsActive && uomCode.Type == uomCodeTypeBaseName);
      return umoCodes.ToList();
    }

    /// <summary>
    /// Gets the charge code detail.
    /// </summary>
    /// <param name = "chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    public ChargeCode GetChargeCodeDetail(int chargeCodeId)
    {
      var chargeCode = ChargeCodeRepository.Single(chargecode => chargecode.IsActive && chargecode.Id == chargeCodeId);
      return chargeCode;
    }

    /// <summary>
    /// Determines whether given cityAirport code exists in master.
    /// </summary>
    /// <param name = "cityAirportCode">The city airport code.</param>
    /// <returns>
    /// true if cityAirport available in master otherwise, false.
    /// </returns>
    public bool IsValidCityAirport(string cityAirportCode)
    {
      var count = CityAirportRepository.GetCount(cityAirport => cityAirport.IsActive && cityAirport.Id == cityAirportCode);
      return count > 0 ? true : false;
    }

    /// <summary>
    /// Determines whether given Invoiceopcode exists in mst_misc_codes master.
    /// </summary>
    /// <param name = "invoiceOpCode"></param>
    /// <returns>
    /// true if cityAirport available in master otherwise, false.
    /// </returns>
    //CMP #636: Standard Update Mobilization
    public bool IsValidMiscCode(string invoiceOpCode)
    {
      var count = MiscCodeRepository.GetAllMiscCodes().Count(mc => mc.IsActive && mc.Name == invoiceOpCode && mc.Group == (int)MiscGroups.InvoiceOpCode);
      return count > 0 ? true : false;
    }

    /// <summary>
    /// Determines whether specified city code is valid city code.
    /// </summary>
    /// <param name = "cityCode">The city code.</param>
    /// <returns>
    /// true if specified city code is valid city code; otherwise, false.
    /// </returns>
    public bool IsValidCityCode(string cityCode)
    {
      return CityAirportRepository.GetCount(city => city.IsActive && city.Id == cityCode) > 0;
    }

    public List<ContactTypeGroup> GetContactTypeGroupList()
    {
      var contactGroups = ContactGroupRepository.Get(cgr => cgr.IsActive).ToList().OrderBy(cgr => cgr.Name);
      return contactGroups.ToList();
    }

    public List<ContactTypeSubGroup> GetContactTypeSubGroupList(List<int> groupIdList)
    {
      List<ContactTypeSubGroup> contactSubGroups;
      if (groupIdList.Count > 0)
      {
        contactSubGroups = ContactSubGroupRepository.Get(cgr => cgr.IsActive && groupIdList.Contains(cgr.GroupId)).OrderBy(cgr => cgr.Name).ToList();
      }
      else
      {
        contactSubGroups = ContactSubGroupRepository.Get(cgr => cgr.IsActive).OrderBy(cgr => cgr.Name).ToList();
      }

      return contactSubGroups.ToList();
    }

    public List<ContactTypeSubGroup> GetAllContactTypeSubGroupList()
    {
      var contactSubGroups = ContactSubGroupRepository.Get(cgr => cgr.IsActive).ToList();
      return contactSubGroups;
    }

    /// <summary>
    /// Returns the number of months up to which a rejection for an invoice can be raised.
    /// </summary>
    /// <param name = "settlementMethod">Settlement Method Indicator of the invoice</param>
    /// <param name = "billingCategory">Billing Category</param>
    /// <returns>Number of months</returns>
    [Obsolete]
    public int GetRejectionTimeLimit(int settlementMethod, BillingCategoryType billingCategory)
    {
      var timeLimit = TimeLimitRepository.First(limit => limit.IsActive && limit.SettlementMethodId == settlementMethod);

      // If time limit data not found for settlement method and billing category combination is should return max time limit.
      if (timeLimit == null)
      {
        // TODO: log error.
        return 12;
      }

      return timeLimit.Limit;
    }

    /// <summary>
    /// Gets the misc code.
    /// </summary>
    /// <param name = "miscGroupName">Name of the misc group.</param>
    /// <returns></returns>
    public Dictionary<int, string> GetMiscCode(MiscGroups miscGroupName)
    {
      var miscCodes = MiscCodeRepository.GetMiscCodes(miscGroupName);

      var miscCodesList = miscCodes.ToDictionary(miscCode => Convert.ToInt32(miscCode.Name), miscCode => miscCode.Description);

      return miscCodesList;
    }

    /// <summary>
    /// Gets the misc code string (overloaded method to get MISC CODE ).
    /// </summary>
    /// <param name = "miscGroupName">Name of the misc group.</param>
    /// <returns></returns>
    public Dictionary<string, string> GetMiscCodeString(MiscGroups miscGroupName)
    {
      var miscCodes = MiscCodeRepository.GetMiscCodes(miscGroupName);

      var miscCodesList = miscCodes.ToDictionary(miscCode => miscCode.Name, miscCode => miscCode.Description);

      return miscCodesList;
    }

    /// <summary>
    /// Gets the display text for given Misc code value and miscGroup.
    /// </summary>
    /// <param name = "miscGroup">The misc group.</param>
    /// <param name = "miscCodeValue">The misc code value.</param>
    /// <returns></returns>
    public string GetDisplayValue(MiscGroups miscGroup, string miscCodeValue)
    {
      var miscCodes = MiscCodeRepository.GetMiscCode(miscGroup, miscCodeValue);

      return miscCodes != null ? miscCodes.Description : string.Empty;
    }

    /// <summary>
    /// Gets the display text for given Misc code value and miscGroup.
    /// </summary>
    /// <param name = "miscGroup">The misc group.</param>
    /// <param name = "miscCodeValue">The misc code value.</param>
    /// <returns></returns>
    public string GetDisplayValue(MiscGroups miscGroup, int miscCodeValue)
    {
      var miscCodeVal = Convert.ToString(miscCodeValue);
      var miscCodes = MiscCodeRepository.GetMiscCode(miscGroup, miscCodeVal);

      return miscCodes != null ? miscCodes.Description : string.Empty;
    }

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    public string GetSettlementMethodDisplayValueForOutput(int settlementMethodId)
    {
      var settlementMethodRecord = SettlementMethodRepository.Single(rec => rec.IsActive && rec.Id == settlementMethodId);
      return settlementMethodRecord != null ? settlementMethodRecord.Name : string.Empty;
    }

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    public string GetSettlementMethodDisplayValue(int settlementMethodId)
    {
      var settlementMethodRecord = SettlementMethodRepository.Single(rec => rec.IsActive && rec.Id == settlementMethodId);
      return settlementMethodRecord != null ? settlementMethodRecord.Description : string.Empty;
    }

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name = "settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    public string GetSettlementMethodDisplayValueForSearchResult(int settlementMethodId)
    {
      var settlementMethodRecord = SettlementMethodRepository.Single(rec => rec.IsActive && rec.Id == settlementMethodId);
      return settlementMethodRecord != null ? settlementMethodRecord.Name : string.Empty;
    }

    /// <summary>
    /// Gets the settlement method display value.
    /// </summary>
    /// <param name = "ichZone">The ich zone.</param>
    /// <returns></returns>
    public string GetIchZoneDisplayValue(int ichZone)
    {
      var ichZoneRecord = IchZoneRepository.Single(rec => rec.IsActive && rec.Id == ichZone);
      if (ichZone != -1)
      {
        return ichZoneRecord != null ? ichZoneRecord.Zone : string.Empty;
      }
      else return "All";
    }

    /// <summary>
    /// Gets the invoice status display value.
    /// </summary>
    /// <param name = "fileStatusId">The file status id.</param>
    /// <returns></returns>
    public string GetFileStatusDisplayValue(int fileStatusId)
    {
      var fileStatusRecord = FileStatusRepository.Single(rec => rec.IsActive && rec.Id == fileStatusId);
      return fileStatusRecord != null ? fileStatusRecord.Name : string.Empty;
    }

    /// <summary>
    /// Gets the invoice status display value.
    /// </summary>
    /// <param name = "invoiceStatusId">The invoice status id.</param>
    /// <returns></returns>
    public string GetInvoiceStatusDisplayValue(int invoiceStatusId)
    {
      var invoiceStatusRecord = InvoiceStatusRepository.Single(rec => rec.IsActive && rec.Id == invoiceStatusId);
      return invoiceStatusRecord != null ? invoiceStatusRecord.Name : string.Empty;
    }

    /// <summary>
    /// Gets the invoice status D list.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetInvoiceStatusDList()
    {
      var invoiceStatusRecords = InvoiceStatusRepository.Get(rec => rec.IsActive);
      var invoiceStatusList = invoiceStatusRecords.ToDictionary(rec => rec.Id, rec => rec.Name);

      return invoiceStatusList;
    }

    /// <summary>
    /// Gets the invoice status list for Cargo.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetCgoInvoiceStatusList()
    {
      // Do not show Future Submission in Cargo Invoice Search dropdown.
      var invoiceStatusRecords = InvoiceStatusRepository.Get(rec => rec.IsActive && !rec.Name.ToLower().Contains("future submission"));
      var invoiceStatusList = invoiceStatusRecords.ToDictionary(rec => rec.Id, rec => rec.Name);

      return invoiceStatusList;
    }

    /// <summary>
    /// Gets the settlement method list.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetSettlementMethodList()
    {
      var settlementMethods = SettlementMethodRepository.GetAll();
      var settlementMethodList = settlementMethods.ToDictionary(rec => rec.Id, rec => rec.Description);

      return settlementMethodList;
    }

    private static List<int> GetSMIsTreatedAsBilateral()
    {
        var settlementMethods = StaticSettlementMethodRepository.Get(rec => rec.IsActive && (rec.TreatAsBilateral || rec.Description.ToLower() == BilateralSmiDescription));
      var settlementMethodList = settlementMethods.Select(rec => rec.Id).ToList();

      return settlementMethodList;
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public Dictionary<int, string> GetSmisTreatedAsBilateralList()
    {
        var settlementMethods = StaticSettlementMethodRepository.Get(rec => rec.IsActive && rec.TreatAsBilateral);
        var settlementMethodList = settlementMethods.ToDictionary(rec => rec.Id, rec => rec.Description);

        return settlementMethodList;
    }

    public SMI GetBilateralSmi(int settlementMethodId)
    {
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (IsSmiLikeBilateral(settlementMethodId, true))
        return SMI.Bilateral;

      return (SMI)settlementMethodId;
    }

    public List<int> GetSMIsToBeTreatedBilateral()
    {
      return SMIsTreatedAsBilateral;
    }

    /// <summary>
    /// Gets the settlement method list for parsing.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetSettlementMethodListForParsing()
    {
      var settlementMethods = SettlementMethodRepository.GetAll();
      var settlementMethodList = settlementMethods.ToDictionary(rec => rec.Name, rec => rec.Id.ToString());

      return settlementMethodList;
    }

    /// <summary>
    /// Gets the settlement method list for parsing.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetSettlementMethodsForParsing()
    {
      var settlementMethods = SettlementMethodRepository.GetAll();
      var settlementMethodList = settlementMethods.ToDictionary(rec => rec.Id, rec => rec.Name);

      return settlementMethodList;
    }


    /// <summary>
    /// Gets the settlement method list for parsing.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetBillingCategoryListForParsing()
    {
      var billingCategories = BillingCategoryRepository.GetAll();
      var billingCategoriesList = billingCategories.ToDictionary(rec => rec.CodeIsxml, rec => rec.Id.ToString());

      return billingCategoriesList;
    }

    /// <summary>
    /// Gets the file status list.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetFileStatusList()
    {
      var fileStatuses = FileStatusRepository.Get(rec => rec.IsActive);
      var fileStatusList = fileStatuses.ToDictionary(rec => rec.Id, rec => rec.Name);

      return fileStatusList;
    }

    /// <summary>
    /// Gets the file status list.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetTaxSubTypeList()
    {
      var taxSubTypes = TaxSubTypeRepository.Get(rec => rec.IsActive && rec.Type == "V");
      var taxSubTypeList = taxSubTypes.ToDictionary(rec => rec.Id, rec => rec.SubType);

      return taxSubTypeList;
    }

    // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
    /// <summary>
    /// Gets the Tax Sub Type list when Tax Type is Tax.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetTaxSubTypeListForTaxTypeTax()
    {
      var taxSubTypes = TaxSubTypeRepository.Get(rec => rec.IsActive && rec.Type == "T");
      var taxSubTypeList = taxSubTypes.ToDictionary(rec => rec.Id, rec => rec.SubType);

      return taxSubTypeList;
    }
    // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

    /// <summary>
    /// Gets the Billing Category list.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetBillingCategoryList()
    {
      return BillingCategoryRepository.Get(billingCategory => billingCategory.IsActive).ToDictionary(billingCategory => billingCategory.Id, billingCategory => billingCategory.Description);
    }

    /// <summary>
    /// Gets the File format list for Is-Idec,Is-Xml,Form C Xml.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, string> GetFileFormatList()
    {
      return FileFormatRepository.Get(fileFormat => fileFormat.IsActive).ToDictionary(fileFormat => fileFormat.Id, fileFormat => fileFormat.Description);
    }

    public Dictionary<int, string> GetIchZoneList()
    {
      return IchZoneRepository.Get(ichZone => ichZone.IsActive).ToDictionary(ichZone => ichZone.Id, ichZone => ichZone.Zone);
    }

    /// <summary>
    /// Add entry in is-file log and make the status of the file as Available for Download
    /// </summary>
    /// <param name = "memberId"></param>
    /// <param name = "billingCategoryType"></param>
    /// <param name = "billingPeriod"></param>
    /// <param name = "fileFormatType"></param>
    /// <param name = "filePath"></param>
    /// <param name="fileSenderRecieverType"></param>
    /// <param name = "isConsolidatedProvFile"></param>
    public void AddOutputFileEntry(int memberId, BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, FileFormatType fileFormatType, string filePath, FileSenderRecieverType fileSenderRecieverType, bool isConsolidatedProvFile = false)
    {
      Logger.InfoFormat("AddOutputFileEntry- MemberID: {0}, BillingCategoryType: {1}, BillingPeriod: {2}, FileFormatType: {3}, FilePath: {4}, FileSenderRecieverType: {5}, IsConsolidatedProvFile: {6}", memberId, billingCategoryType, billingPeriod, fileFormatType, filePath, fileSenderRecieverType, isConsolidatedProvFile);

      var period = 4;
      if (!isConsolidatedProvFile)
      {
        period = billingPeriod.Period;
      }

      var isInputFile = new IsInputFile
      {
        BillingCategory = (int)billingCategoryType,
        BillingMonth = billingPeriod.Month,
        BillingPeriod = period,
        BillingYear = billingPeriod.Year,
        FileDate = DateTime.UtcNow,
        FileFormat = fileFormatType,
        FileLocation = Path.GetDirectoryName(filePath),
        //File location should not contain file name
        FileName = Path.GetFileName(filePath),
        FileStatus = FileStatusType.AvailableForDownload,
        SenderRecieverType = (int)fileSenderRecieverType,
        FileVersion = "0.1",
        IsIncoming = true,
        ReceivedDate = DateTime.UtcNow,
        SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
        SenderReceiver = memberId,
        OutputFileDeliveryMethodId = 1
      };
      try
      {
        AddIsInputFile(isInputFile);
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred while adding entry in Is File Log table.", exception);
      }
    }

    /// <summary>
    /// Gets the UOM code type list
    /// </summary>
    /// <returns></returns>
    public IList<MiscCode> GetUomCodeTypeList()
    {
      //Display UOM code type.
      var uomCodeTypeList = MiscCodeRepository.GetUomCodeTypeList().ToList();
      return uomCodeTypeList;
    }

    public IList<FileFormat> SysMonitorGetFileFormatTypeList()
    {
      // Get the list of all the File format including ACH Recap Sheet . 17 Id is for ACH Recap Sheet

      var fileformattypes = FileFormatRepository.Get(fileformat => fileformat.IsActive || fileformat.Id == 17 || fileformat.Id == 14);

      return fileformattypes.ToList();
    }

    /// <summary>
    /// Sends the IS admin process failed notification when time limit is not found.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method ID.</param>
    /// <param name="transactionType">The transactionType.</param>
    /// <param name="effectiveBillingPeriod">The effectiveBillingPeriod.</param>
    public void SendTimeLimitAlert(int settlementMethodId, Iata.IS.Model.Enums.TransactionType transactionType, DateTime effectiveBillingPeriod, InvoiceBase invoice)
    {
      string messageBody = string.Format("Time Limit not found for SMI type: {0} and transaction type: {1} where effective billing period(DD-MM-YYYY): {2}",
                                       settlementMethodId.ToString(), transactionType.ToString(), effectiveBillingPeriod.ToString("dd-MMM-yyy"));
      if (invoice != null)
      {
          StringBuilder msg=new StringBuilder();
          msg.Append("Time Limit not found ");
          msg.Append("<br/> Billing Category :"+invoice.BillingCategory);
          if(invoice.BillingMember != null)
              msg.Append("<br/> Billing Member :" + invoice.BillingMember.MemberCodeNumeric);
           if(invoice.BilledMember != null)
               msg.Append("<br/> Billed Member :" + invoice.BilledMember.MemberCodeNumeric);
          msg.Append("<br/> Billing Year/Mon/Period :"+effectiveBillingPeriod.ToString("yyy-MMM-dd"));
          msg.Append("<br/> Invoice Number :" + invoice.InvoiceNumber);
          msg.Append("<br/> SMI Type :" + settlementMethodId.ToString());
          msg.Append("<br/> Transaction Type :" + transactionType.ToString());
          messageBody = msg.ToString();
      }
      var context = new VelocityContext();
      context.Put("Message", messageBody);

      //Call to EmailTemplateID: 95
      SendIsAdminTimeLimitFailureNotification(EmailTemplateId.SISAdminAlertTimeLimitFailureNotification, context);

    }
      /// <summary>
      /// Get Settlement menthod for Time Limit.
      /// </summary>
      /// <param name="settlementMethodId"></param>
      /// <returns></returns>
    private int GetSettlementMethodforTimeLimit(int settlementMethodId)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      switch (settlementMethodId)
        {
            case (int)SMI.Ich:
            case (int)SMI.IchSpecialAgreement:
                settlementMethodId = (int) SMI.Ich;
                break;
            /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
            Desc: Prior ro this code fix SMI M was being treated as SMI I. This bug is now fixed. */
            case (int)SMI.AchUsingIataRules:
                settlementMethodId = (int) SMI.AchUsingIataRules;
                break;
            case (int)SMI.Ach:
                settlementMethodId = (int)SMI.Ach;
                break;
            case (int)SMI.AdjustmentDueToProtest:
                settlementMethodId = (int)SMI.Bilateral;
                break;
            default:
                settlementMethodId = (int)SMI.Bilateral;
                break;
        }
        return settlementMethodId;
    }
    /// <summary>
    /// Sends the IS admin process failed notification when lead period is not found.
    /// </summary>
    /// <param name="clearingHouse">The clearingHouse.</param>
    /// <param name="billingCategory">The billingCategory.</param>
    /// <param name="samplingIndicator">The samplingIndicator.</param>
    /// <param name="effectiveBillingPeriod">The effectiveBillingPeriod.</param>
    public void SendLeadPeriodAlert(string clearingHouse, BillingCategoryType billingCategory, string samplingIndicator, DateTime effectiveBillingPeriod, InvoiceBase invoice)
    {
      string messageBody = string.Format("Lead Period not found for ClearingHouse: {0}, billing category type: {1} and sampling indicator: {2}, where effective billing period(DD-MM-YYYY): {3}",
                                      clearingHouse, billingCategory.ToString(), samplingIndicator, effectiveBillingPeriod.ToString("dd-MMM-yyy"));
      if (invoice != null)
      {
          StringBuilder msg = new StringBuilder();
          msg.Append("Lead Period not found ");
          msg.Append("<br/> Billing Category :" + invoice.BillingCategory);
          if (invoice.BillingMember != null)
              msg.Append("<br/> Billing Member :" + invoice.BillingMember.MemberCodeNumeric);
          if (invoice.BilledMember != null)
              msg.Append("<br/> Billed Member :" + invoice.BilledMember.MemberCodeNumeric);
          msg.Append("<br/> Billing Year/Mon/Period :" + effectiveBillingPeriod.ToString("yyy-MMM-dd"));
          msg.Append("<br/> Invoice Number :" + invoice.InvoiceNumber);
          msg.Append("<br/> Clearing House :" + clearingHouse.ToString());
          msg.Append("<br/> Sampling Indicator :" + samplingIndicator.ToString());
          messageBody = msg.ToString();
      }
      var context = new VelocityContext();
      context.Put("Message", messageBody);

      //Call to EmailTemplateID: 95
      SendIsAdminTimeLimitFailureNotification(EmailTemplateId.SISAdminAlertLeadPeriodFailureNotification, context);
    }

    /// <summary>
    /// Sends the IS admin process failed notification.
    /// </summary>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="context">The context.</param>
    private static void SendIsAdminTimeLimitFailureNotification(EmailTemplateId emailTemplateId, VelocityContext context)
    {
      try
      {
        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
        var emailId = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForIsAdminProcessFailedNotification = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for own profile updates contact type mail
        var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var msgForIsAdminProcessFailedNotification = new MailMessage
        {
          From = new MailAddress(emailSettingForIsAdminProcessFailedNotification.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        // Loop through the contacts list and add them to To list of mail to be sent
        string[] emailAddress = emailId.Replace(Convert.ToChar(","), Convert.ToChar(";")).Split(Convert.ToChar(";"));

        if (emailAddress.Length > 0)
        {
          for (int i = 0; i < emailAddress.Length; i++)
          {
            msgForIsAdminProcessFailedNotification.To.Add(emailAddress[i]);
          }
        }
        // Set subject of mail (replace special field placeholders with values)
        msgForIsAdminProcessFailedNotification.Subject = emailSettingForIsAdminProcessFailedNotification.SingleOrDefault().Subject;

        // Set body text of mail
        msgForIsAdminProcessFailedNotification.Body = emailToMemberPrimaryContactsText;

        // Send the mail
        if (msgForIsAdminProcessFailedNotification.To.Count() > 0)
          emailSender.Send(msgForIsAdminProcessFailedNotification);

        return;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred while sending an email to IS-Admin.", exception);
        return;
      }
    }

    /// <summary>
    /// To Validate abillSourCode
    /// </summary>
    /// <param name="sourceCodes"></param>
    /// <param name="sourceCodeId"></param>
    /// <param name="transactionTypeId"></param>
    /// <returns></returns>
    public bool IsValidABillSourceCode(IList<SourceCode> sourceCodes, int sourceCodeId, int transactionTypeId)
    {
      bool result;
      if (sourceCodes != null)
      {
        result = sourceCodes.Count(sourceCode => sourceCode.IsActive && sourceCode.SourceCodeIdentifier == sourceCodeId && sourceCode.TransactionTypeId == transactionTypeId) > 0;
      }
      else
      {
        result = IsValidSourceCode(sourceCodeId, transactionTypeId);
      }
      return result;
    }

    /// <summary>
    /// Gets all the Icao Countries
    /// </summary>
    /// <returns></returns>
    public IList<CountryIcao> GetCountryIcaoList()
    {
        // Get the list of all the active Icao countries
        var countries = CountryIcaoRepository.Get(countryIcao => countryIcao.IsActive).OrderBy(country => country.Name);

        return countries.ToList();
    }

    /// <summary>
    /// Returns the debug log object
    /// </summary>
    /// <param name="logDate"></param>
    /// <param name="logMethodName"></param>
    /// <param name="logClassName"></param>
    /// <param name="logCategory"></param>
    /// <param name="logText"></param>
    /// <param name="logUserId"></param>
    public DebugLog GetDebugLog(DateTime logDate, string logMethodName, string logClassName, string logCategory, string logText, int logUserId, string refId)
    {
        var log = new DebugLog();
        log.LogRefId = refId;
        log.LogDate = logDate;
        log.LogMethodName = logMethodName;
        log.LogClassName = logClassName;
        log.LogCategory = logCategory;
        log.LogText = logText;
        log.LogUserId = logUserId;
        return log;
    }

    /// <summary>
    /// Add debug data to database
    /// </summary>
    /// <param name="log"></param>
    public void LogDebugData(DebugLog log)
    {
        IRepository<DebugLog> _debugLogRepositoy;

        UnitOfWork _unitOfWork = new UnitOfWork(new ObjectContextAdapter());

        try
        {
            _debugLogRepositoy = new Repository<DebugLog>(_unitOfWork); ;
            _debugLogRepositoy.Add(log);
            _unitOfWork.Commit();
            Logger.Info("LogDebug: Committed");
            _debugLogRepositoy.Refresh(log);

        }
        catch (Exception exception)
        {
            Logger.ErrorFormat("Error while logging debug data.{0}", exception);

        }

    }

    //SCP210204: IS-WEB Outage (Logging)
    /// <summary>
    /// Inserts the log debug data.
    /// </summary>
    /// <param name="log">The log.</param>
    public void InsertLogDebugData(DebugLog log)
    {
      try
      {
        if ((ConfigurationManager.AppSettings["ActivateDebugDataLogger"].Trim()) == "true")
        LogDebugDataRepository.InsertLogDebugData(log);
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat("Error while logging debug data.", exception);

      }

    }


      //CMP508 : Audit Trail Download with Supporting Documents
      /// <summary>
      /// Enqueue message in AQ_TRAN_TRAIL_REPORT
      /// </summary>
      /// <param name="message">Message containing values to be enqueued</param>
      public bool EnqueTransactionTrailReport(ReportDownloadRequestMessage message)
      {
          try
          {
            string tranTrailReportQueueName = ConfigurationManager.AppSettings["ReportDownloadQueueName"];
              if (!string.IsNullOrEmpty(tranTrailReportQueueName))
              {
                  var queueHelper = new QueueHelper(tranTrailReportQueueName);
                  IDictionary<string, string> messages = new Dictionary<string, string>();
                  messages.Add("BILLING_CATEGORY_ID", ((int)message.BillingCategoryType).ToString());
                  messages.Add("USER_ID", message.UserId.ToString());
                  messages.Add("INPUT_DATA", message.InputData);
                  messages.Add("DOWNLOAD_URL", message.DownloadUrl);
                  messages.Add("MEMBER_ID", message.RequestingMemberId.ToString());
                  messages.Add("PROCESS_INDICATOR", ((int)message.OfflineReportType).ToString());
                  queueHelper.Enqueue(messages);
                  Logger.InfoFormat("Enqueued transaction trail message for {0}",
                                    Enum.GetName(typeof (BillingCategoryType), message.BillingCategoryType));
                return true;
              }
              // SCP227747: Cargo Invoice Data Download
              // Adding filure reason in logs
              Logger.InfoFormat("Error in En-Queue message for :: Billing Category Id: [{0}], User Id: [{1}], Member Id: [{2}], Correspondence No: [ {3}]",
              (int) message.BillingCategoryType, message.UserId, message.RequestingMemberId, message.CorrespondenceNumbers);
              Logger.InfoFormat("In AppSettings ReportDownloadQueueName: {0}", tranTrailReportQueueName);
              return false;
          }
          catch (Exception exception)
          {
            Logger.ErrorFormat("Error while queuing transaction trail report: {0}", exception);
            // SCP227747: Cargo Invoice Data Download
            // Adding filure reason in logs
            Logger.InfoFormat("Error in En-Queue message for :: Billing Category Id: [{0}], User Id: [{1}], Member Id: [{2}], Correspondence No: [ {3}]",
            (int)message.BillingCategoryType, message.UserId, message.RequestingMemberId, message.CorrespondenceNumbers);
            return false;
          }
      }

      //SCP219674 : InvalidAmountToBeSettled Validation
      /// <summary>
      /// Validate Correspondence Amount to be Settled
      /// </summary>
      /// <param name="billingMemoInvoice"> Invoice Base </param>
      /// <param name="bmNetAmountBilled">Billing Memo Net Amount Billed</param>
      /// <param name="corrCurrencyId">Correspondence Currency Id</param>
      /// <param name="corrAmountToBeSettled">Correspondence Amount to be Settled</param>
      /// <returns>true/false</returns>
      public bool ValidateCorrespondenceAmounttobeSettled(InvoiceBase billingMemoInvoice, ref decimal bmNetAmountBilled, int corrCurrencyId, decimal corrAmountToBeSettled, InvoiceBase originalInvoice = null)
      {
        var isValid = true;
        try
        {
          var decimalPlace = 0;
          decimal corrAmountToBeSettledinBMCurrency = bmNetAmountBilled;
          //Restore miscUatpInvoice Billing Year and Month
          int bmInvoiceBillingYear = billingMemoInvoice.BillingYear;
          int bmInvoiceBillingMonth = billingMemoInvoice.BillingMonth;

          //Get DecimalPlace value 
          switch (billingMemoInvoice.BillingCategory)
          {
            case BillingCategoryType.Pax:
              decimalPlace = Constants.PaxDecimalPlaces;
              //Check if BMInvoice has null tolerance than populate tolerance value 
              if (billingMemoInvoice.Tolerance == null)              
                billingMemoInvoice.Tolerance = CompareUtil.GetTolerance(BillingCategoryType.Pax, billingMemoInvoice.ListingCurrencyId.Value, billingMemoInvoice,
                                                                                                 Constants.PaxDecimalPlaces);             
              break;
            case BillingCategoryType.Cgo:
              decimalPlace = Constants.CgoDecimalPlaces;
              //Check if BMInvoice has null tolerance than populate tolerance value
              if (billingMemoInvoice.Tolerance == null)
                billingMemoInvoice.Tolerance = CompareUtil.GetTolerance(BillingCategoryType.Cgo, billingMemoInvoice.ListingCurrencyId.Value, billingMemoInvoice,
                                                                                                 Constants.CgoDecimalPlaces);
              break;
            case BillingCategoryType.Misc:
              decimalPlace = Constants.MiscDecimalPlaces;
              //Update Billing Year and Month with original Invoice value, which is used further for getting Exchange Rate
              bmInvoiceBillingYear = originalInvoice.BillingYear;
              bmInvoiceBillingMonth = originalInvoice.BillingMonth;
              //Check if BMInvoice has null tolerance than populate tolerance value
              if (billingMemoInvoice.Tolerance == null)
                billingMemoInvoice.Tolerance = CompareUtil.GetTolerance(BillingCategoryType.Misc, billingMemoInvoice.ListingCurrencyId.Value, billingMemoInvoice,
                                                                                                 Constants.MiscDecimalPlaces);
              break;
            case BillingCategoryType.Uatp:
              decimalPlace = Constants.MiscDecimalPlaces; 
              //Update Billing Year and Month with original Invoice value, which is used further for getting Exchange Rate
              bmInvoiceBillingYear = originalInvoice.BillingYear;
              bmInvoiceBillingMonth = originalInvoice.BillingMonth;
              //Check if BMInvoice has null tolerance than populate tolerance value
              if (billingMemoInvoice.Tolerance == null)
                billingMemoInvoice.Tolerance = CompareUtil.GetTolerance(BillingCategoryType.Uatp, billingMemoInvoice.ListingCurrencyId.Value, billingMemoInvoice,
                                                                                                 Constants.MiscDecimalPlaces);
              break;
          }          
                                
          //***********Main Logic*************************************************************************************************************************************
          //Case (1) Currency of Correspondence and the Currency of Listing of 6A/6B BM are the same
          //Case (2) Currency of Correspondence = USD/GBP/EUR , Currency of Listing of the BM is different from the Currency of the Correspondence
          //Case (3) Currency of Correspondence <> USD/GBP/EUR , Currency of Listing of 6A/6B BM = USD/GBP/EUR
          //Case (4) Currency of Correspondence <> USD/GBP/EUR , Currency of Listing of the BM is different from the Currency of the Correspondence, and is NOT USD/GBP/EUR
          //***********************************************************************************************************************************************************
          // CMP#624 : 2.10-Change#4 - Conditional validation of PAX/CGO 6A/6B BM amounts
          // Scenario #1
          if (corrCurrencyId == billingMemoInvoice.ListingCurrencyId)
          {
            //Case (1) : No currency conversions required
            //Removed(Perform direct match between Correspondence amount and BM amount in Currency of Listing) 
            //Logic Changed: insteed of direct match use Tolerance also while compareing the amounts.
            corrAmountToBeSettledinBMCurrency = corrAmountToBeSettled;
            isValid = ValidateAmounts(bmNetAmountBilled, corrAmountToBeSettledinBMCurrency, billingMemoInvoice.Tolerance, decimalPlace);
          }
          // CMP#624 : 2.10-Change#4 - Conditional validation of PAX/CGO 6A/6B BM amounts  
          else if (ValidateCorrespondenceAmounttobeSettledForSmiX( billingMemoInvoice, originalInvoice))
          {
            switch (corrCurrencyId)
            {
              case (int)BillingCurrency.EUR:
              case (int)BillingCurrency.GBP:
              case (int)BillingCurrency.USD:
                //Case (2) : Convert Correspondence Amount to equivalent amount in Currency of Listing of BM
                //Fetch Exchange Rate of Currency of Correspondence, for BM’s Currency of Listing, as per BM’s Billing Year/Month/Period (from Exchange Rate master)
                //This will be a multiplicative factor
                var exchangeRateForBmCurrency = GetExchangeRate(billingMemoInvoice.ListingCurrencyId.Value,
                                                                (BillingCurrency)corrCurrencyId,
                                                                bmInvoiceBillingYear,
                                                                bmInvoiceBillingMonth);

                corrAmountToBeSettledinBMCurrency = exchangeRateForBmCurrency > 0
                                                      ? corrAmountToBeSettled *
                                                        Convert.ToDecimal(exchangeRateForBmCurrency)
                                                      : corrAmountToBeSettled;
                isValid = ValidateAmounts(bmNetAmountBilled, corrAmountToBeSettledinBMCurrency, billingMemoInvoice.Tolerance, decimalPlace);
                break;
              default:
                switch (billingMemoInvoice.ListingCurrencyId)
                {
                  case (int)BillingCurrency.EUR:
                  case (int)BillingCurrency.GBP:
                  case (int)BillingCurrency.USD:
                    //Case (3) Convert Correspondence Amount to equivalent amount in Currency of Listing of BM
                    //Fetch Exchange Rate of BM’s Currency of Listing, for Currency of Correspondence, as per BM’s Billing Year/Month/Period (from Exchange Rate master)
                    //This will be a dividing factor
                    var exchangeRateForCorrCurrency = GetExchangeRate(corrCurrencyId,
                                                                      (BillingCurrency)
                                                                      billingMemoInvoice.ListingCurrencyId,
                                                                      bmInvoiceBillingYear,
                                                                      bmInvoiceBillingMonth);
                    corrAmountToBeSettledinBMCurrency = exchangeRateForCorrCurrency > 0
                                                            ? corrAmountToBeSettled /
                                                              Convert.ToDecimal(exchangeRateForCorrCurrency)
                                                            : corrAmountToBeSettled;
                    isValid = ValidateAmounts(bmNetAmountBilled, corrAmountToBeSettledinBMCurrency, billingMemoInvoice.Tolerance, decimalPlace);
                    break;
                  default:
                    //Case (4): Convert Correspondence Amount to equivalent amount in Currency of Listing of BM,
                    //using USD as a cross rate (both rates will be fetched from the Exchange Rate master), Use BM’s Billing Year/Month/Period for fetching both rates.
                    //Intermediate USD amount will have a precision of 10 decimal places
                    var exchangeRateCorrForUsdCurrency = GetExchangeRate(corrCurrencyId,
                                                                         BillingCurrency.USD,
                                                                         bmInvoiceBillingYear,
                                                                         bmInvoiceBillingMonth);
                    var exchangeRateBmForUsdCurrency = GetExchangeRate(billingMemoInvoice.ListingCurrencyId.Value,
                                                                       BillingCurrency.USD,
                                                                       bmInvoiceBillingYear,
                                                                       bmInvoiceBillingMonth);                    

                    corrAmountToBeSettledinBMCurrency = exchangeRateCorrForUsdCurrency > 0
                                                               ? ConvertUtil.Round((corrAmountToBeSettled /Convert.ToDecimal(exchangeRateCorrForUsdCurrency)),10) * Convert.ToDecimal(exchangeRateBmForUsdCurrency)
                                                               : corrAmountToBeSettled;

                    isValid = ValidateAmounts(bmNetAmountBilled, corrAmountToBeSettledinBMCurrency, billingMemoInvoice.Tolerance, decimalPlace);
                    break;
                }
                break;
            }
          }
          // If Billing Memo Net Amount Billed is 0 (When BM is created using Billing History and Correspondence screen) update value with corr Amount ToBeSettledin BM Currency
          if (bmNetAmountBilled == 0) bmNetAmountBilled = ConvertUtil.Round(corrAmountToBeSettledinBMCurrency, decimalPlace);
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Error while Validating Correspondence Amount to be Settled: ", ex);
          isValid = false;
        }
        return isValid;
      }

      public static bool ValidateAmounts(decimal amountToCompare, decimal amountToCompareWith, Tolerance tolerance, int decimalPlace)
      {
        var isValid = true;
        // Get difference value from amount to be settled and Billing memo billed value
        var differenceValue = ConvertUtil.Round(amountToCompareWith, decimalPlace) - ConvertUtil.Round(amountToCompare, decimalPlace);

        if (differenceValue != 0)
        {
          // If tolerance != null get difference value and compare
          if (tolerance != null)
          {            
            // If above difference value is greater than tolerance Rounding value validation Fail
            if (Convert.ToDouble(Math.Abs(differenceValue)) > tolerance.RoundingTolerance)
              isValid = false;
          }
          else
          {
            isValid = false;
          }
        }
        return isValid;
      }

      // CMP#624 : 2.10-Change#4 - Conditional validation of PAX/CGO 6A/6B BM amounts  
      // Scenario #2
      /// <summary>
      /// Validates the correspondence amounttobe settled for smi X.
      /// </summary>
      /// <param name="billingMemoInvoice">The billing memo invoice.</param>
      /// <param name="originalInvoice">The original invoice.</param>
      /// <returns></returns>
      private static bool ValidateCorrespondenceAmounttobeSettledForSmiX(InvoiceBase billingMemoInvoice, InvoiceBase originalInvoice)
      {
        bool validate = false;
        if (billingMemoInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
        {
          if (billingMemoInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement && originalInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement &&
              originalInvoice.CurrencyRateIndicator.Trim().ToUpper() == "F")
          {
            if (originalInvoice.BillingCurrencyId == (int) BillingCurrency.USD || originalInvoice.BillingCurrencyId == (int) BillingCurrency.GBP ||
                originalInvoice.BillingCurrencyId == (int) BillingCurrency.EUR)
            {
              validate = true;
            }
          }
        }
        else
        {
          validate = true;
        }
        return validate;
      }

      #region CMP#641
      //CMP #641: Time Limit Validation on Third Stage PAX Rejections
      /// <summary>
      /// Gets the transaction time limit for pax rm stage three.
      /// </summary>
      /// <param name="transactionType">Type of the transaction.</param>
      /// <param name="settlementMethodId">The settlement method id.</param>
      /// <param name="yourBillingPeriod">Your billing period.</param>
      /// <returns></returns>
      public TimeLimit GetTransactionTimeLimitForPaxRmStageThree(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime yourBillingPeriod)
      {
          //CMP#624 : 2.10 - Change#6 : Time Limits
          // While calculating time limit for SMI X it should behave like SMI I.
          if (settlementMethodId == (int)SMI.IchSpecialAgreement)
          {
              settlementMethodId = (int)SMI.Ich;
          }
          if (IsSmiLikeBilateral(settlementMethodId,false))
          {
              settlementMethodId = (int)SMI.Bilateral;
          }
          //settlementMethodId = GetSettlementMethodforTimeLimit(settlementMethodId);
          var timeLimits =
              TimeLimitRepository.Get(
                  rec =>
                  rec.IsActive && rec.TransactionTypeId == (int) transactionType && rec.SettlementMethodId == settlementMethodId && yourBillingPeriod >= rec.EffectiveFromPeriod &&
                  yourBillingPeriod <= rec.EffectiveToPeriod).ToList();
          if (timeLimits.Count > 1)
          {
              return null;
          }

          return timeLimits.FirstOrDefault();
         
      }

      #endregion

      public bool IsRecordExistIn_Aq_SanityCheck(string fileName)
      {
          return SystemMonitorRepo.IsRecordExistIn_Aq_SanityCheck(fileName);
      }

      /// <summary>
      /// This function is used to get currency code name based on currency code.
      /// CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1
      /// </summary>
      /// <param name="currencyCodeNum"></param>
      /// <returns></returns>
      public string GetCurrencyCodeName(Int32 currencyCodeNum)
      {
          string currencyCodeName = CurrencyRepository.Get(curr => curr.Id == currencyCodeNum).Select(curr => curr.Name).FirstOrDefault();

          return currencyCodeName;
      }
  }
}


