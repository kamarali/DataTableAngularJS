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
using Iata.IS.Model.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;
using TransactionType = Iata.IS.Model.Common.TransactionType;

namespace Iata.IS.Business.Common.Impl
{
    public class TransactionTypeManager : ITransactionTypeManager
    {
        /// <summary>
        /// Gets or sets the  transactionType repository.
        /// </summary>
        /// <value>
        /// The  transactionType repository.
        /// </value>
        public IRepository<Model.Common.TransactionType> TransactionTypeRepository { get; set; }

        /// <summary>
        /// Adds the  transactionType.
        /// </summary>
        /// <param name=" transactionType">The  transactionType.</param>
        /// <returns></returns>
        public TransactionType AddTransactionType(TransactionType transactionType)
        {
            var transactionTypeData = TransactionTypeRepository.Single(type => type.Name == transactionType.Name);
            //If  TransactionType Code already exists, throw exception
            if (transactionTypeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidTransactionType);
            }
            //Call repository method for adding  transactionType
            TransactionTypeRepository.Add(transactionType);
            UnitOfWork.CommitDefault();
            return transactionType;
        }

        /// <summary>
        /// Updates the  transactionType.
        /// </summary>
        /// <param name=" transactionType">The  transactionType.</param>
        /// <returns></returns>
        public TransactionType UpdateTransactionType(TransactionType transactionType)
        {
            var transactionTypeData = TransactionTypeRepository.Single(type => type.Id != transactionType.Id && type.Name == transactionType.Name);
            //If  TransactionType Code already exists, throw exception
            if (transactionTypeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidTransactionType);
            }
            transactionTypeData = TransactionTypeRepository.Single(type => type.Id == transactionType.Id);
            var updatedtransactionType = TransactionTypeRepository.Update(transactionType);
            UnitOfWork.CommitDefault();
            return updatedtransactionType;
        }

        /// <summary>
        /// Deletes the  transactionType.
        /// </summary>
        /// <param name=" transactionTypeId">The  transactionType id.</param>
        /// <returns></returns>
        public bool DeleteTransactionType(int transactionTypeId)
        {
            bool delete = false;
            var transactionTypeData = TransactionTypeRepository.Single(type => type.Id == transactionTypeId);
            if (transactionTypeData != null)
            {
                transactionTypeData.IsActive = !(transactionTypeData.IsActive);
                var updatedcountry = TransactionTypeRepository.Update(transactionTypeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the  transactionType details.
        /// </summary>
        /// <param name=" transactionTypeId">The  transactionType id.</param>
        /// <returns></returns>
        public TransactionType GetTransactionTypeDetails(int transactionTypeId)
        {
            var transactionType = TransactionTypeRepository.Single(type => type.Id == transactionTypeId);
            return transactionType;
        }

        /// <summary>
        /// Gets all  transactionType list.
        /// </summary>
        /// <returns></returns>
        public List<TransactionType> GetAllTransactionTypeList()
        {
            var transactionTypeList = TransactionTypeRepository.GetAll();
            return transactionTypeList.ToList();
        }

        /// <summary>
        /// Gets the transaction type list.
        /// </summary>
        /// <param name="BillingCategoryCode"></param>
        /// <param name="Description"></param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        public List<TransactionType> GetTransactionTypeList(int BillingCategoryCode, string Description, string Name)
        {
            var transactionTypeList = new List<TransactionType>();
            transactionTypeList = TransactionTypeRepository.GetAll().ToList();
            if (BillingCategoryCode>0)
            {
                transactionTypeList = transactionTypeList.Where(cl => cl.BillingCategoryCode == BillingCategoryCode).ToList();
            }
            if (!string.IsNullOrEmpty(Description))
            {
                transactionTypeList = transactionTypeList.Where(cl => cl.Description != null && cl.Description.Trim().ToLower().Contains(Description.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                transactionTypeList = transactionTypeList.Where(cl => cl.Name != null && cl.Name.Trim().ToLower().Contains(Name.Trim().ToLower())).ToList();
            }
            return transactionTypeList.ToList();
        }
    }
}
