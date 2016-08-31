using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SandBox;
using Iata.IS.Data.Sandbox;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Impl;
namespace Iata.IS.Business.Sandbox.Impl
{
    public class SandboxCertificationManager : ISandboxCertificationManager
    {
      public IRepository<CertificationParameterMaster> CertificationParameterRepository { get; set; }

      public IRepository<CertificationTransactionDetails> certifiTrans { get; set; }

      public IRepository<CertificationTransactionGroup> TransactionGroupRepository { get; set; }
      //public ISandboxTransactionDetailRepository SandboxTransactionDetailRepository { get; set; }
      public ISandboxCertificationRepository SandboxCertificationRepository { get; set; }
       
     // public ISandboxTransactionDetailRepository SandboxTransactionDetailRepository { get; set; }

        public CertificationParameterMaster GetCertificationParameters(BillingCategoryType billingCategoryType, TransactionType transactionType)
        {
            var certificationParameterMaster =
              CertificationParameterRepository.Get(
                c => c.BillingCategoryId == (int)billingCategoryType && c.TransactionTypeId == (int)transactionType);
            return (CertificationParameterMaster)certificationParameterMaster;

        }

        public CertificationParameterMaster GetCertificationParameters(BillingCategoryType billingCategoryType, TransactionType transactionType, FileFormatType fileFormatType)
        {
          List<CertificationParameterMaster> certificationParameterMaster =
              CertificationParameterRepository.Get(
                c => c.BillingCategoryId == (int)billingCategoryType && c.TransactionTypeId == (int)transactionType && c.FileFormatId == (int)fileFormatType).ToList();

          if (certificationParameterMaster.Count() > 0)
            return certificationParameterMaster.FirstOrDefault();
          else
            return null;
        }

        public List<CertificationParameterMaster> GetCertificationParameters(BillingCategoryType billingCategoryType, TransactionGroup transactionGroup, FileFormatType fileFormatType)
        {
          return CertificationParameterRepository.Get(
                c => c.BillingCategoryId == (int)billingCategoryType && c.TransactionGroupId == (int)transactionGroup && c.FileFormatId == (int)fileFormatType).ToList();
        }

        

        public List<CertificationParameterMaster> GetAllRecord()
        {
            //var certificationParameterMaster =
            //  CertificationParameterRepository.GetAll();
            //return certificationParameterMaster.ToList();
            var CertificationParameterList = new List<CertificationParameterMaster>();
            return CertificationParameterList = SandboxCertificationRepository.GetCertificationParameter().ToList();
        }

        /// <summary>
        /// Get The SandBox List
        /// </summary>
        /// <param name="billingCategoryId">billingCategoryId</param>
        /// <param name="FileFormatId">FileFormatId</param>
        /// <param name="TransactionTypeId">TransactionTypeId</param>
        /// <returns></returns>
        public List<CertificationParameterMaster> GetSandBoxList(int billingCategoryId, int FileFormatId, int TransactionTypeId)
        {
            //var certificationParameterMaster =
            //  CertificationParameterRepository.GetAll();
            //return certificationParameterMaster.ToList();
            var CertificationParameterList = new List<CertificationParameterMaster>();
            CertificationParameterList = SandboxCertificationRepository.GetCertificationParameter().ToList();
            if (billingCategoryId > 0)
            {
                CertificationParameterList = CertificationParameterList.Where(s => s.BillingCategoryId == billingCategoryId).ToList();
            }
            if (FileFormatId > 0)
            {
                CertificationParameterList = CertificationParameterList.Where(s => s.FileFormatId == FileFormatId).ToList();
            }
            if (TransactionTypeId > 0)
            {
                CertificationParameterList = CertificationParameterList.Where(s => s.TransactionTypeId == TransactionTypeId).ToList();
            }
            return CertificationParameterList.ToList();
        }

        /// <summary>
        /// Update The sandbox 
        /// </summary>
        /// <param name="certificationParameterMaster">By taking CertificationParameterMasterId as parameter </param>
        /// <returns></returns>
        public CertificationParameterMaster UpdateSandBox(CertificationParameterMaster certificationParameterMaster)
        {
            var subSandBoxData = CertificationParameterRepository.Single(type => type.Id == certificationParameterMaster.Id);
            //If SandBox Data is exists, then update the data
            if (subSandBoxData != null)
            {
                certificationParameterMaster = CertificationParameterRepository.Update(certificationParameterMaster);
                UnitOfWork.CommitDefault();
            }
            else
            {
                //throw new ISBusinessException(ErrorCodes.InvalidSubDivisionCode);
            }
            return certificationParameterMaster;
        }

        public List<CertificationTransactionDetails> GetAllTransactionDetails()
        {
      // var sandBoxRep =   Ioc.Resolve<ISandboxTransactionDetailRepository>();
       //List<CertificationTransactionDetails> certificationTransactionDetails = SandboxTransactionDetailRepository.GetAllTransactionDetails().ToList();
          var certificationTransactionDetails = SandboxCertificationRepository.GetCertificationTransDetails();
          return (List<CertificationTransactionDetails>) certificationTransactionDetails;
        }
    }
}
