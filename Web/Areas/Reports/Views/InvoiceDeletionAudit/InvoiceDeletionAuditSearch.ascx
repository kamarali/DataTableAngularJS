<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.AuditDeletedInvoice>" %>
<script type="text/javascript">

    $(document).ready(function () {

        registerAutocomplete('BilledMember', 'BilledMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        $('#BillingMonth').val('<%=ViewData["currentMonth"]%>');
        $('#BillingYear').val('<%=ViewData["currentYear"]%>');


    });
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
              <div>
               <div>
                    <label>
                        <span class="required">* </span>Billing Category:
                    </label>
                    <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId, categoryType: "billingcategory")%>
               </div>
               <div>
                    <label>
                        <span class="required">* </span>Billing Year:
                    </label>
                    <%:Html.BillingYearTwoDropdownListFor(model => model.BillingYear)%>
               </div>
                <div>
                    <label>
                        <span class="required">* </span>Billing Month:
                    </label>
                    <%: Html.BillingMonthDropdownListFor(model => model.BillingMonth)%>
                 </div>
                <div>
                    <label>
                        Billing Period:
                    </label>
                    <%: Html.StaticBillingPeriodDropdownListAllFor(model => model.BillingPeriod)%>
               </div>
            </div>
            <div>
               <div>
                    <label>
                        Billed Member:
                    </label>
                    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                    Desc: Increasing field size by specifying in-line width
                    Ref: 3.5 Table 19 Row 18 -->
                    <%:Html.TextBoxFor(model => model.BilledMember, new { @class = "autocComplete textboxWidth" })%>
                    <%:Html.TextBoxFor(model => model.BilledMemberId, new { @class = "hidden" })%>
               </div>
               <div>
                    <label>
                        Invoice Number:
                    </label>
                    <%: Html.TextBoxFor(model => model.InvoiceNo, new { @Id = "InvoiceNo",  @maxLength = 10 })%>
               </div>
          <div>
               
                    <label>
                        Deleted From Date:
                    </label>
                    <%:Html.TextBox("DeletionDateFrom", null, new { @class = "datePicker", @id = "DeletionDateFrom" })%>
                    </div>
           <div>
               
                    <label>
                        Deleted To Date:
                    </label>
                    <%:Html.TextBox("DeletionDateTo", null, new { @class = "datePicker", @id = "DeletionDateTo" })%>
                   </div>
         </div>
         <div>
                <div>
                    <label>
                        Deleted By (Username):</label>
                    <%: Html.TextBoxFor(model => model.DeletedBy, new { @Id = "DeletedBy" })%>
               </div>
        </div>
   </div>
    <div class="clear">
    </div>
</div>
