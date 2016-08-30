using System;
using System.Collections.Generic;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Base;

namespace Iata.IS.Data.MiscUatp
{
    public interface ILineItemRepository : IRepository<LineItem>
    {
        LineItem GetLineItemHeaderInformation(Guid lineItemId);

        int GetMaxLineItemNumber(Guid invoiceId);

        LineItem Single(Guid? invoiceId = null, Guid? lineItemId =null);

        List<LineItem> Get(Guid invoiceId);
    }
}