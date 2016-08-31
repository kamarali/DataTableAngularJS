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
    public class TaxCodeManager : ITaxCodeManager
    {
        /// <summary>
        /// Gets or sets the tax code repository.
        /// </summary>
        /// <value>
        /// The tax code repository.
        /// </value>
        public IRepository<TaxCode> TaxCodeRepository { get; set; }

        /// <summary>
        /// Adds the tax code.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <returns></returns>
        public TaxCode AddTaxCode(TaxCode taxCode)
        {
            var TaxCodeData = TaxCodeRepository.Single(type => type.Id.ToLower() == taxCode.Id.ToLower());
            //If TaxCode Code already exists, throw exception
            if (TaxCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidTaxCode);
            }
            TaxCodeRepository.Add(taxCode);
            UnitOfWork.CommitDefault();
            return taxCode;
        }

        /// <summary>
        /// Updates the tax code.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <returns></returns>
        public TaxCode UpdateTaxCode(TaxCode taxCode)
        {
            var TaxCodeData = TaxCodeRepository.Single(type => type.Id == taxCode.Id);
            if (TaxCodeData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidTaxCode);
            }
            var updatedTaxCode = TaxCodeRepository.Update(taxCode);
            UnitOfWork.CommitDefault();
            return updatedTaxCode;
        }

        /// <summary>
        /// Deletes the tax code.
        /// </summary>
        /// <param name="taxCodeId">The tax code id.</param>
        /// <returns></returns>
        public bool DeleteTaxCode(string taxCodeId)
        {
            bool delete = false;
            var taxCodeData = TaxCodeRepository.Single(type => type.Id == taxCodeId);
            if (taxCodeData != null)
            {
                taxCodeData.IsActive = !(taxCodeData.IsActive);
                var updatedcountry = TaxCodeRepository.Update(taxCodeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the tax code details.
        /// </summary>
        /// <param name="taxCodeId">The tax code id.</param>
        /// <returns></returns>
        public TaxCode GetTaxCodeDetails(string taxCodeId)
        {
            var TaxCode = TaxCodeRepository.Single(type => type.Id == taxCodeId);
            return TaxCode;
        }

        /// <summary>
        /// Gets all tax code list.
        /// </summary>
        /// <returns></returns>
        public List<TaxCode> GetAllTaxCodeList()
        {
            var TaxCodeList = TaxCodeRepository.GetAll();
            return TaxCodeList.ToList();
        }

        /// <summary>
        /// Gets the tax code list.
        /// </summary>
        /// <param name="TaxCodeTypeId">The tax code type id.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        public List<TaxCode> GetTaxCodeList(string TaxCodeId, string Description)
        {
            var TaxCodeList = new List<TaxCode>();
            TaxCodeList = TaxCodeRepository.GetAll().ToList();

            if ((!string.IsNullOrEmpty(TaxCodeId)))
            {
                TaxCodeList = TaxCodeList.Where(cl => (cl.Id.ToLower().Contains(TaxCodeId.ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(Description))
            {
                TaxCodeList = TaxCodeList.Where(cl => cl.Description != null && (cl.Description.ToLower().Contains(Description.ToLower()))).ToList();
            }
            return TaxCodeList.ToList();
        }
    }
}
