using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.LegalArchive;

namespace Iata.IS.Data.LegalArchive.Impl
{
    public class ArchiveSearchRepository : Repository<LegalArchiveLog>, IArchiveSearchRepository
    {
        /// <summary>
        /// Gets or sets the retrival job detail repository.
        /// </summary>
        public IRepository<RetrievalJobDetails> RetrievalJobDetailRepository { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveSearchRepository"/> class.
        /// </summary>
        public ArchiveSearchRepository()
        {
            InitializeObjectSet();
        }

        /// <summary>
        /// Initializes the object set.
        /// </summary>
        public override sealed void InitializeObjectSet()
        {
            EntityObjectSet = Context.CreateObjectSet<LegalArchiveLog>();
        }

        /// <summary>
        /// Get list of all legal archive search
        /// </summary>
        public IQueryable<LegalArchiveLog> GetAllLegalArchiveLog()
        {
            return EntityObjectSet;
        }

        /// <summary>
        /// add retrival archive job detail
        /// </summary>
        /// <param name="archiveDetail">archive detail to be Added.</param>
        /// /// <returns></returns>
        public void AddRetrivalArchiveJobDetail(RetrievalJobDetails archiveDetail)
        {
            RetrievalJobDetailRepository.Add(archiveDetail);
        }
        /*
        /// <summary>
        /// add retrival archive job summary
        /// </summary>
        /// <param name="archiveDetail">archive detail to be Added.</param>
        /// /// <returns></returns>
        public void AddRetrivalArchiveJobSummary(RetrievalJobSummary archiveDetail)
        {
            RetrievalJobSummaryRepository.Add(archiveDetail);
        }*/
    }
}