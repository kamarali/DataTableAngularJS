using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common.Impl
{
  class SettlementMethodManager : ISettlementMethodManager
  {
    public IRepository<SettlementMethod> settlementMethodRepository { get; set; }

    /// <summary>
    /// Adds the settlement method
    /// </summary>
    /// <param name="settlementMethod">The settlement method.</param>
    /// <returns></returns>
    public SettlementMethod AddSettlementMethod(SettlementMethod settlementMethod)
    {
      var settlementCount = settlementMethodRepository.GetCount(type => (settlementMethod.Name == type.Name));

      // Settlementmethod record already exists
      if (settlementCount > 0)
      {
        throw new ISBusinessException(ErrorCodes.SettlementMethodAlreadyExists);
      }

      var settlementMethodCount = settlementMethodRepository.GetCount(type => (settlementMethod.Description.ToLower() == type.Description.ToLower()));
      if(settlementMethodCount > 0)
      {
        throw new ISBusinessException(ErrorCodes.SettlementMethodDuplicateDescription);
      }

      settlementMethodRepository.Add(settlementMethod);
      UnitOfWork.CommitDefault();
      return settlementMethod;
    }

    /// <summary>
    /// Updates the settlement methodd.
    /// </summary>
    /// <param name="settlementMethod">The settlement method.</param>
    /// <returns></returns>
    public SettlementMethod UpdateSettlementMethod(SettlementMethod settlementMethod)
    {
      var settlementCount = settlementMethodRepository.GetCount(type => (settlementMethod.Id != type.Id) && (settlementMethod.Name == type.Name));

      // Settlementmethod record already exists
      if (settlementCount > 0)
      {
        throw new ISBusinessException(ErrorCodes.SettlementMethodAlreadyExists);
      }

      var settlementMethodCount = settlementMethodRepository.GetCount(type => (settlementMethod.Id != type.Id && settlementMethod.Description.ToLower() == type.Description.ToLower()));
      if (settlementMethodCount > 0)
      {
        throw new ISBusinessException(ErrorCodes.SettlementMethodDuplicateDescription);
      }
      
      settlementMethodRepository.Single(type => type.Id == settlementMethod.Id);
      var updatedsettlementMethod = settlementMethodRepository.Update(settlementMethod);
      UnitOfWork.CommitDefault();
      return updatedsettlementMethod;
    }

    /// <summary>
    /// Deletes the settlement method.
    /// </summary>
    /// <param name="settlementMethodId">The settlement  method id.</param>
    /// <returns></returns>
    public bool DeleteSettlementMethod(int settlementMethodId)
    {
      bool delete = false;
      var settlementMethodData = settlementMethodRepository.Single(settlement => settlement.Id == settlementMethodId);
      if (settlementMethodData != null)
      {
        settlementMethodData.IsActive = !(settlementMethodData.IsActive);
        var updatedsettlementMethod = settlementMethodRepository.Update(settlementMethodData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the settlement method details.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <returns></returns>
    public SettlementMethod GetSettlementMethodDetails(int settlementMethodId)
    {
      var settlementMethod = settlementMethodRepository.Single(settlement => settlement.Id == settlementMethodId);

      return settlementMethod;
    }

    /// <summary>
    /// Gets all settlement method list.
    /// </summary>
    /// <returns></returns>
    public List<SettlementMethod> GetAllSettlementMethodList()
    {
      var settlementMethodList = settlementMethodRepository.GetAll();

      return settlementMethodList.ToList();
    }

    /// <summary>
    /// Gets the settlement method list.
    /// </summary>
    ///<param name="settlementMethodName">Name of the settlement  method</param>
    /// <returns></returns>
    public List<SettlementMethod> GetSettlementMethodList(string settlementMethodName)
    {
      var settlementMethodList = settlementMethodRepository.GetAll().ToList();
      if (settlementMethodList.Count > 0)
      {
        if (!string.IsNullOrEmpty(settlementMethodName))
          settlementMethodList =
              settlementMethodList.Where(settlement => settlement.Name == settlementMethodName).ToList();
      }

      return settlementMethodList.ToList();
    }
     public string GetSettlementMethodDescription(string settlementMethodName)
    {
         string SettlementMethodDescription = string.Empty;

         var objsmi = settlementMethodRepository.Get(smi => smi.Name == settlementMethodName).FirstOrDefault();
         if (objsmi != null) SettlementMethodDescription = objsmi.Description;
         return SettlementMethodDescription;
     }
  }
}
