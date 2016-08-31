using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
namespace Iata.IS.Business.Common
{
  public interface IBillingCategoryManager
  {
    /// <summary>
    /// Adds the billing category.
    /// </summary>
    /// <param name="billingCategory">The billing category.</param>
    /// <returns></returns>
    BillingCategory AddBillingCategory(BillingCategory billingCategory);

    /// <summary>
    /// Updates the billing category.
    /// </summary>
    /// <param name="billingCategory">The billing category.</param>
    /// <returns></returns>
    BillingCategory UpdateBillingCategory(BillingCategory billingCategory);

    /// <summary>
    /// Deletes the billing category.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    bool DeleteBillingCategory(int billingCategoryId);

    /// <summary>
    /// Gets the billing category details.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    BillingCategory GetBillingCategoryDetails(int billingCategoryId);

    /// <summary>
    /// Gets all billing category list.
    /// </summary>
    /// <returns></returns>
    List<BillingCategory> GetAllBillingCategoryList();

    /// <summary>
    /// Gets the billing category list.
    /// </summary>
    /// <param name="codeIsxml">The code is-xml.</param>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    List<BillingCategory> GetBillingCategoryList(string codeIsxml, string description);
  }
}
