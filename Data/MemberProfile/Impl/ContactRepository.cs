using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class ContactRepository : Repository<Contact>, IContactRepository
  {
    /// <summary>
    /// Deletes Contact. 
    /// </summary>
    /// <param name="contactId">The ContactId.</param>
    /// <returns></returns>
    public int DeleteMemberContact(int contactId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter("CONTACT_ID_I", typeof(int)) { Value = contactId };
      parameters[1] = new ObjectParameter("DEL_O", typeof(int));
      ExecuteStoredProcedure("DeleteMemberContact", parameters);

      return int.Parse(parameters[1].Value.ToString());

    }

    /// <summary>
    /// Get information for contact and contact type assignment
    /// </summary>
    /// <param name="searchCriteria">search criteria.</param>
    /// <param name="memberId">logged in member id</param>
    /// <param name="userCategoryId">User category of logged in user</param>
    /// <param name="recordCount">total record count</param>
    /// <returns>data table containing search result</returns>
    public DataTable GetContactAssignmentData(ContactAssignmentSearchCriteria searchCriteria, int memberId, int userCategoryId, out int recordCount)
    {
      recordCount = 0;
      var connection = new OracleConnection(Core.Configuration.ConnectionString.Instance.ServiceConnectionString);
      try
      {
        connection.Open();

        var command = new OracleCommand("PROC_GET_MEM_CONTACT", connection)
        {
          CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("MEMBER_ID_I", memberId);
        command.Parameters["MEMBER_ID_I"].Direction = System.Data.ParameterDirection.Input;

        command.Parameters.Add("Contact_Type_Category_i", searchCriteria.ContactTypeCategory);
        command.Parameters["Contact_Type_Category_i"].Direction = System.Data.ParameterDirection.Input;

        command.Parameters.Add("USER_CATEGORY_I", userCategoryId);
        command.Parameters["USER_CATEGORY_I"].Direction = System.Data.ParameterDirection.Input;

        if (!string.IsNullOrEmpty(searchCriteria.TypeId))
        {
          command.Parameters.Add("Type_i", int.Parse(searchCriteria.TypeId));
          command.Parameters["Type_i"].Direction = System.Data.ParameterDirection.Input;
        }
        else
        {
          command.Parameters.Add("Type_i", 0);
          command.Parameters["Type_i"].Direction = System.Data.ParameterDirection.Input;
        }
        if (!string.IsNullOrEmpty(searchCriteria.GroupId))
        {
          command.Parameters.Add("Group_ID_i", int.Parse(searchCriteria.GroupId));
          command.Parameters["Group_ID_i"].Direction = System.Data.ParameterDirection.Input;
        }

        else
        {
          command.Parameters.Add("Group_ID_i", 0);
          command.Parameters["Group_ID_i"].Direction = System.Data.ParameterDirection.Input;
        }
        if (!string.IsNullOrEmpty(searchCriteria.SubGroupId))
        {
          command.Parameters.Add("Sub_Group_ID_i", int.Parse(searchCriteria.SubGroupId));
          command.Parameters["Sub_Group_ID_i"].Direction = System.Data.ParameterDirection.Input;
        }
        else
        {
          command.Parameters.Add("Sub_Group_ID_i", 0);
          command.Parameters["Sub_Group_ID_i"].Direction = System.Data.ParameterDirection.Input;
        }

        command.Parameters.Add("PAGE_NO_I", 1);
        command.Parameters["PAGE_NO_I"].Direction = ParameterDirection.Input;
        command.Parameters.Add("PAGE_SIZE_I", 200);
        command.Parameters["PAGE_SIZE_I"].Direction = ParameterDirection.Input;

        //output parameter for total record count.
        command.Parameters.Add("RECORD_COUNT_O", 0);
        command.Parameters["RECORD_COUNT_O"].Direction = ParameterDirection.Output;

        var ds = new DataSet();
        var adapter = new OracleDataAdapter(command);

        adapter.Fill(ds);

        int.TryParse(command.Parameters["RECORD_COUNT_O"].Value.ToString(), out recordCount);

        connection.Close();

        return ds.Tables[0];
      }
      finally
      {
        connection.Close();
      }
    }

    /// <summary>
    /// Checks if only contact assigned for contact type.
    /// </summary>
    /// <param name="contactId">The contactId.</param>
    /// <returns>true if only contact assigned , false otherwise.</returns>
    public int IsOnlyContact(int contactId, int memberId, int contactTypeId)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter("CONTACT_ID_I", typeof(int)) { Value = contactId };
      parameters[1] = new ObjectParameter("MEMBER_ID_I", typeof(int)) { Value = memberId };
      parameters[2] = new ObjectParameter("CONTACT_TYPE_ID_I", typeof(int)) { Value = contactTypeId };
      parameters[3] = new ObjectParameter("DEL_O", typeof(int));
      ExecuteStoredProcedure("IsOnlyContact", parameters);

      return int.Parse(parameters[3].Value.ToString());

    }

    /// <summary>
    /// Get the contact information. 
    /// If email id exists in User table then return first name, last name and staff id from user table 
    /// otherwise return this information from contact table
    /// </summary>
    /// <param name="memberId">member id</param>
    /// <param name="firstName">first name</param>
    /// <param name="lastName">last name</param>
    /// <param name="emailAddress">email address</param>
    /// <param name="staffId">staff id</param>
    /// <param name="userCategory">User category of logged in user</param>
    /// <returns>list of ContactData objects.</returns>
    public List<ContactData> GetContactUserInformation(int memberId, string firstName, string lastName, string emailAddress, string staffId, int userCategory)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(ContactConstants.MemberIdParameterName, typeof(int)) { Value = memberId };
      parameters[1] = new ObjectParameter(ContactConstants.FirstNameParameterName, string.IsNullOrEmpty(firstName) ? string.Empty : firstName);
      parameters[2] = new ObjectParameter(ContactConstants.LastNameParameterName, string.IsNullOrEmpty(lastName) ? string.Empty : lastName);
      parameters[3] = new ObjectParameter(ContactConstants.EmailIdParameterName, string.IsNullOrEmpty(emailAddress) ? string.Empty : emailAddress);
      parameters[4] = new ObjectParameter(ContactConstants.StaffIdParameterName, string.IsNullOrEmpty(staffId) ? string.Empty : staffId);
      parameters[5] = new ObjectParameter(ContactConstants.UserCategoryIdParameterName, userCategory);
      return ExecuteStoredFunction<ContactData>(ContactConstants.GetContactUserInformationFunctionName, parameters).ToList();
    }


      
        /// <summary>
        /// SCP85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
        /// Retrieves the contact information using member id and contact ype id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="contactTypeId"></param>
        /// <returns></returns>
            public List<Contact> GetContactMemberInformation(int memberId,int contactTypeId)
            {
                var parameters = new ObjectParameter[2];
                parameters[0] = new ObjectParameter(ContactConstants.MemberIdForContactParameterName, typeof(int)) { Value = memberId };
                parameters[1] = new ObjectParameter(ContactConstants.ContactTypeIdParameterName, typeof(int)) { Value = contactTypeId };
                return ExecuteStoredFunction<Contact>(ContactConstants.GetContactMemberInformationFunctionName, parameters).ToList();
            }
  
      /// <summary>
      /// Retrieves the contact information using member id and contact ype id
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="contactTypeId"></param>
      /// <param name="billingCategory"></param>
      /// <param name="miscLocationCode"></param>
      /// <returns></returns>
    public List<Contact> GetContactsForMiscOutputAlerts(int memberId, int contactTypeId,  string miscLocationCode)
    {
        var parameters = new ObjectParameter[3];
        parameters[0] = new ObjectParameter(ContactConstants.MemberIdForContactParameterName, typeof(int)) { Value = memberId };
        parameters[1] = new ObjectParameter(ContactConstants.ContactTypeIdParameterName, typeof(int)) { Value = contactTypeId };
        parameters[2] = new ObjectParameter(ContactConstants.MiscLocCodeParameterName, typeof(string)) { Value = miscLocationCode };
        return ExecuteStoredFunction<Contact>(ContactConstants.GetContactsForMiscOutputAlertsFunctionName, parameters).ToList();
    }
      /// <summary>
      /// CMP#655(2.1.3)IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
    public List<LocationAssociation> GetUserContactListForLocAssociation(int userId, int memberId)
    {
        var parameters = new ObjectParameter[2];
        parameters[0] = new ObjectParameter(ContactConstants.MemberIdParameterName, typeof(int)) { Value = memberId };
        parameters[1] = new ObjectParameter(ContactConstants.UserIdParameterName, typeof(int)) { Value = userId };
        return ExecuteStoredFunction<LocationAssociation>(ContactConstants.GetUserContactFunctionName, parameters).ToList();

    }

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    public List<UserContactLocations> GetUserContactAssignedLocAssociation(int userId)
    {
        var parameters = new ObjectParameter[1];        
        parameters[0] = new ObjectParameter(ContactConstants.UserIdParameterName, typeof(int)) { Value = userId };
        return ExecuteStoredFunction<UserContactLocations>(ContactConstants.GetUserContactLocFunctionName, parameters).ToList();

    }

      /// <summary>
      ///  save the changes made to the Location Association of the Target User/contact
      /// </summary>
      /// <param name="locationSelectedIds"> Associated Location Ids </param>
      /// <param name="excludedLocIds"> Invisible Location Ids on screen. These should be excluded while saving the record</param>
      /// <param name="userContactId">Target User/Contact Id </param>
      /// <param name="associtionType"> Association Type</param>
      /// <param name="emailId">Email Id </param>
      /// <param name="loggedInUser">Logged In User</param>
      /// <param name="memberId">Member</param>
      /// <param name="isNewContact"></param>
      /// <returns>1/0</returns>
      public bool InsertLocationAssociation(string locationSelectedIds, string excludedLocIds, int userContactId, string associtionType, string emailId, int loggedInUser, int memberId, int isNewContact =0)
    {
        var parameters = new ObjectParameter[9];
        parameters[0] = new ObjectParameter("USER_CONTACT_ID_I", typeof(int)) { Value = userContactId };
        parameters[1] = new ObjectParameter("SELECTED_LOC_IDS", typeof(string)) { Value = locationSelectedIds };
        parameters[2] = new ObjectParameter("EXCLUDED_LOC_IDS", typeof(string)) { Value = excludedLocIds };
        parameters[3] = new ObjectParameter("EMAIL_ID_I", typeof(string)) { Value = emailId };
        parameters[4] = new ObjectParameter("LOGGEDIN_USER_I", typeof(int)) { Value = loggedInUser };
        parameters[5] = new ObjectParameter("MEMBER_ID_I", typeof(int)) { Value = memberId };
        parameters[6] = new ObjectParameter("ASSOCIATION_TYPE_I", typeof(int)) { Value = associtionType };
        parameters[7] = new ObjectParameter("IS_NEW_CONTACT_I", typeof(int)) { Value = isNewContact };
        parameters[8] = new ObjectParameter("RESULT_O", typeof(int));
        ExecuteStoredProcedure("InsertLocationAssociation", parameters);

        if (int.Parse(parameters[8].Value.ToString()) == 1)
            return true;
        else
            return false;

    }

      

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    public List<UserAssignedLocation> GetOwnAssignedLocAssociation(int userId)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(ContactConstants.UserIdParameterName, typeof(int)) { Value = userId };
        return ExecuteStoredFunction<UserAssignedLocation>(ContactConstants.GetOwnLocFunctionName, parameters).ToList();

    }

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    public List<MemberLocationAssociation> GetMemeberAssociationLocation(int userId)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(ContactConstants.UserIdParameterName, typeof(int)) { Value = userId };

        return ExecuteStoredFunction<MemberLocationAssociation>(ContactConstants.GetMemberAssociationLocation, parameters).ToList();        
    }

  }
}
