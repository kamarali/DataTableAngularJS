using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Base
{
  public abstract class TransmissionSummaryBase : EntityBase<string>
  {
    public int InvoiceCount { get; set; }

    //TODO : Is-XML Name is TotalAmount@CurrencyCode

  }
}