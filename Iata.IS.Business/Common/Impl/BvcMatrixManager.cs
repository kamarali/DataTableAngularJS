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
    public class BvcMatrixManager : IBvcMatrixManager
    {
        public IRepository<BvcMatrix> BvcMatrixRepository { get; set; }

        /// <summary>
        /// Adds the BVC matrix.
        /// </summary>
        /// <param name="bvcMatrix">The BVC matrix.</param>
        /// <returns></returns>
        public BvcMatrix AddBvcMatrix(BvcMatrix bvcMatrix)
        {
            //var bvcMatrixData = BvcMatrixRepository.Single(type => type.Id == bvcMatrix.Id);
            ////If BvcMatrix Code already exists, throw exception
            //if (bvcMatrixData != null)
            //{
            //    throw new ISBusinessException(ErrorCodes.InvalidCountryCode);
            //}
            //Call repository method for adding bvcMatrix
            BvcMatrixRepository.Add(bvcMatrix);
            UnitOfWork.CommitDefault();
            return bvcMatrix;
        }

        /// <summary>
        /// Updates the BVC matrix.
        /// </summary>
        /// <param name="bvcMatrix">The BVC matrix.</param>
        /// <returns></returns>
        public BvcMatrix UpdateBvcMatrix(BvcMatrix bvcMatrix)
        {
            var bvcMatrixData = BvcMatrixRepository.Single(type => type.Id == bvcMatrix.Id);
            if (bvcMatrixData != null)
            {
                bvcMatrix = BvcMatrixRepository.Update(bvcMatrix);
                UnitOfWork.CommitDefault();
            }
            return bvcMatrix;
        }

        /// <summary>
        /// Deletes the BVC matrix.
        /// </summary>
        /// <param name="bvcMatrixId">The BVC matrix id.</param>
        /// <returns></returns>
        public bool DeleteBvcMatrix(int bvcMatrixId)
        {
            bool delete = false;
            var bvcMatrixData = BvcMatrixRepository.Single(type => type.Id == bvcMatrixId);
            if (bvcMatrixData != null)
            {
                bvcMatrixData.IsActive = !(bvcMatrixData.IsActive);
                var updatedcountry = BvcMatrixRepository.Update(bvcMatrixData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the BVC matrix details.
        /// </summary>
        /// <param name="bvcMatrixId">The BVC matrix id.</param>
        /// <returns></returns>
        public BvcMatrix GetBvcMatrixDetails(int bvcMatrixId)
        {
            var bvcMatrix = BvcMatrixRepository.Single(type => type.Id == bvcMatrixId);
            return bvcMatrix;
        }

        /// <summary>
        /// Gets all BVC matrix list.
        /// </summary>
        /// <returns></returns>
        public List<BvcMatrix> GetAllBvcMatrixList()
        {
            var bvcMatrixList = BvcMatrixRepository.GetAll();
            return bvcMatrixList.ToList();
        }

        /// <summary>
        /// Gets the BVC matrix list.
        /// </summary>
        /// <param name="ValidatedPmi"></param>
        /// <param name="effectiveFrom">The effective from.</param>
        /// <param name="effectiveTo">The effective to.</param>
        /// <returns></returns>
        public List<BvcMatrix> GetBvcMatrixList(string ValidatedPmi, string effectiveFrom, string effectiveTo)
        {
            var bvcMatrixList = new List<BvcMatrix>();
            bvcMatrixList = BvcMatrixRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(ValidatedPmi))
            {
                bvcMatrixList = bvcMatrixList.Where(cl => cl.ValidatedPmi.ToLower().Contains(ValidatedPmi.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(effectiveFrom))
            {
                bvcMatrixList = bvcMatrixList.Where(cl => cl.EffectiveFrom.ToLower() == effectiveFrom.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(effectiveTo))
            {
                bvcMatrixList = bvcMatrixList.Where(cl => cl.EffectiveTo.ToLower() == effectiveTo.ToLower()).ToList();
            }
            return bvcMatrixList.ToList();
        }
    }
}