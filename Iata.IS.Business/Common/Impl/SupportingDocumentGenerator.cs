using System;
using System.Linq;
using System.Text;
using Iata.IS.Business.FileCore;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Base;
using System.IO;
using Iata.IS.Model.Pax.Sampling;
using log4net;
using Iata.IS.Core.DI;

namespace Iata.IS.Business.Common.Impl
{
  public class SupportingDocumentGenerator : ISupportingDocumentGenerator
  {
    public IReferenceManager ReferenceManager { private get; set; }
    public IBillingMemoAttachmentRepository BillingMemoAttachmentRepository { get; set; }
    public ICreditMemoAttachmentRepository CreditMemoAttachmentRepository { get; set; }
    public IRejectionMemoAttachmentRepository RejectionMemoAttachmentRepository { get; set; }
    public IBillingMemoCouponAttachmentRepository BillingMemoCouponAttachmentRepository { get; set; }
    public ICreditMemoCouponAttachmentRepository CreditMemoCouponAttachmentRepository { get; set; }
    public IRejectionMemoCouponAttachmentRepository RejectionMemoCouponAttachmentRepository { get; set; }
    public ICouponRecordAttachmentRepository CouponRecordAttachmentRepository { get; set; }
    public ISamplingFormCAttachmentRepository SamplingFormCAttachmentRepository { get; set; }
    public ISamplingFormDAttachmentRepository SamplingFormDAttachmentRepository { get; set; }
    public IMiscUatpInvoiceAttachmentRepository MiscUatpInvoiceAttachmentRepository { get; set; }


    public ICargoAwbAttachmentRepository CargoAwbAttachmentRepository { get; set; }
    public IBMAwbAttachmentRepository BMAwbAttachmentRepository { get; set; }
    public ICargoBillingMemoAttachmentRepository CargoBillingMemoAttachmentRepository { get; set; }
    public ICargoCreditMemoAttachmentRepository CargoCreditMemoAttachmentRepository { get; set; }
    public ICargoCreditMemoAwbAttachmentRepository CMAwbAttachmentRepository { get; set; }
    public ICgoRejectionMemoAttachmentRepository CgoRejectionMemoAttachmentRepository { get; set; }
    public IRMAwbAttachmentRepository RMAwbAttachmentRepository { get; set; }

    //private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public SupportingDocumentGenerator()
    {
    }

    #region Implementation of ISupportingDocumentGenerator

    /// <summary>
    /// Creates the supporting document.
    /// </summary>
    public bool CreateSupportingDocument(PaxInvoice paxInvoice, string supportingDocPath, StringBuilder errors, ILog logger)
    {
      //throw new NotImplementedException();
      return CopySupportingDocuments(paxInvoice, supportingDocPath, errors, logger);

    }

    public bool CreateSupportingDocument(CargoInvoice cgoInvoice, string supportingDocPath, StringBuilder errors, ILog logger)
    {
      //throw new NotImplementedException();
      return CopySupportingDocuments(cgoInvoice, supportingDocPath, errors, logger);

    }

    /// <summary>
    /// Creates the form C supporting document.
    /// </summary>
    /// <param name="samplingFormCRecords">The sampling form C records.</param>
    /// <param name="supportingDocumentPath">The supporting document path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="requestViaIsWeb">if requested via is web then do not change the file path for the attachment.</param>
    public void CreateFormCSupportingDocument(List<SamplingFormCRecord> samplingFormCRecords, string supportingDocumentPath, StringBuilder errors, ILog logger, bool requestViaIsWeb = false)
    {
      try
      {
        logger.Info("Creating Form C Supporting Documents");
        FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
        // loop through SamplingFormCRecords
        foreach (var samplingFormCRecord in samplingFormCRecords)
        {
          //Create sub folder
          if (samplingFormCRecord.Attachments.Count > 0)
          {
            var batchSequenceFolderPath = CreateProvisionalInvoiceBatchSequenceFolder(supportingDocumentPath,
                                                                                      samplingFormCRecord.ProvisionalInvoiceNumber,
                                                                                      samplingFormCRecord.BatchNumberOfProvisionalInvoice,
                                                                                      samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice);
            logger.InfoFormat("Sub-folder [{0}] created.", batchSequenceFolderPath);

            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in samplingFormCRecord.Attachments)
            {
              if (!attachment.IsFullPath && !File.Exists(attachment.FilePath))
              {
                CopyAttachments(attachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger,
                                requestViaIsWeb);

                if (!requestViaIsWeb)
                {
                  SamplingFormCAttachmentRepository.Update(attachment);
                }
              }
              else if (requestViaIsWeb)
              {
                File.Copy(attachment.FilePath, Path.Combine(batchSequenceFolderPath, attachment.OriginalFileName));

              }
            }
            logger.InfoFormat("Attachments for invoice # [{0}] ; sampling form C record [{1}] copied.", samplingFormCRecord.ProvisionalInvoiceNumber, samplingFormCRecord.Id);
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error("Error copying supporting attachments", ex);
        errors.AppendFormat("Error copying supporting attachments.{0} Exception:{1} ", Environment.NewLine, ex);
      }
    }

    /// <summary>
    /// Method does following:
    /// 1. If requestViaIsWeb is false then; moves attachment to given destination folder and updates the attachment path in attachment table.
    /// 2. If requestViaIsWeb is true then; copies attachment to given destination folder.
    /// </summary>
    /// <param name="attachment"></param>
    /// <param name="basePath"></param>
    /// <param name="folderPath"></param>
    /// <param name="errors"></param>
    /// <param name="logger"></param>
    /// <param name="requestViaIsWeb"></param>
    private void CopyAttachments(Attachment attachment, string basePath, string folderPath, StringBuilder errors, ILog logger, bool requestViaIsWeb = false)
    {
      var fileExtension = attachment.OriginalFileName.Substring(attachment.OriginalFileName.LastIndexOf('.'));

      var sourceFile = Path.Combine(basePath, attachment.FilePath, attachment.Id.ToString());
      sourceFile = Path.ChangeExtension(sourceFile, fileExtension);
      logger.Info("Source File:" + sourceFile);

      var destinationFile = Path.Combine(folderPath, attachment.OriginalFileName);
      logger.Info("Destination File:" + destinationFile);

      if (File.Exists(sourceFile))
      {
        if (!requestViaIsWeb)
        {
          attachment.IsFullPath = true;
          attachment.FilePath = destinationFile;
          FileIo.MoveFile(sourceFile, destinationFile);
        }
        else
        {
          File.Copy(sourceFile, destinationFile);
        }       
      }
      else
      {
        logger.InfoFormat("File [{0}] not found on server", sourceFile);
        errors.AppendFormat("File [{0}] not found on server", sourceFile);
      }

    }

    private bool CopySupportingDocuments(PaxInvoice invoice, string destinationFolderPath, StringBuilder errors, ILog logger)
    {
      try
      {
        FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
        bool isFolderCreated = false;

        // Add Prime Coupon attachments
        foreach (var coupon in invoice.CouponDataRecord)
        {
          //Create sub folder
          var batchSequenceFolderPath = coupon.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, coupon.BatchSequenceNumber, coupon.RecordSequenceWithinBatch) : string.Empty;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in coupon.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = CouponRecordAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            CouponRecordAttachmentRepository.Update(dbAttachment);
          }
        }

        #region Commented Code
        //var couponRepository = Ioc.Resolve<ICouponRecordRepository>(typeof(ICouponRecordRepository));
        //var couponRecordAttachmentRepository = Ioc.Resolve<ICouponRecordAttachmentRepository>(typeof(ICouponRecordAttachmentRepository));
        //var invoiceCouponList = couponRepository.Get(coupon => coupon.InvoiceId == invoice.Id).Select(coupon => new {
        //                                                                                                              coupon.Id,
        //                                                                                                              coupon.BatchSequenceNumber,
        //                                                                                                              coupon.RecordSequenceWithinBatch
        //                                                                                                            }).ToList();
        //logger.DebugFormat("Coupon Count:{0}", invoiceCouponList.Count);

        //if (invoiceCouponList.Count > 0)
        //{

        //  var distinctCouponIdList = invoiceCouponList.Select(coupon => coupon.Id);
        //  var allCouponAttachments = couponRecordAttachmentRepository.Get(attachment => (attachment.ParentId == null ? false : distinctCouponIdList.Contains(attachment.ParentId.Value))).ToList();

        //    // Add Prime Coupon attachments);)
        //    foreach (var coupon in invoiceCouponList) //invoice.CouponDataRecord)
        //    {
        //        var couponRecordAttachments =
        //            allCouponAttachments.Where(attachment => attachment.ParentId == coupon.Id).ToList();

        //        //Create sub folder
        //        var batchSequenceFolderPath = couponRecordAttachments.Count > 0
        //                                          ? CreateBatchSequenceFolder(destinationFolderPath,
        //                                                                      coupon.BatchSequenceNumber,
        //                                                                      coupon.RecordSequenceWithinBatch)
        //                                          : string.Empty;

        //        foreach (var attachment in couponRecordAttachments) //coupon.Attachments)
        //        {
        //            if (!attachment.IsFullPath)
        //            {
        //                CopyAttachments(attachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
        //                CouponRecordAttachmentRepository.Update(attachment);
        //            }
        //        }
        //        couponRecordAttachments.Clear();
        //        couponRecordAttachments = null;
        //    }
        //}
        //Ioc.Release(couponRecordAttachmentRepository);
        //Ioc.Release(couponRepository); 
        #endregion

        // Add Billing Memo attachments
        foreach (var billingMemo in invoice.BillingMemoRecord)
        {
          //Create sub folder
          var batchSequenceFolderPath = billingMemo.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, billingMemo.BatchSequenceNumber, billingMemo.RecordSequenceWithinBatch) : string.Empty;
          isFolderCreated = false;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in billingMemo.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = BillingMemoAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            BillingMemoAttachmentRepository.Update(dbAttachment);
          }

          // Add BM Coupon attachments
          foreach (var bmCoupon in billingMemo.CouponBreakdownRecord)
          {
            if (billingMemo.Attachments.Count == 0 && !isFolderCreated && bmCoupon.Attachments.Count > 0)
            {
              batchSequenceFolderPath = CreateBatchSequenceFolder(destinationFolderPath, billingMemo.BatchSequenceNumber, billingMemo.RecordSequenceWithinBatch);
              isFolderCreated = true;
            }
            // Create BreakdownSerialNumber folder
            var breakdownSerialNumberFolderPath = bmCoupon.Attachments.Count > 0 ? CreateBreakdownSerialNumberFolder(batchSequenceFolderPath, bmCoupon.SerialNo) : string.Empty;

            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in bmCoupon.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
            {
              //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
              //Refresh attachment object from database.
              var dbAttachment = BillingMemoCouponAttachmentRepository.Single(attac => attac.Id == attachment.Id);
              CopyAttachments(dbAttachment, fileServer.BasePath, breakdownSerialNumberFolderPath, errors, logger);
              BillingMemoCouponAttachmentRepository.Update(dbAttachment);
            }
          }
        }

        // Add Credit Memo attachments
        foreach (var creditMemo in invoice.CreditMemoRecord)
        {
          //Create sub folder
          var batchSequenceFolderPath = creditMemo.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, creditMemo.BatchSequenceNumber, creditMemo.RecordSequenceWithinBatch) : string.Empty;
          isFolderCreated = false;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in creditMemo.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = CreditMemoAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            CreditMemoAttachmentRepository.Update(dbAttachment);
          }

          // Add BM Coupon attachments
          foreach (var cmCoupon in creditMemo.CouponBreakdownRecord)
          {
            if (creditMemo.Attachments.Count == 0 && !isFolderCreated && cmCoupon.Attachments.Count > 0)
            {
              batchSequenceFolderPath = CreateBatchSequenceFolder(destinationFolderPath, creditMemo.BatchSequenceNumber, creditMemo.RecordSequenceWithinBatch);
              isFolderCreated = true;
            }
            // Create BreakdownSerialNumber folder
            var breakdownSerialNumberFolderPath = cmCoupon.Attachments.Count > 0 ? CreateBreakdownSerialNumberFolder(batchSequenceFolderPath, cmCoupon.SerialNo) : string.Empty;

            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in cmCoupon.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
            {
              //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
              //Refresh attachment object from database.
              var dbAttachment = CreditMemoCouponAttachmentRepository.Single(attac => attac.Id == attachment.Id);
              CopyAttachments(dbAttachment, fileServer.BasePath, breakdownSerialNumberFolderPath, errors, logger);
              CreditMemoCouponAttachmentRepository.Update(dbAttachment);
            }
          }
        }

        // Add Rejection Memo attachments
        foreach (var rejectionMemo in invoice.RejectionMemoRecord)
        {
          //Create sub folder
          var batchSequenceFolderPath = rejectionMemo.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, rejectionMemo.BatchSequenceNumber, rejectionMemo.RecordSequenceWithinBatch) : string.Empty;
          isFolderCreated = false;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in rejectionMemo.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = RejectionMemoAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            RejectionMemoAttachmentRepository.Update(dbAttachment);
          }

          // Add RM Coupon attachments
          foreach (var rmCoupon in rejectionMemo.CouponBreakdownRecord)
          {
            if (rejectionMemo.Attachments.Count == 0 && !isFolderCreated && rmCoupon.Attachments.Count > 0)
            {
              batchSequenceFolderPath = CreateBatchSequenceFolder(destinationFolderPath, rejectionMemo.BatchSequenceNumber, rejectionMemo.RecordSequenceWithinBatch);
              isFolderCreated = true;
            }
            // Create BreakdownSerialNumber folder
            var breakdownSerialNumberFolderPath = rmCoupon.Attachments.Count > 0 ? CreateBreakdownSerialNumberFolder(batchSequenceFolderPath, rmCoupon.SerialNo) : string.Empty;

            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in rmCoupon.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
            {
              //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
              //Refresh attachment object from database.
              var dbAttachment = RejectionMemoCouponAttachmentRepository.Single(attac => attac.Id == attachment.Id);
              CopyAttachments(dbAttachment, fileServer.BasePath, breakdownSerialNumberFolderPath, errors, logger);
              RejectionMemoCouponAttachmentRepository.Update(dbAttachment);
            }
          }
        }

        // Add samplingFormDRecord attachments
        foreach (var samplingFormDRecord in invoice.SamplingFormDRecord)
        {
          //Create sub folder
          var batchSequenceFolderPath = samplingFormDRecord.Attachments.Count > 0 ? CreateProvisionalInvoiceBatchSequenceFolder(destinationFolderPath, samplingFormDRecord.ProvisionalInvoiceNumber, samplingFormDRecord.BatchNumberOfProvisionalInvoice, samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice) : string.Empty;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in samplingFormDRecord.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = SamplingFormDAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            SamplingFormDAttachmentRepository.Update(dbAttachment);
          }
        }

        //Commit the transaction for all the attachment repositories
        UnitOfWork.CommitDefault();
      }
      catch (Exception ex)
      {
        logger.Error("Error copying supporting attachments", ex);
        errors.AppendFormat("Error copying supporting attachments - {0}", ex.Message);
        return false;
      }
      return true;
    }


    private bool CopySupportingDocuments(CargoInvoice cgoInvoice, string destinationFolderPath, StringBuilder errors, ILog logger)
    {
      try
      {
        FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
        bool isFolderCreated = false;
        // Add AWB attachments
        logger.InfoFormat("No Cgo AWBs {0}", cgoInvoice.AwbDataRecord.Count);
        foreach (var awb in cgoInvoice.AwbDataRecord)
        {
          //Create sub folder
          var batchSequenceFolderPath = awb.Attachments.Count > 0
                                          ? CreateBatchSequenceFolder(destinationFolderPath, awb.BatchSequenceNumber,
                                                                      awb.RecordSequenceWithinBatch)
                                          : string.Empty;

          logger.InfoFormat("No of Cgo AWB attachments {0}", awb.Attachments.Count);

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in awb.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = CargoAwbAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            CargoAwbAttachmentRepository.Update(dbAttachment);
          }
        }
        logger.InfoFormat("No of Billing Memos {0}", cgoInvoice.CGOBillingMemo.Count);
        // Add Billing Memo attachments);
        foreach (var billingMemo in cgoInvoice.CGOBillingMemo)
        {
          //Create sub folder
          logger.InfoFormat("No of Billing Memos attachment {0}", billingMemo.Attachments.Count);

          var batchSequenceFolderPath = billingMemo.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, billingMemo.BatchSequenceNumber, billingMemo.RecordSequenceWithinBatch) : string.Empty;
          isFolderCreated = false;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in billingMemo.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = CargoBillingMemoAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            CargoBillingMemoAttachmentRepository.Update(dbAttachment);
          }

          // Add BM AWB attachments
          logger.InfoFormat("No of Billing Memo AWBs {0}", billingMemo.AwbBreakdownRecord.Count);
          foreach (var bmAwb in billingMemo.AwbBreakdownRecord)
          {
            if (billingMemo.Attachments.Count == 0 && !isFolderCreated && bmAwb.Attachments.Count > 0)
            {
              batchSequenceFolderPath = CreateBatchSequenceFolder(destinationFolderPath, billingMemo.BatchSequenceNumber, billingMemo.RecordSequenceWithinBatch);
              isFolderCreated = true;
            }
            // Create BreakdownSerialNumber folder
            var breakdownSerialNumberFolderPath = bmAwb.Attachments.Count > 0 ? CreateBreakdownSerialNumberFolder(batchSequenceFolderPath, bmAwb.BdSerialNumber) : string.Empty;

            logger.InfoFormat("No of Billing Memo AWB attachments {0}", bmAwb.Attachments.Count);
            
            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in bmAwb.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
            {
              //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
              //Refresh attachment object from database.
              var dbAttachment = BMAwbAttachmentRepository.Single(attac => attac.Id == attachment.Id);

              CopyAttachments(dbAttachment, fileServer.BasePath, breakdownSerialNumberFolderPath, errors, logger);
              BMAwbAttachmentRepository.Update(dbAttachment);
            }
          }
        }

        logger.InfoFormat("No of Credit Memos {0}", cgoInvoice.CGOCreditMemo.Count);
        // Add Credit Memo attachments
        foreach (var creditMemo in cgoInvoice.CGOCreditMemo)
        {
          //Create sub folder
          logger.InfoFormat("No of Credit Memos attachment {0}", creditMemo.Attachments.Count);
          var batchSequenceFolderPath = creditMemo.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, creditMemo.BatchSequenceNumber, creditMemo.RecordSequenceWithinBatch) : string.Empty;
          isFolderCreated = false;

          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in creditMemo.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = CargoCreditMemoAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            CargoCreditMemoAttachmentRepository.Update(dbAttachment);
          }

          // Add CM AWB attachments
          logger.InfoFormat("No of Credit Memo AWBs {0}", creditMemo.AWBBreakdownRecord.Count);
          foreach (var cmAwb in creditMemo.AWBBreakdownRecord)
          {
            if (creditMemo.Attachments.Count == 0 && !isFolderCreated && cmAwb.Attachments.Count > 0)
            {
              batchSequenceFolderPath = CreateBatchSequenceFolder(destinationFolderPath, creditMemo.BatchSequenceNumber, creditMemo.RecordSequenceWithinBatch);
              isFolderCreated = true;
            }
            // Create BreakdownSerialNumber folder
            var breakdownSerialNumberFolderPath = cmAwb.Attachments.Count > 0 ? CreateBreakdownSerialNumberFolder(batchSequenceFolderPath, cmAwb.BdSerialNumber) : string.Empty;

            logger.InfoFormat("No of Credit Memo AWB attachment {0}", cmAwb.Attachments.Count);

            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in cmAwb.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
            {
              //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
              //Refresh attachment object from database.
              var dbAttachment = CMAwbAttachmentRepository.Single(attac => attac.Id == attachment.Id);
              CopyAttachments(dbAttachment, fileServer.BasePath, breakdownSerialNumberFolderPath, errors, logger);
              CMAwbAttachmentRepository.Update(dbAttachment);
            }
          }
        }

        // Add Rejection Memo attachments
        logger.InfoFormat("No of Rejection Memos {0}", cgoInvoice.CGORejectionMemo.Count);
        foreach (var rejectionMemo in cgoInvoice.CGORejectionMemo)
        {
          //Create sub folder
          var batchSequenceFolderPath = rejectionMemo.Attachments.Count > 0 ? CreateBatchSequenceFolder(destinationFolderPath, rejectionMemo.BatchSequenceNumber, rejectionMemo.RecordSequenceWithinBatch) : string.Empty;
          isFolderCreated = false;
          logger.InfoFormat("No of Rejection Memo attachments {0}", rejectionMemo.Attachments.Count);
          
          // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
          // added check for, if file path does not exists.
          foreach (var attachment in rejectionMemo.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachment = CgoRejectionMemoAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            CopyAttachments(dbAttachment, fileServer.BasePath, batchSequenceFolderPath, errors, logger);
            CgoRejectionMemoAttachmentRepository.Update(dbAttachment);
          }

          // Add RM AWB attachments
          logger.InfoFormat("No of Rejection Memo AWBs {0}", rejectionMemo.CouponBreakdownRecord.Count);
          foreach (var rmAwb in rejectionMemo.CouponBreakdownRecord)
          {
            if (rejectionMemo.Attachments.Count == 0 && !isFolderCreated && rmAwb.Attachments.Count > 0)
            {
              batchSequenceFolderPath = CreateBatchSequenceFolder(destinationFolderPath, rejectionMemo.BatchSequenceNumber, rejectionMemo.RecordSequenceWithinBatch);
              isFolderCreated = true;
            }
            // Create BreakdownSerialNumber folder
            var breakdownSerialNumberFolderPath = rmAwb.Attachments.Count > 0 ? CreateBreakdownSerialNumberFolder(batchSequenceFolderPath, rmAwb.BdSerialNumber) : string.Empty;

            logger.InfoFormat("No of Rejection Memo AWB attachments {0}", rmAwb.Attachments.Count);

            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.
            foreach (var attachment in rmAwb.Attachments.Where(attachment => !attachment.IsFullPath && !File.Exists(attachment.FilePath)))
            {
              //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
              //Refresh attachment object from database.
              var dbAttachment = RMAwbAttachmentRepository.Single(attac => attac.Id == attachment.Id);
              CopyAttachments(dbAttachment, fileServer.BasePath, breakdownSerialNumberFolderPath, errors, logger);
              RMAwbAttachmentRepository.Update(dbAttachment);
            }
          }
        }
        //Commit the transaction for all the attachment repositories
        UnitOfWork.CommitDefault();
      }
      catch (Exception ex)
      {
        logger.Error("Error copying supporting attachments", ex);
        errors.AppendFormat("Error copying supporting attachments - {0}", ex.Message);
        return false;
      }
      return true;
    }
    /// <summary>
    /// Creates the supporting document batch upload.
    /// </summary>
    private void CreateSupportingDocumentBatchUpload()
    {

    }

    private string CreateBreakdownSerialNumberFolder(string batchSequenceFolderPath, int serialNo)
    {
      string breakdownSerialNumberFolderPath = Path.Combine(batchSequenceFolderPath, serialNo.ToString().PadLeft(5, '0'));

      if (!Directory.Exists(breakdownSerialNumberFolderPath))
        Directory.CreateDirectory(breakdownSerialNumberFolderPath);

      return breakdownSerialNumberFolderPath;
    }

    private string CreateBatchSequenceFolder(string destinationFolderPath, int batchSequenceNumber, int recordSequenceWithinBatch)
    {
      string batchSequenceFolder = string.Format(@"{0}-{1}", batchSequenceNumber.ToString().PadLeft(5, '0'), recordSequenceWithinBatch.ToString().PadLeft(5, '0'));
      string batchSequenceFolderPath = Path.Combine(destinationFolderPath, batchSequenceFolder);

      if (!Directory.Exists(batchSequenceFolderPath))
        Directory.CreateDirectory(batchSequenceFolderPath);

      return batchSequenceFolderPath;
    }

    private string CreateProvisionalInvoiceBatchSequenceFolder(string destinationFolderPath, string provisionalInvoiceNumber, int batchSequenceNumber, int recordSequenceWithinBatch)
    {
      string batchSequenceFolder = string.Format(@"{0}-{1}-{2}", provisionalInvoiceNumber, batchSequenceNumber.ToString().PadLeft(5, '0'), recordSequenceWithinBatch.ToString().PadLeft(5, '0'));
      string batchSequenceFolderPath = Path.Combine(destinationFolderPath, batchSequenceFolder);

      if (!Directory.Exists(batchSequenceFolderPath))
        Directory.CreateDirectory(batchSequenceFolderPath);

      return batchSequenceFolderPath;
    }
    /// <summary>
    /// Creates the supporting document is web.
    /// </summary>
    private void CreateSupportingDocumentIsWeb()
    {

    }
    #endregion
  }
}
