<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>


<div>
  <div class="solidBox dataEntry">
     <div class="fieldContainer verticalFlow">
    <div class="oneColumn">
      <div id="fieldDiv">
        </div>
        <div id="TextboxDiv">
          Period:
          <%: Html.TextBox("FuturePeriod") %>
            Value:
          <%: Html.TextBox("FutureValue")%>
        </div>
         <div id="CheckboxDiv">
            Period:
          <%: Html.TextBox("FuturePeriod1") %>
           Value:
          <%: Html.CheckBox("FutureValue1")%>
        </div>          
         <div id="DateDiv">
            Date:
          <%: Html.TextBox("FutureDate") %>
           Value:
          <%: Html.CheckBox("FutureDateFieldValue")%>
        </div>            
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
  <div class="clear">
  </div>
</div>
