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
  public class ToleranceManager : IToleranceManager
  {
    /// <summary>
    /// Gets or sets the tolerance repository.
    /// </summary>
    /// <value>
    /// The tolerance repository.
    /// </value>
    public IRepository<Tolerance> ToleranceRepository { get; set; }

    /// <summary>
    /// Adds the tolerance.
    /// </summary>
    /// <param name="tolerance">The tolerance.</param>
    /// <returns></returns>
    public Tolerance AddTolerance(Tolerance tolerance)
    {
      // Check whether Effective from and To dates, Day part is greater than 4, if yes throw an Exception
      if (tolerance.EffectiveFromPeriod.Day > 4)
      {
        throw new ISBusinessException(Messages.MinAccepAmtEffectiveFromPeriod);
      }
      if (tolerance.EffectiveToPeriod.Day > 4)
      {
        throw new ISBusinessException(Messages.MinAccepAmtEffectiveToPeriod);
      }

      if (IsToleranceRecordDuplicate(tolerance.ClearingHouse, tolerance.BillingCategoryId, tolerance.EffectiveFromPeriod, tolerance.EffectiveToPeriod, tolerance.Id))
      {
          throw new ISBusinessException(Messages.DuplicateToleranceRecord);
      }

      ToleranceRepository.Add(tolerance);
      UnitOfWork.CommitDefault();
      return tolerance;
    }

    /// <summary>
    /// Updates the tolerance.
    /// </summary>
    /// <param name="tolerance">The tolerance.</param>
    /// <returns></returns>
    public Tolerance UpdateTolerance(Tolerance tolerance)
    {
      // Check whether Effective from and To dates, Day part is greater than 4, if yes throw an Exception
      if (tolerance.EffectiveFromPeriod.Day > 4)
      {
        throw new ISBusinessException(Messages.MinAccepAmtEffectiveFromPeriod);
      }
      if (tolerance.EffectiveToPeriod.Day > 4)
      {
        throw new ISBusinessException(Messages.MinAccepAmtEffectiveToPeriod);
      }

      var updatedtolerance = ToleranceRepository.Single(type => type.Id == tolerance.Id);
      if (updatedtolerance != null && CompareUtil.IsDirty(tolerance.ClearingHouse, updatedtolerance.ClearingHouse) || CompareUtil.IsDirty(tolerance.BillingCategoryId, updatedtolerance.BillingCategoryId) || CompareUtil.IsDirty(tolerance.EffectiveFromPeriod, updatedtolerance.EffectiveFromPeriod) || CompareUtil.IsDirty(tolerance.EffectiveToPeriod, updatedtolerance.EffectiveToPeriod))
      {
          if (IsToleranceRecordDuplicate(tolerance.ClearingHouse, tolerance.BillingCategoryId, tolerance.EffectiveFromPeriod, tolerance.EffectiveToPeriod, tolerance.Id))
          {
              throw new ISBusinessException(Messages.DuplicateToleranceRecord);
          }
      }
      updatedtolerance = ToleranceRepository.Update(tolerance);
      UnitOfWork.CommitDefault();
      return updatedtolerance;
    }

    /// <summary>
    /// Deletes the tolerance.
    /// </summary>
    /// <param name="toleranceId">The tolerance id.</param>
    /// <returns></returns>
    public bool DeleteTolerance(int toleranceId)
    {
      bool delete = false;
      var toleranceData = ToleranceRepository.Single(type => type.Id == toleranceId);
      if (toleranceData != null)
      {
        toleranceData.IsActive = !(toleranceData.IsActive);
        var updatedcountry = ToleranceRepository.Update(toleranceData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the tolerance details.
    /// </summary>
    /// <param name="toleranceId">The tolerance id.</param>
    /// <returns></returns>
    public Tolerance GetToleranceDetails(int toleranceId)
    {
      var tolerance = ToleranceRepository.Single(type => type.Id == toleranceId);
      return tolerance;
    }

    /// <summary>
    /// Gets all tolerance list.
    /// </summary>
    /// <returns></returns>
    public List<Tolerance> GetAllToleranceList()
    {
      var toleranceList = ToleranceRepository.GetAll();
      return toleranceList.ToList();
    }

    /// <summary>
    /// Gets the tolerance list.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="type">The type.</param>
    /// <param name="effectiveFromPeriod"></param>
    /// <param name="effectiveToPeriod"></param>
    /// <returns></returns>
    public List<Tolerance> GetToleranceList(int billingCategoryId, string clearingHouse, string type, DateTime effectiveFromPeriod, DateTime effectiveToPeriod)
    {
      var toleranceList = new List<Tolerance>();
      toleranceList = ToleranceRepository.GetAll().ToList();

      var dateTime = new DateTime(1, 1, 1);
      if (billingCategoryId > 0)
      {
        toleranceList = toleranceList.Where(cl => cl.BillingCategoryId == billingCategoryId).ToList();
      }
      if (!string.IsNullOrEmpty(clearingHouse))
      {
        toleranceList = toleranceList.Where(cl => cl.ClearingHouse != null && cl.ClearingHouse.ToLower().Contains(clearingHouse.ToLower())).ToList();
      }
      if (!string.IsNullOrEmpty(type))
      {
        toleranceList = toleranceList.Where(cl => cl.Type != null && cl.Type.ToLower().Contains(type.ToLower())).ToList();
      }
      // Changed logic for Effective From Period and Effective To Period

      if (effectiveFromPeriod > dateTime || effectiveToPeriod > dateTime)
      {
          toleranceList =
              toleranceList.Where(
                  cl =>
                  ((cl.EffectiveFromPeriod <= effectiveFromPeriod) && (cl.EffectiveToPeriod >= effectiveFromPeriod))
                  ||
                  ((cl.EffectiveFromPeriod <= effectiveToPeriod) && (cl.EffectiveToPeriod >= effectiveToPeriod))
                  ||
                  ((cl.EffectiveFromPeriod <= effectiveToPeriod) && (cl.EffectiveToPeriod >= effectiveFromPeriod))).ToList();
      }

      return toleranceList.ToList();
    }

    private bool IsToleranceRecordDuplicate(string clearingHouse, int billingCategoryId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int toleranceId)
    {
      var tolerance = ToleranceRepository.First(limit => limit.Id != toleranceId && limit.ClearingHouse == clearingHouse && limit.BillingCategoryId == billingCategoryId && ((effectiveFromPeriod >= limit.EffectiveFromPeriod && effectiveFromPeriod <= limit.EffectiveToPeriod) || (effectiveToPeriod >= limit.EffectiveFromPeriod && effectiveToPeriod <= limit.EffectiveToPeriod)));
        if (tolerance == null)
            return false;

        return true;
    }
  }
}
