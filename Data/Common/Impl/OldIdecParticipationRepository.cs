using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class OldIdecParticipationRepository : Repository<OldIdecParticipation>, IOldIdecParticipationRepository
    {
        /// <summary>
        /// Gets all old idec participations.
        /// </summary>
        /// <returns></returns>
        public IQueryable<OldIdecParticipation> GetAllOldIdecParticipations()
        {
            var OldIdecParticipationsList = EntityObjectSet.Include("Member");
            return OldIdecParticipationsList;
        }

        /// <summary>
        /// Gets the old idec participation.
        /// </summary>
        /// <param name="OldIdecParticipationId">The old idec participation id.</param>
        /// <returns></returns>
        public OldIdecParticipation GetOldIdecParticipation(int OldIdecParticipationId)
        {
            OldIdecParticipation oldIdecParticipation = EntityObjectSet.Include("Member").Where(op => op.Id == OldIdecParticipationId).FirstOrDefault();
            return oldIdecParticipation;
        }
    }
}
