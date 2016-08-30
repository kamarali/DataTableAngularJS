<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.AchConfiguration>" %>
<div>
  <div class="solidBox dataEntry">
    <div class="fieldContainer verticalFlow">
      <div class="oneColumn">
        <div class="hidden" id="divAchSuspensionPeriodFrom">
          <label>
            <span class="required">*</span> Suspension Period From:</label>
          <%: Html.SuspensionDropdown(ach => ach.StatusChangedDate, new { @id = "achStatusChangedDate" })%>
        </div>
        <div class="hidden" id="divAchDefaultSuspensionPeriodFrom">
          <label>
            <span class="required">*</span> Suspension Defaulting Period From:</label>
          <%: Html.DefaultSuspensionDropdown(ach => ach.DefaultSuspensionDate, new { @id = "achDefaultSuspensionDate" })%>
        </div>
        <div class="hidden" id="divAchReinstatement">
          <label>
            Reinstatement Period:</label>
          <%: Html.TextBox("AchReinstatementPeriod")%>
        </div>
        <div class="hidden" id="divAchEntryDate">
          <label>
            Entry Date:</label>
          <%: Html.TextBox("AchEntryDate")%>
        </div>
        <div class="hidden" id="divAchTerminationDate">
          <label>
            Termination Date:</label>
          <%: Html.TextBox("AchTerminationDate")%>
        </div>
        <div class="buttonContainer">        
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
