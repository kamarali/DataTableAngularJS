<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.UserPermission>" %>

<script type="text/javascript">
    $(document).ready(function () {
        if ('<%=ViewData["UserCategory"]%>' != 1) {
            $('#divMember').hide();
        }
    });
  
</script>

 <h2>
  Search Criteria</h2>
<div class="solidBox" style= "float: left; height: auto; width: auto;">
  <div class="fieldContainer horizontalFlow" style="float: left;">
    <div style="float: left; height: 50px;">
 <%if ((ViewData["UserCategory"]).Equals(1))
    { %>
     <div style="float: left; width: 400px;">
         <div id="divUserCategory"style="float: left; width: 300px;">
          <label id="lblUserCategory">User Category:<span style="color: Red">*</span></label>
          <%= Html.UserCategoryDropdownListFor(model => model.UserCategoryId, "0", new { style = "width:250px;" })%>
          </div>
 <% } %>    
     </div>
               
     <div>
         <div id="divMember" style="float: left; width: 300px;">
          <label id="lblMember">Member:</label>
          <%:Html.TextBoxFor(model => model.MemberName, new { @class = "autocComplete", style = "width:250px;" })%>
          <%:Html.TextBoxFor(model => model.MemberId, new { @class = "hidden" })%>
          </div> 
           
     </div>
    </div> 
    
    <div style="float: left; width: 300px;">
    
     <div style="float: left; height: 50px;">
         <div id="divUserName"style="float: left; width: 300px;">
          <label id="lblUserName">User Name:</label>
          <%= Html.TextBoxFor(model => model.UserName, new { maxlength = 250 , Style="width:250px;" })%>
          
          </div>
      </div>
   </div>  
   </div>
  <div class="clear">
  </div>
</div>
