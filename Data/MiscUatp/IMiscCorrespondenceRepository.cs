using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Data.MiscUatp
{
  public interface IMiscCorrespondenceRepository : IRepository<MiscCorrespondence>
  {
    IQueryable<MiscCorrespondence> GetCorrespondenceWithInvoice(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where);

    /// <summary>
    /// Updates the correspondence status.
    /// </summary>
    List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold);
      /// <summary>
      /// Get single Correspondence record
      /// </summary>
      /// <param name="correspondenceId">The Correspondence Id</param>
      /// <param name="correspondenceNumber">The Correspondence Number</param>
      /// <param name="correspondenceStage">The Correspondence Stage</param>
      /// <returns>Correspondence record, if exists</returns>
      MiscCorrespondence Single(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null);
      /// <summary>
      /// Get filtered list of Correspondence records
      /// </summary>
      /// <param name="correspondenceNumber">The Correspondence Number</param>
      /// <param name="correspondenceStatusId">The Correspondence Status</param>
      /// <param name="authorityToBill">Authority To Bill</param>
      /// <param name="invoiceId">The Invoice</param>
      /// <param name="fromMemberId">The from member</param>
      /// <param name="correspondenceStage">The Correspondence Stage</param>
      /// <param name="correspondenceSubStatusId">The Correspondence sub status</param>
      /// <returns>List of filtered MiscCorrespondence records</returns>
      List<MiscCorrespondence> Get(long? correspondenceNumber = null,
                                   int? correspondenceStatusId = null,
                                   bool? authorityToBill = null,
                                   Guid? invoiceId = null,
                                   int? fromMemberId = null,
                                   int? correspondenceStage = null,
                                   int? correspondenceSubStatusId = null);

    IQueryable<MiscCorrespondence> GetCorr(Expression<Func<MiscCorrespondence,bool>> where);

    IQueryable<MiscCorrespondence> GetCorrespondenceForTraiReport(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where);

    long GetInitialCorrespondenceNumber(int memberId);

    IQueryable<MiscCorrespondence> GetLastRespondedCorrespondene(
          System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where);

  }
}
