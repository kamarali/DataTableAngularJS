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
    public class RfiscManager : IRfiscManager
    {
        public IRepository<Rfisc> RfiscRepository { get; set; }

        /// <summary>
        /// Adds the rfisc.
        /// </summary>
        /// <param name="rfisc">The rfisc.</param>
        /// <returns></returns>
        public Rfisc AddRfisc(Rfisc rfisc)
        {
            var rfiscData = RfiscRepository.Single(type => type.Id == rfisc.Id);
            //If Rfisc Code already exists, throw exception
            if (rfiscData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidRfiscCode);
            }
            //Call repository method for adding rfisc
            RfiscRepository.Add(rfisc);
            UnitOfWork.CommitDefault();
            return rfisc;
        }

        /// <summary>
        /// Updates the rfisc.
        /// </summary>
        /// <param name="rfisc">The rfisc.</param>
        /// <returns></returns>
        public Rfisc UpdateRfisc(Rfisc rfisc)
        {
            var rfiscData = RfiscRepository.Single(type => type.Id == rfisc.Id);
            //If Rfisc Code already exists, throw exception
            if (rfiscData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidRfiscCode);
            }
            var updatedrfisc = RfiscRepository.Update(rfisc);
            UnitOfWork.CommitDefault();
            return updatedrfisc;
        }

        /// <summary>
        /// Deletes the rfisc.
        /// </summary>
        /// <param name="rfiscId">The rfisc id.</param>
        /// <returns></returns>
        public bool DeleteRfisc(string rfiscId)
        {
            bool delete = false;
            var rfiscData = RfiscRepository.Single(type => type.Id == rfiscId);
            if (rfiscData != null)
            {
                rfiscData.IsActive = !(rfiscData.IsActive);
                var updatedcountry = RfiscRepository.Update(rfiscData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the rfisc details.
        /// </summary>
        /// <param name="rfiscId">The rfisc id.</param>
        /// <returns></returns>
        public Rfisc GetRfiscDetails(string rfiscId)
        {
            var rfisc = RfiscRepository.Single(type => type.Id == rfiscId);
            return rfisc;
        }

        /// <summary>
        /// Gets all rfisc list.
        /// </summary>
        /// <returns></returns>
        public List<Rfisc> GetAllRfiscList()
        {
            var rfiscList = RfiscRepository.GetAll();
            return rfiscList.ToList();
        }

        /// <summary>
        /// Gets the rfisc list.
        /// </summary>
        /// <param name="RficId"></param>
        /// <param name="GroupName"></param>
        /// <param name="CommercialName"></param>
        /// <returns></returns>
        public List<Rfisc> GetRfiscList(string rfiscId, string RficId, string GroupName, string CommercialName)
        {
            var rfiscList = new List<Rfisc>();
            rfiscList = RfiscRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(rfiscId))
            {
                rfiscList = rfiscList.Where(cl => cl.Id.ToLower().Contains(rfiscId.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(RficId))
            {
                rfiscList = rfiscList.Where(cl => cl.RficId.Contains(RficId)).ToList();
            }
            if (!string.IsNullOrEmpty(GroupName))
            {
                rfiscList = rfiscList.Where(cl =>!string.IsNullOrEmpty(cl.GroupName) && cl.GroupName.ToLower().Contains(GroupName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(CommercialName))
            {
                rfiscList = rfiscList.Where(cl => cl.CommercialName != null && cl.CommercialName.ToLower().Contains(CommercialName.ToLower())).ToList();
            }
            return rfiscList.ToList();
        }
    }
}