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
using Iata.IS.Model.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class SourceCodeManager : ISourceCodeManager
    {
        //public ISourceCodeRepository SourceCodeRepository { get; set; }
        /// <summary>
        /// Gets or sets the source code repository.
        /// </summary>
        /// <value>
        /// The source code repository.
        /// </value>
        public IRepository<SourceCode> SourceCodeRepository { get; set; }

        /// <summary>
        /// Adds the source code.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <returns></returns>
        public SourceCode AddSourceCode(SourceCode sourceCode)
        {
            var sourceCodeData = SourceCodeRepository.Single(type => type.SourceCodeIdentifier == sourceCode.SourceCodeIdentifier);
            //If Currency SourceCodeIdentifier already exists, throw exception
            if (sourceCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCurrencyAdjustmentInd);
            }
            //Call repository method for adding sourceCode
            SourceCodeRepository.Add(sourceCode);
            UnitOfWork.CommitDefault();
            return sourceCode;
        }

        /// <summary>
        /// Updates the source code.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <returns></returns>
        public SourceCode UpdateSourceCode(SourceCode sourceCode)
        {
            var sourceCodeData = SourceCodeRepository.Single(type => type.Id == sourceCode.Id);
            var updatedsourceCode = SourceCodeRepository.Update(sourceCode);
            UnitOfWork.CommitDefault();
            return updatedsourceCode;
        }

        /// <summary>
        /// Deletes the source code.
        /// </summary>
        /// <param name="sourceCodeId">The source code id.</param>
        /// <returns></returns>
        public bool DeleteSourceCode(int sourceCodeId)
        {
            bool delete = false;
            var sourceCodeData = SourceCodeRepository.Single(type => type.Id == sourceCodeId);
            if (sourceCodeData != null)
            {
                sourceCodeData.IsActive = !(sourceCodeData.IsActive);
                var updatedsourceCode = SourceCodeRepository.Update(sourceCodeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the source code details.
        /// </summary>
        /// <param name="sourceCodeId">The source code id.</param>
        /// <returns></returns>
        public SourceCode GetSourceCodeDetails(int sourceCodeId)
        {
            var sourceCode = SourceCodeRepository.Single(type => type.Id == sourceCodeId);
            return sourceCode;
        }

        /// <summary>
        /// Gets all source code list.
        /// </summary>
        /// <returns></returns>
        public List<SourceCode> GetAllSourceCodeList()
        {
            var sourceCodeList = SourceCodeRepository.GetAll();
            return sourceCodeList.ToList();
        }

        /// <summary>
        /// Gets the source code list.
        /// </summary>
        /// <param name="SourceCodeIdentifier">The source code identifier.</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="UtilizationType">Type of the utilization.</param>
        /// <returns></returns>
        public List<SourceCode> GetSourceCodeList(int SourceCodeIdentifier, int transactionTypeId, string UtilizationType)
        {
            var sourceCodeList = new List<SourceCode>();
            //sourceCodeList = SourceCodeRepository.GetAllSourceCodes().ToList();
            sourceCodeList = SourceCodeRepository.GetAll().ToList();
            if (SourceCodeIdentifier>0)
            {
                sourceCodeList = sourceCodeList.Where(cl => cl.SourceCodeIdentifier==SourceCodeIdentifier).ToList();
            }
            if (transactionTypeId > 0)
            {
                sourceCodeList = sourceCodeList.Where(cl => cl.TransactionTypeId == transactionTypeId).ToList();
            }
            if (!string.IsNullOrEmpty(UtilizationType))
            {
                sourceCodeList = sourceCodeList.Where(cl => cl.UtilizationType != null && cl.UtilizationType.ToLower().Contains(UtilizationType.ToLower())).ToList();
            }
            return sourceCodeList.ToList();
        }
    }
}
