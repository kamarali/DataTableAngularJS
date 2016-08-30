<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Correspondence>" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax" %>
<h2>
  Correspondence Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="billingMember">
          From Member:
        </label>
        <%:Html.TextBox(ControlIdConstants.FromMemberText, string.Format("{0}-{1}", Model.FromMember.MemberCodeAlpha, Model.FromMember.MemberCodeNumeric), new { @readonly = true })%>
        <%:Html.HiddenFor(c => c.FromMemberId)%>
         <%:Html.HiddenFor(c => c.RejectionMemoIds)%>
      </div>
      <div>
        <label for="billedMember">
          To Member:
        </label>
        <%:Html.TextBox(ControlIdConstants.ToMemberText, string.Format("{0}-{1}", Model.ToMember.MemberCodeAlpha, Model.ToMember.MemberCodeNumeric), new { @readonly = true })%>
        <%:Html.HiddenFor(c => c.ToMemberId)%>
      </div>
      <div>
        <label for="corrDate">
          Correspondence Date:
        </label>
        <%:Html.TextBox(ControlIdConstants.CorrespondenceDate, Model.CorrespondenceDate.ToString(FormatConstants.DateFormat), new { @readonly = true })%>
      </div>
      <div>
        <label for="corrNo">
          Correspondence Reference Number:
        </label>
        <%:Html.TextBox(ControlIdConstants.CorrespondenceNumber, Model.CorrespondenceNumber.HasValue ? Model.CorrespondenceNumber.Value.ToString(FormatConstants.CorrespondenceNumberFormat) : string.Empty , new { @readonly = true })%>
      </div>
      <div>
        <label for="corrStage">
          Correspondence Stage:
        </label>
        <%:Html.TextBoxFor(m => m.CorrespondenceStage, new { @readonly = true })%>
      </div>
      <%--CMP526 - Passenger Correspondence Identifiable by Source Code--%>
      <div>
        <label for="SourceCode">
            Source Code:
        </label>                
        <%:Html.TextBoxFor(m => m.SourceCode, new { @readonly = true })%>
    </div>
    </div>
    <div>
      <div>
        <label for="toEmailId">
          To E-Mail ID(s):
        </label>
        <%:Html.TextAreaFor(m => m.ToEmailId, new { @disabled = "disabled", @cols = 150, @class = "textAreaTrimText" })%>
      </div>
    </div>
    <div>
       <!-- CMP#657: Retention of Additional Email Addresses in Correspondences
               Adding code to get email ids from initiator and non-initiator and removing
               additional email field -->
       <label for="ToAdditionalEmailIds" class="labelexpandedwidth">
         Additional E-Mail ID(s) pertaining to Initiator, <%: string.Format("{0}-{1}", Model.CorrespondenceStage%2 == 0?Model.ToMember.MemberCodeAlpha:Model.FromMember.MemberCodeAlpha, Model.CorrespondenceStage%2 == 0?Model.ToMember.MemberCodeNumeric:Model.FromMember.MemberCodeNumeric)%>:
      </label>
      <%:Html.TextAreaFor(m => m.AdditionalEmailInitiator, new { @cols = 150, @disabled = "disabled", @class = "textAreaTrimText" })%>
    </div>
    <div>
      <label for="ToAdditionalEmailIds" class="labelexpandedwidth">
        Additional E-Mail ID(s) pertaining to Non-Initiator, <%: string.Format("{0}-{1}", Model.CorrespondenceStage%2 == 0?Model.FromMember.MemberCodeAlpha:Model.ToMember.MemberCodeAlpha, Model.CorrespondenceStage%2 == 0?Model.FromMember.MemberCodeNumeric:Model.ToMember.MemberCodeNumeric)%>:
      </label>
      <%:Html.TextAreaFor(m => m.AdditionalEmailNonInitiator, new { @cols = 150, @disabled = "disabled", @class = "textAreaTrimText" })%>
    </div>
    <div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ourReference">
          Our Reference:
        </label>
        <%: Html.TextBoxFor(corr => corr.OurReference, new { @readonly = true })%>
      </div>
      <div>
        <label for="yourReference">
          Your Reference:
        </label>
        <%:Html.TextBoxFor(corr => corr.YourReference, new { @readonly = true })%>
      </div>
      <div>
        <label for="amountToBeSettled">
          Amount To Be Settled:
        </label>
        <%:Html.CurrencyDropdownListFor(corr => corr.CurrencyId, new Dictionary<string,object> {{ "disabled", "disabled"} })%>
        <%:Html.TextBox(ControlIdConstants.AmountToBeSettled, Model.AmountToBeSettled.ToString(FormatConstants.TwoDecimalsEditFormat), new { @style = "width: 60px;", @class = "amt_14_3", @readonly = true })%>
      </div>
      <div>
          <% if (Html.IsAuthorized(BillingHistoryAndCorrespondence.GrantAuthorityToBill))
                   {%> 
        <label for="authorityToBill">
          Authority To Bill:
        </label>
        <%: Html.CheckBoxFor(corr => corr.AuthorityToBill, new { @disabled = "disabled" })%>
         <%}%>
      </div>
    </div>
    <div>
      <% 
        if (Model.FromMemberId == SessionUtil.MemberId)
        {
      %>
      <div>
        <label for="owner">
          Correspondence Owner:
        </label>
        <%:Html.TextBox(ControlIdConstants.CorrespondenceOwnerName,
                                           Model.CorrespondenceOwnerName,
                                           new
                                             {
                                               @readonly = true
                                             })%>
      </div>
      <%
                }
      %>
      <div>
        <label for="chargeCode">
          Correspondence Status:
        </label>
        <%: Html.TextBoxFor(corr => corr.CorrespondenceStatus, new {@readonly = true})%>
      </div>
      <div>
        <label for="chargeCode" >
          Correspondence Sub Status:
        </label>
        <%: Html.TextBoxFor(corr => corr.CorrespondenceSubStatus, new { @readonly = true, @style="Width:200px;"})%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
      <%Html.RenderPartial("~/Views/Correspondence/PaxAcceptedCommentOnCorrScreen.ascx", Model);%>
    <div>
      <br />
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div id="childAttachmentList" class="">
</div>
