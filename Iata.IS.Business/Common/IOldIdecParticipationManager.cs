using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IOldIdecParticipationManager
    {
        /// <summary>
        /// Adds the old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <returns></returns>
        OldIdecParticipation AddOldIdecParticipation(OldIdecParticipation oldIdecParticipation);

        /// <summary>
        /// Updates the old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <returns></returns>
        OldIdecParticipation UpdateOldIdecParticipation(OldIdecParticipation oldIdecParticipation);

        /// <summary>
        /// Deletes the old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipationId">The old idec participation id.</param>
        /// <returns></returns>
        bool DeleteOldIdecParticipation(int oldIdecParticipationId);

        /// <summary>
        /// Gets the old idec participation details.
        /// </summary>
        /// <param name="oldIdecParticipationId">The old idec participation id.</param>
        /// <returns></returns>
        OldIdecParticipation GetOldIdecParticipationDetails(int oldIdecParticipationId);

        /// <summary>
        /// Gets all old idec participation list.
        /// </summary>
        /// <returns></returns>
        List<OldIdecParticipation> GetAllOldIdecParticipationList();

        /// <summary>
        /// Gets the old idec participation list.
        /// </summary>
        /// <param name="MemberId">The member id.</param>
        /// <returns></returns>
        List<OldIdecParticipation> GetOldIdecParticipationList(int MemberId);
    }
}
