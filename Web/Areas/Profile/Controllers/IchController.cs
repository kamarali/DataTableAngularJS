using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core.File;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Profile;
using Iata.IS.Web.Util;
using System;
using Iata.IS.Web.Util.Filters;
using log4net;
using System.Reflection;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
    public class IchController : ISController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMemberManager _memberManager;

        private readonly IBlockingRulesManager _blockingRulesManager;

        public IchController(IMemberManager memberManager, IBlockingRulesManager blockingRulesManager)
        {

            _memberManager = memberManager;
            _blockingRulesManager = blockingRulesManager;
        }
        [ISAuthorize(Business.Security.Permissions.IchOps.ManageBlocksAccess)]
        public ActionResult IchBlockingRules()
        {
            const string memberId = "";
            const string ruleName = "";
            const string description = "";
            ViewData["clearingHouse"] = "ICH";
            var blockingRulesGrid = new BlockingRules("BlockingRuleGrid", Url.Action("BlockingRulesGridData", "Ich", new { memberId, ruleName, description }));
            ViewData["BlockingRuleGrid"] = blockingRulesGrid.Instance;
            return View();
        }

        [ISAuthorize(Business.Security.Permissions.IchOps.ManageBlocksAccess)]
        public ActionResult GetIchBlockingRules()
        {
          // Get temp folder path for creating blocking rule csv report.
          var csvReportTempPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.ISBlockingRulesCsvFolder),Guid.NewGuid().ToString());

          Logger.InfoFormat("Temp File Path: [{0}]", csvReportTempPath);

          // Create csv report file name.
          var csvFileName = Path.Combine(csvReportTempPath,
                                         string.Format("ICH Blocking Rules Download-{0}{1}{2}-{3}{4}{5}.csv",
                                                       DateTime.UtcNow.Year.ToString().PadLeft(4, '0'),
                                                       DateTime.UtcNow.Month.ToString().PadLeft(2, '0'),
                                                       DateTime.UtcNow.Day.ToString().PadLeft(2, '0'),
                                                       DateTime.UtcNow.Hour.ToString().PadLeft(2, '0'),
                                                       DateTime.UtcNow.Minute.ToString().PadLeft(2, '0'),
                                                       DateTime.UtcNow.Second.ToString().PadLeft(2, '0')));

          Logger.InfoFormat("Report File Name: [{0}]", csvFileName);
          
          // Create temp folder if it doesnot exists.
          if (!Directory.Exists(csvReportTempPath))
            Directory.CreateDirectory(csvReportTempPath);

          // Get csv report data.
          var ichBlockingRules = _blockingRulesManager.GetBlokingRulesForClearingHouse("ICH") ??
                                 new List<DownloadBlockingRules>();

          var csvGenerator = new CsvGenerator();
          
          // Generate Csv Report.
          csvGenerator.GenerateCSV(ichBlockingRules, csvFileName);

          // Create Zip Report.
          var zipFileName = FileIo.ZipOutputFile(csvFileName);

          Logger.InfoFormat("Report Zip File Name: [{0}]",zipFileName);

          return File(zipFileName, "application/zip", Server.HtmlEncode(Path.GetFileName(zipFileName)));
        }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult IchBlockingRules(BlockingRule blockingRule)
        {
            const string memberId = "";
            BlockingRules blockingRulesGrid;
            var rulenameStringBuilder = new StringBuilder(HttpUtility.HtmlEncode(blockingRule.RuleName));
            rulenameStringBuilder.Replace("'", string.Empty);
            blockingRule.RuleName = rulenameStringBuilder.ToString();


            var descriptionStringBuilder = new StringBuilder(HttpUtility.HtmlEncode(blockingRule.Description));
            descriptionStringBuilder.Replace("'", string.Empty);


            blockingRule.Description = descriptionStringBuilder.ToString();



            if (blockingRule.MemberId != 0)
            {

                blockingRulesGrid = new BlockingRules("BlockingRuleGrid",
                                                         Url.Action("BlockingRulesGridData", "Ich",
                                                                    new
                                                                      {
                                                                          blockingRule.MemberId,
                                                                          blockingRule.RuleName,
                                                                          blockingRule.Description
                                                                      }));
            }
            else
            {
                blockingRulesGrid = new BlockingRules("BlockingRuleGrid",
                                                       Url.Action("BlockingRulesGridData", "Ich",
                                                                  new
                                                                    {
                                                                        memberId,
                                                                        blockingRule.RuleName,
                                                                        blockingRule.Description
                                                                    }));
            }

            ViewData["BlockingRuleGrid"] = blockingRulesGrid.Instance;
            ViewData["clearingHouse"] = "ICH";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BlockingRuleDetails(BlockingRule blockingRule, IList<BlockMember> items, IList<BlockGroup> blockGroupItems, IList<BlockGroupException> blockGroupExceptionItems)
        {
            try
            {
                string message = "Blocking Rule saved successfully.";
                if (blockingRule.Id != 0)
                    message = "Blocking Rule updated successfully. ";

                blockingRule = _blockingRulesManager.UpdateBlockingRule(blockingRule);

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        // if IsDeleted is true means it has been deleted client side and should not be 
                        // added in database.
                        if (item.IsDeleted)
                            continue;

                        var blockMember = new BlockMember();

                        blockMember.Pax = item.Pax;
                        blockMember.Cargo = item.Cargo;
                        blockMember.IsDebtors = item.IsDebtors;
                        blockMember.Uatp = item.Uatp;
                        blockMember.Misc = item.Misc;
                        blockMember.BlockingRuleId = blockingRule.Id;
                        blockMember.MemberId = item.MemberId;

                        _blockingRulesManager.AddBlockMember(blockMember);
                    }
                }

                // Check whether user has added any Block Group items, if yes add these items to database
                if (blockGroupItems != null)
                {
                    // Iterate through Block Group Items
                    foreach (var groupItem in blockGroupItems)
                    {
                        if (groupItem.IsDeleted)
                            continue;

                        // Instantiate Block Group
                        var blockGroup = new BlockGroup();

                        // Set ZoneTypeId property
                        blockGroup.ZoneTypeId = groupItem.ZoneTypeId;
                        // Set ByAgainst property
                        blockGroup.IsBlockAgainst = groupItem.ByAgainst == 1 ? true : false;
                        // Set Pax property
                        blockGroup.Pax = groupItem.Pax;
                        // Set Cargo property
                        blockGroup.Cargo = groupItem.Cargo;
                        // Set Uatp property
                        blockGroup.Uatp = groupItem.Uatp;
                        // Set Misc property
                        blockGroup.Misc = groupItem.Misc;
                        // Set BlockingRuleId property
                        blockGroup.BlockingRuleId = blockingRule.Id;

                        // Call AddBlockGroup() method which will save Block group records to database
                        var result = _blockingRulesManager.AddBlockGroup(blockGroup);

                        // Check whether user has added any BlockGroupExceptions, if yes add to Database.
                        if (blockGroupExceptionItems != null)
                        {
                            // Iterate through BlockGrupException list  
                            foreach (var exceptionGroupItem in blockGroupExceptionItems)
                            {
                                if (exceptionGroupItem.IsDeleted)
                                    continue;

                                if (exceptionGroupItem.BlockGroupId == groupItem.TempRowCount)
                                {
                                    // Instantiate BlockGroupException
                                    var blockGroupException = new BlockGroupException();
                                    // Set BlockGroupId  
                                    blockGroupException.BlockGroupId = result.Id;
                                    // Set ExceptionMemberId
                                    blockGroupException.ExceptionMemberId = exceptionGroupItem.ExceptionMemberId;
                                    // Call AddBlockGroupException() method which will add BlockGroupException to database. 
                                    _blockingRulesManager.AddBlockGroupException(blockGroupException);
                                }// end if()
                            }// end foreach()   
                        }// end if()

                    }// end foreach()
                }// end if()

                // Check whether user has added new BlockRuleException against BlockGroup, if yes add Exception to Database.
                if (blockGroupExceptionItems != null)
                {
                    // Iterate through BlockGroupExceptionItems list
                    foreach (var exceptionGroupItem in blockGroupExceptionItems)
                    {
                        // Check whether Exception row is deleted on client side, if yes do not add the record to DB.
                        if (exceptionGroupItem.IsDeleted)
                            continue;

                        // Check whether BlockGroupId != 0, if true add BlockGroupException to Database  
                        if (exceptionGroupItem.BlockGroupId != 0)
                        {
                            // Instantiate BlockGroupException
                            var blockGroupException = new BlockGroupException();
                            // Set BlockGroupId  
                            blockGroupException.BlockGroupId = exceptionGroupItem.BlockGroupId;
                            // Set ExceptionMemberId
                            blockGroupException.ExceptionMemberId = exceptionGroupItem.ExceptionMemberId;
                            // Call AddBlockGroupException() method which will add BlockGroupException to database. 
                            _blockingRulesManager.AddBlockGroupException(blockGroupException);
                        }// end if()
                    }// end foreach()
                }// end if()

                // Check whether user has deleted any Creditors row, if yes delete from database 
                if (blockingRule.DeletedBlockedCreditorString != null)
                {
                    // Split Creditor string on comma
                    string[] deletedCreditorsMemberId = blockingRule.DeletedBlockedCreditorString.Split(',');

                    // Iterate through the ID array and call "DeleteBlockedMember" passing it Id.
                    foreach (var deletedCreditor in deletedCreditorsMemberId)
                    {
                        // Execute DeleteBlockedMember action which deletes record from database 
                        DeleteBlockedMember(deletedCreditor);
                    }// end foreach()
                }// end if()

                // Check whether user has deleted any Debtors row, if yes delete from database 
                if (blockingRule.DeletedBlockedDebtorString != null)
                {
                    // Split Debtor string on comma
                    string[] deletedDebtorsMemberId = blockingRule.DeletedBlockedDebtorString.Split(',');

                    // Iterate through the ID array and call "DeleteBlockedMember" passing it Id.
                    foreach (var deletedCreditor in deletedDebtorsMemberId)
                    {
                        // Execute DeleteBlockedMember action which deletes record from database 
                        DeleteBlockedMember(deletedCreditor);
                    }// end foreach()
                }// end if()

                // Check whether user has deleted any Exception row, if yes delete from database 
                if (blockingRule.DeletedExceptionRowString != null)
                {
                    // Split ExceptionId string on comma
                    string[] deletedExceptionId = blockingRule.DeletedExceptionRowString.Split(',');

                    // Iterate through the ID array and call "DeleteBlockedGroupException" passing it ExceptionId.
                    foreach (var deletedException in deletedExceptionId)
                    {
                        string[] group = deletedException.Split('|');
                        // Execute DeleteBlockedGroupException action which deletes record from database 
                        DeleteBlockedGroupException(group[0], group[1]);
                    }// end foreach()
                }// end if()

                // Check whether user has deleted any Exception row, if yes delete from database 
                if (blockingRule.DeletedGroupByBlockString != null)
                {
                    // Split ExceptionId string on comma
                    string[] deletedGroupByBlockId = blockingRule.DeletedGroupByBlockString.Split(',');

                    // Iterate through the ID array and call "DeleteBlockedGroupException" passing it ExceptionId.
                    foreach (var deletedGroup in deletedGroupByBlockId)
                    {
                        // Execute DeleteBlockedGroupException action which deletes record from database 
                        DeleteBlockedGroup(deletedGroup);
                    }// end foreach()
                }// end if()

                TempData[ViewDataConstants.SuccessMessage] = "Success";
                ShowSuccessMessage(message);
                ViewData["Id"] = blockingRule.Id;
                ViewData["ClearingHouse"] = "ICH";
                // Set Edit mode flag to true
                ViewData["IsInEditMode"] = true;

                ViewData["BlockedCreditorsCount"] = _blockingRulesManager.GetBlockMemberCount(blockingRule.Id, false);
                ViewData["BlockedDebitorsCount"] = _blockingRulesManager.GetBlockMemberCount(blockingRule.Id, true);
                ViewData["BlockedGroupCount"] = _blockingRulesManager.GetBlockGroupCount(blockingRule.Id);

                // Call GenerateBlockingRuleUpdateXml() method which will generate Xml for Blockingrule update
                _blockingRulesManager.GenerateBlockingRuleUpdateXml(blockingRule.Id);
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }

            return View(blockingRule);
        }

        public ActionResult BlockedCreditors(int blockingRuleId, int memberId)
        {
            var isDebtor = false;
            var gridModel = new BlockedCreditorsDebtors("BlockedCreditorsGrid", Url.Action("BlockedCreditorsDebitorsGridData", new { blockingRuleId, memberId, isDebtor }));
            gridModel.Instance.ID = "BlockedCreditorsGrid";
            ViewData["BlockedCreditorsDebtorsGrid"] = gridModel.Instance;
            return PartialView("BlockedCreditorsControl");
        }

        public ActionResult BlockedDebitors(int blockingRuleId, int memberId)
        {
            var isDebtor = true;
            var gridModel = new BlockedCreditorsDebtors("BlockedDebtorsGrid", Url.Action("BlockedCreditorsDebitorsGridData", new { blockingRuleId, memberId, isDebtor }));
            gridModel.Instance.ID = "BlockedDebtorsGrid";
            ViewData["BlockedCreditorsDebtorsGrid"] = gridModel.Instance;
            return PartialView("BlockedDebtorsControl");
        }

        public ActionResult BlocksByGroup(int blockingRuleId)
        {
            var groupIds = "0";
            var blocksgrid = new BlocksByGroupBlocks("BlocksByGroupBlocksGrid", Url.Action("BlockesByGroupBlocksGridData", new { blockingRuleId }));
            ViewData["BlocksByGroupBlocksGrid"] = blocksgrid.Instance;

            var exceptionsgrid = new BlocksbyGroupExceptions("BlocksByGroupExceptionsGrid", Url.Action("BlockesByGroupExceptionsGridData", new { groupIds }));
            ViewData["BlocksByGroupExceptionsGrid"] = exceptionsgrid.Instance;

            return PartialView("BlocksByGroupControl");
        }

        public JsonResult BlockedCreditorsDebitorsGridData(int blockingRuleId, bool isDebtor)
        {
            if (isDebtor)
            {
                var gridModel = new BlockedCreditorsDebtors("BlockedDebtorsGrid", Url.Action("BlockedCreditorsDebitorsGridData", new { blockingRuleId, isDebtor }));
                var blockGroupList = _blockingRulesManager.GetBlockMemberList(blockingRuleId, true);
                return gridModel.DataBind(blockGroupList.AsQueryable());
            }
            else
            {
                var gridModel = new BlockedCreditorsDebtors("BlockedCreditorsGrid", Url.Action("BlockedCreditorsDebitorsGridData", new { blockingRuleId, isDebtor }));
                var blockGroupList = _blockingRulesManager.GetBlockMemberList(blockingRuleId, false);
                return gridModel.DataBind(blockGroupList.AsQueryable());
            }

        }

        public JsonResult BlockesByGroupBlocksGridData(int blockingRuleId)
        {
            var gridModel = new BlocksByGroupBlocks("BlocksByBlocksGrid", Url.Action("BlockesByGroupBlocksGridData", new { blockingRuleId }));
            var blockGroupList = _blockingRulesManager.GetBlockGroupList(blockingRuleId);

            // Iterate through blockGroupList and set ByAgainstString to "By" if IsBlockAgainst == false, else to Against 
            foreach (var blockGroupItem in blockGroupList)
            {
                blockGroupItem.ByAgainstString = blockGroupItem.IsBlockAgainst ? "Against" : "By";
             // blockGroupItem.ZoneTypeId = 2;
            }// end foreach()

            return gridModel.DataBind(blockGroupList.AsQueryable());

        }

        public JsonResult BlockesByGroupExceptionsGridData(string groupId)
        {
            var groupIds = 0;
            if (groupId != null)
                groupIds = int.Parse(groupId);
            var gridModel = new BlocksbyGroupExceptions("BlocksByGroupExceptionsGrid", Url.Action("BlockesByGroupExceptionsGridData", new { groupIds }));
            var blockGroupExceptionsList = _blockingRulesManager.GetBlockGroupExceptionsList(groupIds);
            return gridModel.DataBind(blockGroupExceptionsList.AsQueryable());
        }

        [HttpPost]
        public JsonResult AddBlockedMember(string memberId, bool pax, bool cgo, bool uatp, bool misc, bool isDebtor, string blockingRuleId)
        {
            var blockingRules = int.Parse(blockingRuleId);
            var memberIds = int.Parse(memberId);

            var blockMember = new BlockMember();
            blockMember.MemberId = memberIds;
            blockMember.Pax = pax;
            blockMember.Cargo = cgo;
            blockMember.IsDebtors = isDebtor;
            blockMember.Uatp = uatp;
            blockMember.Misc = misc;
            blockMember.BlockingRuleId = blockingRules;
            var result = _blockingRulesManager.AddBlockMember(blockMember);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddBlockedGroup(int blockedAgainst, string zoneId, bool pax, bool cgo, bool uatp, bool misc, string blockingRuleId)
        {
            var blockGroup = new BlockGroup();

            if (zoneId == "")
                zoneId = "0";
            blockGroup.ZoneTypeId = int.Parse(zoneId);
            blockGroup.ByAgainst = blockedAgainst;
            blockGroup.Pax = pax;
            blockGroup.Cargo = cgo;
            blockGroup.Uatp = uatp;
            blockGroup.Misc = misc;
            blockGroup.BlockingRuleId = int.Parse(blockingRuleId);
            var result = _blockingRulesManager.AddBlockGroup(blockGroup);
            return Json(result);
        }

        public JsonResult BlockingRulesGridData(string memberId, string ruleName, string description)
        {
            var blockingRulesGrid = new BlockingRules("BlockingRuleGrid", Url.Action("BlockingRulesGridData", new { memberId, ruleName, description }));
            var blockingRules = _blockingRulesManager.GetBlockingRuleList(memberId, ruleName, description, "ICH");
            try
            {
                return blockingRulesGrid.DataBind(blockingRules.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }
        [HttpPost]
        public JsonResult DeleteBlockingRule(string id)
        {
            UIExceptionDetail details;

            try
            {
                var isDeleted = _blockingRulesManager.DeleteBlockingRule(int.Parse(id));
                details = isDeleted ?
                                      new UIExceptionDetail
                                      {
                                          IsFailed = false,
                                          Message = string.Format(Messages.BPAXNS_10669)
                                      } :
                                          new UIExceptionDetail
                                          {
                                              IsFailed = true,
                                              Message = string.Format(Messages.BPAXNS_10670)
                                          };
            }
            catch (ISBusinessException)
            {
                details = new UIExceptionDetail
                {
                    IsFailed = true,
                    Message = string.Format(Messages.InvoiceDeleteFailed)
                };
            }
            return Json(details);
        }

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.IchOps.ManageBlocksAccess)]
        public ActionResult BlockingRuleEdit(string id)
        {
            int blockingRuleId = int.Parse(id);
            var blockingRuleRecord = _blockingRulesManager.GetBlockingRuleDetails(blockingRuleId);

            ViewData["BlockedCreditorsCount"] = _blockingRulesManager.GetBlockMemberCount(blockingRuleId, false);
            ViewData["BlockedDebitorsCount"] = _blockingRulesManager.GetBlockMemberCount(blockingRuleId, true);
            ViewData["BlockedGroupCount"] = _blockingRulesManager.GetBlockGroupCount(blockingRuleId);

            // Set Edit mode flag to true
            blockingRuleRecord.IsInEditMode = true;
            ViewData["IsInEditMode"] = true;
            ViewData["Id"] = id;
            return View("BlockingRuleDetails", blockingRuleRecord);
        }

        [HttpGet]
        public ActionResult BlockingRuleDetails()
        {
            var blocksgrid = new BlockingRules("BlockingRuleGrid", Url.Action("BlockingRulesGridData"));
            ViewData["BlockingRulesGrid"] = blocksgrid.Instance;
            ViewData["ClearingHouse"] = "ICH";
            return View();
        }

        [HttpPost]
        public JsonResult DeleteBlockedGroup(string id)
        {
            UIExceptionDetail details;

            try
            {
                //Delete record
                var isDeleted = _blockingRulesManager.DeleteBlockedGroup(int.Parse(id));

                details = isDeleted ?
                                      new UIExceptionDetail
                                      {
                                          IsFailed = false,
                                          Message = string.Format(Messages.BPAXNS_10663)
                                      } :
                                          new UIExceptionDetail
                                          {
                                              IsFailed = true,
                                              Message = string.Format(Messages.BPAXNS_10664)
                                          };
            }
            catch (ISBusinessException)
            {
                details = new UIExceptionDetail
                {
                    IsFailed = true,
                    Message = string.Format(Messages.BPAXNS_10619)
                };
            }

            return Json(details);
        }

        [HttpPost]
        public JsonResult DeleteBlockedMember(string id)
        {
            UIExceptionDetail details;
            try
            {

                var isDeleted = _blockingRulesManager.DeleteBlockedMember(int.Parse(id));
                details = isDeleted ?
                                       new UIExceptionDetail
                                       {
                                           IsFailed = false,
                                           Message = string.Format(Messages.BPAXNS_10665)
                                       } :
                                           new UIExceptionDetail
                                           {
                                               IsFailed = true,
                                               Message = string.Format(Messages.BPAXNS_10666)
                                           };
            }
            catch (ISBusinessException)
            {
                details = new UIExceptionDetail
                {
                    IsFailed = true,
                    Message = string.Format(Messages.InvoiceDeleteFailed)
                };
            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult DeleteBlockedGroupException(string blockGroupId, string exceptionId)
        {
            UIExceptionDetail details;

            try
            {
                //Delete record
                var isDeleted = _blockingRulesManager.DeleteBlockedGroupException(int.Parse(blockGroupId), int.Parse(exceptionId));

                details = isDeleted ?
                                      new UIExceptionDetail
                                      {
                                          IsFailed = false,
                                          Message = string.Format(Messages.BPAXNS_10667)
                                      } :
                                          new UIExceptionDetail
                                          {
                                              IsFailed = true,
                                              Message = string.Format(Messages.BPAXNS_10668)
                                          };
            }
            catch (ISBusinessException)
            {
                details = new UIExceptionDetail
                {
                    IsFailed = true,
                    Message = string.Format(Messages.InvoiceDeleteFailed)
                };
            }

            return Json(details);
        }

        [HttpPost]
        public JsonResult AddBlockedGroupException(string groupId, string memberId)
        {
            var blockGroupException = new BlockGroupException();
            blockGroupException.BlockGroupId = int.Parse(groupId);
            blockGroupException.ExceptionMemberId = int.Parse(memberId);
            var result = _blockingRulesManager.AddBlockGroupException(blockGroupException);

            return Json(result);
        }

        /// <summary>
        /// Following action is used to get Member commercial name.
        /// </summary>
        /// <param name="memberId">MemberId whose details are to be retrieved</param>
        /// <returns>Members Commercial name</returns>
        public string GetMemberDetails(string memberId)
        {
            // Call GetMember() which returns member details
            var result = _memberManager.GetMember(Convert.ToInt32(memberId));

            // return Commercial name from Member details
            return string.Format("{0}  {1} {2} {3} {4}", result.CommercialName, "####", result.MemberCodeAlpha, "####", result.MemberCodeNumeric);
        }

        /// <summary>
        /// Following action is used retrieve Blocked Creditors list i.e. used to create string of MemberId's  
        /// </summary>
        /// <param name="blockingRuleId">Blocking rule id</param>
        /// <returns>String of MemberId's</returns>
        public string GetBlockedCreditorsId(int blockingRuleId)
        {
            // Declare string variable to capture MemberId's
            string memberIdString = string.Empty;

            // Retrieve BlockMember list for Creditors
            var blockGroupList = _blockingRulesManager.GetBlockMemberList(blockingRuleId, false);

            // Iterate through list, retrieve MemberId and append it to string variable
            foreach (var blockGroup in blockGroupList)
            {
                memberIdString += "," + blockGroup.MemberId;
            }// end foreach()

            // return MemberId's string
            return memberIdString;
        }// end GetBlockedCreditorsId()

        /// <summary>
        /// Following action is used retrieve Blocked Debtors list i.e. used to create string of MemberId's  
        /// </summary>
        /// <param name="blockingRuleId">Blocking rule id</param>
        /// <returns>String of MemberId's</returns>
        public string GetBlockedDebtorsId(int blockingRuleId)
        {
            // Declare string variable to capture MemberId's
            string memberIdString = string.Empty;

            // Retrieve BlockMember list for Debtors
            var blockGroupList = _blockingRulesManager.GetBlockMemberList(blockingRuleId, true);

            // Iterate through list, retrieve MemberId and append it to string variable
            foreach (var blockGroup in blockGroupList)
            {
                memberIdString += "," + blockGroup.MemberId;
            }// end foreach()

            // return MemberId's string
            return memberIdString;
        }// end GetBlockedDebtorsId()

        /// <summary>
        /// Following action is used retrieve BlockByGroups list i.e. used to create string of ZoneId+ByAgainst  
        /// </summary>
        /// <param name="blockingRuleId">Blocking rule id</param>
        /// <returns>String of ZoneId+ByAgainst</returns>
        public string GetBlockByGroupsZoneId(int blockingRuleId)
        {
            // Declare string variable to capture ZoneId's
            string zoneIdString = string.Empty;

            // Retrieve Block by Groups list
            var blockGroupList = _blockingRulesManager.GetBlockGroupList(blockingRuleId);

            // Iterate through list, retrieve ZoneId. ByAgainst and append it to string variable
            foreach (var blockGroup in blockGroupList)
            {
                string byAgainst = blockGroup.IsBlockAgainst ? "1" : "0";

                zoneIdString += "," + blockGroup.ZoneTypeId + byAgainst;
            } // end foreach()

            // return ZoneId+ByAgainst string
            return zoneIdString;
        }// end GetBlockByGroupsZoneId()

        public string BlockesByGroupExceptionsIdString(string groupId)
        {
            var groupIds = 0;
            string exceptionIdString = string.Empty;
            if (groupId != null)
                groupIds = int.Parse(groupId);

            var blockGroupExceptionsList = _blockingRulesManager.GetBlockGroupExceptionsList(groupIds);

            foreach (var exceptionItem in blockGroupExceptionsList)
            {
                if (exceptionIdString == "")
                    exceptionIdString = exceptionItem.ExceptionMemberId.ToString();
                else
                    exceptionIdString += "," + exceptionItem.ExceptionMemberId;
            }// end foreach()

            return exceptionIdString;
        }// end BlockesByGroupExceptionsIdString

        /// <summary>
        /// Following action is used to Update Blocking member. i.e. Pax, Cargo etc checkbox values
        /// </summary>
        /// <param name="checkboxList">String of checkbox values</param>
        public void UpdateBlockingMember(string checkboxList)
        {
            // Split checkbox values string and add to array
            string[] checkBoxValueArray = checkboxList.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Iterate through each item of array and retrieve each billing category value
            foreach (var chkValue in checkBoxValueArray)
            {
                // Split each row checkbox values
                string[] blockMemberRow = chkValue.Split("!".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (blockMemberRow.Length == 2)
                {
                    // Get current Block member rows memberId
                    var memberId = int.Parse(blockMemberRow[0]);

                    // If memberId != 0, i.e. it is record from database. Update Billing category checkbox values 
                    if (memberId != 0)
                    {
                        // Split each Billing category value
                        string[] eachrow = blockMemberRow[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        var blockMember = new BlockMember();
                        blockMember.MemberId = memberId;
                        blockMember.Pax = Convert.ToBoolean(eachrow[0].Substring(4));
                        blockMember.Cargo = Convert.ToBoolean(eachrow[1].Substring(6));
                        blockMember.Uatp = Convert.ToBoolean(eachrow[2].Substring(5));
                        blockMember.Misc = Convert.ToBoolean(eachrow[3].Substring(5));

                        // Update Block member
                        _blockingRulesManager.UpdateBlockMember(blockMember);
                    }// end if()

                }// end if()
            }// foreach()
        }// end UpdateBlockingMember()

        /// <summary>
        /// Following action is used to Update Blocking Group. i.e. Pax, Cargo etc checkbox values
        /// </summary>
        /// <param name="blockGroupCheckboxList">String of checkbox values</param>
        public void UpdateBlockGroup(string blockGroupCheckboxList)
        {
            // Split checkbox values string and add to array
            string[] checkBoxValueArray = blockGroupCheckboxList.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Iterate through each item of array and retrieve each billing category value
            foreach (var chkValue in checkBoxValueArray)
            {
                // Split each row checkbox values
                string[] blockMemberRow = chkValue.Split("!".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (blockMemberRow.Length == 2)
                {
                    // Temporary rows on BlockByGroup grid contains "#", if present continue
                    if (blockMemberRow[0].Contains("#"))
                        continue;

                    // Get current Block Group rows groupId
                    var groupId = int.Parse(blockMemberRow[0]);

                    // If groupId != 0, i.e. it is record from database. Update Billing category checkbox values 
                    if (groupId != 0)
                    {
                        // Split each Billing category value
                        string[] eachrow = blockMemberRow[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        var blockGroup = new BlockGroup();
                        blockGroup.Id = groupId;
                        blockGroup.Pax = Convert.ToBoolean(eachrow[0].Substring(4));
                        blockGroup.Cargo = Convert.ToBoolean(eachrow[1].Substring(6));
                        blockGroup.Misc = Convert.ToBoolean(eachrow[3].Substring(5));
                        blockGroup.Uatp = Convert.ToBoolean(eachrow[2].Substring(5));

                        // Update Block Group
                        _blockingRulesManager.UpdateBlockGroup(blockGroup);
                    }// end if()

                }// end if()
            }// foreach()
        }// end UpdateBlockGroup()
    }
}
