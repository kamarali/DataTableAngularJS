using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax;
using log4net;

namespace Iata.IS.Business.Common
{
  public interface IEinvoiceDocumentGenerator
  {
    /// <summary>
    /// Creates the E invoice Documents.
    /// </summary>
    void CreateEinvoiceDocuments(InvoiceBase invoice, string eInvoiceDocumentsPath);
  }
}
