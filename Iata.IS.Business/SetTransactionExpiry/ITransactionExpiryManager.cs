using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.SetTransactionExpiry
{
    public interface ITransactionExpiryManager
    {
        void ExcuteSetTransactionExpiryProc(int billingYear, int billingMonth, int period, int billingCategory);
    }
}
