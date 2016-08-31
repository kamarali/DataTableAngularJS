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
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class BillingCategoryManager : IBillingCategoryManager
  {
    /// <summary>
    /// Gets or sets the billing category repository.
    /// </summary>
    /// <value>
    /// The billing category repository.
    /// </value>
    public IRepository<BillingCategory> BillingCategoryRepository { get; set; }

    /// <summary>
    /// Adds the billing category.
    /// </summary>
    /// <param name="billingCategory">The billing category.</param>
    /// <returns></returns>
    public BillingCategory AddBillingCategory(BillingCategory billingCategory)
    {
      var billingCategoryData = BillingCategoryRepository.Single(type => type.Id == billingCategory.Id);
      //If BillingCategory Code already exists, throw exception
      if (billingCategoryData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidCountryCode);
      }
      //Call repository method for adding billingCategory
      BillingCategoryRepository.Add(billingCategory);
      UnitOfWork.CommitDefault();
      return billingCategory;
    }

    /// <summary>
    /// Updates the billing category.
    /// </summary>
    /// <param name="billingCategory">The billing category.</param>
    /// <returns></returns>
    public BillingCategory UpdateBillingCategory(BillingCategory billingCategory)
    {
      var billingCategoryData = BillingCategoryRepository.Single(type => type.Id == billingCategory.Id);
      var updatedbillingCategory = BillingCategoryRepository.Update(billingCategory);
      UnitOfWork.CommitDefault();
      return updatedbillingCategory;
    }

    /// <summary>
    /// Deletes the billing category.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    public bool DeleteBillingCategory(int billingCategoryId)
    {
      bool delete = false;
      var billingCategoryData = BillingCategoryRepository.Single(type => type.Id == billingCategoryId);
      if (billingCategoryData != null)
      {
        billingCategoryData.IsActive = !(billingCategoryData.IsActive);
        var updatedcountry = BillingCategoryRepository.Update(billingCategoryData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the billing category details.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    public BillingCategory GetBillingCategoryDetails(int billingCategoryId)
    {
      var billingCategory = BillingCategoryRepository.Single(type => type.Id == billingCategoryId);
      return billingCategory;
    }

    /// <summary>
    /// Gets all billing category list.
    /// </summary>
    /// <returns></returns>
    public List<BillingCategory> GetAllBillingCategoryList()
    {
      var billingCategoryList = BillingCategoryRepository.GetAll();
      return billingCategoryList.ToList();
    }

    /// <summary>
    /// Gets the billing category list.
    /// </summary>
    /// <param name="codeIsxml">The code is-xml.</param>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public List<BillingCategory> GetBillingCategoryList(string codeIsxml, string description)
    {
      var billingCategoryList = BillingCategoryRepository.GetAll().ToList();

      if (!string.IsNullOrEmpty(codeIsxml))
      {
        billingCategoryList = billingCategoryList.Where(cl => (cl.CodeIsxml.ToLower().Contains(codeIsxml.ToLower()))).ToList();
      }
      if (!string.IsNullOrEmpty(description))
      {
          billingCategoryList = billingCategoryList.Where(cl => cl.Description != null && (cl.Description.ToLower().Contains(description.ToLower()))).ToList();
      }
      return billingCategoryList.ToList();
    }
  }
}
