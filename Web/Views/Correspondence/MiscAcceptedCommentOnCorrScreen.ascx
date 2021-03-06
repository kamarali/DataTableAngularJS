﻿<!--CMP 527:Add new control two acceptance comments on correspondence screen with exist suject and body text. -->
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscCorrespondence>" %>
<div>
    <div style="width:800px">
      <label for="subject">
        Subject:
      </label>
      <%: Html.TextBoxFor(corr => corr.Subject, new { @readonly = true, @style = "width: 765px;" })%>
     </div>

     <%if (!string.IsNullOrEmpty(Model.AcceptanceComment)){%>
     <div>
      <label for="ourReference">
          Accepted On:&nbsp;<%:Model.AcceptanceDateTime.ToString("dd-MMM-yy, HH:mm")%>
        </label>
        <br/>
      <label style="width:400px;">
          Accepted By:&nbsp;<%:Model.AcceptanceUserName%>
        </label>
      </div>
      <%}%>
      </div>
    <div>
      <br />
    </div>
    <div>
    <div style="width:800px;">
       <label for="correspondanceDetails">
        Correspondence Text:
      </label>
      <%: Html.TextAreaFor(corr => corr.CorrespondenceDetails, new { @readonly = true, @rows = 3, @cols = 150, @class = "textAreaTrimText" })%>
       </div>
       <%if (!string.IsNullOrEmpty(Model.AcceptanceComment)){%>
       <div>
       <label for="yourReference">
          Acceptance Comments:
       </label>
       <%: Html.TextAreaFor(corr => corr.AcceptanceComment, new { @readonly = true, @rows = 3, @cols = 80 })%>
        </div>
         <%}%>
    </div>