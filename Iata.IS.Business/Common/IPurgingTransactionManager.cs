using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Pax;

namespace Iata.IS.Business.Common
{
   public interface IPurgingTransactionManager
   {

       /// <summary>
       /// The purgingTransaction trigger method
       /// </summary>
       /// <param name="currentPeriod">The current period for Purging transaction</param>
       //void PurgingTransactionTrigger(DateTime currentPeriod);

     /// <summary>
     /// To delete the transaction data
     /// </summary>
     /// <param name="billingPeriod">The current Billling Period</param>
     /// <param name="billingMonth">The current Billling Month</param>
     /// <param name="billingYear">The current Billling Year</param>
     /// <param name="lastClosedBillingPeriodDate">The current Billling Year</param>
     /// <param name="errorInvoiceRetentionPeriod">The current Billling Year</param>
     bool PurgingTransactionData(int billingPeriod, int billingMonth, int billingYear, DateTime lastClosedBillingPeriodDate, int errorInvoiceRetentionPeriod);

       /// <summary>
       /// To delete the temporary files
       /// </summary>
       /// <param name="billingPeriod">The billing Period</param>
       /// <param name="billingMonth">The billing Month</param>
       /// <param name="billingYear">The billing Year</param>
       bool PurgingTemporaryFiles(int billingPeriod, int billingMonth, int billingYear);

       /// <summary>
       /// To delete the temporary files
       /// </summary>
       ///<param name="currentPeriod">The Current Period</param>
       void PurgingTemporaryFilesTrigger(DateTime currentPeriod);
   }
}
