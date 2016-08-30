using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax
{
  [Serializable]
  public class SourceCode : MasterBase<int>, ICacheable
  {
    /// <summary>
    /// Mapped with SOURCE_CODE which different than id (PK)
    /// </summary>
    public int SourceCodeIdentifier { get; set; }

    public string SourceCodeDescription { get; set; }

    public int TransactionTypeId { get; set; }

    public string UtilizationType { get; set; }

    //public UtilizationType UtilizationType { get; set; }

    public TransactionType TransactionType { get; set; }

    public bool IncludeInAtpcoReport { get; set; }

    public bool IsFFIndicator { get; set; }

    public bool IsBilateralCode { get; set; }

    public bool IsRejectionLevel { get; set; }
  }
}
