using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
    public interface ISubDivisionManager
    {
        /// <summary>
        /// Adds the sub division.
        /// </summary>
        /// <param name="subDivision">The sub division.</param>
        /// <returns></returns>
        SubDivision AddSubDivision(SubDivision subDivision);

        /// <summary>
        /// Updates the sub division.
        /// </summary>
        /// <param name="subDivision">The sub division.</param>
        /// <returns></returns>
        SubDivision UpdateSubDivision(SubDivision subDivision);

        /// <summary>
        /// Deletes the sub division.
        /// </summary>
        /// <param name="subDivisionId">The sub division id.</param>
        /// <returns></returns>
        bool DeleteSubDivision(string subDivisionId, string countryId);

        /// <summary>
        /// Gets the sub division details.
        /// </summary>
        /// <param name="subDivisionId">The sub division id.</param>
        /// <returns></returns>
        SubDivision GetSubDivisionDetails(string subDivisionId,string countryId);

        /// <summary>
        /// Gets all sub division list.
        /// </summary>
        /// <returns></returns>
        List<SubDivision> GetAllSubDivisionList();

        /// <summary>
        /// Gets the sub division list.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="SubDivisionCodeIcao">The sub division code icao.</param>
        /// <returns></returns>
        List<SubDivision> GetSubDivisionList(string Id, string Name, string CountryId);
    }
}
