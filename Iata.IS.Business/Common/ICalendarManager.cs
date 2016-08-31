using System;
using System.Collections.Generic;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common
{
  /// <summary>
  /// 
  /// </summary>
  public interface ICalendarManager
  {
    /// <summary>
    /// This method will return list of billing period objects which contain list of year, month and period values for which billing is allowed
    /// </summary>
    /// <param name="memberId">ID of member who is creating invoices</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="nextPeriodsToInclude">This parameter defines how many next periods after the current billing period are to be returned.</param>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    List<BillingPeriod> GetRelevantBillingPeriods(string memberId, ClearingHouse clearingHouse, int nextPeriodsToInclude = 4);

    /// <summary>
    ///   This method tries to return the current billing period. If the current billing period is void or insufficient data exists in the
    ///   database, then try to get the next billing period. If that fails, raise an exception.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Current/Next billing period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    BillingPeriod GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse clearingHouse);

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
    BillingPeriod GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse clearingHouse);

    /// <summary>
    /// This method is used for retrieving billing period based on current utc date time and <B>ICH</B>.
    /// </summary>
    /// <returns>
    /// Current Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    BillingPeriod GetCurrentBillingPeriod();

    /// <summary>
    /// This method tries to return the billing period for given clearing house and date time.
    /// </summary>
    /// <param name="inputDate">input date</param>
    /// <param name="clearingHouse">The clearing house. Default value is ICH.</param>
    /// <returns>
    /// Billing Period for given date time
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    BillingPeriod GetBillingPeriod(DateTime inputDate, ClearingHouse clearingHouse = ClearingHouse.Ich);

    /// <summary>
    /// This method is used for retrieving current open billing period based on current utc date and given clearing house.
    /// </summary>
    /// <param name="clearingHouse">The clearing house. Default value is ICH.</param>
    /// <returns>
    /// Current Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    ///   </exception>
    BillingPeriod GetCurrentBillingPeriod(ClearingHouse clearingHouse);

    /// <summary>
    /// This method will return period either for current and for the previous  periods.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="includeCurrentPeriod">if set to <c>true</c> [include current period].</param>
    /// <returns>List of suspension form billing periods</returns>
    List<BillingPeriod> GetSuspensionBillingPeriods(ClearingHouse clearingHouse, bool includeCurrentPeriod);

    /// <summary>
    /// Return  previous 6 billing periods
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="includeCurrentPeriod">if set to <c>true</c> [include current period].</param>
    /// <param name="previousPeriodsToInclude">This parameter defines how many previous periods before the current billing period are to be returned.</param>
    /// <returns>List of default suspension billing periods</returns>
    List<BillingPeriod> GetDefaultSuspensionPeriods(ClearingHouse clearingHouse, bool includeCurrentPeriod, int previousPeriodsToInclude = 5);

    /// <summary>
    /// Returns the last closed billing period for current date time and clearing house passed.
    /// </summary>
    /// <param name="clearingHouse">The clearing house. Default value is ICH.</param>
    /// <returns>
    /// Last closed billing period value.
    /// </returns>
    BillingPeriod GetLastClosedBillingPeriod(ClearingHouse clearingHouse = ClearingHouse.Ich);

    /// <summary>
    /// Returns the last closed billing period for the date time and clearing house passed.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Last closed billing period value.
    /// </returns>
    BillingPeriod GetLastClosedBillingPeriod(DateTime date, ClearingHouse clearingHouse = ClearingHouse.Ich);

    /// <summary>
    /// Returns the last closed billing period for current date time and clearing house passed.
    /// Same as GetLastClosedBillingPeriod(ClearingHouse clearingHouse) method.
    /// </summary>
    /// <param name="clearingHouse">clearing house</param>
    /// <returns>
    /// Last closed billing period value
    /// </returns>
    [Obsolete("This method is obsolete, it has been replaced by CalendarManager.GetLastClosedBillingPeriod")]
    BillingPeriod GetPreviousBillingPeriod(ClearingHouse clearingHouse);

    /// <summary>
    /// This method is used for retrieving billing period based on given inputs.
    /// This method is a dummy method and it will be used only till IS Calendar is implemented in application
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
    ///   </exception>
    BillingPeriod GetBillingPeriod(ClearingHouse clearingHouse, int year, int month, int period);

      /// <summary>
      /// Get Auto Billing Period
      /// </summary>
      /// <param name="inputDate"></param>
      /// <param name="offsetHours"></param>
      /// <returns></returns>
    BillingPeriod GetAutoBillingPeriod(DateTime inputDate, double offsetHours, BillingPeriod currentBillingPeriod);

     /// <summary>
      /// Get Current Auto Billing Period
      /// </summary>
      /// <param name="airLineId"></param>
      /// <returns></returns>
      BillingPeriod GetCurrentAutoBillingPeriod(int airLineId);
    

    /// <summary>
    /// This method returns a flag indicating if the late submission window is open or not for the clearing house passed and current utc date time.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse);

    /// <summary>
    /// This method returns a flag indicating if the late submission window is open or not for the clearing house and billing period passed.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, BillingPeriod billingPeriod);

    /// <summary>
    /// This method takes the clearing house as well as date for which returns the status of late submission window.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <param name="inputDate">The input date.</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, DateTime inputDate);

    /// <summary>
    /// This method returns a flag indicating if the late submission window is open or not for the clearing house and billing period passed and datetime.
    /// </summary>
    /// <param name="clearingHouse">Clearing House flag</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="inputDate">The inputDate.</param>
    /// <returns>
    /// a flag indicating if the late submission window is open or not
    /// </returns>
    bool IsLateSubmissionWindowOpen(ClearingHouse clearingHouse, BillingPeriod billingPeriod, DateTime inputDate);

    /// <summary>
    /// Retrieves the next billing period based on clearing house passed and current utc date time.
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <returns>
    /// Next Period value
    /// </returns>
    /// <exception cref="ISCalendarDataNotFoundException">
    /// If insufficient data exists in the database, then raise an 'ISCalendarDataNotFound' exception.
    /// </exception>
    BillingPeriod GetNextBillingPeriod(ClearingHouse clearingHouse);

    /// <summary>
    /// This method is used for retrieving Late Submission Start and End Date Times for a clearing house and billing period
    /// </summary>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns>
    /// Next Period value
    /// </returns>
    TimeInterval GetLateSubmissionWindow(ClearingHouse clearingHouse, BillingPeriod billingPeriod);

    /// <summary>
    /// This method is used for retrieving Late Submission Start and End Date times for a clearing house and billing period
    /// </summary>
    /// <param name="clearingHouse">clearing house flag</param>
    /// <param name="inputDate">The input date.</param>
    /// <returns>
    /// Late Submission Start and End Date times.
    /// </returns>
    TimeInterval GetLateSubmissionWindow(ClearingHouse clearingHouse, DateTime inputDate);

    /// <summary>
    /// Upload/update the calendar schedules from clearing houses
    /// </summary>
    /// <param name="filePath">input csv file path.</param>
    /// <param name="headerRowFlag">header row flag,
    /// 1 - file contains header row
    /// 2 - file doesn't contains header row.</param>
    /// <param name="userId">Id of user uploading the calendar.</param>
    /// <returns></returns>
    List<CalendarValidationError> UploadCalendarFile(string filePath, bool headerRowFlag, int userId);

    /// <summary>
    /// Searches the calendar events.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <returns></returns>
    IList<CalendarSearchResult> SearchCalendarEvents(int year, int month, int period);

    /// <summary>
    /// Adds the IS calendar.
    /// </summary>
    /// <param name="isCalendar">The is calendar.</param>
    /// <returns></returns>
    ISCalendar AddISCalendar(ISCalendar isCalendar);

    /// <summary>
    /// Updates the IS calendar.
    /// </summary>
    /// <param name="isCalendar">The is calendar.</param>
    /// <returns></returns>
    ISCalendar UpdateISCalendar(ISCalendar isCalendar);

    /// <summary>
    /// Deletes the IS calendar.
    /// </summary>
    /// <param name="airportId">The airport id.</param>
    /// <returns></returns>
    bool DeleteISCalendar(int airportId);

    /// <summary>
    /// Gets the IS calendar details.
    /// </summary>
    /// <param name="isCalendarId">The is calendar id.</param>
    /// <returns></returns>
    ISCalendar GetISCalendarDetails(int isCalendarId);

    /// <summary>
    /// Gets all IS calendar list.
    /// </summary>
    /// <returns></returns>
    List<ISCalendar> GetAllISCalendarList();

    /// <summary>
    /// Gets the IS calendar list.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <param name="eventCategory">The event category.</param>
    /// <returns></returns>
    List<ISCalendar> GetISCalendarList(int month, int period, string eventCategory);

    /// <summary>
    /// Gets IS calendar upcoming events
    /// </summary>
    /// <param name="eventCategory">The event category.</param>
    /// <param name="recordCount">No. of records to be fetched.</param>
    /// <param name="localTimeZoneId"></param>
    /// <returns></returns>
    List<UpcomingEventsResultSet> GetUpComingIsCalendarList(string eventCategory, int recordCount,
                                                            string localTimeZoneId);

    /// <summary>
    /// Gets the calendar event list.
    /// </summary>
    /// <returns></returns>
    List<CalendarEvent> GetCalendarEventList();
    
    /// <summary>
    /// Gets the calendar process list.
    /// </summary>
    /// <returns></returns>
    List<CalendarProcess> GetCalendarProcessList();

    /// <summary>
    /// Gets the derived event list.
    /// </summary>
    /// <returns></returns>
    List<DerivedEventOffsetInfo> GetDerivedEventList();

    DateTime GetCalendarEventTime(string eventName, int billingYear, int billingMonth, int Period);

      /// <summary>
      ///  Get Calendar distinct Year as exist in database
      /// </summary>
      /// <returns></returns>
      List<int> GetCalendarYear();

    /// <summary>
    /// This method checks if future submission is allowed for the given future period.
    /// </summary>
    /// <param name="billingPeriod">The future period</param>
    /// <returns></returns>
    bool IsFutureSubmissionOpen(BillingPeriod billingPeriod);
  }
}
