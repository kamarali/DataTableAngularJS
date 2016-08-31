using Iata.IS.Model.Cargo.ParsingModel;

namespace Iata.IS.Business.Cargo
{
  public interface IOutputGeneratorManager
  {

    /// <summary>
    /// Gets Pax invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria">searchCriteria</param>
    /// <returns>CargoInvoiceModel</returns>
    CargoInvoiceModel GetCargoOutputData(Model.Pax.SearchCriteria searchCriteria);

  }
}
