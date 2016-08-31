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
    public class MiscCodeManager : IMiscCodeManager
    {
        /// <summary>
        /// Gets or sets the misc code repository.
        /// </summary>
        /// <value>
        /// The misc code repository.
        /// </value>
        //public IRepository<MiscCode> MiscCodeRepository { get; set; }
        public IMiscCodeRepository MiscCodeRepository { get; set; }
        /// <summary>
        /// Adds the misc code.
        /// </summary>
        /// <param name="miscCode">The misc code.</param>
        /// <returns></returns>
        public MiscCode AddMiscCode(MiscCode miscCode)
        {
            var miscCodeData = MiscCodeRepository.Single(type => type.Group == miscCode.Group && type.Name == miscCode.Name);
            //If MiscCode Code already exists, throw exception
            if (miscCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidMiscCode);
            }
            //Call repository method for adding miscCode
            MiscCodeRepository.Add(miscCode);
            UnitOfWork.CommitDefault();
            return miscCode;
        }

        /// <summary>
        /// Updates the misc code.
        /// </summary>
        /// <param name="miscCode">The misc code.</param>
        /// <returns></returns>
        public MiscCode UpdateMiscCode(MiscCode miscCode)
        {
            var miscCodeData = MiscCodeRepository.Single(type => type.Group == miscCode.Group && type.Name == miscCode.Name && type.Id != miscCode.Id);
            if (miscCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidMiscCode);
            }
            miscCodeData = MiscCodeRepository.Single(type => type.Id == miscCode.Id);
            var updatedmiscCode = MiscCodeRepository.Update(miscCode);
            UnitOfWork.CommitDefault();
            return updatedmiscCode;
        }

        /// <summary>
        /// Deletes the misc code.
        /// </summary>
        /// <param name="miscCodeId">The misc code id.</param>
        /// <returns></returns>
        public bool DeleteMiscCode(int miscCodeId)
        {
            bool delete = false;
            var miscCodeData = MiscCodeRepository.Single(type => type.Id == miscCodeId);
            if (miscCodeData != null)
            {
                miscCodeData.IsActive = !(miscCodeData.IsActive);
                var updatedcountry = MiscCodeRepository.Update(miscCodeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the misc code details.
        /// </summary>
        /// <param name="miscCodeId">The misc code id.</param>
        /// <returns></returns>
        public MiscCode GetMiscCodeDetails(int miscCodeId)
        {
            var miscCode = MiscCodeRepository.Single(type => type.Id == miscCodeId);
            return miscCode;
        }

        /// <summary>
        /// Gets all misc code list.
        /// </summary>
        /// <returns></returns>
        public List<MiscCode> GetAllMiscCodeList()
        {
            var miscCodeList = MiscCodeRepository.GetAllMiscCodes();
            return miscCodeList.ToList();
        }

        /// <summary>
        /// Gets the misc code list.
        /// </summary>
        /// <param name="GroupId">The group id.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        public List<MiscCode> GetMiscCodeList(int GroupId, string Name, string Description)
        {
            var miscCodeList = new List<MiscCode>();
            miscCodeList = MiscCodeRepository.GetAllMiscCodes().Where(cl => cl.ISSYSCode == "0").ToList();
            if (GroupId > 0)
            {
                miscCodeList = miscCodeList.Where(cl => cl.Group == GroupId).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                miscCodeList = miscCodeList.Where(cl => cl.Name.ToLower().Contains(Name.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Description))
            {
                miscCodeList = miscCodeList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(Description.Trim().ToLower())).ToList();
            }
            return miscCodeList.ToList();
        }
    }
}