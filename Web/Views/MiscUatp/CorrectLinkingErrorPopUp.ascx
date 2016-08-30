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
          <%:Html.TextBoxFor(model => model.CorrectLinkingFileName, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff" })%>
       </div>
       <div style="width:250px;">
            <label>
            Exception Code:</label>
            
          <%:Html.TextBoxFor(invoice => invoice.CorrectLinkingExceptionCode, new { @readOnly = true, style = "border: none; background-color:#f5faff" })%>
            </div>
            <div >
              <label>
                Error Description:</label>
            
              <%:Html.TextAreaFor(invoice => invoice.CorrectLinkingErrorDescription, new { @readOnly = true, style = "width:300px;border: none;" })%>
            </div>
            
        </div>
        <div id="rmpopup">
            <div style="width:250px">
              <label><span>*</span>
                Rejected Invoice No:</label>
              <%:Html.TextBoxFor(invoice => invoice.YourInvoiceNo)%>
            </div>
            <div style="width:250px;">
              <label><span>*</span>
                Settlement Period of Rejected Invoice(YYYYMMPP):</label>
              <%:Html.TextBoxFor(invoice => invoice.YourInvoiceBillingDate)%>
            </div>
      
           <%:Html.HiddenFor(model => model.BatchUpdateAllowed)%>
           <%:Html.HiddenFor(model => model.ExceptionSummaryId)%>
           <%:Html.HiddenFor(model => model.ExceptionDetailId)%>
            <%:Html.HiddenFor(model => model.ErrorLevel)%>
           
        </div> 
      
        <div  id="bmpopup">
         <div style="width:250px">
            <label><span>*</span>
           Correspondence Ref Number:</label>
            <%:Html.TextBoxFor(model => model.CorrespondenceRefNo)%>
              <%--<%: Html.TextBox("CorrespondenceRefNo", Model.CorrespondenceRefNo != 0 ? Model.CorrespondenceRefNo.ToString(FormatConstants.CorrespondenceNumberFormat) : string.Empty, new { @class = "numeric", maxLength = 11 })%>--%>

            <%:Html.HiddenFor(model => model.TranscationId)%> 
            <%:Html.HiddenFor(model => model.ReasonCode)%>
             <%:Html.HiddenFor(model => model.RejectionStage)%>
              <%:Html.HiddenFor(model => model.InvoiceID)%>
                <%:Html.HiddenFor(model => model.PkReferenceId)%>
                <!--//SCP252342 - SRM: ICH invoice in ready for billing status-->
                <%:Html.HiddenFor(model => model.LastUpdatedOn)%>
          </div>
             <div style="width:250px;">
              <label><span>*</span>
                Rejection Invoice Number:</label>
              <%:Html.TextBoxFor(invoice => invoice.YourInvoiceNo, new { id = "CorrespondenceRejInvoiceNo" })%>
            </div>
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


