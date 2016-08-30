<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.BroadcastMessages.ISMessagesAlerts>" %>
<% using (Html.BeginForm("Announcements", "BroadcastMessages", FormMethod.Post, new { id = "Announcements" }))
   {%>
 
   <%: Html.AntiForgeryToken() %>
<div class="solidBox dataEntry">
    <div class="fieldContainer">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Detail:</label>
                <%:Html.TextAreaFor(message => message.Message, new { @maxLength = 1002,@class="largeTextField",rows = 5, cols = 200 })%>
            </div>
     
            <div>
                <table>
                    <tr>
                        <td>
                            <label>
                                <span class="required">* </span>From Date:</label>
                            <%:Html.TextBoxFor(message => message.StartDateTimeValue, new { @class = "datePicker" })%>
                        </td>
                        <td>
                            <label>
                                <span class="required">* </span>From Time:</label>
                            <%:Html.TextBoxFor(m => m.TimeHourMinutes, new { @maxLength = 5,Style="width:40px;"})%>(HH:MM 24 hr format EST)
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <label>
                                    Expiry Date:</label>
                                <%:Html.TextBoxFor(message => message.ExpiryDate, new { @class = "datePicker", id = "messageExpiryDate",@readOnly = true })%>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
      
        </div>
    </div>
    <div class="clear">
    </div>
</div>
<div class="buttonContainer">
    <input type="submit" value="Save" class="primaryButton" id="Save" />
    <input class="secondaryButton" type="button" onclick="javascript:location.href = '<%:Url.Action("BroadcastMessages","BroadcastMessages") %>'"
        value="Clear" />
</div>
<%} %>
