using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface IRficManager
    {

        /// <summary>
        /// Adds the rfic.
        /// </summary>
        /// <param name="rfic">The rfic.</param>
        /// <returns></returns>
        Rfic AddRfic(Rfic rfic);

        /// <summary>
        /// Updates the rfic.
        /// </summary>
        /// <param name="rfic">The rfic.</param>
        /// <returns></returns>
        Rfic UpdateRfic(Rfic rfic);

        /// <summary>
        /// Deletes the rfic.
        /// </summary>
        /// <param name="rficId">The rfic id.</param>
        /// <returns></returns>
        bool DeleteRfic(string rficId);

        /// <summary>
        /// Gets the rfic details.
        /// </summary>
        /// <param name="rficId">The rfic id.</param>
        /// <returns></returns>
        Rfic GetRficDetails(string rficId);

        /// <summary>
        /// Gets all rfic list.
        /// </summary>
        /// <returns></returns>
        List<Rfic> GetAllRficList();

        /// <summary>
        /// Gets the rfic list.
        /// </summary>
        /// <param name="rficId">The rfic id.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        List<Rfic> GetRficList(string rficId, string rficDescription);
    }
}
