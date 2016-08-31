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
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class RMReasonAcceptableDiffManager : IRMReasonAcceptableDiffManager
    {
        public IRMReasonAcceptableDiffRepository RMReasonAcceptableDiffRepository { get; set; }

        /// <summary>
        /// Adds the RM reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <returns></returns>
        public RMReasonAcceptableDiff AddRMReasonAcceptableDiff(RMReasonAcceptableDiff rmReasonAcceptableDiff)
        {
            var rmReasonAcceptableDiffData = RMReasonAcceptableDiffRepository.Single(type => type.ReasonCodeId == rmReasonAcceptableDiff.ReasonCodeId && type.EffectiveFrom == rmReasonAcceptableDiff.EffectiveFrom && type.EffectiveTo == rmReasonAcceptableDiff.EffectiveTo);
            if (rmReasonAcceptableDiffData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidPaxRMReasonAcceptableDiff);
            }
            //Call repository method for adding rmReasonAcceptableDiff
            RMReasonAcceptableDiffRepository.Add(rmReasonAcceptableDiff);
            UnitOfWork.CommitDefault();
            return rmReasonAcceptableDiff;
        }

        /// <summary>
        /// Updates the RM reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <returns></returns>
        public RMReasonAcceptableDiff UpdateRMReasonAcceptableDiff(RMReasonAcceptableDiff rmReasonAcceptableDiff)
        {
            var rmReasonAcceptableDiffData = RMReasonAcceptableDiffRepository.Single(type => type.Id != rmReasonAcceptableDiff.Id && type.ReasonCodeId == rmReasonAcceptableDiff.ReasonCodeId && type.EffectiveFrom == rmReasonAcceptableDiff.EffectiveFrom && type.EffectiveTo == rmReasonAcceptableDiff.EffectiveTo);
            if (rmReasonAcceptableDiffData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidPaxRMReasonAcceptableDiff);
            }
            rmReasonAcceptableDiffData = RMReasonAcceptableDiffRepository.Single(type => type.Id == rmReasonAcceptableDiff.Id);
            var updatedrmReasonAcceptableDiff = RMReasonAcceptableDiffRepository.Update(rmReasonAcceptableDiff);
            UnitOfWork.CommitDefault();
            return updatedrmReasonAcceptableDiff;
        }

        /// <summary>
        /// Deletes the RM reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiffId">The rm reason acceptable diff id.</param>
        /// <returns></returns>
        public bool DeleteRMReasonAcceptableDiff(int rmReasonAcceptableDiffId)
        {
            bool delete = false;
            var rmReasonAcceptableDiffData = RMReasonAcceptableDiffRepository.Single(type => type.Id == rmReasonAcceptableDiffId);
            if (rmReasonAcceptableDiffData != null)
            {
                rmReasonAcceptableDiffData.IsActive = !(rmReasonAcceptableDiffData.IsActive);
                var updatedcountry = RMReasonAcceptableDiffRepository.Update(rmReasonAcceptableDiffData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the RM reason acceptable diff details.
        /// </summary>
        /// <param name="rmReasonAcceptableDiffId">The rm reason acceptable diff id.</param>
        /// <returns></returns>
        public RMReasonAcceptableDiff GetRMReasonAcceptableDiffDetails(int rmReasonAcceptableDiffId)
        {
            var rmReasonAcceptableDiff = RMReasonAcceptableDiffRepository.GetRMReasonAcceptableDiffDetail(rmReasonAcceptableDiffId);
            return rmReasonAcceptableDiff;
        }

        /// <summary>
        /// Gets all RM reason acceptable diff list.
        /// </summary>
        /// <returns></returns>
        public List<RMReasonAcceptableDiff> GetAllRMReasonAcceptableDiffList()
        {
            var rmReasonAcceptableDiffList = RMReasonAcceptableDiffRepository.GetAllRMReasonAcceptableDiffs();
            return rmReasonAcceptableDiffList.ToList();
        }

        /// <summary>
        /// Gets the RM reason acceptable diff list.
        /// </summary>
        /// <param name="reasonCodeId"></param>
        /// <param name="effectiveFrom"></param>
        /// <param name="effectiveTo"></param>
        /// <returns></returns>
        public List<RMReasonAcceptableDiff> GetRMReasonAcceptableDiffList(int reasonCodeId,int transactionTypeId, string effectiveFrom, string effectiveTo)
        {
            var rmReasonAcceptableDiffList = new List<RMReasonAcceptableDiff>();
            rmReasonAcceptableDiffList = RMReasonAcceptableDiffRepository.GetAllRMReasonAcceptableDiffs().ToList();
            if (transactionTypeId > 0)
            {
                rmReasonAcceptableDiffList = rmReasonAcceptableDiffList.Where(cl => cl.TransactionTypeId == transactionTypeId).ToList();
            }
            if (reasonCodeId>0)
            {
                rmReasonAcceptableDiffList = rmReasonAcceptableDiffList.Where(cl => cl.ReasonCodeId==reasonCodeId).ToList();
            }
            if (!string.IsNullOrEmpty(effectiveFrom))
            {
                rmReasonAcceptableDiffList = rmReasonAcceptableDiffList.Where(cl => cl.EffectiveFrom.ToLower() == effectiveFrom.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(effectiveTo))
            {
                rmReasonAcceptableDiffList = rmReasonAcceptableDiffList.Where(cl => cl.EffectiveTo.ToLower() == effectiveTo.ToLower()).ToList();
            }
            return rmReasonAcceptableDiffList.ToList();
        }
    }
}
