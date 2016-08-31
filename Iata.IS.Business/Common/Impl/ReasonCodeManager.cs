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
    public class ReasonCodeManager : IReasonCodeManager
    {
        /// <summary>
        /// Gets or sets the reason code repository.
        /// </summary>
        /// <value>
        /// The reason code repository.
        /// </value>
        public IReasonCodeRepository ReasonCodeRepository { get; set; }

        /// <summary>
        /// Adds the reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <returns></returns>
        public ReasonCode AddReasonCode(ReasonCode reasonCode)
        {
            var reasonCodeData = ReasonCodeRepository.Single(type => type.Code == reasonCode.Code && type.TransactionTypeId==reasonCode.TransactionTypeId);
            //If Currency Code already exists, throw exception
            if (reasonCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidReasonCodeTransactionType);
            }
            //Call repository method for adding reasonCode
            ReasonCodeRepository.Add(reasonCode);
            UnitOfWork.CommitDefault();
            return reasonCode;
        }

        /// <summary>
        /// Updates the reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <returns></returns>
        public ReasonCode UpdateReasonCode(ReasonCode reasonCode)
        {
            var reasonCodeData = ReasonCodeRepository.Single(type => type.Id != reasonCode.Id && type.Code == reasonCode.Code && type.TransactionTypeId == reasonCode.TransactionTypeId);
            //If Currency Code already exists, throw exception
            if (reasonCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidReasonCodeTransactionType);
            }
            reasonCodeData = ReasonCodeRepository.Single(type => type.Id == reasonCode.Id);
            var updatedreasonCode = ReasonCodeRepository.Update(reasonCode);
            UnitOfWork.CommitDefault();
            return updatedreasonCode;
        }

        /// <summary>
        /// Deletes the reason code.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <returns></returns>
        public bool DeleteReasonCode(int reasonCodeId)
        {
            bool delete = false;
            var reasonCodeData = ReasonCodeRepository.Single(type => type.Id == reasonCodeId);
            if (reasonCodeData != null)
            {
                reasonCodeData.IsActive = !(reasonCodeData.IsActive);
                var updatedreasonCode = ReasonCodeRepository.Update(reasonCodeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the reason code details.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <returns></returns>
        public ReasonCode GetReasonCodeDetails(int reasonCodeId)
        {
            var reasonCode = ReasonCodeRepository.Single(type => type.Id == reasonCodeId);
            return reasonCode;
        }

        /// <summary>
        /// Gets all reason code list.
        /// </summary>
        /// <returns></returns>
        public List<ReasonCode> GetAllReasonCodeList()
        {
            var reasonCodeList = ReasonCodeRepository.GetAllReasonCodes();
            return reasonCodeList.ToList();
        }

        /// <summary>
        /// Gets the reason code list.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <returns></returns>
        public List<ReasonCode> GetReasonCodeList(string Code, int transactionTypeId)
        {
            var reasonCodeList = new List<ReasonCode>();
            reasonCodeList = ReasonCodeRepository.GetAllReasonCodes().ToList();
            //reasonCodeList = ReasonCodeRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Code))
            {
                reasonCodeList = reasonCodeList.Where(cl => cl.Code.ToLower().Contains(Code.ToLower())).ToList();
            }
            if (transactionTypeId>0)
            {
                reasonCodeList = reasonCodeList.Where(cl => cl.TransactionTypeId==transactionTypeId).ToList();
            }
            return reasonCodeList.ToList();
        }

        /// <summary>
        /// Gets rejection reason code list.
        /// </summary>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <returns></returns>
        public List<ReasonCode> GetRejectionReasonCodeList(int transactionTypeId)
        {
            var reasonCodeList = new List<ReasonCode>();
            reasonCodeList = ReasonCodeRepository.GetAllReasonCodes().ToList();
        
            //fix bug no 8351 on tfs
            // if (transactionTypeId > 0) 
            // {
                reasonCodeList = reasonCodeList.Where(cl => cl.TransactionTypeId == transactionTypeId).ToList();
            // }

            return reasonCodeList.ToList();
        }
    }
}
