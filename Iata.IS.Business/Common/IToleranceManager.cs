using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IToleranceManager
    {
        /// <summary>
        /// Adds the tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns></returns>
        Tolerance AddTolerance(Tolerance tolerance);

        /// <summary>
        /// Updates the tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns></returns>
        Tolerance UpdateTolerance(Tolerance tolerance);

        /// <summary>
        /// Deletes the tolerance.
        /// </summary>
        /// <param name="toleranceId">The tolerance id.</param>
        /// <returns></returns>
        bool DeleteTolerance(int toleranceId);

        /// <summary>
        /// Gets the tolerance details.
        /// </summary>
        /// <param name="toleranceId">The tolerance id.</param>
        /// <returns></returns>
        Tolerance GetToleranceDetails(int toleranceId);

        /// <summary>
        /// Gets all tolerance list.
        /// </summary>
        /// <returns></returns>
        List<Tolerance> GetAllToleranceList();

        /// <summary>
        /// Gets the tolerance list.
        /// </summary>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="clearingHouse">The clearing house.</param>
        /// <param name="type">The type.</param>
        /// <param name="effectiveFromPeriod"></param>
        /// <param name="effectiveToPeriod"></param>
        /// <returns></returns>
        List<Tolerance> GetToleranceList(int billingCategoryId, string clearingHouse, string type,
                                         DateTime effectiveFromPeriod, DateTime effectiveToPeriod);

    }
}
