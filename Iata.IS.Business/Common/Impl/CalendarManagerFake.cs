using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.AdminSystem;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Calendar;
using Iata.IS.Data.Impl;
using Iata.IS.Model;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.Enums;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class CalendarManagerFake : ICalendarManager
  {
    private const string InputDateFormat = "yyMMddHHmm";

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly List<ISCalendar> _calendarEvents = new List<ISCalendar>();
    private readonly IRepository<ISCalendar> _calendarRepository;
    private readonly List<int> _processedPeriods = new List<int>();

    /// <summary>
    /// allowed types for calendar records.
    /// </summary>
    private readonly Type[] _recordTypes = new[] { typeof(AchCalendarRecord), typeof(IchCalendarRecord) };

    private readonly List<CalendarValidationError> _validationErrors = new List<CalendarValidationError>();
    private bool _headerRowFlag;
    private bool _isIgnoreStatus;
    private int _lastUpdatedBy;
    private int _recordCount;

    public CalendarManagerFake(IRepository<ISCalendar> calendarRepository)
    {
      _calendarRepository = calendarRepository;
    }

    public ICalendarRepository CalendarRepository { get; set; }

    #region ICalendarManager Members

    public IList<CalendarSearchResult> SearchCalendarEvents(int year, int month, int period)
    {
      return CalendarRepository.SearchCalendarData(year, month, period);
    }

    /// <summary>
    /// Upload/update the calendar schedules from clearing houses
    /// </summary>
    /// <param name="filePath">input csv file path.</param>
    /// <param name="isIgnoreStatus">flag indicate that ignore previously closed periods, current and next period.</param>
    /// <param name="headerRowFlag">header row flag,
    ///   1 - file contains header row
    ///   2 - file doesn't contains header row.
    /// </param>
    /// <param name="userId">Id of user uploading the calendar.</param>
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
        var fromClearanceHouseId = string.Empty;

        var record = (CalendarRecordBase)multiRecordEngine.ReadNext();

        _recordCount = 1;
        if (_headerRowFlag)
        {
          record = (CalendarRecordBase)multiRecordEngine.ReadNext();
        }

        // Process each record - till the end of the calendar file
        while (record != null)
        {
          if (string.IsNullOrEmpty(fromClearanceHouseId))
          {
            fromClearanceHouseId = record.FromClearanceHouseIdentifier;
          }

          // If file contains different FromClearanceHouseIdentifier then throw exception.
          if (!fromClearanceHouseId.Equals(record.FromClearanceHouseIdentifier))
          {
            throw new ISBusinessException(ErrorCodes.CalendarFileContainsBothCHInfo);
          }

          // Call ProcessCalendarRecord based on record type.
          ProcessCalendarRecord(record);

          _recordCount++;
          record = (CalendarRecordBase)multiRecordEngine.ReadNext();
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

      // If there are no errors and events to update on database then perform database operations.
      if (_validationErrors.Count <= 0 && _calendarEvents.Count > 0)
      {
        foreach (var calendarEvent in _calendarEvents)
        {
          if (calendarEvent.Id <= 0)
          {
            _calendarRepository.Add(calendarEvent);
          }
          else
          {
            _calendarRepository.Update(calendarEvent);
          }
        }
        UnitOfWork.CommitDefault();
      }

      return _validationErrors;
    }

    /// <summary>
    /// This method will return list of billing period objects which contain list of year, month and period values for which billing is allowed
    /// </summary>
    /// <param name="memberId">ID of member who is creating invoices</param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    public List<BillingPeriod> GetRelevantBillingPeriods(string memberId, ClearingHouse clearingHouse, int x = 4)
    {
      var currentPeriod = GetCurrentBillingPeriod();

      var futurePeriod = new BillingPeriod();

      if (currentPeriod.Period == 4)
      {
        if (currentPeriod.Month == 12)
        {
          futurePeriod.Year = currentPeriod.Year + 1;
          futurePeriod.Month = 1;
        }
        else
        {
          futurePeriod.Year = currentPeriod.Year;
          futurePeriod.Month = currentPeriod.Month + 1;
        }
        futurePeriod.Period = 1;
      }
      else
      {
        futurePeriod.Period = currentPeriod.Period + 1;
        futurePeriod.Month = currentPeriod.Month;
        futurePeriod.Year = currentPeriod.Year;
      }

      var lateSubmissionPeriod = new BillingPeriod();

      if (currentPeriod.Period == 1)
      {
        if (currentPeriod.Month == 1)
        {
          lateSubmissionPeriod.Year = currentPeriod.Year - 1;
          lateSubmissionPeriod.Month = 12;
        }
        else
        {
          lateSubmissionPeriod.Year = currentPeriod.Year;
          lateSubmissionPeriod.Month = currentPeriod.Month - 1;
        }
        lateSubmissionPeriod.Period = 4;
      }
      else
      {
        lateSubmissionPeriod.Year = currentPeriod.Year;
        lateSubmissionPeriod.Month = currentPeriod.Month;
        lateSubmissionPeriod.Period = currentPeriod.Period - 1;
      }

      var lstBillingPeriods = new List<BillingPeriod>
                                {
                                  lateSubmissionPeriod,
                                  currentPeriod,
                                  futurePeriod
                                };

      return (lstBillingPeriods);
    }

    /// <summary>
    /// Gets the last closed billing period.
    /// </summary>
    /// <returns></returns>
    public BillingPeriod GetLastClosedBillingPeriod(ClearingHouse clearingHouse = ClearingHouse.Ich)
    {
      var currentPeriod = GetCurrentBillingPeriod();
      var lastClosedBillingPeriod = new BillingPeriod();
      if (currentPeriod.Period == 1)
      {
        if (currentPeriod.Month == 1)
        {
          lastClosedBillingPeriod.Year = currentPeriod.Year - 1;
          lastClosedBillingPeriod.Month = 12;
        }
        else
        {
          lastClosedBillingPeriod.Year = currentPeriod.Year;
          lastClosedBillingPeriod.Month = currentPeriod.Month - 1;
        }
        lastClosedBillingPeriod.Period = 4;
      }
      else
      {
        lastClosedBillingPeriod.Year = currentPeriod.Year;
        lastClosedBillingPeriod.Month = currentPeriod.Month;
        lastClosedBillingPeriod.Period = currentPeriod.Period - 1;
      }

      return lastClosedBillingPeriod;
    }

    /// <summary>
    /// This method is used for retrieving previous billing period based on clearing house passed and date a week ago
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <param name="clearingHouse"></param>
    /// <returns>Previous Period value</returns>
    public BillingPeriod GetPreviousBillingPeriod(ClearingHouse clearingHouse)
    {
      var billingPeriod = GetPreviousBillingPeriod();
      billingPeriod.ClearingHouse = clearingHouse;

      return billingPeriod;
    }

    /// <summary>
    /// This method is used for retrieving current open billing period based on today's utc date time.
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <returns>Current Period value</returns>
    public BillingPeriod GetCurrentBillingPeriod()
    {
      var utcNow = DateTime.UtcNow;
      var year = utcNow.Year;
      var month = utcNow.Month;
      var billingPeriod = new BillingPeriod
                            {
                              Month = month,
                              Year = year
                            };
      var todaysDate = utcNow.Date;

      try
      {
        if ((todaysDate.Day >= 7) && (todaysDate.Day <= 13))
        {
          billingPeriod.Period = 1;
          billingPeriod.StartDate = new DateTime(year, month, 7);
          billingPeriod.EndDate = new DateTime(year, month, 13);
        }
        else if ((todaysDate.Day >= 14) && (todaysDate.Day <= 20))
        {
          billingPeriod.Period = 2;
          billingPeriod.StartDate = new DateTime(year, month, 14);
          billingPeriod.EndDate = new DateTime(year, month, 20);
        }
        else if ((todaysDate.Day >= 21) && (todaysDate.Day <= 27))
        {
          billingPeriod.Period = 3;
          billingPeriod.StartDate = new DateTime(year, month, 21);
          billingPeriod.EndDate = new DateTime(year, month, 27);
        }
        else
        {
          billingPeriod.Period = 4;

          if ((todaysDate.Day >= 1) && (todaysDate.Day <= 6))
          {
            billingPeriod.Month = month == 1 ? 12 : month - 1;
            billingPeriod.StartDate = new DateTime(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1, 28);
            billingPeriod.EndDate = new DateTime(year, month, 6);
            if (month == 1)
            {
              billingPeriod.Year -= 1;
            }
          }
          else
          {
            billingPeriod.StartDate = new DateTime(year, month, 28);
            billingPeriod.EndDate = new DateTime(month == 12 ? year + 1 : year, month == 12 ? 1 : month + 1, 6);
          }
        }
      }
      catch (Exception)
      {
        throw new ISDataException("PeriodError1", "Could not retrieve current period");
      }

      return billingPeriod;
    }

    /// <summary>
    /// This method is used for retrieving current open billing period based on today's date
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <param name="inputDate">The input date.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Current Period value
    /// </returns>
    public BillingPeriod GetBillingPeriod(DateTime inputDate, ClearingHouse clearingHouse = ClearingHouse.Ich)
    {
      var year = inputDate.ToUniversalTime().Year;
      var month = inputDate.ToUniversalTime().Month;
      var billingPeriod = new BillingPeriod
                            {
                              Month = month,
                              Year = year,
                              ClearingHouse = clearingHouse
                            };
      var todaysDate = inputDate.Date;

      try
      {
        if ((todaysDate.Day >= 7) && (todaysDate.Day <= 13))
        {
          billingPeriod.Period = 1;
          billingPeriod.StartDate = new DateTime(year, month, 7);
          billingPeriod.EndDate = new DateTime(year, month, 13);
        }
        else if ((todaysDate.Day >= 14) && (todaysDate.Day <= 20))
        {
          billingPeriod.Period = 2;
          billingPeriod.StartDate = new DateTime(year, month, 14);
          billingPeriod.EndDate = new DateTime(year, month, 20);
        }
        else if ((todaysDate.Day >= 21) && (todaysDate.Day <= 27))
        {
          billingPeriod.Period = 3;
          billingPeriod.StartDate = new DateTime(year, month, 21);
          billingPeriod.EndDate = new DateTime(year, month, 27);
        }
        else
        {
          billingPeriod.Period = 4;

          if ((todaysDate.Day >= 1) && (todaysDate.Day <= 6))
          {
            billingPeriod.Month = month == 1 ? 12 : month - 1;
            billingPeriod.StartDate = new DateTime(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1, 28);
            billingPeriod.EndDate = new DateTime(year, month, 6);
            if (month == 1)
            {
              billingPeriod.Year -= 1;
            }
          }
          else
          {
            billingPeriod.StartDate = new DateTime(year, month, 28);
            billingPeriod.EndDate = new DateTime(month == 12 ? year + 1 : year, month == 12 ? 1 : month + 1, 6);
          }
        }
      }
      catch (Exception)
      {
        throw new ISDataException("PeriodError1", "Could not retrieve current period");
      }

      return billingPeriod;
    }

    public BillingPeriod GetCurrentBillingPeriod(ClearingHouse clearingHouse)
    {
      var billingPeriod = GetCurrentBillingPeriod();
      billingPeriod.ClearingHouse = clearingHouse;

      return billingPeriod;
    }

    /// <summary>
    /// This method is used for retrieving billing period based on given inputs.
    /// </summary>
    /// <param name="clearingHouse"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="period"></param>
    /// <returns>billing period</returns>
    public BillingPeriod GetBillingPeriod(ClearingHouse clearingHouse, int year, int month, int period)
    {
      var billingPeriod = new BillingPeriod(year, month, period)
                            {
                              ClearingHouse = clearingHouse
                            };
      return billingPeriod;
    }

    /// <summary>
    /// This method is used for retrieving next billing period based on clearing house passed and date a next week
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <param name="clearingHouse"></param>
    /// <returns>Previous Period value</returns>
    public BillingPeriod GetNextBillingPeriod(ClearingHouse clearingHouse)
    {
      var billingPeriod = GetNextBillingPeriod();
      billingPeriod.ClearingHouse = clearingHouse;

      return billingPeriod;
    }

    public List<BillingPeriod> GetSuspensionBillingPeriods(ClearingHouse clearingHouse, bool includeCurrentPeriod)
    {
      BillingPeriod billingPeriodPrevious1;
      var billingPeriod = GetCurrentBillingPeriod();

      var lstBillingPeriods = new List<BillingPeriod>();

      var billingPeriodCurrent = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period);

      if (includeCurrentPeriod)
      {
        lstBillingPeriods.Add(billingPeriodCurrent);

        if (billingPeriod.Period == 1)
        {
          billingPeriodPrevious1 = billingPeriod.Month == 1
                                     ? new BillingPeriod(billingPeriod.Year - 1, 12, billingPeriod.Period = 4)
                                     : new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, billingPeriod.Period = 4);
          lstBillingPeriods.Add(billingPeriodPrevious1);
        }
        else if (billingPeriod.Period == 2)
        {
          billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          lstBillingPeriods.Add(billingPeriodPrevious1);
        }
        else if (billingPeriod.Period == 3)
        {
          billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          lstBillingPeriods.Add(billingPeriodPrevious1);
        }
        else if (billingPeriod.Period == 4)
        {
          billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          lstBillingPeriods.Add(billingPeriodPrevious1);
        }
      }

      else
      {
        if (billingPeriod.Period == 1)
        {
          //     billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, billingPeriod.Period = 4);
          billingPeriodPrevious1 = billingPeriod.Month == 1
                                     ? new BillingPeriod(billingPeriod.Year - 1, 12, billingPeriod.Period = 4)
                                     : new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, billingPeriod.Period = 4);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, billingPeriod.Period = 4 - 1);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
        }
        else if (billingPeriod.Period == 2)
        {
          billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, billingPeriod.Period = 4);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
        }
        else if (billingPeriod.Period == 3)
        {
          billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 2);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
        }
        else if (billingPeriod.Period == 4)
        {
          billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 2);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
        }
      }
      return (lstBillingPeriods);
    }

    /// <summary>
    /// Get Default suspension billing period (returns  previous 6 billing periods)
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="includeCurrentPeriod">if set to <c>true</c> [include current period].</param>
    /// <param name="previousPeriodsToInclude"></param>
    /// <returns></returns>
    public List<BillingPeriod> GetDefaultSuspensionPeriods(ClearingHouse clearingHouse, bool includeCurrentPeriod, int previousPeriodsToInclude)
    {
      var billingPeriod = GetCurrentBillingPeriod(clearingHouse);

      var lstBillingPeriods = new List<BillingPeriod>();

      var billingPeriodCurrent = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period);

      if (includeCurrentPeriod)
      {
        lstBillingPeriods.Add(billingPeriodCurrent);

        if (billingPeriod.Month == 1)
        {
          billingPeriod.Year = billingPeriod.Year - 1;
          billingPeriod.Month = 12;
        }

        if (billingPeriod.Period == 1)
        {
          var billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 4);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 3);
          var billingPeriodPrevious3 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 2);
          var billingPeriodPrevious4 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 1);
          var billingPeriodPrevious5 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 4);

          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
          lstBillingPeriods.Add(billingPeriodPrevious3);
          lstBillingPeriods.Add(billingPeriodPrevious4);
          lstBillingPeriods.Add(billingPeriodPrevious5);
        }
        else if (billingPeriod.Period == 2)
        {
          var billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period - 1);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 4);
          var billingPeriodPrevious3 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 3);
          var billingPeriodPrevious4 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 2);
          var billingPeriodPrevious5 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 1);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
          lstBillingPeriods.Add(billingPeriodPrevious3);
          lstBillingPeriods.Add(billingPeriodPrevious4);
          lstBillingPeriods.Add(billingPeriodPrevious5);

          lstBillingPeriods.Add(billingPeriodPrevious1);
        }
        else if (billingPeriod.Period == 3)
        {
          var billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 2);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 1);
          var billingPeriodPrevious3 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 4);
          var billingPeriodPrevious4 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 3);
          var billingPeriodPrevious5 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 2);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
          lstBillingPeriods.Add(billingPeriodPrevious3);
          lstBillingPeriods.Add(billingPeriodPrevious4);
          lstBillingPeriods.Add(billingPeriodPrevious5);
        }
        else if (billingPeriod.Period == 4)
        {
          var billingPeriodPrevious1 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 3);
          var billingPeriodPrevious2 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 2);
          var billingPeriodPrevious3 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month, 1);
          var billingPeriodPrevious4 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 4);
          var billingPeriodPrevious5 = new BillingPeriod(billingPeriod.Year, billingPeriod.Month - 1, 3);
          lstBillingPeriods.Add(billingPeriodPrevious1);
          lstBillingPeriods.Add(billingPeriodPrevious2);
          lstBillingPeriods.Add(billingPeriodPrevious3);
          lstBillingPeriods.Add(billingPeriodPrevious4);
          lstBillingPeriods.Add(billingPeriodPrevious5);
        }
      }

      return (lstBillingPeriods);
    }

    /// <summary>
    /// This method takes the clearing house and returns if its late submission window is open or not
    /// </summary>
    /// <param name="clearingHouse">Clearing House</param>
    /// <returns>a flag indicating if the late submission window is open or not</returns>
    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse)
    {
      try
      {
        var utcDateNow = DateTime.UtcNow;

        if ((utcDateNow.Day >= 14 && utcDateNow.Day <= 15) || (utcDateNow.Day >= 21 && utcDateNow.Day <= 22) || (utcDateNow.Day >= 28 && utcDateNow.Day <= 29))
        {
          return true;
        }
      }
      catch (Exception)
      {
        throw new ISDataException("PeriodError1", "Could not retrieve Late Submission Window Status");
      }
      return false;
    }

    /// <summary>
    /// This method is used for retrieving Late Submission Start and End Date Times for a clearing house and billing period
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <returns>Next Period value</returns>
    public TimeInterval GetLateSubmissionWindow(ClearingHouse clearingHouse, BillingPeriod billingPeriod)
    {
      var timeInterval = new TimeInterval
                           {
                             Start = billingPeriod.EndDate,
                             End = billingPeriod.EndDate.AddHours(3)
                           };
      return timeInterval;
    }

    #endregion

    /// <summary>
    /// This method checks if future submission is allowed for the given future period.
    /// </summary>
    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    public bool IsFutureSubmissionOpen(BillingPeriod billingPeriod)
    {
      var futureSubmissionRecord = CalendarRepository.First(c => (CalendarConstants.FutureDatedSubmissionsOpenColumn.Equals(c.EventDescription)) && c.Year == billingPeriod.Year &&
        c.Month == billingPeriod.Month && c.Period == billingPeriod.Period);
      if (futureSubmissionRecord == null) return false;

      if (DateTime.UtcNow >= futureSubmissionRecord.EventDateTime)
        return true;

      return false;
    }

    /// <summary>
    /// Process ach calendar record.
    /// </summary>
    /// <param name="record">ach calendar record object.</param>
    private void ProcessCalendarRecord<T>(T record) where T : CalendarRecordBase
    {

      // Validate Period value in record.
      int period;
      if (!Int32.TryParse(record.Period, out period) || period < 1 || period > 4)
      {
        CreateValidationError(string.Format(Messages.CalendarPeriodValueInvalid, "Period"));
        return;
      }

      // Validate CalendarMonth value in record.
      DateTime calendarMonth;
      if (!DateTime.TryParseExact(record.CalendarMonth, "yyMM", null, DateTimeStyles.None, out calendarMonth))
      {
        CreateValidationError(string.Format(Messages.CalendarValueInvalid, "CalendarMonth"));
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
        derivedEventCategory = CalendarEventCategory.ISACH.ToString().ToUpper();
      }
      else
      {
        eventCategory = CalendarEventCategory.ICH;
        derivedEventCategory = CalendarEventCategory.ISICH.ToString().ToUpper();
      }

      // Get list of existing events from database for the given event type
      var calendarEventCategory = Convert.ToString(eventCategory).ToUpper();
      var existingEvents =
        _calendarRepository.Get(
          c =>
          c.Year == calendarMonth.Year && c.Month == calendarMonth.Month && c.Period == period &&
          (c.EventCategory.ToUpper().Equals(calendarEventCategory) || c.EventCategory.ToUpper().Equals(derivedEventCategory))).OrderBy(c => c.Id).ToList();

      if (IsPeriodClosedOrCurrentOrNext(newBillingPeriod))
      {
        // if IgnoreStatus flag false then check the event date time change for closed, current open and next periods.
        if (!_isIgnoreStatus)
        {
          if (existingEvents.Count <= 0)
          {
            CreateValidationError(string.Format(Messages.CalendarEventsNotExists, period, calendarMonth.Month, calendarMonth.Year, eventCategory));
          }
          else
            if (eventCategory == CalendarEventCategory.ACH)
            {
              HasValuesChanged(record as AchCalendarRecord, existingEvents);
            }
            else
            {
              HasValuesChanged(record as IchCalendarRecord, existingEvents);
            }
        }

        // if no change in values then go for next record.
        return;
      }

      // If events are already present in database, then 
      // update the existing events.
      if (existingEvents.Count > 0)
      {
        // In update method, derived events will also be recalculated.
        if (eventCategory == CalendarEventCategory.ACH)
        {
          UpdateExistingEvents(record as AchCalendarRecord, existingEvents);
        }
        else
        {
          UpdateExistingEvents(record as IchCalendarRecord, existingEvents, newBillingPeriod);
        }
      }
      else // else, add new events..
      {
        // Derived events will also be calculated while adding new Events.
        if (eventCategory == CalendarEventCategory.ACH)
        {
          // If ICH events exists for processing period then only allow to add ACH events.
          if (IsAllowToAddACHEvents(newBillingPeriod))
            AddNewEvents(record as AchCalendarRecord, newBillingPeriod);
        }
        else
        {
          AddNewEvents(record as IchCalendarRecord, newBillingPeriod);
        }
      }

      return;
    }

    private void HasValuesChanged(AchCalendarRecord record, IEnumerable<ISCalendar> existingEvents)
    {
      foreach (var existingEvent in existingEvents)
      {
        DateTime eventDate;
        switch (existingEvent.Name)
        {
          case CalendarConstants.RecapSheetSubmissionDeadline:
            if (IsValidInputDateTime(record.RecapSheetSubmissionDeadline, CalendarConstants.ClosureDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.SettlementSheetPostingDateAchTransactions:
            if (IsValidInputDateTime(record.SettlementSheetPostingDateAchtransactions, CalendarConstants.AdviceDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.ProtestDeadlineAchTransactions:
            if (IsValidInputDateTime(record.ProtestDeadlineAchtransactions, CalendarConstants.ProtestDeadline, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.SettlementDayAchTransactions:
            if (IsValidInputDateTime(record.SettlementDayAchtransactions, CalendarConstants.EarlyCallDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.SettlementSheetPostingDateIataTransactions:
            if (IsValidInputDateTime(record.SettlementSheetPostingDateIatatransactions, CalendarConstants.CallDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.ProtestDeadlineIataTransactions:
            if (IsValidInputDateTime(record.ProtestDeadlineIatatransactions, CalendarConstants.SettlementDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.SettlementDayIataTransactions:
            if (IsValidInputDateTime(record.SettlementDayIatatransactions, CalendarConstants.SuspensionDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
        }
      }
    }

    private void AddNewEvents(AchCalendarRecord record, BillingPeriod newBillingPeriod)
    {
      var eventCategory = CalendarEventCategory.ACH.ToString();

      // Add Recap Sheet Submission Deadline event
      ProcessEvent(newBillingPeriod, record.RecapSheetSubmissionDeadline, CalendarConstants.RecapSheetSubmissionDeadline, CalendarConstants.RecapSheetSubmissionDeadlineColumn, eventCategory);

      // Add Settlement Sheet Posting Date (ACH transactions) event
      ProcessEvent(newBillingPeriod,
                   record.SettlementSheetPostingDateAchtransactions,
                   CalendarConstants.SettlementSheetPostingDateAchTransactions,
                   CalendarConstants.SettlementSheetPostingDateAchTransactionsColumn,
                   eventCategory);

      // Add Protest Deadline (ACH transactions) event
      ProcessEvent(newBillingPeriod, record.ProtestDeadlineAchtransactions, CalendarConstants.ProtestDeadlineAchTransactions, CalendarConstants.ProtestDeadlineAchTransactionsColumn, eventCategory);

      // Add Settlement Day (ACH transactions) event
      ProcessEvent(newBillingPeriod, record.SettlementDayAchtransactions, CalendarConstants.SettlementDayAchTransactions, CalendarConstants.SettlementDayAchTransactionsColumn, eventCategory);

      // Add Settlement Sheet Posting Date (IATA transactions) event
      ProcessEvent(newBillingPeriod,
                   record.SettlementSheetPostingDateIatatransactions,
                   CalendarConstants.SettlementSheetPostingDateIataTransactions,
                   CalendarConstants.SettlementSheetPostingDateIataTransactionsColumn,
                   eventCategory);

      // Add Protest Deadline (IATA transactions) event
      ProcessEvent(newBillingPeriod, record.ProtestDeadlineIatatransactions, CalendarConstants.ProtestDeadlineIataTransactions, CalendarConstants.ProtestDeadlineIataTransactionsColumn, eventCategory);

      // Add Settlement Day (IATA transactions) event
      ProcessEvent(newBillingPeriod, record.SettlementDayIatatransactions, CalendarConstants.SettlementDayIataTransactions, CalendarConstants.SettlementDayIataTransactionsColumn, eventCategory);
    }

    private void UpdateExistingEvents(AchCalendarRecord record, IEnumerable<ISCalendar> existingEvents)
    {
      DateTime recapSheetSubmissionDeadlineDay;

      if (IsValidInputDateTime(record.RecapSheetSubmissionDeadline, CalendarConstants.ClosureDay, out recapSheetSubmissionDeadlineDay))
      {
        recapSheetSubmissionDeadlineDay = CalendarManager.ConvertYmqTimeToUtc(recapSheetSubmissionDeadlineDay);
      }

      foreach (var existingEvent in existingEvents)
      {
        DateTime eventDateTime;
        switch (existingEvent.Name)
        {
          case CalendarConstants.RecapSheetSubmissionDeadline:
            if (recapSheetSubmissionDeadlineDay != DateTime.MinValue)
            {
              if (existingEvent.EventDateTime != recapSheetSubmissionDeadlineDay)
              {
                UpdateEvent(existingEvent, recapSheetSubmissionDeadlineDay);
              }
            }
            break;
          case CalendarConstants.SettlementSheetPostingDateAchTransactions:
            if (IsValidInputDateTime(record.SettlementSheetPostingDateAchtransactions, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.ProtestDeadlineAchTransactions:
            if (IsValidInputDateTime(record.ProtestDeadlineAchtransactions, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.SettlementDayAchTransactions:
            if (IsValidInputDateTime(record.SettlementDayAchtransactions, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.SettlementSheetPostingDateIataTransactions:
            if (IsValidInputDateTime(record.SettlementSheetPostingDateIatatransactions, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.ProtestDeadlineIataTransactions:
            if (IsValidInputDateTime(record.ProtestDeadlineIatatransactions, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.SettlementDayIataTransactions:
            if (IsValidInputDateTime(record.SettlementDayIatatransactions, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          //
          // ACH Derived events.
          case CalendarConstants.SubmissionDeadlineForAchInvoices:
            if (recapSheetSubmissionDeadlineDay != DateTime.MinValue)
            {
              UpdateEvent(existingEvent, recapSheetSubmissionDeadlineDay);
            }
            break;
          case CalendarConstants.ClosureOfLateSubmissionsAch:
            // Add Closure of Late Submissions for Ach event
            if (SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission.Equals("No", StringComparison.OrdinalIgnoreCase))
            {
              UpdateEvent(existingEvent, recapSheetSubmissionDeadlineDay.Add(SystemParameters.Instance.CalendarParameters.ClosureOfLateSubmissionsACHOffset));
            }
            break;
        }
      }
    }

    /// <summary>
    /// Check there is change in event date time of previously closed periods, current open period or immediate next period. (Only for ICH events)
    /// </summary>
    /// <param name="record"></param>
    /// <param name="existingEvents"></param>
    private void HasValuesChanged(IchCalendarRecord record, IEnumerable<ISCalendar> existingEvents)
    {
      foreach (var existingEvent in existingEvents)
      {
        DateTime eventDate;
        switch (existingEvent.Name)
        {
          case CalendarConstants.ClosureDay:
            if (IsValidInputDateTime(record.ClosureDay, CalendarConstants.ClosureDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.AdviceDay:
            if (IsValidInputDateTime(record.AdviceDay, CalendarConstants.AdviceDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.ProtestDeadline:
            if (IsValidInputDateTime(record.ProtestDeadline, CalendarConstants.ProtestDeadline, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.EarlyCallDay:
            if (IsValidInputDateTime(record.EarlyCallDay, CalendarConstants.EarlyCallDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.CallDay:
            if (IsValidInputDateTime(record.CallDay, CalendarConstants.CallDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.SettlementDay:
            if (IsValidInputDateTime(record.SettlementDay, CalendarConstants.SettlementDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
          case CalendarConstants.SuspensionDay:
            if (IsValidInputDateTime(record.SuspensionDay, CalendarConstants.SuspensionDay, out eventDate))
            {
              if (existingEvent.EventDateTime != CalendarManager.ConvertYmqTimeToUtc(eventDate))
              {
                CreateValidationError(string.Format(Messages.CalendarEventDateChanged, existingEvent.Name));
              }
            }
            break;
        }
      }
    }

    /// <summary>
    /// Add new calendar events.
    /// </summary>
    /// <param name="record">ich calendar record.</param>
    /// <param name="newBillingPeriod">billing period for this record.</param>
    private void AddNewEvents(IchCalendarRecord record, BillingPeriod newBillingPeriod)
    {
      var eventCategory = CalendarEventCategory.ICH.ToString();

      // Add ClosureDay event and related derived events
      ProcessEvent(newBillingPeriod, record.ClosureDay, CalendarConstants.ClosureDay, CalendarConstants.ClosureDayColumn, eventCategory);

      // Add AdviceDay event and related derived events
      ProcessEvent(newBillingPeriod, record.AdviceDay, CalendarConstants.AdviceDay, CalendarConstants.AdviceDayColumn, eventCategory);

      // Add ProtestDeadline event
      ProcessEvent(newBillingPeriod, record.ProtestDeadline, CalendarConstants.ProtestDeadline, CalendarConstants.ProtestDeadlineColumn, eventCategory);

      // Add EarlyCallDay event
      ProcessEvent(newBillingPeriod, record.EarlyCallDay, CalendarConstants.EarlyCallDay, CalendarConstants.EarlyCallDayColumn, eventCategory);

      // Add CallDay event
      ProcessEvent(newBillingPeriod, record.CallDay, CalendarConstants.CallDay, CalendarConstants.CallDayColumn, eventCategory);

      // Add SettlementDay event
      ProcessEvent(newBillingPeriod, record.SettlementDay, CalendarConstants.SettlementDay, CalendarConstants.SettlementDayColumn, eventCategory);

      // Add SuspensionDay event
      ProcessEvent(newBillingPeriod, record.SuspensionDay, CalendarConstants.SuspensionDay, CalendarConstants.SuspensionDayColumn, eventCategory);
    }

    private void ProcessEvent(BillingPeriod billingPeriod, string inputEventDate, string eventName, string eventColumnName, string eventCategory)
    {
      var derivedEventCategory = CalendarEventCategory.ISACH.ToString().ToUpper();
      if (CalendarEventCategory.ICH.ToString().Equals(eventCategory))
        derivedEventCategory = CalendarEventCategory.ISICH.ToString().ToUpper();

      DateTime eventDate;
      if (!IsValidInputDateTime(inputEventDate, eventName, out eventDate))
      {
        return;
      }

      eventDate = CalendarManager.ConvertYmqTimeToUtc(eventDate);
      CreateEvent(eventName, eventColumnName, billingPeriod, eventCategory, eventDate);

      if (eventName.Equals(CalendarConstants.ClosureDay))
      {
        var closureDate = eventDate.Date;

        // Add Submissions Open event for next period
        var nextPeriod = billingPeriod + 1;
        CreateEvent(CalendarConstants.SubmissionsOpen,
                    CalendarConstants.SubmissionsOpenColumn,
                    nextPeriod,
                    derivedEventCategory,
                    closureDate.Add(SystemParameters.Instance.CalendarParameters.SubmissionsOpenOffset));

        // Add Submissions Open (Future dated submissions) event
        var offset = SystemParameters.Instance.CalendarParameters.FutureDatedSubmissionsOpenOffset;
        var futureDatedSubmissionOpenDate = new DateTime(billingPeriod.Year, billingPeriod.Month, offset.Days, offset.Hours, offset.Minutes, offset.Seconds);
        futureDatedSubmissionOpenDate = futureDatedSubmissionOpenDate.AddMonths(-1);
        CreateEvent(CalendarConstants.FutureDatedSubmissionsOpen, CalendarConstants.FutureDatedSubmissionsOpenColumn, billingPeriod, derivedEventCategory, futureDatedSubmissionOpenDate);

        // Add Submissions Deadline ICH and Bilateral Invoices event
        CreateEvent(CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoices,
                    CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoicesColumn,
                    billingPeriod,
                    derivedEventCategory,
                    eventDate);
      }
      else if (eventName.Equals(CalendarConstants.AdviceDay))
      {
        var adviceDate = eventDate.Date;

        // Add Supporting Attachment Linking Deadline event
        var supportingAttachmentLinkingDeadlineDate = adviceDate.Add(SystemParameters.Instance.CalendarParameters.SupportingAttachmentLinkingDeadlineOffset);
        CreateEvent(CalendarConstants.SupportingDocumentsLinkingDeadline,
                    CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                    billingPeriod,
                    derivedEventCategory,
                    supportingAttachmentLinkingDeadlineDate);

        // Add Billing Output Generation event
        CreateEvent(CalendarConstants.BillingOutputGeneration,
                    CalendarConstants.BillingOutputGenerationColumn,
                    billingPeriod,
                    derivedEventCategory,
                    supportingAttachmentLinkingDeadlineDate.AddHours(SystemParameters.Instance.CalendarParameters.BillingOutputGenerationHoursOffset));

        // Add Closure of Late Submissions for Ich event
        if (SystemParameters.Instance.CalendarParameters.ManualControlOnLateSubmissionsICH.Equals("No", StringComparison.OrdinalIgnoreCase))
        {
          CreateEvent(CalendarConstants.ClosureOfLateSubmissionsIch, CalendarConstants.ClosureOfLateSubmissionsIchColumn, billingPeriod, derivedEventCategory, eventDate);
        }
      }
      if (!eventName.Equals(CalendarConstants.RecapSheetSubmissionDeadline))
      {
        return;
      }
      // Add Submission Deadline for ACH Invoices  event
      CreateEvent(CalendarConstants.SubmissionDeadlineForAchInvoices, CalendarConstants.SubmissionDeadlineForAchInvoicesColumn, billingPeriod, derivedEventCategory, eventDate);

      // Add Closure of Late Submissions for Ach event
      if (SystemParameters.Instance.CalendarParameters.ManualControlOnLateSubmissionsACH.Equals("No", StringComparison.OrdinalIgnoreCase))
      {
        CreateEvent(CalendarConstants.ClosureOfLateSubmissionsAch,
                    CalendarConstants.ClosureOfLateSubmissionsAchColumn,
                    billingPeriod,
                    derivedEventCategory,
                    eventDate.Add(SystemParameters.Instance.CalendarParameters.ClosureOfLateSubmissionsACHOffset));
      }
    }

    /// <summary>
    /// Update existing event if the event date time values are different. (Only for ICH and derived events)
    /// </summary>
    /// <param name="record">ich calendar record.</param>
    /// <param name="existingEvents">list of existing events for this period.</param>
    /// <param name="billingPeriod">billing period for this record.</param>
    private void UpdateExistingEvents(IchCalendarRecord record, IEnumerable<ISCalendar> existingEvents, BillingPeriod billingPeriod)
    {
      DateTime closureDay, adviceDay;
      DateTime closureDate = DateTime.MinValue, adviceDate = DateTime.MinValue;

      if (IsValidInputDateTime(record.ClosureDay, CalendarConstants.ClosureDay, out closureDay))
      {
        closureDay = CalendarManager.ConvertYmqTimeToUtc(closureDay);
        closureDate = closureDay.Date;
      }
      if (IsValidInputDateTime(record.AdviceDay, CalendarConstants.AdviceDay, out adviceDay))
      {
        adviceDay = CalendarManager.ConvertYmqTimeToUtc(adviceDay);
        adviceDate = adviceDay.Date;
      }

      foreach (var existingEvent in existingEvents)
      {
        DateTime eventDateTime;
        switch (existingEvent.Name)
        {
          case CalendarConstants.ClosureDay:
            if (closureDay != DateTime.MinValue)
            {
              if (existingEvent.EventDateTime != closureDay)
              {
                UpdateEvent(existingEvent, closureDay);
              }
            }
            break;
          case CalendarConstants.AdviceDay:
            if (adviceDay != DateTime.MinValue)
            {
              if (existingEvent.EventDateTime != adviceDay)
              {
                UpdateEvent(existingEvent, adviceDay);
              }
            }
            break;
          case CalendarConstants.ProtestDeadline:
            if (IsValidInputDateTime(record.ProtestDeadline, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.EarlyCallDay:
            if (IsValidInputDateTime(record.EarlyCallDay, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.CallDay:
            if (IsValidInputDateTime(record.CallDay, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.SettlementDay:
            if (IsValidInputDateTime(record.SettlementDay, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.SuspensionDay:
            if (IsValidInputDateTime(record.SuspensionDay, existingEvent.Name, out eventDateTime))
            {
              eventDateTime = CalendarManager.ConvertYmqTimeToUtc(eventDateTime);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          // Derived events.
          case CalendarConstants.SubmissionsOpen:
            if (closureDate != DateTime.MinValue)
            {
              eventDateTime = closureDate.Add(SystemParameters.Instance.CalendarParameters.SubmissionsOpenOffset);
              var nextPeriod = billingPeriod + 1;

              // Get existing event from database for the event type ACH
              var nextSubmissionsOpenEvent =
                _calendarRepository.First(
                  c => c.Year == nextPeriod.Year && c.Month == nextPeriod.Month && c.Period == nextPeriod.Period && c.Name.Equals(CalendarConstants.SubmissionsOpen, StringComparison.OrdinalIgnoreCase));
              if (nextSubmissionsOpenEvent != null)
              {
                UpdateEvent(nextSubmissionsOpenEvent, eventDateTime);
              }
              else
              {
                CreateEvent(CalendarConstants.SubmissionsOpen, CalendarConstants.SubmissionsOpenColumn, nextPeriod, CalendarEventCategory.ISICH.ToString().ToUpper(), eventDateTime);
              }
            }
            break;
          case CalendarConstants.FutureDatedSubmissionsOpen:
            if (closureDate != DateTime.MinValue)
            {
              TimeSpan offset = SystemParameters.Instance.CalendarParameters.FutureDatedSubmissionsOpenOffset;
              var futureDatedSubmissionOpenDate = new DateTime(billingPeriod.Year, billingPeriod.Month, offset.Days, offset.Hours, offset.Minutes, offset.Seconds);
              futureDatedSubmissionOpenDate = futureDatedSubmissionOpenDate.AddMonths(-1);
              UpdateEvent(existingEvent, futureDatedSubmissionOpenDate);
            }
            break;
          case CalendarConstants.SubmissionDeadlineForIchAndBilateralInvoices:
            if (closureDay != DateTime.MinValue)
            {
              eventDateTime = closureDay;
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.SupportingDocumentsLinkingDeadline:
            if (adviceDate != DateTime.MinValue)
            {
              eventDateTime = adviceDate.Add(SystemParameters.Instance.CalendarParameters.SupportingAttachmentLinkingDeadlineOffset);
              if (existingEvent.EventDateTime != eventDateTime)
              {
                UpdateEvent(existingEvent, eventDateTime);
              }
            }
            break;
          case CalendarConstants.BillingOutputGeneration:
            // Process BillingOutputGeneration event here which is based on SupportingDocumentsLinkingDeadline event
            eventDateTime =
              adviceDate.Add(SystemParameters.Instance.CalendarParameters.SupportingAttachmentLinkingDeadlineOffset).AddHours(
                SystemParameters.Instance.CalendarParameters.BillingOutputGenerationHoursOffset);
            if (existingEvent.EventDateTime != eventDateTime)
            {
              UpdateEvent(existingEvent, eventDateTime);
            }
            break;
          case CalendarConstants.ClosureOfLateSubmissionsIch:
            // Add Closure of Late Submissions for ICH event
            if (SystemParameters.Instance.CalendarParameters.ManualControlOnLateSubmissionsICH.Equals("No", StringComparison.OrdinalIgnoreCase))
            {
              UpdateEvent(existingEvent, adviceDay);
            }
            break;
        }
      }
      return;
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

    private bool IsAllowToAddACHEvents(BillingPeriod billingPeriod)
    {
      var ichEventCategory = CalendarEventCategory.ICH.ToString();
      var ichEventCount = _calendarRepository.GetCount(
          c =>
          c.Year == billingPeriod.Year && c.Month == billingPeriod.Month && c.Period == billingPeriod.Period &&
          (c.EventCategory.ToUpper().Equals(ichEventCategory)));

      if (ichEventCount > 0)
      {
        return true;
      }

      CreateValidationError(string.Format(Messages.CalendarICHEventsNotExists, billingPeriod.Period, billingPeriod.Month, billingPeriod.Year));
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
    /// Custom type selector for calendar record.
    /// </summary>
    /// <param name="engine">MultiRecordEngine object</param>
    /// <param name="record">record</param>
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

      //// Validate column names in input file, If header row provided.
      //if (_recordCount == 0 && _headerRowFlag)
      //{
      //  ValidateHeaderRow(record);
      //}

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
    /// Add event in calendarEvents collection.
    /// </summary>
    /// <param name="name">Event name</param>
    /// <param name="description">Event description</param>
    /// <param name="billingPeriod">Billing period</param>
    /// <param name="eventCategory">Event type.</param>
    /// <param name="eventDate">Event date time.</param>
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
                         DisplayOnHomePage = true,
                         IsActive = true,
                         LastUpdatedBy = _lastUpdatedBy,
                         LastUpdatedOn = DateTime.UtcNow
                       };
      _calendarEvents.Add(newEvent);
    }

    /// <summary>
    /// Update event date time of existing event.
    /// </summary>
    /// <param name="existingEvent">existing event object.</param>
    /// <param name="newEventDate">updated event date time.</param>
    private void UpdateEvent(ISCalendar existingEvent, DateTime newEventDate)
    {
      // If there are validation errors then do not add or update any event.
      if (_validationErrors.Count > 0)
      {
        return;
      }

      existingEvent.EventDateTime = newEventDate;
      existingEvent.LastUpdatedBy = _lastUpdatedBy;
      existingEvent.LastUpdatedOn = DateTime.UtcNow;
      _calendarEvents.Add(existingEvent);
    }

    /// <summary>
    /// Is given period closed, current or next period.
    /// </summary>
    /// <param name="newBillingPeriod">new billing period</param>
    /// <returns>flag indicates that given period is greater than next billing period or not.</returns>
    private bool IsPeriodClosedOrCurrentOrNext(BillingPeriod newBillingPeriod)
    {
      var nextBillingPeriod = GetNextBillingPeriod();

      return newBillingPeriod <= nextBillingPeriod;
    }

    private bool IsValidInputDateTime(string inputDate, string fieldName, out DateTime eventdate)
    {
      eventdate = DateTime.MinValue;
      if (!inputDate.Length.Equals(InputDateFormat.Length) || !DateTime.TryParseExact(inputDate, InputDateFormat, null, DateTimeStyles.None, out eventdate))
      {
        CreateValidationError(string.Format(Messages.CalendarInputDateValueInvalid, fieldName));
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
          CreateValidationError(string.Format(Messages.CalendarDuplicatePeriod, period.Period, period.Month, period.Year));
          return false;
        }

        // Check for missing period in given range and sequence.
        var previousPeriod = period - 1;
        var previousPeriodValue = previousPeriod.Year * 1000 + previousPeriod.Month * 10 + previousPeriod.Period;
        if (_processedPeriods.Last() != previousPeriodValue)
        {
          CreateValidationError(string.Format(Messages.CalendarMissingPeriod, previousPeriod.Period, previousPeriod.Month, previousPeriod.Year));

          // Add to the list of processed periods not to give error for next period.
          _processedPeriods.Add(periodValue);
          return false;
        }
      }
      _processedPeriods.Add(periodValue);
      return true;
    }

    private void CreateValidationError(string errorMessage)
    {
      _validationErrors.Add(new CalendarValidationError
                              {
                                FieldName = "",
                                RecordNo = _recordCount,
                                Message = errorMessage
                              });
    }

    /// <summary>
    /// This method is used for retrieving previous billing period based on date a week ago
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <returns>Previous Period value</returns>
    private static BillingPeriod GetPreviousBillingPeriod()
    {
      var dateAWeekAgo = DateTime.UtcNow.AddDays(-7);
      var year = dateAWeekAgo.Year;
      var month = dateAWeekAgo.Month;
      var billingPeriod = new BillingPeriod
                            {
                              Month = month,
                              Year = year
                            };

      try
      {
        if ((dateAWeekAgo.Day >= 7) && (dateAWeekAgo.Day <= 13))
        {
          billingPeriod.Period = 1;
          billingPeriod.StartDate = new DateTime(year, month, 7);
          billingPeriod.EndDate = new DateTime(year, month, 13);
        }
        else if ((dateAWeekAgo.Day >= 14) && (dateAWeekAgo.Day <= 20))
        {
          billingPeriod.Period = 2;
          billingPeriod.StartDate = new DateTime(year, month, 14);
          billingPeriod.EndDate = new DateTime(year, month, 20);
        }
        else if ((dateAWeekAgo.Day >= 21) && (dateAWeekAgo.Day <= 27))
        {
          billingPeriod.Period = 3;
          billingPeriod.StartDate = new DateTime(year, month, 21);
          billingPeriod.EndDate = new DateTime(year, month, 27);
        }
        else
        {
          billingPeriod.Period = 4;

          if ((dateAWeekAgo.Day >= 1) && (dateAWeekAgo.Day <= 6))
          {
            billingPeriod.Month = month == 1 ? 12 : month - 1;
            billingPeriod.StartDate = new DateTime(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1, 28);
            billingPeriod.EndDate = new DateTime(year, month, 6);
            if (month == 1)
            {
              billingPeriod.Year -= 1;
            }
          }
          else
          {
            billingPeriod.StartDate = new DateTime(year, month, 28);
            billingPeriod.EndDate = new DateTime(month == 12 ? year + 1 : year, month == 12 ? 1 : month + 1, 6);
          }
        }
      }
      catch (Exception)
      {
        throw new ISDataException("PeriodError1", "Could not retrieve previous period");
      }
      return billingPeriod;
    }

    /// <summary>
    /// This method is used for retrieving next billing period based on next week date
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
    /// </summary>
    /// <returns>Next Period value</returns>
    public BillingPeriod GetNextBillingPeriod()
    {
      var dateAWeekAgo = DateTime.UtcNow.AddDays(7);
      var year = dateAWeekAgo.Year;
      var month = dateAWeekAgo.Month;
      var billingPeriod = new BillingPeriod
                            {
                              Month = month,
                              Year = year
                            };

      try
      {
        if ((dateAWeekAgo.Day >= 7) && (dateAWeekAgo.Day <= 13))
        {
          billingPeriod.Period = 1;
          billingPeriod.StartDate = new DateTime(year, month, 7);
          billingPeriod.EndDate = new DateTime(year, month, 13);
        }
        else if ((dateAWeekAgo.Day >= 14) && (dateAWeekAgo.Day <= 20))
        {
          billingPeriod.Period = 2;
          billingPeriod.StartDate = new DateTime(year, month, 14);
          billingPeriod.EndDate = new DateTime(year, month, 20);
        }
        else if ((dateAWeekAgo.Day >= 21) && (dateAWeekAgo.Day <= 27))
        {
          billingPeriod.Period = 3;
          billingPeriod.StartDate = new DateTime(year, month, 21);
          billingPeriod.EndDate = new DateTime(year, month, 27);
        }
        else
        {
          billingPeriod.Period = 4;

          if ((dateAWeekAgo.Day >= 1) && (dateAWeekAgo.Day <= 6))
          {
            billingPeriod.Month = month == 1 ? 12 : month - 1;
            billingPeriod.StartDate = new DateTime(month == 1 ? year - 1 : year, month == 1 ? 12 : month - 1, 28);
            billingPeriod.EndDate = new DateTime(year, month, 6);
            if (month == 1)
            {
              billingPeriod.Year -= 1;
            }
          }
          else
          {
            billingPeriod.StartDate = new DateTime(year, month, 28);
            billingPeriod.EndDate = new DateTime(month == 12 ? year + 1 : year, month == 12 ? 1 : month + 1, 6);
          }
        }
      }
      catch (Exception)
      {
        throw new ISDataException("PeriodError1", "Could not retrieve previous period");
      }
      return billingPeriod;
    }

    /// <summary>
    /// Adds the IS calendar.
    /// </summary>
    /// <param name="isCalendar">The is calendar.</param>
    /// <returns></returns>
    public ISCalendar AddISCalendar(ISCalendar isCalendar)
    {
      var ISCalendarData = CalendarRepository.Single(type => type.Name.ToLower() == isCalendar.Name.ToLower());

      //If Airport Code already exists, throw exception
      if (ISCalendarData != null)
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
      var ISCalendarData = CalendarRepository.Single(type => type.Id == isCalendar.Id);
      var updateISCalendar = CalendarRepository.Update(isCalendar);
      UnitOfWork.CommitDefault();
      return updateISCalendar;
    }

    /// <summary>
    /// Deletes the IS calendar.
    /// </summary>
    /// <param name="isCalendarId">The is calendar id.</param>
    /// <returns></returns>
    public bool DeleteISCalendar(int isCalendarId)
    {
      bool delete = false;
      var ISCalendarData = CalendarRepository.Single(type => type.Id == isCalendarId);
      if (ISCalendarData != null)
      {
        ISCalendarData.IsActive = !(ISCalendarData.IsActive);
        var updatedCalendar = CalendarRepository.Update(ISCalendarData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the IS calendar details.
    /// </summary>
    /// <param name="isCalendarId">The is calendar id.</param>
    /// <returns></returns>
    public ISCalendar GetISCalendarDetails(int isCalendarId)
    {
      var ISCalendarData = CalendarRepository.Single(type => type.Id == isCalendarId);
      return ISCalendarData;
    }

    /// <summary>
    /// Gets all IS calendar list.
    /// </summary>
    /// <returns></returns>
    public List<ISCalendar> GetAllISCalendarList()
    {
      var AirportList = CalendarRepository.GetAll();
      return AirportList.ToList();
    }

    /// <summary>
    /// Gets the IS calendar list.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <param name="eventCategory">The event category.</param>
    /// <returns></returns>
    public List<ISCalendar> GetISCalendarList(int month, int period, string eventCategory)
    {
      var ISCalendarList = new List<ISCalendar>();
      ISCalendarList = CalendarRepository.GetAll().ToList();

      if (month > 0)
      {
        ISCalendarList = ISCalendarList.Where(cl => (cl.Month == month)).ToList();
      }
      if (period > 0)
      {
        ISCalendarList = ISCalendarList.Where(cl => cl.Period == period).ToList();
      }
      if (!string.IsNullOrEmpty(eventCategory))
      {
        ISCalendarList = ISCalendarList.Where(cl => (cl.EventCategory.ToLower().Contains(eventCategory.ToLower()))).ToList();
      }
      return ISCalendarList.ToList();
    }

    /// <summary>
    /// Gets IS calender upcoming events
    /// </summary>
    /// <param name="eventCategory">The event category.</param>
    /// <param name="recordCount">No. of records to be fetched.</param>
    /// <param name="localTimeZoneId"></param>
    /// <returns></returns>
    public List<UpcomingEventsResultSet> GetUpComingIsCalendarList(string eventCategory, int recordCount, string localTimeZoneId)
    {
      TimeZoneInfo utctimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
      TimeZoneInfo ymqtimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
      TimeZoneInfo localtimeZone = TimeZoneInfo.FindSystemTimeZoneById("Fiji Standard Time");
      var timezoneList = TimeZoneInfo.GetSystemTimeZones();
      var isCalendarList = CalendarRepository.Get(calEvent => calEvent.EventDateTime > DateTime.UtcNow && calEvent.EventCategory.ToLower().Contains(eventCategory.ToLower())).OrderBy(calEvent => calEvent.EventDateTime).Take(recordCount).ToList();

      var upcomingEventsResultSet = isCalendarList.Select(t => new UpcomingEventsResultSet
      {
        EventDescription = t.EventDescription,
        LocalDateTime = TimeZoneInfo.ConvertTime(t.EventDateTime, utctimeZone, localtimeZone),
        YmqDateTime = TimeZoneInfo.ConvertTime(t.EventDateTime, utctimeZone, ymqtimeZone),
        Period = EnumList.GetMonthDisplayValue((Month)t.Month) + " " + t.Year + " P" + t.Period
      }).ToList();


      return upcomingEventsResultSet.ToList();
    }


    public TimeInterval GetLateSubmissionWindow(ClearingHouse clearingHouse, DateTime inputDate)
    {
      throw new NotImplementedException();
    }


    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, BillingPeriod billingPeriod)
    {
      throw new NotImplementedException();
    }

    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, BillingPeriod billingPeriod, DateTime inputDate)
    {
        throw new NotImplementedException();
    }

      public BillingPeriod GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse clearingHouse)
    {
      throw new NotImplementedException();
    }


    public BillingPeriod GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse clearingHouse)
    {
      throw new NotImplementedException();
    }


    public bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, DateTime inputDate)
    {
      throw new NotImplementedException();
    }


    public BillingPeriod GetLastClosedBillingPeriod(DateTime date, ClearingHouse clearingHouse = ClearingHouse.Ich)
    {
      throw new NotImplementedException();
    }


    public List<CalendarEvent> GetCalendarEventList()
    {
      throw new NotImplementedException();
    }


    public List<CalendarProcess> GetCalendarProcessList()
    {
      throw new NotImplementedException();
    }

    public List<DerivedEventOffsetInfo> GetDerivedEventList()
    {
      throw new NotImplementedException();
    }
    public DateTime GetCalendarEventTime(string eventName, int billingYear, int billingMonth, int Period)
    {
        throw new NotImplementedException();
    }


      public List<int> GetCalendarYear()
      {
          throw new NotImplementedException();
      }


      public BillingPeriod GetAutoBillingPeriod(DateTime inputDate, double offsetHours, BillingPeriod currentBillingPeriod)
      {
          throw new NotImplementedException();
      }


      public BillingPeriod GetCurrentAutoBillingPeriod(int airLineId)
      {
        throw new NotImplementedException();
      }

  }
}
