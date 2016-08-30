using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.MemberProfile
{
  public class SubDivision : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the country id.
    /// </summary>
    /// <value>The country id.</value>
    public string CountryId { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    /// <value>The country.</value>
    public Country Country { get; set; }

    /// <summary>
    /// Gets the name of the country.
    /// </summary>
    /// <value>
    /// The name of the country.
    /// </value>
    public string CountryName
    {
        get { return Country != null ? this.Country.Name : string.Empty; }
    }
  }
}
