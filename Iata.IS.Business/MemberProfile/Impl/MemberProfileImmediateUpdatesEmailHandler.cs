using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Iata.IS.Core;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Business.TemplatedTextGenerator;
using NVelocity;
using Castle.Core.Smtp;
using Iata.IS.Data;
using iPayables.UserManagement;
using log4net;
using System.Reflection;
using System.Net.Mail;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.MemberProfile.Impl
{
    internal class MemberProfileImmediateUpdatesEmailHandler : IMemberProfileImmediateUpdatesEmailHandler
    {
        public List<FutureUpdates> UpdatesList { get; set; }

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool SendMailsForASingleMember(List<FutureUpdates> updateList)
        {
            try
            {
                //declare an object of the nVelocity data dictionary
                VelocityContext context;
                VelocityContext htmlContentContext;
                //get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

                //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
                var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

                //get an object of the member manager
                var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

                //get an instance of email settings  repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

                //get ISUser Object (this will provide us email address of member profile updating users)
                var iSUser = Ioc.Resolve<I_ISUser>(typeof(I_ISUser));

                //get ISUser Object (this will provide us email address of member profile updating users)
                var memberLocationUpdateHandler = Ioc.Resolve<IMemberLocationUpdateHandler>(typeof(IMemberLocationUpdateHandler));

                //get an instance of element group repository
                var memberProfileMedtadataRepository = Ioc.Resolve<IRepository<ProfileMetadata>>(typeof(IRepository<ProfileMetadata>));

                //send mails to Own profile Update contact type contacts

                var commonFutureUpdateProperties = updateList.First();

                //get list of contacts of contact type own profile updates for this member
                var ownProfileUpdateContactList = memberManager.GetContactsForContactType(commonFutureUpdateProperties.MemberId, ProcessingContactType.OwnProfileUpdates);

                //get user data corresponding to user who made the change
                iSUser.UserID = commonFutureUpdateProperties.LastUpdatedBy;

                //get an object of the future update manager.
                var futureUpdateManager = Ioc.Resolve<IFutureUpdatesManager>(typeof(IFutureUpdatesManager));

                // Fix for Issue ID. 5747
                // Member should be notified of Member Profile changes done only for fields that are visible to them. 
                // Fields updated by SIS Ops (like the IS Membership Sub Status) should not cause an email alert.
                var futureupdateListOwnProfileUpdates = new List<object>();
                var futureupdateListSisOps = new List<object>();

                foreach (FutureUpdates fu in updateList)
                {
                    if (fu.RelationId.HasValue)
                    {
                        fu.RelationIdDisplayName = futureUpdateManager.GetRelationIdDisplayName(fu.Id);
                    }

                    var memberProfileMedtadata = memberProfileMedtadataRepository.Get(pmd => pmd.TableName == fu.TableName && pmd.FieldName == fu.ElementName);

                    if (memberProfileMedtadata == null || memberProfileMedtadata.Count() == 0)
                    {
                        continue;
                    }

                    var metaData = memberProfileMedtadata.FirstOrDefault();

                    var membercodeNumeric = string.Empty;
                    var memberCodeAlpha = string.Empty;
                    if (fu.Member == null)
                    {
                        _logger.Info("Future Update Member ID" + fu.MemberId);
                        var memberDetail = memberManager.GetMemberDetails(fu.MemberId);
                        if (memberDetail != null)
                        {
                            _logger.Info("memberDetail.MemberCodeNumeric :" + memberDetail.MemberCodeNumeric);
                            membercodeNumeric = memberDetail.MemberCodeNumeric;
                            memberCodeAlpha = memberDetail.MemberCodeAlpha;
                        }
                    }
                    else
                    {
                        membercodeNumeric = fu.Member.MemberCodeNumeric;
                        memberCodeAlpha = fu.Member.MemberCodeAlpha;    
                    }
                    

                    var futureUpdate = new
                                         {
                                             membercodeNumeric,
                                             memberCodeAlpha,
                                             fu.DisplayGroup,
                                             ElementName = metaData.FieldDisplayName,
                                             fu.RelationIdDisplayName,
                                             fu.ActionType,
                                             fu.OldValueDisplayName,
                                             fu.NewValueDisplayName
                                         };

                    // Include all future update fields for attachment sent to Sis Ops.
                    futureupdateListSisOps.Add(futureUpdate);

                    // If field is viewable by member user, only then include it in mailing list of field
                    if (metaData.IsViewableToMember)
                    {
                        futureupdateListOwnProfileUpdates.Add(futureUpdate);
                    }
                }

                //get member details for member represented by UpdateList
                var memberCommercialName = string.Empty;
                var memberCodePrefix = string.Empty;
                var memberCodeDesignator = string.Empty;

                if (commonFutureUpdateProperties.Member == null)
                {
                    if (commonFutureUpdateProperties.MemberId > 0)
                    {
                        memberCommercialName = memberManager.GetMemberCommercialName(commonFutureUpdateProperties.MemberId);
                        memberCodePrefix = memberManager.GetMemberCode(commonFutureUpdateProperties.MemberId);
                        memberCodeDesignator = memberManager.GetMemberCodeAlpha(commonFutureUpdateProperties.MemberId);
                    }
                }
                else
                {
                    memberCommercialName = commonFutureUpdateProperties.Member.CommercialName;
                    memberCodePrefix = commonFutureUpdateProperties.Member.MemberCodeNumeric;
                    memberCodeDesignator = commonFutureUpdateProperties.Member.MemberCodeAlpha;
                }

                //object of the nVelocity data dictionary
                context = new VelocityContext();
                htmlContentContext = new VelocityContext();
                context.Put("MemberCommercialName", memberCommercialName);
                context.Put("MemberPrefix", memberCodePrefix);
                context.Put("MemberDesignator", memberCodeDesignator);
                context.Put("EffectiveOn", DateTime.UtcNow.ToString("dd-MMM-yy"));
                context.Put("SISOpsEmail", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                context.Put("n", "\n");

                //Get the eMail settings for member profile future update mail, to be sent to user updating the data
                var emailSettings = emailSettingsRepository.Get(esfc => esfc.Id == (int)EmailTemplateId.MemberProfileUpdates);
                var emailSettingsHtmlContent = emailSettingsRepository.Get(esfc => esfc.Id == (int)EmailTemplateId.MemberProfileUpdatesHtmlContents);

                //contacts of type Own Profile Update are found
                if ((ownProfileUpdateContactList != null && ownProfileUpdateContactList.Count > 0) && (futureupdateListOwnProfileUpdates.Count() > 0))
                {
                    if (iSUser.FirstName == null)
                    {
                        _logger.Info("First Name is null");
                    }
                    if (iSUser.LastName == null)
                    {
                        _logger.Info("Last Name is null");
                    }


                    //fill nVelocity data dictionary with data specific to template used for own profile updates contact type mail
                    htmlContentContext.Put("FutureUpdates", futureupdateListOwnProfileUpdates);
                    var emailContentforAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberProfileUpdatesHtmlContents, htmlContentContext);

                    var emailTextforContact = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberProfileUpdates, context);


                    //create a mail object to send mail to user who had updated member profile data
                    var msgForOwnprofileUpdate = new MailMessage
                    {
                        From = new MailAddress(emailSettings.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = false,
                        Subject = emailSettings.SingleOrDefault().Subject,
                        Body = emailTextforContact
                    };

                    //loop through the contacts list and add them to To list of mail to be sent
                    foreach (var contact in ownProfileUpdateContactList)
                    {
                        msgForOwnprofileUpdate.To.Add(new MailAddress(contact.EmailAddress));
                    }

                    var attachmentPath = Path.Combine(Path.GetTempPath(), memberCodePrefix + " - List of Member profile changes initiated.htm");
                    //set body text of mail

                    StreamWriter contactAttachmentFileStream = new StreamWriter(attachmentPath, false);

                    contactAttachmentFileStream.WriteLine(emailContentforAttachment);
                    contactAttachmentFileStream.Close();
                    msgForOwnprofileUpdate.Attachments.Add(new Attachment(attachmentPath));

                    //send the mail
                    emailSender.Send(msgForOwnprofileUpdate);

                }

                memberLocationUpdateHandler.LocationUpdateSenderForImmediateUpdates(commonFutureUpdateProperties.MemberId, updateList);

                //fill nVelocity data dictionary with data specific to template, used for sending mail to user who had updated member profile data
                htmlContentContext.Put("FutureUpdates", futureupdateListSisOps);

                //generate email body text for mail for user who had updated member profile data
                var emailTextforChanger = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberProfileUpdates, context);
                var emailContentforAttachmentforChanger = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberProfileUpdatesHtmlContents, htmlContentContext);

                //create a mail object to send mail to user who had updated member profile data
                var msgForChanger = new MailMessage
                {
                    From = new MailAddress(emailSettings.SingleOrDefault().FromEmailAddress),
                    IsBodyHtml = false,
                    Subject = emailSettings.SingleOrDefault().Subject,
                    Body = emailTextforChanger
                };

                var changerattachmentPath = Path.Combine(Path.GetTempPath(), memberCodePrefix + " - List of Member profile changes initiated.htm");
                //set body text of mail
                StreamWriter changerAttachmentFileStream = new StreamWriter(changerattachmentPath, false);

                changerAttachmentFileStream.WriteLine(emailContentforAttachmentforChanger);
                changerAttachmentFileStream.Close();
                msgForChanger.Attachments.Add(new Attachment(changerattachmentPath));

                //add email of user to 'To' list of mail to be sent
                msgForChanger.To.Add(new MailAddress(iSUser.Email));

                //send the mail to the user who had updated member profile data
                emailSender.Send(msgForChanger);

                return true;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occurred in Member Profile Immediate Updates Email Handler (Send Mails for a Single Member method).", exception);
                return false;
            }
        }

        public bool SendMailsForImmediateMemberProfileUpdates()
        {
            try
            {
                if (UpdatesList == null)
                {
                    _logger.Info("UpdatesList is null");
                    return false;
                }

                //get the distinct list of members for which updates were done by the service
                var distinctMemberIds = (from fu in UpdatesList
                                         select fu.MemberId).Distinct();

                foreach (var memId in distinctMemberIds)
                {
                    //Now get the future updates for this particular member
                    bool _return = SendMailsForASingleMember((from mfu in UpdatesList
                                                              where mfu.MemberId == memId
                                                              select mfu).ToList());

                    if (!_return)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occurred in Member Profile Immediate Updates Email Handler.", exception);
                return false;
            }
        }
    }
}