using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using log4net;

namespace Iata.IS.Business
{
  /// <summary>
  /// Validation cache will be used to store all the database values required for validating IDEC file.
  /// </summary>
  public class ValidationCache
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Private instance to make this class singleton.
    /// </summary>
    private static ValidationCache _validationCache;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ValidationCache Instance
    {
      get { return _validationCache ?? (_validationCache = new ValidationCache()); }
    }

    /// <summary>
    /// Gets or sets the current billing period.
    /// </summary>
    /// <value>The current billing period.</value>
    public BillingPeriod CurrentBillingPeriod { get; private set; }

    /// <summary>
    /// Gets Pax vat identifiers in the form of key as Vat Identifier and Value is Vat Identifier Id.
    /// </summary>
    public IDictionary<string, int> PaxVatIdentifiers { get; private set; }

    /// <summary>
    /// Gets CGO vat identifiers in the form of key as Vat Identifier and Value is Vat Identifier Id.
    /// </summary>
    public IDictionary<string, int> CgoVatIdentifiers { get; private set; }

    /// <summary>
    /// Gets Valid Tax Codes in the form of key as Tax Code and value isActive or not.
    /// </summary>
    public IDictionary<string, bool> ValidTaxCodes { get; private set; }

    /// <summary>
    /// Gets Valid Currency Codes in the form of key as Currency Code and value is Id.
    /// </summary>
    public IDictionary<string, int> ValidCurrencyCodes { get; private set; }

    /// <summary>
    /// Gets the valid source codes from DB.
    /// </summary>
    /// <value>The valid source codes.</value>
    public IList<SourceCode> ValidSourceCodes { get; private set; }

    /// <summary>
    /// Gets the valid reason codes from DB.
    /// </summary>
    /// <value>The valid reason codes.</value>
    public IList<ReasonCode> ValidReasonCodes { get; private set; }

    /// <summary>
    /// Stores list of FieldMetaData objects from DB.
    /// </summary>
    public List<FieldMetaData> FieldMetaDataList { get; private set; }

    /// <summary>
    /// Gets Valid UOM Codes in the form of key as UOM Code and value isActive or not.
    /// </summary>
    public IDictionary<string, bool> ValidUomCodes { get; private set; }

    /// <summary>
    /// Stores the list of Charge Codes
    /// </summary>
    public List<ChargeCode> ChargeCodeList { get; private set; }

    /// <summary>
    /// Stores the list of Charge Code Types
    /// </summary>
    public List<ChargeCodeType> ChargeCodeTypeList { get; private set; }

    /// <summary>
    /// Stores the list of ChargeCategory
    /// </summary>
    public List<ChargeCategory> ChargeCategoryList { get; private set; }

    public List<Tolerance> ToleranceList { get; private set; }
    /// <summary>
    /// Stores the list of RM reason code differences.
    /// </summary>
    public List<RMReasonAcceptableDiff> ValidRmReasonAcceptableDiffs { get; private set; }

    /// <summary>
    /// Gets Valid Country Codes in the form of key as Country Code and value is IsActive
    /// </summary>
    public IDictionary<string, bool> ValidCountryCodes { get; private set; }

    /// <summary>
    /// Gets the valid Time limits from DB.
    /// </summary>
    public List<TimeLimit> ValidTimeLimits { get; private set; }

    /// <summary>
    /// Gets the valid Lead Periods from DB.
    /// </summary>
    public List<LeadPeriod> ValidLeadPeriods { get; private set; }

    /// <summary>
    /// Stores the Field Charge Code Mapping
    /// </summary>
    public List<FieldChargeCodeMapping> ValidFieldChargeCodeMapping { get; private set; }

    /// <summary>
    /// Stored the Max amounts.
    /// </summary>
    public List<MaxAcceptableAmount> ValidMaxAcceptableAmounts { get; private set; }

    /// <summary>
    /// Stored the Min amounts.
    /// </summary>
    public List<MinAcceptableAmount> ValidMinAcceptableAmounts { get; private set; }

    /// <summary>
    /// Stores Exception code list along with respective ids
    /// </summary>
    public List<ExceptionCode> ExceptionCodeList { get; private set; }

    /// <summary>
    /// Get server dynamic field server validators.
    /// </summary>
    public IList<ServerValidator> DynamicFieldServerValidators { get; private set; }

    /// <summary>
    /// Gets Valid City Airport Codes in the form of key as city airport code and value is IsActive.
    /// </summary>
    public IDictionary<string, bool> ValidCityAirportCodes { get; private set; }

    public List<OnBehalfInvoiceSetup> OnBehalfOfMemberList { get; private set; }

    public string CSVRootBasePath { get; private set; }

    public string FtpRootBasePath { get; private set; }

    public string SISOpsEmailId { get; private set; }

    public bool IsSystemMultilingual { get; private set; }

    public string AtpCoApplicationMode { get; private set; }

    public string AtpCoUploadPath { get; private set; }

    /// <summary>
    /// Get valid unloccode as key and valus as IsActive
    /// </summary>
    public IDictionary<string, bool> ValidUnlocCodes { get; private set; }
    
    public bool IgnoreValidationOnMigrationPeriod { get; private set; }

    public bool PaxRmBilledAllowedAmount { get; private set; }

    public bool CgoRmBilledAllowedAmount { get; private set; }

    /// <summary>
    /// Gets the Cargo OC codes.
    /// </summary>
    public IDictionary<string, string> OtherChargeCodes { get; private set; }

    public long AbillThreshouldValue { get; private set; }

    /// <summary>
    /// Gets Languages and value is IsActive.
    /// </summary>
    public IDictionary<string, bool> Languages { get; private set; }

    /// <summary>
    /// This method will read and store all the required values from the database.
    /// </summary>
    public void Initialize()
    {
      var sw = Stopwatch.StartNew();

      // Initialize IOC container.
      Ioc.Initialize();

      var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

      // Get Currencies from the database.
      var currencyList = referenceManager.GetCurrencyList();
      ValidCurrencyCodes = new ConcurrentDictionary<string, int>();
      foreach (var currencyCode in currencyList)
      {
        if (currencyCode.IsActive)
        {
          ValidCurrencyCodes.Add(currencyCode.Code, currencyCode.Id);
        }
      }

      var maxAcceptableAmountRepository = Ioc.Resolve<IMaxAcceptableAmountRepository>();
      ValidMaxAcceptableAmounts = maxAcceptableAmountRepository.GetAll().ToList();

      var minAcceptableAmountRepository = Ioc.Resolve<IMinAcceptableAmountRepository>();
      ValidMinAcceptableAmounts = minAcceptableAmountRepository.Get( i => i.IsActive).ToList();

      // Get Country code from database
      var countryManager = Ioc.Resolve<ICountryManager>(typeof(ICountryManager));
      var countryList = countryManager.GetAllCountryList();

      ValidCountryCodes = new ConcurrentDictionary<string, bool>();
      foreach (var country in countryList)
      {
        if (!ValidCountryCodes.ContainsKey(country.Id) && country.IsActive)
          ValidCountryCodes.Add(country.Id, country.IsActive);
      }

      // Get city airport codes from database.
      var cityAirportList = referenceManager.GetCityAirportList();

      ValidCityAirportCodes = new ConcurrentDictionary<string, bool>();
      foreach (var cityAirport in cityAirportList)
      {
        ValidCityAirportCodes.Add(cityAirport.Id, true);
      }

      // Get PAX vat identifiers from the database.
      var paxVatIds = referenceManager.GetVatIdentifierList(BillingCategoryType.Pax);
      PaxVatIdentifiers = new ConcurrentDictionary<string, int>();
      foreach (var vatIdentifier in paxVatIds)
      {
        if (!PaxVatIdentifiers.ContainsKey(vatIdentifier.Identifier))
        {
          PaxVatIdentifiers.Add(vatIdentifier.Identifier, vatIdentifier.Id);
        }
      }

      // Get PAX vat identifiers from the database.
      var cgoVatIds = referenceManager.GetCgoVatIdentifierList(null);
      CgoVatIdentifiers = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
      foreach (var vatIdentifier in cgoVatIds)
      {
        if (!CgoVatIdentifiers.ContainsKey(vatIdentifier.Identifier))
        {
          CgoVatIdentifiers.Add(vatIdentifier.Identifier, vatIdentifier.Id);
        }
      }

      // Get tax codes from the database.
      var taxCodeList = referenceManager.GetTaxCodeList();
      ValidTaxCodes = new ConcurrentDictionary<string, bool>();
      foreach (var taxCode in taxCodeList)
      {
        ValidTaxCodes.Add(taxCode.Id, taxCode.IsActive);
      }

      // Get source codes from the database.
      ValidSourceCodes = referenceManager.GetSourceCodeList();

      // Get reason codes from the database.
      ValidReasonCodes = referenceManager.GetReasonCodeList();


      var rmReasonAcceptableDifferenceRepository = Ioc.Resolve<IRepository<RMReasonAcceptableDiff>>(typeof(IRepository<RMReasonAcceptableDiff>));

      ValidRmReasonAcceptableDiffs = rmReasonAcceptableDifferenceRepository.GetAll().ToList();

      //Get Time limits from the database
      var timeLimitRepository = Ioc.Resolve<IRepository<TimeLimit>>();
      ValidTimeLimits = timeLimitRepository.GetAll().ToList();

      var leadPeriodRepository = Ioc.Resolve<IRepository<LeadPeriod>>();
      ValidLeadPeriods = leadPeriodRepository.GetAll().ToList();

      var miscUatpInvoiceManager = Ioc.Resolve<IMiscInvoiceManager>(typeof(IMiscInvoiceManager));
      FieldMetaDataList = miscUatpInvoiceManager.GetFieldMetadata();

      // Get UOM codes from the database.
      var uomCodeManager = Ioc.Resolve<IUomCodeManager>(typeof(IUomCodeManager));
      var uomCodeList = uomCodeManager.GetAllUomCodeList();

      ValidUomCodes = new ConcurrentDictionary<string, bool>();
      foreach (var uomCode in uomCodeList)
      {
        if (!ValidUomCodes.ContainsKey(uomCode.Id))
          ValidUomCodes.Add(uomCode.Id, uomCode.IsActive);
      }

      // Get Charge Codes from the database
      var chargeCodeManager = Ioc.Resolve<IChargeCodeManager>(typeof(IChargeCodeManager));
      ChargeCodeList = chargeCodeManager.GetAllChargeCodeList();

      // Get Charge Code Types from the database
      var chargeTypeCodeManager = Ioc.Resolve<IChargeCodeTypeManager>(typeof(IChargeCodeTypeManager));
      ChargeCodeTypeList = chargeTypeCodeManager.GetAllChargeCodeTypeList();

      // Get Charge Categories from the database
      var chargeCategoryManager = Ioc.Resolve<IChargeCategoryManager>(typeof(IChargeCategoryManager));
      ChargeCategoryList = chargeCategoryManager.GetAllChargeCategoryList();

      //Store the FieldChargeCodeMapping from the db
      var fieldChargeCodeMappingRepository = Ioc.Resolve<IRepository<FieldChargeCodeMapping>>();
      ValidFieldChargeCodeMapping = fieldChargeCodeMappingRepository.GetAll().ToList();

      // Get dynamic field server validators from the DB.
      var serverValidatorRepository = Ioc.Resolve<IRepository<ServerValidator>>(typeof(IRepository<ServerValidator>));
      DynamicFieldServerValidators = serverValidatorRepository.GetAll().ToList();

      var toleranceRepository = Ioc.Resolve<IRepository<Tolerance>>();
      ToleranceList = toleranceRepository.GetAll().ToList();

      var validationonBehalfManager = Ioc.Resolve<IOnBehalfInvoiceSetupManager>(typeof(IOnBehalfInvoiceSetupManager));
      OnBehalfOfMemberList = validationonBehalfManager.GetAllOnBehalfOfMemberList();

      // Get system parameters required for parsing and validation.
      CSVRootBasePath = FileIo.GetForlderPath(SFRFolderPath.CSVOutputPath);

      FtpRootBasePath = SystemParameters.Instance.General.FtpRootBasePath;

      SISOpsEmailId = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;

      IsSystemMultilingual = SystemParameters.Instance.General.IsMultilingualAllowed;

      AbillThreshouldValue = SystemParameters.Instance.AutoBilling.InvoiceRangeThresholdValue;

      AtpCoApplicationMode = SystemParameters.Instance.Atpco.ApplicationMode;

      AtpCoUploadPath = FileIo.GetATPCOFTPDownloadFolderPath();

      IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod;
      PaxRmBilledAllowedAmount = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
      CgoRmBilledAllowedAmount = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.CGORMBilledAllowedAmounts);

      // Get all Unloc codes and cached in the dictionary.
      var unlocCodeManager = Ioc.Resolve<IUnlocCodeManager>(typeof(IUnlocCodeManager));
      var unlocCodeList = unlocCodeManager.GetAllUnlocCodeList();
      ValidUnlocCodes = new ConcurrentDictionary<string, bool>();
      foreach (var unlocCode in unlocCodeList.Where(unlocCode => !ValidUnlocCodes.ContainsKey(unlocCode.Id)))
      {
        ValidUnlocCodes.Add(unlocCode.Id, true);
      }

      var exceptionCodeRepository = Ioc.Resolve<IExceptionCodeRepository>();
      ExceptionCodeList = exceptionCodeRepository.GetAll().ToList();

      // Get Cargo OC Codes from the database.
      OtherChargeCodes = new Dictionary<string, string>();
      OtherChargeCodes = referenceManager.GetMiscCodeString(MiscGroups.CgoOtherChargeCode);

      //Store master languages
      Languages = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
      var langaugesRepository = Ioc.Resolve<IRepository<Language>>();
      var languagesList = langaugesRepository.Get(i => i.IsReqForPdf);
      foreach (var language in languagesList)
      {
        Languages.Add(language.Language_Code,true);
      }

      sw.Stop();
      Logger.InfoFormat("Validation cache initialization : Time Required: [{0}]", sw.Elapsed);
    }
  }
}
