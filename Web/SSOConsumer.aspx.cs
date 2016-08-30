using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using ComponentPro.Saml2;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    /// <summary>
    /// SSOConsumer class for single sing on.
    /// CMP #665: User Related Enhancements-FRS-v1.2.doc [sec 3.2: Single Sign-On from ICP to SIS; and Conditional Redirection of Such Member Users]
    /// </summary>
    public partial class SSOConsumer : Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ISisMemberSubStatusManager _sisMemberSubStatusManager;
        private IPermissionManager _permissionManager;
        private IUserManager _memberManager;

        /// <summary>
        /// Default Constructer
        /// </summary>
        public SSOConsumer()
        {
            InitiliazeComponent();
        }

        /// <summary>
        /// Initialize Component
        /// </summary>
        private void InitiliazeComponent()
        {
            _sisMemberSubStatusManager = Ioc.Resolve<ISisMemberSubStatusManager>(typeof(ISisMemberSubStatusManager));
            _permissionManager = Ioc.Resolve<IPermissionManager>(typeof(IPermissionManager));
            _memberManager = Ioc.Resolve<IUserManager>(typeof(IUserManager));
        }

        /// <summary>
        /// This function is used to Received and Process SAML Response and if user valid then redirect to Home page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="ApplicationException">
        /// SSO Login Failure: Invalid Request. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: Request signature is not valid. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: Request is not signed. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: Login failed. There is some issue with Request. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: There is no assertion found in SAML response. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: Federation ID is not found in the Request. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: Maximum allowed users are already logged in. Please try after sometime or contact SIS Ops.
        /// or
        /// SSO Login Failure: Login Failed due to inactive user. Please contact SIS Ops.
        /// or
        /// SSO Login Failure: Login Failed due to invalid federation id. Please contact SIS Ops.
        /// </exception>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Receive SAML Response

                Response samlResponse;
                try
                {
                    //Create a SAML response from the HTTP request.
                    samlResponse = ComponentPro.Saml2.Response.Create(Request);
                    Logger.InfoFormat("SAML Response: {0}", samlResponse);

                }
                catch (Exception exception)
                {
                    Logger.InfoFormat("Error Occurred while create response: {0}", exception);

                    //SSO Login Failure: Invalid XML Request. Please contact SIS Ops.
                    //SCP#457458: FW: CMP 665 - new error messages
                    throw new ApplicationException("SSO Login Failure: An Invalid Request was received from the IATA Customer Portal. Please contact the IATA Customer Portal support team.");
                }


                //Get certification file path from sisconfig.xml.
                var certFilePath = Core.Configuration.ConnectionString.GetconfigAppSetting("ICP_SSO_Certification_FilePath");

                //Testing Purpose.
                //const string certFilePath = @"D:\CMP 665\SAML\ISS_Portal.crt";

                Logger.InfoFormat("Certification File Path: {0}", certFilePath);

                //Check  given response is signed or not.
                if (samlResponse.IsSigned())
                {
                    //Loaded the certificate.
                    var x509Certificate = new X509Certificate2(certFilePath);

                    //Validate the SAML response with the certificate.
                    if (!samlResponse.Validate(x509Certificate))
                    {
                        //SCP#457458: FW: CMP 665 - new error messages
                        throw new ApplicationException("SSO Login Failure: The Request Signature provided by the IATA Customer Portal is not valid. Please contact the IATA Customer Portal support team.");
                    }
                }
                else
                {
                    //SCP#457458: FW: CMP 665 - new error messages
                    throw new ApplicationException("SSO Login Failure: The Request provided by the IATA Customer Portal is not signed. Please contact the IATA Customer Portal support team.");
                }

                #endregion

                #region Process the response

                //Success?
                if (!samlResponse.IsSuccess())
                {
                    //SCP#457458: FW: CMP 665 - new error messages
                    throw new ApplicationException("SSO Login Failure: There was an unexpected error in SIS. Please contact the SIS Help Desk.");
                }

                Assertion samlAssertion;

                // Get the asserted identity from saml response.
                if (samlResponse.GetAssertions().Count > 0)
                {
                    samlAssertion = samlResponse.GetAssertions()[0];
                }
                else
                {
                    //SCP#457458: FW: CMP 665 - new error messages
                    throw new ApplicationException("SSO Login Failure: There is no assertion found in SAML Response. Please contact the IATA Customer Portal support team.");
                }

                // Get the subject name identifier.
                string federationId;
                if (samlAssertion.Subject.NameId != null)
                {
                    federationId = samlAssertion.Subject.NameId.NameIdentifier;
                }
                else
                {
                    //SCP#457458: FW: CMP 665 - new error messages
                    throw new ApplicationException("SSO Login Failure: Federation ID was missing in the Request provided by the IATA Customer Portal. Please contact the IATA Customer Portal support team.");
                }

                Logger.InfoFormat("Federation Id: {0}", federationId);

                // Get User Details based on federation ID.
                var loginUserInfo = _permissionManager.IsUserLogOn(0,null,federationId: federationId.Trim());

                if (loginUserInfo != null)
                {
                    var updatedUserInfo = _permissionManager.UpdateLogOnUser(loginUserInfo.IS_USER_ID, Session.SessionID, loginUserInfo.LoginStutusId);

                    //Get count of MaxLoginAllowed in SIS system
                    var maxLoginAllowed = SystemParameters.Instance.General.MaxLoginAllowed;
                    if (loginUserInfo.ActiveSessionCount > maxLoginAllowed) // ReSharper disable HeuristicUnreachableCode
                    {
                        //SCP#457458: FW: CMP 665 - new error messages
                        throw new ApplicationException("SSO Login Failure: Maximum allowed users are already logged in. Please try connecting again.");
                    }

                    //Check current user and corresponding member is active or not. if user is not active then display error message otherwise login into SIS.
                    if ((loginUserInfo.IsUserActive == 1 && loginUserInfo.IsUserLocked == 0) && ((loginUserInfo.MemberId > 0 && loginUserInfo.IsMemberStatusId == 1) || loginUserInfo.MemberId <= 0))
                    {
                        //Set session for login user in SIS.
                        FormsAuthentication.SetAuthCookie(loginUserInfo.IS_USER_ID.ToString(), false);
                        SessionUtil.UserId = loginUserInfo.IS_USER_ID;
                        SessionUtil.AdminUserId = loginUserInfo.IS_USER_ID;
                        SessionUtil.UserLanguageCode = loginUserInfo.LanguageCode;
                        SessionUtil.OperatingUserId = loginUserInfo.IS_USER_ID;
                        SessionUtil.Username = string.Format("{0} {1}", loginUserInfo.FirstName, loginUserInfo.LastName);
                        SessionUtil.UserCategory = (UserCategory)loginUserInfo.UserCategoryId;
                        SessionUtil.MemberName = loginUserInfo.MembercommercialName;
                        SessionUtil.IsLogOutProxyOption = false;

                        //Get permission list based on user id.
                        var permissionKeyList = _permissionManager.GetUserPermissions(loginUserInfo.IS_USER_ID);

                        // Save the permissions (either in the cache or a plan B data store).
                        _memberManager.SaveUserPermissions(loginUserInfo.IS_USER_ID, permissionKeyList);
                        SessionUtil.MemberId = loginUserInfo.MemberId;
                        SessionUtil.IsLoggedIn = true;

                        //CMP-665-User Related Enhancements-FRS-v1.2.doc
                        //[Sec 2.8 Conditional Redirection of Users upon Login in IS-WEB]
                        //If RedirectUponLogin is true based on IsMemberSubStatus then redirect to ManageMiscDailyPayablesInvoice page otherwise home page.
                        if (loginUserInfo.UserCategoryId == int.Parse(AppSettings.UserCategoryMember))
                        {
                            //Get value of RedirectUponLogin from sisMemberSubStatusDetails.
                            bool redirectUponLogin =
                                _sisMemberSubStatusManager.GetSisMemberSubStatusDetails(
                                    loginUserInfo.IsMemberSubStatusId).RedirectUponLogin;

                            //If RedirectUponLogin is true then redirect to ManageMiscDailyPayablesInvoice page otherwise home page.
                            if (redirectUponLogin)
                            {
                                isRedirectUponLogin.Value = "true";
                                return;
                            }
                        }

                        //Redirect to Home page.
                        isRedirectUponLogin.Value = "false";
                    }
                    else
                    {
                        //Throw error if user does not exist in SIS.
                        //SCP#457458: FW: CMP 665 - new error messages
                        throw new ApplicationException("SSO Login Failure: This user is in an Inactive status in SIS. Please contact the SIS Help Desk.");
                    }
                }
                else
                {
                    Logger.ErrorFormat("Federation is not exist in SIS, federation Id: {0}", federationId);
                    //Throw error if user does not exist in SIS.);
                    //SCP#457458: FW: CMP 665 - new error messages
                    throw new ApplicationException("SSO Login Failure: The Federation ID in the Request provided by the IATA Customer Portal does not exist in SIS. Please contact the IATA Customer Portal support team.");
                }

                #endregion
            }
            catch (ApplicationException applicationException)
            {
                //Redirect to Error page with appropriate error message.
                Response.Redirect("Account/SSOError?title=" + applicationException.Message, false);
                Logger.ErrorFormat("Error occurred while process SAML Request: {0}.", applicationException.Message);
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat("ServiceProvider An Error occurred, exception: {0}.", exception);
                throw;
            }
        }
    }
}