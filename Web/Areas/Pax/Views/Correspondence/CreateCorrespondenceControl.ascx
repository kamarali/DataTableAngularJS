<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Correspondence>" %>
<h2>
  Correspondence Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="billingMember">
          From Member:
        </label>
        <%=Html.TextBox("billingMember", "UA-016", new { @readonly = true })%>
      </div>
      <div>
        <label for="billedMember">
          To Member:
        </label>
        <%= Html.EditorFor(m=> m.ToMemberId) %>
      </div>
      <div>
        <label for="corrDate">
          <%= Html.LabelFor(corr => corr.CorrespondenceDate)%>
        </label>
        <%=Html.TextBox("CorrDate", "17/Aug/2010", new { @readonly = true })%>
      </div>
      <div>
        <label for="corrNo">
          <%= Html.LabelFor(corr => corr.CorrespondenceNumber)%>
        </label>
        <%=Html.TextBox("corrNo", "00016000027", new { @readonly = true })%>
      </div>
      <div>
        <label for="corrStage">
          <%= Html.LabelFor(corr => corr.CorrespondenceStage)%>
        </label>
        <%=Html.TextBox("corrStage", "1", new { @readonly = true})%>
      </div>
    </div>
    <div>
      <div>
        <label for="toEmailId">
          To E-Mail ID(s):
        </label>
        <%=Html.TextArea("toEmailId", "mary.allen@ba.com; michael.pritchard@ba.com", new { @readonly = true, @cols = 150 })%>
      </div>
    </div>
    <div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ourReference">
          <%= Html.LabelFor(corr => corr.OurReference)%>
        </label>
        <%= Html.EditorFor(corr => corr.OurReference) %>
      </div>
      <div>
        <label for="amountToBeSettled">
          <%= Html.LabelFor(corr => corr.AmountToBeSettled)%>
        </label>
        <%= Html.EditorFor(corr => corr.AmountToBeSettled) %>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <label for="subject">
        <%= Html.LabelFor(corr => corr.Subject)%>
      </label>
      <%= Html.EditorFor(corr => corr.Subject) %>
    </div>
    <div>
      <label for="correspondanceDetails">
        <%= Html.LabelFor(corr => corr.CorrespondenceDetails)%>
      </label>
      <%= Html.EditorFor(corr => corr.CorrespondenceDetails) %>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
