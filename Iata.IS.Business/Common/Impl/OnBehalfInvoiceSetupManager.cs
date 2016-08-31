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
    public class OnBehalfInvoiceSetupManager : IOnBehalfInvoiceSetupManager
    {
        /// <summary>
        /// Gets or sets the on behalf invoice setup repository.
        /// </summary>
        /// <value>
        /// The on behalf invoice setup repository.
        /// </value>
        public IOnBehalfInvoiceSetupRepository OnBehalfInvoiceSetupRepository { get; set; }

        /// <summary>
        /// Adds the on behalf invoice setup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetup">The on behalf invoice setup.</param>
        /// <returns></returns>
        public OnBehalfInvoiceSetup AddOnBehalfInvoiceSetup(OnBehalfInvoiceSetup onBehalfInvoiceSetup)
        {
            var onBehalfInvoiceSetupData = OnBehalfInvoiceSetupRepository.Single(type => type.BillingCategoryId == onBehalfInvoiceSetup.BillingCategoryId && type.TransmitterCode == onBehalfInvoiceSetup.TransmitterCode && type.ChargeCategoryId == onBehalfInvoiceSetup.ChargeCategoryId && type.ChargeCodeId == onBehalfInvoiceSetup.ChargeCodeId);
            //If OnBehalfInvoiceSetup Code already exists, throw exception
            if (onBehalfInvoiceSetupData == null)
            {
                //Call repository method for adding onBehalfInvoiceSetup
                OnBehalfInvoiceSetupRepository.Add(onBehalfInvoiceSetup);
                UnitOfWork.CommitDefault();
            }
            else
            {
                throw new ISBusinessException(ErrorCodes.DuplicateTransmitterExceptionRecordFound);
            }
            return onBehalfInvoiceSetup;
        }

        /// <summary>
        /// Updates the on behalf invoice setup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetup">The on behalf invoice setup.</param>
        /// <returns></returns>
        public OnBehalfInvoiceSetup UpdateOnBehalfInvoiceSetup(OnBehalfInvoiceSetup onBehalfInvoiceSetup)
        {
            var onBehalfInvoiceSetupCheckData = OnBehalfInvoiceSetupRepository.Single(type => type.BillingCategoryId == onBehalfInvoiceSetup.BillingCategoryId && type.TransmitterCode == onBehalfInvoiceSetup.TransmitterCode && type.ChargeCategoryId == onBehalfInvoiceSetup.ChargeCategoryId && type.ChargeCodeId == onBehalfInvoiceSetup.ChargeCodeId && type.Id != onBehalfInvoiceSetup.Id);
            if (onBehalfInvoiceSetupCheckData != null) 
            {
                throw new ISBusinessException(ErrorCodes.DuplicateTransmitterExceptionRecordFound);
            }
            else
            {
                var onBehalfInvoiceSetupData = OnBehalfInvoiceSetupRepository.Single(type => type.Id == onBehalfInvoiceSetup.Id);
                var updatedonBehalfInvoiceSetup = OnBehalfInvoiceSetupRepository.Update(onBehalfInvoiceSetup);
                UnitOfWork.CommitDefault();
                return updatedonBehalfInvoiceSetup;
            }
        }

        /// <summary>
        /// Deletes the on behalf invoice setup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetupId">The on behalf invoice setup id.</param>
        /// <returns></returns>
        public bool DeleteOnBehalfInvoiceSetup(int onBehalfInvoiceSetupId)
        {
            bool delete = false;
            var onBehalfInvoiceSetupData = OnBehalfInvoiceSetupRepository.Single(type => type.Id == onBehalfInvoiceSetupId);
            if (onBehalfInvoiceSetupData != null)
            {
                onBehalfInvoiceSetupData.IsActive = !(onBehalfInvoiceSetupData.IsActive);
                var updatedcountry = OnBehalfInvoiceSetupRepository.Update(onBehalfInvoiceSetupData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the on behalf invoice setup details.
        /// </summary>
        /// <param name="onBehalfInvoiceSetupId">The on behalf invoice setup id.</param>
        /// <returns></returns>
        public OnBehalfInvoiceSetup GetOnBehalfInvoiceSetupDetails(int onBehalfInvoiceSetupId)
        {
            var onBehalfInvoiceSetup = OnBehalfInvoiceSetupRepository.Single(type => type.Id == onBehalfInvoiceSetupId);
            return onBehalfInvoiceSetup;
        }

        /// <summary>
        /// Gets all on behalf invoice setup list.
        /// </summary>
        /// <returns></returns>
        public List<OnBehalfInvoiceSetup> GetAllOnBehalfInvoiceSetupList()
        {
            var onBehalfInvoiceSetupList = OnBehalfInvoiceSetupRepository.GetAllOnBehalfInvoiceSetups();
            return onBehalfInvoiceSetupList.ToList();
        }

        /// <summary>
        /// Gets the on behalf invoice setup list.
        /// </summary>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="transmitterCode">The transmitter code.</param>
        /// <param name="ChargeCategoryId">The charge category id.</param>
        /// <param name="chargeCodeId">The charge code id.</param>
        /// <returns></returns>
        public List<OnBehalfInvoiceSetup> GetOnBehalfInvoiceSetupList(int billingCategoryId, string transmitterCode, int chargeCategoryId, int chargeCodeId)
        {
            var onBehalfInvoiceSetupList = new List<OnBehalfInvoiceSetup>();
            onBehalfInvoiceSetupList = OnBehalfInvoiceSetupRepository.GetAllOnBehalfInvoiceSetups().ToList();
            if (billingCategoryId > 0)
            {
                onBehalfInvoiceSetupList = onBehalfInvoiceSetupList.Where(cl => cl.BillingCategoryId == billingCategoryId).ToList();
            }
            if (!string.IsNullOrEmpty(transmitterCode))
            {
                onBehalfInvoiceSetupList = onBehalfInvoiceSetupList.Where(cl => cl.TransmitterCode.ToLower().Contains(transmitterCode.ToLower())).ToList();
            }
            if (chargeCategoryId > 0)
            {
                onBehalfInvoiceSetupList = onBehalfInvoiceSetupList.Where(cl => cl.ChargeCategoryId == chargeCategoryId).ToList();
            }
            if (chargeCodeId > 0)
            {
                onBehalfInvoiceSetupList = onBehalfInvoiceSetupList.Where(cl => cl.ChargeCodeId == chargeCodeId).ToList();
            }
            return onBehalfInvoiceSetupList.ToList();
        }

        public List<OnBehalfInvoiceSetup> GetAllOnBehalfOfMemberList()
        {
            return OnBehalfInvoiceSetupRepository.GetAll().ToList();
        }
    }
}
