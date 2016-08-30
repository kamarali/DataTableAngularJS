using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.Common.Impl
{
    public class SubDivisionRepository : Repository<SubDivision>, ISubDivisionRepository
    {
        public IQueryable<SubDivision> GetAllSubDivisionCodes()
        {
            var subDivisionCodesList = EntityObjectSet.Include("Country");
            return subDivisionCodesList;
        }

        public SubDivision GetSubDivisionDetail(string subDivisionId, string countryId)
        {
            var subDivision = EntityObjectSet.Include("Country").Where(sub => sub.Id.Trim() == subDivisionId.Trim() && sub.CountryId.Trim() == countryId.Trim()).FirstOrDefault();
            return subDivision;
        }
    }
}
