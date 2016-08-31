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
    public class MinMaxAcceptableAmountManager : IMinMaxAcceptableAmountManager
    {
        /// <summary>
        /// Gets or sets the min max acceptable amount repository.
        /// </summary>
        /// <value>
        /// The min max acceptable amount repository.
        /// </value>
        public IMinMaxAcceptableAmountRepository MinMaxAcceptableAmountRepository { get; set; }

        /// <summary>
        /// Adds the min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <returns></returns>
        public MinMaxAcceptableAmount AddMinMaxAcceptableAmount(MinMaxAcceptableAmount minMaxAcceptableAmount)
        {
            var minMaxAcceptableAmountData = MinMaxAcceptableAmountRepository.Single(type => type.TransactionTypeId == minMaxAcceptableAmount.TransactionTypeId && type.ClearingHouse == minMaxAcceptableAmount.ClearingHouse);
            //If MinMaxAcceptableAmount Code already exists, throw exception
            if (minMaxAcceptableAmountData != null)
            {
                throw new ISBusinessException(ErrorCodes.MinMaxAcceptableAmountAlreadyExists);
            }
            //Call repository method for adding minMaxAcceptableAmount
            MinMaxAcceptableAmountRepository.Add(minMaxAcceptableAmount);
            UnitOfWork.CommitDefault();
            return minMaxAcceptableAmount;
        }

        /// <summary>
        /// Updates the min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <returns></returns>
        public MinMaxAcceptableAmount UpdateMinMaxAcceptableAmount(MinMaxAcceptableAmount minMaxAcceptableAmount)
        {
            var minMaxAcceptableAmountData = MinMaxAcceptableAmountRepository.Single(type => type.Id != minMaxAcceptableAmount.Id && type.TransactionTypeId == minMaxAcceptableAmount.TransactionTypeId && type.ClearingHouse == minMaxAcceptableAmount.ClearingHouse);
            //If MinMaxAcceptableAmount Code already exists, throw exception
            if (minMaxAcceptableAmountData != null)
            {
                throw new ISBusinessException(ErrorCodes.MinMaxAcceptableAmountAlreadyExists);
            }
            minMaxAcceptableAmountData = MinMaxAcceptableAmountRepository.Single(type => type.Id == minMaxAcceptableAmount.Id);
            var updatedminMaxAcceptableAmount = MinMaxAcceptableAmountRepository.Update(minMaxAcceptableAmount);
            UnitOfWork.CommitDefault();
            return updatedminMaxAcceptableAmount;
        }

        /// <summary>
        /// Deletes the min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmountId">The min max acceptable amount id.</param>
        /// <returns></returns>
        public bool DeleteMinMaxAcceptableAmount(int minMaxAcceptableAmountId)
        {
            bool delete = false;
            var minMaxAcceptableAmountData = MinMaxAcceptableAmountRepository.Single(type => type.Id == minMaxAcceptableAmountId);
            if (minMaxAcceptableAmountData != null)
            {
                minMaxAcceptableAmountData.IsActive = !(minMaxAcceptableAmountData.IsActive);
                var updatedminMaxAcceptableAmount = MinMaxAcceptableAmountRepository.Update(minMaxAcceptableAmountData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the min max acceptable amount details.
        /// </summary>
        /// <param name="minMaxAcceptableAmountId">The min max acceptable amount id.</param>
        /// <returns></returns>
        public MinMaxAcceptableAmount GetMinMaxAcceptableAmountDetails(int minMaxAcceptableAmountId)
        {
            var minMaxAcceptableAmount = MinMaxAcceptableAmountRepository.Single(type => type.Id == minMaxAcceptableAmountId);
            return minMaxAcceptableAmount;
        }

        /// <summary>
        /// Gets all min max acceptable amount list.
        /// </summary>
        /// <returns></returns>
        public List<MinMaxAcceptableAmount> GetAllMinMaxAcceptableAmountList()
        {
            var minMaxAcceptableAmountList = MinMaxAcceptableAmountRepository.GetAllMinMaxAcceptableAmounts();

            return minMaxAcceptableAmountList.ToList();
        }

        /// <summary>
        /// Gets the min max acceptable amount list.
        /// </summary>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        public List<MinMaxAcceptableAmount> GetMinMaxAcceptableAmountList(int transactionTypeId, string clearinghouse, double min, double max)
        {
            var minMaxAcceptableAmountList = new List<MinMaxAcceptableAmount>();
            minMaxAcceptableAmountList = MinMaxAcceptableAmountRepository.GetAllMinMaxAcceptableAmounts().ToList();

            if (transactionTypeId>0)
            {
                minMaxAcceptableAmountList = minMaxAcceptableAmountList.Where(cl => cl.TransactionTypeId == transactionTypeId ).ToList();
            }
            if ((!string.IsNullOrEmpty(clearinghouse)))
            {
                minMaxAcceptableAmountList = minMaxAcceptableAmountList.Where(cl => cl.ClearingHouse != null && cl.ClearingHouse.ToLower().Contains(clearinghouse.ToLower())).ToList();
            }
            if (min>0 && max==0)
            {
                minMaxAcceptableAmountList = minMaxAcceptableAmountList.Where(cl => cl.Min ==min).ToList();
            }
            if (max > 0 && min == 0)
            {
                minMaxAcceptableAmountList = minMaxAcceptableAmountList.Where(cl => cl.Max == max).ToList();
            }
            if (min > 0 && max > 0)
            {
                minMaxAcceptableAmountList = minMaxAcceptableAmountList.Where(cl => cl.Min >= min && cl.Max <= max).ToList();
            }

            return minMaxAcceptableAmountList.ToList();
        }
    }
}
