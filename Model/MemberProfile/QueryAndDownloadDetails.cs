using Iata.IS.Model.Base;
namespace Iata.IS.Model.MemberProfile
{
  public class QueryAndDownloadDetails :EntityBase<int>
  {
    public string Designator { get; set; }

    public string Prefix { get; set; }

    public string CommercialName { get; set; }

    public string LegalName { get; set; }

    public string ContactName { get; set; }

    public string Email { get; set; }

    public string Country { get; set; }

    public string AvailableFields { get; set; }

    public string SelectedFields { get; set; }
  }
}
