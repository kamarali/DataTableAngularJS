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
    public class SampleDigitManager : ISampleDigitManager
    {
        /// <summary>
        /// Gets or sets the sample digit repository.
        /// </summary>
        /// <value>
        /// The sample digit repository.
        /// </value>
        public IRepository<SampleDigit> SampleDigitRepository { get; set; }

        /// <summary>
        /// Adds the sample digit.
        /// </summary>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <returns></returns>
        public SampleDigit AddSampleDigit(SampleDigit sampleDigit)
        {
            if (sampleDigit.DigitAnnouncementDateTime <= DateTime.UtcNow.Date)
            {
                throw new ISBusinessException(ErrorCodes.InvalidDigitAnnouncementDateTime);
            }
            var sampleDigitData = SampleDigitRepository.Single(type => type.ProvisionalBillingMonth == sampleDigit.ProvisionalBillingMonth);
            //If SampleDigit Code already exists, throw exception
            if (sampleDigitData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidProvisionalBillingMonthAndDigitAnnouncementDateTime);
            }
            //Call repository method for adding sampleDigit
            SampleDigitRepository.Add(sampleDigit);
            UnitOfWork.CommitDefault();
            return sampleDigit;
        }

        /// <summary>
        /// Updates the sample digit.
        /// </summary>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <returns></returns>
        public SampleDigit UpdateSampleDigit(SampleDigit sampleDigit)
        {
            if (sampleDigit.DigitAnnouncementDateTime <= DateTime.UtcNow.Date)
            {
                throw new ISBusinessException(ErrorCodes.InvalidDigitAnnouncementDateTime);
            }
            var sampleDigitData = SampleDigitRepository.Single(type =>type.Id!=sampleDigit.Id && type.ProvisionalBillingMonth == sampleDigit.ProvisionalBillingMonth );
            if (sampleDigitData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidProvisionalBillingMonthAndDigitAnnouncementDateTime);
            }
            sampleDigitData = SampleDigitRepository.Single(type => type.Id == sampleDigit.Id);
            var updatedsampleDigit = SampleDigitRepository.Update(sampleDigit);
            UnitOfWork.CommitDefault();
            return updatedsampleDigit;
        }

        /// <summary>
        /// Deletes the sample digit.
        /// </summary>
        /// <param name="sampleDigitId">The sample digit id.</param>
        /// <returns></returns>
        public bool DeleteSampleDigit(int sampleDigitId)
        {
            bool delete = false;
            var sampleDigitData = SampleDigitRepository.Single(type => type.Id == sampleDigitId);
            if (sampleDigitData != null)
            {
                sampleDigitData.IsActive = !(sampleDigitData.IsActive);
                var updatedcountry = SampleDigitRepository.Update(sampleDigitData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the sample digit details.
        /// </summary>
        /// <param name="sampleDigitId">The sample digit id.</param>
        /// <returns></returns>
        public SampleDigit GetSampleDigitDetails(int sampleDigitId)
        {
            var sampleDigit = SampleDigitRepository.Single(type => type.Id == sampleDigitId);
            return sampleDigit;
        }

        /// <summary>
        /// Gets all sample digit list.
        /// </summary>
        /// <returns></returns>
        public List<SampleDigit> GetAllSampleDigitList()
        {
            var sampleDigitList = SampleDigitRepository.GetAll();
            return sampleDigitList.ToList();
        }

        /// <summary>
        /// Gets the sample digit list.
        /// </summary>
        /// <param name="billingMonth">The billing month.</param>
        /// <returns></returns>
        public List<SampleDigit> GetSampleDigitList(string billingMonth)
        {
            var sampleDigitList = new List<SampleDigit>();
            sampleDigitList = SampleDigitRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(billingMonth))
            {
                sampleDigitList = sampleDigitList.Where(cl => cl.ProvisionalBillingMonth!=null && cl.ProvisionalBillingMonth.ToLower().Contains(billingMonth.ToLower())).ToList();
            }
            return sampleDigitList.ToList();
        }
    }
}
