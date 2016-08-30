using System;
using System.Globalization;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Calendar
{
  /// <summary>
  /// Represents a billing period.
  /// </summary>
  public struct BillingPeriod : IEquatable<BillingPeriod>
  {
    /// <summary>
    /// Billing year.
    /// </summary>
    public int Year;

    /// <summary>
    /// Billing month.
    /// </summary>
    public int Month;

    /// <summary>
    /// Billing period.
    /// </summary>
    public int Period;

    /// <summary>
    /// Billing period start date.
    /// </summary>
    public DateTime StartDate;

    /// <summary>
    /// Billing period end date.
    /// </summary>
    public DateTime EndDate;

    /// <summary>
    /// Clearing house to which this billing period belongs.
    /// </summary>
    public ClearingHouse ClearingHouse;

    public BillingPeriod(int year, int month, int period)
    {
      Year = year;
      Month = month;
      Period = period;
      ClearingHouse = ClearingHouse.Ich;
      StartDate = new DateTime(Year, Month, Period * 7);
      EndDate = StartDate.AddDays(7);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }

      if (obj.GetType() != typeof(BillingPeriod))
      {
        return false;
      }

      return Equals((BillingPeriod)obj);
    }

    public static bool operator ==(BillingPeriod period1, BillingPeriod period2)
    {
      return period1.Year == period2.Year && period1.Month == period2.Month && period1.Period == period2.Period;
    }

    public static bool operator !=(BillingPeriod period1, BillingPeriod period2)
    {
      return !(period1 == period2);
    }

    public static bool operator <(BillingPeriod period1, BillingPeriod period2)
    {
      if (period1 == period2)
      {
        return false;
      }

      if (period1.Year < period2.Year)
      {
        return true;
      }
      else if (period1.Year > period2.Year)
      {
        return false;
      }
      else
      {
        if (period1.Month < period2.Month)
        {
          return true;
        }
        else if (period1.Month > period2.Month)
        {
          return false;
        }
        else
        {
          if (period1.Period < period2.Period)
          {
            return true;
          }
          return false;
        }
      }
    }

    public static bool operator >(BillingPeriod period1, BillingPeriod period2)
    {
      if (period1 != period2)
      {
        return !(period1 < period2);
      }

      return false;
    }

    public static bool operator <=(BillingPeriod period1, BillingPeriod period2)
    {
      if (period1 == period2) return true;
      if (period1 < period2) return true;
      return false;
    }

    public static bool operator >=(BillingPeriod period1, BillingPeriod period2)
    {
      if (period1 == period2) return true;
      if (period1 > period2) return true;
      return false;
    }

    public static BillingPeriod operator +(BillingPeriod period1, int periodInput)
    {
      //Calculate new period
      int periodCount = period1.Period + periodInput;

      //Calculate new month
      int monthCount = period1.Month;
      if (periodCount > 4)
      {
        monthCount += periodCount / 4;
        if (periodCount % 4 == 0) monthCount--;
        periodCount = periodCount % 4 == 0 ? 4 : periodCount % 4;
      }

      //Calculate new year
      int yearCount = period1.Year;
      if (monthCount > 12)
      {
        yearCount += monthCount / 12;
        monthCount = monthCount % 12 == 0 ? 12 : monthCount % 12;
      }

      return new BillingPeriod(yearCount, monthCount, periodCount);
    }

    public static BillingPeriod operator -(BillingPeriod period1, int periodInput)
    {

      int periodCount = period1.Period, monthCount = period1.Month, yearCount = period1.Year;
      if (period1.Period > periodInput)
      {
        //Calculate new period
        periodCount = period1.Period - periodInput;
      }
      else if (period1.Period == periodInput)
      {
        //Calculate new period
        periodCount = 4;

        //Calculate new month
        if (monthCount == 1)
        {
          monthCount = 12;
          yearCount--;
        }
        else
          monthCount--;
      }
      else
      {
        // todo:implement logic to subtract given period count if current period count less than the input period count.
        throw new Exception("Need to implement logic to subtract given period count from corresponding billing period object if current period count less than the input period count.");
      }

      return new BillingPeriod(yearCount, monthCount, periodCount);
    }

    public bool Equals(BillingPeriod other)
    {
      return other.Year == Year && other.Month == Month && other.Period == Period;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = Year;

        result = (result * 397) ^ Month;
        result = (result * 397) ^ Period;

        return result;
      }
    }
  }
}
