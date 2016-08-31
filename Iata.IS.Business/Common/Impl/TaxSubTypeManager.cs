using System.Collections.Generic;
using System.Linq;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Data.Impl;

namespace Iata.IS.Business.Common.Impl
{
  public class TaxSubTypeManager : ITaxSubTypeManager
  {
    /// <summary>
    /// Gets or sets the taxSubType repository.
    /// </summary>
    /// <value>
    /// The taxSubType repository.
    /// </value>
    public IRepository<TaxSubType> TaxSubTypeRepository { get; set; }

    /// <summary>
    /// Adds the taxSubType.
    /// </summary>
    /// <param name="taxSubType">The taxSubType.</param>
    /// <returns></returns>
    public TaxSubType AddTaxSubType(TaxSubType taxSubType)
    {
      //var taxSubTypeData = TaxSubTypeRepository.Single(type => type.SubType == taxSubType.SubType);
      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // Added filter for Tax Type.
      var taxSubTypeData = TaxSubTypeRepository.Single(type => type.SubType == taxSubType.SubType && type.Type == taxSubType.Type);
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      //If TaxSubType Code already exists, throw exception
      if (taxSubTypeData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTaxSubType);
      }
      //Call repository method for adding taxSubType
      TaxSubTypeRepository.Add(taxSubType);
      UnitOfWork.CommitDefault();
      return taxSubType;
    }

    /// <summary>
    /// Updates the taxSubType.
    /// </summary>
    /// <param name="taxSubType">The taxSubType.</param>
    /// <returns></returns>
    public TaxSubType UpdateTaxSubType(TaxSubType taxSubType)
    {
      var taxSubTypeData = TaxSubTypeRepository.Single(type => type.Id != taxSubType.Id && type.SubType == taxSubType.SubType);
      //If TaxSubType Code already exists, throw exception
      if (taxSubTypeData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTaxSubType);
      }
      taxSubTypeData = TaxSubTypeRepository.Single(type => type.Id == taxSubType.Id);
      var updatedtaxSubType = TaxSubTypeRepository.Update(taxSubType);
      UnitOfWork.CommitDefault();
      return updatedtaxSubType;
    }

    /// <summary>
    /// Deletes the taxSubType.
    /// </summary>
    /// <param name="taxSubTypeId">The taxSubType id.</param>
    /// <returns></returns>
    public bool DeleteTaxSubType(int taxSubTypeId)
    {
      bool delete = false;
      var taxSubTypeData = TaxSubTypeRepository.Single(type => type.Id == taxSubTypeId);
      if (taxSubTypeData != null)
      {
        taxSubTypeData.IsActive = !(taxSubTypeData.IsActive);
        var updatedcountry = TaxSubTypeRepository.Update(taxSubTypeData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the taxSubType details.
    /// </summary>
    /// <param name="taxSubTypeId">The taxSubType id.</param>
    /// <returns></returns>
    public TaxSubType GetTaxSubTypeDetails(int taxSubTypeId)
    {
      var taxSubType = TaxSubTypeRepository.Single(type => type.Id == taxSubTypeId);
      return taxSubType;
    }

    /// <summary>
    /// Gets all taxSubType list.
    /// </summary>
    /// <returns></returns>
    public List<TaxSubType> GetAllTaxSubTypeList()
    {
      var taxSubTypeList = TaxSubTypeRepository.GetAll();
      return taxSubTypeList.ToList();
    }

    /// <summary>
    /// Gets the taxSubType list.
    /// </summary>
    /// <param name="Code">The code.</param>
    /// <param name="Name">The name.</param>
    /// <returns></returns>
    public List<TaxSubType> GetTaxSubTypeList(string taxSubType, string taxType)
    {
      var taxSubTypeList = new List<TaxSubType>();
      taxSubTypeList = TaxSubTypeRepository.GetAll().ToList();
      if (!string.IsNullOrEmpty(taxSubType))
      {
        taxSubTypeList = taxSubTypeList.Where(cl => cl.SubType.ToLower().Contains(taxSubType.ToLower())).ToList();
      }
      if (!string.IsNullOrEmpty(taxType))
      {
        taxSubTypeList = taxSubTypeList.Where(cl => cl.Type.ToLower().Contains(taxType.ToLower())).ToList();
      }
      return taxSubTypeList.ToList();
    }
  }
}
