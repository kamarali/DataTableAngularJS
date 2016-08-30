<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>

<h2>
  Correct Linking Error</h2>

  <div class="searchCriteria">
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
              <div style="width:250px;">
        <label>
          File Name:</label>
          <%:Html.TextBoxFor(model => model.CorrectLinkingFileName, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff" , tabindex=-1})%>
       </div>
       <div style="width:250px;">
            <label>
            Exception Code:</label>
            
          <%:Html.TextBoxFor(invoice => invoice.CorrectLinkingExceptionCode, new { @readOnly = true, style = "border: none; background-color:#f5faff", tabindex = -1 })%>
            </div>
            <div >
              <label>
                Error Description:</label>
            
              <%:Html.TextAreaFor(invoice => invoice.CorrectLinkingErrorDescription, new { @readOnly = true, style = "width:300px;border: none;", tabindex = -1 })%>
            </div>
            
        </div>
        <div id="rmpopup">
            <div style="width:250px">
          <label><span>*</span>
            Your Invoice No:</label>
          <%:Html.TextBoxFor(invoice => invoice.YourInvoiceNo, new{ tabindex=1})%>
        </div>
        <div style="width:250px;">
          <label><span>*</span>
            Your Invoice Billing Date(YYYYMMPP):</label>
          <%:Html.TextBoxFor(invoice => invoice.YourInvoiceBillingDate, new { tabindex = 2 })%>
        </div>
        <div>
          <label>
           Your Rejection Memo Number:</label>
          <%:Html.TextBoxFor(invoice => invoice.YourRejectionMemoNo, new { tabindex = 3 })%>
          
        </div>
           <%:Html.HiddenFor(model => model.BatchUpdateAllowed)%>
           <%:Html.HiddenFor(model => model.ExceptionSummaryId)%>
           <%:Html.HiddenFor(model => model.ExceptionDetailId)%>
            <%:Html.HiddenFor(model => model.ErrorLevel)%>
           
        </div> 
        <div   id="rmpopupcontain">
         <div style="width:250px;">
          <label>
           Your Billing/Credit Memo Number:</label>
          <%:Html.TextBoxFor(invoice => invoice.YourBmCmNo, new { tabindex = 4 })%>
          
        </div>
        <div style="width:250px;">
          <label>
           BM/CM Indicator:</label>
           <%:Html.BmCmIndicatorDropdownList("BmCmIndicator", Model.BmCmIndicator.ToString())%>
          
        </div>
        </div>
        <div  id="bmpopupcontain">
            <label><span>*</span>
           Correspondence Ref Number:</label>
            <%:Html.TextBoxFor(model => model.CorrespondenceRefNo, new { tabindex = 5 })%>
            <%:Html.HiddenFor(model => model.TranscationId)%> 
            <%:Html.HiddenFor(model => model.ReasonCode)%>
             <%:Html.HiddenFor(model => model.RejectionStage)%>
              <%:Html.HiddenFor(model => model.InvoiceID)%>
                <%:Html.HiddenFor(model => model.PkReferenceId)%>
                  <!--//SCP252342 - SRM: ICH invoice in ready for billing status-->
                <%:Html.HiddenFor(model => model.LastUpdatedOn)%>
        </div>
    </div>
    <div class="clear">
  </div>
</div>
 <div>

            <div class="buttonContainer" align="left">
                <input class="primaryButton" type="submit" value="Update"  onclick="javascript:updateCorrctLinkingbuttonclick();" />  
                <input class="secondaryButton" type="button"  value="Close" onclick="javascript:CloseCorrectLinkingErrorClick();" />

            </div>
        </div> 
</div>


