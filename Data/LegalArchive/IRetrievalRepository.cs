using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.LegalArchive;

namespace Iata.IS.Data.LegalArchive
{
    public interface IRetrievalRepository
    {
        /// <summary>
        /// Get all job summary based on member id 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns>list of job summary</returns>
        List<RetrivalJobSummaryGrid> GetRetrievalJobSummaryByMemberId(int memberId, int loggedInUserId);

        /// <summary>
        /// Get all job details based on summary id 
        /// </summary>
        /// <param name="jobSummaryId"></param>
        /// <returns>list of job details</returns>
        List<RetrivalJobDetailGrid> GetRetrievalJobDetailsByJobSummaryId(Guid jobSummaryId);
    }
}
