<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoCorrespondence>" %>
 <div>
    <div style="width:800px">
      <label for="subject">
        Subject:
      </label>
      <%: Html.TextBoxFor(corr => corr.Subject, new { @readonly = true, @style = "width: 740px;" })%>
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
      <%: Html.TextAreaFor(corr => corr.CorrespondenceDetails, new { @readonly = true, @rows = 15, @cols = 150, @class = "textAreaTrimText" })%>
       </div>
       <%if (!string.IsNullOrEmpty(Model.AcceptanceComment)){%>
       <div>
       <label for="yourReference">
          Acceptance Comments:
       </label>
       <%: Html.TextAreaFor(corr => corr.AcceptanceComment, new { @readonly = true, @rows = 15, @cols = 80 })%>
        </div>
         <%}%>
    </div>