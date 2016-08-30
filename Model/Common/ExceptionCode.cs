using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  public class ExceptionCode : MasterBase<int>
  {
    public int BillingCategoryId { get; set; }

    public BillingCategoryType BillingCategoryType
    {
      get { return (BillingCategoryType) BillingCategoryId; }
    }

    public string Description { get; set; }

    public bool IsBatchUpdateAllowed { get; set; }

    /// <summary>
    /// ErrorType = LinkingError\Non Linking Error
    /// </summary>
    public string ErrorType { get; set; }
  }
}
