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
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;
using Iata.IS.Model.Master;

namespace Iata.IS.Business.Common.Impl
{
  public class ChargeCodeManager : IChargeCodeManager
  {
    /// <summary>
    /// Gets or sets the charge code repository.
    /// </summary>
    /// <value>
    /// The charge code repository.
    /// </value>
    public IChargeCodeRepository ChargeCodeRepository { get; set; }

    /// <summary>
    /// Gets or sets the charge category repository.
    /// </summary>
    /// <value>
    /// The charge category repository.
    /// </value>
    public IRepository<ChargeCategory> ChargeCategoryRepository { get; set; }

    /// <summary>
    /// Adds the charge code.
    /// </summary>
    /// <param name="chargeCode">The charge code.</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public void AddChargeCode(ChargeCode chargeCode, int userId)
    {
      var ChargeCodeData = ChargeCodeRepository.Single(type => type.Id == chargeCode.Id);

      //If same Charge Category and Charge Code already exists, throw duplicate error.
      if (ChargeCodeData.IsChargeCodeTypeRequired != null)
      {
        throw new ISBusinessException(Messages.DuplicateChargeCodeRequirement);
      }

      ChargeCodeData.IsActiveChargeCodeType = chargeCode.IsActiveChargeCodeType;
      ChargeCodeData.IsChargeCodeTypeRequired = chargeCode.IsChargeCodeTypeRequired;
      ChargeCodeData.LastUpdatedBy = userId;
      ChargeCodeRepository.Update(ChargeCodeData);
      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// Updates the charge code.
    /// </summary>
    /// <param name="chargeCode">The charge code.</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public void UpdateChargeCode(ChargeCode chargeCode, int userId)
    {
      var ChargeCodeData = ChargeCodeRepository.Single(type => type.Id == chargeCode.Id);
      ChargeCodeData.IsChargeCodeTypeRequired = chargeCode.IsChargeCodeTypeRequired;
      ChargeCodeData.IsActiveChargeCodeType = chargeCode.IsActiveChargeCodeType;
      ChargeCodeData.LastUpdatedBy = userId;
      var updatedChargeCode = ChargeCodeRepository.Update(ChargeCodeData);
      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// Deletes the charge code.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public void DeleteChargeCode(int chargeCodeId, int userId)
    {
      //Update charge code data.
      var chargeCodeData = ChargeCodeRepository.Single(type => type.Id == chargeCodeId);
      chargeCodeData.IsActiveChargeCodeType = !(chargeCodeData.IsActiveChargeCodeType);
      chargeCodeData.LastUpdatedBy = userId;
      ChargeCodeRepository.Update(chargeCodeData);

      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// Gets the charge code details.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    public ChargeCode GetChargeCodeDetails(int chargeCodeId)
    {
      var ChargeCode = ChargeCodeRepository.Single(type => type.Id == chargeCodeId);
      return ChargeCode;
    }

    /// <summary>
    /// Gets all charge code list.
    /// </summary>
    /// <returns></returns>
    public List<ChargeCode> GetAllChargeCodeList()
    {
      var ChargeCodeList = ChargeCodeRepository.GetAll();
      return ChargeCodeList.ToList();
    }

    /// <summary>
    /// Gets the charge code list.
    /// </summary>
    /// <param name="Name">The name.</param>
    /// <param name="ChargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    public List<ChargeCode> GetChargeCodeList(string Name, int ChargeCategoryId)
    {
      var ChargeCodeList = new List<ChargeCode>();
      ChargeCodeList = ChargeCodeRepository.GetAll().ToList();

      if ((!string.IsNullOrEmpty(Name)))
      {
        ChargeCodeList = ChargeCodeList.Where(cl => (cl.Name.ToLower().Contains(Name.ToLower()))).ToList();
      }
      if (ChargeCategoryId > 0)
      {
        ChargeCodeList = ChargeCodeList.Where(cl => (cl.ChargeCategoryId == ChargeCategoryId)).ToList();
      }
      return ChargeCodeList.ToList();
    }

    /// <summary>
    /// This function is used to get misc charge code based on charge category and charge code.
    /// </summary>
    /// <param name="chargeCategoryId"></param>
    /// <param name="chargeCodeId"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public List<ChargeCodeSearchData> GetMiscChargeCode(Int32 chargeCategoryId, Int32 chargeCodeId)
    {
      var filteredList = ChargeCodeRepository.GetMiscChargeCode(chargeCategoryId, chargeCodeId);
      return filteredList;
    }
  }
}
