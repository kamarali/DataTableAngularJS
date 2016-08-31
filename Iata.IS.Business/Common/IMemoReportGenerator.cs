using System.Text;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
  public interface IMemoReportGenerator
  {
    /// <summary>
    /// Creates the memo.
    /// </summary>
    /// <param name="paxInvoice">The pax invoice.</param>
    /// <param name="reportDirPath">The report dir path.</param>
    /// <param name="errors">The errors.</param>
    void CreateMemoReports(PaxInvoice paxInvoice, string reportDirPath, StringBuilder errors);

   /// <summary>
    /// Creates the memo for Cgo Invoice
   /// </summary>
   /// <param name="cgoInvoice"></param>
   /// <param name="reportDirPath"></param>
   /// <param name="errors"></param>
    void CreateMemoReports(CargoInvoice cgoInvoice, string reportDirPath, StringBuilder errors);
  }
}