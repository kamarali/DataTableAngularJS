using System;
using System.IO;
using System.Text;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class MiscSupportingDocumentGenerator : IMiscSupportingDocumentGenerator
  {
    public IReferenceManager ReferenceManager { get; set; }
    public IRepository<MiscUatpAttachment> MiscUatpAttachmentRepository { get; set; }

    #region Implementation of IMiscSupportingDocumentGenerator

    /// <summary>
    /// Creates the misc supporting document.
    /// </summary>
    /// <param name="invoice">The Misc Uatp invoice.</param>
    /// <param name="supportingDocumentPath">The supporting document path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    public void CreateMiscSupportingDocument(MiscUatpInvoice invoice, string supportingDocumentPath, StringBuilder errors, ILog logger)
    {
      try
      {
        // Getting all active attachment servers
        FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
        logger.Info("All Active attachment servers are fetched.");
        if (fileServer != null)
        {
          // Iterate through all MiscInvoice level attachments
          foreach (var attachment in invoice.Attachments)
          {
            //SCP407591: SRM: Offline collection generation failure notification - SIS Prod - 9SEP2015
            //Refresh attachment object from database.
            var dbAttachement = MiscUatpAttachmentRepository.Single(attac => attac.Id == attachment.Id);
            // Copy attachments only if attachments are added from Web.
            // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
            // added check for, if file path does not exists.)
            if (!dbAttachement.IsFullPath && !File.Exists(dbAttachement.FilePath))
            {
              // Build source file Path
                var sourceFile = Path.Combine(fileServer.BasePath, Path.Combine(dbAttachement.FilePath, String.Format("{0}{1}", dbAttachement.Id, Path.GetExtension(dbAttachement.OriginalFileName))));
              // Build Destination file Path
              var destinationFile = Path.Combine(supportingDocumentPath, dbAttachement.OriginalFileName);

              // If Source file path exists, copy it to destination file path.
              if (File.Exists(sourceFile))
              {
                //File.Copy(sourceFile, destinationFile);
                dbAttachement.IsFullPath = true;
                dbAttachement.FilePath = destinationFile;
                FileIo.MoveFile(sourceFile, destinationFile);
                MiscUatpAttachmentRepository.Update(dbAttachement);
                logger.InfoFormat("File copied from [{0}] to [{1}]", sourceFile, destinationFile);
              }
              else
              {
                logger.InfoFormat("File [{0}] not found on server", sourceFile);
                errors.AppendFormat("File [{0}] could not found for MiscInvoice # [{1}]", sourceFile, invoice.InvoiceNumber);
              }
            }
          }
          UnitOfWork.CommitDefault();
        }
        else
        {
          logger.Info("Active attachment server could not found.");
          errors.AppendFormat("Active attachment server could not found for MiscInvoice # [{0}]", invoice.InvoiceNumber);
        }
      }
      catch (Exception ex)
      {
        logger.Error(string.Format("Error copying supporting attachments for MiscInvoice # [{0}]", invoice.InvoiceNumber), ex);
        throw;
      }

    }



    #endregion
  }
}