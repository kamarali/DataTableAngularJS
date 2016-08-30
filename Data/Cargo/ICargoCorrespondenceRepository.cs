using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
  public interface ICargoCorrespondenceRepository : IRepository<CargoCorrespondence>
  {
    IQueryable<CargoCorrespondence> GetCorrespondenceWithInvoice(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where);

    IQueryable<CargoCorrespondence> GetCorr(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where);

    /// <summary>
    /// Updates the correspondence status.
    /// </summary>
    List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold);

    /// <summary>
    /// Get single Correspondence record
    /// </summary>
    /// <param name="correspondenceId">The Correspondence Id</param>
    /// <param name="correspondenceNumber">The Correspondence Number</param>
    /// <param name="correspondenceStage">The Correspondence Stage</param>
    /// <returns>Correspondence record, if exists</returns>
    CargoCorrespondence Single(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null);

    /// <summary>
    /// Gets correspondence list along with attachments.
    /// </summary>
    /// <param name="where">The where expression.</param>
    /// <returns>List of correspondences along with attachments matching the where expression.</returns>
    IQueryable<CargoCorrespondence> GetCorrespondenceWithAttachment(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where);

    /// <summary>
    /// Gets correspondence list along with attachments And Members.
    /// </summary>
    /// <param name="where">The where expression.</param>
    /// <returns>List of correspondences along with attachments matching the where expression.</returns>
    IQueryable<CargoCorrespondence> GetCorrespondenceForTraiReport(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where);

    long GetInitialCorrespondenceNumber(int memberId);

    //SCP106534: ISWEB No-02350000768 
    //Desc: Calling SP to create corr.
    //Date: 20/06/2013
    int CreateCorrespondence(ref CargoCorrespondence correspondenceBaseRecord);

      /// <summary>
      /// Gets Only Correspondence from database Using Load Strategy.
      /// </summary>
      /// <param name="correspondenceId"></param>
      /// <returns></returns>
    CargoCorrespondence GetOnlyCorrespondenceUsingLoadStrategy(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null);

    IQueryable<CargoCorrespondence> GetLastRespondedCorrespondene(
          System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where);

  }
}
