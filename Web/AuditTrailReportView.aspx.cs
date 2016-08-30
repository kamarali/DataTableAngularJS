using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Reports.Enums;
using Iata.IS.Web.Util;
using iPayables.UserManagement;
using log4net;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Web
{
    public partial class AuditTrailReportView : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ICalendarManager CalendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        protected void Page_Load(object sender, EventArgs e)
        {

            // redirect to Login screen in case unauthorized/anonymous user access 
            // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }


            try
            {
                _logger.Info("Audit Trail Report Initiated");
                
                lblErrorMsg.Visible = false;

                List<FutureUpdateDetails> listModel = null;

                int user = -1;
                int memberId = -1;
                string groupList;
                int isDateOrPeriodSearch;
                ClearingHouse membsrClearingHouse = new ClearingHouse();

                var dateInfo = new DateTimeFormatInfo { ShortDatePattern = FormatConstants.DateFormat };

                DateTime? fromDateOrPeriod = Convert.ToDateTime(Request.QueryString["fdate"], dateInfo);
                DateTime? toDateOrPeriod = Convert.ToDateTime(Request.QueryString["tdate"], dateInfo);

                if (!string.IsNullOrEmpty(Request.QueryString["user"]))
                    user = int.Parse(Request.QueryString["user"]);

                isDateOrPeriodSearch = int.Parse(Request.QueryString["isdate"]);
                groupList = Request.QueryString["elist"];

                // if ich/ach user else get member id from session
                if (!string.IsNullOrEmpty(Request.QueryString["mId"]))
                    memberId = int.Parse(Request.QueryString["mId"]);

                string reportType = Request.QueryString["rtype"];

                var isUser = Ioc.Resolve<I_ISUser>(typeof(I_ISUser));
                isUser.UserID = SessionUtil.UserId;

                _logger.Info("Audit Trail Report User ID : " + isUser.UserID);

                string reportPath = "";
                int userType = (int)UserCategory.Member;
                string clearingHouse = string.Empty;

                switch (isUser.CategoryID)
                {
                    case 4:
                        memberId = isUser.Member.MemberID;
                        groupList = PrepareCommaSeparatedList(groupList);
                        membsrClearingHouse = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetClearingHouseDetail(SessionUtil.MemberId);
                        try
                        {
                            reportPath = MapPath("~/Reports/AuditTrail/AuditTrailProfileReport.rpt");
                        }
                        catch (Exception exception)
                        {
                            _logger.Error("Unexpected Error Has Occurred", exception);
                        }
                        clearingHouse = membsrClearingHouse == ClearingHouse.Ich ? "ich" : "ach";
                        break;
                    case 1:
                        userType = (int)UserCategory.SisOps;
                        groupList = PrepareCommaSeparatedList(groupList);
                        try
                        {
                            reportPath = MapPath("~/Reports/AuditTrail/IchAchProfileUpdateReport.rpt");
                        }
                        catch (Exception exception)
                        {
                            _logger.Error("Unexpected Error Has Occurred", exception);
                        }
                        clearingHouse = "ich";
                        break;
                    case 2:
                        userType = (int)UserCategory.IchOps;
                        groupList = Convert.ToInt32(ElementGroupType.Ich).ToString();
                        try
                        {
                            reportPath = MapPath("~/Reports/AuditTrail/IchAchProfileUpdateReport.rpt");
                        }
                        catch (Exception exception)
                        {
                            _logger.Error("Unexpected Error Has Occurred", exception);
                        }
                        clearingHouse = "ich";
                        break;
                    case 3:
                        userType = (int)UserCategory.AchOps;
                        groupList = Convert.ToInt32(ElementGroupType.Ach).ToString();
                        try
                        {
                            reportPath = MapPath("~/Reports/AuditTrail/IchAchProfileUpdateReport.rpt");
                        }
                        catch (Exception exception)
                        {
                            _logger.Error("Unexpected Error Has Occurred", exception);
                        }
                        clearingHouse = "ach";
                        break;
                }

                // if its period search than convert period into corrosponding dates

                if (isDateOrPeriodSearch == 0)
                {
                    // make it 1 as in procedure we are always searching on date now not on period
                    isDateOrPeriodSearch = 1;

                    DateTime fromDate = DateTime.ParseExact(Request.QueryString["fdate"], "yyyy-MMM-dd", null);
                    DateTime toDate = DateTime.ParseExact(Request.QueryString["tdate"], "yyyy-MMM-dd", null);

                    if (membsrClearingHouse == ClearingHouse.Ach || userType == (int)UserCategory.AchOps || (userType == (int)UserCategory.SisOps && reportType == "ach"))
                    {
                        try
                        {
                            //Get from date
                            var period = CalendarManager.GetBillingPeriod(ClearingHouse.Ach, fromDate.Year,
                                                                                                                   fromDate.Month,
                                                                                                                   fromDate.Day);
                            fromDateOrPeriod = period.StartDate;

                            //Get to date
                            period = CalendarManager.GetBillingPeriod(ClearingHouse.Ach,
                                                                                                               toDate.Year, toDate.Month,
                                                                                                               toDate.Day);
                            toDateOrPeriod = period.EndDate;
                        }
                        catch (ISCalendarDataNotFoundException)
                        {
                            if (fromDateOrPeriod == Convert.ToDateTime(Request.QueryString["fdate"], dateInfo))
                                fromDateOrPeriod = null;
                            if (toDateOrPeriod == Convert.ToDateTime(Request.QueryString["tdate"], dateInfo))
                                toDateOrPeriod = null;
                            // lblErrorMsg.Visible = true;
                            //return;
                        }
                    }
                    else if (membsrClearingHouse == ClearingHouse.Ich || userType == (int)UserCategory.IchOps || (userType == (int)UserCategory.SisOps && (reportType == "ich" || String.IsNullOrEmpty(reportType))))
                    {

                        try
                        {
                            //Get from date
                            var period = CalendarManager.GetBillingPeriod(ClearingHouse.Ich,
                                                                                                                 fromDate.Year,
                                                                                                                 fromDate.Month,
                                                                                                                 fromDate.Day);
                            fromDateOrPeriod = period.StartDate;

                            //Get to date
                            period = CalendarManager.GetBillingPeriod(ClearingHouse.Ich,
                                                                                                               toDate.Year, toDate.Month,
                                                                                                               toDate.Day);
                            toDateOrPeriod = period.EndDate;
                        }
                        catch (ISCalendarDataNotFoundException)
                        {
                            if (fromDateOrPeriod == Convert.ToDateTime(Request.QueryString["fdate"], dateInfo))
                                fromDateOrPeriod = null;
                            if (toDateOrPeriod == Convert.ToDateTime(Request.QueryString["tdate"], dateInfo))
                                toDateOrPeriod = null;
                            // lblErrorMsg.Visible = true;
                            //return;
                        }
                    }
                }

                listModel = Ioc.Resolve<IFutureUpdatesManager>(typeof(IFutureUpdatesManager)).GetFutureUpdatesList(groupList, fromDateOrPeriod, toDateOrPeriod, user, userType, isDateOrPeriodSearch, memberId, reportType);

                DataTable dt = PrepareUpdatedList(listModel, clearingHouse);

                ReportDocument orpt = new ReportDocument();

                orpt.Load(reportPath);
                orpt.SetDataSource(dt);

                int exportFormatFlags = (int)(ViewerExportFormats.ExcelRecordFormat);
                CRViewer.AllowedExportFormats = exportFormatFlags;

                CRViewer.ReportSource = orpt;

                if (isUser.CategoryID == (int)UserCategory.Member && (listModel != null && listModel.Count > 0))
                {
                    if (listModel[0].ShowChangedBy == 1)
                    {
                        orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text7"].ObjectFormat.EnableSuppress = true;
                        (orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text14"]).Width = 2390;
                        (orpt.ReportDefinition.Sections["Section3"].ReportObjects["Line1"]).ObjectFormat.EnableSuppress = true;
                    }
                }

            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }

        }

        public DataTable PrepareUpdatedList(List<FutureUpdateDetails> list, string clearingHouse)
        {
            var dt = new DataTable();
            foreach (PropertyInfo info in typeof(FutureUpdateDetails).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }

            foreach (var t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(FutureUpdateDetails).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                // set billing period as p1,p2,p3 or p4
                //try
                //{
                //    BillingPeriod billingPeriod;
                //    if (clearingHouse == "ach")
                //        billingPeriod = //new BillingPeriod { ClearingHouse = ClearingHouse.Ich};
                //    CalendarManager.GetBillingPeriod(
                //    Convert.ToDateTime(t.ChangeEffectiveDate), ClearingHouse.Ach);
                //    else
                //        billingPeriod = //new BillingPeriod { ClearingHouse = ClearingHouse.Ich };
                //          CalendarManager.GetBillingPeriod(
                //            Convert.ToDateTime(t.ChangeEffectiveDate));

                //    if (billingPeriod.Period != 0)
                //        //t.ChangeEffectivePeriod = EnumList.GetMonthDisplayValue((Month)billingPeriod.Month).ToUpper() + "-" + Convert.ToString(billingPeriod.Year) + "-" + "P" + Convert.ToString(billingPeriod.Period);
                //        row["ChangeEffectivePeriod"] = EnumList.GetMonthDisplayValue((Month)billingPeriod.Month).ToUpper() + "-" + Convert.ToString(billingPeriod.Year) + "-" + "P" + Convert.ToString(billingPeriod.Period);
                //}
                //catch (ISCalendarDataNotFoundException)
                //{
                //    row["ChangeEffectivePeriod"] = "";

                //}

                dt.Rows.Add(row);
            }
            return dt;
        }

        private string PrepareCommaSeparatedList(string groupList)
        {

            string prepareGroupList = null;
            string[] elements = groupList.Split('!');

            foreach (var element in elements)
            {
                if (!elements[0].Equals(element))
                {
                    string[] elementsplit = element.Split('|');
                    if (elementsplit[1].Equals("true"))
                    {
                        prepareGroupList = prepareGroupList + elementsplit[0] + ",";
                    }

                }
            }
            // remove comma from the end
            if (prepareGroupList != null)
                prepareGroupList = prepareGroupList.Remove(prepareGroupList.Length - 1, 1);

            return prepareGroupList;
        }

    }

}
