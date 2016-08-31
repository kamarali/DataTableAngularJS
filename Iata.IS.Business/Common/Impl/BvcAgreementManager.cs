using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Data.Common;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.Pax;
using Iata.IS.Data;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Business.Common.Impl
{
    /// <summary>
    /// Performs Database actions for BVC Agreement Controller 
    /// </summary>
    public class BvcAgreementManager : IBvcAgreementManager
    {
        /// <summary>
        /// Get object of MemberManager
        /// </summary>
        public IMemberRepository MemberRepository { get; set; }

        /// <summary>
        /// Get object of Irepository of BvcAgreement type
        /// </summary>
        public IBVCAgreementMasterRepository BvcAgreementRepository { get; set; }

        /// <summary>
        /// CMP #596: Length of Member Accounting Code to be Increased to 12 
        /// Desc: New validation #MW2 and #MW3. The Member should not be a Type B Member.
        /// Disallow Type B members to bill or be billed.
        /// Ref: FRS Section 3.1 Point 11.
        /// </summary>
        public IInvoiceManager InvoiceManager { get; set; }

        /// <summary>
        /// Add new Bvc Agreement
        /// </summary>
        /// <param name="bvcAgreement"></param>
        /// <returns></returns>
        public BvcAgreement AddBVCAgreement(BvcAgreement bvcAgreement)
        {
            var bvcAgreementData = BvcAgreementRepository.Single(bvc => bvc.BillingMemberId.Equals(bvcAgreement.BillingMemberId) && bvc.BilledMemberId.Equals(bvcAgreement.BilledMemberId));

            //If Bvc Agreement already exists, throw exception
            if (bvcAgreementData != null)
            {
                throw new ISBusinessException(ErrorCodes.BvcAgreementCombinationAlreadyExists);
            }
            else
            {
                //Call repository method for adding Bvc Member
                Member billingMember = MemberRepository.Single(mem => mem.Id == bvcAgreement.BillingMemberId);
                Member billedMember = MemberRepository.Single(mem => mem.Id == bvcAgreement.BilledMemberId);

                if (billingMember != null && billedMember != null && billingMember.IsParticipateInValueConfirmation != false && billingMember.IsMembershipStatus != MemberStatus.Terminated && billedMember.IsMembershipStatus != MemberStatus.Terminated)
                {
                    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
                    Desc: New validation #MW2 and #MW3. The Member should not be a Type B Member.
                    Disallow Type B members to bill or be billed.
                    Ref: FRS Section 3.1 Point 11. */
                    if (InvoiceManager.IsTypeBMember(billingMember.MemberCodeNumeric))
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidMemberType);
                    }

                    if (InvoiceManager.IsTypeBMember(billedMember.MemberCodeNumeric))
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidMemberType);
                    }

                    BvcAgreementRepository.Add(bvcAgreement);
                    UnitOfWork.CommitDefault();
                }
                else
                {
                    throw new ISBusinessException(ErrorCodes.MemberIsNotBvcParticipant);
                }

                
            }
            return bvcAgreement;
        }

        /// <summary>
        /// Update provided Bvc Agreement
        /// </summary>
        /// <param name="bvcAgreement"></param>
        /// <returns></returns>
        public BvcAgreement UpdateBVCAgreement(BvcAgreement bvcAgreement)
        {
            var bvcAgreementData = BvcAgreementRepository.Single(bvc => bvc.BvcMappingId == bvcAgreement.BvcMappingId);
            bvcAgreementData.BilledMemberId = bvcAgreement.BilledMemberId;
            bvcAgreementData.BillingMemberId = bvcAgreement.BillingMemberId;
            bvcAgreementData.IsActive = bvcAgreement.IsActive;

            Member billingMember = MemberRepository.Single(mem => mem.Id == bvcAgreement.BillingMemberId);
            Member billedMember = MemberRepository.Single(mem => mem.Id == bvcAgreement.BilledMemberId);

            //As both biiling member are editable @ edit mode. Checking if same combination are already available with other mappingid.
            var bvcAgreementDuplicateData = BvcAgreementRepository.Single(bvc => bvc.BilledMemberId == bvcAgreement.BilledMemberId && bvc.BillingMemberId == bvcAgreement.BillingMemberId);
            if (bvcAgreementDuplicateData != null && bvcAgreementData.BvcMappingId != bvcAgreementDuplicateData.BvcMappingId && bvcAgreementData.BillingMemberId == bvcAgreementDuplicateData.BillingMemberId && bvcAgreementData.BilledMemberId == bvcAgreementDuplicateData.BilledMemberId)
            {
                throw new ISBusinessException(ErrorCodes.BvcAgreementCombinationAlreadyExists);
            }

            if (billingMember != null && billedMember != null && billingMember.IsParticipateInValueConfirmation != false && billingMember.IsMembershipStatus != MemberStatus.Terminated && billedMember.IsMembershipStatus != MemberStatus.Terminated)
            {
                /* CMP #596: Length of Member Accounting Code to be Increased to 12 
                Desc: New validation #MW2 and #MW3. The Member should not be a Type B Member.
                Disallow Type B members to bill or be billed.
                Ref: FRS Section 3.1 Point 11. */
                if (InvoiceManager.IsTypeBMember(billingMember.MemberCodeNumeric))
                {
                    throw new ISBusinessException(ErrorCodes.InvalidMemberType);
                }

                if (InvoiceManager.IsTypeBMember(billedMember.MemberCodeNumeric))
                {
                    throw new ISBusinessException(ErrorCodes.InvalidMemberType);
                }

                bvcAgreement = BvcAgreementRepository.Update(bvcAgreementData);
                UnitOfWork.CommitDefault();
            }
            else
            {
                throw new ISBusinessException(ErrorCodes.MemberIsNotBvcParticipant);
            }
            return bvcAgreement;
        }

        /// <summary>
        /// Active/Deactive Bvc Agreement
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public bool ActiveDeactiveBVCAgreement(string mappingId)
        {
            int id = Convert.ToInt32(mappingId);
            bool isActivated = false;
            var bvcAgreement = BvcAgreementRepository.Single(bvc => bvc.BvcMappingId.Equals(id));

            if (bvcAgreement != null)
            {
                var member = MemberRepository.Single(mem => mem.Id == bvcAgreement.BillingMemberId);
                if (bvcAgreement.IsActive == false && member.IsMembershipStatus != MemberStatus.Terminated && member.IsParticipateInValueConfirmation == false)
                {
                    throw new ISBusinessException(ErrorCodes.InvalidBvcAgreementDetails);
                }
                else
                {
                    bvcAgreement.IsActive = !(bvcAgreement.IsActive);
                }


                var updatedRecord = BvcAgreementRepository.Update(bvcAgreement);
                isActivated = true;
                UnitOfWork.CommitDefault();
            }

            return isActivated;
        }

        /// <summary>
        /// Fetches Bvc Agreement for mapping id
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public BvcAgreement GetBVCAgreementDetails(string mappingId)
        {
            var id = Convert.ToInt32(mappingId);

            var bvcAgreementData = BvcAgreementRepository.Single(bvc => bvc.BvcMappingId == id);

            bvcAgreementData.BilledMemberText = GetMemberText(bvcAgreementData.BilledMemberId);
            bvcAgreementData.BillingMemberText = GetMemberText(bvcAgreementData.BillingMemberId);

            return bvcAgreementData;
        }

        /// <summary>
        /// Get Member text in format of alfacode-num code- comercialname
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private string GetMemberText(int memberId)
        {
            var member = MemberRepository.Single(mem => mem.Id == memberId);

            return string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName);
        }

        /// <summary>
        /// Fetches all bvc agreement.
        /// </summary>
        /// <returns></returns>
        public List<BvcAgreement> GetAllBVCAgreementList()
        {
            var bvcAgreementList = BvcAgreementRepository.GetAll();

            return bvcAgreementList.ToList();
        }

        /// <summary>
        /// Fetches bvc agreement for supplied billing & billed member.
        /// </summary>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <returns></returns>
        public List<BvcAgreement> GetBVCAgreementList(int billingMemberId, int billedMemberId)
        {
            var members = MemberRepository.GetAll();

            var bvcAgreementData = BvcAgreementRepository.GetAll().ToList();
            List<BvcAgreement> bvcAgreementDetails = (from bvcs in bvcAgreementData
                                                      join billingmem in members on bvcs.BillingMemberId equals billingmem.Id
                                                      join billedmem in members on bvcs.BilledMemberId equals billedmem.Id
                                                      select new BvcAgreement
                                                      {
                                                          BilledMemberText = string.Format("{0}-{1}-{2}", billedmem.MemberCodeAlpha, billedmem.MemberCodeNumeric, billedmem.CommercialName),
                                                          BillingMemberText = string.Format("{0}-{1}-{2}", billingmem.MemberCodeAlpha, billingmem.MemberCodeNumeric, billingmem.CommercialName),
                                                          BilledMemberId = bvcs.BilledMemberId,
                                                          BillingMemberId = bvcs.BillingMemberId,
                                                          BvcMappingId = bvcs.BvcMappingId,
                                                          IsActive = bvcs.IsActive,
                                                          LastUpdatedBy = bvcs.LastUpdatedBy,
                                                          LastUpdatedOn = bvcs.LastUpdatedOn
                                                      }).ToList();


            if (billedMemberId != 0 && billingMemberId != 0)
            {
                bvcAgreementDetails = bvcAgreementDetails.Where(bvc => bvc.BilledMemberId == billedMemberId && bvc.BillingMemberId == billingMemberId).ToList();
            }
            else if (billedMemberId != 0)
            {
                bvcAgreementDetails = bvcAgreementDetails.Where(bvc => bvc.BilledMemberId == billedMemberId).ToList();
            }
            else if (billingMemberId != 0)
            {
                bvcAgreementDetails = bvcAgreementDetails.Where(bvc => bvc.BillingMemberId == billingMemberId).ToList();
            }

            return bvcAgreementDetails;
        }
    }
}
