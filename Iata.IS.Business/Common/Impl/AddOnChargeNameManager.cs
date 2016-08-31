using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class AddOnChargeNameManager : IAddOnChargeNameManager
  {
    /// <summary>
    /// Gets or sets the add on charge name repository.
    /// </summary>
    /// <value>
    /// The add on charge name repository.
    /// </value>
    public IRepository<AddOnChargeName> AddOnChargeNameRepository { get; set; }

    /// <summary>
    /// Adds the name of the add on charge.
    /// </summary>
    /// <param name="addOnChargeName">Name of the add on charge.</param>
    /// <returns></returns>
    public AddOnChargeName AddAddOnChargeName(AddOnChargeName addOnChargeName)
    {
      var AddOnChargeNameData = AddOnChargeNameRepository.Single(type => type.Name.ToLower() == addOnChargeName.Name.ToLower());
      //If AddOnChargeName Code already exists, throw exception
      if (AddOnChargeNameData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAddOnChargeName);
      }
      AddOnChargeNameRepository.Add(addOnChargeName);
      UnitOfWork.CommitDefault();
      return addOnChargeName;
    }

    /// <summary>
    /// Updates the name of the add on charge.
    /// </summary>
    /// <param name="addOnChargeName">Name of the add on charge.</param>
    /// <returns></returns>
    public AddOnChargeName UpdateAddOnChargeName(AddOnChargeName addOnChargeName)
    {
      var AddOnChargeNameData = AddOnChargeNameRepository.Single(type => type.Name.ToLower() == addOnChargeName.Name.ToLower() && type.Id != addOnChargeName.Id);
      //If AddOnChargeName Code already exists, throw exception
      if (AddOnChargeNameData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAddOnChargeName);
      }
      AddOnChargeNameData = AddOnChargeNameRepository.Single(type => type.Id == addOnChargeName.Id);
      var updatedAddOnChargeName = AddOnChargeNameRepository.Update(addOnChargeName);
      UnitOfWork.CommitDefault();
      return updatedAddOnChargeName;
    }

    /// <summary>
    /// Deletes the name of the add on charge.
    /// </summary>
    /// <param name="addOnChargeNameId">The add on charge name id.</param>
    /// <returns></returns>
    public bool DeleteAddOnChargeName(int addOnChargeNameId)
    {
      bool delete = false;
      var addOnChargeNameData = AddOnChargeNameRepository.Single(type => type.Id == addOnChargeNameId);
      if (addOnChargeNameData != null)
      {
        addOnChargeNameData.IsActive = !(addOnChargeNameData.IsActive);
        var updatedcountry = AddOnChargeNameRepository.Update(addOnChargeNameData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the add on charge name details.
    /// </summary>
    /// <param name="addOnChargeNameId">The add on charge name id.</param>
    /// <returns></returns>
    public AddOnChargeName GetAddOnChargeNameDetails(int addOnChargeNameId)
    {
      var AddOnChargeName = AddOnChargeNameRepository.Single(type => type.Id == addOnChargeNameId);
      return AddOnChargeName;
    }

    /// <summary>
    /// Gets all add on charge name list.
    /// </summary>
    /// <returns></returns>
    public List<AddOnChargeName> GetAllAddOnChargeNameList()
    {
      var addOnChargeNameList = AddOnChargeNameRepository.GetAll();
      return addOnChargeNameList.ToList();
    }

    /// <summary>
    /// Gets the add on charge name list.
    /// </summary>
    /// <param name="addOnChargeName">Name of the add on charge.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    public List<AddOnChargeName> GetAddOnChargeNameList(string addOnChargeName, int billingCategoryId)
    {
      var addOnChargeNameList = AddOnChargeNameRepository.GetAll().ToList();

      if ((!string.IsNullOrEmpty(addOnChargeName)))
      {
        addOnChargeNameList = addOnChargeNameList.Where(cl => (cl.Name.ToLower().Contains(addOnChargeName.ToLower()))).ToList();
      }
      if (billingCategoryId > 0)
      {
        addOnChargeNameList = addOnChargeNameList.Where(cl => (cl.BillingCategoryId == billingCategoryId)).ToList();
      }
      return addOnChargeNameList.ToList();
    }

  }
}
