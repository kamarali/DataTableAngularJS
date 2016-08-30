<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SystemMonitor.SystemMonitorManage>" %>
<%@ Import Namespace="System.Security.Policy" %>
<% using (Html.BeginForm("Reprocessing", "ManageSystemMonitor", FormMethod.Post, new { id = "ManageSystemMonitorLocation" }))
   {%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Re-Generate Recap Sheet file To ACH</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("RecapBillingYear",0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("RecapBillingMonth", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Period:</label>
                        <%:Html.BillingPeriodDropdownList("RecapBillingPeriod", 0)%>
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="btnRecapSheet" value="Generate" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Re-Generate Old IDEC File</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("OldIdecBillingYear",0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("OldIdecBillingMonth", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            Billing Member:</label>
                        <%:Html.HiddenFor(m => m.OldIdecMemberId, new { style = "width:200px;" })%>
                        <%:Html.TextBoxFor(m => m.OldIdecMemberName, new { @class = "autocComplete" })%>
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="btnOldIdec" value="Generate" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 170px;">
            <div>
                <h2>
                    Re-Generate Offline Collection and Archive</h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                Member:</label>
                            <%:Html.HiddenFor(m => m.OfflineMemberId, new { style = "width:200px;" })%>
                            <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                                    Desc: Non layout related IS-WEB screen changes.
                                    Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 -->
                            <%:Html.TextBoxFor(m => m.OfflineMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Category:</label>
                            <%: Html.SystemMonBillingCategoryDropdownListFor(model => model.OfflineBillingCategoryId)%>
                        </div>
                        <%-- <div>
                    <label>
                    <span>*</span> File Type:</label>
                    <%: Html.DropDownListFor(m => m.OfflineFileTypeId, new SelectList(new List<string>()))%>
                </div>--%>
                    </div>
                    <div style="float: left; width: 100%; height: 50px;">
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearDropdownList("OfflineBillingYear", 0)%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("OfflineBillingMonth", 0)%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("OfflineBillingPeriod", 0)%>
                        </div>
                        <div style="float: left; width: 100px;">
                            <label>
                                Stages</label>
                            <%: Html.DropDownListFor(m => m.OfflineStages, new SelectList(new List<string>()))%>
                        </div>
                    </div>
                    <div style="width: 100%; height: 50px;">
                        <div style="padding-top: 13px; float: left; width: 150px;">
                            <label>
                                <span></span>
                            </label>
                            <input class="primaryButton" type="button" id="OfflineArchive" value="Generate" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Re-Generate Process Invoice CSV</h2>
                <div style="width: 100%;">
                    <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                            Desc: Non layout related IS-WEB screen changes.
                            Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 -->
                    <div style="float: left; width: 200px;">
                        <label>
                            Member:</label>
                        <%:Html.HiddenFor(m => m.CsvMemberId, new { style = "width:200px;" })%>
                        <%:Html.TextBoxFor(m => m.CsvMemberName, new { @class = "autocComplete textboxWidth" })%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("CSVBillingYear", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("CSVBillingMonth", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Period:</label>
                        <%:Html.BillingPeriodDropdownList("CSVBillingPeriod", 0)%>
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="ProcessCSV" value="Generate" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Re-Generate BVC Request File To ATPCO</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("BvcBillingYear",0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("BvcBillingMonth", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Period:</label>
                        <%:Html.BillingPeriodDropdownList("BvcBillingPeriod", 0)%>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="btnBvcRequestGenerate" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 200px;">
            <div>
                <h2>
                    Invoice File Generation</h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                Member:</label>
                            <%:Html.HiddenFor(m => m.MemberId, new { style = "width:200px;" })%>
                            <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                                    Desc: Non layout related IS-WEB screen changes.
                                    Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 -->
                            <%:Html.TextBoxFor(m => m.MemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Category:</label>
                            <%: Html.SystemMonBillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> File Type:</label>
                            <%: Html.DropDownListFor(m => m.FileTypeId, new SelectList(new List<string>()))%>
                        </div>
                        <div id="divFileGenerationDate" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span>File Generation Date:</label>
                            <%:Html.TextBox("FileGenerationDate", Model.FileGenerationDate != null ? Convert.ToDateTime(Model.FileGenerationDate).ToString(FormatConstants.DateFormat) : string.Empty, new { @class = "datePicker", @id = "FileGenerationDate" })%>
                        </div>
                    </div>
                    <div style="float: left; width: 100%; height: 50px;">
                        <div style="float: left; width: 200px;">
                            <label>
                                <span class="indi" style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearDropdownList("InvoiceBillingYear", 0)%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span class="indi" style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("InvoiceBillingMonth", 0)%>
                        </div>
                        <div style="float: left;">
                            <label>
                                <span class="indi" style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("InvoiceBillingPeriod", 0)%>
                        </div>
                    </div>
                    <div style="width: 100%; height: 50px;">
                        <div style="float: left; width: 100px;">
                            <label>
                                Presented</label>
                            <%: Html.CheckBox("chkPresented", true)%>
                        </div>
                        <div style="float: left; width: 150px;">
                            <label>
                                Ready For Billing</label>
                            <%: Html.CheckBox("chkReadyForBilling", false)%>
                        </div>
                        <div style="float: left; width: 100px;">
                            <label>
                                Claimed</label>
                            <%: Html.CheckBox("chkClaimed", false)%>
                        </div>
                        <div style="float: left; width: 150px;">
                            <label>
                                Processing Complete</label>
                            <%: Html.CheckBox("chkProcessingComplete", false)%>
                        </div>
                        <div style="padding-top: 13px; float: left; width: 150px;">
                            <label>
                                <span></span>
                            </label>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="btnInvoiceGenerate" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Re-Generate Nil Form C</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            Member:</label>
                        <%:Html.HiddenFor(m => m.NilCMemberId, new { style = "width:200px;" })%>
                        <%:Html.TextBoxFor(m => m.NilCMemberName, new { @class = "autocComplete" })%>
                    </div>
                    <div style="float: left; width: 250px;">
                        <label>
                            <span style="color: red">*</span> Provisional Billing Month:</label>
                        <%:Html.TextBox("txtProvisionalBillingMonth")%>
                        (YYYYMM)
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="GenerateNilFormC" value="Generate" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 440px;">
            <div>
                <h2>
                    View/Present Pending Invoices</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("PendingInvBillingYear",0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("PendingInvBillingMonth", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Period:</label>
                        <%:Html.BillingPeriodDropdownList("PendingInvBillingPeriod", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            Billing Category:</label>
                        <%: Html.SystemMonBillingCategoryDropdownListFor(model => model.PendingInvoiceBillingCategory)%>
                    </div>
                </div>
                <div style="float: left; width: 100%; height: 50px;">
                    <div style="float: left; width: 200px;">
                        <label>
                            Billing Member:</label>
                        <%:Html.HiddenFor(m => m.PendingInvBillingMemberId, new { style = "width:200px;" })%>
                        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                                Desc: Non layout related IS-WEB screen changes.
                                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 -->
                        <%:Html.TextBoxFor(m => m.PendingInvBillingMemberName, new { @class = "autocComplete textboxWidth" })%>
                    </div>
                    <div style="float: left; width: 200px;">
                        <label>
                            Billed Member:</label>
                        <%:Html.HiddenFor(m => m.PendingInvBilledMemberId, new { style = "width:200px;" })%>
                        <%:Html.TextBoxFor(m => m.PendingInvBilledMemberName, new { @class = "autocComplete textboxWidth" })%>
                    </div>
                </div>
                <div style="float: left; width: 100%; height: 20px;">
                    <input class="primaryButton" type="button" name="btnSearch" value="Search" id="btnPendingInvSearch" />
                </div>
                <div style="float: left; width: 100%; height: 250px;">
                    <h2>
                        Search Results</h2>
                    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SysMonPendingInvoices]); %>
                </div>
                <div style="padding-top: 13px; float: left; width: 50px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="btnPendingInvStatus" value="Update" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 60px;">
            <div>
                <h2>
                    Update Master Data In Services</h2>
                <div style="padding-top: 13px; float: left; width: 40px;">
                    <input class="primaryButton" type="button" id="btnResetValidationCache" value="Update" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<!--CMP#665: User Related Enhancements-FRS-v1.2 [Sec 2.2: SIS Ops Reprocessing Tab in the System Monitor]  -->
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Retry 'ICP New User Web Service' call for failed cases</h2>
                <div style="padding-top: 13px; width: 45%">
                    Clicking Retry will trigger the 'ICP New User Web Service' (hosted by IATA Customer
                    Portal) for recently created users for whom an earlier Web Service call resulted
                    in an issue/failure.<br/>
                    Such users do not have an ICP 'Federation ID'; and the Retry will attempt to receive
                    the 'Federation ID' for them.</div>
            <div style="padding-top: 13px; width: 40px; clear: both;">
                <input class="primaryButton" type="button" id="btnIcpRetry" value="Retry" />
            </div>
        </div>
    </div>
</div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Re-archive Failed Legal Invoices</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("ReArchiveBillingYear", (int)ViewData["previouslyClosedYear"])%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("ReArchiveBillingMonth",  (int)ViewData["previouslyClosedMonth"])%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Period:</label>
                        <%:Html.BillingPeriodDropdownList("ReArchiveBillingPeriod",  (int)ViewData["previouslyClosedPeriod"])%>
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="btnReArchive" value="Re-archive" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Legal-Xml Generation</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%:
                    Html.BillingYearTwoDropdownListFor(model => model.LegalXmlBillingYear)  %>
                    </div>
                    <div style="float: left; width: 250px;">
                        <label>
                            <span style="color: red">*</span>Billing Month:</label>
                        <%: Html.BillingMonthDropdownListFor(model => model.LegalXmlBillingMonth)  %>
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="LegalXmlGeneration" value="Generate" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 100px;">
            <div>
                <h2>
                    Regenerate UATP ATCAN Statement</h2>
                <div style="width: 100%;">
                    <div style="float: left; width: 150px;">
                        <label>
                            Member:</label>
                        <%:Html.HiddenFor(m => m.UatpMemberId, new { style = "width:200px;" })%>
                        <%:Html.TextBoxFor(m => m.UatpMemberName, new { @class = "autocComplete" })%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Year:</label>
                        <%: Html.BillingYearDropdownList("UatpBillingYear", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Month:</label>
                        <%:Html.BillingMonthDropdownList("UatpBillingMonth", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Period:</label>
                        <%:Html.BillingPeriodDropdownList("UatpBillingPeriod", 0)%>
                    </div>
                    <div style="float: left; width: 150px;">
                        <label>
                            <span style="color: red">*</span> Billing Type:</label>
                        <%:Html.BillingTypeDropdownList("UatpBillingType", 1,true)%>
                    </div>
                </div>
                <div style="padding-top: 13px; float: left; width: 150px;">
                    <label>
                        <span></span>
                    </label>
                    <input class="primaryButton" type="button" id="btnUatpAtcanGeneration" value="Generate" />
                </div>
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 180px;">
            <div>
                <h2>
                    Auto-billing/Value-determination File Generation</h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                Member:</label>
                            <%:Html.HiddenFor(m => m.RevRecMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.RevRecMemberName, new { @class = "autocComplete" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> File Type:</label>
                            <%:Html.AutoBillingFileTypeDropdownList("RevRecFileType")%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearDropdownList("RevRecBillingYear", 0)%>
                        </div>
                        <div id="div1" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("RevRecBillingMonth", 0)%>
                        </div>
                        <div id="div2" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("RevRecBillingPeriod", 0)%>
                        </div>
                    </div>
                    <div style="float: left; width: 100%;">
                        <div style="float: left; width: 200px;">
                            <label>
                                Pending/Failed</label>
                            <%: Html.CheckBox("chkPendingFailed", true)%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                Included In Previously Generated files</label>
                            <%: Html.CheckBox("chkIncludeInPrevFile", false)%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="AutoBillingFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<%--CMP529 : Daily Output Generation for MISC Bilateral Invoices--%>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate MISC Daily Bilateral IS-XML Files to Billed Members</h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                Billed Member:</label>
                            <%:Html.HiddenFor(m => m.MiscDailyIsXmlMemberId, new { style = "width:200px;" })%>
                            <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                                    Desc: Non layout related IS-WEB screen changes.
                                    Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 -->
                            <%:Html.TextBoxFor(m => m.MiscDailyIsXmlMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                <span style="color: red">*</span> Pertaining to Original Target Delivery Date:</label>
                            <%:Html.TextBox("MiscDailyIsXmlTargetDate", Model.MiscDailyIsXmlTargetDate != null ? Model.MiscDailyIsXmlTargetDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MISCDailyBilateralISXML" value="Generate" />
            </div>
        </div>
    </div>
</div>
<%--CMP529 : Daily Output Generation for MISC Bilateral Invoices--%>
<div style="height: 10px;">
</div>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate MISC Daily Bilateral Offline Archive Files to Billed Members</h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                Billed Member:</label>
                            <%:Html.HiddenFor(m => m.MiscDailyOarMemberId, new { style = "width:200px;" })%>
                            <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                                    Desc: Non layout related IS-WEB screen changes.
                                    Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 -->
                            <%:Html.TextBoxFor(m => m.MiscDailyOarMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                <span style="color: red">*</span> Pertaining to Original Target Delivery Date:</label>
                            <%:Html.TextBox("MiscDailyOarTargetDate", Model.MiscDailyOarTargetDate != null ? Model.MiscDailyOarTargetDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MISCDailyBilateralOAR" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate On behalf of IS-XML files (MISC Location Specific), Receivables Files
                    to Billing Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billing Member:</label>
                            <%:Html.HiddenFor(m => m.MiscLocOnBehalfMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscLocOnBehalfMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscLocOnBehalfLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearTwoDropdownListFor(m => m.MiscLocOnBehalfBillingYear)%>
                        </div>
                        <div id="div3" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("MiscLocOnBehalfBillingMonth", 0)%>
                        </div>
                        <div id="div4" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("MiscLocOnBehalfBillingPeriod", 0)%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscLocOnBehalfFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate MiscIsWebXml (MISC Location Specific), Receivables Files to Billing
                    Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billing Member:</label>
                            <%:Html.HiddenFor(m => m.MiscIsWebXmlMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscIsWebXmlMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscIsWebXmlLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                <span style="color: red">*</span> Pertaining to Original File Generation Date:</label>
                            <%:Html.TextBox("MiscIsWebXmlGenDate", Model.MiscIsWebXmlGenDate != null ? Model.MiscIsWebXmlGenDate.Value.ToString(FormatConstants.DateFormat) : string.Empty, new { @class = "datePicker" })%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscIsWebXmlFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate Offline Archive files (MISC Location Specific), Receivables Files to
                    Billing Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billing Member:</label>
                            <%:Html.HiddenFor(m => m.MiscLocOarBlgMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscLocOarBlgMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscLocOarBlgLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearTwoDropdownListFor(m => m.MiscLocOarBlgBillingYear)%>
                        </div>
                        <div id="div5" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("MiscLocOarBlgBillingMonth", 0)%>
                        </div>
                        <div id="div6" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("MiscLocOarBlgBillingPeriod", 0)%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscLocOarBlgFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate Offline Archive files (MISC Location Specific), Payables Files to Billed
                    Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billed Member:</label>
                            <%:Html.HiddenFor(m => m.MiscLocOarBldMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscLocOarBldMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscLocOarBldLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearTwoDropdownListFor(m => m.MiscLocOarBldBillingYear)%>
                        </div>
                        <div id="div7" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("MiscLocOarBldBillingMonth", 0)%>
                        </div>
                        <div id="div8" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("MiscLocOarBldBillingPeriod", 0)%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscLocOarBldFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;" id="MiscLocIsXmlFileGenDiv">
            <div>
                <h2>
                    Re-Generate ISXML Outbound (MISC Location Specific), Payables Files to Billed Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billed Member:</label>
                            <%:Html.HiddenFor(m => m.MiscLocIsXmlMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscLocIsXmlMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscLocIsXmlLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Year:</label>
                            <%: Html.BillingYearTwoDropdownListFor(m => m.MiscLocIsXmlBillingYear)%>
                        </div>
                        <div id="div9" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Month:</label>
                            <%:Html.BillingMonthDropdownList("MiscLocIsXmlBillingMonth", 0)%>
                        </div>
                        <div id="div10" style="float: left; width: 200px;">
                            <label>
                                <span style="color: red">*</span> Billing Period:</label>
                            <%:Html.BillingPeriodDropdownList("MiscLocIsXmlBillingPeriod", 0)%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscLocIsXmlFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate MISC Daily Bilateral IS-XML Files, Payables Files to Billed Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billed Member:</label>
                            <%:Html.HiddenFor(m => m.MiscDailyXmlLocMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscDailyXmlLocMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscDailyXmlLocLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                <span style="color: red">*</span> Pertaining to Original Target Delivery Date:</label>
                            <%:Html.TextBox("MiscDailyXmlLocGenDate", Model.MiscDailyXmlLocGenDate != null ? Model.MiscDailyXmlLocGenDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscDailyIsXmlLocFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<div class="solidBox dataEntry">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 120px;">
            <div>
                <h2>
                    Re-Generate MISC Daily Bilateral Offline Archive Files, Payables Files to Billed
                    Members
                </h2>
                <div style="width: 100%;">
                    <div>
                        <div style="float: left; width: 200px; height: 50px;">
                            <label>
                                <span style="color: red">*</span> Billed Member:</label>
                            <%:Html.HiddenFor(m => m.MiscDailyOarLocMemberId, new { style = "width:200px;" })%>
                            <%:Html.TextBoxFor(m => m.MiscDailyOarLocMemberName, new { @class = "autocComplete textboxWidth" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                Location ID:</label>
                            <%:Html.TextBoxFor(m => m.MiscDailyOarLocLocationCode, new { @class = "autocComplete", style = "width:120px;" })%>
                        </div>
                        <div style="float: left; width: 210px;">
                            <label>
                                <span style="color: red">*</span> Pertaining to Original Target Delivery Date:</label>
                            <%:Html.TextBox("MiscDailyOarLocGenDate", Model.MiscDailyOarLocGenDate != null ? Model.MiscDailyOarLocGenDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                        </div>
                    </div>
                </div>
            </div>
            <div style="padding-top: 13px; float: left; width: 150px;">
                <label>
                    <span></span>
                </label>
                <input class="primaryButton" type="button" id="MiscDailyOarLocFileGeneration" value="Generate" />
            </div>
        </div>
    </div>
</div>
<div style="height: 10px;">
</div>
<%--CMP#622: MISC Outputs Split as per Location IDs--%>
<script language="javascript" type="text/javascript">

    $(document).ready(function () {
        $('#RecapBillingYear').focus();
        $('#LegalXmlBillingYear').val('<%: ViewData["previouslyClosedYear"] %>');
        $('#LegalXmlBillingMonth').val('<%: ViewData["previouslyClosedMonth"] %>');
        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.1 Table 6 Row 2 */
        registerAutocomplete('RevRecMemberName', 'RevRecMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MemberName', 'MemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.1 Table 6 Row 1 */
        registerAutocomplete('NilCMemberName', 'NilCMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('CsvMemberName', 'CsvMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('UatpMemberName', 'UatpMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('OfflineMemberName', 'OfflineMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('OldIdecMemberName', 'OldIdecMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('PendingInvBillingMemberName', 'PendingInvBillingMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('PendingInvBilledMemberName', 'PendingInvBilledMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscDailyIsXmlMemberName', 'MiscDailyIsXmlMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscDailyOarMemberName', 'MiscDailyOarMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);

        //CMP#622: MISC Outputs Split as per Location IDs 
        registerAutocomplete('MiscLocOnBehalfMemberName', 'MiscLocOnBehalfMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscIsWebXmlMemberName', 'MiscIsWebXmlMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscLocOarBlgMemberName', 'MiscLocOarBlgMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscLocOarBldMemberName', 'MiscLocOarBldMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscLocIsXmlMemberName', 'MiscLocIsXmlMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscDailyXmlLocMemberName', 'MiscDailyXmlLocMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('MiscDailyOarLocMemberName', 'MiscDailyOarLocMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);

        //CMP#622: MISC Outputs Split as per Location IDs       
        registerAutocompleteWithOutCache('MiscLocOnBehalfLocationCode', 'MiscLocOnBehalfLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscLocOnBehalfMemberId', null);
        registerAutocompleteWithOutCache('MiscIsWebXmlLocationCode', 'MiscIsWebXmlLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscIsWebXmlMemberId', null);
        registerAutocompleteWithOutCache('MiscLocOarBlgLocationCode', 'MiscLocOarBlgLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscLocOarBlgMemberId', null);
        registerAutocompleteWithOutCache('MiscLocOarBldLocationCode', 'MiscLocOarBldLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscLocOarBldMemberId', null);
        registerAutocompleteWithOutCache('MiscLocIsXmlLocationCode', 'MiscLocIsXmlLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscLocIsXmlMemberId', null);
        registerAutocompleteWithOutCache('MiscDailyXmlLocLocationCode', 'MiscDailyXmlLocLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscDailyXmlLocMemberId', null);
        registerAutocompleteWithOutCache('MiscDailyOarLocLocationCode', 'MiscDailyOarLocLocationCode', '<%:Url.Action("GetLocationListOfMemberOnSM", "Data", new { area = "" })%>', 0, true, null, null, null, '#MiscDailyOarLocMemberId', null);

        $("#MiscDailyOarLocMemberName").bind("change", function () {
            $('#MiscDailyOarLocLocationCode').val('');
        });
        $("#MiscDailyXmlLocMemberName").bind("change", function () {
            $('#MiscDailyXmlLocLocationCode').val('');
        });
        $("#MiscLocIsXmlMemberName").bind("change", function () {
            $('#MiscLocIsXmlLocationCode').val('');
        });
        $("#MiscLocOarBldMemberName").bind("change", function () {
            $('#MiscLocOarBldLocationCode').val('');
        });
        $("#MiscLocOarBlgMemberName").bind("change", function () {
            $('#MiscLocOarBlgLocationCode').val('');
        });
        $("#MiscIsWebXmlMemberName").bind("change", function () {
            $('#MiscIsWebXmlLocationCode').val('');
        });
        $("#MiscLocOnBehalfMemberName").bind("change", function () {
            $('#MiscLocOnBehalfLocationCode').val('');
        });

        var items = "";
        var firstItem = "<option value='0'>Select</option>";
        var stageOne = '<option value=1>Stage 1</option>';
        items += stageOne;
        var stageTwo = '<option value=2>Stage 2</option> ';
        items += stageTwo;
        var StageThree = '<option value=3>Stage 3</option>';
        items += StageThree;
        items = firstItem + items;
        $("#OfflineStages").html(items);

        $('#btnRecapSheet').click(function () {
            var recapBillingYear = $('#RecapBillingYear').val();
            var recapBillingMonth = $('#RecapBillingMonth').val();
            var recapBillingPeriod = $('#RecapBillingPeriod').val();
            var validateInput = false;

            if (recapBillingYear != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (recapBillingMonth != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (recapBillingPeriod != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("RegenerateAchRecapSheet", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { recapBillingYear: recapBillingYear, recapBillingMonth: recapBillingMonth, recapBillingPeriod: recapBillingPeriod },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year,Month And Period"); }

        });
        $('#btnOldIdec').click(function () {
            var oldIdecBillingYear = $('#OldIdecBillingYear').val();
            var oldIdecBillingMonth = $('#OldIdecBillingMonth').val();
            var oldIdecMemberId = $('#OldIdecMemberId').val();
            var validateInput = false;
            if (oldIdecBillingYear != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (oldIdecBillingMonth != '') {
                validateInput = true;
            } else { validateInput = false; }
            if (oldIdecMemberId == null || oldIdecMemberId == '') {
                oldIdecMemberId = 0;
            }
            if (validateInput) {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("RegenerateOldIdec", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { oldIdecBillingYear: oldIdecBillingYear, oldIdecBillingMonth: oldIdecBillingMonth, oldIdecMemberId: oldIdecMemberId },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year And Month"); }

        });
        $('#btnBvcRequestGenerate').click(function () {
            var bvcBillingYear = $('#BvcBillingYear').val();
            var bvcBillingMonth = $('#BvcBillingMonth').val();
            var bvcBillingPeriod = $('#BvcBillingPeriod').val();
            var validateInput = false;

            if (bvcBillingYear != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (bvcBillingMonth != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (bvcBillingPeriod != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("RegenerateBvcRequest", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { bvcBillingYear: bvcBillingYear, bvcBillingMonth: bvcBillingMonth, bvcBillingPeriod: bvcBillingPeriod },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year,Month And Period"); }

        });

        $('#OfflineArchive').click(function () {
            var BillingYear = $('#OfflineBillingYear').val();
            var BillingMonth = $('#OfflineBillingMonth').val();
            var BillingPeriod = $('#OfflineBillingPeriod').val();
            var BillingCategory = $('#OfflineBillingCategoryId').val();
            var FileType = "1";
            // var FileType = $('#OfflineFileTypeId').val();
            var OfflineStages = $('#OfflineStages').val();
            //below code to disable the generate button till processing completed of OAR Regeneration(SCP#55909)
            $('#OfflineArchive').attr('disabled', true);
            var validateInput = false;

            if (BillingCategory != '-1') {
                validateInput = true;
                $("#OfflineBillingCategoryId").css("background", "none");
            } else {
                validateInput = false;
                //$("#OfflineBillingCategoryId").css("background", "red");

            }

            //    if (FileType != '0' && validateInput == true) {
            //      validateInput = true;
            //      $("#OfflineFileTypeId").css("background", "none");
            //    } else {
            //      validateInput = false;
            //      if (FileType == '0')
            //        $("#OfflineFileTypeId").css("background", "red");
            //    }

            if (BillingYear != '' && validateInput == true) {
                $("#OfflineBillingYear").css("background", "none");
                validateInput = true;
            } else {
                validateInput = false;
                // if (BillingYear == '')
                // $("#OfflineBillingYear").css("background", "red");
            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
                $("#OfflineBillingMonth").css("background", "none");
            } else {
                validateInput = false;
                // if (BillingMonth == '')
                //  $("#OfflineBillingMonth").css("background", "red");
            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
                $("#OfflineBillingPeriod").css("background", "none");
            } else {
                validateInput = false;
                // if (BillingPeriod == '')
                //   $("#OfflineBillingPeriod").css("background", "red");
            }


            if (OfflineStages != '0' && validateInput == true) {
                validateInput = true;
                $("#OfflineStages").css("background", "none");
            } else {
                validateInput = false;
                // if (OfflineStages == '0')
                //   $("#OfflineStages").css("background", "red");
            }

            var MemberId = $('#OfflineMemberId').val();
            if (MemberId == null || MemberId == '') {
                MemberId = 0;
            }


            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("RegenerateOfflineCollection", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, memberId: MemberId, billingCategory: BillingCategory, fileType: FileType, stages: OfflineStages },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                                //below code to enable generate button after showing error message
                                //SCP#55909
                                $("#OfflineArchive").removeAttr('disabled');
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                                //below code to enable generate button after showing successfull message
                                //SCP#55909
                                $("#OfflineArchive").removeAttr('disabled');
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });


            } else { validateInput = false; showClientErrorMessage("Please Select Billing Category, Billing Year, Month , Period And Stages"); }

        });


        $('#ProcessCSV').click(function () {
            var recapBillingYear = $('#CSVBillingYear').val();
            var recapBillingMonth = $('#CSVBillingMonth').val();
            var recapBillingPeriod = $('#CSVBillingPeriod').val();
            var validateInput = false;

            if (recapBillingYear != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (recapBillingMonth != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (recapBillingPeriod != '') {
                validateInput = true;
            } else { validateInput = false; }

            var MemberId = $('#CsvMemberId').val();
            if (MemberId == '') {
                MemberId = 0;
            }


            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("ReCreateAndTransmitProcessedInvoiceDataCsv", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { recapBillingYear: recapBillingYear, recapBillingMonth: recapBillingMonth, recapBillingPeriod: recapBillingPeriod, memberId: MemberId },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year,Month And Period"); }

        });


        $('#GenerateNilFormC').click(function () {
            var ProvisinalBillingYear = $('#txtProvisionalBillingMonth').val();
            var MemberId = $('#NilCMemberId').val();
            if (MemberId == '') {
                MemberId = 0;
            }

            var validateInput = false;

            if (ProvisinalBillingYear != '') {

                if (ProvisinalBillingYear.length > 6) {
                    validateInput = false;
                } else if (ProvisinalBillingYear.length < 6) { validateInput = false; }
                else { validateInput = true; }

            } else { validateInput = false; }

            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("ReGenerateNilFormC", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { provisinalBillingYear: ProvisinalBillingYear, memberID: MemberId },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else { validateInput = false; showClientErrorMessage("Please Enter Correct Provisional Billing Month"); }

        });


        $('#btnInvoiceGenerate').click(function () {


            var BillingYear = $('#InvoiceBillingYear').val();
            var BillingMonth = $('#InvoiceBillingMonth').val();
            var BillingPeriod = $('#InvoiceBillingPeriod').val();
            var BillingCategory = $('#BillingCategoryId').val();
            var FileType = $('#FileTypeId').val();
            var FileGenerationDate = $('#FileGenerationDate').val();
            var validateInput = false;

            if (BillingCategory != '-1') {
                validateInput = true;
                $("#BillingCategoryId").css("background", "none");
            } else {
                validateInput = false;
                // $("#BillingCategoryId").css("background", "red");

            }

            if (FileType != '0' && validateInput == true) {
                validateInput = true;
                $("#FileTypeId").css("background", "none");
            } else {
                validateInput = false;
                //  if (FileType == '0')
                //    $("#FileTypeId").css("background", "red");
            }

            if (FileType != '7') {
                if (BillingYear != '' && validateInput == true) {
                    $("#InvoiceBillingYear").css("background", "none");
                    validateInput = true;
                } else {
                    validateInput = false;
                    // if (BillingYear == '')
                    // $("#InvoiceBillingYear").css("background", "red");
                }

                if (BillingMonth != '' && validateInput == true) {
                    validateInput = true;
                    $("#InvoiceBillingMonth").css("background", "none");
                } else {
                    validateInput = false;
                    // if (BillingMonth == '')
                    //   $("#InvoiceBillingMonth").css("background", "red");
                }

                if (BillingPeriod != '' && validateInput == true) {
                    validateInput = true;
                    $("#InvoiceBillingPeriod").css("background", "none");
                } else {
                    validateInput = false;
                    //  if (BillingPeriod == '')
                    //   $("#InvoiceBillingPeriod").css("background", "red");
                }
                FileGenerationDate = '';
                $('#FileGenerationDate').val('');
            }
            else if (FileType == '7') {
                if (FileGenerationDate != '' && validateInput == true) {
                    validateInput = true;
                    $("#FileGenerationDate").css("background", "none");
                } else {
                    validateInput = false;
                    //  if (BillingPeriod == '')
                    //   $("#InvoiceBillingPeriod").css("background", "red");
                }
                BillingYear = 0;
                BillingMonth = 0;
                BillingPeriod = 0;
            }


            var MemberId = $('#MemberId').val();

            if (MemberId == null || MemberId == '') {
                MemberId = 0;
            }

            var InvoiceStatusPresented = 0;
            if ($('#chkPresented').is(":checked")) {
                InvoiceStatusPresented = 1;
            }

            var InvoiceStatusReadyForBilling = 0;
            if ($('#chkReadyForBilling').is(":checked")) {
                InvoiceStatusReadyForBilling = 1;
            }

            var InvoiceStatusClaimed = 0;
            if ($('#chkClaimed').is(":checked")) {
                InvoiceStatusClaimed = 1;
            }

            var InvoiceStatusProcessingComp = 0;
            if ($('#chkProcessingComplete').is(":checked")) {
                InvoiceStatusProcessingComp = 1;
            }


            if (validateInput) {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("InvoiceFileGenerate", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: MemberId, billingCategory: BillingCategory, fileType: FileType, isInvoiceStatusPresented: InvoiceStatusPresented, isInvoiceStatusReadyForBilling: InvoiceStatusReadyForBilling, isInvoiceStatusClaimed: InvoiceStatusClaimed, isInvoiceStatusProcessingComp: InvoiceStatusProcessingComp, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, fileGenerationDate: FileGenerationDate },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else {
                validateInput = false;
                if (FileType == '7')
                    showClientErrorMessage("Please Select Billing Category, Billing Year, Month , Period, File Type And File Generation Date");
                else
                    showClientErrorMessage("Please Select Billing Category, Billing Year, Month , Period And File Type");
            }

        });




        $("#BillingCategoryId").bind('change', function () {

            var BillingCategoryID = $("#BillingCategoryId").val();

            var items = "";
            var firstItem = "<option value='0'>Select</option>";
            // File Type 
            var isIDEC = '<option value=1>IS-IDEC</option>';
            var isXML = '<option value=2>IS-XML</option> ';
            var formCXML = '<option value=3>FORMC-XML</option>';
            var provisionalBillingFile = '<option value=4>CONSOLIDATED PROVISIONAL BILLING FILE</option>';
            var oldIDEC = '<option value=5>OLD IDEC DOWNGRADE</option>';
            var onBehalfofInvoice = '<option value=6>ON-BEHALF OF INVOICE</option>';
            var isWeb = '<option value=7>IS-WEB</option>';

            switch (parseInt(BillingCategoryID)) {
                case 1:
                    items += isIDEC;
                    items += isXML;
                    items += formCXML;
                    items += provisionalBillingFile;
                    //  items += oldIDEC;
                    break;
                case 2:

                    items += isIDEC;
                    items += isXML;
                    // items += oldIDEC;
                    break;

                case 3:
                    items += isXML;
                    items += onBehalfofInvoice;
                    items += isWeb;
                    break;

                case 4:
                    items += isXML;
                    items += onBehalfofInvoice;
                    break;

            }

            items = firstItem + items;
            $("#FileTypeId").html(items);
            //SCP#340872 - Issue in 'Daily IS-XML files for Receivables IS-WEB Invoices' output file
            $('#divFileGenerationDate').hide();
            $(".indi").show();
            $('#InvoiceBillingYear').attr('disabled', false);
            $('#InvoiceBillingMonth').attr('disabled', false);
            $('#InvoiceBillingPeriod').attr('disabled', false);
            $('#InvoiceBillingYear').val('');
            $('#InvoiceBillingMonth').val('');
            $('#InvoiceBillingPeriod').val('');

        });

        $('#btnPendingInvSearch').click(function () {

            SearchPendingInvoices();
        });



        function SearchPendingInvoices() {

            var BillingYear = $('#PendingInvBillingYear').val();
            var BillingMonth = $('#PendingInvBillingMonth').val();
            var BillingPeriod = $('#PendingInvBillingPeriod').val();
            var BillingCategory = $('#PendingInvoiceBillingCategory').val();

            var validateInput = false;


            if (BillingYear != '') {
                validateInput = true;
            } else {
                validateInput = false;
            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
            } else {
                validateInput = false;
            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
            } else {
                validateInput = false;
            }


            var billingMemberId = $('#PendingInvBillingMemberId').val();

            if (billingMemberId == null || billingMemberId == '') {
                billingMemberId = 0;
            }

            var billedMemberId = $('#PendingInvBilledMemberId').val();

            if (billedMemberId == null || billedMemberId == '') {
                billedMemberId = 0;
            }

            if (billingMemberId > 0 && billedMemberId > 0 && validateInput == true) {
                if (parseInt(billedMemberId) == parseInt(billingMemberId)) {
                    showClientErrorMessage('Billing and Billed Member should not be same');
                    alert('Billing and Billed Member should not be same');
                    return false;
                }

            }

            if (validateInput) {
                $("#clientErrorMessageContainer").css("display", "none");
                $("#clientSuccessMessageContainer").css("display", "none");

                var postUrl = '<%: Url.Action("PendingInvoiceSearchGridData", "ManageSystemMonitor", new { area = "ISOps"}) %>';
                var url = postUrl + "?" + $.param({ billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, billingCategoryId: BillingCategory, billingMemberId: billingMemberId, billedMemberId: billedMemberId });

                $("#SysMonPendingInvoices").jqGrid('setGridParam', { url: url }).trigger("reloadGrid", [{ page: 1}]);


            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year,Month,Period."); alert('Please Select Billing Year,Month,Period.'); }
        }

        $('#btnPendingInvStatus').click(function () {

            var BillingYear = $('#PendingInvBillingYear').val();
            var BillingMonth = $('#PendingInvBillingMonth').val();
            var BillingPeriod = $('#PendingInvBillingPeriod').val();

            var selectedInvoiceIds = $("#SysMonPendingInvoices").jqGrid('getGridParam', 'selarrrow');
            if (selectedInvoiceIds == '' || selectedInvoiceIds.length == 0) {
                showClientErrorMessage(" Please Select At Least One Invoice to Update Status");
                alert('Please Select At Least One Invoice to Update Status');
                return false;
            }

            var confirmFlag = confirm('Are you sure want to Update selected Invoice Status to Presented?');

            if (confirmFlag == true) {

                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: '<%: Url.Action("UpdatePendingInvoices", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { selectedInvoiceIDs: selectedInvoiceIds.toString(), billingYear: BillingYear, billingMonth: BillingMonth, periodNumber: BillingPeriod },
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                                alert(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                                SearchPendingInvoices();
                            }
                        }
                    },
                    async: false
                });

            }

        });

        $('#btnResetValidationCache').click(function () {

            var confirmFlag = confirm('Are you sure want to Update Master Data In Services?');

            if (confirmFlag == true) {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("ResetValidationCaches", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {
                    }
                });

            }

        });

        //CMP#665: User Related Enhancements-FRS-v1.2 [Sec 2.2: SIS Ops Reprocessing Tab in the System Monitor] 
        $('#btnIcpRetry').click(function () {
            $.ajax({
                type: "POST",
                url: '<%:Url.Action("RetryFailedEnqueueMessageForIcp", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                dataType: "json",
                success: function (response) {
                    if (!response.IsFailed) {
                        showClientSuccessMessage(response.Message);
                    }
                },
                failure: function (response) {
                }
            });
        });

        $('#btnReArchive').click(function () {
            var reArchiveBillingYear = $('#ReArchiveBillingYear').val();
            var reArchiveBillingMonth = $('#ReArchiveBillingMonth').val();
            var reArchiveBillingPeriod = $('#ReArchiveBillingPeriod').val();
            var validateInput = false;

            if (reArchiveBillingYear != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (reArchiveBillingMonth != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (reArchiveBillingPeriod != '') {
                validateInput = true;
            } else { validateInput = false; }

            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("Rearchive", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { reArchiveBillingYear: reArchiveBillingYear, reArchiveBillingMonth: reArchiveBillingMonth, reArchiveBillingPeriod: reArchiveBillingPeriod },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year,Month And Period"); }

        });




        $('#LegalXmlGeneration').click(function () {
            var BillingYear = $('#LegalXmlBillingYear').val();
            var BillingMonth = $('#LegalXmlBillingMonth').val();
            var validateInput = true;
            if (BillingYear == '') {
                validateInput = false;
            }
            else if (BillingMonth == '') {

                validateInput = false;
            }
            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("IsValidMonthForXmlGeneration", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { billingYear: BillingYear, billingMonth: BillingMonth },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);

                                $.ajax({
                                    type: "POST",
                                    url: '<%:Url.Action("GenerateLegalXmlfromSm", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                                    data: { billingYear: BillingYear, billingMonth: BillingMonth },
                                    dataType: "json",
                                    success: function (response) {
                                        if (response.IsFailed) {
                                            if (response.Message) {
                                                showClientErrorMessage(response.Message);
                                            }
                                        } else {
                                            if (response.Message) {
                                                showClientSuccessMessage(response.Message);
                                            }
                                        }
                                    },
                                    failure: function (response) {

                                    }
                                });

                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            }
            else { validateInput = false; showClientErrorMessage("Please Select Billing Year and Month"); }
        });




        $("#FileTypeId").bind('change', function () {

            var FileType = $('#FileTypeId').val();
            //SCP#340872 - Issue in 'Daily IS-XML files for Receivables IS-WEB Invoices' output file
            if (FileType == 7) {
                $('#divFileGenerationDate').show();
                $(".indi").hide();
                $('#InvoiceBillingYear').attr('disabled', true);
                $('#InvoiceBillingMonth').attr('disabled', true);
                $('#InvoiceBillingPeriod').attr('disabled', true);

            }
            else {
                $('#divFileGenerationDate').hide();
                $(".indi").show();
                $('#InvoiceBillingYear').attr('disabled', false);
                $('#InvoiceBillingMonth').attr('disabled', false);
                $('#InvoiceBillingPeriod').attr('disabled', false);
            }
        });

        $(document).ready(function () {
            $('#divFileGenerationDate').hide();

            var $datePickers = $(".datePicker");
            $datePickers.datepicker({ dateFormat: _dateFormat, showOn: 'both', buttonImage: _calendarIcon, buttonImageOnly: true, onClose: sanitizeDate });
            $datePickers.watermark(_dateWatermark);
            $datePickers.attr('maxlength', 11);

            $('#LegalXmlBillingPeriod').val('<%=ViewData["currentMonth"]%>');
        });



        $('#btnUatpAtcanGeneration').click(function () {

            var MemberId = $('#UatpMemberId').val();
            var BillingYear = $('#UatpBillingYear').val();
            var BillingMonth = $('#UatpBillingMonth').val();
            var BillingPeriod = $('#UatpBillingPeriod').val();
            var BillingTypeId = $('#UatpBillingType').val();

            var validateInput = false;

            if (BillingYear != '') {
                validateInput = true;
            } else {
                validateInput = false;
            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
            } else {
                validateInput = false;
            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
            } else {
                validateInput = false;
            }
            if (MemberId != '' && validateInput == true) {
                validateInput = true;
            }
            else {
                MemberId = 0;
            }

            if (validateInput) {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueUatpAtcanDetails", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: MemberId, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, billingTypeId: BillingTypeId },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            }
            else { validateInput = false; showClientErrorMessage("Please Select Member, Billing Year, Billing Month , Period And Billing Type"); }

        });



        $('#AutoBillingFileGeneration').click(function () {

            var MemberId = $('#RevRecMemberId').val();
            var BillingYear = $('#RevRecBillingYear').val();
            var BillingMonth = $('#RevRecBillingMonth').val();
            var BillingPeriod = $('#RevRecBillingPeriod').val();
            var FileType = $('#RevRecFileType').val();

            var CouponStatus = 1;
            if ($('#chkPendingFailed').prop('checked') == true && $('#chkIncludeInPrevFile').prop('checked') == false) {
                CouponStatus = 1;
            }
            if ($('#chkPendingFailed').prop('checked') == false && $('#chkIncludeInPrevFile').prop('checked') == true) {
                CouponStatus = 2;
            }
            if ($('#chkPendingFailed').prop('checked') == true && $('#chkIncludeInPrevFile').prop('checked') == true) {
                CouponStatus = 3;
            }

            var validateInput = false;

            if (BillingYear != '') {
                validateInput = true;
            } else {
                validateInput = false;
            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
            } else {
                validateInput = false;
            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
            } else {
                validateInput = false;
            }
            if (MemberId != '' && validateInput == true) {
                validateInput = true;
            }
            if (FileType != '' && validateInput == true) {
                validateInput = true;
            }
            else {
                validateInput = false;
            }
            if (validateInput) {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueAutoBillingFileGeneration", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: MemberId, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, fileType: FileType, couponStatus: CouponStatus },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });

            }
            else { validateInput = false; showClientErrorMessage("Please Select Auto-billing Airline, Billing Year, Billing Month And Billing Period"); }
        });


        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        $('#MISCDailyBilateralISXML').click(function () {
            var selectedTargetDate = $('#MiscDailyIsXmlTargetDate').val();
            var selectedMemberId = $('#MiscDailyIsXmlMemberId').val();
            if (selectedTargetDate == '') {
                showClientErrorMessage("Please Select MISC Daily Bilateral IS-XML Target Date.");
            }
            else {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueMiscDailyBilateralIsXml", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: selectedMemberId, targetDate: selectedTargetDate },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });
            }
        });

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        $('#MISCDailyBilateralOAR').click(function () {
            var selectedTargetDate = $('#MiscDailyOarTargetDate').val();
            var selectedMemberId = $('#MiscDailyOarMemberId').val();
            if (selectedTargetDate == '') {
                showClientErrorMessage("Please Select MISC Daily Bilateral Offline Archive Target Date.");
            }
            else {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueMiscDailyBilateralOar", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: selectedMemberId, targetDate: selectedTargetDate },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });
            }
        });

        //CMP#622: MISC Outputs Split as per Location IDs -- Loc Spec IS-XML Outbound File
        $('#MiscLocIsXmlFileGeneration').click(function () {

            var BillingYear = $('#MiscLocIsXmlBillingYear').val();
            var BillingMonth = $('#MiscLocIsXmlBillingMonth').val();
            var BillingPeriod = $('#MiscLocIsXmlBillingPeriod').val();
            var BilledMember = $('#MiscLocIsXmlMemberId').val();
            var LocationCode = $('#MiscLocIsXmlLocationCode').val();

            var validateInput = false;

            if (BillingYear != '') {
                $("#MiscLocIsXmlBillingYear").css("background", "none");
                validateInput = true;
            } else {
                validateInput = false;

            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocIsXmlBillingMonth").css("background", "none");
            } else {
                validateInput = false;

            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocIsXmlBillingPeriod").css("background", "none");
            } else {
                validateInput = false;

            }

            if ((BilledMember != null && BilledMember != '' && BilledMember > 0) && validateInput == true) {
                validateInput = true;
                $("#MiscLocIsXmlMemberId").css("background", "none");
            } else {
                validateInput = false;
            }


            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueueMiscLocIsXmlFile", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: BilledMember, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, locationCode: LocationCode },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);

                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);

                            }
                        }
                    },
                    failure: function (response) {

                    }
                });


            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year, Month , Period And Billed Member"); }
        });

        //CMP#622: MISC Outputs Split as per Location IDs -- ISWEB XML Loc Spec File
        $('#MiscIsWebXmlFileGeneration').click(function () {

            var BillingMember = $('#MiscIsWebXmlMemberId').val();
            var LocationCode = $('#MiscIsWebXmlLocationCode').val();
            var XmlFileGenDate = $('#MiscIsWebXmlGenDate').val();

            if (XmlFileGenDate == '') {
                showClientErrorMessage("Please Select Misc Location Specific IsWeb Xml Original Generation Date.");
            }
            else if (BillingMember == '' || BillingMember == 0) {
                showClientErrorMessage("Please Select Misc Location Specific IsWeb Xml Billing member.");
            }
            else {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueueMiscLocIsWebXmlFile", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: BillingMember, locationCode: LocationCode, fileGenerationDate: XmlFileGenDate },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });
            }
        });


        //CMP#622: MISC Outputs Split as per Location IDs -- On Behalf Loc Spec File
        $('#MiscLocOnBehalfFileGeneration').click(function () {

            var BillingYear = $('#MiscLocOnBehalfBillingYear').val();
            var BillingMonth = $('#MiscLocOnBehalfBillingMonth').val();
            var BillingPeriod = $('#MiscLocOnBehalfBillingPeriod').val();
            var BillingMember = $('#MiscLocOnBehalfMemberId').val();
            var LocationCode = $('#MiscLocOnBehalfLocationCode').val();

            var validateInput = false;

            if (BillingYear != '') {
                $("#MiscLocOnBehalfBillingYear").css("background", "none");
                validateInput = true;
            } else {
                validateInput = false;

            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocOnBehalfBillingMonth").css("background", "none");
            } else {
                validateInput = false;

            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocOnBehalfBillingPeriod").css("background", "none");
            } else {
                validateInput = false;

            }

            if ((BillingMember != null && BillingMember != '' && BillingMember > 0) && validateInput == true) {
                validateInput = true;
                $("#MiscLocOnBehalfMemberId").css("background", "none");
            } else {
                validateInput = false;
            }


            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueueMiscLocOnBehalfFile", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: BillingMember, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, locationCode: LocationCode },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);

                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);

                            }
                        }
                    },
                    failure: function (response) {

                    }
                });


            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year, Month , Period And Billing Member"); }
        });

        //CMP#622: MISC Outputs Split as per Location IDs -- OAR Billing Receivable
        $('#MiscLocOarBlgFileGeneration').click(function () {

            var BillingYear = $('#MiscLocOarBlgBillingYear').val();
            var BillingMonth = $('#MiscLocOarBlgBillingMonth').val();
            var BillingPeriod = $('#MiscLocOarBlgBillingPeriod').val();
            var BilledMember = $('#MiscLocOarBlgMemberId').val();
            var LocationCode = $('#MiscLocOarBlgLocationCode').val();

            var validateInput = false;

            if (BillingYear != '') {
                $("#MiscLocOarBlgBillingYear").css("background", "none");
                validateInput = true;
            } else {
                validateInput = false;

            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocOarBlgBillingMonth").css("background", "none");
            } else {
                validateInput = false;

            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocOarBlgBillingPeriod").css("background", "none");
            } else {
                validateInput = false;

            }

            if ((BilledMember != null && BilledMember != '' && BilledMember > 0) && validateInput == true) {
                validateInput = true;
                $("#MiscLocOarBlgMemberId").css("background", "none");
            } else {
                validateInput = false;
            }


            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueueMiscLocOarRecFile", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: BilledMember, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, locationCode: LocationCode },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);

                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);

                            }
                        }
                    },
                    failure: function (response) {

                    }
                });


            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year, Month , Period And Billing Member"); }
        });



        //CMP#622: MISC Outputs Split as per Location IDs -- OAR Billed Payable
        $('#MiscLocOarBldFileGeneration').click(function () {

            var BillingYear = $('#MiscLocOarBldBillingYear').val();
            var BillingMonth = $('#MiscLocOarBldBillingMonth').val();
            var BillingPeriod = $('#MiscLocOarBldBillingPeriod').val();
            var BilledMember = $('#MiscLocOarBldMemberId').val();
            var LocationCode = $('#MiscLocOarBldLocationCode').val();

            var validateInput = false;

            if (BillingYear != '') {
                $("#MiscLocOarBldBillingYear").css("background", "none");
                validateInput = true;
            } else {
                validateInput = false;

            }

            if (BillingMonth != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocOarBldBillingMonth").css("background", "none");
            } else {
                validateInput = false;

            }

            if (BillingPeriod != '' && validateInput == true) {
                validateInput = true;
                $("#MiscLocOarBldBillingPeriod").css("background", "none");
            } else {
                validateInput = false;

            }

            if ((BilledMember != null && BilledMember != '' && BilledMember > 0) && validateInput == true) {
                validateInput = true;
                $("#MiscLocOarBldMemberId").css("background", "none");
            } else {
                validateInput = false;
            }


            if (validateInput) {

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueueMiscLocOarPayFile", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: BilledMember, billingYear: BillingYear, billingMonth: BillingMonth, billingPeriod: BillingPeriod, locationCode: LocationCode },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);

                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);

                            }
                        }
                    },
                    failure: function (response) {

                    }
                });


            } else { validateInput = false; showClientErrorMessage("Please Select Billing Year, Month , Period And Billed Member"); }
        });

        //CMP#622: MISC Outputs Split as per Location IDs -- Daily OAR File
        $('#MiscDailyOarLocFileGeneration').click(function () {

            var selectedTargetDate = $('#MiscDailyOarLocGenDate').val();
            var selectedMemberId = $('#MiscDailyOarLocMemberId').val();
            var selectedLocationCode = $('#MiscDailyOarLocLocationCode').val();

            if (selectedTargetDate == '') {
                showClientErrorMessage("Please Select MISC Daily Bilateral Location Specific Offline Archive Target Date.");
            }
            else if (selectedMemberId == '' || selectedMemberId == 0) {
                showClientErrorMessage("Please Select MISC Daily Bilateral Location Specific Offline Archive Billed member.");
            }
            else {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueMiscDailyLocOar", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: selectedMemberId, targetDate: selectedTargetDate, locationCode: selectedLocationCode },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });
            }
        });

        //CMP#622: MISC Outputs Split as per Location IDs -- Daily XML File
        $('#MiscDailyIsXmlLocFileGeneration').click(function () {

            var selectedTargetDate = $('#MiscDailyXmlLocGenDate').val();
            var selectedMemberId = $('#MiscDailyXmlLocMemberId').val();
            var selectedLocationCode = $('#MiscDailyXmlLocLocationCode').val();

            if (selectedTargetDate == '') {
                showClientErrorMessage("Please Select MISC Daily Bilateral Location Specific IS-XML Target Date.");
            }
            else if (selectedMemberId == '' || selectedMemberId == 0) {
                showClientErrorMessage("Please Select MISC Daily Bilateral Location Specific IS-XML Billed member.");
            }
            else {
                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("EnqueMiscDailyLocXml", "ManageSystemMonitor", new { area = "ISOps"}) %>',
                    data: { memberId: selectedMemberId, targetDate: selectedTargetDate, locationCode: selectedLocationCode },
                    dataType: "json",
                    success: function (response) {
                        if (response.IsFailed) {
                            if (response.Message) {
                                showClientErrorMessage(response.Message);
                            }
                        } else {
                            if (response.Message) {
                                showClientSuccessMessage(response.Message);
                            }
                        }
                    },
                    failure: function (response) {

                    }
                });
            }
        });

    });

  
</script>
<%}%>
