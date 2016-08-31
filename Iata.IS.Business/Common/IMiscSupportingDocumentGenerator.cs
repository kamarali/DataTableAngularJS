using System.Text;
using Iata.IS.Business.Pax;
using Iata.IS.Model.MiscUatp;
using log4net;

namespace Iata.IS.Business.Common
{
  /// <summary>
  /// Interface for generating supporting documents for MiscInvoice
  /// </summary>
  public interface IMiscSupportingDocumentGenerator
  {
    /// <summary>
    /// Creates the misc supporting document.
    /// </summary>
    /// <param name="invoice">The Misc Uatp invoice.</param>
    /// <param name="supportingDocumentPath">The supporting document path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    void CreateMiscSupportingDocument(MiscUatpInvoice invoice, string supportingDocumentPath, StringBuilder errors, ILog logger);
   
  }
}