using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp
{
  public class ServerValidator : EntityBase<int>
  {
    /// <summary>
    /// Gets or sets the charge code id.
    /// </summary>
    /// <value>The charge code id.</value>
    public int? ChargeCodeId { get; set; }

    /// <summary>
    /// Gets or sets the charge code type id.
    /// </summary>
    /// <value>The charge code type id.</value>
    public int? ChargeCodeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the validator.
    /// </summary>
    /// <value>The name of the validator.</value>
    public string ValidatorName { get; set; }

    /// <summary>
    /// Gets or sets the name of the validation class.
    /// </summary>
    /// <value>The name of the validation class.</value>
    public string ValidationClassName { get; set; }
  }
}