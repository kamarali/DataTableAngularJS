<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<h2>
  Correct Linking Error</h2>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style="width: 250px;">
          <label for="CorrectLinkingFileName">
            File Name:
          </label>         
          <%:Html.TextBoxFor(model => model.CorrectLinkingFileName, new { id = "CorrectLinkingFileName", @readOnly = true, style = "width:300px;border: none; background-color:#f5faff;",tabindex=-1 })%>
        </div>
        <div style="width: 250px;">
          <label for="CorrectLinkingExceptionCode">
            Exception Code:
          </label>
          
          <%:Html.TextBoxFor(invoice => invoice.CorrectLinkingExceptionCode, new {id="CorrectLinkingExceptionCode", @readOnly = true, style = "border: none; background-color:#f5faff;",tabindex=-1 })%>
        </div>
        <div>
          <label for="CorrectLinkingErrorDescription">
            Error Description:
          </label>         
          <%:Html.TextAreaFor(invoice => invoice.CorrectLinkingErrorDescription, new { id = "CorrectLinkingErrorDescription", @readOnly = true, style = "width:300px;border: none;background-color:#f5faff;", tabindex = -1 })%>
        </div>
      </div>
      <div id="rmpopup">
        <div style="width: 250px">
          
          <label for="YourInvoiceNo">
           <span class="required">*</span> Your Invoice No:
          </label>        
          <%:Html.TextBoxFor(invoice => invoice.YourInvoiceNo, new { id = "YourInvoiceNo" , maxLength = 10 ,tabindex=1})%>
        </div>
        <div style="width: 250px;">
         
          <label for="YourInvoiceBillingDate">
             <span class="required">*</span> Your Invoice Billing Date(YYYYMMPP):
          </label>
         
          <%:Html.TextBoxFor(invoice => invoice.YourInvoiceBillingDate, new { id = "YourInvoiceBillingDate", tabindex=2 })%>
        </div>
        <div>
         
          <label for="YourRejectionMemoNo">
             Your Rejection Memo Number:
          </label>
       
          <%:Html.TextBoxFor(invoice => invoice.YourRejectionMemoNo, new { id = "YourRejectionMemoNo",tabindex=3 })%>
        </div>
      </div>
      <!--End of RM Popup -->
      <div id="rmpopup1">

        <div style="width: 250px;">
          
           <label for="FimBmCmIndicatorDisplay">
           FIM BM CM Indicator :
          </label>         
          <%:Html.TextBoxFor(model => model.FimBmCmIndicatorDisplay, new { @readOnly = true, tabindex=-1  })%>
        </div>

        <div style="width: 250px;">
         
          <label for="FimBmCmNo">
           FIM Number/Billing Memo/Credit Memo Number:
          </label>
         
          <%:Html.TextBoxFor(invoice => invoice.FimBmCmNo, new { id = "FimBmCmNo", tabindex=4 })%>
        </div>
        <div style="width: 250px;">
         
          <label for="FimCouponNo">
             FIM Coupon Number:
          </label>
       
          <%:Html.TextBoxFor(invoice => invoice.FimCouponNo, new { id = "FimCouponNo",tabindex=5 })%>
        </div>
        <%:Html.HiddenFor(model => model.BatchUpdateAllowed)%>
        <%:Html.HiddenFor(model => model.ExceptionSummaryId)%>
        <%:Html.HiddenFor(model => model.ExceptionDetailId)%>
        <%:Html.HiddenFor(model => model.ErrorLevel)%>
        <%:Html.HiddenFor(model => model.FimBmCmIndicator) %>
        <%:Html.HiddenFor(model => model.SourceCodeId) %>
      </div>
      <!--End of RM Popup1 -->
      <div id="bmpopup">
        <div style="width: 250px">
         
          <label for="CorrespondenceRefNo">
            <span class="required">*</span>  Correspondence Ref Number:
          </label>
        
          <%:Html.TextBoxFor(model => model.CorrespondenceRefNo, new { id = "CorrespondenceRefNo"  })%>
          <%:Html.HiddenFor(model => model.TranscationId)%>
          <%:Html.HiddenFor(model => model.ReasonCode)%>
          <%:Html.HiddenFor(model => model.RejectionStage)%>
          <%:Html.HiddenFor(model => model.InvoiceID)%>
          <%:Html.HiddenFor(model => model.PkReferenceId)%>
          <!-- ID : 252342 - SRM: ICH invoice in ready for billing status-->
           <%:Html.HiddenFor(model => model.LastUpdatedOn)%>
        </div>
        <%--<div style="width: 250px;">
         
          <label for="CorrespondenceRejInvoiceNo">
            <span class="required">*</span>  Provisional Invoice No.:
          </label>
         
          <%:Html.TextBoxFor(invoice => invoice.YourInvoiceNo, new { id = "CorrespondenceRejInvoiceNo" })%>
        </div>--%>
      </div>
      <div id="formdpopup">
        <div style="width: 250px">
        
          <label for="ProvisionalInvoiceNo">
            <span class="required">*</span> Provisional Invoice No :
          </label>
        
          <%:Html.TextBoxFor(invoice => invoice.ProvisionalInvoiceNo, new { id = "ProvisionalInvoiceNo" })%>
        </div>
        <div style="width: 250px">
       
          <label for="BatchSeqNo">
             <span class="required">*</span> Batch Number of Provisional Invoice:
          </label>
         
          <%:Html.TextBoxFor(invoice => invoice.BatchSeqNo, new { id = "BatchSeqNo" })%>
        </div>
        <div style="width: 250px">
         
          <label for="BatchRecordSeq">
            <span class="required">*</span>  Record Sequence within Batch of Provisional Invoice:
          </label>
         
          <%:Html.TextBoxFor(invoice => invoice.BatchRecordSeq, new { id = "BatchRecordSeq" })%>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
  <div>
    <div class="buttonContainer" align="left">
      <input class="primaryButton" type="submit" value="Update" onclick="javascript:updateCorrctLinkingbuttonclick();" />
      <input class="secondaryButton" type="button" value="Close" onclick="javascript:CloseCorrectLinkingErrorClick();" />
    </div>
  </div>
</div>
