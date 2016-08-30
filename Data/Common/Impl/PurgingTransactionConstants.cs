using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Common.Impl
{
  public class PurgingTransactionConstants
  {
    public const string LastClosedPeriod = "LAST_CLOSED_PERIOD_I";
    public const string ErrorInvoiceRetentionPeriod = "ERROR_INV_RET_PERIOD_I";
    public const string CurrentPeriod = "CURRENT_PERIOD";
    public const string ResultCount = "R_RESULT_O";
    public const string DeleteTransactionFunctionName = "DeleteTransaction";
    public const string DeleteTemporaryFilesFunctionName = "DeleteTemporaryFiles";

    public const string QueueOffColFilesForPurgingFunctionName = "QueueOffColFilesForPurging";
    public const string InvoiceEndPeriod = "INV_END_PERIOD_I";
    public const string FormCEndPeriod = "FORMC_END_PERIOD_I";

  }
}
