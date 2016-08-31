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
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;
namespace Iata.IS.Business.Common.Impl
{
  public class UomCodeManager : IUomCodeManager
  {
    /// <summary>
    /// Gets or sets the uom code repository.
    /// </summary>
    /// <value>
    /// The uom code repository.
    /// </value>
    public IRepository<UomCode> UomCodeRepository { get; set; }

    /// <summary>
    /// Adds the uom code.
    /// </summary>
    /// <param name="uomCode">The uom code.</param>
    /// <returns></returns>
    public UomCode AddUomCode(UomCode uomCode)
    {
      var uomCodeData = UomCodeRepository.Single(type => type.Id == uomCode.Id && type.Type == uomCode.Type);
      //If UomCode Code already exists, throw exception
      if (uomCodeData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidUomCode);
      }
      //Call repository method for adding uomCode
      UomCodeRepository.Add(uomCode);
      UnitOfWork.CommitDefault();
      return uomCode;
    }

    /// <summary>
    /// Updates the uom code.
    /// </summary>
    /// <param name="uomCode">The uom code.</param>
    /// <returns></returns>
    public UomCode UpdateUomCode(UomCode uomCode)
    {
      UomCodeRepository.Single(type => type.Id == uomCode.Id && uomCode.Type == type.Type);
      
      var updateduomCode = UomCodeRepository.Update(uomCode);
      UnitOfWork.CommitDefault();
      return updateduomCode;
    }

    /// <summary>
    /// Deletes the uom code.
    /// </summary>
    /// <param name="uomCodeAndType">Uom code and Type.</param>
    /// <returns></returns>
    public bool DeleteUomCode(string uomCodeAndType)
    {
      bool delete = false;
      string[] uomCodeTypeTokens = uomCodeAndType.Split(',');

      string uomCodeId = uomCodeTypeTokens[0];
      int uomCodeType = int.Parse(uomCodeTypeTokens[1]);
      var uomCodeData = UomCodeRepository.Single(type => type.Id == uomCodeId && type.Type == uomCodeType);
      if (uomCodeData != null)
      {
        uomCodeData.IsActive = !(uomCodeData.IsActive);
        UomCodeRepository.Update(uomCodeData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the uom code details.
    /// </summary>
    /// <param name="uomCodeId">The uom code id.</param>
    /// <returns></returns>
    public UomCode GetUomCodeDetails(string uomCodeAndType)
    {
      string[] uomCodeTypeTokens = uomCodeAndType.Split(',');
      string uomCodeId = uomCodeTypeTokens[0];
      int uomCodeType = int.Parse(uomCodeTypeTokens[1]);

      var uomCode = UomCodeRepository.Single(type => type.Id == uomCodeId && type.Type == uomCodeType);
      return uomCode;
    }

    /// <summary>
    /// Gets all uom code list.
    /// </summary>
    /// <returns></returns>
    public List<UomCode> GetAllUomCodeList()
    {
      var uomCodeList = UomCodeRepository.GetAll();
      return uomCodeList.ToList();
    }

    /// <summary>
    /// Gets the uom code list.
    /// </summary>
    /// <param name="uomCodeId"></param>
    /// <param name="type"></param>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public List<UomCode> GetUomCodeList(string uomCodeId, int type, string description)
    {
      var uomCodeList = UomCodeRepository.GetAll().ToList();
      if (!string.IsNullOrEmpty(uomCodeId))
      {
        uomCodeList = uomCodeList.Where(cl => cl.Id.ToLower().Contains(uomCodeId.ToLower())).ToList();
      }
      if (type > -1)
      {
        uomCodeList = uomCodeList.Where(cl => cl.Type == type).ToList();
      }
      if (!string.IsNullOrEmpty(description))
      {
        uomCodeList = uomCodeList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(description.Trim().ToLower())).ToList();
      }
      return uomCodeList.ToList();
    }
  }
}