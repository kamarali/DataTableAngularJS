using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Xml;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Pax;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;
using NVelocity;

namespace Iata.IS.Business.MemberProfile.Impl
{
    public class ICHUpdateHandler : IICHUpdateHandler
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public IFutureUpdatesManager FutureUpdatesManager { get; set; }
        public IMemberManager MemberManager { get; set; }
        public IMemberRepository MemberRepository { get; set; }
        public IRepository<IchConfiguration> IchRepository { get; set; }
        public IRepository<AchConfiguration> AchRepository { get; set; }
        public IRepository<EmailTemplate> EmailSettingsRepository { get; set; }
        public IFutureUpdatesRepository FutureUpdateRepository { get; set; }
        public IBlockingRulesRepository BlockingRulesRepository { get; set; }
        public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }
        public IICHXmlHandler ICHXmlHandler { get; set; }
        public IReferenceManager ReferenceManager { get; set; }
        public IMiscCodeRepository MiscCodeRepository { get; set; }
        private const string memberProfileUpdateFolder = @"\App_Data\SchemaFiles\MemberProfileUpdate.xsd";
        XmlDocument _xmlMemberProfileUpdate;
        ParentMemberDetail parentMemberCurrentValue;
        ParentMemberDetail parentMemberFutureValue;

        public string GenerateXMLforICHUpdates(int memberId)
        {
            MemberProfileUpdate memICHUpdate;

            List<FutureUpdates> futureUpdatesList;
            List<IchConfiguration> currentSponsoredList;
            List<IchConfiguration> currentAggregatedList;

            parentMemberCurrentValue = new ParentMemberDetail();
            parentMemberFutureValue = new ParentMemberDetail();
            var currentSponsors = new List<Member>();
            var currentAggregators = new List<Member>();
            var futureSponsors = new List<Member>();
            var futureAggregators = new List<Member>();
            var contactList = new List<Contact>();
            string memberProfileUpdateXml = null;

            _xmlMemberProfileUpdate = new XmlDocument();

            // Remove Blank nodes
            _logger.Info("Before resolving member manager reference");

            // Get an object of the member manager
            MemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

            _logger.Info("After resolving member manager reference");

            try
            {
                BlockingRulesRepository = Ioc.Resolve<IBlockingRulesRepository>();
                IchRepository = Ioc.Resolve<IRepository<IchConfiguration>>();
                MemberRepository = Ioc.Resolve<IMemberRepository>();
                ReferenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                MiscCodeRepository = Ioc.Resolve<IMiscCodeRepository>(typeof(IMiscCodeRepository));

                ICHXmlHandler = new ICHXmlHandler();
                _logger.Info(string.Format("BlockingRulesRepository instance is: [{0}]", BlockingRulesRepository != null ? "NOT NULL" : "NULL"));
                _logger.Info(string.Format("IchRepository instance is: [{0}]", IchRepository != null ? "NOT NULL" : "NULL"));
                _logger.Info(string.Format("MemberRepository instance is: [{0}]", MemberRepository != null ? "NOT NULL" : "NULL"));

                // Get ICH config
                var ichConfiguration = MemberManager.GetIchConfig(memberId, true);
                _logger.Info("Retrieved ICH configuration for member");

                //CMP #625: New Fields in ICH Member Profile Update XML
                var technicalConfiguration = MemberManager.GetTechnicalConfig(memberId);
                _logger.Info("Retrieved Technical configuration for member");

                //SCP# 305117 - Member Profile update - ACH Members acquisition to ICH
                //Desc: Member profile XML should be generated as per member ICH configuration only for 
                //valid Membership Statuses i.e. - 'Live', Suspended' and 'Terminated'.
                //Hence added a code to bypass members having membership status as 'NotAMember'.

                //CMP-689-Flexible CH Activation Options
                // before this CMP only Members having ‘ICH Membership Status’ as "Live", "Suspended" or "Terminated" are included in the update XML
                // With this CMP, the system should also consider the following:
                //  	Members whose future value of ‘ICH Membership Status’ = “Live” (and where the current value = “Not a Member” or "Terminated")
                _logger.Info("checking Members future value of ‘ICH Membership Status’ = 'Live' (and where the current value = 'Not a Member' or 'Terminated')");
                var isFutureUpdateLive = (ichConfiguration != null &&
                                             ichConfiguration.IchMemberShipStatusIdFutureValue == (int)IchMemberShipStatus.Live &&
                                             (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.NotAMember ||
                                              ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Terminated))
                                                ? true
                                                : false;
                _logger.InfoFormat("isFutureUpdateIsLive : {0}",isFutureUpdateLive);

                if (ichConfiguration != null && (ichConfiguration.IchMemberShipStatusId != (int)IchMemberShipStatus.NotAMember || isFutureUpdateLive))
                {
                    _logger.Info("Before retrieving ICH member status list");
                    ichConfiguration = MemberManager.GetIchMemberStatusList(ichConfiguration);
                    _logger.Info("After retrieving ICH member status list");

                    memICHUpdate = new MemberProfileUpdate();

                    _logger.Info(string.Format("FutureUpdateRepository instance is: [{0}]",
                                               FutureUpdateRepository != null ? "NOT NULL" : "NULL"));
                    _logger.Info(string.Format("ichConfiguration instance is: [{0}]",
                                               ichConfiguration != null ? "NOT NULL" : "NULL"));
                    _logger.Info(string.Format("ichConfiguration.MemberId is: [{0}]", ichConfiguration.MemberId));

                    if (FutureUpdateRepository == null)
                    {
                        FutureUpdateRepository = Ioc.Resolve<IFutureUpdatesRepository>();
                    }
                    _logger.Info(string.Format("FutureUpdateRepository instance after Ioc.Resolve is: [{0}]",
                                               FutureUpdateRepository != null ? "NOT NULL" : "NULL"));
                    _logger.Info(string.Format("FutureUpdatesManager instance is: [{0}]",
                                               FutureUpdatesManager != null ? "NOT NULL" : "NULL"));

                    if (FutureUpdatesManager == null)
                    {
                        FutureUpdatesManager = Ioc.Resolve<IFutureUpdatesManager>();
                    }
                    _logger.Info(string.Format("FutureUpdatesManager instance after Ioc.Resolve is: [{0}]",
                                               FutureUpdatesManager != null ? "NOT NULL" : "NULL"));

                    memICHUpdate.MemberStatusCurrentValue = ReferenceManager.GetDisplayValue(
                        MiscGroups.IchMemberStatus, ichConfiguration.IchMemberShipStatusId);
                    _logger.Info("Member status current value retrieved successfully");

                    // Get zone name from database
                    memICHUpdate.ZoneCurrentValue = ReferenceManager.GetIchZoneDisplayValue(ichConfiguration.IchZoneId);
                    memICHUpdate.CategoryCurrentValue = ReferenceManager.GetDisplayValue(MiscGroups.IchMemberCategory,
                                                                                         ichConfiguration.IchCategoryId);

                    memICHUpdate.NumericMemberCode = ichConfiguration.Member.MemberCodeNumeric;
                    memICHUpdate.AlphaMemberCode = ichConfiguration.Member.MemberCodeAlpha;

                    // Set details about ICH Tab data
                    // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV
                    memICHUpdate.MemberNameCurrentValue = ichConfiguration.Member.LegalName;

                    if (!string.IsNullOrEmpty(ichConfiguration.MemberNameFutureValue) && !string.IsNullOrEmpty(ichConfiguration.MemberNameChangePeriodFrom))
                    {
                      memICHUpdate.MemberNameFutureValue = ichConfiguration.MemberNameFutureValue;
                      memICHUpdate.MemberNameChangePeriodFrom = Convert.ToDateTime(ichConfiguration.MemberNameChangePeriodFrom).ToString("yyMMdd");
                    }


                    memICHUpdate.Comments = ichConfiguration.Member.IsOpsComments;
                    memICHUpdate.EarlyCallDay = ichConfiguration.IsEarlyCallDay;
                    memICHUpdate.ICHComments = ichConfiguration.IchOpsComments;
                    memICHUpdate.ICHWebReportOptions = ichConfiguration.IchWebReportOptionsId;
                    memICHUpdate.CanSubmitPAXWebF12FilesCurrentValue = ichConfiguration.CanSubmitPaxInF12Files;
                    memICHUpdate.CanSubmitCGOWebF12FilesCurrentValue = ichConfiguration.CanSubmitCargoInF12Files;
                    memICHUpdate.CanSubmitMISCWebF12FilesCurrentValue = ichConfiguration.CanSubmitMiscInF12Files;
                    memICHUpdate.CanSubmitUATPWebF12FilesCurrentValue = ichConfiguration.CanSubmitUatpinF12Files;
                    memICHUpdate.AggregatorCurrentValue = ichConfiguration.IsAggregator;
                    memICHUpdate.AggregatedTypeCurrentValue = ichConfiguration.AggregatedTypeId.HasValue
                                                                  ? ichConfiguration.AggregatedTypeId
                                                                  : null;

                    //CMP #625: New Fields in ICH Member Profile Update XML.
                    memICHUpdate.SISMemberID = ichConfiguration.MemberId;
                    memICHUpdate.IInetAccountIds = GetListOfIInetAccountId(ichConfiguration.IchAccountId, technicalConfiguration);

                    _logger.Info(string.Format("Current ICH configuration values set successfully"));
                    //UATPInvoicehandledByATCAN


                    memICHUpdate.UATPInvoiceHandledByATCANCurrentValue =
                        ichConfiguration.Member.UatpInvoiceHandledbyAtcan;
                    // Get future update records corresponding to the UATPInvoiceHandledbyATCAN field
                    _logger.Info("Before retrieving future values for UATP configuration");
                    futureUpdatesList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails,
                                                                                     memberId,
                                                                                     "IS_UATP_INV_HANDLED_BY_ATCAN",
                                                                                     null);
                    _logger.Info("After retrieving future values for UATP configuration");
                    if ((futureUpdatesList != null) && (futureUpdatesList.Count == 1))
                    {
                        _logger.Info(string.Format("1 future update for field IS_UATP_INV_HANDLED_BY_ATCAN is set"));
                        memICHUpdate.UATPInvoiceHandledByATCANFutureValue = bool.Parse(futureUpdatesList[0].NewVAlue);
                        memICHUpdate.UATPInvoiceHandledByATCANPeriodFrom = futureUpdatesList[0].ChangeEffectivePeriod !=
                                                                           null
                                                                               ? futureUpdatesList[0].
                                                                                     ChangeEffectivePeriod.Value.
                                                                                     ToString("yyMMdd")
                                                                               : string.Empty;
                    }
                    else
                    {
                        memICHUpdate.UATPInvoiceHandledByATCANFutureValue = null;
                        _logger.Info(
                            string.Format(
                                "More than 1 future updates or no future updates set for field IS_UATP_INV_HANDLED_BY_ATCAN are set"));
                    }

                    // Indicates current status is suspended
                    if (ichConfiguration.IchMemberShipStatusId == (int) IchMemberShipStatus.Suspended)
                    {
                        memICHUpdate.SuspensionPeriodFrom = ichConfiguration.StatusChangedDate != null
                                                                ? ichConfiguration.StatusChangedDate.Value.ToString(
                                                                    "yyMMdd")
                                                                : string.Empty;
                    }

                    // Indicates member status has changed to live from suspended
                    memICHUpdate.ReinstatementPeriodFrom = ichConfiguration.ReinstatementPeriod != null
                                                               ? ichConfiguration.ReinstatementPeriod.Value.ToString(
                                                                   "yyMMdd")
                                                               : string.Empty;
                    memICHUpdate.MemberStatusFutureValue = ichConfiguration.ReinstatementPeriod != null
                                                               ? ReferenceManager.GetDisplayValue(
                                                                   MiscGroups.IchMemberStatus,
                                                                   (int) IchMemberShipStatus.Live)
                                                               : string.Empty;

                    // Reinstatement period is specified then specify member status future value as Live
                    if ((ichConfiguration.ReinstatementPeriod != null))
                    {
                        memICHUpdate.EntryDate = ichConfiguration.EntryDate != null
                                                     ? ichConfiguration.EntryDate.Value.ToString("dd-MMM-yyyy").ToUpper()
                                                     : string.Empty;
                    }

                    // Indicates current status is live and previous status was not suspended
                    if ((ichConfiguration.IchMemberShipStatusId == (int) IchMemberShipStatus.Live) &&
                        (ichConfiguration.ReinstatementPeriod == null))
                    {
                        memICHUpdate.EntryDate = ichConfiguration.EntryDate != null
                                                     ? ichConfiguration.EntryDate.Value.ToString("dd-MMM-yyyy").ToUpper()
                                                     : string.Empty;
                    }

                    // Indicates current status is Terminated
                    if (ichConfiguration.IchMemberShipStatusId == (int) IchMemberShipStatus.Terminated)
                    {
                        memICHUpdate.TerminationDate = ichConfiguration.TerminationDate != null
                                                           ? ichConfiguration.TerminationDate.Value.ToString(
                                                               "dd-MMM-yyyy").ToUpper()
                                                           : string.Empty;
                    }

                    if (ichConfiguration.IchZoneIdFutureValue != null)
                    {
                        memICHUpdate.ZoneChangePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.IchZoneIdFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.ZoneFutureValue = ichConfiguration.IchZoneIdFutureDisplayValue;
                    }

                    if (ichConfiguration.IchCategoryIdFutureValue != null)
                    {
                        memICHUpdate.CategoryChangePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.IchCategoryIdFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.CategoryFutureValue = ichConfiguration.IchCategoryIdFutureDisplayValue;
                    }
                    
                    if (ichConfiguration.IchMemberShipStatusIdFutureValue == (int) IchMemberShipStatus.Terminated)
                    {
                        memICHUpdate.TerminationPeriodFrom =
                            Convert.ToDateTime(ichConfiguration.IchMemberShipStatusIdFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.MemberStatusFutureValue = ichConfiguration.IchMemberShipStatusIdFutureDisplayValue;
                    }

                    //CMP-689-Flexible CH Activation Options
                    if (isFutureUpdateLive)
                    {
                      memICHUpdate.MemberStatusFutureValue = ichConfiguration.IchMemberShipStatusIdFutureDisplayValue;
                      memICHUpdate.MemberStatusPeriodFrom =
                           Convert.ToDateTime(ichConfiguration.IchMemberShipStatusIdFuturePeriod).ToString("yyMMdd");
                    }

                    if (ichConfiguration.CanSubmitPaxInF12FilesFutureValue != null)
                    {
                        memICHUpdate.CanSubmitPAXChangePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.CanSubmitPaxInF12FilesFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.CanSubmitPAXWebF12FilesFutureValue =
                            ichConfiguration.CanSubmitPaxInF12FilesFutureValue.HasValue
                                ? ichConfiguration.CanSubmitPaxInF12FilesFutureValue
                                : null;
                    }

                    if (ichConfiguration.CanSubmitCargoInF12FilesFutureValue != null)
                    {
                        memICHUpdate.CanSubmitCGOChangePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.CanSubmitCargoInF12FilesFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.CanSubmitCGOWebF12FilesFutureValue =
                            ichConfiguration.CanSubmitCargoInF12FilesFutureValue.HasValue
                                ? ichConfiguration.CanSubmitCargoInF12FilesFutureValue
                                : null;
                    }

                    if (ichConfiguration.CanSubmitMiscInF12FilesFutureValue != null)
                    {
                        memICHUpdate.CanSubmitMISCChangePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.CanSubmitMiscInF12FilesFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.CanSubmitMISCWebF12FilesFutureValue =
                            ichConfiguration.CanSubmitMiscInF12FilesFutureValue.HasValue
                                ? ichConfiguration.CanSubmitMiscInF12FilesFutureValue
                                : null;
                    }

                    if (ichConfiguration.CanSubmitUatpinF12FilesFutureValue != null)
                    {
                        memICHUpdate.CanSubmitUATPChangePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.CanSubmitUatpinF12FilesFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.CanSubmitUATPWebF12FilesFutureValue =
                            ichConfiguration.CanSubmitUatpinF12FilesFutureValue.HasValue
                                ? ichConfiguration.CanSubmitUatpinF12FilesFutureValue
                                : null;
                    }

                    if (ichConfiguration.AggregatedTypeIdFutureValue != null)
                    {
                        memICHUpdate.AggregatedTypePeriodFrom =
                            Convert.ToDateTime(ichConfiguration.AggregatedTypeIdFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.AggregatedTypeFutureValue = ichConfiguration.AggregatedTypeIdFutureValue;
                    }

                    if (ichConfiguration.IsAggregatorFutureValue != null)
                    {
                        memICHUpdate.AggregatorPeriodFrom =
                            Convert.ToDateTime(ichConfiguration.IsAggregatorFuturePeriod).ToString("yyMMdd");
                        memICHUpdate.AggregatorFutureValue = ichConfiguration.IsAggregatorFutureValue.HasValue
                                                                 ? ichConfiguration.IsAggregatorFutureValue
                                                                 : null;
                    }

                    _logger.Info("Getting current aggregators");

                    /*
                     Issue: SCP43358 - Member profile XML from SIS to ICH - Aggregator Function
                     * When we add/delete members in aggregate and sponser member list then a XML send to ICH. 
                     * Defect: In XML we send only, those members who are add/delete on defined period.
                     * 
                     Resolution: In XML we will send those members will be exist on defined period.
                     * this is done for aggregators as well as sponser members.
                    */

                    // Get all current aggregated members
                    currentAggregatedList = IchRepository.Get(ich => ich.AggregatedById == memberId).ToList();

                    if ((currentAggregatedList != null) && (currentAggregatedList.Count > 0))
                    {
                        foreach (var aggregator in currentAggregatedList)
                        {
                            if (aggregator != null)
                            {
                                var member = MemberRepository.Single(mem => mem.Id == aggregator.MemberId);

                                if (member != null)
                                {
                                    member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                    currentAggregators.Add(member);
                                }
                            }
                        }

                        // If aggregators are set for the member
                        if ((currentAggregators != null) && (currentAggregators.Count > 0))
                        {
                            memICHUpdate.AggregatedMembersCurrentValue = currentAggregators;
                        }
                    }

                    _logger.Info("Getting future aggregators");

                    // Get members those will be delete in future.
                    var aggregatedDeletedMembers = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0,
                                                                                                "AGGREGATED_BY",
                                                                                                memberId.ToString(),
                                                                                                null,
                                                                                                Convert.ToString(
                                                                                                    memberId));

                    if ((aggregatedDeletedMembers != null && aggregatedDeletedMembers.Count > 0) &&
                        (currentAggregators != null && currentAggregators.Count > 0))
                    {
                        var aggregatedMember = aggregatedDeletedMembers.First();

                        if (string.IsNullOrEmpty(memICHUpdate.AggregatedMembersPeriodFrom))
                        {
                            memICHUpdate.AggregatedMembersPeriodFrom = aggregatedMember.ChangeEffectivePeriod !=
                                                                       null
                                                                           ? aggregatedMember.ChangeEffectivePeriod.
                                                                                 Value.ToString
                                                                                 ("yyMMdd")
                                                                           : string.Empty;
                        }


                        //add members in future aggregator list exclude mem_future_update members
                        foreach (var aggregator in currentAggregatedList)
                        {
                            //select only delete members
                            if (
                                aggregatedDeletedMembers.Where(
                                    agg => agg.MemberId == aggregator.MemberId && agg.ActionType == ActionType.Delete).
                                    Count() == 0)
                            {
                                var member = MemberRepository.Single(mem => mem.Id == aggregator.MemberId);
                                if (member != null)
                                {
                                    member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                    futureAggregators.Add(member);
                                }
                            }
                        }
                    }


                    // Get members those will be add in future.
                    var aggregatedAddedMembers = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0,
                                                                                              "AGGREGATED_BY",
                                                                                              memberId.ToString(), null);

                    if ((aggregatedAddedMembers != null) && (aggregatedAddedMembers.Count > 0))
                    {
                        //if delete member list doesn't contains members then add current members with newly added members.
                        if (currentAggregators.Count > 0 && aggregatedDeletedMembers.Count == 0)
                        {
                            foreach (var aggregator in currentAggregatedList)
                            {
                                var member = MemberRepository.Single(mem => mem.Id == aggregator.MemberId);
                                if (member != null)
                                {
                                    member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                    futureAggregators.Add(member);
                                }
                            }
                        }


                        //chose only member those will be add in future period
                        foreach (var aggregatedMember in aggregatedAddedMembers.Where(agg=>agg.ActionType == ActionType.Update || agg.ActionType == ActionType.Create))
                        {
                            if (string.IsNullOrEmpty(memICHUpdate.AggregatedMembersPeriodFrom))
                            {
                                memICHUpdate.AggregatedMembersPeriodFrom = aggregatedMember.ChangeEffectivePeriod !=
                                                                           null
                                                                               ? aggregatedMember.ChangeEffectivePeriod.
                                                                                     Value.
                                                                                     ToString("yyMMdd")
                                                                               : string.Empty;
                            }

                            var member = MemberRepository.Single(mem => mem.Id == aggregatedMember.MemberId);
                            if (member != null)
                            {
                                member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                futureAggregators.Add(member);
                            }
                        }
                    }
                
                    //Add future aggregator list to IchConfiguration data to be sent
                    if ((futureAggregators != null) && (futureAggregators.Count > 0))
                    {
                        memICHUpdate.AggregatedMembersFutureValue = futureAggregators;
                    }

                    _logger.Info("Getting current sponsors");

                    // Get list of sponsored members
                    currentSponsoredList = IchRepository.Get(ich => ich.SponsoredById == memberId).ToList();
                    if ((currentSponsoredList != null) && (currentSponsoredList.Count > 0))
                    {
                        foreach (var sponsor in currentSponsoredList)
                        {
                            if (sponsor != null)
                            {
                                var member = MemberRepository.Single(mem => mem.Id == sponsor.MemberId);

                                if (member != null)
                                {
                                    member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                    currentSponsors.Add(member);
                                }
                            }
                        }
                    }

                    if ((currentSponsors != null) && (currentSponsors.Count > 0))
                    {
                        memICHUpdate.SponsoredMembersCurrentValue = currentSponsors;
                    }
                    _logger.Info("Getting future sponsors");

                    // Get future aggregated members for delete operation
                    var sponsoredDeletedMembers = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "SPONSORED_BY", memberId.ToString(), null, Convert.ToString(memberId));

                    // We need to add future sponsored members to current sponsored members
                    if ((sponsoredDeletedMembers != null && sponsoredDeletedMembers.Count > 0) && (currentSponsors != null && currentSponsors.Count > 0))
                    {

                            var sponsoredMember = sponsoredDeletedMembers.First();

                            if (string.IsNullOrEmpty(memICHUpdate.SponsoredMembersPeriodFrom))
                            {
                                memICHUpdate.SponsoredMembersPeriodFrom = sponsoredMember.ChangeEffectivePeriod != null
                                                                              ? sponsoredMember.ChangeEffectivePeriod.
                                                                                    Value.ToString
                                                                                    ("yyMMdd")
                                                                              : string.Empty;
                            }
                    
                        foreach (var sponsor in currentSponsoredList)
                        {
                            //select only those member, which will be delete in future period
                            if (sponsoredDeletedMembers.Where(spon => spon.MemberId == sponsor.MemberId && spon.ActionType == ActionType.Delete).Count() == 0)
                            {
                                var member = MemberRepository.Single(mem => mem.Id == sponsor.MemberId);
                                if (member != null)
                                {
                                    member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                    futureSponsors.Add(member);
                                }
                            }
                        }
                    }


                    // Get future sponsor member list those will be add in future period
                    var sponsoredAddedMembers = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "SPONSORED_BY", memberId.ToString(), null);

                    if ((sponsoredAddedMembers != null) && (sponsoredAddedMembers.Count > 0))
                    {
                        //if delete member list doesn't contains members then add current members.
                        if (currentSponsoredList.Count > 0 && sponsoredDeletedMembers.Count == 0)
                        {
                            foreach (var sponsore in currentSponsoredList)
                            {
                                var member = MemberRepository.Single(mem => mem.Id == sponsore.MemberId);
                                if (member != null)
                                {
                                    member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                    futureSponsors.Add(member);
                                }
                            }
                        }

                        //chose only member those will be add in future period
                        foreach (var sponsoredMember in sponsoredAddedMembers.Where(spon => spon.ActionType == ActionType.Update || spon.ActionType == ActionType.Create))
                        {
                            if (string.IsNullOrEmpty(memICHUpdate.SponsoredMembersPeriodFrom))
                                memICHUpdate.SponsoredMembersPeriodFrom = sponsoredMember.ChangeEffectivePeriod != null ? sponsoredMember.ChangeEffectivePeriod.Value.ToString("yyMMdd") : string.Empty;

                            var member = MemberRepository.Single(mem => mem.Id == sponsoredMember.MemberId);
                            if (member != null)
                            {
                                member.MemberCode = member.MemberCodeAlpha + member.MemberCodeNumeric;
                                futureSponsors.Add(member);
                            }
                        }
                    }
                   
                    // Add future sponsoror list to IchConfiguration data to be sent
                    if ((futureSponsors != null) && (futureSponsors.Count > 0))
                    {
                        memICHUpdate.SponsoredMembersFutureValue = futureSponsors;
                    }

                    // Get list of contacts for member and get the contact assignments for the contact
                    // Search for contacts assigned to contact types having subgroup=7 i.e. having subgroup = ICH
                    var searchCriteria = new ContactAssignmentSearchCriteria { SubGroupId = "7" };

                    _logger.Info("Before retrieving contact assignment data");
                    DataTable result;

                    try
                    {
                        int recordCount;
                        result = MemberManager.GetContactAssignmentData(searchCriteria, memberId, 1, out recordCount);

                        _logger.Info("Contact assignment data retrieved successfully");

                        foreach (DataRow row in result.Rows)
                        {
                            var contact = new Contact();

                            // Check if contact data exists in member profile.If not then get data from member profile
                            var contactDetails = MemberManager.GetContactDetails(Convert.ToInt32(row["CONTACT_ID"]));
                            _logger.Info("Get Contact data for individual contact");

                            if (contactDetails != null)
                            {
                                // Check if details are present in user profile
                                var userData = MemberManager.GetUserByEmailId(contactDetails.EmailAddress, memberId);

                                if ((userData != null) && (userData.UserID != 0))
                                {
                                    // Contact is found in user profile
                                    contact.Id = contactDetails.Id;
                                    contact.IsActive = userData.IsActive;
                                    contact.FirstName = userData.FirstName;
                                    contact.LastName = userData.LastName;
                                    contact.EmailAddress = contactDetails.EmailAddress;
                                    contact.PositionOrTitle = userData.PositionOrTitle;
                                    contact.Salutation = (Salutation)userData.Salutation;
                                }
                                else
                                {
                                    // Contact is found in member profile
                                    contact.Id = contactDetails.Id;
                                    contact.IsActive = contactDetails.IsActive;
                                    contact.FirstName = contactDetails.FirstName;
                                    contact.LastName = contactDetails.LastName;
                                    contact.EmailAddress = contactDetails.EmailAddress;
                                    contact.PositionOrTitle = contactDetails.PositionOrTitle;

                                    if (contactDetails.SalutationId.HasValue)
                                    {
                                        contact.Salutation = contactDetails.Salutation.Value;
                                    }
                                }


                              if(row.Table.Columns.Contains(((int)ProcessingContactType.ICHPrimaryContact).ToString()))
                              {

                                if (Convert.ToInt16(row[((int)ProcessingContactType.ICHPrimaryContact).ToString()]) == 1)
                                {
                                    contact.PrimaryContact = true;
                                }
                              }
                              if (row.Table.Columns.Contains(((int)ProcessingContactType.ICHAdviceContact).ToString()))
                              {
                                if (Convert.ToInt16(row[((int) ProcessingContactType.ICHAdviceContact).ToString()]) == 1)
                                {
                                  contact.AdviceContact = true;
                                }
                              }
                              if (row.Table.Columns.Contains(((int)ProcessingContactType.ICHClaimConfirmationContact).ToString()))
                              {
                                if (
                                  Convert.ToInt16(
                                    row[((int) ProcessingContactType.ICHClaimConfirmationContact).ToString()]) == 1)
                                {
                                  contact.ClaimConfirmationContact = true;
                                }
                              }
                              if (row.Table.Columns.Contains(((int)ProcessingContactType.ICHClearanceInitializationContact).ToString()))
                              {
                                if (
                                  Convert.ToInt16(
                                    row[((int) ProcessingContactType.ICHClearanceInitializationContact).ToString()]) ==
                                  1)
                                {
                                  contact.ClearanceInitializationContact = true;
                                }
                              }
                              if (row.Table.Columns.Contains(((int)ProcessingContactType.ICHFinancialContact).ToString()))
                              {
                                if (
                                  Convert.ToInt16(row[((int) ProcessingContactType.ICHFinancialContact).ToString()]) ==
                                  1)
                                {
                                  contact.FinancialContact = true;
                                }
                              }
                              // Add contact to list of contact which needs to be sent to ICH
                                contactList.Add(contact);

                                _logger.Info("Contact data retrieved successfully");
                            }

                            // Get code for reading Contact type assignment data
                        }

                        // Assign contact list generated to memICHUpdate object
                        if (contactList != null)
                        {
                            memICHUpdate.Contacts = contactList;
                        }
                    }
                    catch (ISBusinessException ex)
                    {
                        _logger.Info(string.Format("Error while retrieving contact assignment data : {0}-{1}", ex.ErrorCode, ex.InnerException));
                    }


                    //Get Merger Information 
                    try
                    {
                        var memberDetails = MemberManager.GetMember(memberId, true);
                        _logger.Info("Retrieved Parent Member Details for member");

                        //Get Current Merger Info
                        parentMemberCurrentValue.IsMerged = memberDetails.IsMerged;
                        parentMemberCurrentValue.MergerEffectivePeriod = memberDetails.ActualMergerDate;
                        parentMemberCurrentValue.ParentMemberCode = memberDetails.ParentMemberIdDisplayValue;

                        // Restructure ParentMemberCode to (MemberCodeAlpha + MemberCodeNumeric) i.e. AA001 formate. 
                        if (!string.IsNullOrWhiteSpace(parentMemberCurrentValue.ParentMemberCode))
                        {
                            var memCode = parentMemberCurrentValue.ParentMemberCode.Split('-');
                            parentMemberCurrentValue.ParentMemberCode = memCode[0] + memCode[1];
                        }

                        //Convert  Future period in yyMMdd formate
                        var fperiod = !string.IsNullOrWhiteSpace(memberDetails.ParentMemberIdFuturePeriod) ? Convert.ToDateTime(memberDetails.ParentMemberIdFuturePeriod).ToString("yyMMdd") : string.Empty;
                        //Get Future Merger Info
                        parentMemberFutureValue.IsMerged = memberDetails.IsMergedFutureValue;
                        parentMemberFutureValue.MergerEffectivePeriod = memberDetails.ActualMergerDateFutureValue;
                        parentMemberFutureValue.ParentMemberCode = memberDetails.ParentMemberIdFutureDisplayValue;
                        parentMemberFutureValue.PeriodFrom = fperiod;

                        // Restructure ParentMemberCode to (MemberCodeAlpha + MemberCodeNumeric) i.e. AA001 formate. 
                        if (!string.IsNullOrWhiteSpace(parentMemberFutureValue.ParentMemberCode))
                        {
                            var memCode = parentMemberFutureValue.ParentMemberCode.Split('-');
                            parentMemberFutureValue.ParentMemberCode = memCode[0] + memCode[1];
                        }

                        // If Current Merger Info already Present in system and user is removing Current merger information i.e. doing unmerge in future merger information.
                        // Then Retrun Future Merger info with current merger detail but updating IsMerge flage as "False" and Future Period when it will apply.
                        if (parentMemberCurrentValue.IsMerged == true &&
                            !string.IsNullOrWhiteSpace(parentMemberCurrentValue.ParentMemberCode) &&
                            parentMemberFutureValue.IsMerged == false &&
                            string.IsNullOrWhiteSpace(parentMemberFutureValue.ParentMemberCode) &&
                           !string.IsNullOrWhiteSpace(parentMemberFutureValue.PeriodFrom))
                        {
                            parentMemberFutureValue.IsMerged = false;
                            parentMemberFutureValue.MergerEffectivePeriod = memberDetails.ActualMergerDate;
                            parentMemberFutureValue.ParentMemberCode = parentMemberCurrentValue.ParentMemberCode;
                            parentMemberFutureValue.PeriodFrom = fperiod;

                        }

                        memICHUpdate.ParentMemberCurrentValue = parentMemberCurrentValue;
                        memICHUpdate.ParentMemberFutureValue = parentMemberFutureValue;
                    }
                    catch (Exception ex)
                    {
                        _logger.Info(string.Format("Error while retrieving Parent Member Details for member data : {0}-{1}", ex.Message, ex.InnerException));
                    }

                    _logger.Info("Before serializing member profile update object");
                    // Convert object to XML
                    memberProfileUpdateXml = ICHXmlHandler.SerializeXml(memICHUpdate, typeof(MemberProfileUpdate));
                    _logger.Info("Member profile update object serialized successfully");

                    // Remove nodes having attribute xsi:nil="true"
                    memberProfileUpdateXml = memberProfileUpdateXml.Replace("xsi:nil=\"true\"", "");
                    _xmlMemberProfileUpdate.LoadXml(memberProfileUpdateXml);

                    _logger.Info("Restructuring contact nodes");

                    // Remove Merger Information node
                    if ((memICHUpdate.ParentMemberCurrentValue.IsMerged == null || memICHUpdate.ParentMemberCurrentValue.IsMerged == false) &&
                        string.IsNullOrWhiteSpace(memICHUpdate.ParentMemberCurrentValue.ParentMemberCode) &&
                        (memICHUpdate.ParentMemberFutureValue.IsMerged == null || memICHUpdate.ParentMemberFutureValue.IsMerged == false) &&
                        string.IsNullOrWhiteSpace(memICHUpdate.ParentMemberFutureValue.ParentMemberCode))
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberCurrentValue"));
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberFutureValue"));
                    }
                    else
                    {
                        if ((memICHUpdate.ParentMemberCurrentValue.IsMerged == null || memICHUpdate.ParentMemberCurrentValue.IsMerged == false) &&
                        string.IsNullOrWhiteSpace(memICHUpdate.ParentMemberCurrentValue.ParentMemberCode))
                        {
                            ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberCurrentValue"));
                        }
                        if ((memICHUpdate.ParentMemberFutureValue.IsMerged == null || memICHUpdate.ParentMemberFutureValue.IsMerged == false) &&
                            string.IsNullOrWhiteSpace(memICHUpdate.ParentMemberFutureValue.ParentMemberCode) &&
                            string.IsNullOrWhiteSpace(memICHUpdate.ParentMemberFutureValue.PeriodFrom))
                        {
                            ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberFutureValue"));
                        }

                    }

                    // Rename node Id to contactId
                    var childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact/Id");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "ContactId");
                    }

                    // Rename node IsActive to isActive
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact/IsActive");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "isActive");
                    }

                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact/PositionOrTitle");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "Position");
                    }

                    var contactNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact");
                    if (contactNodeList != null)
                    {
                        foreach (XmlNode contactNode in contactNodeList)
                        {
                            if (contactNode.SelectSingleNode("SalutationId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("SalutationId"));
                            }

                            if (contactNode.SelectSingleNode("LastUpdatedOn") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("LastUpdatedOn"));
                            }

                            if (contactNode.SelectSingleNode("LastUpdatedBy") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("LastUpdatedBy"));
                            }

                            if (contactNode.SelectSingleNode("IsUserId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("IsUserId"));
                            }

                            if (contactNode.SelectSingleNode("LocationId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("LocationId"));
                            }

                            if (contactNode.SelectSingleNode("CountryId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("CountryId"));
                            }

                            if (contactNode.SelectSingleNode("StartDate") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("StartDate"));
                            }

                            if (contactNode.SelectSingleNode("EndDate") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("EndDate"));
                            }

                            if (contactNode.SelectSingleNode("MemberId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("MemberId"));
                            }

                            //SCP#0000: Impact of CMP#655 changes.
                            if (contactNode.SelectSingleNode("IsContactIsUser") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("IsContactIsUser"));
                            }

                            if (contactNode.SelectSingleNode("EmailAddress") != null)
                            {
                                if (contactNode.SelectSingleNode("LastName") != null)
                                {
                                    contactNode.InsertAfter(contactNode.RemoveChild(contactNode.SelectSingleNode("EmailAddress")), contactNode.SelectSingleNode("LastName"));
                                }
                                else
                                {
                                    contactNode.InsertAfter(contactNode.RemoveChild(contactNode.SelectSingleNode("EmailAddress")), contactNode.SelectSingleNode("FirstName"));
                                }
                            }
                        }
                    }

                    _logger.Info("Restructuring Aggregator and sponsoror nodes");

                    // Form AggregatedMember nodes as per XSD
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersCurrentValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "AggregatedMember");
                    }

                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersFutureValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "AggregatedMember");
                    }

                    // Form Sponsored member nodes as per XSD
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersCurrentValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "SponsoredMember");
                    }

                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersFutureValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "SponsoredMember");
                    }

                    // Insert PeriodFrom nodes inside AggregatedMembersFutureValue and SponsoredMembersFutureValue nodes
                    var parentNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersFutureValue");
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersFutureValue/SponsoredMember");

                    if ((childNodeList != null) && (memICHUpdate.SponsoredMembersPeriodFrom != null))
                    {
                        ICHXmlHandler.AddPeriodFromNodetoNodeList(childNodeList, parentNodeList, memICHUpdate.SponsoredMembersPeriodFrom, ref _xmlMemberProfileUpdate);
                    }

                    parentNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersFutureValue");
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersFutureValue/AggregatedMember");

                    if ((childNodeList != null) && (memICHUpdate.AggregatedMembersPeriodFrom != null))
                    {
                        ICHXmlHandler.AddPeriodFromNodetoNodeList(childNodeList, parentNodeList, memICHUpdate.AggregatedMembersPeriodFrom, ref _xmlMemberProfileUpdate);
                    }

                    _logger.Info("Removing null nodes");

                    // Remove nodes for which value is null and min occurrence is marked as 0 in XSD
                    // Remove AggregatedTypeCurrentValue node if value is null
                    if (memICHUpdate.AggregatedTypeCurrentValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedTypeCurrentValue"));
                    }

                    // Remove SponsoredMembersPeriodFrom node since it is not required in xml
                    ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/SponsoredMembersPeriodFrom"));

                    // Remove AggregatorPeriodFrom node since it is not required in xml
                    ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedMembersPeriodFrom"));

                    // Remove MemberStatusFutureValue node if value is null
                    if (memICHUpdate.MemberStatusFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/MemberStatusFutureValue"));
                    }

                    // Remove ZoneFutureValue node if value is null
                    if (memICHUpdate.ZoneFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ZoneFutureValue"));
                    }

                    // Remove CategoryFutureValue node if value is null
                    if (memICHUpdate.CategoryFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/CategoryFutureValue"));
                    }

                    // Remove AggregatedTypeCurrentValue node if value is null
                    if (memICHUpdate.AggregatedTypeCurrentValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedTypeCurrentValue"));
                    }

                    // Remove AggregatedTypeFutureValue node if value is null
                    if (memICHUpdate.AggregatedTypeFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedTypeFutureValue"));
                    }
                    //Can submit - check what is the value when value is not set

                    _logger.Info("Applying XSLT for removing blank nodes");
                    const string xslStylesheet = "<?xml version='1.0' encoding='UTF-8'?> <xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:fn='http://www.w3.org/2005/xpath-functions'> <xsl:output method='xml' version='1.0' encoding='UTF-8' indent='yes'/> <xsl:strip-space elements='*'/> <xsl:template match='*[not(node()) and not(./@*)]'/> <xsl:template match='@* | node()'> <xsl:copy> <xsl:apply-templates select='@* | node()'/> </xsl:copy> </xsl:template> </xsl:stylesheet>";

                    _xmlMemberProfileUpdate = ICHXmlHandler.CallXsltToModifyXml(_xmlMemberProfileUpdate, xslStylesheet);
                    _logger.Info("Applying XSLT for removing blank nodes - done");

                    memberProfileUpdateXml = _xmlMemberProfileUpdate.InnerXml;
                    // Replace '-' character getting displayed for complex nodes
                    _logger.Info("Before validating Member profile update XML");
                    _logger.InfoFormat("Generated XML is [{0}]", memberProfileUpdateXml);

                    var XSDPath = string.Format("{0}{1}", ConnectionString.GetAppSetting("AppSettingPath"), memberProfileUpdateFolder);
                    //var sValResult = ICHXmlHandler.Validate(memberProfileUpdateXml, "MemberProfileUpdate.xsd");
                    var sValResult = ICHXmlHandler.Validate(memberProfileUpdateXml, XSDPath);

                    _logger.Info("Validation Result");
                    _logger.Info(sValResult);

                    // Get details from future update
                    if (sValResult != "OK")
                    {
                        var invalidXML = memberProfileUpdateXml;
                        memberProfileUpdateXml = "Error";
                        SendAlertForXmlValidationFailure(ichConfiguration.MemberId, invalidXML, "MemberProfileUpdate", sValResult);
                    }
                }

            }
            catch (ISBusinessException ex)
            {
                _logger.Info(string.Format("In Error : {0}-{1}", ex.ErrorCode, ex.InnerException));
            }

            return memberProfileUpdateXml;
        }

        /// <summary>
        /// Generates the XML for ACH updates.
        /// </summary>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public string GenerateXMLforACHUpdates(int memberId)
        {
            parentMemberCurrentValue = new ParentMemberDetail();
            parentMemberFutureValue = new ParentMemberDetail();
            MemberProfileUpdate memACHUpdate;
            List<FutureUpdates> futureUpdatesList;
            string memberProfileUpdateXml = null;

            _xmlMemberProfileUpdate = new XmlDocument();

            // Remove Blank nodes
            _logger.Info("Before resolving member manager reference");

            // Get an object of the member manager
            MemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

            _logger.Info("After resolving member manager reference");

            try
            {
                BlockingRulesRepository = Ioc.Resolve<IBlockingRulesRepository>();
                AchRepository = Ioc.Resolve<IRepository<AchConfiguration>>();
                MemberRepository = Ioc.Resolve<IMemberRepository>();
                ReferenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                MiscCodeRepository = Ioc.Resolve<IMiscCodeRepository>(typeof(IMiscCodeRepository));

                ICHXmlHandler = new ICHXmlHandler();
                _logger.Info(string.Format("BlockingRulesRepository instance is: [{0}]", BlockingRulesRepository != null ? "NOT NULL" : "NULL"));
                _logger.Info(string.Format("AchRepository instance is: [{0}]", AchRepository != null ? "NOT NULL" : "NULL"));
                _logger.Info(string.Format("MemberRepository instance is: [{0}]", MemberRepository != null ? "NOT NULL" : "NULL"));

                // Get ICH config
                var achConfiguration = MemberManager.GetAchConfig(memberId, true);
                _logger.Info("Retrieved ACH configuration for member");

                //CMP #625: New Fields in ICH Member Profile Update XML
                var technicalConfiguration = MemberManager.GetTechnicalConfig(memberId);
                _logger.Info("Retrieved Technical configuration for member");


                //CMP-689-Flexible CH Activation Options
                // before this CMP only Members having ‘ACH Membership Status’  as "Live", "Suspended" or "Terminated" are included in the update XML
                // With this CMP, the system should also consider the following:
                //  a.	Members whose future value of ‘ACH Membership Status’ = “Live” (and where the current value = “Not a Member” or "Terminated")
                _logger.Info("checking Members future value of ‘ACH Membership Status’ = 'Live' (and where the current value = 'Not a Member' or 'Terminated')");
                var isFutureUpdateLive = (achConfiguration != null &&
                                             achConfiguration.AchMembershipStatusIdFutureValue == (int)IchMemberShipStatus.Live &&
                                             (achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.NotAMember ||
                                              achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Terminated))
                                                ? true
                                                : false;
                _logger.InfoFormat("isFutureUpdateIsLive : {0}",isFutureUpdateLive);


                if (achConfiguration != null)
                {
                    _logger.Info("Before retrieving ICH member status list");
                    achConfiguration = MemberManager.GetAchMemberStatusList(achConfiguration);
                    _logger.Info("After retrieving ICH member status list");

                    // Get UATP configuration corresponding to the member
                    memACHUpdate = new MemberProfileUpdate();

                    _logger.Info(string.Format("FutureUpdateRepository instance is: [{0}]", FutureUpdateRepository != null ? "NOT NULL" : "NULL"));
                    _logger.Info(string.Format("AchConfiguration instance is: [{0}]", achConfiguration != null ? "NOT NULL" : "NULL"));
                    _logger.Info(string.Format("AchConfiguration.MemberId is: [{0}]", achConfiguration.MemberId));

                    if (FutureUpdateRepository == null)
                    {
                        FutureUpdateRepository = Ioc.Resolve<IFutureUpdatesRepository>();
                    }
                    _logger.Info(string.Format("FutureUpdateRepository instance after Ioc.Resolve is: [{0}]", FutureUpdateRepository != null ? "NOT NULL" : "NULL"));
                    _logger.Info(string.Format("FutureUpdatesManager instance is: [{0}]", FutureUpdatesManager != null ? "NOT NULL" : "NULL"));

                    if (FutureUpdatesManager == null)
                    {
                        FutureUpdatesManager = Ioc.Resolve<IFutureUpdatesManager>();
                    }
                    _logger.Info(string.Format("FutureUpdatesManager instance after Ioc.Resolve is: [{0}]", FutureUpdatesManager != null ? "NOT NULL" : "NULL"));

                    memACHUpdate.MemberStatusCurrentValue = ReferenceManager.GetDisplayValue(MiscGroups.AchMemberStatus, achConfiguration.AchMembershipStatusId);
                    _logger.Info("Member status current value retrieved successfully");

                    // Get zone name from database
                    memACHUpdate.ZoneCurrentValue = ReferenceManager.GetIchZoneDisplayValue((int)IchZoneType.C);
                    memACHUpdate.CategoryCurrentValue = ReferenceManager.GetDisplayValue(MiscGroups.AchMemberCategory, achConfiguration.AchCategoryId);
                    memACHUpdate.NumericMemberCode = achConfiguration.Member.MemberCodeNumeric;
                    memACHUpdate.AlphaMemberCode = achConfiguration.Member.MemberCodeAlpha;

                    // Set details about ICH Tab data
                    // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV
                    memACHUpdate.MemberNameCurrentValue = achConfiguration.Member.LegalName;
                    if (!string.IsNullOrEmpty(achConfiguration.MemberNameFutureValue) && !string.IsNullOrEmpty(achConfiguration.MemberNameChangePeriodFrom))
                    {
                      memACHUpdate.MemberNameFutureValue = achConfiguration.MemberNameFutureValue;
                      memACHUpdate.MemberNameChangePeriodFrom = Convert.ToDateTime(achConfiguration.MemberNameChangePeriodFrom).ToString("yyMMdd");
                    }

                    memACHUpdate.Comments = achConfiguration.Member.IsOpsComments;
                    memACHUpdate.ICHWebReportOptions = 1;
                    memACHUpdate.CanSubmitPAXWebF12FilesCurrentValue = true;
                    memACHUpdate.CanSubmitCGOWebF12FilesCurrentValue = true;
                    memACHUpdate.CanSubmitMISCWebF12FilesCurrentValue = true;
                    memACHUpdate.CanSubmitUATPWebF12FilesCurrentValue = true;
                    memACHUpdate.AggregatedTypeCurrentValue = 1;

                    //CMP #625: New Fields in ICH Member Profile Update XML.
                    memACHUpdate.SISMemberID = achConfiguration.MemberId;
                    memACHUpdate.IInetAccountIds = GetListOfIInetAccountId(null, technicalConfiguration);

                    _logger.Info(string.Format("Current ACH configuration values set successfully"));
                    // UATPInvoicehandledByATCAN

                    memACHUpdate.UATPInvoiceHandledByATCANCurrentValue = achConfiguration.Member.UatpInvoiceHandledbyAtcan;

                    // Get future update records corresponding to the UATPInvoiceHandledbyATCAN field
                    _logger.Info("Before retrieving future values for UATP configuration");
                    futureUpdatesList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails, memberId, "IS_UATP_INV_HANDLED_BY_ATCAN", null);
                    _logger.Info("After retrieving future values for UATP configuration");
                    if ((futureUpdatesList != null) && (futureUpdatesList.Count == 1))
                    {
                        _logger.Info(string.Format("1 future update for field IS_UATP_INV_HANDLED_BY_ATCAN is set"));
                        memACHUpdate.UATPInvoiceHandledByATCANFutureValue = bool.Parse(futureUpdatesList[0].NewVAlue);
                        memACHUpdate.UATPInvoiceHandledByATCANPeriodFrom = futureUpdatesList[0].ChangeEffectivePeriod != null ? futureUpdatesList[0].ChangeEffectivePeriod.Value.ToString("yyMMdd") : string.Empty;
                    }
                    else
                    {
                        memACHUpdate.UATPInvoiceHandledByATCANFutureValue = null;
                        _logger.Info(string.Format("More than 1 future updates or no future updates set for field IS_UATP_INV_HANDLED_BY_ATCAN are set"));
                    }

                    // Indicates current status is suspended
                    if (achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Suspended)
                    {
                        memACHUpdate.SuspensionPeriodFrom = achConfiguration.StatusChangedDate != null ? achConfiguration.StatusChangedDate.Value.ToString("yyMMdd") : string.Empty;
                    }

                    // Indicates member status has changed to live from suspended
                    memACHUpdate.ReinstatementPeriodFrom = achConfiguration.ReinstatementPeriod != null ? achConfiguration.ReinstatementPeriod.Value.ToString("yyMMdd") : string.Empty;
                    memACHUpdate.MemberStatusFutureValue = achConfiguration.ReinstatementPeriod != null
                                                             ? ReferenceManager.GetDisplayValue(MiscGroups.IchMemberStatus, (int)IchMemberShipStatus.Live) : string.Empty;

                    // Reinstatement period is specified then specify member status future value as Live
                    if ((achConfiguration.ReinstatementPeriod != null))
                    {
                        memACHUpdate.EntryDate = achConfiguration.EntryDate != null ? achConfiguration.EntryDate.Value.ToString("dd-MMM-yyyy").ToUpper() : string.Empty;
                    }

                    // Indicates current status is live and previous status was not suspended
                    if ((achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Live) && (achConfiguration.ReinstatementPeriod == null))
                    {
                        memACHUpdate.EntryDate = achConfiguration.EntryDate != null ? achConfiguration.EntryDate.Value.ToString("dd-MMM-yyyy").ToUpper() : string.Empty;
                    }

                    // Indicates current status is Terminated
                    if (achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Terminated)
                    {
                        memACHUpdate.TerminationDate = achConfiguration.TerminationDate != null ? achConfiguration.TerminationDate.Value.ToString("dd-MMM-yyyy").ToUpper() : string.Empty;
                    }

                    if (achConfiguration.AchMembershipStatusIdFutureValue == (int)IchMemberShipStatus.Terminated)
                    {
                        memACHUpdate.TerminationPeriodFrom = Convert.ToDateTime(achConfiguration.AchMembershipStatusIdFuturePeriod).ToString("yyMMdd");
                        memACHUpdate.MemberStatusFutureValue = achConfiguration.AchMembershipStatusIdFutureDisplayValue;
                    }

                    //CMP-689-Flexible CH Activation Options
                    if (isFutureUpdateLive)
                    {
                      memACHUpdate.MemberStatusFutureValue = achConfiguration.AchMembershipStatusIdFutureDisplayValue;
                      memACHUpdate.MemberStatusPeriodFrom =
                           Convert.ToDateTime(achConfiguration.AchMembershipStatusIdFuturePeriod).ToString("yyMMdd");
                    }

                    //Get Merger Information 
                    try
                    {
                        var memberDetails = MemberManager.GetMember(memberId, true);
                        _logger.Info("Retrieved Parent Member Details for member");

                        //Get Current Merger Info
                        parentMemberCurrentValue.IsMerged = memberDetails.IsMerged;
                        parentMemberCurrentValue.MergerEffectivePeriod = memberDetails.ActualMergerDate;
                        parentMemberCurrentValue.ParentMemberCode = memberDetails.ParentMemberIdDisplayValue;

                        // Restructure ParentMemberCode to (MemberCodeAlpha + MemberCodeNumeric) i.e. AA001 formate. 
                        if (!string.IsNullOrWhiteSpace(parentMemberCurrentValue.ParentMemberCode))
                        {
                            var memCode = parentMemberCurrentValue.ParentMemberCode.Split('-');
                            parentMemberCurrentValue.ParentMemberCode = memCode[0] + memCode[1];
                        }

                        //Convert  Future period in yyMMdd formate
                        var fperiod = !string.IsNullOrWhiteSpace(memberDetails.ParentMemberIdFuturePeriod) ? Convert.ToDateTime(memberDetails.ParentMemberIdFuturePeriod).ToString("yyMMdd") : string.Empty;
                        //Get Future Merger Info
                        parentMemberFutureValue.IsMerged = memberDetails.IsMergedFutureValue;
                        parentMemberFutureValue.MergerEffectivePeriod = memberDetails.ActualMergerDateFutureValue;
                        parentMemberFutureValue.ParentMemberCode = memberDetails.ParentMemberIdFutureDisplayValue;
                        parentMemberFutureValue.PeriodFrom = fperiod;

                        // Restructure ParentMemberCode to (MemberCodeAlpha + MemberCodeNumeric) i.e. AA001 formate. 
                        if (!string.IsNullOrWhiteSpace(parentMemberFutureValue.ParentMemberCode))
                        {
                            var memCode = parentMemberFutureValue.ParentMemberCode.Split('-');
                            parentMemberFutureValue.ParentMemberCode = memCode[0] + memCode[1];
                        }

                        // If Current Merger Info already Present in system and user is removing Current merger information i.e. doing unmerge in future merger information.
                        // Then Retrun Future Merger info with current merger detail but updating IsMerge flage as "False" and Future Period when it will apply.
                        if (parentMemberCurrentValue.IsMerged == true &&
                            !string.IsNullOrWhiteSpace(parentMemberCurrentValue.ParentMemberCode) &&
                            parentMemberFutureValue.IsMerged == false &&
                            string.IsNullOrWhiteSpace(parentMemberFutureValue.ParentMemberCode) &&
                           !string.IsNullOrWhiteSpace(parentMemberFutureValue.PeriodFrom))
                        {
                            parentMemberFutureValue.IsMerged = false;
                            parentMemberFutureValue.MergerEffectivePeriod = memberDetails.ActualMergerDate;
                            parentMemberFutureValue.ParentMemberCode = parentMemberCurrentValue.ParentMemberCode;
                            parentMemberFutureValue.PeriodFrom = fperiod;

                        }

                        memACHUpdate.ParentMemberCurrentValue = parentMemberCurrentValue;
                        memACHUpdate.ParentMemberFutureValue = parentMemberFutureValue;
                    }
                    catch (Exception ex)
                    {
                        _logger.Info(string.Format("Error while retrieving Parent Member Details for member data : {0}-{1}", ex.Message, ex.InnerException));
                    }

                    _logger.Info("Before serializing member profile update object");
                    // Convert object to XML
                    memberProfileUpdateXml = ICHXmlHandler.SerializeXml(memACHUpdate, typeof(MemberProfileUpdate));
                    _logger.Info("Member profile update object serialized successfully");
                    // Remove nodes having attribute xsi:nil="true"
                    memberProfileUpdateXml = memberProfileUpdateXml.Replace("xsi:nil=\"true\"", "");
                    _xmlMemberProfileUpdate.LoadXml(memberProfileUpdateXml);

                    _logger.Info("Restructuring contact nodes");

                    // Remove Merger Information node
                    if ((memACHUpdate.ParentMemberCurrentValue.IsMerged == null || memACHUpdate.ParentMemberCurrentValue.IsMerged == false) &&
                        string.IsNullOrWhiteSpace(memACHUpdate.ParentMemberCurrentValue.ParentMemberCode) &&
                        (memACHUpdate.ParentMemberFutureValue.IsMerged == null || memACHUpdate.ParentMemberFutureValue.IsMerged == false) &&
                        string.IsNullOrWhiteSpace(memACHUpdate.ParentMemberFutureValue.ParentMemberCode))
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberCurrentValue"));
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberFutureValue"));
                    }
                    else
                    {
                        if ((memACHUpdate.ParentMemberCurrentValue.IsMerged == null || memACHUpdate.ParentMemberCurrentValue.IsMerged == false) &&
                        string.IsNullOrWhiteSpace(memACHUpdate.ParentMemberCurrentValue.ParentMemberCode))
                        {
                            ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberCurrentValue"));
                        }
                        if ((memACHUpdate.ParentMemberFutureValue.IsMerged == null || memACHUpdate.ParentMemberFutureValue.IsMerged == false) &&
                            string.IsNullOrWhiteSpace(memACHUpdate.ParentMemberFutureValue.ParentMemberCode) &&
                            string.IsNullOrWhiteSpace(memACHUpdate.ParentMemberFutureValue.PeriodFrom))
                        {
                            ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ParentMemberFutureValue"));
                        }
                    }

                    // Rename node Id to contactId
                    var childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact/Id");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "ContactId");
                    }

                    // Rename node IsActive to isActive
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact/IsActive");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "isActive");
                    }

                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact/PositionOrTitle");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "Position");
                    }

                    var contactNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /Contacts/Contact");
                    if (contactNodeList != null)
                    {
                        foreach (XmlNode contactNode in contactNodeList)
                        {
                            if (contactNode.SelectSingleNode("SalutationId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("SalutationId"));
                            }

                            if (contactNode.SelectSingleNode("LastUpdatedOn") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("LastUpdatedOn"));
                            }

                            if (contactNode.SelectSingleNode("LastUpdatedBy") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("LastUpdatedBy"));
                            }

                            if (contactNode.SelectSingleNode("IsUserId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("IsUserId"));
                            }

                            if (contactNode.SelectSingleNode("LocationId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("LocationId"));
                            }

                            if (contactNode.SelectSingleNode("CountryId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("CountryId"));
                            }

                            if (contactNode.SelectSingleNode("StartDate") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("StartDate"));
                            }

                            if (contactNode.SelectSingleNode("EndDate") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("EndDate"));
                            }

                            if (contactNode.SelectSingleNode("MemberId") != null)
                            {
                                contactNode.RemoveChild(contactNode.SelectSingleNode("MemberId"));
                            }

                            if (contactNode.SelectSingleNode("EmailAddress") != null)
                            {
                                if (contactNode.SelectSingleNode("LastName") != null)
                                {
                                    contactNode.InsertAfter(contactNode.RemoveChild(contactNode.SelectSingleNode("EmailAddress")), contactNode.SelectSingleNode("LastName"));
                                }
                                else
                                {
                                    contactNode.InsertAfter(contactNode.RemoveChild(contactNode.SelectSingleNode("EmailAddress")), contactNode.SelectSingleNode("FirstName"));
                                }
                            }
                        }
                    }
                    _logger.Info("Restructuring Aggregator and sponsoror nodes");

                    // Form AggregatedMember nodes as per XSD
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersCurrentValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "AggregatedMember");
                    }

                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersFutureValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "AggregatedMember");
                    }

                    // Form Sponsored member nodes as per XSD
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersCurrentValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "SponsoredMember");
                    }

                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersFutureValue/Member");
                    ICHXmlHandler.RemoveExtraNodesFromNodeList(childNodeList, "MemberCode");
                    if (childNodeList != null)
                    {
                        ICHXmlHandler.RenameNode(childNodeList, "SponsoredMember");
                    }

                    // Insert PeriodFrom nodes inside AggregatedMembersFutureValue and SponsoredMembersFutureValue nodes
                    var parentNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersFutureValue");
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /SponsoredMembersFutureValue/SponsoredMember");

                    if ((childNodeList != null) && (memACHUpdate.SponsoredMembersPeriodFrom != null))
                    {
                        ICHXmlHandler.AddPeriodFromNodetoNodeList(childNodeList, parentNodeList, memACHUpdate.SponsoredMembersPeriodFrom, ref _xmlMemberProfileUpdate);
                    }

                    parentNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersFutureValue");
                    childNodeList = _xmlMemberProfileUpdate.SelectNodes("/MemberProfileUpdate /AggregatedMembersFutureValue/AggregatedMember");

                    if ((childNodeList != null) && (memACHUpdate.AggregatedMembersPeriodFrom != null))
                    {
                        ICHXmlHandler.AddPeriodFromNodetoNodeList(childNodeList, parentNodeList, memACHUpdate.AggregatedMembersPeriodFrom, ref _xmlMemberProfileUpdate);
                    }

                    _logger.Info("Removing null nodes");

                    // Remove nodes for which value is null and min occurrence is marked as 0 in XSD
                    // Remove AggregatedTypeCurrentValue node if value is null
                    if (memACHUpdate.AggregatedTypeCurrentValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedTypeCurrentValue"));
                    }

                    // Remove SponsoredMembersPeriodFrom node since it is not required in xml
                    ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/SponsoredMembersPeriodFrom"));

                    // Remove AggregatorPeriodFrom node since it is not required in xml
                    ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedMembersPeriodFrom"));

                    // Remove MemberStatusFutureValue node if value is null
                    if (memACHUpdate.MemberStatusFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/MemberStatusFutureValue"));
                    }

                    // Remove ZoneFutureValue node if value is null
                    if (memACHUpdate.ZoneFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/ZoneFutureValue"));
                    }

                    // Remove CategoryFutureValue node if value is null
                    if (memACHUpdate.CategoryFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/CategoryFutureValue"));
                    }

                    // Remove AggregatedTypeCurrentValue node if value is null
                    if (memACHUpdate.AggregatedTypeCurrentValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedTypeCurrentValue"));
                    }

                    // Remove AggregatedTypeFutureValue node if value is null
                    if (memACHUpdate.AggregatedTypeFutureValue == null)
                    {
                        ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatedTypeFutureValue"));
                    }
                    //Can submit - check what is the value when value is not set

                    // Remove Node EarlyCallDay
                    ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/EarlyCallDay"));
                    // Remove Node AggregatorCurrentValue
                    ICHXmlHandler.RemoveOptionalNodes(_xmlMemberProfileUpdate.SelectSingleNode("/MemberProfileUpdate/AggregatorCurrentValue"));
                    _logger.Info("Applying XSLT for removing blank nodes");
                    const string xslStylesheet = "<?xml version='1.0' encoding='UTF-8'?> <xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:fn='http://www.w3.org/2005/xpath-functions'> <xsl:output method='xml' version='1.0' encoding='UTF-8' indent='yes'/> <xsl:strip-space elements='*'/> <xsl:template match='*[not(node()) and not(./@*)]'/> <xsl:template match='@* | node()'> <xsl:copy> <xsl:apply-templates select='@* | node()'/> </xsl:copy> </xsl:template> </xsl:stylesheet>";

                    _xmlMemberProfileUpdate = ICHXmlHandler.CallXsltToModifyXml(_xmlMemberProfileUpdate, xslStylesheet);
                    _logger.Info("Applying XSLT for removing blank nodes - done");

                    memberProfileUpdateXml = _xmlMemberProfileUpdate.InnerXml;

                    // Replace '-' character getting displayed for complex nodes
                    // memberProfileUpdateXml = memberProfileUpdateXml.Replace("-", "");
                    _logger.Info("Before validating Member profile update XML");
                    _logger.InfoFormat("Generated XML is [{0}]", memberProfileUpdateXml);

                    //var sValResult = ICHXmlHandler.Validate(memberProfileUpdateXml, "MemberProfileUpdate.xsd");
                    var XSDPath = string.Format("{0}{1}", ConnectionString.GetAppSetting("AppSettingPath"), memberProfileUpdateFolder);
                    var sValResult = ICHXmlHandler.Validate(memberProfileUpdateXml, XSDPath);

                    _logger.Info("Validation Result");
                    _logger.Info(sValResult);

                    // Get details from future update
                    if (sValResult != "OK")
                    {
                        var invalidXML = memberProfileUpdateXml;
                        memberProfileUpdateXml = "Error";
                        SendAlertForXmlValidationFailure(achConfiguration.MemberId, invalidXML, "MemberProfileUpdate", sValResult);
                    }
                }
            }
            catch (ISBusinessException ex)
            {
                _logger.Info(string.Format("In Error : {0}-{1}", ex.ErrorCode, ex.InnerException));
            }
            return memberProfileUpdateXml;
        }

        /// <summary>
        /// To send email notification to IS Admin when member profile update sending fails.
        /// </summary>
        public bool SendAlertToISAdminforMemProfileUpdateSendingFailure(int memberorBlockingRuleId, string updateType, string updateXml, string failureReason)
        {
            try
            {
                var memberPrefix = "";
                var memberDesignator = "";
                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

                int msgId = 0;
                // if update type is blocking rules then get member id from blocking rule id passed
                if (updateType == "Blocking Rules")
                {
                    var bRule = BlockingRulesRepository.Single(blRules => blRules.Id == memberorBlockingRuleId);

                    if (bRule != null)
                    {
                        msgId = bRule.MemberId;
                    }
                }
                else if (updateType == "Member Profile")
                {
                    msgId = memberorBlockingRuleId;
                }

                // Get details of member to which ICH updates are sent
                var memberData = MemberRepository.Single(mem => mem.Id == msgId);

                _logger.Info("Getting Member data for sending email");

                if (memberData != null)
                {
                    memberPrefix = memberData.MemberCodeNumeric;
                    memberDesignator = memberData.MemberCodeAlpha;

                    string.Format("Member Prefix, Member Designator : {0}-{1}", memberPrefix, memberDesignator);
                }

                // Object of the nVelocity data dictionary
                var context = new VelocityContext();
                context.Put("MemberPrefix", memberPrefix);
                context.Put("MemberDesignator", memberDesignator);
                context.Put("UpdateType", updateType);
                context.Put("FailureReason", failureReason);

                _logger.Info(string.Format("EmailSettingsRepository instance is: [{0}]", EmailSettingsRepository != null ? "NOT NULL" : "NULL"));
                var emailSettingForISAdminAlert = EmailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.ISAdminAlertMemberProfileUpdate);

                _logger.Info(string.Format("TemplatedTextGenerator instance is: [{0}]", TemplatedTextGenerator != null ? "NOT NULL" : "NULL"));
                // Generate email body text for own profile updates contact type mail
                var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ISAdminAlertMemberProfileUpdate, context);
                _logger.Info("Generated Email Text");

                // Create a mail object to send mail
                var msgForISAdminAlert = new MailMessage { From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress), IsBodyHtml = false };

                // loop through the contacts list and add them to To list of mail to be sent
                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                {
                    var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
                    var formatedEmailList = emailAddressList.Replace(',', ';');
                    var mailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

                    foreach (var mailaddr in mailAdressList)
                    {
                        msgForISAdminAlert.To.Add(mailaddr);
                    }
                }

                // Set subject of mail (replace special field placeholders with values)
                var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                msgForISAdminAlert.Subject = subject.Replace("$MemberName$", memberPrefix + "-" + memberDesignator);

                // Set body text of mail
                msgForISAdminAlert.Body = emailToISAdminText;
                _logger.Info(string.Format("updateXml is: [{0}]", updateXml));
                if (!string.IsNullOrEmpty(updateXml))
                {
                    var attachmentPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.SFRTempRootPath), memberPrefix + " - UpdateType.xml");
                    var contactAttachmentFileStream = new StreamWriter(attachmentPath, false);

                    contactAttachmentFileStream.WriteLine(updateXml);
                    contactAttachmentFileStream.Close();
                    msgForISAdminAlert.Attachments.Add(new Attachment(attachmentPath));
                }
                _logger.Info("Added Attachments Successfully");

                // Send the mail
                emailSender.Send(msgForISAdminAlert);

                return true;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occurred while sending alert to IS Admin for member profile update sending failed", exception);
                return false;
            }

        }

        /// <summary>
        /// To send email notification to IS Admin when member profile update sending fails.
        /// </summary>
        public bool SendAlertForXmlValidationFailure(int memberId, string updateXml, string operationType, string validationErrors)
        {
            try
            {
                // Get an object of the EmailSender component
                var memberPrefix = string.Empty;
                var memberDesignator = string.Empty;
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                var memberData = MemberRepository.Single(mem => mem.Id == memberId);

                _logger.Info("Getting Member data for sending email");

                if (memberData != null)
                {
                    memberPrefix = memberData.MemberCodeNumeric;
                    memberDesignator = memberData.MemberCodeAlpha;
                }
                if (memberId == 0)
                {
                    memberPrefix = "ICH";
                    memberDesignator = "Settlement";
                }
                _logger.Info(string.Format("EmailSender instance is: [{0}]", emailSender != null ? "NOT NULL" : "NULL"));

                // Object of the nVelocity data dictionary
                var context = new VelocityContext();
                context.Put("OperationType", operationType);
                context.Put("ValidationFailureDetails", validationErrors);

                _logger.Info(string.Format("EmailSettingsRepository instance is: [{0}]", EmailSettingsRepository != null ? "NOT NULL" : "NULL"));

                var emailSettingForISAdminAlert = EmailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.XmlValidationFailureNotification);

                // Generate email body text for own profile updates contact type mail
                _logger.Info(string.Format("TemplatedTextGenerator instance is: [{0}]", TemplatedTextGenerator != null ? "NOT NULL" : "NULL"));
                var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.XmlValidationFailureNotification, context);

                // Create a mail object to send mail
                var msgForISAdminAlert = new MailMessage { From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };

                var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                msgForISAdminAlert.Subject = subject;
                msgForISAdminAlert.Subject = subject.Replace("$MemberName$", memberPrefix + "-" + memberDesignator);

                // Loop through the contacts list and add them to To list of mail to be sent
                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                {
                    var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
                    // If (emailAddressList.Contains(','))
                    var formatedEmailList = emailAddressList.Replace(',', ';');
                    var mailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

                    foreach (var mailaddr in mailAdressList)
                    {
                        msgForISAdminAlert.To.Add(mailaddr);
                    }
                }

                // Set body text of mail
                msgForISAdminAlert.Body = emailToISAdminText;
                _logger.Info(string.Format("updateXml is: [{0}]", updateXml));
                if (!string.IsNullOrEmpty(updateXml))
                {
                    var attachmentPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.SFRTempRootPath), memberPrefix + " - UpdateType.xml");
                    var contactAttachmentFileStream = new StreamWriter(attachmentPath, false);

                    contactAttachmentFileStream.WriteLine(updateXml);
                    contactAttachmentFileStream.Close();
                    msgForISAdminAlert.Attachments.Add(new Attachment(attachmentPath));
                }
                _logger.Info("Added Attachments Successfully");
                _logger.Info(string.Format("Email Text is: [{0}]", emailToISAdminText));

                // Send the mail
                emailSender.Send(msgForISAdminAlert);

                return true;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occurred while sending alert to IS Admin for XML validation failure failed", exception);
                return false;
            }

        }

        /// <summary>
        /// This function is used to get list of all iinet account ids.
        /// </summary>
        /// <param name="ichAccountId"></param>
        /// <param name="technicalConfiguration"></param>
        /// <returns></returns>
        //CMP #625: New Fields in ICH Member Profile Update XML
        private static List<IInetAccountId> GetListOfIInetAccountId(String ichAccountId, TechnicalConfiguration technicalConfiguration)
        {
          var iiNetAccountIds = new List<IInetAccountId>();

          if (technicalConfiguration != null)
          {
            //iinet account id will display in the XML configuration if these account id will not null or empty.
            if (!String.IsNullOrEmpty(technicalConfiguration.PaxAccountId))
              iiNetAccountIds.Add(new IInetAccountId { Value = technicalConfiguration.PaxAccountId, Type = "PAX" });

            if (!String.IsNullOrEmpty(technicalConfiguration.CgoAccountId))
              iiNetAccountIds.Add(new IInetAccountId { Value = technicalConfiguration.CgoAccountId, Type = "CGO" });

            if (!String.IsNullOrEmpty(technicalConfiguration.MiscAccountId))
              iiNetAccountIds.Add(new IInetAccountId { Value = technicalConfiguration.MiscAccountId, Type = "MISC" });

            if (!String.IsNullOrEmpty(technicalConfiguration.UatpAccountId))
              iiNetAccountIds.Add(new IInetAccountId { Value = technicalConfiguration.UatpAccountId, Type = "UATP" });
          }

          if (!String.IsNullOrEmpty(ichAccountId))
            iiNetAccountIds.Add(new IInetAccountId { Value = ichAccountId, Type = "ICH" });
          return iiNetAccountIds;
        }

    }
}
