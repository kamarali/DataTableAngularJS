<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
 <div id="successMessageForRM" class="hidden">
    <div id="successMessageContainer" class="serverMessage serverSuccessMessage">
      <div style="float: left; width: 20px">
         <img alt="Success" src='<%:Url.Content("~/Content/Images/success_message.png")%>' />
      </div>
      <div id="successMessageContent" style="margin-left: 23px">
      </div>
    </div>
  </div>

  <div id="errorMessageForRM" class="hidden">
    <div id="clientErrorMessageContainer" class="clientMessage clientErrorMessage">
      <div style="float: left; width: 20px">
       <img alt="Error" src='<%:Url.Content("~/Content/Images/error_message.png")%>'/>
      </div>
      <div id="clientErrorMessage" style="margin-left: 23px">
         <%--SCP450430: Exception occurred in Report Download Service. - SIS Production--%>
         An error occurred while generating the audit trail. Please try generation of audit trail again.
      </div>
    </div>
  </div>

<div id="Div2">
  <h2>
    Stage 3 Rejection Memos Linked with Correspondence</h2>
  <%= Html.Trirand().JQGrid(ViewData.Model, "LinkedRejectionMemoGridId")%>
</div>

<div class="buttonContainer">
  <input type="button" value="Generate Audit Trail PDF(s)" class="primaryButton" id="generateAuditTrailPdf"
    onclick="GenerateAuditTrailLinkedCorrRMs();" />
</div>
