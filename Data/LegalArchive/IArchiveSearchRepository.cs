using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.LegalArchive;

namespace Iata.IS.Data.LegalArchive
{
    public interface IArchiveSearchRepository
    {
        /// <summary>
        /// Get list of all legal archive logs
        /// </summary>
        IQueryable<LegalArchiveLog> GetAllLegalArchiveLog();

        /// <summary>
        /// add retrival archive job detail
        /// </summary>
        /// <param name="archiveDetail">archive lot to be Added.</param>
        /// /// <returns></returns>
        void AddRetrivalArchiveJobDetail(RetrievalJobDetails archiveDetail);

    }
}
