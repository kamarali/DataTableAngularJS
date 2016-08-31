using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class SisMemberSubStatusManager : ISisMemberSubStatusManager
    {
        /// <summary>
        /// Gets or sets the sis member sub status repository.
        /// </summary>
        /// <value>
        /// The sis member sub status repository.
        /// </value>
        //public ISisMemberSubStatusRepository SisMemberSubStatusRepository { get; set; }

        public IRepository<SisMemberSubStatus> SisMemberSubStatusRepository { get; set; }


        private IMemberRepository _memberManager;
        /// <summary>
        /// Adds the sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <returns></returns>
        public SisMemberSubStatus AddSisMemberSubStatus(SisMemberSubStatus sisMemberSubStatus)
        {
            var sisMemberSubStatusData = SisMemberSubStatusRepository.Single(type => type.Description == sisMemberSubStatus.Description);
            //If SisMemberSubStatus Code already exists, throw exception
            if (sisMemberSubStatusData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidSisMemberSubStatus);
            }
            //Call repository method for adding sisMemberSubStatus
            SisMemberSubStatusRepository.Add(sisMemberSubStatus);
            UnitOfWork.CommitDefault();
            return sisMemberSubStatus;
        }

        /// <summary>
        /// Updates the sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <returns></returns>
        public SisMemberSubStatus UpdateSisMemberSubStatus(SisMemberSubStatus sisMemberSubStatus)
        {
            
           var sisMemberSubStatusData = SisMemberSubStatusRepository.Single(type =>  type.Id == sisMemberSubStatus.Id);

           sisMemberSubStatusData.Description = sisMemberSubStatus.Description;
           sisMemberSubStatusData.IsActive = sisMemberSubStatus.IsActive;

           //CMP #665: User Related Enhancements-FRS-v1.2.doc.
           sisMemberSubStatusData.SuppressOtpEmail = sisMemberSubStatus.SuppressOtpEmail;
           sisMemberSubStatusData.RedirectUponLogin = sisMemberSubStatus.RedirectUponLogin;
           sisMemberSubStatusData.LimitedMemProfileAccess = sisMemberSubStatus.LimitedMemProfileAccess;
           sisMemberSubStatusData.DisableUserProfileUpdates = sisMemberSubStatus.DisableUserProfileUpdates;

            var updatedsisMemberSubStatus = SisMemberSubStatusRepository.Update(sisMemberSubStatusData);
            UnitOfWork.CommitDefault();
            return updatedsisMemberSubStatus;
        }

        /// <summary>
        /// Deletes the sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatusId">The sis member sub status id.</param>
        /// <returns></returns>
        public bool DeleteSisMemberSubStatus(int sisMemberSubStatusId)
        {
            bool delete = false;
            var sisMemberSubStatusData = SisMemberSubStatusRepository.Single(type => type.Id == sisMemberSubStatusId);
            if (sisMemberSubStatusData != null)
            {
                sisMemberSubStatusData.IsActive = !(sisMemberSubStatusData.IsActive);
                var updatedcountry = SisMemberSubStatusRepository.Update(sisMemberSubStatusData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the sis member sub status details.
        /// </summary>
        /// <param name="sisMemberSubStatusId">The sis member sub status id.</param>
        /// <returns></returns>
        public SisMemberSubStatus GetSisMemberSubStatusDetails(int sisMemberSubStatusId)
        {
          var sisMemberSubStatus = SisMemberSubStatusRepository.First(t => t.Id == sisMemberSubStatusId);
            return sisMemberSubStatus;
        }

        /// <summary>
        /// Gets all sis member sub status list.
        /// </summary>
        /// <returns></returns>
        public List<SisMemberSubStatus> GetAllSisMemberSubStatusList()
        {
          var sisMemberSubStatusList = SisMemberSubStatusRepository.GetAll();
            return sisMemberSubStatusList.ToList();
        }

        /// <summary>
        /// Gets the sis member sub status list.
        /// </summary>
        /// <param name="sisMemberStatusId">The sis member status id.</param>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public List<SisMemberSubStatus> GetSisMemberSubStatusList(string description)
        {
            var sisMemberSubStatusList = new List<SisMemberSubStatus>();
          sisMemberSubStatusList = SisMemberSubStatusRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(description))
            {
                sisMemberSubStatusList = sisMemberSubStatusList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(description.ToLower())).ToList();
            }
            return sisMemberSubStatusList.ToList();
        }

        /// <summary>
        /// Check the Member Sub status existance in Member Detail table
        /// </summary>
        /// <param name="sisMemberSubStatusId"></param>
        /// <returns>true/false</returns>
      public bool IsSubStatusExistanceInMemberProfile(int sisMemberSubStatusId)
      {
        _memberManager = Ioc.Resolve<IMemberRepository>();

        return _memberManager.GetCount(m => m.IsMembershipSubStatusId == sisMemberSubStatusId) > 0 ? true : false;

      }

      /// <summary>
      /// Check for duplicate entry
      /// </summary>
      /// <param name="sisMemberSubStatusDesc"></param>
      /// <returns></returns>
      public bool CheckSubStatusDuplication(string  sisMemberSubStatusDesc)
      {
        return SisMemberSubStatusRepository.GetCount(s => s.Description.ToUpper() == sisMemberSubStatusDesc.ToUpper()) > 0 ? true : false;
      }

    }
}
