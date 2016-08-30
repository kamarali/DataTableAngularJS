<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div>
  <div id ="transactionsRejectedTitle" class="hidden">
  <b>
    Some or all transactions have been already rejected in the below RMs:</b>
  </div>
  <div id = "dupRejections">
  </div>
  <div id= "outsideTimeLimitMessage" class ="hidden">Rejection is outside time limit.</div>
  <div>
  <br />
    Are you sure you want to continue?
  </div>
  <div class="buttonContainer">
    <input class="primaryButton" type="button" value="OK" id="ProceedButton" />
    <input class="secondaryButton" type="button" value="Cancel" onclick = "closeDialog('#divDuplicateRejections')" />
  </div>
</div>
