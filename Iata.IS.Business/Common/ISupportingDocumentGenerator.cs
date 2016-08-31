using System.Collections.Generic;
using System.Text;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;
using log4net;

namespace Iata.IS.Business.Common
{
  public interface ISupportingDocumentGenerator
  {
    /// <summary>
    /// Creates the supporting document.
    /// </summary>
    bool CreateSupportingDocument(PaxInvoice paxInvoice, string supportingDocPath, StringBuilder errors, ILog logger);

    /// <summary>
    /// Creates the supporting document for Cgo Invoice
    /// </summary>
    /// <param name="cgoInvoice"></param>
    /// <param name="supportingDocPath"></param>
    /// <param name="errors"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    bool CreateSupportingDocument(CargoInvoice cgoInvoice, string supportingDocPath, StringBuilder errors, ILog logger);

    /// <summary>
    /// Creates the form C supporting document.
    /// </summary>
    /// <param name="samplingFormCRecords">The sampling form C records.</param>
    /// <param name="supportingDocumentPath">The supporting document path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    void CreateFormCSupportingDocument(List<SamplingFormCRecord> samplingFormCRecords, string supportingDocumentPath, StringBuilder errors, ILog logger, bool requestViaIsWeb = false);
  }
}