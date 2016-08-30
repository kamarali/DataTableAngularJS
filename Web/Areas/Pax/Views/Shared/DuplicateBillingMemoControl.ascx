<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div>
  <div id = "duplicateBMs">
  </div>
  
  <div>
  <br /><b>
    Hence a new BM cannot be raised.</b>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton" type="button" value="OK" onclick = "closeDialog('#divDuplicateBillingMemos')"/>
  </div>
</div>
