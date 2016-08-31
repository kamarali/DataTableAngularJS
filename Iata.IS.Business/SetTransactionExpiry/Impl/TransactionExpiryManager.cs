using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using log4net;

namespace Iata.IS.Business.SetTransactionExpiry.Impl
{
    public class TransactionExpiryManager : Repository<InvoiceBase>, ITransactionExpiryManager
    {

        #region Constants

        private const string BillingYear = "BILLING_YEAR_I";
        private const string BillingMonth = "BILLING_MONTH_I";
        private const string BillingPeriod = "BILLING_PERIOD_I";
        private const string BillingCategoryId = "BILLING_CATEGORY_I";
        private const string SetTransactionExpiry = "SetTransactionExpiry";
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        public void ExcuteSetTransactionExpiryProc(int billingYear, int billingMonth, int period, int billingCategory)
        {
            _logger.Info("Start executing SetTransactionExpiry procedure");

            try
            {
                _logger.Info("Year "+Convert.ToString(billingYear));
                _logger.Info("Month" + Convert.ToString(billingMonth));
                _logger.Info("Period" + Convert.ToString(period));
                _logger.Info("Billing Category" + Convert.ToString(billingCategory));

                _logger.Info("Procedure paramters are " +
                             String.Format("Year: {0}, Month: {1}, Period: {2}, BillingCategory: {3}",
                                           billingYear, billingMonth, period, billingCategory));

                var parameters = new ObjectParameter[4];
                parameters[0] = new ObjectParameter(BillingYear, billingYear);
                parameters[1] = new ObjectParameter(BillingMonth, billingMonth);
                parameters[2] = new ObjectParameter(BillingPeriod, period);
                parameters[3] = new ObjectParameter(BillingCategoryId, billingCategory);

                // Execute Store Procedure PROC_SET_TRANSACTION_EXPIRY in database to set transaction expiry
                ExecuteStoredProcedure(SetTransactionExpiry, parameters);

                _logger.Info("SetTransactionExpiry procedure executed successfully.");
            }
            catch (Exception ex)
            {

                _logger.Error("Error occured while executing  SetTransactionExpiry procedure", ex);

            }
        }
    }
}
