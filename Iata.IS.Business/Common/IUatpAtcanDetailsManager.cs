using Iata.IS.Data.Reports.UatpAtcan;
namespace Iata.IS.Business.Common
{
    public interface IUatpAtcanDetailsManager
    { 
        /// <summary>
        /// Create and transmit UatpAtcand details report
        /// </summary>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billingTypeId"></param>
       /// <param name="isReProcessing"> </param>
       /// <param name="retryCount"> current retry count </param>
       /// <param name="maxRetryCnt"> maxRetry Count </param>
       /// <returns></returns>
        bool CreateAndTransmitUatpAtcanCsv(int billingMemberId, int billingMonth, int billingYear, int billingPeriod, int billingTypeId, int isReProcessing = 0 , int retryCount = 1, int maxRetryCnt = 1);

        ///// <summary>
        ///// Create trigger for UatpAtcan deatils trigger 
        ///// </summary>
        ///// <param name="triggerName"></param>
        ///// <returns></returns>
        //bool CreateAndTransmitUatpAtcanCSV(string triggerName);

    }
}
