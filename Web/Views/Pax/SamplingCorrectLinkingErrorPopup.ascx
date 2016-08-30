<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<h2>
    Correct Linking Error</h2>
<div class="searchCriteria">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow">
            <div>
                <div style="width: 250px;">
                    <label>
                        File Name:</label>
                    <%:Html.TextBoxFor(model => model.samplingFileName, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff" })%>
                </div>
                <div style="width: 250px;">
                    <label>
                        Exception Code:</label>
                    <%:Html.TextBoxFor(invoice => invoice.SamplingExceptionCode, new { @readOnly = true, style = "border: none; background-color:#f5faff" })%>
                </div>
                <div>
                    <label>
                        Error Description:</label>
                    <%:Html.TextAreaFor(invoice => invoice.SamplingLinkErrorDescription, new { @readOnly = true, style = "width:300px;border: none;" })%>
                </div>
            </div>
            <div id="rmpopupFormc">
                <div style="width: 250px">
                    <label>
                        <span>*</span> Provisional Invoice No:</label>
                    <%:Html.TextBoxFor(invoice => invoice.SamplingYourInvoiceNo)%>
                </div>
                <div style="width: 250px;">
                    <label>
                        <span>*</span> Batch Number of Provisional Invoice:</label>
                    <%:Html.TextBoxFor(invoice => invoice.SamplingBatchSeqNo)%>
                </div>
                <div>
                    <label style = "width:300px">
                     <span>*</span> Record Sequence within Batch of Provisional Invoice:</label>
                    <%:Html.TextBoxFor(invoice => invoice.SamplingBatchRecordSeq)%>
                </div>
              
                <%:Html.HiddenFor(model => model.BatchUpdateAllowed)%>
                <%:Html.HiddenFor(model => model.ExceptionSummaryId)%>
                <%:Html.HiddenFor(model => model.ExceptionDetailId)%>
                <%:Html.HiddenFor(model => model.ErrorLevel)%>
                <%:Html.HiddenFor(model => model.FimBmCmIndicator)  %>
            </div>
            <div id="bmpopupFormc">
                <div style="width: 250px">
                    <label>
                        <span>*</span> Correspondence Ref Number:</label>
                    <%:Html.TextBoxFor(model => model.CorrespondenceRefNo)%>
                    <%:Html.HiddenFor(model => model.TranscationId)%>
                    <%:Html.HiddenFor(model => model.ReasonCode)%>
                    <%:Html.HiddenFor(model => model.RejectionStage)%>
                    <%:Html.HiddenFor(model => model.InvoiceID)%>
                    <%:Html.HiddenFor(model => model.PkReferenceId)%>
                    <!--//SCP252342 - SRM: ICH invoice in ready for billing status-->
                    <%:Html.HiddenFor(model => model.LastUpdatedOn)%>
                </div>
                <div style="width: 250px;">
                    <label>
                        <span>*</span> Rejection Invoice Number:</label>
                    <%:Html.TextBoxFor(invoice => invoice.YourInvoiceNo, new { id = "CorrespondenceRejInvoiceNo" })%>
                </div>
            </div>
           
        </div>
        <div class="clear">
        </div>
    </div>
    <div>
        <div class="buttonContainer" align="left">
            <input class="primaryButton" type="submit" value="Update" onclick="javascript:SamplingupdateCorrctLinkingbuttonclick();" />
            <input class="secondaryButton" type="button" value="Close" onclick="javascript:SamplingCloseCorrectLinkingErrorClick();" />
        </div>
    </div>
</div>
