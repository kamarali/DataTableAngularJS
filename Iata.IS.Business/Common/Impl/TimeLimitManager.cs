using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class TimeLimitManager : ITimeLimitManager
  {
    /// <summary>
    /// Gets or sets the time limit repository.
    /// </summary>
    /// <value>
    /// The time limit repository.
    /// </value>
    public ITimeLimitRepository TimeLimitRepository { get; set; }

    /// <summary>
    /// Adds the time limit.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <returns></returns>
    public TimeLimit AddTimeLimit(TimeLimit timeLimit)
    {
      // User should not be able to enter/save duplicate active records for a combination of Effective From Period, Effective To Period, Transaction Type ID and Settlement Method.
      if(IsTimeLimitRecordDuplicate(timeLimit.SettlementMethodId, timeLimit.TransactionTypeId, timeLimit.EffectiveFromPeriod, timeLimit.EffectiveToPeriod, timeLimit.Id))
      {
        throw new ISBusinessException(Messages.DuplicateTimeLimitRecord);
      }
      // TODO: Put a validation that day part should not exceed 4 for certain transactions.

      TimeLimitRepository.Add(timeLimit);
      UnitOfWork.CommitDefault();
      return timeLimit;
    }

    /// <summary>
    /// Updates the time limit.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <returns></returns>
    public TimeLimit UpdateTimeLimit(TimeLimit timeLimit)
    {
      var updatedTimeLimit = TimeLimitRepository.Single(type => type.Id == timeLimit.Id);
      if (updatedTimeLimit != null && CompareUtil.IsDirty(timeLimit.SettlementMethodId, updatedTimeLimit.SettlementMethodId) || CompareUtil.IsDirty(timeLimit.TransactionTypeId, updatedTimeLimit.TransactionTypeId) || CompareUtil.IsDirty(timeLimit.EffectiveFromPeriod, updatedTimeLimit.EffectiveFromPeriod) || CompareUtil.IsDirty(timeLimit.EffectiveToPeriod, updatedTimeLimit.EffectiveToPeriod))
      {
        // User should not be able to enter/save duplicate active records for a combination of Effective From Period, Effective To Period, Transaction Type ID and Settlement Method.
        if (IsTimeLimitRecordDuplicate(timeLimit.SettlementMethodId, timeLimit.TransactionTypeId, timeLimit.EffectiveFromPeriod, timeLimit.EffectiveToPeriod, timeLimit.Id))
        {
          throw new ISBusinessException(Messages.DuplicateTimeLimitRecord);
        }
      }
      // TODO: Put a validation that day part should not exceed 4 for certain transactions.
      
      updatedTimeLimit = TimeLimitRepository.Update(timeLimit);
      UnitOfWork.CommitDefault();
      
      return updatedTimeLimit;
    }

    /// <summary>
    /// Deletes the time limit.
    /// </summary>
    /// <param name="timeLimitId">The time limit id.</param>
    /// <returns></returns>
    public bool DeleteTimeLimit(int timeLimitId)
    {
      bool delete = false;
      var timeLimitData = TimeLimitRepository.Single(type => type.Id == timeLimitId);
      if (timeLimitData != null)
      {
        timeLimitData.IsActive = !(timeLimitData.IsActive);
        var updatedcountry = TimeLimitRepository.Update(timeLimitData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the time limit details.
    /// </summary>
    /// <param name="timeLimitId">The time limit id.</param>
    /// <returns></returns>
    public TimeLimit GetTimeLimitDetails(int timeLimitId)
    {
      var TimeLimit = TimeLimitRepository.Single(type => type.Id == timeLimitId);
      return TimeLimit;
    }

    /// <summary>
    /// Gets all time limit list.
    /// </summary>
    /// <returns></returns>
    public List<TimeLimit> GetAllTimeLimitList()
    {
      var TimeLimitList = TimeLimitRepository.GetAllTimeLimits();

      return TimeLimitList.ToList();
    }

    /// <summary>
    /// Gets the time limit list.
    /// </summary>
    /// <param name="timeLimit">The time llimit.</param>
    /// <param name="settlementMethodId">The settlement method.</param>
    /// <param name="transactionTypeId">The transaction type id.</param>
    /// <param name="effectiveFromPeriod">The Effective From Period.</param>
    /// <param name="effectiveToPeriod">The Effective To Period.</param>
    /// <returns></returns>
    public List<TimeLimit> GetTimeLimitList(int timeLimit, int settlementMethodId, int transactionTypeId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod)
    {
      var dateTime = new DateTime(1,1,1);
      var timeLimitList = TimeLimitRepository.GetAllTimeLimits().ToList();

      if(effectiveFromPeriod > dateTime) // not the default datetime
      {
        timeLimitList = timeLimitList.Where(cl => ((cl.EffectiveFromPeriod == effectiveFromPeriod)
                                            || (cl.EffectiveFromPeriod > effectiveFromPeriod))).ToList();
      }

      if (effectiveToPeriod > dateTime) // not the default datetime
      {
        timeLimitList = timeLimitList.Where(cl => ((cl.EffectiveToPeriod == effectiveToPeriod) ||
                                            (cl.EffectiveToPeriod < effectiveToPeriod))).ToList();
      }

      if (timeLimit > 0)
      {
        timeLimitList = timeLimitList.Where(cl => (cl.Limit == timeLimit)).ToList();
      }

      if (settlementMethodId > 0)
      {
        timeLimitList = timeLimitList.Where(cl => cl.SettlementMethodId == settlementMethodId).ToList();
      }

      if (transactionTypeId > 0)
      {
        timeLimitList = timeLimitList.Where(cl => (cl.TransactionTypeId == transactionTypeId)).ToList();
      }

      return timeLimitList.ToList();
    }

    private bool IsTimeLimitRecordDuplicate(int settlementMethodId, int transactionTypeId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int limitId)
    {
      var timeLimit = TimeLimitRepository.First(limit => limit.Id  != limitId && limit.TransactionTypeId == transactionTypeId && limit.SettlementMethodId == settlementMethodId && ((effectiveFromPeriod >= limit.EffectiveFromPeriod && effectiveFromPeriod <= limit.EffectiveToPeriod) || (effectiveToPeriod >= limit.EffectiveFromPeriod && effectiveToPeriod <= limit.EffectiveToPeriod)) && limit.IsActive);
      if (timeLimit == null)
        return false;
      
      return true;
    }
  }
}
