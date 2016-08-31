using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
  public interface ITimeLimitManager
  {
    /// <summary>
    /// Adds the time limit.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <returns></returns>
    TimeLimit AddTimeLimit(TimeLimit timeLimit);

    /// <summary>
    /// Updates the time limit.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <returns></returns>
    TimeLimit UpdateTimeLimit(TimeLimit timeLimit);

    /// <summary>
    /// Deletes the time limit.
    /// </summary>
    /// <param name="timeLimitId">The time limit id.</param>
    /// <returns></returns>
    bool DeleteTimeLimit(int timeLimitId);

    /// <summary>
    /// Gets the time limit details.
    /// </summary>
    /// <param name="timeLimitId">The time limit id.</param>
    /// <returns></returns>
    TimeLimit GetTimeLimitDetails(int timeLimitId);

    /// <summary>
    /// Gets all time limit list.
    /// </summary>
    /// <returns></returns>
    List<TimeLimit> GetAllTimeLimitList();

    /// <summary>
    /// Gets the time limit list.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <param name="effectiveFromPeriod">The effective from period.</param>
    /// <param name="effectiveToPeriod">The effective to period.</param>
    /// <returns></returns>
    List<TimeLimit> GetTimeLimitList(int timeLimit, int settlementMethodId, int transactionTypeId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod);
  }
}
