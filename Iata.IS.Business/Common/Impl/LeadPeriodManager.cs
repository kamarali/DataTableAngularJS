using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;

namespace Iata.IS.Business.Common.Impl
{
    public class LeadPeriodManager:ILeadPeriodManager
    {
        /// <summary>
        /// Gets or sets the lead period repository.
        /// </summary>
        /// <value>
        /// The lead period repository.
        /// </value>
        public ILeadPeriodRepository LeadPeriodRepository { get; set; }

        /// <summary>
        /// Adds the Lead Period.
        /// </summary>
        /// <param name="leadPeriod">The lead period.</param>
        /// <returns></returns>
        public LeadPeriod AddLeadPeriod(LeadPeriod leadPeriod)
        {
            // User should not be able to enter/save duplicate active records for a combination of Effective From Period, Effective To Period, Transaction Type ID and Settlement Method.
            if (IsleadPeriodRecordDuplicate(leadPeriod.Period, leadPeriod.ClearingHouse, leadPeriod.BillingCategoryId, leadPeriod.EffectiveFromPeriod, leadPeriod.EffectiveToPeriod))
            {
                throw new ISBusinessException(Messages.DuplicateLeadPeriodRecord);
            }

            LeadPeriodRepository.Add(leadPeriod);
            UnitOfWork.CommitDefault();
            return leadPeriod;
        }

        /// <summary>
        /// Updates the lead period.
        /// </summary>
        /// <param name="leadPeriod">The lead period.</param>
        /// <returns></returns>
        public LeadPeriod UpdateLeadPeriod(LeadPeriod leadPeriod)
        {
            var updatedLeadPeriod = LeadPeriodRepository.Single(lead => lead.Id == leadPeriod.Id);
            if (updatedLeadPeriod != null && CompareUtil.IsDirty(leadPeriod.ClearingHouse, updatedLeadPeriod.ClearingHouse) 
                || CompareUtil.IsDirty(leadPeriod.BillingCategoryId, updatedLeadPeriod.BillingCategoryId) 
                || CompareUtil.IsDirty(leadPeriod.EffectiveFromPeriod, updatedLeadPeriod.EffectiveFromPeriod) 
                || CompareUtil.IsDirty(leadPeriod.EffectiveToPeriod, updatedLeadPeriod.EffectiveToPeriod))
            {
                // User should not be able to enter/save duplicate active records for a combination of Effective From Period, Effective To Period, Transaction Type ID and Settlement Method.
                if (IsleadPeriodRecordDuplicate(leadPeriod.Period, leadPeriod.ClearingHouse, leadPeriod.BillingCategoryId, leadPeriod.EffectiveFromPeriod, leadPeriod.EffectiveToPeriod))
                {
                    throw new ISBusinessException(Messages.DuplicateLeadPeriodRecord);
                }
            }

            updatedLeadPeriod = LeadPeriodRepository.Update(leadPeriod);
            UnitOfWork.CommitDefault();

            return updatedLeadPeriod;
        }

        /// <summary>
        /// Deletes the lead period.
        /// </summary>
        /// <param name="leadPeriodId">The lead period id.</param>
        /// <returns></returns>
        public bool DeleteLeadPeriod(int leadPeriodId)
        {
            bool delete = false;
            var leadPeriodData = LeadPeriodRepository.Single(lead => lead.Id == leadPeriodId);
            if (leadPeriodData != null)
            {
                leadPeriodData.IsActive = !(leadPeriodData.IsActive);
                var updatedLeadPeriod = LeadPeriodRepository.Update(leadPeriodData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the lead period details.
        /// </summary>
        /// <param name="leadPeriodId">The lead period id.</param>
        /// <returns></returns>
        public LeadPeriod GetLeadPeriodDetails(int leadPeriodId)
        {
            var leadPeriod = LeadPeriodRepository.Single(lead => lead.Id == leadPeriodId);
            return leadPeriod;
        }

        /// <summary>
        /// Gets all lead period list.
        /// </summary>
        /// <returns></returns>
        public List<LeadPeriod> GetAllLeadPeriodList()
        {
            var leadPeriodList = LeadPeriodRepository.GetAllLeadPeriods();

            return leadPeriodList.ToList();
        }

        /// <summary>
        /// Gets the lead period list.
        /// </summary>
        /// <param name="period">The lead period.</param>
        /// <param name="clearingHouse">The clearing house.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="effectiveFromPeriod">The Effective From Period.</param>
        /// <param name="effectiveToPeriod">The Effective To Period.</param>
        /// <returns></returns>
        public List<LeadPeriod> GetLeadPeriodList(int period, string clearingHouse, int billingCategoryId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod)
        {
            var dateTime = new DateTime(1, 1, 1);
            var leadPeriodList = LeadPeriodRepository.GetAllLeadPeriods().ToList();
            
            if (effectiveFromPeriod > dateTime) // not the default datetime
            {
                leadPeriodList = leadPeriodList.Where(cl => ((cl.EffectiveFromPeriod == effectiveFromPeriod)||
                                                            (cl.EffectiveFromPeriod > effectiveFromPeriod))).ToList();
            }

            if (effectiveToPeriod > dateTime) // not the default datetime
            {
                leadPeriodList = leadPeriodList.Where(cl => ((cl.EffectiveToPeriod == effectiveToPeriod) || (cl.EffectiveToPeriod < effectiveToPeriod))).ToList();
            }

            if (period > 0)
            {
                leadPeriodList = leadPeriodList.Where(cl => (cl.Period == period)).ToList();
            }

            if (!string.IsNullOrEmpty(clearingHouse))
            {
                leadPeriodList = leadPeriodList.Where(cl => cl.ClearingHouse == clearingHouse).ToList();
            }
            if (billingCategoryId > 0)
            {
                leadPeriodList = leadPeriodList.Where(cl => (cl.BillingCategoryId == billingCategoryId)).ToList();
            }
            
            return leadPeriodList.ToList();
        }
                    
        /// <summary>
        /// check if lead is already exist or not?.
        /// </summary>
        /// <returns></returns>
        private bool IsleadPeriodRecordDuplicate(int period, string clearingHouse, int billingCategoryId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod)
        {

            var leadPeriod =
                LeadPeriodRepository.First(
                    lead =>
                    lead.Period == period && lead.ClearingHouse == clearingHouse &&
                    lead.BillingCategoryId == billingCategoryId &&
                    lead.EffectiveFromPeriod == effectiveFromPeriod && lead.EffectiveToPeriod == effectiveToPeriod);
            if (leadPeriod == null)
                return false;
            return true;
        }
    }
}
