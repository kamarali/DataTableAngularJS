using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.File;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.Reports.MemberLocations;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MemberProfileDataFile;
using Iata.IS.Model.Pax.Common;
using log4net;

namespace Iata.IS.Business.MemberProfileDataFile.Impl
{
  public class MemberProfileDataFile : IMemberProfileDataFile
  {

    #region Private  Member

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string LocationTableName = "MEM_LOCATION";
    private readonly List<string> _nlist = new List<string>
                                                {
                                                    "MEMBER_LEGAL_NAME",
                                                    "IS_ACTIVE",
                                                    "REGISTRATION_ID",
                                                    "TAX_VAT_REGISTRATION_NUMBER",
                                                    "ADD_TAX_VAT_REGISTRATION_NUM",
                                                    "COUNTRY_CODE",
                                                    "ADDRESS_LINE1",
                                                    "ADDRESS_LINE2",
                                                    "ADDRESS_LINE3",
                                                    "CITY_NAME",
                                                    "SUB_DIVISION_NAME",
                                                    "POSTAL_CODE"
                                                };
    #endregion

    #region Properties

    private IFutureUpdatesRepository FutureUpdateRepository { get; set; }

    private IFutureUpdatesServiceRepository FutureUpdatesServiceRepository { get; set; }

    private ILocationRepository LocationRepository { get; set; }

    private IMemberLocationData MemberLocationParam { get; set; }

    private IQueryAndDownloadDetailsManager QueryAndDownloadDetailsManager { get; set; }

    private IReferenceManager ReferenceManager { get; set; }

    private IMemberManager MemberManager { get; set; }

    public IRepository<EBillingConfiguration> EBillingRepository { get; set; }

    public IRepository<TechnicalConfiguration> TechnicalRepository { get; set; }

    public IRepository<IsFileLogExt1Model> IsFileLogExt1ModelRepository { get; set; }

    private IContactRepository ContactsRepository { get; set; }

    private ICalendarManager CalendarManager { get; set; }

    #endregion

    #region Internal Properties

    private static int MaxAccountIdsForIinetSingleUpload
    {
      get
      {
        var accountIdscount = ConfigurationManager.AppSettings.Get("MaxAccountIdsForIinetSingleUpload");
        int accountIdscountV;
        if (!string.IsNullOrEmpty(accountIdscount) && Int32.TryParse(accountIdscount, out accountIdscountV)) return accountIdscountV;

        // Default Value
        return 100;
      }
    }

    #endregion Internal Properties

    #region Constructor
    public MemberProfileDataFile(IFutureUpdatesRepository futureUpdateRepository, ILocationRepository locationRepository, IFutureUpdatesServiceRepository futureUpdatesServiceRepository, IMemberLocationData memberLocationData, IQueryAndDownloadDetailsManager queryAndDownloadDetailsManager, IReferenceManager referenceManager, IMemberManager memberManager, IContactRepository contactsRepository, ICalendarManager calendarManager)
    {
      FutureUpdateRepository = futureUpdateRepository;
      LocationRepository = locationRepository;
      FutureUpdatesServiceRepository = futureUpdatesServiceRepository;
      MemberLocationParam = memberLocationData;
      QueryAndDownloadDetailsManager = queryAndDownloadDetailsManager;
      ReferenceManager = referenceManager;
      MemberManager = memberManager;
      ContactsRepository = contactsRepository;
      CalendarManager = calendarManager;
    }

    #endregion

    #region Method Implimentation

    public bool CreateMemberProfileDataFile(int billingPeriod, int billingMonth, int billingYear)
    {
      try
      {
        var changeEffectivePeriodOn = new DateTime(billingYear, billingMonth, billingPeriod);
        var changeEffectivePeriodOnString = string.Format("{0}{1}{2}", billingYear,
                                                          billingMonth.ToString().PadLeft(2, '0'),
                                                          billingPeriod.ToString().PadLeft(2, '0'));

        //*****Check here if all the future update changes for the current period are applied or not*****
        _logger.Info("MemberProfileDataFile : Check for future update changes are applied or not");

        if (!IsFutureUpdateChangesApplied(changeEffectivePeriodOn))
        {
          _logger.InfoFormat(
            "MemberProfileDataFile : Future update changes are not applied for billing Period (PP/mm/yyyy) - {0}/{1}/{2}",
            billingPeriod, billingMonth, billingYear);
          return false;
        }

        //*******************File Path*************************************************************
        var folderPath = FileIo.GetForlderPath(SFRFolderPath.MemberProfileDataFile);
        if (!Directory.Exists(folderPath))
          Directory.CreateDirectory(folderPath); //Create MemberProfileDataFile folder on SFRRoot folder

        var fileNameRefdataChg = Path.Combine(folderPath, ("REFDATA-CHG-" + changeEffectivePeriodOnString + ".CSV"));
        var fileNameRefdataComp = Path.Combine(folderPath, ("REFDATA-COMP-" + changeEffectivePeriodOnString + ".CSV"));
        var fileNameRefdataCtcdata = Path.Combine(folderPath, ("CTCDATA-COMP-" + changeEffectivePeriodOnString + ".CSV"));
        //*****************************************************************************************

        //*****Create CSV/Zip files******
        _logger.Info("MemberProfileDataFile: Future update changes are applied.File Creation started....");

        CreateChangeInfoforReferenceData(changeEffectivePeriodOn, fileNameRefdataChg);
        CreateCompleteReferenceData(changeEffectivePeriodOn, fileNameRefdataComp);
        CreateCompleteContactsData(changeEffectivePeriodOn, fileNameRefdataCtcdata);

        _logger.Info("MemberProfileData File Creation executed successfuly.");
        return true;
      }
      catch (Exception ex)
      {
        _logger.Error("Exception occurred while Createing MemberProfileDataFiles.", ex);
        return false;
      }
    }

    #endregion

    #region Private Methods


    /// <summary>
    /// Check the pending future update for a specific period
    /// </summary>
    /// <param name="changeEffectivePeriodOn"></param>
    /// <returns></returns>
    private bool IsFutureUpdateChangesApplied(DateTime changeEffectivePeriodOn)
    {

      var futureUpdateData = FutureUpdateRepository.Get(
          futureUpdate => futureUpdate.IsChangeApplied == false && futureUpdate.ChangeEffectivePeriod == changeEffectivePeriodOn).ToList();
      return (futureUpdateData.Count == 0);
    }

    /// <summary>
    /// Create Change Information for Reference Data csv/zip file
    /// </summary>
    /// <param name="changeEffectivePeriodOn"></param>
    /// <param name="fileName"></param>
    private void CreateChangeInfoforReferenceData(DateTime changeEffectivePeriodOn, string fileName)
    {
      try
      {
        _logger.Info("Getting data for Change Information for Reference Data  and File creation started........");
        var serialNoCount = 1;
        var changeInfoReferenceDataModel = new List<ChangeInfoReferenceDataModel>();

        //Get Data of members for which status is change Pending to any other
        var changeInfoReferenceDataForStatusChange = GetCirDataForPendingToAnyStatusChange(changeEffectivePeriodOn);
        //--------------------------------------------------------------------------------------------

        var billingPeriod = CalendarManager.GetLastClosedBillingPeriod();

        var currentBillingPeriod = CalendarManager.GetCurrentBillingPeriod(ClearingHouse.Ich);

        //Get future update data of Location from future update table
        var futureUpdateData = FutureUpdateRepository.Get(
          futureUpdate =>
          (futureUpdate.Member.IsMembershipStatusId != (int)MemberStatus.Pending &&
           futureUpdate.Member.IsMembershipSubStatusId != 6) && // CMP603:6 = MemberSubStatus.Terminated  
           futureUpdate.Member.MemberCodeNumeric.Length == 3 && // CMP#683: Changes in Report
           futureUpdate.TableName == LocationTableName &&
          ((futureUpdate.ChangeEffectiveOn >= billingPeriod.StartDate && futureUpdate.ChangeEffectiveOn <= billingPeriod.EndDate && futureUpdate.IsChangeApplied)
         || (futureUpdate.ChangeEffectivePeriod == new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period)))).ToList();

        var futureUpdateDataN =
          futureUpdateData.Where(fUpd => GetAction(fUpd.OldVAlue, fUpd.NewVAlue) == "N");
        var futureUpdateDataC =
          futureUpdateData.Where(fUpd => GetAction(fUpd.OldVAlue, fUpd.NewVAlue) == "C");


        if (changeInfoReferenceDataForStatusChange != null)
          changeInfoReferenceDataModel.AddRange(changeInfoReferenceDataForStatusChange);

        var changeInfoReferenceDataModelN = GetChangeInfoReferenceDataModel(futureUpdateDataN);
        if (changeInfoReferenceDataModelN != null)
        {
          var changeInfoReferenceDataModelNSorted = (from cirdmn in changeInfoReferenceDataModelN
                                                     orderby cirdmn.ParticipantCode ascending
                                                     select cirdmn).ToList();
          changeInfoReferenceDataModel.AddRange(changeInfoReferenceDataModelNSorted);
        }

        var changeInfoReferenceDataModelC = GetChangeInfoReferenceDataModel(futureUpdateDataC);
        if (changeInfoReferenceDataModelC != null)
        {
          var changeInfoReferenceDataModelCSorted = (from cirdmc in changeInfoReferenceDataModelC
                                                     orderby cirdmc.ParticipantCode ascending
                                                     select cirdmc).ToList();
          changeInfoReferenceDataModel.AddRange(changeInfoReferenceDataModelCSorted);
        }


        //update serial no 
        foreach (var cIrDmodel in changeInfoReferenceDataModel)
        {
          cIrDmodel.SerialNo = serialNoCount;
          cIrDmodel.LocationId = ReplaceDoubleQuoteString(cIrDmodel.LocationId);
          cIrDmodel.NewValue = ReplaceDoubleQuoteString(cIrDmodel.NewValue);
          cIrDmodel.OldValue = ReplaceDoubleQuoteString(cIrDmodel.OldValue);
          serialNoCount++;
        }

        // create csv file
        if (changeInfoReferenceDataModel.Count <= 0) return;

        // Rename the Field as per FRS

        changeInfoReferenceDataModel = RenameFieldNameForChangeReferenceData(changeInfoReferenceDataModel);

        _logger.Info("Change Information for Reference Data csv file Creation Started.");
        var csvGenerator = new CsvGenerator();
        csvGenerator.GenerateCSV(changeInfoReferenceDataModel, fileName);
        fileName = FileIo.ZipOutputFile(fileName);
        _logger.Info("Change Information for Reference Data file created successfuly.");
        //*********Insert in IS_FILE_LOG table************
        AddInFileLogDb(changeEffectivePeriodOn.Year, changeEffectivePeriodOn.Month, changeEffectivePeriodOn.Day, fileName, FileFormatType.ChangeInfoReferenceDataUpdateCsv);
      }
      catch (Exception ex)
      {
        _logger.Error("Exception occurred while createing Change Information for Reference Data File.", ex);
      }
    }





    /// <summary>
    /// Create Complete Reference Data csv/zip file
    /// </summary>
    /// <param name="changeEffectivePeriodOn"></param>
    /// <param name="fileName"></param>
    private void CreateCompleteReferenceData(DateTime changeEffectivePeriodOn, string fileName)
    {
      try
      {
        _logger.Info("Getting Complete Reference Data and file Creation Started.");
        //Get Complete Reference Data
        var completeReferenceDataM = MemberLocationParam.GetMemberLocationDetails(0, "");
        if (completeReferenceDataM.Count <= 0) return;

        // CMP#683: Changes in Report
        var completeReferenceDataModel = completeReferenceDataM.Select(crd => new CompleteReferenceDataModel()
        {
          SerialNo = crd.SerialNo,
          ParticipantID = crd.ParticipantID,
          LocationId = crd.LocationId,
          Active = crd.Active,
          CompanyLegalName = crd.CompanyLegalName,
          TaxVatRegistrationID = crd.TaxVatRegistrationID,
          AdditionalTaxVatRegistrationId = crd.AdditionalTaxVatRegistrationId,
          CompanyRegistrationID = crd.CompanyRegistrationID,
          AddressLine1 = crd.AddressLine1,
          AddressLine2 = crd.AddressLine2,
          AddressLine3 = crd.AddressLine3,
          CityName = crd.CityName,
          SubdivisionCode = crd.SubdivisionCode,
          SubdivisionName = crd.SubdivisionName,
          CountryCode = crd.CountryCode,
          CountryName = crd.CountryName,
          PostalCode = crd.PostalCode
        }).Where(crd => crd.ParticipantID.Length == 3).ToList();
  
        //Sort Location ID based on FRS
        completeReferenceDataModel = SortCompleteReferenceDataModel(completeReferenceDataModel);

        //update serial no 
        var serialNoCount = 1;
        foreach (var cIrDmodel in completeReferenceDataModel)
        {
          cIrDmodel.SerialNo = serialNoCount;
          cIrDmodel.CompanyLegalName = ReplaceDoubleQuoteString(cIrDmodel.CompanyLegalName);
          cIrDmodel.TaxVatRegistrationID = ReplaceDoubleQuoteString(cIrDmodel.TaxVatRegistrationID);
          cIrDmodel.AdditionalTaxVatRegistrationId = ReplaceDoubleQuoteString(cIrDmodel.AdditionalTaxVatRegistrationId);
          cIrDmodel.CompanyRegistrationID = ReplaceDoubleQuoteString(cIrDmodel.CompanyRegistrationID);
          cIrDmodel.AddressLine1 = ReplaceDoubleQuoteString(cIrDmodel.AddressLine1);
          cIrDmodel.AddressLine2 = ReplaceDoubleQuoteString(cIrDmodel.AddressLine2);
          cIrDmodel.AddressLine3 = ReplaceDoubleQuoteString(cIrDmodel.AddressLine3);
          cIrDmodel.CityName = ReplaceDoubleQuoteString(cIrDmodel.CityName);
          cIrDmodel.CountryName = ReplaceDoubleQuoteString(cIrDmodel.CountryName);
          serialNoCount++;
        }

        // create csv file
        _logger.Info("Complete Reference Data csv file Creation Started...");
        var csvGenerator = new CsvGenerator();
        csvGenerator.GenerateCSV(completeReferenceDataModel, fileName);
        fileName = FileIo.ZipOutputFile(fileName);
        //*********Insert in IS_FILE_LOG table
        AddInFileLogDb(changeEffectivePeriodOn.Year, changeEffectivePeriodOn.Month, changeEffectivePeriodOn.Day, fileName, FileFormatType.CompleteReferenceDataCsv);
        _logger.Info("Complete Reference Data file created successfuly.");
      }
      catch (Exception ex)
      {
        _logger.Error("Exception occurred while createing Complete Reference Data File.", ex);
      }
    }

    private static List<CompleteReferenceDataModel> SortCompleteReferenceDataModel(List<CompleteReferenceDataModel> completeReferenceDataModel)
    {
      var returncompleteReferenceDataModel = new List<CompleteReferenceDataModel>();
      var distinctMemberName = completeReferenceDataModel
        .GroupBy(c => c.ParticipantID)
        .Select(g => g.First());

      foreach (var referenceDataModel in distinctMemberName)
      {
        CompleteReferenceDataModel model = referenceDataModel;
        var completeReferenceDataModelMain =
          completeReferenceDataModel.Where(
            fUpd => fUpd.LocationId.ToLower().Equals("main") && fUpd.ParticipantID == model.ParticipantID);
        var completeReferenceDataModelUatp =
          completeReferenceDataModel.Where(
            fUpd => fUpd.LocationId.ToLower().Equals("uatp") && fUpd.ParticipantID == model.ParticipantID);
        var completeReferenceDataModelRest =
          completeReferenceDataModel.Where(
            fUpd =>
            fUpd.LocationId.ToLower() != "main" && fUpd.LocationId.ToLower() != "uatp" &&
            fUpd.ParticipantID == model.ParticipantID);

        completeReferenceDataModelRest = (completeReferenceDataModelRest.Select(cirdmr => new
        {
          cirdmr.SerialNo,
          cirdmr.ParticipantID,
          LocationId =
        int.Parse(cirdmr.LocationId),
          cirdmr.Active,
          cirdmr.CompanyLegalName,
          cirdmr.
        TaxVatRegistrationID,
          cirdmr.
        AdditionalTaxVatRegistrationId,
          cirdmr.
        CompanyRegistrationID,
          cirdmr.AddressLine1,
          cirdmr.AddressLine2,
          cirdmr.AddressLine3,
          cirdmr.CityName,
          cirdmr.SubdivisionCode,
          cirdmr.SubdivisionName,
          cirdmr.CountryCode,
          cirdmr.CountryName,
          cirdmr.PostalCode
        })).ToList().OrderBy(
                                                                                              cirdmr =>
                                                                                              Convert.ToUInt32(cirdmr.LocationId)).Select(
                                                                                                rr =>
                                                                                                new CompleteReferenceDataModel
                                                                                                  ()
                                                                                                {
                                                                                                  SerialNo =
                                                                                                    rr.SerialNo,
                                                                                                  ParticipantID =
                                                                                                    rr.ParticipantID,
                                                                                                  LocationId =
                                                                                                    rr.LocationId.
                                                                                                    ToString(),
                                                                                                  Active = rr.Active,
                                                                                                  CompanyLegalName =
                                                                                                    rr.
                                                                                                    CompanyLegalName,
                                                                                                  TaxVatRegistrationID
                                                                                                    =
                                                                                                    rr.
                                                                                                    TaxVatRegistrationID,
                                                                                                  AdditionalTaxVatRegistrationId
                                                                                                    =
                                                                                                    rr.
                                                                                                    AdditionalTaxVatRegistrationId,
                                                                                                  CompanyRegistrationID
                                                                                                    =
                                                                                                    rr.
                                                                                                    CompanyRegistrationID,
                                                                                                  AddressLine1 =
                                                                                                    rr.AddressLine1,
                                                                                                  AddressLine2 =
                                                                                                    rr.AddressLine2,
                                                                                                  AddressLine3 =
                                                                                                    rr.AddressLine3,
                                                                                                  CityName =
                                                                                                    rr.CityName,
                                                                                                  SubdivisionCode =
                                                                                                    rr.SubdivisionCode,
                                                                                                  SubdivisionName =
                                                                                                    rr.SubdivisionName,
                                                                                                  CountryCode =
                                                                                                    rr.CountryCode,
                                                                                                  CountryName =
                                                                                                    rr.CountryName,
                                                                                                  PostalCode =
                                                                                                    rr.PostalCode
                                                                                                }).ToList();


        returncompleteReferenceDataModel.AddRange(completeReferenceDataModelMain);
        returncompleteReferenceDataModel.AddRange(completeReferenceDataModelUatp);
        returncompleteReferenceDataModel.AddRange(completeReferenceDataModelRest);


      }

      return returncompleteReferenceDataModel;
    }

    /// <summary>
    /// Create Complete Contacts Data csv/zip files
    /// </summary>
    /// <param name="changeEffectivePeriodOn"></param>
    /// <param name="fileName"></param>
    private void CreateCompleteContactsData(DateTime changeEffectivePeriodOn, string fileName)
    {
      try
      {
        _logger.Info("Getting Complete Contacts Data and file Creation Started.");

        var searchCriteria = new QueryAndDownloadSearchCriteria
        {
          MemberId = "",
          MetaIdList =
            "M1,M2,M3,M4,C12,C13,C15,C16,C17,C18,C19,C20,C24,C25,C26,C27,C23,C29,C30,C31,C32,C33,C34,C35,M170,M100,T1,T2,T3,T4,T7,T10,T13,T16,T17,T18,T22,T23,T24,T25,T26,T27,T28,T29,T30,T31,T32,T33,T34,T35,T36,T37,T38,T39,T40,T41,T42,T43,T44,T45,T46,T47,T48,T49,T50,T51,T52,T53,T54,T55,T56,T57,T58,T59,T60,T61,T62,T63,T64,T65,T66,T67,T68,T69,T71,T72,T73,T74,T75,T77,T78,T79,T80,T81,T82,T83,T84,T85,T86,T87,T88,T89,C11,T98,T99,T100,T101,T102,T103,T93,T95,T96,T97,T5,T91,T92,T94",
          TypeMetaIdList = null,
          UserCategoryId = "1",
          ISOwnMember = false,
          ReportType = "2", // Exclude MemberShip Status = Terminated
          CountryId = "",
          ContactId = null,
          EmailId = null,
          ISAch = false,
          ISIch = false,
          ISIata = false,
          ISNonCh = false,
          ISDual = false,
          ContactTypeIdList =
            "1,2,3,4,7,10,13,16,17,18,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,71,72,73,74,75,77,78,79,80,81,82,83,84,85,86,87,88,89,98,99,100,101,102,103,93,95,96,97,5,91,92,94",
          GroupIdList = "",
          SubGroupIdList = "",
          SortIds = "",
          SortOrder = ""
        };

        int totalRecordCount;
        var resultSet = QueryAndDownloadDetailsManager.SearchMemberContactDetails(searchCriteria, false,
                                                                                   out totalRecordCount, true);
        var result = resultSet.Tables[0];
        if (result.Columns.Contains("MEMBER_ID")) result.Columns.Remove("MEMBER_ID");

        //CMP#683: Changes in Report
        if (result.Columns.Contains("M1"))
        {
            for (var i = result.Rows.Count - 1; i >= 0; i--)
            {
                var dataRow = result.Rows[i];

                if (dataRow["M1"].ToString().Length != 3)
                {
                    result.Rows.Remove(dataRow);
                }
            }
        }

        _logger.Info("Complete Contacts Data csv file Creation Started...");

        var tempFile = QueryAndDownloadDetailsManager.ConvertDataTableToCsv(result, Path.GetFileNameWithoutExtension(fileName), ",");
        if (tempFile != null)
        {
          tempFile = FileIo.ZipOutputFile(tempFile);
          var sanfileLocation = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileName(tempFile));
          File.Copy(tempFile, sanfileLocation, true);
          File.Delete(tempFile);
          //*********Insert in IS_FILE_LOG table
          AddInFileLogDb(changeEffectivePeriodOn.Year, changeEffectivePeriodOn.Month, changeEffectivePeriodOn.Day, sanfileLocation, FileFormatType.CompleteContactsDataCsv);
          _logger.Info("Complete Contacts Data file created successfuly.");
        }

      }
      catch (Exception ex)
      {
        _logger.Error("Exception occurred while createing Complete Contacts Data File.", ex);
      }
    }

    private IEnumerable<ChangeInfoReferenceDataModel> GetCirDataForPendingToAnyStatusChange(DateTime changeEffectivePeriodOn)
    {
      var changeInfoReferenceDataForStatusChange = new List<ChangeInfoReferenceDataModel>();

      var billingPeriod = CalendarManager.GetBillingPeriod(ClearingHouse.Ich, changeEffectivePeriodOn.Year,
                                                           changeEffectivePeriodOn.Month, changeEffectivePeriodOn.Day);

      var currentBillingPeriod = CalendarManager.GetCurrentBillingPeriod(ClearingHouse.Ich);


      var pendingStatusChangeFuRec = FutureUpdateRepository.Get(
        futureUpdate =>
        ((futureUpdate.ElementName == "IS_MEMBERSHIP_STATUS") && (futureUpdate.TableName == "MEMBER_DETAILS") &&
         (futureUpdate.OldVAlue == "5") && (futureUpdate.NewVAlue != "5")
         && ((futureUpdate.ChangeEffectiveOn >= billingPeriod.StartDate && futureUpdate.ChangeEffectiveOn <= billingPeriod.EndDate && futureUpdate.IsChangeApplied)
         || (futureUpdate.ChangeEffectivePeriod == new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period) && futureUpdate.OldVAlue == "5")))).ToList();


      if (pendingStatusChangeFuRec.Count == 0) return null;

      var pendingStatusChangeFuRecord = (from fur in pendingStatusChangeFuRec
                                         where fur.Member.IsMembershipStatusId != (int)MemberStatus.Pending
                                         group fur by fur.MemberId
                                           into grp
                                           select grp.OrderByDescending(dd => dd.ModifiedOn).FirstOrDefault()).ToList();


      foreach (var location in
        pendingStatusChangeFuRecord.Select(fuu => LocationRepository.Get(lr => lr.MemberId.Equals(fuu.MemberId))).
          SelectMany(gg => gg))
      {
        if (!string.IsNullOrEmpty(location.MemberLegalName))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "MEMBER_LEGAL_NAME",
            OldValue = "",
            NewValue = location.MemberLegalName
          });
        if (!string.IsNullOrEmpty(location.IsActive.ToString()))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "IS_ACTIVE",
            OldValue = "",
            NewValue = location.IsActive.ToString()
          });
        if (!string.IsNullOrEmpty(location.RegistrationId))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "REGISTRATION_ID",
            OldValue = "",
            NewValue = location.RegistrationId
          });
        if (!string.IsNullOrEmpty(location.TaxVatRegistrationNumber))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "TAX_VAT_REGISTRATION_NUMBER",
            OldValue = "",
            NewValue = location.TaxVatRegistrationNumber
          });
        if (!string.IsNullOrEmpty(location.AdditionalTaxVatRegistrationNumber))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "ADD_TAX_VAT_REGISTRATION_NUM",
            OldValue = "",
            NewValue = location.AdditionalTaxVatRegistrationNumber
          });
        if (!string.IsNullOrEmpty(location.Country.Id))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "COUNTRY_CODE",
            OldValue = "",
            NewValue = location.Country.Id
          });
        if (!string.IsNullOrEmpty(location.Country.Name))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "COUNTRY_NAME",
            OldValue = "",
            NewValue = location.Country.Name
          });
        if (!string.IsNullOrEmpty(location.AddressLine1))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "ADDRESS_LINE1",
            OldValue = "",
            NewValue = location.AddressLine1
          });
        if (!string.IsNullOrEmpty(location.AddressLine2))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "ADDRESS_LINE2",
            OldValue = "",
            NewValue = location.AddressLine2
          });
        if (!string.IsNullOrEmpty(location.AddressLine3))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "ADDRESS_LINE3",
            OldValue = "",
            NewValue = location.AddressLine3
          });
        if (!string.IsNullOrEmpty(location.CityName))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "CITY_NAME",
            OldValue = "",
            NewValue = location.CityName
          });
        if (!string.IsNullOrEmpty(location.SubDivisionName))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "SUB_DIVISION_NAME",
            OldValue = "",
            NewValue = location.SubDivisionName
          });
        if (!string.IsNullOrEmpty(location.PostalCode))
          changeInfoReferenceDataForStatusChange.Add(new ChangeInfoReferenceDataModel()
          {
            Action = "N",
            ParticipantCode = location.Member.MemberCodeNumeric,
            LocationId = location.LocationCode,
            FieldName = "POSTAL_CODE",
            OldValue = "",
            NewValue = location.PostalCode
          });
        //Sort here for Location Code
        changeInfoReferenceDataForStatusChange = (from ci in changeInfoReferenceDataForStatusChange
                                                  orderby ci.LocationId ascending
                                                  select ci).ToList();
      }

      changeInfoReferenceDataForStatusChange = (from ci in changeInfoReferenceDataForStatusChange
                                                orderby ci.ParticipantCode ascending
                                                select ci).ToList();
      return changeInfoReferenceDataForStatusChange;
    }

    private IEnumerable<ChangeInfoReferenceDataModel> GetChangeInfoReferenceDataModel(IEnumerable<FutureUpdates> futureUpdateData)
    {
      var changeInfoReferenceDataModel = new List<ChangeInfoReferenceDataModel>();

      var futureUpdateDataLocationMain =
        futureUpdateData.Where(
          fUpd => FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id).ToLower().Equals("main"));
      var futureUpdateDataLocationUatp =
        futureUpdateData.Where(
          fUpd => FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id).ToLower().Equals("uatp"));
      var futureUpdateDataLocationRest =
        futureUpdateData.Where(
          fUpd =>
          FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id).ToLower() != "main" &&
          FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id).ToLower() != "uatp");

      var changeInfoReferenceDataModelMain = new List<ChangeInfoReferenceDataModel>();
      var changeInfoReferenceDataModelUatp = new List<ChangeInfoReferenceDataModel>();
      var changeInfoReferenceDataModelRest = new List<ChangeInfoReferenceDataModel>();

      foreach (var nl in _nlist)
      {

        if (!nl.Equals("COUNTRY_CODE"))
        {
          //----Main Location List-------------------------------------------------------------------------------------------------
          var ttnM = futureUpdateDataLocationMain.Where(fUpdM => fUpdM.ElementName.Equals(nl)).Select(
            fUpd => new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode = fUpd.Member.MemberCodeNumeric,
              LocationId = FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id),
              FieldName = fUpd.ElementName,
              OldValue = fUpd.OldVAlue,
              NewValue = fUpd.NewVAlue
            }).ToList();
          changeInfoReferenceDataModelMain.AddRange(ttnM);
          //----UATP Location List-------------------------------------------------------------------------------------------------
          var ttnU = futureUpdateDataLocationUatp.Where(fUpdU => fUpdU.ElementName.Equals(nl)).Select(
            fUpd => new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode = fUpd.Member.MemberCodeNumeric,
              LocationId = FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id),
              FieldName = fUpd.ElementName,
              OldValue = fUpd.OldVAlue,
              NewValue = fUpd.NewVAlue
            }).ToList();
          changeInfoReferenceDataModelUatp.AddRange(ttnU);
          //----Other Then Main and UATP Location List-------------------------------------------------------------------------------------------------
          var ttnR = futureUpdateDataLocationRest.Where(fUpdM => fUpdM.ElementName.Equals(nl)).Select(
            fUpd => new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode = fUpd.Member.MemberCodeNumeric,
              LocationId = FutureUpdatesServiceRepository.GetRelationIdDisplayName(fUpd.Id),
              FieldName = fUpd.ElementName,
              OldValue = fUpd.OldVAlue,
              NewValue = fUpd.NewVAlue
            }).ToList();
          changeInfoReferenceDataModelRest.AddRange(ttnR);
        }
        else
        {
          //----Main Location List-------------------------------------------------------------------------------------------------
          foreach (var fUpd in futureUpdateDataLocationMain.Where(fUpd => fUpd.ElementName.Contains("COUNTRY_CODE"))
            )
          {
            changeInfoReferenceDataModelMain.Add(new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode =
                fUpd.Member.MemberCodeNumeric,
              LocationId =
                FutureUpdatesServiceRepository.GetRelationIdDisplayName(
                  fUpd.Id),
              FieldName = fUpd.ElementName,
              OldValue = fUpd.OldVAlue,
              NewValue = fUpd.NewVAlue
            });
            changeInfoReferenceDataModelMain.Add(new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode =
                fUpd.Member.MemberCodeNumeric,
              LocationId =
                FutureUpdatesServiceRepository.GetRelationIdDisplayName(
                  fUpd.Id),
              FieldName = "COUNTRY_NAME",
              OldValue = fUpd.OldValueDisplayName,
              NewValue = fUpd.NewValueDisplayName
            });

          }

          //----Uatp Location List-------------------------------------------------------------------------------------------------
          foreach (var fUpd in futureUpdateDataLocationUatp.Where(fUpd => fUpd.ElementName.Contains("COUNTRY_CODE"))
            )
          {
            changeInfoReferenceDataModelMain.Add(new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode =
                fUpd.Member.MemberCodeNumeric,
              LocationId =
                FutureUpdatesServiceRepository.GetRelationIdDisplayName(
                  fUpd.Id),
              FieldName = fUpd.ElementName,
              OldValue = fUpd.OldVAlue,
              NewValue = fUpd.NewVAlue
            });
            changeInfoReferenceDataModelMain.Add(new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode =
                fUpd.Member.MemberCodeNumeric,
              LocationId =
                FutureUpdatesServiceRepository.GetRelationIdDisplayName(
                  fUpd.Id),
              FieldName = "COUNTRY_NAME",
              OldValue = fUpd.OldValueDisplayName,
              NewValue = fUpd.NewValueDisplayName
            });

          }
          //----Other Then Main and UATP Location List-------------------------------------------------------------------------------------------------
          foreach (var fUpd in futureUpdateDataLocationRest.Where(fUpd => fUpd.ElementName.Contains("COUNTRY_CODE"))
            )
          {
            changeInfoReferenceDataModelRest.Add(new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode =
                fUpd.Member.MemberCodeNumeric,
              LocationId =
                FutureUpdatesServiceRepository.GetRelationIdDisplayName(
                  fUpd.Id),
              FieldName = fUpd.ElementName,
              OldValue = fUpd.OldVAlue,
              NewValue = fUpd.NewVAlue
            });
            changeInfoReferenceDataModelRest.Add(new ChangeInfoReferenceDataModel
            {
              Action = GetAction(fUpd.OldVAlue, fUpd.NewVAlue),
              ParticipantCode =
                fUpd.Member.MemberCodeNumeric,
              LocationId =
                FutureUpdatesServiceRepository.GetRelationIdDisplayName(
                  fUpd.Id),
              FieldName = "COUNTRY_NAME",
              OldValue = fUpd.OldValueDisplayName,
              NewValue = fUpd.NewValueDisplayName
            });

          }
        }
      }


      changeInfoReferenceDataModelRest = (changeInfoReferenceDataModelRest.Select(cirdmr => new
      {
        cirdmr.SerialNo,
        cirdmr.Action,
        cirdmr.ParticipantCode,
        LocationId = int.Parse(cirdmr.LocationId),
        cirdmr.FieldName,
        cirdmr.OldValue,
        cirdmr.NewValue
      })).OrderBy(cirdmr => cirdmr.LocationId).Select(rr => new ChangeInfoReferenceDataModel
      {
        SerialNo = rr.SerialNo,
        Action = rr.Action,
        ParticipantCode = rr.ParticipantCode,
        LocationId = rr.LocationId.ToString(),
        FieldName = rr.FieldName,
        OldValue = rr.OldValue,
        NewValue = rr.NewValue
      }).ToList();


      changeInfoReferenceDataModel.AddRange(changeInfoReferenceDataModelMain);
      changeInfoReferenceDataModel.AddRange(changeInfoReferenceDataModelUatp);
      changeInfoReferenceDataModel.AddRange(changeInfoReferenceDataModelRest);
      return changeInfoReferenceDataModel;
    }

    private static string GetAction(string oldValue, string newValue)
    {
      if (string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(newValue))
      {
        return "N";
      }
      if (!string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(newValue) && oldValue != newValue)
      {
        return "C";
      }
      return string.Empty;
    }

    private void AddInFileLogDb(int billingYear, int billingMonth, int billingPeriod, string fileName, FileFormatType fileFormatType)
    {
      //*****Update in IS_FILE_LOG table***************
      _logger.Info("Update in IS_FILE_LOG table...");
      var isInputFile = new IsInputFile
      {
        BillingMonth = billingMonth,
        BillingPeriod = billingPeriod,
        BillingYear = billingYear,
        FileDate = DateTime.UtcNow,
        FileFormat = fileFormatType,
        FileLocation = Path.GetDirectoryName(fileName),
        //File location should not contain file name
        BillingCategory = null,
        SenderReceiver = null,
        FileName = Path.GetFileName(fileName),
        FileStatus = FileStatusType.AvailableForDownload,
        SenderRecieverType = (int)FileSenderRecieverType.Member,
        FileVersion = "0.1",
        IsIncoming = true,
        ReceivedDate = DateTime.UtcNow,
        SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
        OutputFileDeliveryMethodId = 1
      };
      var isFileLog = ReferenceManager.AddIsInputFile(isInputFile);

      //*****Update in IS_FILE_LOG_EXT1 table to store sending information regarding iinet upload of files*****************

      _logger.Info("Update in IS_FILE_LOG_EXT1 table for is_file_log_id : " + isFileLog.Id);

      // Get member Profile Intermediate Data
      var memberProfileIntermediateDataModel = GetMemberProfileIntermediateDataModel(fileFormatType);

      _logger.Info("Member Profile IntermediateDataModel Count : " + memberProfileIntermediateDataModel.Count);

      // Store this Profile Intermediate Data in to "IS_FILE_LOG_EXT1" table dealing with following logic
      // Store Member ID , Account ID, Conctact Id in camma seperated string
      // file will be send in bulk of iiNET Account IDs,using camma seperated iiNET Account IDs
      // Currenty 100 iiNET Account IDs will be be used in single bulk send



      if (memberProfileIntermediateDataModel.Count != 0)
      {
        _logger.Info("Max AccountIds Count For iinet Upload : " + MaxAccountIdsForIinetSingleUpload);
        for (var index = 0; index < memberProfileIntermediateDataModel.Count; index += MaxAccountIdsForIinetSingleUpload)
        {
          var accountIdLIst = string.Empty;
          var contactIdLIst = string.Empty;
          var memberIdLIst = string.Empty;

          var accArray = memberProfileIntermediateDataModel.GetRange(index,
                                                                     ((memberProfileIntermediateDataModel.Count -
                                                                       index) > MaxAccountIdsForIinetSingleUpload)
                                                                       ? MaxAccountIdsForIinetSingleUpload
                                                                       : (memberProfileIntermediateDataModel.Count -
                                                                          index));

          memberIdLIst =
            accArray.Select(i => i.MemberId.ToString(CultureInfo.InvariantCulture)).Aggregate(
              (s1, s2) => s1 + "," + s2);

          accountIdLIst =
            accArray.Select(i => i.AccountId).Aggregate((s1, s2) => s1 + "," + s2);

          var contacts = new List<string>();

          foreach (var contact in from contactList in accArray.ToList().Select(a => a.ContactsList)
                                  from contact in contactList
                                  where !contacts.Contains(contact)
                                  select contact)
          {
            contacts.Add(contact);
          }

          contactIdLIst = string.Join(",", contacts);


          var isFileLogExt1Model = new IsFileLogExt1Model
          {
            MemberIdLIst = memberIdLIst,
            AccountIdLIst = accountIdLIst,
            ContactIdLIst = contactIdLIst,
            IinetUploadStatus = "P",
            IsFileLogId = isFileLog.Id
          };
          IsFileLogExt1ModelRepository.Add(isFileLogExt1Model);
          UnitOfWork.CommitDefault();
          _logger.Info("New Data added in IS_FILE_LOG_EXT1 table succesfully");
        }
      }
      else
      {
        // Update  IS FILE LOG In case No one has opted for file transfer through iiNet
        isFileLog.FileStatus = FileStatusType.iiNetRecipientNotFound;
        isFileLog.FileStatusId = (int)FileStatusType.iiNetRecipientNotFound;
        ReferenceManager.UpdateIsInputFile(isFileLog);

      }
      _logger.Info("Not a single member has opted for " + fileFormatType);
    }

    private List<MemberProfileIntermediateDataModel> GetMemberProfileIntermediateDataModel(FileFormatType fileFormatType)
    {
      var memberProfileIntermediateDataModel = new List<MemberProfileIntermediateDataModel>();

      // Determine the list of Members and their unique iiNET Accounts that require this file. This will be:
      // Members having ‘IS Membership Status’ as “Active” or “Restricted” And 
      // Where in the Member Profile’s e-Billing tab, that Billing Category’s iiNET Account is marked as “True/Yes” 
      switch (fileFormatType)
      {
        case FileFormatType.ChangeInfoReferenceDataUpdateCsv:

          #region ChangeInfoReferenceDataUpdateCsv

          var eBillingConfigCird =
            EBillingRepository.Get(
              ebr =>
              ebr.ChangeInfoRefDataPax || ebr.ChangeInfoRefDataMisc || ebr.ChangeInfoRefDataCgo ||
              ebr.ChangeInfoRefDataUatp).Where(
                ebr =>
                (ebr.Member.IsMembershipStatusId == (int)MemberStatus.Active) ||
                (ebr.Member.IsMembershipStatusId == (int)MemberStatus.Restricted));

          var technicalConfigCird =
            TechnicalRepository.Get(tr =>
                                    (tr.Member.IsMembershipStatusId == (int)MemberStatus.Active) ||
                                    (tr.Member.IsMembershipStatusId == (int)MemberStatus.Restricted));


          var compositeRepoCird = eBillingConfigCird.Join(technicalConfigCird, au => au.MemberId, u => u.MemberId,
                                                          (ebc, tc) =>
                                                          new { EBillingConfigCird = ebc, TechnicalConfigCird = tc });


          foreach (var ebc in compositeRepoCird)
          {
            MemberProfileIntermediateDataModel memberProfileIntermediateData;
            var contact =
              ContactsRepository.GetContactMemberInformation(ebc.TechnicalConfigCird.MemberId,
                                                             (int)
                                                             ProcessingContactType.
                                                               OtherMembersInvoiceReferenceDataUpdates).Select(
                                                                 dd => dd.EmailAddress).ToList();

            if (ebc.EBillingConfigCird.ChangeInfoRefDataPax &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCird.PaxAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCird.MemberId;

              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCird.PaxAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);

              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);

            }

            if (ebc.EBillingConfigCird.ChangeInfoRefDataCgo &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCird.CgoAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCird.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCird.CgoAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);


            }

            if (ebc.EBillingConfigCird.ChangeInfoRefDataMisc &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCird.MiscAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCird.MemberId;

              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCird.MiscAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);


            }
            if (ebc.EBillingConfigCird.ChangeInfoRefDataUatp &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCird.UatpAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCird.MemberId;

              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCird.UatpAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);

            }

          }
          break;

          #endregion

        case FileFormatType.CompleteReferenceDataCsv:

          #region CompleteReferenceDataCsv

          var eBillingConfigCrd =
            EBillingRepository.Get(
              ebr =>
              ebr.CompleteRefDataPax || ebr.CompleteRefDataCgo || ebr.CompleteRefDataMisc ||
              ebr.CompleteRefDataUatp).Where(
                ebr =>
                (ebr.Member.IsMembershipStatusId == (int)MemberStatus.Active) ||
                (ebr.Member.IsMembershipStatusId == (int)MemberStatus.Restricted));

          var technicalConfigCrd =
            TechnicalRepository.Get(tr =>
                                    (tr.Member.IsMembershipStatusId == (int)MemberStatus.Active) ||
                                    (tr.Member.IsMembershipStatusId == (int)MemberStatus.Restricted));

          var compositeRepoCrd = eBillingConfigCrd.Join(technicalConfigCrd, au => au.MemberId, u => u.MemberId,
                                                        (ebc, tc) =>
                                                        new { EBillingConfigCrd = ebc, TechnicalConfigCrd = tc });
          foreach (var ebc in compositeRepoCrd)
          {
            MemberProfileIntermediateDataModel memberProfileIntermediateData;


            var contact =
              ContactsRepository.GetContactMemberInformation(ebc.TechnicalConfigCrd.MemberId,
                                                             (int)
                                                             ProcessingContactType.
                                                               OtherMembersInvoiceReferenceDataUpdates).Select(
                                                                 dd => dd.EmailAddress).ToList();

            if (ebc.EBillingConfigCrd.CompleteRefDataPax && !string.IsNullOrEmpty(ebc.TechnicalConfigCrd.PaxAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCrd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCrd.PaxAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);

              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
            if (ebc.EBillingConfigCrd.CompleteRefDataCgo && !string.IsNullOrEmpty(ebc.TechnicalConfigCrd.CgoAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCrd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCrd.CgoAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
            if (ebc.EBillingConfigCrd.CompleteRefDataMisc && !string.IsNullOrEmpty(ebc.TechnicalConfigCrd.MiscAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCrd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCrd.MiscAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
            if (ebc.EBillingConfigCrd.CompleteRefDataUatp && !string.IsNullOrEmpty(ebc.TechnicalConfigCrd.UatpAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCrd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCrd.UatpAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
          }
          break;

          #endregion

        case FileFormatType.CompleteContactsDataCsv:

          #region CompleteContactsDataCsv

          var eBillingConfigCcd =
            EBillingRepository.Get(
              ebr =>
              ebr.CompleteContactsDataPax || ebr.CompleteContactsDataCgo || ebr.CompleteContactsDataMisc ||
              ebr.CompleteContactsDataUatp).Where(
                ebr =>
                (ebr.Member.IsMembershipStatusId == (int)MemberStatus.Active) ||
                (ebr.Member.IsMembershipStatusId == (int)MemberStatus.Restricted));

          var technicalConfigCcd =
            TechnicalRepository.Get(tr =>
                                    (tr.Member.IsMembershipStatusId == (int)MemberStatus.Active) ||
                                    (tr.Member.IsMembershipStatusId == (int)MemberStatus.Restricted));
          var compositeRepoCcd = eBillingConfigCcd.Join(technicalConfigCcd, au => au.MemberId, u => u.MemberId,
                                                        (ebc, tc) =>
                                                        new { EBillingConfigCcd = ebc, TechnicalConfigCcd = tc });
          foreach (var ebc in compositeRepoCcd)
          {
            MemberProfileIntermediateDataModel memberProfileIntermediateData;


            var contact =
          ContactsRepository.GetContactMemberInformation(ebc.TechnicalConfigCcd.MemberId,
                                                         (int)
                                                         ProcessingContactType.
                                                           OtherMembersInvoiceReferenceDataUpdates).Select(
                                                             dd => dd.EmailAddress).ToList();

            if (ebc.EBillingConfigCcd.CompleteContactsDataPax &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCcd.PaxAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCcd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCcd.PaxAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);

              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
            if (ebc.EBillingConfigCcd.CompleteContactsDataCgo &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCcd.CgoAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCcd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCcd.CgoAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
            if (ebc.EBillingConfigCcd.CompleteContactsDataMisc &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCcd.MiscAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCcd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCcd.MiscAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
            if (ebc.EBillingConfigCcd.CompleteContactsDataUatp &&
                !string.IsNullOrEmpty(ebc.TechnicalConfigCcd.UatpAccountId))
            {
              memberProfileIntermediateData = new MemberProfileIntermediateDataModel();
              memberProfileIntermediateData.MemberId = ebc.TechnicalConfigCcd.MemberId;
              memberProfileIntermediateData.AccountId = ebc.TechnicalConfigCcd.UatpAccountId;
              memberProfileIntermediateData.ContactsList.AddRange(contact);
              AddToMemberProfileIntermediateDataModel(memberProfileIntermediateData, memberProfileIntermediateDataModel);
            }
          }
          break;

          #endregion
      }
      return memberProfileIntermediateDataModel;
    }

    private static void AddToMemberProfileIntermediateDataModel(MemberProfileIntermediateDataModel memberProfileIntermediateData, List<MemberProfileIntermediateDataModel> memberProfileIntermediateDataModel)
    {
      if (memberProfileIntermediateDataModel.Where(
        memProfile =>
        memProfile.MemberId == memberProfileIntermediateData.MemberId &&
        memProfile.AccountId == memberProfileIntermediateData.AccountId).Count() > 0)
      {
        var mem =
          memberProfileIntermediateDataModel.Where(
            memProfile =>
            memProfile.MemberId == memberProfileIntermediateData.MemberId &&
            memProfile.AccountId == memberProfileIntermediateData.AccountId).First();

        mem.ContactsList.AddRange(memberProfileIntermediateData.ContactsList);
      }
      else
      {
        memberProfileIntermediateDataModel.Add(memberProfileIntermediateData);
      }
    }

    private string ReplaceDoubleQuoteString(string q)
    {
      if (q != null)
      {
        return q.Replace("\"", "\"\"");
      }
      else
      {
        return string.Empty;
      }
    }



    private List<ChangeInfoReferenceDataModel> RenameFieldNameForChangeReferenceData(List<ChangeInfoReferenceDataModel> changeInfoReferenceDataModels)
    {
      foreach (var changeInfoReferenceDataModel in changeInfoReferenceDataModels)
      {

        switch (changeInfoReferenceDataModel.FieldName)
        {
          case "MEMBER_LEGAL_NAME":
            changeInfoReferenceDataModel.FieldName = "Member Legal Name";
            break;
          case "IS_ACTIVE":
            changeInfoReferenceDataModel.FieldName = "Active";
            break;
          case "TAX_VAT_REGISTRATION_NUMBER":
            changeInfoReferenceDataModel.FieldName = "Tax Vat Registration Id";
            break;
          case "ADD_TAX_VAT_REGISTRATION_NUM":
            changeInfoReferenceDataModel.FieldName = "Additional Tax Vat Registration Id";
            break;
          case "REGISTRATION_ID":
            changeInfoReferenceDataModel.FieldName = "Company Registration Id";
            break;
          case "ADDRESS_LINE1":
            changeInfoReferenceDataModel.FieldName = "Address Line1";
            break;
          case "ADDRESS_LINE2":
            changeInfoReferenceDataModel.FieldName = "Address Line2";
            break;
          case "ADDRESS_LINE3":
            changeInfoReferenceDataModel.FieldName = "Address Line3";
            break;
          case "CITY_NAME":
            changeInfoReferenceDataModel.FieldName = "City Name";
            break;
          case "SUB_DIVISION_NAME":
            changeInfoReferenceDataModel.FieldName = "Subdivision Name";
            break;
          case "COUNTRY_CODE":
            changeInfoReferenceDataModel.FieldName = "Country Code";
            break;
          case "COUNTRY_NAME":
            changeInfoReferenceDataModel.FieldName = "Country Name";
            break;
          case "POSTAL_CODE":
            changeInfoReferenceDataModel.FieldName = "Postal Code";
            break;
        }

      }

      return changeInfoReferenceDataModels;

    }


    #endregion
  }

}
