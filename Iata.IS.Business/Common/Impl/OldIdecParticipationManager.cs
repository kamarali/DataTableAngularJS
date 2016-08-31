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
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Business.MemberProfile;

namespace Iata.IS.Business.Common.Impl
{
    public class OldIdecParticipationManager : IOldIdecParticipationManager
    {
        /// <summary>
        /// Gets or sets the old idec participation repository.
        /// </summary>
        /// <value>
        /// The old idec participation repository.
        /// </value>
        public IOldIdecParticipationRepository OldIdecParticipationRepository { get; set; }
        
        /// <summary>
        /// Adds the old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <returns></returns>
        public OldIdecParticipation AddOldIdecParticipation(OldIdecParticipation oldIdecParticipation)
        {
            var oldIdecParticipationData = OldIdecParticipationRepository.Single(type => type.MemberId == oldIdecParticipation.MemberId);
            //If OldIdecParticipation Code already exists, throw exception
            if (oldIdecParticipationData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidMemberCode);
            }
            //Call repository method for adding oldIdecParticipation
            OldIdecParticipationRepository.Add(oldIdecParticipation);
            UnitOfWork.CommitDefault();
            return oldIdecParticipation;
        }

        /// <summary>
        /// Updates the old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <returns></returns>
        public OldIdecParticipation UpdateOldIdecParticipation(OldIdecParticipation oldIdecParticipation)
        {
            var oldIdecParticipationData = OldIdecParticipationRepository.Single(type => type.MemberId == oldIdecParticipation.MemberId && type.Id != oldIdecParticipation.Id);
            //If OldIdecParticipation Code already exists, throw exception
            if (oldIdecParticipationData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidMemberCode);
            }

            oldIdecParticipationData = OldIdecParticipationRepository.Single(type => type.Id == oldIdecParticipation.Id);
            if (oldIdecParticipationData != null)
            {
                oldIdecParticipation = OldIdecParticipationRepository.Update(oldIdecParticipation);
                UnitOfWork.CommitDefault();
            }
            return oldIdecParticipation;
        }

        /// <summary>
        /// Deletes the old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipationId">The old idec participation id.</param>
        /// <returns></returns>
        public bool DeleteOldIdecParticipation(int oldIdecParticipationId)
        {
            bool delete = false;
            var oldIdecParticipationData = OldIdecParticipationRepository.Single(type => type.Id == oldIdecParticipationId);
            if (oldIdecParticipationData != null)
            {
                oldIdecParticipationData.IsActive = !(oldIdecParticipationData.IsActive);
                var updatedoldIdecParticipation = OldIdecParticipationRepository.Update(oldIdecParticipationData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the old idec participation details.
        /// </summary>
        /// <param name="oldIdecParticipationId">The old idec participation id.</param>
        /// <returns></returns>
        public OldIdecParticipation GetOldIdecParticipationDetails(int oldIdecParticipationId)
        {
            var oldIdecParticipation = OldIdecParticipationRepository.GetOldIdecParticipation(oldIdecParticipationId);
            return oldIdecParticipation;
        }

        /// <summary>
        /// Gets all old idec participation list.
        /// </summary>
        /// <returns></returns>
        public List<OldIdecParticipation> GetAllOldIdecParticipationList()
        {
            var oldIdecParticipationList = OldIdecParticipationRepository.GetAllOldIdecParticipations();

            return oldIdecParticipationList.ToList();
        }

        /// <summary>
        /// Gets the old idec participation list.
        /// </summary>
        /// <param name="MemberId">The member id.</param>
        /// <returns></returns>
        public List<OldIdecParticipation> GetOldIdecParticipationList(int MemberId)
        {
            var oldIdecParticipationList = new List<OldIdecParticipation>();
            oldIdecParticipationList = OldIdecParticipationRepository.GetAllOldIdecParticipations().ToList();

            if (MemberId>0)
            {
                oldIdecParticipationList = oldIdecParticipationList.Where(cl => cl.MemberId == MemberId).ToList();
            }
            return oldIdecParticipationList.ToList();
        }
    }
}
