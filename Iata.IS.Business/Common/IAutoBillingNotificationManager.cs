using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Business.Common
{
  public interface IAutoBillingNotificationManager
  {
    /// <summary>
    /// To send the AutoBilling Email notification for UnavaliableInvoiceFound and
    /// For the Threshould value of the Invoice Reached.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="processingContact"></param>
    /// <param name="sisOpsMail"></param>
    /// <param name="emailTemplateId"></param>
    /// <param name="invThreshoulValue"></param>
    /// <param name="avaliablrInvoices"></param>
    /// <returns></returns>
    bool SendUnavaliableOrThresholdReachedInvoiceNotification(int memberId, ProcessingContactType processingContact, string sisOpsMail,int emailTemplateId,long invThreshoulValue = 0 ,long avaliablrInvoices= 0);
    
  }
}
