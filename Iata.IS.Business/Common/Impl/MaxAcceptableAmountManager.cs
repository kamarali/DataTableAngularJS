using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common.Impl
{
    public class MaxAcceptableAmountManager : IMaxAcceptableAmountManager
    {
        /// <summary>
        /// Gets or sets the min acceptable amount repository.
        /// </summary>
        /// <value>
        /// The min acceptable amount repository.
        /// </value>
        public IMaxAcceptableAmountRepository MaxAcceptableAmountRepository { get; set; }

        /// <summary>
        /// Add minimum acceptable ammount
        /// </summary>
        /// <param name="maxAcceptableAmount">Object of MaxAcceptableAmount</param>
        /// <returns></returns>
        public MaxAcceptableAmount AddMaxAcceptableAmount(MaxAcceptableAmount maxAcceptableAmount)
        {
          if (maxAcceptableAmount.EffectiveFromPeriod.Day > 4)
          {
            throw new ISBusinessException(Messages.MaxAccepAmtEffectiveFromPeriod);
          }
          if (maxAcceptableAmount.EffectiveToPeriod.Day > 4)
          {
            throw new ISBusinessException(Messages.MaxAccepAmtEffectiveToPeriod);
          }
          var maxAcceptableAmountData = MaxAcceptableAmountRepository.GetCount(type => ((maxAcceptableAmount.EffectiveFromPeriod >= type.EffectiveFromPeriod && maxAcceptableAmount.EffectiveFromPeriod <= type.EffectiveToPeriod) || (maxAcceptableAmount.EffectiveToPeriod >= type.EffectiveFromPeriod && maxAcceptableAmount.EffectiveToPeriod <= type.EffectiveToPeriod)) && (type.TransactionTypeId == maxAcceptableAmount.TransactionTypeId) && (type.ClearingHouse == maxAcceptableAmount.ClearingHouse));

          //If MaxAcceptableAmount Code already exists, throw exception
          if (maxAcceptableAmountData > 0)
          {
            throw new ISBusinessException(ErrorCodes.MaxAcceptableAmountAlreadyExists);
          }

          //Call repository method for adding minMaxAcceptableAmount
          MaxAcceptableAmountRepository.Add(maxAcceptableAmount);

          UnitOfWork.CommitDefault();
          return maxAcceptableAmount;
        }

        /// <summary>
        /// Update maximum acceptable ammount
        /// </summary>
        /// <param name="maxAcceptableAmount">Object of MinimumAcceptableAmount</param>
        /// <returns></returns>
        public MaxAcceptableAmount UpdateMaxAcceptableAmount(MaxAcceptableAmount maxAcceptableAmount)
        {
          if (maxAcceptableAmount.EffectiveFromPeriod.Day > 4)
          {
            throw new ISBusinessException(Messages.MaxAccepAmtEffectiveFromPeriod);
          }
          if (maxAcceptableAmount.EffectiveToPeriod.Day > 4)
          {
            throw new ISBusinessException(Messages.MaxAccepAmtEffectiveToPeriod);
          }

          var maxAcceptableAmountData = MaxAcceptableAmountRepository.Single(
                                                                              type => type.Id != maxAcceptableAmount.Id &&
                                                                              ((maxAcceptableAmount.EffectiveFromPeriod >= type.EffectiveFromPeriod && maxAcceptableAmount.EffectiveFromPeriod <= type.EffectiveToPeriod) || (maxAcceptableAmount.EffectiveToPeriod >= type.EffectiveFromPeriod && maxAcceptableAmount.EffectiveToPeriod <= type.EffectiveToPeriod)) &&
                                                                              type.TransactionTypeId == maxAcceptableAmount.TransactionTypeId &&
                                                                              type.ClearingHouse == maxAcceptableAmount.ClearingHouse
                                                                              );

          //If MaxAcceptableAmount Code already exists, throw exception
          if (maxAcceptableAmountData != null)
          {
            throw new ISBusinessException(ErrorCodes.MaxAcceptableAmountAlreadyExists);
          }
          maxAcceptableAmountData = MaxAcceptableAmountRepository.Single(type => type.Id == maxAcceptableAmount.Id);
          var updatedminMaxAcceptableAmount = MaxAcceptableAmountRepository.Update(maxAcceptableAmount);
          UnitOfWork.CommitDefault();
          return updatedminMaxAcceptableAmount;
        }

        /// <summary>
        /// Delete maximum acceptable amount
        /// </summary>
        /// <param name="maxAcceptableAmountId">The max acceptable amount Id</param>
        /// <returns></returns>
        public bool DeleteMaxAcceptableAmount(int maxAcceptableAmountId)
        {
            bool delete = false;
            var maxAcceptableAmountData = MaxAcceptableAmountRepository.Single(type => type.Id == maxAcceptableAmountId);
            if (maxAcceptableAmountData != null)
            {
                maxAcceptableAmountData.IsActive = !(maxAcceptableAmountData.IsActive);
                var updatedMaxAcceptableAmount = MaxAcceptableAmountRepository.Update(maxAcceptableAmountData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets all max acceptable amount list.
        /// </summary>
        /// <returns></returns>
        public List<MaxAcceptableAmount> GetAllMaxAcceptableAmountList()
        {
            var maxAcceptableAmountList = MaxAcceptableAmountRepository.GetAllMaxAcceptableAmounts();

            return maxAcceptableAmountList.ToList();
        }

        /// <summary>
        /// Gets the max acceptable amount details.
        /// </summary>
        /// <param name="maxAcceptableAmountId">The max acceptable amount id.</param>
        /// <returns></returns>
        public MaxAcceptableAmount GetMaxAcceptableAmountDetails(int maxAcceptableAmountId)
        {
            var maxAcceptableAmount = MaxAcceptableAmountRepository.Single(type => type.Id == maxAcceptableAmountId);
            return maxAcceptableAmount;
        }

        /// <summary>
        /// Gets the max acceptable amount list.
        /// </summary>
        /// <param name="effectiveFromPeriod">Effective From Period</param>
        /// <param name="effectiveToPeriod">Effective To Period</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="max">The min.</param>
        /// <returns></returns>
        public List<MaxAcceptableAmount> GetMaxAcceptableAmountList(DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int transactionTypeId, string clearinghouse, double max)
        {
            //var maxAcceptableAmountList = new List<MaxAcceptableAmount>();
           var maxAcceptableAmountList = MaxAcceptableAmountRepository.GetAllMaxAcceptableAmounts().ToList();
           var dateTime = new DateTime(1, 1, 1);

            // Changed logic for Effective From Period and Effective To Period

           if (effectiveFromPeriod > dateTime || effectiveToPeriod > dateTime)
           {
               maxAcceptableAmountList =
                   maxAcceptableAmountList.Where(
                       cl =>
                       ((cl.EffectiveFromPeriod <= effectiveFromPeriod) && (cl.EffectiveToPeriod >= effectiveFromPeriod))
                       ||
                       ((cl.EffectiveFromPeriod <= effectiveToPeriod) && (cl.EffectiveToPeriod >= effectiveToPeriod))
                       ||
                       ((cl.EffectiveFromPeriod <= effectiveToPeriod) && (cl.EffectiveToPeriod >= effectiveFromPeriod))).ToList();
           }
            if (transactionTypeId > 0)
            {
                maxAcceptableAmountList = maxAcceptableAmountList.Where(cl => cl.TransactionTypeId == transactionTypeId).ToList();
            }
            if ((!string.IsNullOrEmpty(clearinghouse)))
            {
                maxAcceptableAmountList = maxAcceptableAmountList.Where(
                                                                        cl => cl.ClearingHouse != null && cl.ClearingHouse.ToLower().Contains(
                                                                        clearinghouse.ToLower())
                                                                        ).ToList();
            }
            if (max > 0)
            {
                maxAcceptableAmountList = maxAcceptableAmountList.Where(cl => cl.Amount == max).ToList();
            }
            return maxAcceptableAmountList.ToList();
        }
    }
}
