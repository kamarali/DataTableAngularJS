using System;
using System.IO;
using System.Reflection;
using System.Text;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class EinvoiceDocumentGenerator : IEinvoiceDocumentGenerator
  {
    //private readonly IReferenceManager _referenceManager;
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }
    public EinvoiceDocumentGenerator()
    {
     // _referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
    }

    /// <summary>
    /// Method to create Einvoice Documents
    /// </summary>
    /// <param name="invoice">invoice base</param>
    /// <param name="eInvoiceDocumentsPath">eInvoiceDocumentsPath</param>
    public void CreateEinvoiceDocuments(InvoiceBase invoice, string eInvoiceDocumentsPath)
    {
      CopyEInvoiceDocuments(invoice, eInvoiceDocumentsPath);
    }

    /// <summary>
    /// Method to copy EInvoice Documents
    /// Copy following files if present at the location:- 
    /// 1. Legal Pdf File
    /// 2. Legal Xml file
    /// 3. Xml Signature file
    /// 4. Xml Verification log file
    /// </summary>
    /// <param name="invoice">invoice base</param>
    /// <param name="eInvoiceDocumentsPath">eInvoiceDocumentsPath</param>
    private void CopyEInvoiceDocuments(InvoiceBase invoice, string eInvoiceDocumentsPath)
    {
      if(invoice != null) 
        invoice = InvoiceBaseRepository.First(i => i.Id == invoice.Id);
      if (invoice != null)
      {
        var legalPdfLocation = invoice.LegalPdfLocation;
        var legalXmlLocation = invoice.LegalXmlLocation;
        var xmlSignatureLocation = invoice.XmlSignatureLocation;
        var xmlVerificationLogLocation = invoice.XmlVerificationLogLocation;

        // If legalPdfLocation file path exists, copy it to destination file path.
        Logger.InfoFormat("Searching PDF on location [{0}]", invoice.LegalPdfLocation);
        if (!string.IsNullOrEmpty(legalPdfLocation) && File.Exists(legalPdfLocation))
        {
          string destinationFile = Path.Combine(eInvoiceDocumentsPath, Path.GetFileName(legalPdfLocation));

          // if file already exists at destination do not copy.
          if (!File.Exists(destinationFile))
          {
            File.Copy(legalPdfLocation, destinationFile, true);
            Logger.InfoFormat("Legal Pdf file copied from location {0} to {1}", legalPdfLocation, destinationFile);  
          }
          else
          {
            Logger.InfoFormat("Legal Pdf file already exists at location [{0}].",destinationFile);  
          }

        }
        else
        {
          Logger.Info("Legal pdf file not found on server.");
        }

        // If legalXmlLocation file path exists, copy it to destination file path.
        Logger.InfoFormat("Searching Legal Xml on location [{0}]", legalXmlLocation);
        if (!string.IsNullOrEmpty(legalXmlLocation) && File.Exists(legalXmlLocation))
        {
          string destinationFile = Path.Combine(eInvoiceDocumentsPath, Path.GetFileName(legalXmlLocation));

          // if file already exists at destination do not copy.
          if (!File.Exists(destinationFile))
          {
            File.Copy(legalXmlLocation, destinationFile, true);
            Logger.InfoFormat("Legal Xml file copied from location {0} to {1}", legalXmlLocation, destinationFile);  
          }
          else
          {
            Logger.InfoFormat("Legal Xml file already exists at location [{0}].", destinationFile);  
          }
          
        }
        else
        {
          Logger.Info("Legal Xml file not found on server.");
        }

        // If xmlSignatureLocation file path exists, copy it to destination file path.
        Logger.InfoFormat("Searching Xml Signature on location [{0}]", xmlSignatureLocation);
        if (!string.IsNullOrEmpty(xmlSignatureLocation) && File.Exists(xmlSignatureLocation))
        {
          string destinationFile = Path.Combine(eInvoiceDocumentsPath, Path.GetFileName(xmlSignatureLocation));

          // if file already exists at destination do not copy.
          if (!File.Exists(destinationFile))
          {
            File.Copy(xmlSignatureLocation, destinationFile, true);
            Logger.InfoFormat("xml Signature file copied from location {0} to {1}", xmlSignatureLocation,
                              destinationFile);
          }
          else
          {
            Logger.InfoFormat("xml Signature file already exists at location [{0}].",destinationFile);
          }
        }
        else
        {
          Logger.Info("xml Signature file not found on server.");
        }

        // If xmlVerificationLogLocation file path exists, copy it to destination file path.
        Logger.InfoFormat("Searching Xml Verification on location [{0}]", xmlVerificationLogLocation);
        if (!string.IsNullOrEmpty(xmlVerificationLogLocation) && File.Exists(xmlVerificationLogLocation))
        {
          string destinationFile = Path.Combine(eInvoiceDocumentsPath, Path.GetFileName(xmlVerificationLogLocation));

          // if file already exists at destination do not copy.
          if (!File.Exists(destinationFile))
          {
            File.Copy(xmlVerificationLogLocation, destinationFile, true);
            Logger.InfoFormat("xml Verification Log file copied from location {0} to {1}", xmlVerificationLogLocation,
                              destinationFile);
          }
          else
          {
            Logger.InfoFormat("xml Verification Log file already exists at location [{0}].", destinationFile);
          }
        }
        else
        {
          Logger.Info("xml Verification Log file not found on server.");
        }
      }
      else
      {
        Logger.Info("Invoice is null");
      }
    }
  }
}
