using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax.Common
{
  public class RMReasonAcceptableDiff : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the reason code id.
    /// </summary>
    /// <value>The reason code id.</value>
    public int ReasonCodeId { get; set; }

    /// <summary>
    /// Gets or sets the effective from.
    /// </summary>
    /// <value>The effective from.</value>
    public string EffectiveFrom { get; set; }

    /// <summary>
    /// Gets or sets the effective to.
    /// </summary>
    /// <value>The effective to.</value>
    public string EffectiveTo { get; set; }

    /// <summary>
    /// Gets or sets the fare amount.
    /// </summary>
    /// <value>The fare amount.</value>
    public bool IsFareAmount { get; set; }

    /// <summary>
    /// Gets or sets the tax amount.
    /// </summary>
    /// <value>The tax amount.</value>
    public bool IsTaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the isc amount.
    /// </summary>
    /// <value>The isc amount.</value>
    public bool IsIscAmount { get; set; }

    /// <summary>
    /// Gets or sets the oc amount.
    /// </summary>
    /// <value>The oc amount.</value>
    public bool IsOcAmount { get; set; }

    /// <summary>
    /// Gets or sets the uatp amount.
    /// </summary>
    /// <value>The uatp amount.</value>
    public bool IsUatpAmount { get; set; }

    /// <summary>
    /// Gets or sets the hf amount.
    /// </summary>
    /// <value>The hf amount.</value>
    public bool IsHfAmount { get; set; }

    /// <summary>
    /// Gets or sets the vat amount.
    /// </summary>
    /// <value>The vat amount.</value>
    public bool IsVatAmount { get; set; }

    public ReasonCode ReasonCode { get; set; }

    public string ReasonCodeName
    {
        get { return ReasonCode != null ? this.ReasonCode.Code : string.Empty; }
    }

      public string TransactionTypeName
      {
          get { return ReasonCode != null ? this.ReasonCode.TransactionTypeName : string.Empty; }
      }
    private int _transactionTypeId;

    public int TransactionTypeId
    {
        get { return ReasonCode != null ? ReasonCode.TransactionTypeId : _transactionTypeId; }
        set { _transactionTypeId = value; }
    }

  }
}
