using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Core.DI;
using System;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class ManageContactsManager : IManageContactsManager
  {

    /// <summary>
    /// Gets or sets the ContactType repository.
    /// </summary>
    /// <value>The ContactType repository.</value>
    public IContactTypeRepository ContactTypesRepository { get; set; }

    //public IContactTypeRepository ContactTypeRepository { get; set; }

    public IContactRepository ContactsRepository { get; set; }

    

    /// <summary>
    /// Add contact type to database
    /// </summary>
    /// <param name="contactType">ContactType class object</param>
    /// <returns></returns>
    public ContactType AddContactType(ContactType contactType)
    {
    //Check whether contact type name already exists in database
        var contactTypeData = ContactTypesRepository.Single(type => type.ContactTypeName.ToLower() == contactType.ContactTypeName.ToLower());

      //If contact type name already exists, throw exception
      if (contactTypeData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidContactTypeName);
      }

      var maxValues = ContactTypesRepository.GetMaxContactTypeIdAndMaxSeqNum();
      contactType.Id = maxValues[0] + 1;
      if (contactType.SequenceNo == null)
        contactType.SequenceNo = maxValues[1] + 1;
      ContactTypesRepository.Add(contactType);
      UnitOfWork.CommitDefault();
      return contactType;
    }

    /// <summary>
    /// Update Contact Type to database
    /// </summary>
    /// <param name="contactType">ContactType class object</param>
    /// <returns></returns>
    public ContactType UpdateContactType(ContactType contactType)
    {
      // First fetch the original entity from the key.
      var originalcontactTypeData = ContactTypesRepository.Single(type => type.Id == contactType.Id);
      var contactTypeData = ContactTypesRepository.Single(type => type.Id != contactType.Id && type.ContactTypeName.ToLower() == contactType.ContactTypeName.ToLower());

      //If contact type name already exists, throw exception
      if (contactTypeData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidContactTypeName);
      }

      // Set dependent field as it is in database, this is an internal field.
      contactType.DependentField = originalcontactTypeData.DependentField;

      //Call repository method for update contact type
      var updatedContactType = ContactTypesRepository.Update(contactType);
      UnitOfWork.CommitDefault();
      return updatedContactType;
    }

    /// <summary>
    /// Delete contact type from database
    /// </summary>
    /// <param name="contactTypeId">ID of contact type to be deleted</param>
    /// <returns></returns>
    public bool DeleteContactType(int contactTypeId)
    {
      // First fetch the entity from the key.
      var contactType = ContactTypesRepository.Single(type => type.Id == contactTypeId);

      if (contactType == null) return false;
      // Mark the entity to be deleted in the context.
      contactType.IsActive = !contactType.IsActive;
      ContactTypesRepository.Update(contactType);
      UnitOfWork.CommitDefault();
      return true;
    }

    public ContactType GetContactTypeDetails(int contactTypeId)
    {
      // Fetch details of contact type corresponding to contact type id passed
      var contactType = ContactTypesRepository.Single(type => type.Id == contactTypeId);
      return contactType;
    }

    /// <summary>
    /// Gets a list of all contact type records.
    /// </summary>
    /// <returns>List of Contact type class objects</returns>
    public IList<ContactType> GetContactTypeList()
    {
      var contactTypeList = ContactTypesRepository.GetAll();

      return contactTypeList.ToList();
    }

    /// <summary>
    /// Gets the contact type list.
    /// </summary>
    /// <param name="typeId">The type id.</param>
    /// <param name="groupId">The group id.</param>
    /// <param name="subGroupId">The sub group id.</param>
    /// <returns></returns>
    public List<ContactType> GetContactTypeList(int typeId, int groupId, int subGroupId)
    {
        var ContactTypeList = new List<ContactType>();
        ContactTypeList = ContactTypesRepository.GetAll().ToList();

        if (typeId>0)
        {
            ContactTypeList = ContactTypeList.Where(cl => cl.TypeId==typeId).ToList();
        }
        if (groupId > 0)
        {
            ContactTypeList = ContactTypeList.Where(cl => cl.GroupId == groupId).ToList();
        }
        if (subGroupId > 0)
        {
            ContactTypeList = ContactTypeList.Where(cl => cl.SubGroupId == subGroupId).ToList();
        }
        return ContactTypeList.ToList();
    }

    /// <summary>
    /// Get User/Contact List based on UserId and Member Id
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public List<LocationAssociation> GetUserContactListForLocAssociation(int userId, int memberId)
    {
        return ContactsRepository.GetUserContactListForLocAssociation(userId,memberId);
    }

    /// <summary>
    /// Get Assigned User/Conact association
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<UserContactLocations> GetUserContactAssignedLocAssociation(int userId)
    {
        return ContactsRepository.GetUserContactAssignedLocAssociation(userId);
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
      public bool InsertLocationAssociation(string locationSelectedIds, string excludedLocIds, int userContactId, string associtionType, string emailId, int loggedInUser, int memberId, int isNewContact = 0)
    {
        return ContactsRepository.InsertLocationAssociation(locationSelectedIds, excludedLocIds, userContactId, associtionType, emailId, loggedInUser, memberId, isNewContact);
        
    }

   
      /// <summary>
    /// MEMBER USERS TO VIEW ‘OWN LOCATION ASSOCIATION’
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<UserAssignedLocation> GetOwnAssignedLocAssociation(int userId)
    {
        return ContactsRepository.GetOwnAssignedLocAssociation(userId);
    }

    /// <summary>
    /// Populate Location Association based on User Id and MemberId 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public List<MemberLocationAssociation> GetMemberAssociationLocForDropdown(int userId, int memberId)
    {
        var memberAssociation = ContactsRepository.GetMemeberAssociationLocation(userId);
        var associationType = GetAssociationType(memberAssociation);

        switch (associationType)
        {
            case (int)Association.None:
                return new List<MemberLocationAssociation>();
            case (int)Association.AllLocation:
                {
                    var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                    var memberLocationList = memberManager.GetMemberLocationDetailsForDropdown(memberId, true).ToList();
                    memberAssociation.AddRange(memberLocationList.Select(item => new MemberLocationAssociation
                                                                                     {
                                                                                         LocationId = item.Id, CurrencyCodeNum = (item.CurrencyId == null ? null : Convert.ToString(item.CurrencyId)), LocationCode = item.LocationCode, CityName = item.CityName, CountryCode = item.CountryId, CurrencyCodeAlfa = (item.Currency == null ? null : item.Currency.Code)
                                                                                     }));
                    return memberAssociation;
                }
            default:
                return memberAssociation.Where(w=> w.IsActive == 1).ToList();
        }
        return memberAssociation;
    }


    /// <summary>
    /// Populate Location Association based on User Id and MemberId 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public List<MemberLocationAssociation> GetMemberAssociationLocForSearch(int userId, int memberId)
    {
        var memberAssociation = ContactsRepository.GetMemeberAssociationLocation(userId);
        var associationType = GetAssociationType(memberAssociation);

        switch (associationType)
        {
            case (int)Association.None:
                return new List<MemberLocationAssociation>();
            case (int)Association.AllLocation:
                {
                    var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                    var memberLocationList = memberManager.GetMemberLocationDetailsForDropdown(memberId, false).ToList();
                    memberAssociation.AddRange(memberLocationList.Select(item => new MemberLocationAssociation
                                                                                     {
                                                                                         LocationId = item.Id, CurrencyCodeNum = (item.CurrencyId == null ? null : Convert.ToString(item.CurrencyId)), LocationCode = item.LocationCode, CityName = item.CityName, CountryCode = item.CountryId, CurrencyCodeAlfa = (item.Currency == null ? null : item.Currency.Code)
                                                                                     }));
                    return GetSortedLocationCode(memberAssociation);
                }
            default:
                return GetSortedLocationCode(memberAssociation);
        }
    }


    /// <summary>
    /// Populate Location Association based on User Id and MemberId 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public List<MemberLocationAssociation> GetMemberAssoLocForInvCapture(int userId, int memberId)
    {
        var memberAssociation = ContactsRepository.GetMemeberAssociationLocation(userId);
        var associationType = GetAssociationType(memberAssociation);

        switch (associationType)
        {
            case (int)Association.None:
                return new List<MemberLocationAssociation>();
            case (int)Association.AllLocation:
                {
                    var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                    var memberLocationList = memberManager.GetMemberLocationDetailsForDropdown(memberId, true).ToList();
                    memberAssociation.AddRange(memberLocationList.Select(item => new MemberLocationAssociation
                    {
                        LocationId = item.Id,
                        CurrencyCodeNum = (item.CurrencyId == null ? null : Convert.ToString(item.CurrencyId)),
                        LocationCode = item.LocationCode,
                        CityName = item.CityName,
                        CountryCode = item.CountryId,
                        CurrencyCodeAlfa = (item.Currency == null ? null : item.Currency.Code)
                    }));
                    return GetSortedLocationCode(memberAssociation);
                }
            default:
                return GetSortedLocationCode(memberAssociation);
        }
    }


    private int GetAssociationType(List<MemberLocationAssociation> listUserContactLocations)
    {
        if (listUserContactLocations.Count >= 1)
        {
            if (listUserContactLocations.Count == 1)
            {
                if (listUserContactLocations[0].LocationId == (int)Association.None)
                {
                    return (int)Association.None;
                }
                return (int)Association.SpecificLocation;
            }
            return (int)Association.SpecificLocation;
        }
        return (int)Association.AllLocation;
    }




    /// <summary>
    ///  Data should be sorted in ascending order of the Location ID’s numeric value
    ///    b.	Exception: 
    ///    i.	Location IDs ‘Main’ and ‘UATP’ should be shown before numeric Locations. 
    ///    ii.	Location ‘Main’ should be shown before ‘UATP’
    /// </summary>
    /// <param name="listLocationCode"></param>
    /// <returns></returns>
    private List<MemberLocationAssociation> GetSortedLocationCode(List<MemberLocationAssociation> listLocationCode)
    {

        if (listLocationCode.Count() > 0)
        {
            var integerMemberLocationList = listLocationCode.ToList();
            integerMemberLocationList.Clear();
            var stringMemberLocationList = listLocationCode.ToList();
            stringMemberLocationList.Clear();

            foreach (var mll in listLocationCode)
            {
                if (Regex.IsMatch(mll.LocationCode, @"^[0-9]+$"))
                {
                    integerMemberLocationList.Add(mll);
                }
                else stringMemberLocationList.Add(mll);
            }

            if (integerMemberLocationList.Count != 0)
                integerMemberLocationList = integerMemberLocationList.OrderBy(l => int.Parse(l.LocationCode)).ToList();

            if (stringMemberLocationList.Count != 0)
                stringMemberLocationList = stringMemberLocationList.OrderBy(l => l.LocationCode).ToList();

            if (integerMemberLocationList.Count != 0)
                stringMemberLocationList.AddRange(integerMemberLocationList);
            return stringMemberLocationList.ToList();
        }
        return listLocationCode;
    }
   
  }
}
