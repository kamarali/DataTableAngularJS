using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common
{
    public interface IOldIdecParticipationRepository : IRepository<OldIdecParticipation>
    {
        /// <summary>
        /// Gets all old idec participations.
        /// </summary>
        /// <returns></returns>
        IQueryable<OldIdecParticipation> GetAllOldIdecParticipations();
        
        /// <summary>
        /// Gets the old idec participation.
        /// </summary>
        /// <param name="OldIdecParticipationId">The old idec participation id.</param>
        /// <returns></returns>
        OldIdecParticipation GetOldIdecParticipation(int OldIdecParticipationId);
    }
}
