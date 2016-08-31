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
    public class IchZoneManager : IIchZoneManager
    {
        public IRepository<IchZone> IchZoneRepository { get; set; }

        public IchZone AddIchZone(IchZone ichZone)
        {
            var ichZoneData = IchZoneRepository.Single(type => type.Id == ichZone.Id);
            //If IchZone Code already exists, throw exception
            if (ichZoneData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCountryCode);
            }
            //Call repository method for adding ichZone
            IchZoneRepository.Add(ichZone);
            UnitOfWork.CommitDefault();
            return ichZone;
        }

        public IchZone UpdateIchZone(IchZone ichZone)
        {
            var ichZoneData = IchZoneRepository.Single(type => type.Id == ichZone.Id);
            var updatedichZone = IchZoneRepository.Update(ichZone);
            UnitOfWork.CommitDefault();
            return updatedichZone;
        }

        public bool DeleteIchZone(int ichZoneId)
        {
            bool delete = false;
            var ichZoneData = IchZoneRepository.Single(type => type.Id == ichZoneId);
            if (ichZoneData != null)
            {
                ichZoneData.IsActive = !(ichZoneData.IsActive);
                var updatedcountry = IchZoneRepository.Update(ichZoneData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        public IchZone GetIchZoneDetails(int ichZoneId)
        {
            var ichZone = IchZoneRepository.Single(type => type.Id == ichZoneId);
            return ichZone;
        }

        public List<IchZone> GetAllIchZoneList()
        {
            var ichZoneList = IchZoneRepository.GetAll();
            return ichZoneList.ToList();
        }

        public List<IchZone> GetIchZoneList(string Zone, string ClearanceCurrency, string Description)
        {
            var ichZoneList = new List<IchZone>();
            ichZoneList = IchZoneRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Zone))
            {
                ichZoneList = ichZoneList.Where(cl => cl.Zone.ToLower().Contains(Zone.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(ClearanceCurrency))
            {
                ichZoneList = ichZoneList.Where(cl => cl.ClearanceCurrency.ToLower().Contains(ClearanceCurrency.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Description))
            {
                ichZoneList = ichZoneList.Where(cl => cl.Description.ToLower().Contains(Description.ToLower())).ToList();
            }
            return ichZoneList.ToList();
        }
    }
}