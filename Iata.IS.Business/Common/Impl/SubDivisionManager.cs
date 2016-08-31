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
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    class SubDivisionManager : ISubDivisionManager
    {
        /// <summary>
        /// Gets or sets the subDivision repository.
        /// </summary>
        /// <value>
        /// The subDivision repository.
        /// </value>
        public ISubDivisionRepository  SubDivisionRepository { get; set; }

        /// <summary>
        /// Adds the subDivision.
        /// </summary>
        /// <param name="subDivision">The subDivision.</param>
        /// <returns></returns>
        public SubDivision AddSubDivision(SubDivision subDivision)
        {
            var subDivisionData = SubDivisionRepository.Single(type => type.Id.Trim().ToLower() == subDivision.Id.Trim().ToLower() && type.CountryId == subDivision.CountryId);
            //If SubDivision Code already exists, throw exception
            if (subDivisionData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidSubDivisionCode);
            }
            //Call repository method for adding subDivision
            SubDivisionRepository.Add(subDivision);
            UnitOfWork.CommitDefault();
            return subDivision;
        }

        /// <summary>
        /// Updates the subDivision.
        /// </summary>
        /// <param name="subDivision">The subDivision.</param>
        /// <returns></returns>
        public SubDivision UpdateSubDivision(SubDivision subDivision)
        {
            var subDivisionData = SubDivisionRepository.Single(type => type.Id.Trim().ToLower() == subDivision.Id.Trim().ToLower() && type.CountryId == subDivision.CountryId);
            //If SubDivision Code already exists, throw exception
            if (subDivisionData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidSubDivisionCode);
            }
            var updatedsubDivision = SubDivisionRepository.Update(subDivision);
            UnitOfWork.CommitDefault();
            return updatedsubDivision;
        }

        /// <summary>
        /// Deletes the subDivision.
        /// </summary>
        /// <param name="subDivisionId">The subDivision id.</param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public bool DeleteSubDivision(string subDivisionId, string countryId)
        {
            bool delete = false;
            var subDivisionData = SubDivisionRepository.Single(type => type.Id.Trim() == subDivisionId.Trim() && type.CountryId==countryId.Trim());
            if (subDivisionData != null)
            {
                subDivisionData.IsActive = !(subDivisionData.IsActive);
                var updatedsubDivision = SubDivisionRepository.Update(subDivisionData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the subDivision details.
        /// </summary>
        /// <param name="subDivisionId">The subDivision id.</param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public SubDivision GetSubDivisionDetails(string subDivisionId,string countryId)
        {
            var subDivision = SubDivisionRepository.GetSubDivisionDetail(subDivisionId, countryId);
            return subDivision;
        }

        /// <summary>
        /// Gets all subDivision list.
        /// </summary>
        /// <returns></returns>
        public List<SubDivision> GetAllSubDivisionList()
        {
            var subDivisionList = SubDivisionRepository.GetAll();

            return subDivisionList.ToList();
        }

        /// <summary>
        /// Gets the subDivision list.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="SubDivisionCodeIcao">The subDivision code icao.</param>
        /// <returns></returns>
        public List<SubDivision> GetSubDivisionList(string Id, string Name, string CountryId)
        {
            var subDivisionList = new List<SubDivision>();
            subDivisionList = SubDivisionRepository.GetAllSubDivisionCodes().ToList();

            if (!string.IsNullOrEmpty(Id))
            {
                subDivisionList = subDivisionList.Where(cl => cl.Id != null && cl.Id.Trim().ToLower().Contains(Id.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                subDivisionList = subDivisionList.Where(cl => cl.Name != null && cl.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(CountryId))
            {
                subDivisionList = subDivisionList.Where(cl => cl.CountryId != null && cl.CountryId.ToLower().Contains(CountryId.ToLower())).ToList();
            }

            return subDivisionList.ToList();
        }
    }
}
