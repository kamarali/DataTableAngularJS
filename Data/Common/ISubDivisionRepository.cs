using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.Common
{
    public interface ISubDivisionRepository : IRepository<SubDivision>
    {
        IQueryable<SubDivision> GetAllSubDivisionCodes();

        SubDivision GetSubDivisionDetail(string subDivisionId, string countryId);
    }
}
