using System.Collections.Generic;
using System.Linq;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Master;
using System.Globalization;
using System;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Impl;

namespace Iata.IS.Business.Common.Impl
{
  public class ChargeCodeTypeManager : IChargeCodeTypeManager
  {
    /// <summary>
    /// Gets or sets the charge code repository.
    /// </summary>
    /// <value>
    /// The charge code repository.
    /// </value>
    public IChargeCodeTypeRepository ChargeCodeTypeRepository { get; set; }

    /// <summary>
    /// Gets all the Charge Code Types from the data base
    /// </summary>
    /// <returns>List of Charge Code Type</returns>
    public List<ChargeCodeType> GetAllChargeCodeTypeList()
    {
      var chargeCodeTypeList = ChargeCodeTypeRepository.GetAll();
      return chargeCodeTypeList.ToList();
    }

    /// <summary>
    /// This function is used to get misc charge code type based on criteria.
    /// </summary>
    /// <param name="ChargeCategoryId"></param>
    /// <param name="ChargeCodeId"></param>
    /// <param name="chargeCodeTypeName"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization.
    public List<ChargeCodeTypeSearchData> GetMiscChargeCodeType(int chargeCategoryId, int chargeCodeId, string chargeCodeTypeName)
    {
      var filteredList = ChargeCodeTypeRepository.GetMiscChargeCodeType(chargeCategoryId, chargeCodeId, chargeCodeTypeName);
      return filteredList;
    }

    /// <summary>
    /// This function is used to add chargeCodeType values.
    /// </summary>
    /// <param name="chargeCodeType"></param>
    /// <param name="userId"></param>
    //CMP #636: Standard Update Mobilization
    public void AddChargeCodeType(ChargeCodeType chargeCodeType, int userId)
    {
      chargeCodeType.Name = chargeCodeType.Name.Trim();

      var chargeCodeTypeData = ChargeCodeTypeRepository.First(type => type.ChargeCodeId == chargeCodeType.ChargeCodeId && type.Name.ToUpper() == chargeCodeType.Name.ToUpper());

      //If same charge code type already exist then throw duplicate error.
      if (chargeCodeTypeData != null)
      {
        throw new ISBusinessException(Messages.DuplicateChargeCodeTypeName);
      }

      chargeCodeType.LastUpdatedBy = userId;
      ChargeCodeTypeRepository.Add(chargeCodeType);
      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// This function is used to update charge code type data.
    /// </summary>
    /// <param name="chargeCodeType"></param>
    /// <param name="userId"></param>
    //CMP #636: Standard Update Mobilization
    public void UpdateChargeCodeType(ChargeCodeType chargeCodeType, int userId)
    {
      //Get charge code type data based on id.
      var chargeCodeTypeData = ChargeCodeTypeRepository.Single(type => type.Id == chargeCodeType.Id);
     
      //Get charge code type data based on id.
      var chargeCodeExistingdata = ChargeCodeTypeRepository.First(type => type.Id != chargeCodeType.Id && type.ChargeCodeId == chargeCodeTypeData.ChargeCodeId && type.Name.ToUpper() == chargeCodeType.Name.Trim().ToUpper());
       
      //If same charge code type already exist then throw duplicate error.
      if (chargeCodeExistingdata != null)
      {
        throw new ISBusinessException(Messages.DuplicateChargeCodeTypeName);
      }

      //Update value for charge code type.
      chargeCodeTypeData.Name = chargeCodeType.Name.Trim();
      chargeCodeTypeData.IsActive = chargeCodeType.IsActive;
      chargeCodeTypeData.LastUpdatedBy = userId;
      ChargeCodeTypeRepository.Update(chargeCodeTypeData);

      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// This function is used to delete charge code type
    /// </summary>
    /// <param name="chargeCodeTypeId"></param>
    /// <param name="userId"></param>
    //CMP #636: Standard Update Mobilization
    public void DeleteChargeCodeType(int chargeCodeTypeId, int userId)
    {
      //Update charge code type data.
      var chargeCodeTypeData = ChargeCodeTypeRepository.Single(type => type.Id == chargeCodeTypeId);
      
      chargeCodeTypeData.IsActive = !(chargeCodeTypeData.IsActive);
      chargeCodeTypeData.LastUpdatedBy = userId;
      ChargeCodeTypeRepository.Update(chargeCodeTypeData);
      UnitOfWork.CommitDefault();
    }


    /// <summary>
    /// This function is used for get charge code type details based on id.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public ChargeCodeType GetChargeCodeTypeDetails(int Id)
    {
      var chargeCodeType = ChargeCodeTypeRepository.Single(type => type.Id == Id);
      return chargeCodeType;
    }
  }
}
