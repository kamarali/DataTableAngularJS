<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.BroadcastMessages.ISMessageRecipients>" %>
<% using (Html.BeginForm("SendMessages", "BroadcastMessages", FormMethod.Post, new { id = "SendMessages" }))
   {%>

    
   <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
  <div class="fieldContainer">
    <div>
      <div>
        <label>
          <span class="required">* </span>Detail:</label> 
        <%:Html.TextAreaFor(message => message.IsMessagesAlerts.Message, new { @maxLength = 1002, @class = "largeTextField", rows = 5, cols = 200 })%>
      </div>
      <div>
        <label>
          <span class="required">* </span>Recipients:</label> 
        <%:Html.MessageSendingMemberDropdownListFor(message => message.MemberCategory, new { @class = "MemberCategory largeTextField" })%>
      </div>
      
      <div>
        <label>All Super Users:</label> 
        <%:Html.RadioButtonFor(message => message.AllSuperUsers, Model != null ? Model.AllSuperUsers : false, new { @id = "rdAllSuperUsers" })%>
      </div>
       <div>
        <label>All Users:</label> 
        <%:Html.RadioButtonFor(message => message.AllUsers, Model != null ? Model.AllUsers : false, new { @id = "rdAllUsers" })%>
      </div>
    </div>
  </div>
  <div class="clear"></div>
</div>
<div class="buttonContainer">
    <input type="submit" value="Send" class="primaryButton" id="Save" />
    <input class="secondaryButton" type="button" onclick="javascript:initializeMessage();" value="Clear" />
  </div>
<%} %>
