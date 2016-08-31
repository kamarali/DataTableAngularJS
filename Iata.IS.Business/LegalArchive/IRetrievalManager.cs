using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Iata.IS.Model.LegalArchive;

namespace Iata.IS.Business.LegalArchive
{
    public interface IRetrievalManager
    {
        /// <summary>
        /// Legal Archive Retrieval Process
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="archieveId"></param>
        /// <param name="jobSummaryId"></param>
        void LegalArchiveRetrievalProcess(string invoiceId, string archieveId, string jobSummaryId);

        /// <summary>
        /// Get all job summary based on member id 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns>list of job summary</returns>
        List<RetrivalJobSummaryGrid> GetRetrievedJobs(int memberId, int loggedInUserId);

        string GetJobSummaryId(int memberId, int loggedInUserId);

        /// <summary>
        /// Get all job details based on summary id 
        /// </summary>
        /// <param name="jobSummaryId"> job summary id</param>
        /// <param name="imgAltText">image alt text</param>
        /// <param name="imgToolTip">image tool tip</param>
        /// <param name="imgPath">image display url</param>
        /// <returns>list of job details</returns>
        List<RetrivalJobDetailGrid> GetJobDetailsByJobSummaryId(Guid jobSummaryId, string imgAltText, string imgToolTip, string imgPath);

        /// <summary>
        /// Following method returns job summary details for given job summary Id
        /// </summary>
        /// <param name="jobSummaryId">Job Summary Id</param>
        /// <returns>Job Summary details</returns>
        RetrievalJobSummary GetJobSummaryDetails(Guid jobSummaryId);

        /// <summary>
        /// Following method returns Archieve details object for given archieve Id
        /// </summary>
        /// <param name="archieveId">Archieve Id</param>
        /// <returns>Archieve details</returns>
        RetrievalJobDetails GetArchieveDetailsRecord(Guid invoiceID, Guid jobSummaryId, string IUA);

        /// <summary>
        /// Create Http Web request for archieve retrieval
        /// </summary>
        /// <param name="requestUrl">Web request url</param>
        /// <param name="userName">Web request username</param>
        /// <param name="password">Web request password</param>
        /// <returns>Http web request object</returns>
        HttpWebRequest CreateHttpWebRequestForArchieveRetrieval(string requestUrl, string userName, string password);

        /// <summary>
        /// Get retrieval job details list for given job summary Id.
        /// </summary>
        /// <param name="jobSummaryId">Job summary Id</param>
        /// <returns>Archieve retrieval job details list</returns>
        List<RetrievalJobDetails> GetRetrievalJobDetailsList(Guid jobSummaryId);
    }
}
