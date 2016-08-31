using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.AdminSystem;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Calendar;
using Iata.IS.Data.Impl;
using Iata.IS.Model;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.Enums;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  /// <summary>
  /// This class provide the functionality related to calendar.
  /// </summary>
  public class CalendarManager : ICalendarManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ICalendarRepository CalendarRepository { get; set; }
    public IRepository<CalendarEvent> CalendarEventRepository { get; set; }
    public IRepository<DerivedEventOffsetInfo> CalendarDerivedEventRepository { get; set; }
    public ICalendarProcessRepository CalendarProcessRepository { get; set; }

    /// <summary>
    /// Gets or sets the Passenger Configuration repository.
    /// </summary>
    public IRepository<PassengerConfiguration> PassengerReository { get; set; }

    private readonly List<int> _processedPeriods = new List<int>();
    private readonly List<ISCalendar> _calendarEvents = new List<ISCalendar>();
    private List<CalendarEvent> _calendarEventDetails;
    private List<DerivedEventOffsetInfo> _calendarDerivedEventDetails;
    private List<string> _calendarPeriodOffsetDerivedEventNames;
    private readonly Dictionary<string, int> _eventMap = new Dictionary<string, int>();

    /// <summary>
    ///   allowed types for calendar records.
    /// </summary>
    private readonly Type[] _recordTypes = new[] { typeof(AchCalendarRecord), typeof(IchCalendarRecord) };

    private readonly List<CalendarValidationError> _validationErrors = new List<CalendarValidationError>();
    private bool _headerRowFlag;
    private int _lastUpdatedBy;
    private int _recordCount;
    private bool _isFirstTimeCalendarUpload;
    private BillingPeriod _nextBillingPeriod;

    public List<CalendarEvent> GetCalendarEventList()
    {
      return CalendarEventRepository.GetAll().ToList();
    }

    public List<CalendarProcess> GetCalendarProcessList()
    {
      return CalendarProcessRepository.GetAllCalendarProcesses().ToList();
    }

    public List<DerivedEventOffsetInfo> GetDerivedEventList()
    {
      return CalendarDerivedEventRepository.GetAll().ToList();
    }

    /// <summary>
    /// Searches the calendar events.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <returns></returns>
    public IList<CalendarSearchResult> SearchCalendarEvents(int year, int month, int period)
    {
      return CalendarRepository.SearchCalendarData(year, month, period);
    }

    /// <summary>
    /// Upload/update the calendar schedules from clearing houses
    /// </summary>
    /// <param name="filePath">input csv file path.</param>
    /// <param name="headerRowFlag">header row flag,
    /// 1 - file contains header row
    /// 2 - file doesn't contains header row.</param>
    /// <param name="userId">Id of user uploading the calendar.</param>
    /// <returns></returns>
    public List<CalendarValidationError> UploadCalendarFile(string filePath, bool headerRowFlag, int userId)
    {
      // Set up the multi record engine used to parse ACH or ICH Calendar record.
      Logger.Debug("Initializing MultiRecordEngine.");
      var multiRecordEngine = new MultiRecordEngine(CustomSelector, _recordTypes);
      Logger.Debug("MultiRecordEngine initialized.");
      _headerRowFlag = headerRowFlag;
      _lastUpdatedBy = userId;
      try
      {
        // Begin to read input calendar csv file.
        multiRecordEngine.BeginReadFile(filePath);
        var fromClearanceHouseId = String.Empty;

        var record = (CalendarRecordBase)multiRecordEngine.ReadNext();

        _recordCount = 1;
        if (_headerRowFlag)
        {
          record = (CalendarRecordBase)multiRecordEngine.ReadNext();
        }

        // Process each record - till the end of the calendar file
        while (record != null)
        {
          if (String.IsNullOrEmpty(fromClearanceHouseId))
          {
            fromClearanceHouseId = record.FromClearanceHouseIdentifier;
          }

          // For non consistent Clearance House Identifier, throw an exception.
          if (!fromClearanceHouseId.Equals(record.FromClearanceHouseIdentifier))
          {
            throw new ISBusinessException(ErrorCodes.CalendarFileContainsBothCHInfo);
          }

          if (_recordCount == 1)
          {

            InitilizeEventMap(record.GetType());

            string derivedEventCategory, calendarEventCategory;
            var clearingHouse = ClearingHouse.Ich;
            if (record is AchCalendarRecord)
            {
              calendarEventCategory = CalendarEventCategory.ACH.ToString().ToUpper();
              derivedEventCategory = CalendarEventCategory.ISACH.ToString().ToUpper();
              clearingHouse = ClearingHouse.Ach;
            }
            else
            {
              calendarEventCategory = CalendarEventCategory.ICH.ToString().ToUpper();
              derivedEventCategory = CalendarEventCategory.ISICH.ToString().ToUpper();
            }

            // Check that calendar uploaded first time.
            _isFirstTimeCalendarUpload = CalendarRepository.GetCount(c => c.EventCategory.Equals(calendarEventCategory)) <= 0;

            if (!_isFirstTimeCalendarUpload) _nextBillingPeriod = GetNextBillingPeriod(clearingHouse);

            _calendarEventDetails = CalendarEventRepository.Get(c => (c.EventType.ToUpper().Equals(calendarEventCategory) ||
              c.EventType.ToUpper().Equals(derivedEventCategory))).OrderBy(c => c.Id).ToList();

            _calendarDerivedEventDetails = CalendarDerivedEventRepository.GetAll().ToList();
            _calendarDerivedEventDetails = _calendarDerivedEventDetails.Where(c => _calendarEventDetails.Where(i => i.EventType.Equals(derivedEventCategory)).Select(i => i.Id).Contains(c.Id)).ToList();
            var idList = _calendarDerivedEventDetails.Where(c => c.OffsetPeriods != 0).Select(i => i.Id).ToList();
            _calendarPeriodOffsetDerivedEventNames = _calendarEventDetails.Where(e => idList.Contains(e.Id)).Select(e => e.EventName).ToList();

          }

          // Call ProcessCalendarRecord based on record type.
          ProcessCalendarRecord(record);

          _recordCount++;
          record = (CalendarRecordBase)multiRecordEngine.ReadNext();
        }

        if (_recordCount == 0 || (_headerRowFlag && _recordCount == 1))
        {
          CreateValidationError("There are no records in csv file for processing.");
        }
      }
      catch (FileHelpersException fileHelpersException)
      {
        throw new ISBusinessException(fileHelpersException.Message);
      }
      finally
      {
        multiRecordEngine.Close();
      }

      // If there are no errors and events to add on database then perform database operations.
      if (_validationErrors.Count <= 0 && _calendarEvents.Count > 0)
      {
        foreach (var calendarEvent in _calendarEvents)
        {
          if (calendarEvent.Id <= 0)
          {
            CalendarRepository.Add(calendarEvent);
          }
        }

        // Commit the changes.
        UnitOfWork.CommitDefault();
      }

      return _validationErrors;
    }

    /// <summary>
    /// This method will return list of billing period objects which contain list of year, month and period values for which billing is allowed
    /// </summary>
    /// <param name="memberId">ID of member who is creating invoices</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="nextPeriodsToInclude">This parameter defines how many next periods after the current billing period are to be returned. Default value is 4</param>
    /// <returns></returns>
    public List<BillingPeriod> GetRelevantBillingPeriods(string memberId, ClearingHouse clearingHouse, int nextPeriodsToInclude = 4)
    {
      var billingPeriods = new List<BillingPeriod>();
      var currentPeriod = GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);

      // If late submission window is open then only add previous period.
      var previousPeriod = GetPreviousBillingPeriod(currentPeriod, true);
      if (IsLateSubmissionWindowOpen(previousPeriod))
      {
        billingPeriods.Add(previousPeriod);
      }

      // Add current period.
      billingPeriods.Add(currentPeriod);

      // Get the specified number of future periods
      var count = 0;
      var futurePeriod = currentPeriod;
      while (count < nextPeriodsToInclude)
      {
        try
        {
          futurePeriod = GetNextBillingPeriod(futurePeriod, true);
          billingPeriods.Add(futurePeriod);
        }
        catch (ISCalendarDataNotFoundException exception)
        {
          Logger.Error(String.Format("Error while 'GetNextBillingPeriod' call for period: {0}-{1}-{2}", futurePeriod.Year, futurePeriod.Month, futurePeriod.Period), exception);
        }
        count++;
      }

      return (billingPeriods);
    }

    /// <summary>
    /// This method checks if future submission is allowed for the given future period.
    /// </summary>
    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    public bool IsFutureSubmissionOpen(BillingPeriod billingPeriod)
    {
      var futureSubmissionRecord = CalendarRepository.First(c => (CalendarConstants.FutureDatedSubmissionsOpenColumn.Equals(c.EventDescription)) && c.Year == billingPeriod.Year &&
        c.Month == billingPeriod.Month && c.Period == billingPeriod.Period && c.IsActive);
      if (futureSubmissionRecord == null) return false;

      if (DateTime.UtcNow >= futureSubmissionRecord.EventDateTime)
        return true;

      return false;
    }

    /// <summary>
    /// This method tries to return the current billing period. If the current billing period is void or insufficient data exists in the
    /// database, then try to get the previous billing period. If that fails, raise an exception.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Current/Previous billing period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    public BillingPeriod GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse clearingHouse)
    {
      BillingPeriod currentPeriod;

      try
      {
        // Try to get the current billing period.
        currentPeriod = GetCurrentBillingPeriod(clearingHouse);
      }
      catch (ISCalendarDataNotFoundException)
      {
        currentPeriod = GetLastClosedBillingPeriod(DateTime.UtcNow, clearingHouse);
      }

      return currentPeriod;
    }

    /// <summary>
    /// Returns the last closed billing period for current date time and clearing house passed.
    /// </summary>
    /// <param name="clearingHouse">The clearing house. Default value is ICH.</param>
    /// <returns>
    /// Last closed billing period value.
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    public BillingPeriod GetLastClosedBillingPeriod(ClearingHouse clearingHouse = ClearingHouse.Ich)
    {
      return GetLastClosedBillingPeriod(DateTime.UtcNow, clearingHouse);
    }

    /// <summary>
    /// Returns the last closed billing period for the date time and clearing house passed.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Last closed billing period value.
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    public BillingPeriod GetLastClosedBillingPeriod(DateTime date, ClearingHouse clearingHouse = ClearingHouse.Ich)
    {
      // Get last closed billing period.
      var periodEvents = GetPreviousPeriodEvents(clearingHouse, date);
      var periodInfo = string.Format("{0}-Previous | Time:{1}", clearingHouse, date);
      return GetBillingPeriod(periodEvents, clearingHouse, periodInfo);
    }

    /// <summary>
    /// Returns the last closed billing period for current date time and clearing house passed.
    /// Same as GetLastClosedBillingPeriod(ClearingHouse clearingHouse) method.
    /// </summary>
    /// <param name="clearingHouse">clearing house</param>
    /// <returns>
    /// Last closed billing period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    [Obsolete("This method is obsolete, it has been replaced by CalendarManager.GetLastClosedBillingPeriod")]
    public BillingPeriod GetPreviousBillingPeriod(ClearingHouse clearingHouse)
    {
      return GetLastClosedBillingPeriod(DateTime.UtcNow, clearingHouse);
    }

    /// <summary>
    /// This method is used for retrieving current open billing period based on current utc date time and <B>ICH</B>.
    /// </summary>
    /// <returns>
    /// Current Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If the billing period is void or insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    ///   </exception>
    public BillingPeriod GetCurrentBillingPeriod()
    {
      return GetBillingPeriod(ClearingHouse.Ich, DateTime.UtcNow);
    }

    /// <summary>
    /// This method tries to return the billing period for given clearing house and date time.
    /// </summary>
    /// <param name="inputDate">input date</param>
    /// <param name="clearingHouse">The clearing house. Default value is ICH.</param>
    /// <returns>
    /// Billing Period for given date time
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If the billing period is void or insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    public BillingPeriod GetBillingPeriod(DateTime inputDate, ClearingHouse clearingHouse = ClearingHouse.Ich)
    {
      return GetBillingPeriod(clearingHouse, inputDate);
    }

    /// <summary>
    /// This method is used for retrieving current open billing period based on current utc date and given clearing house.
    /// </summary>
    /// <param name="clearingHouse">The clearing house. Default value is ICH.</param>
    /// <returns>
    /// Current Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If the billing period is void or insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    ///   </exception>
    public BillingPeriod GetCurrentBillingPeriod(ClearingHouse clearingHouse)
    {
      return GetBillingPeriod(clearingHouse, DateTime.UtcNow);
    }

    /// <summary>
    /// This method is used for retrieving billing period based on given inputs.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <returns>
    /// billing period
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If the billing period is void or insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    public BillingPeriod GetBillingPeriod(ClearingHouse clearingHouse, int year, int month, int period)
    {
      var inputPeriod = new DateTime(year, month, period);
      var calendarEvents = CalendarRepository.GetPeriodEvents(inputPeriod, 1, clearingHouse.ToString(), 0);

      var periodInfo = string.Format("{0}-Period | Period:{1:D4}-{2:D2}-P{3}", clearingHouse, year, month, period);
      return GetBillingPeriod(calendarEvents, clearingHouse, periodInfo);
    }

    public BillingPeriod GetAutoBillingPeriod(DateTime inputDate, double offsetHours, BillingPeriod currentOpenPeriod)
    {
      try
      {
        //var billingMemberAutoBillingFinalizationDate = 
        var currentPeriodFinalizationEvent =
          CalendarRepository.Get(
            c =>
            CalendarConstants.AutoBillingInvoiceFinalizationColumn.Equals(c.EventDescription) &&
            c.Year == currentOpenPeriod.Year
            && c.Month == currentOpenPeriod.Month && c.Period == currentOpenPeriod.Period)
            .OrderBy(c => c.EventDateTime).First();

        if (currentPeriodFinalizationEvent == null)
        {
          throw new Exception();
          //throw new ISCalendarDataNotFoundException(ErrorCodes.ISCalendarDataNotFoundException);
        }
        else
        {
          // If cut-off date time is greater than System date get Current Auto biliing period as open period else get next Auto Billing period as open period
          if (currentPeriodFinalizationEvent.EventDateTime.AddHours(offsetHours*-1) >= inputDate)
          {
            return new BillingPeriod
                     {
                       Year = currentPeriodFinalizationEvent.Year,
                       Month = currentPeriodFinalizationEvent.Month,
                       Period = currentPeriodFinalizationEvent.Period
                     };
          }
          else
          {
            var nextOpenPeriod = GetNextBillingPeriod(currentOpenPeriod,false);
            
            return new BillingPeriod
                     {
                       Year = nextOpenPeriod.Year,
                       Month = nextOpenPeriod.Month,
                       Period = nextOpenPeriod.Period
                     };
          }
        }
      }
      catch (Exception exception)
      {
        Logger.Error("Error while deriving Auto Billing Open Period", exception);
        throw new ISCalendarDataNotFoundException(ErrorCodes.ISCalendarDataNotFoundException);
      }
    }

    public BillingPeriod GetCurrentAutoBillingPeriod(int airLineId)
    {
      var offSetHrs = 0;
      BillingPeriod billingPeriod;

      try
      {
        if (PassengerReository != null)
        {
          offSetHrs = PassengerReository.Single(p => p.MemberId == airLineId) != null
                        ? PassengerReository.Single(p => p.MemberId == airLineId).CutOffTime
                        : 0;
        }
        var currentOrNextPeriod = GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

        billingPeriod = GetAutoBillingPeriod(DateTime.Now, offSetHrs, currentOrNextPeriod);
      }
      catch (ISCalendarDataNotFoundException)
      {
        try
        {
          billingPeriod = GetBillingPeriod(DateTime.UtcNow, ClearingHouse.Ich);

        }
        catch (ISCalendarDataNotFoundException)
        {
          billingPeriod = GetLastClosedBillingPeriod(DateTime.UtcNow, ClearingHouse.Ich);
        }
        catch (Exception)
        {
          billingPeriod = GetLastClosedBillingPeriod(DateTime.UtcNow, ClearingHouse.Ich);
        }
      }
      
      return billingPeriod;
    }

    /// <summary>
    /// This method tries to return the current billing period.
    /// If the current billing period is void or insufficient data exists in the database, then try to get the next billing period. If that fails, raise an exception.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Billing Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    ///   </exception>
    public BillingPeriod GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse clearingHouse)
    {
      BillingPeriod currentPeriod;

      try
      {
        // Try to get the current billing period.
        currentPeriod = GetCurrentBillingPeriod(clearingHouse);
      }
      catch (ISCalendarDataNotFoundException)
      {
        // Current billing period not found, try to get the next billing period.
        currentPeriod = GetNextBillingPeriod(DateTime.UtcNow, clearingHouse);
      }

      return currentPeriod;
    }

    /// <summary>
    /// Retrieves the next billing period based on clearing house passed and current date time.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Next Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    public BillingPeriod GetNextBillingPeriod(ClearingHouse clearingHouse)
    {
      return GetNextBillingPeriod(DateTime.UtcNow, clearingHouse);
    }

    /// <summary>
    /// Retrieves the next billing period based on clearing house and date time passed.
    /// </summary>
    /// <param name="inputDate">The input date.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Next Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    private BillingPeriod GetNextBillingPeriod(DateTime inputDate, ClearingHouse clearingHouse)
    {

      var periodEvents = GetNextPeriodEvents(clearingHouse, inputDate);
      var periodInfo = string.Format("{0}-Next | Time:{1}", clearingHouse, inputDate);
      return GetBillingPeriod(periodEvents, clearingHouse, periodInfo);
    }

    /// <summary>
    /// This method will return period either for current and for the previous  periods.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="includeCurrentPeriod">if set to <c>true</c> [include current period].</param>
    /// <returns>
    /// List of suspension form billing periods
    /// </returns>
    public List<BillingPeriod> GetSuspensionBillingPeriods(ClearingHouse clearingHouse, bool includeCurrentPeriod)
    {
      // Get current billing period.
      var currentBillingPeriod = GetCurrentPeriodIfOpenOrPreviousAsCurrent(clearingHouse);
      var suspensionBillingPeriods = new List<BillingPeriod>();

      // Add current billing period if includeCurrentPeriod set to true.
      if (includeCurrentPeriod)
      {
        suspensionBillingPeriods.Add(currentBillingPeriod);
      }

      try
      {
        var previousBillingPeriod = GetPreviousBillingPeriod(currentBillingPeriod, true);
        suspensionBillingPeriods.Add(previousBillingPeriod);
      }
      catch (ISCalendarDataNotFoundException exception)
      {
        Logger.Error(String.Format("Error while 'GetPreviousBillingPeriod' call for period: {0}-{1}-{2}", currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period), exception);
      }

      return suspensionBillingPeriods;
    }

    /// <summary>
    /// Get Default suspension billing period (returns  previous 6 billing periods)
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="includeCurrentPeriod">if set to <c>true</c> [include current period].</param>
    /// <param name="previousPeriodsToInclude">This parameter defines how many previous periods before the current billing period are to be returned. Default value is 5.</param>
    /// <returns>
    /// List of default suspension billing periods
    /// </returns>
    public List<BillingPeriod> GetDefaultSuspensionPeriods(ClearingHouse clearingHouse, bool includeCurrentPeriod, int previousPeriodsToInclude = 5)
    {
      // Get current billing period.
      var currentBillingPeriod = GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);

      var lstBillingPeriods = new List<BillingPeriod> { currentBillingPeriod };
      var count = 0;
      var previousPeriod = currentBillingPeriod;

      while (count < 5)
      {
        try
        {
          previousPeriod = GetPreviousBillingPeriod(previousPeriod, true);
          lstBillingPeriods.Add(previousPeriod);
        }
        catch (ISCalendarDataNotFoundException exception)
        {
          Logger.Error(String.Format("Error while 'GetPreviousBillingPeriod' call for period: {0}-{1}-{2}", previousPeriod.Year, previousPeriod.Month, previousPeriod.Period), exception);
        }

        count++;
      }

      return lstBillingPeriods;
    }

    /// <summary>
    /// This method returns a flag indicating if the late submission window is open or not for the clearing house passed and current utc date time.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse)
    {
      try
      {
        return IsLateSubmissionWindowOpen(clearingHouse, DateTime.UtcNow);
      }
      catch (ISCalendarDataNotFoundException)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
        return false;
      }
    }

    /// <summary>
    /// This method returns a flag indicating if the late submission window is open or not for the clearing house and billing period passed.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, BillingPeriod billingPeriod)
    {
      try
      {
        var timeInterval = GetLateSubmissionWindow(clearingHouse, billingPeriod);

        return timeInterval.Start <= DateTime.UtcNow && timeInterval.End >= DateTime.UtcNow;
      }
      catch (Exception)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
        return false;
      }
    }

    /// <summary>
    /// This method takes the clearing house as well as date for which returns the status of late submission window.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <param name="inputDate">The input date.</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, DateTime inputDate)
    {
      try
      {
        var timeInterval = GetLateSubmissionWindow(clearingHouse, inputDate);

        return timeInterval.Start <= inputDate && timeInterval.End >= inputDate;
      }
      catch (Exception)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
        return false;
      }
    }
   
    /// <summary>
    /// This method returns a flag indicating if the late submission window is open or not for the clearing house and billing period passed and datetime.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="inputDate">The inputDate.</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, BillingPeriod billingPeriod, DateTime inputDate)
    {
        try
        {
            var timeInterval = GetLateSubmissionWindow(clearingHouse, billingPeriod);

            return timeInterval.Start <= inputDate && timeInterval.End >= inputDate;
        }
        catch (Exception)
        {
            Logger.Error(Messages.LateSubmissionWindowError);
            return false;
        }
    }
    /// <summary>
    /// This method is used for retrieving Late Submission Start and End Date times for a clearing house and billing period
    /// </summary>
    /// <param name="clearingHouse">clearing house flag</param>
    /// <param name="billingPeriod">corresponding billing period.</param>
    /// <returns>
    /// Late Submission Start and End Date times.
    /// </returns>
    public TimeInterval GetLateSubmissionWindow(ClearingHouse clearingHouse, BillingPeriod billingPeriod)
    {
      var calendarEvents =
        (clearingHouse == ClearingHouse.Ach
           ? CalendarRepository.Get(
             c =>
             (CalendarConstants.SubmissionDeadlineForAchInvoicesColumn.Equals(c.EventDescription) || CalendarConstants.ClosureOfLateSubmissionsAchColumn.Equals(c.EventDescription)) &&
             c.Year == billingPeriod.Year && c.Month == billingPeriod.Month && c.Period == billingPeriod.Period)
           : CalendarRepository.Get(
             c =>
             (CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoicesColumn.Equals(c.EventDescription) || CalendarConstants.ClosureOfLateSubmissionsIchColumn.Equals(c.EventDescription)) &&
             c.Year == billingPeriod.Year && c.Month == billingPeriod.Month && c.Period == billingPeriod.Period)).ToList();

      if (calendarEvents.Count != 2)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
      }

      return GetLateSubmissionWindowTimeInterval(calendarEvents);
    }

    /// <summary>
    /// This method is used for retrieving Late Submission Start and End Date times for a clearing house and billing period
    /// </summary>
    /// <param name="clearingHouse">clearing house flag</param>
    /// <param name="inputDate">The input date.</param>
    /// <returns>
    /// Late Submission Start and End Date times.
    /// </returns>
    public TimeInterval GetLateSubmissionWindow(ClearingHouse clearingHouse, DateTime inputDate)
    {
      var calendarEvents = CalendarRepository.GetPeriodEvents(inputDate, 0, clearingHouse.ToString(), 1);
      var periodInfo = string.Format("{0}-Date | Time:{1}", clearingHouse, DateTime.UtcNow);

      if (calendarEvents.Count != 2)
      {
        Logger.ErrorFormat("Less than 2 calendar events found for late submission window: {0}", periodInfo);
        throw new ISCalendarDataNotFoundException(Messages.LateSubmissionWindowError);
      }

      // If year, month and period values of two events are different then throw error.
      if (calendarEvents[0].Year != calendarEvents[1].Year || calendarEvents[0].Month != calendarEvents[1].Month || calendarEvents[0].Period != calendarEvents[1].Period)
      {
        Logger.ErrorFormat("The year, month and period values of two events is mismatched for late submission window: {0}", periodInfo);
        throw new ISCalendarDataNotFoundException(ErrorCodes.ISCalendarDataNotFoundException);
      }

      return GetLateSubmissionWindowTimeInterval(calendarEvents);
    }

    /// <summary>
    /// Adds the IS calendar.
    /// </summary>
    /// <param name="isCalendar">The is calendar.</param>
    /// <returns></returns>
    public ISCalendar AddISCalendar(ISCalendar isCalendar)
    {
      var isCalendarData = CalendarRepository.Single(type => type.Name.ToLower() == isCalendar.Name.ToLower());

      // If calendar data already exists, throw exception
      if (isCalendarData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAirportCombination);
      }

      CalendarRepository.Add(isCalendar);
      UnitOfWork.CommitDefault();

      return isCalendar;
    }

    /// <summary>
    /// Updates the IS calendar.
    /// </summary>
    /// <param name="isCalendar">The is calendar.</param>
    /// <returns></returns>
    public ISCalendar UpdateISCalendar(ISCalendar isCalendar)
    {
      var updateISCalendar = CalendarRepository.Update(isCalendar);
      UnitOfWork.CommitDefault();

      return updateISCalendar;
    }

    /// <summary>
    ///   Deletes the IS calendar.
    /// </summary>
    /// <param name = "isCalendarId">The is calendar id.</param>
    public bool DeleteISCalendar(int isCalendarId)
    {
      var delete = false;
      var isCalendarData = CalendarRepository.Single(type => type.Id == isCalendarId);

      if (isCalendarData != null)
      {
        isCalendarData.IsActive = !(isCalendarData.IsActive);
        delete = true;

        UnitOfWork.CommitDefault();
      }

      return delete;
    }

    /// <summary>
    ///   Gets the IS calendar details.
    /// </summary>
    /// <param name = "isCalendarId">The is calendar id.</param>
    /// <returns></returns>
    public ISCalendar GetISCalendarDetails(int isCalendarId)
    {
      var isCalendarData = CalendarRepository.Single(type => type.Id == isCalendarId);
      return isCalendarData;
    }

    /// <summary>
    ///   Gets all IS calendar list.
    /// </summary>
    /// <returns></returns>
    public List<ISCalendar> GetAllISCalendarList()
    {
      var calendarData = CalendarRepository.GetAll();

      return calendarData.ToList();
    }

    /// <summary>
    ///   Gets the IS calendar list.
    /// </summary>
    /// <param name = "month">The month.</param>
    /// <param name = "period">The period.</param>
    /// <param name = "eventCategory">The event category.</param>
    /// <returns></returns>
    public List<ISCalendar> GetISCalendarList(int month, int period, string eventCategory)
    {
      var isCalendarList = CalendarRepository.GetAll().ToList();

      if (month > 0)
      {
        isCalendarList = isCalendarList.Where(cl => (cl.Month == month)).ToList();
      }

      if (period > 0)
      {
        isCalendarList = isCalendarList.Where(cl => cl.Period == period).ToList();
      }

      if (!String.IsNullOrEmpty(eventCategory))
      {
        isCalendarList = isCalendarList.Where(cl => (cl.EventCategory.ToLower().Contains(eventCategory.ToLower()))).ToList();
      }

      return isCalendarList.ToList();
    }

    public DateTime GetCalendarEventTime(string eventName, int billingYear, int billingMonth, int Period)
      {
          DateTime eventTime= new DateTime();
          var calendarEvent = CalendarRepository.First(
                  c => eventName.Equals(c.EventDescription) &&
                       c.Year == billingYear && c.Month == billingMonth &&
                       c.Period == Period);
          if (calendarEvent != null)
              eventTime = calendarEvent.EventDateTime;
          return eventTime;
      }

    /// <summary>
    ///   Gets IS calendar upcoming events
    /// </summary>
    /// <param name = "eventCategory">The event category.</param>
    /// <param name = "recordCount">No. of records to be fetched.</param>
    /// <param name = "localTimeZoneId"></param>
    /// <returns></returns>
    public List<UpcomingEventsResultSet> GetUpComingIsCalendarList(string eventCategory, int recordCount, string localTimeZoneId)
    {

      TimeZoneInfo localtimeZone;
      TimeZoneInfo ymqtimeZone;
      var utctimeZone = TimeZoneInfo.Utc;
      const string easternStandardTime = "Eastern Standard Time";
      try
      {
        ymqtimeZone = TimeZoneInfo.FindSystemTimeZoneById(SystemParameters.Instance.General.YmqTimeZoneName);

      }
      catch (Exception)
      {

        ymqtimeZone = TimeZoneInfo.FindSystemTimeZoneById(easternStandardTime);
      }

      try
      {
        localtimeZone = TimeZoneInfo.FindSystemTimeZoneById(String.IsNullOrEmpty(localTimeZoneId) ? SystemParameters.Instance.General.YmqTimeZoneName : localTimeZoneId);
      }
      catch (Exception e)
      {
        localtimeZone = TimeZoneInfo.FindSystemTimeZoneById(easternStandardTime);
      }
      var isCalendarList =
        CalendarRepository.Get(calEvent => calEvent.EventDateTime > DateTime.UtcNow && calEvent.DisplayOnHomePage && calEvent.EventCategory.ToLower().Contains(eventCategory.ToLower())).OrderBy(
          calEvent => calEvent.EventDateTime).Take(recordCount).ToList();

      var upcomingEventsResultSet =
        isCalendarList.Select(
          t =>
          new UpcomingEventsResultSet
            {
              EventDescription = t.Name,
              LocalDateTime = TimeZoneInfo.ConvertTime(t.EventDateTime, utctimeZone, localtimeZone),
              YmqDateTime = TimeZoneInfo.ConvertTime(t.EventDateTime, utctimeZone, ymqtimeZone),
              Period = String.Format("{0} {1} P{2}", EnumList.GetMonthDisplayValue((Month)t.Month), t.Year, t.Period)
            }).ToList();

      return upcomingEventsResultSet.ToList();
    }

    private BillingPeriod GetBillingPeriod(ClearingHouse clearingHouse, DateTime inputUtcDate)
    {
      var calendarEvents = CalendarRepository.GetPeriodEvents(inputUtcDate, 0, clearingHouse.ToString(), 0);
      var periodInfo = string.Format("{0}-Date | Time:{1}", clearingHouse, inputUtcDate);

      return GetBillingPeriod(calendarEvents, clearingHouse, periodInfo);
    }

    private static BillingPeriod GetBillingPeriod(IList<ISCalendar> calendarEvents, ClearingHouse clearingHouse, string periodInfo = "")
    {
      if (calendarEvents.Count != 2)
      {
        Logger.ErrorFormat("Less than 2 calendar events found for period: {0}", periodInfo);
        throw new ISCalendarDataNotFoundException(ErrorCodes.ISCalendarDataNotFoundException);
      }

      var billingPeriod = new BillingPeriod { ClearingHouse = clearingHouse };

      // If year, month and period values of two events are different then throw error.
      if (calendarEvents[0].Year != calendarEvents[1].Year || calendarEvents[0].Month != calendarEvents[1].Month || calendarEvents[0].Period != calendarEvents[1].Period)
      {
        Logger.ErrorFormat("The year, month and period values of two events is mismatched for period: {0}", periodInfo);
        throw new ISCalendarDataNotFoundException(ErrorCodes.ISCalendarDataNotFoundException);
      }

      // Set year, month and period values for billing period.
      billingPeriod.Year = calendarEvents[0].Year;
      billingPeriod.Month = calendarEvents[0].Month;
      billingPeriod.Period = calendarEvents[0].Period;

      // Set start date and end date value of billing period.
      foreach (var isCalendar in calendarEvents)
      {
        if (isCalendar.EventDescription.Equals(CalendarConstants.SubmissionsOpenColumn))
        {
          billingPeriod.StartDate = isCalendar.EventDateTime;
        }
        else if (isCalendar.EventDescription.Equals(CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoicesColumn) ||
                 isCalendar.EventDescription.Equals(CalendarConstants.SubmissionDeadlineForAchInvoicesColumn))
        {
          billingPeriod.EndDate = isCalendar.EventDateTime;
        }
      }
      return billingPeriod;
    }

    private BillingPeriod GetNextBillingPeriod(BillingPeriod period, bool isDateRangeRequired)
    {
      int periodCount, monthCount = period.Month, yearCount = period.Year;

      if (period.Period == 4)
      {
        if (period.Month == 12)
        {
          yearCount = period.Year + 1;
          monthCount = 1;
        }
        else
        {
          monthCount = period.Month + 1;
        }
        periodCount = 1;
      }
      else
      {
        periodCount = period.Period + 1;
      }
      if (isDateRangeRequired)
      {
        return GetBillingPeriod(period.ClearingHouse, yearCount, monthCount, periodCount);
      }

      return new BillingPeriod { ClearingHouse = period.ClearingHouse, Year = yearCount, Month = monthCount, Period = periodCount };
    }

    private BillingPeriod GetPreviousBillingPeriod(BillingPeriod period, bool isDateRangeRequired)
    {
      int periodCount, monthCount = period.Month, yearCount = period.Year;
      if (period.Period == 1)
      {
        if (period.Month == 1)
        {
          yearCount = period.Year - 1;
          monthCount = 12;
        }
        else
        {
          monthCount = period.Month - 1;
        }
        periodCount = 4;
      }
      else
      {
        periodCount = period.Period - 1;
      }

      if (isDateRangeRequired)
      {
        return GetBillingPeriod(period.ClearingHouse, yearCount, monthCount, periodCount);
      }

      return new BillingPeriod { ClearingHouse = period.ClearingHouse, Year = yearCount, Month = monthCount, Period = periodCount };
    }

    /// <summary>
    ///   This method takes the clearing house and returns if its late submission window is open or not
    /// </summary>
    /// <param name = "period">The period.</param>
    /// <returns>
    ///   a flag indicating if the late submission window is open or not
    /// </returns>
    private bool IsLateSubmissionWindowOpen(BillingPeriod period)
    {
      try
      {
        var timeInterval = GetLateSubmissionWindow(period.ClearingHouse, period);

        return timeInterval.Start <= DateTime.UtcNow && timeInterval.End >= DateTime.UtcNow;
      }
      catch (ISCalendarDataNotFoundException)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
        return false;
      }
      catch (ISBusinessException businessException)
      {
        Logger.Error(businessException.Message, businessException);
        return false;
      }
    }

    /// <summary>
    ///   Gets the next period events.
    /// </summary>
    /// <param name = "clearingHouse">The clearing house.</param>
    /// <param name = "inputDate">The input date.</param>
    /// <returns>List of calendar events for next billing period.</returns>
    private List<ISCalendar> GetNextPeriodEvents(ClearingHouse clearingHouse, DateTime inputDate)
    {
      var endEvent = CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoicesColumn;
      if (clearingHouse == ClearingHouse.Ach)
      {
        endEvent = CalendarConstants.SubmissionDeadlineForAchInvoicesColumn;
      }
      var calendarEvents =
        CalendarRepository.Get(c => (CalendarConstants.SubmissionsOpenColumn.Equals(c.EventDescription) || endEvent.Equals(c.EventDescription)) && c.EventDateTime >= inputDate).OrderBy(
      c => c.EventDateTime).Take(3).ToList();

      // To get proper calendar events in void as well as current open period case.
      if (calendarEvents.Count == 3)
      {
        calendarEvents.RemoveAt(calendarEvents[0].EventDescription.Equals(endEvent) ? 0 : 2);
      }

      if (calendarEvents.Count != 2)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
      }

      return calendarEvents;
    }

    /// <summary>
    ///   Gets the previous period events.
    /// </summary>
    /// <param name = "clearingHouse">The clearing house.</param>
    /// <param name = "inputDate">The input date.</param>
    /// <returns>List of calendar events for previous billing period.</returns>
    private List<ISCalendar> GetPreviousPeriodEvents(ClearingHouse clearingHouse, DateTime inputDate)
    {
      var endEvent = CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoicesColumn;
      if (clearingHouse == ClearingHouse.Ach)
      {
        endEvent = CalendarConstants.SubmissionDeadlineForAchInvoicesColumn;
      }

     var calendarEvents =
       CalendarRepository.Get(c => (CalendarConstants.SubmissionsOpenColumn.Equals(c.EventDescription) || endEvent.Equals(c.EventDescription)) && c.EventDateTime <= inputDate).OrderByDescending(
        c => c.EventDateTime).Take(3).ToList();


      // To get proper calendar events in void as well as current open period case.
      if (calendarEvents.Count == 3)
      {
        calendarEvents.RemoveAt(calendarEvents[0].EventDescription.Equals(CalendarConstants.SubmissionsOpenColumn) ? 0 : 2);
      }

      if (calendarEvents.Count != 2)
      {
        Logger.Error(Messages.LateSubmissionWindowError);
      }

      return calendarEvents;
    }

    private static TimeInterval GetLateSubmissionWindowTimeInterval(IEnumerable<ISCalendar> calendarEvents)
    {
      var timeInterval = new TimeInterval();
      foreach (var calendarEvent in calendarEvents)
      {
        switch (calendarEvent.EventDescription)
        {
          case CalendarConstants.SubmissionDeadlineForAchInvoicesColumn:
          case CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoicesColumn:
            timeInterval.Start = calendarEvent.EventDateTime;
            break;
          case CalendarConstants.ClosureOfLateSubmissionsAchColumn:
          case CalendarConstants.ClosureOfLateSubmissionsIchColumn:
            timeInterval.End = calendarEvent.EventDateTime;
            break;
        }
      }
      return timeInterval;
    }



    /// <summary>
    ///   Process ach calendar record.
    /// </summary>
    /// <param name = "record">ach calendar record object.</param>
    private void ProcessCalendarRecord<T>(T record) where T : CalendarRecordBase
    {
      // Validate Period value in record.
      int period;
      if (!Int32.TryParse(record.Period, out period) || period < 1 || period > 4)
      {
        CreateValidationError(String.Format(Messages.CalendarPeriodValueInvalid, CalendarConstants.Period));
        return;
      }

      // Validate CalendarMonth value in record.
      DateTime calendarMonth;
      if (!DateTime.TryParseExact(record.CalendarMonth, "yyMM", null, DateTimeStyles.None, out calendarMonth))
      {
        CreateValidationError(String.Format(Messages.CalendarValueInvalid, CalendarConstants.CalendarMonth));
        return;
      }

      // Get billing period for current processing record.
      var newBillingPeriod = new BillingPeriod(calendarMonth.Year, calendarMonth.Month, period);
      // Validate the billing period.
      if (!IsValidPeriod(newBillingPeriod))
      {
        return;
      }

      // Get event category and derived event category for corresponding period record.
      CalendarEventCategory eventCategory;
      string derivedEventCategory;
      if (record is AchCalendarRecord)
      {
        eventCategory = CalendarEventCategory.ACH;
        newBillingPeriod.ClearingHouse = ClearingHouse.Ach;
        derivedEventCategory = CalendarEventCategory.ISACH.ToString().ToUpper();
      }
      else
      {
        eventCategory = CalendarEventCategory.ICH;
        derivedEventCategory = CalendarEventCategory.ISICH.ToString().ToUpper();
      }

      // For closed, current open and next periods ignore the record processing.
      if (IsPeriodLessThanNextBillingPeriod(newBillingPeriod))
      {
        return;
      }

      // Get list of existing events for current record from database for the given event type
      var calendarEventCategory = Convert.ToString(eventCategory).ToUpper();
      var existingEvents =
        CalendarRepository.Get(
          c =>
          c.Year == calendarMonth.Year && c.Month == calendarMonth.Month && c.Period == period &&
          (c.EventCategory.ToUpper().Equals(calendarEventCategory) || c.EventCategory.ToUpper().Equals(derivedEventCategory)) && !_calendarPeriodOffsetDerivedEventNames.Contains(c.EventDescription)).OrderBy(c => c.Id).ToList();

      // If events are already present in database, then delete the existing events.)
      if (existingEvents.Count > 1 || (existingEvents.Count == 1 && existingEvents[0].EventDescription.Equals(CalendarConstants.SubmissionsOpenColumn)))
      {
        foreach (var existingEvent in existingEvents)
        {
          CalendarRepository.Delete(existingEvent);
        }
      }

      // Derived events will also be calculated while adding new Events.
      if (eventCategory == CalendarEventCategory.ACH)
      {
        // If ICH events exists for processing period then only allow to add ACH events.
        if (IsAllowToAddACHEvents(newBillingPeriod))
        {
          AddNewEvents(record as AchCalendarRecord, newBillingPeriod);

          CreateIndependentDerivedEvents(newBillingPeriod);
        }
      }
      else
      {
        AddNewEvents(record as IchCalendarRecord, newBillingPeriod);

        CreateIndependentDerivedEvents(newBillingPeriod);
      }

      return;
    }

    /// <summary>
    /// Adds the new events for ACH calendar record.
    /// </summary>
    /// <param name="record">The record.</param>
    /// <param name="newBillingPeriod">The new billing period.</param>
    private void AddNewEvents(AchCalendarRecord record, BillingPeriod newBillingPeriod)
    {
      foreach (var propertyInfo in typeof(AchCalendarRecord).GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
      {
        var eventDateTimeSting = Convert.ToString(propertyInfo.GetValue(record, null));
        ProcessEvents(newBillingPeriod, eventDateTimeSting, _eventMap[propertyInfo.Name]);
      }
    }

    /// <summary>
    /// Adds the new events for ICH calendar record.
    /// </summary>
    /// <param name="record">The record.</param>
    /// <param name="newBillingPeriod">The new billing period.</param>
    private void AddNewEvents(IchCalendarRecord record, BillingPeriod newBillingPeriod)
    {
      foreach (var propertyInfo in typeof(IchCalendarRecord).GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
      {
        var eventDateTimeSting = Convert.ToString(propertyInfo.GetValue(record, null));
        ProcessEvents(newBillingPeriod, eventDateTimeSting, _eventMap[propertyInfo.Name]);
      }
    }

    private void CreateIndependentDerivedEvents(BillingPeriod newBillingPeriod)
    {
      // Get list of independent derived events [EventDerivedFromId is null] from database for given billing period.
      var derivedEvents = _calendarDerivedEventDetails.Where(c => c.EventDerivedFromId == 0).ToList();
      if (derivedEvents.Count <= 0)
        return;

      foreach (var derivedEvent in derivedEvents)
      {
        var eventDate = new DateTime(newBillingPeriod.Year, newBillingPeriod.Month, 1);
        CreateDerivedEvent(eventDate, derivedEvent, newBillingPeriod);
      }
    }

    private void ProcessEvents(BillingPeriod billingPeriod, string inputEventDate, int eventId)
    {
      var calendarEvent = _calendarEventDetails.Single(c => c.Id == eventId);

      DateTime eventDate;
      if (!IsValidInputDateTime(inputEventDate, calendarEvent.EventName, out eventDate))
      {
        return;
      }

      eventDate = ConvertYmqTimeToUtc(eventDate);
      CreateEvent(calendarEvent, billingPeriod, eventDate);

      var derivedEvents = _calendarDerivedEventDetails.Where(c => c.EventDerivedFromId == eventId).ToList();
      if (derivedEvents.Count <= 0)
        return;

      foreach (var derivedEvent in derivedEvents)
      {
        CreateDerivedEvent(eventDate, derivedEvent, billingPeriod);
      }
    }

    private void CreateEvent(CalendarEvent calendarEvent, BillingPeriod billingPeriod, DateTime eventDate)
    {
      CreateEvent(calendarEvent.EventDescription, calendarEvent.EventName, billingPeriod, calendarEvent.EventType, eventDate);
    }

    /// <summary>
    ///   Add event in calendarEvents collection.
    /// </summary>
    /// <param name = "name">Event name</param>
    /// <param name = "description">Event description</param>
    /// <param name = "billingPeriod">Billing period</param>
    /// <param name = "eventCategory">Event type.</param>
    /// <param name = "eventDate">Event date time.</param>
    private void CreateEvent(string name, string description, BillingPeriod billingPeriod, string eventCategory, DateTime eventDate)
    {
      // If there are validation errors then do not add or update any event.
      if (_validationErrors.Count > 0)
      {
        return;
      }

      var newEvent = new ISCalendar
      {
        Id = 0,
        Name = name,
        EventDescription = description,
        EventCategory = eventCategory,
        EventDateTime = eventDate,
        Year = billingPeriod.Year,
        Month = billingPeriod.Month,
        Period = billingPeriod.Period,
        DisplayOnHomePage = !description.Equals(CalendarConstants.ClosureOfLateSubmissionsIchColumn),
        IsActive = true,
        LastUpdatedBy = _lastUpdatedBy,
        LastUpdatedOn = DateTime.UtcNow
      };
      _calendarEvents.Add(newEvent);

      //Logger.InfoFormat(@"### {0} -{1} {2:D2}-{3:D2}-P{4} {5}", eventCategory, description, billingPeriod.Year, billingPeriod.Month, billingPeriod.Period, eventDate);
    }

    private void CreateDerivedEvent(DateTime originalDateTime, DerivedEventOffsetInfo eventOffsetInfo, BillingPeriod billingPeriod)
    {
      var offset = new TimeSpan(eventOffsetInfo.OffsetDays, eventOffsetInfo.OffsetHours, eventOffsetInfo.OffsetMinutes, eventOffsetInfo.OffsetSeconds);

      var derivedDateTime = originalDateTime;
      if (offset == TimeSpan.MinValue && eventOffsetInfo.OffsetMonths == 0)
      {
        derivedDateTime = originalDateTime;
      }
      else
      {
        if (eventOffsetInfo.EventType.Equals("F"))
        {
          // Set fixed values for months, days, hours, minutes and seconds.
          var fixedMonth = eventOffsetInfo.OffsetMonths == 0 ? derivedDateTime.Month : eventOffsetInfo.OffsetMonths;
          var fixedDay = offset.Days == 0 ? derivedDateTime.Day : offset.Days;

          derivedDateTime = new DateTime(derivedDateTime.Year, fixedMonth, fixedDay, offset.Hours, offset.Minutes, offset.Seconds);
        }
        else if (eventOffsetInfo.EventType.Equals("O"))
        {
          // Add values in offset for months, days, hours, minutes and seconds.
          derivedDateTime = derivedDateTime.Add(offset);
          if (eventOffsetInfo.OffsetMonths > 0) derivedDateTime = derivedDateTime.AddMonths(eventOffsetInfo.OffsetMonths);
        }
        else if (eventOffsetInfo.EventType.Equals("B"))
        {
          // Here first non-zero value offset set as offset and all other as fixed.
          if (eventOffsetInfo.OffsetMonths != 0)
          {
            derivedDateTime = new DateTime(derivedDateTime.Year, derivedDateTime.Month, offset.Days, offset.Hours, offset.Minutes, offset.Seconds).AddMonths(eventOffsetInfo.OffsetMonths);
          }
          else if (offset.Days != 0)
          {
            derivedDateTime = new DateTime(derivedDateTime.Year, derivedDateTime.Month, derivedDateTime.Day, offset.Hours, offset.Minutes, offset.Seconds).AddDays(offset.Days);
          }
          else if (offset.Hours != 0)
          {
            derivedDateTime = new DateTime(derivedDateTime.Year, derivedDateTime.Month, derivedDateTime.Day, derivedDateTime.Hour, offset.Minutes, offset.Seconds).AddHours(offset.Hours);
          }
          else if (offset.Minutes != 0)
          {
            derivedDateTime = new DateTime(derivedDateTime.Year, derivedDateTime.Month, derivedDateTime.Day, derivedDateTime.Hour, derivedDateTime.Minute, offset.Seconds).AddMinutes(offset.Minutes);
          }
          else if (offset.Seconds != 0)
          {
            derivedDateTime = new DateTime(derivedDateTime.Year, derivedDateTime.Month, derivedDateTime.Day, derivedDateTime.Hour, derivedDateTime.Minute, derivedDateTime.Second).AddSeconds(offset.Seconds);
          }
        }
      }

      var calendarEvent = _calendarEventDetails.Single(c => c.Id == eventOffsetInfo.Id);

      if (calendarEvent != null)
      {

        // Set Billing Period offset.
        if (eventOffsetInfo.OffsetPeriods != 0)
        {
          billingPeriod = GetBillingPeriodForOffset(eventOffsetInfo.OffsetPeriods, billingPeriod);

          // Get existing derived event for given information to recreate.
          var existingEvents =
            CalendarRepository.Get(
              c =>
              c.Year == billingPeriod.Year && c.Month == billingPeriod.Month && c.Period == billingPeriod.Period && c.EventDescription.Equals(calendarEvent.EventName)).ToList();

          // If events are already present in database, then delete the existing events.
          if (existingEvents.Count > 0)
          {
            foreach (var existingEvent in existingEvents)
            {
              CalendarRepository.Delete(existingEvent);
            }
          }
        }

        CreateEvent(calendarEvent, billingPeriod, derivedDateTime);
      }
      else
      {
        Logger.Error(string.Format(@"Calendar event information for event id '{0}' not exists in master data.", eventOffsetInfo.Id));
      }
    }

    private static BillingPeriod GetBillingPeriodForOffset(int offsetPeriods, BillingPeriod billingPeriod)
    {
      if (offsetPeriods == 0) return billingPeriod;

      if (offsetPeriods > 0)
      {
        return billingPeriod + offsetPeriods;
      }

      while (offsetPeriods < 0)
      {
        billingPeriod = billingPeriod - 1;
        offsetPeriods++;
      }
      return billingPeriod;
    }

    private CalendarEventCategory GetCalendarEventCategory(char clearanceHouseIdentifier)
    {
      switch (clearanceHouseIdentifier)
      {
        case 'A':
          return CalendarEventCategory.ACH;
        case 'I':
          return CalendarEventCategory.ICH;
        case 'F':
          if (_recordCount == 0 && _headerRowFlag)
          {
            return CalendarEventCategory.ICH;
          }
          throw new ISBusinessException(ErrorCodes.CalendarInvalidCHId);
        default:
          throw new ISBusinessException(ErrorCodes.CalendarInvalidCHId);
      }
    }

    private void InitilizeEventMap(IReflect calendarRecordType)
    {
      foreach (var propertyInfo in calendarRecordType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
      {
        var eventMapAttribute = propertyInfo.GetCustomAttributes(typeof(EventMapAttribute), false);
        if (eventMapAttribute.Length == 1)
        {
          _eventMap.Add(propertyInfo.Name, (int)((EventMapAttribute)eventMapAttribute[0]).CalendarEvents);
        }
      }
    }

    private bool IsAllowToAddACHEvents(BillingPeriod billingPeriod)
    {
      var ichEventCategory = CalendarEventCategory.ICH.ToString();
      var ichEventCount =
        CalendarRepository.GetCount(c => c.Year == billingPeriod.Year && c.Month == billingPeriod.Month && c.Period == billingPeriod.Period && (c.EventCategory.ToUpper().Equals(ichEventCategory)));

      if (ichEventCount > 0)
      {
        return true;
      }

      CreateValidationError(String.Format(Messages.CalendarICHEventsNotExists, billingPeriod.Period, billingPeriod.Month, billingPeriod.Year));
      return false;
    }

    private static void ValidateHeaderRow(string record)
    {
      var nameList = record.Split(new[] { ',' });
      var achColumnNameList = new List<string>
                                {
                                  CalendarConstants.FromClearanceHouse,
                                  CalendarConstants.CalendarMonth,
                                  CalendarConstants.Period,
                                  CalendarConstants.RecapSheetSubmissionDeadline,
                                  CalendarConstants.SettlementSheetPostingDateAchTransactions,
                                  CalendarConstants.ProtestDeadlineAchTransactions,
                                  CalendarConstants.SettlementDayAchTransactions,
                                  CalendarConstants.SettlementSheetPostingDateIataTransactions,
                                  CalendarConstants.ProtestDeadlineIataTransactions,
                                  CalendarConstants.SettlementDayIataTransactions
                                };
      var ichColumnNameList = new List<string>
                                {
                                  CalendarConstants.FromClearanceHouse,
                                  CalendarConstants.CalendarMonth,
                                  CalendarConstants.Period,
                                  CalendarConstants.ClosureDay,
                                  CalendarConstants.AdviceDay,
                                  CalendarConstants.ProtestDeadline,
                                  CalendarConstants.EarlyCallDay,
                                  CalendarConstants.CallDay,
                                  CalendarConstants.SettlementDay,
                                  CalendarConstants.SuspensionDay
                                };

      bool isAchNotValid = false, isIchNotValid = false;

      if (achColumnNameList.Count == nameList.Length)
      {
        if (nameList.Where((t, counter) => !achColumnNameList[counter].Equals(t)).Any())
        {
          isAchNotValid = true;
        }

        if (ichColumnNameList.Count == nameList.Length)
        {
          if (nameList.Where((t, counter) => !ichColumnNameList[counter].Equals(t)).Any())
          {
            isIchNotValid = true;
          }
        }
      }

      if (isAchNotValid && isIchNotValid)
      {
        throw new ISBusinessException(ErrorCodes.CalendarInputColumnMismatch);
      }
    }

    /// <summary>
    ///   Custom type selector for calendar record.
    /// </summary>
    /// <param name = "engine">MultiRecordEngine object</param>
    /// <param name = "record">record</param>
    /// <returns>type of calendar record.</returns>
    private Type CustomSelector(MultiRecordEngine engine, string record)
    {
      // Validate column count in csv file. (Both file has 10 columns means each record should contain 9 commas.)
      var count = record.Count(f => f == ',');
      if (count > 9)
      {
        throw new ISBusinessException(ErrorCodes.CalendarInputFileMoreColumns);
      }

      if (count < 9)
      {
        throw new ISBusinessException(ErrorCodes.CalendarInputFileLessColumns);
      }

      var calendarEventCategory = GetCalendarEventCategory(record[0]);

      if (_recordCount == 0 && _headerRowFlag)
      {
        ValidateHeaderRow(record);
      }

      switch (calendarEventCategory)
      {
        case CalendarEventCategory.ACH:
          return typeof(AchCalendarRecord);
        case CalendarEventCategory.ICH:
          return typeof(IchCalendarRecord);
        default:
          return typeof(CalendarRecordBase);
      }
    }

    /// <summary>
    ///   Is given period closed, current or next period.
    /// </summary>
    /// <param name = "newBillingPeriod">new billing period</param>
    /// <returns>flag indicates that given period is greater than next billing period or not.</returns>
    private bool IsPeriodLessThanNextBillingPeriod(BillingPeriod newBillingPeriod)
    {
      // Note: Check Next Billing Period only if not first time calendar upload.
      return !_isFirstTimeCalendarUpload && newBillingPeriod <= _nextBillingPeriod;
    }

    private bool IsValidInputDateTime(string inputDate, string fieldName, out DateTime eventdate)
    {
      eventdate = DateTime.MinValue;
      if (!inputDate.Length.Equals(CalendarConstants.InputDateFormat.Length) || !DateTime.TryParseExact(inputDate, CalendarConstants.InputDateFormat, null, DateTimeStyles.None, out eventdate))
      {
        CreateValidationError(String.Format(Messages.CalendarInputDateValueInvalid, fieldName));
        return false;
      }
      return true;
    }

    private bool IsValidPeriod(BillingPeriod period)
    {
      // E.g. 2010124
      var periodValue = period.Year * 1000 + period.Month * 10 + period.Period;
      if (_processedPeriods.Count > 0)
      {
        // Check for duplicate period
        if (_processedPeriods.Contains(periodValue))
        {
          CreateValidationError(String.Format(Messages.CalendarDuplicatePeriod, period.Period, period.Month, period.Year));
          return false;
        }

        // Check for missing period in given range and sequence.
        try
        {
          var previousPeriod = GetPreviousBillingPeriod(period, false);
          var previousPeriodValue = previousPeriod.Year * 1000 + previousPeriod.Month * 10 + previousPeriod.Period;
          if (_processedPeriods.Last() != previousPeriodValue)
          {
            CreateValidationError(String.Format(Messages.CalendarMissingPeriod, previousPeriod.Period, previousPeriod.Month, previousPeriod.Year));

            // Add to the list of processed periods not to give error for next period.
            _processedPeriods.Add(periodValue);
            return false;
          }
        }
        catch (ISCalendarDataNotFoundException exception)
        {
          Logger.Error(String.Format(@"Unable to get previous period for {0:D4}-{1:D2}-P{2}", period.Year, period.Month, period.Period), exception);
        }
      }
      _processedPeriods.Add(periodValue);
      return true;
    }

    private void CreateValidationError(string errorMessage)
    {
      _validationErrors.Add(new CalendarValidationError { FieldName = String.Empty, RecordNo = _recordCount, Message = errorMessage });
    }

    public static DateTime ConvertYmqTimeToUtc(DateTime ymqDateTime)
    {
      var cst = TimeZoneInfo.FindSystemTimeZoneById(SystemParameters.Instance.CalendarParameters.YmqTimeZoneName);
      return TimeZoneInfo.ConvertTimeToUtc(ymqDateTime, cst);
    }

    public static DateTime ConvertUtcTimeToYmq(DateTime utcDateTime)
    {
      var cst = TimeZoneInfo.FindSystemTimeZoneById(SystemParameters.Instance.CalendarParameters.YmqTimeZoneName);
      return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cst);
    }
      
      
   public List<int> GetCalendarYear()
      {
       var iSCalendatRepo =    Ioc.Resolve<IRepository<ISCalendar>>(typeof(IRepository<ISCalendar>));
       
       var isCalendar = iSCalendatRepo.GetAll().Select(c=>c.Year).Distinct().ToList();
       
       return isCalendar;
      }

  }
}
