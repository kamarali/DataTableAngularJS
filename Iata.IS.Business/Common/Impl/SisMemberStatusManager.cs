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
    public class SisMemberStatusManager : ISisMemberStatusManager
    {
        public IRepository<SisMemberStatus> SisMemberStatusRepository { get; set; }

        /// <summary>
        /// Adds the sis member status.
        /// </summary>
        /// <param name="sisMemberStatus">The sis member status.</param>
        /// <returns></returns>
        public SisMemberStatus AddSisMemberStatus(SisMemberStatus sisMemberStatus)
        {
            var sisMemberStatusData = SisMemberStatusRepository.Single(type => type.MemberStatus == sisMemberStatus.MemberStatus);
            //If SisMemberStatus Code already exists, throw exception
            if (sisMemberStatusData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidSisMemberStatus);
            }
            //Call repository method for adding sisMemberStatus
            SisMemberStatusRepository.Add(sisMemberStatus);
            UnitOfWork.CommitDefault();
            return sisMemberStatus;
        }

        /// <summary>
        /// Updates the sis member status.
        /// </summary>
        /// <param name="sisMemberStatus">The sis member status.</param>
        /// <returns></returns>
        public SisMemberStatus UpdateSisMemberStatus(SisMemberStatus sisMemberStatus)
        {
            var sisMemberStatusData = SisMemberStatusRepository.Single(type => type.MemberStatus == sisMemberStatus.MemberStatus && type.Id != sisMemberStatus.Id);
            //If SisMemberStatus Code already exists, throw exception
            if (sisMemberStatusData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidSisMemberStatus);
            }
            sisMemberStatusData = SisMemberStatusRepository.Single(type => type.Id == sisMemberStatus.Id);
            //Call repository method for update sisMemberStatus
            sisMemberStatus = SisMemberStatusRepository.Update(sisMemberStatus);
            UnitOfWork.CommitDefault();
            return sisMemberStatus;
        }

        /// <summary>
        /// Deletes the sis member status.
        /// </summary>
        /// <param name="sisMemberStatusId">The sis member status id.</param>
        /// <returns></returns>
        public bool DeleteSisMemberStatus(int sisMemberStatusId)
        {
            bool delete = false;
            var sisMemberStatusData = SisMemberStatusRepository.Single(type => type.Id == sisMemberStatusId);
            if (sisMemberStatusData != null)
            {
                sisMemberStatusData.IsActive = !(sisMemberStatusData.IsActive);
                var updatedcountry = SisMemberStatusRepository.Update(sisMemberStatusData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the sis member status details.
        /// </summary>
        /// <param name="sisMemberStatusId">The sis member status id.</param>
        /// <returns></returns>
        public SisMemberStatus GetSisMemberStatusDetails(int sisMemberStatusId)
        {
            var sisMemberStatus = SisMemberStatusRepository.Single(type => type.Id == sisMemberStatusId);
            return sisMemberStatus;
        }

        /// <summary>
        /// Gets all sis member status list.
        /// </summary>
        /// <returns></returns>
        public List<SisMemberStatus> GetAllSisMemberStatusList()
        {
            var sisMemberStatusList = SisMemberStatusRepository.GetAll();
            return sisMemberStatusList.ToList();
        }

        /// <summary>
        /// Gets the sis member status list.
        /// </summary>
        /// <param name="sisMemberStatus">The sis member status.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public List<SisMemberStatus> GetSisMemberStatusList(string sisMemberStatus, string description)
        {
            var sisMemberStatusList = new List<SisMemberStatus>();
            sisMemberStatusList = SisMemberStatusRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(sisMemberStatus))
            {
                sisMemberStatusList = sisMemberStatusList.Where(cl => cl.MemberStatus.ToLower().Contains(sisMemberStatus.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(description))
            {
                sisMemberStatusList = sisMemberStatusList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(description.ToLower())).ToList();
            }
            return sisMemberStatusList.ToList();
        }
    }
}
