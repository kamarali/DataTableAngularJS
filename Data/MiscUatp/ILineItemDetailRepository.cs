using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Data.MiscUatp
{
  public interface ILineItemDetailRepository : IRepository<LineItemDetail>
  {
    void UpdateLineItemDetailEndDate(Guid lineItemId, DateTime lineItemEndDate);

    int GetMaxDetailNumber(Guid lineItemId);

    /// <summary>
    /// Gets the line item detail header information.
    /// </summary>
    /// <param name="where">The where clause.</param>
    /// <returns></returns>
    LineItemDetail GetLineItemDetailHeaderInformation(Expression<Func<LineItemDetail, bool>> where);

      /// <summary>
      /// Singles the specified line item detail id.
      /// </summary>
      /// <param name="lineItemDetailId">The line item detail id.</param>
      /// <param name="lineItemNo">The line item no.</param>
      /// <returns></returns>
      LineItemDetail Single(Guid? lineItemDetailId = null, int? lineItemNo = null);

      List<LineItemDetail> Get(Guid? lineItemId = null, int? lineItemNo = null);



  }
}