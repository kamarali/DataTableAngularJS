<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SIS.Web.UIModels.Account.CreateUserView>" %>
<div class="bottomLine"></div>

<div>
    <div>
        <div>
            <label>
                *Pass Phrase Question 1:</label>
            <%: Html.TextBoxFor(m => m.PassPhraseQuestion1)%>
            <%: Html.ValidationMessageFor(m => m.PassPhraseQuestion1)%>
        </div>
        <div>
            <label>
                *Pass Phrase Answer 1:</label>
            <%: Html.TextBoxFor(m => m.PassPhraseAnswer1)%>
            <%: Html.ValidationMessageFor(m => m.PassPhraseAnswer1)%>
        </div>
    </div>
    <div>
        <div>
            <label>
                *Pass Phrase Question 2:</label>
            <%: Html.TextBoxFor(m => m.PassPhraseQuestion2)%>
            <%: Html.ValidationMessageFor(m => m.PassPhraseQuestion2)%>
        </div>
        <div>
            <label>
                *Pass Phrase Answer 2:</label>
            <%: Html.TextBoxFor(m => m.PassPhraseAnswer2)%>
            <%: Html.ValidationMessageFor(m => m.PassPhraseAnswer2)%>
        </div>
    </div>
    <div>
        <div>
            <label>
                *Pass Phrase Question 3:</label>
            <%: Html.TextBoxFor(m => m.PassPhraseQuestion3)%>
            <%: Html.ValidationMessageFor(m => m.PassPhraseQuestion3)%>
        </div>
        <div>
            <label>
                *Pass Phrase Answer 3:</label>
            <%: Html.TextBoxFor(m => m.PassPhraseAnswer3)%>
            <%: Html.ValidationMessageFor(m => m.PassPhraseAnswer3)%>
        </div>
    </div>
</div>
