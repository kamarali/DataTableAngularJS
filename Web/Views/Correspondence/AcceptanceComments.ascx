<!-- CNMP527: This control will be show only in scenario no 4,5,6,7 to enter maindatory comments.  -->
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
 <div class="fieldContainer dataEntry">
     <div>     
      <label for="Description">
           <span>*</span>Acceptance Comments:
      </label>
            <%:Html.TextArea("userAcceptanceComment", "", new { @rows = 15, @cols = 145 })%>      
    </div>
 
 
  <div class="clear">
  </div>
  <div class="buttonContainer">
  <input class="primaryButton ignoredirty" onclick="return validationTextArea();" type="submit" value="Close Correspondence" />
  </div>
  </div>

