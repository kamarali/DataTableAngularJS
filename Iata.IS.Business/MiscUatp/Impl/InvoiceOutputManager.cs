using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Linq;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports.MiscUatp;
using Iata.IS.Business.Reports.MiscUatp.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.MiscUatp.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using log4net;
using MailComponent = System.Net.Mail;

namespace Iata.IS.Business.MiscUatp.Impl
{
  /// <summary>
  /// This class contains all the functionality related to download Misc and Uatp Invoice (output)
  /// </summary>
  public class InvoiceOutputManager : IInvoiceOutputManager
  {
    #region Private Members

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly IReferenceManager _referenceManager;
    private readonly IMemberManager _memberManager;

    private string _listingReportFileName;
    private string _indexXmlPath;
    private string _eInvoiceFolderPath;
    private string _supportingDocFolderPath;
    
  
    #endregion

    #region Constructor

    public InvoiceOutputManager()
    {
      // Initialize managers
      _referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
      _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
    }

    #endregion

    #region Public Methods

    
    /// <summary>
    /// Download the requested invoice details like Invoice details, Listing report, Memos and attached supporting documents depending on the user choice
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="isReceivable"></param>
    /// <param name="options"></param>
    /// <param name="outputZipFileName"></param>
    /// <returns></returns>
    public string DownloadInvoice(Guid invoiceId, bool isReceivable, List<string> options, string outputZipFileName)
    {
      try
      {
        // Get invoice details for given InvoiceId
        IMiscInvoiceRepository miscInvoiceRepository = new MiscInvoiceRepository();

        // Replaced with LoadStrategy single call
        // MiscUatpInvoice invoice = miscInvoiceRepository.Single(inv => inv.Id == invoiceId);
        MiscUatpInvoice invoice = miscInvoiceRepository.Single(invoiceId);

        // Create the base folder as per specifications in IS File Specs
        string baseFolder = CreateBaseFolder(invoice, isReceivable);

        Logger.InfoFormat("Base folder for keeping invoice related files has been created for Invoice No. [{0}]", invoice.InvoiceNumber);

        // Create the sub folders as per specifications in IS File Specs
        CreateSubFolders(baseFolder, invoice, options, isReceivable);

        // Generate the output files required as per choice.

        // EInvoice files
        if (options.Contains("1"))
        {
          CopyEInvoicingFiles(new List<string>());
          Logger.Info("Invoicing files are generated and copied");
        }
        // Build the details listing report
        if (options.Contains("2"))
        {
          ILineItemRepository lineItemRepository = new LineItemRepository();
          //IEnumerable<LineItem> lineItems = lineItemRepository.Get(lineItem => lineItem.InvoiceId == invoice.Id);
          IEnumerable<LineItem> lineItems = lineItemRepository.Get(invoice.Id);
          var orderedLineItems = from li in lineItems
                                 orderby li.LineItemNumber ascending
                                 select li;

          IMiscUatpReportManager miscUatpReportManager = new MiscUatpReportManager();
          miscUatpReportManager.BuildListingReport(invoice, orderedLineItems, _listingReportFileName,0);

          Logger.InfoFormat("The detailed listing report for invoice [No. {0}] is generated and copied", invoice.InvoiceNumber);
        }

        //TODO: not applicable for Misc invoice so not implemented now. Will be implemented when Pax and Cgo are considered.
        // Build the Memos
        //if (options.Contains("3"))
        //  BuildMemos(invoice);

        // Fetch supporting documents attached to invoice
        if (options.Contains("4"))
        {
          CopyInvoiceSupportingAttachments(invoice, _supportingDocFolderPath);
          Logger.InfoFormat("The supporting documents attached to invoice [No. {0}] is fetched and copied", invoice.InvoiceNumber);
        }

        Logger.Info("All the output files as per selected options is ready for zipping");

        // Create index.xml to provide the information related to files in the zipped output folder
        CreateIndexFile(invoice, isReceivable);
        Logger.InfoFormat("Index file [{0}] containing detailed information about the invoice related files lying inside the zip folder has been created", "INDEX.XML");

        // TODO: Get dynamically the member's shared path as base path and append it to outputZipFileName
        var memberBasePath = System.Configuration.ConfigurationManager.AppSettings["MemberSharedPath"];
        if (!Directory.Exists(memberBasePath))
        {
          Directory.CreateDirectory(memberBasePath);
        }

        var downloadableZipFilePath = Path.Combine(memberBasePath, outputZipFileName);

        // Zip the output folder. provide full path for zip file name
        FileIo.ZipOutputFolder(baseFolder, downloadableZipFilePath);

        return downloadableZipFilePath;
      }
      catch (Exception exception)
      {
        Logger.Error("Error downloading the invoice!", exception);
        throw;
      }
    }

    /// <summary>
    /// En-queues the invoice download request to the system for background processing.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public bool EnqueueDownloadRequest(IDictionary<string, string> messages)
    {
      try
      {
        var queueHelper = new QueueHelper("DOWNLOADINVOICEQUEUE");
        queueHelper.Enqueue(messages);

        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error en-queuing download request", ex);
        throw;
      }
    }

    /// <summary>
    /// Sends the mail to the specified recipients
    /// </summary>
    /// <param name="userId">recipient id</param>
    /// <param name="outputZipFileName">output file name where invoice is downloaded</param>
    /// <returns></returns>
    public bool SendInvoiceDownloadNotificationEMail(int userId, string outputZipFileName)
    {
      var message = new MailComponent.MailMessage();
      try
      {
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        iPayables.UserManagement.IUserManagement AuthManager = new iPayables.UserManagement.UserManagementModel();
        iPayables.UserManagement.I_ISUser SISUser = AuthManager.GetUserByUserID(userId);
        if (SISUser != null && !string.IsNullOrEmpty(SISUser.Email))
        {
          message.To.Add(SISUser.Email);
        }
        else
        {
          Logger.Info("Email address is not available for the user, so not able to send the message");
          return false;
        }

        // HARDCODED EMAIl ADDRESS.
        message.From = new MailComponent.MailAddress("sis_noreply@kaleconsultants.com", "SIS No Reply");
        message.Subject = string.Format("Your invoice files are ready for download. [File: {0}]", Path.GetFileName(outputZipFileName));

        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        var context = new NVelocity.VelocityContext();
        context.Put("Message",
                    new InvoiceDownloadNotificationMessage()
                      {
                        DownloadableInvoicePath = outputZipFileName,
                        RecipientName = SISUser.FirstName
                      });
        message.Body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.InvoiceDownloadNotification, context);
        //message.Body = string.Format("The downloadable zip file requested is ready for download at {0}", outputZipFileName);

        emailSender.Send(message);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed sending the mail", ex);
        message.Dispose();
        throw;
      }

      return true;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Create base folder as per specification in IS File Specs
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="isReceivable"></param>
    /// <returns></returns>
    private string CreateBaseFolder(MiscUatpInvoice invoice, bool isReceivable)
    {
      try
      {
        Member member;
        if (isReceivable)
          member = _memberManager.GetMember(invoice.BillingMemberId);
        else
          member = _memberManager.GetMember(invoice.BilledMemberId);

        string type = isReceivable ? "R" : "P";
        string billingCategoryString = invoice.BillingCategory.ToString().ToUpper(); //"MISC"; //TO DO: dynamically get the value depending on billing category
        string billingAirlinePrefix = member.MemberCodeAlpha;
        string billingAirlineNumericCode = member.MemberCodeNumeric;
        string billingYearMonthPeriod = string.Format("{0}{1}{2}", invoice.BillingYear, invoice.BillingMonth.ToString().PadLeft(2, '0'), invoice.BillingPeriod.ToString().PadLeft(2, '0'));

        string folderName = string.Format("{0}-{1}-{2}-{3}-{4}", billingCategoryString, type, billingAirlinePrefix, billingAirlineNumericCode, billingYearMonthPeriod);

        //Temporary base path to be configured and fetched from config.
        string basePath = AdminSystem.SystemParameters.Instance.General.TempInvoiceOutputFiles; 

        if (!Directory.Exists(basePath))
        {
          Directory.CreateDirectory(basePath);
        }

        string folderFullPath = Path.Combine(basePath, folderName);

        if (Directory.Exists(folderFullPath))
        {
          Directory.Delete(folderFullPath, true);
        }
        Directory.CreateDirectory(folderFullPath);

        return folderFullPath;
      }
      catch (Exception ex)
      {
        Logger.Error("Error creating base folder", ex);
        throw;
      }
    }

    /// <summary>
    /// Creates sub folder as per specification in IS File Specs
    /// </summary>
    /// <param name="baseFolder"></param>
    /// <param name="invoice"></param>
    /// <param name="options"></param>
    /// <param name="isReceivable"></param>
    private void CreateSubFolders(string baseFolder, MiscUatpInvoice invoice, List<string> options, bool isReceivable)
    {
      try
      {
        var member = _memberManager.GetMember(isReceivable ? invoice.BilledMemberId : invoice.BillingMemberId);

        var subFolderName = string.Format("{0}-{1}", member.MemberCodeAlpha, member.MemberCodeNumeric);
        var subFolderFullPath = Path.Combine(baseFolder, subFolderName);

        if (!Directory.Exists(subFolderFullPath))
        {
          Directory.CreateDirectory(subFolderFullPath);
        }

        //Build Index.xml path
        var indexXmlPath = Path.Combine(baseFolder, "INDEX.XML");
        _indexXmlPath = indexXmlPath;

        //Create Invoice folder
        var invoiceFolderPath = Path.Combine(subFolderFullPath, string.Format("INV-{0}", invoice.InvoiceNumber));
        if (!Directory.Exists(invoiceFolderPath))
        {
          Directory.CreateDirectory(invoiceFolderPath);
        }

        //Create E-INVOICE folder
        if (options.Contains("1"))
        {
          var eInvoiceFolderPath = Path.Combine(invoiceFolderPath, "E-INVOICE");
          if (!Directory.Exists(eInvoiceFolderPath))
          {
            Directory.CreateDirectory(eInvoiceFolderPath);
          }
          _eInvoiceFolderPath = eInvoiceFolderPath;
        }

        //Create LISTINGS folder
        if (options.Contains("2"))
        {
          var listingFolderPath = Path.Combine(invoiceFolderPath, "LISTINGS");
          if (!Directory.Exists(listingFolderPath))
          {
            Directory.CreateDirectory(listingFolderPath);
          }
          var reportFileName = Path.Combine(listingFolderPath, GetDetailedListingFile(invoice.BillingCategory, invoice.InvoiceNumber));
          _listingReportFileName = reportFileName;
        }

        //Create SUPPDOCS folder
        if (options.Contains("4"))
        {
          var supportingDocFolderPath = Path.Combine(invoiceFolderPath, "SUPPDOCS");
          if (!Directory.Exists(supportingDocFolderPath))
          {
            Directory.CreateDirectory(supportingDocFolderPath);
          }
          _supportingDocFolderPath = supportingDocFolderPath;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error creating sub-folder", ex);
        throw;
      }
    }

    /// <summary>
    /// Creates index.xml to provide the zip folder content details.
    /// </summary>
    /// <param name="invoice">requested invoice</param>
    private void CreateIndexFile(MiscUatpInvoice invoice, bool isReceivable)
    {
      try
      {
        var xDoc = new XmlDocument();

        //Add the XML declaration section
        XmlNode xmlnode = xDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
        xDoc.AppendChild(xmlnode);

        //Add root element
        XmlElement xmlRootElement = (isReceivable) ? xDoc.CreateElement("", "SISReceivablesIndexTransmission", "") : xDoc.CreateElement("", "SISPayablesIndexTransmission", "");
        XmlText xmlRootText = xDoc.CreateTextNode("");
        xmlRootElement.AppendChild(xmlRootText);
        xDoc.AppendChild(xmlRootElement);

        XmlElement xmlHeaderElement = AddElement(xDoc, (isReceivable) ? "SISReceivablesIndexHeader" : "SISPayablesIndexHeader", "", xmlRootElement);
        AddElement(xDoc, "Version", (isReceivable) ? "IATA:SISReceivablesIndexV1.0.0.0" : "IATA:SISPayablesIndexV1.0.0.0", xmlHeaderElement);

        //TODO: fetch value once implemented.
        //AddElement(xDoc, "TransmissionID", "", xmlHeaderElement);  

        Member billingMember = _memberManager.GetMember(invoice.BillingMemberId);
        AddElement(xDoc, "BillingMember", billingMember.MemberCodeNumeric, xmlHeaderElement);
        AddElement(xDoc, "ClearanceMonth", string.Format("{0}{1}", invoice.BillingMonth.ToString().PadLeft(2, '0'), invoice.BillingYear), xmlHeaderElement);
        AddElement(xDoc, "PeriodNumber", invoice.BillingPeriod.ToString(), xmlHeaderElement);
        AddElement(xDoc, "BillingCategory", "M", xmlHeaderElement);
        XmlElement xmlInvoiceHeaderElement = AddElement(xDoc, "InvoiceHeader", "", xmlHeaderElement);

        Member billedgMember = _memberManager.GetMember(invoice.BilledMemberId);
        AddElement(xDoc, "BilledMember", billedgMember.MemberCodeNumeric, xmlInvoiceHeaderElement);
        AddElement(xDoc, "InvoiceNumber", invoice.InvoiceNumber, xmlInvoiceHeaderElement);

        //Add E-Invoicing Files
        XmlElement xmlEInvoicingFilesElement = AddElement(xDoc, "EInvoicingFiles", "", xmlInvoiceHeaderElement);
        var eInvoicingFiles = BuildEInvoicingFiles(invoice, "");  //TODO: source folder path should be provided (configurable) to create the invoices and copy here
        int srNo = 0;
        foreach (var eInvoicingFile in eInvoicingFiles)
        {
          srNo++;
          //Add container tag
          var xmlEInvoicingFileElement = AddElement(xDoc, "EInvoicingFile", string.Empty, xmlEInvoicingFilesElement);

          AddElement(xDoc, "SrNo", srNo.ToString(), xmlEInvoicingFileElement);
          AddElement(xDoc, "FileName", eInvoicingFile, xmlEInvoicingFileElement);
        }

        //Add Detailed ListingFiles
        XmlElement xmlDetailedListingFilesElement = AddElement(xDoc, "DetailedListingFiles", "", xmlInvoiceHeaderElement);
        var detailedListingFile = GetDetailedListingFile(invoice.BillingCategory, invoice.InvoiceNumber);
        //Add container tag
        var xmlDetailedListingFileElement = AddElement(xDoc, "DetailedListingFile", string.Empty, xmlDetailedListingFilesElement);

        AddElement(xDoc, "SrNo", "1", xmlDetailedListingFileElement);
        AddElement(xDoc, "FileName", detailedListingFile, xmlDetailedListingFileElement);

        //Add Invoice Supporting Attachments
        XmlElement xmlInvoiceSupportingAttachmentsElement = AddElement(xDoc, "InvoiceSupportingAttachments", "", xmlInvoiceHeaderElement);

        srNo = 0;
        foreach (var attachment in invoice.Attachments)
        {
          srNo++;
          //Add container tag
          var xmlInvoiceSupportingAttachmentElement = AddElement(xDoc, "InvoiceSupportingAttachment", string.Empty, xmlInvoiceSupportingAttachmentsElement);

          AddElement(xDoc, "AttachmentNumber", srNo.ToString(), xmlInvoiceSupportingAttachmentElement);
          AddElement(xDoc, "AttachmentFileName", attachment.OriginalFileName, xmlInvoiceSupportingAttachmentElement);
        }

        xDoc.Save(_indexXmlPath);
      }
      catch (Exception exception)
      {
        Logger.Error(string.Format("Error creating index file [{0}]", _indexXmlPath), exception);
        throw;
      }
    }

    /// <summary>
    /// Copies supporting documents associated with the invoice.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="destinationFolderPath"></param>
    /// <returns></returns>
    public IEnumerable<Attachment> CopyInvoiceSupportingAttachments(MiscUatpInvoice invoice, string destinationFolderPath)
    {
      try
      {
        var fileServer = _referenceManager.GetActiveAttachmentServer();

        foreach (var attachment in invoice.Attachments)
        {
          var fileExtension = attachment.OriginalFileName.Substring(attachment.OriginalFileName.LastIndexOf('.'));
          var sourceFile = Path.Combine(fileServer.BasePath, attachment.FilePath, attachment.Id.ToString());
          sourceFile = Path.ChangeExtension(sourceFile, fileExtension);

          var destinationFile = Path.Combine(destinationFolderPath, attachment.OriginalFileName);
          if (File.Exists(sourceFile))
          {
            File.Copy(sourceFile, destinationFile);
          }
          else
          {
            Logger.InfoFormat("File [{0}] not found on server", sourceFile);
          }

        }
        return invoice.Attachments;
      }
      catch (Exception ex)
      {
        Logger.Error("Error copying supporting attachments", ex);
        throw;
      }
    }

    private static IEnumerable<string> BuildEInvoicingFiles(MiscUatpInvoice invoice, string sourceFolderPath)
    {
      //TODO: This is to be implemented by IPayables. This is just dummy implementation
      var eInvoicingFiles = new List<string>
                              {
                                Path.Combine(sourceFolderPath, string.Format("MINV-{0}.PDF", invoice.InvoiceNumber)),
                                Path.Combine(sourceFolderPath, string.Format("MXVF-{0}.XML", invoice.InvoiceNumber))
                              };

      return eInvoicingFiles;
    }

    private void CopyEInvoicingFiles(IEnumerable<string> eInvoicingFiles)
    {
      try
      {
        foreach (var eInvoicingFile in eInvoicingFiles)
        {
          var destinationFile = Path.Combine(_eInvoiceFolderPath, eInvoicingFile.Substring(eInvoicingFile.LastIndexOf(@"\")));
          File.Copy(eInvoicingFile, destinationFile);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error copying EInvoicing files", ex);
        throw;
      }
    }

    private static string GetDetailedListingFile(BillingCategoryType billingCategory, string invoiceNumber)
    {
      return string.Format("{0}DETLST-{1}.PDF", billingCategory.ToString().Substring(0, 1), invoiceNumber);
    }

    private static IEnumerable<string> GetEInvoicingFiles(MiscUatpInvoice invoice)
    {
      var eInvoicingFiles = new List<string>();
      eInvoicingFiles.Add(string.Format("MINV-{0}.PDF", invoice.InvoiceNumber));
      eInvoicingFiles.Add(string.Format("MXVF-{0}.XML", invoice.InvoiceNumber));

      return eInvoicingFiles;
    }

    private static XmlElement AddElement(XmlDocument xDoc, string nodeText, string nodeValue, XmlElement parentElement)
    {
      try
      {
        XmlElement xmlElement = xDoc.CreateElement("", nodeText, "");
        XmlText xmlText = xDoc.CreateTextNode(nodeValue);
        xmlElement.AppendChild(xmlText);
        parentElement.AppendChild(xmlElement);

        return xmlElement;
      }
      catch (Exception ex)
      {
        Logger.Error("Error adding xml element to xml document", ex);
        throw;
      }
    }

    #endregion
  }
}
