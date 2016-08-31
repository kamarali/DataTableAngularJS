using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface IBvcMatrixManager
    {

        /// <summary>
        /// Adds the BVC matrix.
        /// </summary>
        /// <param name="bvcMatrix">The BVC matrix.</param>
        /// <returns></returns>
        BvcMatrix AddBvcMatrix(BvcMatrix bvcMatrix);

        /// <summary>
        /// Updates the BVC matrix.
        /// </summary>
        /// <param name="bvcMatrix">The BVC matrix.</param>
        /// <returns></returns>
        BvcMatrix UpdateBvcMatrix(BvcMatrix bvcMatrix);

        /// <summary>
        /// Deletes the BVC matrix.
        /// </summary>
        /// <param name="bvcMatrixId">The BVC matrix id.</param>
        /// <returns></returns>
        bool DeleteBvcMatrix(int bvcMatrixId);

        /// <summary>
        /// Gets the BVC matrix details.
        /// </summary>
        /// <param name="bvcMatrixId">The BVC matrix id.</param>
        /// <returns></returns>
        BvcMatrix GetBvcMatrixDetails(int bvcMatrixId);

        /// <summary>
        /// Gets all BVC matrix list.
        /// </summary>
        /// <returns></returns>
        List<BvcMatrix> GetAllBvcMatrixList();

        /// <summary>
        /// Gets the BVC matrix list.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <param name="effectiveFrom">The effective from.</param>
        /// <param name="effectiveTo">The effective to.</param>
        /// <returns></returns>
        List<BvcMatrix> GetBvcMatrixList(string ValidatedPmi, string effectiveFrom, string effectiveTo);
    }
}
