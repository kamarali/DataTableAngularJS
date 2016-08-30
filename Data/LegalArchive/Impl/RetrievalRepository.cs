using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.LegalArchive;
using System.Data.Objects;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.LegalArchive.Impl
{
    public class RetrievalRepository : Repository<InvoiceBase>, IRetrievalRepository
    {
        private const string RetrievalJobSummaryGrid = "RetrievalJobSummaryFunction";
        private const string RetrievalJobDetailsGrid = "RetrievalJobDetailsFunction";

        /// <summary>
        /// Get all job summary based on member id 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns>list of job summary</returns>
        public List<RetrivalJobSummaryGrid> GetRetrievalJobSummaryByMemberId(int memberId, int loggedInUserId)
        {
            var parameters = new ObjectParameter[2];

            parameters[0] = new ObjectParameter("MEMBER_ID_i", typeof(int)) { Value = memberId };

            parameters[1] = new ObjectParameter("USER_ID_i", typeof(int)) { Value = loggedInUserId };

            // call SP FOR PROC_GET_RET_JOBSUMMARYDETAIL
            var list = ExecuteStoredFunction<RetrivalJobSummaryGrid>(RetrievalJobSummaryGrid, parameters);

            return list.ToList();
        }

        /// <summary>
        /// Get all job details based on summary id 
        /// </summary>
        /// <param name="jobSummaryId"></param>
        /// <returns>list of job details</returns>
        public List<RetrivalJobDetailGrid> GetRetrievalJobDetailsByJobSummaryId(Guid jobSummaryId)
        {
            var parameters = new ObjectParameter[1];

            parameters[0] = new ObjectParameter("JOB_SUMMARY_ID_i", typeof(Guid)) { Value = jobSummaryId };

            // CALL SP FOR PROC_GET_RET_JOBDETAIL
            var list = ExecuteStoredFunction<RetrivalJobDetailGrid>(RetrievalJobDetailsGrid, parameters);

            return list.ToList();
        }
    }
}
