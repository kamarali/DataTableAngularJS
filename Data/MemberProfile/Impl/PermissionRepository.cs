using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Core.Configuration;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using log4net;

namespace Iata.IS.Data.MemberProfile.Impl
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {

        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<TreeViewPermissions> GetPermissions(int templateId)
        {


            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter("TEMPLATEID", typeof(int))
            {
                Value = templateId
            };


            var permissionList = ExecuteStoredFunction<TreeViewPermissions>("GetPermissionForTreeView", parameters);

            return permissionList.ToList();

        }


        public List<TreeViewPermissions> GetAllPermissionsWithUserIdAssigned(int userId)
        {


            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter("USERID", typeof(int))
            {
                Value = userId
            };


            var permissionList = ExecuteStoredFunction<TreeViewPermissions>("GetPermissionWithUserAssigned", parameters);

            return permissionList.ToList();

        }


        public IList<PermissionToUserList> GetUserPermissions(int userId = -1)
        {

            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter("USERID", typeof(int))
            {
                Value = userId
            };


            var userPermissionList = ExecuteStoredFunction<PermissionToUserList>("GetPermissionToUserList",parameters);
            return userPermissionList.ToList();

        }

        public IList<PermissionListWithCategory> GetPermissionWithCategoryId()
        {

            var userPermissionList = ExecuteStoredFunction<PermissionListWithCategory>("GetPermissionWithCategoryId");

            return userPermissionList.ToList();

        }


        public AspNetSessions GetActiveSessions()
        {

            var userPermissionList = ExecuteStoredFunction<AspNetSessions>("GetAspNetSessions").SingleOrDefault();

            return userPermissionList;

        }


        public List<PermissionToUserList> GetAssignedUserPermissionList(int userId)
        {

            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter("USERID", typeof(int))
            {
                Value = userId
            };

            var userPermissionList = ExecuteStoredFunction<PermissionToUserList>("GetAssignedUserPermission", parameters);

            return userPermissionList.ToList();
        }


        /// <summary>
        /// Get Assigned permission flag 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionId"></param>
        /// <returns>boolean value </returns>
        public bool IsUserAuthorized(int userId, int permissionId)
        {
            var returnValue = false;

            try
            {
                var parameters = new ObjectParameter[3];
                parameters[0] = new ObjectParameter("USERID", typeof(int))
                {
                    Value = userId
                };
                parameters[1] = new ObjectParameter("PERMISSION_ID_IN", typeof(int))
                {
                    Value = permissionId
                };
                parameters[2] = new ObjectParameter("PERMISSION_COUNT", typeof(int));

                ExecuteStoredProcedure("IsPermissionAssignedToUser", parameters);
                int outputParam = Convert.ToInt32(parameters[2].Value);

                if (outputParam == 1)
                {
                    returnValue = true;
                }

            }
            catch (Exception exception)
            {
                returnValue = false;

            }

            return returnValue;
        }

        /// <summary>
        /// This function is used to get user detail based on either email id or federation id.
        /// case 1: if email id is present then federation id should be null. 
        /// case 2: if federation id is present then email id should be null.
        /// </summary>
        /// <param name="email_Id">email id</param>
        /// <param name="federationId">federation id</param>
        /// <returns>return user detail</returns>
        //CMP-665-User Related Enhancements-FRS-v1.2.doc [sec 3.2: Single Sign-On from ICP to SIS; and Conditional Redirection of Such Member Users]
        public ISLoginUser IsUserLogOn(string email_Id, string federationId, int timeFrame)
        {
            try
            {
                var parameters = new ObjectParameter[3];
                parameters[0] = new ObjectParameter("EMAIL_ID_IN", typeof(string))
                {
                    Value = email_Id
                };
                parameters[1] = new ObjectParameter("FEDERATION_ID_IN", typeof(string))
                {
                    Value = federationId
                };
                parameters[2] = new ObjectParameter("TIMEFRAME_IN", typeof(int))
                {
                  Value = timeFrame
                };

                var isUserLogonInfo = ExecuteStoredFunction<ISLoginUser>("GetLogOnUserDetail", parameters);

                return isUserLogonInfo.SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error("Error Occured in IsUserLogOn Repository Function" , exception);
                return null;
            }
        }


        //CMP685 - User Management
        public bool UpdateLogOnUser(int userId, string sessionId, int loginStatusId, bool isCloseOtherSessions = false)
        {
          var returnValue = false;

          try
          {

            //SCPID : 221797 - Password change policy : System does not prompt the user to change their password
            // Removed Password expiry duration parameter.
            var parameters = new ObjectParameter[4];
            parameters[0] = new ObjectParameter("USER_ID_IN", typeof(int))
            {
              Value = userId
            };
            parameters[1] = new ObjectParameter("LOGIN_STATUS_IN", typeof(int))
            {
              Value = loginStatusId
            };

            parameters[2] = new ObjectParameter("SESSION_ID_IN", typeof(string))
            {
              Value = sessionId
            };
            parameters[3] = new ObjectParameter("IS_CLOSE_SESSIONS_IN", typeof(int))
            {
              Value = isCloseOtherSessions
            };

            ExecuteStoredProcedure("UpdateLogOnUserDetails", parameters);

            returnValue = true;

          }
          catch (Exception exception)
          {
            Logger.Error("Error Occured in UpdateLogOnUser Repository Function", exception);

            returnValue = false;

          }

          return returnValue;

        }

        public void SavePermissionsToUser(int userId, string permissionlist, int templateId)
        {
            using (OracleConnection con = new OracleConnection(ConnectionString.Instance.ServiceConnectionString))
            {
                using (OracleCommand dbCommand = new OracleCommand("PROC_SAVE_USER_PERMISSION_LIST"))
                {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                    dbCommand.Connection = con;
                    dbCommand.Parameters.Add("USERID", OracleDbType.Number, ParameterDirection.Input).Value = userId;
                    dbCommand.Parameters.Add("PERMISSION_LIST", OracleDbType.VarChar, ParameterDirection.Input).Value =
                        permissionlist;
                    dbCommand.Parameters.Add("TEMPLATE_ID_I", OracleDbType.Number, ParameterDirection.Input).Value =
                        templateId;

                    con.Open();
                    dbCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
