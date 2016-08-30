<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
             <%:Html.SupportingDocBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, Model.SupportingDocumentTypeId)%>
          <%--<%:Html.BillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>--%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(m => m.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            <span>*</span> Billed Member:</label>
          <%:Html.HiddenFor(m => m.BilledMemberId)%>
          <%:Html.TextBoxFor(m => m.BilledMemberText, new { @class = "autocComplete" })%>
        </div>

       <%-- <div>
          <label>
            Type:</label>
          <%:Html.SupportingDocTypeDropdownList(ControlIdConstants.SupportingDocumentType, (int)Model.Type)%>
        </div>--%>

        <div>
          <label>
            Invoice Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
       
        
      </div>
     

      <div>

      <div>
       <label>
           Billing Code:</label>
       <%-- <%:Html.StaticCgoBillingCodeDropdownListFor(m => m.DisplayBillingCode,true)%>--%>
       <%:Html.StaticCgoBillingCodeDropdownListFor(m => m.BillingCode, true)%>
      </div>

        <div>
        <label for="BatchSequenceNumber">
           Batch Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.BatchSequenceNumber, new { @class = "integer", @maxLength = 5 })%>
      </div>
      <div>
        <label for="RecordSequenceWithinBatch">
           Sequence Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.RecordSequenceWithinBatch, new { @class = "integer", @maxLength = 5 })%>
      </div>
        <div>
          <label>
            RM/BM/CM No:</label>
          <%: Html.TextBox(ControlIdConstants.RMBMCMNumber, Model.RMBMCMNumber, new { @maxLength = 11 })%>
      </div>
      <div>
          <label for="AWBSerialNumber">
             AWB No.:</label>
          <%:Html.TextBoxFor(m => m.AWBSerialNumber, new { @class = "integer", @maxLength = 7 })%>
        </div>
      
      
      </div>
      
      <div>
            
        <div>
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator - Original:
          </label>
          <%:Html.SupportingDocAttachmentIndicatorDropdownListFor(m => m.AttachmentIndicatorOriginal)%>
        </div>
        <div>
          <label for="IsMismatchCases">
            Mismatch Cases:
          </label>
          <%:Html.CheckBoxFor(m => m.IsMismatchCases)%>
        </div>
      </div>

    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="button" onclick="resetSearchCriteria();" value="Clear" />
</div>
<div class="clear">
</div>
