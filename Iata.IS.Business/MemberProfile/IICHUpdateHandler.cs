namespace Iata.IS.Business.MemberProfile
{
  public interface IICHUpdateHandler
  {
    /// <summary>
    /// This method will accept memberID for which ICH details are updated
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    string GenerateXMLforICHUpdates(int memberId);

    /// <summary>
    /// Generates the XML for ACH updates.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    string GenerateXMLforACHUpdates(int memberId);

    /// <summary>
    /// To send email notification to IS Admin when member profile update sending fails.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="errorCode"></param>
    /// <param name="updateXml"></param>
    /// <param name="failureReason"></param>
    /// <returns></returns>
    bool SendAlertToISAdminforMemProfileUpdateSendingFailure(int memberId, string errorCode, string updateXml, string failureReason);

    bool SendAlertForXmlValidationFailure(int memberId, string updateXml, string operationType, string validationErrors);
  }
}
