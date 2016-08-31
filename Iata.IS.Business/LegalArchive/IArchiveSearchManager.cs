using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.LegalArchive;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Common;
using System;

namespace Iata.IS.Business.LegalArchive
{
    /// <summary>
    /// This interface is specific to invoice search functionality
    /// </summary>
    public interface IArchiveSearchManager
    {
        /// <summary>
        /// Gets invoices matching the specified search criteria
        /// </summary>
        /// <param name="searchCriteria">Search Criteria</param>
        /// <param name="memberId">id of member</param>
        /// <returns>Invoices matching the search criteria.</returns>
        IQueryable<LegalArchiveSearch> SearchArchives(LegalArchiveSearchCriteria searchCriteria, int memberId);

        /// <summary>
        /// Get all selected legal archives
        /// </summary>
        /// <param name="archiveIdList">archiveIdList</param>
        /// <param name="userId">user id </param>
        /// <param name="memberId">member id </param>
        /// <param name="searchCriteria">actual search criteria choose by user</param>
        /// <param name="jobId">out parameters for job id </param>
        /// <param name="totalNoInvoicesRetrieved">total no of invoices retrive</param>
        /// <returns>true if job_id is successfully created or flase</returns>
        bool RetriveLegalArchive(List<string> archiveIdList, int userId, int memberId,LegalArchiveSearchCriteria searchCriteria, out string jobId,out int totalNoInvoicesRetrieved);
        
        /// <summary>
        /// get job summary object from archive log 
        /// </summary>
        /// <param name="memberData">member object</param>
        /// <param name="userData">user object</param>
        /// <param name="jobId">job id</param>
        /// <param name="totalInvoiceRetrive">total no of invoice retrive</param>
        /// <param name="searchCriteria">archive search criteria</param>
        /// <returns>RetrievalJobSummary object</returns>
        RetrievalJobSummary GetJobSummary(Member memberData, User userData, string jobId, int totalInvoiceRetrive, LegalArchiveSearchCriteria searchCriteria);

        /// <summary>
        /// get job summary model object
        /// </summary>
        /// <param name="archiveLog">LegalArchiveLog</param>
        /// <param name="jobSummaryId">job summary id</param>
        /// <param name="userId">user id</param>
        ///<returns>RetrievalJobDetails object</returns>
        RetrievalJobDetails GetJobDetail(LegalArchiveLog archiveLog, Guid jobSummaryId, int userId);
        
        /// <summary>
        /// get search criteria object
        /// </summary>
        /// <param name="modelValues">values of search criteria</param>
        ///<returns>RetrievalJobDetails object</returns>
        LegalArchiveSearchCriteria GetSearchCriteria(List<string> modelValues);

        /// <summary>
        /// To send email notification to IS admin about unexpected errror.
        /// </summary>
        /// <param name="erroMessage">"error message"</param>
        /// <param name="serviceName">"service name"</param>
        /// <param name="memberId">"member id"</param>
        /// <returns>bool</returns>
        bool SendUnexpectedErrorNotificationToISAdmin(string serviceName, string erroMessage, int memberId);
    }
}
