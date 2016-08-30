using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp
{
  /// <summary>
  /// Represents mapping between Charge Code 
  /// and Field Metadata
  /// </summary>
  [Serializable]
  public class FieldChargeCodeMapping : EntityBase<int>, ICacheable
  {

    /// <summary>
    /// Gets or sets the field meta data id.
    /// </summary>
    /// <value>The field meta data id.</value>
    public Guid FieldMetaDataId { get; set; }

    /// <summary>
    /// Gets or sets the charge code id.
    /// </summary>
    /// <value>The charge code id.</value>
    public int ChargeCodeId { get; set; }
    
    /// <summary>
    /// Gets or sets the charge code type id.
    /// </summary>
    /// <value>The charge code type id.</value>
    public int? ChargeCodeTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the required type id.
    /// </summary>
    /// <value>The required type id.</value>
    public int RequiredTypeId { get; set; }

    /// <summary>
    /// Navigational property for <see cref="ChargeCode"/> object based on 
    /// <see cref="ChargeCodeId"/>.
    /// </summary>
    /// <value>The charge code.</value>
    public ChargeCode ChargeCode { get; set; }
    
    /// <summary>
    /// Navigational property for <see cref="ChargeCodeType"/> object based on 
    /// <see cref="ChargeCodeTypeId"/>.
    /// </summary>
    /// <value>The charge code type object.</value>
    /// <remarks>
    /// This property can be null if no charge code type is present or defined 
    /// to the mapping.
    /// </remarks>
    public ChargeCodeType ChargeCodeType { get; set; }

    /// <summary>
    /// Navigational property for <see cref="FieldMetaData"/> object based on 
    /// <see cref="FieldMetaDataId"/>.
    /// </summary>
    /// <value>The Field Metadata object.</value>
    public FieldMetaData FieldMetaData { get; set; }

    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the type of the required.
    /// </summary>
    /// <value>The type of the required.</value>
    public RequiredType RequiredType
    {
      get
      {
        return (RequiredType)RequiredTypeId;
      }
      set
      {
        RequiredTypeId = Convert.ToInt32(value);
      }
    }

  }
}
