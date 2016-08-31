using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ILeadPeriodManager
    {
        /// <summary>
        /// Adds the lead period.
        /// </summary>
        /// <param name="leadPeriod">The lead period.</param>
        /// <returns></returns>
        LeadPeriod AddLeadPeriod(LeadPeriod leadPeriod);

        /// <summary>
        /// Updates the lead period.
        /// </summary>
        /// <param name="leadPeriod">The lead period.</param>
        /// <returns></returns>
        LeadPeriod UpdateLeadPeriod(LeadPeriod leadPeriod);

        /// <summary>
        /// Deletes the lead period.
        /// </summary>
        /// <param name="leadPeriodId">The lead period id.</param>
        /// <returns></returns>
        bool DeleteLeadPeriod(int leadPeriodId);

        /// <summary>
        /// Gets the lead period details.
        /// </summary>
        /// <param name="leadPeriodId">The lead period id.</param>
        /// <returns></returns>
        LeadPeriod GetLeadPeriodDetails(int leadPeriodId);

        /// <summary>
        /// Gets all lead period list.
        /// </summary>
        /// <returns></returns>
        List<LeadPeriod> GetAllLeadPeriodList();

        /// <summary>
        /// Gets the lead period list.
        /// </summary>
        /// <param name="period">The limit.</param>
        /// <param name="clearingHouse">The clearing house.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="effectiveFromPeriod">The effective from period.</param>
        /// <param name="effectiveToPeriod">The effective to period.</param>
        /// <returns></returns>
        List<LeadPeriod> GetLeadPeriodList(int period, string clearingHouse, int billingCategoryId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod);
    }
}