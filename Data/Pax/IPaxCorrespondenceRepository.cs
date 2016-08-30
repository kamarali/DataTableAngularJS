using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax
{
  public interface IPaxCorrespondenceRepository : IRepository<Correspondence>
  {
    IQueryable<Correspondence> GetCorrespondenceWithInvoice(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where);

    IQueryable<Correspondence> GetCorr(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where);

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
    Correspondence Single(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null);

    //SCP210204: IS-WEB Outage (Added new method)
    /// <summary>
    /// Gets the correspondence and attachment.
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns></returns>
    Correspondence GetCorrespondenceAndAttachment(Guid correspondenceId);

    /// <summary>
    /// Gets correspondence list along with attachments.
    /// </summary>
    /// <param name="where">The where expression.</param>
    /// <returns>List of correspondences along with attachments matching the where expression.</returns>
    IQueryable<Correspondence> GetCorrespondenceWithAttachment(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where);

    /// <summary>
    /// Gets correspondence list along with attachments And Members.
    /// </summary>
    /// <param name="where">The where expression.</param>
    /// <returns>List of correspondences along with attachments matching the where expression.</returns>
    IQueryable<Correspondence> GetCorrespondenceForTraiReport(
    System.Linq.Expressions.Expression<Func<Correspondence, bool>> where);

    long GetInitialCorrespondenceNumber(int memberId);

    /*SCP# 120094 - Pax Correspondence Download- Download All button 
    Desc: Comma seperated field in queue had size limit of 4000, now it is updated (to CLOB) to accomodate value of any length.
    Method below is to enqueue correspondences.
    Date: 10-May-2013*/
    void EnqueueCorrespondencesForDownloadReport(int memberId, int userId, String correspondenceNumbers, String downloadUrl);

    //SCP106534: ISWEB No-02350000768 
    //Desc: Calling SP to create corr.
    //Date: 20/06/2013
    int CreateCorrespondence(ref Correspondence correspondenceBaseRecord);

    /// <summary>
    /// Gets Only Correspondence from database Using Load Strategy. No other details like attachment, from member/ to member etc.. will be loaded.
    /// </summary>
    /// <param name="correspondenceId"></param>
    /// <returns></returns>
    Correspondence GetOnlyCorrespondenceUsingLoadStrategy(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null);

      IQueryable<Correspondence> GetLastRespondedCorrespondene(
          System.Linq.Expressions.Expression<Func<Correspondence, bool>> where);

      /// <summary>
      /// GEt first correspondence details
      /// </summary>
      /// <param name="correspondenceId"></param>
      /// <returns></returns>
      Correspondence GetFirstCorrespondence(Guid? correspondenceId = null);

  }
}
