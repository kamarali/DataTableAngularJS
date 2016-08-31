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
    public class RficManager : IRficManager
    {
        public IRepository<Rfic> RficRepository { get; set; }

        /// <summary>
        /// Adds the rfic.
        /// </summary>
        /// <param name="rfic">The rfic.</param>
        /// <returns></returns>
        public Rfic AddRfic(Rfic rfic)
        {
            var rficData = RficRepository.Single(type => type.Id == rfic.Id);
            //If Rfic Code already exists, throw exception
            if (rficData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidRficCode);
            }
            //Call repository method for adding rfic
            RficRepository.Add(rfic);
            UnitOfWork.CommitDefault();
            return rfic;
        }

        /// <summary>
        /// Updates the rfic.
        /// </summary>
        /// <param name="rfic">The rfic.</param>
        /// <returns></returns>
        public Rfic UpdateRfic(Rfic rfic)
        {
            var rficData = RficRepository.Single(type => type.Id == rfic.Id);
            //If Rfic Code already exists, throw exception
            if (rficData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidRficCode);
            }
            var updatedrfic = RficRepository.Update(rfic);
            UnitOfWork.CommitDefault();
            return updatedrfic;
        }

        /// <summary>
        /// Deletes the rfic.
        /// </summary>
        /// <param name="rficId">The rfic id.</param>
        /// <returns></returns>
        public bool DeleteRfic(string rficId)
        {
            bool delete = false;
            var rficData = RficRepository.Single(type => type.Id == rficId);
            if (rficData != null)
            {
                rficData.IsActive = !(rficData.IsActive);
                var updatedcountry = RficRepository.Update(rficData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the rfic details.
        /// </summary>
        /// <param name="rficId">The rfic id.</param>
        /// <returns></returns>
        public Rfic GetRficDetails(string rficId)
        {
            var rfic = RficRepository.Single(type => type.Id == rficId);
            return rfic;
        }

        /// <summary>
        /// Gets all rfic list.
        /// </summary>
        /// <returns></returns>
        public List<Rfic> GetAllRficList()
        {
            var rficList = RficRepository.GetAll();
            return rficList.ToList();
        }

        /// <summary>
        /// Gets the rfic list.
        /// </summary>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        public List<Rfic> GetRficList(string rficId, string rficDescription)
        {
            var rficList = new List<Rfic>();
            rficList = RficRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(rficId))
            {
                rficList = rficList.Where(cl => cl.Id != null && cl.Id.ToLower().Contains(rficId.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(rficDescription))
            {
                rficList = rficList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(rficDescription.ToLower())).ToList();
            }
            return rficList.ToList();
        }
    }
}