using Iata.IS.Model.Enums;

namespace Iata.IS.Business.ICP
{
    /// <summary>
    /// This interface is used to send email to sis ops and sis support for failure ICP web service.
    /// CMP #665: User Related Enhancements-FRS-v1.2
    /// </summary>
    public interface IIcpEmailSenderManager
    {
        /// <summary>
        /// This function is used to send email to sis ops and sis support for failure ICP web service.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="emailId"></param>
        /// <param name="userCategory"></param>
        /// <param name="memberId"></param>
        /// <param name="memberCode"></param>
        /// <param name="template"></param>
        /// <param name="fedId"></param>
        /// <param name="requestType"></param>
        /// <param name="reasonFailure"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorDesc"></param>
        /// <param name="isSuccess"></param>
        void SendIcpFailedEmailNotification(string firstName, string lastName, string emailId, string userCategory,
                                            int memberId, string memberCode, EmailTemplateId template,
                                            string fedId = null, string requestType = null, string reasonFailure = null,
                                            string errorCode = null, string errorDesc = null, bool isSuccess = true);
    }
}
