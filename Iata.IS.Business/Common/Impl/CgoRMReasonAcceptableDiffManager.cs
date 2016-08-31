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
    public class CgoRMReasonAcceptableDiffManager : ICgoRMReasonAcceptableDiffManager
    {
        /// <summary>
        /// Gets or sets the cgo RM reason acceptable diff repository.
        /// </summary>
        /// <value>
        /// The cgo RM reason acceptable diff repository.
        /// </value>
        public ICgoRMReasonAcceptableDiffRepository CgoRMReasonAcceptableDiffRepository { get; set; }

        /// <summary>
        /// Adds the cgo RM reason acceptable diff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiff">The cgo RM reason acceptable diff.</param>
        /// <returns></returns>
        public CgoRMReasonAcceptableDiff AddCgoRMReasonAcceptableDiff(CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff)
        {
            var cgoRMReasonAcceptableDiffData = CgoRMReasonAcceptableDiffRepository.Single(type => type.ReasonCodeId == cgoRMReasonAcceptableDiff.ReasonCodeId && type.EffectiveFrom == cgoRMReasonAcceptableDiff.EffectiveFrom && type.EffectiveTo == cgoRMReasonAcceptableDiff.EffectiveTo);
            if (cgoRMReasonAcceptableDiffData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCgoRMReasonAcceptableDiff);
            }
            //Call repository method for adding cgoRMReasonAcceptableDiff
            CgoRMReasonAcceptableDiffRepository.Add(cgoRMReasonAcceptableDiff);
            UnitOfWork.CommitDefault();
            return cgoRMReasonAcceptableDiff;
        }

        /// <summary>
        /// Updates the cgo RM reason acceptable diff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiff">The cgo RM reason acceptable diff.</param>
        /// <returns></returns>
        public CgoRMReasonAcceptableDiff UpdateCgoRMReasonAcceptableDiff(CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff)
        {
            var cgoRMReasonAcceptableDiffData = CgoRMReasonAcceptableDiffRepository.Single(type => type.Id != cgoRMReasonAcceptableDiff.Id && type.ReasonCodeId == cgoRMReasonAcceptableDiff.ReasonCodeId && type.EffectiveFrom == cgoRMReasonAcceptableDiff.EffectiveFrom && type.EffectiveTo == cgoRMReasonAcceptableDiff.EffectiveTo);
            if (cgoRMReasonAcceptableDiffData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCgoRMReasonAcceptableDiff);
            }
            cgoRMReasonAcceptableDiffData = CgoRMReasonAcceptableDiffRepository.Single(type => type.Id == cgoRMReasonAcceptableDiff.Id);
            var updatedcgoRMReasonAcceptableDiff = CgoRMReasonAcceptableDiffRepository.Update(cgoRMReasonAcceptableDiff);
            UnitOfWork.CommitDefault();
            return updatedcgoRMReasonAcceptableDiff;
        }

        /// <summary>
        /// Deletes the cgo RM reason acceptable diff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiffId">The cgo RM reason acceptable diff id.</param>
        /// <returns></returns>
        public bool DeleteCgoRMReasonAcceptableDiff(int cgoRMReasonAcceptableDiffId)
        {
            bool delete = false;
            var cgoRMReasonAcceptableDiffData = CgoRMReasonAcceptableDiffRepository.Single(type => type.Id == cgoRMReasonAcceptableDiffId);
            if (cgoRMReasonAcceptableDiffData != null)
            {
                cgoRMReasonAcceptableDiffData.IsActive = !(cgoRMReasonAcceptableDiffData.IsActive);
                var updatedcountry = CgoRMReasonAcceptableDiffRepository.Update(cgoRMReasonAcceptableDiffData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the cgo RM reason acceptable diff details.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiffId">The cgo RM reason acceptable diff id.</param>
        /// <returns></returns>
        public CgoRMReasonAcceptableDiff GetCgoRMReasonAcceptableDiffDetails(int cgoRMReasonAcceptableDiffId)
        {
            var cgoRMReasonAcceptableDiff = CgoRMReasonAcceptableDiffRepository.GetCgoRMReasonAcceptableDiffDetail(cgoRMReasonAcceptableDiffId);
            return cgoRMReasonAcceptableDiff;
        }

        /// <summary>
        /// Gets all cgo RM reason acceptable diff list.
        /// </summary>
        /// <returns></returns>
        public List<CgoRMReasonAcceptableDiff> GetAllCgoRMReasonAcceptableDiffList()
        {
            var cgoRMReasonAcceptableDiffList = CgoRMReasonAcceptableDiffRepository.GetAllCgoRMReasonAcceptableDiffs();
            return cgoRMReasonAcceptableDiffList.ToList();
        }

        /// <summary>
        /// Gets the cgo RM reason acceptable diff list.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <param name="effectiveFrom">The effective from.</param>
        /// <param name="effectiveTo">The effective to.</param>
        /// <returns></returns>
        public List<CgoRMReasonAcceptableDiff> GetCgoRMReasonAcceptableDiffList(int reasonCodeId,int TransactionTypeId, string effectiveFrom, string effectiveTo)
        {
            var cgoRMReasonAcceptableDiffList = new List<CgoRMReasonAcceptableDiff>();
            cgoRMReasonAcceptableDiffList = CgoRMReasonAcceptableDiffRepository.GetAllCgoRMReasonAcceptableDiffs().ToList();
            if (TransactionTypeId > 0)
            {
                cgoRMReasonAcceptableDiffList = cgoRMReasonAcceptableDiffList.Where(cl => cl.ReasonCode.TransactionTypeId == TransactionTypeId).ToList();
            }
            if (reasonCodeId>0)
            {
                cgoRMReasonAcceptableDiffList = cgoRMReasonAcceptableDiffList.Where(cl => cl.ReasonCodeId == reasonCodeId).ToList();
            }
            if (!string.IsNullOrEmpty(effectiveFrom))
            {
                cgoRMReasonAcceptableDiffList = cgoRMReasonAcceptableDiffList.Where(cl => cl.EffectiveFrom==effectiveFrom).ToList();
            }
            if (!string.IsNullOrEmpty(effectiveTo))
            {
                cgoRMReasonAcceptableDiffList = cgoRMReasonAcceptableDiffList.Where(cl => cl.EffectiveTo==effectiveTo).ToList();
            }
            return cgoRMReasonAcceptableDiffList.ToList();
        }
    }
}
