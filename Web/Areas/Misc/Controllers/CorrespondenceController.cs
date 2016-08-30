using System;
using System.IO;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Security;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.Areas.MU.Controllers;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
  public class CorrespondenceController : CorrespondenceControllerBase
  {
    public CorrespondenceController(IMiscCorrespondenceManager miscCorrespondenceManager, ICalendarManager calendarManager, IAuthorizationManager authorizationManager, ICorrespondenceManager correspondenceManager)
      : base(miscCorrespondenceManager, calendarManager, authorizationManager, correspondenceManager)
    {}

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpGet]
    public ActionResult CreateCorrespondence(string invoiceId, string transactionId)
    {
      return base.CreateCorrespondenceBase(invoiceId, transactionId, ProcessingContactType.MiscCorrespondence);
    }
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    public override ActionResult ReadyToSubmitCorrespondence(string invoiceId, string transactionId, MiscCorrespondence correspondence)
    {
      return base.ReadyToSubmitCorrespondence(invoiceId, transactionId, correspondence);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    public override ActionResult CreateCorrespondence(string invoiceId, MiscCorrespondence correspondence)
    {
      return base.CreateCorrespondence(invoiceId, correspondence);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpPost]
    public override ActionResult CreateAndSendCorrespondence(string invoiceId, MiscCorrespondence correspondence)
    {
        
        return base.CreateAndSendCorrespondence(invoiceId, correspondence);
    }

    /// <summary>
    /// Opens the correspondence for edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public override ActionResult OpenCorrespondenceForEdit(string invoiceId, string transactionId)
    {
      return base.OpenCorrespondenceForEdit(invoiceId, transactionId);
    }

    /// <summary>
    /// Allows to edit correspondence.
    /// </summary>
    ///  <param name="invoiceId">The invoice Id</param>
    /// <param name="transactionId">The correspondence Id</param>
    /// <returns></returns>
  
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpGet]
    public override ActionResult EditCorrespondence(string invoiceId, string transactionId)
    {
     
      return base.EditCorrespondence(invoiceId, transactionId);
    }

    /// <summary>
    /// Update sampling form D record
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <param name="correspondence"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    public override ActionResult EditCorrespondence(string invoiceId, string transactionId, MiscCorrespondence correspondence)
    {
       
      return base.EditCorrespondence(invoiceId, transactionId, correspondence);
    }

    /// <summary>
    /// Used from invoice search result grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public override ActionResult ViewCorrespondence(string invoiceId)
    {
      return base.ViewCorrespondence(invoiceId);
    }

    /// <summary>
    /// Used from correspondence search result grid.
    /// </summary>
    /// <param name="invoiceId">Invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    [Obsolete]
    public override ActionResult ViewCorrespondenceDetails(string invoiceId, string transactionId)
    {
      return base.ViewCorrespondenceDetails(invoiceId, transactionId);
    }

    /// <summary>
    /// Views the linked correspondence.
    /// </summary>
    /// <param name="invoiceId">The correspondence id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public override ActionResult ViewLinkedCorrespondence(string invoiceId)
    {
      return base.ViewLinkedCorrespondence(invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public override ActionResult Correspondence(string invoiceId)
    {
      return base.Correspondence(invoiceId);
    }
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpPost]
    public override ActionResult SendCorrespondence(string invoiceId, string transactionId, MiscCorrespondence correspondence)
    {
       
        return base.SendCorrespondence(invoiceId, transactionId, correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpGet]
    public ActionResult ReplyCorrespondence(string invoiceId, string transactionId)
    {
      return base.ReplyCorrespondenceBase(invoiceId, transactionId, ProcessingContactType.MiscCorrespondence);
    }

    /// <summary>
    /// Fetch data for source code data and display in grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    public override JsonResult CorrespondenceRejectionGridData(string invoiceId)
    {
      return base.CorrespondenceRejectionGridData(invoiceId);
    }

    /// <summary>
    /// Fetch data for source code data and display in grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    public override JsonResult CorrespondenceHistoryGridData(string invoiceId)
    {
      return base.CorrespondenceHistoryGridData(invoiceId);
    }
    
    /// <summary>
    /// Upload multiple Correspondence Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit)]
    // SCP68888: errore sis
    // Permission changed from 'Misc.Receivables.Invoice.CreateOrEdit' to 'Misc.BillingHistoryAndCorrespondence.DraftCorrespondence'
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    public override JsonResult CorrespondenceAttachmentUpload(string invoiceId, string transactionId)
    {
      return base.CorrespondenceAttachmentUpload(invoiceId, transactionId);
    }

    /// <summary>
    /// Download Correspondence Attachment  
    /// </summary>
    //[ISAuthorize(Business.Security.Permissions.Misc.Receivables.Invoice.Download)]
    [HttpGet]
    public override FileStreamResult CorrespondenceAttachmentDownload(string attachmentId)
    {
      return base.CorrespondenceAttachmentDownload(attachmentId);
    }

    [HttpGet]
    public FilePathResult DownloadCorrespondence(string invoiceId, string transactionId)
    {
      FileStream file = null;
      try
      {
        string pdfPath = _miscCorrespondenceManager.CreateCorrespondenceFormatPdf(transactionId, ProcessingContactType.MiscCorrespondence);

        file = System.IO.File.Open(pdfPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return File(pdfPath, "application/pdf");
      }
      catch (ISBusinessException exception)
      {
        Logger.Error("Error while downloading correspondence.", exception);
        return null;
      }
      finally
      {
        if (file != null)
        {
          file.Close();
        }
      }
    }

    /// <summary>
    /// Upload Attachment 
    /// </summary>
    [HttpPost]
    public override JsonResult Upload(string invoiceId, string transactionId)
    {
      return base.Upload(invoiceId, transactionId);
    }

    /// <summary>
    /// This method will not allow the user to reply to correspondence if a correspondence invoice already exists 
    /// for that correspondence.
    /// </summary>
    /// <param name="correspondenceRefNumber"></param>
    /// <returns></returns>
    [HttpPost]
    public override JsonResult IsCorrespondenceInvoiceExistsForCorrespondence(long correspondenceRefNumber)
    {
      return base.IsCorrespondenceInvoiceExistsForCorrespondence(correspondenceRefNumber);
    }

    [HttpPost]
    public override JsonResult IsCorrespondenceOutSideTimeLimit(string invoiceId)
    {
      return base.IsCorrespondenceOutSideTimeLimit(invoiceId);
    }

    /// <summary>
    /// CMP 527:CloseCorrespondence
    /// </summary>
    /// <param name="correspondenceId">correspondence Id</param>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="correspondenceStage">correspondence stage</param>
    /// <param name="correspondenceStatus">correspondence status</param>
    /// <param name="correspondenceSubStatus">correspondence sub status</param>
    /// <param name="scenarioId">close scenario</param>
    /// <param name="userAcceptanceComment">acceptance comments</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    public override ActionResult CloseCorrespondence(string correspondenceId, string invoiceId, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioId, string userAcceptanceComment)
    {
      return base.CloseCorrespondence(correspondenceId,invoiceId, correspondenceStage, correspondenceStatus, correspondenceSubStatus, scenarioId, userAcceptanceComment);
    }
  }
}

