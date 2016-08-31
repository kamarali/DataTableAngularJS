using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Reports;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Reports;

namespace Iata.IS.Business.Reports.Impl
{
    class ManageSuspendedInvoicesManager : IManageSuspendedInvoicesManager
    {
        /// <summary>
        /// Gets or sets InvoiceBase repository.
        /// </summary>
        public IManageSuspendedInvoicesRepository SuspendedInvoiceDetailsRepository { get; set; }

        public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }

        public IRepository<MemberStatusDetails> MemberStatusRepository { get; set; }

        private readonly IMemberManager _memberManager;

        private readonly ICalendarManager _calenderManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calenderManager"></param>
        public ManageSuspendedInvoicesManager(ICalendarManager calenderManager, IMemberManager memberManager)
        {
            _memberManager = memberManager;
            _calenderManager = calenderManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billingMemberId"></param>
        /// <param name="fromBillingMonth"></param>
        /// <param name="toBillingMonth"></param>
        /// <param name="fromBillingPeriod"></param>
        /// <param name="toBillingPeriod"></param>
        /// <param name="smi"></param>
        /// <param name="resubmissionStatus"></param>
        /// <param name="billedEntityId"></param>
        /// <param name="fromBillingYear"></param>
        /// <param name="toBillingYear"></param>
        /// <returns></returns>
        public List<SuspendedInvoiceDetails> GetSuspendedInvoiceList(int billingMemberId, int fromBillingMonth, int toBillingMonth, int fromBillingPeriod, int toBillingPeriod, int smi, int resubmissionStatus, int billedEntityId, int fromBillingYear, int toBillingYear)
        {
            var suspendedInvoiceList = SuspendedInvoiceDetailsRepository.GetSuspendedInvoiceList(billingMemberId, fromBillingMonth, toBillingMonth, fromBillingPeriod, toBillingPeriod, smi, resubmissionStatus, billedEntityId, fromBillingYear, toBillingYear);

            // Check the List contains any invoices
            if (suspendedInvoiceList.Count > 0)
            {
                // Method to deactivate suspended invoices
                suspendedInvoiceList = DeactivateDefaultSuspensionInvoices(suspendedInvoiceList);
            }

            return suspendedInvoiceList.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateInvoiceRemark(Guid invoiceId, string remark)
        {
            try
            {
                var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
                if (invoiceRecord != null)
                {
                    invoiceRecord.ResubmissionRemarks = remark;
                    InvoiceBaseRepository.Update(invoiceRecord);
                    UnitOfWork.CommitDefault();
                }
                return true;

            }
            catch (Exception)
            {

                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public InvoiceBase GetInvoice(Guid invoiceId)
        {
            var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
            return invoiceRecord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceIdList"></param>
        /// <param name="resubmitInIchPreviousPeriod"></param>
        /// <param name="resubmitInAchPreviousPeriod"></param>
        /// <returns></returns>
        public bool MarkInvoicesAsResubmitted(List<string> invoiceIdList, bool resubmitInIchPreviousPeriod, bool resubmitInAchPreviousPeriod)
        {

            //var currentPeriod = _calenderManager.GetCurrentBillingPeriod();
            BillingPeriod currentPeriod;
            try
            {
                currentPeriod = _calenderManager.GetCurrentBillingPeriod();
            }
            catch (ISCalendarDataNotFoundException)
            {
                currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
            }
            var ichPreviousPerid = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);
            var achPreviousPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach);

            //var firstInvoiceId = new Guid(invoiceIdList[0]);
            //var firstInvoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == firstInvoiceId);
            //var ichBillingMemberStatus = GetMemberStatus(firstInvoiceRecord.BillingMemberId, "ICH");
            //var achBillingMemberStatus = GetMemberStatus(firstInvoiceRecord.BillingMemberId, "ACH");


            //if (((achBillingMemberStatus != (int)IchMemberShipStatus.Live) && (achBillingMemberStatus != (int)IchMemberShipStatus.NotAMember)) || ((ichBillingMemberStatus != (int)IchMemberShipStatus.Live) && (ichBillingMemberStatus != (int)IchMemberShipStatus.NotAMember)))
            //  throw new ISBusinessException(ErrorCodes.BillingMemberSuspended);

            try
            {

                foreach (var item in invoiceIdList)
                {
                    var invoiceId = new Guid(item);
                    var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
                    //Billing Member
                    var ichBillingMemberStatus = GetMemberStatus(invoiceRecord.BillingMemberId, "ICH");
                    var achBillingMemberStatus = GetMemberStatus(invoiceRecord.BillingMemberId, "ACH");
                    //Billed Member
                    var achbilledMemberStatus = GetMemberStatus(invoiceRecord.BilledMemberId, "ACH");
                    var ichbilledMemberStatus = GetMemberStatus(invoiceRecord.BilledMemberId, "ICH");

                    var parentBillingMember = _memberManager.GetFinalParentDetails(invoiceRecord.BillingMemberId);
                    var parentBilledMember = _memberManager.GetFinalParentDetails(invoiceRecord.BilledMemberId);

                    if (invoiceRecord != null)
                    {
                        // Update for CMP-409 to check Parent Member
                        if (parentBillingMember != invoiceRecord.BillingMemberId)
                            throw new ISBusinessException(ErrorCodes.CheckforParentMember, invoiceRecord.InvoiceNumber);
                        if (parentBilledMember != invoiceRecord.BilledMemberId)
                            throw new ISBusinessException(ErrorCodes.CheckforParentMember, invoiceRecord.InvoiceNumber);

                        if (invoiceRecord.ResubmissionStatusId == (int)ResubmissionStatus.R)
                            throw new ISBusinessException(ErrorCodes.AlreadyResubmitedInvoice, invoiceRecord.InvoiceNumber);

                        if (invoiceRecord.ResubmissionStatusId == (int)ResubmissionStatus.B)
                            throw new ISBusinessException(ErrorCodes.InvoiceIsBilaterallySettled, invoiceRecord.InvoiceNumber);

                        if (invoiceRecord.ResubmissionStatusId == (int)ResubmissionStatus.C)
                            throw new ISBusinessException(ErrorCodes.AlreadyResubmitedInvoice, invoiceRecord.InvoiceNumber);

                        /* CMP #624: ICH Rewrite-New SMI X 
                        Description: 2.25 IS-WEB Manage Suspended invoices
                        The system should perform actions on SMI X invoice as if it was billed using SMI I.
                        Resubmission should be denied if the Billed Member is still suspended */
                        if (invoiceRecord.SettlementMethodId == (int)SMI.Ich || invoiceRecord.SettlementMethodId == (int)SMI.IchSpecialAgreement)
                        {

                            if ((ichBillingMemberStatus != (int)IchMemberShipStatus.Live) && (ichBillingMemberStatus != (int)IchMemberShipStatus.NotAMember))
                                throw new ISBusinessException(ErrorCodes.BillingMemberSuspended);

                            // Pilot Issue fix.4416
                            // When Settlement method indicator is 'I' , the member can be ich/ach or dual member.
                            // If member is ACH member then ACH membership status verified to check whether the member is suspended.

                            // If member is not a member of ICH, then he will be ACH member.
                            // If member is ACH member and not in live status then exception is thrown.
                            if (ichbilledMemberStatus == (int)IchMemberShipStatus.NotAMember)
                            {
                                if (achbilledMemberStatus != (int)AchMembershipStatus.Live)
                                {
                                    throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                                }
                            }
                            // If member is ICH/dual member then ICH membership status verified to check whether the member is suspended or terminated.
                            // If suspended or terminated then exception is thrown.
                            else if ((ichbilledMemberStatus == (int)IchMemberShipStatus.Suspended) || (ichbilledMemberStatus == (int)IchMemberShipStatus.Terminated))
                            {
                                throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                            }

                            //-------------------------------
                            var isResubmitt = CheckResubmissionInDefaultSuspensionPeriod(invoiceRecord);
                            if (!isResubmitt)
                            {
                                throw new ISBusinessException(ErrorCodes.UnableToResubmit, invoiceRecord.InvoiceNumber);
                            }
                            //-----------------------------------------------
                            var isResubmit = CheckInvoiceSuspensionPeriod(invoiceRecord);
                            if (!isResubmit)
                            {
                                throw new ISBusinessException(ErrorCodes.UnableToResubmit, invoiceRecord.InvoiceNumber);
                            }
                            if (resubmitInIchPreviousPeriod)
                            {
                                invoiceRecord.ResubmissionPeriod = ichPreviousPerid.Period;
                                invoiceRecord.ResubmissionBillingMonth = ichPreviousPerid.Month.ToString().Length < 2 ? int.Parse(ichPreviousPerid.Year + "0" + ichPreviousPerid.Month) : int.Parse(ichPreviousPerid.Year.ToString() + ichPreviousPerid.Month);
                            }
                            else
                            {
                                invoiceRecord.ResubmissionPeriod = currentPeriod.Period;
                                invoiceRecord.ResubmissionBillingMonth = currentPeriod.Month.ToString().Length < 2 ? int.Parse(currentPeriod.Year + "0" + currentPeriod.Month) : int.Parse(currentPeriod.Year.ToString() + currentPeriod.Month);
                            }
                        }

                        if ((invoiceRecord.SettlementMethodId == (int)SMI.Ach) || (invoiceRecord.SettlementMethodId == (int)SMI.AchUsingIataRules))
                        {
                            //For Billing Member
                            if ((achBillingMemberStatus != (int)AchMembershipStatus.Live) && (achBillingMemberStatus != (int)AchMembershipStatus.NotAMember))
                                throw new ISBusinessException(ErrorCodes.BillingMemberSuspended);

                            // For Billed Member
                            // If member is not a member of ACH, then he will be ICH member.
                            // If member is ICH member and not in live status then exception is thrown.
                            if (achbilledMemberStatus == (int)IchMemberShipStatus.NotAMember)
                            {
                                if (ichbilledMemberStatus != (int)AchMembershipStatus.Live)
                                {
                                    throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                                }
                            }
                            // If member is ACH/dual member then ACH membership status verified to check whether the member is suspended or terminated.
                            // If suspended or terminated then exception is thrown.
                            else if ((achbilledMemberStatus == (int)AchMembershipStatus.Suspended) || (achbilledMemberStatus == (int)AchMembershipStatus.Terminated))
                            {
                                throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                            }

                            //if ((achbilledMemberStatus != (int)AchMembershipStatus.Live))
                            //{
                            //    if ((invoiceRecord.SettlementMethodId == (int)SMI.AchUsingIataRules) && (ichbilledMemberStatus != (int)IchMemberShipStatus.Live))
                            //    {
                            //        throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                            //    }
                            //    throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                            //}
                            //-------------------------------
                            var isResubmitt = CheckResubmissionInDefaultSuspensionPeriod(invoiceRecord);
                            if (!isResubmitt)
                            {
                                throw new ISBusinessException(ErrorCodes.UnableToResubmit, invoiceRecord.InvoiceNumber);
                            }
                            //-----------------------------------------------

                            var isResubmit = CheckInvoiceSuspensionPeriod(invoiceRecord);
                            if (!isResubmit)
                            {
                                throw new ISBusinessException(ErrorCodes.UnableToResubmit, invoiceRecord.InvoiceNumber);
                            }
                            if (resubmitInAchPreviousPeriod)
                            {
                                invoiceRecord.ResubmissionPeriod = achPreviousPeriod.Period;
                                invoiceRecord.ResubmissionBillingMonth = ichPreviousPerid.Month.ToString().Length < 2 ? int.Parse(ichPreviousPerid.Year + "0" + ichPreviousPerid.Month) : int.Parse(ichPreviousPerid.Year.ToString() + ichPreviousPerid.Month);
                            }
                            else
                            {
                                invoiceRecord.ResubmissionPeriod = currentPeriod.Period;
                                invoiceRecord.ResubmissionBillingMonth = currentPeriod.Month.ToString().Length < 2 ? int.Parse(currentPeriod.Year + "0" + currentPeriod.Month) : int.Parse(currentPeriod.Year.ToString() + currentPeriod.Month);
                            }


                        }
                        invoiceRecord.ResubmissionStatusId = (int)ResubmissionStatus.R;
                        if (invoiceRecord.ClearingHouse == "I")
                        {
                            invoiceRecord.IchSettlementStatus = null;
                        }
                        if (invoiceRecord.ClearingHouse == "A")
                        {
                            invoiceRecord.RecapsheetProcessStatus = null;
                        }
                        if (invoiceRecord.ClearingHouse == "B")
                        {
                            invoiceRecord.IchSettlementStatus = null;
                            invoiceRecord.RecapsheetProcessStatus = null;
                        }
                        InvoiceBaseRepository.Update(invoiceRecord);
                        UnitOfWork.CommitDefault();

                    }

                }

            }
            catch (ISBusinessException ex)
            {
                if (ex.ErrorCode == ErrorCodes.CheckforParentMember)
                {
                    throw new ISBusinessException(ErrorCodes.CheckforParentMember, ex.Message);
                }
                if (ex.ErrorCode == ErrorCodes.InvoiceIsBilaterallySettled)
                {
                    throw new ISBusinessException(ErrorCodes.InvoiceIsBilaterallySettled, ex.Message);
                }
                if (ex.ErrorCode == ErrorCodes.BillingMemberSuspended)
                {
                    throw new ISBusinessException(ErrorCodes.BillingMemberSuspended);
                }
                if (ex.ErrorCode == ErrorCodes.BilledMemberSuspended)
                {
                    throw new ISBusinessException(ErrorCodes.BilledMemberSuspended);
                }
                if (ex.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice)
                {
                    throw new ISBusinessException(ErrorCodes.AlreadyResubmitedInvoice, ex.Message);
                }
                if (ex.ErrorCode == ErrorCodes.UnableToResubmit)
                {
                    throw new ISBusinessException(ErrorCodes.UnableToResubmit, ex.Message);
                }
                return false;
            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceIdList"></param>
        /// <returns></returns>
        public bool MarkInvoicesAsBilaterallySettled(List<string> invoiceIdList)
        {
            try
            {

                foreach (var item in invoiceIdList)
                {
                    var invoiceId = new Guid(item);
                    var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
                    if (invoiceRecord != null)
                    {
                        if (invoiceRecord.ResubmissionStatusId == (int)ResubmissionStatus.R)
                            throw new ISBusinessException(ErrorCodes.AlreadyResubmitedInvoice);
                        if (invoiceRecord.ResubmissionStatusId == (int)ResubmissionStatus.B)
                            throw new ISBusinessException(ErrorCodes.AlreadyBilaterallySettledInvoice);

                        if (invoiceRecord.ResubmissionStatusId == (int)ResubmissionStatus.C)
                            throw new ISBusinessException(ErrorCodes.AlreadyResubmitedInvoice);

                        invoiceRecord.ResubmissionStatusId = (int)ResubmissionStatus.B;

                        InvoiceBaseRepository.Update(invoiceRecord);
                        UnitOfWork.CommitDefault();

                    }

                }
            }
            catch (ISBusinessException ex)
            {
                if (ex.ErrorCode == ErrorCodes.AlreadyBilaterallySettledInvoice)
                {
                    throw new ISBusinessException(ErrorCodes.AlreadyBilaterallySettledInvoice);
                }
                if (ex.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice)
                {
                    throw new ISBusinessException(ErrorCodes.AlreadyResubmitedInvoice);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function to Set the ResubmissionStatus property to Deactivate 
        /// 
        /// </summary>
        /// <param name="susPendedInvoices"></param>
        private List<SuspendedInvoiceDetails> DeactivateDefaultSuspensionInvoices(List<SuspendedInvoiceDetails> susPendedInvoices)
        {

            // Loop through each invoice and set  resubmission status to 'Deactivate' 
            foreach (var invoice in susPendedInvoices)
            {
                // Get the latest updated SuspendedDate from the MemberStatus table  against Billed Member
                DateTime dtSuspenisonDate = GetSuspensionDate(invoice.BilledMemberId);

                // Check The invoice Date with the Suspension Date
                if (invoice.InvoiceDate >= dtSuspenisonDate)
                {
                    // Set the ResubmissionStatus property to Deactivate state
                    invoice.ResubmissionStatusId = 4;
                }
            }

            return susPendedInvoices;
        }

        /// <summary>
        /// Function to retrieve SuspendedDate
        /// Get latest updated SuspendedDate from the MemberStatus table  
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private DateTime GetSuspensionDate(int memberId)
        {
            DateTime dtSuspendedDate = MemberStatusRepository.Get(m => m.MemberId == memberId).Select(m => m.StatusChangeDate).Max().Date;
            return dtSuspendedDate;
        }
        //-----------------------------------------------------------------------

        /// <summary>
        /// Below method check invoices will not resubmitted  between suspension period and default suspension period
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        private bool CheckResubmissionInDefaultSuspensionPeriod(InvoiceBase invoice)
        {
            var membertype = "";
            if (invoice.SettlementMethodId == (int)SMI.Ich)
            {
                membertype = "ICH";
            }
            else if (invoice.SettlementMethodId == (int)SMI.Ach)
            {
                membertype = "ACH";
            }
            else if (invoice.SettlementMethodId == (int)SMI.AchUsingIataRules)
            {
                membertype = "MEM";
            }
            if (invoice.BillingMemberId > 0)
            {

                var statusDetail = MemberStatusRepository.Get(ichConf => ichConf.MemberId == invoice.BillingMemberId && ichConf.MembershipStatusId == 2 && ichConf.MemberType == membertype).OrderByDescending(ichConf => ichConf.Id).Take(2);
                MemberStatusDetails previousStatuss;
                if (statusDetail.Count() > 1)
                {
                    if (invoice.SuspensionMonth != 0)
                    {
                        var suspensionPeriod = invoice.SuspensionMonth.ToString().Substring(0, 4) + "/" +
                                               invoice.SuspensionMonth.ToString().Substring(4, 2) + "/" +
                                               invoice.SuspensionPeriod;
                        var suspensionDate = Convert.ToDateTime(suspensionPeriod);
                        previousStatuss = statusDetail.Skip(1).FirstOrDefault();
                        var StatusChangedDate = Convert.ToDateTime(previousStatuss.StatusChangeDate);
                        var billingPeriod = invoice.BillingYear + "/" + invoice.BillingMonth + "/" +
                                            invoice.BillingPeriod;
                        var billingDate = Convert.ToDateTime(billingPeriod);
                        if (billingDate >= suspensionDate && billingDate < StatusChangedDate)
                        {
                            return false;
                        }
                    }
                }
            }
            if (invoice.BilledMemberId > 0)
            {
                var statusDetail = MemberStatusRepository.Get(ichConf => ichConf.MemberId == invoice.BilledMemberId && ichConf.MembershipStatusId == 2 && ichConf.MemberType == membertype).OrderByDescending(ichConf => ichConf.Id).Take(2);
                MemberStatusDetails previousStatuss;

                if (statusDetail.Count() > 1)
                {
                    if (invoice.SuspensionMonth != 0)
                    {
                        var suspensionPeriod = invoice.SuspensionMonth.ToString().Substring(0, 4) + "/" +
                                               invoice.SuspensionMonth.ToString().Substring(4, 2) + "/" +
                                               invoice.SuspensionPeriod;
                        var suspensionDate = Convert.ToDateTime(suspensionPeriod);
                        previousStatuss = statusDetail.Skip(1).FirstOrDefault();
                        var StatusChangedDate = Convert.ToDateTime(previousStatuss.StatusChangeDate);
                        var billingPeriod = invoice.BillingYear + "/" + invoice.BillingMonth + "/" +
                                            invoice.BillingPeriod;
                        var billingDate = Convert.ToDateTime(billingPeriod);
                        if (billingDate >= suspensionDate && billingDate < StatusChangedDate)
                        {
                            return false;
                        }
                    }

                }
            }
            return true;

        }
        //------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        private bool CheckInvoiceSuspensionPeriod(InvoiceBase invoice)
        {
            //var dateInfo = new DateTimeFormatInfo { ShortDatePattern = FormatConstants.DateFormat };
            var billingPeriod = invoice.BillingYear + "/" + invoice.BillingMonth + "/" + invoice.BillingPeriod;
            if (invoice.SuspensionMonth != 0)
            {
                var suspensionPeriod = invoice.SuspensionMonth.ToString().Substring(0, 4) + "/" +
                                       invoice.SuspensionMonth.ToString().Substring(4, 2) + "/" + invoice.SuspensionPeriod;
                var billingDate = Convert.ToDateTime(billingPeriod);
                var suspensionDate = Convert.ToDateTime(suspensionPeriod);

                if (billingDate < suspensionDate)
                    return false;
                return true;
            }
            return true;

        }
        public bool UndoBilateral(List<string> invoiceIdList)
        {
            try
            {
                foreach (var item in invoiceIdList)
                {
                    var invoiceId = new Guid(item);
                    var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
                    if (invoiceRecord != null)
                    {
                        invoiceRecord.ResubmissionStatusId = null;
                        InvoiceBaseRepository.Update(invoiceRecord);
                        UnitOfWork.CommitDefault();

                    }

                }
            }
            catch (ISBusinessException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceIdList"></param>
        /// <returns></returns>
        public bool[] CheckIfLateSubmissionWindowOpen(List<string> invoiceIdList)
        {
            var isLateAcceptanceAllowed = new bool[2];
            isLateAcceptanceAllowed[0] = false;
            isLateAcceptanceAllowed[1] = false;
            foreach (var item in invoiceIdList)
            {
                var invoiceId = new Guid(item);
                var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
                if (invoiceRecord != null)
                {
                    if (invoiceRecord.SettlementMethodId == (int)SMI.Ich)
                    {
                        isLateAcceptanceAllowed[0] = _calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ich,
                                                                                                 _calenderManager.
                                                                                                   GetLastClosedBillingPeriod(
                                                                                                     ClearingHouse.Ich));
                    }
                    if (invoiceRecord.SettlementMethodId == (int)SMI.Ach)
                    {
                        isLateAcceptanceAllowed[1] = _calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ach,
                                                                                                 _calenderManager.
                                                                                                   GetLastClosedBillingPeriod(
                                                                                                     ClearingHouse.Ach));
                    }

                }

            }
            return isLateAcceptanceAllowed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        private int GetMemberStatus(int memberId, string memberType)
        {
            //var memberDetailsList =
            //  MemberStatusRepository.Get(member => member.MemberId == memberId && member.MemberType.Equals(memberType));
            //if(memberDetailsList.Count()!=0)
            var memberDetails = MemberStatusRepository.Get(member => member.MemberId == memberId && member.MemberType.Equals(memberType)).OrderBy(member => member.LastUpdatedOn).ToList();
            if (memberDetails.Count > 0)
                return memberDetails[memberDetails.Count - 1].MembershipStatusId;
            return (int)IchMemberShipStatus.NotAMember;
        }


        /// <summary>
        /// Gets the member suspended invoices list.
        /// </summary>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="fromClearanceYear">From clearance year.</param>
        /// <param name="fromClearanceMonth">From clearance month.</param>
        /// <param name="fromClearancePeriod">From clearance period.</param>
        /// <param name="toClearanceYear">To clearance year.</param>
        /// <param name="toClearanceMonth">To clearance month.</param>
        /// <param name="toClearancePeriod">To clearance period.</param>
        /// <param name="settlementMethodIndicatorId">The settlement method indicator id.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="suspendedEntityCode">The suspended entity code.</param>
        /// <returns></returns>
        public List<MemberSuspendedInvoices> GetMemberSuspendedInvoicesList(int billingMemberId, int fromClearanceYear, int fromClearanceMonth, int fromClearancePeriod, int toClearanceYear, int toClearanceMonth, int toClearancePeriod, int settlementMethodIndicatorId, int billingCategoryId, int suspendedEntityCode, int iataMemberId, int achMemberId)
        {
            var suspendedInvoiceList = SuspendedInvoiceDetailsRepository.GetMemberSuspendedInvoicesList(billingMemberId, fromClearanceYear, fromClearanceMonth, fromClearancePeriod, toClearanceYear, toClearanceMonth, toClearancePeriod, settlementMethodIndicatorId, billingCategoryId, suspendedEntityCode, iataMemberId, achMemberId);

            return suspendedInvoiceList.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public int? GetInvoiceResubmissionStatus(Guid invoiceId)
        {
            try
            {
                var invoiceRecord = InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
                if (invoiceRecord != null)
                {
                    return invoiceRecord.ResubmissionStatusId;
                }

                return 0;

            }
            catch (Exception)
            {

                return 0;
            }

        }
    }
}
