using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class OtherOrganizationAdditionalDetail : EntityBase<Guid>
  {
    public Guid OtherOrganizationInfoId { get; set; }
    public string AdditionalDetail { get; set; }
    public string AdditionalDetailDescription { get; set; }
    public int AdditionalDetailType { get; set; }
  }
}