using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface IRfiscManager
    {

        /// <summary>
        /// Adds the rfisc.
        /// </summary>
        /// <param name="rfisc">The rfisc.</param>
        /// <returns></returns>
        Rfisc AddRfisc(Rfisc rfisc);

        /// <summary>
        /// Updates the rfisc.
        /// </summary>
        /// <param name="rfisc">The rfisc.</param>
        /// <returns></returns>
        Rfisc UpdateRfisc(Rfisc rfisc);

        /// <summary>
        /// Deletes the rfisc.
        /// </summary>
        /// <param name="rfiscId">The rfisc id.</param>
        /// <returns></returns>
        bool DeleteRfisc(string rfiscId);

        /// <summary>
        /// Gets the rfisc details.
        /// </summary>
        /// <param name="rfiscId">The rfisc id.</param>
        /// <returns></returns>
        Rfisc GetRfiscDetails(string rfiscId);

        /// <summary>
        /// Gets all rfisc list.
        /// </summary>
        /// <returns></returns>
        List<Rfisc> GetAllRfiscList();

        /// <summary>
        /// Gets the rfisc list.
        /// </summary>
        List<Rfisc> GetRfiscList(string rfiscId, string rficId, string groupName, string commercialName);
    }
}
