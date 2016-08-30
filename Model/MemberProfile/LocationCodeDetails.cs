namespace Iata.IS.Model.MemberProfile
{
  /// <summary>
  /// Used in Pax, Misc. invoice header screens for displaying location codes.
  /// </summary>
  public class LocationCodeDetails
  {
    public string CityName { get; set; }
    public string LocationCode { get; set; }
    public string CountryId { get; set; }
    public string CurrencyCode { get; set; }
  }
}
