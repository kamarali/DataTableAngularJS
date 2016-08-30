using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class ChargeCodeType : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the charge code id.
    /// </summary>
    /// <value>The charge code id.</value>
    public int ChargeCodeId { get; set; }
    
    /// <summary>
    /// Gets or sets the charge code type id.
    /// </summary>
    /// <value>The charge code type id.</value>
    //CMP #636: Standard Update Mobilization
    public int ChargeCategoryId { get; set; }
  }
}