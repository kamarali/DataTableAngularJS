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
using Iata.IS.Model.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class VatIdentifierManager : IVatIdentifierManager
    {
        /// <summary>
        /// Gets or sets the vat identifier repository.
        /// </summary>
        /// <value>
        /// The vat identifier repository.
        /// </value>
        public IRepository<VatIdentifier> VatIdentifierRepository { get; set; }

        /// <summary>
        /// Adds the vat identifier.
        /// </summary>
        /// <param name="vatIdentifier">The vat identifier.</param>
        /// <returns></returns>
        public VatIdentifier AddVatIdentifier(VatIdentifier vatIdentifier)
        {
            var vatIdentifierData = VatIdentifierRepository.Single(type => type.Identifier == vatIdentifier.Identifier);
            //If VatIdentifier Code already exists, throw exception
            if (vatIdentifierData != null)
            {
                throw new ISBusinessException(ErrorCodes.VatIdentifierAlreadyExists);
            }
            //Call repository method for adding vatIdentifier
            VatIdentifierRepository.Add(vatIdentifier);
            UnitOfWork.CommitDefault();
            return vatIdentifier;
        }

        /// <summary>
        /// Updates the vat identifier.
        /// </summary>
        /// <param name="vatIdentifier">The vat identifier.</param>
        /// <returns></returns>
        public VatIdentifier UpdateVatIdentifier(VatIdentifier vatIdentifier)
        {
            var vatIdentifierData = VatIdentifierRepository.Single(type => type.Id != vatIdentifier.Id && type.Identifier == vatIdentifier.Identifier);
            //If VatIdentifier Code already exists, throw exception
            if (vatIdentifierData != null)
            {
                throw new ISBusinessException(ErrorCodes.VatIdentifierAlreadyExists);
            }
            vatIdentifierData = VatIdentifierRepository.Single(type => type.Id == vatIdentifier.Id);
            var updatedvatIdentifier = VatIdentifierRepository.Update(vatIdentifier);
            UnitOfWork.CommitDefault();
            return updatedvatIdentifier;
        }

        /// <summary>
        /// Deletes the vat identifier.
        /// </summary>
        /// <param name="vatIdentifierId">The vat identifier id.</param>
        /// <returns></returns>
        public bool DeleteVatIdentifier(int vatIdentifierId)
        {
            bool delete = false;
            var vatIdentifierData = VatIdentifierRepository.Single(type => type.Id == vatIdentifierId);
            if (vatIdentifierData != null)
            {
                vatIdentifierData.IsActive = !(vatIdentifierData.IsActive);
                var updatedcountry = VatIdentifierRepository.Update(vatIdentifierData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the vat identifier details.
        /// </summary>
        /// <param name="vatIdentifierId">The vat identifier id.</param>
        /// <returns></returns>
        public VatIdentifier GetVatIdentifierDetails(int vatIdentifierId)
        {
            var vatIdentifier = VatIdentifierRepository.Single(type => type.Id == vatIdentifierId);
            return vatIdentifier;
        }

        /// <summary>
        /// Gets all vat identifier list.
        /// </summary>
        /// <returns></returns>
        public List<VatIdentifier> GetAllVatIdentifierList()
        {
            var vatIdentifierList = VatIdentifierRepository.GetAll();
            return vatIdentifierList.ToList();
        }

        /// <summary>
        /// Gets the vat identifier list.
        /// </summary>
        /// <param name="vatIdentifier">The vat identifier.</param>
        /// <param name="billingCategory">The billing category.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        public List<VatIdentifier> GetVatIdentifierList(string Identifier, int billingCategory, string Description)
        {
            var vatIdentifierList = new List<VatIdentifier>();
            vatIdentifierList = VatIdentifierRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Identifier))
            {
                vatIdentifierList = vatIdentifierList.Where(cl => cl.Identifier.ToLower().Contains(Identifier.ToLower())).ToList();
            }
            if (billingCategory>0)
            {
                vatIdentifierList = vatIdentifierList.Where(cl => cl.BillingCategoryCode == billingCategory).ToList();
            }
            if (!string.IsNullOrEmpty(Description))
            {
                vatIdentifierList = vatIdentifierList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(Description.ToLower())).ToList();
            }
            return vatIdentifierList.ToList();
        }
    }
}
