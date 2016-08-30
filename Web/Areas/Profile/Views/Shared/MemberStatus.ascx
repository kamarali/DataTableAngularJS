<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.IchConfiguration>" %>
<div>
  <div class="solidBox dataEntry">
    <div class="fieldContainer verticalFlow">
      <div class="oneColumn">
        <div class="hidden" id="divSuspensionPeriodFrom">
          <label>
            <span class="required">*</span> Suspension Period From:</label>
          <%: Html.SuspensionDropdown(ich => ich.StatusChangedDate)%>
        </div>
        <div class="hidden" id="divDefaultSuspensionPeriodFrom">
          <label>
            <span class="required">*</span> Suspension Defaulting Period From:</label>
          <%: Html.DefaultSuspensionDropdown(ich => ich.DefaultSuspensionDate)%>
        </div>
        <div class="hidden" id="divReinstatement">
          <label>
            Reinstatement Period:</label>
          <%: Html.TextBox("ReinstatementPeriod")%>
        </div>
        <div class="hidden" id="divEntryDate">
          <label>
            Entry Date:</label>
          <%: Html.TextBox("EntryDate")%>
        </div>
        <div class="hidden" id="divTerminationDate">
          <label>
            Termination Date:</label>
          <%: Html.TextBox("TerminationDate")%>
        </div>
        <div class="buttonContainer">
          <input id="hdnIchMemberShipStatusId" type="Hidden" />
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
