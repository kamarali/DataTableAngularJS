<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.Language>" %>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#Language_Code').focus(); });   </script>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
        <div>
            <label>
             Language Code:</label>
            <%: Html.TextBoxFor(model => model.Language_Code, new { @class = "alphaNumeric lowerCase", @maxLength = 2, @minLength = 2 })%>
          </div>
           <div >
               <label>Language Description:</label>
                 <%: Html.TextBoxFor(model => model.Language_Desc, new { @class = "alphaNumeric", @maxLength = 255, }) %>
            </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
