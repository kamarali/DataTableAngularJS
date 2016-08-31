using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Common;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Business.Common.Impl
{
    public class MinAcceptableAmountManager : IMinAcceptableAmountManager
    {
        /// <summary>
        /// Gets or sets the min acceptable amount repository.
        /// </summary>
        /// <value>
        /// The min acceptable amount repository.
        /// </value>
        public IMinAcceptableAmountRepository MinAcceptableAmountRepository { get; set; }

        /// <summary>
        /// Gets or sets the Rejection Reson code
        /// </summary>
        public IReasonCodeRepository RejectionReasonCodeRepository { get; set; }

        /// <summary>
        /// Gets or sets the Rejection Reson code
        /// </summary>
        public IReasonCodeRepository ReasonCodeRepository { get; set; }

        /// <summary>
        /// Add minimum acceptable ammount
        /// </summary>
        /// <param name="minAcceptableAmount">Object of MinimumAcceptableAmount</param>
        /// <returns></returns>
        public MinAcceptableAmount AddMinAcceptableAmount(MinAcceptableAmount minAcceptableAmount)
        {
            var reasonCode = minAcceptableAmount.RejectionReasonCode;
             long minCount=0;
             if (minAcceptableAmount.EffectiveFromPeriod.Day > 4)
             {
                 throw new ISBusinessException(Messages.MinAccepAmtEffectiveFromPeriod);
             }
             if (minAcceptableAmount.EffectiveToPeriod.Day > 4)
             {
               throw new ISBusinessException(Messages.MinAccepAmtEffectiveToPeriod);
             }
             if ((minAcceptableAmount.IsActive) && (!string.IsNullOrEmpty(reasonCode)))
            {
                 minCount = MinAcceptableAmountRepository.GetCount(
                type => ((minAcceptableAmount.EffectiveFromPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveFromPeriod <= type.EffectiveToPeriod) || (minAcceptableAmount.EffectiveToPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveToPeriod <= type.EffectiveToPeriod)) &&
                         (type.TransactionTypeId == minAcceptableAmount.TransactionTypeId) &&
                         (type.RejectionReasonCode == minAcceptableAmount.RejectionReasonCode) &&
                         (type.ApplicableMinimumFieldId == minAcceptableAmount.ApplicableMinimumFieldId) &&
                          (type.ClearingHouse == minAcceptableAmount.ClearingHouse));

            }
            else
            {
                 minCount = MinAcceptableAmountRepository.GetCount(
                type => ((minAcceptableAmount.EffectiveFromPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveFromPeriod <= type.EffectiveToPeriod) || (minAcceptableAmount.EffectiveToPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveToPeriod <= type.EffectiveToPeriod)) &&
                         (type.TransactionTypeId == minAcceptableAmount.TransactionTypeId) &&
                         (type.ApplicableMinimumFieldId == minAcceptableAmount.ApplicableMinimumFieldId) &&
                          (type.ClearingHouse == minAcceptableAmount.ClearingHouse));
            }

            
            
            //If MinAcceptableAmount Code already exists, throw exception
            if (minCount >0)
            {

                throw new ISBusinessException(ErrorCodes.MinAcceptableAmountAlreadyExists);
            }

            //Call repository method for adding minMinAcceptableAmount
            MinAcceptableAmountRepository.Add(minAcceptableAmount);

            UnitOfWork.CommitDefault();
            return minAcceptableAmount;
        }

        /// <summary>
        /// Update minimum acceptable ammount
        /// </summary>
        /// <param name="minAcceptableAmount">Object of MinimumAcceptableAmount</param>
        /// <returns></returns>
        public MinAcceptableAmount UpdateMinAcceptableAmount(MinAcceptableAmount minAcceptableAmount)
        {

            var reasonCode = minAcceptableAmount.RejectionReasonCode;
            long minCount = 0;
            if(minAcceptableAmount.EffectiveFromPeriod.Day > 4)
            {
                throw new ISBusinessException(Messages.MinAccepAmtEffectiveFromPeriod);
            }
            if (minAcceptableAmount.EffectiveToPeriod.Day > 4)
            {
              throw new ISBusinessException(Messages.MinAccepAmtEffectiveToPeriod);
            }
            if ((minAcceptableAmount.IsActive))
            {
                if (!string.IsNullOrEmpty(reasonCode))
                {
                    minCount = MinAcceptableAmountRepository.GetCount(
                        type => ((type.Id!=minAcceptableAmount.Id) &&
                                 ((minAcceptableAmount.EffectiveFromPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveFromPeriod <= type.EffectiveToPeriod) || (minAcceptableAmount.EffectiveToPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveToPeriod <= type.EffectiveToPeriod)) &&
                                 (type.TransactionTypeId == minAcceptableAmount.TransactionTypeId) &&
                                 (type.RejectionReasonCode == minAcceptableAmount.RejectionReasonCode) &&
                                 (type.ApplicableMinimumFieldId == minAcceptableAmount.ApplicableMinimumFieldId) &&
                                 (type.ClearingHouse == minAcceptableAmount.ClearingHouse)));

                }
                else
                {
                    minCount = MinAcceptableAmountRepository.GetCount(
                        type => ((type.Id != minAcceptableAmount.Id) &&
                                 ((minAcceptableAmount.EffectiveFromPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveFromPeriod <= type.EffectiveToPeriod) || (minAcceptableAmount.EffectiveToPeriod >= type.EffectiveFromPeriod && minAcceptableAmount.EffectiveToPeriod <= type.EffectiveToPeriod)) &&
                                 (type.TransactionTypeId == minAcceptableAmount.TransactionTypeId) &&
                                 (type.ApplicableMinimumFieldId == minAcceptableAmount.ApplicableMinimumFieldId) &&
                                 (type.ClearingHouse == minAcceptableAmount.ClearingHouse)));
                }
            }

            //If MinAcceptableAmount Code already exists, throw exception
            if (minCount > 1)
            {

                throw new ISBusinessException(ErrorCodes.MinAcceptableAmountAlreadyExists);
            }
           var minAcceptableAmountData = MinAcceptableAmountRepository.Single(type => type.Id == minAcceptableAmount.Id);
            var updatedminMinAcceptableAmount = MinAcceptableAmountRepository.Update(minAcceptableAmount);
            UnitOfWork.CommitDefault();
            return updatedminMinAcceptableAmount;
        }

        /// <summary>
        /// Delete minimum acceptable amount
        /// </summary>
        /// <param name="minAcceptableAmountId">The min acceptable amount Id</param>
        /// <returns></returns>
        public bool DeleteMinAcceptableAmount(int minAcceptableAmountId)
        {
            bool delete = false;
            var minAcceptableAmountData = MinAcceptableAmountRepository.Single(type => type.Id == minAcceptableAmountId);
            if (minAcceptableAmountData != null)
            {
                minAcceptableAmountData.IsActive = !(minAcceptableAmountData.IsActive);
                var updatedMinAcceptableAmount = MinAcceptableAmountRepository.Update(minAcceptableAmountData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets all min acceptable amount list.
        /// </summary>
        /// <returns></returns>
        public List<MinAcceptableAmount> GetAllMinAcceptableAmountList()
        {
            var minAcceptableAmountList = MinAcceptableAmountRepository.GetAllMinAcceptableAmounts();

            return minAcceptableAmountList.ToList();
        }

        /// <summary>
        /// Gets the min acceptable amount details.
        /// </summary>
        /// <param name="minAcceptableAmountId">The min acceptable amount id.</param>
        /// <returns></returns>
        public MinAcceptableAmount GetMinAcceptableAmountDetails(int minAcceptableAmountId)
        {
            var minAcceptableAmount = MinAcceptableAmountRepository.Single(type => type.Id == minAcceptableAmountId);
            return minAcceptableAmount;
        }
        /// <summary>
        /// Gets the min acceptable amount list.
        /// </summary>
        /// <param name="applicableMinFieldId">Applicable Amount Field</param>
        /// <param name="effectiveFromPeriod">Effective From Period</param>
        /// <param name="effectiveToPeriod">Effective To Period</param>
        /// <param name="reasonCode">Rejection Reasoncode code</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="amount">The minimum amount.</param>
        /// <returns></returns>
        public List<MinAcceptableAmount> GetMinAcceptableAmountList(DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int applicableMinFieldId, string reasonCode, int transactionTypeId, string clearinghouse, double amount)
        {
            var minAcceptableAmountList = MinAcceptableAmountRepository.GetAllMinAcceptableAmounts().ToList();
            var dateTime = new DateTime(1, 1, 1);

            // Changed the logic for Effective From Period and Effective To Period

            if (effectiveFromPeriod > dateTime || effectiveToPeriod > dateTime)
            {
                minAcceptableAmountList =
                    minAcceptableAmountList.Where(
                        cl =>
                        ((cl.EffectiveFromPeriod <= effectiveFromPeriod) && (cl.EffectiveToPeriod >= effectiveFromPeriod))
                        ||
                        ((cl.EffectiveFromPeriod <= effectiveToPeriod) && (cl.EffectiveToPeriod >= effectiveToPeriod))
                        ||
                        ((cl.EffectiveFromPeriod <= effectiveToPeriod) && (cl.EffectiveToPeriod >= effectiveFromPeriod))).ToList();
            }

            if (transactionTypeId > 0)
            {
                minAcceptableAmountList =
                    minAcceptableAmountList.Where(c => c.TransactionTypeId == transactionTypeId).ToList();
            }
            if (applicableMinFieldId > 0)
            {
                minAcceptableAmountList =
                    minAcceptableAmountList.Where(c => c.ApplicableMinimumFieldId == applicableMinFieldId).ToList();
            }
            if (reasonCode != null)
            {
                minAcceptableAmountList = minAcceptableAmountList.Where(c => c.RejectionReasonCode == reasonCode).ToList();
            }
            //if (reasonCodeId > 0)
            //{
            //    minAcceptableAmountList =
            //        minAcceptableAmountList.Where(c => c.RejectionReasonCodeId == reasonCodeId).ToList();
            //}
            if (!string.IsNullOrEmpty(clearinghouse))
            {
                minAcceptableAmountList =
                    minAcceptableAmountList.Where(
                        cl =>
                        cl.ClearingHouse != null && cl.ClearingHouse.ToLower().Contains(clearinghouse.ToLower())).
                        ToList();
            }
            if (amount > 0)
            {
                minAcceptableAmountList = minAcceptableAmountList.Where(c => c.Amount == amount).ToList();
            }

            //var reasonCodes = minAcceptableAmountList.Select(minRecord => minRecord.RejectionReasonCode != null ? minRecord.RejectionReasonCode.ToUpper() : string.Empty);


            //var reasonCodesfromDb = ReasonCodeRepository.Get(code => reasonCodes.Contains(code.Code.ToUpper())).ToList();

            //if (reasonCodesfromDb.Count() > 0)
            //{
            //    foreach (var minAcceptableAmountRecord in minAcceptableAmountList)
            //    {
            //        if (minAcceptableAmountRecord.TransactionTypeId > 0)
            //        {
            //            var record = minAcceptableAmountRecord;
            //            var rejectionReasonCode = reasonCodesfromDb.FirstOrDefault(
            //                    rCode =>
            //                    rCode.Code == record.RejectionReasonCode && rCode.TransactionTypeId == minAcceptableAmountRecord.TransactionTypeId);
            //            if (rejectionReasonCode != null)
            //                minAcceptableAmountRecord.RejectionReasonCode = rejectionReasonCode.Description;
            //        }
            //    }
            //}
            return minAcceptableAmountList.ToList();
        }

        public List<ReasonCode> GetRejectionReasonCodeList(int transactionTypeId)
        {
            List<ReasonCode> rejectionReasonCodeList=null;
           if(transactionTypeId>0)
           {
               rejectionReasonCodeList = RejectionReasonCodeRepository.GetAllReasonCodes().ToList();
               rejectionReasonCodeList = rejectionReasonCodeList.Where(reasonCode =>reasonCode.TransactionTypeId==transactionTypeId).ToList();
           }
            return rejectionReasonCodeList;
        }
    }
}
